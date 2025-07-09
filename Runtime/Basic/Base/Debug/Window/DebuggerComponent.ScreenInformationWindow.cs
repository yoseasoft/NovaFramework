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

using UnitySleepTimeout = UnityEngine.SleepTimeout;
using UnityResolution = UnityEngine.Resolution;
using UnityRect = UnityEngine.Rect;
using UnityGUILayout = UnityEngine.GUILayout;
using UnityScreen = UnityEngine.Screen;
using UnityCursor = UnityEngine.Cursor;

namespace GameEngine.Debug
{
    /// <summary>
    /// 游戏调试器组件对象类，用于定义调试器对象的基础属性及访问操作函数
    /// </summary>
    public sealed partial class DebuggerComponent
    {
        /// <summary>
        /// 屏幕信息展示窗口的对象类
        /// </summary>
        private sealed class ScreenInformationWindow : BaseScrollableDebuggerWindow
        {
            protected override void OnDrawScrollableWindow()
            {
                UnityGUILayout.Label("<b>Screen Information</b>");
                UnityGUILayout.BeginVertical("box");
                {
                    DrawItem("Current Resolution", GetResolutionString(UnityScreen.currentResolution));
                    DrawItem("Screen Width", NovaEngine.Utility.Text.Format("{0} px / {1} in / {2} cm",
                                                                            UnityScreen.width.ToString(),
                                                                            NovaEngine.Utility.Convertion.GetInchesFromPixels(UnityScreen.width).ToString("F2"),
                                                                            NovaEngine.Utility.Convertion.GetCentimetersFromPixels(UnityScreen.width).ToString("F2")));
                    DrawItem("Screen Height", NovaEngine.Utility.Text.Format("{0} px / {1} in / {2} cm",
                                                                             UnityScreen.height.ToString(),
                                                                             NovaEngine.Utility.Convertion.GetInchesFromPixels(UnityScreen.height).ToString("F2"),
                                                                             NovaEngine.Utility.Convertion.GetCentimetersFromPixels(UnityScreen.height).ToString("F2")));
                    DrawItem("Screen DPI", UnityScreen.dpi.ToString("F2"));
                    DrawItem("Screen Orientation", UnityScreen.orientation.ToString());
                    DrawItem("Is Full Screen", UnityScreen.fullScreen.ToString());
                    DrawItem("Full Screen Mode", UnityScreen.fullScreenMode.ToString());
                    DrawItem("Sleep Timeout", GetSleepTimeoutDescription(UnityScreen.sleepTimeout));
                    DrawItem("Brightness", UnityScreen.brightness.ToString("F2"));
                    DrawItem("Cursor Visible", UnityCursor.visible.ToString());
                    DrawItem("Cursor Lock State", UnityCursor.lockState.ToString());
                    DrawItem("Auto Landscape Left", UnityScreen.autorotateToLandscapeLeft.ToString());
                    DrawItem("Auto Landscape Right", UnityScreen.autorotateToLandscapeRight.ToString());
                    DrawItem("Auto Portrait", UnityScreen.autorotateToPortrait.ToString());
                    DrawItem("Auto Portrait Upside Down", UnityScreen.autorotateToPortraitUpsideDown.ToString());
                    DrawItem("Safe Area", UnityScreen.safeArea.ToString());
                    DrawItem("Cutouts", GetCutoutsString(UnityScreen.cutouts));
                    DrawItem("Support Resolutions", GetResolutionsString(UnityScreen.resolutions));
                }
                UnityGUILayout.EndVertical();
            }

            /// <summary>
            /// 获取指定睡眠超时值的描述信息
            /// </summary>
            /// <param name="sleepTimeout">睡眠超时值</param>
            /// <returns>返回超时值对应的描述信息</returns>
            private string GetSleepTimeoutDescription(int sleepTimeout)
            {
                if (UnitySleepTimeout.NeverSleep == sleepTimeout)
                {
                    return "Never Sleep";
                }

                if (UnitySleepTimeout.SystemSetting == sleepTimeout)
                {
                    return "System Setting";
                }

                return sleepTimeout.ToString();
            }

            /// <summary>
            /// 获取指定分辨率值的描述字符串
            /// </summary>
            /// <param name="resolution">分辨率值</param>
            /// <returns>返回分辨率值对应的描述字符串</returns>
            private string GetResolutionString(UnityResolution resolution)
            {
                return NovaEngine.Utility.Text.Format("{0} x {1} @ {2}Hz", resolution.width.ToString(), resolution.height.ToString(), resolution.refreshRateRatio.ToString());
            }

            /// <summary>
            /// 获取指定裁剪区域的描述字符串信息
            /// </summary>
            /// <param name="cutouts">裁剪区域</param>
            /// <returns>返回裁剪区域对应的描述字符串</returns>
            private string GetCutoutsString(UnityRect[] cutouts)
            {
                string[] cutoutStrings = new string[cutouts.Length];
                for (int n = 0; n < cutouts.Length; ++n)
                {
                    cutoutStrings[n] = cutouts[n].ToString();
                }

                return string.Join("; ", cutoutStrings);
            }

            /// <summary>
            /// 批量获取指定分辨率数组的描述字符串
            /// </summary>
            /// <param name="resolutions">分辨率数组</param>
            /// <returns>返回分辨率数组对应的描述字符串</returns>
            private string GetResolutionsString(UnityResolution[] resolutions)
            {
                string[] resolutionStrings = new string[resolutions.Length];
                for (int n = 0; n < resolutions.Length; ++n)
                {
                    resolutionStrings[n] = GetResolutionString(resolutions[n]);
                }

                return string.Join("; ", resolutionStrings);
            }
        }
    }
}
