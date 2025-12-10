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

namespace GameEngine.Loader
{
    /// 程序集的分析处理类
    public static partial class CodeLoader
    {
        /// <summary>
        /// 加载对象类的子模块行为流程回调的缓存队列
        /// </summary>
        private static IDictionary<Type, Delegate> _cachedSubmoduleActionCallbacks = null;

        #region 加载对象的子模块调度管理接口函数

        /// <summary>
        /// 加载对象子模块初始化回调处理接口函数
        /// </summary>
        private static void OnCodeLoaderSubmoduleInitCallback()
        {
            _cachedSubmoduleActionCallbacks = new Dictionary<Type, Delegate>();

            OnCodeLoaderSubmoduleActionCallbackOfTargetAttribute(typeof(OnClassSubmoduleInitializeCallbackAttribute));
        }

        /// <summary>
        /// 加载对象子模块清理回调处理接口函数
        /// </summary>
        private static void OnCodeLoaderSubmoduleCleanupCallback()
        {
            OnCodeLoaderSubmoduleActionCallbackOfTargetAttribute(typeof(OnClassSubmoduleCleanupCallbackAttribute));

            _cachedSubmoduleActionCallbacks.Clear();
            _cachedSubmoduleActionCallbacks = null;
        }

        /// <summary>
        /// 加载对象子模块指定类型的回调函数触发处理接口
        /// </summary>
        /// <param name="attrType">属性类型</param>
        private static void OnCodeLoaderSubmoduleActionCallbackOfTargetAttribute(Type attrType)
        {
            if (TryGetCodeLoaderSubmoduleActionCallback(attrType, out Delegate callback))
            {
                callback.DynamicInvoke();
            }
        }

        private static bool TryGetCodeLoaderSubmoduleActionCallback(Type targetType, out Delegate callback)
        {
            if (_cachedSubmoduleActionCallbacks.TryGetValue(targetType, out Delegate handler))
            {
                callback = handler;
                return null != callback;
            }

            callback = Utils.CreateSubmoduleBehaviourCallback(typeof(CodeLoader), targetType);

            _cachedSubmoduleActionCallbacks.Add(targetType, callback);

            return null != callback;
        }

        #endregion
    }
}
