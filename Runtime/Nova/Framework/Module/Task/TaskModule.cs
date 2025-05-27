/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
/// Copyring (C) 2017 - 2020, Shanghai Tommon Network Technology Co., Ltd.
/// Copyring (C) 2020 - 2022, Guangzhou Xinyuan Technology Co., Ltd.
/// Copyring (C) 2022 - 2023, Shanghai Bilibili Technology Co., Ltd.
/// Copyring (C) 2023 - 2024, Guangzhou Shiyue Network Technology Co., Ltd.
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
    /// 任务管理器，统一处理所有外部协调的任务逻辑
    /// </summary>
    public sealed partial class TaskModule : ModuleObject
    {
        /// <summary>
        /// 任务句柄管理列表
        /// </summary>
        private IList<ITaskExecute> m_taskHandlerList = null;

        /// <summary>
        /// 任务模块事件类型
        /// </summary>
        public override int EventType => (int) EEventType.Task;

        /// <summary>
        /// 管理器对象初始化接口函数
        /// </summary>
        protected override void OnInitialize()
        {
            m_taskHandlerList = new List<ITaskExecute>();
        }

        /// <summary>
        /// 管理器对象清理接口函数
        /// </summary>
        protected override void OnCleanup()
        {
            m_taskHandlerList.Clear();
            m_taskHandlerList = null;
        }

        /// <summary>
        /// 管理器对象初始启动接口
        /// </summary>
        protected override void OnStartup()
        {
        }

        /// <summary>
        /// 管理器对象结束关闭接口
        /// </summary>
        protected override void OnShutdown()
        {
        }

        /// <summary>
        /// 管理器对象垃圾回收调度接口
        /// </summary>
        protected override void OnDump()
        {
        }

        /// <summary>
        /// 任务管理器内部事务更新接口
        /// </summary>
        protected override void OnUpdate()
        {
            for (int n = 0; n < m_taskHandlerList.Count; ++n)
            {
                ITaskExecute execute = m_taskHandlerList[n];
                execute.Execute();
            }
        }

        /// <summary>
        /// 任务管理器内部后置更新接口
        /// </summary>
        protected override void OnLateUpdate()
        {
            for (int n = 0; n < m_taskHandlerList.Count; ++n)
            {
                ITaskExecute execute = m_taskHandlerList[n];
                execute.LateExecute();
            }
        }

        /// <summary>
        /// 任务管理器添加回调句柄接口
        /// </summary>
        /// <param name="handler">目标回调句柄</param>
        public void AddTaskHandler(ITaskExecute handler)
        {
            for (int n = 0; n < m_taskHandlerList.Count; ++n)
            {
                ITaskExecute e = m_taskHandlerList[n];
                if (e.ExecutingCode() == handler.ExecutingCode())
                {
                    Logger.Warn("添加更新处理单元到任务管理器失败，目标{0}执行单元已存在！", e.ExecutingCode());
                    return;
                }
            }

            m_taskHandlerList.Add(handler);
        }

        /// <summary>
        /// 任务管理器移除回调句柄接口
        /// </summary>
        /// <param name="handler">目标回调句柄</param>
        public void RemoveTaskHandler(ITaskExecute handler)
        {
            for (int n = 0; n < m_taskHandlerList.Count; ++n)
            {
                ITaskExecute e = m_taskHandlerList[n];
                if (e.ExecutingCode() == handler.ExecutingCode())
                {
                    m_taskHandlerList.RemoveAt(n);
                    return;
                }
            }
        }

        #region 任务调度的回调执行接口定义

        /// <summary>
        /// 任务调度的回调执行对象接口，预定义回调操作接口
        /// </summary>
        public interface ITaskExecute
        {
            /// <summary>
            /// 逻辑执行操作接口
            /// </summary>
            void Execute();

            /// <summary>
            /// 逻辑后置执行操作接口
            /// </summary>
            void LateExecute();

            /// <summary>
            /// 更新对象唯一识别编码
            /// </summary>
            /// <returns>返回一个整型数字用于区分不同更新对象</returns>
            int ExecutingCode();
        }

        #endregion
    }
}
