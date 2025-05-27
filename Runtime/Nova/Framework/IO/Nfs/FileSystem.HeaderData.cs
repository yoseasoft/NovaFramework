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
            private readonly byte[] m_header;

            private readonly byte m_version;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = ENCRYPT_BYTES_LENGTH)]
            private readonly byte[] m_encryptBytes;

            private readonly int m_maxFileCount;
            private readonly int m_maxBlockCount;
            private readonly int m_blockCount;

            public HeaderData(int maxFileCount, int maxBlockCount)
                : this(0, new byte[ENCRYPT_BYTES_LENGTH], maxFileCount, maxBlockCount, 0)
            {
                Utility.Random.GetRandomBytes(this.m_encryptBytes);
            }

            public HeaderData(byte version, byte[] encryptBytes, int maxFileCount, int maxBlockCount, int blockCount)
            {
                this.m_header = Header;
                this.m_version = version;
                this.m_encryptBytes = encryptBytes;
                this.m_maxFileCount = maxFileCount;
                this.m_maxBlockCount = maxBlockCount;
                this.m_blockCount = blockCount;
            }

            public bool IsValid
            {
                get
                {
                    return m_header.Length == HEADER_LENGTH &&
                           m_header[0] == Header[0] && m_header[1] == Header[1] && m_header[2] == Header[2] &&
                           m_version == 0 && m_encryptBytes.Length == ENCRYPT_BYTES_LENGTH &&
                           m_maxFileCount > 0 && m_maxBlockCount > 0 && m_maxFileCount <= m_maxBlockCount &&
                           m_blockCount > 0 && m_blockCount <= m_maxBlockCount;
                }
            }

            public byte Version
            {
                get { return m_version; }
            }

            public int MaxFileCount
            {
                get { return m_maxFileCount; }
            }

            public int MaxBlockCount
            {
                get { return m_maxBlockCount; }
            }

            public int BlockCount
            {
                get { return m_blockCount; }
            }

            public byte[] GetEncryptBytes()
            {
                return m_encryptBytes;
            }

            public HeaderData SetBlockCount(int blockCount)
            {
                return new HeaderData(m_version, m_encryptBytes, m_maxFileCount, m_maxBlockCount, blockCount);
            }
        }
    }
}
