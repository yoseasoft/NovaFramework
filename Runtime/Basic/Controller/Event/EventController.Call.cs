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
using System.Reflection;

using SystemType = System.Type;
using SystemAttribute = System.Attribute;
using SystemAttributeUsageAttribute = System.AttributeUsageAttribute;
using SystemAttributeTargets = System.AttributeTargets;
using SystemAction = System.Action;
using SystemDelegate = System.Delegate;

namespace GameEngine
{
    /// <summary>
    /// 事件管理对象类，用于对场景上下文中的所有节点对象进行事件管理及分发
    /// </summary>
    internal partial class EventController
    {
        /// <summary>
        /// 通过事件标识分发调度接口的数据结构容器
        /// </summary>
        private IDictionary<int, IList<EventCallInfo>> _eventIdDistributeCallInfos = null;
        /// <summary>
        /// 通过事件数据分发调度接口的数据结构容器
        /// </summary>
        private IDictionary<SystemType, IList<EventCallInfo>> _eventDataDistributeCallInfos = null;

        /// <summary>
        /// 事件分发回调管理模块的初始化函数
        /// </summary>
        [OnControllerSubmoduleInitCallback]
        private void InitializeForEventCall()
        {
            // 事件标识分发容器初始化
            _eventIdDistributeCallInfos = new Dictionary<int, IList<EventCallInfo>>();
            // 事件数据分发容器初始化
            _eventDataDistributeCallInfos = new Dictionary<SystemType, IList<EventCallInfo>>();
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
        /// 针对事件标识进行事件分发的调度入口函数
        /// </summary>
        /// <param name="eventID">事件标识</param>
        /// <param name="args">事件参数列表</param>
        private void OnEventDistributeCallDispatched(int eventID, params object[] args)
        {
            IList<EventCallInfo> list = null;
            if (_eventIdDistributeCallInfos.TryGetValue(eventID, out list))
            {
                IEnumerator<EventCallInfo> e_info = list.GetEnumerator();
                while (e_info.MoveNext())
                {
                    EventCallInfo info = e_info.Current;
                    if (null == info.TargetType)
                    {
                        info.Invoke(eventID, args);
                    }
                    else
                    {
                        IList<IProto> protos = ProtoController.Instance.FindAllProtos(info.TargetType);
                        if (null != protos)
                        {
                            IEnumerator<IProto> e_proto = protos.GetEnumerator();
                            while (e_proto.MoveNext())
                            {
                                IProto proto = e_proto.Current;
                                info.Invoke(proto, eventID, args);
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

            IList<EventCallInfo> list = null;
            if (_eventDataDistributeCallInfos.TryGetValue(eventDataType, out list))
            {
                IEnumerator<EventCallInfo> e_info = list.GetEnumerator();
                while (e_info.MoveNext())
                {
                    EventCallInfo info = e_info.Current;
                    if (null == info.TargetType)
                    {
                        info.Invoke(eventData);
                    }
                    else
                    {
                        IList<IProto> protos = ProtoController.Instance.FindAllProtos(info.TargetType);
                        if (null != protos)
                        {
                            IEnumerator<IProto> e_proto = protos.GetEnumerator();
                            while (e_proto.MoveNext())
                            {
                                IProto proto = e_proto.Current;
                                info.Invoke(proto, eventData);
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
        /// <param name="eventID">事件标识</param>
        /// <param name="callback">函数回调句柄</param>
        private void AddEventDistributeCallInfo(string fullname, SystemType targetType, int eventID, SystemDelegate callback)
        {
            EventCallInfo info = new EventCallInfo(fullname, targetType, eventID, callback);

            Debugger.Info(LogGroupTag.Controller, "Add new event distribute call '{0}' to target ID '{1}' of the class type '{2}'.",
                    fullname, eventID, NovaEngine.Utility.Text.ToString(targetType));
            if (_eventIdDistributeCallInfos.ContainsKey(eventID))
            {
                IList<EventCallInfo> list = _eventIdDistributeCallInfos[eventID];
                list.Add(info);
            }
            else
            {
                IList<EventCallInfo> list = new List<EventCallInfo>();
                list.Add(info);
                _eventIdDistributeCallInfos.Add(eventID, list);
            }
        }

        /// <summary>
        /// 新增指定的分发函数到当前消息事件管理容器中
        /// </summary>
        /// <param name="fullname">完整名称</param>
        /// <param name="targetType">目标对象类型</param>
        /// <param name="eventDataType">事件数据类型</param>
        /// <param name="callback">函数回调句柄</param>
        private void AddEventDistributeCallInfo(string fullname, SystemType targetType, SystemType eventDataType, SystemDelegate callback)
        {
            EventCallInfo info = new EventCallInfo(fullname, targetType, eventDataType, callback);

            Debugger.Info(LogGroupTag.Controller, "Add new event distribute call '{0}' to target data type '{1}' of the class type '{2}'.",
                    fullname, NovaEngine.Utility.Text.ToString(eventDataType), NovaEngine.Utility.Text.ToString(targetType));
            if (_eventDataDistributeCallInfos.ContainsKey(eventDataType))
            {
                IList<EventCallInfo> list = _eventDataDistributeCallInfos[eventDataType];
                list.Add(info);
            }
            else
            {
                IList<EventCallInfo> list = new List<EventCallInfo>();
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
            Debugger.Info(LogGroupTag.Controller, "Remove event distribute call '{0}' with target ID '{1}' and class type '{2}'.",
                    fullname, eventID, NovaEngine.Utility.Text.ToString(targetType));
            if (false == _eventIdDistributeCallInfos.ContainsKey(eventID))
            {
                Debugger.Warn(LogGroupTag.Controller, "Could not found any event distribute call '{0}' with target ID '{1}', removed it failed.", fullname, eventID);
                return;
            }

            IList<EventCallInfo> list = _eventIdDistributeCallInfos[eventID];
            for (int n = 0; n < list.Count; ++n)
            {
                EventCallInfo info = list[n];
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

            Debugger.Warn(LogGroupTag.Controller, "Could not found any event distribute call ‘{0}’ with target ID '{1}' and class type '{2}', removed it failed.",
                    fullname, eventID, NovaEngine.Utility.Text.ToString(targetType));
        }

        /// <summary>
        /// 从当前消息事件管理容器中移除指定类型的分发函数信息
        /// </summary>
        /// <param name="fullname">完整名称</param>
        /// <param name="targetType">目标对象类型</param>
        /// <param name="eventDataType">事件数据类型</param>
        private void RemoveEventDistributeCallInfo(string fullname, SystemType targetType, SystemType eventDataType)
        {
            Debugger.Info(LogGroupTag.Controller, "Remove event distribute call '{0}' with target data type '{1}' and class type '{2}'.",
                    fullname, NovaEngine.Utility.Text.ToString(eventDataType), NovaEngine.Utility.Text.ToString(targetType));
            if (false == _eventDataDistributeCallInfos.ContainsKey(eventDataType))
            {
                Debugger.Warn(LogGroupTag.Controller, "Could not found any event distribute call '{0}' with target data type '{1}', removed it failed.",
                        fullname, NovaEngine.Utility.Text.ToString(eventDataType));
                return;
            }

            IList<EventCallInfo> list = _eventDataDistributeCallInfos[eventDataType];
            for (int n = 0; n < list.Count; ++n)
            {
                EventCallInfo info = list[n];
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

            Debugger.Warn(LogGroupTag.Controller, "Could not found any event distribute call '{0}' with target data type '{1}' and class type '{2}', removed it failed.",
                    fullname, NovaEngine.Utility.Text.ToString(eventDataType), NovaEngine.Utility.Text.ToString(targetType));
        }

        /// <summary>
        /// 移除当前消息事件管理器中登记的所有分发函数回调句柄
        /// </summary>
        private void RemoveAllEventDistributeCalls()
        {
            _eventIdDistributeCallInfos.Clear();
            _eventDataDistributeCallInfos.Clear();
        }

        #region 事件调度的数据信息结构对象声明

        /// <summary>
        /// 事件调度的数据信息类
        /// </summary>
        private class EventCallInfo
        {
            /// <summary>
            /// 事件调度类的完整名称
            /// </summary>
            private string _fullname;
            /// <summary>
            /// 事件调度类的目标对象类型
            /// </summary>
            private SystemType _targetType;
            /// <summary>
            /// 事件调度类的事件标识
            /// </summary>
            private int _eventID;
            /// <summary>
            /// 事件调用类的事件数据类型
            /// </summary>
            private SystemType _eventDataType;
            /// <summary>
            /// 事件调度类的回调句柄
            /// </summary>
            private SystemDelegate _callback;
            /// <summary>
            /// 事件调度回调函数的无参状态标识
            /// </summary>
            private bool _isNullParameterType;

            public string Fullname => _fullname;
            public SystemType TargetType => _targetType;
            public int EventID => _eventID;
            public SystemType EventDataType => _eventDataType;
            public SystemDelegate Callback => _callback;
            public bool IsNullParameterType => _isNullParameterType;

            public EventCallInfo(string fullname, SystemType targetType, int eventID, SystemDelegate callback)
                : this(fullname, targetType, eventID, null, callback)
            {
            }

            public EventCallInfo(string fullname, SystemType targetType, SystemType eventDataType, SystemDelegate callback)
                : this(fullname, targetType, 0, eventDataType, callback)
            {
            }

            private EventCallInfo(string fullname, SystemType targetType, int eventID, SystemType eventDataType, SystemDelegate callback)
            {
                Debugger.Assert(null != callback, "Invalid arguments.");

                _fullname = fullname;
                _targetType = targetType;
                _eventID = eventID;
                _eventDataType = eventDataType;
                _callback = callback;
                _isNullParameterType = Loader.Inspecting.CodeInspector.IsNullParameterTypeOfEventCallFunction(callback.Method);
            }

            /// <summary>
            /// 基于事件标识的事件回调转发函数
            /// </summary>
            /// <param name="eventID">事件标识</param>
            /// <param name="args">事件参数列表</param>
            public void Invoke(int eventID, params object[] args)
            {
                if (_isNullParameterType)
                {
                    _callback.DynamicInvoke();
                }
                else
                {
                    _callback.DynamicInvoke(eventID, args);
                }
            }

            /// <summary>
            /// 基于事件标识的事件回调转发函数
            /// </summary>
            /// <param name="proto">目标原型对象</param>
            /// <param name="eventID">事件标识</param>
            /// <param name="args">事件参数列表</param>
            public void Invoke(IProto proto, int eventID, params object[] args)
            {
                if (_isNullParameterType)
                {
                    _callback.DynamicInvoke(proto);
                }
                else
                {
                    _callback.DynamicInvoke(proto, eventID, args);
                }
            }

            /// <summary>
            /// 基于事件数据类型的事件回调转发函数
            /// </summary>
            /// <param name="eventData">事件数据</param>
            public void Invoke(object eventData)
            {
                if (_isNullParameterType)
                {
                    _callback.DynamicInvoke();
                }
                else
                {
                    _callback.DynamicInvoke(eventData);
                }
            }

            /// <summary>
            /// 基于事件数据类型的事件回调转发函数
            /// </summary>
            /// <param name="proto">目标原型对象</param>
            /// <param name="eventData">事件数据</param>
            public void Invoke(IProto proto, object eventData)
            {
                if (_isNullParameterType)
                {
                    _callback.DynamicInvoke(proto);
                }
                else
                {
                    _callback.DynamicInvoke(proto, eventData);
                }
            }
        }

        #endregion
    }
}
