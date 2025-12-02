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
using System.Reflection;

using SystemType = System.Type;

namespace GameEngine
{
    /// 基础对象抽象类
    public abstract partial class CBase
    {
        /// <summary>
        /// 基础对象内部消息事件的订阅回调映射列表
        /// </summary>
        private IDictionary<int, IDictionary<string, bool>> _eventSubscribeCallForId;
        /// <summary>
        /// 基础对象内部事件类型的订阅回调映射列表
        /// </summary>
        private IDictionary<SystemType, IDictionary<string, bool>> _eventSubscribeCallForType;

        /// <summary>
        /// 事件订阅回调函数的绑定接口缓存容器
        /// </summary>
        private IDictionary<string, EventCallMethodInfo> _eventCallBindingCaches;

        /// <summary>
        /// 基础对象的事件订阅回调初始化函数接口
        /// </summary>
        private void OnEventSubscribeCallInitialize()
        {
            // 事件订阅回调映射容器初始化
            _eventSubscribeCallForId = new Dictionary<int, IDictionary<string, bool>>();
            _eventSubscribeCallForType = new Dictionary<SystemType, IDictionary<string, bool>>();
        }

        /// <summary>
        /// 基础对象的事件订阅回调清理函数接口
        /// </summary>
        private void OnEventSubscribeCallCleanup()
        {
            // 移除所有订阅事件
            UnsubscribeAllEvents();

            _eventSubscribeCallForId = null;
            _eventSubscribeCallForType = null;

            // 移除事件订阅的委托句柄缓存实例
            RemoveAllEventCallDelegateHandlers();
            _eventCallBindingCaches = null;
        }

        #region 基础对象事件分发提供的服务接口函数

        /// <summary>
        /// 发送事件消息到事件管理器中等待派发
        /// </summary>
        /// <param name="eventID">事件标识</param>
        /// <param name="args">事件参数列表</param>
        public void Send(int eventID, params object[] args)
        {
            EventController.Instance.Send(eventID, args);
        }

        /// <summary>
        /// 发送事件消息到事件管理器中等待派发
        /// </summary>
        /// <param name="arg">事件数据</param>
        public void Send<T>(T arg) where T : struct
        {
            EventController.Instance.Send<T>(arg);
        }

        /// <summary>
        /// 发送事件消息到事件管理器中并立即处理掉它
        /// </summary>
        /// <param name="eventID">事件标识</param>
        /// <param name="args">事件参数列表</param>
        public void Fire(int eventID, params object[] args)
        {
            EventController.Instance.Fire(eventID, args);
        }

        /// <summary>
        /// 发送事件消息到事件管理器中并立即处理掉它
        /// </summary>
        /// <param name="arg">事件数据</param>
        public void Fire<T>(T arg) where T : struct
        {
            EventController.Instance.Fire<T>(arg);
        }

        #endregion

        #region 基础对象事件订阅相关回调函数的操作接口定义

        /// <summary>
        /// 基础对象的订阅事件的监听回调函数<br/>
        /// 该函数针对事件转发接口的标准实现，禁止子类重写该函数<br/>
        /// 若子类需要根据需要自行解析事件，可以通过重写<see cref="GameEngine.CBase.OnEvent(int, object[])"/>实现事件的自定义处理逻辑
        /// </summary>
        /// <param name="eventID">事件标识</param>
        /// <param name="args">事件数据参数</param>
        public virtual void OnEventDispatchForId(int eventID, params object[] args)
        {
            if (_eventSubscribeCallForId.TryGetValue(eventID, out IDictionary<string, bool> calls))
            {
                foreach (KeyValuePair<string, bool> kvp in calls)
                {
                    // EventController.Instance.InvokeEventSubscribeBindingCall(this, kvp.Key, BeanType, eventID, args);
                    InvokeEventCall(kvp.Key, eventID, args);
                }
            }

            OnEvent(eventID, args);
        }

        /// <summary>
        /// 基础对象的订阅事件的监听回调函数<br/>
        /// 该函数针对事件转发接口的标准实现，禁止子类重写该函数<br/>
        /// 若子类需要根据需要自行解析事件，可以通过重写<see cref="GameEngine.CBase.OnEvent(object)"/>实现事件的自定义处理逻辑
        /// </summary>
        /// <param name="eventData">事件数据</param>
        public virtual void OnEventDispatchForType(object eventData)
        {
            if (_eventSubscribeCallForType.TryGetValue(eventData.GetType(), out IDictionary<string, bool> calls))
            {
                foreach (KeyValuePair<string, bool> kvp in calls)
                {
                    // EventController.Instance.InvokeEventSubscribeBindingCall(this, kvp.Key, BeanType, eventData);
                    InvokeEventCall(kvp.Key, eventData);
                }
            }

            OnEvent(eventData);
        }

        /// <summary>
        /// 用户自定义的事件处理函数，您可以通过重写该函数处理自定义事件行为
        /// </summary>
        /// <param name="eventID">事件标识</param>
        /// <param name="args">事件数据参数</param>
        protected abstract void OnEvent(int eventID, params object[] args);

        /// <summary>
        /// 用户自定义的事件处理函数，您可以通过重写该函数处理自定义事件行为
        /// </summary>
        /// <param name="eventData">事件数据</param>
        protected abstract void OnEvent(object eventData);

        /// <summary>
        /// 针对指定事件标识新增事件订阅的后处理程序
        /// </summary>
        /// <param name="eventID">事件标识</param>
        /// <returns>返回后处理的操作结果</returns>
        protected abstract bool OnSubscribeActionPostProcess(int eventID);
        /// <summary>
        /// 针对指定事件类型新增事件订阅的后处理程序
        /// </summary>
        /// <param name="eventType">事件类型</param>
        /// <returns>返回后处理的操作结果</returns>
        protected abstract bool OnSubscribeActionPostProcess(SystemType eventType);
        /// <summary>
        /// 针对指定事件标识移除事件订阅的后处理程序
        /// </summary>
        /// <param name="eventID">事件标识</param>
        protected abstract void OnUnsubscribeActionPostProcess(int eventID);
        /// <summary>
        /// 针对指定事件类型移除事件订阅的后处理程序
        /// </summary>
        /// <param name="eventType">事件类型</param>
        protected abstract void OnUnsubscribeActionPostProcess(SystemType eventType);

        /// <summary>
        /// 检测当前基础对象是否订阅了目标事件标识
        /// </summary>
        /// <param name="eventID">事件标识</param>
        /// <returns>若订阅了给定事件标识则返回true，否则返回false</returns>
        protected internal virtual bool IsSubscribedOfTargetId(int eventID)
        {
            if (_eventSubscribeCallForId.ContainsKey(eventID) && _eventSubscribeCallForId[eventID].Count > 0)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 检测当前基础对象是否订阅了目标事件类型
        /// </summary>
        /// <param name="eventType">事件类型</param>
        /// <returns>若订阅了给定事件类型则返回true，否则返回false</returns>
        protected internal virtual bool IsSubscribedOfTargetType(SystemType eventType)
        {
            if (_eventSubscribeCallForType.ContainsKey(eventType) && _eventSubscribeCallForType[eventType].Count > 0)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 基础对象的事件订阅函数接口，对一个指定的事件进行订阅监听
        /// </summary>
        /// <param name="eventID">事件标识</param>
        /// <returns>若事件订阅成功则返回true，否则返回false</returns>
        public virtual bool Subscribe(int eventID)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// 基础对象的事件订阅函数接口，将一个指定的事件绑定到给定的监听回调函数上
        /// </summary>
        /// <param name="fullname">函数名称</param>
        /// <param name="methodInfo">监听回调函数</param>
        /// <param name="eventID">事件标识</param>
        /// <returns>若事件订阅成功则返回true，否则返回false</returns>
        public bool Subscribe(string fullname, MethodInfo methodInfo, int eventID)
        {
            return Subscribe(fullname, methodInfo, eventID, false);
        }

        /// <summary>
        /// 基础对象的事件订阅函数接口，将一个指定的事件绑定到给定的监听回调函数上
        /// </summary>
        /// <param name="fullname">函数名称</param>
        /// <param name="methodInfo">监听回调函数</param>
        /// <param name="eventID">事件标识</param>
        /// <param name="automatically">自动装载状态标识</param>
        /// <returns>若事件订阅成功则返回true，否则返回false</returns>
        protected internal bool Subscribe(string fullname, MethodInfo methodInfo, int eventID, bool automatically)
        {
            // 2025-11-30：
            // 针对普通函数采用对象自身构建的方式
            // EventController.Instance.AddEventSubscribeBindingCallInfo(fullname, BeanType, methodInfo, eventID, automatically);
            AddEventCallDelegateHandler(fullname, methodInfo, eventID, automatically);

            if (false == _eventSubscribeCallForId.TryGetValue(eventID, out IDictionary<string, bool> calls))
            {
                // 创建回调列表
                calls = new Dictionary<string, bool>();
                calls.Add(fullname, automatically);

                _eventSubscribeCallForId.Add(eventID, calls);

                // 新增事件订阅的后处理程序
                return OnSubscribeActionPostProcess(eventID);
            }

            if (calls.ContainsKey(fullname))
            {
                Debugger.Warn("The '{%t}' instance's event '{%d}' was already add same listener by name '{%s}', repeat do it failed.",
                        BeanType, eventID, fullname);
                return false;
            }

            calls.Add(fullname, automatically);

            return true;
        }

        /// <summary>
        /// 基础对象的事件订阅函数接口，对一个指定的事件进行订阅监听
        /// </summary>
        /// <typeparam name="T">事件类型</typeparam>
        /// <returns>若事件订阅成功则返回true，否则返回false</returns>
        public bool Subscribe<T>() where T : struct
        {
            return Subscribe(typeof(T));
        }

        /// <summary>
        /// 基础对象的事件订阅函数接口，对一个指定的事件进行订阅监听
        /// </summary>
        /// <param name="eventType">事件类型</param>
        /// <returns>若事件订阅成功则返回true，否则返回false</returns>
        public virtual bool Subscribe(SystemType eventType)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// 基础对象的事件订阅函数接口，将一个指定的事件绑定到给定的监听回调函数上
        /// </summary>
        /// <typeparam name="T">事件类型</typeparam>
        /// <param name="func">监听回调函数</param>
        /// <returns>若事件订阅成功则返回true，否则返回false</returns>
        public bool Subscribe<T>(System.Action<T> func) where T : struct
        {
            string fullname = NovaEngine.Utility.Text.GetFullName(func.Method);
            return Subscribe(fullname, func.Method, typeof(T));
        }

        /// <summary>
        /// 基础对象的事件订阅函数接口，将一个指定的事件绑定到给定的监听回调函数上
        /// </summary>
        /// <param name="fullname">函数名称</param>
        /// <param name="methodInfo">监听回调函数</param>
        /// <param name="eventType">事件类型</param>
        /// <returns>若事件订阅成功则返回true，否则返回false</returns>
        public bool Subscribe(string fullname, MethodInfo methodInfo, SystemType eventType)
        {
            return Subscribe(fullname, methodInfo, eventType, false);
        }

        /// <summary>
        /// 基础对象的事件订阅函数接口，将一个指定的事件绑定到给定的监听回调函数上
        /// </summary>
        /// <param name="fullname">函数名称</param>
        /// <param name="methodInfo">监听回调函数</param>
        /// <param name="eventType">事件类型</param>
        /// <param name="automatically">自动装载状态标识</param>
        /// <returns>若事件订阅成功则返回true，否则返回false</returns>
        protected internal bool Subscribe(string fullname, MethodInfo methodInfo, SystemType eventType, bool automatically)
        {
            // 2025-11-30：
            // 针对普通函数采用对象自身构建的方式
            // EventController.Instance.AddEventSubscribeBindingCallInfo(fullname, BeanType, methodInfo, eventType, automatically);
            AddEventCallDelegateHandler(fullname, methodInfo, eventType, automatically);

            if (false == _eventSubscribeCallForType.TryGetValue(eventType, out IDictionary<string, bool> calls))
            {
                // 创建回调列表
                calls = new Dictionary<string, bool>();
                calls.Add(fullname, automatically);

                _eventSubscribeCallForType.Add(eventType, calls);

                // 新增事件订阅的后处理程序
                return OnSubscribeActionPostProcess(eventType);
            }

            if (calls.ContainsKey(fullname))
            {
                Debugger.Warn("The '{%t}' instance's event '{%t}' was already add same listener by name '{%s}', repeat do it failed.",
                        BeanType, eventType, fullname);
                return false;
            }

            calls.Add(fullname, automatically);

            return true;
        }

        /// <summary>
        /// 取消当前基础对象对指定事件的订阅
        /// </summary>
        /// <param name="eventID">事件标识</param>
        public virtual void Unsubscribe(int eventID)
        {
            // 若针对特定事件绑定了监听回调，则移除相应的回调句柄
            if (_eventSubscribeCallForId.ContainsKey(eventID))
            {
                _eventSubscribeCallForId.Remove(eventID);
            }

            // 移除事件订阅的后处理程序
            OnUnsubscribeActionPostProcess(eventID);
        }

        /// <summary>
        /// 取消当前基础对象对指定扩展事件对应的监听回调函数
        /// </summary>
        /// <param name="methodInfo">监听回调函数</param>
        /// <param name="eventID">事件标识</param>
        public void Unsubscribe(MethodInfo methodInfo, int eventID)
        {
            string fullname = _Generator.GenUniqueName(methodInfo);

            Unsubscribe(fullname, eventID);
        }

        /// <summary>
        /// 取消当前基础对象对指定扩展事件对应的监听回调函数
        /// </summary>
        /// <param name="fullname">函数名称</param>
        /// <param name="eventID">事件标识</param>
        protected internal void Unsubscribe(string fullname, int eventID)
        {
            if (_eventSubscribeCallForId.TryGetValue(eventID, out IDictionary<string, bool> calls))
            {
                if (calls.ContainsKey(fullname))
                {
                    calls.Remove(fullname);
                    // 移除构建的委托句柄
                    RemoveEventCallDelegateHandler(fullname);
                }
            }

            // 当前监听列表为空时，移除该事件的监听
            if (false == IsSubscribedOfTargetId(eventID))
            {
                Unsubscribe(eventID);
            }
        }

        /// <summary>
        /// 取消当前基础对象对指定事件的订阅
        /// </summary>
        /// <typeparam name="T">事件类型</typeparam>
        public void Unsubscribe<T>()
        {
            Unsubscribe(typeof(T));
        }

        /// <summary>
        /// 取消当前基础对象对指定事件的订阅
        /// </summary>
        /// <param name="eventType">事件类型</param>
        public virtual void Unsubscribe(SystemType eventType)
        {
            // 若针对特定事件绑定了监听回调，则移除相应的回调句柄
            if (_eventSubscribeCallForType.ContainsKey(eventType))
            {
                _eventSubscribeCallForType.Remove(eventType);
            }

            // 移除事件订阅的后处理程序
            OnUnsubscribeActionPostProcess(eventType);
        }

        /// <summary>
        /// 取消当前基础对象对指定事件对应的监听回调函数
        /// </summary>
        /// <typeparam name="T">事件类型</typeparam>
        /// <param name="methodInfo">监听回调函数</param>
        public void Unsubscribe<T>(MethodInfo methodInfo)
        {
            Unsubscribe(methodInfo, typeof(T));
        }

        /// <summary>
        /// 取消当前基础对象对指定事件的订阅
        /// </summary>
        /// <param name="methodInfo">监听回调函数</param>
        /// <param name="eventType">事件类型</param>
        public void Unsubscribe(MethodInfo methodInfo, SystemType eventType)
        {
            string fullname = _Generator.GenUniqueName(methodInfo);

            Unsubscribe(fullname, eventType);
        }

        /// <summary>
        /// 取消当前基础对象对指定事件对应的监听回调函数
        /// </summary>
        /// <typeparam name="T">事件类型</typeparam>
        /// <param name="fullname">函数名称</param>
        protected internal void Unsubscribe<T>(string fullname)
        {
            Unsubscribe(fullname, typeof(T));
        }

        /// <summary>
        /// 取消当前基础对象对指定事件对应的监听回调函数
        /// </summary>
        /// <param name="fullname">函数名称</param>
        /// <param name="eventType">事件类型</param>
        protected internal void Unsubscribe(string fullname, SystemType eventType)
        {
            if (_eventSubscribeCallForType.TryGetValue(eventType, out IDictionary<string, bool> calls))
            {
                if (calls.ContainsKey(fullname))
                {
                    calls.Remove(fullname);
                    // 移除构建的委托句柄
                    RemoveEventCallDelegateHandler(fullname);
                }
            }

            // 当前监听列表为空时，移除该事件的监听
            if (false == IsSubscribedOfTargetType(eventType))
            {
                Unsubscribe(eventType);
            }
        }

        /// <summary>
        /// 移除所有自动注册的事件订阅回调接口
        /// </summary>
        protected internal void UnsubscribeAllAutomaticallyEvents()
        {
            OnAutomaticallyCallSyntaxInfoProcessHandler<int>(_eventSubscribeCallForId, Unsubscribe);
            OnAutomaticallyCallSyntaxInfoProcessHandler<SystemType>(_eventSubscribeCallForType, Unsubscribe);
        }

        /// <summary>
        /// 取消当前基础对象的所有事件订阅
        /// </summary>
        public virtual void UnsubscribeAllEvents()
        {
            IList<int> id_keys = NovaEngine.Utility.Collection.ToListForKeys<int, IDictionary<string, bool>>(_eventSubscribeCallForId);
            if (null != id_keys)
            {
                int c = id_keys.Count;
                for (int n = 0; n < c; ++n) { Unsubscribe(id_keys[n]); }
            }

            IList<SystemType> type_keys = NovaEngine.Utility.Collection.ToListForKeys<SystemType, IDictionary<string, bool>>(_eventSubscribeCallForType);
            if (null != type_keys)
            {
                int c = type_keys.Count;
                for (int n = 0; n < c; ++n) { Unsubscribe(type_keys[n]); }
            }

            _eventSubscribeCallForId.Clear();
            _eventSubscribeCallForType.Clear();
        }

        #endregion

        #region 基础对象事件订阅相关回调函数的构建委托接口定义

        /// <summary>
        /// 构建一个目标事件回调函数的委托句柄，并添加到调度容器中
        /// </summary>
        /// <param name="fullname">函数名称</param>
        /// <param name="methodInfo">监听回调函数</param>
        /// <param name="eventID">事件标识</param>
        /// <param name="automatically">自动装载状态标识</param>
        private void AddEventCallDelegateHandler(string fullname, MethodInfo methodInfo, int eventID, bool automatically)
        {
            // 静态函数（包括扩展类型）
            if (methodInfo.IsStatic)
            {
                EventController.Instance.AddEventSubscribeBindingCallInfo(fullname, BeanType, methodInfo, eventID, automatically);
                return;
            }

            if (null == _eventCallBindingCaches)
            {
                _eventCallBindingCaches = new Dictionary<string, EventCallMethodInfo>();
            }

            if (_eventCallBindingCaches.ContainsKey(fullname))
            {
                return;
            }

            Debugger.Info(LogGroupTag.Controller, "新增指定的消息事件‘{%d}’对应的事件订阅绑定回调函数，其响应接口函数来自于目标类型‘{%t}’的‘{%s}’函数。",
                    eventID, BeanType, fullname);

            EventCallMethodInfo eventCallMethodInfo = new EventCallMethodInfo(this, fullname, BeanType, methodInfo, eventID, automatically);
            _eventCallBindingCaches.Add(fullname, eventCallMethodInfo);
        }

        /// <summary>
        /// 构建一个目标事件回调函数的委托句柄，并添加到调度容器中
        /// </summary>
        /// <param name="fullname">函数名称</param>
        /// <param name="methodInfo">监听回调函数</param>
        /// <param name="eventType">事件类型</param>
        /// <param name="automatically">自动装载状态标识</param>
        private void AddEventCallDelegateHandler(string fullname, MethodInfo methodInfo, SystemType eventType, bool automatically)
        {
            // 静态函数（包括扩展类型）
            if (methodInfo.IsStatic)
            {
                EventController.Instance.AddEventSubscribeBindingCallInfo(fullname, BeanType, methodInfo, eventType, automatically);
                return;
            }

            if (null == _eventCallBindingCaches)
            {
                _eventCallBindingCaches = new Dictionary<string, EventCallMethodInfo>();
            }

            if (_eventCallBindingCaches.ContainsKey(fullname))
            {
                return;
            }

            Debugger.Info(LogGroupTag.Controller, "新增指定的事件类型‘{%t}’对应的事件订阅绑定回调函数，其响应接口函数来自于目标类型‘{%t}’的‘{%s}’函数。",
                    eventType, BeanType, fullname);

            EventCallMethodInfo eventCallMethodInfo = new EventCallMethodInfo(this, fullname, BeanType, methodInfo, eventType, automatically);
            _eventCallBindingCaches.Add(fullname, eventCallMethodInfo);
        }

        /// <summary>
        /// 从调度容器中移除指定名称对应的事件回调函数的委托句柄
        /// </summary>
        /// <param name="fullname">函数名称</param>
        private void RemoveEventCallDelegateHandler(string fullname)
        {
            if (null == _eventCallBindingCaches)
            {
                return;
            }

            if (_eventCallBindingCaches.ContainsKey(fullname))
            {
                _eventCallBindingCaches.Remove(fullname);
            }
        }

        /// <summary>
        /// 移除当前事件订阅回调函数构建的全部委托句柄实例
        /// </summary>
        private void RemoveAllEventCallDelegateHandlers()
        {
            _eventCallBindingCaches?.Clear();
        }

        /// <summary>
        /// 针对消息事件调用指定的回调绑定函数
        /// </summary>
        /// <param name="fullname">完整名称</param>
        /// <param name="eventID">事件标识</param>
        /// <param name="args">事件参数</param>
        private void InvokeEventCall(string fullname, int eventID, params object[] args)
        {
            if (null != _eventCallBindingCaches && _eventCallBindingCaches.TryGetValue(fullname, out EventCallMethodInfo eventCallMethodInfo))
            {
                eventCallMethodInfo.Invoke(eventID, args);
                return;
            }

            EventController.Instance.InvokeEventSubscribeBindingCall(this, fullname, BeanType, eventID, args);
        }

        /// <summary>
        /// 针对输入数据调用指定的回调绑定函数
        /// </summary>
        /// <param name="fullname">完整名称</param>
        /// <param name="eventData">事件数据</param>
        private void InvokeEventCall(string fullname, object eventData)
        {
            if (null != _eventCallBindingCaches && _eventCallBindingCaches.TryGetValue(fullname, out EventCallMethodInfo eventCallMethodInfo))
            {
                eventCallMethodInfo.Invoke(eventData);
                return;
            }

            EventController.Instance.InvokeEventSubscribeBindingCall(this, fullname, BeanType, eventData);
        }

        #endregion
    }
}
