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
        protected int _channelID = 0;

        /// <summary>
        /// 网络通道名称
        /// </summary>
        protected string _channelName = null;

        /// <summary>
        /// 网络通道地址
        /// </summary>
        protected string _url = null;

        /// <summary>
        /// 网络通道错误码记录
        /// </summary>
        protected int _errorCode = 0;

        /// <summary>
        /// 网络通道关闭状态标识
        /// </summary>
        protected bool _isClosed = false;

        /// <summary>
        /// 网络通道相关服务引用实例
        /// </summary>
        protected NetworkService _service = null;

        /// <summary>
        /// 网络连接回调通知代理接口
        /// </summary>
        protected System.Action<NetworkChannel> _connectionCallback;

        /// <summary>
        /// 网络断开连接回调通知代理接口
        /// </summary>
        protected System.Action<NetworkChannel> _disconnectionCallback;

        /// <summary>
        /// 网络错误回调通知代理接口
        /// </summary>
        protected System.Action<NetworkChannel, int> _errorCallback;

        /// <summary>
        /// 网络数据读入操作回调接口代理接口
        /// </summary>
        protected System.Action<SystemMemoryStream, int> _readCallback;

        /// <summary>
        /// 网络数据写入操作失败回调接口代理接口
        /// </summary>
        protected System.Action<SystemMemoryStream, int> _writeFailedCallback;

        /// <summary>
        /// 获取网络通道唯一标识
        /// </summary>
        public int ChannelID
        {
            get { return _channelID; }
        }

        /// <summary>
        /// 获取网络通道名称
        /// </summary>
        public string ChannelName
        {
            get { return _channelName; }
        }

        /// <summary>
        /// 获取网络通道地址
        /// </summary>
        public string Url
        {
            get { return _url; }
        }

        /// <summary>
        /// 获取或设置网络通道错误码记录
        /// </summary>
        public int ErrorCode
        {
            get { return _errorCode; }
            set { _errorCode = value; }
        }

        /// <summary>
        /// 获取网络通道关闭状态标识
        /// </summary>
        public bool IsClosed
        {
            get { return _isClosed; }
        }

        /// <summary>
        /// 获取网络通道相关服务引用实例
        /// </summary>
        public NetworkService Service
        {
            get { return _service; }
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
            add { _connectionCallback += value; }
            remove { _connectionCallback -= value; }
        }

        /// <summary>
        /// 添加或移除网络断开连接回调通知代理接口
        /// </summary>
        public event System.Action<NetworkChannel> DisconnectionCallback
        {
            add { _disconnectionCallback += value; }
            remove { _disconnectionCallback -= value; }
        }

        /// <summary>
        /// 添加或移除网络错误回调通知代理接口
        /// </summary>
        public event System.Action<NetworkChannel, int> ErrorCallback
        {
            add { _errorCallback += value; }
            remove { _errorCallback -= value; }
        }

        /// <summary>
        /// 添加或移除网络数据读入操作回调接口代理接口
        /// </summary>
        public event System.Action<SystemMemoryStream, int> ReadCallback
        {
            add { _readCallback += value; }
            remove { _readCallback -= value; }
        }

        /// <summary>
        /// 添加或移除网络数据写入操作失败回调接口代理接口
        /// </summary>
        public event System.Action<SystemMemoryStream, int> WriteFailedCallback
        {
            add { _writeFailedCallback += value; }
            remove { _writeFailedCallback -= value; }
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

            this._channelID = Session.NextSessionID((int) ModuleObject.ModuleEventType.Network);
            this._channelName = name;
            this._url = url;

            this._service = service;
        }

        ~NetworkChannel()
        {
            this._channelID = 0;
            this._channelName = null;
            this._url = null;
            this._errorCode = 0;
            this._isClosed = false;

            this._service = null;
        }

        /// <summary>
        /// 网络错误回调通知操作接口
        /// </summary>
        /// <param name="e">网络错误码</param>
        protected void OnError(int e)
        {
            this._errorCode = e;
            this._errorCallback?.Invoke(this, e);
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
            if (this._isClosed)
            {
                return;
            }

            this._isClosed = true;

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
            Logger.Assert(string.IsNullOrEmpty(_channelName) && string.IsNullOrEmpty(_url), "The name or url was already assigned value.");

            _channelName = name;
            _url = url;

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
