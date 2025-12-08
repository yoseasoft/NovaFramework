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

namespace NovaEngine
{
    /// 事件对象缓冲池实例定义
    internal sealed partial class EventPool<T>
    {
        /// <summary>
        /// 事件节点对象实例
        /// </summary>
        private sealed class Event : IReference
        {
            private object _sender;
            private T _eventArgs;

            public Event()
            {
                _sender = null;
                _eventArgs = null;
            }

            public object Sender
            {
                get { return _sender; }
            }

            public T EventArgs
            {
                get { return _eventArgs; }
            }

            public static Event Create(object sender, T e)
            {
                Event eventNode = ReferencePool.Acquire<Event>();
                eventNode._sender = sender;
                eventNode._eventArgs = e;
                eventNode.Initialize();
                return eventNode;
            }

            /// <summary>
            /// 事件节点初始化接口
            /// </summary>
            public void Initialize()
            {
            }

            /// <summary>
            /// 事件节点清理接口
            /// </summary>
            public void Cleanup()
            {
                _sender = null;
                _eventArgs = null;
            }
        }
    }
}
