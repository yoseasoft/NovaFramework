/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyright (C) 2020 - 2022, Guangzhou Xinyuan Technology Co., Ltd.
/// Copyright (C) 2022 - 2023, Shanghai Bilibili Technology Co., Ltd.
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

using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine.Scripting;

namespace GameEngine
{
    /// 网络模块封装的句柄对象类
    public sealed partial class NetworkHandler
    {
        /// <summary>
        /// 网络消息转发监听回调管理容器
        /// </summary>
        private IDictionary<int, IList<IMessageDispatch>> _messageDispatchListeners;

        /// <summary>
        /// 协议编码分发调度接口的数据结构容器
        /// </summary>
        private IDictionary<int, IList<MessageCallMethodInfo>> _messageDistributeCallInfos;

        /// <summary>
        /// 消息回调转发接口初始化回调函数
        /// </summary>
        [Preserve]
        [OnSubmoduleInitCallback]
        private void OnMessageCallDispatchingInitialize()
        {
            // 初始化消息转发监听回调映射字典
            _messageDispatchListeners = new Dictionary<int, IList<IMessageDispatch>>();
            // 初始化回调句柄映射字典
            _messageDistributeCallInfos = new Dictionary<int, IList<MessageCallMethodInfo>>();
        }

        /// <summary>
        /// 消息回调转发接口清理回调函数
        /// </summary>
        [Preserve]
        [OnSubmoduleCleanupCallback]
        private void OnMessageCallDispatchingCleanup()
        {
            // 清理消息通知转发监听管理容器
            _messageDispatchListeners.Clear();
            _messageDispatchListeners = null;

            // 移除所有消息分发回调信息
            RemoveAllMessageDistributeCalls();

            // 销毁回调句柄映射字典
            _messageDistributeCallInfos.Clear();
            _messageDistributeCallInfos = null;
        }

        /// <summary>
        /// 消息回调转发接口重载回调函数
        /// </summary>
        [Preserve]
        [OnSubmoduleReloadCallback]
        private void OnMessageCallDispatchingReload()
        {
        }

        #region 针对实例对象消息监听的分发调度管理接口函数

        /// <summary>
        /// 新增一个指定协议码对应的消息转发通知的监听回调接口
        /// </summary>
        /// <param name="opcode">协议操作码</param>
        /// <param name="listener">消息监听回调接口</param>
        /// <returns>若添加监听接口成功则返回true，否则返回false</returns>
        public bool AddMessageDispatchListener(int opcode, IMessageDispatch listener)
        {
            if (false == _messageDispatchListeners.TryGetValue(opcode, out IList<IMessageDispatch> list))
            {
                list = new List<IMessageDispatch>() { listener };

                _messageDispatchListeners.Add(opcode, list);
                return true;
            }

            // 检查是否重复注册
            if (list.Contains(listener))
            {
                Debugger.Warn("The listener for target message '{%d}' was already exist, cannot repeat add it.", opcode);
                return false;
            }

            list.Add(listener);

            return true;
        }

        /// <summary>
        /// 新增一个指定消息类型对应的消息转发通知的监听回调接口
        /// </summary>
        /// <param name="messageType">消息类型</param>
        /// <param name="listener">消息监听回调接口</param>
        /// <returns>若添加监听接口成功则返回true，否则返回false</returns>
        public bool AddMessageDispatchListener(Type messageType, IMessageDispatch listener)
        {
            int opcode = GetOpcodeByMessageType(messageType);

            return AddMessageDispatchListener(opcode, listener);
        }

        /// <summary>
        /// 移除指定协议码对应的消息转发监听回调接口
        /// </summary>
        /// <param name="opcode">协议操作码</param>
        /// <param name="listener">消息监听回调接口</param>
        public void RemoveMessageDispatchListener(int opcode, IMessageDispatch listener)
        {
            if (false == _messageDispatchListeners.TryGetValue(opcode, out IList<IMessageDispatch> list))
            {
                Debugger.Warn("Could not found any listener for target message '{%d}' in dispatch container, removed it failed.", opcode);
                return;
            }

            list.Remove(listener);
            // 列表为空则移除对应的消息监听列表实例
            if (list.Count == 0)
            {
                _messageDispatchListeners.Remove(opcode);
            }
        }

        /// <summary>
        /// 移除指定消息类型对应的消息转发监听回调接口
        /// </summary>
        /// <param name="messageType">消息类型</param>
        /// <param name="listener">消息监听回调接口</param>
        public void RemoveMessageDispatchListener(Type messageType, IMessageDispatch listener)
        {
            int opcode = GetOpcodeByMessageType(messageType);

            RemoveMessageDispatchListener(opcode, listener);
        }

        /// <summary>
        /// 移除指定监听接口的所有注册消息通知
        /// </summary>
        public void RemoveAllMessageDispatchListeners(IMessageDispatch listener)
        {
            IList<int> ids = NovaEngine.Utility.Collection.ToListForKeys<int, IList<IMessageDispatch>>(_messageDispatchListeners);
            for (int n = 0; null != ids && n < ids.Count; ++n)
            {
                RemoveMessageDispatchListener(ids[n], listener);
            }
        }

        #endregion

        #region 针对全局转发消息监听的分发调度管理接口函数

        /// <summary>
        /// 针对网络数据进行消息分发的调度入口函数
        /// </summary>
        /// <param name="opcode">协议编码</param>
        /// <param name="message">消息内容</param>
        private void OnMessageDistributeCallDispatched(int opcode, object message)
        {
            if (_messageDistributeCallInfos.TryGetValue(opcode, out IList<MessageCallMethodInfo> list))
            {
                IEnumerator<MessageCallMethodInfo> e_info = list.GetEnumerator();
                while (e_info.MoveNext())
                {
                    MessageCallMethodInfo info = e_info.Current;
                    if (null == info.TargetType)
                    {
                        info.Invoke(message);
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
                                info.Invoke(bean, message);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 新增指定的分发函数到当前消息监听管理容器中
        /// </summary>
        /// <param name="fullname">完整名称</param>
        /// <param name="targetType">目标对象类型</param>
        /// <param name="methodInfo">函数对象</param>
        /// <param name="opcode">协议编码</param>
        private void AddMessageDistributeCallInfo(string fullname, Type targetType, MethodInfo methodInfo, int opcode)
        {
            MessageCallMethodInfo info = new MessageCallMethodInfo(fullname, targetType, methodInfo, opcode);

            Debugger.Info(LogGroupTag.Module, "Add new message distribute call '{%t}' to target opcode '{%d}' of the class type '{%t}'.",
                    methodInfo, opcode, targetType);
            if (_messageDistributeCallInfos.ContainsKey(opcode))
            {
                IList<MessageCallMethodInfo> list = _messageDistributeCallInfos[opcode];
                list.Add(info);
            }
            else
            {
                IList<MessageCallMethodInfo> list = new List<MessageCallMethodInfo>() { info };
                _messageDistributeCallInfos.Add(opcode, list);
            }
        }

        /// <summary>
        /// 新增指定的分发函数到当前消息监听管理容器中
        /// </summary>
        /// <param name="fullname">完整名称</param>
        /// <param name="targetType">目标对象类型</param>
        /// <param name="methodInfo">函数对象</param>
        /// <param name="messageType">消息对象类型</param>
        private void AddMessageDistributeCallInfo(string fullname, Type targetType, MethodInfo methodInfo, Type messageType)
        {
            int opcode = GetOpcodeByMessageType(messageType);

            AddMessageDistributeCallInfo(fullname, targetType, methodInfo, opcode);
        }

        /// <summary>
        /// 从当前消息监听管理容器中移除指定标识的分发函数
        /// </summary>
        /// <param name="fullname">完整名称</param>
        /// <param name="targetType">目标对象类型</param>
        /// <param name="opcode">协议编码</param>
        private void RemoveMessageDistributeCallInfo(string fullname, Type targetType, int opcode)
        {
            Debugger.Info(LogGroupTag.Module, "Remove message distribute call '{%s}' with target opcode '{%d}' and class type '{%t}'.",
                    fullname, opcode, targetType);
            if (false == _messageDistributeCallInfos.ContainsKey(opcode))
            {
                Debugger.Warn("Could not found any message distribute call '{%s}' with target opcode '{%d}', removed it failed.", fullname, opcode);
                return;
            }

            IList<MessageCallMethodInfo> list = _messageDistributeCallInfos[opcode];
            for (int n = 0; n < list.Count; ++n)
            {
                MessageCallMethodInfo info = list[n];
                if (info.Fullname.Equals(fullname))
                {
                    Debugger.Assert(info.TargetType == targetType && info.Opcode == opcode, NovaEngine.ErrorText.InvalidArguments);

                    list.RemoveAt(n);
                    if (list.Count <= 0)
                    {
                        _messageDistributeCallInfos.Remove(opcode);
                    }

                    return;
                }
            }

            Debugger.Warn("Could not found any message distribute call '{%s}' with target opcode '{%d}' and class type '{%t}', removed it failed.",
                    fullname, opcode, targetType);
        }

        /// <summary>
        /// 从当前消息监听管理容器中移除指定类型的分发函数
        /// </summary>
        /// <param name="fullname">完整名称</param>
        /// <param name="targetType">目标对象类型</param>
        /// <param name="messageType">消息对象类型</param>
        private void RemoveMessageDistributeCallInfo(string fullname, Type targetType, Type messageType)
        {
            int opcode = GetOpcodeByMessageType(messageType);

            RemoveMessageDistributeCallInfo(fullname, targetType, opcode);
        }

        /// <summary>
        /// 移除当前消息监听管理器中登记的所有分发函数回调句柄
        /// </summary>
        private void RemoveAllMessageDistributeCalls()
        {
            _messageDistributeCallInfos.Clear();
        }

        #endregion
    }
}
