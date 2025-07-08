/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
/// Copyright (C) 2020 - 2022, Guangzhou Xinyuan Technology Co., Ltd.
/// Copyright (C) 2025, Hurley, Independent Studio.
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
        /// 日志输出规范定义代理句柄接口
        /// </summary>
        /// <param name="level">日志级别</param>
        /// <param name="message">日志内容</param>
        public delegate void OutputHandler(LogOutputLevelType level, object message);

        /// <summary>
        /// 日志输入代理回调接口
        /// </summary>
        private static OutputHandler _logOutputHandler = null;

        /// <summary>
        /// 启动日志系统
        /// </summary>
        public static void Startup()
        {
            int channels = Configuration.LogChannel;

            //#if !UNITY_EDITOR
            //// 如果处于非编辑器环境，需要禁用掉编辑器相关通道类型
            //int mask = ~((int) LogOutputChannelType.Editor);
            //channels = channels & mask;
            //#endif

            if (Application.IsEditor())
            {
                // 如果处于编辑器环境，可以默认开启编辑器相关通道类型
                //channels = channels | (int) LogOutputChannelType.Editor;
            }
            else
            {
                // 如果处于非编辑器环境，需要禁用掉编辑器相关通道类型
                int mask = ~(int) LogOutputChannelType.Editor;
                channels = channels & mask;
            }

            // 打开指定的日志通道
            OpenOutputChannels(channels);
        }

        /// <summary>
        /// 关闭日志系统
        /// </summary>
        public static void Shutdown()
        {
            // 关闭所有日志通道
            CloseOutputChannels();
        }

        // internal static void AddOutputChannel(OutputHandler output)
        // {
        // s_logOutputHandler += output;
        // }

        /// <summary>
        /// 添加指定的输出通道
        /// </summary>
        /// <param name="log">通道实例</param>
        internal static void AddOutputChannel(ILogOutput log)
        {
            _logOutputHandler += log.Output;
        }

        // internal static void RemoveOutputChannel(OutputHandler output)
        // {
        // s_logOutputHandler -= output;
        // }

        /// <summary>
        /// 移除指定的输出通道
        /// </summary>
        /// <param name="log">通道实例</param>
        internal static void RemoveOutputChannel(ILogOutput log)
        {
            _logOutputHandler -= log.Output;
        }
    }
}
