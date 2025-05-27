/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
/// Copyring (C) 2025, Hurley, Independent Studio.
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
        /// 系统异常，提供一个标准接口给引擎内部使用<br/>
        /// 此处对异常进行一次封装，之所以这样做，是为了在发布模式下能支持提供更多的错误信息
        /// </summary>
        internal static void Throw()
        {
            __Throw_ImplementedOnSystem();
        }

        /// <summary>
        /// 系统异常，提供一个标准接口给引擎内部使用<br/>
        /// 此处对异常进行一次封装，之所以这样做，是为了在发布模式下能支持提供更多的错误信息
        /// </summary>
        /// <param name="errorCode">错误码</param>
        internal static void Throw(int errorCode)
        {
            __Throw_ImplementedOnSystem(errorCode);
        }

        /// <summary>
        /// 系统异常，提供一个标准接口给引擎内部使用<br/>
        /// 此处对异常进行一次封装，之所以这样做，是为了在发布模式下能支持提供更多的错误信息
        /// </summary>
        /// <param name="message">消息内容</param>
        internal static void Throw(string message)
        {
            __Throw_ImplementedOnSystem(message);
        }

        /// <summary>
        /// 系统异常，提供一个标准接口给引擎内部使用<br/>
        /// 此处对异常进行一次封装，之所以这样做，是为了在发布模式下能支持提供更多的错误信息
        /// </summary>
        /// <param name="format">格式内容</param>
        /// <param name="args">消息格式化参数</param>
        internal static void Throw(string format, params object[] args)
        {
            __Throw_ImplementedOnSystem(format, args);
        }

        /// <summary>
        /// 系统异常，提供一个标准接口给引擎内部使用<br/>
        /// 此处对异常进行一次封装，之所以这样做，是为了在发布模式下能支持提供更多的错误信息
        /// </summary>
        /// <param name="exception">异常实例</param>
        internal static void Throw(System.Exception exception)
        {
            __Throw_ImplementedOnSystem(exception);
        }

        /// <summary>
        /// 系统异常，提供一个标准接口给引擎内部使用<br/>
        /// 此处对异常进行一次封装，之所以这样做，是为了在发布模式下能支持提供更多的错误信息
        /// </summary>
        /// <typeparam name="T">异常类型</typeparam>
        internal static void Throw<T>() where T : System.Exception
        {
        }

        #region

        /// <summary>
        /// 系统异常，通过引擎接口实现的异常函数
        /// </summary>
        internal static void __Throw_ImplementedOnSystem()
        {
            throw new CFrameworkException();
        }

        /// <summary>
        /// 系统异常，通过引擎接口实现的异常函数
        /// </summary>
        /// <param name="errorCode">错误码</param>
        internal static void __Throw_ImplementedOnSystem(int errorCode)
        {
            throw new CFrameworkException(errorCode);
        }

        /// <summary>
        /// 系统异常，通过引擎接口实现的异常函数
        /// </summary>
        /// <param name="message">消息内容</param>
        internal static void __Throw_ImplementedOnSystem(string message)
        {
            throw new CFrameworkException(message);
        }

        /// <summary>
        /// 系统异常，通过引擎接口实现的异常函数
        /// </summary>
        /// <param name="format">格式内容</param>
        /// <param name="args">消息格式化参数</param>
        internal static void __Throw_ImplementedOnSystem(string format, params object[] args)
        {
            throw new CFrameworkException(format, args);
        }

        /// <summary>
        /// 系统异常，通过引擎接口实现的异常函数
        /// </summary>
        /// <param name="exception">异常实例</param>
        internal static void __Throw_ImplementedOnSystem(System.Exception exception)
        {
            throw new CFrameworkException(exception);
        }

        /// <summary>
        /// 系统异常，通过引擎接口实现的异常函数
        /// </summary>
        /// <param name="errorCode">错误码</param>
        internal static void __Throw_ImplementedOnOutput(int errorCode)
        {
            Fatal(errorCode);
        }

        /// <summary>
        /// 系统异常，通过引擎接口实现的异常函数
        /// </summary>
        /// <param name="message">消息内容</param>
        internal static void __Throw_ImplementedOnOutput(string message)
        {
            Fatal(message);
        }

        /// <summary>
        /// 系统异常，通过引擎接口实现的异常函数
        /// </summary>
        /// <param name="format">格式内容</param>
        /// <param name="args">消息格式化参数</param>
        internal static void __Throw_ImplementedOnOutput(string format, params object[] args)
        {
            Fatal(format, args);
        }

        /// <summary>
        /// 系统异常，通过引擎接口实现的异常函数
        /// </summary>
        /// <param name="exception">异常实例</param>
        internal static void __Throw_ImplementedOnOutput(System.Exception exception)
        {
            Fatal(exception.Message);
        }

        #endregion
    }
}
