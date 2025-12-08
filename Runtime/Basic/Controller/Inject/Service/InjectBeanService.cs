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

namespace GameEngine
{
    /// <summary>
    /// 提供注入操作接口的服务类，对整个程序内部的对象实例提供注入操作的服务逻辑处理
    /// </summary>
    public static partial class InjectBeanService
    {
        /// <summary>
        /// 服务初始化调度接口函数的属性定义
        /// </summary>
        [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
        private class OnServiceProcessInitCallbackAttribute : Attribute
        {
            public OnServiceProcessInitCallbackAttribute() : base() { }
        }

        /// <summary>
        /// 服务清理调度接口函数的属性定义
        /// </summary>
        [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
        private class OnServiceProcessCleanupCallbackAttribute : Attribute
        {
            public OnServiceProcessCleanupCallbackAttribute() : base() { }
        }

        /// <summary>
        /// 服务初始化处理回调的函数容器
        /// </summary>
        private static IList<Delegate> _serviceProcessInitCallbacks = null;
        /// <summary>
        /// 服务清理处理回调的函数容器
        /// </summary>
        private static IList<Delegate> _serviceProcessCleanupCallbacks = null;

        /// <summary>
        /// 初始化注入服务处理类声明的全部回调接口
        /// </summary>
        internal static void InitAllServiceProcessingCallbacks()
        {
            _serviceProcessInitCallbacks = new List<Delegate>();
            _serviceProcessCleanupCallbacks = new List<Delegate>();

            Type classType = typeof(InjectBeanService);
            MethodInfo[] methods = classType.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            for (int n = 0; n < methods.Length; ++n)
            {
                MethodInfo method = methods[n];
                IEnumerable<Attribute> e = method.GetCustomAttributes();
                foreach (Attribute attr in e)
                {
                    Type attrType = attr.GetType();
                    if (typeof(OnServiceProcessInitCallbackAttribute) == attrType)
                    {
                        Delegate callback = method.CreateDelegate(typeof(NovaEngine.Definition.Delegate.EmptyFunctionHandler));

                        _serviceProcessInitCallbacks.Add(callback);
                    }
                    else if (typeof(OnServiceProcessCleanupCallbackAttribute) == attrType)
                    {
                        Delegate callback = method.CreateDelegate(typeof(NovaEngine.Definition.Delegate.EmptyFunctionHandler));

                        _serviceProcessCleanupCallbacks.Add(callback);
                    }
                }
            }

            for (int n = 0; n < _serviceProcessInitCallbacks.Count; ++n)
            {
                _serviceProcessInitCallbacks[n].DynamicInvoke();
            }
        }

        /// <summary>
        /// 清理注入服务处理类声明的全部回调接口
        /// </summary>
        internal static void CleanupAllServiceProcessingCallbacks()
        {
            for (int n = 0; n < _serviceProcessCleanupCallbacks.Count; ++n)
            {
                _serviceProcessCleanupCallbacks[n].DynamicInvoke();
            }

            _serviceProcessInitCallbacks.Clear();
            _serviceProcessInitCallbacks = null;

            _serviceProcessCleanupCallbacks.Clear();
            _serviceProcessCleanupCallbacks = null;
        }
    }
}
