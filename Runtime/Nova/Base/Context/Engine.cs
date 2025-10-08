/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
/// Copyright (C) 2017 - 2020, Shanghai Tommon Network Technology Co., Ltd.
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

namespace NovaEngine
{
    /// <summary>
    /// 程序核心加载引擎，包括关键实例的初始化，和控制上层脚本的启动，刷新及关闭
    /// MONO组件启动顺序：
    ///     -> Reset
    ///     -> Awake
    ///     -> OnEnable
    ///     -> Start
    ///     -> FixedUpdate
    ///     -> Update
    ///     -> LateUpdate
    ///     -> OnWillRenderObject
    ///     -> OnGUI
    ///     -> OnApplicationPause
    ///     -> OnDisable
    ///     -> OnDestroy
    ///     -> OnApplicationQuit
    /// </summary>
    public sealed /*partial*/ class Engine : IUpdatable
    {
        /// <summary>
        /// 核心引擎对象静态实例
        /// </summary>
        private static Engine _instance;

        /// <summary>
        /// 表现层管理对象实例
        /// </summary>
        private Facade _facade = null;

        /// <summary>
        /// 记录当前引擎对象实例是否已经启动的状态标识
        /// </summary>
        private bool _isOnStartup = false;

        /// <summary>
        /// 引擎对象实例所依赖的MONO组件对象
        /// </summary>
        // private readonly UnityMonoBehaviour _monoBehaviour = null;

        /// <summary>
        /// 获取当前引擎对象的表现层管理实例
        /// </summary>
        public Facade Facade
        {
            get { return _facade; }
        }

        /// <summary>
        /// 检测当前引擎对象是否处于启动状态
        /// </summary>
        public bool IsOnStartup
        {
            get { return _isOnStartup; }
        }

        /// <summary>
        /// 引擎对象构造函数
        /// </summary>
        private Engine()
        {
            // MONO组件初始化
            // _monoBehaviour = monoBehaviour;
        }

        /// <summary>
        /// 引擎对象析构函数
        /// </summary>
        ~Engine()
        {
            // _monoBehaviour = null;
        }

        /// <summary>
        /// 对象初始化回调接口，在实例构建成功时调用，子类中可以不处理该接口
        /// </summary>
        /// <returns>默认返回true，若返回值为false，则实例初始化失败</returns>
        private bool Initialize()
        {
            // 该初始化接口仅可调用一次，若需再次初始化该接口，需将之前的实例销毁掉
            Logger.Assert(null == _instance);

            // if (null == _monoBehaviour) { Logger.Error("引擎对象实例的MONO组件对象实例为空，引擎初始化失败！"); return false; }

            // 表现层对象实例初始化
            _facade = Facade.Create(this);

            // 将当前对象赋予引擎静态实例
            // _instance = this;

            return true;
        }

        /// <summary>
        /// 对象清理回调接口，在实例销毁之前调用，子类中可以不处理该接口
        /// </summary>
        private void Cleanup()
        {
            if (_isOnStartup)
            {
                this.Shutdown();
            }

            // 表现层对象实例清理
            Facade.Destroy();
            _facade = null;

            // 清理表现层的静态实例
            Facade.Destroy();
        }

        /// <summary>
        /// 该接口并不产生单例对象，仅返回当前的静态实例属性值
        /// </summary>
        /// <returns>返回引擎对象的静态实例</returns>
        public static Engine Instance
        {
            get { return _instance; }
        }

        /// <summary>
        /// 单例类的实例创建接口
        /// </summary>
        public static Engine Create()
        {
            if (_instance == null)
            {
                Engine engine = new Engine();
                if (engine.Initialize())
                {
                    _instance = engine;
                }
            }

            return _instance;
        }

        /// <summary>
        /// 单例类的实例销毁接口
        /// </summary>
        public static void Destroy()
        {
            if (_instance != null)
            {
                _instance.Cleanup();
                _instance = null;
            }
        }

        public void Startup()
        {
            // 程序正式启动
            Application.Instance.Startup();

            // 引擎正常启动，总控对象不可为null
            Logger.Assert(null != _facade);

            _facade.Startup();

            _isOnStartup = true;
        }

        public void Shutdown()
        {
            // 引擎尚未启动
            if (false == _isOnStartup)
            {
                Logger.Warn("The kernel engine was not startup, do it shutdown failed.");
                return;
            }

            _isOnStartup = false;

            _facade.Shutdown();

            // 程序关闭
            Application.Instance.Shutdown();
        }

        // FixedExecute is often called more frequently than Execute
        // public void FixedExecute() { }

        // Execute is called once per frame
        public void Execute()
        {
        }

        // LateExecute is called once per frame, after Execute has finished
        public void LateExecute()
        {
        }

        // FixedUpdate is often called more frequently than Update
        // public void FixedUpdate() { }

        // Update is called once per frame
        public void Update()
        {
            // 刷新时间戳
            Timestamp.RefreshTimeOnUpdate();

            _facade.Update();
        }

        // LateUpdate is called once per frame, after Update has finished
        public void LateUpdate()
        {
            _facade.LateUpdate();
        }
    }
}
