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

using UnityLocationServiceStatus = UnityEngine.LocationServiceStatus;
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
        /// 输入位置信息展示窗口的对象类
        /// </summary>
        private sealed class InputLocationInformationWindow : BaseScrollableDebuggerWindow
        {
            protected override void OnDrawScrollableWindow()
            {
                UnityGUILayout.Label("<b>Input Location Information</b>");
                UnityGUILayout.BeginVertical("box");
                {
                    UnityGUILayout.BeginHorizontal();
                    {
                        if (UnityGUILayout.Button("Enable", UnityGUILayout.Height(30f)))
                        {
                            UnityInput.location.Start();
                        }
                        if (UnityGUILayout.Button("Disable", UnityGUILayout.Height(30f)))
                        {
                            UnityInput.location.Stop();
                        }
                    }
                    UnityGUILayout.EndHorizontal();

                    DrawItem("Is Enabled By User", UnityInput.location.isEnabledByUser.ToString());
                    DrawItem("Status", UnityInput.location.status.ToString());
                    if (UnityInput.location.status == UnityLocationServiceStatus.Running)
                    {
                        DrawItem("Horizontal Accuracy", UnityInput.location.lastData.horizontalAccuracy.ToString());
                        DrawItem("Vertical Accuracy", UnityInput.location.lastData.verticalAccuracy.ToString());
                        DrawItem("Longitude", UnityInput.location.lastData.longitude.ToString());
                        DrawItem("Latitude", UnityInput.location.lastData.latitude.ToString());
                        DrawItem("Altitude", UnityInput.location.lastData.altitude.ToString());
                        DrawItem("Timestamp", UnityInput.location.lastData.timestamp.ToString());
                    }
                }
                UnityGUILayout.EndVertical();
            }
        }
    }
}
