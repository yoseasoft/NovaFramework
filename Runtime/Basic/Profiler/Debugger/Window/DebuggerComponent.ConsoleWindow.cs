/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyright (C) 2023, Guangzhou Shiyue Network Technology Co., Ltd.
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

namespace GameEngine.Profiler.Debugging
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
            private readonly Queue<LogNode> _logNodes = new Queue<LogNode>();

            private IDebuggerSetting _debuggerSetting = null;
            private UnityVector2 _logScrollPosition = UnityVector2.zero;
            private UnityVector2 _stackScrollPosition = UnityVector2.zero;
            private int _infoCount = 0;
            private int _warnCount = 0;
            private int _errorCount = 0;
            private int _fatalCount = 0;
            private LogNode _selectedNode = null;
            private bool _lastLockScroll = true;
            private bool _lastInfoFilter = true;
            private bool _lastWarnFilter = true;
            private bool _lastErrorFilter = true;
            private bool _lastFatalFilter = true;

            [UnityEngine.SerializeField]
            private bool _lockScroll = true;

            [UnityEngine.SerializeField]
            private int _maxLine = 100;

            [UnityEngine.SerializeField]
            private bool _infoFilter = true;

            [UnityEngine.SerializeField]
            private bool _warnFilter = true;

            [UnityEngine.SerializeField]
            private bool _errorFilter = true;

            [UnityEngine.SerializeField]
            private bool _fatalFilter = true;

            [UnityEngine.SerializeField]
            private UnityColor32 _infoColor = UnityColor.white;

            [UnityEngine.SerializeField]
            private UnityColor32 _warnColor = UnityColor.yellow;

            [UnityEngine.SerializeField]
            private UnityColor32 _errorColor = UnityColor.red;

            [UnityEngine.SerializeField]
            private UnityColor32 _fatalColor = new UnityColor(0.7f, 0.2f, 0.2f);

            public int InfoCount
            {
                get { return _infoCount; }
            }

            public int WarnCount
            {
                get { return _warnCount; }
            }

            public int ErrorCount
            {
                get { return _errorCount; }
            }

            public int FatalCount
            {
                get { return _fatalCount; }
            }

            public bool LockScroll
            {
                get { return _lockScroll; }
                set { _lockScroll = value; }
            }

            public int MaxLine
            {
                get { return _maxLine; }
                set { _maxLine = value; }
            }

            public bool InfoFilter
            {
                get { return _infoFilter; }
                set { _infoFilter = value; }
            }

            public bool WarnFilter
            {
                get { return _warnFilter; }
                set { _warnFilter = value; }
            }

            public bool ErrorFilter
            {
                get { return _errorFilter; }
                set { _errorFilter = value; }
            }

            public bool FatalFilter
            {
                get { return _fatalFilter; }
                set { _fatalFilter = value; }
            }

            public UnityColor32 InfoColor
            {
                get { return _infoColor; }
                set { _infoColor = value; }
            }

            public UnityColor32 WarnColor
            {
                get { return _warnColor; }
                set { _warnColor = value; }
            }

            public UnityColor32 ErrorColor
            {
                get { return _errorColor; }
                set { _errorColor = value; }
            }

            public UnityColor32 FatalColor
            {
                get { return _fatalColor; }
                set { _fatalColor = value; }
            }

            /// <summary>
            /// 调试器窗口初始化操作函数
            /// </summary>
            /// <param name="args">参数列表</param>
            public void Initialize(params object[] args)
            {
                IDebuggerManager debuggerManager = NovaEngine.AppEntry.GetManager<IDebuggerManager>();
                _debuggerSetting = debuggerManager.DebuggerSetting;
                if (null == _debuggerSetting)
                {
                    Debugger.Fatal("Setting component is invalid.");
                    return;
                }

                UnityApplication.logMessageReceived += OnLogMessageReceived;

                _lockScroll = _lastLockScroll = _debuggerSetting.GetBool(IDebuggerSetting.ConsoleLockScroll, true);
                _infoFilter = _lastInfoFilter = _debuggerSetting.GetBool(IDebuggerSetting.ConsoleInfoFilter, true);
                _warnFilter = _lastWarnFilter = _debuggerSetting.GetBool(IDebuggerSetting.ConsoleWarnFilter, true);
                _errorFilter = _lastErrorFilter = _debuggerSetting.GetBool(IDebuggerSetting.ConsoleErrorFilter, true);
                _fatalFilter = _lastFatalFilter = _debuggerSetting.GetBool(IDebuggerSetting.ConsoleFatalFilter, true);
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
                if (_lastLockScroll != _lockScroll)
                {
                    _lastLockScroll = _lockScroll;
                    _debuggerSetting.SetBool(IDebuggerSetting.ConsoleLockScroll, _lockScroll);
                }

                if (_lastInfoFilter != _infoFilter)
                {
                    _lastInfoFilter = _infoFilter;
                    _debuggerSetting.SetBool(IDebuggerSetting.ConsoleInfoFilter, _infoFilter);
                }

                if (_lastWarnFilter != _warnFilter)
                {
                    _lastWarnFilter = _warnFilter;
                    _debuggerSetting.SetBool(IDebuggerSetting.ConsoleWarnFilter, _warnFilter);
                }

                if (_lastErrorFilter != _errorFilter)
                {
                    _lastErrorFilter = _errorFilter;
                    _debuggerSetting.SetBool(IDebuggerSetting.ConsoleErrorFilter, _errorFilter);
                }

                if (_lastFatalFilter != _fatalFilter)
                {
                    _lastFatalFilter = _fatalFilter;
                    _debuggerSetting.SetBool(IDebuggerSetting.ConsoleFatalFilter, _fatalFilter);
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

                    _lockScroll = UnityGUILayout.Toggle(_lockScroll, "Lock Scroll", UnityGUILayout.Width(90f));
                    UnityGUILayout.FlexibleSpace();
                    _infoFilter = UnityGUILayout.Toggle(_infoFilter, NovaEngine.Utility.Text.Format("Info ({0})", _infoCount.ToString()), UnityGUILayout.Width(90f));
                    _warnFilter = UnityGUILayout.Toggle(_warnFilter, NovaEngine.Utility.Text.Format("Warn ({0})", _warnCount.ToString()), UnityGUILayout.Width(90f));
                    _errorFilter = UnityGUILayout.Toggle(_errorFilter, NovaEngine.Utility.Text.Format("Error ({0})", _errorCount.ToString()), UnityGUILayout.Width(90f));
                    _fatalFilter = UnityGUILayout.Toggle(_fatalFilter, NovaEngine.Utility.Text.Format("Fatal ({0})", _fatalCount.ToString()), UnityGUILayout.Width(90f));
                }
                UnityGUILayout.EndHorizontal();

                UnityGUILayout.BeginVertical("box");
                {
                    if (_lockScroll)
                    {
                        _logScrollPosition.y = float.MaxValue;
                    }

                    _logScrollPosition = UnityGUILayout.BeginScrollView(_logScrollPosition);
                    {
                        bool selected = false;
                        foreach (LogNode logNode in _logNodes)
                        {
                            switch (logNode.Type)
                            {
                                case UnityLogType.Log:
                                    if (!_infoFilter) { continue; }
                                    break;
                                case UnityLogType.Warning:
                                    if (!_warnFilter) { continue; }
                                    break;
                                case UnityLogType.Error:
                                    if (!_errorFilter) { continue; }
                                    break;
                                case UnityLogType.Exception:
                                    if (!_fatalFilter) { continue; }
                                    break;
                            }

                            if (UnityGUILayout.Toggle(_selectedNode == logNode, GetLogString(logNode)))
                            {
                                selected = true;
                                if (_selectedNode != logNode)
                                {
                                    _selectedNode = logNode;
                                    _stackScrollPosition = UnityVector2.zero;
                                }
                            }
                        }

                        if (!selected)
                        {
                            _selectedNode = null;
                        }
                    }
                    UnityGUILayout.EndScrollView();
                }
                UnityGUILayout.EndVertical();

                UnityGUILayout.BeginVertical("box");
                {
                    _stackScrollPosition = UnityGUILayout.BeginScrollView(_stackScrollPosition, UnityGUILayout.Height(100f));
                    {
                        if (_selectedNode != null)
                        {
                            UnityColor32 color = GetLogStringColor(_selectedNode.Type);
                            if (UnityGUILayout.Button(NovaEngine.Utility.Text.Format("",
                                    color.r.ToString("x2"), color.g.ToString("x2"), color.b.ToString("x2"), color.a.ToString("x2"),
                                    _selectedNode.Message, _selectedNode.StackTrace, System.Environment.NewLine), "label"))
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
                _logNodes.Clear();
            }

            /// <summary>
            /// 刷新日志记录的统计计数
            /// </summary>
            public void RefreshLogCount()
            {
                _infoCount = 0;
                _warnCount = 0;
                _errorCount = 0;
                _fatalCount = 0;

                foreach (LogNode logNode in _logNodes)
                {
                    switch (logNode.Type)
                    {
                        case UnityLogType.Log:
                            _infoCount++;
                            break;
                        case UnityLogType.Warning:
                            _warnCount++;
                            break;
                        case UnityLogType.Error:
                            _errorCount++;
                            break;
                        case UnityLogType.Exception:
                            _fatalCount++;
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
                foreach (LogNode logNode in _logNodes)
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

                int position = _logNodes.Count - count;
                if (position < 0)
                {
                    position = 0;
                }

                int index = 0;
                results.Clear();
                foreach (LogNode logNode in _logNodes)
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

                _logNodes.Enqueue(LogNode.Create(logType, logMessage, stackTrace));
                // 超出存储上限，移除旧的日志记录
                while (_logNodes.Count > _maxLine)
                {
                    LogNode.Release(_logNodes.Dequeue());
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
                        color = _infoColor;
                        break;
                    case UnityLogType.Warning:
                        color = _warnColor;
                        break;
                    case UnityLogType.Error:
                        color = _errorColor;
                        break;
                    case UnityLogType.Exception:
                        color = _fatalColor;
                        break;
                }

                return color;
            }
        }
    }
}
