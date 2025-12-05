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
using System.Runtime.InteropServices;

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
            private static readonly byte[] _cachedBytes = new byte[byte.MaxValue + 1];

            private readonly byte _length;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = byte.MaxValue)]
            private readonly byte[] _bytes;

            public StringData(byte length, byte[] bytes)
            {
                _length = length;
                _bytes = bytes;
            }

            public string GetString(byte[] encryptBytes)
            {
                if (this._length <= 0)
                {
                    return null;
                }

                Array.Copy(this._bytes, 0, _cachedBytes, 0, this._length);
                Utility.Encryption.GetXorBytesOnSelf(_cachedBytes, 0, this._length, encryptBytes);
                return Utility.Convertion.GetString(_cachedBytes, 0, this._length);
            }

            public StringData SetString(string value, byte[] encryptBytes)
            {
                if (string.IsNullOrEmpty(value))
                {
                    return this.Clear();
                }

                int length = Utility.Convertion.GetBytes(value, _cachedBytes);
                if (length > byte.MaxValue)
                {
                    throw new CFrameworkException("String '{0}' is too long.", value);
                }

                Utility.Encryption.GetXorBytesOnSelf(_cachedBytes, encryptBytes);
                Array.Copy(_cachedBytes, 0, this._bytes, 0, length);
                return new StringData((byte) length, this._bytes);
            }

            public StringData Clear()
            {
                return new StringData(0, this._bytes);
            }
        }
    }
}
