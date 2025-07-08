/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyring (C) 2023, Guangzhou Shiyue Network Technology Co., Ltd.
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

namespace GameEngine.Debug
{
    /// <summary>
    /// 调试管理器的对象实现类，对接口函数进行具体逻辑的实现
    /// </summary>
    internal sealed partial class DebuggerManager : NovaEngine.CFrameworkManager, IDebuggerManager
    {
        /// <summary>
        /// 调试管理器的配置对象实例
        /// </summary>
        private readonly IDebuggerSetting m_debuggerSetting;
        /// <summary>
        /// 调试管理器的根窗口组实例
        /// </summary>
        private readonly DebuggerWindowGroup m_debuggerWindowRoot;
        /// <summary>
        /// 当前管理器激或窗口状态标识
        /// </summary>
        private bool m_activeWindow;

        /// <summary>
        /// 获取管理器实例的优先级
        /// </summary>
        public override int Priority
        {
            get { return -1; }
        }

        /// <summary>
        /// 获取或设置调试器窗口是否激活的状态标识
        /// </summary>
        public bool ActiveWindow
        {
            get { return m_activeWindow; }
            set { m_activeWindow = value; }
        }

        /// <summary>
        /// 获取调试器的设置实例
        /// </summary>
        public IDebuggerSetting DebuggerSetting
        {
            get { return m_debuggerSetting; }
        }

        /// <summary>
        /// 获取调试器窗口的根节点
        /// </summary>
        public IDebuggerWindowGroup DebuggerWindowRoot
        {
            get { return m_debuggerWindowRoot; }
        }

        public DebuggerManager()
        {
            m_debuggerSetting = new DebuggerSetting();
            m_debuggerWindowRoot = new DebuggerWindowGroup();
            m_activeWindow = false;
        }

        /// <summary>
        /// 管理器对象的初始化回调函数
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();

            m_debuggerWindowRoot.Initialize();
        }

        /// <summary>
        /// 管理器对象的清理回调函数
        /// </summary>
        public override void Cleanup()
        {
            m_activeWindow = false;

            m_debuggerWindowRoot.Cleanup();
        }

        /// <summary>
        ///管理器对象的刷新回调函数
        /// </summary>
        public override void Update()
        {
            if (!m_activeWindow)
            {
                return;
            }

            m_debuggerWindowRoot.OnUpdate(NovaEngine.Timestamp.DeltaTime, NovaEngine.Timestamp.UnscaledDeltaTime);
        }

        /// <summary>
        ///管理器对象的后置刷新回调函数
        /// </summary>
        public override void LateUpdate()
        {
        }

        /// <summary>
        /// 注册指定的路径和窗口对象实例到当前的调试管理器中
        /// </summary>
        /// <param name="path">窗口路径</param>
        /// <param name="debuggerWindow">调试窗口实例</param>
        /// <param name="args">窗口初始化参数</param>
        public void RegisterDebuggerWindow(string path, IDebuggerWindow debuggerWindow, params object[] args)
        {
            if (string.IsNullOrEmpty(path))
            {
                Debugger.Error("The path is invalid.");
                return;
            }

            if (null == debuggerWindow)
            {
                Debugger.Error("The debugger window is invalid.");
                return;
            }

            m_debuggerWindowRoot.RegisterDebuggerWindow(path, debuggerWindow);
            debuggerWindow.Initialize(args);
        }

        /// <summary>
        /// 从当前调试管理器中注销指定路径对应的窗口对象实例
        /// </summary>
        /// <param name="path">窗口路径</param>
        /// <returns>若窗口注销成功返回true，否则返回false</returns>
        public bool UnregisterDebuggerWindow(string path)
        {
            return m_debuggerWindowRoot.UnregisterDebuggerWindow(path);
        }

        /// <summary>
        /// 获取指定路径对应的调试窗口对象实例
        /// </summary>
        /// <param name="path">窗口路径</param>
        /// <returns>若存在路径对应的窗口实例则返回其引用，否则返回null</returns>
        public IDebuggerWindow GetDebuggerWindow(string path)
        {
            return m_debuggerWindowRoot.GetDebuggerWindow(path);
        }

        /// <summary>
        /// 选中当前调试管理器中指定路径对应的调试窗口实例
        /// </summary>
        /// <param name="path">窗口路径</param>
        /// <returns>若选中窗口实例成功返回true，否则返回false</returns>
        public bool SelectedDebuggerWindow(string path)
        {
            return m_debuggerWindowRoot.SelectDebuggerWindow(path);
        }
    }
}
