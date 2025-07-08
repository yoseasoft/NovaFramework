/// -------------------------------------------------------------------------------
/// NovaEngine Framework
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

namespace NovaEngine
{
    /// <summary>
    /// 系统级的应用管理类，用于定义应用程序的系统级访问接口
    /// </summary>
    public partial class Application : Singleton<Application>
    {
        /// <summary>
        /// 应用程序通知转发协议类型定义
        /// </summary>
        public enum ProtocolType : byte
        {
            /// <summary>
            /// 未知状态
            /// </summary>
            Unknown = 0,

            /// <summary>
            /// 程序进入非活动状态
            /// </summary>
            ResignActive = 11,

            /// <summary>
            /// 程序进入活动状态
            /// </summary>
            BecomeActive = 12,

            /// <summary>
            /// 程序推送到后台状态
            /// </summary>
            EnterBackground = 21,

            /// <summary>
            /// 程序回到前台状态
            /// </summary>
            EnterForeground = 22,

            /// <summary>
            /// 程序正确载入状态
            /// </summary>
            FinishLaunching = 31,

            /// <summary>
            /// 程序终止退出状态
            /// </summary>
            Terminate = 32,

            /// <summary>
            /// 程序模块启动通知
            /// </summary>
            Startup = 41,

            /// <summary>
            /// 程序模块关闭通知
            /// </summary>
            Shutdown = 42,

            /// <summary>
            /// 程序模块重启通知
            /// </summary>
            Restart = 43,

            /// <summary>
            /// 程序模块固定刷新通知
            /// </summary>
            FixedUpdate = 51,

            /// <summary>
            /// 程序模块刷新通知
            /// </summary>
            Update = 52,

            /// <summary>
            /// 程序模块后置刷新通知
            /// </summary>
            LateUpdate = 53,

            /// <summary>
            /// 程序模块固定执行通知
            /// </summary>
            FixedExecute = 61,

            /// <summary>
            /// 程序模块执行通知
            /// </summary>
            Execute = 62,

            /// <summary>
            /// 程序模块后置执行通知
            /// </summary>
            LateExecute = 63,
        }
    }
}
