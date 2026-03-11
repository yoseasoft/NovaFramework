/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyright (C) 2025 - 2026, Hainan Yuanyou Information Technology Co., Ltd. Guangzhou Branch
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

namespace GameEngine
{
    /// <summary>
    /// 数据同步播报方式的枚举类型定义
    /// </summary>
    [Flags]
    public enum ReplicateAnnounceType : byte
    {
        /// <summary>
        /// 空白
        /// </summary>
        None = 0,

        /// <summary>
        /// 创建类型
        /// </summary>
        Created = 0x01,
        /// <summary>
        /// 删除类型
        /// </summary>
        Deleted = 0x02,
        /// <summary>
        /// 修改类型
        /// </summary>
        Changed = 0x04,

        /// <summary>
        /// 创建和删除类型
        /// </summary>
        CreatedAndDeleted = Created | Deleted,
        /// <summary>
        /// 创建和修改类型
        /// </summary>
        CreatedAndChanged = Created | Changed,
        /// <summary>
        /// 删除和修改类型
        /// </summary>
        DeletedAndChanged = Deleted | Changed,

        /// <summary>
        /// 创建、删除和修改类型
        /// </summary>
        All = Created | Deleted | Changed,
    }
}
