/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
/// Copyright (C) 2020 - 2022, Guangzhou Xinyuan Technology Co., Ltd.
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

namespace System.Customize.Extension
{
    /// <summary>
    /// 为系统默认的基础字节数据类型提供扩展接口支持
    /// </summary>
    public static class __byte
    {
        public static string ToHexString(this byte self)
        {
            return self.ToString("X2");
        }

        public static string ToHexString(this byte[] self)
        {
            Text.StringBuilder stringBuilder = new Text.StringBuilder();
            foreach (byte b in self)
            {
                stringBuilder.Append(b.ToString("X2"));
            }
            return stringBuilder.ToString();
        }

        public static string ToHexString(this byte[] self, string format)
        {
            Text.StringBuilder stringBuilder = new Text.StringBuilder();
            foreach (byte b in self)
            {
                stringBuilder.Append(b.ToString(format));
            }
            return stringBuilder.ToString();
        }

        public static string ToHexString(this byte[] self, int offset, int count)
        {
            Text.StringBuilder stringBuilder = new Text.StringBuilder();
            for (int n = offset; n < offset + count; ++n)
            {
                stringBuilder.Append(self[n].ToString("X2"));
            }
            return stringBuilder.ToString();
        }

        public static string ToTextString(this byte[] self)
        {
            return Text.Encoding.Default.GetString(self);
        }

        public static string ToTextString(this byte[] self, int index, int count)
        {
            return Text.Encoding.Default.GetString(self, index, count);
        }

        public static string ToUtf8String(this byte[] self)
        {
            return Text.Encoding.UTF8.GetString(self);
        }

        public static string ToUtf8String(this byte[] self, int index, int count)
        {
            return Text.Encoding.UTF8.GetString(self, index, count);
        }

        public static short ReadInt16(this byte[] self, int offset)
        {
            return (short) (self[offset] | (self[offset + 1] << 8));
        }

        public static ushort ReadUInt16(this byte[] self, int offset)
        {
            return (ushort) (self[offset] | (self[offset + 1] << 8));
        }

        public static int ReadInt32(this byte[] self, int offset)
        {
            return self[offset] | (self[offset + 1] << 8) | (self[offset + 2] << 16) | (self[offset + 3] << 24);
        }

        public static uint ReadUInt32(this byte[] self, int offset)
        {
            return (uint) (self[offset] | (self[offset + 1] << 8) | (self[offset + 2] << 16) | (self[offset + 3] << 24));
        }

        public static short ReadBigInt16(this byte[] self, int offset)
        {
            return (short) (self[offset + 1] | (self[offset] << 8));
        }

        public static ushort ReadBigUInt16(this byte[] self, int offset)
        {
            return (ushort) (self[offset + 1] | (self[offset] << 8));
        }

        public static int ReadBigInt32(this byte[] self, int offset)
        {
            return self[offset + 3] | (self[offset + 2] << 8) | (self[offset + 1] << 16) | (self[offset] << 24);
        }

        public static uint ReadBigUInt32(this byte[] self, int offset)
        {
            return (uint) (self[offset + 3] | (self[offset + 2] << 8) | (self[offset + 1] << 16) | (self[offset] << 24));
        }

        public static void WriteTo(this byte[] self, int offset, byte num)
        {
            self[offset] = num;
        }

        public static void WriteTo(this byte[] self, int offset, short num)
        {
            self[offset    ] = (byte)  (num & 0xff);
            self[offset + 1] = (byte) ((num & 0xff00) >> 8);
        }

        public static void WriteTo(this byte[] self, int offset, ushort num)
        {
            self[offset    ] = (byte)  (num & 0xff);
            self[offset + 1] = (byte) ((num & 0xff00) >> 8);
        }

        public static void WriteTo(this byte[] self, int offset, int num)
        {
            self[offset] = (byte) (num & 0xff);
            self[offset + 1] = (byte) ((num & 0xff00) >> 8);
            self[offset + 2] = (byte) ((num & 0xff0000) >> 16);
            self[offset + 3] = (byte) ((num & 0xff000000) >> 24);
        }

        public static void WriteTo(this byte[] self, int offset, uint num)
        {
            self[offset] = (byte) (num & 0xff);
            self[offset + 1] = (byte) ((num & 0xff00) >> 8);
            self[offset + 2] = (byte) ((num & 0xff0000) >> 16);
            self[offset + 3] = (byte) ((num & 0xff000000) >> 24);
        }

        public static void WriteToBig(this byte[] self, int offset, short num)
        {
            self[offset + 1] = (byte)  (num & 0xff);
            self[offset    ] = (byte) ((num & 0xff00) >> 8);
        }

        public static void WriteToBig(this byte[] self, int offset, ushort num)
        {
            self[offset + 1] = (byte)  (num & 0xff);
            self[offset    ] = (byte) ((num & 0xff00) >> 8);
        }

        public static void WriteToBig(this byte[] self, int offset, int num)
        {
            self[offset + 3] = (byte)  (num & 0xff);
            self[offset + 2] = (byte) ((num & 0xff00)     >> 8);
            self[offset + 1] = (byte) ((num & 0xff0000)   >> 16);
            self[offset    ] = (byte) ((num & 0xff000000) >> 24);
        }

        public static void WriteToBig(this byte[] self, int offset, uint num)
        {
            self[offset + 3] = (byte)  (num & 0xff);
            self[offset + 2] = (byte) ((num & 0xff00)     >> 8);
            self[offset + 1] = (byte) ((num & 0xff0000)   >> 16);
            self[offset    ] = (byte) ((num & 0xff000000) >> 24);
        }
    }
}
