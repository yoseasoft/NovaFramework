/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyright (C) 2023 - 2024, Guangzhou Shiyue Network Technology Co., Ltd.
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
        /// 事件订阅回调绑定接口的缓存容器
        /// </summary>
        private IDictionary<SystemType, IDictionary<string, EventCallMethodInfo>> _eventSubscribeBindingCaches;

        /// <summary>
        /// 事件订阅绑定管理模块的初始化函数
        /// </summary>
        [OnControllerSubmoduleInitCallback]
        private void InitializeForEventSubscribeBinding()
        {
            // 初始化回调绑定缓存容器
            _eventSubscribeBindingCaches = new Dictionary<SystemType, IDictionary<string, EventCallMethodInfo>>();
        }

        /// <summary>
        /// 事件订阅绑定管理模块的清理函数
        /// </summary>
        [OnControllerSubmoduleCleanupCallback]
        private void CleanupForEventSubscribeBinding()
        {
            // 移除所有事件订阅绑定缓存函数
            RemoveAllEventSubscribeBindingCalls();

            // 清理回调绑定缓存容器
            _eventSubscribeBindingCaches.Clear();
            _eventSubscribeBindingCaches = null;
        }

        /// <summary>
        /// 事件订阅绑定管理模块的重载函数
        /// </summary>
        [OnControllerSubmoduleReloadCallback]
        private void ReloadForEventSubscribeBinding()
        {
            // 移除所有事件订阅绑定缓存函数
            RemoveAllEventSubscribeBindingCalls();
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

            if (eventCallMethodInfos.ContainsKey(fullname))
            {
                return;
            }

            Debugger.Info(LogGroupTag.Controller, "新增指定的消息事件‘{%d}’对应的事件订阅绑定回调函数，其响应接口函数来自于目标类型‘{%t}’的‘{%s}’函数。",
                    eventID, targetType, fullname);

            EventCallMethodInfo eventCallMethodInfo = new EventCallMethodInfo(fullname, targetType, methodInfo, eventID, automatically);
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

            if (eventCallMethodInfos.ContainsKey(fullname))
            {
                return;
            }

            Debugger.Info(LogGroupTag.Controller, "新增指定的事件类型‘{%t}’对应的事件订阅绑定回调函数，其响应接口函数来自于目标类型‘{%t}’的‘{%s}’函数。",
                    eventDataType, targetType, fullname);

            EventCallMethodInfo eventCallMethodInfo = new EventCallMethodInfo(fullname, targetType, methodInfo, eventDataType, automatically);
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
        /// <param name="targetObject">对象实例</param>
        /// <param name="fullname">完整名称</param>
        /// <param name="targetType">目标对象类型</param>
        /// <param name="eventID">事件标识</param>
        /// <param name="args">事件参数</param>
        internal void InvokeEventSubscribeBindingCall(IBean targetObject, string fullname, SystemType targetType, int eventID, params object[] args)
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
        /// <param name="targetObject">对象实例</param>
        /// <param name="fullname">完整名称</param>
        /// <param name="targetType">目标对象类型</param>
        /// <param name="eventData">事件数据</param>
        internal void InvokeEventSubscribeBindingCall(IBean targetObject, string fullname, SystemType targetType, object eventData)
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
