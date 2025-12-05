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

using System;
using System.Collections;
using System.IO;
using System.Text;

using UnityWaitForEndOfFrame = UnityEngine.WaitForEndOfFrame;
using UnityWebRequest = UnityEngine.Networking.UnityWebRequest;
using UnityUploadHandlerRaw = UnityEngine.Networking.UploadHandlerRaw;
using UnityDownloadHandlerBuffer = UnityEngine.Networking.DownloadHandlerBuffer;

namespace NovaEngine.Module
{
    /// <summary>
    /// HTTP模式网络通道对象抽象基类
    /// </summary>
    internal sealed class HttpChannel : NetworkChannel
    {
        private readonly MemoryStream _memoryStream = null;

        /// <summary>
        /// HTTP网络通道对象的新实例构建接口
        /// </summary>
        /// <param name="name">通道名称</param>
        /// <param name="url">通道地址参数</param>
        /// <param name="service">服务对象实例</param>
        public HttpChannel(string name, string url, HttpService service) : base(name, url, service)
        {
            this._memoryStream = service.MemoryStreamManager.GetStream(name, ushort.MaxValue);
        }

        /// <summary>
        /// 网络通道关闭操作回调接口
        /// </summary>
        protected override void OnClose()
        {
            this._memoryStream.Dispose();

            base.OnClose();
        }

        /// <summary>
        /// 网络通道连接操作接口
        /// </summary>
        public override void Connect()
        {
            Facade.Instance.StartCoroutine(__ConnectAsync());
        }

        /// <summary>
        /// 网络通道数据下行操作接口
        /// </summary>
        /// <param name="message">消息内容</param>
        public override void Send(string message)
        {
            Facade.Instance.StartCoroutine(__DoPostRequest(message));
        }

        /// <summary>
        /// 网络通道数据下行操作接口
        /// </summary>
        /// <param name="message">消息内容</param>
        public override void Send(byte[] message)
        {
            Facade.Instance.StartCoroutine(__DoPostRequest(message));
        }

        /// <summary>
        /// 网络通道数据下行操作接口
        /// </summary>
        /// <param name="memoryStream">消息数据流</param>
        public override void Send(MemoryStream memoryStream)
        {
            throw new NotImplementedException();
        }

        private IEnumerator __ConnectAsync()
        {
            yield return new UnityWaitForEndOfFrame();

            this._connectionCallback?.Invoke(this);
        }

        private IEnumerator __DoPostRequest(byte[] body)
        {
            UnityWebRequest webRequest = new UnityWebRequest(this._url, "POST");
            webRequest.certificateHandler = new Network.AcceptAllCertificatesHandler();// 必须验证, 具体原因可进入Network.AcceptAllCertificatesHandler查看
            webRequest.uploadHandler = new UnityUploadHandlerRaw(body);
            webRequest.downloadHandler = new UnityDownloadHandlerBuffer();
            webRequest.SetRequestHeader("Content-Type", "application/json;charset=utf-8");
            yield return webRequest.SendWebRequest();

            if (UnityWebRequest.Result.ProtocolError == webRequest.result || UnityWebRequest.Result.ConnectionError == webRequest.result)
            {
                Logger.Error(webRequest.error);
                this._memoryStream.Seek(0, SeekOrigin.Begin);
                this._memoryStream.SetLength(body.Length);
                Array.Copy(body, 0, this._memoryStream.GetBuffer(), 0, body.Length);
                _writeFailedCallback?.Invoke(this._memoryStream, MessageStreamCode.String);
            }
            else
            {
                long size = (long) webRequest.downloadedBytes;
                // Logger.Debug(webRequest.downloadHandler.text);
                this._memoryStream.Seek(0, SeekOrigin.Begin);
                this._memoryStream.SetLength(size);
                Array.Copy(webRequest.downloadHandler.data, 0, this._memoryStream.GetBuffer(), 0, size);

                try
                {
                    // 协议码为空
                    this._readCallback.Invoke(this._memoryStream, MessageStreamCode.String);
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }
        }

        private IEnumerator __DoPostRequest(string message)
        {
            byte[] body = Encoding.UTF8.GetBytes(message);
            yield return __DoPostRequest(body);
        }
    }
}
