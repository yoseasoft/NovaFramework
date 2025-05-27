/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyring (C) 2020 - 2022, Guangzhou Xinyuan Technology Co., Ltd.
/// Copyring (C) 2022 - 2023, Shanghai Bilibili Technology Co., Ltd.
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

namespace GameEngine
{
    /// <summary>
    /// 场景对象抽象类，对场景上下文进行封装及调度管理
    /// </summary>
    public abstract class CScene : CEntity, IUpdateActivation
    {
        /// <summary>
        /// 场景对象的类后缀标签
        /// </summary>
        public const string CLASS_SUFFIX_TAG = "Scene";

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
    }
}
