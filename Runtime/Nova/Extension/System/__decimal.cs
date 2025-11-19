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
    /// 为系统默认的数字类型提供扩展接口支持
    /// </summary>
    public static class __decimal
    {
        /// <summary>
        /// 将数字类型转换为整数类型数值
        /// </summary>
        /// <param name="self">数字对象实例</param>
        /// <returns>返回整数类型数值</returns>
        public static int ToInt32(this decimal self)
        {
            return (int) Math.Floor(self);
        }

        /// <summary>
        /// 将数字类型转换为浮点数类型数值
        /// </summary>
        /// <param name="self">数字对象实例</param>
        /// <returns>返回浮点数类型数值</returns>
        public static double ToDouble(this decimal self)
        {
            return (double) self;
        }

        /// <summary>
        /// 对数字进行舍入的函数
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
        /// <param name="self">数字对象实例</param>
        /// <param name="decimals">保留小数位数，默认为0，即保留到整数</param>
        /// <param name="mode">舍入方式</param>
        /// <returns>返回舍入后的数值</returns>
        public static decimal Round(this decimal self, int decimals = 0, MidpointRounding mode = MidpointRounding.AwayFromZero)
        {
            self = Math.Round(self, decimals, mode);
            return self;
        }

        /// <summary>
        /// 对数字进行舍入的函数，<br/>
        /// 使用规则请参数<see cref="Round(decimal, int, MidpointRounding)"/>
        /// </summary>
        /// <param name="self">数字对象实例</param>
        /// <param name="decimals">保留小数位数，默认为0，即保留到整数</param>
        /// <param name="mode">舍入方式</param>
        /// <returns>返回舍入后的数值</returns>
        public static decimal? Round(this decimal? self, int decimals = 0, MidpointRounding mode = MidpointRounding.AwayFromZero)
        {
            if (self.HasValue)
            {
                self = Math.Round(self.Value, decimals, mode);
            }
            return self;
        }
    }
}
