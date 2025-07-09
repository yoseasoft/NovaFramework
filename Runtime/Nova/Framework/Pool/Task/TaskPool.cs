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

using System.Collections.Generic;

namespace NovaEngine
{
    /// <summary>
    /// 任务对象缓冲池实例定义
    /// </summary>
    /// <typeparam name="T">任务类型</typeparam>
    internal sealed partial class TaskPool<T> where T : TaskArgs
    {
        private readonly Stack<ITaskAgent<T>> _freeAgents;
        private readonly CacheLinkedList<ITaskAgent<T>> _workingAgents;
        private readonly CacheLinkedList<T> _waitingTasks;
        private bool _isPaused;

        /// <summary>
        /// 任务缓冲池的新实例构建接口
        /// </summary>
        public TaskPool()
        {
            _freeAgents = new Stack<ITaskAgent<T>>();
            _workingAgents = new CacheLinkedList<ITaskAgent<T>>();
            _waitingTasks = new CacheLinkedList<T>();
            _isPaused = false;
        }

        /// <summary>
        /// 获取或设置任务池暂停状态标识
        /// </summary>
        public bool Paused
        {
            get { return _isPaused; }
            set { _isPaused = value; }
        }

        /// <summary>
        /// 获取任务代理的开启总数量
        /// </summary>
        public int TotalAgentCount
        {
            get { return (FreeAgentCount + WorkingAgentCount); }
        }

        /// <summary>
        /// 获取当前可用的任务代理数量
        /// </summary>
        public int FreeAgentCount
        {
            get { return _freeAgents.Count; }
        }

        /// <summary>
        /// 获取工作中的任务代理数量
        /// </summary>
        public int WorkingAgentCount
        {
            get { return _workingAgents.Count; }
        }

        /// <summary>
        /// 获取处于等待状态的任务数量
        /// </summary>
        public int WaitingTaskCount
        {
            get { return _waitingTasks.Count; }
        }

        /// <summary>
        /// 任务池轮询调度接口
        /// </summary>
        public void Update()
        {
            if (_isPaused)
            {
                return;
            }

            OnRunningTaskProcess();
            OnWaitingTaskProcess();
        }

        /// <summary>
        /// 增加任务代理对象实例
        /// </summary>
        /// <param name="agent">任务代理对象实例</param>
        public void AddAgent(ITaskAgent<T> agent)
        {
            if (null == agent)
            {
                throw new CFrameworkException("Task agent is invalid.");
            }

            agent.Initialize();
            _freeAgents.Push(agent);
        }

        /// <summary>
        /// 增加任务实例
        /// </summary>
        /// <param name="task">任务对象实例</param>
        public void AddTask(T task)
        {
            LinkedListNode<T> current = _waitingTasks.Last;
            while (null != current)
            {
                if (task.Priority <= current.Value.Priority)
                {
                    break;
                }

                current = current.Previous;
            }

            if (null != current)
            {
                _waitingTasks.AddAfter(current, task);
            }
            else
            {
                _waitingTasks.AddFirst(task);
            }
        }

        /// <summary>
        /// 从当前的等待列表及执行列表中移除指定序列标识的任务实例
        /// </summary>
        /// <param name="serialID">任务的序列编号</param>
        /// <returns>若移除任务成功则返回true，否则返回false</returns>
        public bool RemoveTask(int serialID)
        {
            foreach (T task in _waitingTasks)
            {
                if (task.SerialID == serialID)
                {
                    _waitingTasks.Remove(task);
                    ReferencePool.Release(task);
                    return true;
                }
            }

            foreach (ITaskAgent<T> workingAgent in _workingAgents)
            {
                if (workingAgent.Task.SerialID == serialID)
                {
                    T task = workingAgent.Task;
                    workingAgent.Reset();
                    _freeAgents.Push(workingAgent);
                    _workingAgents.Remove(workingAgent);
                    ReferencePool.Release(task);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 移除全部任务实例
        /// </summary>
        public void RemoveAllTasks()
        {
            foreach (T task in _waitingTasks)
            {
                ReferencePool.Release(task);
            }
            _waitingTasks.Clear();

            foreach (ITaskAgent<T> workingAgent in _workingAgents)
            {
                T task = workingAgent.Task;
                workingAgent.Reset();
                _freeAgents.Push(workingAgent);
                ReferencePool.Release(task);
            }
            _workingAgents.Clear();
        }

        public TaskInfo[] GetAllTaskInfos()
        {
            List<TaskInfo> results = new List<TaskInfo>();
            foreach (ITaskAgent<T> workingAgent in _workingAgents)
            {
                T workingTask = workingAgent.Task;
                results.Add(new TaskInfo(workingTask.SerialID, workingTask.Priority, workingTask.Done ? TaskStatus.Done : TaskStatus.Doing, workingTask.Description));
            }

            foreach (T waitingTask in _waitingTasks)
            {
                results.Add(new TaskInfo(waitingTask.SerialID, waitingTask.Priority, TaskStatus.Todo, waitingTask.Description));
            }

            return results.ToArray();
        }

        private void OnRunningTaskProcess()
        {
            LinkedListNode<ITaskAgent<T>> current = _workingAgents.First;
            while (null != current)
            {
                T task = current.Value.Task;
                if (false == task.Done)
                {
                    current.Value.Update();
                    current = current.Next;
                    continue;
                }

                LinkedListNode<ITaskAgent<T>> next = current.Next;
                current.Value.Reset();
                _freeAgents.Push(current.Value);
                _workingAgents.Remove(current);
                ReferencePool.Release(task);
                current = next;
            }
        }

        private void OnWaitingTaskProcess()
        {
            LinkedListNode<T> current = _waitingTasks.First;
            while (null != current && FreeAgentCount > 0)
            {
                ITaskAgent<T> agent = _freeAgents.Pop();
                LinkedListNode<ITaskAgent<T>> agentNode = _workingAgents.AddLast(agent);
                T task = current.Value;
                LinkedListNode<T> next = current.Next;
                TaskStartupResultType status = agent.Startup(task);
                if (TaskStartupResultType.Done == status || TaskStartupResultType.OnWait == status || TaskStartupResultType.UnknownError == status)
                {
                    agent.Reset();
                    _freeAgents.Push(agent);
                    _workingAgents.Remove(agentNode);
                }

                if (TaskStartupResultType.Done == status || TaskStartupResultType.OnResume == status || TaskStartupResultType.UnknownError == status)
                {
                    _waitingTasks.Remove(current);
                }

                if (TaskStartupResultType.Done == status || TaskStartupResultType.UnknownError == status)
                {
                    ReferencePool.Release(task);
                }

                current = next;
            }
        }
    }
}
