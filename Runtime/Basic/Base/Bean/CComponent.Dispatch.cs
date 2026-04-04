/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyright (C) 2023 - 2024, Guangzhou Shiyue Network Technology Co., Ltd.
/// Copyright (C) 2024 - 2025, Hurley, Independent Studio.
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

namespace GameEngine
{
    /// 基于ECS模式的组件对象封装类
    public abstract partial class CComponent
    {
        #region 组件对象输入响应相关操作函数合集

        /// <summary>
        /// 用户自定义的输入处理函数，您可以通过重写该函数处理自定义输入行为
        /// </summary>
        /// <param name="keyCode">按键编码</param>
        /// <param name="operationType">输入操作类型</param>
        protected override void OnInput(VirtualKeyCode keyCode, InputOperationType operationType) { }

        /// <summary>
        /// 用户自定义的输入处理函数，您可以通过重写该函数处理自定义输入行为
        /// </summary>
        /// <param name="inputData">输入数据</param>
        protected override void OnInput(object inputData) { }

        /// <summary>
        /// 针对指定输入编码新增输入响应的后处理程序
        /// </summary>
        /// <param name="keyCode">按键编码</param>
        /// <param name="operationType">输入操作类型</param>
        /// <returns>返回后处理的操作结果</returns>
        protected override bool OnInputResponseAddedActionPostProcess(VirtualKeyCode keyCode, InputOperationType operationType)
        {
            return Entity.AddInputResponseFromComponent(keyCode, operationType);
        }

        /// <summary>
        /// 针对指定输入类型新增输入响应的后处理程序
        /// </summary>
        /// <param name="inputType">输入类型</param>
        /// <returns>返回后处理的操作结果</returns>
        protected override bool OnInputResponseAddedActionPostProcess(Type inputType)
        {
            return Entity.AddInputResponseFromComponent(inputType);
        }

        /// <summary>
        /// 针对指定输入编码移除输入响应的后处理程序
        /// </summary>
        /// <param name="keyCode">按键编码</param>
        /// <param name="operationType">输入操作类型</param>
        protected override void OnInputResponseRemovedActionPostProcess(VirtualKeyCode keyCode, InputOperationType operationType)
        {
            // 移除实体中对应的输入响应
            Entity?.RemoveInputResponseFromComponent(keyCode, operationType);
        }

        /// <summary>
        /// 针对指定输入类型移除输入响应的后处理程序
        /// </summary>
        /// <param name="inputType">输入类型类型</param>
        protected override void OnInputResponseRemovedActionPostProcess(Type inputType)
        {
            // 移除实体中对应的输入响应
            Entity?.RemoveInputResponseFromComponent(inputType);
        }

        #endregion

        #region 组件对象事件转发相关操作函数合集

        /// <summary>
        /// 用户自定义的事件处理函数，您可以通过重写该函数处理自定义事件行为
        /// </summary>
        /// <param name="eventID">事件标识</param>
        /// <param name="args">事件数据参数</param>
        protected override void OnEvent(int eventID, params object[] args) { }

        /// <summary>
        /// 用户自定义的事件处理函数，您可以通过重写该函数处理自定义事件行为
        /// </summary>
        /// <param name="eventData">事件数据</param>
        protected override void OnEvent(object eventData) { }

        /// <summary>
        /// 针对指定事件标识新增事件订阅的后处理程序
        /// </summary>
        /// <param name="eventID">事件标识</param>
        /// <returns>返回后处理的操作结果</returns>
        protected override bool OnSubscribeActionPostProcess(int eventID)
        {
            return Entity.SubscribeFromComponent(eventID);
        }

        /// <summary>
        /// 针对指定事件类型新增事件订阅的后处理程序
        /// </summary>
        /// <param name="eventType">事件类型</param>
        /// <returns>返回后处理的操作结果</returns>
        protected override bool OnSubscribeActionPostProcess(Type eventType)
        {
            return Entity.SubscribeFromComponent(eventType);
        }

        /// <summary>
        /// 针对指定事件标识移除事件订阅的后处理程序
        /// </summary>
        /// <param name="eventID">事件标识</param>
        protected override void OnUnsubscribeActionPostProcess(int eventID)
        {
            // 移除实体中对应的事件订阅
            Entity?.UnsubscribeFromComponent(eventID);
        }

        /// <summary>
        /// 针对指定事件类型移除事件订阅的后处理程序
        /// </summary>
        /// <param name="eventType">事件类型</param>
        protected override void OnUnsubscribeActionPostProcess(Type eventType)
        {
            // 移除实体中对应的事件订阅
            Entity?.UnsubscribeFromComponent(eventType);
        }

        #endregion

        #region 组件对象消息通知相关操作函数合集

        /// <summary>
        /// 用户自定义的消息处理函数，您可以通过重写该函数处理自定义消息通知
        /// </summary>
        /// <param name="opcode">协议操作码</param>
        /// <param name="message">消息对象实例</param>
        protected override void OnMessage(int opcode, object message) { }

        /// <summary>
        /// 针对指定消息标识新增消息监听的后处理程序
        /// </summary>
        /// <param name="opcode">协议操作码</param>
        /// <returns>返回后处理的操作结果</returns>
        protected override sealed bool OnMessageListenerAddedActionPostProcess(int opcode)
        {
            return Entity.AddMessageListenerFromComponent(opcode);
        }

        /// <summary>
        /// 针对指定消息标识移除消息监听的后处理程序
        /// </summary>
        /// <param name="opcode">协议操作码</param>
        protected override sealed void OnMessageListenerRemovedActionPostProcess(int opcode)
        {
            // 移除实体中对应的消息通知
            Entity?.RemoveMessageListenerFromComponent(opcode);
        }

        #endregion

        #region 组件对象同步传输相关操作函数合集

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
            return Entity.AddReplicateCommunicateFromComponent(tags, announceType);
        }

        /// <summary>
        /// 针对指定输入编码移除同步传输的后处理程序
        /// </summary>
        /// <param name="tags">数据标签</param>
        /// <param name="announceType">数据播报类型</param>
        protected override void OnReplicateCommunicateRemovedActionPostProcess(string tags, ReplicateAnnounceType announceType)
        {
            // 移除实体中对应的同步传输
            Entity?.RemoveReplicateCommunicateFromComponent(tags, announceType);
        }

        #endregion
    }
}
