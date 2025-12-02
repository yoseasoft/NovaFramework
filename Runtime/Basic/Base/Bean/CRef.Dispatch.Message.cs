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

using System.Collections.Generic;

using SystemType = System.Type;

namespace GameEngine
{
    /// 引用对象抽象类
    public abstract partial class CRef
    {
        /// <summary>
        /// 对象内部监听消息的协议码管理容器
        /// </summary>
        private IList<int> _messageTypes;

        /// <summary>
        /// 引用对象的消息监听处理初始化函数接口
        /// </summary>
        private void OnMessageListenerProcessingInitialize()
        {
            // 消息协议码容器初始化
            _messageTypes = new List<int>();
        }

        /// <summary>
        /// 引用对象的消息监听处理清理函数接口
        /// </summary>
        private void OnMessageListenerProcessingCleanup()
        {
            // 移除所有消息通知
            Debugger.Assert(_messageTypes.Count == 0);
            _messageTypes = null;
        }

        #region 基础对象消息监听相关处理函数的操作接口定义

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
            return AddMessageListener(opcode);
        }

        /// <summary>
        /// 针对指定消息标识移除消息监听的后处理程序
        /// </summary>
        /// <param name="opcode">协议操作码</param>
        protected override sealed void OnMessageListenerRemovedActionPostProcess(int opcode)
        { }

        /// <summary>
        /// 引用对象的消息监听函数接口，对一个指定的消息进行转发监听
        /// </summary>
        /// <param name="opcode">协议操作码</param>
        /// <returns>若消息监听成功则返回true，否则返回false</returns>
        protected internal override sealed bool AddMessageListener(int opcode)
        {
            if (_messageTypes.Contains(opcode))
            {
                // Debugger.Warn("The 'CRef' instance's message type '{0}' was already exist, repeat added it failed.", opcode);
                return true;
            }

            if (false == NetworkHandler.Instance.AddMessageDispatchListener(opcode, this))
            {
                Debugger.Warn("The 'CRef' instance add message listener '{0}' failed.", opcode);
                return false;
            }

            _messageTypes.Add(opcode);

            return true;
        }

        /// <summary>
        /// 取消当前引用对象对指定协议码的监听回调
        /// </summary>
        /// <param name="opcode">协议操作码</param>
        public override sealed void RemoveMessageListener(int opcode)
        {
            if (false == _messageTypes.Contains(opcode))
            {
                // Debugger.Warn("Could not found any message type '{0}' for target 'CRef' instance with on listened, do removed it failed.", opcode);
                return;
            }

            NetworkHandler.Instance.RemoveMessageDispatchListener(opcode, this);
            _messageTypes.Remove(opcode);

            base.RemoveMessageListener(opcode);
        }

        /// <summary>
        /// 取消当前引用对象的所有注册的消息监听回调
        /// </summary>
        public override sealed void RemoveAllMessageListeners()
        {
            base.RemoveAllMessageListeners();

            NetworkHandler.Instance.RemoveAllMessageDispatchListeners(this);

            _messageTypes.Clear();
        }

        #endregion
    }
}
