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

using UnityGUILayout = UnityEngine.GUILayout;
using UnityInput = UnityEngine.Input;
using UnityTouch = UnityEngine.Touch;

namespace GameEngine.Debug
{
    /// <summary>
    /// 游戏调试器组件对象类，用于定义调试器对象的基础属性及访问操作函数
    /// </summary>
    public sealed partial class DebuggerComponent
    {
        /// <summary>
        /// 输入点击信息展示窗口的对象类
        /// </summary>
        private sealed class InputTouchInformationWindow : BaseScrollableDebuggerWindow
        {
            protected override void OnDrawScrollableWindow()
            {
                UnityGUILayout.Label("<b>Input Touch Information</b>");
                UnityGUILayout.BeginVertical("box");
                {
                    DrawItem("Touch Supported", UnityInput.touchSupported.ToString());
                    DrawItem("Touch Pressure Supported", UnityInput.touchPressureSupported.ToString());
                    DrawItem("Stylus Touch Supported", UnityInput.stylusTouchSupported.ToString());
                    DrawItem("Simulate Mouse With Touches", UnityInput.simulateMouseWithTouches.ToString());
                    DrawItem("Multi Touch Enabled", UnityInput.multiTouchEnabled.ToString());
                    DrawItem("Touch Count", UnityInput.touchCount.ToString());
                    DrawItem("Touches", GetTouchesString(UnityInput.touches));
                }
                UnityGUILayout.EndVertical();
            }

            private string GetTouchString(UnityTouch touch)
            {
                return NovaEngine.Utility.Text.Format("{0}, {1}, {2}, {3}, {4}",
                                                      touch.position.ToString(), touch.deltaPosition.ToString(),
                                                      touch.rawPosition.ToString(), touch.pressure.ToString(),
                                                      touch.phase.ToString());
            }

            private string GetTouchesString(UnityTouch[] touches)
            {
                string[] touchStrings = new string[touches.Length];
                for (int n = 0; n < touches.Length; ++n)
                {
                    touchStrings[n] = GetTouchString(touches[n]);
                }

                return string.Join("; ", touchStrings);
            }
        }
    }
}
