/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
/// Copyright (C) 2017 - 2020, Shanghai Tommon Network Technology Co., Ltd.
/// Copyright (C) 2020 - 2022, Guangzhou Xinyuan Technology Co., Ltd.
/// Copyright (C) 2022 - 2023, Shanghai Bilibili Technology Co., Ltd.
/// Copyright (C) 2023 - 2024, Guangzhou Shiyue Network Technology Co., Ltd.
/// Copyright (C) 2025, Hainan Yuanyou Information Tecdhnology Co., Ltd. Guangzhou Branch
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
using System.Threading.Tasks;

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
        /// 执行中状态下的任务对象管理列表
        /// </summary>
        private IList<ITask> _executionTasks = null;
        /// <summary>
        /// 已完成状态下的任务对象管理列表
        /// </summary>
        private IList<ITask> _completedTasks = null;

        /// <summary>
        /// 接口回调执行队列
        /// </summary>
        private Queue<System.Action> _executionActions = null;

        /// <summary>
        /// 任务模块事件类型
        /// </summary>
        public override int EventType => (int) ModuleEventType.Task;

        /// <summary>
        /// 管理器对象初始化接口函数
        /// </summary>
        protected override void OnInitialize()
        {
            _executionTasks = new List<ITask>();
            _completedTasks = new List<ITask>();

            _executionActions = new Queue<System.Action>();
        }

        /// <summary>
        /// 管理器对象清理接口函数
        /// </summary>
        protected override void OnCleanup()
        {
            _executionTasks.Clear();
            _executionTasks = null;
            _completedTasks.Clear();
            _completedTasks = null;

            _executionActions.Clear();
            _executionActions = null;
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
            if (_executionTasks.Count > 0)
            {
                for (int n = 0; n < _executionTasks.Count; ++n)
                {
                    ITask task = _executionTasks[n];
                    task.Execute();
                }
            }
        }

        /// <summary>
        /// 任务管理器内部后置更新接口
        /// </summary>
        protected override void OnLateUpdate()
        {
            if (_executionTasks.Count > 0)
            {
                for (int n = 0; n < _executionTasks.Count; ++n)
                {
                    ITask task = _executionTasks[n];
                    task.LateExecute();

                    // 任务完成
                    if (task.IsCompleted())
                    {
                        _completedTasks.Add(task);
                    }
                }

                if (_completedTasks.Count > 0)
                {
                    for (int n = 0; n < _completedTasks.Count; ++n)
                    {
                        RemoveTask(_completedTasks[n]);
                    }

                    _completedTasks.Clear();
                }
            }
        }

        /// <summary>
        /// 任务管理器添加任务对象实例的接口函数
        /// </summary>
        /// <param name="task">目标任务实例</param>
        public void AddTask(ITask task)
        {
            for (int n = 0; n < _executionTasks.Count; ++n)
            {
                ITask e = _executionTasks[n];
                if (e.UniqueCode() == task.UniqueCode())
                {
                    Logger.Warn("添加更新处理单元到任务管理器失败，目标{0}执行单元已存在！", e.UniqueCode());
                    return;
                }
            }

            _executionTasks.Add(task);
        }

        /// <summary>
        /// 任务管理器移除任务对象实例的接口函数
        /// </summary>
        /// <param name="task">目标任务实例</param>
        public void RemoveTask(ITask task)
        {
            for (int n = 0; n < _executionTasks.Count; ++n)
            {
                ITask e = _executionTasks[n];
                if (e.UniqueCode() == task.UniqueCode())
                {
                    _executionTasks.RemoveAt(n);
                    return;
                }
            }
        }
    }
}
