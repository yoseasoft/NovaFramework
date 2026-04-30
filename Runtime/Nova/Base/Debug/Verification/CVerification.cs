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
    /// 验证工具类，对类型，函数等定义进行格式校验
    /// </summary>
    public static partial class CVerification
    {
        /// <summary>
        /// 对象类型校验模式启用状态标识
        /// </summary>
        private static bool _isClassTypeVerificationEnabled = false;

        /// <summary>
        /// 函数信息校验模式启用状态标识
        /// </summary>
        private static bool _isMethodInfoVerificationEnabled = false;

        /// <summary>
        /// 参数信息校验模式启用状态标识
        /// </summary>
        private static bool _isParameterInfoVerificationEnabled = false;

        /// <summary>
        /// 调试校验中的断言模式启用状态标识
        /// </summary>
        private static bool _isDebuggingVerificationAssertModeEnabled = false;

        /// <summary>
        /// 对象类型校验模式启用状态标识的getter/setter属性
        /// </summary>
        public static bool IsClassTypeVerificationEnabled
        { get { return _isClassTypeVerificationEnabled; } internal set { _isClassTypeVerificationEnabled = value; } }

        /// <summary>
        /// 函数信息校验模式启用状态标识的getter/setter属性
        /// </summary>
        public static bool IsMethodInfoVerificationEnabled
        { get { return _isMethodInfoVerificationEnabled; } internal set { _isMethodInfoVerificationEnabled = value; } }

        /// <summary>
        /// 参数信息校验模式启用状态标识的getter/setter属性
        /// </summary>
        public static bool IsParameterInfoVerificationEnabled
        { get { return _isParameterInfoVerificationEnabled; } internal set { _isParameterInfoVerificationEnabled = value; } }

        /// <summary>
        /// 调试校验中的断言模式启用状态标识的getter/setter属性
        /// </summary>
        public static bool IsDebuggingVerificationAssertModeEnabled
        { get { return _isDebuggingVerificationAssertModeEnabled; } internal set { _isDebuggingVerificationAssertModeEnabled = value; } }

        /// <summary>
        /// 检测当前调试校验模式是否已处于激活状态
        /// </summary>
        /// <returns>若调试校验模式已激活则返回true，否则返回false</returns>
        public static bool IsOnDebuggingVerificationActivated()
        {
            if (Environment.DebugMode)
            {
                return true;
            }

            return false;
        }
    }
}
