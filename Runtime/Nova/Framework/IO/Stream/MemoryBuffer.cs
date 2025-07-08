/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
/// Copyright (C) 2020 - 2022, Guangzhou Xinyuan Technology Co., Ltd.
/// Copyright (C) 2022 - 2023, Shanghai Bilibili Technology Co., Ltd.
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

using System;

namespace NovaEngine.IO
{
    /// <summary>
    /// 基于内存字节流封装的缓冲区对象类，用于提供字节流数据的转译操作
    /// </summary>
    public sealed class MemoryBuffer : System.IO.MemoryStream, System.Buffers.IBufferWriter<byte>
    {
        private int _origin;

        public MemoryBuffer()
        {
        }

        public MemoryBuffer(int capacity) : base(capacity)
        {
        }

        public MemoryBuffer(byte[] buffer) : base(buffer)
        {
        }

        public MemoryBuffer(byte[] buffer, int index, int length) : base(buffer, index, length)
        {
            this._origin = index;
        }

        public System.ReadOnlyMemory<byte> WrittenMemory => this.GetBuffer().AsMemory(this._origin, (int) this.Position);

        public System.ReadOnlySpan<byte> WrittenSpan => this.GetBuffer().AsSpan(this._origin, (int) this.Position);

        public void Advance(int count)
        {
            long newLength = this.Position + count;
            if (newLength > this.Length)
            {
                this.SetLength(newLength);
            }
            this.Position = newLength;
        }

        public System.Memory<byte> GetMemory(int sizeHint = 0)
        {
            if (this.Length - this.Position < sizeHint)
            {
                this.SetLength(this.Position + sizeHint);
            }
            var memory = this.GetBuffer().AsMemory((int) this.Position + this._origin, (int) (this.Length - this.Position));
            return memory;
        }

        public System.Span<byte> GetSpan(int sizeHint = 0)
        {
            if (this.Length - this.Position < sizeHint)
            {
                this.SetLength(this.Position + sizeHint);
            }
            var span = this.GetBuffer().AsSpan((int) this.Position + this._origin, (int) (this.Length - this.Position));
            return span;
        }
    }
}
