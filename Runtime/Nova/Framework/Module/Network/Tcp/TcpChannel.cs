/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
/// Copyring (C) 2020 - 2022, Guangzhou Xinyuan Technology Co., Ltd.
/// Copyring (C) 2022 - 2023, Shanghai Bilibili Technology Co., Ltd.
/// Copyring (C) 2023 - 2024, Guangzhou Shiyue Network Technology Co., Ltd.
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
        private SystemSocket m_socket = null;

        private SystemSocketAsyncEventArgs m_readEventArgs = null;

        private SystemSocketAsyncEventArgs m_writeEventArgs = null;

        private readonly IO.CircularLinkedBuffer m_readBuffer = null;

        private readonly IO.CircularLinkedBuffer m_writeBuffer = null;

        /// <summary>
        /// 当前通道的包头长度
        /// </summary>
        private readonly int m_headerSize = 0;

        /// <summary>
        /// 数据包的包头缓冲区
        /// </summary>
        private readonly byte[] m_packetHeaderCached = null;


        private readonly SystemMemoryStream m_memoryStream = null;

        private readonly MessagePacket m_packet = null;

        private readonly SystemIPEndPoint m_remoteIP = null;

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
            this.m_headerSize = MessageConstant.HeaderSize2;
            this.m_packetHeaderCached = new byte[this.m_headerSize];

            this.m_readBuffer = new IO.CircularLinkedBuffer();
            this.m_writeBuffer = new IO.CircularLinkedBuffer();
            this.m_memoryStream = service.MemoryStreamManager.GetStream(name, ushort.MaxValue);

            this.m_socket = new SystemSocket(SystemAddressFamily.InterNetwork, SystemSocketType.Stream, SystemProtocolType.Tcp);
            this.m_socket.NoDelay = true;
            this.m_packet = new MessagePacket(this.m_headerSize, this.m_readBuffer, this.m_memoryStream);
            this.m_readEventArgs = new SystemSocketAsyncEventArgs();
            this.m_writeEventArgs = new SystemSocketAsyncEventArgs();
            this.m_readEventArgs.Completed += this.OnOperationComplete;
            this.m_writeEventArgs.Completed += this.OnOperationComplete;

            SystemIPEndPoint ip = Utility.Network.ToIPEndPoint(url);

            // this.m_url = ip.ToString();
            this.m_remoteIP = ip;
            this.m_isConnected = false;
            this.m_isOnWriting = false;
        }

        /// <summary>
        /// 网络通道关闭操作回调接口
        /// </summary>
        protected override void OnClose()
        {
            this.m_readEventArgs.Dispose();
            this.m_writeEventArgs.Dispose();
            this.m_readEventArgs = null;
            this.m_writeEventArgs = null;

            this.m_socket.Close();
            this.m_socket = null;

            this.m_readBuffer.Clear();
            this.m_writeBuffer.Clear();
            this.m_memoryStream.Dispose();

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
            this.m_memoryStream.Seek(MessageConstant.MessageIndex, SystemSeekOrigin.Begin);
            this.m_memoryStream.SetLength(message.Length);

            SystemArray.Copy(message, 0, this.m_memoryStream.GetBuffer(), 0, message.Length);

            this.Send(this.m_memoryStream);
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
            switch (this.m_headerSize)
            {
                case MessageConstant.HeaderSize4:
                    if (packetSize > ushort.MaxValue * 16)
                    {
                        throw new CFrameworkException("send packet size '{0}' too large.", packetSize);
                    }
                    this.m_packetHeaderCached.WriteTo(0, (int) packetSize);
                    break;
                case MessageConstant.HeaderSize2:
                    if (packetSize > ushort.MaxValue)
                    {
                        throw new CFrameworkException("send packet size '{0}' too large.", packetSize);
                    }
                    this.m_packetHeaderCached.WriteToBig(0, (short) packetSize);
                    break;
                default:
                    throw new CFrameworkException("packet size is invalid.");
            }

            this.m_writeBuffer.Write(this.m_packetHeaderCached, 0, this.m_packetHeaderCached.Length);
            this.m_writeBuffer.Write(memoryStream);

            // 记录当前通道为待发送状态
            ((TcpService) this.Service).WaitingForSend(this.m_channelID);
        }

        private void OnConnectAsync()
        {
            this.m_writeEventArgs.RemoteEndPoint = this.m_remoteIP;

            if (this.m_socket.ConnectAsync(this.m_writeEventArgs))
            {
                return;
            }

            this.OnConnectionComplete(this.m_writeEventArgs);
        }

        /// <summary>
        /// 接收数据处理操作接口
        /// </summary>
        private void OnRecv()
        {
            int size = IO.CircularLinkedBuffer.BUFFER_CHUNK_SIZE - this.m_readBuffer.LastIndex;
            this.OnRecvAsync(this.m_readBuffer.Last, this.m_readBuffer.LastIndex, size);
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
                this.m_readEventArgs.SetBuffer(buffer, offset, count);
            }
            catch (SystemException e)
            {
                throw new CFrameworkException("socket set buffer error.", e);
            }

            if (this.m_socket.ReceiveAsync(this.m_readEventArgs))
            {
                return;
            }
            this.OnRecvComplete(this.m_readEventArgs);
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
            if (0 == this.m_writeBuffer.Length)
            {
                this.m_isOnWriting = false;
                return;
            }

            this.m_isOnWriting = true;

            int size = IO.CircularLinkedBuffer.BUFFER_CHUNK_SIZE - this.m_writeBuffer.FirstIndex;
            if (size > this.m_writeBuffer.Length)
            {
                size = (int) this.m_writeBuffer.Length;
            }
            this.OnSendAsync(this.m_writeBuffer.First, this.m_writeBuffer.FirstIndex, size);
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
                this.m_writeEventArgs.SetBuffer(buffer, offset, count);
            }
            catch (SystemException e)
            {
                throw new CFrameworkException("socket set buffer error.", e);
            }

            if (this.m_socket.SendAsync(this.m_writeEventArgs))
            {
                return;
            }

            this.OnSendComplete(this.m_writeEventArgs);
        }

        private void OnConnectionComplete(object o)
        {
            // 连接已被清除
            if (null == this.m_socket)
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

            this.m_connectionCallback?.Invoke(this);
        }

        private void OnDisconnectionComplete(object o)
        {
            SystemSocketAsyncEventArgs e = (SystemSocketAsyncEventArgs) o;
            this.OnError((int) e.SocketError);
        }

        private void OnRecvComplete(object o)
        {
            if (null == this.m_socket)
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

            this.m_readBuffer.LastIndex += e.BytesTransferred;
            if (IO.CircularLinkedBuffer.BUFFER_CHUNK_SIZE == this.m_readBuffer.LastIndex)
            {
                this.m_readBuffer.AddLast();
                this.m_readBuffer.LastIndex = 0;
            }

            // 收到消息回调
            while (true)
            {
                try
                {
                    if (false == this.m_packet.ParsePacket())
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
                    this.m_readCallback.Invoke(this.m_packet.GetPacket(), MessageStreamCode.Byte);
                }
                catch (SystemException ee)
                {
                    Logger.Error(ee.ToString());
                }
            }

            if (null != this.m_socket)
            {
                this.OnRecv();
            }
        }

        private void OnSendComplete(object o)
        {
            if (null == this.m_socket)
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

            this.m_writeBuffer.FirstIndex += e.BytesTransferred;
            if (IO.CircularLinkedBuffer.BUFFER_CHUNK_SIZE == this.m_writeBuffer.FirstIndex)
            {
                this.m_writeBuffer.FirstIndex = 0;
                this.m_writeBuffer.RemoveFirst();
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
