/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
/// Copyring (C) 2023, Guangzhou Shiyue Network Technology Co., Ltd.
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

using SystemIConvertible = System.IConvertible;

namespace NovaEngine
{
    /// <summary>
    /// 作为基础类型（如INT，BOOL等）的包装器对象，适用于属性成员为多变类型的应用场景
    /// </summary>
    public sealed class Value
    {
        /// <summary>
        /// 值包装对象的类型枚举
        /// </summary>
        public enum Type : byte
        {
            None = 0, // 无效类型，指明当前值对象为空值
            Byte,
            Int,
            UInt,
            Long,
            ULong,
            Float,
            Double,
            Boolean,
            String,
        }

        private Variable m_variable = null;

        public Value()
        {
        }

        public Value(byte value)
        {
        }

        public Value(int value) { }

        public Value(uint value) { }

        public Value(long value) { }

        public Value(ulong value) { }

        public Value(float value) { }

        public Value(double value) { }

        public Value(bool value) { }

        public Value(string value) { }

        private SystemIConvertible m_field;


    }
}
