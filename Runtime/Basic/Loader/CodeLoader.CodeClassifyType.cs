/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyright (C) 2023 - 2024, Guangzhou Shiyue Network Technology Co., Ltd.
/// Copyright (C) 2025, Hainan Yuanyou Information Technology Co., Ltd. Guangzhou Branch
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

namespace GameEngine.Loader
{
    /// 程序集的分析处理类
    public static partial class CodeLoader
    {
        /// <summary>
        /// 程序集的编码分类类型的枚举定义
        /// </summary>
        // [Flags]
        private enum CodeClassifyType : byte
        {
            /// <summary>
            /// 未知类型
            /// </summary>
            Unknown = 0,

            /// <summary>
            /// 输入类型
            /// </summary>
            Input = 1,

            /// <summary>
            /// 事件类型
            /// </summary>
            Event = 2,

            /// <summary>
            /// 网络类型
            /// </summary>
            Network = 3,

            /// <summary>
            /// 实体类型
            /// </summary>
            Bean = 11,

            /// <summary>
            /// 扩展类型
            /// </summary>
            Extend = 12,

            /// <summary>
            /// 通知类型
            /// </summary>
            Notice = 13,

            /// <summary>
            /// 切面类型
            /// </summary>
            Aspect = 21,

            /// <summary>
            /// 注入类型
            /// </summary>
            Inject = 22,

            /// <summary>
            /// 对象池类型
            /// </summary>
            Pool = 31,

            /// <summary>
            /// 编程接口类型
            /// </summary>
            Api = 41,
        }
    }
}
