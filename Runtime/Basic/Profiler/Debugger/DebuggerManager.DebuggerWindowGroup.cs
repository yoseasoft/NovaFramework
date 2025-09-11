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

namespace GameEngine.Profiler.Debugging
{
    /// <summary>
    /// 调试管理器的对象实现类，对接口函数进行具体逻辑的实现
    /// </summary>
    internal sealed partial class DebuggerManager
    {
        /// <summary>
        /// 调试器窗口组的对象实现类，对窗口组操作函数进行具体逻辑的实现
        /// </summary>
        private sealed class DebuggerWindowGroup : IDebuggerWindowGroup
        {
            private readonly List<KeyValuePair<string, IDebuggerWindow>> _debuggerWindows;
            private int _selectedIndex;
            private string[] _debuggerWindowNames;

            /// <summary>
            /// 获取调试器窗口的数量
            /// </summary>
            public int DebuggerWindowCount { get { return _debuggerWindows.Count; } }

            /// <summary>
            /// 获取或设置当前选中的调试器窗口的索引
            /// </summary>
            public int SelectedIndex
            {
                get { return _selectedIndex; }
                set { _selectedIndex = value; }
            }

            /// <summary>
            /// 获取当前选中的调试器窗口
            /// </summary>
            public IDebuggerWindow SelectedWindow
            {
                get
                {
                    if (_selectedIndex >= _debuggerWindows.Count || _selectedIndex < 0)
                    {
                        return null;
                    }

                    return _debuggerWindows[_selectedIndex].Value;
                }
            }

            public DebuggerWindowGroup()
            {
                _debuggerWindows = new List<KeyValuePair<string, IDebuggerWindow>>();
                _selectedIndex = 0;
                _debuggerWindowNames = null;
            }

            /// <summary>
            /// 调试器窗口组的初始化操作函数
            /// </summary>
            /// <param name="args">参数列表</param>
            public void Initialize(params object[] args)
            {
            }

            /// <summary>
            /// 调试器窗口组的清理操作函数
            /// </summary>
            public void Cleanup()
            {
                foreach (KeyValuePair<string, IDebuggerWindow> debuggerWindow in _debuggerWindows)
                {
                    debuggerWindow.Value.Cleanup();
                }

                _debuggerWindows.Clear();
            }

            /// <summary>
            /// 进入调试器窗口
            /// </summary>
            public void OnEnter()
            {
                SelectedWindow.OnEnter();
            }

            /// <summary>
            /// 退出调试器窗口
            /// </summary>
            public void OnExit()
            {
                SelectedWindow.OnExit();
            }

            /// <summary>
            /// 调试器窗口轮询刷新函数
            /// </summary>
            /// <param name="elapseSeconds">逻辑流逝时间，以秒为单位</param>
            /// <param name="realElapseSeconds">真实流逝时间，以秒为单位</param>
            public void OnUpdate(float elapseSeconds, float realElapseSeconds)
            {
                SelectedWindow.OnUpdate(elapseSeconds, realElapseSeconds);
            }

            /// <summary>
            /// 调试器窗口绘制函数
            /// </summary>
            public void OnDraw()
            {
            }

            /// <summary>
            /// 刷新当前记录的全部调试器窗口的名称
            /// </summary>
            private void RefreshDebuggerWindowNames()
            {
                _debuggerWindowNames = new string[_debuggerWindows.Count];
                for (int n = 0; n < _debuggerWindows.Count; ++n)
                {
                    _debuggerWindowNames[n] = _debuggerWindows[n].Key;
                }
            }

            /// <summary>
            /// 获取该调试组的所有调试器窗口的名称集合
            /// </summary>
            /// <returns>返回调试器窗口名称的集合，若没有则返回空</returns>
            public string[] GetAllDebuggerWindowNames()
            {
                return _debuggerWindowNames;
            }

            /// <summary>
            /// 获取指定路径对应的调试器窗口实例
            /// </summary>
            /// <param name="path">窗口路径</param>
            /// <returns>返回路径对应的窗口实例，若不存在则返回null</returns>
            public IDebuggerWindow GetDebuggerWindow(string path)
            {
                if (string.IsNullOrEmpty(path))
                {
                    return null;
                }

                int pos = path.IndexOf(NovaEngine.Definition.CCharacter.Slash);
                if (pos < 0 || pos >= path.Length - 1)
                {
                    return InternalGetDebuggerWindow(path);
                }

                string debuggerWindowGroupName = path.Substring(0, pos);
                string leftPath = path.Substring(pos + 1);
                DebuggerWindowGroup debuggerWindowGroup = (DebuggerWindowGroup) InternalGetDebuggerWindow(debuggerWindowGroupName);
                if (null == debuggerWindowGroup)
                {
                    return null;
                }

                return debuggerWindowGroup.GetDebuggerWindow(leftPath);
            }

            /// <summary>
            /// 选中指定路径对应的调试器窗口实例
            /// </summary>
            /// <param name="path">窗口路径</param>
            /// <returns>若选择窗口实例成功则返回true，否则返回false</returns>
            public bool SelectDebuggerWindow(string path)
            {
                if (string.IsNullOrEmpty(path))
                {
                    return false;
                }

                int pos = path.IndexOf(NovaEngine.Definition.CCharacter.Slash);
                if (pos < 0 || pos >= path.Length - 1)
                {
                    return InternalSelectDebuggerWindow(path);
                }

                string debuggerWindowGroupName = path.Substring(0, pos);
                string leftPath = path.Substring(pos + 1);
                DebuggerWindowGroup debuggerWindowGroup = (DebuggerWindowGroup) InternalGetDebuggerWindow(debuggerWindowGroupName);
                if (null == debuggerWindowGroup || !InternalSelectDebuggerWindow(debuggerWindowGroupName))
                {
                    return false;
                }

                return debuggerWindowGroup.SelectDebuggerWindow(leftPath);
            }

            /// <summary>
            /// 注册指定的路径和窗口对象实例到当前的调试组中
            /// </summary>
            /// <param name="path">窗口路径</param>
            /// <param name="debuggerWindow">调试窗口实例</param>
            public void RegisterDebuggerWindow(string path, IDebuggerWindow debuggerWindow)
            {
                if (string.IsNullOrEmpty(path))
                {
                    Debugger.Error("The path is invalid.");
                    return;
                }

                int pos = path.IndexOf(NovaEngine.Definition.CCharacter.Slash);
                if (pos < 0 || pos >= path.Length - 1)
                {
                    if (null != InternalGetDebuggerWindow(path))
                    {
                        throw new NovaEngine.CFrameworkException("Debugger window has been registered.");
                    }

                    _debuggerWindows.Add(new KeyValuePair<string, IDebuggerWindow>(path, debuggerWindow));
                    RefreshDebuggerWindowNames();
                }
                else
                {
                    string debuggerWindowGroupName = path.Substring(0, pos);
                    string leftPath = path.Substring(pos + 1);
                    DebuggerWindowGroup debuggerWindowGroup = (DebuggerWindowGroup) InternalGetDebuggerWindow(debuggerWindowGroupName);
                    if (null == debuggerWindowGroup)
                    {
                        if (null != InternalGetDebuggerWindow(debuggerWindowGroupName))
                        {
                            throw new NovaEngine.CFrameworkException("Debugger window has been registered, cannot create debugger window group.");
                        }

                        debuggerWindowGroup = new DebuggerWindowGroup();
                        _debuggerWindows.Add(new KeyValuePair<string, IDebuggerWindow>(debuggerWindowGroupName, debuggerWindowGroup));
                        RefreshDebuggerWindowNames();
                    }

                    debuggerWindowGroup.RegisterDebuggerWindow(leftPath, debuggerWindow);
                }
            }

            /// <summary>
            /// 从当前的调试组中注销指定路径对应的窗口对象实例
            /// </summary>
            /// <param name="path">窗口路径</param>
            /// <returns>若注销窗口实例成功返回true，否则返回false</returns>
            public bool UnregisterDebuggerWindow(string path)
            {
                if (string.IsNullOrEmpty(path))
                {
                    return false;
                }

                int pos = path.IndexOf(NovaEngine.Definition.CCharacter.Slash);
                if (pos < 0 || pos >= path.Length - 1)
                {
                    IDebuggerWindow debuggerWindow = InternalGetDebuggerWindow(path);
                    bool result = _debuggerWindows.Remove(new KeyValuePair<string, IDebuggerWindow>(path, debuggerWindow));
                    debuggerWindow.Cleanup();
                    RefreshDebuggerWindowNames();
                    return result;
                }

                string debuggerWindowGroupName = path.Substring(0, pos);
                string leftPath = path.Substring(pos + 1);
                DebuggerWindowGroup debuggerWindowGroup = (DebuggerWindowGroup) InternalGetDebuggerWindow(debuggerWindowGroupName);
                if (null == debuggerWindowGroup)
                {
                    return false;
                }

                return debuggerWindowGroup.UnregisterDebuggerWindow(leftPath);
            }

            private IDebuggerWindow InternalGetDebuggerWindow(string name)
            {
                foreach (KeyValuePair<string, IDebuggerWindow> debuggerWindow in _debuggerWindows)
                {
                    if (debuggerWindow.Key == name)
                    {
                        return debuggerWindow.Value;
                    }
                }

                return null;
            }

            private bool InternalSelectDebuggerWindow(string name)
            {
                for (int n = 0; n < _debuggerWindows.Count; ++n)
                {
                    if (_debuggerWindows[n].Key == name)
                    {
                        _selectedIndex = n;
                        return true;
                    }
                }

                return false;
            }
        }
    }
}
