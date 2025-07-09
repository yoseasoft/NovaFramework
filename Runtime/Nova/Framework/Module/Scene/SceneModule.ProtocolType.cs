/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
/// Copyright (C) 2020 - 2022, Guangzhou Xinyuan Technology Co., Ltd.
/// Copyright (C) 2022 - 2023, Shanghai Bilibili Technology Co., Ltd.
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

namespace NovaEngine
{
    /// <summary>
    /// 场景管理器，处理场景相关的加载/卸载，及同屏场景间切换等访问接口
    /// </summary>
    public sealed partial class SceneModule : ModuleObject
    {
        /// <summary>
        /// 场景基础指令协议类型
        /// </summary>
        public enum ProtocolType : byte
        {
            /// <summary>
            /// 未知状态
            /// </summary>
            Unknown = 0,

            /// <summary>
            /// 加载成功
            /// </summary>
            Loaded = 11,

            /// <summary>
            /// 卸载成功
            /// </summary>
            Unloaded = 12,

            /// <summary>
            /// 载入异常
            /// </summary>
            Exception = 13,

            /// <summary>
            /// 加载开启通知
            /// </summary>
            Startup = 21,

            /// <summary>
            /// 加载进度通知
            /// </summary>
            Progressed = 22,

            /// <summary>
            /// 内部协议分隔符
            /// </summary>
            User = 101,
        }
    }
}
