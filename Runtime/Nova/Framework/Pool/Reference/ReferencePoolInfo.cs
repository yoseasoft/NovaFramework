/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
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

using SystemType = System.Type;
using SystemLayoutKind = System.Runtime.InteropServices.LayoutKind;

namespace NovaEngine
{
    /// <summary>
    /// 引用对象缓冲池信息数据结构定义
    /// </summary>
    [System.Runtime.InteropServices.StructLayout(SystemLayoutKind.Auto)]
    public struct ReferencePoolInfo
    {
        private readonly SystemType m_type;
        private readonly int m_unusedReferenceCount;
        private readonly int m_usingReferenceCount;
        private readonly int m_acquireReferenceCount;
        private readonly int m_releaseReferenceCount;
        private readonly int m_addReferenceCount;
        private readonly int m_removeReferenceCount;

        /// <summary>
        /// 引用对象池数据结构的新实例构建接口
        /// </summary>
        /// <param name="type">引用池类型</param>
        /// <param name="unusedReferenceCount">未使用引用数量</param>
        /// <param name="usingReferenceCount">正在使用引用数量</param>
        /// <param name="acquireReferenceCount">获取引用数量</param>
        /// <param name="releaseReferenceCount">归还引用数量</param>
        /// <param name="addReferenceCount">增加引用数量</param>
        /// <param name="removeReferenceCount">移除引用数量</param>
        public ReferencePoolInfo(SystemType type,
                                 int unusedReferenceCount,
                                 int usingReferenceCount,
                                 int acquireReferenceCount,
                                 int releaseReferenceCount,
                                 int addReferenceCount,
                                 int removeReferenceCount)
        {
            m_type = type;
            m_unusedReferenceCount = unusedReferenceCount;
            m_usingReferenceCount = usingReferenceCount;
            m_acquireReferenceCount = acquireReferenceCount;
            m_releaseReferenceCount = releaseReferenceCount;
            m_addReferenceCount = addReferenceCount;
            m_removeReferenceCount = removeReferenceCount;
        }

        /// <summary>
        /// 获取引用对象池数据结构实例类型
        /// </summary>
        public SystemType Type
        {
            get { return m_type; }
        }

        /// <summary>
        /// 获取未使用引用数量
        /// </summary>
        public int UnusedReferenceCount
        {
            get { return m_unusedReferenceCount; }
        }

        /// <summary>
        /// 获取正在使用引用数量
        /// </summary>
        public int UsingReferenceCount
        {
            get { return m_usingReferenceCount; }
        }

        /// <summary>
        /// 获取获取引用数量
        /// </summary>
        public int AcquireReferenceCount
        {
            get { return m_acquireReferenceCount; }
        }

        /// <summary>
        /// 获取归还引用数量
        /// </summary>
        public int ReleaseReferenceCount
        {
            get { return m_releaseReferenceCount; }
        }

        /// <summary>
        /// 获取增加引用数量
        /// </summary>
        public int AddReferenceCount
        {
            get { return m_addReferenceCount; }
        }

        /// <summary>
        /// 获取移除引用数量
        /// </summary>
        public int RemoveReferenceCount
        {
            get { return m_removeReferenceCount; }
        }
    }
}
