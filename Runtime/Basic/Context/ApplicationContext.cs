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

namespace GameEngine
{
    /// <summary>
    /// 应用程序的上下文管理器对象类，对应用通用接口进行封装，对外提供访问函数
    /// </summary>
    public static partial class ApplicationContext
    {
        /// <summary>
        /// 应用的子模块行为流程回调的缓存队列
        /// </summary>
        private static IDictionary<Type, Delegate> _cachedSubmoduleActionCallbacks = null;

        /// <summary>
        /// 应用程序上下文的启动函数
        /// </summary>
        internal static void Startup()
        {
            // 子模块初始化
            OnApplicationSubmoduleInitCallback();
        }

        /// <summary>
        /// 应用程序上下文的关闭函数
        /// </summary>
        internal static void Shutdown()
        {
            // 子模块清理
            OnApplicationSubmoduleCleanupCallback();
        }

        /// <summary>
        /// 应用程序上下文的重载函数
        /// </summary>
        internal static void Restart()
        {
        }

        #region 加载对象的子模块调度管理接口函数

        /// <summary>
        /// 应用子模块初始化回调处理接口函数
        /// </summary>
        private static void OnApplicationSubmoduleInitCallback()
        {
            _cachedSubmoduleActionCallbacks = new Dictionary<Type, Delegate>();

            OnApplicationSubmoduleActionCallbackOfTargetAttribute(typeof(OnClassSubmoduleInitializeCallbackAttribute));
        }

        /// <summary>
        /// 应用子模块清理回调处理接口函数
        /// </summary>
        private static void OnApplicationSubmoduleCleanupCallback()
        {
            OnApplicationSubmoduleActionCallbackOfTargetAttribute(typeof(OnClassSubmoduleCleanupCallbackAttribute));

            _cachedSubmoduleActionCallbacks.Clear();
            _cachedSubmoduleActionCallbacks = null;
        }

        /// <summary>
        /// 应用子模块指定类型的回调函数触发处理接口
        /// </summary>
        /// <param name="attrType">属性类型</param>
        private static void OnApplicationSubmoduleActionCallbackOfTargetAttribute(Type attrType)
        {
            if (TryGetApplicationSubmoduleActionCallback(attrType, out Delegate callback))
            {
                callback.DynamicInvoke();
            }
        }

        private static bool TryGetApplicationSubmoduleActionCallback(Type targetType, out Delegate callback)
        {
            if (_cachedSubmoduleActionCallbacks.TryGetValue(targetType, out Delegate handler))
            {
                callback = handler;
                return null != callback;
            }

            callback = Utils.CreateSubmoduleBehaviourCallback(typeof(ApplicationContext), targetType);

            _cachedSubmoduleActionCallbacks.Add(targetType, callback);

            return null != callback;
        }

        #endregion
    }
}
