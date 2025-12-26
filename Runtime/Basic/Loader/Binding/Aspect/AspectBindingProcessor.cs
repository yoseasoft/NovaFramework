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
using UnityEngine.Scripting;

namespace GameEngine.Loader
{
    /// <summary>
    /// 切面控制的绑定回调管理服务类，对切面模块的回调接口绑定/注销操作进行统一定义管理
    /// </summary>
    internal static class AspectBindingProcessor
    {
        /// <summary>
        /// 加载切面控制类相关回调函数的管理容器
        /// </summary>
        private readonly static IDictionary<Type, Delegate> _registerClassLoadCallbacks = new Dictionary<Type, Delegate>();
        /// <summary>
        /// 清理切面控制类相关回调函数的管理容器
        /// </summary>
        private readonly static IDictionary<Type, Delegate> _registerClassUnloadCallbacks = new Dictionary<Type, Delegate>();

        /// <summary>
        /// 初始化针对绑定类声明的全部回调接口
        /// </summary>
        [Preserve]
        [CodeLoader.OnBindingProcessorInit]
        private static void InitAllCodeBindingCallbacks()
        {
            Type classType = typeof(AspectController);
            MethodInfo[] methods = classType.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
            for (int n = 0; n < methods.Length; ++n)
            {
                MethodInfo method = methods[n];
                IEnumerable<Attribute> e = method.GetCustomAttributes();
                foreach (Attribute attr in e)
                {
                    Type attrType = attr.GetType();
                    if (typeof(OnAspectCallRegisterClassOfTargetAttribute) == attrType)
                    {
                        Debugger.Assert(method.IsStatic);

                        OnAspectCallRegisterClassOfTargetAttribute _attr = (OnAspectCallRegisterClassOfTargetAttribute) attr;

                        Delegate callback = method.CreateDelegate(typeof(CodeLoader.OnCodeTypeLoadedHandler));
                        _registerClassLoadCallbacks.Add(_attr.ClassType, callback);

                        CodeLoader.AddCodeTypeLoadedCallback(_attr.ClassType, callback as CodeLoader.OnCodeTypeLoadedHandler);
                    }
                    else if (typeof(OnAspectCallUnregisterClassOfTargetAttribute) == attrType)
                    {
                        Debugger.Assert(method.IsStatic);

                        OnAspectCallUnregisterClassOfTargetAttribute _attr = (OnAspectCallUnregisterClassOfTargetAttribute) attr;

                        Delegate callback = method.CreateDelegate(typeof(CodeLoader.OnCleanupAllCodeTypesHandler));
                        _registerClassUnloadCallbacks.Add(_attr.ClassType, callback);
                    }
                }
            }
        }

        /// <summary>
        /// 清理针对绑定类声明的全部回调接口
        /// </summary>
        [Preserve]
        [CodeLoader.OnBindingProcessorCleanup]
        private static void CleanupAllCodeBindingCallbacks()
        {
            IEnumerator<KeyValuePair<Type, Delegate>> e = _registerClassUnloadCallbacks.GetEnumerator();
            while (e.MoveNext())
            {
                CodeLoader.OnCleanupAllCodeTypesHandler handler = e.Current.Value as CodeLoader.OnCleanupAllCodeTypesHandler;
                Debugger.Assert(null != handler, "Invalid cleanup aspect register class unload callback.");

                handler.Invoke();
            }

            _registerClassLoadCallbacks.Clear();
            _registerClassUnloadCallbacks.Clear();
        }
    }
}
