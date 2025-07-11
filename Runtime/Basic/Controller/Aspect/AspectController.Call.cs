/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyright (C) 2023 - 2024, Guangzhou Shiyue Network Technology Co., Ltd.
/// Copyright (C) 2025, Hainan Yuanyou Information Tecdhnology Co., Ltd. Guangzhou Branch
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
using SystemEnum = System.Enum;
using SystemDelegate = System.Delegate;

using SystemAction_object = System.Action<object>;

namespace GameEngine
{
    /// <summary>
    /// 切面访问接口的控制器类，对整个程序所有切面访问函数进行统一的整合和管理
    /// </summary>
    public sealed partial class AspectController
    {
        /// <summary>
        /// 切面调用的数据信息类
        /// </summary>
        private class AspectCallInfo
        {
            /// <summary>
            /// 切面调用类的完整名称
            /// </summary>
            private string _fullname;
            /// <summary>
            /// 切面调用类的目标对象类型
            /// </summary>
            private SystemType _targetType;
            /// <summary>
            /// 切面调用类的目标函数名称
            /// </summary>
            private string _methodName;
            /// <summary>
            /// 切面调用类的接入访问方式
            /// </summary>
            private AspectAccessType _accessType;
            /// <summary>
            /// 切面调用类的回调函数句柄
            /// </summary>
            private SystemAction_object _callback;

            public string Fullname { get { return _fullname; } internal set { _fullname = value; } }
            public SystemType TargetType { get { return _targetType; } internal set { _targetType = value; } }
            public string MethodName { get { return _methodName; } internal set { _methodName = value; } }
            public AspectAccessType AccessType { get { return _accessType; } internal set { _accessType = value; } }
            public SystemAction_object Callback { get { return _callback; } internal set { _callback = value; } }
        }

        /// <summary>
        /// 通用类型切点回调的数据结构容器
        /// </summary>
        private IDictionary<SystemType, IList<AspectCallInfo>> _genericTypeCallInfos = null;

        /// <summary>
        /// 通用类型的回调句柄对应标识的映射容器
        /// </summary>
        private IDictionary<SystemType, IDictionary<AspectAccessType, bool>> _genericTypeCallStatus = null;
        /// <summary>
        /// 通用类型的回调句柄管理容器
        /// </summary>
        private IDictionary<SystemType, IDictionary<AspectAccessType, IDictionary<string, SystemAction_object>>> _genericTypeCachedCallbacks = null;

        /// <summary>
        /// 切面回调相关内容的初始化回调函数
        /// </summary>
        [OnControllerSubmoduleInitCallback]
        private void InitAspectCallInfos()
        {
            // 数据容器初始化
            _genericTypeCallInfos = new Dictionary<SystemType, IList<AspectCallInfo>>();
            _genericTypeCallStatus = new Dictionary<SystemType, IDictionary<AspectAccessType, bool>>();
            _genericTypeCachedCallbacks = new Dictionary<SystemType, IDictionary<AspectAccessType, IDictionary<string, SystemAction_object>>>();
        }

        /// <summary>
        /// 切面回调相关内容的清理回调函数
        /// </summary>
        [OnControllerSubmoduleCleanupCallback]
        private void CleanupAspectCallInfos()
        {
            RemoveAllAspectCalls();

            // 移除数据容器
            _genericTypeCallInfos = null;
            _genericTypeCallStatus = null;
            _genericTypeCachedCallbacks = null;
        }

        /// <summary>
        /// 切面行为对指定函数接口的扩展回调处理函数
        /// </summary>
        /// <param name="obj">目标对象</param>
        /// <param name="methodName">函数名称</param>
        private void CallExtend(object obj, string methodName)
        {
            SystemType targetType = obj.GetType();

            if (TryGetAspectCallback(targetType, methodName, AspectAccessType.Extend, out SystemAction_object callback))
            {
                callback.Invoke(obj);
            }
        }

        /// <summary>
        /// 切面行为对指定函数接口的前置回调处理函数
        /// </summary>
        /// <param name="obj">目标对象</param>
        /// <param name="methodName">函数名称</param>
        private void CallBefore(object obj, string methodName)
        {
            SystemType targetType = obj.GetType();

            SystemAction_object callback = null;
            if (TryGetAspectCallback(targetType, methodName, AspectAccessType.Before, out callback))
            {
                callback.Invoke(obj);
            }
        }

        /// <summary>
        /// 切面行为对指定函数接口的后置回调处理函数
        /// </summary>
        /// <param name="obj">目标对象</param>
        /// <param name="methodName">函数名称</param>
        /// <param name="isException">异常状态标识</param>
        private void CallAfter(object obj, string methodName, bool isException)
        {
            SystemType targetType = obj.GetType();

            SystemAction_object callback = null;
            if (TryGetAspectCallback(targetType, methodName, AspectAccessType.After, out callback))
            {
                callback.Invoke(obj);
            }

            // 异常检测
            if (isException)
            {
                if (TryGetAspectCallback(targetType, methodName, AspectAccessType.AfterThrowing, out callback))
                {
                    callback.Invoke(obj);
                }
            }
            else
            {
                if (TryGetAspectCallback(targetType, methodName, AspectAccessType.AfterReturning, out callback))
                {
                    callback.Invoke(obj);
                }

                // 正常调用后，启动切面服务处理逻辑的调度
                AspectCallService.CallServiceProcess(obj, methodName);
            }
        }

        /// <summary>
        /// 切面行为对指定函数接口的环绕回调处理函数
        /// </summary>
        /// <param name="obj">目标对象</param>
        /// <param name="methodName">函数名称</param>
        /// <returns>若存在可执行的环绕回调函数则返回true，否则返回false</returns>
        private bool CallAround(object obj, string methodName)
        {
            SystemType targetType = obj.GetType();

            if (TryGetAspectCallback(targetType, methodName, AspectAccessType.Around, out SystemAction_object callback))
            {
                callback.Invoke(obj);

                return true;
            }

            return false;
        }

        /// <summary>
        /// 新增指定的切入函数到当前控制管理容器中
        /// </summary>
        /// <param name="fullname">完整名称</param>
        /// <param name="targetType">目标对象类型</param>
        /// <param name="methodName">函数名称</param>
        /// <param name="accessType">函数访问类型</param>
        /// <param name="callback">函数回调句柄</param>
        private void AddAspectCallForGenericType(string fullname, SystemType targetType, string methodName, AspectAccessType accessType, SystemAction_object callback)
        {
            AspectCallInfo info = new AspectCallInfo();
            info.Fullname = fullname;
            info.TargetType = targetType;
            info.MethodName = methodName;
            info.AccessType = accessType;
            info.Callback = callback;

            Debugger.Info(LogGroupTag.Controller, "Add new aspect call '{0}' to target method '{1}' of the class type '{2}'.",
                    fullname, methodName, NovaEngine.Utility.Text.ToString(targetType));

            if (false == _genericTypeCallInfos.TryGetValue(targetType, out IList<AspectCallInfo> callInfos))
            {
                callInfos = new List<AspectCallInfo>();
                _genericTypeCallInfos.Add(targetType, callInfos);
            }

            // 添加切面回调信息
            callInfos.Add(info);
        }

        /// <summary>
        /// 从当前控制管理容器中移除指定的切入函数
        /// </summary>
        /// <param name="fullname">完整名称</param>
        /// <param name="targetType">目标对象类型</param>
        /// <param name="methodName">函数名称</param>
        /// <param name="accessType">函数访问类型</param>
        private void RemoveAspectCallForGenericType(string fullname, SystemType targetType, string methodName, AspectAccessType accessType)
        {
            Debugger.Info(LogGroupTag.Controller, "Remove aspect call '{0}' with target method '{1}' and class type '{2}'.", fullname, methodName, NovaEngine.Utility.Text.ToString(targetType));
            if (false == _genericTypeCallInfos.ContainsKey(targetType))
            {
                Debugger.Warn(LogGroupTag.Controller, "Could not found any apsect call '{0}' with target class type '{1}', removed it failed.", fullname, NovaEngine.Utility.Text.ToString(targetType));
                return;
            }

            IList<AspectCallInfo> list = _genericTypeCallInfos[targetType];
            for (int n = 0; n < list.Count; ++n)
            {
                AspectCallInfo info = list[n];
                if (info.Fullname.Equals(fullname))
                {
                    Debugger.Assert(info.TargetType == targetType && info.MethodName.Equals(methodName) && info.AccessType == accessType, "Invalid arguments.");

                    list.RemoveAt(n);
                    if (list.Count <= 0)
                    {
                        _genericTypeCallInfos.Remove(targetType);
                    }

                    // 当切面函数发生变化时，清除所有的函数缓存队列
                    RemoveAllAspectCallCaches();

                    return;
                }
            }

            Debugger.Warn(LogGroupTag.Controller, "Could not found any apsect call '{0}' with target method '{1}' and class type '{2}', removed it failed.",
                    fullname, methodName, NovaEngine.Utility.Text.ToString(targetType));
        }

        /// <summary>
        /// 移除当前切面管理容器中的所有缓存回调句柄
        /// </summary>
        private void RemoveAllAspectCallCaches()
        {
            // 移除缓存
            _genericTypeCallStatus.Clear();
            _genericTypeCachedCallbacks.Clear();
        }

        /// <summary>
        /// 移除当前切面控制管理器中登记的所有切入函数回调句柄
        /// </summary>
        private void RemoveAllAspectCalls()
        {
            _genericTypeCallInfos.Clear();

            // 移除缓存
            RemoveAllAspectCallCaches();
        }

        /// <summary>
        /// 通过指定的对象类型获取其对应的缓存管理容器
        /// </summary>
        /// <param name="targetType">目标对象类型</param>
        /// <returns>返回给定类型对应的缓存容器，若类型错误则返回null</returns>
        private IDictionary<AspectAccessType, IDictionary<string, SystemAction_object>> GetGenericTypeCachedCallbackByType(SystemType targetType)
        {
            if (false == _genericTypeCachedCallbacks.TryGetValue(targetType, out IDictionary<AspectAccessType, IDictionary<string, SystemAction_object>> container))
            {
                container = new Dictionary<AspectAccessType, IDictionary<string, SystemAction_object>>();
                _genericTypeCachedCallbacks.Add(targetType, container);
            }

            return container;
        }

        /// <summary>
        /// 在当前切面控制管理容器中查找指定类型对应的切入回调函数
        /// </summary>
        /// <param name="targetType">目标对象类型</param>
        /// <param name="methodName">函数名称</param>
        /// <param name="accessType">函数访问类型</param>
        /// <param name="callback">回调句柄实例</param>
        /// <returns>若查找回调句柄成功返回true，否则返回false</returns>
        private bool TryGetAspectCallback(SystemType targetType,
                                          string methodName,
                                          AspectAccessType accessType,
                                          out SystemAction_object callback)
        {
            callback = null;

            bool result = false;
            if (false == TryGetCachedAspectCallStatus(targetType, accessType, out result))
            {
                result = TryCachedAspectCallbacks(targetType);
            }

            if (!result)
            {
                // 不存在任何回调句柄实例
                return result;
            }

            // 先将处理状态重置
            result = false;

            IDictionary<AspectAccessType, IDictionary<string, SystemAction_object>> container = GetGenericTypeCachedCallbackByType(targetType);
            if (null == container)
            {
                // 正常情况下缓存容器不可能为null
                Debugger.Warn("Could not found any cached container with target class type '{0}', aspect call service running failed.",
                        NovaEngine.Utility.Text.ToString(targetType));

                // 获取缓存容器失败
                return result;
            }

            if (false == container.TryGetValue(accessType, out IDictionary<string, SystemAction_object> callbacks))
            {
                return result;
            }

            // 根据函数名称获取对应回调句柄
            if (callbacks.TryGetValue(methodName, out callback))
            {
                result = true;
            }

            return result;
        }

        /// <summary>
        /// 将指定对象类型及函数名称的回调函数存放在目标容器中<br/>
        /// 若目标容器中已存在回调函数实例，则将当前回调和原有的回调函数实例进行合并后存放
        /// </summary>
        /// <param name="targetType">目标对象类型</param>
        /// <param name="methodName">函数名称</param>
        /// <param name="accessType">访问类型</param>
        /// <param name="callback">回调函数句柄</param>
        /// <param name="container">目标管理容器</param>
        private void CombineAspectCallbackByType(SystemType targetType,
                                                 string methodName,
                                                 AspectAccessType accessType,
                                                 SystemAction_object callback,
                                                 IDictionary<AspectAccessType, IDictionary<string, SystemAction_object>> container)
        {
            if (null == targetType || string.IsNullOrEmpty(methodName) || AspectAccessType.Unknown == accessType || null == callback || null == container)
            {
                Debugger.Error("Invalid arguments with aspect callback on combine.");
                return;
            }

            IDictionary<string, SystemAction_object> callbacks = null;

            if (false == container.TryGetValue(accessType, out callbacks))
            {
                callbacks = new Dictionary<string, SystemAction_object>();
                container.Add(accessType, callbacks);
            }

            if (callbacks.ContainsKey(methodName))
            {
                // 已有委托函数，则对原有函数进行合并操作
                callbacks[methodName] = (SystemAction_object) SystemDelegate.Combine(callbacks[methodName], callback);
            }
            else
            {
                // 添加新的委托函数
                callbacks.Add(methodName, callback);
            }
        }

        /// <summary>
        /// 重置指定对象类型所属下的全部访问类型回调状态，修改为FALSE值
        /// </summary>
        /// <param name="container">访问类型回调状态容器</param>
        private void ResetAspectCallStatusOnClass(IDictionary<AspectAccessType, bool> container)
        {
            foreach (AspectAccessType enumValue in SystemEnum.GetValues(typeof(AspectAccessType)))
            {
                if (AspectAccessType.Unknown == enumValue)
                {
                    continue;
                }

                if (container.ContainsKey(enumValue))
                {
                    Debugger.Warn("The aspect access type '{0}' was already exist within call status, repeat added it will be override old value.", enumValue.ToString());
                    container.Remove(enumValue);
                }

                container.Add(enumValue, false);
            }
        }

        /// <summary>
        /// 获取指定对象类型和访问类型下对应的回调状态
        /// </summary>
        /// <param name="targetType">目标对象类型</param>
        /// <param name="accessType">访问类型</param>
        /// <param name="status">回调状态标识</param>
        /// <returns>若容器中已记录对应的状态标识则返回true，否则返回false</returns>
        private bool TryGetCachedAspectCallStatus(SystemType targetType, AspectAccessType accessType, out bool status)
        {
            status = false;

            if (false == _genericTypeCallStatus.TryGetValue(targetType, out IDictionary<AspectAccessType, bool> container))
            {
                return false;
            }

            if (container.ContainsKey(accessType))
            {
                status = container[accessType];
            }

            return true;
        }

        /// <summary>
        /// 对指定对象类型的切面服务回调函数进行缓存处理
        /// </summary>
        /// <param name="targetType">目标对象类型</param>
        /// <returns>若缓存成功返回true，没有需要缓存的回调则返回false</returns>
        private bool TryCachedAspectCallbacks(SystemType targetType)
        {
            bool result = false;

            IList<AspectCallInfo> infos = null;

            SystemType currentClassType = targetType;
            while (null != currentClassType)
            {
                if (_genericTypeCallInfos.TryGetValue(currentClassType, out IList<AspectCallInfo> list))
                {
                    infos ??= new List<AspectCallInfo>();
                    // 此处不使用 AddRange，因为要把父类注册的接口放在子类之前调用
                    ((List<AspectCallInfo>) infos).InsertRange(0, list);
                }

                currentClassType = currentClassType.BaseType;
            }

            if (null != infos && infos.Count > 0)
            {
                // 存在切面委托服务回调
                result = true;

                if (false == _genericTypeCallStatus.TryGetValue(targetType, out IDictionary<AspectAccessType, bool> status))
                {
                    status = new Dictionary<AspectAccessType, bool>();
                    _genericTypeCallStatus.Add(targetType, status);
                }

                // 重置状态标识
                ResetAspectCallStatusOnClass(status);

                foreach (AspectCallInfo info in infos)
                {
                    IDictionary<AspectAccessType, IDictionary<string, SystemAction_object>> container = null;
                    container = GetGenericTypeCachedCallbackByType(targetType);

                    CombineAspectCallbackByType(targetType, info.MethodName, info.AccessType, info.Callback, container);

                    // 指定的访问类型存在回调函数，则将其状态设置为TRUE
                    status[info.AccessType] = true;
                }
            }

            return result;
        }
    }
}
