/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
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
    /// 调试器对象工具类，用于引擎内部调试控制及输出相关接口声明
    /// </summary>
    internal /*sealed*/ partial class Debugger : Singleton<Debugger>
    {
        /// <summary>
        /// 调试器对象类初始化接口
        /// </summary>
        protected override void Initialize()
        {
        }

        /// <summary>
        /// 调试器对象类清理接口
        /// </summary>
        protected override void Cleanup()
        {
        }

        /// <summary>
        /// 基于调试模式下的日志输出接口，参考<see cref="LogOutputLevelType.Debug"/>类型定义
        /// </summary>
        /// <param name="message">日志内容</param>
        public static void Log(object message)
        {
            Instance._log_object?.Invoke(message);
        }

        /// <summary>
        /// 基于调试模式下的日志输出接口，参考<see cref="LogOutputLevelType.Debug"/>类型定义
        /// </summary>
        /// <param name="message">日志内容</param>
        public static void Log(string message)
        {
            Instance._log_string?.Invoke(message);
        }

        /// <summary>
        /// 基于调试模式下的日志输出接口，参考<see cref="LogOutputLevelType.Debug"/>类型定义
        /// </summary>
        /// <param name="condition">条件表达式</param>
        /// <param name="message">日志内容</param>
        public static void Log(bool condition, string message)
        {
            if (false == condition) Log(message);
        }

        /// <summary>
        /// 基于调试模式下的日志输出接口，参考<see cref="LogOutputLevelType.Debug"/>类型定义
        /// </summary>
        /// <param name="format">日志格式内容</param>
        /// <param name="args">日志格式化参数</param>
        public static void Log(string format, params object[] args)
        {
            Instance._log_format_args?.Invoke(format, args);
        }

        /// <summary>
        /// 基于调试模式下的日志输出接口，参考<see cref="LogOutputLevelType.Debug"/>类型定义
        /// </summary>
        /// <param name="condition">条件表达式</param>
        /// <param name="format">日志格式内容</param>
        /// <param name="args">日志格式化参数</param>
        public static void Log(bool condition, string format, params object[] args)
        {
            if (false == condition) Log(format, args);
        }

        /// <summary>
        /// 基于常规模式下的日志输出接口，参考<see cref="LogOutputLevelType.Info"/>类型定义
        /// </summary>
        /// <param name="message">日志内容</param>
        public static void Info(object message)
        {
            Instance._info_object?.Invoke(message);
        }

        /// <summary>
        /// 基于常规模式下的日志输出接口，参考<see cref="LogOutputLevelType.Info"/>类型定义
        /// </summary>
        /// <param name="message">日志内容</param>
        public static void Info(string message)
        {
            Instance._info_string?.Invoke(message);
        }

        /// <summary>
        /// 基于常规模式下的日志输出接口，参考<see cref="LogOutputLevelType.Info"/>类型定义
        /// </summary>
        /// <param name="condition">条件表达式</param>
        /// <param name="message">日志内容</param>
        public static void Info(bool condition, string message)
        {
            if (false == condition) Info(message);
        }

        /// <summary>
        /// 基于常规模式下的日志输出接口，参考<see cref="LogOutputLevelType.Info"/>类型定义
        /// </summary>
        /// <param name="format">日志格式内容</param>
        /// <param name="args">日志格式化参数</param>
        public static void Info(string format, params object[] args)
        {
            Instance._info_format_args?.Invoke(format, args);
        }

        /// <summary>
        /// 基于常规模式下的日志输出接口，参考<see cref="LogOutputLevelType.Info"/>类型定义
        /// </summary>
        /// <param name="condition">条件表达式</param>
        /// <param name="format">日志格式内容</param>
        /// <param name="args">日志格式化参数</param>
        public static void Info(bool condition, string format, params object[] args)
        {
            if (false == condition) Info(format, args);
        }

        /// <summary>
        /// 基于警告模式下的日志输出接口，参考<see cref="LogOutputLevelType.Warning"/>类型定义
        /// </summary>
        /// <param name="message">日志内容</param>
        public static void Warn(object message)
        {
            Instance._warn_object?.Invoke(message);
        }

        /// <summary>
        /// 基于警告模式下的日志输出接口，参考<see cref="LogOutputLevelType.Warning"/>类型定义
        /// </summary>
        /// <param name="message">日志内容</param>
        public static void Warn(string message)
        {
            Instance._warn_string?.Invoke(message);
        }

        /// <summary>
        /// 基于警告模式下的日志输出接口，参考<see cref="LogOutputLevelType.Warning"/>类型定义
        /// </summary>
        /// <param name="condition">条件表达式</param>
        /// <param name="message">日志内容</param>
        public static void Warn(bool condition, string message)
        {
            if (false == condition) Warn(message);
        }

        /// <summary>
        /// 基于警告模式下的日志输出接口，参考<see cref="LogOutputLevelType.Warning"/>类型定义
        /// </summary>
        /// <param name="format">日志格式内容</param>
        /// <param name="args">日志格式化参数</param>
        public static void Warn(string format, params object[] args)
        {
            Instance._warn_format_args?.Invoke(format, args);
        }

        /// <summary>
        /// 基于警告模式下的日志输出接口，参考<see cref="LogOutputLevelType.Warning"/>类型定义
        /// </summary>
        /// <param name="condition">条件表达式</param>
        /// <param name="format">日志格式内容</param>
        /// <param name="args">日志格式化参数</param>
        public static void Warn(bool condition, string format, params object[] args)
        {
            if (false == condition) Warn(format, args);
        }

        /// <summary>
        /// 基于错误模式下的日志输出接口，参考<see cref="LogOutputLevelType.Error"/>类型定义
        /// </summary>
        /// <param name="message">日志内容</param>
        public static void Error(object message)
        {
            Instance._error_object?.Invoke(message);
        }

        /// <summary>
        /// 基于错误模式下的日志输出接口，参考<see cref="LogOutputLevelType.Error"/>类型定义
        /// </summary>
        /// <param name="message">日志内容</param>
        public static void Error(string message)
        {
            Instance._error_string?.Invoke(message);
        }

        /// <summary>
        /// 基于错误模式下的日志输出接口，参考<see cref="LogOutputLevelType.Error"/>类型定义
        /// </summary>
        /// <param name="condition">条件表达式</param>
        /// <param name="message">日志内容</param>
        public static void Error(bool condition, string message)
        {
            if (false == condition) Error(message);
        }

        /// <summary>
        /// 基于错误模式下的日志输出接口，参考<see cref="LogOutputLevelType.Error"/>类型定义
        /// </summary>
        /// <param name="format">日志格式内容</param>
        /// <param name="args">日志格式化参数</param>
        public static void Error(string format, params object[] args)
        {
            Instance._error_format_args?.Invoke(format, args);
        }

        /// <summary>
        /// 基于错误模式下的日志输出接口，参考<see cref="LogOutputLevelType.Error"/>类型定义
        /// </summary>
        /// <param name="condition">条件表达式</param>
        /// <param name="format">日志格式内容</param>
        /// <param name="args">日志格式化参数</param>
        public static void Error(bool condition, string format, params object[] args)
        {
            if (false == condition) Error(format, args);
        }

        /// <summary>
        /// 基于崩溃模式下的日志输出接口，参考<see cref="LogOutputLevelType.Fatal"/>类型定义
        /// </summary>
        /// <param name="message">日志内容</param>
        public static void Fatal(object message)
        {
            Instance._fatal_object?.Invoke(message);
        }

        /// <summary>
        /// 基于崩溃模式下的日志输出接口，参考<see cref="LogOutputLevelType.Fatal"/>类型定义
        /// </summary>
        /// <param name="message">日志内容</param>
        public static void Fatal(string message)
        {
            Instance._fatal_string?.Invoke(message);
        }

        /// <summary>
        /// 基于崩溃模式下的日志输出接口，参考<see cref="LogOutputLevelType.Fatal"/>类型定义
        /// </summary>
        /// <param name="condition">条件表达式</param>
        /// <param name="message">日志内容</param>
        public static void Fatal(bool condition, string message)
        {
            if (false == condition) Fatal(message);
        }

        /// <summary>
        /// 基于崩溃模式下的日志输出接口，参考<see cref="LogOutputLevelType.Fatal"/>类型定义
        /// </summary>
        /// <param name="format">日志格式内容</param>
        /// <param name="args">日志格式化参数</param>
        public static void Fatal(string format, params object[] args)
        {
            Instance._fatal_format_args?.Invoke(format, args);
        }

        /// <summary>
        /// 基于崩溃模式下的日志输出接口，参考<see cref="LogOutputLevelType.Fatal"/>类型定义
        /// </summary>
        /// <param name="condition">条件表达式</param>
        /// <param name="format">日志格式内容</param>
        /// <param name="args">日志格式化参数</param>
        public static void Fatal(bool condition, string format, params object[] args)
        {
            if (false == condition) Fatal(format, args);
        }

        /// <summary>
        /// 基于指定的日志级别和内容进行输出的接口函数
        /// </summary>
        /// <param name="level">日志级别</param>
        /// <param name="message">日志内容</param>
        public static void Output(LogOutputLevelType level, object message)
        {
            Instance._output_object?.Invoke(level, message);
        }

        /// <summary>
        /// 基于指定的日志级别和内容进行输出的接口函数
        /// </summary>
        /// <param name="level">日志级别</param>
        /// <param name="message">日志内容</param>
        public static void Output(LogOutputLevelType level, string message)
        {
            Instance._output_string?.Invoke(level, message);
        }

        /// <summary>
        /// 基于指定的日志级别和内容进行输出的接口函数
        /// </summary>
        /// <param name="level">日志级别</param>
        /// <param name="format">日志格式内容</param>
        /// <param name="args">日志格式化参数</param>
        public static void Output(LogOutputLevelType level, string format, params object[] args)
        {
            Instance._output_format_args?.Invoke(level, format, args);
        }
    }
}
