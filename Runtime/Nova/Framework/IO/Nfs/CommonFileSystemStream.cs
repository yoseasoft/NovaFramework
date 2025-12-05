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
    /// 通用模式文件系统数据流定义类
    /// </summary>
    public sealed class CommonFileSystemStream : FileSystemStream, IDisposable
    {
        private readonly FileStream _fileStream;

        /// <summary>
        /// 初始化通用文件系统数据流的新实例
        /// </summary>
        /// <param name="fullPath">要加载的文件系统的完整路径</param>
        /// <param name="access">要加载的文件系统的访问方式</param>
        /// <param name="createNew">是否创建新的文件系统数据流</param>
        public CommonFileSystemStream(string fullPath, FileSystemAccessType accessType, bool createNew)
        {
            if (string.IsNullOrEmpty(fullPath))
            {
                throw new CFrameworkException("Full path is invalid.");
            }

            switch (accessType)
            {
                case FileSystemAccessType.ReadOnly:
                    _fileStream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                    break;

                case FileSystemAccessType.WriteOnly:
                    _fileStream = new FileStream(fullPath, createNew ? FileMode.Create : FileMode.Open, FileAccess.Write, FileShare.Read);
                    break;

                case FileSystemAccessType.ReadWrite:
                    _fileStream = new FileStream(fullPath, createNew ? FileMode.Create : FileMode.Open, FileAccess.ReadWrite, FileShare.Read);
                    break;

                default:
                    throw new CFrameworkException("Access type '{0}' is invalid.", (int) accessType);
            }
        }

        /// <summary>
        /// 获取或设置文件系统数据流位置
        /// </summary>
        protected internal override long Position
        {
            get
            {
                return _fileStream.Position;
            }
            set
            {
                _fileStream.Position = value;
            }
        }

        /// <summary>
        /// 获取文件系统数据流长度
        /// </summary>
        protected internal override long Length
        {
            get
            {
                return _fileStream.Length;
            }
        }

        /// <summary>
        /// 设置文件系统数据流长度
        /// </summary>
        /// <param name="length">要设置的文件系统数据流的长度</param>
        protected internal override void SetLength(long length)
        {
            _fileStream.SetLength(length);
        }

        /// <summary>
        /// 定位文件系统数据流位置
        /// </summary>
        /// <param name="offset">要定位的文件系统流位置的偏移</param>
        /// <param name="origin">要定位的文件系统流位置的方式</param>
        protected internal override void Seek(long offset, SeekOrigin origin)
        {
            _fileStream.Seek(offset, origin);
        }

        /// <summary>
        /// 从文件系统数据流中读取一个字节
        /// </summary>
        /// <returns>返回读取的字节，若已经到达文件结尾，则返回-1</returns>
        protected internal override int ReadByte()
        {
            return _fileStream.ReadByte();
        }

        /// <summary>
        /// 从文件系统数据流中读取二进制流
        /// </summary>
        /// <param name="buffer">存储读取文件内容的二进制流</param>
        /// <param name="startIndex">存储读取文件内容的二进制流的起始位置</param>
        /// <param name="length">存储读取文件内容的二进制流的长度</param>
        /// <returns>返回实际读取的字节数</returns>
        protected internal override int Read(byte[] buffer, int startIndex, int length)
        {
            return _fileStream.Read(buffer, startIndex, length);
        }

        /// <summary>
        /// 向文件系统数据流中写入一个字节
        /// </summary>
        /// <param name="value">要写入的字节</param>
        protected internal override void WriteByte(byte value)
        {
            _fileStream.WriteByte(value);
        }

        /// <summary>
        /// 向文件系统数据流中写入二进制流
        /// </summary>
        /// <param name="buffer">存储写入文件内容的二进制流</param>
        /// <param name="startIndex">存储写入文件内容的二进制流的起始位置</param>
        /// <param name="length">存储写入文件内容的二进制流的长度</param>
        protected internal override void Write(byte[] buffer, int startIndex, int length)
        {
            _fileStream.Write(buffer, startIndex, length);
        }

        /// <summary>
        /// 将文件系统数据流立刻更新到存储介质中
        /// </summary>
        protected internal override void Flush()
        {
            _fileStream.Flush();
        }

        /// <summary>
        /// 关闭文件系统数据流
        /// </summary>
        protected internal override void Close()
        {
            _fileStream.Close();
        }

        /// <summary>
        /// 销毁文件系统数据流
        /// </summary>
        public void Dispose()
        {
            _fileStream.Dispose();
        }
    }
}
