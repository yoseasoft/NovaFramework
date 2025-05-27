/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
/// Copyring (C) 2022 - 2023, Shanghai Bilibili Technology Co., Ltd.
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

using UnitySystemInfo = UnityEngine.SystemInfo;

namespace NovaEngine
{
    /// <summary>
    /// 实用函数集合工具类
    /// </summary>
    public static partial class Utility
    {
        /// <summary>
        /// 系统信息相关实用函数集合
        /// </summary>
        public static class SystemInfo
        {
            /// <summary>
            /// 获取当前终端设备型号
            /// </summary>
            /// <returns>返回当前终端设备型号</returns>
            public static string DeviceModel()
            {
                return UnitySystemInfo.deviceModel;
            }

            /// <summary>
            /// 获取当前终端设备名称
            /// </summary>
            /// <returns>返回当前终端设备名称</returns>
            public static string DeviceName()
            {
                return UnitySystemInfo.deviceName;
            }

            /// <summary>
            /// 获取当前终端设备唯一标识码，每一台设备都有唯一的标识符
            /// </summary>
            /// <returns>返回当前终端设备唯一标识码</returns>
            public static string DeviceUniqueIdentifier()
            {
                return UnitySystemInfo.deviceUniqueIdentifier;
            }

            /// <summary>
            /// 获取当前终端设备显卡的唯一标识符ID
            /// </summary>
            /// <returns>返回当前终端设备显卡以为标识符ID</returns>
            public static int GraphicsDeviceID()
            {
                return UnitySystemInfo.graphicsDeviceID;
            }

            /// <summary>
            /// 获取当前终端设备显卡的名称
            /// </summary>
            /// <returns>返回当前终端设备显卡名称</returns>
            public static string GraphicsDeviceName()
            {
                return UnitySystemInfo.graphicsDeviceName;
            }

            /// <summary>
            /// 获取当前终端设备显卡的存储空间大小
            /// </summary>
            /// <returns>返回当前终端设备显存大小</returns>
            public static int GraphicsMemorySize()
            {
                return UnitySystemInfo.graphicsMemorySize;
            }

            /// <summary>
            /// 获取当前终端设备最大能支持的立方体大小
            /// </summary>
            /// <returns>返回当前终端设备最大支持立方体大小</returns>
            public static int MaxCubemapSize()
            {
                return UnitySystemInfo.maxCubemapSize;
            }

            /// <summary>
            /// 获取当前终端设备最大能支持的纹理大小
            /// </summary>
            /// <returns>返回当前终端设备最大支持纹理大小</returns>
            public static int MaxTextureSize()
            {
                return UnitySystemInfo.maxTextureSize;
            }

            /// <summary>
            /// 检测当前终端设备是否支持陀螺仪功能
            /// </summary>
            /// <returns>若当前终端设备支持陀螺仪功能返回true，否则返回false</returns>
            public static bool SupportsGyroscope()
            {
                return UnitySystemInfo.supportsGyroscope;
            }

            /// <summary>
            /// 检测当前终端设备是否支持定位功能
            /// </summary>
            /// <returns>若当前终端设备支持定位功能返回true，否则返回false</returns>
            public static bool SupportsLocationService()
            {
                return UnitySystemInfo.supportsLocationService;
            }

            /// <summary>
            /// 检测当前终端设备是否支持运动向量功能
            /// </summary>
            /// <returns>若当前终端设备支持运动向量功能返回true，否则返回false</returns>
            public static bool SupportsMotionVectors()
            {
                return UnitySystemInfo.supportsMotionVectors;
            }

            /// <summary>
            /// 获取当前终端设备的存储空间大小
            /// </summary>
            /// <returns>返回当前终端设备内存大小</returns>
            public static int SystemMemorySize()
            {
                return UnitySystemInfo.systemMemorySize;
            }
        }
    }
}
