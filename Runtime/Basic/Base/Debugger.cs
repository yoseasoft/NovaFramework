/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyring (C) 2023 - 2024, Guangzhou Shiyue Network Technology Co., Ltd.
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

using System.Diagnostics;

namespace GameEngine
{
    /// <summary>
    /// 应用层提供的调试对象类，它是基于对<see cref="NovaEngine.Debugger"/>的便捷性接口封装
    /// </summary>
    public static partial class Debugger
    {
        static Debugger()
        {
            // 重置调试输出
            ResettingDebuggingOutputHandler();
        }

        /// <summary>
        /// 基于调试模式下的日志输出接口，参考<see cref="NovaEngine.LogOutputLevelType.Debug"/>类型定义
        /// </summary>
        /// <param name="message">日志内容</param>
        [Conditional(NovaEngine.GlobalMacros.BUILD_CONFIGURATION_DEBUG)]
        public static void Log(object message)
        {
            s_logForObject(message);
        }

        /// <summary>
        /// 基于调试模式下的日志输出接口，参考<see cref="NovaEngine.LogOutputLevelType.Debug"/>类型定义
        /// </summary>
        /// <param name="message">日志内容</param>
        [Conditional(NovaEngine.GlobalMacros.BUILD_CONFIGURATION_DEBUG)]
        public static void Log(string message)
        {
            s_logForString(message);
        }

        /// <summary>
        /// 基于调试模式下的日志输出接口，参考<see cref="NovaEngine.LogOutputLevelType.Debug"/>类型定义
        /// </summary>
        /// <param name="condition">条件表达式</param>
        /// <param name="message">日志内容</param>
        [Conditional(NovaEngine.GlobalMacros.BUILD_CONFIGURATION_DEBUG)]
        public static void Log(bool condition, string message)
        {
            s_logForCondString(condition, message);
        }

        /// <summary>
        /// 基于调试模式下的日志输出接口，参考<see cref="NovaEngine.LogOutputLevelType.Debug"/>类型定义
        /// </summary>
        /// <param name="format">日志格式内容</param>
        /// <param name="args">日志格式化参数</param>
        [Conditional(NovaEngine.GlobalMacros.BUILD_CONFIGURATION_DEBUG)]
        public static void Log(string format, params object[] args)
        {
            s_logForFormatArgs(format, args);
        }

        /// <summary>
        /// 基于调试模式下的日志输出接口，参考<see cref="NovaEngine.LogOutputLevelType.Debug"/>类型定义
        /// </summary>
        /// <param name="condition">条件表达式</param>
        /// <param name="format">日志格式内容</param>
        /// <param name="args">日志格式化参数</param>
        [Conditional(NovaEngine.GlobalMacros.BUILD_CONFIGURATION_DEBUG)]
        public static void Log(bool condition, string format, params object[] args)
        {
            s_logForCondFormatArgs(condition, format, args);
        }

        /// <summary>
        /// 基于常规模式下的日志输出接口，参考<see cref="NovaEngine.LogOutputLevelType.Info"/>类型定义
        /// </summary>
        /// <param name="message">日志内容</param>
        public static void Info(object message)
        {
            s_infoForObject(message);
        }

        /// <summary>
        /// 基于常规模式下的日志输出接口，参考<see cref="NovaEngine.LogOutputLevelType.Info"/>类型定义
        /// </summary>
        /// <param name="message">日志内容</param>
        public static void Info(string message)
        {
            s_infoForString(message);
        }

        /// <summary>
        /// 基于常规模式下的日志输出接口，参考<see cref="NovaEngine.LogOutputLevelType.Info"/>类型定义
        /// </summary>
        /// <param name="condition">条件表达式</param>
        /// <param name="message">日志内容</param>
        public static void Info(bool condition, string message)
        {
            s_infoForCondString(condition, message);
        }

        /// <summary>
        /// 基于常规模式下的日志输出接口，参考<see cref="NovaEngine.LogOutputLevelType.Info"/>类型定义
        /// </summary>
        /// <param name="format">日志格式内容</param>
        /// <param name="args">日志格式化参数</param>
        public static void Info(string format, params object[] args)
        {
            s_infoForFormatArgs(format, args);
        }

        /// <summary>
        /// 基于常规模式下的日志输出接口，参考<see cref="NovaEngine.LogOutputLevelType.Info"/>类型定义
        /// </summary>
        /// <param name="condition">条件表达式</param>
        /// <param name="format">日志格式内容</param>
        /// <param name="args">日志格式化参数</param>
        public static void Info(bool condition, string format, params object[] args)
        {
            s_infoForCondFormatArgs(condition, format, args);
        }

        /// <summary>
        /// 基于警告模式下的日志输出接口，参考<see cref="NovaEngine.LogOutputLevelType.Warning"/>类型定义
        /// </summary>
        /// <param name="message">日志内容</param>
        public static void Warn(object message)
        {
            s_warnForObject(message);
        }

        /// <summary>
        /// 基于警告模式下的日志输出接口，参考<see cref="NovaEngine.LogOutputLevelType.Warning"/>类型定义
        /// </summary>
        /// <param name="message">日志内容</param>
        public static void Warn(string message)
        {
            s_warnForString(message);
        }

        /// <summary>
        /// 基于警告模式下的日志输出接口，参考<see cref="NovaEngine.LogOutputLevelType.Warning"/>类型定义
        /// </summary>
        /// <param name="condition">条件表达式</param>
        /// <param name="message">日志内容</param>
        public static void Warn(bool condition, string message)
        {
            s_warnForCondString(condition, message);
        }

        /// <summary>
        /// 基于警告模式下的日志输出接口，参考<see cref="NovaEngine.LogOutputLevelType.Warning"/>类型定义
        /// </summary>
        /// <param name="format">日志格式内容</param>
        /// <param name="args">日志格式化参数</param>
        public static void Warn(string format, params object[] args)
        {
            s_warnForFormatArgs(format, args);
        }

        /// <summary>
        /// 基于警告模式下的日志输出接口，参考<see cref="NovaEngine.LogOutputLevelType.Warning"/>类型定义
        /// </summary>
        /// <param name="condition">条件表达式</param>
        /// <param name="format">日志格式内容</param>
        /// <param name="args">日志格式化参数</param>
        public static void Warn(bool condition, string format, params object[] args)
        {
            s_warnForCondFormatArgs(condition, format, args);
        }

        /// <summary>
        /// 基于错误模式下的日志输出接口，参考<see cref="NovaEngine.LogOutputLevelType.Error"/>类型定义
        /// </summary>
        /// <param name="message">日志内容</param>
        public static void Error(object message)
        {
            s_errorForObject(message);
        }

        /// <summary>
        /// 基于错误模式下的日志输出接口，参考<see cref="NovaEngine.LogOutputLevelType.Error"/>类型定义
        /// </summary>
        /// <param name="message">日志内容</param>
        public static void Error(string message)
        {
            s_errorForString(message);
        }

        /// <summary>
        /// 基于错误模式下的日志输出接口，参考<see cref="NovaEngine.LogOutputLevelType.Error"/>类型定义
        /// </summary>
        /// <param name="condition">条件表达式</param>
        /// <param name="message">日志内容</param>
        public static void Error(bool condition, string message)
        {
            s_errorForCondString(condition, message);
        }

        /// <summary>
        /// 基于错误模式下的日志输出接口，参考<see cref="NovaEngine.LogOutputLevelType.Error"/>类型定义
        /// </summary>
        /// <param name="format">日志格式内容</param>
        /// <param name="args">日志格式化参数</param>
        public static void Error(string format, params object[] args)
        {
            s_errorForFormatArgs(format, args);
        }

        /// <summary>
        /// 基于错误模式下的日志输出接口，参考<see cref="NovaEngine.LogOutputLevelType.Error"/>类型定义
        /// </summary>
        /// <param name="condition">条件表达式</param>
        /// <param name="format">日志格式内容</param>
        /// <param name="args">日志格式化参数</param>
        public static void Error(bool condition, string format, params object[] args)
        {
            s_errorForCondFormatArgs(condition, format, args);
        }

        /// <summary>
        /// 基于崩溃模式下的日志输出接口，参考<see cref="NovaEngine.LogOutputLevelType.Fatal"/>类型定义
        /// </summary>
        /// <param name="message">日志内容</param>
        public static void Fatal(object message)
        {
            s_fatalForObject(message);
        }

        /// <summary>
        /// 基于崩溃模式下的日志输出接口，参考<see cref="NovaEngine.LogOutputLevelType.Fatal"/>类型定义
        /// </summary>
        /// <param name="message">日志内容</param>
        public static void Fatal(string message)
        {
            s_fatalForString(message);
        }

        /// <summary>
        /// 基于崩溃模式下的日志输出接口，参考<see cref="NovaEngine.LogOutputLevelType.Fatal"/>类型定义
        /// </summary>
        /// <param name="condition">条件表达式</param>
        /// <param name="message">日志内容</param>
        public static void Fatal(bool condition, string message)
        {
            s_fatalForCondString(condition, message);
        }

        /// <summary>
        /// 基于崩溃模式下的日志输出接口，参考<see cref="NovaEngine.LogOutputLevelType.Fatal"/>类型定义
        /// </summary>
        /// <param name="format">日志格式内容</param>
        /// <param name="args">日志格式化参数</param>
        public static void Fatal(string format, params object[] args)
        {
            s_fatalForFormatArgs(format, args);
        }

        /// <summary>
        /// 基于崩溃模式下的日志输出接口，参考<see cref="NovaEngine.LogOutputLevelType.Fatal"/>类型定义
        /// </summary>
        /// <param name="condition">条件表达式</param>
        /// <param name="format">日志格式内容</param>
        /// <param name="args">日志格式化参数</param>
        public static void Fatal(bool condition, string format, params object[] args)
        {
            s_fatalForCondFormatArgs(condition, format, args);
        }

        /// <summary>
        /// 系统断言，仅在调试模式下该函数有效
        /// </summary>
        /// <param name="condition">条件表达式</param>
        public static void Assert(bool condition)
        {
            NovaEngine.Debugger.Assert(condition);
        }

        /// <summary>
        /// 系统断言，仅在调试模式下该函数有效
        /// </summary>
        /// <param name="condition">条件表达式</param>
        /// <param name="message">消息内容</param>
        public static void Assert(bool condition, object message)
        {
            NovaEngine.Debugger.Assert(condition, message);
        }

        /// <summary>
        /// 系统断言，仅在调试模式下该函数有效
        /// </summary>
        /// <param name="condition">条件表达式</param>
        /// <param name="message">消息内容</param>
        public static void Assert(bool condition, string message)
        {
            NovaEngine.Debugger.Assert(condition, message);
        }

        /// <summary>
        /// 系统断言，仅在调试模式下该函数有效
        /// </summary>
        /// <param name="condition">条件表达式</param>
        /// <param name="format">格式内容</param>
        /// <param name="args">消息格式化参数</param>
        public static void Assert(bool condition, string format, params object[] args)
        {
            NovaEngine.Debugger.Assert(condition, format, args);
        }

        /// <summary>
        /// 对象非空的断言检查，仅在调试模式下该函数有效
        /// </summary>
        /// <param name="obj">对象实例</param>
        public static void Assert(object obj)
        {
            NovaEngine.Debugger.Assert(obj);
        }

        /// <summary>
        /// 对象非空的断言检查，仅在调试模式下该函数有效
        /// </summary>
        /// <param name="obj">对象实例</param>
        /// <param name="message">消息内容</param>
        public static void Assert(object obj, object message)
        {
            NovaEngine.Debugger.Assert(obj, message);
        }

        /// <summary>
        /// 对象非空的断言检查，仅在调试模式下该函数有效
        /// </summary>
        /// <param name="obj">对象实例</param>
        /// <param name="message">消息内容</param>
        public static void Assert(object obj, string message)
        {
            NovaEngine.Debugger.Assert(obj, message);
        }

        /// <summary>
        /// 对象非空的断言检查，仅在调试模式下该函数有效
        /// </summary>
        /// <param name="obj">对象实例</param>
        /// <param name="format">格式内容</param>
        /// <param name="args">消息格式化参数</param>
        public static void Assert(object obj, string format, params object[] args)
        {
            NovaEngine.Debugger.Assert(obj, format, args);
        }

        /// <summary>
        /// 系统默认提供的日志输出接口，用于在引擎尚未初始化完成前提供输出
        /// </summary>
        /// <param name="message">日志内容</param>
        private static void Unity_Output(object message)
        {
            UnityEngine.Debug.Log(message);
        }

        /// <summary>
        /// 系统默认提供的日志输出接口，用于在引擎尚未初始化完成前提供输出
        /// </summary>
        /// <param name="message">日志内容</param>
        private static void Unity_Output(string message)
        {
            UnityEngine.Debug.Log(message);
        }

        /// <summary>
        /// 系统默认提供的日志输出接口，用于在引擎尚未初始化完成前提供输出
        /// </summary>
        /// <param name="condition">条件表达式</param>
        /// <param name="message">日志内容</param>
        private static void Unity_Output(bool condition, string message)
        {
            if (false == condition) Unity_Output(message);
        }

        /// <summary>
        /// 系统默认提供的日志输出接口，用于在引擎尚未初始化完成前提供输出
        /// </summary>
        /// <param name="format">日志格式内容</param>
        /// <param name="args">日志格式化参数</param>
        private static void Unity_Output(string format, params object[] args)
        {
            UnityEngine.Debug.Log(NovaEngine.Utility.Text.Format(format, args));
        }

        /// <summary>
        /// 系统默认提供的日志输出接口，用于在引擎尚未初始化完成前提供输出
        /// </summary>
        /// <param name="condition">条件表达式</param>
        /// <param name="format">日志格式内容</param>
        /// <param name="args">日志格式化参数</param>
        private static void Unity_Output(bool condition, string format, params object[] args)
        {
            if (false == condition) Unity_Output(format, args);
        }
    }
}
