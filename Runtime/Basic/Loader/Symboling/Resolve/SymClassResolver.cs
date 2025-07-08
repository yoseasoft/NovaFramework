/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyright (C) 2023 - 2024, Guangzhou Shiyue Network Technology Co., Ltd.
/// Copyright (C) 2025, Hainan Yuanyou Information Tecdhnology Co., Ltd. Guangzhou Branch
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

using System.Collections.Generic;
using System.Reflection;

using SystemType = System.Type;
using SystemAttribute = System.Attribute;
using SystemFieldInfo = System.Reflection.FieldInfo;
using SystemPropertyInfo = System.Reflection.PropertyInfo;
using SystemMethodInfo = System.Reflection.MethodInfo;
using SystemBindingFlags = System.Reflection.BindingFlags;

namespace GameEngine.Loader.Symboling
{
    /// <summary>
    /// 标记对象的解析类，对基础对象类的注入标记进行解析和构建
    /// </summary>
    internal static partial class SymClassResolver
    {
        /// <summary>
        /// 对象类标记数据解析接口函数
        /// </summary>
        /// <param name="targetType">目标对象类型</param>
        /// <param name="reload">重载状态标识</param>
        /// <returns>若对象标记解析成功则返回数据实例，否则返回null</returns>
        public static SymClass ResolveSymClass(SystemType targetType, bool reload)
        {
            SymClass symbol = new SymClass();

            // 2024-07-08:
            // 所有类都进行解析和标记的注册登记
            // 
            // if (false == NovaEngine.Utility.Reflection.IsTypeOfInstantiableClass(targetType))
            // {
            //     // Debugger.Info("The target class type '{0}' must be instantiable type, parsed it failed.", NovaEngine.Utility.Text.ToString(targetType));
            //     return null;
            // }

            // 记录目标对象类型
            symbol.ClassType = targetType;

            IEnumerable<SystemAttribute> classTypeAttrs = targetType.GetCustomAttributes();
            foreach (SystemAttribute attr in classTypeAttrs)
            {
                // 添加类属性实例
                symbol.AddAttribute(attr);

                // 添加特性标识
                symbol.AddFeatureType(attr.GetType());
            }

            SystemType[] interfaceTypes = targetType.GetInterfaces();
            for (int n = 0; null != interfaceTypes && n < interfaceTypes.Length; ++n)
            {
                SystemType interfaceType = interfaceTypes[n];

                // 添加接口标识
                symbol.AddInterfaceType(interfaceType);
            }

            SystemFieldInfo[] fields = targetType.GetFields(SystemBindingFlags.Public | SystemBindingFlags.NonPublic | SystemBindingFlags.Instance); // SystemBindingFlags.Static
            for (int n = 0; null != fields && n < fields.Length; ++n)
            {
                SystemFieldInfo field = fields[n];

                if (false == TryGetSymField(field, out SymField symField))
                {
                    Debugger.Warn("Cannot resolve field '{0}' from target class type '{1}', added it failed.", field.Name, symbol.FullName);
                    continue;
                }

                symbol.AddField(symField);
            }

            SystemPropertyInfo[] properties = targetType.GetProperties(SystemBindingFlags.Public | SystemBindingFlags.NonPublic | SystemBindingFlags.Instance); // SystemBindingFlags.Static
            for (int n = 0; null != properties && n < properties.Length; ++n)
            {
                SystemPropertyInfo property = properties[n];

                if (false == TryGetSymProperty(property, out SymProperty symProperty))
                {
                    Debugger.Warn("Cannot resolve property '{0}' from target class type '{1}', added it failed.", property.Name, symbol.FullName);
                    continue;
                }

                symbol.AddProperty(symProperty);
            }

            SystemMethodInfo[] methods = targetType.GetMethods(SystemBindingFlags.Public | SystemBindingFlags.NonPublic | SystemBindingFlags.Static | SystemBindingFlags.Instance);
            for (int n = 0; null != methods && n < methods.Length; ++n)
            {
                SystemMethodInfo method = methods[n];

                // 忽略掉从“object”继承下来的方法
                // 暂时先这样处理，因为目前暂未发现有任何情况下需要通过反射或特性等原因来驱动“object”中定义的接口
                if (method.DeclaringType == typeof(object))
                {
                    continue;
                }

                if (false == TryGetSymMethod(method, out SymMethod symMethod))
                {
                    Debugger.Warn("Cannot resolve method '{0}' from target class type '{1}', added it failed.", method.Name, symbol.FullName);
                    continue;
                }

                symbol.AddMethod(symMethod);
            }

            // 2024-08-05:
            // 标记对象不设置默认实体对象，仅在解析标记成功后才配置默认实体
            // 2024-08-21:
            // 在解析对象类时，直接将所有定义的实体Bean解析并注册
            // 因为在后面原型对象绑定时，需要对其内部数据进行采集
            IList<Bean> beans = CreateBeanObjectsFromSymClass(symbol);
            for (int n = 0; null != beans && n < beans.Count; ++n)
            {
                Bean classBean = beans[n];
                symbol.AddBean(classBean);
            }

            // 读取配置数据
            IList< Configuring.BeanConfigureInfo> beanConfigureInfos = CodeLoader.GetConfigureBeanByType(targetType);
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
                    Debugger.Warn("Cannot resolve bean object with target configure info '{0}', loaded it failed.", beanConfigureInfo.Name);
                    return null;
                }
            }

            // 个性化定制
            DoPersonalizedCustomizationOfClass(symbol);

            return symbol;
        }

        /// <summary>
        /// 对象字段标记数据解析接口函数
        /// </summary>
        /// <param name="fieldInfo">字段对象实例</param>
        /// <param name="symbol">类型标记结构</param>
        /// <returns>若字段标记解析成功则返回true，否则返回false</returns>
        private static bool TryGetSymField(SystemFieldInfo fieldInfo, out SymField symbol)
        {
            symbol = new SymField();
            symbol.FieldInfo = fieldInfo;

            IEnumerable<SystemAttribute> field_attrs = fieldInfo.GetCustomAttributes();
            foreach (SystemAttribute attr in field_attrs)
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
        private static bool TryGetSymProperty(SystemPropertyInfo propertyInfo, out SymProperty symbol)
        {
            symbol = new SymProperty();
            symbol.PropertyInfo = propertyInfo;

            IEnumerable<SystemAttribute> property_attrs = propertyInfo.GetCustomAttributes();
            foreach (SystemAttribute attr in property_attrs)
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
        private static bool TryGetSymMethod(SystemMethodInfo methodInfo, out SymMethod symbol)
        {
            symbol = new SymMethod();
            symbol.MethodInfo = methodInfo;

            IEnumerable<SystemAttribute> method_attrs = methodInfo.GetCustomAttributes();
            foreach (SystemAttribute attr in method_attrs)
            {
                // 添加属性实例
                symbol.AddAttribute(attr);
            }

            return true;
        }
    }
}
