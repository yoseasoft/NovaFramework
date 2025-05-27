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

using SystemAction = System.Action;
using SystemMemoryStream = System.IO.MemoryStream;

namespace NovaEngine
{
    /// <summary>
    /// 网络通道对象抽象基类
    /// </summary>
    public abstract class NetworkChannel
    {
        /// <summary>
        /// 网络通道唯一标识
        /// </summary>
        protected int m_channelID = 0;

        /// <summary>
        /// 网络通道名称
        /// </summary>
        protected string m_channelName = null;

        /// <summary>
        /// 网络通道地址
        /// </summary>
        protected string m_url = null;

        /// <summary>
        /// 网络通道错误码记录
        /// </summary>
        protected int m_errorCode = 0;

        /// <summary>
        /// 网络通道关闭状态标识
        /// </summary>
        protected bool m_isClosed = false;

        /// <summary>
        /// 网络通道相关服务引用实例
        /// </summary>
        protected NetworkService m_service = null;

        /// <summary>
        /// 网络连接回调通知代理接口
        /// </summary>
        protected System.Action<NetworkChannel> m_connectionCallback;

        /// <summary>
        /// 网络断开连接回调通知代理接口
        /// </summary>
        protected System.Action<NetworkChannel> m_disconnectionCallback;

        /// <summary>
        /// 网络错误回调通知代理接口
        /// </summary>
        protected System.Action<NetworkChannel, int> m_errorCallback;

        /// <summary>
        /// 网络数据读入操作回调接口代理接口
        /// </summary>
        protected System.Action<SystemMemoryStream, int> m_readCallback;

        /// <summary>
        /// 网络数据写入操作失败回调接口代理接口
        /// </summary>
        protected System.Action<SystemMemoryStream, int> m_writeFailedCallback;

        /// <summary>
        /// 获取网络通道唯一标识
        /// </summary>
        public int ChannelID
        {
            get { return m_channelID; }
        }

        /// <summary>
        /// 获取网络通道名称
        /// </summary>
        public string ChannelName
        {
            get { return m_channelName; }
        }

        /// <summary>
        /// 获取网络通道地址
        /// </summary>
        public string Url
        {
            get { return m_url; }
        }

        /// <summary>
        /// 获取或设置网络通道错误码记录
        /// </summary>
        public int ErrorCode
        {
            get { return m_errorCode; }
            set { m_errorCode = value; }
        }

        /// <summary>
        /// 获取网络通道关闭状态标识
        /// </summary>
        public bool IsClosed
        {
            get { return m_isClosed; }
        }

        /// <summary>
        /// 获取网络通道相关服务引用实例
        /// </summary>
        public NetworkService Service
        {
            get { return m_service; }
        }

        /// <summary>
        /// 获取网络通道的服务类型
        /// </summary>
        public int ServiceType
        {
            get { return this.Service.ServiceType; }
        }

        /// <summary>
        /// 添加或移除网络连接回调通知代理接口
        /// </summary>
        public event System.Action<NetworkChannel> ConnectionCallback
        {
            add { m_connectionCallback += value; }
            remove { m_connectionCallback -= value; }
        }

        /// <summary>
        /// 添加或移除网络断开连接回调通知代理接口
        /// </summary>
        public event System.Action<NetworkChannel> DisconnectionCallback
        {
            add { m_disconnectionCallback += value; }
            remove { m_disconnectionCallback -= value; }
        }

        /// <summary>
        /// 添加或移除网络错误回调通知代理接口
        /// </summary>
        public event System.Action<NetworkChannel, int> ErrorCallback
        {
            add { m_errorCallback += value; }
            remove { m_errorCallback -= value; }
        }

        /// <summary>
        /// 添加或移除网络数据读入操作回调接口代理接口
        /// </summary>
        public event System.Action<SystemMemoryStream, int> ReadCallback
        {
            add { m_readCallback += value; }
            remove { m_readCallback -= value; }
        }

        /// <summary>
        /// 添加或移除网络数据写入操作失败回调接口代理接口
        /// </summary>
        public event System.Action<SystemMemoryStream, int> WriteFailedCallback
        {
            add { m_writeFailedCallback += value; }
            remove { m_writeFailedCallback -= value; }
        }

        /// <summary>
        /// 网络通道对象的新实例构建接口
        /// </summary>
        /// <param name="name">网络通道名称</param>
        /// <param name="url">网络地址参数</param>
        /// <param name="service">服务对象实例</param>
        public NetworkChannel(string name, string url, NetworkService service)
        {
            Logger.Assert(null != service);

            this.m_channelID = Session.NextSessionID((int) ModuleObject.EEventType.Network);
            this.m_channelName = name;
            this.m_url = url;

            this.m_service = service;
        }

        ~NetworkChannel()
        {
            this.m_channelID = 0;
            this.m_channelName = null;
            this.m_url = null;
            this.m_errorCode = 0;
            this.m_isClosed = false;

            this.m_service = null;
        }

        /// <summary>
        /// 网络错误回调通知操作接口
        /// </summary>
        /// <param name="e">网络错误码</param>
        protected void OnError(int e)
        {
            this.m_errorCode = e;
            this.m_errorCallback?.Invoke(this, e);
        }

        /// <summary>
        /// 网络通道关闭操作回调接口
        /// </summary>
        protected virtual void OnClose()
        {
        }

        /// <summary>
        /// 网络通道关闭处理操作接口
        /// </summary>
        public void Close()
        {
            if (this.m_isClosed)
            {
                return;
            }

            this.m_isClosed = true;

            this.OnClose();
        }

        /// <summary>
        /// 网络通道连接操作接口函数
        /// </summary>
        public abstract void Connect();

        /// <summary>
        /// 网络通道连接操作接口函数
        /// </summary>
        /// <param name="name">通道名称</param>
        /// <param name="url">通道连接目标地址</param>
        public void Connect(string name, string url)
        {
            Logger.Assert(string.IsNullOrEmpty(m_channelName) && string.IsNullOrEmpty(m_url), "The name or url was already assigned value.");

            m_channelName = name;
            m_url = url;

            Connect();
        }

        /// <summary>
        /// 网络通道数据下行操作接口
        /// </summary>
        /// <param name="message">消息内容</param>
        public abstract void Send(string message);

        /// <summary>
        /// 网络通道数据下行操作接口
        /// </summary>
        /// <param name="message">消息内容</param>
        public abstract void Send(byte[] message);

        /// <summary>
        /// 网络通道数据下行操作接口
        /// </summary>
        /// <param name="memoryStream">消息数据流</param>
        public abstract void Send(SystemMemoryStream memoryStream);
    }
}
