/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
/// Copyright (C) 2020 - 2022, Guangzhou Xinyuan Technology Co., Ltd.
/// Copyright (C) 2022 - 2023, Shanghai Bilibili Technology Co., Ltd.
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

namespace NovaEngine
{
    /// <summary>
    /// 事件对象缓冲池实例定义
    /// </summary>
    /// <typeparam name="T">事件类型</typeparam>
    internal sealed partial class EventPool<T> where T : EventArgs
    {
        private readonly MultiDictionary<int, System.EventHandler<T>> _eventHandlers;
        private readonly Queue<Event> _events;
        private readonly Dictionary<object, LinkedListNode<System.EventHandler<T>>> _cachedNodes;
        private readonly Dictionary<object, LinkedListNode<System.EventHandler<T>>> _tempNodes;
        private readonly AllowHandlerType _mode;

        private System.EventHandler<T> _defaultHandler;

        /// <summary>
        /// 事件对象缓冲池的新实例构建接口
        /// </summary>
        /// <param name="mode">事件池模式</param>
        public EventPool(AllowHandlerType mode)
        {
            _eventHandlers = new MultiDictionary<int, System.EventHandler<T>>();
            _events = new Queue<Event>();
            _cachedNodes = new Dictionary<object, LinkedListNode<System.EventHandler<T>>>();
            _tempNodes = new Dictionary<object, LinkedListNode<System.EventHandler<T>>>();
            _mode = mode;
            _defaultHandler = null;
        }

        /// <summary>
        /// 获取事件处理函数的数量
        /// </summary>
        public int EventHandlerCount
        {
            get { return _eventHandlers.Count; }
        }

        /// <summary>
        /// 获取事件记录数量
        /// </summary>
        public int EventCount
        {
            get { return _events.Count; }
        }

        /// <summary>
        /// 事件池启动接口
        /// </summary>
        public void Startup()
        {
        }

        /// <summary>
        /// 事件池关闭接口
        /// </summary>
        public void Shutdown()
        {
            Clear();

            _eventHandlers.Clear();
            _cachedNodes.Clear();
            _tempNodes.Clear();
            _defaultHandler = null;
        }

        /// <summary>
        /// 事件池轮询调度接口
        /// </summary>
        public void Update()
        {
            while (_events.Count > 0)
            {
                Event eventNode = null;
                lock (_events)
                {
                    eventNode = _events.Dequeue();
                    HandleEvent(eventNode.Sender, eventNode.EventArgs);
                }

                ReferencePool.Release(eventNode);
            }
        }

        /// <summary>
        /// 清理事件记录
        /// </summary>
        public void Clear()
        {
            lock (_events)
            {
                _events.Clear();
            }
        }

        /// <summary>
        /// 获取指定类型编号下的事件处理函数的数量
        /// </summary>
        /// <param name="id">事件类型编号</param>
        /// <returns>返回对应事件类型编号的处理函数的数量</returns>
        public int Count(int id)
        {
            DoubleLinkedList<System.EventHandler<T>> range = default(DoubleLinkedList<System.EventHandler<T>>);
            if (_eventHandlers.TryGetValue(id, out range))
            {
                return range.Count;
            }

            return 0;
        }

        /// <summary>
        /// 检查是否存在指定事件类型编号的对应事件处理函数
        /// </summary>
        /// <param name="id">事件类型编号</param>
        /// <param name="handler">待校验的事件处理函数</param>
        /// <returns>若存在对应的事件处理函数则返回true，否则返回false</returns>
        public bool Check(int id, System.EventHandler<T> handler)
        {
            if (null == handler)
            {
                throw new CFrameworkException("Event handler is invalid.");
            }

            return _eventHandlers.Contains(id, handler);
        }

        /// <summary>
        /// 订阅事件处理函数
        /// </summary>
        /// <param name="id">事件类型编号</param>
        /// <param name="handler">待订阅的事件处理函数</param>
        public void Subscribe(int id, System.EventHandler<T> handler)
        {
            if (null == handler)
            {
                throw new CFrameworkException("Event handler is invalid.");
            }

            if (false == _eventHandlers.Contains(id))
            {
                _eventHandlers.Add(id, handler);
            }
            else if ((_mode & AllowHandlerType.AllowMultiHandler) != AllowHandlerType.AllowMultiHandler)
            {
                throw new CFrameworkException("Event '{0}' not allow multi handler.", id.ToString());
            }
            else if ((_mode & AllowHandlerType.AllowDuplicateHandler) != AllowHandlerType.AllowDuplicateHandler)
            {
                throw new CFrameworkException("Event '{0}' not allow duplicate handler.", id.ToString());
            }
            else
            {
                _eventHandlers.Add(id, handler);
            }
        }

        /// <summary>
        /// 取消订阅事件处理函数
        /// </summary>
        /// <param name="id">事件类型编号</param>
        /// <param name="handler">待取消订阅的事件处理函数</param>
        public void Unsubscribe(int id, System.EventHandler<T> handler)
        {
            if (null == handler)
            {
                throw new CFrameworkException("Event handler is invalid.");
            }

            if (_cachedNodes.Count > 0)
            {
                foreach (KeyValuePair<object, LinkedListNode<System.EventHandler<T>>> cachedNode in _cachedNodes)
                {
                    if (cachedNode.Value != null && cachedNode.Value.Value == handler)
                    {
                        _tempNodes.Add(cachedNode.Key, cachedNode.Value.Next);
                    }
                }

                if (_tempNodes.Count > 0)
                {
                    foreach (KeyValuePair<object, LinkedListNode<System.EventHandler<T>>> cachedNode in _tempNodes)
                    {
                        _cachedNodes[cachedNode.Key] = cachedNode.Value;
                    }

                    _tempNodes.Clear();
                }
            }

            if (false == _eventHandlers.Remove(id, handler))
            {
                throw new CFrameworkException("Event '{0}' not exists specified handler.", id.ToString());
            }
        }

        /// <summary>
        /// 设置默认事件处理函数
        /// </summary>
        /// <param name="handler">待设置的事件处理函数</param>
        public void SetDefaultHandler(System.EventHandler<T> handler)
        {
            _defaultHandler = handler;
        }

        /// <summary>
        /// 抛出事件，该操作为线程安全模式，即使在非主线程中抛出，也可保证在主线程中执行回调事件处理函数，但事件会在抛出后的下一帧进行分发
        /// </summary>
        /// <param name="sender">事件源</param>
        /// <param name="e">事件参数</param>
        public void Fire(object sender, T e)
        {
            if (null == e)
            {
                throw new CFrameworkException("Event is invalid.");
            }

            Event eventNode = Event.Create(sender, e);
            lock (_events)
            {
                _events.Enqueue(eventNode);
            }
        }

        /// <summary>
        /// 抛出事件并立刻执行模式，该操作为非线程安全模式，事件会在当前线程中被立刻执行分发
        /// </summary>
        /// <param name="sender">事件源</param>
        /// <param name="e">事件参数</param>
        public void FireNow(object sender, T e)
        {
            if (null == e)
            {
                throw new CFrameworkException("Event is invalid.");
            }

            HandleEvent(sender, e);
        }

        /// <summary>
        /// 处理事件节点的函数执行接口
        /// </summary>
        /// <param name="sender">事件源</param>
        /// <param name="e">事件参数</param>
        private void HandleEvent(object sender, T e)
        {
            bool noHandlerException = false;
            DoubleLinkedList<System.EventHandler<T>> range = default(DoubleLinkedList<System.EventHandler<T>>);
            if (_eventHandlers.TryGetValue(e.ID, out range))
            {
                LinkedListNode<System.EventHandler<T>> current = range.First;
                while (current != null && current != range.Terminal)
                {
                    _cachedNodes[e] = current.Next != range.Terminal ? current.Next : null;
                    current.Value(sender, e);
                    current = _cachedNodes[e];
                }

                _cachedNodes.Remove(e);
            }
            else if (null != _defaultHandler)
            {
                _defaultHandler(sender, e);
            }
            else if ((_mode & AllowHandlerType.AllowNoHandler) == 0)
            {
                noHandlerException = true;
            }

            ReferencePool.Release(e);

            if (noHandlerException)
            {
                throw new CFrameworkException("Event '{0}' not allow no handler.", e.ID.ToString());
            }
        }
    }
}
