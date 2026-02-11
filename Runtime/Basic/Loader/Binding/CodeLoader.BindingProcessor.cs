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

using System;
using System.Collections.Generic;
using System.Reflection;

namespace GameEngine.Loader
{
    /// 程序集的分析处理类
    public static partial class CodeLoader
    {
        /// <summary>
        /// 初始化绑定处理服务类的函数句柄定义
        /// </summary>
        private delegate void OnInitAllBindingProcessorClassesHandler();
        /// <summary>
        /// 清理绑定处理服务类的函数句柄定义
        /// </summary>
        private delegate void OnCleanupAllBindingProcessorClassesHandler();

        /// <summary>
        /// 绑定处理器类初始化函数的属性定义
        /// </summary>
        [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
        internal sealed class OnBindingProcessorInitAttribute : Attribute
        {
            public OnBindingProcessorInitAttribute() { }
        }

        /// <summary>
        /// 绑定处理器类清理函数的属性定义
        /// </summary>
        [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
        internal sealed class OnBindingProcessorCleanupAttribute : Attribute
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
        private static readonly IDictionary<Type, Delegate> _codeBindingProcessorInitCallbacks = new Dictionary<Type, Delegate>();
        /// <summary>
        /// 清理绑定处理服务类相关回调函数的管理容器
        /// </summary>
        private static readonly IDictionary<Type, Delegate> _codeBindingProcessorCleanupCallbacks = new Dictionary<Type, Delegate>();

        /// <summary>
        /// 初始化针对所有绑定处理类声明的全部绑定回调接口
        /// </summary>
        [OnClassSubmoduleInitializeCallback]
        private static void InitAllBindingProcessorClassLoadingCallbacks()
        {
            string namespaceTag = typeof(CodeLoader).Namespace;

            foreach (string enumName in Enum.GetNames(typeof(CodeClassifyType)))
            {
                if (enumName.Equals(CodeClassifyType.Unknown.ToString()))
                {
                    // 未知类型直接忽略
                    continue;
                }

                // 类名反射时需要包含命名空间前缀
                string processorName = NovaEngine.FormatString.Format("{%s}.{%s}{%s}", namespaceTag, enumName, BindingProcessorClassUnifiedStandardName);

                Type processorType = Type.GetType(processorName);
                if (null == processorType)
                {
                    Debugger.Info(LogGroupTag.CodeLoader, "Could not found any code binding processor class with target name {%s}.", processorName);
                    continue;
                }

                if (false == processorType.IsAbstract || false == processorType.IsSealed)
                {
                    Debugger.Warn(LogGroupTag.CodeLoader, "The code binding processor type {%s} must be static class.", processorName);
                    continue;
                }

                // Debugger.Info(LogGroupTag.CodeLoader, "Register new code binding processor {%s} with target type {%s}.", processorName, enumName);

                AddBindingProcessorTypeImplementedClass(processorType);
            }

            IEnumerator<KeyValuePair<Type, Delegate>> e = _codeBindingProcessorInitCallbacks.GetEnumerator();
            while (e.MoveNext())
            {
                OnInitAllBindingProcessorClassesHandler handler = e.Current.Value as OnInitAllBindingProcessorClassesHandler;
                Debugger.Assert(handler, "Invalid code binding processor class init handler.");

                handler.Invoke();
            }
        }

        /// <summary>
        /// 清理针对所有绑定处理类声明的全部绑定回调接口
        /// </summary>
        [OnClassSubmoduleCleanupCallback]
        private static void CleanupAllBindingProcessorClassLoadingCallbacks()
        {
            IEnumerator<KeyValuePair<Type, Delegate>> e = _codeBindingProcessorCleanupCallbacks.GetEnumerator();
            while (e.MoveNext())
            {
                OnCleanupAllBindingProcessorClassesHandler handler = e.Current.Value as OnCleanupAllBindingProcessorClassesHandler;
                Debugger.Assert(handler, "Invalid code binding processor class cleanup handler.");

                handler.Invoke();
            }

            _codeBindingProcessorInitCallbacks.Clear();
            _codeBindingProcessorCleanupCallbacks.Clear();
        }

        /// <summary>
        /// 绑定处理服务类加载器的具体实现类注册接口
        /// </summary>
        /// <param name="targetType">对象类型</param>
        private static void AddBindingProcessorTypeImplementedClass(Type targetType)
        {
            OnInitAllBindingProcessorClassesHandler initCallback = null;
            OnCleanupAllBindingProcessorClassesHandler cleanupCallback = null;

            MethodInfo[] methods = targetType.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            for (int n = 0; n < methods.Length; ++n)
            {
                MethodInfo method = methods[n];
                IEnumerable<Attribute> e = method.GetCustomAttributes();
                foreach (Attribute attr in e)
                {
                    Type attrType = attr.GetType();
                    if (typeof(OnBindingProcessorInitAttribute) == attrType)
                    {
                        initCallback = method.CreateDelegate(typeof(OnInitAllBindingProcessorClassesHandler)) as OnInitAllBindingProcessorClassesHandler;
                    }
                    else if (typeof(OnBindingProcessorCleanupAttribute) == attrType)
                    {
                        cleanupCallback = method.CreateDelegate(typeof(OnCleanupAllBindingProcessorClassesHandler)) as OnCleanupAllBindingProcessorClassesHandler;
                    }
                }
            }

            // 所有回调接口必须全部实现，该加载器才能正常使用
            if (null == initCallback || null == cleanupCallback)
            {
                Debugger.Warn(LogGroupTag.CodeLoader, "Could not found all callbacks from the incompleted class type '{%t}', added it to loader list failed.", targetType);
                return;
            }

            _codeBindingProcessorInitCallbacks.Add(targetType, initCallback);
            _codeBindingProcessorCleanupCallbacks.Add(targetType, cleanupCallback);

            // Debugger.Log(LogGroupTag.CodeLoader, "Add binding processor implemented class '{%t}' to loader list.", targetType);
        }

        /// <summary>
        /// 通过指定的处理句柄，获取其属性标识的目标解析对象类型
        /// </summary>
        /// <param name="handler">句柄实例</param>
        /// <returns>返回给定句柄对应的目标解析对象类型，若不存在则返回null</returns>
        private static Type GetProcessRegisterClassTypeByHandler(OnCodeTypeLoadedHandler handler)
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
