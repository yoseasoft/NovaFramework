/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyright (C) 2023 - 2024, Guangzhou Shiyue Network Technology Co., Ltd.
/// Copyright (C) 2025, Hurley, Independent Studio.
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

using System.Collections.Generic;

namespace GameEngine
{
    /// <summary>
    /// 通用对象抽象类，对场景中的通用对象上下文进行封装及调度管理
    /// </summary>
    public abstract class CObject : CRef, IBeanLifecycle
    {
        /// <summary>
        /// 获取通用对象的分类标签
        /// </summary>
        public override sealed CBeanClassificationLabel ClassificationLabel => CBeanClassificationLabel.Object;

        /// <summary>
        /// 通用对象等待销毁状态标识
        /// </summary>
        private bool _isOnWaitingDestroy = false;

        /// <summary>
        /// 通用对象初始化通知接口函数
        /// </summary>
        public override sealed void Initialize()
        {
            base.Initialize();

            OnInitialize();
        }

        /// <summary>
        /// 通用对象内部初始化通知接口函数
        /// </summary>
        protected virtual void OnInitialize() { }

        /// <summary>
        /// 通用对象清理通知接口函数
        /// </summary>
        public override sealed void Cleanup()
        {
            OnCleanup();

            base.Cleanup();
        }

        /// <summary>
        /// 通用对象内部清理通知接口函数
        /// </summary>
        protected virtual void OnCleanup() { }

        /// <summary>
        /// 通用对象启动通知接口函数
        /// </summary>
        public override sealed void Startup()
        {
            // base.Startup();

            OnStartup();
        }

        /// <summary>
        /// 通用对象内部启动通知接口函数
        /// </summary>
        protected virtual void OnStartup() { }

        /// <summary>
        /// 通用对象关闭通知接口函数
        /// </summary>
        public override sealed void Shutdown()
        {
            OnShutdown();

            // base.Shutdown();
        }

        /// <summary>
        /// 通用对象内部关闭通知接口函数
        /// </summary>
        protected virtual void OnShutdown() { }

        /// <summary>
        /// 通用对象执行通知接口函数
        /// </summary>
        public override sealed void Execute()
        {
            OnExecute();
        }

        /// <summary>
        /// 通用对象内部执行通知接口函数
        /// </summary>
        protected virtual void OnExecute() { }

        /// <summary>
        /// 通用对象后置执行通知接口函数
        /// </summary>
        public override sealed void LateExecute()
        {
            OnLateExecute();
        }

        /// <summary>
        /// 通用对象内部后置执行通知接口函数
        /// </summary>
        protected virtual void OnLateExecute() { }

        /// <summary>
        /// 通用对象刷新通知接口函数
        /// </summary>
        public override sealed void Update()
        {
            OnUpdate();
        }

        /// <summary>
        /// 通用对象内部刷新通知接口函数
        /// </summary>
        protected virtual void OnUpdate() { }

        /// <summary>
        /// 通用对象后置刷新通知接口函数
        /// </summary>
        public override sealed void LateUpdate()
        {
            OnLateUpdate();
        }

        /// <summary>
        /// 通用对象内部后置刷新通知接口函数
        /// </summary>
        protected virtual void OnLateUpdate() { }

        /// <summary>
        /// 通用对象唤醒通知函数接口
        /// </summary>
        public void Awake()
        {
            OnAwake();
        }

        /// <summary>
        /// 通用对象内部唤醒通知接口函数
        /// </summary>
        protected virtual void OnAwake() { }

        /// <summary>
        /// 通用对象开始通知函数接口
        /// </summary>
        public void Start()
        {
        }

        /// <summary>
        /// 通用对象内部开始通知接口函数
        /// </summary>
        protected virtual void OnStart() { }

        /// <summary>
        /// 通用对象销毁通知函数接口
        /// </summary>
        public void Destroy()
        {
            _isOnWaitingDestroy = false;

            OnDestroy();
        }

        /// <summary>
        /// 通用对象内部销毁通知接口函数
        /// </summary>
        protected virtual void OnDestroy() { }

        /// <summary>
        /// 标记当前通用对象此刻为待销毁状态<br/>
        /// 待销毁状态一旦设定便不可更改，只能等待系统删除回收此通用对象实例
        /// </summary>
        public void OnPrepareToDestroy()
        {
            _isOnWaitingDestroy = true;
            BeanController.Instance.RegBeanLifecycleNotification(AspectBehaviourType.Destroy, this);
        }
    }
}
