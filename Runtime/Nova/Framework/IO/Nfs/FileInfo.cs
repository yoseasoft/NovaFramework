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

namespace NovaEngine.IO.FileSystem
{
    /// <summary>
    /// 文件描述信息基类
    /// </summary>
    public struct FileInfo
    {
        private readonly string m_name;
        private readonly long m_offset;
        private readonly int m_length;

        /// <summary>
        /// 文件描述信息的新实例构建接口
        /// </summary>
        /// <param name="name">文件名称</param>
        /// <param name="offset">文件数据偏移</param>
        /// <param name="length">文件数据长度</param>
        public FileInfo(string name, long offset, int length)
        {
            Logger.Assert(string.IsNullOrEmpty(name), "Name is invalid.");

            Logger.Assert(offset < 0L, "Offset is invalid.");

            Logger.Assert(length < 0, "Length is invalid.");

            this.m_name = name;
            this.m_offset = offset;
            this.m_length = length;
        }

        /// <summary>
        /// 获取文件信息的有效状态标识
        /// </summary>
        public bool IsValid
        {
            get
            {
                return !string.IsNullOrEmpty(this.m_name) && this.m_offset >= 0L && this.m_length >= 0;
            }
        }

        /// <summary>
        /// 获取文件名称
        /// </summary>
        public string Name
        {
            get { return m_name; }
        }

        /// <summary>
        /// 获取文件数据偏移
        /// </summary>
        public long Offset
        {
            get { return m_offset; }
        }

        /// <summary>
        /// 获取文件数据长度
        /// </summary>
        public int Length
        {
            get { return m_length; }
        }
    }
}
