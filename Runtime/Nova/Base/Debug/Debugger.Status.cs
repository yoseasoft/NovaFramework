/// -------------------------------------------------------------------------------
/// NovaEngine Framework
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

namespace NovaEngine
{
    /// <summary>
    /// 调试器对象工具类，用于引擎内部调试控制及输出相关接口声明
    /// </summary>
    public partial class Debugger
    {
        /// <summary>
        /// 对象类型校验模式启用状态标识
        /// </summary>
        private bool m_isClassTypeVerificationEnabled = false;

        /// <summary>
        /// 函数信息校验模式启用状态标识
        /// </summary>
        private bool m_isMethodInfoVerificationEnabled = false;

        /// <summary>
        /// 参数信息校验模式启用状态标识
        /// </summary>
        private bool m_isParameterInfoVerificationEnabled = false;

        /// <summary>
        /// 调试校验中的断言模式启用状态标识
        /// </summary>
        private bool m_isDebuggingVerificationAssertModeEnabled = false;

        /// <summary>
        /// 对象类型校验模式启用状态标识的getter/setter属性
        /// </summary>
        public bool IsClassTypeVerificationEnabled
        { get { return m_isClassTypeVerificationEnabled; } set { m_isClassTypeVerificationEnabled = value; } }

        /// <summary>
        /// 函数信息校验模式启用状态标识的getter/setter属性
        /// </summary>
        public bool IsMethodInfoVerificationEnabled
        { get { return m_isMethodInfoVerificationEnabled; } set { m_isMethodInfoVerificationEnabled = value; } }

        /// <summary>
        /// 参数信息校验模式启用状态标识的getter/setter属性
        /// </summary>
        public bool IsParameterInfoVerificationEnabled
        { get { return m_isParameterInfoVerificationEnabled; } set { m_isParameterInfoVerificationEnabled = value; } }

        /// <summary>
        /// 调试校验中的断言模式启用状态标识的getter/setter属性
        /// </summary>
        public bool IsDebuggingVerificationAssertModeEnabled
        { get { return m_isDebuggingVerificationAssertModeEnabled; } set { m_isDebuggingVerificationAssertModeEnabled = value; } }

        /// <summary>
        /// 检测当前调试校验模式是否已处于激活状态
        /// </summary>
        /// <returns>若调试校验模式已激活则返回true，否则返回false</returns>
        public bool IsOnDebuggingVerificationActivated()
        {
            if (Environment.debugMode)
            {
                return true;
            }

            return false;
        }
    }
}
