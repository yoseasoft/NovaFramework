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

namespace GameEngine
{
    /// 引用对象抽象类
    public abstract partial class CRef
    {
        /// <summary>
        /// 对象内部订阅事件的标识管理容器
        /// </summary>
        private IList<int> _eventIds;
        /// <summary>
        /// 对象内部订阅事件的类型管理容器
        /// </summary>
        private IList<Type> _eventTypes;

        /// <summary>
        /// 引用对象的事件订阅处理初始化函数接口
        /// </summary>
        private void OnEventSubscribeProcessingInitialize()
        {
            // 事件标识容器初始化
            _eventIds = new List<int>();
            // 事件类型容器初始化
            _eventTypes = new List<Type>();
        }

        /// <summary>
        /// 引用对象的事件订阅处理清理函数接口
        /// </summary>
        private void OnEventSubscribeProcessingCleanup()
        {
            // 移除所有订阅事件
            Debugger.Assert(_eventIds.Count == 0 && _eventTypes.Count == 0);
            _eventIds = null;
            _eventTypes = null;
        }

        #region 基础对象事件订阅相关处理函数的操作接口定义

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
        protected override bool OnSubscribeActionPostProcess(Type eventType)
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
        protected override void OnUnsubscribeActionPostProcess(Type eventType)
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
                // Debugger.Warn("The 'CRef' instance event '{%d}' was already subscribed, repeat do it failed.", eventID);
                return true;
            }

            if (false == EventController.Instance.Subscribe(eventID, this))
            {
                Debugger.Warn("The 'CRef' instance subscribe event '{%d}' failed.", eventID);
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
        public override sealed bool Subscribe(Type eventType)
        {
            if (_eventTypes.Contains(eventType))
            {
                // Debugger.Warn("The 'CRef' instance's event '{%t}' was already subscribed, repeat do it failed.", eventType);
                return true;
            }

            if (false == EventController.Instance.Subscribe(eventType, this))
            {
                Debugger.Warn("The 'CRef' instance subscribe event '{%t}' failed.", eventType);
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
                // Debugger.Warn("Could not found any event '{%d}' for target 'CRef' instance with on subscribed, do unsubscribe failed.", eventID);
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
        public override sealed void Unsubscribe(Type eventType)
        {
            if (false == _eventTypes.Contains(eventType))
            {
                // Debugger.Warn("Could not found any event '{%t}' for target 'CRef' instance with on subscribed, do unsubscribe failed.", eventType);
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
    }
}
