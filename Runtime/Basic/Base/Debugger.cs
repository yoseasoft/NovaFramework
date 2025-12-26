/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyright (C) 2023 - 2024, Guangzhou Shiyue Network Technology Co., Ltd.
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

using System;

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
            ResettingDebugOutputHandler();
        }

        /// <summary>
        /// 基于调试模式下的日志输出接口，参考<see cref="NovaEngine.LogOutputLevelType.Debug"/>类型定义
        /// </summary>
        /// <param name="message">日志内容</param>
        //[Conditional(NovaEngine.GlobalMacros.BUILD_CONFIGURATION_DEBUG)]
        public static void Log(object message)
        {
            _logForObject(message);
        }

        /// <summary>
        /// 基于调试模式下的日志输出接口，参考<see cref="NovaEngine.LogOutputLevelType.Debug"/>类型定义
        /// </summary>
        /// <param name="message">日志内容</param>
        //[Conditional(NovaEngine.GlobalMacros.BUILD_CONFIGURATION_DEBUG)]
        public static void Log(string message)
        {
            _logForString(message);
        }

        /// <summary>
        /// 基于调试模式下的日志输出接口，参考<see cref="NovaEngine.LogOutputLevelType.Debug"/>类型定义
        /// </summary>
        /// <param name="condition">条件表达式</param>
        /// <param name="message">日志内容</param>
        //[Conditional(NovaEngine.GlobalMacros.BUILD_CONFIGURATION_DEBUG)]
        public static void Log(bool condition, string message)
        {
            _logForCondString(condition, message);
        }

        /// <summary>
        /// 基于调试模式下的日志输出接口，参考<see cref="NovaEngine.LogOutputLevelType.Debug"/>类型定义
        /// </summary>
        /// <param name="format">日志格式内容</param>
        /// <param name="args">日志格式化参数</param>
        //[Conditional(NovaEngine.GlobalMacros.BUILD_CONFIGURATION_DEBUG)]
        public static void Log(string format, params object[] args)
        {
            _logForFormatArgs(format, args);
        }

        /// <summary>
        /// 基于调试模式下的日志输出接口，参考<see cref="NovaEngine.LogOutputLevelType.Debug"/>类型定义
        /// </summary>
        /// <param name="condition">条件表达式</param>
        /// <param name="format">日志格式内容</param>
        /// <param name="args">日志格式化参数</param>
        //[Conditional(NovaEngine.GlobalMacros.BUILD_CONFIGURATION_DEBUG)]
        public static void Log(bool condition, string format, params object[] args)
        {
            _logForCondFormatArgs(condition, format, args);
        }

        /// <summary>
        /// 基于常规模式下的日志输出接口，参考<see cref="NovaEngine.LogOutputLevelType.Info"/>类型定义
        /// </summary>
        /// <param name="message">日志内容</param>
        public static void Info(object message)
        {
            _infoForObject(message);
        }

        /// <summary>
        /// 基于常规模式下的日志输出接口，参考<see cref="NovaEngine.LogOutputLevelType.Info"/>类型定义
        /// </summary>
        /// <param name="message">日志内容</param>
        public static void Info(string message)
        {
            _infoForString(message);
        }

        /// <summary>
        /// 基于常规模式下的日志输出接口，参考<see cref="NovaEngine.LogOutputLevelType.Info"/>类型定义
        /// </summary>
        /// <param name="condition">条件表达式</param>
        /// <param name="message">日志内容</param>
        public static void Info(bool condition, string message)
        {
            _infoForCondString(condition, message);
        }

        /// <summary>
        /// 基于常规模式下的日志输出接口，参考<see cref="NovaEngine.LogOutputLevelType.Info"/>类型定义
        /// </summary>
        /// <param name="format">日志格式内容</param>
        /// <param name="args">日志格式化参数</param>
        public static void Info(string format, params object[] args)
        {
            _infoForFormatArgs(format, args);
        }

        /// <summary>
        /// 基于常规模式下的日志输出接口，参考<see cref="NovaEngine.LogOutputLevelType.Info"/>类型定义
        /// </summary>
        /// <param name="condition">条件表达式</param>
        /// <param name="format">日志格式内容</param>
        /// <param name="args">日志格式化参数</param>
        public static void Info(bool condition, string format, params object[] args)
        {
            _infoForCondFormatArgs(condition, format, args);
        }

        /// <summary>
        /// 基于警告模式下的日志输出接口，参考<see cref="NovaEngine.LogOutputLevelType.Warning"/>类型定义
        /// </summary>
        /// <param name="message">日志内容</param>
        public static void Warn(object message)
        {
            _warnForObject(message);
        }

        /// <summary>
        /// 基于警告模式下的日志输出接口，参考<see cref="NovaEngine.LogOutputLevelType.Warning"/>类型定义
        /// </summary>
        /// <param name="message">日志内容</param>
        public static void Warn(string message)
        {
            _warnForString(message);
        }

        /// <summary>
        /// 基于警告模式下的日志输出接口，参考<see cref="NovaEngine.LogOutputLevelType.Warning"/>类型定义
        /// </summary>
        /// <param name="condition">条件表达式</param>
        /// <param name="message">日志内容</param>
        public static void Warn(bool condition, string message)
        {
            _warnForCondString(condition, message);
        }

        /// <summary>
        /// 基于警告模式下的日志输出接口，参考<see cref="NovaEngine.LogOutputLevelType.Warning"/>类型定义
        /// </summary>
        /// <param name="format">日志格式内容</param>
        /// <param name="args">日志格式化参数</param>
        public static void Warn(string format, params object[] args)
        {
            _warnForFormatArgs(format, args);
        }

        /// <summary>
        /// 基于警告模式下的日志输出接口，参考<see cref="NovaEngine.LogOutputLevelType.Warning"/>类型定义
        /// </summary>
        /// <param name="condition">条件表达式</param>
        /// <param name="format">日志格式内容</param>
        /// <param name="args">日志格式化参数</param>
        public static void Warn(bool condition, string format, params object[] args)
        {
            _warnForCondFormatArgs(condition, format, args);
        }

        /// <summary>
        /// 基于错误模式下的日志输出接口，参考<see cref="NovaEngine.LogOutputLevelType.Error"/>类型定义
        /// </summary>
        /// <param name="message">日志内容</param>
        public static void Error(object message)
        {
            _errorForObject(message);
        }

        /// <summary>
        /// 基于错误模式下的日志输出接口，参考<see cref="NovaEngine.LogOutputLevelType.Error"/>类型定义
        /// </summary>
        /// <param name="message">日志内容</param>
        public static void Error(string message)
        {
            _errorForString(message);
        }

        /// <summary>
        /// 基于错误模式下的日志输出接口，参考<see cref="NovaEngine.LogOutputLevelType.Error"/>类型定义
        /// </summary>
        /// <param name="condition">条件表达式</param>
        /// <param name="message">日志内容</param>
        public static void Error(bool condition, string message)
        {
            _errorForCondString(condition, message);
        }

        /// <summary>
        /// 基于错误模式下的日志输出接口，参考<see cref="NovaEngine.LogOutputLevelType.Error"/>类型定义
        /// </summary>
        /// <param name="format">日志格式内容</param>
        /// <param name="args">日志格式化参数</param>
        public static void Error(string format, params object[] args)
        {
            _errorForFormatArgs(format, args);
        }

        /// <summary>
        /// 基于错误模式下的日志输出接口，参考<see cref="NovaEngine.LogOutputLevelType.Error"/>类型定义
        /// </summary>
        /// <param name="condition">条件表达式</param>
        /// <param name="format">日志格式内容</param>
        /// <param name="args">日志格式化参数</param>
        public static void Error(bool condition, string format, params object[] args)
        {
            _errorForCondFormatArgs(condition, format, args);
        }

        /// <summary>
        /// 基于崩溃模式下的日志输出接口，参考<see cref="NovaEngine.LogOutputLevelType.Fatal"/>类型定义
        /// </summary>
        /// <param name="message">日志内容</param>
        public static void Fatal(object message)
        {
            _fatalForObject(message);
        }

        /// <summary>
        /// 基于崩溃模式下的日志输出接口，参考<see cref="NovaEngine.LogOutputLevelType.Fatal"/>类型定义
        /// </summary>
        /// <param name="message">日志内容</param>
        public static void Fatal(string message)
        {
            _fatalForString(message);
        }

        /// <summary>
        /// 基于崩溃模式下的日志输出接口，参考<see cref="NovaEngine.LogOutputLevelType.Fatal"/>类型定义
        /// </summary>
        /// <param name="condition">条件表达式</param>
        /// <param name="message">日志内容</param>
        public static void Fatal(bool condition, string message)
        {
            _fatalForCondString(condition, message);
        }

        /// <summary>
        /// 基于崩溃模式下的日志输出接口，参考<see cref="NovaEngine.LogOutputLevelType.Fatal"/>类型定义
        /// </summary>
        /// <param name="format">日志格式内容</param>
        /// <param name="args">日志格式化参数</param>
        public static void Fatal(string format, params object[] args)
        {
            _fatalForFormatArgs(format, args);
        }

        /// <summary>
        /// 基于崩溃模式下的日志输出接口，参考<see cref="NovaEngine.LogOutputLevelType.Fatal"/>类型定义
        /// </summary>
        /// <param name="condition">条件表达式</param>
        /// <param name="format">日志格式内容</param>
        /// <param name="args">日志格式化参数</param>
        public static void Fatal(bool condition, string format, params object[] args)
        {
            _fatalForCondFormatArgs(condition, format, args);
        }

        #region 断言操作相关的接口函数

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

        #endregion

        #region 异常操作相关的接口函数

        /// <summary>
        /// 系统异常，仅在调试模式下该函数有效
        /// </summary>
        public static void Throw()
        {
            NovaEngine.Debugger.Throw();
        }

        /// <summary>
        /// 系统异常，仅在调试模式下该函数有效
        /// </summary>
        /// <param name="errorCode">错误码</param>
        public static void Throw(int errorCode)
        {
            NovaEngine.Debugger.Throw(errorCode);
        }

        /// <summary>
        /// 系统异常，仅在调试模式下该函数有效
        /// </summary>
        /// <param name="message">消息内容</param>
        public static void Throw(string message)
        {
            NovaEngine.Debugger.Throw(message);
        }

        /// <summary>
        /// 系统异常，仅在调试模式下该函数有效
        /// </summary>
        /// <param name="format">格式内容</param>
        /// <param name="args">消息格式化参数</param>
        public static void Throw(string format, params object[] args)
        {
            NovaEngine.Debugger.Throw(format, args);
        }

        /// <summary>
        /// 系统异常，仅在调试模式下该函数有效
        /// </summary>
        /// <param name="exception">异常实例</param>
        public static void Throw(Exception exception)
        {
            NovaEngine.Debugger.Throw(exception);
        }

        /// <summary>
        /// 系统异常，仅在调试模式下该函数有效
        /// </summary>
        /// <param name="type">异常类型</param>
        public static void Throw(Type type)
        {
            NovaEngine.Debugger.Throw(type);
        }

        /// <summary>
        /// 系统异常，仅在调试模式下该函数有效
        /// </summary>
        /// <param name="type">异常类型</param>
        /// <param name="message">消息内容</param>
        public static void Throw(Type type, string message)
        {
            NovaEngine.Debugger.Throw(type, message);
        }

        /// <summary>
        /// 系统异常，仅在调试模式下该函数有效
        /// </summary>
        /// <param name="type">异常类型</param>
        /// <param name="format">格式内容</param>
        /// <param name="args">消息格式化参数</param>
        public static void Throw(Type type, string format, params object[] args)
        {
            NovaEngine.Debugger.Throw(type, format, args);
        }

        /// <summary>
        /// 系统异常，仅在调试模式下该函数有效
        /// </summary>
        /// <typeparam name="T">异常类型</typeparam>
        public static void Throw<T>() where T : Exception
        {
            NovaEngine.Debugger.Throw<T>();
        }

        /// <summary>
        /// 系统异常，仅在调试模式下该函数有效
        /// </summary>
        /// <typeparam name="T">异常类型</typeparam>
        /// <param name="message">消息内容</param>
        public static void Throw<T>(string message) where T : Exception
        {
            NovaEngine.Debugger.Throw<T>(message);
        }

        /// <summary>
        /// 系统异常，仅在调试模式下该函数有效
        /// </summary>
        /// <typeparam name="T">异常类型</typeparam>
        /// <param name="format">格式内容</param>
        /// <param name="args">消息格式化参数</param>
        public static void Throw<T>(string format, params object[] args) where T : Exception
        {
            NovaEngine.Debugger.Throw<T>(format, args);
        }

        /// <summary>
        /// 对象条件判定的异常检查，仅在调试模式下该函数有效
        /// </summary>
        /// <param name="condition">条件表达式</param>
        public static void Throw(bool condition)
        {
            NovaEngine.Debugger.Throw(condition);
        }

        /// <summary>
        /// 对象条件判定的异常检查，仅在调试模式下该函数有效
        /// </summary>
        /// <param name="condition">条件表达式</param>
        /// <param name="errorCode">错误码</param>
        public static void Throw(bool condition, int errorCode)
        {
            NovaEngine.Debugger.Throw(condition, errorCode);
        }

        /// <summary>
        /// 对象条件判定的异常检查，仅在调试模式下该函数有效
        /// </summary>
        /// <param name="condition">条件表达式</param>
        /// <param name="message">消息内容</param>
        public static void Throw(bool condition, string message)
        {
            NovaEngine.Debugger.Throw(condition, message);
        }

        /// <summary>
        /// 对象条件判定的异常检查，仅在调试模式下该函数有效
        /// </summary>
        /// <param name="condition">条件表达式</param>
        /// <param name="format">格式内容</param>
        /// <param name="args">消息格式化参数</param>
        public static void Throw(bool condition, string format, params object[] args)
        {
            NovaEngine.Debugger.Throw(condition, format, args);
        }

        /// <summary>
        /// 系统异常，仅在调试模式下该函数有效
        /// </summary>
        /// <param name="condition">条件表达式</param>
        /// <param name="exception">异常实例</param>
        public static void Throw(bool condition, Exception exception)
        {
            NovaEngine.Debugger.Throw(condition, exception);
        }

        /// <summary>
        /// 系统异常，仅在调试模式下该函数有效
        /// </summary>
        /// <param name="condition">条件表达式</param>
        /// <param name="type">异常类型</param>
        public static void Throw(bool condition, Type type)
        {
            NovaEngine.Debugger.Throw(condition, type);
        }

        /// <summary>
        /// 系统异常，仅在调试模式下该函数有效
        /// </summary>
        /// <param name="condition">条件表达式</param>
        /// <param name="type">异常类型</param>
        /// <param name="message">消息内容</param>
        public static void Throw(bool condition, Type type, string message)
        {
            NovaEngine.Debugger.Throw(condition, type, message);
        }

        /// <summary>
        /// 系统异常，仅在调试模式下该函数有效
        /// </summary>
        /// <param name="condition">条件表达式</param>
        /// <param name="type">异常类型</param>
        /// <param name="format">格式内容</param>
        /// <param name="args">消息格式化参数</param>
        public static void Throw(bool condition, Type type, string format, params object[] args)
        {
            NovaEngine.Debugger.Throw(condition, type, format, args);
        }

        /// <summary>
        /// 系统异常，仅在调试模式下该函数有效
        /// </summary>
        /// <typeparam name="T">异常类型</typeparam>
        /// <param name="condition">条件表达式</param>
        public static void Throw<T>(bool condition) where T : Exception
        {
            NovaEngine.Debugger.Throw<T>(condition);
        }

        /// <summary>
        /// 系统异常，仅在调试模式下该函数有效
        /// </summary>
        /// <typeparam name="T">异常类型</typeparam>
        /// <param name="condition">条件表达式</param>
        /// <param name="message">消息内容</param>
        public static void Throw<T>(bool condition, string message) where T : Exception
        {
            NovaEngine.Debugger.Throw<T>(condition, message);
        }

        /// <summary>
        /// 系统异常，仅在调试模式下该函数有效
        /// </summary>
        /// <typeparam name="T">异常类型</typeparam>
        /// <param name="condition">条件表达式</param>
        /// <param name="format">格式内容</param>
        /// <param name="args">消息格式化参数</param>
        public static void Throw<T>(bool condition, string format, params object[] args) where T : Exception
        {
            NovaEngine.Debugger.Throw<T>(condition, format, args);
        }

        #endregion

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
