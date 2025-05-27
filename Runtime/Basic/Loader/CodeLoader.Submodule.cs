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
using SystemAttribute = System.Attribute;
using SystemAttributeUsageAttribute = System.AttributeUsageAttribute;
using SystemAttributeTargets = System.AttributeTargets;
using SystemDelegate = System.Delegate;
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
        /// 代码加载类的子模块初始化回调句柄的声明属性类型定义
        /// </summary>
        [SystemAttributeUsage(SystemAttributeTargets.Method, AllowMultiple = false, Inherited = false)]
        private sealed class OnCodeLoaderSubmoduleInitCallbackAttribute : SystemAttribute
        {
            public OnCodeLoaderSubmoduleInitCallbackAttribute() : base() { }
        }

        /// <summary>
        /// 代码加载类的子模块清理回调句柄的声明属性类型定义
        /// </summary>
        [SystemAttributeUsage(SystemAttributeTargets.Method, AllowMultiple = false, Inherited = false)]
        private sealed class OnCodeLoaderSubmoduleCleanupCallbackAttribute : SystemAttribute
        {
            public OnCodeLoaderSubmoduleCleanupCallbackAttribute() : base() { }
        }

        /// <summary>
        /// 加载对象类的子模块行为流程回调的缓存队列
        /// </summary>
        private static IDictionary<SystemType, SystemDelegate> s_cachedSubmoduleActionCallbacks = null;

        #region 加载对象的子模块调度管理接口函数

        /// <summary>
        /// 加载对象子模块初始化回调处理接口函数
        /// </summary>
        private static void OnCodeLoaderSubmoduleInitCallback()
        {
            s_cachedSubmoduleActionCallbacks = new Dictionary<SystemType, SystemDelegate>();

            OnCodeLoaderSubmoduleActionCallbackOfTargetAttribute(typeof(OnCodeLoaderSubmoduleInitCallbackAttribute));
        }

        /// <summary>
        /// 加载对象子模块清理回调处理接口函数
        /// </summary>
        private static void OnCodeLoaderSubmoduleCleanupCallback()
        {
            OnCodeLoaderSubmoduleActionCallbackOfTargetAttribute(typeof(OnCodeLoaderSubmoduleCleanupCallbackAttribute));

            s_cachedSubmoduleActionCallbacks.Clear();
            s_cachedSubmoduleActionCallbacks = null;
        }

        /// <summary>
        /// 加载对象子模块指定类型的回调函数触发处理接口
        /// </summary>
        /// <param name="attrType">属性类型</param>
        private static void OnCodeLoaderSubmoduleActionCallbackOfTargetAttribute(SystemType attrType)
        {
            SystemDelegate callback;
            if (TryGetCodeLoaderSubmoduleActionCallback(attrType, out callback))
            {
                callback.DynamicInvoke();
            }
        }

        private static bool TryGetCodeLoaderSubmoduleActionCallback(SystemType targetType, out SystemDelegate callback)
        {
            SystemDelegate handler;
            if (s_cachedSubmoduleActionCallbacks.TryGetValue(targetType, out handler))
            {
                callback = handler;
                return null == callback ? false : true;
            }

            IList<SystemDelegate> list = new List<SystemDelegate>();
            SystemType classType = typeof(CodeLoader);
            SystemMethodInfo[] methods = classType.GetMethods(SystemBindingFlags.Public | SystemBindingFlags.NonPublic | SystemBindingFlags.Static);
            for (int n = 0; n < methods.Length; ++n)
            {
                SystemMethodInfo method = methods[n];
                IEnumerable<SystemAttribute> e = method.GetCustomAttributes();
                SystemAttribute attr = method.GetCustomAttribute(targetType);
                if (null != attr)
                {
                    SystemDelegate c = method.CreateDelegate(typeof(NovaEngine.Definition.Delegate.EmptyFunctionHandler));
                    list.Add(c);
                }
            }

            // 先重置回调
            callback = null;

            if (list.Count > 0)
            {
                for (int n = 0; n < list.Count; ++n)
                {
                    if (null == callback)
                    {
                        callback = list[n];
                    }
                    else
                    {
                        callback = SystemDelegate.Combine(list[n], callback);
                    }
                }

                s_cachedSubmoduleActionCallbacks.Add(targetType, callback);
                return true;
            }

            s_cachedSubmoduleActionCallbacks.Add(targetType, callback);
            return false;
        }

        #endregion
    }
}
