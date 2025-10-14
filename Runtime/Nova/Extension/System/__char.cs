/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
/// Copyright (C) 2024 - 2025, Hurley, Independent Studio.
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

namespace NovaEngine
{
    /// <summary>
    /// 为系统默认的字符数据类型提供扩展接口支持
    /// </summary>
    public static class __char
    {
        /// <summary>
        /// 判断指定的字符是否为变量命名的有效字符<br/>
        /// 变量命名的有效字符包括：英文字母，数字和下划线
        /// </summary>
        /// <param name="self">字符值</param>
        /// <returns>若给定的字符值为变量命名的有效字符则返回true，否则返回false</returns>
        public static bool IsValidCharForVariableNaming(this char self)
        {
            // 数字字符
            if (char.IsDigit(self))
            {
                return true;
            }

            // 英文字符（包括大写和小写）
            if (char.IsLetter(self))
            {
                return true;
            }

            // 下划线字符
            if (Definition.CCharacter.Underline == self)
            {
                return true;
            }

            return false;
        }
    }
}
