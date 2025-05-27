/// -------------------------------------------------------------------------------
/// NovaEngine Framework
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

using UnityApplication = UnityEngine.Application;
using UnityRuntimePlatform = UnityEngine.RuntimePlatform;

namespace NovaEngine
{
    /// <summary>
    /// 系统级的平台管理类的模块子类声明，用于进行平台配置相关数据接口声明
    /// </summary>
    public partial class Platform
    {
        /// <summary>
        /// 平台类型的枚举定义
        /// </summary>
        public enum PlatformType : byte
        {
            Unknown = 0,
            Windows,
            Linux,
            MacOS,
            Android,
            IPhone,
            WebGL,
        }

        /// <summary>
        /// 当前平台类型属性接口
        /// </summary>
        public static PlatformType CurrentPlatformType
        {
            get
            {
#if UNITY_EDITOR
                if (!UnityApplication.isEditor)
                {
#endif
                    return UnityApplication.platform switch
                    {
                        UnityRuntimePlatform.WindowsEditor => PlatformType.Windows,
                        UnityRuntimePlatform.WindowsPlayer => PlatformType.Windows,
                        UnityRuntimePlatform.LinuxEditor => PlatformType.Linux,
                        UnityRuntimePlatform.LinuxPlayer => PlatformType.Linux,
                        UnityRuntimePlatform.OSXEditor => PlatformType.MacOS,
                        UnityRuntimePlatform.OSXPlayer => PlatformType.MacOS,
                        UnityRuntimePlatform.Android => PlatformType.Android,
                        UnityRuntimePlatform.IPhonePlayer => PlatformType.IPhone,
                        UnityRuntimePlatform.WebGLPlayer => PlatformType.WebGL,
                        _ => PlatformType.Unknown,
                    };
#if UNITY_EDITOR
                }
                else
                {
                    return UnityEditor.EditorUserBuildSettings.activeBuildTarget switch
                    {
                        UnityEditor.BuildTarget.StandaloneWindows => PlatformType.Windows,
                        UnityEditor.BuildTarget.StandaloneWindows64 => PlatformType.Windows,
                        // UnityEditor.BuildTarget.StandaloneLinux => PlatformType.Linux,
                        UnityEditor.BuildTarget.StandaloneLinux64 => PlatformType.Linux,
                        UnityEditor.BuildTarget.StandaloneOSX => PlatformType.MacOS,
                        // UnityEditor.BuildTarget.StandaloneOSXIntel => PlatformType.MacOS,
                        // UnityEditor.BuildTarget.StandaloneOSXIntel64 => PlatformType.MacOS,
                        UnityEditor.BuildTarget.Android => PlatformType.Android,
                        UnityEditor.BuildTarget.iOS => PlatformType.IPhone,
                        UnityEditor.BuildTarget.WebGL => PlatformType.WebGL,
                        _ => PlatformType.Unknown,
                    };
                }
#endif
            }
        }

        /// <summary>
        /// 当前平台名称属性接口
        /// </summary>
        public static string CurrentPlatformName
        {
            get
            {
                return CurrentPlatformType switch
                {
                    PlatformType.Windows => Definition.Platform.OS_WINDOWS,
                    PlatformType.Linux => Definition.Platform.OS_LINUX,
                    PlatformType.MacOS => Definition.Platform.OS_MACOS,
                    PlatformType.Android => Definition.Platform.OS_ANDROID,
                    PlatformType.IPhone => Definition.Platform.OS_IPHONE,
                    PlatformType.WebGL => Definition.Platform.OS_WEBGL,
                    _ => Definition.Platform.OS_UNKNOWN,
                };
            }
        }
    }
}
