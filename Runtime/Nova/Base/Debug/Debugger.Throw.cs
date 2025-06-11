/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
/// Copyright (C) 2025, Hurley, Independent Studio.
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

namespace NovaEngine
{
    /// <summary>
    /// 调试器对象工具类，用于引擎内部调试控制及输出相关接口声明
    /// </summary>
    public partial class Debugger : Singleton<Debugger>
    {
        /// <summary>
        /// 系统异常，仅在调试模式下该函数有效
        /// </summary>
        public static void Throw()
        {
            Instance.m_throw_empty?.Invoke();
        }

        /// <summary>
        /// 系统异常，仅在调试模式下该函数有效
        /// </summary>
        /// <param name="errorCode">错误码</param>
        public static void Throw(int errorCode)
        {
            Instance.m_throw_code?.Invoke(errorCode);
        }

        /// <summary>
        /// 系统异常，仅在调试模式下该函数有效
        /// </summary>
        /// <param name="message">消息内容</param>
        public static void Throw(string message)
        {
            Instance.m_throw_string?.Invoke(message);
        }

        /// <summary>
        /// 系统异常，仅在调试模式下该函数有效
        /// </summary>
        /// <param name="format">格式内容</param>
        /// <param name="args">消息格式化参数</param>
        public static void Throw(string format, params object[] args)
        {
            Instance.m_throw_format_args?.Invoke(format, args);
        }

        /// <summary>
        /// 系统异常，仅在调试模式下该函数有效
        /// </summary>
        /// <param name="exception">异常实例</param>
        public static void Throw(System.Exception exception)
        {
            Instance.m_throw_exception?.Invoke(exception);
        }

        /// <summary>
        /// 系统异常，仅在调试模式下该函数有效
        /// </summary>
        /// <param name="type">异常类型</param>
        public static void Throw(System.Type type)
        {
            Instance.m_throw_type?.Invoke(type);
        }

        /// <summary>
        /// 系统异常，仅在调试模式下该函数有效
        /// </summary>
        /// <param name="type">异常类型</param>
        /// <param name="message">消息内容</param>
        public static void Throw(System.Type type, string message)
        {
            Instance.m_throw_type_string?.Invoke(type, message);
        }

        /// <summary>
        /// 系统异常，仅在调试模式下该函数有效
        /// </summary>
        /// <param name="type">异常类型</param>
        /// <param name="format">格式内容</param>
        /// <param name="args">消息格式化参数</param>
        public static void Throw(System.Type type, string format, params object[] args)
        {
            Instance.m_throw_type_format_args?.Invoke(type, format, args);
        }

        /// <summary>
        /// 系统异常，仅在调试模式下该函数有效
        /// </summary>
        /// <typeparam name="T">异常类型</typeparam>
        public static void Throw<T>() where T : System.Exception
        {
            Throw(typeof(T));
        }

        /// <summary>
        /// 系统异常，仅在调试模式下该函数有效
        /// </summary>
        /// <typeparam name="T">异常类型</typeparam>
        /// <param name="message">消息内容</param>
        public static void Throw<T>(string message) where T : System.Exception
        {
            Throw(typeof(T), message);
        }

        /// <summary>
        /// 系统异常，仅在调试模式下该函数有效
        /// </summary>
        /// <typeparam name="T">异常类型</typeparam>
        /// <param name="format">格式内容</param>
        /// <param name="args">消息格式化参数</param>
        public static void Throw<T>(string format, params object[] args) where T : System.Exception
        {
            Throw(typeof(T), format, args);
        }

        /// <summary>
        /// 对象条件判定的异常检查，仅在调试模式下该函数有效
        /// </summary>
        /// <param name="condition">条件表达式</param>
        public static void Throw(bool condition)
        {
            if (condition) Throw();
        }

        /// <summary>
        /// 对象条件判定的异常检查，仅在调试模式下该函数有效
        /// </summary>
        /// <param name="condition">条件表达式</param>
        /// <param name="errorCode">错误码</param>
        public static void Throw(bool condition, int errorCode)
        {
            if (condition) Throw(errorCode);
        }

        /// <summary>
        /// 对象条件判定的异常检查，仅在调试模式下该函数有效
        /// </summary>
        /// <param name="condition">条件表达式</param>
        /// <param name="message">消息内容</param>
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
        public static void Throw(bool condition, string format, params object[] args)
        {
            if (condition) Throw(format, args);
        }

        /// <summary>
        /// 系统异常，仅在调试模式下该函数有效
        /// </summary>
        /// <param name="condition">条件表达式</param>
        /// <param name="exception">异常实例</param>
        public static void Throw(bool condition, System.Exception exception)
        {
            if (condition) Throw(exception);
        }

        /// <summary>
        /// 系统异常，仅在调试模式下该函数有效
        /// </summary>
        /// <param name="condition">条件表达式</param>
        /// <param name="type">异常类型</param>
        public static void Throw(bool condition, System.Type type)
        {
            if (condition) Throw(type);
        }

        /// <summary>
        /// 系统异常，仅在调试模式下该函数有效
        /// </summary>
        /// <param name="condition">条件表达式</param>
        /// <param name="type">异常类型</param>
        /// <param name="message">消息内容</param>
        public static void Throw(bool condition, System.Type type, string message)
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
        public static void Throw(bool condition, System.Type type, string format, params object[] args)
        {
            if (condition) Throw(type, format, args);
        }

        /// <summary>
        /// 系统异常，仅在调试模式下该函数有效
        /// </summary>
        /// <typeparam name="T">异常类型</typeparam>
        /// <param name="condition">条件表达式</param>
        public static void Throw<T>(bool condition) where T : System.Exception
        {
            if (condition) Throw<T>();
        }

        /// <summary>
        /// 系统异常，仅在调试模式下该函数有效
        /// </summary>
        /// <typeparam name="T">异常类型</typeparam>
        /// <param name="condition">条件表达式</param>
        /// <param name="message">消息内容</param>
        public static void Throw<T>(bool condition, string message) where T : System.Exception
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
        public static void Throw<T>(bool condition, string format, params object[] args) where T : System.Exception
        {
            if (condition) Throw<T>(format, args);
        }
    }
}
