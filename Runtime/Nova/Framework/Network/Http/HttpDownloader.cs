/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
/// Copyright (C) 2020 - 2022, Guangzhou Xinyuan Technology Co., Ltd.
/// Copyright (C) 2022 - 2023, Shanghai Bilibili Technology Co., Ltd.
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

using SystemException = System.Exception;
using SystemIEnumerator = System.Collections.IEnumerator;
using SystemFileMode = System.IO.FileMode;
using SystemFileAccess = System.IO.FileAccess;
using SystemSeekOrigin = System.IO.SeekOrigin;
using SystemStream = System.IO.Stream;
using SystemFileStream = System.IO.FileStream;
using SystemHttpWebRequest = System.Net.HttpWebRequest;
using SystemHttpWebResponse = System.Net.HttpWebResponse;
using SystemThread = System.Threading.Thread;

namespace NovaEngine.Network
{
    /// <summary>
    /// 基于HTTP模式封装的的网络下载管理器对象类，用于对HTTP服务的网络文件提供下载接口
    /// </summary>
    public static class HttpDownloader
    {
        /// <summary>
        /// HTTP模式资源下载回调接口
        /// </summary>
        /// <param name="arg">回调参数</param>
        public delegate void OnHttpDownloadHandler(object arg);

        /// <summary>
        /// 网络HTTP模式下载数据最大缓冲区限制字节数
        /// </summary>
        private const int HTTP_DOWNLOAD_MAX_BUFFER_SIZE = 1024;

        /// <summary>
        /// 当前HTTP对象下所有下载操作的运行状态标识
        /// </summary>
        private static bool _isDownloadRunning = false;

        /// <summary>
        /// HTTP服务当前所有下载行为停止操作接口
        /// </summary>
        public static void StopDownload()
        {
            _isDownloadRunning = false;
        }

        /// <summary>
        /// HTTP服务数据下载操作接口
        /// </summary>
        /// <param name="url">远程资源访问地址</param>
        /// <param name="path">本地资源保存路径</param>
        /// <param name="size">原始资源数据大小</param>
        /// <param name="msTime">下载分时间隔</param>
        /// <param name="transmit">传输回调接口实例</param>
        public static void Download(string url, string path, long size, int msTime, IHttpTransmit transmit)
        {
            Download(url, path, size, msTime,
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
        /// HTTP服务数据下载操作接口
        /// </summary>
        /// <param name="url">远程资源访问地址</param>
        /// <param name="path">本地资源保存路径</param>
        /// <param name="size">原始资源数据大小</param>
        /// <param name="msTime">下载分时间隔</param>
        /// <param name="progress">进度回调接口</param>
        /// <param name="succeed">成功回调接口</param>
        /// <param name="failed">失败回调接口</param>
        public static void Download(string url, string path, long size, int msTime,
            OnHttpDownloadHandler progress, OnHttpDownloadHandler succeed, OnHttpDownloadHandler failed)
        {
            // 默认启动下载运行状态
            _isDownloadRunning = true;

            SystemFileStream fstream = null;
            SystemStream respStream = null;
            SystemHttpWebRequest request = null;
            SystemHttpWebResponse response = null;

            // 获取文件现在的长度
            long fileLength = 0;
            bool isDone = false;
            try
            {
                Utility.Path.CreateDirectory(path);
                Utility.Path.DeleteFile(path); // 断点续传

                fstream = new SystemFileStream(path, SystemFileMode.OpenOrCreate, SystemFileAccess.Write);
                // 断点续传核心，设置本地文件流的起始位置
                fstream.Seek(fileLength, SystemSeekOrigin.Begin);
                request = SystemHttpWebRequest.Create(url) as SystemHttpWebRequest;

                request.KeepAlive = false;
                request.Proxy = SystemHttpWebRequest.DefaultWebProxy;

                // 断点续传核心，设置远程访问文件流的起始位置
                request.AddRange((int) fileLength);

                response = (SystemHttpWebResponse) request.GetResponse();
                respStream = response.GetResponseStream();

                int bufferLength = HTTP_DOWNLOAD_MAX_BUFFER_SIZE;
                byte[] buffer = new byte[bufferLength];
                int length = respStream.Read(buffer, 0, bufferLength);
                while (length > 0)
                {
                    if (false == _isDownloadRunning) { break; }

                    fstream.Write(buffer, 0, length);
                    fileLength += length;
                    float p = (float) fileLength / (float) size;
                    progress(p);

                    if (msTime > 0)
                    {
                        SystemThread.Sleep(msTime);
                    }

                    length = respStream.Read(buffer, 0, bufferLength);
                }

                if (fileLength == size)
                {
                    isDone = true;
                }
            }
            catch (SystemException e)
            {
                Logger.Error("An error occurred for download file to target url {0}: {1}.", url, e.ToString());
                isDone = false;
            }
            finally
            {
                try
                {
                    if (null != fstream)
                    {
                        fstream.Close();
                        fstream.Dispose();
                        fstream = null;
                    }
                }
                catch (SystemException e) { Logger.Error(e.ToString()); }

                try
                {
                    if (null != respStream)
                    {
                        respStream.Close();
                        respStream.Dispose();
                        respStream = null;
                    }
                }
                catch (SystemException e) { Logger.Error(e.ToString()); }

                try
                {
                    if (null != request)
                    {
                        request.Abort();
                        request = null;
                    }
                }
                catch (SystemException e) { Logger.Error(e.ToString()); }

                try
                {
                    if (null != response)
                    {
                        response.Close();
                        response = null;
                    }
                }
                catch (SystemException e) { Logger.Error(e.ToString()); }
            }

            // if (false == s_isDownloadRunning) { return; }

            if (isDone)
            {
                succeed(path);
            }
            else
            {
                failed(string.Format("{0}|{1}", fileLength, size));
            }
        }
    }
}
