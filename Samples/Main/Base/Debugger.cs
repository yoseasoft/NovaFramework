/// -------------------------------------------------------------------------------
/// NovaEngine Framework Samples
///
/// Copyright (C) 2024 - 2025, Hurley, Independent Studio.
/// Copyright (C) 2025, Hainan Yuanyou Information Tecdhnology Co., Ltd. Guangzhou Branch
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

namespace GameEngine.Sample
{
    /// <summary>
    /// 游戏层提供的调试对象类，它是基于对<see cref="GameEngine.Debugger"/>的便捷性接口封装
    /// </summary>
    public static class Debugger
    {
        /// <summary>
        /// 基于调试模式下的日志输出接口
        /// </summary>
        /// <param name="message">日志内容</param>
        public static void Log(object message)
        {
            GameEngine.Debugger.Log(message);
        }

        /// <summary>
        /// 基于调试模式下的日志输出接口
        /// </summary>
        /// <param name="message">日志内容</param>
        public static void Log(string message)
        {
            GameEngine.Debugger.Log(message);
        }

        /// <summary>
        /// 基于调试模式下的日志输出接口
        /// </summary>
        /// <param name="format">日志格式内容</param>
        /// <param name="args">日志格式化参数</param>
        public static void Log(string format, params object[] args)
        {
            GameEngine.Debugger.Log(format, args);
        }

        /// <summary>
        /// 基于常规模式下的日志输出接口
        /// </summary>
        /// <param name="message">日志内容</param>
        public static void Info(object message)
        {
            GameEngine.Debugger.Info(message);
        }

        /// <summary>
        /// 基于常规模式下的日志输出接口
        /// </summary>
        /// <param name="message">日志内容</param>
        public static void Info(string message)
        {
            GameEngine.Debugger.Info(message);
        }

        /// <summary>
        /// 基于常规模式下的日志输出接口
        /// </summary>
        /// <param name="format">日志格式内容</param>
        /// <param name="args">日志格式化参数</param>
        public static void Info(string format, params object[] args)
        {
            GameEngine.Debugger.Info(format, args);
        }

        /// <summary>
        /// 基于警告模式下的日志输出接口
        /// </summary>
        /// <param name="message">日志内容</param>
        public static void Warn(object message)
        {
            GameEngine.Debugger.Warn(message);
        }

        /// <summary>
        /// 基于警告模式下的日志输出接口
        /// </summary>
        /// <param name="message">日志内容</param>
        public static void Warn(string message)
        {
            GameEngine.Debugger.Warn(message);
        }

        /// <summary>
        /// 基于警告模式下的日志输出接口
        /// </summary>
        /// <param name="format">日志格式内容</param>
        /// <param name="args">日志格式化参数</param>
        public static void Warn(string format, params object[] args)
        {
            GameEngine.Debugger.Warn(format, args);
        }

        /// <summary>
        /// 基于错误模式下的日志输出接口
        /// </summary>
        /// <param name="message">日志内容</param>
        public static void Error(object message)
        {
            GameEngine.Debugger.Error(message);
        }

        /// <summary>
        /// 基于错误模式下的日志输出接口
        /// </summary>
        /// <param name="message">日志内容</param>
        public static void Error(string message)
        {
            GameEngine.Debugger.Error(message);
        }

        /// <summary>
        /// 基于错误模式下的日志输出接口
        /// </summary>
        /// <param name="format">日志格式内容</param>
        /// <param name="args">日志格式化参数</param>
        public static void Error(string format, params object[] args)
        {
            GameEngine.Debugger.Error(format, args);
        }

        /// <summary>
        /// 基于崩溃模式下的日志输出接口
        /// </summary>
        /// <param name="message">日志内容</param>
        public static void Fatal(object message)
        {
            GameEngine.Debugger.Fatal(message);
        }

        /// <summary>
        /// 基于崩溃模式下的日志输出接口
        /// </summary>
        /// <param name="message">日志内容</param>
        public static void Fatal(string message)
        {
            GameEngine.Debugger.Fatal(message);
        }

        /// <summary>
        /// 基于崩溃模式下的日志输出接口
        /// </summary>
        /// <param name="format">日志格式内容</param>
        /// <param name="args">日志格式化参数</param>
        public static void Fatal(string format, params object[] args)
        {
            GameEngine.Debugger.Fatal(format, args);
        }

        #region 断言操作相关的接口函数

        /// <summary>
        /// 系统断言，仅在调试模式下该函数有效
        /// </summary>
        /// <param name="condition">条件表达式</param>
        public static void Assert(bool condition)
        {
            GameEngine.Debugger.Assert(condition);
        }

        /// <summary>
        /// 系统断言，仅在调试模式下该函数有效
        /// </summary>
        /// <param name="condition">条件表达式</param>
        /// <param name="message">消息内容</param>
        public static void Assert(bool condition, object message)
        {
            GameEngine.Debugger.Assert(condition, message);
        }

        /// <summary>
        /// 系统断言，仅在调试模式下该函数有效
        /// </summary>
        /// <param name="condition">条件表达式</param>
        /// <param name="message">消息内容</param>
        public static void Assert(bool condition, string message)
        {
            GameEngine.Debugger.Assert(condition, message);
        }

        /// <summary>
        /// 系统断言，仅在调试模式下该函数有效
        /// </summary>
        /// <param name="condition">条件表达式</param>
        /// <param name="format">格式内容</param>
        /// <param name="args">消息格式化参数</param>
        public static void Assert(bool condition, string format, params object[] args)
        {
            GameEngine.Debugger.Assert(condition, format, args);
        }

        #endregion

        #region 异常操作相关的接口函数

        /// <summary>
        /// 系统异常，仅在调试模式下该函数有效
        /// </summary>
        public static void Throw()
        {
            GameEngine.Debugger.Throw();
        }

        /// <summary>
        /// 系统异常，仅在调试模式下该函数有效
        /// </summary>
        /// <param name="errorCode">错误码</param>
        public static void Throw(int errorCode)
        {
            GameEngine.Debugger.Throw(errorCode);
        }

        /// <summary>
        /// 系统异常，仅在调试模式下该函数有效
        /// </summary>
        /// <param name="message">消息内容</param>
        public static void Throw(string message)
        {
            GameEngine.Debugger.Throw(message);
        }

        /// <summary>
        /// 系统异常，仅在调试模式下该函数有效
        /// </summary>
        /// <param name="format">格式内容</param>
        /// <param name="args">消息格式化参数</param>
        public static void Throw(string format, params object[] args)
        {
            GameEngine.Debugger.Throw(format, args);
        }

        /// <summary>
        /// 系统异常，仅在调试模式下该函数有效
        /// </summary>
        /// <param name="exception">异常实例</param>
        public static void Throw(System.Exception exception)
        {
            GameEngine.Debugger.Throw(exception);
        }

        /// <summary>
        /// 系统异常，仅在调试模式下该函数有效
        /// </summary>
        /// <param name="type">异常类型</param>
        public static void Throw(System.Type type)
        {
            GameEngine.Debugger.Throw(type);
        }

        /// <summary>
        /// 系统异常，仅在调试模式下该函数有效
        /// </summary>
        /// <param name="type">异常类型</param>
        /// <param name="message">消息内容</param>
        public static void Throw(System.Type type, string message)
        {
            GameEngine.Debugger.Throw(type, message);
        }

        /// <summary>
        /// 系统异常，仅在调试模式下该函数有效
        /// </summary>
        /// <param name="type">异常类型</param>
        /// <param name="format">格式内容</param>
        /// <param name="args">消息格式化参数</param>
        public static void Throw(System.Type type, string format, params object[] args)
        {
            GameEngine.Debugger.Throw(type, format, args);
        }

        /// <summary>
        /// 系统异常，仅在调试模式下该函数有效
        /// </summary>
        /// <typeparam name="T">异常类型</typeparam>
        public static void Throw<T>() where T : System.Exception
        {
            GameEngine.Debugger.Throw<T>();
        }

        /// <summary>
        /// 系统异常，仅在调试模式下该函数有效
        /// </summary>
        /// <typeparam name="T">异常类型</typeparam>
        /// <param name="message">消息内容</param>
        public static void Throw<T>(string message) where T : System.Exception
        {
            GameEngine.Debugger.Throw<T>(message);
        }

        /// <summary>
        /// 系统异常，仅在调试模式下该函数有效
        /// </summary>
        /// <typeparam name="T">异常类型</typeparam>
        /// <param name="format">格式内容</param>
        /// <param name="args">消息格式化参数</param>
        public static void Throw<T>(string format, params object[] args) where T : System.Exception
        {
            GameEngine.Debugger.Throw<T>(format, args);
        }

        /// <summary>
        /// 对象条件判定的异常检查，仅在调试模式下该函数有效
        /// </summary>
        /// <param name="condition">条件表达式</param>
        public static void Throw(bool condition)
        {
            GameEngine.Debugger.Throw(condition);
        }

        /// <summary>
        /// 对象条件判定的异常检查，仅在调试模式下该函数有效
        /// </summary>
        /// <param name="condition">条件表达式</param>
        /// <param name="errorCode">错误码</param>
        public static void Throw(bool condition, int errorCode)
        {
            GameEngine.Debugger.Throw(condition, errorCode);
        }

        /// <summary>
        /// 对象条件判定的异常检查，仅在调试模式下该函数有效
        /// </summary>
        /// <param name="condition">条件表达式</param>
        /// <param name="message">消息内容</param>
        public static void Throw(bool condition, string message)
        {
            GameEngine.Debugger.Throw(condition, message);
        }

        /// <summary>
        /// 对象条件判定的异常检查，仅在调试模式下该函数有效
        /// </summary>
        /// <param name="condition">条件表达式</param>
        /// <param name="format">格式内容</param>
        /// <param name="args">消息格式化参数</param>
        public static void Throw(bool condition, string format, params object[] args)
        {
            GameEngine.Debugger.Throw(condition, format, args);
        }

        /// <summary>
        /// 系统异常，仅在调试模式下该函数有效
        /// </summary>
        /// <param name="condition">条件表达式</param>
        /// <param name="exception">异常实例</param>
        public static void Throw(bool condition, System.Exception exception)
        {
            GameEngine.Debugger.Throw(condition, exception);
        }

        /// <summary>
        /// 系统异常，仅在调试模式下该函数有效
        /// </summary>
        /// <param name="condition">条件表达式</param>
        /// <param name="type">异常类型</param>
        public static void Throw(bool condition, System.Type type)
        {
            GameEngine.Debugger.Throw(condition, type);
        }

        /// <summary>
        /// 系统异常，仅在调试模式下该函数有效
        /// </summary>
        /// <param name="condition">条件表达式</param>
        /// <param name="type">异常类型</param>
        /// <param name="message">消息内容</param>
        public static void Throw(bool condition, System.Type type, string message)
        {
            GameEngine.Debugger.Throw(condition, type, message);
        }

        /// <summary>
        /// 系统异常，仅在调试模式下该函数有效
        /// </summary>
        /// <param name="condition">条件表达式</param>
        /// <param name="type">异常类型</param>
        /// <param name="format">格式内容</param>
        /// <param name="args">消息格式化参数</param>
        public static void Throw(bool condition, System.Type type, string format, params object[] args)
        {
            GameEngine.Debugger.Throw(condition, type, format, args);
        }

        /// <summary>
        /// 系统异常，仅在调试模式下该函数有效
        /// </summary>
        /// <typeparam name="T">异常类型</typeparam>
        /// <param name="condition">条件表达式</param>
        public static void Throw<T>(bool condition) where T : System.Exception
        {
            GameEngine.Debugger.Throw<T>(condition);
        }

        /// <summary>
        /// 系统异常，仅在调试模式下该函数有效
        /// </summary>
        /// <typeparam name="T">异常类型</typeparam>
        /// <param name="condition">条件表达式</param>
        /// <param name="message">消息内容</param>
        public static void Throw<T>(bool condition, string message) where T : System.Exception
        {
            GameEngine.Debugger.Throw<T>(condition, message);
        }

        /// <summary>
        /// 系统异常，仅在调试模式下该函数有效
        /// </summary>
        /// <typeparam name="T">异常类型</typeparam>
        /// <param name="condition">条件表达式</param>
        /// <param name="format">格式内容</param>
        /// <param name="args">消息格式化参数</param>
        public static void Throw<T>(bool condition, string format, params object[] args) where T : System.Exception
        {
            GameEngine.Debugger.Throw<T>(condition, format, args);
        }

        #endregion
    }
}
