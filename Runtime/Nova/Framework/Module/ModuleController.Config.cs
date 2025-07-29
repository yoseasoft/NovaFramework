/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
/// Copyright (C) 2020 - 2022, Guangzhou Xinyuan Technology Co., Ltd.
/// Copyright (C) 2022 - 2023, Shanghai Bilibili Technology Co., Ltd.
/// Copyright (C) 2023 - 2024, Guangzhou Shiyue Network Technology Co., Ltd.
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

using SystemType = System.Type;

namespace NovaEngine
{
    /// <summary>
    /// 引擎框架模块中控台管理类
    /// </summary>
    public static partial class ModuleController
    {
        /// <summary>
        /// 模块对象参数配置数据对象类定义
        /// </summary>
        public static class Config
        {
            /// <summary>
            /// 模块对象配置信息数据结构定义
            /// </summary>
            private class ModuleConfigureInfo
            {
                string _name;                // 模块名称
                int _type;                   // 模块类型
                int _priority;               // 模块优先级
                SystemType _reflectionType;  // 模块映射类型

                /// <summary>
                /// 模块名称属性访问Getter/Setter接口
                /// </summary>
                public string Name { get { return _name; } set { _name = value; } }

                /// <summary>
                /// 模块类型属性访问Getter/Setter接口
                /// </summary>
                public int Type { get { return _type; } set { _type = value; } }

                /// <summary>
                /// 模块优先级属性访问Getter/Setter接口
                /// </summary>
                public int Priority { get { return _priority; } set { _priority = value; } }

                /// <summary>
                /// 模块映射类型属性访问Getter/Setter接口
                /// </summary>
                public SystemType ReflectionType { get { return _reflectionType; } set { _reflectionType = value; } }
            }

            /// <summary>
            /// 模块类的后缀名称常量定义
            /// </summary>
            private const string MODULE_CLASS_SUFFIX_NAME = "Module";

            /// <summary>
            /// 模块对象配置信息
            /// </summary>
            private static IList<ModuleConfigureInfo> _configureInfos;

            /// <summary>
            /// 对当前引擎框架中所有模块对象的配置参数进行初始化
            /// 需要注意的是，默认所有模块对象均不加载，您需要自行配置需要使用到的模块
            /// </summary>
            public static void InitModuleConfigure()
            {
                if (ModuleController._isRunning)
                {
                    throw new CFrameworkException("Cannot do reset on running state.");
                }

                // 配置容器初始化
                _configureInfos = new List<ModuleConfigureInfo>();

                // 获取当前命名空间
                string namespace_tag = typeof(ModuleController).Namespace;

                foreach (ModuleObject.ModuleEventType enumValue in System.Enum.GetValues(typeof(ModuleObject.ModuleEventType)))
                {
                    if (ModuleObject.ModuleEventType.Default == enumValue || ModuleObject.ModuleEventType.User == enumValue)
                    {
                        // 忽略默认值和用户自定义值
                        continue;
                    }

                    string enumName = enumValue.ToString();
                    // 类名反射时需要包含命名空间前缀
                    string moduleName = Utility.Text.Format("{0}.{1}{2}", namespace_tag, enumName, MODULE_CLASS_SUFFIX_NAME);

                    SystemType moduleType = SystemType.GetType(moduleName);
                    if (null == moduleType)
                    {
                        Logger.Info("Could not found any module class with target name {0}.", moduleName);
                        continue;
                    }

                    if (false == typeof(ModuleObject).IsAssignableFrom(moduleType))
                    {
                        Logger.Warn("The module type {0} must be inherited from 'ModuleObject' class.", moduleName);
                        continue;
                    }

                    // Logger.Info("Register new module class {0} to target type {1}.", moduleName, enumName);

                    RegModuleConfigureInfo(enumValue, moduleType);
                }
            }

            /// <summary>
            /// 检测给定的类型是否为错误的模块对象类型
            /// </summary>
            /// <param name="type">模块对象类型</param>
            /// <returns>若给定的模块类型是错误的则返回true，否则返回false</returns>
            public static bool IsUnresolvedModuleType(int type)
            {
                IEnumerator<ModuleConfigureInfo> e = _configureInfos.GetEnumerator();
                while (e.MoveNext())
                {
                    if (e.Current.Type == type)
                        return false;
                }

                return true;
            }

            /// <summary>
            /// 注册模块对象的配置信息
            /// 对象数据结构请参考<see cref="NovaEngine.ModuleController.Config.ModuleConfigureInfo"/>
            /// </summary>
            /// <param name="moduleType">模块对象类型</param>
            /// <param name="clsType">模块映射类型</param>
            private static void RegModuleConfigureInfo(ModuleObject.ModuleEventType moduleType, SystemType clsType)
            {
                // 若打开启用标识，则必须提供映射类型参数
                Logger.Assert(null != clsType, "Invalid arguments.");

                ModuleConfigureInfo found = GetModuleConfigureInfoByType(moduleType);
                if (null != found)
                {
                    throw new CFrameworkException("Cannot repeat registation module configure.");
                }
                
                ModuleConfigureInfo mbi = new ModuleConfigureInfo();
                mbi.Name = moduleType.ToString().ToUpper();
                mbi.Type = (int) moduleType;
                mbi.Priority = ModuleObject.GetModulePriorityWithEventType(moduleType);
                mbi.ReflectionType = clsType;

                _configureInfos.Add(mbi);
            }

            /// <summary>
            /// 通过模块对象类型在当前注册的配置信息中查找对应的数据项
            /// </summary>
            /// <param name="moduleType">模块对象类型</param>
            /// <returns>返回指定类型的模块配置信息，若查找失败返回null</returns>
            private static ModuleConfigureInfo GetModuleConfigureInfoByType(ModuleObject.ModuleEventType moduleType)
            {
                return GetModuleConfigureInfoByType((int) moduleType);
            }

            /// <summary>
            /// 通过模块对象类型在当前注册的配置信息中查找对应的数据项
            /// </summary>
            /// <param name="type">模块对象类型</param>
            /// <returns>返回指定类型的模块配置信息，若查找失败返回null</returns>
            private static ModuleConfigureInfo GetModuleConfigureInfoByType(int type)
            {
                IEnumerator<ModuleConfigureInfo> e = _configureInfos.GetEnumerator();
                while (e.MoveNext())
                {
                    if (e.Current.Type == type)
                        return e.Current;
                }

                return null;
            }

            /// <summary>
            /// 通过模块对象类型获取该模块的名称
            /// </summary>
            /// <param name="type">模块对象类型</param>
            /// <returns>返回给定类型模块的名称</returns>
            public static string GetModuleName(int type)
            {
                ModuleConfigureInfo found = GetModuleConfigureInfoByType(type);
                if (null != found)
                {
                    return found.Name;
                }

                return null;
            }

            /// <summary>
            /// 通过模块映射类型获取该模块的名称
            /// </summary>
            /// <param name="clsType">模块映射类型</param>
            /// <returns>返回给定映射类型模块的名称</returns>
            public static string GetModuleName(SystemType clsType)
            {
                IEnumerator<ModuleConfigureInfo> e = _configureInfos.GetEnumerator();
                while (e.MoveNext())
                {
                    if (clsType.IsAssignableFrom(e.Current.ReflectionType))
                        return e.Current.Name;
                }

                return null;
            }

            /// <summary>
            /// 通过模块名称获取该模块的对象类型
            /// </summary>
            /// <param name="name">模块名称</param>
            /// <returns>返回给定名称模块的对象类型</returns>
            public static int GetModuleType(string name)
            {
                IEnumerator<ModuleConfigureInfo> e = _configureInfos.GetEnumerator();
                while (e.MoveNext())
                {
                    if (e.Current.Name == name)
                        return e.Current.Type;
                }

                return 0;
            }

            /// <summary>
            /// 通过模块映射类型获取该模块的对象类型
            /// </summary>
            /// <param name="clsType">模块映射类型</param>
            /// <returns>返回给定映射类型模块的对象类型</returns>
            public static int GetModuleType(SystemType clsType)
            {
                IEnumerator<ModuleConfigureInfo> e = _configureInfos.GetEnumerator();
                while (e.MoveNext())
                {
                    if (clsType.IsAssignableFrom(e.Current.ReflectionType))
                        return e.Current.Type;
                }

                return 0;
            }

            /// <summary>
            /// 通过模块对象类型获取该模块的优先级
            /// </summary>
            /// <param name="type">模块对象类型</param>
            /// <returns>返回给定类型模块的优先级</returns>
            public static int GetModulePriority(int type)
            {
                ModuleConfigureInfo found = GetModuleConfigureInfoByType(type);
                if (null != found)
                {
                    return found.Priority;
                }

                return 0;
            }

            /// <summary>
            /// 通过模块对象类型获取该模块的映射类型
            /// </summary>
            /// <param name="type">模块对象类型</param>
            /// <returns>返回给定类型模块的映射类型</returns>
            public static SystemType GetModuleReflectionType(int type)
            {
                ModuleConfigureInfo found = GetModuleConfigureInfoByType(type);
                if (null != found)
                {
                    return found.ReflectionType;
                }

                return null;
            }

            /// <summary>
            /// 获取当前已注册的全部模块对象类型列表
            /// </summary>
            /// <returns>返回模块对象类型列表</returns>
            public static IList<int> GetAllRegModuleTypes()
            {
                IList<int> moduleTypes = new List<int>(_configureInfos.Count);
                IEnumerator<ModuleConfigureInfo> e = _configureInfos.GetEnumerator();
                while (e.MoveNext())
                {
                    moduleTypes.Add(e.Current.Type);
                }

                return moduleTypes;
            }
        }
    }
}
