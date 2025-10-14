/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
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

using System.Collections.Generic;

namespace NovaEngine
{
    /// <summary>
    /// 任务管理器，统一处理所有外部协调的任务逻辑<br/>
    /// 参考开源项目<code>UnityMainThreadDispatcher</code>的实现方式，以队列的形式逐个完成任务的调度<br/>
    /// 开源项目下载地址：https://github.com/PimDeWitte/UnityMainThreadDispatcher.git
    /// </summary>
    public sealed partial class TaskModule : ModuleObject
    {
        /// <summary>
        /// 任务调度的回调执行对象接口，预定义回调操作接口
        /// </summary>
        public interface ITask
        {
            /// <summary>
            /// 任务对象逻辑执行操作接口
            /// </summary>
            void Execute();

            /// <summary>
            /// 任务对象逻辑后置执行操作接口
            /// </summary>
            void LateExecute();

            /// <summary>
            /// 任务对象唯一识别编码
            /// </summary>
            /// <returns>返回一个整型数字用于区分不同任务对象</returns>
            int UniqueCode();

            /// <summary>
            /// 任务对象完成状态检测接口
            /// </summary>
            /// <returns>若任务对象处于完成状态则返回true，否则返回false</returns>
            bool IsCompleted();
        }
    }
}
