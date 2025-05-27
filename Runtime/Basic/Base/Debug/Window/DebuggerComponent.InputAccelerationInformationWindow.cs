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

using UnityAccelerationEvent = UnityEngine.AccelerationEvent;

using UnityGUILayout = UnityEngine.GUILayout;
using UnityInput = UnityEngine.Input;

namespace GameEngine.Debug
{
    /// <summary>
    /// 游戏调试器组件对象类，用于定义调试器对象的基础属性及访问操作函数
    /// </summary>
    public sealed partial class DebuggerComponent
    {
        /// <summary>
        /// 输入加速器信息展示窗口的对象类
        /// </summary>
        private sealed class InputAccelerationInformationWindow : BaseScrollableDebuggerWindow
        {
            protected override void OnDrawScrollableWindow()
            {
                UnityGUILayout.Label("<b>Input Acceleration Information</b>");
                UnityGUILayout.BeginVertical("box");
                {
                    DrawItem("Acceleration", UnityInput.acceleration.ToString());
                    DrawItem("Acceleration Event Count", UnityInput.accelerationEventCount.ToString());
                    DrawItem("Acceleration Events", GetAccelerationEventsString(UnityInput.accelerationEvents));
                }
                UnityGUILayout.EndVertical();
            }

            private string GetAccelerationEventString(UnityAccelerationEvent accelerationEvent)
            {
                return NovaEngine.Utility.Text.Format("{0}, {1}",
                                                      accelerationEvent.acceleration.ToString(),
                                                      accelerationEvent.deltaTime.ToString());
            }

            private string GetAccelerationEventsString(UnityAccelerationEvent[] accelerationEvents)
            {
                string[] accelerationEventStrings = new string[accelerationEvents.Length];
                for (int n = 0; n < accelerationEvents.Length; ++n)
                {
                    accelerationEventStrings[n] = GetAccelerationEventString(accelerationEvents[n]);
                }

                return string.Join("; ", accelerationEventStrings);
            }
        }
    }
}
