/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
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

using System;
using System.Collections.Generic;

namespace GameEngine
{
    /// <summary>
    /// 事件管理对象类，用于对场景上下文中的所有节点对象进行事件管理及分发
    /// </summary>
    internal sealed partial class EventController : BaseController<EventController>
    {
        /// <summary>
        /// 针对事件标识进行分发的监听对象管理列表容器
        /// </summary>
        private IDictionary<int, IList<IEventDispatch>> _eventListenersForId;

        /// <summary>
        /// 针对事件类型进行分发的监听对象管理列表容器
        /// </summary>
        private IDictionary<Type, IList<IEventDispatch>> _eventListenersForType;

        /// <summary>
        /// 事件分发数据的缓冲管理队列
        /// </summary>
        private Queue<EventData> _eventBuffers;

        /// <summary>
        /// 事件分发对象初始化通知接口函数
        /// </summary>
        protected override sealed void OnInitialize()
        {
            // 初始化监听列表
            _eventListenersForId = new Dictionary<int, IList<IEventDispatch>>();
            _eventListenersForType = new Dictionary<Type, IList<IEventDispatch>>();

            // 初始化事件数据缓冲队列
            _eventBuffers = new Queue<EventData>();
        }

        /// <summary>
        /// 事件分发对象清理通知接口函数
        /// </summary>
        protected override sealed void OnCleanup()
        {
            // 清理事件数据缓冲队列
            _eventBuffers.Clear();
            _eventBuffers = null;

            // 清理监听列表
            _eventListenersForId.Clear();
            _eventListenersForId = null;
            _eventListenersForType.Clear();
            _eventListenersForType = null;
        }

        /// <summary>
        /// 事件管理对象刷新通知接口函数
        /// </summary>
        protected override sealed void OnUpdate()
        {
            if (_eventBuffers.Count > 0)
            {
                Queue<EventData> queue = new Queue<EventData>(_eventBuffers);
                _eventBuffers.Clear();

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
        /// 事件管理对象重载调度函数接口
        /// </summary>
        protected override sealed void OnReload()
        {
        }

        /// <summary>
        /// 事件管理对象倾泻调度函数接口
        /// </summary>
        protected override sealed void OnDump()
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
            _eventBuffers.Enqueue(eventData);
        }

        /// <summary>
        /// 发送事件消息到当前管理器中等待派发
        /// </summary>
        /// <param name="arg">事件数据</param>
        public void Send<T>(T arg) where T : struct
        {
            EventData eventData = new EventData(arg);
            _eventBuffers.Enqueue(eventData);
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

            if (_eventListenersForId.TryGetValue(eventID, out IList<IEventDispatch> listeners))
            {
                // 2024-06-22:
                // 因为网络消息处理逻辑中存在删除对象对象的情况，
                // 考虑到该情况同样适用于事件系统，因此在此处做相同方式的处理
                // 通过临时列表来进行迭代
                IList<IEventDispatch> list;
                if (listeners.Count > 1)
                {
                    list = new List<IEventDispatch>(listeners);
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

            if (_eventListenersForType.TryGetValue(eventData.GetType(), out IList<IEventDispatch> listeners))
            {
                // 2024-06-22:
                // 因为网络消息处理逻辑中存在删除对象对象的情况，
                // 考虑到该情况同样适用于事件系统，因此在此处做相同方式的处理
                // 通过临时列表来进行迭代
                IList<IEventDispatch> list;
                if (listeners.Count > 1)
                {
                    list = new List<IEventDispatch>(listeners);
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
            if (false == _eventListenersForId.TryGetValue(eventID, out IList<IEventDispatch> list))
            {
                list = new List<IEventDispatch>() { listener };

                _eventListenersForId.Add(eventID, list);
                return true;
            }

            // 检查是否重复订阅
            if (list.Contains(listener))
            {
                Debugger.Warn(LogGroupTag.Controller, "The listener for target event '{%d}' was already subscribed, cannot repeat do it.", eventID);
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
        public bool Subscribe(Type eventType, IEventDispatch listener)
        {
            if (false == _eventListenersForType.TryGetValue(eventType, out IList<IEventDispatch> list))
            {
                list = new List<IEventDispatch>() { listener };

                _eventListenersForType.Add(eventType, list);
                return true;
            }

            // 检查是否重复订阅
            if (list.Contains(listener))
            {
                Debugger.Warn(LogGroupTag.Controller, "The listener for target event '{%t}' was already subscribed, cannot repeat do it.", eventType);
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
            if (false == _eventListenersForId.TryGetValue(eventID, out IList<IEventDispatch> list))
            {
                Debugger.Warn(LogGroupTag.Controller, "Could not found any listener for target event '{%d}' with on subscribed, do unsubscribe failed.", eventID);
                return;
            }

            list.Remove(listener);
            // 列表为空则移除对应的事件监听列表实例
            if (list.Count == 0)
            {
                _eventListenersForId.Remove(eventID);
            }
        }

        /// <summary>
        /// 取消指定事件的订阅监听回调接口
        /// </summary>
        /// <param name="eventType">事件类型</param>
        /// <param name="listener">事件监听回调接口</param>
        public void Unsubscribe(Type eventType, IEventDispatch listener)
        {
            if (false == _eventListenersForType.TryGetValue(eventType, out IList<IEventDispatch> list))
            {
                Debugger.Warn(LogGroupTag.Controller, "Could not found any listener for target event '{%t}' with on subscribed, do unsubscribe failed.", eventType);
                return;
            }

            list.Remove(listener);
            // 列表为空则移除对应的事件监听列表实例
            if (list.Count == 0)
            {
                _eventListenersForType.Remove(eventType);
            }
        }

        /// <summary>
        /// 取消指定的监听回调接口对应的所有事件订阅
        /// </summary>
        public void UnsubscribeAll(IEventDispatch listener)
        {
            IList<int> ids = NovaEngine.Utility.Collection.ToListForKeys(_eventListenersForId);
            for (int n = 0; null != ids && n < ids.Count; ++n)
            {
                Unsubscribe(ids[n], listener);
            }

            IList<Type> types = NovaEngine.Utility.Collection.ToListForKeys(_eventListenersForType);
            for (int n = 0; null != types && n < types.Count; ++n)
            {
                Unsubscribe(types[n], listener);
            }
        }

        #endregion
    }
}
