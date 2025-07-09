/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
/// Copyright (C) 2023, Guangzhou Shiyue Network Technology Co., Ltd.
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

namespace NovaEngine
{
    /// <summary>
    /// 场景数据结构对象类，对场景资源及相关使用参数进行统一封装管理
    /// 其数据结构包含场景资源，层级，物件，相关烘焙或光照等
    /// </summary>
    public partial class SceneRecordInfo
    {
        /// <summary>
        /// 场景数据状态类型枚举定义
        /// </summary>
        public enum EStateType : byte
        {
            /// <summary>
            /// 默认状态标识
            /// </summary>
            None = 0,

            /// <summary>
            /// 正在加载状态标识
            /// </summary>
            Loading,

            /// <summary>
            /// 完整数据状态标识
            /// </summary>
            Complete,

            /// <summary>
            /// 破损数据状态标识
            /// </summary>
            Fault,
        }
    }
}
