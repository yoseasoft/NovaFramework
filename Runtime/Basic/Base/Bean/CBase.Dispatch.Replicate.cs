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
using System.Reflection;

namespace GameEngine
{
    /// 基础对象抽象类
    public abstract partial class CBase
    {
        /// <summary>
        /// 基础对象内部数据同步的传输回调映射列表
        /// </summary>
        private IDictionary<string, IDictionary<string, bool>> _replicateCommunicateCallForTag;

        /// <summary>
        /// 同步传输回调函数的绑定接口缓存容器
        /// </summary>
        private IDictionary<string, ReplicateCallMethodInfo> _replicateCallBindingCaches;

        /// <summary>
        /// 基础对象的同步传输回调初始化函数接口
        /// </summary>
        private void OnReplicateCommunicateCallInitialize()
        {
            // 同步传输回调映射容器初始化
            _replicateCommunicateCallForTag = new Dictionary<string, IDictionary<string, bool>>();
        }

        /// <summary>
        /// 基础对象的同步传输回调清理函数接口
        /// </summary>
        private void OnReplicateCommunicateCallCleanup()
        {
            // 移除所有同步数据
            RemoveAllReplicateCommunicates();

            _replicateCommunicateCallForTag = null;

            // 移除同步传输的委托句柄缓存实例
            RemoveAllReplicateCallDelegateHandlers();
            _replicateCallBindingCaches = null;
        }

        #region 基础对象同步分发提供的服务接口函数

        /// <summary>
        /// 播报数据标签到同步管理器中等待派发
        /// </summary>
        /// <param name="tags">数据标签</param>
        /// <param name="announceType">数据播报类型</param>
        public void Send(string tags, ReplicateAnnounceType announceType)
        {
            ReplicateController.Instance.Send(tags, announceType);
        }

        /// <summary>
        /// 立即推送指定的数据标签到分发调度中
        /// </summary>
        /// <param name="tags">数据标签</param>
        /// <param name="announceType">数据播报类型</param>
        public void Fire(string tags, ReplicateAnnounceType announceType)
        {
            ReplicateController.Instance.Fire(tags, announceType);
        }

        #endregion

        #region 基础对象同步传输相关回调函数的操作接口定义

        /// <summary>
        /// 基础对象的同步通讯的监听回调函数<br/>
        /// 该函数针对同步转发接口的标准实现，禁止子类重写该函数<br/>
        /// 若子类需要根据需要自行解析同步，可以通过重写<see cref="GameEngine.CBase.OnReplicate(string, ReplicateAnnounceType)"/>实现同步的自定义处理逻辑
        /// </summary>
        /// <param name="tags">数据标签</param>
        /// <param name="announceType">数据播报类型</param>
        public virtual void OnReplicateDispatchForTag(string tags, ReplicateAnnounceType announceType)
        {
            if (_replicateCommunicateCallForTag.TryGetValue(tags, out IDictionary<string, bool> calls))
            {
                foreach (KeyValuePair<string, bool> kvp in calls)
                {
                    // ReplicateController.Instance.InvokeReplicateCommunicateBindingCall(this, kvp.Key, BeanType, tags, announceType);
                    InvokeReplicateCall(kvp.Key, tags, announceType);
                }
            }

            OnReplicate(tags, announceType);
        }

        /// <summary>
        /// 用户自定义的同步处理函数，您可以通过重写该函数处理自定义同步行为
        /// </summary>
        /// <param name="tags">数据标签</param>
        /// <param name="announceType">数据播报类型</param>
        protected abstract void OnReplicate(string tags, ReplicateAnnounceType announceType);

        /// <summary>
        /// 针对指定数据标签新增同步传输的后处理程序
        /// </summary>
        /// <param name="tags">数据标签</param>
        /// <param name="announceType">数据播报类型</param>
        /// <returns>返回后处理的操作结果</returns>
        protected abstract bool OnReplicateCommunicateAddedActionPostProcess(string tags, ReplicateAnnounceType announceType);
        /// <summary>
        /// 针对指定数据标签移除同步传输的后处理程序
        /// </summary>
        /// <param name="tags">数据标签</param>
        /// <param name="announceType">数据播报类型</param>
        protected abstract void OnReplicateCommunicateRemovedActionPostProcess(string tags, ReplicateAnnounceType announceType);

        /// <summary>
        /// 检测当前基础对象是否传输了目标数据标签
        /// </summary>
        /// <param name="tags">数据标签</param>
        /// <param name="announceType">数据播报类型</param>
        /// <returns>若传输了给定数据标签则返回true，否则返回false</returns>
        protected internal virtual bool IsReplicateCommunicatedOfTargetTag(string tags, ReplicateAnnounceType announceType)
        {
            if (_replicateCommunicateCallForTag.ContainsKey(tags) && _replicateCommunicateCallForTag[tags].Count > 0)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 基础对象的同步传输函数接口，对一个指定的数据进行通讯监听
        /// </summary>
        /// <param name="tags">数据标签</param>
        /// <param name="announceType">数据播报类型</param>
        /// <returns>若同步传输成功则返回true，否则返回false</returns>
        protected internal virtual bool AddReplicateCommunicate(string tags, ReplicateAnnounceType announceType)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 基础对象的同步传输函数接口，将一个指定的同步绑定到给定的监听回调函数上
        /// </summary>
        /// <param name="fullname">函数名称</param>
        /// <param name="methodInfo">监听回调函数</param>
        /// <param name="tags">数据标签</param>
        /// <param name="announceType">数据播报类型</param>
        /// <returns>若同步传输成功则返回true，否则返回false</returns>
        public bool AddReplicateCommunicate(string fullname, MethodInfo methodInfo, string tags, ReplicateAnnounceType announceType)
        {
            return AddReplicateCommunicate(fullname, methodInfo, tags, announceType, false);
        }

        /// <summary>
        /// 基础对象的同步传输函数接口，将一个指定的同步绑定到给定的监听回调函数上
        /// </summary>
        /// <param name="fullname">函数名称</param>
        /// <param name="methodInfo">监听回调函数</param>
        /// <param name="tags">数据标签</param>
        /// <param name="announceType">数据播报类型</param>
        /// <param name="automatically">自动装载状态标识</param>
        /// <returns>若同步传输成功则返回true，否则返回false</returns>
        protected internal bool AddReplicateCommunicate(string fullname, MethodInfo methodInfo, string tags, ReplicateAnnounceType announceType, bool automatically)
        {
            AddReplicateCallDelegateHandler(fullname, methodInfo, tags, announceType, automatically);

            if (false == _replicateCommunicateCallForTag.TryGetValue(tags, out IDictionary<string, bool> calls))
            {
                // 创建回调列表
                calls = new Dictionary<string, bool>() { { fullname, automatically }, };

                _replicateCommunicateCallForTag.Add(tags, calls);

                // 新增输入响应的后处理程序
                return OnReplicateCommunicateAddedActionPostProcess(tags, announceType);
            }

            if (calls.ContainsKey(fullname))
            {
                Debugger.Warn("The '{%t}' instance's replicate '{%d}' was already add same listener by name '{%s}', repeat do it failed.",
                        BeanType, tags, fullname);
                return false;
            }

            calls.Add(fullname, automatically);

            return true;
        }

        /// <summary>
        /// 取消当前基础对象对指定数据的通讯
        /// </summary>
        /// <param name="tags">数据标签</param>
        protected internal void RemoveReplicateCommunicate(string tags)
        {
            RemoveReplicateCommunicate(tags, 0);
        }

        /// <summary>
        /// 取消当前基础对象对指定数据的通讯
        /// </summary>
        /// <param name="tags">数据标签</param>
        /// <param name="announceType">数据播报类型</param>
        protected internal virtual void RemoveReplicateCommunicate(string tags, ReplicateAnnounceType announceType)
        {
            // 若针对特定数据绑定了监听回调，则移除相应的回调句柄
            if (_replicateCommunicateCallForTag.ContainsKey(tags))
            {
                _replicateCommunicateCallForTag.Remove(tags);
            }

            // 移除同步传输的后处理程序
            OnReplicateCommunicateRemovedActionPostProcess(tags, announceType);
        }

        /// <summary>
        /// 取消当前基础对象对指定数据的监听回调函数
        /// </summary>
        /// <param name="inputCode">输入编码</param>
        /// <param name="fullname">函数名称</param>
        protected internal void RemoveReplicateCommunicate(string fullname, string tags)
        {
            RemoveReplicateCommunicate(fullname, tags, 0);
        }

        /// <summary>
        /// 取消当前基础对象对指定数据的监听回调函数
        /// </summary>
        /// <param name="fullname">函数名称</param>
        /// <param name="tags">数据标签</param>
        /// <param name="announceType">数据播报类型</param>
        protected internal void RemoveReplicateCommunicate(string fullname, string tags, ReplicateAnnounceType announceType)
        {
            if (_replicateCommunicateCallForTag.TryGetValue(tags, out IDictionary<string, bool> calls))
            {
                if (calls.ContainsKey(fullname))
                {
                    calls.Remove(fullname);
                    // 移除构建的委托句柄
                    RemoveReplicateCallDelegateHandler(fullname);
                }
            }

            // 当前监听列表为空时，移除该编码的监听
            if (false == IsReplicateCommunicatedOfTargetTag(tags, announceType))
            {
                RemoveReplicateCommunicate(tags, announceType);
            }
        }

        /// <summary>
        /// 移除所有自动注册的同步传输回调接口
        /// </summary>
        protected internal void RemoveAllAutomaticallyReplicateCommunicates()
        {
            OnAutomaticallyCallSyntaxInfoProcessHandler<string>(_replicateCommunicateCallForTag, RemoveReplicateCommunicate);
        }

        /// <summary>
        /// 取消当前基础对象的所有同步传输
        /// </summary>
        public virtual void RemoveAllReplicateCommunicates()
        {
            IList<string> tag_keys = NovaEngine.Utility.Collection.ToListForKeys<string, IDictionary<string, bool>>(_replicateCommunicateCallForTag);
            if (null != tag_keys)
            {
                int c = tag_keys.Count;
                for (int n = 0; n < c; ++n) { RemoveReplicateCommunicate(tag_keys[n]); }
            }

            _replicateCommunicateCallForTag.Clear();
        }

        #endregion

        #region 基础对象同步传输相关回调函数的构建委托接口定义

        /// <summary>
        /// 构建一个同步传输回调函数的委托句柄，并添加到调度容器中
        /// </summary>
        /// <param name="fullname">完整名称</param>
        /// <param name="methodInfo">函数对象</param>
        /// <param name="tags">数据标签</param>
        /// <param name="announceType">数据播报类型</param>
        /// <param name="automatically">自动装载状态标识</param>
        private void AddReplicateCallDelegateHandler(string fullname, MethodInfo methodInfo, string tags, ReplicateAnnounceType announceType, bool automatically)
        {
            // 静态函数（包括扩展类型）
            if (methodInfo.IsStatic)
            {
                ReplicateController.Instance.AddReplicateCommunicateBindingCallInfo(fullname, BeanType, methodInfo, tags, announceType, automatically);
                return;
            }

            if (null == _replicateCallBindingCaches)
            {
                _replicateCallBindingCaches = new Dictionary<string, ReplicateCallMethodInfo>();
            }

            if (_replicateCallBindingCaches.ContainsKey(fullname))
            {
                return;
            }

            Debugger.Info(LogGroupTag.Module, "新增指定的数据标签‘{%s}’及播报类型‘{%v}’对应的同步传输绑定回调函数，其传输接口函数来自于目标类型‘{%t}’的‘{%s}’函数。",
                    tags, announceType, BeanType, fullname);

            ReplicateCallMethodInfo replicateCallMethodInfo = new ReplicateCallMethodInfo(this, fullname, BeanType, methodInfo, tags, announceType, automatically);
            _replicateCallBindingCaches.Add(fullname, replicateCallMethodInfo);
        }

        /// <summary>
        /// 从调度容器中移除指定名称对应的同步传输函数的委托句柄
        /// </summary>
        /// <param name="fullname">函数名称</param>
        private void RemoveReplicateCallDelegateHandler(string fullname)
        {
            if (null == _replicateCallBindingCaches)
            {
                return;
            }

            if (_replicateCallBindingCaches.ContainsKey(fullname))
            {
                _replicateCallBindingCaches.Remove(fullname);
            }
        }

        /// <summary>
        /// 移除当前同步传输回调函数构建的全部委托句柄实例
        /// </summary>
        private void RemoveAllReplicateCallDelegateHandlers()
        {
            _replicateCallBindingCaches?.Clear();
        }

        /// <summary>
        /// 针对消息事件调用指定的回调绑定函数
        /// </summary>
        /// <param name="fullname">完整名称</param>
        /// <param name="tags">数据标签</param>
        /// <param name="announceType">数据播报类型</param>
        private void InvokeReplicateCall(string fullname, string tags, ReplicateAnnounceType announceType)
        {
            if (null != _replicateCallBindingCaches && _replicateCallBindingCaches.TryGetValue(fullname, out ReplicateCallMethodInfo replicateCallMethodInfo))
            {
                replicateCallMethodInfo.Invoke(tags, announceType);
                return;
            }

            ReplicateController.Instance.InvokeReplicateCommunicateBindingCall(this, fullname, BeanType, tags, announceType);
        }

        #endregion
    }
}
