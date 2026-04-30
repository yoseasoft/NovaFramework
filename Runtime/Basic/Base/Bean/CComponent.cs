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

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace GameEngine
{
    /// <summary>
    /// 基于ECS模式的组件对象封装类，该类定义组件对象的常用接口及基础调度逻辑<br/>
    /// 组件的具体实现类可以通过继承<see cref="NovaEngine.IExecutable"/>来声明该类是否具备自执行功能<br/>
    /// 也可以通过继承<see cref="NovaEngine.IUpdatable"/>来声明该类是否具备自刷新功能<br/>
    /// 但除了需要继承指定接口外，还要求上层的实体对象必须打开执行或刷新调度才可以正常工作
    /// </summary>
    public abstract partial class CComponent : CBase, IComponent,
        NovaEngine.IExecutable, NovaEngine.IUpdatable, IBeanLifecycle
    {
        /// <summary>
        /// 获取组件对象的分类标签
        /// </summary>
        public override sealed CBeanClassificationLabel ClassificationLabel => CBeanClassificationLabel.Component;

        /// <summary>
        /// 获取组件对象的名称
        /// </summary>
        public string DeclareClassName => BeanController.Instance.GetComponentNameByType(BeanType);

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

        #region 当前组件行为状态检测相关的辅助接口函数

        /// <summary>
        /// 检测当前组件对象实例是否具有可执行的行为类型
        /// </summary>
        /// <returns>若该实例有可执行行为则返回true，否则返回false</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected internal bool HasExecutableBehaviourType()
        {
            return HasAspectBehaviourType(AspectBehaviourType.Execute) || HasAspectBehaviourType(AspectBehaviourType.LateExecute);
        }

        /// <summary>
        /// 检测当前组件对象实例是否具有可刷新的行为类型
        /// </summary>
        /// <returns>若该实例有可刷新行为则返回true，否则返回false</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected internal bool HasUpdatableBehaviourType()
        {
            return HasAspectBehaviourType(AspectBehaviourType.Update) || HasAspectBehaviourType(AspectBehaviourType.LateUpdate);
        }

        #endregion

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

        #region 当前组件所属实体对象的同步快捷访问操作接口函数

        /// <summary>
        /// 发送同步消息到自身所属的实体对象中进行派发
        /// </summary>
        /// <param name="tags">数据标签</param>
        /// <param name="announceType">数据播报类型</param>
        public void SendToSelf(string tags, ReplicateAnnounceType announceType)
        {
            this.Entity?.SendToSelf(tags, announceType);
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
            return Schedule(interval, NovaEngine.Module.TimerModule.SCHEDULE_REPEAT_FOREVER, handler);
        }

        /// <summary>
        /// 定时任务调度启动接口，设置启动一个新的无限循环模式的任务定时器
        /// </summary>
        /// <param name="interval">任务延时间隔，以毫秒为单位</param>
        /// <param name="handler">回调句柄</param>
        /// <returns>若任务启动成功，则返回对应的会话值，否则返回0</returns>
        public int Schedule(int interval, TimerHandler.TimerReportingForSessionHandler handler)
        {
            return Schedule(interval, NovaEngine.Module.TimerModule.SCHEDULE_REPEAT_FOREVER, handler);
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
            return Schedule(interval, NovaEngine.Module.TimerModule.SCHEDULE_REPEAT_FOREVER, handler);
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
            return Schedule(interval, NovaEngine.Module.TimerModule.SCHEDULE_REPEAT_FOREVER, handler);
        }

        /// <summary>
        /// 引用对象的定时任务调度启动接口，设置启动一个属于该对象的任务定时器<br/>
        /// 若需要设置一个无限循环的任务，可以将‘loop’设置为<see cref="NovaEngine.Module.TimerModule.SCHEDULE_REPEAT_FOREVER"/>
        /// </summary>
        /// <param name="interval">任务延时间隔，以毫秒为单位</param>
        /// <param name="loop">任务循环次数</param>
        /// <param name="handler">回调句柄</param>
        /// <returns>若任务启动成功，则返回对应的会话值，否则返回0</returns>
        public int Schedule(int interval, int loop, TimerHandler.TimerReportingHandler handler)
        {
            return this.Entity?.Schedule(interval, loop, handler) ?? NovaEngine.Module.TimerModule.SCHEDULE_CALL_FAILED;
        }

        /// <summary>
        /// 引用对象的定时任务调度启动接口，设置启动一个属于该对象的任务定时器<br/>
        /// 若需要设置一个无限循环的任务，可以将‘loop’设置为<see cref="NovaEngine.Module.TimerModule.SCHEDULE_REPEAT_FOREVER"/>
        /// </summary>
        /// <param name="interval">任务延时间隔，以毫秒为单位</param>
        /// <param name="loop">任务循环次数</param>
        /// <param name="handler">回调句柄</param>
        /// <returns>若任务启动成功，则返回对应的会话值，否则返回0</returns>
        public int Schedule(int interval, int loop, TimerHandler.TimerReportingForSessionHandler handler)
        {
            return this.Entity?.Schedule(interval, loop, handler) ?? NovaEngine.Module.TimerModule.SCHEDULE_CALL_FAILED;
        }

        /// <summary>
        /// 引用对象的定时任务调度启动接口，设置启动一个属于该对象的任务定时器<br/>
        /// 若需要设置一个无限循环的任务，可以将‘loop’设置为<see cref="NovaEngine.Module.TimerModule.SCHEDULE_REPEAT_FOREVER"/>
        /// </summary>
        /// <param name="name">任务名称</param>
        /// <param name="interval">任务延时间隔，以毫秒为单位</param>
        /// <param name="loop">任务循环次数</param>
        /// <param name="handler">回调句柄</param>
        /// <returns>若任务启动成功，则返回对应的会话值，否则返回0</returns>
        public int Schedule(string name, int interval, int loop, TimerHandler.TimerReportingHandler handler)
        {
            return this.Entity?.Schedule(name, interval, loop, handler) ?? NovaEngine.Module.TimerModule.SCHEDULE_CALL_FAILED;
        }

        /// <summary>
        /// 引用对象的定时任务调度启动接口，设置启动一个属于该对象的任务定时器<br/>
        /// 若需要设置一个无限循环的任务，可以将‘loop’设置为<see cref="NovaEngine.Module.TimerModule.SCHEDULE_REPEAT_FOREVER"/>
        /// </summary>
        /// <param name="name">任务名称</param>
        /// <param name="interval">任务延时间隔，以毫秒为单位</param>
        /// <param name="loop">任务循环次数</param>
        /// <param name="handler">回调句柄</param>
        /// <returns>若任务启动成功，则返回对应的会话值，否则返回0</returns>
        public int Schedule(string name, int interval, int loop, TimerHandler.TimerReportingForSessionHandler handler)
        {
            return this.Entity?.Schedule(name, interval, loop, handler) ?? NovaEngine.Module.TimerModule.SCHEDULE_CALL_FAILED;
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
        public void RemoveComponent(Type componentType)
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
    }
}
