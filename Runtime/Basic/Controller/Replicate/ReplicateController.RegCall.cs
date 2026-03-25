/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyright (C) 2024 - 2025, Hurley, Independent Studio.
/// Copyright (C) 2025 - 2026, Hainan Yuanyou Information Technology Co., Ltd. Guangzhou Branch
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
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine.Scripting;

namespace GameEngine
{
    /// 数据同步管理对象类
    internal sealed partial class ReplicateController
    {
        /// <summary>
        /// 同步传输回调绑定接口的缓存容器
        /// </summary>
        private IDictionary<Type, IDictionary<string, ReplicateCallMethodInfo>> _replicateCommunicateBindingCaches;

        /// <summary>
        /// 同步传输绑定接口初始化回调函数
        /// </summary>
        [Preserve]
        [OnControllerSubmoduleInitCallback]
        private void OnReplicateCommunicateBindingInitialize()
        {
            // 初始化回调绑定缓存容器
            _replicateCommunicateBindingCaches = new Dictionary<Type, IDictionary<string, ReplicateCallMethodInfo>>();
        }

        /// <summary>
        /// 同步传输绑定接口清理回调函数
        /// </summary>
        [Preserve]
        [OnControllerSubmoduleCleanupCallback]
        private void OnReplicateCommunicateBindingCleanup()
        {
            // 清理回调绑定缓存容器
            _replicateCommunicateBindingCaches.Clear();
            _replicateCommunicateBindingCaches = null;
        }

        /// <summary>
        /// 同步传输绑定接口重载回调函数
        /// </summary>
        [Preserve]
        [OnControllerSubmoduleReloadCallback]
        private void OnReplicateCommunicateBindingReload()
        {
            // 移除全部同步传输回调函数
            RemoveAllReplicateCommunicateBindingCalls();
        }

        #region 实体对象同步传输绑定的回调函数注册/注销相关的接口函数

        /// <summary>
        /// 新增指定的回调绑定函数到当前同步传输缓存管理容器中
        /// </summary>
        /// <param name="fullname">完整名称</param>
        /// <param name="targetType">目标对象类型</param>
        /// <param name="methodInfo">函数对象</param>
        /// <param name="tags">数据标签</param>
        /// <param name="announceType">数据播报类型</param>
        /// <param name="automatically">自动装载状态标识</param>
        internal void AddReplicateCommunicateBindingCallInfo(string fullname, Type targetType, MethodInfo methodInfo, string tags, ReplicateAnnounceType announceType, bool automatically)
        {
            if (false == _replicateCommunicateBindingCaches.TryGetValue(targetType, out IDictionary<string, ReplicateCallMethodInfo> replicateCallMethodInfos))
            {
                replicateCallMethodInfos = new Dictionary<string, ReplicateCallMethodInfo>();
                _replicateCommunicateBindingCaches.Add(targetType, replicateCallMethodInfos);
            }

            if (replicateCallMethodInfos.ContainsKey(fullname))
            {
                return;
            }

            Debugger.Info(LogGroupTag.Controller, "新增指定的数据标签‘{%s}’及播报类型‘{%v}’对应的同步传输绑定回调函数，其通讯接口函数来自于目标类型‘{%t}’的‘{%s}’函数。",
                    tags, announceType, targetType, fullname);

            ReplicateCallMethodInfo inputCallMethodInfo = new ReplicateCallMethodInfo(fullname, targetType, methodInfo, tags, announceType, automatically);
            replicateCallMethodInfos.Add(fullname, inputCallMethodInfo);
        }

        /// <summary>
        /// 从当前同步传输缓存管理容器中移除指定标识的回调绑定函数
        /// </summary>
        /// <param name="fullname">完整名称</param>
        /// <param name="targetType">目标对象类型</param>
        internal void RemoveReplicateCommunicateBindingCallInfo(string fullname, Type targetType)
        {
            Debugger.Info(LogGroupTag.Controller, "移除指定的同步传输绑定回调函数，其通讯接口函数来自于目标类型‘{%t}’的‘{%s}’函数。", targetType, fullname);

            if (_replicateCommunicateBindingCaches.TryGetValue(targetType, out IDictionary<string, ReplicateCallMethodInfo> replicateCallMethodInfos))
            {
                if (replicateCallMethodInfos.ContainsKey(fullname))
                {
                    replicateCallMethodInfos.Remove(fullname);
                }

                if (replicateCallMethodInfos.Count <= 0)
                {
                    _replicateCommunicateBindingCaches.Remove(targetType);
                }
            }
        }

        /// <summary>
        /// 移除当前同步传输缓存管理容器中登记的所有回调绑定函数
        /// </summary>
        private void RemoveAllReplicateCommunicateBindingCalls()
        {
            _replicateCommunicateBindingCaches.Clear();
        }

        /// <summary>
        /// 针对数据标签调用指定的回调绑定函数
        /// </summary>
        /// <param name="targetObject">对象实例</param>
        /// <param name="fullname">完整名称</param>
        /// <param name="targetType">目标对象类型</param>
        /// <param name="tags">数据标签</param>
        /// <param name="announceType">数据播报类型</param>
        internal void InvokeReplicateCommunicateBindingCall(IBean targetObject, string fullname, Type targetType, string tags, ReplicateAnnounceType announceType)
        {
            ReplicateCallMethodInfo replicateCallMethodInfo = FindReplicateCommunicateBindingCallByName(fullname, targetType);
            if (null == replicateCallMethodInfo)
            {
                Debugger.Warn(LogGroupTag.Controller, "当前的同步传输缓存管理容器中无法检索到指定类型‘{%t}’及名称‘{%s}’对应的回调绑定函数，此次数据标签‘{%s}’转发通知失败！", targetType, fullname, tags);
                return;
            }

            replicateCallMethodInfo.Invoke(targetObject, tags, announceType);
        }

        /// <summary>
        /// 通过指定的名称及对象类型，在当前的缓存容器中查找对应的回调绑定函数
        /// </summary>
        /// <param name="fullname">完整名称</param>
        /// <param name="targetType">目标对象类型</param>
        /// <returns>返回绑定函数实例</returns>
        private ReplicateCallMethodInfo FindReplicateCommunicateBindingCallByName(string fullname, Type targetType)
        {
            if (_replicateCommunicateBindingCaches.TryGetValue(targetType, out IDictionary<string, ReplicateCallMethodInfo> replicateCallMethodInfos))
            {
                if (replicateCallMethodInfos.TryGetValue(fullname, out ReplicateCallMethodInfo replicateCallMethodInfo))
                {
                    return replicateCallMethodInfo;
                }
            }

            return null;
        }

        #endregion
    }
}
