/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
/// Copyring (C) 2017 - 2020, Shanghai Tommon Network Technology Co., Ltd.
/// Copyring (C) 2020 - 2022, Guangzhou Xinyuan Technology Co., Ltd.
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
        private static object m_locked = new object();

        private ISocketCall m_notification = null;

        private SystemTcpClient m_tcpClient = null;
        private SystemNetworkStream m_outStream = null;

        private SystemMemoryStream m_memStream = null;
        private SystemBinaryReader m_reader = null;
        private SystemAsyncCallback m_readCallback = null;
        private SystemAsyncCallback m_writeCallback = null;

        private byte[] m_buffer = new byte[MAX_READBUFFER_SIZE];

        /// <summary>
        /// 当前链接是否关闭状态标识
        /// </summary>
        private bool m_isClosed = false;

        /// <summary>
        /// 自定义链接描述符序列标识
        /// </summary>
        private int m_sequenceID = 0;

        public bool IsClosed
        {
            get
            {
                return m_isClosed;
            }
        }

        public int SequenceID
        {
            get 
            {
                return m_sequenceID;
            }
        }

        public SocketClient(int id, ISocketCall handler)
        {
            // 必须传入上层管理容器用于回调通知
            Logger.Assert(null != handler);
            m_notification = handler;
            m_sequenceID = id;

            m_memStream = new SystemMemoryStream();
            m_reader = new SystemBinaryReader(m_memStream);
            m_readCallback = new SystemAsyncCallback(OnRead);
            m_writeCallback = new SystemAsyncCallback(OnWrite);

            // 初始默认为关闭状态
            m_isClosed = true;
        }

        ~SocketClient()
        {
            this.Disconnect();

            m_reader.Close();
            m_reader = null;
            m_memStream.Close();
            m_memStream = null;

            m_notification = null;
            m_readCallback = null;
            m_writeCallback = null;
        }

        /// <summary>
        /// 重置当前的读写数据流
        /// </summary>
        private void ResetStream()
        {
            if (null != m_reader)
            {
                m_reader.Close();
                m_reader = null;

                m_memStream.Close();
                m_memStream = null;
            }

            m_memStream = new SystemMemoryStream();
            m_reader = new SystemBinaryReader(m_memStream);
        }

        /// <summary>
        /// 连接到目标网络终端的执行接口
        /// </summary>
        /// <param name="host">网络地址</param>
        /// <param name="port">网络端口</param>
        public int Connect(string host, int port)
        {
            m_isClosed = false;

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
                    m_tcpClient = new SystemTcpClient(SystemAddressFamily.InterNetworkV6);
                }
                else // IPv4的连接模式
                {
                    m_tcpClient = new SystemTcpClient(SystemAddressFamily.InterNetwork);
                }

                m_tcpClient.SendTimeout = 1000;
                m_tcpClient.ReceiveTimeout = 1000;
                m_tcpClient.NoDelay = true;
                m_tcpClient.BeginConnect(host, port, new SystemAsyncCallback(OnConnection), null);

                ModuleController.QueueOnMainThread(delegate () {
                    if (false == m_isClosed && null != m_tcpClient && false == m_tcpClient.Connected)
                    {
                        this.OnConnectError(m_sequenceID);
                    }
                }, CONNECT_TIMEOUT);
                return m_sequenceID;
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
            m_isClosed = true;

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
            m_isClosed = true;

            if (null != m_tcpClient)
            {
                if (m_tcpClient.Connected)
                {
                    m_tcpClient.Close();
                }
                
                m_tcpClient = null;
            }
        }

        /// <summary>
        /// 检查当前的网络是否处于连接状态
        /// </summary>
        /// <returns>若当前网络处于连接状态则返回true，否则返回false</returns>
        public bool IsConnected()
        {
            if (null == m_tcpClient)
            {
                return false;
            }

            return m_tcpClient.Connected;
        }

        /// <summary>
        /// 基于SOCKET网络通信消息发送接口
        /// </summary>
        /// <param name="message">待发送的数据流</param>
        public void WriteMessage(byte[] message)
        {
            if (m_isClosed)
            {
                return;
            }

            if (false == this.IsConnected())
            {
                Logger.Warn("当前SOCKET网络尚未连接到任何有效远程服务终端，调用此接口发送消息失败！");
                m_notification.OnDisconnection(m_sequenceID);
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
                m_outStream.BeginWrite(payload, 0, payload.Length, m_writeCallback, null);
                writer.Close();
            }
        }

        /// <summary>
        /// 连接成功回调通知
        /// </summary>
        /// <param name="ar">异步操作结果参数</param>
        private void OnConnection(SystemIAsyncResult ar)
        {
            if (m_isClosed)
            {
                this.OnDisconnection(m_sequenceID);
                return;
            }

            if (m_tcpClient.Connected)
            {
                m_outStream = m_tcpClient.GetStream();
                m_tcpClient.GetStream().BeginRead(m_buffer, 0, MAX_READBUFFER_SIZE, m_readCallback, null);
                // 通知上层容器网络连接成功
                m_notification.OnConnection(m_sequenceID);
            }
            else
            {
                OnConnectError(m_sequenceID);
            }
        }

        /// <summary>
        /// 连接异常回调通知
        /// </summary>
        /// <param name="fd">连接通道标识</param>
        private void OnConnectError(int fd)
        {
            // 通知上层容器网络通信异常
            m_notification.OnConnectError(fd);

            if (null != m_tcpClient)
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
            m_notification.OnDisconnection(fd);
        }

        private void OnRead(SystemIAsyncResult ar)
        {
            int readBytes = 0;
            try
            {
                if (m_isClosed)
                {
                    this.OnDisconnection(m_sequenceID);
                    return;
                }

                lock (m_tcpClient.GetStream())
                {
                    // 读取字节流到缓冲区
                    readBytes = m_tcpClient.GetStream().EndRead(ar);
                }

                if (readBytes < 1)
                {
                    Logger.Error("SOCKET: read bytes error ({0}).", readBytes);
                    // 字节长度异常，断线处理
                    this.OnConnectError(m_sequenceID);
                    return;
                }

                // 解析数据包内容，转换为业务层协议原型
                this.OnReceived(m_buffer, readBytes);

                lock (m_tcpClient.GetStream())
                {
                    // 继续读取网络上行数据流
                    SystemArray.Clear(m_buffer, 0, m_buffer.Length);
                    m_tcpClient.GetStream().BeginRead(m_buffer, 0, MAX_READBUFFER_SIZE, m_readCallback, null);
                }
            } catch (SystemException e)
            {
                Logger.Error(e.Message);

                this.OnConnectError(m_sequenceID);
            }
        }

        private void OnWrite(SystemIAsyncResult ar)
        {
            try
            {
                m_outStream.EndWrite(ar);
            } catch (SystemException e)
            {
                Logger.Error(e.Message);

                this.OnConnectError(m_sequenceID);
            }
        }

        private void OnReceived(byte[] buffer, int size)
        {
            m_memStream.Seek(0, SystemSeekOrigin.End);
            m_memStream.Write(buffer, 0, size);
            // Reset to beginning
            m_memStream.Seek(0, SystemSeekOrigin.Begin);
            while (this.RemainingBytes() > 2)
            {
                short messageLength = SystemBitConverter.ToInt16(m_reader.ReadBytes(2), 0);
                messageLength = SystemIPAddress.NetworkToHostOrder(messageLength);
                if (this.RemainingBytes() >= messageLength)
                {
                    SystemMemoryStream ms = new SystemMemoryStream();
                    SystemBinaryWriter writer = new SystemBinaryWriter(ms);
                    writer.Write(m_reader.ReadBytes(messageLength));
                    ms.Seek(0, SystemSeekOrigin.Begin);
                    this.OnReceivedMessage(ms);
                }
                else
                {
                    // Back up the position two bytes
                    m_memStream.Position = m_memStream.Position - 2;
                    break;
                }
            }

            // Create a new stream with any leftover bytes
            byte[] leftover = m_reader.ReadBytes((int) this.RemainingBytes());
            m_memStream.SetLength(0); // Clear
            m_memStream.Write(leftover, 0, leftover.Length);
        }

        /// <summary>
        /// 检查当前内存缓冲区中已接收数据的剩余长度，已处理过的数据需从缓冲区中移除
        /// </summary>
        /// <returns>返回当前已接收的数据长度值</returns>
        private long RemainingBytes()
        {
            return (m_memStream.Length - m_memStream.Position);
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

            // m_notification.OnReceivedMessage(m_sequenceID, message);
        }
    }
}
