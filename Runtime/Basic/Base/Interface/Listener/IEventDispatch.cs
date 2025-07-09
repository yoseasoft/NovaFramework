/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyright (C) 2023, Guangzhou Shiyue Network Technology Co., Ltd.
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

namespace GameEngine
{
    /// <summary>
    /// 事件监听接口类，用于定义接收监听事件的函数接口
    /// </summary>
    public interface IEventDispatch : IListener
    {
        /// <summary>
        /// 对象内部订阅事件的监听回调接口，通过该类型函数对指定标识事件进行监听处理
        /// </summary>
        /// <param name="eventID">事件标识</param>
        /// <param name="args">事件数据参数</param>
        // public delegate void EventDispatchingListenerForId(int eventID, params object[] args);

        /// <summary>
        /// 对象内部订阅事件的监听回调接口，通过该类型函数对指定类型事件进行监听处理
        /// </summary>
        /// <param name="eventData">事件数据</param>
        // public delegate void EventDispatchingListenerForType(object eventData);

        /// <summary>
        /// 接收监听指定标识的事件的回调接口函数
        /// </summary>
        /// <param name="eventID">事件标识</param>
        /// <param name="args">事件数据参数</param>
        void OnEventDispatchForId(int eventID, params object[] args);

        /// <summary>
        /// 接收监听指定类型的事件的回调接口函数
        /// </summary>
        /// <param name="eventData">事件数据</param>
        void OnEventDispatchForType(object eventData);
    }
}
