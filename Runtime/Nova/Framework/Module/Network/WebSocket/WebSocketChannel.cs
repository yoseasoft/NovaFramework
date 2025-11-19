/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
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

// 使用系统库中的WebSocket类
// #define __USING_SystemNetWebsocket_LIBRARIES_TYPE

// 使用WebSocketSharp第三方库中的WebSocket类
// #define __USING_WebSocketSharp_LIBRARIES_TYPE

// 使用微信小程序官方平台提供的WebSocket类
#define __USING_UnityWebSocket_LIBRARIES_TYPE

// #define __USED_WEBSOCKET_LIBRARIES_TYPE

using System.Collections.Generic;
using System.Customize.Extension;

using SystemArray = System.Array;
using SystemUri = System.Uri;
using SystemMemoryStream = System.IO.MemoryStream;
using SystemSeekOrigin = System.IO.SeekOrigin;

#if __USING_SystemNetWebsocket_LIBRARIES_TYPE
using SystemCancellationTokenSource = System.Threading.CancellationTokenSource;
using SystemWebSocket = System.Net.WebSockets.WebSocket;
using SystemWebSocketState = System.Net.WebSockets.WebSocketState;
using SystemClientWebSocket = System.Net.WebSockets.ClientWebSocket;
using SystemWebSocketMessageType = System.Net.WebSockets.WebSocketMessageType;
using SystemHttpListenerWebSocketContext = System.Net.WebSockets.HttpListenerWebSocketContext;
using SystemWebSocketReceiveResult = System.Net.WebSockets.WebSocketReceiveResult;
using SystemValueWebSocketReceiveResult = System.Net.WebSockets.ValueWebSocketReceiveResult;
using SystemWebSocketCloseStatus = System.Net.WebSockets.WebSocketCloseStatus;
#endif

namespace NovaEngine
{
    /// <summary>
    /// WebSocket模式网络通道对象抽象基类
    /// </summary>
    public sealed partial class WebSocketChannel : NetworkChannel
    {
        /// <summary>
        /// 数据缓冲区的预分配空间大小
        /// </summary>
        private const int BUFFER_CHUNK_SIZE = 1024 * 4;
        /// <summary>
        /// 数据缓冲区池对象实例数量
        /// </summary>
        private const int BUFFER_POOL_SIZE = 10;

#if __USING_SystemNetWebsocket_LIBRARIES_TYPE

        private SystemClientWebSocket _webSocket;
        private SystemCancellationTokenSource _cancellationTokenSource;

        private readonly byte[] _readBuffer = null;

#elif __USING_WebSocketSharp_LIBRARIES_TYPE

        private WebSocketSharp.WebSocket _webSocket;

#elif __USING_UnityWebSocket_LIBRARIES_TYPE

        private UnityWebSocket.IWebSocket _webSocket;

#endif

        private readonly Queue<IO.MemoryBuffer> _writeBuffer = null;

        private readonly Queue<IO.MemoryBuffer> _bufferCached = null;

        /// <summary>
        /// 当前通道的包头长度
        /// </summary>
        private readonly int _headerSize = 0;

        /// <summary>
        /// 数据包的包头缓冲区
        /// </summary>
        private readonly byte[] _packetHeaderCached = null;


        private readonly SystemMemoryStream _memoryStream = null;

        private readonly MessagePacket _packet = null;

        /// <summary>
        /// 网络通道当前连接状态标识
        /// </summary>
        private bool _isConnected = false;

        /// <summary>
        /// 网络通道当前写入状态标识
        /// </summary>
        private bool _isOnWriting = false;

        /// <summary>
        /// 获取网络通道当前连接状态标识
        /// </summary>
        public bool IsConnected
        {
            get { return _isConnected; }
        }

        /// <summary>
        /// 获取网络通道当前写入状态标识
        /// </summary>
        public bool IsOnWriting
        {
            get { return _isOnWriting; }
        }

        /// <summary>
        /// TCP网络通道对象的新实例构建接口
        /// </summary>
        /// <param name="name">通道名称</param>
        /// <param name="url">网络地址参数</param>
        /// <param name="service">服务对象实例</param>
        public WebSocketChannel(string name, string url, WebSocketService service) : base(name, url, service)
        {
#if __USING_SystemNetWebsocket_LIBRARIES_TYPE
            this._readBuffer = new byte[ushort.MaxValue];
#endif
            this._writeBuffer = new Queue<IO.MemoryBuffer>();
            this._bufferCached = new Queue<IO.MemoryBuffer>();

            this._headerSize = MessageConstant.HeaderSize_4;
            this._packetHeaderCached = new byte[this._headerSize];

            this._memoryStream = service.MemoryStreamManager.GetStream(name, ushort.MaxValue);

            this._packet = new MessagePacket(this._headerSize, this._memoryStream, (m) =>
            {
                this._readCallback.Invoke(m, MessageStreamCode.Byte);
            });

#if __USING_SystemNetWebsocket_LIBRARIES_TYPE
            this._webSocket = new SystemClientWebSocket();
            this._cancellationTokenSource = new SystemCancellationTokenSource();
#elif __USING_WebSocketSharp_LIBRARIES_TYPE
            this._webSocket = new WebSocketSharp.WebSocket(url);

            this._webSocket.OnOpen += OnConnectionComplete;
            this._webSocket.OnClose += OnDisconnectionComplete;
            this._webSocket.OnError += OnConnectionError;
            this._webSocket.OnMessage += OnRecvMessage;
#elif __USING_UnityWebSocket_LIBRARIES_TYPE
            this._webSocket = new UnityWebSocket.WebSocket(url);

            this._webSocket.OnOpen += OnConnectionComplete;
            this._webSocket.OnClose += OnDisconnectionComplete;
            this._webSocket.OnError += OnConnectionError;
            this._webSocket.OnMessage += OnRecvMessage;
#endif

            this._isConnected = false;
            this._isOnWriting = false;
        }

        /// <summary>
        /// 网络通道关闭操作回调接口
        /// </summary>
        protected override void OnClose()
        {
#if __USING_SystemNetWebsocket_LIBRARIES_TYPE
            this._cancellationTokenSource.Cancel();
            this._cancellationTokenSource.Dispose();
            this._cancellationTokenSource = null;

            this._webSocket.Dispose();
            this._webSocket = null;
#elif __USING_WebSocketSharp_LIBRARIES_TYPE
            this._webSocket.Close();
            this._webSocket = null;
#elif __USING_UnityWebSocket_LIBRARIES_TYPE
            this._webSocket.CloseAsync();
            this._webSocket = null;
#endif

            this._memoryStream.Dispose();

            // this._readBuffer.Clear();
            foreach (IO.MemoryBuffer v in this._writeBuffer) { v.Dispose(); }
            this._writeBuffer.Clear();
            foreach (IO.MemoryBuffer v in this._bufferCached) { v.Dispose(); }
            this._bufferCached.Clear();

            base.OnClose();
        }

        /// <summary>
        /// 网络通道连接操作接口
        /// </summary>
        public override void Connect()
        {
            if (this._isConnected)
            {
                return;
            }

#if __USING_SystemNetWebsocket_LIBRARIES_TYPE
            OnConnectAsync();
#elif __USING_WebSocketSharp_LIBRARIES_TYPE
            _webSocket.ConnectAsync();
#elif __USING_UnityWebSocket_LIBRARIES_TYPE
            _webSocket.ConnectAsync();
#endif
        }

#if __USING_SystemNetWebsocket_LIBRARIES_TYPE
        private async void OnConnectAsync()
        {
            try
            {
                await _webSocket.ConnectAsync(new SystemUri(_url), _cancellationTokenSource.Token);

                // Logger.Info("The WebSocket connect state {0} for target url {1}.", _webSocket.State, _url);

                OnConnectionComplete();
            }
            catch (System.Exception exception)
            {
                Logger.Error(exception);

                OnConnectionError();
            }
        }

        /// <summary>
        /// 连接完成的回调通知接口函数
        /// </summary>
        private void OnConnectionComplete()
        {
            // 连接已被清除
            if (null == this._webSocket)
            {
                return;
            }

            if (_webSocket.State != SystemWebSocketState.Open)
            {
                this.OnError(NetworkErrorCode.SocketError);
                return;
            }

            this._isConnected = true;

            this.OnRecv();

            this._connectionCallback?.Invoke(this);
        }

        /// <summary>
        /// 连接错误的回调通知接口函数
        /// </summary>
        private void OnConnectionError()
        {
            this.OnError(NetworkErrorCode.SocketError);
        }
#elif __USING_WebSocketSharp_LIBRARIES_TYPE
        /// <summary>
        /// 连接完成的回调通知接口函数
        /// </summary>
        private void OnConnectionComplete(object sender, System.EventArgs e)
        {
            this._isConnected = true;

            this._connectionCallback?.Invoke(this);
        }

        private void OnDisconnectionComplete(object sender, WebSocketSharp.CloseEventArgs e)
        {
            this._isConnected = false;

            this._disconnectionCallback?.Invoke(this);
        }

        /// <summary>
        /// 连接错误的回调通知接口函数
        /// </summary>
        private void OnConnectionError(object sender, WebSocketSharp.ErrorEventArgs e)
        {
            this.OnError(NetworkErrorCode.SocketError);
        }
#elif __USING_UnityWebSocket_LIBRARIES_TYPE
        /// <summary>
        /// 连接完成的回调通知接口函数
        /// </summary>
        private void OnConnectionComplete(object sender, UnityWebSocket.OpenEventArgs e)
        {
            this._isConnected = true;

            this._connectionCallback?.Invoke(this);
        }

        private void OnDisconnectionComplete(object sender, UnityWebSocket.CloseEventArgs e)
        {
            this._isConnected = false;

            this._disconnectionCallback?.Invoke(this);
        }

        /// <summary>
        /// 连接错误的回调通知接口函数
        /// </summary>
        private void OnConnectionError(object sender, UnityWebSocket.ErrorEventArgs e)
        {
            this.OnError(NetworkErrorCode.SocketError);
        }
#endif

#if __USING_SystemNetWebsocket_LIBRARIES_TYPE
        /// <summary>
        /// 检测远程链接是否被关闭
        /// </summary>
        /// <returns>若远程链接被关闭返回true，否则返回false</returns>
        private bool IsRemoteClosed()
        {
            Logger.Assert(null != this._webSocket || null != this._cancellationTokenSource, ErrorText.InvalidArguments);

            if (false == this._isConnected) // IsClosed
            {
                return true;
            }

            if (this._webSocket.State != SystemWebSocketState.Open || this._cancellationTokenSource.IsCancellationRequested)
            {
                return true;
            }

            if (this._webSocket.CloseStatus.HasValue && this._webSocket.CloseStatus.Value != SystemWebSocketCloseStatus.Empty)
            {
                Logger.Info("The WebSocket was closed by status {0}.", this._webSocket.CloseStatus.Value);
                return true;
            }

            return false;
        }

#endif

        /// <summary>
        /// 网络通道数据下行操作接口
        /// </summary>
        /// <param name="message">消息内容</param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public override void Send(string message)
        {
            Send(Utility.Convertion.GetBytes(message));
        }

        /// <summary>
        /// 网络通道数据下行操作接口
        /// </summary>
        /// <param name="message">消息内容</param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public override void Send(byte[] message)
        {
            Send(message, 0, message.Length);
        }

        /// <summary>
        /// 网络通道数据下行操作接口
        /// </summary>
        /// <param name="memoryStream">消息数据流</param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public override void Send(SystemMemoryStream memoryStream)
        {
            Send(memoryStream.GetBuffer(), 0, (int) memoryStream.Length);
        }

        /// <summary>
        /// 网络通道数据下行操作接口
        /// </summary>
        /// <param name="message">消息内容</param>
        /// <param name="offset">偏移参数</param>
        /// <param name="count">字节长度</param>
        private void Send(byte[] message, int offset, int count)
        {
            // 写入包头长度到缓冲区
            int packetSize = count;
            switch (this._headerSize)
            {
                case MessageConstant.HeaderSize_4:
                    if (packetSize > MessageConstant.MaxPacketSize_4)
                    {
                        throw new CFrameworkException("send packet size '{%d}' too large.", packetSize);
                    }
                    this._packetHeaderCached.WriteTo(0, (int) packetSize);
                    break;
                case MessageConstant.HeaderSize_2:
                    if (packetSize > MessageConstant.MaxPacketSize_2)
                    {
                        throw new CFrameworkException("send packet size '{%d}' too large.", packetSize);
                    }
                    this._packetHeaderCached.WriteToBig(0, (short) packetSize);
                    break;
                default:
                    throw new CFrameworkException("packet size is invalid.");
            }

            IO.MemoryBuffer buffer = Acquire();
            buffer.Write(this._packetHeaderCached, 0, this._headerSize);
            buffer.Write(message, 0, count);

            Send(buffer);
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        private void Send(IO.MemoryBuffer memoryBuffer)
        {
            this._writeBuffer.Enqueue(memoryBuffer);

            // 记录当前通道为待发送状态
            ((WebSocketService) this.Service).WaitingForSend(this._channelID);
        }

        /// <summary>
        /// 发送数据处理操作接口
        /// </summary>
        internal void OnSend()
        {
#if __USING_SystemNetWebsocket_LIBRARIES_TYPE
            // if (false == this._isConnected)
            if (IsRemoteClosed())
#elif __USING_WebSocketSharp_LIBRARIES_TYPE || __USING_UnityWebSocket_LIBRARIES_TYPE
            if (!IsConnected)
#endif
            {
                this.OnError(NetworkErrorCode.RemoteDisconnect);
                return;
            }

            // 没有待写入数据
            if (0 == this._writeBuffer.Count)
            {
                this._isOnWriting = false;
                return;
            }

            this._isOnWriting = true;

            this.OnSendAsync();
        }

        /// <summary>
        /// 异步发送数据操作接口
        /// </summary>
        /// <param name="buffer">数据流引用</param>
        /// <param name="offset">数据流偏移位置</param>
        /// <param name="count">数据流字节长度</param>
#if __USING_SystemNetWebsocket_LIBRARIES_TYPE
        private async void OnSendAsync()
#elif __USING_WebSocketSharp_LIBRARIES_TYPE || __USING_UnityWebSocket_LIBRARIES_TYPE
        private void OnSendAsync()
#endif
        {
            while (this._writeBuffer.Count > 0)
            {
                IO.MemoryBuffer stream = this._writeBuffer.Dequeue();

                try
                {
                    stream.Seek(0, SystemSeekOrigin.Begin);

#if __USING_SystemNetWebsocket_LIBRARIES_TYPE
                    // SystemCancellationTokenSource tokenSource = new SystemCancellationTokenSource();
                    // tokenSource.CancelAfter(5000);
                    // await this._webSocket.SendAsync(stream.GetMemory(), SystemWebSocketMessageType.Text, true, tokenSource.Token);
                    await this._webSocket.SendAsync(stream.GetMemory(), SystemWebSocketMessageType.Text, true, this._cancellationTokenSource.Token);
#elif __USING_WebSocketSharp_LIBRARIES_TYPE
                    this._webSocket.Send(stream, (int) stream.Length);
#elif __USING_UnityWebSocket_LIBRARIES_TYPE
                    this._webSocket.SendAsync(stream.GetMemory().ToArray());
#endif
                }
                catch (System.Exception e)
                {
                    Logger.Error(e);

                    this.OnError(NetworkErrorCode.SocketError);
                }
                finally
                {
                    Recycle(stream);
                }
            }

            this._isOnWriting = false;
        }

#if __USING_SystemNetWebsocket_LIBRARIES_TYPE
        /// <summary>
        /// 接收数据处理操作接口
        /// </summary>
        private void OnRecv()
        {
            OnRecvAsync();
        }

        private async void OnRecvAsync()
        {
            SystemValueWebSocketReceiveResult result;
            int receiveCount = 0;
            do
            {
                result = await _webSocket.ReceiveAsync(new System.Memory<byte>(_readBuffer, receiveCount, _readBuffer.Length - receiveCount),
                        this._cancellationTokenSource.Token);
                receiveCount += result.Count;
            } while (false == result.EndOfMessage);

            if (result.MessageType == SystemWebSocketMessageType.Close)
            {
                this.OnError(NetworkErrorCode.PacketReadError);
                return;
            }

            if (receiveCount > _readBuffer.Length)
            {
                await this._webSocket.CloseAsync(SystemWebSocketCloseStatus.MessageTooBig, $"message too big: {receiveCount}",
                        this._cancellationTokenSource.Token);
                this.OnError(NetworkErrorCode.PacketReadError);
                return;
            }

            // Logger.Info($"Received Data = {System.Text.Encoding.UTF8.GetString(_readBuffer, 0, result.Count)}, Count = {result.Count}, Result CloseStatus = {result.MessageType}.");

            this.OnRecvComplete(receiveCount);
        }

        private void OnRecvComplete(int recvSize)
        {
            if (IsRemoteClosed())
            {
                this.OnError(NetworkErrorCode.RemoteDisconnect);
                return;
            }

            try
            {
                IO.MemoryBuffer buffer = Acquire();
                buffer.Write(this._readBuffer, 0, recvSize);

                buffer.Seek(0, SystemSeekOrigin.Begin);

                // this._readCallback.Invoke(buffer, MessageStreamCode.Byte);
                this._packet.OnDispatched(buffer);

                Recycle(buffer);
            }
            catch (System.Exception exception)
            {
                Logger.Error(exception);
            }

            this.OnRecv();
        }
#elif __USING_WebSocketSharp_LIBRARIES_TYPE
        /// <summary>
        /// 接收数据处理操作接口
        /// </summary>
        private void OnRecvMessage(object sender, WebSocketSharp.MessageEventArgs e)
        {
            if (e.IsPing)
            {
                Logger.Info("The WebSocket channel was received ping notify now.");
                return;
            }

            IO.MemoryBuffer buffer = Acquire();
            buffer.Write(e.RawData, 0, e.RawData.Length);

            buffer.Seek(0, SystemSeekOrigin.Begin);

            // this._readCallback.Invoke(buffer, MessageStreamCode.Byte);
            this._packet.OnDispatched(buffer);

            Recycle(buffer);

            // Logger.Info($"Received Data = {e.Data.Length}, Count = {e.RawData.Length}, Opcode = {e.Opcode}.");
        }
#elif __USING_UnityWebSocket_LIBRARIES_TYPE
        /// <summary>
        /// 接收数据处理操作接口
        /// </summary>
        private void OnRecvMessage(object sender, UnityWebSocket.MessageEventArgs e)
        {
            IO.MemoryBuffer buffer = Acquire();

            if (e.IsBinary)
            {
                buffer.Write(e.RawData, 0, e.RawData.Length);
            }
            else if (e.IsText)
            {
                // 2024-05-30:
                // 服务端发送的文本格式信息，接收过程中发生异常从而关闭链接，到底是什么原因导致的呢？
                // 出错原因尚未找到，暂时使用二进制形式进行消息传输，这里的问题后面有时间再说
                buffer.Write(System.Text.Encoding.UTF8.GetBytes(e.Data), 0, e.RawData.Length);

                throw new CFrameworkException("Unknown error!");
            }
            else
            {
                Recycle(buffer);
                this.OnError(NetworkErrorCode.PacketReadError);
                return;
            }

            buffer.Seek(0, SystemSeekOrigin.Begin);

            // this._readCallback.Invoke(buffer, MessageStreamCode.Byte);
            this._packet.OnDispatched(buffer);

            Recycle(buffer);

            // Logger.Info($"Received Data Count = {e.RawData.Length}, Opcode = {e.Opcode}, Data = {e.Data}.");
        }
#endif

        /// <summary>
        /// 分配指定空间大小的数据缓冲区
        /// </summary>
        /// <param name="size">空间大小</param>
        /// <returns>返回新配发的缓冲区实例</returns>
        private IO.MemoryBuffer Acquire(int size = 0)
        {
            if (size > BUFFER_CHUNK_SIZE)
            {
                return new IO.MemoryBuffer(size);
            }

            if (size < BUFFER_CHUNK_SIZE)
            {
                size = BUFFER_CHUNK_SIZE;
            }

            if (this._bufferCached.Count == 0)
            {
                return new IO.MemoryBuffer(size);
            }

            return _bufferCached.Dequeue();
        }

        /// <summary>
        /// 回收指定的数据缓冲区实例
        /// </summary>
        /// <param name="memoryBuffer">缓冲区实例</param>
        private void Recycle(IO.MemoryBuffer memoryBuffer)
        {
            if (memoryBuffer.Capacity > BUFFER_CHUNK_SIZE)
            {
                return;
            }

            if (this._bufferCached.Count > BUFFER_POOL_SIZE) // 这里不需要太大，其实Kcp跟Tcp,这里1就足够了
            {
                return;
            }

            memoryBuffer.Seek(0, SystemSeekOrigin.Begin);
            memoryBuffer.SetLength(0);

            this._bufferCached.Enqueue(memoryBuffer);
        }
    }
}
