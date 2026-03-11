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

namespace GameEngine
{
    /// <summary>
    /// 数据同步管理对象类，用于对场景实体对象中的所有数据成员进行自动同步处理
    /// </summary>
    internal sealed partial class ReplicateController : BaseController<ReplicateController>
    {
        /// <summary>
        /// 针对数据标签进行分发的监听对象管理列表容器
        /// </summary>
        private IDictionary<string, IList<IReplicateDispatch>> _replicateListenersForTag;

        /// <summary>
        /// 数据同步管理对象初始化通知接口函数
        /// </summary>
        protected override sealed void OnInitialize()
        {
            // 初始化监听列表
            _replicateListenersForTag = new Dictionary<string, IList<IReplicateDispatch>>();
        }

        /// <summary>
        /// 数据同步管理对象清理通知接口函数
        /// </summary>
        protected override sealed void OnCleanup()
        {
            // 清理监听列表
            _replicateListenersForTag.Clear();
            _replicateListenersForTag = null;
        }

        /// <summary>
        /// 数据同步管理对象刷新通知接口函数
        /// </summary>
        protected override sealed void OnUpdate()
        {
        }

        /// <summary>
        /// 数据同步管理对象后置刷新通知接口函数
        /// </summary>
        protected override sealed void OnLateUpdate()
        {
        }

        /// <summary>
        /// 数据同步管理对象重载调度函数接口
        /// </summary>
        protected override sealed void OnReload()
        {
        }

        /// <summary>
        /// 数据同步管理对象倾泻调度函数接口
        /// </summary>
        protected override sealed void OnDump()
        {
        }

        /// <summary>
        /// 发送指定的数据标签到分发调度中等待派发
        /// </summary>
        /// <param name="tags">数据标签</param>
        /// <param name="announceType">数据播报类型</param>
        public void Post(string tags, ReplicateAnnounceType announceType)
        {
            // 缓存数据标签
        }

        /// <summary>
        /// 立即推送指定的数据标签到分发调度中
        /// </summary>
        /// <param name="tags">数据标签</param>
        /// <param name="announceType">数据播报类型</param>
        public void Push(string tags, ReplicateAnnounceType announceType)
        {
            OnReplicateDispatched(tags, announceType);
        }

        /// <summary>
        /// 数据标签的同步派发调度接口函数
        /// </summary>
        /// <param name="tags">数据标签</param>
        /// <param name="announceType">数据播报类型</param>
        private void OnReplicateDispatched(string tags, ReplicateAnnounceType announceType)
        {
            int[] keys = ReplicateTagToCodeArray(tags);

            // 事件分发调度
            OnReplicateDistributeCallDispatched(tags, keys, announceType);

            if (_replicateListenersForTag.TryGetValue(tags, out IList<IReplicateDispatch> listeners))
            {
                // 2026-03-11:
                // 因为网络消息处理逻辑中存在删除对象对象的情况，
                // 考虑到该情况同样适用于同步系统，因此在此处做相同方式的处理
                // 通过临时列表来进行迭代
                IList<IReplicateDispatch> list;
                if (listeners.Count > 1)
                {
                    list = new List<IReplicateDispatch>(listeners);
                }
                else
                {
                    list = listeners;
                }

                for (int n = 0; n < list.Count; ++n)
                {
                    IReplicateDispatch listener = list[n];
                    listener.OnReplicateDispatchForTag(tags, announceType);
                }
            }
        }

        #region 同步回调句柄的传输绑定和撤销接口函数

        /// <summary>
        /// 同步分发对象的同步传输函数接口，指派一个指定的监听回调接口到目标数据标签
        /// </summary>
        /// <param name="tags">数据标签</param>
        /// <param name="listener">监听回调接口</param>
        /// <returns>若同步传输添加成功则返回true，否则返回false</returns>
        public bool AddReplicateCommunicate(string tags, IReplicateDispatch listener)
        {
            // Debugger.Info(LogGroupTag.Module, "新增目标对象类型‘{%t}’的同步转发监听回调接口到指定的数据标签‘{%s}’对应的同步传输管理容器中！", listener, tags);

            if (false == _replicateListenersForTag.TryGetValue(tags, out IList<IReplicateDispatch> list))
            {
                list = new List<IReplicateDispatch>() { listener };

                _replicateListenersForTag.Add(tags, list);
                return true;
            }

            // 检查是否重复添加
            if (list.Contains(listener))
            {
                Debugger.Warn("The listener for target replicate '{%s}' was already added, cannot repeat do it.", tags);
                return false;
            }

            list.Add(listener);

            return true;
        }

        /// <summary>
        /// 取消指定数据标签的传输监听回调接口
        /// </summary>
        /// <param name="tags">数据标签</param>
        /// <param name="listener">监听回调接口</param>
        public void RemoveReplicateCommunicate(string tags, IReplicateDispatch listener)
        {
            if (false == _replicateListenersForTag.TryGetValue(tags, out IList<IReplicateDispatch> list))
            {
                Debugger.Warn("Could not found any listener for target replicate '{%s}' with on added, do removed it failed.", tags);
                return;
            }

            list.Remove(listener);
            // 列表为空则移除对应的输入监听列表实例
            if (list.Count == 0)
            {
                _replicateListenersForTag.Remove(tags);
            }

            // Debugger.Info(LogGroupTag.Module, "从同步传输管理容器中移除指定的数据标签‘{%s}’对应的目标对象类型‘{%t}’的同步转发监听回调接口！", tags, listener);
        }

        /// <summary>
        /// 取消指定的监听回调接口对应的所有同步传输
        /// </summary>
        public void RemoveReplicateCommunicateForTarget(IReplicateDispatch listener)
        {
            IList<string> ids = NovaEngine.Utility.Collection.ToListForKeys<string, IList<IReplicateDispatch>>(_replicateListenersForTag);
            for (int n = 0; null != ids && n < ids.Count; ++n)
            {
                RemoveReplicateCommunicate(ids[n], listener);
            }
        }

        #endregion

        /// <summary>
        /// 将数据标签转换成整数数组
        /// </summary>
        /// <param name="tags">数据标签</param>
        /// <returns>返回对应的整型编码数组</returns>
        internal int[] ReplicateTagToCodeArray(string tags)
        {
            string[] tag_list = tags.Split('.', StringSplitOptions.RemoveEmptyEntries);

            // return tag_list.Select(tag => tag.GetHashCode()).ToArray();
            return Array.ConvertAll(tag_list, tag => tag.GetHashCode());
        }
    }
}
