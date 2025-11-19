/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
/// Copyright (C) 2023, Guangzhou Shiyue Network Technology Co., Ltd.
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
    /// 为系统默认的单精度浮点数据类型提供扩展接口支持
    /// </summary>
    public static class __float
    {
        /// <summary>
        /// 判断指定的浮点数是否为零
        /// </summary>
        /// <param name="self">浮点数值</param>
        /// <returns>若给定的浮点数值为零则返回true，否则返回false</returns>
        public static bool IsZero(this float self)
        {
            return (Math.Abs(self) < float.Epsilon);
        }

        /// <summary>
        /// 将浮点类型数值转换为整数类型数值
        /// </summary>
        /// <param name="self">浮点数值</param>
        /// <returns>返回转换后的整数类型数值</returns>
        public static int ToInt32(this float self)
        {
            return (int) Math.Floor(self);
        }

        /// <summary>
        /// 将浮点类型数值转换为数字类型数值
        /// </summary>
        /// <param name="self">浮点数值</param>
        /// <returns>返回转换后的数字类型数值</returns>
        public static decimal ToDecimal(this float self)
        {
            return self.ConvertTo<decimal>();
        }

        /// <summary>
        /// 将浮点类型数值转换为数字类型数值，并保留指定位数
        /// </summary>
        /// <param name="self">浮点数值</param>
        /// <param name="precision">小数位数</param>
        /// <returns>返回转换后的数字类型数值</returns>
        public static decimal ToDecimal(this float self, int precision)
        {
            return Math.Round(self.ConvertTo<decimal>(), precision);
        }
    }
}
