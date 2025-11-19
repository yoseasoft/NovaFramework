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

namespace System.Customize.Extension
{
    /// <summary>
    /// 为系统默认的基础布尔数据类型提供扩展接口支持
    /// </summary>
    public static class __bool
    {
        /// <summary>
        /// 比较指定的布尔类型值和整型值是否相同
        /// </summary>
        /// <param name="self">布尔类型值</param>
        /// <param name="value">整型值</param>
        /// <returns>若两值相同则返回true，否则返回false</returns>
        public static bool Equals(this bool self, int value)
        {
            if ((self && value != 0) || (!self && value == 0))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 比较指定的布尔类型值和浮点型值是否相同
        /// </summary>
        /// <param name="self">布尔类型值</param>
        /// <param name="value">浮点型值</param>
        /// <returns>若两值相同则返回true，否则返回false</returns>
        public static bool Equals(this bool self, float value)
        {
            if ((self && false == value.IsZero()) || (!self && value.IsZero()))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 比较指定的布尔类型值和字符值是否相同
        /// </summary>
        /// <param name="self">布尔类型值</param>
        /// <param name="value">字符值</param>
        /// <returns>若两值相同则返回true，否则返回false</returns>
        public static bool Equals(this bool self, char value)
        {
            if ((self && (value > 0 && value != NovaEngine.Definition.CCharacter.N)) ||
                (!self && (value == 0 || value == NovaEngine.Definition.CCharacter.N)))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 比较指定的布尔类型值和字符串值是否相同
        /// </summary>
        /// <param name="self">布尔类型值</param>
        /// <param name="value">字符串值</param>
        /// <returns>若两值相同则返回true，否则返回false</returns>
        public static bool Equals(this bool self, string value)
        {
            if (null == value)
            {
                return !self;
            }

            if ((self && value.Trim().ToLower().Equals(NovaEngine.Definition.CString.True)) ||
                (!self && value.Trim().ToLower().Equals(NovaEngine.Definition.CString.False)))
            {
                return true;
            }

            return false;
        }
    }
}
