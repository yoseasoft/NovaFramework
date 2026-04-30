/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
/// Copyright (C) 2026, Hurley, Independent Studio.
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
    /// 为系统默认的数组类型提供扩展接口支持
    /// </summary>
    public static class __Array
    {
        /// <summary>
        /// 克隆数组并在新数组起始位置插入新数据
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="array">数组对象</param>
        /// <param name="value">新数据内容</param>
        /// <returns>返回克隆数组实例</returns>
        public static T[] Prepend<T>(this T[] array, T value)
        {
            if (array == null) return new[] { value };

            T[] result = new T[array.Length + 1];
            result[0] = value;
            Array.Copy(array, 0, result, 1, array.Length);
            return result;
        }
    }
}
