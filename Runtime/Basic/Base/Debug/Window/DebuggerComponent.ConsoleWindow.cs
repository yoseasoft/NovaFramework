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

using System.Collections.Generic;

using UnityColor = UnityEngine.Color;
using UnityColor32 = UnityEngine.Color32;
using UnityVector2 = UnityEngine.Vector2;
using UnityLogType = UnityEngine.LogType;
using UnityApplication = UnityEngine.Application;
using UnityGUILayout = UnityEngine.GUILayout;

namespace GameEngine.Debug
{
    /// <summary>
    /// 游戏调试器组件对象类，用于定义调试器对象的基础属性及访问操作函数
    /// </summary>
    public sealed partial class DebuggerComponent
    {
        /// <summary>
        /// 调试器的控制台窗口对象类，声明了控制台输出内容的管理控制接口函数
        /// </summary>
        [System.Serializable]
        public sealed class ConsoleWindow : IDebuggerWindow
        {
            /// <summary>
            /// 日志节点的存储管理队列
            /// </summary>
            private readonly Queue<LogNode> m_logNodes = new Queue<LogNode>();

            private IDebuggerSetting m_debuggerSetting = null;
            private UnityVector2 m_logScrollPosition = UnityVector2.zero;
            private UnityVector2 m_stackScrollPosition = UnityVector2.zero;
            private int m_infoCount = 0;
            private int m_warnCount = 0;
            private int m_errorCount = 0;
            private int m_fatalCount = 0;
            private LogNode m_selectedNode = null;
            private bool m_lastLockScroll = true;
            private bool m_lastInfoFilter = true;
            private bool m_lastWarnFilter = true;
            private bool m_lastErrorFilter = true;
            private bool m_lastFatalFilter = true;

            [UnityEngine.SerializeField]
            private bool m_lockScroll = true;

            [UnityEngine.SerializeField]
            private int m_maxLine = 100;

            [UnityEngine.SerializeField]
            private bool m_infoFilter = true;

            [UnityEngine.SerializeField]
            private bool m_warnFilter = true;

            [UnityEngine.SerializeField]
            private bool m_errorFilter = true;

            [UnityEngine.SerializeField]
            private bool m_fatalFilter = true;

            [UnityEngine.SerializeField]
            private UnityColor32 m_infoColor = UnityColor.white;

            [UnityEngine.SerializeField]
            private UnityColor32 m_warnColor = UnityColor.yellow;

            [UnityEngine.SerializeField]
            private UnityColor32 m_errorColor = UnityColor.red;

            [UnityEngine.SerializeField]
            private UnityColor32 m_fatalColor = new UnityColor(0.7f, 0.2f, 0.2f);

            public int InfoCount
            {
                get { return m_infoCount; }
            }

            public int WarnCount
            {
                get { return m_warnCount; }
            }

            public int ErrorCount
            {
                get { return m_errorCount; }
            }

            public int FatalCount
            {
                get { return m_fatalCount; }
            }

            public bool LockScroll
            {
                get { return m_lockScroll; }
                set { m_lockScroll = value; }
            }

            public int MaxLine
            {
                get { return m_maxLine; }
                set { m_maxLine = value; }
            }

            public bool InfoFilter
            {
                get { return m_infoFilter; }
                set { m_infoFilter = value; }
            }

            public bool WarnFilter
            {
                get { return m_warnFilter; }
                set { m_warnFilter = value; }
            }

            public bool ErrorFilter
            {
                get { return m_errorFilter; }
                set { m_errorFilter = value; }
            }

            public bool FatalFilter
            {
                get { return m_fatalFilter; }
                set { m_fatalFilter = value; }
            }

            public UnityColor32 InfoColor
            {
                get { return m_infoColor; }
                set { m_infoColor = value; }
            }

            public UnityColor32 WarnColor
            {
                get { return m_warnColor; }
                set { m_warnColor = value; }
            }

            public UnityColor32 ErrorColor
            {
                get { return m_errorColor; }
                set { m_errorColor = value; }
            }

            public UnityColor32 FatalColor
            {
                get { return m_fatalColor; }
                set { m_fatalColor = value; }
            }

            /// <summary>
            /// 调试器窗口初始化操作函数
            /// </summary>
            /// <param name="args">参数列表</param>
            public void Initialize(params object[] args)
            {
                IDebuggerManager debuggerManager = NovaEngine.AppEntry.GetManager<IDebuggerManager>();
                m_debuggerSetting = debuggerManager.DebuggerSetting;
                if (null == m_debuggerSetting)
                {
                    Debugger.Fatal("Setting component is invalid.");
                    return;
                }

                UnityApplication.logMessageReceived += OnLogMessageReceived;

                m_lockScroll = m_lastLockScroll = m_debuggerSetting.GetBool(IDebuggerSetting.ConsoleLockScroll, true);
                m_infoFilter = m_lastInfoFilter = m_debuggerSetting.GetBool(IDebuggerSetting.ConsoleInfoFilter, true);
                m_warnFilter = m_lastWarnFilter = m_debuggerSetting.GetBool(IDebuggerSetting.ConsoleWarnFilter, true);
                m_errorFilter = m_lastErrorFilter = m_debuggerSetting.GetBool(IDebuggerSetting.ConsoleErrorFilter, true);
                m_fatalFilter = m_lastFatalFilter = m_debuggerSetting.GetBool(IDebuggerSetting.ConsoleFatalFilter, true);
            }

            /// <summary>
            /// 调试器窗口清理操作函数
            /// </summary>
            public void Cleanup()
            {
                UnityApplication.logMessageReceived -= OnLogMessageReceived;

                // 移除日志记录
                ClearAllLogs();
            }

            /// <summary>
            /// 进入调试器窗口
            /// </summary>
            public void OnEnter()
            {
            }

            /// <summary>
            /// 离开调试器窗口
            /// </summary>
            public void OnExit()
            {
            }

            /// <summary>
            /// 调试器窗口轮询刷新函数
            /// </summary>
            /// <param name="elapseSeconds">逻辑流逝时间，以秒为单位</param>
            /// <param name="realElapseSeconds">真实流逝时间，以秒为单位</param>
            public void OnUpdate(float elapseSeconds, float realElapseSeconds)
            {
                if (m_lastLockScroll != m_lockScroll)
                {
                    m_lastLockScroll = m_lockScroll;
                    m_debuggerSetting.SetBool(IDebuggerSetting.ConsoleLockScroll, m_lockScroll);
                }

                if (m_lastInfoFilter != m_infoFilter)
                {
                    m_lastInfoFilter = m_infoFilter;
                    m_debuggerSetting.SetBool(IDebuggerSetting.ConsoleInfoFilter, m_infoFilter);
                }

                if (m_lastWarnFilter != m_warnFilter)
                {
                    m_lastWarnFilter = m_warnFilter;
                    m_debuggerSetting.SetBool(IDebuggerSetting.ConsoleWarnFilter, m_warnFilter);
                }

                if (m_lastErrorFilter != m_errorFilter)
                {
                    m_lastErrorFilter = m_errorFilter;
                    m_debuggerSetting.SetBool(IDebuggerSetting.ConsoleErrorFilter, m_errorFilter);
                }

                if (m_lastFatalFilter != m_fatalFilter)
                {
                    m_lastFatalFilter = m_fatalFilter;
                    m_debuggerSetting.SetBool(IDebuggerSetting.ConsoleFatalFilter, m_fatalFilter);
                }
            }

            /// <summary>
            /// 调试器窗口绘制函数
            /// </summary>
            public void OnDraw()
            {
                RefreshLogCount();

                UnityGUILayout.BeginHorizontal();
                {
                    if (UnityGUILayout.Button("Clear All", UnityGUILayout.Width(100f)))
                    {
                        ClearAllLogs();
                    }

                    m_lockScroll = UnityGUILayout.Toggle(m_lockScroll, "Lock Scroll", UnityGUILayout.Width(90f));
                    UnityGUILayout.FlexibleSpace();
                    m_infoFilter = UnityGUILayout.Toggle(m_infoFilter, NovaEngine.Utility.Text.Format("Info ({0})", m_infoCount.ToString()), UnityGUILayout.Width(90f));
                    m_warnFilter = UnityGUILayout.Toggle(m_warnFilter, NovaEngine.Utility.Text.Format("Warn ({0})", m_warnCount.ToString()), UnityGUILayout.Width(90f));
                    m_errorFilter = UnityGUILayout.Toggle(m_errorFilter, NovaEngine.Utility.Text.Format("Error ({0})", m_errorCount.ToString()), UnityGUILayout.Width(90f));
                    m_fatalFilter = UnityGUILayout.Toggle(m_fatalFilter, NovaEngine.Utility.Text.Format("Fatal ({0})", m_fatalCount.ToString()), UnityGUILayout.Width(90f));
                }
                UnityGUILayout.EndHorizontal();

                UnityGUILayout.BeginVertical("box");
                {
                    if (m_lockScroll)
                    {
                        m_logScrollPosition.y = float.MaxValue;
                    }

                    m_logScrollPosition = UnityGUILayout.BeginScrollView(m_logScrollPosition);
                    {
                        bool selected = false;
                        foreach (LogNode logNode in m_logNodes)
                        {
                            switch (logNode.Type)
                            {
                                case UnityLogType.Log:
                                    if (!m_infoFilter) { continue; }
                                    break;
                                case UnityLogType.Warning:
                                    if (!m_warnFilter) { continue; }
                                    break;
                                case UnityLogType.Error:
                                    if (!m_errorFilter) { continue; }
                                    break;
                                case UnityLogType.Exception:
                                    if (!m_fatalFilter) { continue; }
                                    break;
                            }

                            if (UnityGUILayout.Toggle(m_selectedNode == logNode, GetLogString(logNode)))
                            {
                                selected = true;
                                if (m_selectedNode != logNode)
                                {
                                    m_selectedNode = logNode;
                                    m_stackScrollPosition = UnityVector2.zero;
                                }
                            }
                        }

                        if (!selected)
                        {
                            m_selectedNode = null;
                        }
                    }
                    UnityGUILayout.EndScrollView();
                }
                UnityGUILayout.EndVertical();

                UnityGUILayout.BeginVertical("box");
                {
                    m_stackScrollPosition = UnityGUILayout.BeginScrollView(m_stackScrollPosition, UnityGUILayout.Height(100f));
                    {
                        if (m_selectedNode != null)
                        {
                            UnityColor32 color = GetLogStringColor(m_selectedNode.Type);
                            if (UnityGUILayout.Button(NovaEngine.Utility.Text.Format("",
                                    color.r.ToString("x2"), color.g.ToString("x2"), color.b.ToString("x2"), color.a.ToString("x2"),
                                    m_selectedNode.Message, m_selectedNode.StackTrace, System.Environment.NewLine), "label"))
                            {
                            }
                        }
                    }
                    UnityGUILayout.EndScrollView();
                }
                UnityGUILayout.EndVertical();
            }

            /// <summary>
            /// 清理全部日志记录信息
            /// </summary>
            private void ClearAllLogs()
            {
                m_logNodes.Clear();
            }

            /// <summary>
            /// 刷新日志记录的统计计数
            /// </summary>
            public void RefreshLogCount()
            {
                m_infoCount = 0;
                m_warnCount = 0;
                m_errorCount = 0;
                m_fatalCount = 0;

                foreach (LogNode logNode in m_logNodes)
                {
                    switch (logNode.Type)
                    {
                        case UnityLogType.Log:
                            m_infoCount++;
                            break;
                        case UnityLogType.Warning:
                            m_warnCount++;
                            break;
                        case UnityLogType.Error:
                            m_errorCount++;
                            break;
                        case UnityLogType.Exception:
                            m_fatalCount++;
                            break;
                    }
                }
            }

            /// <summary>
            /// 获取最近新增的日志记录，通过传入的列表实例进行返回
            /// 该函数将返回记录的全部日志节点
            /// </summary>
            /// <param name="results">日志记录列表</param>
            public void GetRecentLogs(List<LogNode> results)
            {
                if (null == results)
                {
                    Debugger.Error("Results is invalid.");
                    return;
                }

                results.Clear();
                foreach (LogNode logNode in m_logNodes)
                {
                    results.Add(logNode);
                }
            }

            /// <summary>
            /// 获取最近新增的日志记录，通过传入的列表实例进行返回
            /// 此处限定了获取日志记录的数量
            /// </summary>
            /// <param name="results"></param>
            /// <param name="count"></param>
            public void GetRecentLogs(List<LogNode> results, int count)
            {
                if (null == results)
                {
                    Debugger.Error("Results is invalid.");
                    return;
                }

                if (count <= 0)
                {
                    Debugger.Error("Count is must-be great than zero.");
                    return;
                }

                int position = m_logNodes.Count - count;
                if (position < 0)
                {
                    position = 0;
                }

                int index = 0;
                results.Clear();
                foreach (LogNode logNode in m_logNodes)
                {
                    if (index++ < position)
                    {
                        continue;
                    }

                    results.Add(logNode);
                }
            }

            /// <summary>
            /// 日志消息记录接收回调函数，将新日志消息推入管理队列中
            /// </summary>
            /// <param name="logMessage">日志消息内容</param>
            /// <param name="stackTrace">日志堆栈信息</param>
            /// <param name="logType">日志类型</param>
            private void OnLogMessageReceived(string logMessage, string stackTrace, UnityLogType logType)
            {
                if (UnityLogType.Assert == logType)
                {
                    logType = UnityLogType.Error;
                }

                m_logNodes.Enqueue(LogNode.Create(logType, logMessage, stackTrace));
                // 超出存储上限，移除旧的日志记录
                while (m_logNodes.Count > m_maxLine)
                {
                    LogNode.Release(m_logNodes.Dequeue());
                }
            }

            /// <summary>
            /// 通过日志记录节点获取其字符文本的格式化信息
            /// </summary>
            /// <param name="logNode">日志节点对象实例</param>
            /// <returns>返回日志节点的文本格式化信息</returns>
            private string GetLogString(LogNode logNode)
            {
                UnityColor32 color = GetLogStringColor(logNode.Type);
                return NovaEngine.Utility.Text.Format("<color=#{0}{1}{2}{3}>[{4}][{5}] {6}</color>",
                        color.r.ToString("x2"), color.g.ToString("x2"), color.b.ToString("x2"), color.a.ToString("x2"),
                        logNode.Time.ToLocalTime().ToString("HH:mm:ss.fff"), logNode.FrameCount.ToString(), logNode.Message);
            }

            /// <summary>
            /// 根据指定日志类型获取其对应的文本颜色
            /// </summary>
            /// <param name="logType">日志类型</param>
            /// <returns>返回日志类型对应的文本颜色</returns>
            internal UnityColor32 GetLogStringColor(UnityLogType logType)
            {
                UnityColor32 color = UnityColor.white;
                switch (logType)
                {
                    case UnityLogType.Log:
                        color = m_infoColor;
                        break;
                    case UnityLogType.Warning:
                        color = m_warnColor;
                        break;
                    case UnityLogType.Error:
                        color = m_errorColor;
                        break;
                    case UnityLogType.Exception:
                        color = m_fatalColor;
                        break;
                }

                return color;
            }
        }
    }
}
