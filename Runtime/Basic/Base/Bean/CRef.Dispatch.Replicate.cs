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

namespace GameEngine
{
    /// 引用对象抽象类
    public abstract partial class CRef
    {
        /// <summary>
        /// 对象内部同步传输的标签管理容器
        /// </summary>
        private IList<string> _replicateTags;

        /// <summary>
        /// 引用对象的同步传输处理初始化函数接口
        /// </summary>
        private void OnReplicateCommunicateProcessingInitialize()
        {
            // 同步标签容器初始化
            _replicateTags = new List<string>();
        }

        /// <summary>
        /// 引用对象的同步传输处理清理函数接口
        /// </summary>
        private void OnReplicateCommunicateProcessingCleanup()
        {
            // 移除所有同步标签
            Debugger.Assert(_replicateTags.Count == 0);
            _replicateTags = null;
        }

        #region 基础对象同步传输相关处理函数的操作接口定义

        /// <summary>
        /// 发送同步消息到自己的同步管理器中进行派发
        /// </summary>
        /// <param name="tags">数据标签</param>
        /// <param name="announceType">数据播报类型</param>
        public void SendToSelf(string tags, ReplicateAnnounceType announceType)
        {
            OnReplicateDispatchForTag(tags, announceType);
        }

        /// <summary>
        /// 用户自定义的同步处理函数，您可以通过重写该函数处理自定义同步行为
        /// </summary>
        /// <param name="tags">数据标签</param>
        /// <param name="announceType">数据播报类型</param>
        protected override void OnReplicate(string tags, ReplicateAnnounceType announceType) { }

        /// <summary>
        /// 针对指定数据标签新增同步传输的后处理程序
        /// </summary>
        /// <param name="tags">数据标签</param>
        /// <param name="announceType">数据播报类型</param>
        /// <returns>返回后处理的操作结果</returns>
        protected override bool OnReplicateCommunicateAddedActionPostProcess(string tags, ReplicateAnnounceType announceType)
        {
            return AddReplicateCommunicate(tags, announceType);
        }

        /// <summary>
        /// 针对指定数据标签移除同步传输的后处理程序
        /// </summary>
        /// <param name="tags">数据标签</param>
        /// <param name="announceType">数据播报类型</param>
        protected override void OnReplicateCommunicateRemovedActionPostProcess(string tags, ReplicateAnnounceType announceType)
        { }

        /// <summary>
        /// 引用对象的同步传输函数接口，对一个指定的数据标签进行通讯监听
        /// </summary>
        /// <param name="tags">数据标签</param>
        /// <param name="announceType">数据播报类型</param>
        /// <returns>若同步传输成功则返回true，否则返回false</returns>
        protected internal override sealed bool AddReplicateCommunicate(string tags, ReplicateAnnounceType announceType)
        {
            if (_replicateTags.Contains(tags))
            {
                Debugger.Warn(LogGroupTag.Bean, "The 'CRef' instance replicate '{%s}' was already added, repeat do it failed.", tags);
                return true;
            }

            if (false == ReplicateController.Instance.AddReplicateCommunicate(tags, this))
            {
                Debugger.Warn(LogGroupTag.Bean, "The 'CRef' instance add replicate communicate '{%s}' failed.", tags);
                return false;
            }

            _replicateTags.Add(tags);

            return true;
        }

        /// <summary>
        /// 取消当前引用对象对指定同步的传输
        /// </summary>
        /// <param name="tags">数据标签</param>
        /// <param name="announceType">数据播报类型</param>
        protected internal override sealed void RemoveReplicateCommunicate(string tags, ReplicateAnnounceType announceType)
        {
            if (false == _replicateTags.Contains(tags))
            {
                // Debugger.Warn(LogGroupTag.Bean, "Could not found any replicate '{%s}' for target 'CRef' instance with on added, do removed it failed.", tags);
                return;
            }

            ReplicateController.Instance.RemoveReplicateCommunicate(tags, this);
            _replicateTags.Remove(tags);

            base.RemoveReplicateCommunicate(tags, announceType);
        }

        /// <summary>
        /// 取消当前引用对象的所有输入响应
        /// </summary>
        public override sealed void RemoveAllReplicateCommunicates()
        {
            base.RemoveAllReplicateCommunicates();

            ReplicateController.Instance.RemoveReplicateCommunicateForTarget(this);

            _replicateTags.Clear();
        }

        #endregion
    }
}
