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

using System.Runtime.InteropServices;

namespace NovaEngine.IO.FileSystem
{
    /// <summary>
    /// 文件系统基类
    /// </summary>
    internal sealed partial class FileSystem : IFileSystem
    {
        /// <summary>
        /// 文件系统的头数据流对象类
        /// </summary>
        private struct HeaderData
        {
            private const int HEADER_LENGTH = 3;
            private const int ENCRYPT_BYTES_LENGTH = 4;
            private static readonly byte[] Header = new byte[HEADER_LENGTH] { (byte) 'G', (byte) 'F', (byte) 'F' };

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = HEADER_LENGTH)]
            private readonly byte[] _header;

            private readonly byte _version;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = ENCRYPT_BYTES_LENGTH)]
            private readonly byte[] _encryptBytes;

            private readonly int _maxFileCount;
            private readonly int _maxBlockCount;
            private readonly int _blockCount;

            public HeaderData(int maxFileCount, int maxBlockCount)
                : this(0, new byte[ENCRYPT_BYTES_LENGTH], maxFileCount, maxBlockCount, 0)
            {
                Utility.Random.GetRandomBytes(this._encryptBytes);
            }

            public HeaderData(byte version, byte[] encryptBytes, int maxFileCount, int maxBlockCount, int blockCount)
            {
                this._header = Header;
                this._version = version;
                this._encryptBytes = encryptBytes;
                this._maxFileCount = maxFileCount;
                this._maxBlockCount = maxBlockCount;
                this._blockCount = blockCount;
            }

            public bool IsValid
            {
                get
                {
                    return _header.Length == HEADER_LENGTH &&
                           _header[0] == Header[0] && _header[1] == Header[1] && _header[2] == Header[2] &&
                           _version == 0 && _encryptBytes.Length == ENCRYPT_BYTES_LENGTH &&
                           _maxFileCount > 0 && _maxBlockCount > 0 && _maxFileCount <= _maxBlockCount &&
                           _blockCount > 0 && _blockCount <= _maxBlockCount;
                }
            }

            public byte Version
            {
                get { return _version; }
            }

            public int MaxFileCount
            {
                get { return _maxFileCount; }
            }

            public int MaxBlockCount
            {
                get { return _maxBlockCount; }
            }

            public int BlockCount
            {
                get { return _blockCount; }
            }

            public byte[] GetEncryptBytes()
            {
                return _encryptBytes;
            }

            public HeaderData SetBlockCount(int blockCount)
            {
                return new HeaderData(_version, _encryptBytes, _maxFileCount, _maxBlockCount, blockCount);
            }
        }
    }
}
