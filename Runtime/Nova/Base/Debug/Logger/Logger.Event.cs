/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
/// Copyring (C) 2020 - 2022, Guangzhou Xinyuan Technology Co., Ltd.
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
        /// 日志输出事件模式操作管理类
        /// </summary>
        public sealed class Event : Singleton<Event>, ILogOutput
        {
            /// <summary>
            /// 日志输出规范定义代理句柄接口
            /// </summary>
            /// <param name="level">日志级别</param>
            /// <param name="message">日志内容</param>
            public delegate void OutputHandler(LogOutputLevelType level, object message);

            /// <summary>
            /// 输出事件回调接口
            /// </summary>
            private OutputHandler m_outputCallback = null;

            /// <summary>
            /// 启动日志输出事件模式
            /// </summary>
            public static void Startup()
            {
                Event e = Event.Instance;
                Logger.AddOutputChannel(e);
            }

            /// <summary>
            /// 关闭日志输出事件模式
            /// </summary>
            public static void Shutdown()
            {
                Event e = Event.Instance;
                Logger.RemoveOutputChannel(e);
                Event.Destroy();
            }

            /// <summary>
            /// 日志事件类初始化接口
            /// </summary>
            protected override void Initialize()
            {
            }

            /// <summary>
            /// 日志事件类清理接口
            /// </summary>
            protected override void Cleanup()
            {
                this.RemoveAllOutputHandler();
            }

            /// <summary>
            /// 日志输入记录接口
            /// </summary>
            /// <param name="level">日志等级</param>
            /// <param name="message">日志内容</param>
            public void Output(LogOutputLevelType level, object message)
            {
                m_outputCallback(level, message);
            }

            /// <summary>
            /// 添加指定的输出句柄
            /// </summary>
            /// <param name="handler">输出句柄实例</param>
            public void AddOutputHandler(OutputHandler handler)
            {
                m_outputCallback += handler;
            }

            /// <summary>
            /// 移除指定的输出句柄
            /// </summary>
            /// <param name="handler">输出句柄实例</param>
            public void RemoveOutputHandler(OutputHandler handler)
            {
                m_outputCallback -= handler;
            }

            /// <summary>
            /// 移除已添加的全部输出句柄
            /// </summary>
            public void RemoveAllOutputHandler()
            {
                m_outputCallback = null;
            }
        }
    }
}
