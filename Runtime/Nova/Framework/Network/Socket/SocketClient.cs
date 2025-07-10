/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
/// Copyright (C) 2017 - 2020, Shanghai Tommon Network Technology Co., Ltd.
/// Copyright (C) 2020 - 2022, Guangzhou Xinyuan Technology Co., Ltd.
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
using SystemBitConverter = System.BitConverter;
using SystemAsyncCallback = System.AsyncCallback;
using SystemIAsyncResult = System.IAsyncResult;
using SystemSeekOrigin = System.IO.SeekOrigin;
using SystemBinaryReader = System.IO.BinaryReader;
using SystemBinaryWriter = System.IO.BinaryWriter;
using SystemMemoryStream = System.IO.MemoryStream;
using SystemDns = System.Net.Dns;
using SystemIPAddress = System.Net.IPAddress;
using SystemAddressFamily = System.Net.Sockets.AddressFamily;
using SystemNetworkStream = System.Net.Sockets.NetworkStream;
using SystemTcpClient = System.Net.Sockets.TcpClient;
using SystemUdpClient = System.Net.Sockets.UdpClient;

namespace NovaEngine.Network
{
    /// <summary>
    /// 基于SOCKET模式的网络连接客户端，用于处于TCP/UDP模式下的网络连接及消息处理
    /// 所有的处理结果均采用事件方式发送到网络管理器中，由网络管理器进行统一转发
    /// </summary>
    public sealed class SocketClient
    {
        /// <summary>
        /// 当前SOCKET网络下行数据读取最大缓冲区数量
        /// </summary>
        private const int MAX_READBUFFER_SIZE = 4096;

        /// <summary>
        /// 连接超时时间，以秒为单位
        /// </summary>
        private const float CONNECT_TIMEOUT = 5f;

        // 当前SOCKET连接互斥锁
        private static object _locked = new object();

        private ISocketCall _notification = null;

        private SystemTcpClient _tcpClient = null;
        private SystemNetworkStream _outStream = null;

        private SystemMemoryStream _memStream = null;
        private SystemBinaryReader _reader = null;
        private SystemAsyncCallback _readCallback = null;
        private SystemAsyncCallback _writeCallback = null;

        private byte[] _buffer = new byte[MAX_READBUFFER_SIZE];

        /// <summary>
        /// 当前链接是否关闭状态标识
        /// </summary>
        private bool _isClosed = false;

        /// <summary>
        /// 自定义链接描述符序列标识
        /// </summary>
        private int _sequenceId = 0;

        public bool IsClosed
        {
            get
            {
                return _isClosed;
            }
        }

        public int SequenceId
        {
            get 
            {
                return _sequenceId;
            }
        }

        public SocketClient(int id, ISocketCall handler)
        {
            // 必须传入上层管理容器用于回调通知
            Logger.Assert(null != handler);
            _notification = handler;
            _sequenceId = id;

            _memStream = new SystemMemoryStream();
            _reader = new SystemBinaryReader(_memStream);
            _readCallback = new SystemAsyncCallback(OnRead);
            _writeCallback = new SystemAsyncCallback(OnWrite);

            // 初始默认为关闭状态
            _isClosed = true;
        }

        ~SocketClient()
        {
            this.Disconnect();

            _reader.Close();
            _reader = null;
            _memStream.Close();
            _memStream = null;

            _notification = null;
            _readCallback = null;
            _writeCallback = null;
        }

        /// <summary>
        /// 重置当前的读写数据流
        /// </summary>
        private void ResetStream()
        {
            if (null != _reader)
            {
                _reader.Close();
                _reader = null;

                _memStream.Close();
                _memStream = null;
            }

            _memStream = new SystemMemoryStream();
            _reader = new SystemBinaryReader(_memStream);
        }

        /// <summary>
        /// 连接到目标网络终端的执行接口
        /// </summary>
        /// <param name="host">网络地址</param>
        /// <param name="port">网络端口</param>
        public int Connect(string host, int port)
        {
            _isClosed = false;

            try
            {
                this.ResetStream();

                SystemIPAddress[] address = SystemDns.GetHostAddresses(host);
                if (address.Length == 0)
                {
                    Logger.Error("请求网络连接的主机地址{0}格式错误！", host);
                    return 0;
                }

                if (address[0].AddressFamily == SystemAddressFamily.InterNetworkV6) // IPv6的连接模式
                {
                    _tcpClient = new SystemTcpClient(SystemAddressFamily.InterNetworkV6);
                }
                else // IPv4的连接模式
                {
                    _tcpClient = new SystemTcpClient(SystemAddressFamily.InterNetwork);
                }

                _tcpClient.SendTimeout = 1000;
                _tcpClient.ReceiveTimeout = 1000;
                _tcpClient.NoDelay = true;
                _tcpClient.BeginConnect(host, port, new SystemAsyncCallback(OnConnection), null);

                ModuleController.QueueOnMainThread(delegate () {
                    if (false == _isClosed && null != _tcpClient && false == _tcpClient.Connected)
                    {
                        this.OnConnectError(_sequenceId);
                    }
                }, CONNECT_TIMEOUT);
                return _sequenceId;
            } catch (SystemException e)
            {
                Logger.Error(e.Message);

                this.Disconnect();
                return 0;
            }
        }

        /// <summary>
        /// 关闭当前socket连接
        /// </summary>
        public void Close()
        {
            _isClosed = true;

            try
            {
                this.Disconnect();
            } catch (SystemException e)
            {
                Logger.Error(e.Message);
            }
        }

        /// <summary>
        /// 切断当前目标网络终端连接的执行接口
        /// </summary>
        public void Disconnect()
        {
            _isClosed = true;

            if (null != _tcpClient)
            {
                if (_tcpClient.Connected)
                {
                    _tcpClient.Close();
                }
                
                _tcpClient = null;
            }
        }

        /// <summary>
        /// 检查当前的网络是否处于连接状态
        /// </summary>
        /// <returns>若当前网络处于连接状态则返回true，否则返回false</returns>
        public bool IsConnected()
        {
            if (null == _tcpClient)
            {
                return false;
            }

            return _tcpClient.Connected;
        }

        /// <summary>
        /// 基于SOCKET网络通信消息发送接口
        /// </summary>
        /// <param name="message">待发送的数据流</param>
        public void WriteMessage(byte[] message)
        {
            if (_isClosed)
            {
                return;
            }

            if (false == this.IsConnected())
            {
                Logger.Warn("当前SOCKET网络尚未连接到任何有效远程服务终端，调用此接口发送消息失败！");
                _notification.OnDisconnection(_sequenceId);
                return;
            }

            SystemMemoryStream ms = null;
            using (ms = new SystemMemoryStream())
            {
                ms.Position = 0;
                SystemBinaryWriter writer = new SystemBinaryWriter(ms);
                short len = SystemIPAddress.HostToNetworkOrder((short) message.Length);
                writer.Write(len);
                writer.Write(message);
                writer.Flush();
                byte[] payload = ms.ToArray();
                _outStream.BeginWrite(payload, 0, payload.Length, _writeCallback, null);
                writer.Close();
            }
        }

        /// <summary>
        /// 连接成功回调通知
        /// </summary>
        /// <param name="ar">异步操作结果参数</param>
        private void OnConnection(SystemIAsyncResult ar)
        {
            if (_isClosed)
            {
                this.OnDisconnection(_sequenceId);
                return;
            }

            if (_tcpClient.Connected)
            {
                _outStream = _tcpClient.GetStream();
                _tcpClient.GetStream().BeginRead(_buffer, 0, MAX_READBUFFER_SIZE, _readCallback, null);
                // 通知上层容器网络连接成功
                _notification.OnConnection(_sequenceId);
            }
            else
            {
                OnConnectError(_sequenceId);
            }
        }

        /// <summary>
        /// 连接异常回调通知
        /// </summary>
        /// <param name="fd">连接通道标识</param>
        private void OnConnectError(int fd)
        {
            // 通知上层容器网络通信异常
            _notification.OnConnectError(fd);

            if (null != _tcpClient)
            {
                this.OnDisconnection(fd);
            }
        }

        /// <summary>
        /// 断开连接回调通知
        /// </summary>
        /// <param name="fd">连接通道标识</param>
        private void OnDisconnection(int fd)
        {
            this.Disconnect();

            // 通知上层容器网络已经断开
            _notification.OnDisconnection(fd);
        }

        private void OnRead(SystemIAsyncResult ar)
        {
            int readBytes = 0;
            try
            {
                if (_isClosed)
                {
                    this.OnDisconnection(_sequenceId);
                    return;
                }

                lock (_tcpClient.GetStream())
                {
                    // 读取字节流到缓冲区
                    readBytes = _tcpClient.GetStream().EndRead(ar);
                }

                if (readBytes < 1)
                {
                    Logger.Error("SOCKET: read bytes error ({0}).", readBytes);
                    // 字节长度异常，断线处理
                    this.OnConnectError(_sequenceId);
                    return;
                }

                // 解析数据包内容，转换为业务层协议原型
                this.OnReceived(_buffer, readBytes);

                lock (_tcpClient.GetStream())
                {
                    // 继续读取网络上行数据流
                    SystemArray.Clear(_buffer, 0, _buffer.Length);
                    _tcpClient.GetStream().BeginRead(_buffer, 0, MAX_READBUFFER_SIZE, _readCallback, null);
                }
            } catch (SystemException e)
            {
                Logger.Error(e.Message);

                this.OnConnectError(_sequenceId);
            }
        }

        private void OnWrite(SystemIAsyncResult ar)
        {
            try
            {
                _outStream.EndWrite(ar);
            } catch (SystemException e)
            {
                Logger.Error(e.Message);

                this.OnConnectError(_sequenceId);
            }
        }

        private void OnReceived(byte[] buffer, int size)
        {
            _memStream.Seek(0, SystemSeekOrigin.End);
            _memStream.Write(buffer, 0, size);
            // Reset to beginning
            _memStream.Seek(0, SystemSeekOrigin.Begin);
            while (this.RemainingBytes() > 2)
            {
                short messageLength = SystemBitConverter.ToInt16(_reader.ReadBytes(2), 0);
                messageLength = SystemIPAddress.NetworkToHostOrder(messageLength);
                if (this.RemainingBytes() >= messageLength)
                {
                    SystemMemoryStream ms = new SystemMemoryStream();
                    SystemBinaryWriter writer = new SystemBinaryWriter(ms);
                    writer.Write(_reader.ReadBytes(messageLength));
                    ms.Seek(0, SystemSeekOrigin.Begin);
                    this.OnReceivedMessage(ms);
                }
                else
                {
                    // Back up the position two bytes
                    _memStream.Position = _memStream.Position - 2;
                    break;
                }
            }

            // Create a new stream with any leftover bytes
            byte[] leftover = _reader.ReadBytes((int) this.RemainingBytes());
            _memStream.SetLength(0); // Clear
            _memStream.Write(leftover, 0, leftover.Length);
        }

        /// <summary>
        /// 检查当前内存缓冲区中已接收数据的剩余长度，已处理过的数据需从缓冲区中移除
        /// </summary>
        /// <returns>返回当前已接收的数据长度值</returns>
        private long RemainingBytes()
        {
            return (_memStream.Length - _memStream.Position);
        }

        /// <summary>
        /// 接收下行数据流并打包输出到外部回调接口
        /// </summary>
        /// <param name="ms">下行数据流</param>
        private void OnReceivedMessage(SystemMemoryStream ms)
        {
            SystemBinaryReader r = new SystemBinaryReader(ms);
            byte[] message = r.ReadBytes((int) (ms.Length - ms.Position));
            // int msglen = message.Length;

            // _notification.OnReceivedMessage(_sequenceID, message);
        }
    }
}
