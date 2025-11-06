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
using SystemDelegate = System.Delegate;
using SystemMethodInfo = System.Reflection.MethodInfo;

namespace GameEngine
{
    /// <summary>
    /// 基于ECS模式的组件对象封装类，该类定义组件对象的常用接口及基础调度逻辑<br/>
    /// 组件的具体实现类可以通过继承<see cref="NovaEngine.IExecutable"/>来声明该类是否具备自执行功能<br/>
    /// 也可以通过继承<see cref="NovaEngine.IUpdatable"/>来声明该类是否具备自刷新功能<br/>
    /// 但除了需要继承指定接口外，还要求上层的实体对象必须打开执行或刷新调度才可以正常工作
    /// </summary>
    public abstract partial class CComponent : CBase, IComponent,
        NovaEngine.IExecutable, NovaEngine.IUpdatable, IBeanLifecycle, IEventDispatch, IMessageDispatch
    {
        /// <summary>
        /// 获取组件对象所属的实体对象实例
        /// </summary>
        public CEntity Entity { get; internal set; }

        /// <summary>
        /// 组件对象初始化函数接口
        /// </summary>
        public override sealed void Initialize()
        {
            base.Initialize();

            // 资源收集初始化
            OnAssetCollectInitialize();
        }

        /// <summary>
        /// 组件对象清理函数接口
        /// </summary>
        public override sealed void Cleanup()
        {
            // 资源收集清理
            OnAssetCollectCleanup();

            base.Cleanup();

            // 清空宿主
            Entity = null;
        }

        /// <summary>
        /// 组件对象启动函数接口
        /// </summary>
        public override sealed void Startup()
        { }

        /// <summary>
        /// 组件对象关闭函数接口
        /// </summary>
        public override sealed void Shutdown()
        { }

        /// <summary>
        /// 组件对象执行通知接口函数
        /// </summary>
        public void Execute()
        { }

        /// <summary>
        /// 组件对象后置执行通知接口函数
        /// </summary>
        public void LateExecute()
        { }

        /// <summary>
        /// 组件对象刷新通知接口函数
        /// </summary>
        public void Update()
        { }

        /// <summary>
        /// 组件对象后置刷新通知接口函数
        /// </summary>
        public void LateUpdate()
        { }

        /// <summary>
        /// 组件对象唤醒通知函数接口
        /// </summary>
        public void Awake()
        { }

        /// <summary>
        /// 组件对象开始通知函数接口
        /// </summary>
        public void Start()
        { }

        /// <summary>
        /// 组件对象销毁通知函数接口
        /// </summary>
        public void Destroy()
        { }

        #region 当前组件所属实体对象的事件快捷访问操作接口函数

        /// <summary>
        /// 发送事件消息到自身所属的实体对象中进行派发
        /// </summary>
        /// <param name="eventID">事件标识</param>
        /// <param name="args">事件参数列表</param>
        public void SendToSelf(int eventID, params object[] args)
        {
            this.Entity?.SendToSelf(eventID, args);
        }

        /// <summary>
        /// 发送事件消息到自身所属的实体对象中进行派发
        /// </summary>
        /// <param name="eventData">事件数据</param>
        public void SendToSelf<T>(T eventData) where T : struct
        {
            this.Entity?.SendToSelf<T>(eventData);
        }

        #endregion

        #region 当前组件所属实体对象的定时器快捷访问操作接口函数

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
            return this.Entity?.Schedule(interval, loop, handler) ?? NovaEngine.TimerModule.SCHEDULE_CALL_FAILED;
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
            return this.Entity?.Schedule(interval, loop, handler) ?? NovaEngine.TimerModule.SCHEDULE_CALL_FAILED;
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
            return this.Entity?.Schedule(name, interval, loop, handler) ?? NovaEngine.TimerModule.SCHEDULE_CALL_FAILED;
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
            return this.Entity?.Schedule(name, interval, loop, handler) ?? NovaEngine.TimerModule.SCHEDULE_CALL_FAILED;
        }

        /// <summary>
        /// 停止当前引用对象指定标识对应的定时任务
        /// </summary>
        /// <param name="sessionID">会话标识</param>
        public void Unschedule(int sessionID)
        {
            this.Entity?.Unschedule(sessionID);
        }

        /// <summary>
        /// 停止当前引用对象指定名称对应的定时任务
        /// </summary>
        /// <param name="name">任务名称</param>
        public void Unschedule(string name)
        {
            this.Entity?.Unschedule(name);
        }

        #endregion

        #region 当前组件所属实体对象的组件快捷访问操作接口函数

        /// <summary>
        /// 通过指定的组件类型，动态创建一个新的组件实例，并添加到当前组件所挂载的实体对象
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <returns>返回新添加的组件实例，失败则返回null</returns>
        public T AddComponent<T>() where T : CComponent
        {
            return Entity?.AddComponent<T>();
        }

        /// <summary>
        /// 通过组件类型在当前组件所挂载的实体对象中获取对应的组件对象实例
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <returns>若查找组件实例成功则返回对应实例的引用，否则返回null</returns>
        public T GetComponent<T>() where T : CComponent
        {
            return Entity?.GetComponent<T>();
        }

        /// <summary>
        /// 从当前组件所挂载的实体对象中移除指定类型的组件对象实例
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        public void RemoveComponent<T>() where T : CComponent
        {
            Entity?.RemoveComponent<T>();
        }

        /// <summary>
        /// 从当前组件所挂载的实体对象中移除指定类型的组件对象实例
        /// </summary>
        /// <param name="componentType">组件类型</param>
        public void RemoveComponent(SystemType componentType)
        {
            Entity?.RemoveComponent(componentType);
        }

        /// <summary>
        /// 从当前组件所挂载的实体对象中移除指定的组件对象实例
        /// </summary>
        /// <param name="component">组件对象实例</param>
        public void RemoveComponent(CComponent component)
        {
            Entity?.RemoveComponent(component);
        }

        #endregion

        #region 组件对象输入响应相关操作函数合集

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
            return Entity.AddInputResponseFromComponent(inputCode, operationType);
        }

        /// <summary>
        /// 针对指定输入类型新增输入响应的后处理程序
        /// </summary>
        /// <param name="inputType">输入类型</param>
        /// <returns>返回后处理的操作结果</returns>
        protected override bool OnInputResponseAddedActionPostProcess(SystemType inputType)
        {
            return Entity.AddInputResponseFromComponent(inputType);
        }

        /// <summary>
        /// 针对指定输入编码移除输入响应的后处理程序
        /// </summary>
        /// <param name="inputCode">输入编码</param>
        /// <param name="operationType">输入操作类型</param>
        protected override void OnInputResponseRemovedActionPostProcess(int inputCode, int operationType)
        {
            // 移除实体中对应的输入响应
            Entity?.RemoveInputResponseFromComponent(inputCode, operationType);
        }

        /// <summary>
        /// 针对指定输入类型移除输入响应的后处理程序
        /// </summary>
        /// <param name="inputType">输入类型类型</param>
        protected override void OnInputResponseRemovedActionPostProcess(SystemType inputType)
        {
            // 移除实体中对应的输入响应
            Entity?.RemoveInputResponseFromComponent(inputType);
        }

        #endregion

        #region 组件对象事件转发相关操作函数合集

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
            return Entity.SubscribeFromComponent(eventID);
        }

        /// <summary>
        /// 针对指定事件类型新增事件订阅的后处理程序
        /// </summary>
        /// <param name="eventType">事件类型</param>
        /// <returns>返回后处理的操作结果</returns>
        protected override bool OnSubscribeActionPostProcess(SystemType eventType)
        {
            return Entity.SubscribeFromComponent(eventType);
        }

        /// <summary>
        /// 针对指定事件标识移除事件订阅的后处理程序
        /// </summary>
        /// <param name="eventID">事件标识</param>
        protected override void OnUnsubscribeActionPostProcess(int eventID)
        {
            // 移除实体中对应的事件订阅
            Entity?.UnsubscribeFromComponent(eventID);
        }

        /// <summary>
        /// 针对指定事件类型移除事件订阅的后处理程序
        /// </summary>
        /// <param name="eventType">事件类型</param>
        protected override void OnUnsubscribeActionPostProcess(SystemType eventType)
        {
            // 移除实体中对应的事件订阅
            Entity?.UnsubscribeFromComponent(eventType);
        }

        #endregion

        #region 组件对象消息通知相关操作函数合集

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
            return Entity.AddMessageListenerFromComponent(opcode);
        }

        /// <summary>
        /// 针对指定消息标识移除消息监听的后处理程序
        /// </summary>
        /// <param name="opcode">协议操作码</param>
        protected override sealed void OnMessageListenerRemovedActionPostProcess(int opcode)
        {
            // 移除实体中对应的消息通知
            Entity?.RemoveMessageListenerFromComponent(opcode);
        }

        #endregion
    }
}
