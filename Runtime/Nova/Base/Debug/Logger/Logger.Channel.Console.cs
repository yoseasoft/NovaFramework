/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
/// Copyright (C) 2020 - 2022, Guangzhou Xinyuan Technology Co., Ltd.
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
    /// 日志相关函数集合工具类
    /// </summary>
    public static partial class Logger
    {
        /// <summary>
        /// 日志输出控制台模式操作管理类
        /// </summary>
        [LogOutputChannelBinding(LogOutputChannelType.Console)]
        public sealed class LogConsole : Singleton<LogConsole>, ILogOutput
        {
            /// <summary>
            /// 启动日志输出控制台模式
            /// </summary>
            public static void Startup()
            {
                LogConsole c = Instance;
                AddOutputChannel(c);
            }

            /// <summary>
            /// 关闭日志输出控制台模式
            /// </summary>
            public static void Shutdown()
            {
                LogConsole c = Instance;
                RemoveOutputChannel(c);
                Destroy();
            }

            /// <summary>
            /// 日志控制台类初始化接口
            /// </summary>
            protected override void Initialize()
            {
            }

            /// <summary>
            /// 日志控制台类清理接口
            /// </summary>
            protected override void Cleanup()
            {
            }

            /// <summary>
            /// 日志输入记录接口
            /// </summary>
            /// <param name="level">日志等级</param>
            /// <param name="message">日志内容</param>
            public void Output(LogOutputLevelType level, object message)
            {
                System.Console.WriteLine(message.ToString());
            }
        }
    }
}
