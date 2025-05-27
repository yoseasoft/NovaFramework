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

using SystemDateTime = System.DateTime;
using SystemTimeSpan = System.TimeSpan;

namespace NovaEngine
{
    /// <summary>
    /// 定时管理模块，通过一个全局唯一队列对所有的定时任务进行统一处理
    /// </summary>
    public sealed partial class TimerModule : ModuleObject
    {
        /// <summary>
        /// 计划任务永久重复执行标识参数
        /// </summary>
        public const int SCHEDULE_REPEAT_FOREVER = -1;

        /// <summary>
        /// 当前活动任务实例的管理容器
        /// </summary>
        private IList<TimerInfo> m_activeTaskQueue = null;

        /// <summary>
        /// 定时器模块事件类型
        /// </summary>
        public override int EventType => (int) EEventType.Timer;

        /// <summary>
        /// 管理器对象初始化接口函数
        /// </summary>
        protected override void OnInitialize()
        {
            m_activeTaskQueue = new List<TimerInfo>();
        }

        /// <summary>
        /// 管理器对象清理接口函数
        /// </summary>
        protected override void OnCleanup()
        {
            m_activeTaskQueue.Clear();
            m_activeTaskQueue = null;
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
        /// 定时管理器内部事务更新接口
        /// </summary>
        protected override void OnUpdate()
        {
            // 检查定时器超时及过期属性
            if (m_activeTaskQueue.Count > 0)
            {
                for (int n = 0; n < m_activeTaskQueue.Count; ++n)
                {
                    TimerInfo info = m_activeTaskQueue[n];
                    info.Update();
                }

                this.CleanupExpiredTimer();
            }
        }

        /// <summary>
        /// 定时管理器内部后置更新接口
        /// </summary>
        protected override void OnLateUpdate()
        {
        }

        /// <summary>
        /// 检测当前任务队列中是否存在同名的定时器实例
        /// </summary>
        /// <param name="name">定时器名称</param>
        /// <returns>若存在同名定时器则返回true，否则返回false</returns>
        public bool IsTimerInfoExistByName(string name)
        {
            // 名称为空则认定为不存在重复定时器
            if (string.IsNullOrEmpty(name))
            {
                return false;
            }

            for (int n = 0; n < m_activeTaskQueue.Count; ++n)
            {
                TimerInfo info = m_activeTaskQueue[n];
                if (null != info.Name && name.Equals(info.Name))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 尝试从当前任务队列中获取指定名称的定时器实例
        /// </summary>
        /// <param name="name">任务名称</param>
        /// <param name="info">定时器对象</param>
        /// <returns>若获取指定名称的定时器成功则返回true，否则返回false</returns>
        private bool TryGetTimerInfoByName(string name, out TimerInfo info)
        {
            info = null;

            // 名称为空则认定为不存在重复定时器
            if (string.IsNullOrEmpty(name))
            {
                return false;
            }

            for (int n = 0; n < m_activeTaskQueue.Count; ++n)
            {
                TimerInfo _info = m_activeTaskQueue[n];
                if (null != _info.Name && name.Equals(_info.Name))
                {
                    info = _info;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 定时任务调度启动接口，开始挂载一个新的任务定时器
        /// </summary>
        /// <param name="interval">任务延时间隔，以毫秒为单位</param>
        /// <param name="loop">任务循环次数</param>
        /// <returns>若任务启动成功，则返回对应的会话值，否则返回0</returns>
        public (int, bool) Schedule(int interval, int loop)
        {
            return Schedule(null, interval, loop);
        }

        /// <summary>
        /// 定时任务调度启动接口，开始挂载一个新的指定名称的任务定时器
        /// </summary>
        /// <param name="name">任务名称</param>
        /// <param name="interval">任务延时间隔，以毫秒为单位</param>
        /// <param name="loop">任务循环次数</param>
        /// <returns>若任务启动成功，则返回对应的会话值，否则返回0</returns>
        public (int, bool) Schedule(string name, int interval, int loop)
        {
            TimerInfo info = null;
            if (TryGetTimerInfoByName(name, out info))
            {
                if (false == info.Expired)
                {
                    Logger.Warn("The target timer was running with same name '{0}', repeat scheduled it failed.", name);
                    return (0, false);
                }

                info.Interval = interval;
                info.Counting = loop;
                info.Looper = (SCHEDULE_REPEAT_FOREVER == loop ? true : false);
                info.ReloadTime();
                return (info.Session, false);
            }

            int session = Session.NextSessionID((int) EEventType.Timer);

            info = new TimerInfo(this);
            info.Session = session;
            info.Name = name;
            info.Interval = interval;
            info.Counting = loop;
            info.Looper = (SCHEDULE_REPEAT_FOREVER == loop ? true : false);
            info.Initialize();
            m_activeTaskQueue.Add(info);

            return (session, true);
        }

        /// <summary>
        /// 停止指定会话标识的定时任务
        /// </summary>
        /// <param name="session">会话标识</param>
        public void Unschedule(int session)
        {
            for (int n = 0; n < m_activeTaskQueue.Count; ++n)
            {
                TimerInfo info = m_activeTaskQueue[n];
                if (info.Session == session)
                {
                    info.Expired = true;
                    return;
                }
            }
        }

        /// <summary>
        /// 停止指定名称标识的定时任务
        /// </summary>
        /// <param name="name">任务名称</param>
        public void Unschedule(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                Logger.Warn("The unschedule task name must be non-null or empty space.");
                return;
            }

            for (int n = 0; n < m_activeTaskQueue.Count; ++n)
            {
                TimerInfo info = m_activeTaskQueue[n];
                if (null != info.Name && name.Equals(info.Name))
                {
                    info.Expired = true;
                    return;
                }
            }
        }

        /// <summary>
        /// 停止当前所有活动的定时任务
        /// </summary>
        public void UnscheduleAll()
        {
            for (int n = 0; n < m_activeTaskQueue.Count; ++n)
            {
                TimerInfo info = m_activeTaskQueue[n];
                info.Expired = true;
            }
        }

        /// <summary>
        /// 清理所有过期的定时任务对象
        /// </summary>
        private void CleanupExpiredTimer()
        {
            for (int n = m_activeTaskQueue.Count - 1; n >= 0; --n)
            {
                TimerInfo info = m_activeTaskQueue[n];
                if (info.Expired)
                {
                    info.Cleanup();

                    m_activeTaskQueue.RemoveAt(n);
                }
            }
        }

        /// <summary>
        /// 通过指定的会话标识，直接移除对应的定时任务<br/>
        /// 该接口不推荐使用者主动调用，它将破坏定时任务的正常执行流程<br/>
        /// 如果需要中途关闭定时任务，推荐使用<see cref="NovaEngine.TimerModule.Unschedule(int)"/>接口
        /// </summary>
        /// <param name="session">会话标识</param>
        public void RemoveTimerInfoBySession(int session)
        {
            for (int n = m_activeTaskQueue.Count - 1; n >= 0; --n)
            {
                TimerInfo info = m_activeTaskQueue[n];
                if (info.Session == session)
                {
                    info.Cleanup();

                    m_activeTaskQueue.RemoveAt(n);
                }
            }
        }

        /// <summary>
        /// 通过会话标识获取其对应的定时任务的名称
        /// </summary>
        /// <param name="session">会话标识</param>
        /// <returns>返回给定标识对应的任务名称，若不存在指定任务则返回null</returns>
        public string GetTimerNameBySession(int session)
        {
            for (int n = m_activeTaskQueue.Count - 1; n >= 0; --n)
            {
                TimerInfo info = m_activeTaskQueue[n];
                if (info.Session == session)
                {
                    return info.Name;
                }
            }

            return null;
        }

        /// <summary>
        /// 定时任务到期通知回调接口
        /// </summary>
        /// <param name="session">定时任务会话标识</param>
        // public virtual void OnSchedule(int type, int session)
        // {
        //     // this.SendMessageCommand(Constant.LUA_FUNCTION_SCHEDULE, session);
        //     TimerEventArgs e = this.AcquireEvent<TimerEventArgs>();
        //     e.Type = type;
        //     e.Session = session;
        //     this.SendEvent(e);
        // }

        private void SendEvent(int type, int session)
        {
            TimerEventArgs e = this.AcquireEvent<TimerEventArgs>();
            e.Type = type;
            e.Session = session;
            // this.SendEvent(e);
            // 此处需要立即发送事件，否则当业务对象清理时，注册的定时任务接收到结束通知时，对象已经被移除
            this.SendEvent(e, true);
        }

        #region 基础属性快捷访问操作接口

        /// <summary>
        /// 获取一个标准时钟滴答数
        /// </summary>
        public long Ticks
        {
            get
            {
                SystemTimeSpan ts = new SystemTimeSpan(SystemDateTime.UtcNow.Ticks - new SystemDateTime(1970, 1, 1, 0, 0, 0).Ticks);
                return (long) ts.TotalMilliseconds;
            }
        }

        #endregion

        #region 定时任务的内部管理数据对象类声明

        /// <summary>
        /// 定时任务对象基础模型，用于管理定时任务对象相关属性，如会话信息，计划任务等，及回调接口
        /// </summary>
        internal sealed class TimerInfo
        {
            /// <summary>
            /// 定时任务对应的会话标识
            /// </summary>
            private int m_session = 0;

            /// <summary>
            /// 定时任务的名称，可以重复定义
            /// </summary>
            private string m_name = null;

            /// <summary>
            /// 任务当次执行启动时间
            /// </summary>
            private int m_timeStartup = 0;

            /// <summary>
            /// 任务当前执行结束时间
            /// </summary>
            private int m_timeFinished = 0;

            /// <summary>
            /// 任务调度的间隔时间，以毫秒为单位
            /// </summary>
            private int m_interval = 0;

            /// <summary>
            /// 任务累计回调次数计数
            /// </summary>
            private int m_counting = 0;

            /// <summary>
            /// 任务计时未缩放状态标识
            /// </summary>
            private bool m_unscaled = false;

            /// <summary>
            /// 任务循环执行处理标识
            /// </summary>
            private bool m_looper = false;

            /// <summary>
            /// 任务运行过期处理标识
            /// </summary>
            private bool m_expired = false;

            /// <summary>
            /// 任务重新开始运行处理标识
            /// </summary>
            private bool m_restarted = false;

            /// <summary>
            /// 定时任务管理器实例
            /// </summary>
            private TimerModule m_timerModule = null;

            public TimerInfo(TimerModule module)
            {
                Logger.Assert(null != module, "构建定时任务对象必须依附于一个模块载体实例，该实例不能为空！");
                m_timerModule = module;
            }

            /// <summary>
            /// 定时任务初始化接口，针对计时相关属性进行处理
            /// </summary>
            public void Initialize()
            {
                ResetTime();

                m_timerModule.SendEvent((int) ProtocolType.Startup, m_session);
            }

            /// <summary>
            /// 定时任务清理接口
            /// </summary>
            public void Cleanup()
            {
                m_timerModule.SendEvent((int) ProtocolType.Finished, m_session);
            }

            /// <summary>
            /// 重置定时任务当前的时间记录
            /// </summary>
            private void ResetTime()
            {
                // m_timeStartup = Facade.Timestamp.RealtimeSinceStartup;
                m_timeStartup = Facade.Timestamp.TimeOfMilliseconds;
                m_timeFinished = m_timeStartup + m_interval;
            }

            /// <summary>
            /// 外部重新加载定时器的接口函数
            /// </summary>
            public void ReloadTime()
            {
                // 重置定时器
                ResetTime();

                // 重置过期状态标识，针对同一个定时器在同一帧重新调度的情况
                m_expired = false;

                // 每次重置定时器，都开启该标识
                m_restarted = true;
            }

            /// <summary>
            /// 定时任务自刷新调度接口
            /// </summary>
            public void Update()
            {
                // 定时任务可能会被其它服务主动关闭，在此提前判定，若已被关闭则直接退出
                if (m_expired)
                {
                    return;
                }

                // 重置标识
                m_restarted = false;

                // if (Facade.Timestamp.RealtimeSinceStartup >= m_timeFinished)
                if (Facade.Timestamp.TimeOfMilliseconds >= m_timeFinished)
                {
                    // 当前调度接口由于是在主刷新接口中被执行，因此不用加入事件队列，直接发送消息指令即可
                    // m_timerModule.OnSchedule(m_session);
                    m_timerModule.SendEvent((int) ProtocolType.Dispatched, m_session);

                    if (m_restarted)
                    {
                        // 若在此次转发通知中重新启动了定时器，则简单的进行标识重置
                        // 因为定时相关参数已经被重新赋值了
                        m_restarted = false;
                    }
                    else if (m_looper)
                    {
                        // 循环定时器直接重置计时参数即可
                        this.ResetTime();
                    }
                    else // no loop
                    {
                        --m_counting;
                        if (m_counting > 0)
                        {
                            this.ResetTime();
                        }
                        else
                        {
                            m_expired = true;
                        }
                    }
                }
            }

            #region 定时数据对象基础属性快捷访问操作接口

            public int Session { get { return m_session; } set { m_session = value; } }

            public string Name { get { return m_name; } set { m_name = value; } }

            public int Interval { get { return m_interval; } set { m_interval = value; } }

            public int Counting { get { return m_counting; } set { m_counting = value; } }

            public bool Unscaled { get { return m_unscaled; } set { m_unscaled = value; } }

            public bool Looper { get { return m_looper; } set { m_looper = value; } }

            public bool Expired { get { return m_expired; } set { m_expired = value; } }

            #endregion
        }

        #endregion
    }
}
