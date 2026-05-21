/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyright (C) 2026, Hurley, Independent Studio.
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
using System.Collections;
using System.Collections.Generic;

namespace GameEngine.Profiler.Debugging
{
    /// 控制台组件对象类
    internal sealed partial class ConsoleComponent
    {
        /// <summary>
        /// 控制台错误码集合
        /// </summary>
        private static class ConsoleErrorCode
        {
            public const int Succeed = 0;
            // 退出
            public const int Quit = 1;
            // 无效指令
            public const int InvalidCommand = 101;
        }

        /// <summary>
        /// 控制台错误码对应文本的映射容器
        /// </summary>
        private readonly static IDictionary<int, string> _errorCodeText = new Dictionary<int, string>()
        {
            { ConsoleErrorCode.Quit, @"Goodbye." },
            { ConsoleErrorCode.InvalidCommand, @"Invalid command." },
        };

        /// <summary>
        /// 通过指定的错误码获取对应的错误提升信息
        /// </summary>
        /// <param name="errorCode">错误码</param>
        /// <returns>返回错误提示字符串</returns>
        private static string GetErrorMessage(int errorCode)
        {
            if (_errorCodeText.TryGetValue(errorCode, out string errorText))
            {
                return errorText;
            }

            return $"unknown error {errorCode}.";
        }
    }
}
