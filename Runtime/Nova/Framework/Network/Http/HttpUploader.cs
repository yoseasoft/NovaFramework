/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
/// Copyring (C) 2020 - 2022, Guangzhou Xinyuan Technology Co., Ltd.
/// Copyring (C) 2022 - 2023, Shanghai Bilibili Technology Co., Ltd.
/// Copyring (C) 2023, Guangzhou Shiyue Network Technology Co., Ltd.
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

using SystemDateTime = System.DateTime;
using SystemException = System.Exception;
using SystemEncoding = System.Text.Encoding;
using SystemStringBuilder = System.Text.StringBuilder;
using SystemFileMode = System.IO.FileMode;
using SystemFileAccess = System.IO.FileAccess;
using SystemSeekOrigin = System.IO.SeekOrigin;
using SystemStream = System.IO.Stream;
using SystemFileStream = System.IO.FileStream;
using SystemBinaryReader = System.IO.BinaryReader;
using SystemStreamReader = System.IO.StreamReader;
using SystemWebResponse = System.Net.WebResponse;
using SystemHttpWebRequest = System.Net.HttpWebRequest;

namespace NovaEngine.Network
{
    /// <summary>
    /// 基于HTTP模式封装的的网络上传管理器对象类，用于对HTTP服务的网络文件提供上传接口
    /// </summary>
    public static class HttpUploader
    {
        /// <summary>
        /// HTTP模式资源上传回调接口
        /// </summary>
        /// <param name="arg">回调参数</param>
        public delegate void OnHttpUploadHandler(object arg);

        /// <summary>
        /// 网络HTTP模式上传数据最大缓冲区限制字节数
        /// </summary>
        private const int HTTP_UPLOAD_MAX_BUFFER_SIZE = 4096;

        /// <summary>
        /// 当前HTTP对象下所有上传操作的运行状态标识
        /// </summary>
        private static bool s_isUploadRunning = false;

        /// <summary>
        /// HTTP服务当前所有上传行为停止操作接口
        /// </summary>
        public static void StopUpload()
        {
            s_isUploadRunning = false;
        }

        /// <summary>
        /// HTTP服务数据上传操作接口
        /// </summary>
        /// <param name="path">资源路径</param>
        /// <param name="url">上传网络地址</param>
        /// <param name="filename">上传文件名称</param>
        /// <param name="args">扩展参数</param>
        /// <param name="transmit">传输回调接口实例</param>
        public static void Upload(string path, string url, string filename, string args, IHttpTransmit transmit)
        {
            Upload(path, url, filename, args,
                delegate (object arg)
                {
                    transmit?.OnTransmitProgressed((float) arg);
                },
                delegate (object arg)
                {
                    transmit?.OnTransmitCompleted((string) arg);
                },
                delegate (object arg)
                {
                    transmit?.OnTransmitException((string) arg);
                });
        }

        /// <summary>
        /// HTTP服务数据上传操作接口
        /// </summary>
        /// <param name="path">资源路径</param>
        /// <param name="url">上传网络地址</param>
        /// <param name="filename">上传文件名称</param>
        /// <param name="args">扩展参数</param>
        /// <param name="progress">进度回调接口</param>
        /// <param name="succeed">成功回调接口</param>
        /// <param name="failed">失败回调接口</param>
        public static void Upload(string path, string url, string filename, string args,
            OnHttpUploadHandler progress, OnHttpUploadHandler succeed, OnHttpUploadHandler failed)
        {
            // 默认启动上传运行状态
            s_isUploadRunning = true;

            SystemFileStream fstream = new SystemFileStream(path, SystemFileMode.Open, SystemFileAccess.Read);

            SystemBinaryReader reader = new SystemBinaryReader(fstream);

            string boundary = "----------" + SystemDateTime.Now.Ticks.ToString("x");

            byte[] boundaryBytes = SystemEncoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");

            // 请求头部信息
            SystemStringBuilder sb = new SystemStringBuilder();
            sb.Append("--");
            sb.Append(boundary);
            sb.Append("\r\n");
            sb.Append("Content-Disposition: form-data; name=\"");
            sb.Append("file");
            sb.Append("\"; filename=\"");
            sb.Append(filename);
            sb.Append("\"");
            sb.Append("\r\n");
            sb.Append("Content-Type: ");
            sb.Append("application/octet-stream");
            sb.Append("\r\n");
            sb.Append("\r\n");
            string header = sb.ToString();
            byte[] headerBytes = SystemEncoding.UTF8.GetBytes(header);

            SystemHttpWebRequest httpReq = SystemHttpWebRequest.Create(url) as SystemHttpWebRequest;
            httpReq.Method = "POST";
            httpReq.AllowWriteStreamBuffering = false;

            // 设置获得响应的超时时间（300秒）
            httpReq.Timeout = 300000;
            httpReq.ContentType = "multipart/form-data; boundary=" + boundary;
            long length = fstream.Length + headerBytes.Length + boundaryBytes.Length;
            long fileLength = fstream.Length;

            try
            {
                SystemStream postStream = null;

                // 写入附带的参数
                string stringKeyHeader = "\r\n--" + boundary +
                           "\r\nContent-Disposition: form-data; name=\"{0}\"" +
                           "\r\n\r\n{1}\r\n";
                string fieldsStr = "";

                if (null == args)
                {
                    args = "";
                }
                string[] temp = args.Split('|');
                if (temp.Length >= 2 && (temp.Length % 2) == 0)
                {
                    for (int n = 0; n < temp.Length; n += 2)
                    {
                        string field_key = temp[n];
                        string field_val = temp[n + 1];
                        fieldsStr += string.Format(stringKeyHeader, field_key, field_val);
                    }
                }

                if (fieldsStr.Length == 0)
                {
                    httpReq.ContentLength = length;
                    postStream = httpReq.GetRequestStream();
                }
                else
                {
                    byte[] postParamsBytes = SystemEncoding.UTF8.GetBytes(fieldsStr);
                    length += postParamsBytes.Length;
                    httpReq.ContentLength = length;
                    postStream = httpReq.GetRequestStream();
                    postStream.Write(postParamsBytes, 0, postParamsBytes.Length);
                }

                // 每次上传4k
                int bufferLength = HTTP_UPLOAD_MAX_BUFFER_SIZE;
                byte[] buffer = new byte[bufferLength];

                // 已上传的字节数
                long offset = 0;

                // 开始上传时间
                // SystemDateTime startTime = SystemDateTime.Now;
                int size = reader.Read(buffer, 0, bufferLength);

                // 发送请求头部消息
                postStream.Write(headerBytes, 0, headerBytes.Length);

                while (size > 0)
                {
                    if (false == s_isUploadRunning)
                    {
                        throw new SystemException("The upload process was stopped!");
                    }

                    postStream.Write(buffer, 0, size);

                    float p = (float) offset / (float) fileLength;
                    progress(p);

                    offset += size;

                    // SystemThread.Sleep(1);

                    size = reader.Read(buffer, 0, bufferLength);
                }

                // 添加尾部的时间戳
                postStream.Write(boundaryBytes, 0, boundaryBytes.Length);
                postStream.Close();

                // 获取服务器端的响应
                SystemWebResponse webRespon = httpReq.GetResponse();
                SystemStream s = webRespon.GetResponseStream();
                SystemStreamReader sr = new SystemStreamReader(s);

                // 读取服务器端返回的消息
                var sReturnString = sr.ReadToEnd();
                s.Close();
                sr.Close();

                succeed(sReturnString);
            }
            catch (SystemException e)
            {
                Logger.Error("An error occurred for upload file to target url {0}: {1}.", url, e.ToString());

                failed(e.ToString());
            }
            finally
            {
                fstream.Close();
                reader.Close();
            }
        }
    }
}
