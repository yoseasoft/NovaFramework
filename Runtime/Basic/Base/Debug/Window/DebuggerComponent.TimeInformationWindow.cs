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

using UnityTime = UnityEngine.Time;
using UnityGUILayout = UnityEngine.GUILayout;

namespace GameEngine.Debug
{
    /// <summary>
    /// 游戏调试器组件对象类，用于定义调试器对象的基础属性及访问操作函数
    /// </summary>
    public sealed partial class DebuggerComponent
    {
        /// <summary>
        /// 计时信息展示窗口的对象类
        /// </summary>
        private sealed class TimeInformationWindow : BaseScrollableDebuggerWindow
        {
            protected override void OnDrawScrollableWindow()
            {
                UnityGUILayout.Label("<b>Time Information</b>");
                UnityGUILayout.BeginVertical("box");
                {
                    DrawItem("Time Scale", NovaEngine.Utility.Text.Format("{0} [{1}]", UnityTime.timeScale.ToString(), GetTimeScaleDescription(UnityTime.timeScale)));
                    DrawItem("Realtime Since Startup", UnityTime.realtimeSinceStartup.ToString());
                    DrawItem("Time Since Level Load", UnityTime.timeSinceLevelLoad.ToString());
                    DrawItem("Time", UnityTime.time.ToString());
                    DrawItem("Fixed Time", UnityTime.fixedTime.ToString());
                    DrawItem("Unscaled Time", UnityTime.unscaledTime.ToString());
                    DrawItem("Fixed Unscaled Time", UnityTime.fixedUnscaledTime.ToString());
                    DrawItem("Delta Time", UnityTime.deltaTime.ToString());
                    DrawItem("Fixed Delta Time", UnityTime.fixedDeltaTime.ToString());
                    DrawItem("Unscaled Delta Time", UnityTime.unscaledDeltaTime.ToString());
                    DrawItem("Fixed Unscaled Delta Time", UnityTime.fixedUnscaledDeltaTime.ToString());
                    DrawItem("Smooth Delta Time", UnityTime.smoothDeltaTime.ToString());
                    DrawItem("Maximum Delta Time", UnityTime.maximumDeltaTime.ToString());
                    DrawItem("Maximum Particle Delta Time", UnityTime.maximumParticleDeltaTime.ToString());
                    DrawItem("Frame Count", UnityTime.frameCount.ToString());
                    DrawItem("Rendered Frame Count", UnityTime.renderedFrameCount.ToString());
                    DrawItem("Capture Framerate", UnityTime.captureFramerate.ToString());
                    DrawItem("Capture Delta Time", UnityTime.captureDeltaTime.ToString());
                    DrawItem("In Fixed Time Step", UnityTime.inFixedTimeStep.ToString());
                }
                UnityGUILayout.EndVertical();
            }

            private string GetTimeScaleDescription(float timeScale)
            {
                if (timeScale <= 0f)
                {
                    return "Pause";
                }

                if (timeScale < 1f)
                {
                    return "Slower";
                }

                if (timeScale > 1f)
                {
                    return "Faster";
                }

                return "Normal";
            }
        }
    }
}
