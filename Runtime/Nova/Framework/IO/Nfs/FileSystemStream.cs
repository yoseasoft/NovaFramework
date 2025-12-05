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

using System;
using System.IO;

namespace NovaEngine.IO.FileSystem
{
    /// <summary>
    /// 文件系统数据流定义类
    /// </summary>
    public abstract class FileSystemStream
    {
        /// <summary>
        /// 缓存二进制流的长度
        /// </summary>
        protected const int CACHED_BYTES_LENGTH = 0x1000;

        /// <summary>
        /// 缓存二进制流的字节数组
        /// </summary>
        protected static readonly byte[] _cachedBytes = new byte[CACHED_BYTES_LENGTH];

        /// <summary>
        /// 获取或设置文件系统数据流位置
        /// </summary>
        protected internal abstract long Position
        {
            get;
            set;
        }

        /// <summary>
        /// 获取或设置文件系统数据流长度
        /// </summary>
        protected internal abstract long Length
        {
            get;
        }

        /// <summary>
        /// 设置文件系统数据流长度
        /// </summary>
        /// <param name="length">要设置的文件系统流的长度</param>
        protected internal abstract void SetLength(long length);

        /// <summary>
        /// 定位文件系统数据流位置
        /// </summary>
        /// <param name="offset">要定位的文件系统流位置的偏移</param>
        /// <param name="origin">要定位的文件系统流位置的方式</param>
        protected internal abstract void Seek(long offset, SeekOrigin origin);

        /// <summary>
        /// 从文件系统数据流中读取一个字节
        /// </summary>
        /// <returns>返回读取的字节，若已经到达文件结尾，则返回-1</returns>
        protected internal abstract int ReadByte();

        /// <summary>
        /// 从文件系统数据流中读取二进制流
        /// </summary>
        /// <param name="buffer">存储读取文件内容的二进制流</param>
        /// <param name="startIndex">存储读取文件内容的二进制流的起始位置</param>
        /// <param name="length">存储读取文件内容的二进制流的长度</param>
        /// <returns>返回实际读取的字节数</returns>
        protected internal abstract int Read(byte[] buffer, int startIndex, int length);

        /// <summary>
        /// 从文件系统数据流中读取二进制流
        /// </summary>
        /// <param name="stream">存储读取文件内容的二进制流</param>
        /// <param name="length">存储读取文件内容的二进制流的长度</param>
        /// <returns>返回实际读取的字节数</returns>
        protected internal int Read(Stream stream, int length)
        {
            int bytesRead = 0;
            int bytesLeft = length;
            while ((bytesRead = Read(_cachedBytes, 0, bytesLeft < CACHED_BYTES_LENGTH ? bytesLeft : CACHED_BYTES_LENGTH)) > 0)
            {
                bytesLeft -= bytesRead;
                stream.Write(_cachedBytes, 0, bytesRead);
            }

            Array.Clear(_cachedBytes, 0, CACHED_BYTES_LENGTH);
            return length - bytesLeft;
        }

        /// <summary>
        /// 向文件系统数据流中写入一个字节
        /// </summary>
        /// <param name="value">要写入的字节</param>
        protected internal abstract void WriteByte(byte value);

        /// <summary>
        /// 向文件系统流中写入二进制流
        /// </summary>
        /// <param name="buffer">存储写入文件内容的二进制流</param>
        /// <param name="startIndex">存储写入文件内容的二进制流的起始位置</param>
        /// <param name="length">存储写入文件内容的二进制流的长度</param>
        protected internal abstract void Write(byte[] buffer, int startIndex, int length);

        /// <summary>
        /// 向文件系统流中写入二进制流
        /// </summary>
        /// <param name="stream">存储写入文件内容的二进制流</param>
        /// <param name="length">存储写入文件内容的二进制流的长度</param>
        protected internal void Write(Stream stream, int length)
        {
            int bytesRead = 0;
            int bytesLeft = length;
            while ((bytesRead = stream.Read(_cachedBytes, 0, bytesLeft < CACHED_BYTES_LENGTH ? bytesLeft : CACHED_BYTES_LENGTH)) > 0)
            {
                bytesLeft -= bytesRead;
                Write(_cachedBytes, 0, bytesRead);
            }

            Array.Clear(_cachedBytes, 0, CACHED_BYTES_LENGTH);
        }

        /// <summary>
        /// 将文件系统数据流立刻更新到存储介质中
        /// </summary>
        protected internal abstract void Flush();

        /// <summary>
        /// 关闭文件系统数据流
        /// </summary>
        protected internal abstract void Close();
    }
}
