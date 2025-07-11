/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyright (C) 2023 - 2024, Guangzhou Shiyue Network Technology Co., Ltd.
/// Copyright (C) 2025, Hurley, Independent Studio.
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
    /// <summary>
    /// 控制器的管理对象类，负责对所有的控制器对象实例进行统一的调度管理
    /// </summary>
    public static partial class ControllerManagement
    {
        /// <summary>
        /// 控制器的模块分类类型的枚举定义
        /// </summary>
        [System.Flags]
        private enum ModuleType : uint
        {
            /// <summary>
            /// 未知类型
            /// </summary>
            Unknown = 0x00,

            /// <summary>
            /// 实体类型
            /// </summary>
            Proto = 0x02,

            /// <summary>
            /// 对象池类型
            /// </summary>
            Pool = 0x04,

            /// <summary>
            /// 切面类型
            /// </summary>
            Aspect = 0x08,

            /// <summary>
            /// 注入类型
            /// </summary>
            Inject = 0x10,

            /// <summary>
            /// 事件类型
            /// </summary>
            Event = 0x20,

            /// <summary>
            /// 接口类型
            /// </summary>
            Api = 0x40,

            /// <summary>
            /// 状态类型
            /// </summary>
            Fsm = 0x80,
        }
    }
}
