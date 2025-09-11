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

using UnityRect = UnityEngine.Rect;
using UnityMathf = UnityEngine.Mathf;
using UnityGUILayout = UnityEngine.GUILayout;
using UnityScreen = UnityEngine.Screen;

namespace GameEngine.Profiler.Debugging
{
    /// <summary>
    /// 游戏调试器组件对象类，用于定义调试器对象的基础属性及访问操作函数
    /// </summary>
    public sealed partial class DebuggerComponent
    {
        /// <summary>
        /// 设置信息展示窗口的对象类
        /// </summary>
        private sealed class SettingsWindow : BaseScrollableDebuggerWindow
        {
            private DebuggerComponent _debuggerComponent = null;
            private IDebuggerSetting _debuggerSetting = null;
            private float _lastIconX = 0f;
            private float _lastIconY = 0f;
            private float _lastWindowX = 0f;
            private float _lastWindowY = 0f;
            private float _lastWindowWidth = 0f;
            private float _lastWindowHeight = 0f;
            private float _lastWindowScale = 0f;

            /// <summary>
            /// 设置窗口初始化操作函数
            /// </summary>
            /// <param name="args">参数列表</param>
            public override void Initialize(params object[] args)
            {
                base.Initialize(args);

                _debuggerComponent = NovaEngine.AppEntry.GetComponent<DebuggerComponent>();
                if (null == _debuggerComponent)
                {
                    Debugger.Fatal("Debugger component is invalid.");
                    return;
                }

                IDebuggerManager debuggerManager = NovaEngine.AppEntry.GetManager<IDebuggerManager>();
                if (null == debuggerManager)
                {
                    Debugger.Fatal("Debugger manager is invalid.");
                    return;
                }

                _debuggerSetting = debuggerManager.DebuggerSetting;
                if (null == _debuggerSetting)
                {
                    Debugger.Fatal("Debugger setting is invalid.");
                    return;
                }

                _lastIconX = _debuggerSetting.GetFloat(IDebuggerSetting.IconX, DefaultIconRect.x);
                _lastIconY = _debuggerSetting.GetFloat(IDebuggerSetting.IconY, DefaultIconRect.y);
                _lastWindowX = _debuggerSetting.GetFloat(IDebuggerSetting.WindowX, DefaultWindowRect.x);
                _lastWindowY = _debuggerSetting.GetFloat(IDebuggerSetting.WindowY, DefaultWindowRect.y);
                _lastWindowWidth = _debuggerSetting.GetFloat(IDebuggerSetting.WindowWidth, DefaultWindowRect.width);
                _lastWindowHeight = _debuggerSetting.GetFloat(IDebuggerSetting.WindowHeight, DefaultWindowRect.height);
                _lastWindowScale = _debuggerSetting.GetFloat(IDebuggerSetting.WindowScale, DefaultWindowScale);

                _debuggerComponent.WindowScale = _lastWindowScale;
                _debuggerComponent.IconRect = new UnityRect(_lastIconX, _lastIconY, DefaultIconRect.width, DefaultIconRect.height);
                _debuggerComponent.WindowRect = new UnityRect(_lastWindowX, _lastWindowY, _lastWindowWidth, _lastWindowHeight);
            }

            /// <summary>
            /// 设置窗口清理操作函数
            /// </summary>
            public override void Cleanup()
            {
                base.Cleanup();
            }

            /// <summary>
            /// 设置窗口轮询刷新函数
            /// </summary>
            /// <param name="elapseSeconds">逻辑流逝时间，以秒为单位</param>
            /// <param name="realElapseSeconds">真实流逝时间，以秒为单位</param>
            public override void OnUpdate(float elapseSeconds, float realElapseSeconds)
            {
                if (_lastIconX != _debuggerComponent.IconRect.x)
                {
                    _lastIconX = _debuggerComponent.IconRect.x;
                    _debuggerSetting.SetFloat(IDebuggerSetting.IconX, _lastIconX);
                }

                if (_lastIconY != _debuggerComponent.IconRect.y)
                {
                    _lastIconY = _debuggerComponent.IconRect.y;
                    _debuggerSetting.SetFloat(IDebuggerSetting.IconY, _lastIconY);
                }

                if (_lastWindowX != _debuggerComponent.WindowRect.x)
                {
                    _lastWindowX = _debuggerComponent.WindowRect.x;
                    _debuggerSetting.SetFloat(IDebuggerSetting.WindowX, _lastWindowX);
                }

                if (_lastWindowY != _debuggerComponent.WindowRect.y)
                {
                    _lastWindowY = _debuggerComponent.WindowRect.y;
                    _debuggerSetting.SetFloat(IDebuggerSetting.WindowY, _lastWindowY);
                }

                if (_lastWindowWidth != _debuggerComponent.WindowRect.width)
                {
                    _lastWindowWidth = _debuggerComponent.WindowRect.width;
                    _debuggerSetting.SetFloat(IDebuggerSetting.WindowWidth, _lastWindowWidth);
                }

                if (_lastWindowHeight != _debuggerComponent.WindowRect.height)
                {
                    _lastWindowHeight = _debuggerComponent.WindowRect.height;
                    _debuggerSetting.SetFloat(IDebuggerSetting.WindowHeight, _lastWindowHeight);
                }

                if (_lastWindowScale != _debuggerComponent.WindowScale)
                {
                    _lastWindowScale = _debuggerComponent.WindowScale;
                    _debuggerSetting.SetFloat(IDebuggerSetting.WindowScale, _lastWindowScale);
                }
            }

            protected override void OnDrawScrollableWindow()
            {
                UnityGUILayout.Label("<b>Window Settings</b>");
                UnityGUILayout.BeginVertical("box");
                {
                    UnityGUILayout.BeginHorizontal();
                    {
                        UnityGUILayout.Label("Position:", UnityGUILayout.Width(60f));
                        UnityGUILayout.Label("Drag window caption to move position.");
                    }
                    UnityGUILayout.EndHorizontal();

                    UnityGUILayout.BeginHorizontal();
                    {
                        float width = _debuggerComponent.WindowRect.width;
                        UnityGUILayout.Label("Width:", UnityGUILayout.Width(60f));
                        if (UnityGUILayout.RepeatButton("-", UnityGUILayout.Width(30f)))
                        {
                            width--;
                        }
                        width = UnityGUILayout.HorizontalSlider(width, 100f, UnityScreen.width - 20f);
                        if (UnityGUILayout.RepeatButton("+", UnityGUILayout.Width(30f)))
                        {
                            width++;
                        }
                        width = UnityMathf.Clamp(width, 100f, UnityScreen.width - 20f);
                        if (width != _debuggerComponent.WindowRect.width)
                        {
                            _debuggerComponent.WindowRect = new UnityRect(_debuggerComponent.WindowRect.x,
                                                                           _debuggerComponent.WindowRect.y,
                                                                           width,
                                                                           _debuggerComponent.WindowRect.height);
                        }
                    }
                    UnityGUILayout.EndHorizontal();

                    UnityGUILayout.BeginHorizontal();
                    {
                        float height = _debuggerComponent.WindowRect.height;
                        UnityGUILayout.Label("Height:", UnityGUILayout.Width(60f));
                        if (UnityGUILayout.RepeatButton("-", UnityGUILayout.Width(30f)))
                        {
                            height--;
                        }
                        height = UnityGUILayout.HorizontalSlider(height, 100f, UnityScreen.height - 20f);
                        if (UnityGUILayout.RepeatButton("+", UnityGUILayout.Width(30f)))
                        {
                            height++;
                        }
                        height = UnityMathf.Clamp(height, 100f, UnityScreen.height - 20f);
                        if (height != _debuggerComponent.WindowRect.height)
                        {
                            _debuggerComponent.WindowRect = new UnityRect(_debuggerComponent.WindowRect.x,
                                                                           _debuggerComponent.WindowRect.y,
                                                                           _debuggerComponent.WindowRect.width,
                                                                           height);
                        }
                    }
                    UnityGUILayout.EndHorizontal();

                    UnityGUILayout.BeginHorizontal();
                    {
                        float scale = _debuggerComponent.WindowScale;
                        UnityGUILayout.Label("Scale:", UnityGUILayout.Width(60f));
                        if (UnityGUILayout.RepeatButton("-", UnityGUILayout.Width(30f)))
                        {
                            scale -= 0.01f;
                        }
                        scale = UnityGUILayout.HorizontalSlider(scale, 0.5f, 4f);
                        if (UnityGUILayout.RepeatButton("+", UnityGUILayout.Width(30f)))
                        {
                            scale += 0.01f;
                        }
                        scale = UnityMathf.Clamp(scale, 0.5f, 4f);
                        if (scale != _debuggerComponent.WindowScale)
                        {
                            _debuggerComponent.WindowScale = scale;
                        }
                    }
                    UnityGUILayout.EndHorizontal();

                    UnityGUILayout.BeginHorizontal();
                    {
                        if (UnityGUILayout.Button("0.5x", UnityGUILayout.Height(60f)))
                        {
                            _debuggerComponent.WindowScale = 0.5f;
                        }
                        if (UnityGUILayout.Button("1.0x", UnityGUILayout.Height(60f)))
                        {
                            _debuggerComponent.WindowScale = 1f;
                        }
                        if (UnityGUILayout.Button("1.5x", UnityGUILayout.Height(60f)))
                        {
                            _debuggerComponent.WindowScale = 1.5f;
                        }
                        if (UnityGUILayout.Button("2.0x", UnityGUILayout.Height(60f)))
                        {
                            _debuggerComponent.WindowScale = 2f;
                        }
                        if (UnityGUILayout.Button("2.5x", UnityGUILayout.Height(60f)))
                        {
                            _debuggerComponent.WindowScale = 2.5f;
                        }
                        if (UnityGUILayout.Button("3.0x", UnityGUILayout.Height(60f)))
                        {
                            _debuggerComponent.WindowScale = 3f;
                        }
                        if (UnityGUILayout.Button("3.5x", UnityGUILayout.Height(60f)))
                        {
                            _debuggerComponent.WindowScale = 3.5f;
                        }
                        if (UnityGUILayout.Button("4.0x", UnityGUILayout.Height(60f)))
                        {
                            _debuggerComponent.WindowScale = 4f;
                        }
                    }
                    UnityGUILayout.EndHorizontal();

                    if (UnityGUILayout.Button("Reset Layout", UnityGUILayout.Height(30f)))
                    {
                        _debuggerComponent.ResetLayout();
                    }
                }
                UnityGUILayout.EndVertical();
            }
        }
    }
}
