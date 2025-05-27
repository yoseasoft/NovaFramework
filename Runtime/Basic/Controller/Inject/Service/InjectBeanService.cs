/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyring (C) 2023 - 2024, Guangzhou Shiyue Network Technology Co., Ltd.
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
using SystemBindingFlags = System.Reflection.BindingFlags;
using SystemMethodInfo = System.Reflection.MethodInfo;

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
        [SystemAttributeUsage(SystemAttributeTargets.Method, AllowMultiple = false, Inherited = false)]
        private class OnServiceProcessInitCallbackAttribute : SystemAttribute
        {
            public OnServiceProcessInitCallbackAttribute() : base() { }
        }

        /// <summary>
        /// 服务清理调度接口函数的属性定义
        /// </summary>
        [SystemAttributeUsage(SystemAttributeTargets.Method, AllowMultiple = false, Inherited = false)]
        private class OnServiceProcessCleanupCallbackAttribute : SystemAttribute
        {
            public OnServiceProcessCleanupCallbackAttribute() : base() { }
        }

        /// <summary>
        /// 服务初始化处理回调的函数容器
        /// </summary>
        private static IList<SystemDelegate> s_serviceProcessInitCallbacks = null;
        /// <summary>
        /// 服务清理处理回调的函数容器
        /// </summary>
        private static IList<SystemDelegate> s_serviceProcessCleanupCallbacks = null;

        /// <summary>
        /// 初始化注入服务处理类声明的全部回调接口
        /// </summary>
        internal static void InitAllServiceProcessingCallbacks()
        {
            s_serviceProcessInitCallbacks = new List<SystemDelegate>();
            s_serviceProcessCleanupCallbacks = new List<SystemDelegate>();

            SystemType classType = typeof(InjectBeanService);
            SystemMethodInfo[] methods = classType.GetMethods(SystemBindingFlags.Public | SystemBindingFlags.NonPublic | SystemBindingFlags.Static);
            for (int n = 0; n < methods.Length; ++n)
            {
                SystemMethodInfo method = methods[n];
                IEnumerable<SystemAttribute> e = method.GetCustomAttributes();
                foreach (SystemAttribute attr in e)
                {
                    SystemType attrType = attr.GetType();
                    if (typeof(OnServiceProcessInitCallbackAttribute) == attrType)
                    {
                        SystemDelegate callback = method.CreateDelegate(typeof(NovaEngine.Definition.Delegate.EmptyFunctionHandler));

                        s_serviceProcessInitCallbacks.Add(callback);
                    }
                    else if (typeof(OnServiceProcessCleanupCallbackAttribute) == attrType)
                    {
                        SystemDelegate callback = method.CreateDelegate(typeof(NovaEngine.Definition.Delegate.EmptyFunctionHandler));

                        s_serviceProcessCleanupCallbacks.Add(callback);
                    }
                }
            }

            for (int n = 0; n < s_serviceProcessInitCallbacks.Count; ++n)
            {
                s_serviceProcessInitCallbacks[n].DynamicInvoke();
            }
        }

        /// <summary>
        /// 清理注入服务处理类声明的全部回调接口
        /// </summary>
        internal static void CleanupAllServiceProcessingCallbacks()
        {
            for (int n = 0; n < s_serviceProcessCleanupCallbacks.Count; ++n)
            {
                s_serviceProcessCleanupCallbacks[n].DynamicInvoke();
            }

            s_serviceProcessInitCallbacks.Clear();
            s_serviceProcessInitCallbacks = null;

            s_serviceProcessCleanupCallbacks.Clear();
            s_serviceProcessCleanupCallbacks = null;
        }
    }
}
