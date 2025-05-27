/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
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

using SystemType = System.Type;
using SystemRandom = System.Random;

namespace GameEngine
{
    /// <summary>
    /// 基于ECS模式的实体对象封装类，该类定义实体对象的常用接口及基础调度逻辑<br/>
    /// 这里定义的实体类，默认将开启内部刷新模式，可能将造成一定的性能消耗<br/>
    /// 如果您对实体不存在调度刷新的需求（包括内部的组件等），可以手动关闭它
    /// </summary>
    public abstract class CEntity : CRef, IEntity, NovaEngine.IUpdatable, IProtoLifecycle
    {
        /// <summary>
        /// 实体对象过期状态标识
        /// </summary>
        private bool m_isOnExpired = false;
        /// <summary>
        /// 实体对象等待销毁状态标识
        /// </summary>
        private bool m_isOnWaitingDestroy = false;

        /// <summary>
        /// 实体对象的组件列表容器
        /// </summary>
        protected IDictionary<string, CComponent> m_components = null;

        /// <summary>
        /// 实体对象内部组件实例的刷新统计列表
        /// </summary>
        protected IList<CComponent> m_componentUpdateList = null;

        /// <summary>
        /// 实体对象内部组件实例的事件分发列表
        /// </summary>
        protected IList<CComponent> m_componentEventDispatchList = null;

        /// <summary>
        /// 实体对象内部组件实例的消息分发列表
        /// </summary>
        protected IList<CComponent> m_componentMessageDispatchList = null;

        /// <summary>
        /// 实体对象内部随机因子管理容器
        /// </summary>
        protected IDictionary<string, SystemRandom> m_randoms = null;

        /// <summary>
        /// 获取实体对象当前过期状态
        /// </summary>
        public bool IsOnExpired => (m_isOnExpired || m_isOnWaitingDestroy);

        /// <summary>
        /// 获取实体对象的名称
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// 实体对象初始化通知接口函数
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();

            // 组件列表初始化
            m_components = new Dictionary<string, CComponent>();
            // 组件刷新列表初始化
            m_componentUpdateList = new List<CComponent>();
            // 组件事件分发列表初始化
            m_componentEventDispatchList = new List<CComponent>();
            // 组件消息分发列表初始化
            m_componentMessageDispatchList = new List<CComponent>();

            // 随机因子管理容器初始化
            m_randoms = new Dictionary<string, SystemRandom>();
        }

        /// <summary>
        /// 实体对象清理通知接口函数
        /// </summary>
        public override void Cleanup()
        {
            // 重置过期状态标识
            m_isOnExpired = false;
            // 重置销毁待命状态标识
            m_isOnWaitingDestroy = false;

            // 移除全部随机因子对象实例
            m_randoms.Clear();
            m_randoms = null;

            // 移除全部组件对象实例
            RemoveAllComponents();
            m_components = null;
            m_componentUpdateList = null;
            m_componentEventDispatchList = null;
            m_componentMessageDispatchList = null;

            base.Cleanup();
        }

        /// <summary>
        /// 实体对象启动通知函数接口
        /// </summary>
        public override void Startup()
        { }

        /// <summary>
        /// 实体对象关闭通知函数接口
        /// </summary>
        public override void Shutdown()
        { }

        /// <summary>
        /// 实体对象刷新通知接口函数
        /// </summary>
        public virtual void Update()
        {
            // 组件实例刷新
            UpdateAllComponents();
        }

        /// <summary>
        /// 实体对象后置刷新通知接口函数
        /// </summary>
        public virtual void LateUpdate()
        {
            // 组件实例后置刷新
            LateUpdateAllComponents();
        }

        /// <summary>
        /// 实体对象唤醒通知函数接口
        /// </summary>
        public virtual void Awake()
        {
            // 唤醒当前加载的全部组件
            foreach (KeyValuePair<string, CComponent> pair in m_components)
            {
                CComponent component = pair.Value;

                // 组件尚未唤醒
                // Debugger.Assert(false == component.IsOnAwakingStatus() && false == component.IsOnDestroyingStatus(), "Invalid entity lifecycle.");
                // 2024-05-20:
                // 在唤醒阶段添加的组件对象，其实例通过"AddComponent"函数添加时，就会自动唤醒
                // 因此在大部分的情况下，所有添加成功的组件，均已处于唤醒状态了，无需在此处重复调用
                if (false == component.IsOnAwakingStatus() && false == component.IsOnDestroyingStatus())
                {
                    // 唤醒组件实例
                    Call(component.Awake, LifecycleKeypointType.Awake);
                }
            }
        }

        /// <summary>
        /// 实体对象开始通知函数接口
        /// </summary>
        public virtual void Start()
        {
            // 开始当前加载的全部组件
            IList<string> keys = NovaEngine.Utility.Collection.ToListForKeys<string, CComponent>(m_components);
            for (int n = 0; n < keys.Count; ++n)
            {
                if (false == m_components.TryGetValue(keys[n], out CComponent component))
                {
                    Debugger.Warn("Could not found any component instance with target name '{0}', started it failed.", keys[n]);
                    continue;
                }

                // 组件尚未开始
                Debugger.Assert(false == component.IsOnStartingStatus() && false == component.IsOnDestroyingStatus(), "Invalid entity lifecycle.");
                // 开始组件实例
                OnComponentStartProcessing(component);
            }
        }

        /// <summary>
        /// 实体对象销毁通知函数接口
        /// </summary>
        public virtual void Destroy()
        {
            m_isOnExpired = false;
            m_isOnWaitingDestroy = false;

            // 停止当前加载的全部组件
            IList<string> keys = NovaEngine.Utility.Collection.ToListForKeys<string, CComponent>(m_components);
            for (int n = keys.Count - 1; n >= 0; --n)
            {
                if (false == m_components.TryGetValue(keys[n], out CComponent component))
                {
                    Debugger.Warn("Could not found any component instance with target name '{0}', destroyed it failed.", keys[n]);
                    continue;
                }

                // 组件唤醒后尚未销毁
                if (component.IsOnAwakingStatus() && false == component.IsOnDestroyingStatus())
                {
                    // 销毁组件实例
                    Call(component.Destroy, LifecycleKeypointType.Destroy);
                }
            }
        }

        /// <summary>
        /// 标记当前实体对象此刻为过期状态<br/>
        /// 过期状态一旦设定便不可更改，只能等待系统删除回收此实体对象实例<br/>
        /// 该表示和待删除状态的区别在于，此状态标记后，并不会删除对象实例，只是对其中部分调度逻辑进行忽略<br/>
        /// 但实例仍然是有效的，可以正常访问其内部的成员属性，只有标记待销毁状态标识后，才会真正移除该实例
        /// </summary>
        public void OnExpired()
        {
            m_isOnExpired = true;

            // 2024-07-04:
            // 过期状态不再移除对象实例
            // ProtoController.Instance.RegProtoLifecycleNotification(AspectBehaviourType.Destroy, this);
        }

        /// <summary>
        /// 标记当前实体对象此刻为待销毁状态<br/>
        /// 待销毁状态一旦设定便不可更改，只能等待系统删除回收此实体对象实例
        /// </summary>
        public void OnPrepareToDestroy()
        {
            // 标记待销毁状态的同时，需要将其设置为过期状态
            OnExpired();

            m_isOnWaitingDestroy = true;
            ProtoController.Instance.RegProtoLifecycleNotification(AspectBehaviourType.Destroy, this);
        }

        #region 实体对象事件订阅相关操作函数合集

        /// <summary>
        /// 实体对象的订阅事件的监听回调函数<br/>
        /// 该函数针对事件转发接口的标准实现，禁止子类重写该函数<br/>
        /// 若子类需要根据需要自行解析事件，可以通过重写<see cref="GameEngine.CEntity.OnEvent(int, object[])"/>实现事件的自定义处理逻辑
        /// </summary>
        /// <param name="eventID">事件标识</param>
        /// <param name="args">事件数据参数</param>
        public override sealed void OnEventDispatchForId(int eventID, params object[] args)
        {
            base.OnEventDispatchForId(eventID, args);

            for (int n = 0; null != m_componentEventDispatchList && n < m_componentEventDispatchList.Count; ++n)
            {
                CComponent component = m_componentEventDispatchList[n];
                if (component.IsOnAwakingStatus())
                {
                    component.OnEventDispatchForId(eventID, args);
                }
            }
        }

        /// <summary>
        /// 实体对象的订阅事件的监听回调函数<br/>
        /// 该函数针对事件转发接口的标准实现，禁止子类重写该函数<br/>
        /// 若子类需要根据需要自行解析事件，可以通过重写<see cref="GameEngine.CEntity.OnEvent(object)"/>实现事件的自定义处理逻辑
        /// </summary>
        /// <param name="eventData">事件数据</param>
        public override sealed void OnEventDispatchForType(object eventData)
        {
            base.OnEventDispatchForType(eventData);

            for (int n = 0; null != m_componentEventDispatchList && n < m_componentEventDispatchList.Count; ++n)
            {
                CComponent component = m_componentEventDispatchList[n];
                if (component.IsOnAwakingStatus())
                {
                    component.OnEventDispatchForType(eventData);
                }
            }
        }

        /// <summary>
        /// 检测当前实体对象是否订阅了目标事件标识
        /// </summary>
        /// <param name="eventID">事件标识</param>
        /// <returns>若订阅了给定事件标识则返回true，否则返回false</returns>
        protected internal override sealed bool IsSubscribedOfTargetId(int eventID)
        {
            if (base.IsSubscribedOfTargetId(eventID))
            {
                return true;
            }

            for (int n = 0; n < m_componentEventDispatchList.Count; ++n)
            {
                CComponent component = m_componentEventDispatchList[n] as CComponent;
                if (component.IsSubscribedOfTargetId(eventID))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 检测当前实体对象是否订阅了目标事件类型
        /// </summary>
        /// <param name="eventType">事件类型</param>
        /// <returns>若订阅了给定事件类型则返回true，否则返回false</returns>
        protected internal override sealed bool IsSubscribedOfTargetType(SystemType eventType)
        {
            if (base.IsSubscribedOfTargetType(eventType))
            {
                return true;
            }

            for (int n = 0; n < m_componentEventDispatchList.Count; ++n)
            {
                CComponent component = m_componentEventDispatchList[n] as CComponent;
                if (component.IsSubscribedOfTargetType(eventType))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 当前实体对象的组件实例进行的事件订阅行为通知
        /// </summary>
        /// <param name="eventID">事件标识</param>
        /// <returns>若事件订阅成功则返回true，否则返回false</returns>
        protected internal bool SubscribeFromComponent(int eventID)
        {
            return Subscribe(eventID);
        }

        /// <summary>
        /// 当前实体对象的组件实例进行的事件订阅行为通知
        /// </summary>
        /// <typeparam name="T">事件类型</typeparam>
        /// <returns>若事件订阅成功则返回true，否则返回false</returns>
        protected internal bool SubscribeFromComponent<T>() where T : struct
        {
            return SubscribeFromComponent(typeof(T));
        }

        /// <summary>
        /// 当前实体对象的组件实例进行的事件订阅行为通知
        /// </summary>
        /// <param name="eventType">事件类型</param>
        /// <returns>若事件订阅成功则返回true，否则返回false</returns>
        protected internal bool SubscribeFromComponent(SystemType eventType)
        {
            return Subscribe(eventType);
        }

        /// <summary>
        /// 当前实体对象的组件实例进行的取消事件订阅行为通知
        /// </summary>
        /// <param name="eventID">事件标识</param>
        protected internal void UnsubscribeFromComponent(int eventID)
        {
            if (IsSubscribedOfTargetId(eventID))
            {
                return;
            }

            // 实体对象没有任何订阅回调，移除该事件的订阅
            Unsubscribe(eventID);
        }

        /// <summary>
        /// 当前实体对象的组件实例进行的取消事件订阅行为通知
        /// </summary>
        /// <typeparam name="T">事件类型</typeparam>
        protected internal void UnsubscribeFromComponent<T>() where T : struct
        {
            UnsubscribeFromComponent(typeof(T));
        }

        /// <summary>
        /// 当前实体对象的组件实例进行的取消事件订阅行为通知
        /// </summary>
        /// <param name="eventType">事件类型</param>
        protected internal void UnsubscribeFromComponent(SystemType eventType)
        {
            if (IsSubscribedOfTargetType(eventType))
            {
                return;
            }

            // 实体对象没有任何订阅回调，移除该事件的订阅
            Unsubscribe(eventType);
        }

        #endregion

        #region 实体对象消息通知相关操作函数合集

        /// <summary>
        /// 组件对象的消息通知的监听回调函数<br/>
        /// 该函数针对消息转发接口的标准实现，禁止子类重写该函数<br/>
        /// 若子类需要根据需要自行处理消息，可以通过重写<see cref="GameEngine.CRef.OnMessage(ProtoBuf.Extension.IMessage)"/>实现消息的自定义处理逻辑
        /// </summary>
        /// <param name="opcode">协议操作码</param>
        /// <param name="message">消息对象实例</param>
        public override sealed void OnMessageDispatch(int opcode, ProtoBuf.Extension.IMessage message)
        {
            base.OnMessageDispatch(opcode, message);

            for (int n = 0; null != m_componentEventDispatchList && n < m_componentMessageDispatchList.Count; ++n)
            {
                CComponent component = m_componentMessageDispatchList[n];
                if (component.IsOnAwakingStatus())
                {
                    component.OnMessageDispatch(opcode, message);
                }
            }
        }

        /// <summary>
        /// 检测当前实体对象是否监听了目标消息类型
        /// </summary>
        /// <param name="opcode">协议操作码</param>
        /// <returns>若监听了给定消息类型则返回true，否则返回false</returns>
        protected internal override sealed bool IsMessageListenedOfTargetType(int opcode)
        {
            if (base.IsMessageListenedOfTargetType(opcode))
            {
                return true;
            }

            for (int n = 0; n < m_componentMessageDispatchList.Count; ++n)
            {
                CComponent component = m_componentMessageDispatchList[n] as CComponent;
                if (component.IsMessageListenedOfTargetType(opcode))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 实体对象组件实例的消息监听函数接口，对内部组件一个指定的消息进行转发监听
        /// </summary>
        /// <param name="opcode">协议操作码</param>
        /// <returns>若消息监听成功则返回true，否则返回false</returns>
        protected internal bool AddMessageListenerFromComponent(int opcode)
        {
            return AddMessageListener(opcode);
        }

        /// <summary>
        /// 实体对象组件实例的消息监听函数接口，将内部组件一个指定的消息类型绑定到给定的监听回调函数上
        /// </summary>
        /// <typeparam name="T">消息类型</typeparam>
        /// <returns>若消息监听成功则返回true，否则返回false</returns>
        protected internal bool AddMessageListenerFromComponent<T>() where T : ProtoBuf.Extension.IMessage
        {
            return AddMessageListenerFromComponent(typeof(T));
        }

        /// <summary>
        /// 实体对象组件实例的消息监听函数接口，将内部组件一个指定的消息类型绑定到给定的监听回调函数上
        /// </summary>
        /// <param name="messageType">消息类型</param>
        /// <returns>若消息监听成功则返回true，否则返回false</returns>
        protected internal bool AddMessageListenerFromComponent(SystemType messageType)
        {
            int opcode = NetworkHandler.Instance.GetOpcodeByMessageType(messageType);

            return AddMessageListenerFromComponent(opcode);
        }

        /// <summary>
        /// 取消当前实体对象组件实例对指定协议码的监听回调
        /// </summary>
        /// <param name="opcode">协议操作码</param>
        protected internal void RemoveMessageListenerFromComponent(int opcode)
        {
            if (IsMessageListenedOfTargetType(opcode))
            {
                return;
            }

            RemoveMessageListener(opcode);
        }

        /// <summary>
        /// 取消当前实体对象组件实例对指定消息类型的监听回调
        /// </summary>
        /// <typeparam name="T">消息类型</typeparam>
        protected internal void RemoveMessageListenerFromComponent<T>()
        {
            RemoveMessageListenerFromComponent(typeof(T));
        }

        /// <summary>
        /// 取消当前实体对象组件实例对指定消息类型的监听回调
        /// </summary>
        /// <param name="messageType">消息类型</param>
        protected internal void RemoveMessageListenerFromComponent(SystemType messageType)
        {
            int opcode = NetworkHandler.Instance.GetOpcodeByMessageType(messageType);

            RemoveMessageListenerFromComponent(opcode);
        }

        #endregion

        #region 实体对象组件相关操作函数合集

        /// <summary>
        /// 检测当前实体对象中是否已经添加了指定名称的组件实例
        /// </summary>
        /// <param name="name">组件名称</param>
        /// <returns>若当前实体已经添加给定名称的组件则返回true，否则返回false</returns>
        public bool HasComponent(string name)
        {
            return m_components.ContainsKey(name);
        }

        /// <summary>
        /// 检测当前实体对象中是否已经添加了指定类型的组件实例
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <returns>若当前实体已经添加给定类型的组件则返回true，否则返回false</returns>
        public bool HasComponent<T>() where T : CComponent
        {
            SystemType componentType = typeof(T);

            return HasComponent(componentType);
        }

        /// <summary>
        /// 检测当前实体对象中是否已经添加了指定类型的组件实例
        /// </summary>
        /// <param name="componentType">组件类型</param>
        /// <returns>若当前实体已经添加给定类型的组件则返回true，否则返回false</returns>
        public bool HasComponent(SystemType componentType)
        {
            string componentName = GetComponentNameByType(componentType);

            return HasComponent(componentName);
        }

        /// <summary>
        /// 检测当前实体对象中是否已经添加了指定组件实例<br/>
        /// 这里需要注意的是，我们进行组件检测时，使用的是组件名称进行检查，而非组件实例对象<br/>
        /// 所以可能存在相同名称但非同一实例的组件对象存在于列表中，从而返回true值
        /// </summary>
        /// <param name="component">组件对象实例</param>
        /// <returns>若当前实体已经添加给定组件实例则返回true，否则返回false</returns>
        public bool HasComponent(CComponent component)
        {
            IEnumerator<KeyValuePair<string, CComponent>> e = m_components.GetEnumerator();
            while (e.MoveNext())
            {
                if (e.Current.Value == component)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 通过指定的组件名称，动态创建一个新的组件实例，并添加到当前实体对象中<br/>
        /// 添加的组件名称具备唯一性，不能对相同名称的组件进行多次重复添加操作
        /// </summary>
        /// <param name="name">组件名称</param>
        /// <returns>返回新添加的组件实例，失败则返回null</returns>
        public CComponent AddComponent(string name)
        {
            SystemType componentType = ProtoController.Instance.FindComponentTypeByName(name);
            if (null != componentType)
            {
                return AddComponent(componentType);
            }

            return null;
        }

        /// <summary>
        /// 通过指定的组件类型，动态创建一个新的组件实例，并添加到当前实体对象中<br/>
        /// 添加的组件类型具备唯一性，不能对相同类型的组件进行多次重复添加操作
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <returns>返回新添加的组件实例，失败则返回null</returns>
        public T AddComponent<T>() where T : CComponent
        {
            SystemType componentType = typeof(T);
            return AddComponent(componentType) as T;
        }

        /// <summary>
        /// 通过指定的组件类型，动态创建一个新的组件实例，并添加到当前实体对象中<br/>
        /// 添加的组件类型具备唯一性，不能对相同类型的组件进行多次重复添加操作
        /// </summary>
        /// <param name="componentType">组件类型</param>
        /// <returns>返回新添加的组件实例，失败则返回null</returns>
        public CComponent AddComponent(SystemType componentType)
        {
            CComponent component = EcsmHandler.CreateInstance(componentType) as CComponent;
            return AddComponent(component);
        }

        /// <summary>
        /// 添加指定的组件对象实例到当前实体对象中<br/>
        /// 添加的组件具备名称唯一性，因此不能对相同名称的组件进行重复添加操作<br/>
        /// 因为这样会同时导致其它问题，我们在每次进行添加操作时，会对组件实例进行初始化操作<br/>
        /// 重复添加会让同一实例多次初始化，产生其它未知的错误
        /// </summary>
        /// <param name="component">组件对象实例</param>
        /// <returns>返回新添加的组件实例，失败则返回null</returns>
        public CComponent AddComponent(CComponent component)
        {
            if (null == component)
            {
                Debugger.Error("The new component instance must be non-null, added it failed.");
                return null;
            }

            if (IsOnUpdatingStatus())
            {
                Debugger.Error("The entity instance was updating now, cannot added any component at once.");
                return null;
            }

            if (IsOnDestroyingStatus())
            {
                Debugger.Error("The entity instance was destroying now, cannot added any component at once.");
                return null;
            }

            if (IsOnExpired)
            {
                Debugger.Warn("The entity instance was expired, added component failed.");
                return null;
            }

            // 在考虑这里是否需要对实例对象进行检查
            // 只有在添加一个组件实例后，用户自己手动修改该实例的组件名称，并且再次进行添加操作才可能出现重复的情况
            // 是否有检查的必要呢？
            if (HasComponent(component))
            {
                Debugger.Warn("The component instance '{0}' was already registed, repeat added it failed.", component.GetType().FullName);
                return null;
            }

            string componentName = GetComponentName(component);
            if (m_components.ContainsKey(componentName))
            {
                Debugger.Warn("The component name '{0}' was already registed, repeat added it failed.", componentName);
                return null;
            }

            // 增加实体对象的引用
            component.Entity = this;

            // 初始化组件实例
            Call(component.Initialize, LifecycleKeypointType.Initialize);

            m_components.Add(componentName, component);

            // 如果组件激活了事件分发接口，则添加到事件分发队列中
            if (typeof(IEventActivation).IsAssignableFrom(component.GetType()))
            {
                m_componentEventDispatchList.Add(component);
            }

            // 如果组件激活了消息分发接口，则添加到消息分发队列中
            if (typeof(IMessageActivation).IsAssignableFrom(component.GetType()))
            {
                m_componentMessageDispatchList.Add(component);
            }

            // 启动组件实例
            Call(component.Startup, LifecycleKeypointType.Startup);

            // 实体对象已经唤醒
            if (IsOnAwakingStatus())
            {
                // 唤醒组件实例
                Call(component.Awake, LifecycleKeypointType.Awake);
            }

            // 实体对象已经开始
            if (IsOnStartingStatus())
            {
                // 添加的唤醒通知队列
                ProtoController.Instance.RegProtoLifecycleNotification(AspectBehaviourType.Start, component);
            }

            return component;
        }

        /// <summary>
        /// 通过组件名称在当前实体对象中获取对应的组件对象实例
        /// </summary>
        /// <param name="name">组件名称</param>
        /// <returns>若查找组件实例成功则返回对应实例的引用，否则返回null</returns>
        public CComponent GetComponent(string name)
        {
            CComponent component;
            if (false == m_components.TryGetValue(name, out component))
            {
                return null;
            }

            return component;
        }

        /// <summary>
        /// 通过组件类型在当前实体对象中获取对应的组件对象实例
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <returns>若查找组件实例成功则返回对应实例的引用，否则返回null</returns>
        public T GetComponent<T>() where T : CComponent
        {
            SystemType componentType = typeof(T);

            return GetComponent(componentType) as T;
        }

        /// <summary>
        /// 通过组件类型在当前实体对象中获取对应的组件对象实例
        /// </summary>
        /// <param name="componentType">组件类型</param>
        /// <returns>若查找组件实例成功则返回对应实例的引用，否则返回null</returns>
        public CComponent GetComponent(SystemType componentType)
        {
            string componentName = GetComponentNameByType(componentType);

            return GetComponent(componentName);
        }

        /// <summary>
        /// 获取当前实体对象中的所有组件实例
        /// </summary>
        /// <returns>返回实体对象注册的所有组件实例</returns>
        public IList<CComponent> GetAllComponents()
        {
            return NovaEngine.Utility.Collection.ToListForValues<string, CComponent>(m_components);
        }

        /// <summary>
        /// 通过指定的组件对象实例获取其对应的组件名称
        /// </summary>
        /// <param name="component">组件对象实例</param>
        /// <returns>返回给定组件实例对应的名称</returns>
        private string GetComponentName(CComponent component)
        {
            SystemType componentType = component.GetType();

            return GetComponentNameByType(componentType);
        }

        /// <summary>
        /// 通过指定的组件对象类型获取其对应的组件名称
        /// </summary>
        /// <param name="componentType">组件类型</param>
        /// <returns>返回给定组件类型对应的名称</returns>
        private string GetComponentNameByType(SystemType componentType)
        {
            return ProtoController.Instance.GetComponentNameByType(componentType);
        }

        /// <summary>
        /// 从当前实体对象中移除指定名称的组件对象实例<br/>
        /// 我们在进行移除时，会同时对该组件实例进行清理操作
        /// </summary>
        /// <param name="name">组件名称</param>
        public void RemoveComponent(string name)
        {
            CComponent component;
            if (false == m_components.TryGetValue(name, out component))
            {
                Debugger.Warn("Could not found any component instance with name '{0}', removed it failed.", name);
                return;
            }

            if (IsOnUpdatingStatus())
            {
                // Debugger.Warn("The entity instance was updating now, cannot removed any component at once.");
                ProtoController.Instance.RegProtoLifecycleNotification(AspectBehaviourType.Destroy, component);
                return;
            }

            ProtoController.Instance.UnregProtoLifecycleNotification(component);

            // 仅在组件对象被唤醒成功后才处理该流程
            if (component.IsOnAwakingStatus())
            {
                // 销毁组件实例
                Call(component.Destroy, LifecycleKeypointType.Destroy);
            }

            m_componentUpdateList.Remove(component);

            // 关闭组件实例
            Call(component.Shutdown, LifecycleKeypointType.Shutdown);

            m_componentEventDispatchList.Remove(component);
            m_componentMessageDispatchList.Remove(component);
            m_components.Remove(name);

            // 清理组件实例
            Call(component.Cleanup, LifecycleKeypointType.Cleanup);

            // 回收组件实例
            EcsmHandler.ReleaseInstance(component);
        }

        /// <summary>
        /// 从当前实体对象中移除指定类型的组件对象实例<br/>
        /// 我们在进行移除时，会同时对该组件实例进行清理操作
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        public void RemoveComponent<T>() where T : CComponent
        {
            SystemType componentType = typeof(T);

            RemoveComponent(componentType);
        }

        /// <summary>
        /// 从当前实体对象中移除指定类型的组件对象实例<br/>
        /// 我们在进行移除时，会同时对该组件实例进行清理操作
        /// </summary>
        /// <param name="componentType">组件类型</param>
        public void RemoveComponent(SystemType componentType)
        {
            string componentName = GetComponentNameByType(componentType);

            RemoveComponent(componentName);
        }

        /// <summary>
        /// 从当前实体对象中移除指定的组件对象实例<br/>
        /// 我们在进行移除时，会同时对该组件实例进行清理操作<br/>
        /// 这里需要注意的是，我们进行组件检测时，使用的是组件名称进行检查，而非组件实例对象<br/>
        /// 所以可能存在相同名称但非同一实例的组件对象存在于列表中，从而被移除掉
        /// </summary>
        /// <param name="component">组件对象实例</param>
        public void RemoveComponent(CComponent component)
        {
            string componentName = GetComponentName(component);

            RemoveComponent(componentName);
        }

        /// <summary>
        /// 移除当前实体对象中记录的所有组件对象实例
        /// </summary>
        public void RemoveAllComponents()
        {
            List<string> keys = new List<string>();
            keys.AddRange(m_components.Keys);

            for (int n = keys.Count - 1; n >= 0; --n)
            {
                RemoveComponent(keys[n]);
            }
        }

        /// <summary>
        /// 当前实体对象的指定组件实例开启处理的回调函数
        /// </summary>
        /// <param name="component">组件对象实例</param>
        protected internal void OnComponentStartProcessing(CComponent component)
        {
            if (false == m_components.Values.Contains(component))
            {
                Debugger.Error("Could not found any added record of the component instance '{0}', calling start process failed.", component.GetType().FullName);
                return;
            }

            // 开始运行实例
            Call(component.Start, LifecycleKeypointType.Start);

            // 如果组件实现了刷新接口，则添加到刷新队列中
            if (typeof(IUpdateActivation).IsAssignableFrom(component.GetType()))
            {
                m_componentUpdateList.Add(component);
            }
        }

        /// <summary>
        /// 刷新当前实体对象中记录的所有组件对象实例<br/>
        /// 您可以关闭实体对象的帧刷新标识，然后在子类中根据需要手动调用组件的刷新接口
        /// </summary>
        protected void UpdateAllComponents()
        {
            for (int n = 0; false == IsOnExpired && n < m_componentUpdateList.Count; ++n)
            {
                Call(m_componentUpdateList[n].Update);
            }
        }

        /// <summary>
        /// 后置刷新当前实体对象中记录的所有组件对象实例<br/>
        /// 您可以关闭实体对象的帧刷新标识，然后在子类中根据需要手动调用组件的刷新接口
        /// </summary>
        protected void LateUpdateAllComponents()
        {
            for (int n = 0; false == IsOnExpired && n < m_componentUpdateList.Count; ++n)
            {
                Call(m_componentUpdateList[n].LateUpdate);
            }
        }

        #endregion

        #region 实体对象随机模块操作接口函数

        /// <summary>
        /// 对实体对象的默认随机实例进行初始化
        /// </summary>
        /// <param name="seed">随机种子</param>
        /// <returns>若随机因子初始化成功返回true，否则返回false</returns>
        public bool InitRandomSeed(int seed)
        {
            return InitRandomSeed(NovaEngine.Utility.Text.GetFullName(this.GetType()), seed);
        }

        /// <summary>
        /// 对实体对象指定名称的随机实例进行初始化
        /// </summary>
        /// <param name="name">实例名称</param>
        /// <param name="seed">随机种子</param>
        /// <returns>若随机因子初始化成功返回true，否则返回false</returns>
        public bool InitRandomSeed(string name, int seed)
        {
            if (string.IsNullOrEmpty(name))
            {
                return false;
            }

            if (m_randoms.ContainsKey(name))
            {
                Debugger.Warn("The random '{%s}' was already exist in entity instance '{%t}', repeated init it will be created new instance.", name, this);
                m_randoms.Remove(name);
            }

            SystemRandom random = new SystemRandom(seed);
            m_randoms.Add(name, random);

            return true;
        }

        /// <summary>
        /// 获取实体对象的默认随机实例的随机值
        /// </summary>
        /// <returns>返回随机值</returns>
        public int RandomNext()
        {
            return RandomNext(NovaEngine.Utility.Text.GetFullName(this.GetType()));
        }

        /// <summary>
        /// 获取实体对象指定名称的随机实例的随机值
        /// </summary>
        /// <param name="name">实例名称</param>
        /// <returns>返回随机值</returns>
        public int RandomNext(string name)
        {
            if (false == m_randoms.TryGetValue(name, out SystemRandom random))
            {
                Debugger.Warn("Could not found target random '{%s}' from entity instance '{%t}', getted next value failed.", name, this);
                return 0;
            }

            return random.Next();
        }

        /// <summary>
        /// 获取实体对象的默认随机实例的随机值，此函数将指定返回随机值的上限
        /// </summary>
        /// <param name="maxValue">最大值</param>
        /// <returns>返回随机值</returns>
        public int RandomNext(int maxValue)
        {
            return RandomNext(NovaEngine.Utility.Text.GetFullName(this.GetType()), maxValue);
        }

        /// <summary>
        /// 获取实体对象指定名称的随机实例的随机值，此函数将指定返回随机值的上限
        /// </summary>
        /// <param name="name">实例名称</param>
        /// <param name="maxValue">最大值</param>
        /// <returns>返回随机值</returns>
        public int RandomNext(string name, int maxValue)
        {
            if (false == m_randoms.TryGetValue(name, out SystemRandom random))
            {
                Debugger.Warn("Could not found target random '{%s}' from entity instance '{%t}', getted next value failed.", name, this);
                return 0;
            }

            return random.Next(maxValue);
        }

        /// <summary>
        /// 获取实体对象的默认随机实例的随机值，此函数将指定返回随机值的范围
        /// </summary>
        /// <param name="minValue">最小值</param>
        /// <param name="maxValue">最大值</param>
        /// <returns>返回随机值</returns>
        public int RandomNext(int minValue, int maxValue)
        {
            return RandomNext(NovaEngine.Utility.Text.GetFullName(this.GetType()), minValue, maxValue);
        }

        /// <summary>
        /// 获取实体对象指定名称的随机实例的随机值，此函数将指定返回随机值的范围
        /// </summary>
        /// <param name="name">实例名称</param>
        /// <param name="minValue">最小值</param>
        /// <param name="maxValue">最大值</param>
        /// <returns>返回随机值</returns>
        public int RandomNext(string name, int minValue, int maxValue)
        {
            if (false == m_randoms.TryGetValue(name, out SystemRandom random))
            {
                Debugger.Warn("Could not found target random '{%s}' from entity instance '{%t}', getted next value failed.", name, this);
                return 0;
            }

            return random.Next(minValue, maxValue);
        }

        #endregion
    }
}
