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
using UnityApplication = UnityEngine.Application;
using UnitySystemInfo = UnityEngine.SystemInfo;

namespace GameEngine.Profiler.Debugging
{
    /// <summary>
    /// 游戏调试器组件对象类，用于定义调试器对象的基础属性及访问操作函数
    /// </summary>
    public sealed partial class DebuggerComponent
    {
        /// <summary>
        /// 系统信息展示窗口的对象类
        /// </summary>
        private sealed class SystemInformationWindow : BaseScrollableDebuggerWindow
        {
            protected override void OnDrawScrollableWindow()
            {
                UnityGUILayout.Label("<b>System Information</b>");
                UnityGUILayout.BeginVertical("box");
                {
                    DrawItem("Device Unique ID", UnitySystemInfo.deviceUniqueIdentifier);
                    DrawItem("Device Name", UnitySystemInfo.deviceName);
                    DrawItem("Device Type", UnitySystemInfo.deviceType.ToString());
                    DrawItem("Device Model", UnitySystemInfo.deviceModel);
                    DrawItem("Processor Type", UnitySystemInfo.processorType);
                    DrawItem("Processor Count", UnitySystemInfo.processorCount.ToString());
                    DrawItem("Processor Frequency", NovaEngine.Utility.Text.Format("{0} MHz", UnitySystemInfo.processorFrequency.ToString()));
                    DrawItem("System Memory Size", NovaEngine.Utility.Text.Format("{0} MB", UnitySystemInfo.systemMemorySize.ToString()));
                    DrawItem("Operating System Family", UnitySystemInfo.operatingSystemFamily.ToString());
                    DrawItem("Operating System", UnitySystemInfo.operatingSystem);
                    DrawItem("Battery Status", UnitySystemInfo.batteryStatus.ToString());
                    DrawItem("Battery Level", GetBatteryLevelString(UnitySystemInfo.batteryLevel));
                    DrawItem("Supports Audio", UnitySystemInfo.supportsAudio.ToString());
                    DrawItem("Supports Location Service", UnitySystemInfo.supportsLocationService.ToString());
                    DrawItem("Supports Accelerometer", UnitySystemInfo.supportsAccelerometer.ToString());
                    DrawItem("Supports Gyroscope", UnitySystemInfo.supportsGyroscope.ToString());
                    DrawItem("Supports Vibration", UnitySystemInfo.supportsVibration.ToString());
                    DrawItem("Genuine", UnityApplication.genuine.ToString());
                    DrawItem("Genuine Check Available", UnityApplication.genuineCheckAvailable.ToString());
                }
                UnityGUILayout.EndVertical();
            }

            /// <summary>
            /// 获取当前硬件的电池电量的字符串信息
            /// </summary>
            /// <param name="batteryLevel">电池电量</param>
            /// <returns>返回电池电量的字符串信息</returns>
            private string GetBatteryLevelString(float batteryLevel)
            {
                if (batteryLevel < 0f)
                {
                    return "Unavailable";
                }

                return batteryLevel.ToString("P0");
            }
        }
    }
}
