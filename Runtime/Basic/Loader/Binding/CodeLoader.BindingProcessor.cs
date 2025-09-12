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
using SystemDelegate = System.Delegate;
using SystemAttribute = System.Attribute;
using SystemAttributeUsageAttribute = System.AttributeUsageAttribute;
using SystemAttributeTargets = System.AttributeTargets;
using SystemEnum = System.Enum;
using SystemMethodInfo = System.Reflection.MethodInfo;
using SystemBindingFlags = System.Reflection.BindingFlags;

namespace GameEngine.Loader
{
    /// <summary>
    /// 程序集的分析处理类，对业务层载入的所有对象类进行统一加载及分析处理
    /// </summary>
    public static partial class CodeLoader
    {
        /// <summary>
        /// 初始化绑定处理服务类的函数句柄定义
        /// </summary>
        public delegate void OnInitAllBindingProcessorClassesHandler();
        /// <summary>
        /// 清理绑定处理服务类的函数句柄定义
        /// </summary>
        public delegate void OnCleanupAllBindingProcessorClassesHandler();

        /// <summary>
        /// 绑定处理器类初始化函数的属性定义
        /// </summary>
        [SystemAttributeUsage(SystemAttributeTargets.Method, AllowMultiple = false, Inherited = false)]
        internal class OnBindingProcessorInitAttribute : SystemAttribute
        {
            public OnBindingProcessorInitAttribute() { }
        }

        /// <summary>
        /// 绑定处理器类清理函数的属性定义
        /// </summary>
        [SystemAttributeUsage(SystemAttributeTargets.Method, AllowMultiple = false, Inherited = false)]
        internal class OnBindingProcessorCleanupAttribute : SystemAttribute
        {
            public OnBindingProcessorCleanupAttribute() { }
        }

        /// <summary>
        /// 绑定处理服务类的后缀名称常量定义
        /// </summary>
        private const string BindingProcessorClassUnifiedStandardName = "BindingProcessor";

        /// <summary>
        /// 初始化绑定处理服务类相关回调函数的管理容器
        /// </summary>
        private static IDictionary<SystemType, SystemDelegate> _codeBindingProcessorInitCallbacks = new Dictionary<SystemType, SystemDelegate>();
        /// <summary>
        /// 清理绑定处理服务类相关回调函数的管理容器
        /// </summary>
        private static IDictionary<SystemType, SystemDelegate> _codeBindingProcessorCleanupCallbacks = new Dictionary<SystemType, SystemDelegate>();

        /// <summary>
        /// 初始化针对所有绑定处理类声明的全部绑定回调接口
        /// </summary>
        [OnCodeLoaderSubmoduleInitCallback]
        private static void InitAllBingdingProcessorClassLoadingCallbacks()
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
                string processorName = NovaEngine.Utility.Text.Format("{0}.{1}{2}", namespaceTag, enumName, BindingProcessorClassUnifiedStandardName);

                SystemType processorType = SystemType.GetType(processorName);
                if (null == processorType)
                {
                    Debugger.Info("Could not found any code binding processor class with target name {0}.", processorName);
                    continue;
                }

                if (false == processorType.IsAbstract || false == processorType.IsSealed)
                {
                    Debugger.Warn("The code binding processor type {0} must be static class.", processorName);
                    continue;
                }

                // Debugger.Info("Register new code binding processor {0} with target type {1}.", processorName, enumName);

                AddBindingProcessorTypeImplementedClass(processorType);
            }

            IEnumerator<KeyValuePair<SystemType, SystemDelegate>> e = _codeBindingProcessorInitCallbacks.GetEnumerator();
            while (e.MoveNext())
            {
                OnInitAllBindingProcessorClassesHandler handler = e.Current.Value as OnInitAllBindingProcessorClassesHandler;
                Debugger.Assert(null != handler, "Invalid code bingding processor class init handler.");

                handler.Invoke();
            }
        }

        /// <summary>
        /// 清理针对所有绑定处理类声明的全部绑定回调接口
        /// </summary>
        [OnCodeLoaderSubmoduleCleanupCallback]
        private static void CleanupAllBindingProcessorClassLoadingCallbacks()
        {
            IEnumerator<KeyValuePair<SystemType, SystemDelegate>> e = _codeBindingProcessorCleanupCallbacks.GetEnumerator();
            while (e.MoveNext())
            {
                OnCleanupAllBindingProcessorClassesHandler handler = e.Current.Value as OnCleanupAllBindingProcessorClassesHandler;
                Debugger.Assert(null != handler, "Invalid code bingding processor class cleanup handler.");

                handler.Invoke();
            }

            _codeBindingProcessorInitCallbacks.Clear();
            _codeBindingProcessorCleanupCallbacks.Clear();
        }

        /// <summary>
        /// 绑定处理服务类加载器的具体实现类注册接口
        /// </summary>
        /// <param name="targetType">对象类型</param>
        private static void AddBindingProcessorTypeImplementedClass(SystemType targetType)
        {
            OnInitAllBindingProcessorClassesHandler initCallback = null;
            OnCleanupAllBindingProcessorClassesHandler cleanupCallback = null;

            SystemMethodInfo[] methods = targetType.GetMethods(SystemBindingFlags.Public | SystemBindingFlags.NonPublic | SystemBindingFlags.Static);
            for (int n = 0; n < methods.Length; ++n)
            {
                SystemMethodInfo method = methods[n];
                IEnumerable<SystemAttribute> e = method.GetCustomAttributes();
                foreach (SystemAttribute attr in e)
                {
                    SystemType attrType = attr.GetType();
                    if (typeof(OnBindingProcessorInitAttribute).IsAssignableFrom(attrType))
                    {
                        initCallback = method.CreateDelegate(typeof(OnInitAllBindingProcessorClassesHandler)) as OnInitAllBindingProcessorClassesHandler;
                    }
                    else if (typeof(OnBindingProcessorCleanupAttribute).IsAssignableFrom(attrType))
                    {
                        cleanupCallback = method.CreateDelegate(typeof(OnCleanupAllBindingProcessorClassesHandler)) as OnCleanupAllBindingProcessorClassesHandler;
                    }
                }
            }

            // 所有回调接口必须全部实现，该加载器才能正常使用
            if (null == initCallback || null == cleanupCallback)
            {
                Debugger.Warn("Could not found all callbacks from the incompleted class type '{0}', added it to loader list failed.", targetType.FullName);
                return;
            }

            _codeBindingProcessorInitCallbacks.Add(targetType, initCallback);
            _codeBindingProcessorCleanupCallbacks.Add(targetType, cleanupCallback);

            Debugger.Log(LogGroupTag.CodeLoader, "Add binding processor implemented class '{0}' to loader list.", targetType.FullName);
        }

        /// <summary>
        /// 通过指定的处理句柄，获取其属性标识的目标解析对象类型
        /// </summary>
        /// <param name="handler">句柄实例</param>
        /// <returns>返回给定句柄对应的目标解析对象类型，若不存在则返回null</returns>
        private static SystemType GetProcessRegisterClassTypeByHandler(OnCodeTypeLoadedHandler handler)
        {
            OnProcessRegisterClassOfTargetAttribute attr = handler.Method.GetCustomAttribute<OnProcessRegisterClassOfTargetAttribute>();
            if (null != attr)
            {
                return attr.ClassType;
            }

            return null;
        }
    }
}
