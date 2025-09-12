/// -------------------------------------------------------------------------------
/// GameEngine Framework
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

using System.Collections.Generic;

namespace GameEngine
{
    /// <summary>
    /// 定时器模块封装的句柄对象类
    /// 模块具体功能接口请参考<see cref="NovaEngine.TimerModule"/>类
    /// </summary>
    public sealed partial class TimerHandler : BaseHandler
    {
        /// <summary>
        /// 定时器通知回调函数接口
        /// </summary>
        public delegate void TimerReportingHandler();

        /// <summary>
        /// 定时器通知回调函数接口
        /// </summary>
        /// <param name="sessionID">定时器对应的唯一标识</param>
        public delegate void TimerReportingForSessionHandler(int sessionID);

        /// <summary>
        /// 定时器到时回调函数映射字典
        /// </summary>
        private IDictionary<int, TimerReportingCallback> _timerClockingCallbacks;

        /// <summary>
        /// 定时器结束回调函数映射字典
        /// </summary>
        private IDictionary<int, TimerReportingCallback> _timerFinishingCallbacks;

        /// <summary>
        /// 句柄对象的单例访问获取接口
        /// </summary>
        public static TimerHandler Instance => HandlerManagement.TimerHandler;

        /// <summary>
        /// 句柄对象内置初始化接口函数
        /// </summary>
        /// <returns>若句柄对象初始化成功则返回true，否则返回false</returns>
        protected override bool OnInitialize()
        {
            // 初始化映射字典
            _timerClockingCallbacks = new Dictionary<int, TimerReportingCallback>();
            _timerFinishingCallbacks = new Dictionary<int, TimerReportingCallback>();

            return true;
        }

        /// <summary>
        /// 句柄对象内置清理接口函数
        /// </summary>
        protected override void OnCleanup()
        {
            // 取消所有定时任务
            UnscheduleAll();

            // 销毁映射字典实例
            _timerClockingCallbacks.Clear();
            _timerClockingCallbacks = null;
            _timerFinishingCallbacks.Clear();
            _timerFinishingCallbacks = null;
        }

        /// <summary>
        /// 句柄对象内置刷新接口
        /// </summary>
        protected override void OnUpdate()
        {
        }

        /// <summary>
        /// 句柄对象内置延迟刷新接口
        /// </summary>
        protected override void OnLateUpdate()
        {
        }

        /// <summary>
        /// 句柄对象的模块事件转发回调接口
        /// </summary>
        /// <param name="e">模块事件参数</param>
        public override void OnEventDispatch(NovaEngine.ModuleEventArgs e)
        {
            NovaEngine.TimerEventArgs tea = e as NovaEngine.TimerEventArgs;
            Debugger.Assert(tea != null);

            switch (tea.Type)
            {
                case (int) NovaEngine.TimerModule.ProtocolType.Dispatched:
                {
                    // 统计定时事件派发
                    _Profiler.CallStat(Profiler.Statistics.StatCode.TimerDispatched, tea.Session);

                    TimerReportingCallback handler;
                    if (false == _timerClockingCallbacks.TryGetValue(tea.Session, out handler))
                    {
                        Debugger.Error("Could not found any valid timer clocking handler with session id {0}, dispatched event args failed..", tea.Session);
                    }
                    handler?.Invoke(tea.Session);
                }
                break;
                case (int) NovaEngine.TimerModule.ProtocolType.Finished:
                {
                    // 统计定时事件结束
                    _Profiler.CallStat(Profiler.Statistics.StatCode.TimerFinished, tea.Session);

                    // 先检查该会话是否需要进行结束回调通知
                    TimerReportingCallback handler;
                    if (_timerFinishingCallbacks.TryGetValue(tea.Session, out handler))
                    {
                        handler?.Invoke(tea.Session);
                        _timerFinishingCallbacks.Remove(tea.Session);
                    }

                    // Debugger.Log("Remove timer clocking handler with session id {0}.", tea.Session);

                    // 会话合法性检查
                    if (false == _timerClockingCallbacks.ContainsKey(tea.Session))
                    {
                        Debugger.Error("The timer clocked session id {0} was not found, remove it failed.", tea.Session);
                    }
                    _timerClockingCallbacks.Remove(tea.Session);
                }
                break;
            }
        }

        /// <summary>
        /// 定时任务调度启动接口，设置启动一个新的无限循环模式的任务定时器
        /// </summary>
        /// <param name="interval">任务延时间隔，以毫秒为单位</param>
        /// <param name="clocking">到时回调句柄</param>
        /// <returns>若任务启动成功，则返回对应的会话值，否则返回0</returns>
        public int Schedule(int interval, TimerReportingHandler clocking)
        {
            return Schedule(interval, NovaEngine.TimerModule.SCHEDULE_REPEAT_FOREVER, clocking);
        }

        /// <summary>
        /// 定时任务调度启动接口，设置启动一个新的无限循环模式的任务定时器
        /// </summary>
        /// <param name="interval">任务延时间隔，以毫秒为单位</param>
        /// <param name="clocking">到时回调句柄</param>
        /// <returns>若任务启动成功，则返回对应的会话值，否则返回0</returns>
        public int Schedule(int interval, TimerReportingForSessionHandler clocking)
        {
            return Schedule(interval, NovaEngine.TimerModule.SCHEDULE_REPEAT_FOREVER, clocking);
        }

        /// <summary>
        /// 定时任务调度启动接口，设置启动一个新的无限循环模式的任务定时器
        /// </summary>
        /// <param name="name">任务名称</param>
        /// <param name="interval">任务延时间隔，以毫秒为单位</param>
        /// <param name="clocking">到时回调句柄</param>
        /// <returns>若任务启动成功，则返回对应的会话值，否则返回0</returns>
        public int Schedule(string name, int interval, TimerReportingHandler clocking)
        {
            return Schedule(name, interval, NovaEngine.TimerModule.SCHEDULE_REPEAT_FOREVER, clocking);
        }

        /// <summary>
        /// 定时任务调度启动接口，设置启动一个新的无限循环模式的任务定时器
        /// </summary>
        /// <param name="name">任务名称</param>
        /// <param name="interval">任务延时间隔，以毫秒为单位</param>
        /// <param name="clocking">到时回调句柄</param>
        /// <returns>若任务启动成功，则返回对应的会话值，否则返回0</returns>
        public int Schedule(string name, int interval, TimerReportingForSessionHandler clocking)
        {
            return Schedule(name, interval, NovaEngine.TimerModule.SCHEDULE_REPEAT_FOREVER, clocking);
        }

        /// <summary>
        /// 定时任务调度启动接口，设置启动一个新的任务定时器
        /// 若需要设置一个无限循环的任务，可以将‘loop’设置为<see cref="NovaEngine.TimerModule.SCHEDULE_REPEAT_FOREVER"/>
        /// </summary>
        /// <param name="interval">任务延时间隔，以毫秒为单位</param>
        /// <param name="loop">任务循环次数</param>
        /// <param name="clocking">到时回调句柄</param>
        /// <returns>若任务启动成功，则返回对应的会话值，否则返回0</returns>
        public int Schedule(int interval, int loop, TimerReportingHandler clocking)
        {
            return Schedule(interval, loop, clocking, (TimerReportingHandler) null);
        }

        /// <summary>
        /// 定时任务调度启动接口，设置启动一个新的任务定时器
        /// 若需要设置一个无限循环的任务，可以将‘loop’设置为<see cref="NovaEngine.TimerModule.SCHEDULE_REPEAT_FOREVER"/>
        /// </summary>
        /// <param name="interval">任务延时间隔，以毫秒为单位</param>
        /// <param name="loop">任务循环次数</param>
        /// <param name="clocking">到时回调句柄</param>
        /// <returns>若任务启动成功，则返回对应的会话值，否则返回0</returns>
        public int Schedule(int interval, int loop, TimerReportingForSessionHandler clocking)
        {
            return Schedule(interval, loop, clocking, (TimerReportingForSessionHandler) null);
        }

        /// <summary>
        /// 定时任务调度启动接口，设置启动一个新的任务定时器
        /// 若需要设置一个无限循环的任务，可以将‘loop’设置为<see cref="NovaEngine.TimerModule.SCHEDULE_REPEAT_FOREVER"/>
        /// </summary>
        /// <param name="name">任务名称</param>
        /// <param name="interval">任务延时间隔，以毫秒为单位</param>
        /// <param name="loop">任务循环次数</param>
        /// <param name="clocking">到时回调句柄</param>
        /// <returns>若任务启动成功，则返回对应的会话值，否则返回0</returns>
        public int Schedule(string name, int interval, int loop, TimerReportingHandler clocking)
        {
            return Schedule(name, interval, loop, clocking, (TimerReportingHandler) null);
        }

        /// <summary>
        /// 定时任务调度启动接口，设置启动一个新的任务定时器
        /// 若需要设置一个无限循环的任务，可以将‘loop’设置为<see cref="NovaEngine.TimerModule.SCHEDULE_REPEAT_FOREVER"/>
        /// </summary>
        /// <param name="name">任务名称</param>
        /// <param name="interval">任务延时间隔，以毫秒为单位</param>
        /// <param name="loop">任务循环次数</param>
        /// <param name="clocking">到时回调句柄</param>
        /// <returns>若任务启动成功，则返回对应的会话值，否则返回0</returns>
        public int Schedule(string name, int interval, int loop, TimerReportingForSessionHandler clocking)
        {
            return Schedule(name, interval, loop, clocking, (TimerReportingForSessionHandler) null);
        }

        /// <summary>
        /// 定时任务调度启动接口，设置启动一个新的任务定时器
        /// 若需要设置一个无限循环的任务，可以将‘loop’设置为<see cref="NovaEngine.TimerModule.SCHEDULE_REPEAT_FOREVER"/>
        /// </summary>
        /// <param name="interval">任务延时间隔，以毫秒为单位</param>
        /// <param name="loop">任务循环次数</param>
        /// <param name="clocking">到时回调句柄</param>
        /// <param name="finishing">结束回调句柄</param>
        /// <returns>若任务启动成功，则返回对应的会话值，否则返回0</returns>
        public int Schedule(int interval, int loop, TimerReportingHandler clocking, TimerReportingHandler finishing)
        {
            TimerReportingCallback _clocking = null == clocking ? null : new TimerReportingCallback(this, clocking);
            TimerReportingCallback _finishing = null == finishing ? null : new TimerReportingCallback(this, finishing);

            return Schedule(interval, loop, _clocking, _finishing);
        }

        internal int Schedule(int interval, int loop, TimerReportingHandler clocking, TimerReportingForSessionHandler finishing)
        {
            TimerReportingCallback _clocking = null == clocking ? null : new TimerReportingCallback(this, clocking);
            TimerReportingCallback _finishing = null == finishing ? null : new TimerReportingCallback(this, finishing);

            return Schedule(interval, loop, _clocking, _finishing);
        }

        /// <summary>
        /// 定时任务调度启动接口，设置启动一个新的任务定时器
        /// 若需要设置一个无限循环的任务，可以将‘loop’设置为<see cref="NovaEngine.TimerModule.SCHEDULE_REPEAT_FOREVER"/>
        /// </summary>
        /// <param name="name">任务名称</param>
        /// <param name="interval">任务延时间隔，以毫秒为单位</param>
        /// <param name="loop">任务循环次数</param>
        /// <param name="clocking">到时回调句柄</param>
        /// <param name="finishing">结束回调句柄</param>
        /// <returns>若任务启动成功，则返回对应的会话值，否则返回0</returns>
        public int Schedule(string name, int interval, int loop, TimerReportingHandler clocking, TimerReportingHandler finishing)
        {
            TimerReportingCallback _clocking = null == clocking ? null : new TimerReportingCallback(this, clocking);
            TimerReportingCallback _finishing = null == finishing ? null : new TimerReportingCallback(this, finishing);

            return Schedule(name, interval, loop, _clocking, _finishing);
        }

        internal int Schedule(string name, int interval, int loop, TimerReportingHandler clocking, TimerReportingForSessionHandler finishing)
        {
            TimerReportingCallback _clocking = null == clocking ? null : new TimerReportingCallback(this, clocking);
            TimerReportingCallback _finishing = null == finishing ? null : new TimerReportingCallback(this, finishing);

            return Schedule(name, interval, loop, _clocking, _finishing);
        }

        /// <summary>
        /// 定时任务调度启动接口，设置启动一个新的任务定时器
        /// 若需要设置一个无限循环的任务，可以将‘loop’设置为<see cref="NovaEngine.TimerModule.SCHEDULE_REPEAT_FOREVER"/>
        /// </summary>
        /// <param name="interval">任务延时间隔，以毫秒为单位</param>
        /// <param name="loop">任务循环次数</param>
        /// <param name="clocking">到时回调句柄</param>
        /// <param name="finishing">结束回调句柄</param>
        /// <returns>若任务启动成功，则返回对应的会话值，否则返回0</returns>
        public int Schedule(int interval, int loop, TimerReportingForSessionHandler clocking, TimerReportingHandler finishing)
        {
            TimerReportingCallback _clocking = null == clocking ? null : new TimerReportingCallback(this, clocking);
            TimerReportingCallback _finishing = null == finishing ? null : new TimerReportingCallback(this, finishing);

            return Schedule(interval, loop, _clocking, _finishing);
        }

        public int Schedule(int interval, int loop, TimerReportingForSessionHandler clocking, TimerReportingForSessionHandler finishing)
        {
            TimerReportingCallback _clocking = null == clocking ? null : new TimerReportingCallback(this, clocking);
            TimerReportingCallback _finishing = null == finishing ? null : new TimerReportingCallback(this, finishing);

            return Schedule(interval, loop, _clocking, _finishing);
        }

        /// <summary>
        /// 定时任务调度启动接口，设置启动一个新的任务定时器
        /// 若需要设置一个无限循环的任务，可以将‘loop’设置为<see cref="NovaEngine.TimerModule.SCHEDULE_REPEAT_FOREVER"/>
        /// </summary>
        /// <param name="name">任务名称</param>
        /// <param name="interval">任务延时间隔，以毫秒为单位</param>
        /// <param name="loop">任务循环次数</param>
        /// <param name="clocking">到时回调句柄</param>
        /// <param name="finishing">结束回调句柄</param>
        /// <returns>若任务启动成功，则返回对应的会话值，否则返回0</returns>
        public int Schedule(string name, int interval, int loop, TimerReportingForSessionHandler clocking, TimerReportingHandler finishing)
        {
            TimerReportingCallback _clocking = null == clocking ? null : new TimerReportingCallback(this, clocking);
            TimerReportingCallback _finishing = null == finishing ? null : new TimerReportingCallback(this, finishing);

            return Schedule(name, interval, loop, _clocking, _finishing);
        }

        public int Schedule(string name, int interval, int loop, TimerReportingForSessionHandler clocking, TimerReportingForSessionHandler finishing)
        {
            TimerReportingCallback _clocking = null == clocking ? null : new TimerReportingCallback(this, clocking);
            TimerReportingCallback _finishing = null == finishing ? null : new TimerReportingCallback(this, finishing);

            return Schedule(name, interval, loop, _clocking, _finishing);
        }

        /// <summary>
        /// 定时任务调度启动接口，设置启动一个新的任务定时器
        /// 若需要设置一个无限循环的任务，可以将‘loop’设置为<see cref="NovaEngine.TimerModule.SCHEDULE_REPEAT_FOREVER"/>
        /// </summary>
        /// <param name="interval">任务延时间隔，以毫秒为单位</param>
        /// <param name="loop">任务循环次数</param>
        /// <param name="clocking">到时回调句柄</param>
        /// <param name="finishing">结束回调句柄</param>
        /// <returns>若任务启动成功，则返回对应的会话值，否则返回0</returns>
        private int Schedule(int interval, int loop, TimerReportingCallback clocking, TimerReportingCallback finishing)
        {
            return Schedule(null, interval, loop, clocking, finishing);
        }

        /// <summary>
        /// 定时任务调度启动接口，设置启动一个新的任务定时器
        /// 若需要设置一个无限循环的任务，可以将‘loop’设置为<see cref="NovaEngine.TimerModule.SCHEDULE_REPEAT_FOREVER"/>
        /// </summary>
        /// <param name="name">任务名称</param>
        /// <param name="interval">任务延时间隔，以毫秒为单位</param>
        /// <param name="loop">任务循环次数</param>
        /// <param name="clocking">到时回调句柄</param>
        /// <param name="finishing">结束回调句柄</param>
        /// <returns>若任务启动成功，则返回对应的会话值，否则返回0</returns>
        private int Schedule(string name, int interval, int loop, TimerReportingCallback clocking, TimerReportingCallback finishing)
        {
            (int sessionID, bool newly) = TimerModule.Schedule(name, interval, loop);

            if (sessionID <= 0)
            {
                Debugger.Error("The target timer '{0}' processing was occurred exception, scheduled it failed.", name);
                return sessionID;
            }

            if (_timerClockingCallbacks.ContainsKey(sessionID))
            {
                Debugger.Error(false == newly, "The timer clocked session id {0} was already exist, repeat add will be override old handler.", sessionID);
                _timerClockingCallbacks.Remove(sessionID);
            }

            // Debugger.Log("Add timer clocking handler with session id {0}.", sessionID);
            _timerClockingCallbacks.Add(sessionID, clocking);

            if (_timerFinishingCallbacks.ContainsKey(sessionID))
            {
                Debugger.Error(false == newly, "The timer finished session id {0} was already exist, repeat add will be override old handler.", sessionID);
                _timerFinishingCallbacks.Remove(sessionID);
            }

            if (finishing != null)
            {
                // Debugger.Log("Add timer finishing handler with session id {0}.", sessionID);
                _timerFinishingCallbacks.Add(sessionID, finishing);
            }

            if (newly)
            {
                // 统计定时事件发生
                _Profiler.CallStat(Profiler.Statistics.StatCode.TimerStartup, sessionID, name);
            }

            return sessionID;
        }

        /// <summary>
        /// 停止指定标识对应的定时任务
        /// </summary>
        /// <param name="sessionID">会话标识</param>
        public void Unschedule(int sessionID)
        {
            TimerModule.Unschedule(sessionID);
        }

        /// <summary>
        /// 停止指定名称对应的定时任务
        /// </summary>
        /// <param name="name">任务名称</param>
        public void Unschedule(string name)
        {
            TimerModule.Unschedule(name);
        }

        /// <summary>
        /// 停止当前设置的所有定时任务
        /// </summary>
        public void UnscheduleAll()
        {
            TimerModule.UnscheduleAll();
        }

        /// <summary>
        /// 通过指定的会话标识，直接移除对应的定时任务<br/>
        /// 该接口不推荐使用者主动调用，它将破坏定时任务的正常执行流程<br/>
        /// 如果需要中途关闭定时任务，推荐使用<see cref="GameEngine.TimerHandler.Unschedule(int)"/>接口
        /// </summary>
        /// <param name="session">会话标识</param>
        internal void RemoveTimerBySession(int session)
        {
            TimerModule.RemoveTimerInfoBySession(session);
        }

        #region 定时任务的回调通知类声明

        /// <summary>
        /// 定时任务的回调通知类
        /// </summary>
        private sealed class TimerReportingCallback
        {
            /// <summary>
            /// 定时任务管理句柄对象实例
            /// </summary>
            private readonly TimerHandler _handler;

            /// <summary>
            /// 空参数的回调句柄
            /// </summary>
            private readonly TimerReportingHandler _emptyCallback;
            /// <summary>
            /// 会话参数的回调句柄
            /// </summary>
            private readonly TimerReportingForSessionHandler _sessionCallback;

            public TimerReportingCallback(TimerHandler handler, TimerReportingHandler callback) : this(handler, callback, null)
            { }

            public TimerReportingCallback(TimerHandler handler, TimerReportingForSessionHandler callback) : this(handler, null, callback)
            { }

            public TimerReportingCallback(TimerHandler handler,
                                          TimerReportingHandler emptyCallback,
                                          TimerReportingForSessionHandler sessionCallback)
            {
                Debugger.Assert(null != handler, "Invalid arguments");
                Debugger.Assert(null != emptyCallback || null != sessionCallback, "Invalid arguments.");

                _handler = handler;
                _emptyCallback = emptyCallback;
                _sessionCallback = sessionCallback;
            }

            public TimerReportingHandler EmptyCallback => _emptyCallback;
            public TimerReportingForSessionHandler SessionCallback => _sessionCallback;

            /// <summary>
            /// 定时任务回调句柄的调用执行函数
            /// </summary>
            /// <param name="sessionID">会话标识</param>
            public void Invoke(int sessionID)
            {
                if (null != _emptyCallback)
                {
                    _emptyCallback();
                }
                else if (null != _sessionCallback)
                {
                    _sessionCallback(sessionID);
                }
                else
                {
                    Debugger.Warn("Could not found any schedule callback with target session '{0}', invoked it failed.", sessionID);
                }
            }
        }

        #endregion
    }
}
