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

using UnityRect = UnityEngine.Rect;
using UnityMathf = UnityEngine.Mathf;
using UnityGUILayout = UnityEngine.GUILayout;
using UnityScreen = UnityEngine.Screen;

namespace GameEngine.Debug
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
            private DebuggerComponent m_debuggerComponent = null;
            private IDebuggerSetting m_debuggerSetting = null;
            private float m_lastIconX = 0f;
            private float m_lastIconY = 0f;
            private float m_lastWindowX = 0f;
            private float m_lastWindowY = 0f;
            private float m_lastWindowWidth = 0f;
            private float m_lastWindowHeight = 0f;
            private float m_lastWindowScale = 0f;

            /// <summary>
            /// 设置窗口初始化操作函数
            /// </summary>
            /// <param name="args">参数列表</param>
            public override void Initialize(params object[] args)
            {
                base.Initialize(args);

                m_debuggerComponent = NovaEngine.AppEntry.GetComponent<DebuggerComponent>();
                if (null == m_debuggerComponent)
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

                m_debuggerSetting = debuggerManager.DebuggerSetting;
                if (null == m_debuggerSetting)
                {
                    Debugger.Fatal("Debugger setting is invalid.");
                    return;
                }

                m_lastIconX = m_debuggerSetting.GetFloat(IDebuggerSetting.IconX, DefaultIconRect.x);
                m_lastIconY = m_debuggerSetting.GetFloat(IDebuggerSetting.IconY, DefaultIconRect.y);
                m_lastWindowX = m_debuggerSetting.GetFloat(IDebuggerSetting.WindowX, DefaultWindowRect.x);
                m_lastWindowY = m_debuggerSetting.GetFloat(IDebuggerSetting.WindowY, DefaultWindowRect.y);
                m_lastWindowWidth = m_debuggerSetting.GetFloat(IDebuggerSetting.WindowWidth, DefaultWindowRect.width);
                m_lastWindowHeight = m_debuggerSetting.GetFloat(IDebuggerSetting.WindowHeight, DefaultWindowRect.height);
                m_lastWindowScale = m_debuggerSetting.GetFloat(IDebuggerSetting.WindowScale, DefaultWindowScale);

                m_debuggerComponent.WindowScale = m_lastWindowScale;
                m_debuggerComponent.IconRect = new UnityRect(m_lastIconX, m_lastIconY, DefaultIconRect.width, DefaultIconRect.height);
                m_debuggerComponent.WindowRect = new UnityRect(m_lastWindowX, m_lastWindowY, m_lastWindowWidth, m_lastWindowHeight);
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
                if (m_lastIconX != m_debuggerComponent.IconRect.x)
                {
                    m_lastIconX = m_debuggerComponent.IconRect.x;
                    m_debuggerSetting.SetFloat(IDebuggerSetting.IconX, m_lastIconX);
                }

                if (m_lastIconY != m_debuggerComponent.IconRect.y)
                {
                    m_lastIconY = m_debuggerComponent.IconRect.y;
                    m_debuggerSetting.SetFloat(IDebuggerSetting.IconY, m_lastIconY);
                }

                if (m_lastWindowX != m_debuggerComponent.WindowRect.x)
                {
                    m_lastWindowX = m_debuggerComponent.WindowRect.x;
                    m_debuggerSetting.SetFloat(IDebuggerSetting.WindowX, m_lastWindowX);
                }

                if (m_lastWindowY != m_debuggerComponent.WindowRect.y)
                {
                    m_lastWindowY = m_debuggerComponent.WindowRect.y;
                    m_debuggerSetting.SetFloat(IDebuggerSetting.WindowY, m_lastWindowY);
                }

                if (m_lastWindowWidth != m_debuggerComponent.WindowRect.width)
                {
                    m_lastWindowWidth = m_debuggerComponent.WindowRect.width;
                    m_debuggerSetting.SetFloat(IDebuggerSetting.WindowWidth, m_lastWindowWidth);
                }

                if (m_lastWindowHeight != m_debuggerComponent.WindowRect.height)
                {
                    m_lastWindowHeight = m_debuggerComponent.WindowRect.height;
                    m_debuggerSetting.SetFloat(IDebuggerSetting.WindowHeight, m_lastWindowHeight);
                }

                if (m_lastWindowScale != m_debuggerComponent.WindowScale)
                {
                    m_lastWindowScale = m_debuggerComponent.WindowScale;
                    m_debuggerSetting.SetFloat(IDebuggerSetting.WindowScale, m_lastWindowScale);
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
                        float width = m_debuggerComponent.WindowRect.width;
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
                        if (width != m_debuggerComponent.WindowRect.width)
                        {
                            m_debuggerComponent.WindowRect = new UnityRect(m_debuggerComponent.WindowRect.x,
                                                                           m_debuggerComponent.WindowRect.y,
                                                                           width,
                                                                           m_debuggerComponent.WindowRect.height);
                        }
                    }
                    UnityGUILayout.EndHorizontal();

                    UnityGUILayout.BeginHorizontal();
                    {
                        float height = m_debuggerComponent.WindowRect.height;
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
                        if (height != m_debuggerComponent.WindowRect.height)
                        {
                            m_debuggerComponent.WindowRect = new UnityRect(m_debuggerComponent.WindowRect.x,
                                                                           m_debuggerComponent.WindowRect.y,
                                                                           m_debuggerComponent.WindowRect.width,
                                                                           height);
                        }
                    }
                    UnityGUILayout.EndHorizontal();

                    UnityGUILayout.BeginHorizontal();
                    {
                        float scale = m_debuggerComponent.WindowScale;
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
                        if (scale != m_debuggerComponent.WindowScale)
                        {
                            m_debuggerComponent.WindowScale = scale;
                        }
                    }
                    UnityGUILayout.EndHorizontal();

                    UnityGUILayout.BeginHorizontal();
                    {
                        if (UnityGUILayout.Button("0.5x", UnityGUILayout.Height(60f)))
                        {
                            m_debuggerComponent.WindowScale = 0.5f;
                        }
                        if (UnityGUILayout.Button("1.0x", UnityGUILayout.Height(60f)))
                        {
                            m_debuggerComponent.WindowScale = 1f;
                        }
                        if (UnityGUILayout.Button("1.5x", UnityGUILayout.Height(60f)))
                        {
                            m_debuggerComponent.WindowScale = 1.5f;
                        }
                        if (UnityGUILayout.Button("2.0x", UnityGUILayout.Height(60f)))
                        {
                            m_debuggerComponent.WindowScale = 2f;
                        }
                        if (UnityGUILayout.Button("2.5x", UnityGUILayout.Height(60f)))
                        {
                            m_debuggerComponent.WindowScale = 2.5f;
                        }
                        if (UnityGUILayout.Button("3.0x", UnityGUILayout.Height(60f)))
                        {
                            m_debuggerComponent.WindowScale = 3f;
                        }
                        if (UnityGUILayout.Button("3.5x", UnityGUILayout.Height(60f)))
                        {
                            m_debuggerComponent.WindowScale = 3.5f;
                        }
                        if (UnityGUILayout.Button("4.0x", UnityGUILayout.Height(60f)))
                        {
                            m_debuggerComponent.WindowScale = 4f;
                        }
                    }
                    UnityGUILayout.EndHorizontal();

                    if (UnityGUILayout.Button("Reset Layout", UnityGUILayout.Height(30f)))
                    {
                        m_debuggerComponent.ResetLayout();
                    }
                }
                UnityGUILayout.EndVertical();
            }
        }
    }
}
