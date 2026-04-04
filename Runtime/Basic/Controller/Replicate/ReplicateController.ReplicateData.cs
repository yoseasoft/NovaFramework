/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyright (C) 2024 - 2025, Hurley, Independent Studio.
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

namespace GameEngine
{
    /// 数据同步管理对象类
    internal sealed partial class ReplicateController
    {
        /// <summary>
        /// 同步的数据存储对象类，用于临时存放同步数据的标签
        /// </summary>
        private class ReplicateData : NovaEngine.IReference
        {
            /// <summary>
            /// 数据对象的标签
            /// </summary>
            private string _tags;
            /// <summary>
            /// 数据标签的播报类型
            /// </summary>
            private ReplicateAnnounceType _announceType;

            public string Tags => _tags;
            public ReplicateAnnounceType AnnounceType => _announceType;

            /// <summary>
            /// 同步数据对象的构造函数
            /// </summary>
            public ReplicateData()
            {
                _tags = null;
                _announceType = ReplicateAnnounceType.None;
            }

            /// <summary>
            /// 同步数据对象的构造函数
            /// </summary>
            /// <param name="tags">数据标签</param>
            /// <param name="announceType">数据播报类型</param>
            public ReplicateData(string tags, ReplicateAnnounceType announceType)
            {
                this._tags = tags;
                this._announceType = announceType;
            }

            /// <summary>
            /// 同步数据对象初始化函数接口
            /// </summary>
            public void Initialize()
            {
            }

            /// <summary>
            /// 同步数据对象清理函数接口
            /// </summary>
            public void Cleanup()
            {
                _tags = null;
                _announceType = ReplicateAnnounceType.None;
            }
        }
    }
}
