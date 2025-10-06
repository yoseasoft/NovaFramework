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

using System.Collections.Generic;

using SystemType = System.Type;
using SystemMethodInfo = System.Reflection.MethodInfo;

namespace GameEngine
{
    /// <summary>
    /// 事件管理对象类，用于对场景上下文中的所有节点对象进行事件管理及分发
    /// </summary>
    internal partial class EventController
    {
        /// <summary>
        /// 事件标识分发调度接口的数据结构容器
        /// </summary>
        private IDictionary<int, IList<EventCallMethodInfo>> _eventIdDistributeCallInfos;
        /// <summary>
        /// 事件数据分发调度接口的数据结构容器
        /// </summary>
        private IDictionary<SystemType, IList<EventCallMethodInfo>> _eventDataDistributeCallInfos;

        /// <summary>
        /// 事件订阅回调绑定接口的缓存容器
        /// </summary>
        private IDictionary<SystemType, IDictionary<string, EventCallMethodInfo>> _eventSubscribeBindingCaches;

        /// <summary>
        /// 事件分发回调管理模块的初始化函数
        /// </summary>
        [OnControllerSubmoduleInitCallback]
        private void InitializeForEventCall()
        {
            // 事件标识分发容器初始化
            _eventIdDistributeCallInfos = new Dictionary<int, IList<EventCallMethodInfo>>();
            // 事件数据分发容器初始化
            _eventDataDistributeCallInfos = new Dictionary<SystemType, IList<EventCallMethodInfo>>();

            // 初始化回调绑定缓存容器
            _eventSubscribeBindingCaches = new Dictionary<SystemType, IDictionary<string, EventCallMethodInfo>>();
        }

        /// <summary>
        /// 事件分发回调管理模块的清理函数
        /// </summary>
        [OnControllerSubmoduleCleanupCallback]
        private void CleanupForEventCall()
        {
            // 移除所有事件分发回调句柄
            RemoveAllEventDistributeCalls();

            // 事件标识分发容器清理
            _eventIdDistributeCallInfos.Clear();
            _eventIdDistributeCallInfos = null;
            // 事件数据分发容器清理
            _eventDataDistributeCallInfos.Clear();
            _eventDataDistributeCallInfos = null;

            // 移除所有事件订阅绑定缓存函数
            RemoveAllEventSubscribeBindingCalls();

            // 清理回调绑定缓存容器
            _eventSubscribeBindingCaches.Clear();
            _eventSubscribeBindingCaches = null;
        }

        /// <summary>
        /// 事件分发回调管理模块的重载函数
        /// </summary>
        [OnControllerSubmoduleReloadCallback]
        private void ReloadForEventCall()
        {
            // 移除所有事件订阅绑定缓存函数
            RemoveAllEventSubscribeBindingCalls();
        }

        /// <summary>
        /// 针对事件标识进行事件分发的调度入口函数
        /// </summary>
        /// <param name="eventID">事件标识</param>
        /// <param name="args">事件参数列表</param>
        private void OnEventDistributeCallDispatched(int eventID, params object[] args)
        {
            IList<EventCallMethodInfo> list = null;
            if (_eventIdDistributeCallInfos.TryGetValue(eventID, out list))
            {
                IEnumerator<EventCallMethodInfo> e_info = list.GetEnumerator();
                while (e_info.MoveNext())
                {
                    EventCallMethodInfo info = e_info.Current;
                    if (null == info.TargetType)
                    {
                        info.Invoke(eventID, args);
                    }
                    else
                    {
                        IList<IBean> beans = BeanController.Instance.FindAllBeans(info.TargetType);
                        if (null != beans)
                        {
                            IEnumerator<IBean> e_bean = beans.GetEnumerator();
                            while (e_bean.MoveNext())
                            {
                                IBean bean = e_bean.Current;
                                info.Invoke(bean, eventID, args);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 针对事件数据进行事件分发的调度入口函数
        /// </summary>
        /// <param name="eventData">事件数据</param>
        private void OnEventDistributeCallDispatched(object eventData)
        {
            SystemType eventDataType = eventData.GetType();

            IList<EventCallMethodInfo> list = null;
            if (_eventDataDistributeCallInfos.TryGetValue(eventDataType, out list))
            {
                IEnumerator<EventCallMethodInfo> e_info = list.GetEnumerator();
                while (e_info.MoveNext())
                {
                    EventCallMethodInfo info = e_info.Current;
                    if (null == info.TargetType)
                    {
                        info.Invoke(eventData);
                    }
                    else
                    {
                        IList<IBean> beans = BeanController.Instance.FindAllBeans(info.TargetType);
                        if (null != beans)
                        {
                            IEnumerator<IBean> e_bean = beans.GetEnumerator();
                            while (e_bean.MoveNext())
                            {
                                IBean bean = e_bean.Current;
                                info.Invoke(bean, eventData);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 新增指定的分发函数到当前消息事件管理容器中
        /// </summary>
        /// <param name="fullname">完整名称</param>
        /// <param name="targetType">目标对象类型</param>
        /// <param name="methodInfo">函数对象</param>
        /// <param name="eventID">事件标识</param>
        private void AddEventDistributeCallInfo(string fullname, SystemType targetType, SystemMethodInfo methodInfo, int eventID)
        {
            EventCallMethodInfo info = new EventCallMethodInfo(fullname, targetType, methodInfo, eventID);

            Debugger.Info(LogGroupTag.Controller, "Add new event distribute call '{%s}' to target ID '{%d}' of the class type '{%t}'.",
                    fullname, eventID, targetType);
            if (_eventIdDistributeCallInfos.ContainsKey(eventID))
            {
                IList<EventCallMethodInfo> list = _eventIdDistributeCallInfos[eventID];
                list.Add(info);
            }
            else
            {
                IList<EventCallMethodInfo> list = new List<EventCallMethodInfo>();
                list.Add(info);
                _eventIdDistributeCallInfos.Add(eventID, list);
            }
        }

        /// <summary>
        /// 新增指定的分发函数到当前消息事件管理容器中
        /// </summary>
        /// <param name="fullname">完整名称</param>
        /// <param name="targetType">目标对象类型</param>
        /// <param name="methodInfo">函数对象</param>
        /// <param name="eventDataType">事件数据类型</param>
        private void AddEventDistributeCallInfo(string fullname, SystemType targetType, SystemMethodInfo methodInfo, SystemType eventDataType)
        {
            EventCallMethodInfo info = new EventCallMethodInfo(fullname, targetType, methodInfo, eventDataType);

            Debugger.Info(LogGroupTag.Controller, "Add new event distribute call '{%s}' to target data type '{%t}' of the class type '{%t}'.",
                    fullname, eventDataType, targetType);
            if (_eventDataDistributeCallInfos.ContainsKey(eventDataType))
            {
                IList<EventCallMethodInfo> list = _eventDataDistributeCallInfos[eventDataType];
                list.Add(info);
            }
            else
            {
                IList<EventCallMethodInfo> list = new List<EventCallMethodInfo>();
                list.Add(info);
                _eventDataDistributeCallInfos.Add(eventDataType, list);
            }
        }

        /// <summary>
        /// 从当前消息事件管理容器中移除指定标识的分发函数信息
        /// </summary>
        /// <param name="fullname">完整名称</param>
        /// <param name="targetType">目标对象类型</param>
        /// <param name="eventID">事件标识</param>
        private void RemoveEventDistributeCallInfo(string fullname, SystemType targetType, int eventID)
        {
            Debugger.Info(LogGroupTag.Controller, "Remove event distribute call '{%s}' with target ID '{%d}' and class type '{%t}'.",
                    fullname, eventID, targetType);
            if (false == _eventIdDistributeCallInfos.ContainsKey(eventID))
            {
                Debugger.Warn(LogGroupTag.Controller, "Could not found any event distribute call '{%s}' with target ID '{%d}', removed it failed.", fullname, eventID);
                return;
            }

            IList<EventCallMethodInfo> list = _eventIdDistributeCallInfos[eventID];
            for (int n = 0; n < list.Count; ++n)
            {
                EventCallMethodInfo info = list[n];
                if (info.Fullname.Equals(fullname))
                {
                    Debugger.Assert(info.TargetType == targetType && info.EventID == eventID, "Invalid arguments.");

                    list.RemoveAt(n);
                    if (list.Count <= 0)
                    {
                        _eventIdDistributeCallInfos.Remove(eventID);
                    }

                    return;
                }
            }

            Debugger.Warn(LogGroupTag.Controller, "Could not found any event distribute call ‘{%s}’ with target ID '{%d}' and class type '{%t}', removed it failed.",
                    fullname, eventID, targetType);
        }

        /// <summary>
        /// 从当前消息事件管理容器中移除指定类型的分发函数信息
        /// </summary>
        /// <param name="fullname">完整名称</param>
        /// <param name="targetType">目标对象类型</param>
        /// <param name="eventDataType">事件数据类型</param>
        private void RemoveEventDistributeCallInfo(string fullname, SystemType targetType, SystemType eventDataType)
        {
            Debugger.Info(LogGroupTag.Controller, "Remove event distribute call '{%s}' with target data type '{%t}' and class type '{%t}'.",
                    fullname, eventDataType, targetType);
            if (false == _eventDataDistributeCallInfos.ContainsKey(eventDataType))
            {
                Debugger.Warn(LogGroupTag.Controller, "Could not found any event distribute call '{%s}' with target data type '{%t}', removed it failed.",
                        fullname, eventDataType);
                return;
            }

            IList<EventCallMethodInfo> list = _eventDataDistributeCallInfos[eventDataType];
            for (int n = 0; n < list.Count; ++n)
            {
                EventCallMethodInfo info = list[n];
                if (info.Fullname.Equals(fullname))
                {
                    Debugger.Assert(info.TargetType == targetType && info.EventDataType == eventDataType, "Invalid arguments.");

                    list.RemoveAt(n);
                    if (list.Count <= 0)
                    {
                        _eventDataDistributeCallInfos.Remove(eventDataType);
                    }

                    return;
                }
            }

            Debugger.Warn(LogGroupTag.Controller, "Could not found any event distribute call '{%s}' with target data type '{%t}' and class type '{%t}', removed it failed.",
                    fullname, eventDataType, targetType);
        }

        /// <summary>
        /// 移除当前消息事件管理器中登记的所有分发函数回调句柄
        /// </summary>
        private void RemoveAllEventDistributeCalls()
        {
            _eventIdDistributeCallInfos.Clear();
            _eventDataDistributeCallInfos.Clear();
        }

        #region 实体对象事件订阅绑定的回调函数注册/注销相关的接口函数

        /// <summary>
        /// 新增指定的回调绑定函数到当前消息事件缓存管理容器中
        /// </summary>
        /// <param name="fullname">完整名称</param>
        /// <param name="targetType">目标对象类型</param>
        /// <param name="methodInfo">函数对象</param>
        /// <param name="eventID">事件标识</param>
        /// <param name="automatically">自动装载状态标识</param>
        internal void AddEventSubscribeBindingCallInfo(string fullname, SystemType targetType, SystemMethodInfo methodInfo, int eventID, bool automatically)
        {
            if (false == _eventSubscribeBindingCaches.TryGetValue(targetType, out IDictionary<string, EventCallMethodInfo> eventCallMethodInfos))
            {
                eventCallMethodInfos = new Dictionary<string, EventCallMethodInfo>();
                _eventSubscribeBindingCaches.Add(targetType, eventCallMethodInfos);
            }

            if (eventCallMethodInfos.TryGetValue(fullname, out EventCallMethodInfo eventCallMethodInfo))
            {
                return;
            }

            // 函数格式校验
            if (NovaEngine.Debugger.Instance.IsOnDebuggingVerificationActivated())
            {
                bool verificated = false;
                if (NovaEngine.Utility.Reflection.IsTypeOfExtension(methodInfo))
                {
                    verificated = Loader.Inspecting.CodeInspector.CheckFunctionFormatOfEventCallWithBeanExtensionType(methodInfo);
                }
                else
                {
                    verificated = Loader.Inspecting.CodeInspector.CheckFunctionFormatOfEventCall(methodInfo);
                }

                // 校验失败
                if (false == verificated)
                {
                    Debugger.Error(LogGroupTag.Controller, "目标对象类型‘{%t}’的‘{%s}’函数判定为非法格式的事件订阅绑定回调函数，添加回调绑定操作失败！", targetType, fullname);
                    return;
                }
            }

            Debugger.Info(LogGroupTag.Controller, "新增指定的消息事件‘{%d}’对应的事件订阅绑定回调函数，其响应接口函数来自于目标类型‘{%t}’的‘{%s}’函数。",
                    eventID, targetType, fullname);

            eventCallMethodInfo = new EventCallMethodInfo(fullname, targetType, methodInfo, eventID, automatically);
            eventCallMethodInfos.Add(fullname, eventCallMethodInfo);
        }

        /// <summary>
        /// 新增指定的回调绑定函数到当前消息事件缓存管理容器中
        /// </summary>
        /// <param name="fullname">完整名称</param>
        /// <param name="targetType">目标对象类型</param>
        /// <param name="methodInfo">函数对象</param>
        /// <param name="eventDataType">事件数据类型</param>
        /// <param name="automatically">自动装载状态标识</param>
        internal void AddEventSubscribeBindingCallInfo(string fullname, SystemType targetType, SystemMethodInfo methodInfo, SystemType eventDataType, bool automatically)
        {
            if (false == _eventSubscribeBindingCaches.TryGetValue(targetType, out IDictionary<string, EventCallMethodInfo> eventCallMethodInfos))
            {
                eventCallMethodInfos = new Dictionary<string, EventCallMethodInfo>();
                _eventSubscribeBindingCaches.Add(targetType, eventCallMethodInfos);
            }

            if (eventCallMethodInfos.TryGetValue(fullname, out EventCallMethodInfo eventCallMethodInfo))
            {
                return;
            }

            // 函数格式校验
            if (NovaEngine.Debugger.Instance.IsOnDebuggingVerificationActivated())
            {
                bool verificated = false;
                if (NovaEngine.Utility.Reflection.IsTypeOfExtension(methodInfo))
                {
                    verificated = Loader.Inspecting.CodeInspector.CheckFunctionFormatOfEventCallWithBeanExtensionType(methodInfo);
                }
                else
                {
                    verificated = Loader.Inspecting.CodeInspector.CheckFunctionFormatOfEventCall(methodInfo);
                }

                // 校验失败
                if (false == verificated)
                {
                    Debugger.Error(LogGroupTag.Controller, "目标对象类型‘{%t}’的‘{%s}’函数判定为非法格式的事件订阅绑定回调函数，添加回调绑定操作失败！", targetType, fullname);
                    return;
                }
            }

            Debugger.Info(LogGroupTag.Controller, "新增指定的事件类型‘{%t}’对应的事件订阅绑定回调函数，其响应接口函数来自于目标类型‘{%t}’的‘{%s}’函数。",
                    eventDataType, targetType, fullname);

            eventCallMethodInfo = new EventCallMethodInfo(fullname, targetType, methodInfo, eventDataType, automatically);
            eventCallMethodInfos.Add(fullname, eventCallMethodInfo);
        }

        /// <summary>
        /// 从当前消息事件缓存管理容器中移除指定标识的回调绑定函数
        /// </summary>
        /// <param name="fullname">完整名称</param>
        /// <param name="targetType">目标对象类型</param>
        internal void RemoveEventSubscribeBindingCallInfo(string fullname, SystemType targetType)
        {
            Debugger.Info(LogGroupTag.Controller, "移除指定的事件订阅绑定回调函数，其响应接口函数来自于目标类型‘{%t}’的‘{%s}’函数。", targetType, fullname);

            if (_eventSubscribeBindingCaches.TryGetValue(targetType, out IDictionary<string, EventCallMethodInfo> eventCallMethodInfos))
            {
                if (eventCallMethodInfos.ContainsKey(fullname))
                {
                    eventCallMethodInfos.Remove(fullname);
                }

                if (eventCallMethodInfos.Count <= 0)
                {
                    _eventSubscribeBindingCaches.Remove(targetType);
                }
            }
        }

        /// <summary>
        /// 移除当前消息事件缓存管理容器中登记的所有回调绑定函数
        /// </summary>
        private void RemoveAllEventSubscribeBindingCalls()
        {
            _eventSubscribeBindingCaches.Clear();
        }

        /// <summary>
        /// 针对消息事件调用指定的回调绑定函数
        /// </summary>
        /// <param name="fullname">完整名称</param>
        /// <param name="targetType">目标对象类型</param>
        /// <param name="targetObject">对象实例</param>
        /// <param name="eventID">事件标识</param>
        /// <param name="args">事件参数</param>
        internal void InvokeEventSubscribeBindingCall(string fullname, SystemType targetType, IBean targetObject, int eventID, params object[] args)
        {
            EventCallMethodInfo eventCallMethodInfo = FindEventSubscribeBindingCallByName(fullname, targetType);
            if (null == eventCallMethodInfo)
            {
                Debugger.Warn(LogGroupTag.Controller, "当前的消息事件缓存管理容器中无法检索到指定类型‘{%t}’及名称‘{%s}’对应的回调绑定函数，此次消息事件‘{%d}’转发通知失败！", targetType, fullname, eventID);
                return;
            }

            eventCallMethodInfo.Invoke(targetObject, eventID, args);
        }

        /// <summary>
        /// 针对输入数据调用指定的回调绑定函数
        /// </summary>
        /// <param name="fullname">完整名称</param>
        /// <param name="targetType">目标对象类型</param>
        /// <param name="targetObject">对象实例</param>
        /// <param name="eventData">事件数据</param>
        internal void InvokeEventSubscribeBindingCall(string fullname, SystemType targetType, IBean targetObject, object eventData)
        {
            EventCallMethodInfo eventCallMethodInfo = FindEventSubscribeBindingCallByName(fullname, targetType);
            if (null == eventCallMethodInfo)
            {
                Debugger.Warn(LogGroupTag.Controller, "当前的消息事件缓存管理容器中无法检索到指定类型‘{%t}’及名称‘{%s}’对应的回调绑定函数，此次事件数据‘{%t}’转发通知失败！", targetType, fullname, eventData);
                return;
            }

            eventCallMethodInfo.Invoke(targetObject, eventData);
        }

        /// <summary>
        /// 通过指定的名称及对象类型，在当前的缓存容器中查找对应的回调绑定函数
        /// </summary>
        /// <param name="fullname">完整名称</param>
        /// <param name="targetType">目标对象类型</param>
        /// <returns>返回绑定函数实例</returns>
        private EventCallMethodInfo FindEventSubscribeBindingCallByName(string fullname, SystemType targetType)
        {
            if (_eventSubscribeBindingCaches.TryGetValue(targetType, out IDictionary<string, EventCallMethodInfo> eventCallMethodInfos))
            {
                if (eventCallMethodInfos.TryGetValue(fullname, out EventCallMethodInfo eventCallMethodInfo))
                {
                    return eventCallMethodInfo;
                }
            }

            return null;
        }

        /// <summary>
        /// 检测指定回调绑定函数是否是以自动绑定的方式注册的
        /// </summary>
        /// <param name="fullname">完整名称</param>
        /// <param name="targetType">目标对象类型</param>
        /// <returns>若函数以自动方式进行绑定则返回true，否则返回false</returns>
        internal bool IsEventSubscribeBindingForAutomatically(string fullname, SystemType targetType)
        {
            EventCallMethodInfo eventCallMethodInfo = FindEventSubscribeBindingCallByName(fullname, targetType);
            if (null == eventCallMethodInfo)
                return false;

            return eventCallMethodInfo.Automatically;
        }

        #endregion
    }
}
