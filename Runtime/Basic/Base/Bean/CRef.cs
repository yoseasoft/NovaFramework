/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyright (C) 2023 - 2024, Guangzhou Shiyue Network Technology Co., Ltd.
/// Copyright (C) 2024 - 2025, Hurley, Independent Studio.
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

using SystemType = System.Type;

namespace GameEngine
{
    /// <summary>
    /// 引用对象抽象类，对场景中的引用对象上下文进行封装及调度管理
    /// </summary>
    public abstract partial class CRef : CBase, NovaEngine.IExecutable, NovaEngine.IUpdatable
    {
        /// <summary>
        /// 对象内部定时任务的会话管理容器
        /// </summary>
        private IList<int> _schedules;

        /// <summary>
        /// 引用对象初始化通知接口函数
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();

            OnDispatchProcessingInitialize();

            // 任务会话容器初始化
            _schedules = new List<int>();
        }

        /// <summary>
        /// 引用对象清理通知接口函数
        /// </summary>
        public override void Cleanup()
        {
            // 移除所有定时任务
            RemoveAllSchedules();
            Debugger.Assert(_schedules.Count == 0);
            _schedules = null;

            base.Cleanup();

            OnDispatchProcessingCleanup();
        }

        /// <summary>
        /// 引用对象执行通知接口函数
        /// </summary>
        public abstract void Execute();

        /// <summary>
        /// 引用对象后置执行通知接口函数
        /// </summary>
        public abstract void LateExecute();

        /// <summary>
        /// 引用对象刷新通知接口函数
        /// </summary>
        public abstract void Update();

        /// <summary>
        /// 引用对象后置刷新通知接口函数
        /// </summary>
        public abstract void LateUpdate();

        #region 引用对象行为检测封装接口函数（包括对象接口，特性等标签）

        /// <summary>
        /// 检测当前引用对象是否激活执行行为
        /// </summary>
        /// <returns>若引用对象激活执行行为则返回true，否则返回false</returns>
        protected internal virtual bool IsExecuteActivation()
        {
            if (typeof(IExecuteActivation).IsAssignableFrom(GetType()))
            {
                return true;
            }

            // 引用对象自身需要执行
            if (HasAspectBehaviourType(AspectBehaviourType.Execute) || HasAspectBehaviourType(AspectBehaviourType.LateExecute))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 检测当前引用对象是否激活刷新行为
        /// </summary>
        /// <returns>若引用对象激活刷新行为则返回true，否则返回false</returns>
        protected internal virtual bool IsUpdateActivation()
        {
            if (typeof(IUpdateActivation).IsAssignableFrom(GetType()))
            {
                return true;
            }

            // 引用对象自身需要刷新
            if (HasAspectBehaviourType(AspectBehaviourType.Update) || HasAspectBehaviourType(AspectBehaviourType.LateUpdate))
            {
                return true;
            }

            return false;
        }

        #endregion

        #region 引用对象定时任务相关操作函数合集

        /// <summary>
        /// 定时任务调度启动接口，设置启动一个新的无限循环模式的任务定时器
        /// </summary>
        /// <param name="interval">任务延时间隔，以毫秒为单位</param>
        /// <param name="handler">回调句柄</param>
        /// <returns>若任务启动成功，则返回对应的会话值，否则返回0</returns>
        public int Schedule(int interval, TimerHandler.TimerReportingHandler handler)
        {
            return Schedule(interval, NovaEngine.TimerModule.SCHEDULE_REPEAT_FOREVER, handler);
        }

        /// <summary>
        /// 定时任务调度启动接口，设置启动一个新的无限循环模式的任务定时器
        /// </summary>
        /// <param name="interval">任务延时间隔，以毫秒为单位</param>
        /// <param name="handler">回调句柄</param>
        /// <returns>若任务启动成功，则返回对应的会话值，否则返回0</returns>
        public int Schedule(int interval, TimerHandler.TimerReportingForSessionHandler handler)
        {
            return Schedule(interval, NovaEngine.TimerModule.SCHEDULE_REPEAT_FOREVER, handler);
        }

        /// <summary>
        /// 定时任务调度启动接口，设置启动一个新的无限循环模式的任务定时器
        /// </summary>
        /// <param name="name">任务名称</param>
        /// <param name="interval">任务延时间隔，以毫秒为单位</param>
        /// <param name="handler">回调句柄</param>
        /// <returns>若任务启动成功，则返回对应的会话值，否则返回0</returns>
        public int Schedule(string name, int interval, TimerHandler.TimerReportingHandler handler)
        {
            return Schedule(interval, NovaEngine.TimerModule.SCHEDULE_REPEAT_FOREVER, handler);
        }

        /// <summary>
        /// 定时任务调度启动接口，设置启动一个新的无限循环模式的任务定时器
        /// </summary>
        /// <param name="name">任务名称</param>
        /// <param name="interval">任务延时间隔，以毫秒为单位</param>
        /// <param name="handler">回调句柄</param>
        /// <returns>若任务启动成功，则返回对应的会话值，否则返回0</returns>
        public int Schedule(string name, int interval, TimerHandler.TimerReportingForSessionHandler handler)
        {
            return Schedule(interval, NovaEngine.TimerModule.SCHEDULE_REPEAT_FOREVER, handler);
        }

        /// <summary>
        /// 引用对象的定时任务调度启动接口，设置启动一个属于该对象的任务定时器<br/>
        /// 若需要设置一个无限循环的任务，可以将‘loop’设置为<see cref="NovaEngine.TimerModule.SCHEDULE_REPEAT_FOREVER"/>
        /// </summary>
        /// <param name="interval">任务延时间隔，以毫秒为单位</param>
        /// <param name="loop">任务循环次数</param>
        /// <param name="handler">回调句柄</param>
        /// <returns>若任务启动成功，则返回对应的会话值，否则返回0</returns>
        public int Schedule(int interval, int loop, TimerHandler.TimerReportingHandler handler)
        {
            int sessionID = TimerHandler.Instance.Schedule(interval, loop, handler, delegate (int v) {
                if (false == _schedules.Contains(v))
                {
                    Debugger.Warn("Could not found target session {0} scheduled with this object.", v);
                    return;
                }
                _schedules.Remove(v);
            });

            if (sessionID > 0)
            {
                Debugger.Assert(false == _schedules.Contains(sessionID), "Duplicate session ID.");
                _schedules.Add(sessionID);
            }

            return sessionID;
        }

        /// <summary>
        /// 引用对象的定时任务调度启动接口，设置启动一个属于该对象的任务定时器<br/>
        /// 若需要设置一个无限循环的任务，可以将‘loop’设置为<see cref="NovaEngine.TimerModule.SCHEDULE_REPEAT_FOREVER"/>
        /// </summary>
        /// <param name="interval">任务延时间隔，以毫秒为单位</param>
        /// <param name="loop">任务循环次数</param>
        /// <param name="handler">回调句柄</param>
        /// <returns>若任务启动成功，则返回对应的会话值，否则返回0</returns>
        public int Schedule(int interval, int loop, TimerHandler.TimerReportingForSessionHandler handler)
        {
            int sessionID = TimerHandler.Instance.Schedule(interval, loop, handler, delegate (int v) {
                if (false == _schedules.Contains(v))
                {
                    Debugger.Warn("Could not found target session {0} scheduled with this object.", v);
                    return;
                }
                _schedules.Remove(v);
            });

            if (sessionID > 0)
            {
                // Debugger.Assert(false == _schedules.Contains(sessionID), "Duplicate session ID.");
                if (false == _schedules.Contains(sessionID))
                {
                    _schedules.Add(sessionID);
                }
            }

            return sessionID;
        }

        /// <summary>
        /// 引用对象的定时任务调度启动接口，设置启动一个属于该对象的任务定时器<br/>
        /// 若需要设置一个无限循环的任务，可以将‘loop’设置为<see cref="NovaEngine.TimerModule.SCHEDULE_REPEAT_FOREVER"/>
        /// </summary>
        /// <param name="name">任务名称</param>
        /// <param name="interval">任务延时间隔，以毫秒为单位</param>
        /// <param name="loop">任务循环次数</param>
        /// <param name="handler">回调句柄</param>
        /// <returns>若任务启动成功，则返回对应的会话值，否则返回0</returns>
        public int Schedule(string name, int interval, int loop, TimerHandler.TimerReportingHandler handler)
        {
            int sessionID = TimerHandler.Instance.Schedule(name, interval, loop, handler, delegate (int v) {
                if (false == _schedules.Contains(v))
                {
                    Debugger.Warn("Could not found target session {0} scheduled with this object.", v);
                    return;
                }
                _schedules.Remove(v);
            });

            if (sessionID > 0)
            {
                // Debugger.Assert(false == _schedules.Contains(sessionID), "Duplicate session ID.");
                if (false == _schedules.Contains(sessionID))
                {
                    _schedules.Add(sessionID);
                }
            }

            return sessionID;
        }

        /// <summary>
        /// 引用对象的定时任务调度启动接口，设置启动一个属于该对象的任务定时器<br/>
        /// 若需要设置一个无限循环的任务，可以将‘loop’设置为<see cref="NovaEngine.TimerModule.SCHEDULE_REPEAT_FOREVER"/>
        /// </summary>
        /// <param name="name">任务名称</param>
        /// <param name="interval">任务延时间隔，以毫秒为单位</param>
        /// <param name="loop">任务循环次数</param>
        /// <param name="handler">回调句柄</param>
        /// <returns>若任务启动成功，则返回对应的会话值，否则返回0</returns>
        public int Schedule(string name, int interval, int loop, TimerHandler.TimerReportingForSessionHandler handler)
        {
            int sessionID = TimerHandler.Instance.Schedule(name, interval, loop, handler, delegate (int v) {
                if (false == _schedules.Contains(v))
                {
                    Debugger.Warn("Could not found target session {0} scheduled with this object.", v);
                    return;
                }
                _schedules.Remove(v);
            });

            if (sessionID > 0)
            {
                Debugger.Assert(false == _schedules.Contains(sessionID), "Duplicate session ID.");
                _schedules.Add(sessionID);
            }

            return sessionID;
        }

        /// <summary>
        /// 停止当前引用对象指定标识对应的定时任务
        /// </summary>
        /// <param name="sessionID">会话标识</param>
        public void Unschedule(int sessionID)
        {
            TimerHandler.Instance.Unschedule(sessionID);
        }

        /// <summary>
        /// 停止当前引用对象指定名称对应的定时任务
        /// </summary>
        /// <param name="name">任务名称</param>
        public void Unschedule(string name)
        {
            TimerHandler.Instance.Unschedule(name);
        }

        /// <summary>
        /// 停止当前引用对象设置的所有定时任务
        /// </summary>
        public void UnscheduleAll()
        {
            for (int n = 0; n < _schedules.Count; ++n)
            {
                Unschedule(_schedules[n]);
            }
        }

        /// <summary>
        /// 移除当前引用对象中启动的所有定时任务
        /// </summary>
        private void RemoveAllSchedules()
        {
            // 拷贝一份会话列表
            List<int> list = new List<int>();
            list.AddRange(_schedules);

            for (int n = 0; n < list.Count; ++n)
            {
                TimerHandler.Instance.RemoveTimerBySession(list[n]);
            }
        }

        #endregion
    }
}
