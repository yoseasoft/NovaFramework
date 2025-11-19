/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
/// Copyright (C) 2020 - 2022, Guangzhou Xinyuan Technology Co., Ltd.
/// Copyright (C) 2023 - 2024, Guangzhou Shiyue Network Technology Co., Ltd.
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

namespace System.Customize.Extension
{
    /// <summary>
    /// 为系统默认的基础字符串数据类型提供扩展接口支持
    /// </summary>
    public static class __string
    {
        /// <summary>
        /// 将字符串类型数据转换为整型数值
        /// </summary>
        /// <param name="self">原始字符串</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>返回转换后的整型数值，若转换失败则返回默认值</returns>
        [Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static int ToInt32(this string self, int defaultValue = 0)
        {
            return self.TryConvertTo(defaultValue);
        }

        /// <summary>
        /// 将字符串类型数据转换为长整型数值
        /// </summary>
        /// <param name="self">原始字符串</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>返回转换后的长整型数值，若转换失败则返回默认值</returns>
        [Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static long ToInt64(this string self, long defaultValue = 0L)
        {
            return self.TryConvertTo(defaultValue);
        }

        /// <summary>
        /// 将字符串类型数据转换为浮点型数值
        /// </summary>
        /// <param name="self">原始字符串</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>返回转换后的浮点型数值，若转换失败则返回默认值</returns>
        [Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static double ToDouble(this string self, double defaultValue = 0)
        {
            return self.TryConvertTo(defaultValue);
        }

        /// <summary>
        /// 将字符串类型数据转换为日期时间类型对象
        /// </summary>
        /// <param name="self">原始字符串</param>
        /// <returns>返回转换后的日期时间类型对象实例</returns>
        [Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static DateTime ToDateTime(this string self)
        {
            DateTime.TryParse(self, out var result);
            return result;
        }

        /// <summary>
        /// 将字符串类型数据转换为GUID类型对象
        /// </summary>
        /// <param name="self">原始字符串</param>
        /// <returns>返回转换后的GUID类型对象实例</returns>
        [Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static Guid ToGuid(this string self)
        {
            return Guid.Parse(self);
        }

        /// <summary>
        /// 将字符串类型数据转换为字节数组
        /// </summary>
        /// <param name="self">原始字符串</param>
        /// <returns>返回转换后的字节数组</returns>
        [Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static byte[] ToByteArray(this string self)
        {
            return Text.Encoding.UTF8.GetBytes(self);
        }

        /// <summary>
        /// 判断字符串是否为null或空字符串
        /// </summary>
        /// <param name="self">原始字符串</param>
        /// <returns>若给定字符串为null或空字符串则返回true，否则返回false</returns>
        [Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static bool IsNullOrEmpty(this string self)
        {
            return string.IsNullOrEmpty(self);
        }

        /// <summary>
        /// 判断字符串是否不为null且非空字符串
        /// </summary>
        /// <param name="self">原始字符串</param>
        /// <returns>若给定字符串不为null且非空字符串则返回true，否则返回false</returns>
        [Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static bool IsNotNullOrEmpty(this string self)
        {
            return !self.IsNullOrEmpty();
        }

        /// <summary>
        /// 判断字符串是否为null或空白字符
        /// </summary>
        /// <param name="self">原始字符串</param>
        /// <returns>若给定字符串为null或空白字符则返回true，否则返回false</returns>
        [Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static bool IsNullOrWhiteSpace(this string self)
        {
            return string.IsNullOrWhiteSpace(self) || self.EqualsIgnoreCase(NovaEngine.Definition.CString.Null);
        }

        /// <summary>
        /// 判断字符串是否不为null且非空白字符
        /// </summary>
        /// <param name="self">原始字符串</param>
        /// <returns>若给定字符串不为null且非空白字符则返回true，否则返回false</returns>
        [Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static bool IsNotNullOrWhiteSpace(this string self)
        {
            return !self.IsNullOrWhiteSpace();
        }

        /// <summary>
        /// 忽略大小写模式的相等判断检测
        /// </summary>
        /// <param name="self">原始字符串</param>
        /// <param name="other">对比字符串</param>
        /// <returns>若两个字符串相等则返回true，否则返回false</returns>
        [Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static bool EqualsIgnoreCase(this string self, string other)
        {
            return self.ToLower().Equals(other?.ToLower());
        }

        /// <summary>
        /// 检查当前字符串中是否包含中文信息
        /// </summary>
        /// <param name="self">原始字符串</param>
        /// <returns>若包含中文则返回true，否则返回false</returns>
        public static bool IsContainsChinese(this string self)
        {
            foreach (char a in self)
            {
                if (a >= 0x4e00 && a <= 0x9fff)
                {
                    return true;
                }
            }
            return false;
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
        /// 将驼峰命名的字符串转换为下划线分隔的小写形式（蛇形命名）
        /// </summary>
        /// <param name="self">原始字符串</param>
        /// <returns>返回转换后的字符串实例</returns>
        public static string ToSnakeCaseName(this string self)
        {
            if (string.IsNullOrEmpty(self))
                return self;

            Text.RegularExpressions.Match startUnderscores = Text.RegularExpressions.Regex.Match(self, @"^_+");
            return startUnderscores + Text.RegularExpressions.Regex.Replace(self, @"([a-z0-9])([A-Z])", "$1_$2").ToLower();
        }

        /// <summary>
        /// 将当前字符串转换为大驼峰的文本格式名称
        /// </summary>
        /// <param name="self">原始字符串</param>
        /// <returns>返回转换后的字符串实例</returns>
        public static string ToLargeHumpName(this string self)
        {
            if (string.IsNullOrEmpty(self))
                return self;

            string result = Text.RegularExpressions.Regex.Replace(self, @"([^\p{L}\p{N}])(\p{L})", m => $"{m.Groups[1]}{char.ToUpper(m.Groups[2].Value[0])}");
            result = Text.RegularExpressions.Regex.Replace(result, @"[^A-Za-z0-9]", "");
            return result;
        }

        /// <summary>
        /// 将当前字符串转换为小驼峰的文本格式名称
        /// </summary>
        /// <param name="self">原始字符串</param>
        /// <returns>返回转换后的字符串实例</returns>
        public static string ToLittleHumpName(this string self)
        {
            string result = self.ToLargeHumpName();
            if (false == string.IsNullOrEmpty(self) && char.IsLower(result[0]))
            {
                result = char.ToUpper(result[0]) + result.Substring(1);
            }

            return result;
        }
    }
}
