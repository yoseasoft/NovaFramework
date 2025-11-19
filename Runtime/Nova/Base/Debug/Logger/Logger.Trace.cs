/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
/// Copyright (C) 2020 - 2022, Guangzhou Xinyuan Technology Co., Ltd.
/// Copyright (C) 2023 - 2024, Guangzhou Shiyue Network Technology Co., Ltd.
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
    internal static partial class Logger
    {
        /// <summary>
        /// 基于调试模式下的日志追踪输出接口，参考<see cref="LogOutputLevelType.Debug"/>类型定义
        /// </summary>
        /// <param name="message">日志内容</param>
        public static void TraceDebug(object message)
        {
            __TraceOutput(LogOutputLevelType.Debug, message);
        }

        /// <summary>
        /// 基于调试模式下的日志追踪输出接口，参考<see cref="LogOutputLevelType.Debug"/>类型定义
        /// </summary>
        /// <param name="message">日志内容</param>
        public static void TraceDebug(string message)
        {
            __TraceOutput(LogOutputLevelType.Debug, message);
        }

        /// <summary>
        /// 基于调试模式下的日志追踪输出接口，参考<see cref="LogOutputLevelType.Debug"/>类型定义
        /// </summary>
        /// <param name="format">日志格式内容</param>
        /// <param name="args">日志格式化参数</param>
        public static void TraceDebug(string format, params object[] args)
        {
            __TraceOutput(LogOutputLevelType.Debug, format, args);
        }

        /// <summary>
        /// 基于常规模式下的日志追踪输出接口，参考<see cref="LogOutputLevelType.Info"/>类型定义
        /// </summary>
        /// <param name="message">日志内容</param>
        public static void TraceInfo(object message)
        {
            __TraceOutput(LogOutputLevelType.Info, message);
        }

        /// <summary>
        /// 基于常规模式下的日志追踪输出接口，参考<see cref="LogOutputLevelType.Info"/>类型定义
        /// </summary>
        /// <param name="message">日志内容</param>
        public static void TraceInfo(string message)
        {
            __TraceOutput(LogOutputLevelType.Info, message);
        }

        /// <summary>
        /// 基于常规模式下的日志追踪输出接口，参考<see cref="LogOutputLevelType.Info"/>类型定义
        /// </summary>
        /// <param name="format">日志格式内容</param>
        /// <param name="args">日志格式化参数</param>
        public static void TraceInfo(string format, params object[] args)
        {
            __TraceOutput(LogOutputLevelType.Info, format, args);
        }

        /// <summary>
        /// 基于警告模式下的日志追踪输出接口，参考<see cref="LogOutputLevelType.Warning"/>类型定义
        /// </summary>
        /// <param name="message">日志内容</param>
        public static void TraceWarn(object message)
        {
            __TraceOutput(LogOutputLevelType.Warning, message);
        }

        /// <summary>
        /// 基于警告模式下的日志追踪输出接口，参考<see cref="LogOutputLevelType.Warning"/>类型定义
        /// </summary>
        /// <param name="message">日志内容</param>
        public static void TraceWarn(string message)
        {
            __TraceOutput(LogOutputLevelType.Warning, message);
        }

        /// <summary>
        /// 基于警告模式下的日志追踪输出接口，参考<see cref="LogOutputLevelType.Warning"/>类型定义
        /// </summary>
        /// <param name="format">日志格式内容</param>
        /// <param name="args">日志格式化参数</param>
        public static void TraceWarn(string format, params object[] args)
        {
            __TraceOutput(LogOutputLevelType.Warning, format, args);
        }

        /// <summary>
        /// 基于错误模式下的日志追踪输出接口，参考<see cref="LogOutputLevelType.Error"/>类型定义
        /// </summary>
        /// <param name="message">日志内容</param>
        public static void TraceError(object message)
        {
            __TraceOutput(LogOutputLevelType.Error, message);
        }

        /// <summary>
        /// 基于错误模式下的日志追踪输出接口，参考<see cref="LogOutputLevelType.Error"/>类型定义
        /// </summary>
        /// <param name="message">日志内容</param>
        public static void TraceError(string message)
        {
            __TraceOutput(LogOutputLevelType.Error, message);
        }

        /// <summary>
        /// 基于错误模式下的日志追踪输出接口，参考<see cref="LogOutputLevelType.Error"/>类型定义
        /// </summary>
        /// <param name="format">日志格式内容</param>
        /// <param name="args">日志格式化参数</param>
        public static void TraceError(string format, params object[] args)
        {
            __TraceOutput(LogOutputLevelType.Error, format, args);
        }

        /// <summary>
        /// 基于崩溃模式下的日志追踪输出接口，参考<see cref="LogOutputLevelType.Fatal"/>类型定义
        /// </summary>
        /// <param name="message">日志内容</param>
        public static void TraceFatal(object message)
        {
            __TraceOutput(LogOutputLevelType.Fatal, message);
        }

        /// <summary>
        /// 基于崩溃模式下的日志追踪输出接口，参考<see cref="LogOutputLevelType.Fatal"/>类型定义
        /// </summary>
        /// <param name="message">日志内容</param>
        public static void TraceFatal(string message)
        {
            __TraceOutput(LogOutputLevelType.Fatal, message);
        }

        /// <summary>
        /// 基于崩溃模式下的日志追踪输出接口，参考<see cref="LogOutputLevelType.Fatal"/>类型定义
        /// </summary>
        /// <param name="format">日志格式内容</param>
        /// <param name="args">日志格式化参数</param>
        public static void TraceFatal(string format, params object[] args)
        {
            __TraceOutput(LogOutputLevelType.Fatal, format, args);
        }

        /// <summary>
        /// 使用给定的日志级别进行目标日志的追踪输出
        /// </summary>
        /// <param name="level">日志级别</param>
        /// <param name="message">日志内容</param>
        internal static void __TraceOutput(LogOutputLevelType level, object message)
        {
            __TraceOutput(level, message.ToString());
        }

        /// <summary>
        /// 使用给定的日志级别进行目标日志的追踪输出
        /// </summary>
        /// <param name="level">日志级别</param>
        /// <param name="message">日志内容</param>
        internal static void __TraceOutput(LogOutputLevelType level, string message)
        {
            _logOutputHandler?.Invoke(level, message);
        }

        /// <summary>
        /// 使用给定的日志级别进行目标日志的追踪输出
        /// </summary>
        /// <param name="level">日志级别</param>
        /// <param name="format">日志格式内容</param>
        /// <param name="args">日志格式化参数</param>
        internal static void __TraceOutput(LogOutputLevelType level, string format, params object[] args)
        {
            __TraceOutput(level, Utility.Text.Format(format, args));
        }
    }
}
