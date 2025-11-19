/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
/// Copyright (C) 2020 - 2022, Guangzhou Xinyuan Technology Co., Ltd.
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
    /// <summary>
    /// 日志相关函数集合工具类
    /// </summary>
    internal static partial class Logger
    {
        /// <summary>
        /// 系统断言，提供一个标准接口给引擎内部使用<br/>
        /// 此处对断言进行一次检测，在非断言情况下跳过断言流程<br/>
        /// 之所以这样做，是为了在断言消息内容需要格式化的情况下，提升运行效率
        /// </summary>
        /// <param name="condition">条件表达式</param>
        internal static void Assert(bool condition)
        {
            if (!condition) __Assert_ImplementedOnSystem(condition);
        }

        /// <summary>
        /// 系统断言，提供一个标准接口给引擎内部使用<br/>
        /// 此处对断言进行一次检测，在非断言情况下跳过断言流程<br/>
        /// 之所以这样做，是为了在断言消息内容需要格式化的情况下，提升运行效率
        /// </summary>
        /// <param name="condition">条件表达式</param>
        /// <param name="message">消息内容</param>
        internal static void Assert(bool condition, object message)
        {
            if (!condition) __Assert_ImplementedOnSystem(condition, message);
        }

        /// <summary>
        /// 系统断言，提供一个标准接口给引擎内部使用<br/>
        /// 此处对断言进行一次检测，在非断言情况下跳过断言流程<br/>
        /// 之所以这样做，是为了在断言消息内容需要格式化的情况下，提升运行效率
        /// </summary>
        /// <param name="condition">条件表达式</param>
        /// <param name="message">消息内容</param>
        internal static void Assert(bool condition, string message)
        {
            if (!condition) __Assert_ImplementedOnSystem(condition, message);
        }

        /// <summary>
        /// 系统断言，提供一个标准接口给引擎内部使用<br/>
        /// 此处对断言进行一次检测，在非断言情况下跳过断言流程<br/>
        /// 之所以这样做，是为了在断言消息内容需要格式化的情况下，提升运行效率
        /// </summary>
        /// <param name="condition">条件表达式</param>
        /// <param name="format">格式内容</param>
        /// <param name="args">消息格式化参数</param>
        internal static void Assert(bool condition, string format, params object[] args)
        {
            if (!condition) __Assert_ImplementedOnSystem(condition, format, args);
        }

        #region

        /// <summary>
        /// 系统断言，通过引擎接口实现的断言函数
        /// </summary>
        /// <param name="condition">条件表达式</param>
        internal static void __Assert_ImplementedOnSystem(bool condition)
        {
            UnityEngine.Debug.Assert(condition);
        }

        /// <summary>
        /// 系统断言，通过引擎接口实现的断言函数
        /// </summary>
        /// <param name="condition">条件表达式</param>
        /// <param name="message">消息内容</param>
        internal static void __Assert_ImplementedOnSystem(bool condition, object message)
        {
            UnityEngine.Debug.Assert(condition, message);
        }

        /// <summary>
        /// 系统断言，通过引擎接口实现的断言函数
        /// </summary>
        /// <param name="condition">条件表达式</param>
        /// <param name="message">消息内容</param>
        internal static void __Assert_ImplementedOnSystem(bool condition, string message)
        {
            UnityEngine.Debug.Assert(condition, message);
        }

        /// <summary>
        /// 系统断言，通过引擎接口实现的断言函数
        /// </summary>
        /// <param name="condition">条件表达式</param>
        /// <param name="format">格式内容</param>
        /// <param name="args">消息格式化参数</param>
        internal static void __Assert_ImplementedOnSystem(bool condition, string format, params object[] args)
        {
            UnityEngine.Debug.Assert(condition, Utility.Text.Format(format, args));
        }

        /// <summary>
        /// 系统断言，通过错误打印输出的方式实现的断言函数
        /// </summary>
        /// <param name="condition">条件表达式</param>
        internal static void __Assert_ImplementedOnOutput(bool condition)
        {
            if (false == condition)
            {
                Fatal("Application running assert!");
            }
        }

        /// <summary>
        /// 系统断言，通过错误打印输出的方式实现的断言函数
        /// </summary>
        /// <param name="condition">条件表达式</param>
        /// <param name="message">消息内容</param>
        internal static void __Assert_ImplementedOnOutput(bool condition, object message)
        {
            if (false == condition)
            {
                Fatal(message);
            }
        }

        /// <summary>
        /// 系统断言，通过错误打印输出的方式实现的断言函数
        /// </summary>
        /// <param name="condition">条件表达式</param>
        /// <param name="message">消息内容</param>
        internal static void __Assert_ImplementedOnOutput(bool condition, string message)
        {
            if (false == condition)
            {
                Fatal(message);
            }
        }

        /// <summary>
        /// 系统断言，通过错误打印输出的方式实现的断言函数
        /// </summary>
        /// <param name="condition">条件表达式</param>
        /// <param name="format">格式内容</param>
        /// <param name="args">消息格式化参数</param>
        internal static void __Assert_ImplementedOnOutput(bool condition, string format, params object[] args)
        {
            if (false == condition)
            {
                Fatal(format, args);
            }
        }

        #endregion
    }
}
