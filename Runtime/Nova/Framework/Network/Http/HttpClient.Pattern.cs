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

using UnityEngine.Networking;

using SystemIEnumerator = System.Collections.IEnumerator;
using SystemEncoding = System.Text.Encoding;

namespace NovaEngine.Network
{
    /// <summary>
    /// 使UnityWebRequest直接通过所有证书验证(即不进行任何验证)
    /// Android使用UnityWebRequest需使用自定义验证, 此处直接通过, 可根据项目具体需要修改验证方法
    /// https://forum.unity.com/threads/unitywebrequest-report-an-error-ssl-ca-certificate-error.617521/
    /// </summary>
    class AcceptAllCertificatesHandler : UnityEngine.Networking.CertificateHandler
    {
        protected override bool ValidateCertificate(byte[] certificateData)
        {
            return true;
        }
    }

    /// <summary>
    /// 基于HTTP模式的网络连接客户端，用于处于WEB模式下的网络连接及消息处理
    /// 所有的处理结果均采用事件方式发送到网络管理器中，由网络管理器进行统一转发
    /// </summary>
    public sealed partial class HttpClient
    {
        /// <summary>
        /// HTTP服务POST请求发送接口
        /// </summary>
        /// <param name="linkID">链接唯一标识</param>
        /// <param name="url">网络服务地址</param>
        /// <param name="data">数据流</param>
        /// <param name="handler">网络响应接口</param>
        public SystemIEnumerator SendJsonRequest(int linkID, string url, string data, OnHttpResponseHandler handler)
        {
            UnityWebRequest request = new UnityWebRequest(url, "POST");
            byte[] postBytes = SystemEncoding.UTF8.GetBytes(data);
            request.uploadHandler = new UploadHandlerRaw(postBytes);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.certificateHandler = new AcceptAllCertificatesHandler();// 必须验证, 具体原因可进入AcceptAllCertificatesHandler查看
            request.SetRequestHeader("Content-Type", "application/json;charset=utf-8");
            yield return request.SendWebRequest();

            if (false == request.isDone)
            {
                Logger.Error(request.error);
            }

            // Logger.Debug(request.downloadHandler.text);

            handler(linkID, request);
        }
    }
}
