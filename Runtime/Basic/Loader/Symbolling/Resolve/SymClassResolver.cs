/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyright (C) 2023 - 2024, Guangzhou Shiyue Network Technology Co., Ltd.
/// Copyright (C) 2025, Hainan Yuanyou Information Technology Co., Ltd. Guangzhou Branch
///
/// Permission is hereby granted, free of charge, to any person obtaining a copy
/// of this software and associated documentation files (the "Software"), to deal
/// in the Software without restriction, including without limitation the rights
/// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
/// copies of the Software, and to permit persons to whom the Software is
/// furnished to do so, subject to the following conditions:
///
/// The above copyright notice and this permission notice shall be included in
/// all copies or substantial portions of the Software.
///
/// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
/// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
/// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
/// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
/// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
/// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
/// THE SOFTWARE.
/// -------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Customize.Extension;
using System.Reflection;

namespace GameEngine.Loader.Symbolling
{
    /// <summary>
    /// 标记对象的解析类，对基础对象类的注入标记进行解析和构建
    /// </summary>
    internal static partial class SymClassResolver
    {
        /// <summary>
        /// 解析流程的初始化函数
        /// </summary>
        public static void OnInitialize()
        {
            // 自定义对象解析业务初始化
            OnCustomizeInitialize();
        }

        /// <summary>
        /// 解析流程的清理函数
        /// </summary>
        public static void OnCleanup()
        {
            // 自定义对象解析业务清理
            OnCustomizeCleanup();
        }

        /// <summary>
        /// 对象类标记数据解析接口函数
        /// </summary>
        /// <param name="targetType">目标对象类型</param>
        /// <param name="reload">重载状态标识</param>
        /// <returns>若对象标记解析成功则返回数据实例，否则返回null</returns>
        public static SymClass ResolveSymClass(Type targetType, bool reload)
        {
            SymClass symbol = new SymClass();

            // 2024-07-08:
            // 所有类都进行解析和标记的注册登记，包括抽象基类
            // 
            // if (false == NovaEngine.Utility.Reflection.IsTypeOfInstantiableClass(targetType))
            // {
            //     // Debugger.Info(LogGroupTag.CodeLoader, "The target class type '{%t}' must be instantiable type, parsed it failed.", targetType);
            //     return null;
            // }

            // 记录目标对象类型
            symbol.ClassType = targetType;

            IEnumerable<Attribute> classTypeAttrs = targetType.GetCustomAttributes();
            foreach (Attribute attr in classTypeAttrs)
            {
                // 添加类属性实例
                symbol.AddAttribute(attr);

                // 添加特性标识
                symbol.AddFeatureType(attr.GetType());
            }

            Type[] interfaceTypes = targetType.GetInterfaces();
            for (int n = 0; null != interfaceTypes && n < interfaceTypes.Length; ++n)
            {
                Type interfaceType = interfaceTypes[n];

                // 添加接口标识
                symbol.AddInterfaceType(interfaceType);
            }

            FieldInfo[] fields = targetType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance); // BindingFlags.Static
            for (int n = 0; null != fields && n < fields.Length; ++n)
            {
                FieldInfo field = fields[n];

                if (false == CodeLoader.SymbolResolvingOfCompleteClassStructure)
                {
                    // 仅保留业务部分的字段类型
                    if (EngineDefine.IsCoreScopeClassType(field.DeclaringType))
                        continue;
                }

                if (false == TryGetSymField(field, out SymField symField))
                {
                    Debugger.Warn(LogGroupTag.CodeLoader, "Cannot resolve field '{%s}' from target class type '{%s}', added it failed.", field.Name, symbol.FullName);
                    continue;
                }

                symbol.AddField(symField);
            }

            PropertyInfo[] properties = targetType.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance); // BindingFlags.Static
            for (int n = 0; null != properties && n < properties.Length; ++n)
            {
                PropertyInfo property = properties[n];

                if (false == CodeLoader.SymbolResolvingOfCompleteClassStructure)
                {
                    // 仅保留业务部分的属性类型
                    if (EngineDefine.IsCoreScopeClassType(property.DeclaringType))
                        continue;
                }

                if (false == TryGetSymProperty(property, out SymProperty symProperty))
                {
                    Debugger.Warn(LogGroupTag.CodeLoader, "Cannot resolve property '{%s}' from target class type '{%s}', added it failed.", property.Name, symbol.FullName);
                    continue;
                }

                symbol.AddProperty(symProperty);
            }

            MethodInfo[] methods = targetType.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
            for (int n = 0; null != methods && n < methods.Length; ++n)
            {
                MethodInfo method = methods[n];

                if (false == CodeLoader.SymbolResolvingOfCompleteClassStructure)
                {
                    // 仅保留业务部分的函数类型
                    if (EngineDefine.IsCoreScopeClassType(method.DeclaringType))
                        continue;
                }

                // 忽略掉从“object”继承下来的方法
                // 暂时先这样处理，因为目前暂未发现有任何情况下需要通过反射或特性等原因来驱动“object”中定义的接口
                if (method.DeclaringType == typeof(object))
                {
                    continue;
                }

                // 忽略掉所有泛型函数
                if (method.IsGenericMethod)
                {
                    continue;
                }

                if (false == TryGetSymMethod(method, out SymMethod symMethod))
                {
                    Debugger.Warn(LogGroupTag.CodeLoader, "Cannot resolve method '{%s}' from target class type '{%s}', added it failed.", method.Name, symbol.FullName);
                    continue;
                }

                symbol.AddMethod(symMethod);
            }

            // 个性化定制
            DoPersonalizedCustomizationOfClass(symbol);

            // 2025-12-10：
            // 如果符号类不是'IBean'的子类，则不进行Bean的解析和装配
            if (false == symbol.ClassType.Is<IBean>())
            {
                return symbol;
            }

            ///////////////////////////////////////////////////////////////////////////////////////////////////
            // 符号类解析完成，接下来进行Bean的解析和装配
            ///////////////////////////////////////////////////////////////////////////////////////////////////

            // 2024-08-05:
            // 标记对象不设置默认实体对象，仅在解析标记成功后才配置默认实体
            // 2024-08-21:
            // 在解析对象类时，直接将所有定义的实体Bean解析并注册
            // 因为在后面原型对象绑定时，需要对其内部数据进行采集
            // 2025-08-07：
            // 在代码中仅支持定义默认Bean实例，自定义Bean组合均在配置文件中定义
            Bean defaultBeanInstance = CreateDefaultBeanObjectWithSymClass(symbol);
            if (null != defaultBeanInstance)
            {
                symbol.AddBean(defaultBeanInstance);
            }

            // 从配置文件创建实体对象
            CreateBeanObjectsWithConfigureFile(symbol);

            return symbol;
        }

        /// <summary>
        /// 从配置文件数据创建Bean对象实例
        /// </summary>
        /// <param name="symbol">类型标记结构</param>
        private static void CreateBeanObjectsWithConfigureFile(SymClass symbol)
        {
            Type targetType = symbol.ClassType;

            // 读取配置数据
            IReadOnlyList< Configuring.BeanConfigureInfo> beanConfigureInfos = CodeLoader.GetBeanConfigureByType(targetType);
            for (int n = 0; null != beanConfigureInfos && n < beanConfigureInfos.Count; ++n)
            {
                Configuring.BeanConfigureInfo beanConfigureInfo = beanConfigureInfos[n];
                Bean classBean = CreateBeanObjectFromConfigureInfo(symbol, beanConfigureInfo);
                if (null != classBean)
                {
                    symbol.AddBean(classBean);
                }
                else
                {
                    Debugger.Error(LogGroupTag.CodeLoader, "不能正确解析名字为‘{%s}’的Bean文件配置项！", beanConfigureInfo.Name);
                    return;
                }
            }
        }

        /// <summary>
        /// 重载符号类的Bean相关配置内容
        /// </summary>
        /// <param name="symbol">类型标记结构</param>
        public static void RebuildBeanObjectsWithConfigureFile(SymClass symbol)
        {
            // 移除旧的配置Bean实例
            symbol.RemoveAllConfigureBeans();

            // 重新加载新的配置数据
            CreateBeanObjectsWithConfigureFile(symbol);
        }

        /// <summary>
        /// 对象字段标记数据解析接口函数
        /// </summary>
        /// <param name="fieldInfo">字段对象实例</param>
        /// <param name="symbol">类型标记结构</param>
        /// <returns>若字段标记解析成功则返回true，否则返回false</returns>
        private static bool TryGetSymField(FieldInfo fieldInfo, out SymField symbol)
        {
            symbol = new SymField();
            symbol.FieldInfo = fieldInfo;

            IEnumerable<Attribute> field_attrs = fieldInfo.GetCustomAttributes();
            foreach (Attribute attr in field_attrs)
            {
                // 添加属性实例
                symbol.AddAttribute(attr);
            }

            return true;
        }

        /// <summary>
        /// 对象属性标记数据解析接口函数
        /// </summary>
        /// <param name="propertyInfo">属性对象实例</param>
        /// <param name="symbol">类型标记结构</param>
        /// <returns>若属性标记解析成功则返回true，否则返回false</returns>
        private static bool TryGetSymProperty(PropertyInfo propertyInfo, out SymProperty symbol)
        {
            symbol = new SymProperty();
            symbol.PropertyInfo = propertyInfo;

            IEnumerable<Attribute> property_attrs = propertyInfo.GetCustomAttributes();
            foreach (Attribute attr in property_attrs)
            {
                // 添加属性实例
                symbol.AddAttribute(attr);
            }

            return true;
        }

        /// <summary>
        /// 对象函数标记数据解析接口函数
        /// </summary>
        /// <param name="methodInfo">函数对象实例</param>
        /// <param name="symbol">类型标记结构</param>
        /// <returns>若函数标记解析成功则返回true，否则返回false</returns>
        private static bool TryGetSymMethod(MethodInfo methodInfo, out SymMethod symbol)
        {
            symbol = new SymMethod();
            symbol.MethodInfo = methodInfo;

            IEnumerable<Attribute> method_attrs = methodInfo.GetCustomAttributes();
            foreach (Attribute attr in method_attrs)
            {
                // 添加属性实例
                symbol.AddAttribute(attr);
            }

            return true;
        }
    }
}
