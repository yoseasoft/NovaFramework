/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
/// Copyright (C) 2017 - 2020, Shanghai Tommon Network Technology Co., Ltd.
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

using System.Collections.Generic;
using System.Runtime.CompilerServices;

using UnityWebRequest = UnityEngine.Networking.UnityWebRequest;

namespace NovaEngine.Module
{
    /// <summary>
    /// 网络管理模块对象类，处理网络相关的连接/断开，及数据包通信等访问接口
    /// 优化网络操作接口，整合SOCKET，HTTP链接及读写接口
    /// </summary>
    internal sealed partial class NetworkModule : ModuleObject, Network.ISocketCall, Network.IHttpCall
    {
        /// <summary>
        /// 处理HTTP模式的应用实例，对基于HTTP协议的网络请求进行封装实现
        /// </summary>
        private Network.HttpClient _httpClient;

        /// <summary>
        /// 网络模块事件类型
        /// </summary>
        public override sealed int EventType => (int) ModuleEventType.Network;

        /// <summary>
        /// 管理器对象初始化接口函数
        /// </summary>
        protected override sealed void OnInitialize()
        {
        }

        /// <summary>
        /// 管理器对象清理接口函数
        /// </summary>
        protected override sealed void OnCleanup()
        {
        }

        /// <summary>
        /// 管理器对象初始启动接口
        /// </summary>
        protected override sealed void OnStartup()
        {
            NetworkAdapter.Startup();

            _httpClient = new Network.HttpClient();
        }

        /// <summary>
        /// 管理器对象结束关闭接口
        /// </summary>
        protected override sealed void OnShutdown()
        {
            _httpClient = null;

            NetworkAdapter.Shutdown();
        }

        /// <summary>
        /// 管理器对象垃圾回收调度接口
        /// </summary>
        protected override sealed void OnDump()
        {
        }

        protected override sealed void OnUpdate()
        {
            NetworkAdapter.Update();
        }

        protected override sealed void OnLateUpdate()
        {
        }

        private void SendEvent(int type, int fd, object data)
        {
            NetworkEventArgs e = this.AcquireEvent<NetworkEventArgs>();
            e.Type = type;
            e.ChannelID = fd;
            e.Data = data;
            this.SendEvent(e);
        }

        #region SOCKET访问模式相关操作接口

        /// <summary>
        /// 远程终端基于SOCKET方式网络连接请求接口，若连接请求处理成功，<see cref="NetworkModule.OnConnection"/>接口将被触发
        /// </summary>
        /// <param name="protocol">网络协议类型</param>
        /// <param name="name">网络名称</param>
        /// <param name="url">网络地址</param>
        /// <returns>若网络连接请求发送成功返回对应的通道标识，否则返回0</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Connect(int protocol, string name, string url)
        {
            return NetworkAdapter.Connect(protocol, name, url);
        }

        /// <summary>
        /// 远程终端网络断开请求接口，若断开请求处理成功，<see cref="NetworkModule.OnDisconnection"/>接口将被触发
        /// </summary>
        /// <param name="channelID">当前网络连接唯一标识</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Disconnect(int channelID)
        {
            NetworkAdapter.Disconnect(channelID);
        }

        /// <summary>
        /// 向当前已连接的远程终端发送消息，该接口仅用于SOCKET模式连接下的消息发送
        /// </summary>
        /// <param name="channelID">网络通道标识</param>
        /// <param name="message">消息字节流</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteMessage(int channelID, byte[] message)
        {
            NetworkAdapter.Send(channelID, message);
        }

        /// <summary>
        /// 向指定通道对应的连接发送消息
        /// </summary>
        /// <param name="channelID">网络通道标识</param>
        /// <param name="buffer">字符串数据</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteMessage(int channelID, string buffer)
        {
            NetworkAdapter.Send(channelID, buffer);
        }

        #endregion

        #region HTTP访问模式相关操作接口

        /// <summary>
        /// 针对目标网络服务地址的消息请求操作接口
        /// </summary>
        /// <param name="linkID">链接唯一标识</param>
        /// <param name="url">网络服务地址</param>
        public void SendRequest(int linkID, string url)
        {
            this.SendRequest(linkID, url, delegate (int id, UnityWebRequest request)
            {
                if (UnityWebRequest.Result.ProtocolError == request.result || UnityWebRequest.Result.ConnectionError == request.result)
                {
                    this.OnHttpException(id, request.error);
                }
                else
                {
                    this.OnHttpResponse(id, request.downloadHandler.text);
                }
            });
        }

        /// <summary>
        /// 针对目标网络服务地址的消息请求操作接口
        /// </summary>
        /// <param name="linkID">链接唯一标识</param>
        /// <param name="url">网络服务地址</param>
        /// <param name="handler">消息响应通知句柄</param>
        public void SendRequest(int linkID, string url, Network.HttpClient.OnHttpResponseHandler handler)
        {
            Facade.Instance.StartCoroutine(_httpClient.SendRequest(linkID, url, handler));
        }

        /// <summary>
        /// 针对目标网络服务地址的消息POST请求操作接口
        /// </summary>
        /// <param name="linkID">链接唯一标识</param>
        /// <param name="url">网络服务地址</param>
        /// <param name="fields">包头字段</param>
        /// <param name="data">数据流</param>
        public void SendPostRequest(int linkID, string url, string fields, byte[] data)
        {
            this.SendPostRequest(linkID, url, fields, data, delegate (int id, UnityWebRequest request)
            {
                if (UnityWebRequest.Result.ProtocolError == request.result || UnityWebRequest.Result.ConnectionError == request.result)
                {
                    this.OnHttpException(id, request.error);
                }
                else
                {
                    this.OnHttpResponse(id, request.downloadHandler.text);
                }
            });
        }

        /// <summary>
        /// 针对目标网络服务地址的消息POST请求操作接口
        /// </summary>
        /// <param name="linkID">链接唯一标识</param>
        /// <param name="url">网络服务地址</param>
        /// <param name="fields">包头字段</param>
        /// <param name="data">数据流</param>
        /// <param name="handler">消息响应通知句柄</param>
        public void SendPostRequest(int linkID, string url, string fields, byte[] data, Network.HttpClient.OnHttpResponseHandler handler)
        {
            Facade.Instance.StartCoroutine(_httpClient.SendPostRequest(linkID, url, fields, data, handler));
        }


        public void SendPostUploadRequest(int linkID, string url, string fields, string fileContent, string fileName, Network.HttpClient.OnHttpResponseHandler handler)
        {
            Facade.Instance.StartCoroutine(_httpClient.SendPostUploadRequest(linkID, url, fields, fileContent, fileName, handler));
        }

        /// <summary>
        /// 针对目标网络服务地址的消息POST请求操作接口
        /// </summary>
        /// <param name="linkID">链接唯一标识</param>
        /// <param name="url">网络服务地址</param>
        /// <param name="data">数据流</param>
        /// <param name="handler">消息响应通知句柄</param>
        public void SendJsonRequest(int linkID, string url, string data, Network.HttpClient.OnHttpResponseHandler handler)
        {
            Facade.Instance.StartCoroutine(_httpClient.SendJsonRequest(linkID, url, data, handler));
        }

        #endregion

        #region HTTP网络资源上传相关操作接口

        /// <summary>
        /// 采用异步方式进行数据上传操作接口
        /// </summary>
        /// <param name="path">资源路径</param>
        /// <param name="url">上传网络地址</param>
        /// <param name="filename">上传文件名称</param>
        /// <param name="args">扩展参数</param>
        /// <param name="transmit">传输回调接口实例</param>
        public void Upload(string path, string url, string filename, string args, Network.IHttpTransmit transmit)
        {
            GetModule<ThreadModule>().RunAsync(delegate ()
            {
                Network.HttpUploader.Upload(path, url, filename, args, transmit);
            });
        }

        /// <summary>
        /// 采用异步方式进行数据上传操作接口
        /// </summary>
        /// <param name="path">资源路径</param>
        /// <param name="url">上传网络地址</param>
        /// <param name="filename">上传文件名称</param>
        /// <param name="args">扩展参数</param>
        /// <param name="progress">进度回调接口</param>
        /// <param name="succeed">成功回调接口</param>
        /// <param name="failed">失败回调接口</param>
        public void Upload(string path, string url, string filename, string args,
            Network.HttpUploader.OnHttpUploadHandler progress, Network.HttpUploader.OnHttpUploadHandler succeed, Network.HttpUploader.OnHttpUploadHandler failed)
        {
            GetModule<ThreadModule>().RunAsync(delegate ()
            {
                Network.HttpUploader.Upload(path, url, filename, args, progress, succeed, failed);
            });
        }

        #endregion

        #region HTTP网络资源下载相关操作接口

        /// <summary>
        /// 采用异步方式进行数据下载操作
        /// </summary>
        /// <param name="url">远程资源访问地址</param>
        /// <param name="path">本地资源保存路径</param>
        /// <param name="size">原始资源数据大小</param>
        /// <param name="msTime">下载分时间隔</param>
        /// <param name="transmit">传输回调接口实例</param>
        public void Download(string url, string path, long size, int msTime, Network.IHttpTransmit transmit)
        {
            GetModule<ThreadModule>().RunAsync(delegate ()
            {
                Network.HttpDownloader.Download(url, path, size, msTime, transmit);
            });
        }

        /// <summary>
        /// 采用异步方式进行数据下载操作
        /// </summary>
        /// <param name="url">远程资源访问地址</param>
        /// <param name="path">本地资源保存路径</param>
        /// <param name="size">原始资源数据大小</param>
        /// <param name="msTime">下载分时间隔</param>
        /// <param name="progress">进度回调接口</param>
        /// <param name="succeed">成功回调接口</param>
        /// <param name="failed">失败回调接口</param>
        public void Download(string url, string path, long size, int msTime,
            Network.HttpDownloader.OnHttpDownloadHandler progress, Network.HttpDownloader.OnHttpDownloadHandler succeed, Network.HttpDownloader.OnHttpDownloadHandler failed)
        {
            GetModule<ThreadModule>().RunAsync(delegate ()
            {
                Network.HttpDownloader.Download(url, path, size, msTime, progress, succeed, failed);
            });
        }

        #endregion

        #region 网络相关事件转发回调通知接口

        public void OnConnection(int fd)
        {
            this.SendEvent((int) ProtocolType.Connected, fd, null);
        }

        public void OnDisconnection(int fd)
        {
        }

        public void OnConnectError(int fd)
        {
            this.SendEvent((int) ProtocolType.Exception, fd, null);
        }

        public void OnReceivedMessage(int fd, byte[] message)
        {
            this.SendEvent((int) ProtocolType.Dispatched, fd, new IO.ByteStreamBuffer(message));
        }

        public void OnReceivedMessage(int fd, string message)
        {
            this.SendEvent((int) ProtocolType.Dispatched, fd, message);
        }

        public void OnSendFailedMessage(int fd, byte[] message)
        {
            this.SendEvent((int) ProtocolType.WriteError, fd, new IO.ByteStreamBuffer(message));
        }

        public void OnSendFailedMessage(int fd, string message)
        {
            this.SendEvent((int) ProtocolType.WriteError, fd, message);
        }

        public void OnHttpResponse(int linkID, string message)
        {
            this.SendEvent((int) ProtocolType.HttpResponse, linkID, message);
        }

        public void OnHttpException(int linkID, string message)
        {
            this.SendEvent((int) ProtocolType.HttpException, linkID, message);
        }

        #endregion
    }
}
