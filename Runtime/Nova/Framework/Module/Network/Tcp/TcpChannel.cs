/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
/// Copyright (C) 2020 - 2022, Guangzhou Xinyuan Technology Co., Ltd.
/// Copyright (C) 2022 - 2023, Shanghai Bilibili Technology Co., Ltd.
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

using SystemException = System.Exception;
using SystemArray = System.Array;
using SystemMemoryStream = System.IO.MemoryStream;
using SystemSeekOrigin = System.IO.SeekOrigin;
using SystemIPEndPoint = System.Net.IPEndPoint;
using SystemAddressFamily = System.Net.Sockets.AddressFamily;
using SystemSocket = System.Net.Sockets.Socket;
using SystemSocketType = System.Net.Sockets.SocketType;
using SystemSocketError = System.Net.Sockets.SocketError;
using SystemProtocolType = System.Net.Sockets.ProtocolType;
using SystemSocketAsyncEventArgs = System.Net.Sockets.SocketAsyncEventArgs;
using SystemSocketAsyncOperation = System.Net.Sockets.SocketAsyncOperation;

namespace NovaEngine
{
    /// <summary>
    /// TCP模式网络通道对象抽象基类
    /// </summary>
    public sealed partial class TcpChannel : NetworkChannel
    {
        private SystemSocket _socket = null;

        private SystemSocketAsyncEventArgs _readEventArgs = null;

        private SystemSocketAsyncEventArgs _writeEventArgs = null;

        private readonly IO.CircularLinkedBuffer _readBuffer = null;

        private readonly IO.CircularLinkedBuffer _writeBuffer = null;

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

        private readonly SystemIPEndPoint _remoteIp = null;

        /// <summary>
        /// 网络通道当前连接状态标识
        /// </summary>
        private bool m_isConnected = false;

        /// <summary>
        /// 网络通道当前写入状态标识
        /// </summary>
        private bool m_isOnWriting = false;

        /// <summary>
        /// 获取网络通道当前连接状态标识
        /// </summary>
        public bool IsConnected
        {
            get { return m_isConnected; }
        }

        /// <summary>
        /// 获取网络通道当前写入状态标识
        /// </summary>
        public bool IsOnWriting
        {
            get { return m_isOnWriting; }
        }

        /// <summary>
        /// TCP网络通道对象的新实例构建接口
        /// </summary>
        /// <param name="name">通道名称</param>
        /// <param name="url">网络地址参数</param>
        /// <param name="service">服务对象实例</param>
        public TcpChannel(string name, string url, TcpService service) : base(name, url, service)
        {
            this._headerSize = MessageConstant.HeaderSize2;
            this._packetHeaderCached = new byte[this._headerSize];

            this._readBuffer = new IO.CircularLinkedBuffer();
            this._writeBuffer = new IO.CircularLinkedBuffer();
            this._memoryStream = service.MemoryStreamManager.GetStream(name, ushort.MaxValue);

            this._socket = new SystemSocket(SystemAddressFamily.InterNetwork, SystemSocketType.Stream, SystemProtocolType.Tcp);
            this._socket.NoDelay = true;
            this._packet = new MessagePacket(this._headerSize, this._readBuffer, this._memoryStream);
            this._readEventArgs = new SystemSocketAsyncEventArgs();
            this._writeEventArgs = new SystemSocketAsyncEventArgs();
            this._readEventArgs.Completed += this.OnOperationComplete;
            this._writeEventArgs.Completed += this.OnOperationComplete;

            SystemIPEndPoint ip = Utility.Network.ToIPEndPoint(url);

            // this.m_url = ip.ToString();
            this._remoteIp = ip;
            this.m_isConnected = false;
            this.m_isOnWriting = false;
        }

        /// <summary>
        /// 网络通道关闭操作回调接口
        /// </summary>
        protected override void OnClose()
        {
            this._readEventArgs.Dispose();
            this._writeEventArgs.Dispose();
            this._readEventArgs = null;
            this._writeEventArgs = null;

            this._socket.Close();
            this._socket = null;

            this._readBuffer.Clear();
            this._writeBuffer.Clear();
            this._memoryStream.Dispose();

            base.OnClose();
        }

        /// <summary>
        /// 网络通道连接操作接口
        /// </summary>
        public override void Connect()
        {
            this.OnConnectAsync();
        }

        /// <summary>
        /// 网络通道数据下行操作接口
        /// </summary>
        /// <param name="message">消息内容</param>
        public override void Send(string message)
        {
            Send(Utility.Convertion.GetBytes(message));
        }

        /// <summary>
        /// 网络通道数据下行操作接口
        /// </summary>
        /// <param name="message">消息内容</param>
        public override void Send(byte[] message)
        {
            this._memoryStream.Seek(MessageConstant.MessageIndex, SystemSeekOrigin.Begin);
            this._memoryStream.SetLength(message.Length);

            SystemArray.Copy(message, 0, this._memoryStream.GetBuffer(), 0, message.Length);

            this.Send(this._memoryStream);
        }

        /// <summary>
        /// 网络通道数据下行操作接口
        /// </summary>
        /// <param name="memoryStream">消息数据流</param>
        public override void Send(SystemMemoryStream memoryStream)
        {
            if (this.IsClosed)
            {
                throw new CFrameworkException("Channel '{0}' is closed on TCP mode.", this.ChannelID);
            }

            // 写入包头长度到缓冲区
            int packetSize = (int) memoryStream.Length;
            switch (this._headerSize)
            {
                case MessageConstant.HeaderSize4:
                    if (packetSize > ushort.MaxValue * 16)
                    {
                        throw new CFrameworkException("send packet size '{0}' too large.", packetSize);
                    }
                    this._packetHeaderCached.WriteTo(0, (int) packetSize);
                    break;
                case MessageConstant.HeaderSize2:
                    if (packetSize > ushort.MaxValue)
                    {
                        throw new CFrameworkException("send packet size '{0}' too large.", packetSize);
                    }
                    this._packetHeaderCached.WriteToBig(0, (short) packetSize);
                    break;
                default:
                    throw new CFrameworkException("packet size is invalid.");
            }

            this._writeBuffer.Write(this._packetHeaderCached, 0, this._packetHeaderCached.Length);
            this._writeBuffer.Write(memoryStream);

            // 记录当前通道为待发送状态
            ((TcpService) this.Service).WaitingForSend(this._channelID);
        }

        private void OnConnectAsync()
        {
            this._writeEventArgs.RemoteEndPoint = this._remoteIp;

            if (this._socket.ConnectAsync(this._writeEventArgs))
            {
                return;
            }

            this.OnConnectionComplete(this._writeEventArgs);
        }

        /// <summary>
        /// 接收数据处理操作接口
        /// </summary>
        private void OnRecv()
        {
            int size = IO.CircularLinkedBuffer.BUFFER_CHUNK_SIZE - this._readBuffer.LastIndex;
            this.OnRecvAsync(this._readBuffer.Last, this._readBuffer.LastIndex, size);
        }

        /// <summary>
        /// 接收数据异步操作接口
        /// </summary>
        /// <param name="buffer">数据流引用</param>
        /// <param name="offset">数据流偏移位置</param>
        /// <param name="count">数据流字节长度</param>
        private void OnRecvAsync(byte[] buffer, int offset, int count)
        {
            try
            {
                this._readEventArgs.SetBuffer(buffer, offset, count);
            }
            catch (SystemException e)
            {
                throw new CFrameworkException("socket set buffer error.", e);
            }

            if (this._socket.ReceiveAsync(this._readEventArgs))
            {
                return;
            }
            this.OnRecvComplete(this._readEventArgs);
        }

        /// <summary>
        /// 发送数据处理操作接口
        /// </summary>
        internal void OnSend()
        {
            if (false == this.m_isConnected)
            {
                return;
            }

            // 没有待写入数据
            if (0 == this._writeBuffer.Length)
            {
                this.m_isOnWriting = false;
                return;
            }

            this.m_isOnWriting = true;

            int size = IO.CircularLinkedBuffer.BUFFER_CHUNK_SIZE - this._writeBuffer.FirstIndex;
            if (size > this._writeBuffer.Length)
            {
                size = (int) this._writeBuffer.Length;
            }
            this.OnSendAsync(this._writeBuffer.First, this._writeBuffer.FirstIndex, size);
        }

        /// <summary>
        /// 异步发送数据操作接口
        /// </summary>
        /// <param name="buffer">数据流引用</param>
        /// <param name="offset">数据流偏移位置</param>
        /// <param name="count">数据流字节长度</param>
        private void OnSendAsync(byte[] buffer, int offset, int count)
        {
            try
            {
                this._writeEventArgs.SetBuffer(buffer, offset, count);
            }
            catch (SystemException e)
            {
                throw new CFrameworkException("socket set buffer error.", e);
            }

            if (this._socket.SendAsync(this._writeEventArgs))
            {
                return;
            }

            this.OnSendComplete(this._writeEventArgs);
        }

        private void OnConnectionComplete(object o)
        {
            // 连接已被清除
            if (null == this._socket)
            {
                return;
            }

            SystemSocketAsyncEventArgs e = (SystemSocketAsyncEventArgs) o;
            if (SystemSocketError.Success != e.SocketError)
            {
                this.OnError((int) e.SocketError);
                return;
            }

            e.RemoteEndPoint = null;
            this.m_isConnected = true;

            this.OnRecv();

            this._connectionCallback?.Invoke(this);
        }

        private void OnDisconnectionComplete(object o)
        {
            SystemSocketAsyncEventArgs e = (SystemSocketAsyncEventArgs) o;
            this.OnError((int) e.SocketError);
        }

        private void OnRecvComplete(object o)
        {
            if (null == this._socket)
            {
                return;
            }

            SystemSocketAsyncEventArgs e = (SystemSocketAsyncEventArgs) o;
            if (SystemSocketError.Success != e.SocketError)
            {
                this.OnError((int) e.SocketError);
                return;
            }

            if (0 == e.BytesTransferred)
            {
                this.OnError(NetworkErrorCode.RemoteDisconnect);
                return;
            }

            this._readBuffer.LastIndex += e.BytesTransferred;
            if (IO.CircularLinkedBuffer.BUFFER_CHUNK_SIZE == this._readBuffer.LastIndex)
            {
                this._readBuffer.AddLast();
                this._readBuffer.LastIndex = 0;
            }

            // 收到消息回调
            while (true)
            {
                try
                {
                    if (false == this._packet.ParsePacket())
                    {
                        break;
                    }
                }
                catch (SystemException ee)
                {
                    Logger.Error("receive bytes parse failed '{0}'.", ee.ToString());

                    this.OnError(NetworkErrorCode.SocketError);
                    return;
                }

                try
                {
                    this._readCallback.Invoke(this._packet.GetPacket(), MessageStreamCode.Byte);
                }
                catch (SystemException ee)
                {
                    Logger.Error(ee.ToString());
                }
            }

            if (null != this._socket)
            {
                this.OnRecv();
            }
        }

        private void OnSendComplete(object o)
        {
            if (null == this._socket)
            {
                return;
            }

            SystemSocketAsyncEventArgs e = (SystemSocketAsyncEventArgs) o;
            if (SystemSocketError.Success != e.SocketError)
            {
                this.OnError((int) e.SocketError);
                return;
            }

            if (0 == e.BytesTransferred)
            {
                this.OnError(NetworkErrorCode.RemoteDisconnect);
                return;
            }

            this._writeBuffer.FirstIndex += e.BytesTransferred;
            if (IO.CircularLinkedBuffer.BUFFER_CHUNK_SIZE == this._writeBuffer.FirstIndex)
            {
                this._writeBuffer.FirstIndex = 0;
                this._writeBuffer.RemoveFirst();
            }

            this.OnSend();
        }

        private void OnOperationComplete(object sender, SystemSocketAsyncEventArgs e)
        {
            switch (e.LastOperation)
            {
                case SystemSocketAsyncOperation.Connect:
                    ModuleController.QueueOnMainThread(this.OnConnectionComplete, e);
                    break;
                case SystemSocketAsyncOperation.Disconnect:
                    ModuleController.QueueOnMainThread(this.OnDisconnectionComplete, e);
                    break;
                case SystemSocketAsyncOperation.Receive:
                    ModuleController.QueueOnMainThread(this.OnRecvComplete, e);
                    break;
                case SystemSocketAsyncOperation.Send:
                    ModuleController.QueueOnMainThread(this.OnSendComplete, e);
                    break;
                default:
                    throw new CFrameworkException("socket error '{0}'.", e.LastOperation);
            }
        }
    }
}
