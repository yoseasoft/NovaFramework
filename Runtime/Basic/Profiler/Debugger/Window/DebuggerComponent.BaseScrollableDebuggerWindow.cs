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

using UnityVector2 = UnityEngine.Vector2;
using UnityGUILayout = UnityEngine.GUILayout;

namespace GameEngine.Profiler.Debugging
{
    /// <summary>
    /// 游戏调试器组件对象类，用于定义调试器对象的基础属性及访问操作函数
    /// </summary>
    public sealed partial class DebuggerComponent
    {
        /// <summary>
        /// 可滚动的调试窗口对象的通用基类定义，此处声明一个通用组件，在其它任何地方需要使用到滚动窗口时，可继承该组件类
        /// </summary>
        public abstract class BaseScrollableDebuggerWindow : IDebuggerWindow
        {
            private const float TITLE_WIDTH = 240f;

            /// <summary>
            /// 当前窗口的滚动位置
            /// </summary>
            private UnityVector2 _scrollPosition = UnityVector2.zero;

            /// <summary>
            /// 调试器窗口初始化操作函数
            /// </summary>
            /// <param name="args">参数列表</param>
            public virtual void Initialize(params object[] args)
            {
            }

            /// <summary>
            /// 调试器窗口清理操作函数
            /// </summary>
            public virtual void Cleanup()
            {
            }

            /// <summary>
            /// 进入调试器窗口
            /// </summary>
            public virtual void OnEnter()
            {
            }

            /// <summary>
            /// 退出调试器窗口
            /// </summary>
            public virtual void OnExit()
            {
            }

            /// <summary>
            /// 调试器窗口轮询刷新函数
            /// </summary>
            /// <param name="elapseSeconds">逻辑流逝时间，以秒为单位</param>
            /// <param name="realElapseSeconds">真实流逝时间，以秒为单位</param>
            public virtual void OnUpdate(float elapseSeconds, float realElapseSeconds)
            {
            }

            /// <summary>
            /// 调试器窗口绘制函数
            /// </summary>
            public void OnDraw()
            {
                _scrollPosition = UnityGUILayout.BeginScrollView(_scrollPosition);
                {
                    OnDrawScrollableWindow();
                }
                UnityGUILayout.EndScrollView();
            }

            /// <summary>
            /// 滚动窗口绘制的回调函数<br/>
            /// 该函数将在默认绘制函数中自动进行调用，请勿自行手动调佣
            /// </summary>
            protected abstract void OnDrawScrollableWindow();

            protected static void DrawItem(string title, string content)
            {
                UnityGUILayout.BeginHorizontal();
                {
                    UnityGUILayout.Label(title, UnityGUILayout.Width(TITLE_WIDTH));
                    if (UnityGUILayout.Button(content, "label"))
                    {
                        CopyToClipboard(content);
                    }
                }
                UnityGUILayout.EndHorizontal();
            }

            /// <summary>
            /// 获取字节长度对应的字符串显示信息<br/>
            /// 转换的值截取小数点后两位进行显示，最大支持到“EB”
            /// </summary>
            /// <param name="byteLength">字节长度</param>
            /// <returns>返回字节长度对应的字符串显示信息</returns>
            protected static string GetByteLengthString(long byteLength)
            {
                if (byteLength < 1024L) // 2 ^ 10
                {
                    return NovaEngine.Utility.Text.Format("{0} Bytes", byteLength.ToString());
                }

                if (byteLength < 1048576L) // 2 ^ 20
                {
                    return NovaEngine.Utility.Text.Format("{0} KB", (byteLength / 1024f).ToString("F2"));
                }

                if (byteLength < 1073741824L) // 2 ^ 30
                {
                    return NovaEngine.Utility.Text.Format("{0} MB", (byteLength / 1048576f).ToString("F2"));
                }

                if (byteLength < 1099511627776L) // 2 ^ 40
                {
                    return NovaEngine.Utility.Text.Format("{0} GB", (byteLength / 1073741824f).ToString("F2"));
                }

                if (byteLength < 1125899906842624L) // 2 ^ 50
                {
                    return NovaEngine.Utility.Text.Format("{0} TB", (byteLength / 1099511627776f).ToString("F2"));
                }

                if (byteLength < 1152921504606846976L) // 2 ^ 60
                {
                    return NovaEngine.Utility.Text.Format("{0} PB", (byteLength / 1125899906842624f).ToString("F2"));
                }

                return NovaEngine.Utility.Text.Format("{0} EB", (byteLength / 1152921504606846976f).ToString("F2"));
            }
        }
    }
}
