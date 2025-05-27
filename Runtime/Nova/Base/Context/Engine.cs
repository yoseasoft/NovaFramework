/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
/// Copyring (C) 2017 - 2020, Shanghai Tommon Network Technology Co., Ltd.
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

using UnityMonoBehaviour = UnityEngine.MonoBehaviour;

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
    public partial class Engine : IUpdatable
    {
        /// <summary>
        /// 核心引擎对象静态实例
        /// </summary>
        protected static Engine s_instance;

        /// <summary>
        /// 表现层管理对象实例
        /// </summary>
        protected Facade m_facade = null;

        /// <summary>
        /// 记录当前引擎对象实例是否已经启动的状态标识
        /// </summary>
        protected bool m_isOnStartup = false;

        /// <summary>
        /// 引擎对象实例所依赖的MONO组件对象
        /// </summary>
        // private readonly UnityMonoBehaviour m_monoBehaviour = null;

        /// <summary>
        /// 获取当前引擎对象的表现层管理实例
        /// </summary>
        public Facade Facade
        {
            get { return m_facade; }
        }

        /// <summary>
        /// 检测当前引擎对象是否处于启动状态
        /// </summary>
        public bool IsOnStartup
        {
            get { return m_isOnStartup; }
        }

        /// <summary>
        /// 引擎对象构造函数
        /// </summary>
        protected Engine()
        {
            // MONO组件初始化
            // m_monoBehaviour = monoBehaviour;
        }

        /// <summary>
        /// 引擎对象析构函数
        /// </summary>
        ~Engine()
        {
            // m_monoBehaviour = null;
        }

        /// <summary>
        /// 对象初始化回调接口，在实例构建成功时调用，子类中可以不处理该接口
        /// </summary>
        /// <returns>默认返回true，若返回值为false，则实例初始化失败</returns>
        protected virtual bool Initialize()
        {
            // 该初始化接口仅可调用一次，若需再次初始化该接口，需将之前的实例销毁掉
            Logger.Assert(null == s_instance);

            // if (null == m_monoBehaviour) { Logger.Error("引擎对象实例的MONO组件对象实例为空，引擎初始化失败！"); return false; }

            // 表现层对象实例初始化
            m_facade = Facade.Create(this);

            // 将当前对象赋予引擎静态实例
            // s_instance = this;

            return true;
        }

        /// <summary>
        /// 对象清理回调接口，在实例销毁之前调用，子类中可以不处理该接口
        /// </summary>
        protected virtual void Cleanup()
        {
            if (m_isOnStartup)
            {
                this.Shutdown();
            }

            // 表现层对象实例清理
            Facade.Destroy();
            m_facade = null;

            // 清理表现层的静态实例
            Facade.Destroy();
        }

        /// <summary>
        /// 该接口并不产生单例对象，仅返回当前的静态实例属性值
        /// </summary>
        /// <returns>返回引擎对象的静态实例</returns>
        public static Engine Instance
        {
            get { return s_instance; }
        }

        /// <summary>
        /// 单例类的实例创建接口
        /// </summary>
        public static Engine Create()
        {
            if (s_instance == null)
            {
                Engine engine = new Engine();
                if (engine.Initialize())
                {
                    s_instance = engine;
                }
            }

            return s_instance;
        }

        /// <summary>
        /// 单例类的实例销毁接口
        /// </summary>
        public static void Destroy()
        {
            if (s_instance != null)
            {
                s_instance.Cleanup();
                s_instance = null;
            }
        }

        public virtual void Startup()
        {
            // 程序正式启动
            Application.Instance.Startup();

            // 引擎正常启动，总控对象不可为null
            Logger.Assert(null != m_facade);

            m_facade.Startup();

            m_isOnStartup = true;
        }

        public virtual void Shutdown()
        {
            // 引擎尚未启动
            if (false == m_isOnStartup)
            {
                Logger.Warn("The kernel engine was not startup, do it shutdown failed.");
                return;
            }

            m_isOnStartup = false;

            m_facade.Shutdown();

            // 程序关闭
            Application.Instance.Shutdown();
        }

        // FixedUpdate is often called more frequently than Update
        // public virtual void FixedUpdate() { }

        // Update is called once per frame
        public virtual void Update()
        {
            m_facade.Update();
        }

        // LateUpdate is called once per frame, after Update has finished
        public virtual void LateUpdate()
        {
            m_facade.LateUpdate();
        }
    }
}
