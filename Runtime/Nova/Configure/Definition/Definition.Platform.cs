/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
/// Copyright (C) 2023, Guangzhou Shiyue Network Technology Co., Ltd.
/// Copyright (C) 2025, Hainan Yuanyou Information Technology Co., Ltd. Guangzhou Branch
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

namespace NovaEngine
{
    /// <summary>
    /// 基础常量数据定义类，将基础数据类型中的一些通用常量在此统一进行定义使用
    /// </summary>
    public static partial class Definition
    {
        /// <summary>
        /// 平台相关常量数据定义
        /// </summary>
        public static class Platform
        {
            /// <summary>
            /// 无效操作系统名称
            /// </summary>
            public const string OS_UNKNOWN = CString.Unknown;
            /// <summary>
            /// Windows操作系统名称
            /// </summary>
            public const string OS_WINDOWS = "windows";
            /// <summary>
            /// Linux操作系统名称
            /// </summary>
            public const string OS_LINUX = "linux";
            /// <summary>
            /// MacOS操作系统名称
            /// </summary>
            public const string OS_MACOS = "macos";
            /// <summary>
            /// WinPhone操作系统名称
            /// </summary>
            public const string OS_WINPHONE = "winphone";
            /// <summary>
            /// Android操作系统名称
            /// </summary>
            public const string OS_ANDROID = "android";
            /// <summary>
            /// iPhone操作系统名称
            /// </summary>
            public const string OS_IPHONE = "iphone";
            /// <summary>
            /// WebGL操作系统名称
            /// </summary>
            public const string OS_WEBGL = "webgl";
        }
    }
}
