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

namespace NovaEngine
{
    /// <summary>
    /// 事件对象缓冲池实例定义
    /// </summary>
    /// <typeparam name="T">事件类型</typeparam>
    internal sealed partial class EventPool<T> where T : EventArgs
    {
        /// <summary>
        /// 事件池分配处理模式的枚举类型定义
        /// </summary>
        [System.Flags]
        internal enum AllowHandlerType : byte
        {
            /// <summary>
            /// 默认事件池模式，即必须存在有且只有一个事件处理函数
            /// </summary>
            Default = 0,

            /// <summary>
            /// 允许不存在事件处理函数
            /// </summary>
            AllowNoHandler = 1,

            /// <summary>
            /// 允许存在多个事件处理函数
            /// </summary>
            AllowMultiHandler = 2,

            /// <summary>
            /// 允许存在重复的事件处理函数
            /// </summary>
            AllowDuplicateHandler = 4,
        }
    }
}
