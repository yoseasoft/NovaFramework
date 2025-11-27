/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
/// Copyright (C) 2023 - 2024, Guangzhou Shiyue Network Technology Co., Ltd.
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
    /// 调试器对象工具类
    internal partial class Debugger
    {
        /// <summary>
        /// 系统断言，仅在调试模式下该函数有效
        /// </summary>
        /// <param name="condition">条件表达式</param>
        public static void Assert(bool condition)
        {
            Instance._assert_empty?.Invoke(condition);
        }

        /// <summary>
        /// 系统断言，仅在调试模式下该函数有效
        /// </summary>
        /// <param name="condition">条件表达式</param>
        /// <param name="message">消息内容</param>
        public static void Assert(bool condition, object message)
        {
            Instance._assert_object?.Invoke(condition, message);
        }

        /// <summary>
        /// 系统断言，仅在调试模式下该函数有效
        /// </summary>
        /// <param name="condition">条件表达式</param>
        /// <param name="message">消息内容</param>
        public static void Assert(bool condition, string message)
        {
            Instance._assert_string?.Invoke(condition, message);
        }

        /// <summary>
        /// 系统断言，仅在调试模式下该函数有效
        /// </summary>
        /// <param name="condition">条件表达式</param>
        /// <param name="format">格式内容</param>
        /// <param name="args">消息格式化参数</param>
        public static void Assert(bool condition, string format, params object[] args)
        {
            Instance._assert_format_args?.Invoke(condition, format, args);
        }

        /// <summary>
        /// 对象非空的断言检查，仅在调试模式下该函数有效
        /// </summary>
        /// <param name="obj">对象实例</param>
        public static void Assert(object obj)
        {
            Assert(null != obj);
        }

        /// <summary>
        /// 对象非空的断言检查，仅在调试模式下该函数有效
        /// </summary>
        /// <param name="obj">对象实例</param>
        /// <param name="message">消息内容</param>
        public static void Assert(object obj, object message)
        {
            Assert(null != obj, message);
        }

        /// <summary>
        /// 对象非空的断言检查，仅在调试模式下该函数有效
        /// </summary>
        /// <param name="obj">对象实例</param>
        /// <param name="message">消息内容</param>
        public static void Assert(object obj, string message)
        {
            Assert(null != obj, message);
        }

        /// <summary>
        /// 对象非空的断言检查，仅在调试模式下该函数有效
        /// </summary>
        /// <param name="obj">对象实例</param>
        /// <param name="format">格式内容</param>
        /// <param name="args">消息格式化参数</param>
        public static void Assert(object obj, string format, params object[] args)
        {
            Assert(null != obj, format, args);
        }
    }
}
