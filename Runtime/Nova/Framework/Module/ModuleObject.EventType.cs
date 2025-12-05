/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
/// Copyright (C) 2017 - 2020, Shanghai Tommon Network Technology Co., Ltd.
/// Copyright (C) 2020 - 2022, Guangzhou Xinyuan Technology Co., Ltd.
/// Copyright (C) 2022 - 2023, Shanghai Bilibili Technology Co., Ltd.
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

namespace NovaEngine.Module
{
    /// 引擎框架模块对象的抽象定义类
    internal abstract partial class ModuleObject
    {
        /// <summary>
        /// 模块对象事件类型定义
        /// 也可用此参数来对模块对象进行优先级定义，因为该类型在定义时已进行权重排序
        /// </summary>
        public enum ModuleEventType : byte
        {
            /// <summary>
            /// 默认定义
            /// </summary>
            Default = 0,

            /// <summary>
            /// 定时器模块
            /// </summary>
            Timer = 1,

            /// <summary>
            /// 线程模块
            /// </summary>
            Thread = 2,

            /// <summary>
            /// 任务模块
            /// </summary>
            Task = 3,

            /// <summary>
            /// 网络模块
            /// </summary>
            Network = 11,

            /// <summary>
            /// 输入模块
            /// </summary>
            Input = 21,

            /// <summary>
            /// 输出模块
            /// </summary>
            // Output = 22,

            /// <summary>
            /// 资源模块
            /// </summary>
            Resource = 31,

            /// <summary>
            /// 文件模块
            /// </summary>
            File = 32,

            /// <summary>
            /// 对象模块
            /// </summary>
            // Object = 41,

            /// <summary>
            /// 场景模块
            /// </summary>
            Scene = 42,

            /// <summary>
            /// 角色模块
            /// </summary>
            // Actor = 43,

            /// <summary>
            /// UI模块
            /// </summary>
            // Gui = 44,

            /// <summary>
            /// 用户自定义
            /// </summary>
            User = 101,
        }

        /// <summary>
        /// 通过模块的事件类型获取其对应实例的优先级
        /// </summary>
        /// <param name="type">模块事件类型</param>
        /// <returns>返回事件类型对应模块的优先级</returns>
        public static int GetModulePriorityWithEventType(ModuleEventType type)
        {
            return (int) type;
        }

        /// <summary>
        /// 通过模块的事件类型获取其对应实例的优先级
        /// </summary>
        /// <param name="type">模块事件类型</param>
        /// <returns>返回事件类型对应模块的优先级</returns>
        public static int GetModulePriorityWithEventType(int type)
        {
            return type;
        }
    }
}
