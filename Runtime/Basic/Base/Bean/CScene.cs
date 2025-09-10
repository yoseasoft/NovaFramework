/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyright (C) 2020 - 2022, Guangzhou Xinyuan Technology Co., Ltd.
/// Copyright (C) 2022 - 2023, Shanghai Bilibili Technology Co., Ltd.
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

namespace GameEngine
{
    /// <summary>
    /// 场景对象抽象类，对场景上下文进行封装及调度管理
    /// </summary>
    public abstract class CScene : CEntity
    {
        /// <summary>
        /// 获取场景句柄对象
        /// </summary>
        public static SceneHandler SceneHandler => SceneHandler.Instance;

        /// <summary>
        /// 获取实体对象的名称
        /// </summary>
        public override sealed string Name => SceneHandler.GetSceneNameForType(GetType());

        /// <summary>
        /// 场景对象初始化函数
        /// </summary>
        public override sealed void Initialize()
        {
            base.Initialize();

            OnInitialize();
        }

        /// <summary>
        /// 场景对象内部初始化函数
        /// </summary>
        protected virtual void OnInitialize() { }

        /// <summary>
        /// 场景对象清理函数
        /// </summary>
        public override sealed void Cleanup()
        {
            OnCleanup();

            base.Cleanup();
        }

        /// <summary>
        /// 场景对象内部清理函数
        /// </summary>
        protected virtual void OnCleanup() { }

        /// <summary>
        /// 场景对象启动通知接口函数
        /// </summary>
        public override sealed void Startup()
        {
            base.Startup();

            OnStartup();
        }

        /// <summary>
        /// 场景对象内部启动通知接口函数
        /// </summary>
        protected virtual void OnStartup() { }

        /// <summary>
        /// 场景对象关闭通知接口函数
        /// </summary>
        public override sealed void Shutdown()
        {
            OnShutdown();

            base.Shutdown();
        }

        /// <summary>
        /// 场景对象内部关闭通知接口函数
        /// </summary>
        protected virtual void OnShutdown() { }

        /// <summary>
        /// 场景对象刷新通知接口函数
        /// </summary>
        public override sealed void Update()
        {
            base.Update();

            OnUpdate();
        }

        /// <summary>
        /// 场景对象内部刷新通知接口函数
        /// </summary>
        protected virtual void OnUpdate() { }

        /// <summary>
        /// 场景对象后置刷新通知接口函数
        /// </summary>
        public override sealed void LateUpdate()
        {
            base.LateUpdate();

            OnLateUpdate();
        }

        /// <summary>
        /// 场景对象内部后置刷新通知接口函数
        /// </summary>
        protected virtual void OnLateUpdate() { }

        /// <summary>
        /// 场景对象唤醒通知函数接口
        /// </summary>
        public override sealed void Awake()
        {
            base.Awake();

            OnAwake();
        }

        /// <summary>
        /// 场景对象内部唤醒通知函数接口
        /// </summary>
        protected virtual void OnAwake()
        { }

        /// <summary>
        /// 场景对象开始通知函数接口
        /// </summary>
        public override sealed void Start()
        {
            base.Start();

            OnStart();
        }

        /// <summary>
        /// 场景对象内部开始通知函数接口
        /// </summary>
        protected virtual void OnStart()
        { }

        /// <summary>
        /// 场景对象销毁通知函数接口
        /// </summary>
        public override sealed void Destroy()
        {
            OnDestroy();

            base.Destroy();
        }

        /// <summary>
        /// 场景对象内部销毁通知函数接口
        /// </summary>
        protected virtual void OnDestroy()
        { }

        /// <summary>
        /// 加载场景资源的接口函数
        /// </summary>
        /// <param name="assetName">资源名称</param>
        /// <param name="assetUrl">资源路径</param>
        public void LoadAsset(string assetName, string assetUrl)
        {
        }

        /// <summary>
        /// 卸载场景资源的接口函数
        /// </summary>
        /// <param name="assetName">资源名称</param>
        public void UnloadAsset(string assetName)
        {
        }

        #region 场景对象功能检测相关接口函数合集

        /// <summary>
        /// 检测当前实体对象是否激活刷新行为<br/>
        /// 检测的激活条件包括实体自身和其内部的组件实例
        /// </summary>
        /// <returns>若实体对象激活刷新行为则返回true，否则返回false</returns>
        protected internal override bool IsUpdateActivation()
        {
            return true;
        }

        #endregion

        #region 场景对象内部组件相关的接口实现函数

        /// <summary>
        /// 当前场景对象内部的组件列表发生改变时的回调通知
        /// </summary>
        protected override sealed void OnInternalComponentsChanged()
        {
            // 2025-08-13：
            // 目前通知组件变更主要用于更新刷新列表
            // 但场景对象是默认处于刷新激活状态，无需更新
            // 所以此处暂时屏蔽该调用，若该通知有其它意义后可重新打开此通知
            // SceneHandler.OnEntityInternalComponentsChanged(this);
        }

        #endregion
    }
}
