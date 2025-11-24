/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
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
using System.Reflection;

using SystemType = System.Type;
using SystemAttribute = System.Attribute;
using SystemAttributeUsageAttribute = System.AttributeUsageAttribute;
using SystemAttributeTargets = System.AttributeTargets;
using SystemEnum = System.Enum;
using SystemDelegate = System.Delegate;

namespace GameEngine.Loader
{
    /// <summary>
    /// 程序集的分析处理类，对业务层载入的所有对象类进行统一加载及分析处理
    /// </summary>
    public static partial class CodeLoader
    {
        /// <summary>
        /// 初始化通用类的函数句柄定义
        /// </summary>
        public delegate void OnInitAllGeneralCodeLoaderHandler();
        /// <summary>
        /// 清理通用类的函数句柄定义
        /// </summary>
        public delegate void OnCleanupAllGeneralCodeLoaderHandler();
        /// <summary>
        /// 匹配通用类的函数句柄定义
        /// </summary>
        /// <param name="symClass">对象标记类型</param>
        /// <param name="filterType">过滤对象类型</param>
        /// <returns>若匹配通用类成功则返回true，否则返回false</returns>
        public delegate bool OnGeneralCodeLoaderMatchHandler(Symboling.SymClass symClass, SystemType filterType);
        /// <summary>
        /// 加载通用类的函数句柄定义
        /// </summary>
        /// <param name="symClass">对象标记类型</param>
        /// <param name="reload">重载状态标识</param>
        /// <returns>若加载通用类成功则返回true，否则返回false</returns>
        public delegate bool OnGeneralCodeLoaderLoadHandler(Symboling.SymClass symClass, bool reload);
        /// <summary>
        /// 查找通用类对应的结构信息的函数句柄定义
        /// </summary>
        /// <param name="symClass">对象标记类型</param>
        /// <returns>返回给定类型对应的结构信息，若查找失败则返回null</returns>
        public delegate Structuring.GeneralCodeInfo OnGeneralCodeLoaderLookupHandler(Symboling.SymClass symClass);

        /// <summary>
        /// 通用类初始化函数的属性定义
        /// </summary>
        [SystemAttributeUsage(SystemAttributeTargets.Method, AllowMultiple = false, Inherited = false)]
        internal class OnGeneralCodeLoaderInitAttribute : SystemAttribute
        {
            public OnGeneralCodeLoaderInitAttribute() { }
        }

        /// <summary>
        /// 通用类清理函数的属性定义
        /// </summary>
        [SystemAttributeUsage(SystemAttributeTargets.Method, AllowMultiple = false, Inherited = false)]
        internal class OnGeneralCodeLoaderCleanupAttribute : SystemAttribute
        {
            public OnGeneralCodeLoaderCleanupAttribute() { }
        }

        /// <summary>
        /// 通用类匹配函数的属性定义
        /// </summary>
        [SystemAttributeUsage(SystemAttributeTargets.Method, AllowMultiple = false, Inherited = false)]
        internal class OnGeneralCodeLoaderMatchAttribute : SystemAttribute
        {
            public OnGeneralCodeLoaderMatchAttribute() { }
        }

        /// <summary>
        /// 通用类加载函数的属性定义
        /// </summary>
        [SystemAttributeUsage(SystemAttributeTargets.Method, AllowMultiple = false, Inherited = false)]
        internal class OnGeneralCodeLoaderLoadAttribute : SystemAttribute
        {
            public OnGeneralCodeLoaderLoadAttribute() { }
        }

        /// <summary>
        /// 通用类结构信息查找函数的属性定义
        /// </summary>
        [SystemAttributeUsage(SystemAttributeTargets.Method, AllowMultiple = false, Inherited = false)]
        internal class OnGeneralCodeLoaderLookupAttribute : SystemAttribute
        {
            public OnGeneralCodeLoaderLookupAttribute() { }
        }

        /// <summary>
        /// 通用类加载器类的反射管理容器对象
        /// </summary>
        private class GeneralLoaderClassReflectionHolder
        {
            /// <summary>
            /// 第三方加载器的初始化回调函数
            /// </summary>
            private OnInitAllGeneralCodeLoaderHandler _initCallback;
            /// <summary>
            /// 第三方加载器的清理回调函数
            /// </summary>
            private OnCleanupAllGeneralCodeLoaderHandler _cleanupCallback;
            /// <summary>
            /// 第三方加载器的匹配回调函数
            /// </summary>
            private OnGeneralCodeLoaderMatchHandler _matchCallback;
            /// <summary>
            /// 第三方加载器的加载回调函数
            /// </summary>
            private OnGeneralCodeLoaderLoadHandler _loadCallback;
            /// <summary>
            /// 第三方加载器的查找回调函数
            /// </summary>
            private OnGeneralCodeLoaderLookupHandler _lookupCallback;

            public OnInitAllGeneralCodeLoaderHandler InitCallback => _initCallback;
            public OnCleanupAllGeneralCodeLoaderHandler CleanupCallback => _cleanupCallback;
            public OnGeneralCodeLoaderMatchHandler MatchCallback => _matchCallback;
            public OnGeneralCodeLoaderLoadHandler LoadCallback => _loadCallback;
            public OnGeneralCodeLoaderLookupHandler LookupCallback => _lookupCallback;

            public GeneralLoaderClassReflectionHolder(OnInitAllGeneralCodeLoaderHandler initCallback,
                                                      OnCleanupAllGeneralCodeLoaderHandler cleanupCallback,
                                                      OnGeneralCodeLoaderMatchHandler matchCallback,
                                                      OnGeneralCodeLoaderLoadHandler loadCallback,
                                                      OnGeneralCodeLoaderLookupHandler lookupCallback)
            {
                _initCallback = initCallback;
                _cleanupCallback = cleanupCallback;
                _matchCallback = matchCallback;
                _loadCallback = loadCallback;
                _lookupCallback = lookupCallback;
            }
        }

        /// <summary>
        /// 加载器类的后缀名称常量定义
        /// </summary>
        private const string CodeLoaderClassUnifiedStandardName = "CodeLoader";

        /// <summary>
        /// 通用类加载器类的反射管理容器列表
        /// </summary>
        private static IList<GeneralLoaderClassReflectionHolder> _generalLoaderList = new List<GeneralLoaderClassReflectionHolder>();

        /// <summary>
        /// 初始化针对所有标准核心类声明的全部绑定回调接口
        /// </summary>
        [OnClassSubmoduleInitializeCallback]
        private static void InitAllGeneralClassLoadingCallbacks()
        {
            string namespaceTag = typeof(CodeLoader).Namespace;

            foreach (string enumName in SystemEnum.GetNames(typeof(CodeClassifyType)))
            {
                if (enumName.Equals(CodeClassifyType.Unknown.ToString()))
                {
                    // 未知类型直接忽略
                    continue;
                }

                // 类名反射时需要包含命名空间前缀
                string loaderName = NovaEngine.Utility.Text.Format("{0}.{1}{2}", namespaceTag, enumName, CodeLoaderClassUnifiedStandardName);

                SystemType loaderType = SystemType.GetType(loaderName);
                if (null == loaderType)
                {
                    Debugger.Info("Could not found any code loader class with target name {%s}.", loaderName);
                    continue;
                }

                if (false == loaderType.IsAbstract || false == loaderType.IsSealed)
                {
                    Debugger.Warn("The code loader type {%s} must be static class.", loaderName);
                    continue;
                }

                // Debugger.Info("Register new code loader {%s} with target type {%s}.", loaderName, enumName);

                AddGeneralTypeImplementedClass(loaderType);
            }

            foreach (GeneralLoaderClassReflectionHolder v in _generalLoaderList)
            {
                v.InitCallback();
            }
        }

        /// <summary>
        /// 清理针对所有标准核心类声明的全部绑定回调接口
        /// </summary>
        [OnClassSubmoduleCleanupCallback]
        private static void CleanupAllGeneralClassLoadingCallbacks()
        {
            foreach (GeneralLoaderClassReflectionHolder v in _generalLoaderList)
            {
                v.CleanupCallback();
            }

            _generalLoaderList.Clear();
        }

        /// <summary>
        /// 加载通用类库指定的类型
        /// </summary>
        /// <param name="targetType">对象类型</param>
        /// <param name="reload">重载状态标识</param>
        /// <returns>若存在给定类型属性通用类库则返回对应处理结果，否则返回false</returns>
        private static bool LoadGeneralClass(SystemType targetType, bool reload)
        {
            Symboling.SymClass symClass = GetSymClassByType(targetType);

            return LoadGeneralClass(symClass, reload);
        }

        /// <summary>
        /// 加载通用类库指定的类型
        /// </summary>
        /// <param name="symClass">标记类型</param>
        /// <param name="reload">重载状态标识</param>
        /// <returns>若存在给定类型属性通用类库则返回对应处理结果，否则返回false</returns>
        private static bool LoadGeneralClass(Symboling.SymClass symClass, bool reload)
        {
            bool result = false;
            for (int n = 0; n < _generalLoaderList.Count; n++)
            {
                GeneralLoaderClassReflectionHolder holder = _generalLoaderList[n];
                if (holder.MatchCallback(symClass, null))
                {
                    if (holder.LoadCallback(symClass, reload))
                    {
                        result = true;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// 查找通用类库指定的类型对应的结构信息
        /// </summary>
        /// <param name="targetType">目标对象类型</param>
        /// <param name="filterType">过滤对象类型</param>
        /// <returns>返回类型对应的结构信息</returns>
        public static Structuring.GeneralCodeInfo LookupGeneralCodeInfo(SystemType targetType, SystemType filterType = null)
        {
            Symboling.SymClass symClass = GetSymClassByType(targetType);

            return LookupGeneralCodeInfo(symClass, filterType);
        }

        /// <summary>
        /// 查找通用类库指定的类型对应的结构信息
        /// </summary>
        /// <param name="symClass">对象标记类型</param>
        /// <param name="filterType">过滤对象类型</param>
        /// <returns>返回类型对应的结构信息</returns>
        public static Structuring.GeneralCodeInfo LookupGeneralCodeInfo(Symboling.SymClass symClass, SystemType filterType = null)
        {
            for (int n = 0; n < _generalLoaderList.Count; ++n)
            {
                GeneralLoaderClassReflectionHolder holder = _generalLoaderList[n];
                if (holder.MatchCallback(symClass, filterType))
                {
                    return holder.LookupCallback(symClass);
                }
            }

            return null;
        }

        /// <summary>
        /// 通用类加载器的具体实现类注册接口
        /// </summary>
        /// <param name="targetType">对象类型</param>
        private static void AddGeneralTypeImplementedClass(SystemType targetType)
        {
            OnInitAllGeneralCodeLoaderHandler initCallback = null;
            OnCleanupAllGeneralCodeLoaderHandler cleanupCallback = null;
            OnGeneralCodeLoaderMatchHandler matchCallback = null;
            OnGeneralCodeLoaderLoadHandler loadCallback = null;
            OnGeneralCodeLoaderLookupHandler lookupCallback = null;

            MethodInfo[] methods = targetType.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            for (int n = 0; n < methods.Length; ++n)
            {
                MethodInfo method = methods[n];
                IEnumerable<SystemAttribute> e = method.GetCustomAttributes();
                foreach (SystemAttribute attr in e)
                {
                    SystemType attrType = attr.GetType();
                    if (typeof(OnGeneralCodeLoaderInitAttribute).IsAssignableFrom(attrType))
                    {
                        initCallback = method.CreateDelegate(typeof(OnInitAllGeneralCodeLoaderHandler)) as OnInitAllGeneralCodeLoaderHandler;
                    }
                    else if (typeof(OnGeneralCodeLoaderCleanupAttribute).IsAssignableFrom(attrType))
                    {
                        cleanupCallback = method.CreateDelegate(typeof(OnCleanupAllGeneralCodeLoaderHandler)) as OnCleanupAllGeneralCodeLoaderHandler;
                    }
                    else if (typeof(OnGeneralCodeLoaderMatchAttribute).IsAssignableFrom(attrType))
                    {
                        matchCallback = method.CreateDelegate(typeof(OnGeneralCodeLoaderMatchHandler)) as OnGeneralCodeLoaderMatchHandler;
                    }
                    else if (typeof(OnGeneralCodeLoaderLoadAttribute).IsAssignableFrom(attrType))
                    {
                        loadCallback = method.CreateDelegate(typeof(OnGeneralCodeLoaderLoadHandler)) as OnGeneralCodeLoaderLoadHandler;
                    }
                    else if (typeof(OnGeneralCodeLoaderLookupAttribute).IsAssignableFrom(attrType))
                    {
                        lookupCallback = method.CreateDelegate(typeof(OnGeneralCodeLoaderLookupHandler)) as OnGeneralCodeLoaderLookupHandler;
                    }
                }
            }

            // 所有回调接口必须全部实现，该加载器才能正常使用
            if (null == initCallback ||
                null == cleanupCallback ||
                null == matchCallback ||
                null == loadCallback ||
                null == lookupCallback)
            {
                Debugger.Warn("Could not found all callbacks from the incompleted class type '{%t}', added it to loader list failed.", targetType);
                return;
            }

            GeneralLoaderClassReflectionHolder holder = new GeneralLoaderClassReflectionHolder(initCallback, cleanupCallback, matchCallback, loadCallback, lookupCallback);
            _generalLoaderList.Add(holder);

            // Debugger.Log(LogGroupTag.CodeLoader, "Add general implemented class '{%t}' to loader list.", targetType);
        }
    }
}
