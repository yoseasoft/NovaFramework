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
        private NovaEngine.MultiwayTree<int, IList<IReplicateDispatch>> _replicateListeners;

        /// <summary>
        /// 数据同步管理对象初始化通知接口函数
        /// </summary>
        protected override sealed void OnInitialize()
        {
            // 初始化监听列表
            _replicateListeners = new NovaEngine.MultiwayTree<int, IList<IReplicateDispatch>>();
        }

        /// <summary>
        /// 数据同步管理对象清理通知接口函数
        /// </summary>
        protected override sealed void OnCleanup()
        {
            // 清理监听列表
            _replicateListeners.Clear();
            _replicateListeners = null;
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

            if (_replicateListeners.TryGetValue(keys, out IList<IReplicateDispatch> listeners))
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
                    listener.OnReplicateDispatch(tags, announceType);
                }
            }
        }

        /// <summary>
        /// 将数据标签转换成整数数组
        /// </summary>
        /// <param name="tags">数据标签</param>
        /// <returns>返回对应的整型编码数组</returns>
        private int[] ReplicateTagToCodeArray(string tags)
        {
            string[] tag_list = tags.Split('.', StringSplitOptions.RemoveEmptyEntries);

            // return tag_list.Select(tag => tag.GetHashCode()).ToArray();
            return Array.ConvertAll(tag_list, tag => tag.GetHashCode());
        }
    }
}
