/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
/// Copyright (C) 2025, Hurley, Independent Studio.
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
using System.Runtime.CompilerServices;

namespace NovaEngine
{
    /// 调试器对象工具类
    internal partial class Debugger
    {
        /// <summary>
        /// 系统异常，仅在调试模式下该函数有效
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Throw()
        {
            Instance._throw_empty?.Invoke();
        }

        /// <summary>
        /// 系统异常，仅在调试模式下该函数有效
        /// </summary>
        /// <param name="errorCode">错误码</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Throw(int errorCode)
        {
            Instance._throw_code?.Invoke(errorCode);
        }

        /// <summary>
        /// 系统异常，仅在调试模式下该函数有效
        /// </summary>
        /// <param name="message">消息内容</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Throw(string message)
        {
            Instance._throw_string?.Invoke(message);
        }

        /// <summary>
        /// 系统异常，仅在调试模式下该函数有效
        /// </summary>
        /// <param name="format">格式内容</param>
        /// <param name="args">消息格式化参数</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Throw(string format, params object[] args)
        {
            Instance._throw_format_args?.Invoke(format, args);
        }

        /// <summary>
        /// 系统异常，仅在调试模式下该函数有效
        /// </summary>
        /// <param name="exception">异常实例</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Throw(Exception exception)
        {
            Instance._throw_exception?.Invoke(exception);
        }

        /// <summary>
        /// 系统异常，仅在调试模式下该函数有效
        /// </summary>
        /// <param name="type">异常类型</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Throw(Type type)
        {
            Instance._throw_type?.Invoke(type);
        }

        /// <summary>
        /// 系统异常，仅在调试模式下该函数有效
        /// </summary>
        /// <param name="type">异常类型</param>
        /// <param name="message">消息内容</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Throw(Type type, string message)
        {
            Instance._throw_type_string?.Invoke(type, message);
        }

        /// <summary>
        /// 系统异常，仅在调试模式下该函数有效
        /// </summary>
        /// <param name="type">异常类型</param>
        /// <param name="format">格式内容</param>
        /// <param name="args">消息格式化参数</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Throw(Type type, string format, params object[] args)
        {
            Instance._throw_type_format_args?.Invoke(type, format, args);
        }

        /// <summary>
        /// 系统异常，仅在调试模式下该函数有效
        /// </summary>
        /// <typeparam name="T">异常类型</typeparam>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Throw<T>() where T : Exception
        {
            Throw(typeof(T));
        }

        /// <summary>
        /// 系统异常，仅在调试模式下该函数有效
        /// </summary>
        /// <typeparam name="T">异常类型</typeparam>
        /// <param name="message">消息内容</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Throw<T>(string message) where T : Exception
        {
            Throw(typeof(T), message);
        }

        /// <summary>
        /// 系统异常，仅在调试模式下该函数有效
        /// </summary>
        /// <typeparam name="T">异常类型</typeparam>
        /// <param name="format">格式内容</param>
        /// <param name="args">消息格式化参数</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Throw<T>(string format, params object[] args) where T : Exception
        {
            Throw(typeof(T), format, args);
        }

        /// <summary>
        /// 对象条件判定的异常检查，仅在调试模式下该函数有效
        /// </summary>
        /// <param name="condition">条件表达式</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Throw(bool condition)
        {
            if (condition) Throw();
        }

        /// <summary>
        /// 对象条件判定的异常检查，仅在调试模式下该函数有效
        /// </summary>
        /// <param name="condition">条件表达式</param>
        /// <param name="errorCode">错误码</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Throw(bool condition, int errorCode)
        {
            if (condition) Throw(errorCode);
        }

        /// <summary>
        /// 对象条件判定的异常检查，仅在调试模式下该函数有效
        /// </summary>
        /// <param name="condition">条件表达式</param>
        /// <param name="message">消息内容</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Throw(bool condition, string message)
        {
            if (condition) Throw(message);
        }

        /// <summary>
        /// 对象条件判定的异常检查，仅在调试模式下该函数有效
        /// </summary>
        /// <param name="condition">条件表达式</param>
        /// <param name="format">格式内容</param>
        /// <param name="args">消息格式化参数</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Throw(bool condition, string format, params object[] args)
        {
            if (condition) Throw(format, args);
        }

        /// <summary>
        /// 系统异常，仅在调试模式下该函数有效
        /// </summary>
        /// <param name="condition">条件表达式</param>
        /// <param name="exception">异常实例</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Throw(bool condition, Exception exception)
        {
            if (condition) Throw(exception);
        }

        /// <summary>
        /// 系统异常，仅在调试模式下该函数有效
        /// </summary>
        /// <param name="condition">条件表达式</param>
        /// <param name="type">异常类型</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Throw(bool condition, Type type)
        {
            if (condition) Throw(type);
        }

        /// <summary>
        /// 系统异常，仅在调试模式下该函数有效
        /// </summary>
        /// <param name="condition">条件表达式</param>
        /// <param name="type">异常类型</param>
        /// <param name="message">消息内容</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Throw(bool condition, Type type, string message)
        {
            if (condition) Throw(type, message);
        }

        /// <summary>
        /// 系统异常，仅在调试模式下该函数有效
        /// </summary>
        /// <param name="condition">条件表达式</param>
        /// <param name="type">异常类型</param>
        /// <param name="format">格式内容</param>
        /// <param name="args">消息格式化参数</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Throw(bool condition, Type type, string format, params object[] args)
        {
            if (condition) Throw(type, format, args);
        }

        /// <summary>
        /// 系统异常，仅在调试模式下该函数有效
        /// </summary>
        /// <typeparam name="T">异常类型</typeparam>
        /// <param name="condition">条件表达式</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Throw<T>(bool condition) where T : Exception
        {
            if (condition) Throw<T>();
        }

        /// <summary>
        /// 系统异常，仅在调试模式下该函数有效
        /// </summary>
        /// <typeparam name="T">异常类型</typeparam>
        /// <param name="condition">条件表达式</param>
        /// <param name="message">消息内容</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Throw<T>(bool condition, string message) where T : Exception
        {
            if (condition) Throw<T>(message);
        }

        /// <summary>
        /// 系统异常，仅在调试模式下该函数有效
        /// </summary>
        /// <typeparam name="T">异常类型</typeparam>
        /// <param name="condition">条件表达式</param>
        /// <param name="format">格式内容</param>
        /// <param name="args">消息格式化参数</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Throw<T>(bool condition, string format, params object[] args) where T : Exception
        {
            if (condition) Throw<T>(format, args);
        }
    }
}
