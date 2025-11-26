/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
/// Copyright (C) 2020 - 2022, Guangzhou Xinyuan Technology Co., Ltd.
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

using System.IO;

namespace NovaEngine.IO
{
    /// <summary>
    /// 缓冲字节流写操作接口
    /// </summary>
    public sealed class ByteStreamWriter
    {
        private MemoryStream _stream = null;
        private BinaryWriter _writer = null;

        /// <summary>
        /// 字节流缓冲区的新实例构建接口
        /// </summary>
        public ByteStreamWriter()
        {
            _stream = new MemoryStream();
            _writer = new BinaryWriter(_stream);
        }

        /// <summary>
        /// 字节流缓冲区实例析构接口
        /// </summary>
        ~ByteStreamWriter()
        {
            this.Close();
        }

        /// <summary>
        /// 关闭此字节缓冲区，清除所有数据
        /// </summary>
        public void Close()
        {
            if (null != _writer)
            {
                _writer.Close();
                _writer = null;
            }

            if (null != _stream)
            {
                _stream.Close();
                _stream = null;
            }
        }

        /// <summary>
        /// 向字节流缓冲区中写入字节数据
        /// </summary>
        /// <param name="v">字节数值</param>
        public void WriteByte(byte v)
        {
            _writer.Write(v);
        }

        /// <summary>
        /// 向字节流缓冲区中写入短整型数据
        /// </summary>
        /// <param name="v">短整型数值</param>
        public void WriteShort(short v)
        {
            _writer.Write(v);
        }

        /// <summary>
        /// 向字节流缓冲区中写入整型数据
        /// </summary>
        /// <param name="v">整型数值</param>
        public void WriteInt(int v)
        {
            _writer.Write(v);
        }

        /// <summary>
        /// 向字节流缓冲区中写入长整型数据
        /// </summary>
        /// <param name="v">长整型数值</param>
        public void WriteLong(long v)
        {
            _writer.Write(v);
        }

        /// <summary>
        /// 向字节流缓冲区中写入单精度浮点数据
        /// </summary>
        /// <param name="v">单精度浮点数值</param>
        public void WriteFloat(float v)
        {
            _writer.Write(v);
        }

        /// <summary>
        /// 向字节流缓冲区中写入双精度浮点数据
        /// </summary>
        /// <param name="v">双精度浮点数值</param>
        public void WriteDouble(double v)
        {
            _writer.Write(v);
        }

        /// <summary>
        /// 向字节流缓冲区中写入字符串数据
        /// </summary>
        /// <param name="v">字符串数值</param>
        public void WriteString(string v)
        {
            _writer.Write(v);
        }

        /// <summary>
        /// 向字节流缓冲区中写入字节数组数据
        /// </summary>
        /// <param name="v">字节数组数值</param>
        public void WriteBytes(byte[] v)
        {
            _writer.Write((int) v.Length);
            _writer.Write(v);
        }

        /// <summary>
        /// 将写入的数据流刷新到数据缓冲区
        /// </summary>
        public void Flush()
        {
            _writer.Flush();
        }

        /// <summary>
        /// 获取当前数据缓冲区中的字节数据流
        /// </summary>
        /// <returns>返回缓冲数据对应的字节流</returns>
        public byte[] ToBytes()
        {
            this.Flush();
            return _stream.ToArray();
        }
    }
}
