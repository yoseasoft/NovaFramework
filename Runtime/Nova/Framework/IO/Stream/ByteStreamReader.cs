/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
/// Copyring (C) 2020 - 2022, Guangzhou Xinyuan Technology Co., Ltd.
/// Copyring (C) 2022 - 2023, Shanghai Bilibili Technology Co., Ltd.
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

using SystemBinaryReader = System.IO.BinaryReader;
using SystemMemoryStream = System.IO.MemoryStream;

namespace NovaEngine.IO
{
    /// <summary>
    /// 缓冲字节流读操作接口
    /// </summary>
    public sealed class ByteStreamReader
    {
        private SystemMemoryStream m_stream = null;
        private SystemBinaryReader m_reader = null;

        /// <summary>
        /// 字节流缓冲区的新实例构建接口
        /// </summary>
        /// <param name="data">初始字节流</param>
        public ByteStreamReader(byte[] data)
        {
            Logger.Assert(null != data);

            m_stream = new SystemMemoryStream(data);
            m_reader = new SystemBinaryReader(m_stream);
        }

        /// <summary>
        /// 字节流缓冲区实例析构接口
        /// </summary>
        ~ByteStreamReader()
        {
            this.Close();
        }

        /// <summary>
        /// 关闭此字节缓冲区，清除所有数据
        /// </summary>
        public void Close()
        {
            if (null != m_reader)
            {
                m_reader.Close();
                m_reader = null;
            }

            if (null != m_stream)
            {
                m_stream.Close();
                m_stream = null;
            }
        }

        /// <summary>
        /// 从字节流缓冲区中读取字节数据
        /// </summary>
        /// <returns>返回字节数据</returns>
        public byte ReadByte()
        {
            return m_reader.ReadByte();
        }

        /// <summary>
        /// 从字节流缓冲区中读取短整型数据
        /// </summary>
        /// <returns>返回短整型数据</returns>
        public short ReadShort()
        {
            return m_reader.ReadInt16();
        }

        /// <summary>
        /// 从字节流缓冲区中读取整型数据
        /// </summary>
        /// <returns>返回整型数据</returns>
        public int ReadInt()
        {
            return m_reader.ReadInt32();
        }

        /// <summary>
        /// 从字节流缓冲区中读取长整型数据
        /// </summary>
        /// <returns>返回长整型数据</returns>
        public long ReadLong()
        {
            return m_reader.ReadInt64();
        }

        /// <summary>
        /// 从字节流缓冲区中读取单精度浮点数据
        /// </summary>
        /// <returns>返回单精度浮点数据</returns>
        public float ReadFloat()
        {
            return m_reader.ReadSingle();
        }

        /// <summary>
        /// 从字节流缓冲区中读取双精度浮点数据
        /// </summary>
        /// <returns>返回双精度浮点数据</returns>
        public double ReadDouble()
        {
            return m_reader.ReadDouble();
        }

        /// <summary>
        /// 从字节流缓冲区中读取字符串数据
        /// </summary>
        /// <returns>返回字符串数据</returns>
        public string ReadString()
        {
            return m_reader.ReadString();
        }

        /// <summary>
        /// 从字节流缓冲区中读取字节数组数据
        /// </summary>
        /// <returns>返回字节数组数据</returns>
        public byte[] ReadBytes()
        {
            int count = this.ReadInt();
            return m_reader.ReadBytes(count);
        }
    }
}
