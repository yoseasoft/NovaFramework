/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyright (C) 2024 - 2025, Hurley, Independent Studio.
/// Copyright (C) 2025 - 2026, Hainan Yuanyou Information Technology Co., Ltd. Guangzhou Branch
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
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace GameEngine.Loader
{
    /// <summary>
    /// 绑定管理对象的回调收集器对象类，用于对绑定管理对象内部的回调句柄统一管理
    /// </summary>
    internal sealed class BindingObjectCallbackCollector
    {
        /// <summary>
        /// 加载实体类相关回调函数的管理容器
        /// </summary>
        private readonly Dictionary<Type, Delegate> _registerClassLoadCallbacks;
        /// <summary>
        /// 清理实体类相关回调函数的管理容器
        /// </summary>
        private readonly Dictionary<Type, Delegate> _registerClassUnloadCallbacks;

        public BindingObjectCallbackCollector()
        {
            _registerClassLoadCallbacks = new Dictionary<Type, Delegate>();
            _registerClassUnloadCallbacks = new Dictionary<Type, Delegate>();
        }

        /// <summary>
        /// 目标绑定对象的初始化函数
        /// </summary>
        /// <param name="targetType">对象类型</param>
        public void OnInitializeContext<TRegisterAttribute, TUnregisterAttribute>(Type targetType)
            where TRegisterAttribute : OnProcessRegisterClassOfTargetAttribute
            where TUnregisterAttribute : OnProcessUnregisterClassOfTargetAttribute
        {
            MethodInfo[] methods = targetType.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
            for (int n = 0; n < methods.Length; ++n)
            {
                MethodInfo method = methods[n];
                IEnumerable<Attribute> e = method.GetCustomAttributes();
                foreach (Attribute attr in e)
                {
                    Type attrType = attr.GetType();
                    if (typeof(TRegisterAttribute) == attrType)
                    {
                        Debugger.Assert(method.IsStatic);

                        TRegisterAttribute _attr = (TRegisterAttribute) attr;

                        Delegate callback = method.CreateDelegate(typeof(CodeLoader.OnCodeTypeLoadedHandler));
                        _registerClassLoadCallbacks.Add(_attr.ClassType, callback);

                        CodeLoader.AddCodeTypeLoadedCallback(_attr.ClassType, callback as CodeLoader.OnCodeTypeLoadedHandler);
                    }
                    else if (typeof(TUnregisterAttribute) == attrType)
                    {
                        Debugger.Assert(method.IsStatic);

                        TUnregisterAttribute _attr = (TUnregisterAttribute) attr;

                        Delegate callback = method.CreateDelegate(typeof(CodeLoader.OnCleanupAllCodeTypesHandler));
                        _registerClassUnloadCallbacks.Add(_attr.ClassType, callback);
                    }
                }
            }
        }

        /// <summary>
        /// 目标绑定对象的清理函数
        /// </summary>
        public void OnCleanupContext()
        {
            foreach (Delegate callback in _registerClassUnloadCallbacks.Values)
            {
                CodeLoader.OnCleanupAllCodeTypesHandler handler = callback as CodeLoader.OnCleanupAllCodeTypesHandler;
                Debugger.Assert(handler, "Invalid cleanup register class unload callback.");

                handler.Invoke();
            }

            _registerClassLoadCallbacks.Clear();
            _registerClassUnloadCallbacks.Clear();
        }
    }
}
