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
using SystemAttributeTargets = System.AttributeTargets;
using SystemMethodInfo = System.Reflection.MethodInfo;
using SystemBindingFlags = System.Reflection.BindingFlags;

namespace GameEngine.Loader
{
    /// <summary>
    /// 注入控制的绑定回调管理服务类，对注入模块的回调接口绑定/注销操作进行统一定义管理
    /// </summary>
    internal static class InjectBindingProcessor
    {
        /// <summary>
        /// 加载注入控制类相关回调函数的管理容器
        /// </summary>
        private static IDictionary<SystemType, SystemDelegate> _injectRegisterClassLoadCallbacks = new Dictionary<SystemType, SystemDelegate>();
        /// <summary>
        /// 清理注入控制类相关回调函数的管理容器
        /// </summary>
        private static IDictionary<SystemType, SystemDelegate> _injectRegisterClassUnloadCallbacks = new Dictionary<SystemType, SystemDelegate>();

        /// <summary>
        /// 初始化针对绑定类声明的全部回调接口
        /// </summary>
        [CodeLoader.OnBindingProcessorInit]
        private static void InitAllCodeBindingCallbacks()
        {
            SystemType classType = typeof(InjectController);
            SystemMethodInfo[] methods = classType.GetMethods(SystemBindingFlags.Public | SystemBindingFlags.NonPublic | SystemBindingFlags.Static | SystemBindingFlags.Instance);
            for (int n = 0; n < methods.Length; ++n)
            {
                SystemMethodInfo method = methods[n];
                IEnumerable<SystemAttribute> e = method.GetCustomAttributes();
                foreach (SystemAttribute attr in e)
                {
                    SystemType attrType = attr.GetType();
                    if (typeof(OnInjectOfControlRegisterClassOfTargetAttribute) == attrType)
                    {
                        Debugger.Assert(method.IsStatic);

                        OnInjectOfControlRegisterClassOfTargetAttribute _attr = (OnInjectOfControlRegisterClassOfTargetAttribute) attr;
                        _injectRegisterClassLoadCallbacks.Add(_attr.ClassType, method.CreateDelegate(typeof(CodeLoader.OnCodeTypeLoadedHandler)));

                        CodeLoader.AddCodeTypeLoadedCallback(_attr.ClassType, method.CreateDelegate(typeof(CodeLoader.OnCodeTypeLoadedHandler)) as CodeLoader.OnCodeTypeLoadedHandler);
                    }
                    else if (typeof(OnInjectOfControlUnregisterClassOfTargetAttribute) == attrType)
                    {
                        Debugger.Assert(method.IsStatic);

                        OnInjectOfControlUnregisterClassOfTargetAttribute _attr = (OnInjectOfControlUnregisterClassOfTargetAttribute) attr;
                        _injectRegisterClassUnloadCallbacks.Add(_attr.ClassType, method.CreateDelegate(typeof(CodeLoader.OnCleanupAllCodeTypesHandler)));
                    }
                }
            }
        }

        /// <summary>
        /// 清理针对绑定类声明的全部回调接口
        /// </summary>
        [CodeLoader.OnBindingProcessorCleanup]
        private static void CleanupAllCodeBindingCallbacks()
        {
            IEnumerator<KeyValuePair<SystemType, SystemDelegate>> e = _injectRegisterClassUnloadCallbacks.GetEnumerator();
            while (e.MoveNext())
            {
                CodeLoader.OnCleanupAllCodeTypesHandler handler = e.Current.Value as CodeLoader.OnCleanupAllCodeTypesHandler;
                Debugger.Assert(null != handler, "Invalid cleanup inject register class unload callback.");

                handler.Invoke();
            }

            _injectRegisterClassLoadCallbacks.Clear();
            _injectRegisterClassUnloadCallbacks.Clear();
        }
    }
}
