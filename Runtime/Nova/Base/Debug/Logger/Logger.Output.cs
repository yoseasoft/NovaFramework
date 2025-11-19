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
        /// 基于调试模式下的日志标准输出接口，参考<see cref="LogOutputLevelType.Debug"/>类型定义
        /// </summary>
        /// <param name="message">日志内容</param>
        public static void Debug(object message)
        {
            __Output(LogOutputLevelType.Debug, message);
        }

        /// <summary>
        /// 基于调试模式下的日志标准输出接口，参考<see cref="LogOutputLevelType.Debug"/>类型定义
        /// </summary>
        /// <param name="message">日志内容</param>
        public static void Debug(string message)
        {
            __Output(LogOutputLevelType.Debug, message);
        }

        /// <summary>
        /// 基于调试模式下的日志标准输出接口，参考<see cref="LogOutputLevelType.Debug"/>类型定义
        /// </summary>
        /// <param name="format">日志格式内容</param>
        /// <param name="args">日志格式化参数</param>
        public static void Debug(string format, params object[] args)
        {
            __Output(LogOutputLevelType.Debug, format, args);
        }

        /// <summary>
        /// 基于常规模式下的日志标准输出接口，参考<see cref="LogOutputLevelType.Info"/>类型定义
        /// </summary>
        /// <param name="message">日志内容</param>
        public static void Info(object message)
        {
            __Output(LogOutputLevelType.Info, message);
        }

        /// <summary>
        /// 基于常规模式下的日志标准输出接口，参考<see cref="LogOutputLevelType.Info"/>类型定义
        /// </summary>
        /// <param name="message">日志内容</param>
        public static void Info(string message)
        {
            __Output(LogOutputLevelType.Info, message);
        }

        /// <summary>
        /// 基于常规模式下的日志标准输出接口，参考<see cref="LogOutputLevelType.Info"/>类型定义
        /// </summary>
        /// <param name="format">日志格式内容</param>
        /// <param name="args">日志格式化参数</param>
        public static void Info(string format, params object[] args)
        {
            __Output(LogOutputLevelType.Info, format, args);
        }

        /// <summary>
        /// 基于警告模式下的日志标准输出接口，参考<see cref="LogOutputLevelType.Warning"/>类型定义
        /// </summary>
        /// <param name="message">日志内容</param>
        public static void Warn(object message)
        {
            __Output(LogOutputLevelType.Warning, message);
        }

        /// <summary>
        /// 基于警告模式下的日志标准输出接口，参考<see cref="LogOutputLevelType.Warning"/>类型定义
        /// </summary>
        /// <param name="message">日志内容</param>
        public static void Warn(string message)
        {
            __Output(LogOutputLevelType.Warning, message);
        }

        /// <summary>
        /// 基于警告模式下的日志标准输出接口，参考<see cref="LogOutputLevelType.Warning"/>类型定义
        /// </summary>
        /// <param name="format">日志格式内容</param>
        /// <param name="args">日志格式化参数</param>
        public static void Warn(string format, params object[] args)
        {
            __Output(LogOutputLevelType.Warning, format, args);
        }

        /// <summary>
        /// 基于错误模式下的日志标准输出接口，参考<see cref="LogOutputLevelType.Error"/>类型定义
        /// </summary>
        /// <param name="message">日志内容</param>
        public static void Error(object message)
        {
            __Output(LogOutputLevelType.Error, message);
        }

        /// <summary>
        /// 基于错误模式下的日志标准输出接口，参考<see cref="LogOutputLevelType.Error"/>类型定义
        /// </summary>
        /// <param name="message">日志内容</param>
        public static void Error(string message)
        {
            __Output(LogOutputLevelType.Error, message);
        }

        /// <summary>
        /// 基于错误模式下的日志标准输出接口，参考<see cref="LogOutputLevelType.Error"/>类型定义
        /// </summary>
        /// <param name="format">日志格式内容</param>
        /// <param name="args">日志格式化参数</param>
        public static void Error(string format, params object[] args)
        {
            __Output(LogOutputLevelType.Error, format, args);
        }

        /// <summary>
        /// 基于崩溃模式下的日志标准输出接口，参考<see cref="LogOutputLevelType.Fatal"/>类型定义
        /// </summary>
        /// <param name="message">日志内容</param>
        public static void Fatal(object message)
        {
            __Output(LogOutputLevelType.Fatal, message);
        }

        /// <summary>
        /// 基于崩溃模式下的日志标准输出接口，参考<see cref="LogOutputLevelType.Fatal"/>类型定义
        /// </summary>
        /// <param name="message">日志内容</param>
        public static void Fatal(string message)
        {
            __Output(LogOutputLevelType.Fatal, message);
        }

        /// <summary>
        /// 基于崩溃模式下的日志标准输出接口，参考<see cref="LogOutputLevelType.Fatal"/>类型定义
        /// </summary>
        /// <param name="format">日志格式内容</param>
        /// <param name="args">日志格式化参数</param>
        public static void Fatal(string format, params object[] args)
        {
            __Output(LogOutputLevelType.Fatal, format, args);
        }

        /// <summary>
        /// 使用给定的日志级别进行目标日志的输出
        /// </summary>
        /// <param name="level">日志级别</param>
        /// <param name="message">日志内容</param>
        internal static void __Output(LogOutputLevelType level, object message)
        {
            __Output(level, message.ToString());
        }

        /// <summary>
        /// 使用给定的日志级别进行目标日志的输出
        /// </summary>
        /// <param name="level">日志级别</param>
        /// <param name="message">日志内容</param>
        internal static void __Output(LogOutputLevelType level, string message)
        {
            _logOutputHandler?.Invoke(level, message);
        }

        /// <summary>
        /// 使用给定的日志级别进行目标日志的输出
        /// </summary>
        /// <param name="level">日志级别</param>
        /// <param name="format">日志格式内容</param>
        /// <param name="args">日志格式化参数</param>
        internal static void __Output(LogOutputLevelType level, string format, params object[] args)
        {
            __Output(level, Utility.Text.Format(format, args));
        }
    }
}
