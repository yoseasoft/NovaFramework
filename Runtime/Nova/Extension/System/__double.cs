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

using SystemMath = System.Math;
using SystemMidpointRounding = System.MidpointRounding;

namespace NovaEngine
{
    /// <summary>
    /// 为系统默认的双精度浮点数据类型提供扩展接口支持
    /// </summary>
    public static class __double
    {
        /// <summary>
        /// 判断指定的浮点数是否为零
        /// </summary>
        /// <param name="self">浮点数值</param>
        /// <returns>若给定的浮点数值为零则返回true，否则返回false</returns>
        public static bool IsZero(this double self)
        {
            return (SystemMath.Abs(self) < double.Epsilon);
        }

        /// <summary>
        /// 将浮点型数值的小数位截断为8位
        /// </summary>
        /// <param name="self">浮点数值</param>
        /// <returns>返回截断后的浮点数值</returns>
        public static double Digits8(this double self)
        {
            return (long) (self * 1E+8) * 1E-8;
        }

        /// <summary>
        /// 将浮点类型数值转换为整数类型数值
        /// </summary>
        /// <param name="self">浮点数值</param>
        /// <returns>返回转换后的整数类型数值</returns>
        public static int ToInt32(this double self)
        {
            return (int) SystemMath.Floor(self);
        }

        /// <summary>
        /// 将浮点类型数值转换为数字类型数值
        /// </summary>
        /// <param name="self">浮点数值</param>
        /// <returns>返回转换后的数字类型数值</returns>
        public static decimal ToDecimal(this double self)
        {
            return self.ConvertTo<decimal>();
        }

        /// <summary>
        /// 将浮点类型数值转换为数字类型数值，并保留指定位数
        /// </summary>
        /// <param name="self">浮点数值</param>
        /// <param name="precision">小数位数</param>
        /// <returns>返回转换后的数字类型数值</returns>
        public static decimal ToDecimal(this double self, int precision)
        {
            return SystemMath.Round(self.ConvertTo<decimal>(), precision);
        }

        /// <summary>
        /// 对浮点数进行舍入的函数
        /// 舍入方式的选项包括：
        /// MidpointRounding.ToEven
        /// （默认）银行家舍入规则。如果处于中间值（如 0.5），将向最近的偶数舍入。例如：
        /// 1.5 → 2
        /// 2.5 → 2
        /// MidpointRounding.AwayFromZero
        /// 传统的“四舍五入”。中间值（如 0.5）总是向远离 0 的方向舍入。例如：
        /// 1.5 → 2
        /// 2.5 → 3
        /// MidpointRounding.ToZero
        /// 舍入到零（截断小数部分），忽略小数值。例如：
        /// 1.5 → 1
        /// -1.5 → -1
        /// MidpointRounding.Up
        /// 始终向远离零的方向舍入。例如：
        /// 1.2 → 2
        /// -1.2 → -2
        /// MidpointRounding.Down
        /// 始终向靠近零的方向舍入。例如：
        /// 1.7 → 1
        /// -1.7 → -1
        /// MidpointRounding.AwayFromZero
        /// 总是向绝对值更大的方向舍入，即传统意义上的“进位”。
        /// </summary>
        /// <param name="self">浮点数值</param>
        /// <param name="decimals">保留小数位数，默认为0，即保留到整数</param>
        /// <param name="mode">舍入方式</param>
        /// <returns>返回舍入后的数值</returns>
        public static double Round(this double self, int decimals = 0, SystemMidpointRounding mode = SystemMidpointRounding.AwayFromZero)
        {
            self = SystemMath.Round(self, decimals, mode);
            return self;
        }

        /// <summary>
        /// 对浮点数进行舍入的函数，<br/>
        /// 使用规则请参数<see cref="Round(double, int, SystemMidpointRounding)"/>
        /// </summary>
        /// <param name="self">浮点数值</param>
        /// <param name="decimals">保留小数位数，默认为0，即保留到整数</param>
        /// <param name="mode">舍入方式</param>
        /// <returns>返回舍入后的数值</returns>
        public static double? Round(this double? self, int decimals = 0, SystemMidpointRounding mode = SystemMidpointRounding.AwayFromZero)
        {
            if (self.HasValue)
            {
                self = SystemMath.Round(self.Value, decimals, mode);
            }
            return self;
        }
    }
}
