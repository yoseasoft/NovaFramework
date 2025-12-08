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
using System.Reflection;

namespace GameEngine
{
    /// 事件管理对象类
    internal partial class EventController
    {
        /// <summary>
        /// 事件标识分发调度接口的数据结构容器
        /// </summary>
        private IDictionary<int, IList<EventCallMethodInfo>> _eventIdDistributeCallInfos;
        /// <summary>
        /// 事件数据分发调度接口的数据结构容器
        /// </summary>
        private IDictionary<Type, IList<EventCallMethodInfo>> _eventDataDistributeCallInfos;

        /// <summary>
        /// 事件分发回调管理模块的初始化函数
        /// </summary>
        [OnControllerSubmoduleInitCallback]
        private void InitializeForEventCall()
        {
            // 事件标识分发容器初始化
            _eventIdDistributeCallInfos = new Dictionary<int, IList<EventCallMethodInfo>>();
            // 事件数据分发容器初始化
            _eventDataDistributeCallInfos = new Dictionary<Type, IList<EventCallMethodInfo>>();
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
        }

        /// <summary>
        /// 事件分发回调管理模块的重载函数
        /// </summary>
        [OnControllerSubmoduleReloadCallback]
        private void ReloadForEventCall()
        {
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
            Type eventDataType = eventData.GetType();

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
        private void AddEventDistributeCallInfo(string fullname, Type targetType, MethodInfo methodInfo, int eventID)
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
        private void AddEventDistributeCallInfo(string fullname, Type targetType, MethodInfo methodInfo, Type eventDataType)
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
        private void RemoveEventDistributeCallInfo(string fullname, Type targetType, int eventID)
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
                    Debugger.Assert(info.TargetType == targetType && info.EventID == eventID, NovaEngine.ErrorText.InvalidArguments);

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
        private void RemoveEventDistributeCallInfo(string fullname, Type targetType, Type eventDataType)
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
                    Debugger.Assert(info.TargetType == targetType && info.EventDataType == eventDataType, NovaEngine.ErrorText.InvalidArguments);

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
    }
}
