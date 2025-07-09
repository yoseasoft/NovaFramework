/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
/// Copyright (C) 2017 - 2020, Shanghai Tommon Network Technology Co., Ltd.
/// Copyright (C) 2020 - 2022, Guangzhou Xinyuan Technology Co., Ltd.
/// Copyright (C) 2023, Guangzhou Shiyue Network Technology Co., Ltd.
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

using SystemIEnumerator = System.Collections.IEnumerator;

using UnityWebRequest = UnityEngine.Networking.UnityWebRequest;
using UnityWWWForm = UnityEngine.WWWForm;

namespace NovaEngine.Network
{
    /// <summary>
    /// 基于HTTP模式的网络连接客户端，用于处于WEB模式下的网络连接及消息处理
    /// 所有的处理结果均采用事件方式发送到网络管理器中，由网络管理器进行统一转发
    /// </summary>
    public sealed partial class HttpClient
    {
        /// <summary>
        /// HTTP模式网络通信回调接口
        /// </summary>
        /// <param name="linkID">链接唯一标识</param>
        /// <param name="request">网络消息对象</param>
        public delegate void OnHttpResponseHandler(int linkID, UnityWebRequest request);

        /// <summary>
        /// HTTP客户端对象的默认构造函数
        /// </summary>
        public HttpClient()
        { }

        /// <summary>
        /// HTTP客户端对象的析构函数
        /// </summary>
        ~HttpClient()
        { }

        /// <summary>
        /// HTTP服务请求发送接口
        /// </summary>
        /// <param name="linkID">链接唯一标识</param>
        /// <param name="url">网络服务地址</param>
        /// <param name="handler">网络响应接口</param>
        public SystemIEnumerator SendRequest(int linkID, string url, OnHttpResponseHandler handler)
        {
            UnityWebRequest request = UnityWebRequest.Get(url);// new UnityWebRequest(url);
            request.certificateHandler = new AcceptAllCertificatesHandler();// 必须验证, 具体原因可进入AcceptAllCertificatesHandler查看
            yield return request.SendWebRequest();

            handler(linkID, request);
        }

        /// <summary>
        /// HTTP服务POST请求发送接口
        /// </summary>
        /// <param name="linkID">链接唯一标识</param>
        /// <param name="url">网络服务地址</param>
        /// <param name="fields">包头字段</param>
        /// <param name="data">数据流</param>
        /// <param name="handler">网络响应接口</param>
        public SystemIEnumerator SendPostRequest(int linkID, string url, string fields, byte[] data, OnHttpResponseHandler handler)
        {
            UnityWWWForm form = new UnityWWWForm();
            string[] temp = fields.Split('|');
            for (int n = 0; n < temp.Length; n += 2)
            {
                string field_key = temp[n];
                string field_val = temp[n + 1];
                form.AddField(field_key, field_val);
            }

            if (null != data)
            {
                form.AddBinaryData("bin_data", data);
            }

            UnityWebRequest request = UnityWebRequest.Post(url, form);
            request.certificateHandler = new AcceptAllCertificatesHandler();// 必须验证, 具体原因可进入AcceptAllCertificatesHandler查看
            yield return request.SendWebRequest();

            handler(linkID, request);
        }

        /// <summary>
        ///  http post 上传文本文件
        /// </summary>
        /// <param name="linkID"></param>
        /// <param name="url"></param>
        /// <param name="fields"></param>
        /// <param name="fileContent"></param>
        /// <param name="fileName"></param>
        /// <param name="handler"></param>
        /// <returns></returns>
        public SystemIEnumerator SendPostUploadRequest(int linkID, string url, string fields, string fileContent, string fileName, OnHttpResponseHandler handler)
        {
            System.Collections.Generic.List<UnityEngine.Networking.IMultipartFormSection> formData = new System.Collections.Generic.List<UnityEngine.Networking.IMultipartFormSection>();

            string[] temp = fields.Split('|');
            for (int n = 0; n < temp.Length; n += 2)
            {
                string field_key = temp[n];
                string field_val = temp[n + 1];
                formData.Add(new UnityEngine.Networking.MultipartFormDataSection(field_key, field_val));
            }

            formData.Add(new UnityEngine.Networking.MultipartFormFileSection("inputfile", fileContent, System.Text.Encoding.UTF8, fileName));

            using (UnityWebRequest request = UnityWebRequest.Post(url, formData))
            {
                request.certificateHandler = new AcceptAllCertificatesHandler();// 必须验证, 具体原因可进入AcceptAllCertificatesHandler查看
                yield return request.SendWebRequest();
                handler(linkID, request);
            }
        }
    }
}
