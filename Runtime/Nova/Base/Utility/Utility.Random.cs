/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
/// Copyright (C) 2022 - 2023, Shanghai Bilibili Technology Co., Ltd.
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

using SystemDateTime = System.DateTime;
using SystemRandom = System.Random;

namespace NovaEngine
{
    /// <summary>
    /// 实用函数集合工具类
    /// </summary>
    public static partial class Utility
    {
        /// <summary>
        /// 随机相关实用函数集合
        /// </summary>
        public static class Random
        {
            private static SystemRandom _random = new SystemRandom((int) SystemDateTime.Now.Ticks);

            /// <summary>
            /// 设置随机数种子
            /// </summary>
            /// <param name="seed">随机数种子</param>
            public static void SetSeed(int seed)
            {
                _random = new SystemRandom(seed);
            }

            /// <summary>
            /// 返回非负随机数
            /// </summary>
            /// <returns>大于等于零且小于System.Int32.MaxValue的32位带符号整数</returns>
            public static int Next()
            {
                return _random.Next();
            }

            /// <summary>
            /// 返回一个小于所指定最大值的非负随机数
            /// </summary>
            /// <param name="maxValue">要生成的随机数的上界（随机数不能取该上界值），maxValue必须大于等于零</param>
            /// <returns>大于等于零且小于maxValue的32位带符号整数，即：返回值的范围通常包括零但不包括maxValue；如果maxValue等于零，则返回maxValue</returns>
            public static int Next(int maxValue)
            {
                return _random.Next(maxValue);
            }

            /// <summary>
            /// 返回一个指定范围内的随机数
            /// </summary>
            /// <param name="minValue">返回的随机数的下界（随机数可取该下界值）</param>
            /// <param name="maxValue">返回的随机数的上界（随机数不能取该上界值），maxValue必须大于等于minValue</param>
            /// <returns>一个大于等于minValue且小于maxValue的32位带符号整数，即：返回的值范围包括minValue但不包括maxValue；如果minValue等于maxValue，则返回minValue</returns>
            public static int Next(int minValue, int maxValue)
            {
                return _random.Next(minValue, maxValue);
            }

            /// <summary>
            /// 返回一个介于0.0和1.0之间的随机数
            /// </summary>
            /// <returns>大于等于0.0并且小于1.0的双精度浮点数</returns>
            public static double NextDouble()
            {
                return _random.NextDouble();
            }

            /// <summary>
            /// 用随机数填充指定字节数组的元素
            /// </summary>
            /// <param name="buffer">包含随机数的字节数组</param>
            public static void NextBytes(byte[] buffer)
            {
                _random.NextBytes(buffer);
            }
        }
    }
}
