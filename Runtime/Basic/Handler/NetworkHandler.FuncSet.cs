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

using System.Collections.Generic;

using SystemType = System.Type;
using SystemMethodInfo = System.Reflection.MethodInfo;

namespace GameEngine
{
    /// <summary>
    /// 网络模块封装的句柄对象类
    /// 模块具体功能接口请参考<see cref="NovaEngine.NetworkModule"/>类
    /// </summary>
    public sealed partial class NetworkHandler
    {
        /// <summary>
        /// 消息监听回调绑定接口的缓存容器
        /// </summary>
        private IDictionary<SystemType, IDictionary<string, MessageCallMethodInfo>> _messageListenerBindingCaches;

        /// <summary>
        /// 消息监听绑定接口初始化回调函数
        /// </summary>
        [UnityEngine.Scripting.Preserve]
        [OnSubmoduleInitCallback]
        private void OnMessageListenerBindingInitialize()
        {
            // 初始化回调绑定缓存容器
            _messageListenerBindingCaches = new Dictionary<SystemType, IDictionary<string, MessageCallMethodInfo>>();
        }

        /// <summary>
        /// 消息监听绑定接口清理回调函数
        /// </summary>
        [UnityEngine.Scripting.Preserve]
        [OnSubmoduleCleanupCallback]
        private void OnMessageListenerBindingCleanup()
        {
            // 移除所有消息监听回调函数
            RemoveAllMessageListenerBindingCalls();

            // 销毁回调函数缓存列表
            _messageListenerBindingCaches.Clear();
            _messageListenerBindingCaches = null;
        }

        /// <summary>
        /// 消息监听绑定接口重载回调函数
        /// </summary>
        [UnityEngine.Scripting.Preserve]
        [OnSubmoduleReloadCallback]
        private void OnMessageListenerBindingReload()
        {
            // 移除所有消息监听回调函数
            RemoveAllMessageListenerBindingCalls();
        }

        #region 实体对象消息监听绑定的回调函数注册/注销相关的接口函数

        /// <summary>
        /// 新增指定的回调绑定函数到当前消息监听缓存管理容器中
        /// </summary>
        /// <param name="fullname">完整名称</param>
        /// <param name="targetType">目标对象类型</param>
        /// <param name="methodInfo">函数对象</param>
        /// <param name="opcode">协议编码</param>
        /// <param name="automatically">自动装载状态标识</param>
        internal void AddMessageListenerBindingCallInfo(string fullname, SystemType targetType, SystemMethodInfo methodInfo, int opcode, bool automatically)
        {
            if (false == _messageListenerBindingCaches.TryGetValue(targetType, out IDictionary<string, MessageCallMethodInfo> messageCallMethodInfos))
            {
                messageCallMethodInfos = new Dictionary<string, MessageCallMethodInfo>();
                _messageListenerBindingCaches.Add(targetType, messageCallMethodInfos);
            }

            if (messageCallMethodInfos.TryGetValue(fullname, out MessageCallMethodInfo messageCallMethodInfo))
            {
                return;
            }

            // 函数格式校验
            if (NovaEngine.Debugger.Instance.IsOnDebuggingVerificationActivated())
            {
                bool verificated = false;
                if (NovaEngine.Utility.Reflection.IsTypeOfExtension(methodInfo))
                {
                    verificated = Loader.Inspecting.CodeInspector.CheckFunctionFormatOfMessageCallWithBeanExtensionType(methodInfo);
                }
                else
                {
                    verificated = Loader.Inspecting.CodeInspector.CheckFunctionFormatOfMessageCall(methodInfo);
                }

                // 校验失败
                if (false == verificated)
                {
                    Debugger.Error(LogGroupTag.Module, "目标对象类型‘{%t}’的‘{%s}’函数判定为非法格式的消息监听绑定回调函数，添加回调绑定操作失败！", targetType, fullname);
                    return;
                }
            }

            Debugger.Info(LogGroupTag.Module, "新增指定的协议编码‘{%d}’对应的消息监听绑定回调函数，其响应接口函数来自于目标类型‘{%t}’的‘{%s}’函数。",
                    opcode, targetType, fullname);

            messageCallMethodInfo = new MessageCallMethodInfo(fullname, targetType, methodInfo, opcode, automatically);
            messageCallMethodInfos.Add(fullname, messageCallMethodInfo);
        }

        /// <summary>
        /// 新增指定的分发函数到当前消息监听管理容器中
        /// </summary>
        /// <param name="fullname">完整名称</param>
        /// <param name="targetType">目标对象类型</param>
        /// <param name="methodInfo">函数对象</param>
        /// <param name="messageType">消息对象类型</param>
        /// <param name="automatically">自动装载状态标识</param>
        internal void AddMessageListenerBindingCallInfo(string fullname, SystemType targetType, SystemMethodInfo methodInfo, SystemType messageType, bool automatically)
        {
            int opcode = GetOpcodeByMessageType(messageType);

            AddMessageListenerBindingCallInfo(fullname, targetType, methodInfo, opcode, automatically);
        }

        /// <summary>
        /// 从当前消息监听管理容器中移除指定标识的分发函数
        /// </summary>
        /// <param name="fullname">完整名称</param>
        /// <param name="targetType">目标对象类型</param>
        internal void RemoveMessageListenerBindingCallInfo(string fullname, SystemType targetType)
        {
            Debugger.Info(LogGroupTag.Module, "移除指定的消息监听绑定回调函数，其响应接口函数来自于目标类型‘{%t}’的‘{%s}’函数。", targetType, fullname);

            if (_messageListenerBindingCaches.TryGetValue(targetType, out IDictionary<string, MessageCallMethodInfo> messageCallMethodInfos))
            {
                if (messageCallMethodInfos.ContainsKey(fullname))
                {
                    messageCallMethodInfos.Remove(fullname);
                }

                if (messageCallMethodInfos.Count <= 0)
                {
                    _messageListenerBindingCaches.Remove(targetType);
                }
            }
        }

        /// <summary>
        /// 移除当前消息监听缓存管理容器中登记的所有回调绑定函数
        /// </summary>
        private void RemoveAllMessageListenerBindingCalls()
        {
            _messageListenerBindingCaches.Clear();
        }

        /// <summary>
        /// 针对网络消息调用指定的回调绑定函数
        /// </summary>
        /// <param name="fullname">完整名称</param>
        /// <param name="targetType">目标对象类型</param>
        /// <param name="targetObject">对象实例</param>
        /// <param name="message">消息内容</param>
        internal void InvokeMessageListenerBindingCall(string fullname, SystemType targetType, IBean targetObject, object message)
        {
            MessageCallMethodInfo messageCallMethodInfo = FindMessageListenerBindingCallByName(fullname, targetType);
            if (null == messageCallMethodInfo)
            {
                Debugger.Warn(LogGroupTag.Module, "当前的消息监听缓存管理容器中无法检索到指定类型‘{%t}’及名称‘{%s}’对应的回调绑定函数，此次消息内容‘{%t}’转发通知失败！", targetType, fullname, message);
                return;
            }

            messageCallMethodInfo.Invoke(targetObject, message);
        }

        /// <summary>
        /// 通过指定的名称及对象类型，在当前的缓存容器中查找对应的回调绑定函数
        /// </summary>
        /// <param name="fullname">完整名称</param>
        /// <param name="targetType">目标对象类型</param>
        /// <returns>返回绑定函数实例</returns>
        private MessageCallMethodInfo FindMessageListenerBindingCallByName(string fullname, SystemType targetType)
        {
            if (_messageListenerBindingCaches.TryGetValue(targetType, out IDictionary<string, MessageCallMethodInfo> messageCallMethodInfos))
            {
                if (messageCallMethodInfos.TryGetValue(fullname, out MessageCallMethodInfo messageCallMethodInfo))
                {
                    return messageCallMethodInfo;
                }
            }

            return null;
        }

        #endregion
    }
}
