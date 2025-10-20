/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyright (C) 2020 - 2022, Guangzhou Xinyuan Technology Co., Ltd.
/// Copyright (C) 2022 - 2023, Shanghai Bilibili Technology Co., Ltd.
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

using UniTask = Cysharp.Threading.Tasks.UniTask;

namespace GameEngine
{
    /// <summary>
    /// 视图对象窗口类型的枚举定义
    /// </summary>
    public enum ViewFormType : byte
    {
        /// <summary>
        /// 无效
        /// </summary>
        Unknown = 0,

        UGUI,

        FairyGUI,

        UIToolkit,
    }

    /// <summary>
    /// 视图对象抽象类，对用户界面对象上下文进行封装及调度管理
    /// 
    /// 2024-06-22：
    /// 直接在视图基类添加刷新激活标识，因为项目中同时存在的视图总数不多，至多不超过5个
    /// 若项目中同时存在大量视图的情况，需要禁掉该标识，在具体实现类中视情况手动添加该标识
    /// 
    /// 2025-08-13：
    /// 将刷新激活流程统一作动态检测处理，避免无效的刷新调度消耗性能
    /// </summary>
    public abstract class CView : CEntity
    {
        /// <summary>
        /// 获取视图句柄对象
        /// </summary>
        public static GuiHandler GuiHandler => GuiHandler.Instance;

        /// <summary>
        /// 获取视图对象的名称
        /// </summary>
        public override sealed string Name => GuiHandler.GetViewNameForType(GetType());

        /// <summary>
        /// 视图对象实例已经关闭的状态标识
        /// </summary>
        protected bool _isClosed = false;

        /// <summary>
        /// 视图对象挂载的窗口实例
        /// </summary>
        protected Form _form;

        /// <summary>
        /// 窗口组件的根节点对象实例
        /// </summary>
        public object Window => _form?.Root;

        /// <summary>
        /// 取消异步任务
        /// </summary>
        public System.Threading.CancellationTokenSource CancellationTokenSource { get; } = new();

        /// <summary>
        /// 是否准备好显示界面
        /// </summary>
        public bool IsReady { get; protected set; }

        /// <summary>
        /// 视图对象模型加载成功状态标识
        /// </summary>
        public bool IsLoaded => _form?.IsLoaded ?? false;

        /// <summary>
        /// 获取当前视图对象实例的关闭状态
        /// </summary>
        public bool IsClosed => _isClosed;

        /// <summary>
        /// 视图对象初始化通知接口函数
        /// </summary>
        public override sealed void Initialize()
        {
            base.Initialize();

            OnInitialize();
        }

        /// <summary>
        /// 视图对象内部通知接口函数
        /// </summary>
        protected virtual void OnInitialize() { }

        /// <summary>
        /// 视图对象清理通知接口函数
        /// </summary>
        public override sealed void Cleanup()
        {
            CancellationTokenSource.Cancel();
            CancellationTokenSource.Dispose();

            OnCleanup();

            base.Cleanup();
        }

        /// <summary>
        /// 视图对象内部清理通知接口函数
        /// </summary>
        protected virtual void OnCleanup() { }

        /// <summary>
        /// 视图对象启动通知接口函数
        /// </summary>
        public override sealed void Startup()
        {
            base.Startup();

            OnStartup();
        }

        /// <summary>
        /// 视图对象内部启动通知接口函数
        /// </summary>
        protected virtual void OnStartup() { }

        /// <summary>
        /// 视图对象关闭通知接口函数
        /// </summary>
        public override sealed void Shutdown()
        {
            OnShutdown();

            base.Shutdown();
        }

        /// <summary>
        /// 视图对象内部关闭通知接口函数
        /// </summary>
        protected virtual void OnShutdown() { }

        /// <summary>
        /// 视图对象执行通知接口函数
        /// </summary>
        public override sealed void Execute()
        {
            base.Execute();

            OnExecute();
        }

        /// <summary>
        /// 视图对象内部执行通知接口函数
        /// </summary>
        protected virtual void OnExecute() { }

        /// <summary>
        /// 视图对象后置执行通知接口函数
        /// </summary>
        public override sealed void LateExecute()
        {
            base.LateExecute();

            OnLateExecute();
        }

        /// <summary>
        /// 视图对象内部后置执行通知接口函数
        /// </summary>
        protected virtual void OnLateExecute() { }

        /// <summary>
        /// 视图对象刷新通知接口函数
        /// </summary>
        public override sealed void Update()
        {
            base.Update();

            OnUpdate();
        }

        /// <summary>
        /// 视图对象内部刷新通知接口函数
        /// </summary>
        protected virtual void OnUpdate() { }

        /// <summary>
        /// 视图对象后置刷新通知接口函数
        /// </summary>
        public override sealed void LateUpdate()
        {
            base.LateUpdate();

            OnLateUpdate();
        }

        /// <summary>
        /// 视图对象内部后置刷新通知接口函数
        /// </summary>
        protected virtual void OnLateUpdate() { }

        /// <summary>
        /// 视图对象唤醒通知函数接口
        /// </summary>
        public override sealed void Awake()
        {
            base.Awake();

            OnAwake();
        }

        /// <summary>
        /// 视图对象内部唤醒通知函数接口
        /// </summary>
        protected virtual void OnAwake()
        { }

        /// <summary>
        /// 视图对象开始通知函数接口
        /// </summary>
        public override sealed void Start()
        {
            base.Start();

            OnStart();

            IsReady = true;
        }

        /// <summary>
        /// 视图对象内部开始通知函数接口
        /// </summary>
        protected virtual void OnStart()
        { }

        /// <summary>
        /// 视图对象销毁通知函数接口
        /// </summary>
        public override sealed void Destroy()
        {
            OnDestroy();

            base.Destroy();
        }

        /// <summary>
        /// 视图对象内部销毁通知函数接口
        /// </summary>
        protected virtual void OnDestroy()
        { }

        /// <summary>
        /// 关闭当前的视图对象实例
        /// </summary>
        protected internal void __Close()
        {
            if (_isClosed)
                return;

            // 先标记关闭状态
            _isClosed = true;

            // 关闭通知
            Call(Shutdown, AspectBehaviourType.Shutdown);

            // 销毁窗口
            DestroyWindow();

            // 移除容器中的实例引用
            GuiHandler.RemoveUI(this);
        }

        /// <summary>
        /// 关闭当前的视图对象实例
        /// </summary>
        public void Close()
        {
            // 移除容器中的实例引用
            GuiHandler.CloseUI(this);
        }

        #region 视图对象窗口资源管理相关接口函数

        /// <summary>
        /// 加载当前视图对象的窗口实例
        /// </summary>
        internal async UniTask CreateWindow()
        {
            _form = FormHelper.CreateForm(FormType.FairyGUI, BeanType);

            await _form.Load();
        }

        /// <summary>
        /// 显示当前视图对象的窗口实例
        /// </summary>
        internal void ShowWindow()
        {
            _form?.Show();
        }

        /// <summary>
        /// 销毁当前视图对象的窗口实例
        /// </summary>
        protected void DestroyWindow()
        {
            _form?.Unload();
            _form = null;
        }

        #endregion

        #region 视图对象窗口内部节点操作相关接口函数

        /// <summary>
        /// 通过指定节点路径获取对应的节点对象实例
        /// </summary>
        /// <param name="path">节点路径</param>
        /// <returns>返回给定路径对应的节点对象实例，若不存在则返回null</returns>
        public object GetChild(string path)
        {
            if (null == _form || false == _form.IsLoaded)
            {
                Debugger.Warn(LogGroupTag.Bean, "当前视图对象‘{%t}’的窗口组件尚未载入完成，无法对窗口内部的控件实例进行访问。", this);
                return null;
            }

            return _form.GetChild(path);
        }

        #endregion

        #region 视图对象内部组件相关的接口实现函数

        /// <summary>
        /// 当前视图对象内部的组件列表发生改变时的回调通知
        /// </summary>
        protected override sealed void OnInternalComponentsChanged()
        {
            GuiHandler.OnEntityInternalComponentsChanged(this);
        }

        #endregion
    }
}
