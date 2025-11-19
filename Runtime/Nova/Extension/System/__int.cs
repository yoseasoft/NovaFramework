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

namespace System.Customize.Extension
{
    /// <summary>
    /// 为系统默认的整型数据类型提供扩展接口支持
    /// </summary>
    public static class __int
    {
        /// <summary>
        /// 将整数类型数值转换为数字类型数值
        /// </summary>
        /// <param name="self">整数数值</param>
        /// <returns>返回转换后的数字类型数值</returns>
        public static double ToDouble(this int self)
        {
            return self * 1.0;
        }

        /// <summary>
        /// 将整数类型数值转换为数字类型数值
        /// </summary>
        /// <param name="self">整数数值</param>
        /// <returns>返回转换后的数字类型数值</returns>
        public static decimal ToDecimal(this int self)
        {
            return new decimal(self);
        }

        /// <summary>
        /// 将整型数值转换为字节数组
        /// </summary>
        /// <param name="self">整型数值</param>
        /// <returns>返回转换后的字节数组</returns>
        public static byte[] GetBytes(this int self)
        {
            return BitConverter.GetBytes(self);
        }
    }
}
