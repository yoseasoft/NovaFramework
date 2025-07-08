/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
/// Copyright (C) 2020 - 2022, Guangzhou Xinyuan Technology Co., Ltd.
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

using SystemRegex = System.Text.RegularExpressions.Regex;

namespace NovaEngine
{
    /// <summary>
    /// 为系统默认的基础字符串数据类型提供扩展接口支持
    /// </summary>
    public static class __string
    {
        /// <summary>
        /// 忽略大小写模式的相等判断检测
        /// </summary>
        /// <param name="self">原始字符串</param>
        /// <param name="other">对比字符串</param>
        /// <returns>若两个字符串相等则返回true，否则返回false</returns>
        public static bool EqualsIgnoreCase(this string self, string other)
        {
            return self.ToLower().Equals(other?.ToLower());
        }

        /// <summary>
        /// 从指定位置开始读取一行数据
        /// </summary>
        /// <param name="self">原始字符串</param>
        /// <param name="position">起始位置</param>
        /// <returns>返回从原始字符串中截取的一段字符串数据，若查找失败返回null</returns>
        public static string ReadLine(this string self, int position)
        {
            if (position < 0)
            {
                return null;
            }

            int length = self.Length;
            int offset = position;
            while (offset < length)
            {
                char ch = self[offset];
                switch (ch)
                {
                    case '\r':
                    case '\n':
                        if (offset > position)
                        {
                            string line = self.Substring(position, offset - position);
                            position = offset + 1;
                            if ((ch == '\r') && (position < length) && (self[position] == '\n'))
                            {
                                position++;
                            }

                            return line;
                        }

                        offset++;
                        position++;
                        break;

                    default:
                        offset++;
                        break;
                }
            }

            if (offset > position)
            {
                string line = self.Substring(position, offset - position);
                position = offset;
                return line;
            }

            return null;
        }

        /// <summary>
        /// 将当前字符串转换为大驼峰的文本格式
        /// </summary>
        /// <param name="self">原始字符串</param>
        /// <returns>返回转换后的字符串实例</returns>
        public static string ToLargeHumpFormat(this string self)
        {
            string result = SystemRegex.Replace(self, @"([^\p{L}\p{N}])(\p{L})", m => $"{m.Groups[1]}{char.ToUpper(m.Groups[2].Value[0])}");
            result = SystemRegex.Replace(result, @"[^A-Za-z0-9]", "");
            return result;
        }

        /// <summary>
        /// 将当前字符串转换为小驼峰的文本格式
        /// </summary>
        /// <param name="self">原始字符串</param>
        /// <returns>返回转换后的字符串实例</returns>
        public static string ToLittleHumpFormat(this string self)
        {
            string result = self.ToLargeHumpFormat();
            if (char.IsLower(result[0]))
            {
                result = char.ToUpper(result[0]) + result.Substring(1);
            }

            return result;
        }
    }
}
