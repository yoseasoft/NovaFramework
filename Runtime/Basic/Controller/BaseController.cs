/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyright (C) 2023 - 2024, Guangzhou Shiyue Network Technology Co., Ltd.
/// Copyright (C) 2025, Hainan Yuanyou Information Technology Co., Ltd. Guangzhou Branch
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
using System.Runtime.CompilerServices;

namespace GameEngine
{
    /// <summary>
    /// 控制器对象的抽象基类，对所有的控制器类型进行标准接口函数及流程的封装
    /// </summary>
    public abstract class BaseController<T> : NovaEngine.Singleton<T>, IController where T : class, new()
    {
        /// <summary>
        /// 控制器子模块行为流程回调的缓存队列
        /// </summary>
        private IDictionary<Type, Delegate> _cachedSubmoduleBehaviourCallbacks = null;

        /// <summary>
        /// 控制器对象初始化通知接口函数
        /// </summary>
        protected override sealed void Initialize()
        {
            // 初始化子模块行为流程缓存队列
            _cachedSubmoduleBehaviourCallbacks = new Dictionary<Type, Delegate>();

            OnInitialize();

            // 子模块初始化回调
            OnSubmoduleInitCallback();
        }

        /// <summary>
        /// 控制器对象清理通知接口函数
        /// </summary>
        protected override sealed void Cleanup()
        {
            // 子模块清理回调
            OnSubmoduleCleanupCallback();

            OnCleanup();

            // 清理子模块行为流程缓存队列
            _cachedSubmoduleBehaviourCallbacks.Clear();
            _cachedSubmoduleBehaviourCallbacks = null;
        }

        /// <summary>
        /// 控制器对象初始化回调函数
        /// </summary>
        protected abstract void OnInitialize();

        /// <summary>
        /// 控制器对象清理回调函数
        /// </summary>
        protected abstract void OnCleanup();

        /// <summary>
        /// 控制器对象刷新调度函数接口
        /// </summary>
        public void Update()
        {
            OnUpdate();

            // 子模块刷新回调
            OnSubmoduleUpdateCallback();
        }

        /// <summary>
        /// 控制器对象后置刷新调度函数接口
        /// </summary>
        public void LateUpdate()
        {
            OnLateUpdate();

            // 子模块后置刷新回调
            OnSubmoduleLateUpdateCallback();
        }

        /// <summary>
        /// 控制器对象重载调度函数接口
        /// </summary>
        public void Reload()
        {
            OnReload();

            // 子模块重载回调
            OnSubmoduleReloadCallback();
        }

        /// <summary>
        /// 控制器对象倾泻调度函数接口
        /// </summary>
        public void Dump()
        {
            OnDump();

            // 子模块倾泻回调
            OnSubmoduleDumpCallback();
        }

        /// <summary>
        /// 控制器对象刷新回调函数
        /// </summary>
        protected abstract void OnUpdate();

        /// <summary>
        /// 控制器对象后置刷新回调函数
        /// </summary>
        protected abstract void OnLateUpdate();

        /// <summary>
        /// 控制器对象重载回调函数
        /// </summary>
        protected abstract void OnReload();

        /// <summary>
        /// 控制器对象倾泻回调函数
        /// </summary>
        protected abstract void OnDump();

        /// <summary>
        /// 控制器对象子模块初始化回调处理接口函数
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void OnSubmoduleInitCallback()
        {
            OnSubmoduleActionCallbackOfTargetAttribute(typeof(OnControllerSubmoduleInitCallbackAttribute));
        }

        /// <summary>
        /// 控制器对象子模块清理回调处理接口函数
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void OnSubmoduleCleanupCallback()
        {
            OnSubmoduleActionCallbackOfTargetAttribute(typeof(OnControllerSubmoduleCleanupCallbackAttribute));
        }

        /// <summary>
        /// 控制器对象子模块刷新回调处理接口函数
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void OnSubmoduleUpdateCallback()
        {
            OnSubmoduleActionCallbackOfTargetAttribute(typeof(OnControllerSubmoduleUpdateCallbackAttribute));
        }

        /// <summary>
        /// 控制器对象子模块后置刷新回调处理接口函数
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void OnSubmoduleLateUpdateCallback()
        {
            OnSubmoduleActionCallbackOfTargetAttribute(typeof(OnControllerSubmoduleLateUpdateCallbackAttribute));
        }

        /// <summary>
        /// 控制器对象子模块重载回调处理接口函数
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void OnSubmoduleReloadCallback()
        {
            OnSubmoduleActionCallbackOfTargetAttribute(typeof(OnControllerSubmoduleReloadCallbackAttribute));
        }

        /// <summary>
        /// 控制器对象子模块倾泻回调处理接口函数
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void OnSubmoduleDumpCallback()
        {
            OnSubmoduleActionCallbackOfTargetAttribute(typeof(OnControllerSubmoduleDumpCallbackAttribute));
        }

        /// <summary>
        /// 控制器对象子模块指定类型的回调函数触发处理接口
        /// </summary>
        /// <param name="attrType">属性类型</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void OnSubmoduleActionCallbackOfTargetAttribute(Type attrType)
        {
            if (TryGetSubmoduleBehaviourCallback(attrType, out Delegate callback))
            {
                callback.DynamicInvoke();
            }
        }

        private bool TryGetSubmoduleBehaviourCallback(Type targetType, out Delegate callback)
        {
            if (_cachedSubmoduleBehaviourCallbacks.TryGetValue(targetType, out Delegate handler))
            {
                callback = handler;
                return null != callback;
            }

            callback = Utils.CreateSubmoduleBehaviourCallback(this, targetType);

            _cachedSubmoduleBehaviourCallbacks.Add(targetType, callback);

            return null != callback;
        }
    }
}
