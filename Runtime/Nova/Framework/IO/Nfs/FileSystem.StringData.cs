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

using SystemArray = System.Array;

namespace NovaEngine.IO.FileSystem
{
    /// <summary>
    /// 文件系统基类
    /// </summary>
    internal sealed partial class FileSystem : IFileSystem
    {
        /// <summary>
        /// 文件系统的字符串数据流对象类
        /// </summary>
        private struct StringData
        {
            private static readonly byte[] s_cachedBytes = new byte[byte.MaxValue + 1];

            private readonly byte m_length;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = byte.MaxValue)]
            private readonly byte[] m_bytes;

            public StringData(byte length, byte[] bytes)
            {
                m_length = length;
                m_bytes = bytes;
            }

            public string GetString(byte[] encryptBytes)
            {
                if (this.m_length <= 0)
                {
                    return null;
                }

                SystemArray.Copy(this.m_bytes, 0, s_cachedBytes, 0, this.m_length);
                Utility.Encryption.GetXorBytesOnSelf(s_cachedBytes, 0, this.m_length, encryptBytes);
                return Utility.Convertion.GetString(s_cachedBytes, 0, this.m_length);
            }

            public StringData SetString(string value, byte[] encryptBytes)
            {
                if (string.IsNullOrEmpty(value))
                {
                    return this.Clear();
                }

                int length = Utility.Convertion.GetBytes(value, s_cachedBytes);
                if (length > byte.MaxValue)
                {
                    throw new CFrameworkException("String '{0}' is too long.", value);
                }

                Utility.Encryption.GetXorBytesOnSelf(s_cachedBytes, encryptBytes);
                SystemArray.Copy(s_cachedBytes, 0, this.m_bytes, 0, length);
                return new StringData((byte) length, this.m_bytes);
            }

            public StringData Clear()
            {
                return new StringData(0, this.m_bytes);
            }
        }
    }
}
