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

namespace GameEngine
{
    /// <summary>
    /// 事件管理对象类，用于对场景上下文中的所有节点对象进行事件管理及分发
    /// </summary>
    public sealed partial class EventController : BaseController<EventController>
    {
        /// <summary>
        /// 针对事件标识进行分发的监听对象管理列表容器
        /// </summary>
        private IDictionary<int, IList<IEventDispatch>> m_eventListenersForId = null;

        /// <summary>
        /// 针对事件类型进行分发的监听对象管理列表容器
        /// </summary>
        private IDictionary<SystemType, IList<IEventDispatch>> m_eventListenersForType = null;

        /// <summary>
        /// 事件分发数据的缓冲管理队列
        /// </summary>
        private Queue<EventData> m_eventBuffers = null;

        /// <summary>
        /// 事件分发对象初始化通知接口函数
        /// </summary>
        protected override void OnInitialize()
        {
            // 初始化监听列表
            m_eventListenersForId = new Dictionary<int, IList<IEventDispatch>>();
            m_eventListenersForType = new Dictionary<SystemType, IList<IEventDispatch>>();

            // 初始化事件数据缓冲队列
            m_eventBuffers = new Queue<EventData>();
        }

        /// <summary>
        /// 事件分发对象清理通知接口函数
        /// </summary>
        protected override void OnCleanup()
        {
            // 清理事件数据缓冲队列
            m_eventBuffers.Clear();
            m_eventBuffers = null;

            // 清理监听列表
            m_eventListenersForId.Clear();
            m_eventListenersForId = null;
            m_eventListenersForType.Clear();
            m_eventListenersForType = null;
        }

        /// <summary>
        /// 事件管理对象刷新通知接口函数
        /// </summary>
        protected override sealed void OnUpdate()
        {
            if (m_eventBuffers.Count > 0)
            {
                Queue<EventData> queue = new Queue<EventData>(m_eventBuffers);
                m_eventBuffers.Clear();

                while (queue.Count > 0)
                {
                    EventData eventData = queue.Dequeue();
                    OnEventDispatched(eventData);
                }
            }
        }

        /// <summary>
        /// 事件管理对象后置刷新通知接口函数
        /// </summary>
        protected override sealed void OnLateUpdate()
        {
        }

        /// <summary>
        /// 事件管理对象倾泻调度函数接口
        /// </summary>
        protected override void OnDump()
        {
        }

        /// <summary>
        /// 发送事件消息到当前管理器中等待派发
        /// </summary>
        /// <param name="eventID">事件标识</param>
        /// <param name="args">事件参数列表</param>
        public void Send(int eventID, params object[] args)
        {
            EventData eventData = new EventData(eventID, args);
            m_eventBuffers.Enqueue(eventData);
        }

        /// <summary>
        /// 发送事件消息到当前管理器中等待派发
        /// </summary>
        /// <param name="arg">事件数据</param>
        public void Send<T>(T arg) where T : struct
        {
            EventData eventData = new EventData(arg);
            m_eventBuffers.Enqueue(eventData);
        }

        /// <summary>
        /// 发送事件消息到当前管理器中并立即处理掉它
        /// </summary>
        /// <param name="eventID">事件标识</param>
        /// <param name="args">事件参数列表</param>
        public void Fire(int eventID, params object[] args)
        {
            OnEventDispatched(eventID, args);
        }

        /// <summary>
        /// 发送事件消息到当前管理器中并立即处理掉它
        /// </summary>
        /// <param name="arg">事件数据</param>
        public void Fire<T>(T arg) where T : struct
        {
            OnEventDispatched(arg);
        }

        /// <summary>
        /// 事件消息派发调度接口函数
        /// </summary>
        /// <param name="eventData">事件数据对象</param>
        private void OnEventDispatched(EventData eventData)
        {
            if (eventData.EventID > 0)
            {
                OnEventDispatched(eventData.EventID, eventData.Params);
            }
            else
            {
                Debugger.Assert(null != eventData.Params && eventData.Params.Length == 1, "Invalid event data arguments.");

                OnEventDispatched(eventData.Params[0]);
            }
        }

        /// <summary>
        /// 事件标识消息派发调度接口函数
        /// </summary>
        /// <param name="eventID">事件标识</param>
        /// <param name="args">事件参数列表</param>
        private void OnEventDispatched(int eventID, params object[] args)
        {
            // 事件分发调度
            OnEventDistributeCallDispatched(eventID, args);

            IList<IEventDispatch> listeners;
            if (m_eventListenersForId.TryGetValue(eventID, out listeners))
            {
                // 2024-06-22:
                // 因为网络消息处理逻辑中存在删除对象对象的情况，
                // 考虑到该情况同样适用于事件系统，因此在此处做相同方式的处理
                // 通过临时列表来进行迭代
                IList<IEventDispatch> list;
                if (listeners.Count > 1)
                {
                    list = new List<IEventDispatch>();
                    ((List<IEventDispatch>) list).AddRange(listeners);
                }
                else
                {
                    list = listeners;
                }

                for (int n = 0; n < list.Count; ++n)
                {
                    IEventDispatch listener = list[n];
                    listener.OnEventDispatchForId(eventID, args);
                }

                list = null;
            }
        }

        /// <summary>
        /// 事件数据消息派发调度接口函数
        /// </summary>
        /// <param name="eventData">事件数据</param>
        private void OnEventDispatched(object eventData)
        {
            // 事件分发调度
            OnEventDistributeCallDispatched(eventData);

            IList<IEventDispatch> listeners;
            if (m_eventListenersForType.TryGetValue(eventData.GetType(), out listeners))
            {
                // 2024-06-22:
                // 因为网络消息处理逻辑中存在删除对象对象的情况，
                // 考虑到该情况同样适用于事件系统，因此在此处做相同方式的处理
                // 通过临时列表来进行迭代
                IList<IEventDispatch> list;
                if (listeners.Count > 1)
                {
                    list = new List<IEventDispatch>();
                    ((List<IEventDispatch>) list).AddRange(listeners);
                }
                else
                {
                    list = listeners;
                }

                for (int n = 0; n < list.Count; ++n)
                {
                    IEventDispatch listener = list[n];
                    listener.OnEventDispatchForType(eventData);
                }

                list = null;
            }
        }

        #region 事件回调句柄的订阅绑定和撤销接口函数

        /// <summary>
        /// 事件分发对象的事件订阅函数接口，指派一个指定的监听回调接口到目标事件
        /// </summary>
        /// <param name="eventID">事件标识</param>
        /// <param name="listener">事件监听回调接口</param>
        /// <returns>若事件订阅成功则返回true，否则返回false</returns>
        public bool Subscribe(int eventID, IEventDispatch listener)
        {
            IList<IEventDispatch> list;
            if (false == m_eventListenersForId.TryGetValue(eventID, out list))
            {
                list = new List<IEventDispatch>();
                list.Add(listener);

                m_eventListenersForId.Add(eventID, list);
                return true;
            }

            // 检查是否重复订阅
            if (list.Contains(listener))
            {
                Debugger.Warn("The listener for target event '{0}' was already subscribed, cannot repeat do it.", eventID);
                return false;
            }

            list.Add(listener);

            return true;
        }

        /// <summary>
        /// 事件分发对象的事件订阅函数接口，指派一个指定的监听回调接口到目标事件
        /// </summary>
        /// <param name="eventType">事件类型</param>
        /// <param name="listener">事件监听回调接口</param>
        /// <returns>若事件订阅成功则返回true，否则返回false</returns>
        public bool Subscribe(SystemType eventType, IEventDispatch listener)
        {
            IList<IEventDispatch> list;
            if (false == m_eventListenersForType.TryGetValue(eventType, out list))
            {
                list = new List<IEventDispatch>();
                list.Add(listener);

                m_eventListenersForType.Add(eventType, list);
                return true;
            }

            // 检查是否重复订阅
            if (list.Contains(listener))
            {
                Debugger.Warn("The listener for target event '{0}' was already subscribed, cannot repeat do it.", eventType.FullName);
                return false;
            }

            list.Add(listener);

            return true;
        }

        /// <summary>
        /// 取消指定事件的订阅监听回调接口
        /// </summary>
        /// <param name="eventID">事件标识</param>
        /// <param name="listener">事件监听回调接口</param>
        public void Unsubscribe(int eventID, IEventDispatch listener)
        {
            IList<IEventDispatch> list;
            if (false == m_eventListenersForId.TryGetValue(eventID, out list))
            {
                Debugger.Warn("Could not found any listener for target event '{0}' with on subscribed, do unsubscribe failed.", eventID);
                return;
            }

            list.Remove(listener);
            // 列表为空则移除对应的事件监听列表实例
            if (list.Count == 0)
            {
                m_eventListenersForId.Remove(eventID);
            }
        }

        /// <summary>
        /// 取消指定事件的订阅监听回调接口
        /// </summary>
        /// <param name="eventType">事件类型</param>
        /// <param name="listener">事件监听回调接口</param>
        public void Unsubscribe(SystemType eventType, IEventDispatch listener)
        {
            IList<IEventDispatch> list;
            if (false == m_eventListenersForType.TryGetValue(eventType, out list))
            {
                Debugger.Warn("Could not found any listener for target event '{0}' with on subscribed, do unsubscribe failed.", eventType.FullName);
                return;
            }

            list.Remove(listener);
            // 列表为空则移除对应的事件监听列表实例
            if (list.Count == 0)
            {
                m_eventListenersForType.Remove(eventType);
            }
        }

        /// <summary>
        /// 取消指定的监听回调接口对应的所有事件订阅
        /// </summary>
        public void UnsubscribeAll(IEventDispatch listener)
        {
            IList<int> ids = NovaEngine.Utility.Collection.ToListForKeys<int, IList<IEventDispatch>>(m_eventListenersForId);
            for (int n = 0; null != ids && n < ids.Count; ++n)
            {
                Unsubscribe(ids[n], listener);
            }

            IList<SystemType> types = NovaEngine.Utility.Collection.ToListForKeys<SystemType, IList<IEventDispatch>>(m_eventListenersForType);
            for (int n = 0; null != types && n < types.Count; ++n)
            {
                Unsubscribe(types[n], listener);
            }
        }

        #endregion
    }
}
