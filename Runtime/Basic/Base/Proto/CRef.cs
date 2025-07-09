/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyright (C) 2023 - 2024, Guangzhou Shiyue Network Technology Co., Ltd.
/// Copyright (C) 2024 - 2025, Hurley, Independent Studio.
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

using SystemType = System.Type;

namespace GameEngine
{
    /// <summary>
    /// 引用对象抽象类，对场景中的引用对象上下文进行封装及调度管理
    /// </summary>
    public abstract partial class CRef : CBase
    {
        /// <summary>
        /// 对象内部订阅事件的标识管理容器
        /// </summary>
        private IList<int> _eventIds;
        /// <summary>
        /// 对象内部订阅事件的类型管理容器
        /// </summary>
        private IList<SystemType> _eventTypes;

        /// <summary>
        /// 对象内部订阅事件的监听回调的映射容器列表
        /// </summary>
        // private IDictionary<int, IDictionary<string, EventCallSyntaxInfo>> _eventCallInfosForId;
        /// <summary>
        /// 对象内部订阅事件的监听回调的映射容器列表
        /// </summary>
        // private IDictionary<SystemType, IDictionary<string, EventCallSyntaxInfo>> _eventCallInfosForType;

        /// <summary>
        /// 对象内部监听消息的协议码管理容器
        /// </summary>
        private IList<int> _messageTypes;

        /// <summary>
        /// 对象内部消息通知的监听回调的映射容器列表
        /// </summary>
        // private IDictionary<int, IDictionary<string, MessageCallSyntaxInfo>> _messageCallInfosForType;

        /// <summary>
        /// 对象内部定时任务的会话管理容器
        /// </summary>
        private IList<int> m_schedules;

        /// <summary>
        /// 引用对象初始化通知接口函数
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();

            // 事件标识容器初始化
            _eventIds = new List<int>();
            // 事件类型容器初始化
            _eventTypes = new List<SystemType>();
            // 事件监听回调映射容器初始化
            // _eventCallInfosForId = new Dictionary<int, IDictionary<string, EventCallSyntaxInfo>>();
            // _eventCallInfosForType = new Dictionary<SystemType, IDictionary<string, EventCallSyntaxInfo>>();

            // 消息协议码容器初始化
            _messageTypes = new List<int>();
            // 消息监听回调映射容器初始化
            // _messageCallInfosForType = new Dictionary<int, IDictionary<string, MessageCallSyntaxInfo>>();

            // 任务会话容器初始化
            m_schedules = new List<int>();
        }

        /// <summary>
        /// 引用对象清理通知接口函数
        /// </summary>
        public override void Cleanup()
        {
            // 移除所有定时任务
            RemoveAllSchedules();
            Debugger.Assert(m_schedules.Count == 0);
            m_schedules = null;

            base.Cleanup();

            // 移除所有订阅事件
            Debugger.Assert(_eventIds.Count == 0 && _eventTypes.Count == 0);
            _eventIds = null;
            _eventTypes = null;

            // 移除所有消息通知
            Debugger.Assert(_messageTypes.Count == 0);
            _messageTypes = null;
        }

        #region 引用对象事件订阅相关操作函数合集

        /// <summary>
        /// 发送事件消息到自己的事件管理器中进行派发
        /// </summary>
        /// <param name="eventID">事件标识</param>
        /// <param name="args">事件参数列表</param>
        public void SendToSelf(int eventID, params object[] args)
        {
            OnEventDispatchForId(eventID, args);
        }

        /// <summary>
        /// 发送事件消息到自己的事件管理器中进行派发
        /// </summary>
        /// <param name="arg">事件数据</param>
        public void SendToSelf<T>(T arg) where T : struct
        {
            OnEventDispatchForType(arg);
        }

        /// <summary>
        /// 用户自定义的事件处理函数，您可以通过重写该函数处理自定义事件行为
        /// </summary>
        /// <param name="eventID">事件标识</param>
        /// <param name="args">事件数据参数</param>
        protected override void OnEvent(int eventID, params object[] args) { }

        /// <summary>
        /// 用户自定义的事件处理函数，您可以通过重写该函数处理自定义事件行为
        /// </summary>
        /// <param name="eventData">事件数据</param>
        protected override void OnEvent(object eventData) { }

        /// <summary>
        /// 针对指定事件标识新增事件订阅的后处理程序
        /// </summary>
        /// <param name="eventID">事件标识</param>
        /// <returns>返回后处理的操作结果</returns>
        protected override bool OnSubscribeActionPostProcess(int eventID)
        {
            return Subscribe(eventID);
        }

        /// <summary>
        /// 针对指定事件类型新增事件订阅的后处理程序
        /// </summary>
        /// <param name="eventType">事件类型</param>
        /// <returns>返回后处理的操作结果</returns>
        protected override bool OnSubscribeActionPostProcess(SystemType eventType)
        {
            return Subscribe(eventType);
        }

        /// <summary>
        /// 针对指定事件标识移除事件订阅的后处理程序
        /// </summary>
        /// <param name="eventID">事件标识</param>
        protected override void OnUnsubscribeActionPostProcess(int eventID)
        { }

        /// <summary>
        /// 针对指定事件类型移除事件订阅的后处理程序
        /// </summary>
        /// <param name="eventType">事件类型</param>
        protected override void OnUnsubscribeActionPostProcess(SystemType eventType)
        { }

        /// <summary>
        /// 引用对象的事件订阅函数接口，对一个指定的事件进行订阅监听
        /// </summary>
        /// <param name="eventID">事件标识</param>
        /// <returns>若事件订阅成功则返回true，否则返回false</returns>
        public override sealed bool Subscribe(int eventID)
        {
            if (_eventIds.Contains(eventID))
            {
                // Debugger.Warn("The 'CRef' instance event '{0}' was already subscribed, repeat do it failed.", eventID);
                return true;
            }

            if (false == EventController.Instance.Subscribe(eventID, this))
            {
                Debugger.Warn("The 'CRef' instance subscribe event '{0}' failed.", eventID);
                return false;
            }

            _eventIds.Add(eventID);

            return true;
        }

        /// <summary>
        /// 引用对象的事件订阅函数接口，对一个指定的事件进行订阅监听
        /// </summary>
        /// <param name="eventType">事件类型</param>
        /// <returns>若事件订阅成功则返回true，否则返回false</returns>
        public override sealed bool Subscribe(SystemType eventType)
        {
            if (_eventTypes.Contains(eventType))
            {
                // Debugger.Warn("The 'CRef' instance's event '{0}' was already subscribed, repeat do it failed.", eventType.FullName);
                return true;
            }

            if (false == EventController.Instance.Subscribe(eventType, this))
            {
                Debugger.Warn("The 'CRef' instance subscribe event '{0}' failed.", eventType.FullName);
                return false;
            }

            _eventTypes.Add(eventType);

            return true;
        }

        /// <summary>
        /// 取消当前引用对象对指定事件的订阅
        /// </summary>
        /// <param name="eventID">事件标识</param>
        public override sealed void Unsubscribe(int eventID)
        {
            if (false == _eventIds.Contains(eventID))
            {
                // Debugger.Warn("Could not found any event '{0}' for target 'CRef' instance with on subscribed, do unsubscribe failed.", eventID);
                return;
            }

            EventController.Instance.Unsubscribe(eventID, this);
            _eventIds.Remove(eventID);

            base.Unsubscribe(eventID);
        }

        /// <summary>
        /// 取消当前引用对象对指定事件的订阅
        /// </summary>
        /// <param name="eventType">事件类型</param>
        public override sealed void Unsubscribe(SystemType eventType)
        {
            if (false == _eventTypes.Contains(eventType))
            {
                // Debugger.Warn("Could not found any event '{0}' for target 'CRef' instance with on subscribed, do unsubscribe failed.", eventType.FullName);
                return;
            }

            EventController.Instance.Unsubscribe(eventType, this);
            _eventTypes.Remove(eventType);

            base.Unsubscribe(eventType);
        }

        /// <summary>
        /// 取消当前引用对象的所有事件订阅
        /// </summary>
        public override sealed void UnsubscribeAllEvents()
        {
            base.UnsubscribeAllEvents();

            EventController.Instance.UnsubscribeAll(this);

            _eventIds.Clear();
            _eventTypes.Clear();
        }

        #endregion

        #region 引用对象消息通知相关操作函数合集

        /// <summary>
        /// 用户自定义的消息处理函数，您可以通过重写该函数处理自定义消息通知
        /// </summary>
        /// <param name="opcode">协议操作码</param>
        /// <param name="message">消息对象实例</param>
        protected override void OnMessage(int opcode, ProtoBuf.Extension.IMessage message) { }

        /// <summary>
        /// 针对指定消息标识新增消息监听的后处理程序
        /// </summary>
        /// <param name="opcode">协议操作码</param>
        /// <returns>返回后处理的操作结果</returns>
        protected override sealed bool OnMessageListenerAddedActionPostProcess(int opcode)
        {
            return AddMessageListener(opcode);
        }

        /// <summary>
        /// 针对指定消息标识移除消息监听的后处理程序
        /// </summary>
        /// <param name="opcode">协议操作码</param>
        protected override sealed void OnMessageListenerRemovedActionPostProcess(int opcode)
        { }

        /// <summary>
        /// 引用对象的消息监听函数接口，对一个指定的消息进行转发监听
        /// </summary>
        /// <param name="opcode">协议操作码</param>
        /// <returns>若消息监听成功则返回true，否则返回false</returns>
        public override sealed bool AddMessageListener(int opcode)
        {
            if (_messageTypes.Contains(opcode))
            {
                // Debugger.Warn("The 'CRef' instance's message type '{0}' was already exist, repeat added it failed.", opcode);
                return true;
            }

            if (false == NetworkHandler.Instance.AddMessageDispatchListener(opcode, this))
            {
                Debugger.Warn("The 'CRef' instance add message listener '{0}' failed.", opcode);
                return false;
            }

            _messageTypes.Add(opcode);

            return true;
        }

        /// <summary>
        /// 取消当前引用对象对指定协议码的监听回调
        /// </summary>
        /// <param name="opcode">协议操作码</param>
        public override sealed void RemoveMessageListener(int opcode)
        {
            if (false == _messageTypes.Contains(opcode))
            {
                // Debugger.Warn("Could not found any message type '{0}' for target 'CRef' instance with on listened, do removed it failed.", opcode);
                return;
            }

            NetworkHandler.Instance.RemoveMessageDispatchListener(opcode, this);
            _messageTypes.Remove(opcode);

            base.RemoveMessageListener(opcode);
        }

        /// <summary>
        /// 取消当前引用对象的所有注册的消息监听回调
        /// </summary>
        public override sealed void RemoveAllMessageListeners()
        {
            base.RemoveAllMessageListeners();

            NetworkHandler.Instance.RemoveAllMessageDispatchListeners(this);

            _messageTypes.Clear();
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
                if (false == m_schedules.Contains(v))
                {
                    Debugger.Warn("Could not found target session {0} scheduled with this object.", v);
                    return;
                }
                m_schedules.Remove(v);
            });

            if (sessionID > 0)
            {
                Debugger.Assert(false == m_schedules.Contains(sessionID), "Duplicate session ID.");
                m_schedules.Add(sessionID);
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
                if (false == m_schedules.Contains(v))
                {
                    Debugger.Warn("Could not found target session {0} scheduled with this object.", v);
                    return;
                }
                m_schedules.Remove(v);
            });

            if (sessionID > 0)
            {
                // Debugger.Assert(false == m_schedules.Contains(sessionID), "Duplicate session ID.");
                if (false == m_schedules.Contains(sessionID))
                {
                    m_schedules.Add(sessionID);
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
                if (false == m_schedules.Contains(v))
                {
                    Debugger.Warn("Could not found target session {0} scheduled with this object.", v);
                    return;
                }
                m_schedules.Remove(v);
            });

            if (sessionID > 0)
            {
                // Debugger.Assert(false == m_schedules.Contains(sessionID), "Duplicate session ID.");
                if (false == m_schedules.Contains(sessionID))
                {
                    m_schedules.Add(sessionID);
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
                if (false == m_schedules.Contains(v))
                {
                    Debugger.Warn("Could not found target session {0} scheduled with this object.", v);
                    return;
                }
                m_schedules.Remove(v);
            });

            if (sessionID > 0)
            {
                Debugger.Assert(false == m_schedules.Contains(sessionID), "Duplicate session ID.");
                m_schedules.Add(sessionID);
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
            for (int n = 0; n < m_schedules.Count; ++n)
            {
                Unschedule(m_schedules[n]);
            }
        }

        /// <summary>
        /// 移除当前引用对象中启动的所有定时任务
        /// </summary>
        private void RemoveAllSchedules()
        {
            // 拷贝一份会话列表
            List<int> list = new List<int>();
            list.AddRange(m_schedules);

            for (int n = 0; n < list.Count; ++n)
            {
                TimerHandler.Instance.RemoveTimerBySession(list[n]);
            }
        }

        #endregion
    }
}
