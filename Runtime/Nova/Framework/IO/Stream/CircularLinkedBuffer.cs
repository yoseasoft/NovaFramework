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

using System.Collections.Generic;

using SystemArray = System.Array;
using SystemStream = System.IO.Stream;
using SystemBinaryReader = System.IO.BinaryReader;
using SystemBinaryWriter = System.IO.BinaryWriter;
using SystemMemoryStream = System.IO.MemoryStream;

namespace NovaEngine.IO
{
    /// <summary>
    /// 循环链接数据流缓冲区结构类型
    /// </summary>
    public sealed class CircularLinkedBuffer
    {
        public const int BUFFER_CHUNK_SIZE = (1024 * 8);

        /// <summary>
        /// 数据推送队列
        /// </summary>
        private readonly Queue<byte[]> m_bufferQueue = new Queue<byte[]>();

        /// <summary>
        /// 数据缓存队列
        /// </summary>
        private readonly Queue<byte[]> m_bufferCache = new Queue<byte[]>();

        private byte[] lastBuffer;

        public CircularLinkedBuffer()
        {
            this.AddLast();
        }

        public int LastIndex { get; set; }

        public int FirstIndex { get; set; }

        public byte[] First
        {
            get
            {
                if (this.m_bufferQueue.Count == 0)
                {
                    this.AddLast();
                }

                return this.m_bufferQueue.Peek();
            }
        }

        public byte[] Last
        {
            get
            {
                if (this.m_bufferQueue.Count == 0)
                {
                    this.AddLast();
                }

                return this.lastBuffer;
            }
        }

        /// <summary>
        /// 获取存储数据长度
        /// </summary>
        public long Length
        {
            get
            {
                int c = 0;
                if (this.m_bufferQueue.Count > 0)
                {
                    c = (this.m_bufferQueue.Count - 1) * BUFFER_CHUNK_SIZE + this.LastIndex - this.FirstIndex;
                }

                if (c < 0)
                {
                    Logger.Error("circular buffer count '{0}' error.", this.m_bufferQueue.Count);
                }

                return c;
            }
        }

        public void AddLast()
        {
            byte[] buffer;
            if (this.m_bufferCache.Count > 0)
            {
                buffer = this.m_bufferCache.Dequeue();
            }
            else
            {
                buffer = new byte[BUFFER_CHUNK_SIZE];
            }
            this.m_bufferQueue.Enqueue(buffer);
            this.lastBuffer = buffer;
        }

        public void RemoveFirst()
        {
            this.m_bufferCache.Enqueue(m_bufferQueue.Dequeue());
        }

        // 从CircularBuffer读到stream
        public void Read(SystemStream stream, int count)
        {
            if (count > this.Length)
            {
                throw new CFrameworkException("data out of the buffer range.");
            }

            int alreadyCopyCount = 0;
            while (alreadyCopyCount < count)
            {
                int n = count - alreadyCopyCount;
                if (BUFFER_CHUNK_SIZE - this.FirstIndex > n)
                {
                    stream.Write(this.First, this.FirstIndex, n);
                    this.FirstIndex += n;
                    alreadyCopyCount += n;
                }
                else
                {
                    stream.Write(this.First, this.FirstIndex, BUFFER_CHUNK_SIZE - this.FirstIndex);
                    alreadyCopyCount += BUFFER_CHUNK_SIZE - this.FirstIndex;
                    this.FirstIndex = 0;
                    this.RemoveFirst();
                }
            }
        }

        // 从stream写入CircularBuffer
        public void Write(SystemStream stream)
        {
            int count = (int) (stream.Length - stream.Position);

            int alreadyCopyCount = 0;
            while (alreadyCopyCount < count)
            {
                if (this.LastIndex == BUFFER_CHUNK_SIZE)
                {
                    this.AddLast();
                    this.LastIndex = 0;
                }

                int n = count - alreadyCopyCount;
                if (BUFFER_CHUNK_SIZE - this.LastIndex > n)
                {
                    stream.Read(this.lastBuffer, this.LastIndex, n);
                    this.LastIndex += count - alreadyCopyCount;
                    alreadyCopyCount += n;
                }
                else
                {
                    stream.Read(this.lastBuffer, this.LastIndex, BUFFER_CHUNK_SIZE - this.LastIndex);
                    alreadyCopyCount += BUFFER_CHUNK_SIZE - this.LastIndex;
                    this.LastIndex = BUFFER_CHUNK_SIZE;
                }
            }
        }

        // 把CircularBuffer中数据写入buffer
        public int Read(byte[] buffer, int offset, int count)
        {
            if (buffer.Length < offset + count)
            {
                throw new CFrameworkException("data out of the buffer range.");
            }

            long length = this.Length;
            if (length < count)
            {
                count = (int) length;
            }

            int alreadyCopyCount = 0;
            while (alreadyCopyCount < count)
            {
                int n = count - alreadyCopyCount;
                if (BUFFER_CHUNK_SIZE - this.FirstIndex > n)
                {
                    SystemArray.Copy(this.First, this.FirstIndex, buffer, alreadyCopyCount + offset, n);
                    this.FirstIndex += n;
                    alreadyCopyCount += n;
                }
                else
                {
                    SystemArray.Copy(this.First, this.FirstIndex, buffer, alreadyCopyCount + offset, BUFFER_CHUNK_SIZE - this.FirstIndex);
                    alreadyCopyCount += BUFFER_CHUNK_SIZE - this.FirstIndex;
                    this.FirstIndex = 0;
                    this.RemoveFirst();
                }
            }

            return count;
        }

        // 把buffer写入CircularBuffer中
        public void Write(byte[] buffer, int offset, int count)
        {
            int alreadyCopyCount = 0;
            while (alreadyCopyCount < count)
            {
                if (this.LastIndex == BUFFER_CHUNK_SIZE)
                {
                    this.AddLast();
                    this.LastIndex = 0;
                }

                int n = count - alreadyCopyCount;
                if (BUFFER_CHUNK_SIZE - this.LastIndex > n)
                {
                    SystemArray.Copy(buffer, alreadyCopyCount + offset, this.lastBuffer, this.LastIndex, n);
                    this.LastIndex += count - alreadyCopyCount;
                    alreadyCopyCount += n;
                }
                else
                {
                    SystemArray.Copy(buffer, alreadyCopyCount + offset, this.lastBuffer, this.LastIndex, BUFFER_CHUNK_SIZE - this.LastIndex);
                    alreadyCopyCount += BUFFER_CHUNK_SIZE - this.LastIndex;
                    this.LastIndex = BUFFER_CHUNK_SIZE;
                }
            }
        }

        // 清理buffer中的全部数据
        public void Clear()
        {
            this.m_bufferQueue.Clear();
            this.m_bufferCache.Clear();
        }
    }
}
