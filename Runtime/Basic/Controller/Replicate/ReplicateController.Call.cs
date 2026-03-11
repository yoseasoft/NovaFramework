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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine.Scripting;

namespace GameEngine
{
    /// 数据同步管理对象类
    internal sealed partial class ReplicateController
    {
        /// <summary>
        /// 数据标签分发调度接口的数据结构容器
        /// </summary>
        private NovaEngine.MultiwayTree<int, IDictionary<string, ReplicateCallMethodInfo>> _replicateTagDistributeCallInfos;

        /// <summary>
        /// 同步分发回调管理模块的初始化函数
        /// </summary>
        [Preserve]
        [OnControllerSubmoduleInitCallback]
        private void InitializeForReplicateCall()
        {
            // 数据标签分发容器初始化
            _replicateTagDistributeCallInfos = new NovaEngine.MultiwayTree<int, IDictionary<string, ReplicateCallMethodInfo>>();
        }

        /// <summary>
        /// 同步分发回调管理模块的清理函数
        /// </summary>
        [Preserve]
        [OnControllerSubmoduleCleanupCallback]
        private void CleanupForReplicateCall()
        {
            // 移除所有同步分发回调句柄
            RemoveAllReplicateDistributeCalls();

            // 数据标签分发容器清理
            _replicateTagDistributeCallInfos.Clear();
            _replicateTagDistributeCallInfos = null;
        }

        /// <summary>
        /// 同步分发回调管理模块的重载函数
        /// </summary>
        [Preserve]
        [OnControllerSubmoduleReloadCallback]
        private void ReloadForReplicateCall()
        {
        }

        /// <summary>
        /// 针对数据标签进行同步分发的调度入口函数
        /// </summary>
        /// <param name="tags">数据标签</param>
        /// <param name="keys">标签解码列表</param>
        /// <param name="announceType">数据播报类型</param>
        private void OnReplicateDistributeCallDispatched(string tags, int[] keys, ReplicateAnnounceType announceType)
        {
            if (_replicateTagDistributeCallInfos.TryGetValue(keys, out IDictionary<string, ReplicateCallMethodInfo> infos))
            {
                IEnumerator<KeyValuePair<string, ReplicateCallMethodInfo>> e_info = infos.GetEnumerator();
                while (e_info.MoveNext())
                {
                    ReplicateCallMethodInfo info = e_info.Current.Value;
                    if (null == info.TargetType)
                    {
                        info.Invoke(tags, announceType);
                    }
                    else
                    {
                        IReadOnlyList<IBean> beans = BeanController.Instance.FindAllBeans(info.TargetType);
                        if (null != beans)
                        {
                            IEnumerator<IBean> e_bean = beans.GetEnumerator();
                            while (e_bean.MoveNext())
                            {
                                IBean bean = e_bean.Current;
                                info.Invoke(bean, tags, announceType);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 新增指定的分发函数到当前同步调度管理容器中
        /// </summary>
        /// <param name="fullname">完整名称</param>
        /// <param name="targetType">目标对象类型</param>
        /// <param name="methodInfo">函数对象</param>
        /// <param name="tags">数据标签</param>
        /// <param name="announceType">数据播报类型</param>
        private void AddReplicateDistributeCallInfo(string fullname, Type targetType, MethodInfo methodInfo, string tags, ReplicateAnnounceType announceType)
        {
            int[] keys = ReplicateTagToCodeArray(tags);
            if (false == _replicateTagDistributeCallInfos.TryGetValue(keys, out IDictionary<string, ReplicateCallMethodInfo> infos))
            {
                infos = new Dictionary<string, ReplicateCallMethodInfo>();
                _replicateTagDistributeCallInfos.Add(keys, infos);
            }

            if (infos.TryGetValue(fullname, out ReplicateCallMethodInfo info))
            {
                Debugger.Info(LogGroupTag.Controller, "Update replicate distribute call '{%s}' to target tags '{%s}' and announce type '{%i}' of the class type '{%t}'.",
                    fullname, tags, announceType, targetType);
                info.RegisterAnnounceType(tags, announceType);
                return;
            }

            info = new ReplicateCallMethodInfo(fullname, targetType, methodInfo, tags, announceType);

            Debugger.Info(LogGroupTag.Controller, "Add new replicate distribute call '{%s}' to target tags '{%s}' and announce type '{%i}' of the class type '{%t}'.",
                    fullname, tags, announceType, targetType);

            infos.Add(fullname, info);
        }

        /// <summary>
        /// 从当前同步调度管理容器中移除指定标识的分发函数信息
        /// </summary>
        /// <param name="fullname">完整名称</param>
        /// <param name="targetType">目标对象类型</param>
        /// <param name="tags">数据标签</param>
        private void RemoveReplicateDistributeCallInfo(string fullname, Type targetType, string tags)
        {
            Debugger.Info(LogGroupTag.Controller, "Remove replicate distribute call '{%s}' with target tags '{%s}' and class type '{%t}'.",
                    fullname, tags, targetType);
            int[] keys = ReplicateTagToCodeArray(tags);
            if (false == _replicateTagDistributeCallInfos.TryGetValue(keys, out IDictionary<string, ReplicateCallMethodInfo> infos))
            {
                Debugger.Warn(LogGroupTag.Controller, "Could not found any replicate distribute call '{%s}' with target tags '{%s}', removed it failed.", fullname, tags);
                return;
            }

            infos.Remove(fullname);
            if (infos.Count <= 0)
            {
                _replicateTagDistributeCallInfos.Remove(keys);
            }
        }

        /// <summary>
        /// 移除当前同步调度管理器中登记的所有分发函数回调句柄
        /// </summary>
        private void RemoveAllReplicateDistributeCalls()
        {
            _replicateTagDistributeCallInfos.Clear();
        }
    }
}
