/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyright (C) 2023 - 2024, Guangzhou Shiyue Network Technology Co., Ltd.
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

using Cysharp.Threading.Tasks;

using SystemType = System.Type;

using UniTaskForMessage = Cysharp.Threading.Tasks.UniTask<object>;
using UniTaskCompletionSourceForMessage = Cysharp.Threading.Tasks.UniTaskCompletionSource<object>;

namespace GameEngine
{
    /// <summary>
    /// 对套接字模式的网络通道进行封装后的消息通信对象类
    /// </summary>
    public abstract class SocketMessageChannel : MessageChannel
    {
        /// <summary>
        /// 网络套接字连接模式下的接收超时时间（以毫秒为单位）
        /// </summary>
        private const int SOCKET_CONNECTION_RECEIVED_TIME_SPAN_OF_TIMEOUT = 3000; // milliseconds

        /// <summary>
        /// 异步消息接收的缓冲容器
        /// </summary>
        private IDictionary<int, UniTaskCompletionSourceForMessage> _waitingForResponseMessages = null;

        protected SocketMessageChannel(int channelID, int channelType) : base(channelID, channelType)
        { }

        protected SocketMessageChannel(int channelID, int channelType, string name) : base(channelID, channelType, name)
        { }

        protected SocketMessageChannel(int channelID, int channelType, string name, string url) : base(channelID, channelType, name, url)
        { }

        /// <summary>
        /// 通道对象初始化函数接口
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            // 初始化异步消息缓冲容器
            _waitingForResponseMessages = new Dictionary<int, UniTaskCompletionSourceForMessage>();
        }

        /// <summary>
        /// 通道对象清理函数接口
        /// </summary>
        protected override void Cleanup()
        {
            int[] codes = NovaEngine.Utility.Collection.ToArrayForKeys<int, UniTaskCompletionSourceForMessage>(_waitingForResponseMessages);
            for (int n = 0; null != codes && n < codes.Length; ++n)
            {
                _waitingForResponseMessages[codes[n]].TrySetCanceled();
            }

            // 清理异步消息缓冲容器
            _waitingForResponseMessages.Clear();
            _waitingForResponseMessages = null;

            base.Cleanup();
        }

        /// <summary>
        /// 检测当前消息通道是否正在待命指定的目标协议操作码
        /// </summary>
        /// <param name="opcode">协议操作码</param>
        /// <returns>若正在待命给定的协议操作码则返回true，否则返回false</returns>
        internal bool IsWaitingForTargetCode(int opcode)
        {
            if (_waitingForResponseMessages.ContainsKey(opcode))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 向当前消息通道进行消息派发操作
        /// </summary>
        /// <param name="message">消息内容</param>
        internal void OnMessageDispatched(int opcode, object message)
        {
            Debugger.Assert(_waitingForResponseMessages.ContainsKey(opcode), "The waiting for response message was invalid opcode '{%d}'.", opcode);

            _waitingForResponseMessages[opcode].TrySetResult(message as object);
        }

        public void Send(object message)
        {
            byte[] buffer = _messageTranslator.Encode(message);
            InternalSendMessage(buffer);
        }

        public UniTaskForMessage SendAwait(object message)
        {
            int opcode = NetworkHandler.Instance.GetOpcodeByMessageType(message.GetType());

            Send(message);

            return SendAwaitProcess(opcode);
        }

        /// <summary>
        /// 异步形式发送指定的消息码
        /// </summary>
        /// <param name="opcode">消息操作码</param>
        /// <returns>返回异步任务对象实例</returns>
        private async UniTaskForMessage SendAwaitProcess(int opcode)
        {
            Loader.Structuring.NetworkMessageCodeInfo codeInfo = GetMessageCodeInfoByType(opcode);
            int responseCode = 0;

            if (null != codeInfo)
            {
                responseCode = codeInfo.ResponseCode;
            }

            if (responseCode <= 0)
            {
                Debugger.Warn("Could not found any response code with target message type '{%d}'.", opcode);

                // 如果你想在一个异步UniTask方法中取消行为，请手动抛出OperationCanceledException：
                throw new System.OperationCanceledException();
            }

            var (failed, result) = await SendAwaitOfTargetResponse(responseCode).TimeoutWithoutException<object>(System.TimeSpan.FromMilliseconds(SOCKET_CONNECTION_RECEIVED_TIME_SPAN_OF_TIMEOUT));
            if (failed)
            {
                Debugger.Warn("Send target message opcode '{%d}' was timeout, awaited it response failed.", opcode);

                Debugger.Assert(_waitingForResponseMessages.ContainsKey(responseCode), "The waiting for response message was invalid opcode '{%d}'.", opcode);
                _waitingForResponseMessages.Remove(responseCode);

                return null;
            }

            return result;
        }

        /// <summary>
        /// 发送消息后的等待过程处理函数
        /// </summary>
        /// <param name="responseCode">消息响应码</param>
        /// <returns>返回异步任务对象实例</returns>
        private UniTaskForMessage SendAwaitOfTargetResponse(int responseCode)
        {
            Debugger.Assert(false == _waitingForResponseMessages.ContainsKey(responseCode), "The channel was waiting for target code '{%d}' now.", responseCode);

            UniTaskCompletionSourceForMessage completionSource = new UniTaskCompletionSourceForMessage();
            // 注册成功回调
            completionSource.OnCompleted(o => { _waitingForResponseMessages.Remove((int) o); }, responseCode, 1);

            _waitingForResponseMessages.Add(responseCode, completionSource);

            return completionSource.Task;
        }

        /// <summary>
        /// 通过指定的消息类型获取对应的消息编码结构信息
        /// </summary>
        /// <param name="opcode">消息操作码</param>
        /// <returns>返回给定类型对应的消息编码结构信息，若不存在则返回null</returns>
        private Loader.Structuring.NetworkMessageCodeInfo GetMessageCodeInfoByType(int opcode)
        {
            SystemType messageType = NetworkHandler.Instance.GetMessageClassByType(opcode);
            if (null == messageType)
            {
                Debugger.Warn("Could not found any message class with target type '{%d}', getting the code info failed.", opcode);
                return null;
            }

            Loader.Structuring.GeneralCodeInfo codeInfo = Loader.CodeLoader.LookupGeneralCodeInfo(messageType, typeof(MessageObjectAttribute));
            if (null == codeInfo)
            {
                Debugger.Warn("Could not found any message code info with target class '{%t}', please check the message loader process at first.", messageType);
                return null;
            }

            return codeInfo as Loader.Structuring.NetworkMessageCodeInfo;
        }
    }
}
