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
    public abstract partial class CRef : CBase
    {
        /// <summary>
        /// 对象内部输入响应的编码管理容器
        /// </summary>
        private IList<int> _inputCodes;
        /// <summary>
        /// 对象内部输入响应的类型管理容器
        /// </summary>
        private IList<SystemType> _inputTypes;

        /// <summary>
        /// 基础对象内部输入编码的监听回调容器列表
        /// </summary>
        // private IDictionary<int, IDictionary<string, InputCallSyntaxInfo>> _inputCallInfosForCode;
        /// <summary>
        /// 基础对象内部输入编码的监听回调容器列表
        /// </summary>
        // private IDictionary<SystemType, IDictionary<string, InputCallSyntaxInfo>> _inputCallInfosForType;

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
        private IList<int> _schedules;

        /// <summary>
        /// 引用对象初始化通知接口函数
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();

            // 输入编码容器初始化
            _inputCodes = new List<int>();
            // 输入类型容器初始化
            _inputTypes = new List<SystemType>();
            // 输入监听回调映射容器初始化
            // _inputCallInfosForCode = new Dictionary<int, IDictionary<string, InputCallSyntaxInfo>>();
            // _inputCallInfosForType = new Dictionary<SystemType, IDictionary<string, InputCallSyntaxInfo>>();

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

            // 移除所有输入响应
            Debugger.Assert(_inputCodes.Count == 0 && _inputTypes.Count == 0);
            _inputCodes = null;
            _inputTypes = null;

            // 移除所有订阅事件
            Debugger.Assert(_eventIds.Count == 0 && _eventTypes.Count == 0);
            _eventIds = null;
            _eventTypes = null;

            // 移除所有消息通知
            Debugger.Assert(_messageTypes.Count == 0);
            _messageTypes = null;
        }

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
            if (HasAspectBehaviourType(AspectBehaviourType.Update) ||
                HasAspectBehaviourType(AspectBehaviourType.LateUpdate))
            {
                return true;
            }

            return false;
        }

        #endregion

        #region 引用对象输入响应相关操作函数合集

        /// <summary>
        /// 发送输入编码到自己的输入管理器中进行派发
        /// </summary>
        /// <param name="inputCode">输入编码</param>
        /// <param name="operationType">输入操作类型</param>
        public void SimulateKeycodeForSelf(int inputCode, int operationType)
        {
            OnInputDispatchForCode(inputCode, operationType);
        }

        /// <summary>
        /// 发送输入数据到自己的输入管理器中进行派发
        /// </summary>
        /// <param name="inputData">输入数据</param>
        public void SimulateKeycodeForSelf<T>(T inputData) where T : struct
        {
            OnEventDispatchForType(inputData);
        }

        /// <summary>
        /// 用户自定义的输入处理函数，您可以通过重写该函数处理自定义输入行为
        /// </summary>
        /// <param name="inputCode">输入编码</param>
        /// <param name="operationType">输入操作类型</param>
        protected override void OnInput(int inputCode, int operationType) { }

        /// <summary>
        /// 用户自定义的输入处理函数，您可以通过重写该函数处理自定义输入行为
        /// </summary>
        /// <param name="inputData">输入数据</param>
        protected override void OnInput(object inputData) { }

        /// <summary>
        /// 针对指定输入编码新增输入响应的后处理程序
        /// </summary>
        /// <param name="inputCode">输入编码</param>
        /// <param name="operationType">输入操作类型</param>
        /// <returns>返回后处理的操作结果</returns>
        protected override bool OnInputResponseAddedActionPostProcess(int inputCode, int operationType)
        {
            return AddInputResponse(inputCode, operationType);
        }

        /// <summary>
        /// 针对指定输入类型新增输入响应的后处理程序
        /// </summary>
        /// <param name="inputType">输入类型</param>
        /// <returns>返回后处理的操作结果</returns>
        protected override bool OnInputResponseAddedActionPostProcess(SystemType inputType)
        {
            return AddInputResponse(inputType);
        }

        /// <summary>
        /// 针对指定输入编码移除输入响应的后处理程序
        /// </summary>
        /// <param name="inputCode">输入编码</param>
        /// <param name="operationType">输入操作类型</param>
        protected override void OnInputResponseRemovedActionPostProcess(int inputCode, int operationType)
        { }

        /// <summary>
        /// 针对指定输入类型移除输入响应的后处理程序
        /// </summary>
        /// <param name="inputType">输入类型</param>
        protected override void OnInputResponseRemovedActionPostProcess(SystemType inputType)
        { }

        /// <summary>
        /// 引用对象的输入响应函数接口，对一个指定的输入编码进行响应监听
        /// </summary>
        /// <param name="inputCode">输入编码</param>
        /// <param name="operationType">输入操作类型</param>
        /// <returns>若输入响应成功则返回true，否则返回false</returns>
        protected internal override sealed bool AddInputResponse(int inputCode, int operationType)
        {
            if (_inputCodes.Contains(inputCode))
            {
                Debugger.Warn("The 'CRef' instance input '{0}' was already added, repeat do it failed.", inputCode);
                return true;
            }

            if (false == InputHandler.Instance.AddInputResponse(inputCode, this))
            {
                Debugger.Warn("The 'CRef' instance add input response '{0}' failed.", inputCode);
                return false;
            }

            _inputCodes.Add(inputCode);

            return true;
        }

        /// <summary>
        /// 引用对象的输入响应函数接口，对一个指定的输入类型进行响应监听
        /// </summary>
        /// <param name="inputType">输入类型</param>
        /// <returns>若输入响应成功则返回true，否则返回false</returns>
        protected internal override sealed bool AddInputResponse(SystemType inputType)
        {
            if (_inputTypes.Contains(inputType))
            {
                // Debugger.Warn("The 'CRef' instance's input '{0}' was already added, repeat do it failed.", inputType.FullName);
                return true;
            }

            if (false == InputHandler.Instance.AddInputResponse(inputType, this))
            {
                Debugger.Warn("The 'CRef' instance add input response '{0}' failed.", inputType.FullName);
                return false;
            }

            _inputTypes.Add(inputType);

            return true;
        }

        /// <summary>
        /// 取消当前引用对象对指定输入的响应
        /// </summary>
        /// <param name="inputCode">输入编码</param>
        /// <param name="operationType">输入操作类型</param>
        protected internal override sealed void RemoveInputResponse(int inputCode, int operationType)
        {
            if (false == _inputCodes.Contains(inputCode))
            {
                // Debugger.Warn("Could not found any input '{0}' for target 'CRef' instance with on added, do removed it failed.", inputCode);
                return;
            }

            InputHandler.Instance.RemoveInputResponse(inputCode, this);
            _inputCodes.Remove(inputCode);

            base.RemoveInputResponse(inputCode, operationType);
        }

        /// <summary>
        /// 取消当前引用对象对指定输入的响应
        /// </summary>
        /// <param name="inputType">输入类型</param>
        protected internal override sealed void RemoveInputResponse(SystemType inputType)
        {
            if (false == _inputTypes.Contains(inputType))
            {
                // Debugger.Warn("Could not found any input '{0}' for target 'CRef' instance with on added, do removed it failed.", inputType.FullName);
                return;
            }

            InputHandler.Instance.RemoveInputResponse(inputType, this);
            _inputTypes.Remove(inputType);

            base.RemoveInputResponse(inputType);
        }

        /// <summary>
        /// 取消当前引用对象的所有输入响应
        /// </summary>
        public override sealed void RemoveAllInputResponses()
        {
            base.RemoveAllInputResponses();

            InputHandler.Instance.RemoveInputResponseForTarget(this);

            _inputCodes.Clear();
            _inputTypes.Clear();
        }

        #endregion

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
        /// <param name="eventData">事件数据</param>
        public void SendToSelf<T>(T eventData) where T : struct
        {
            OnEventDispatchForType(eventData);
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
        protected internal override sealed bool AddMessageListener(int opcode)
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
