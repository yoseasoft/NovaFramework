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

namespace NovaEngine
{
    /// <summary>
    /// 网络管理模块，处理网络相关的连接/断开，及数据包通信等访问接口
    /// </summary>
    internal partial class NetworkModule
    {
        /// <summary>
        /// 网络基础指令协议类型
        /// </summary>
        [System.Flags]
        public enum ProtocolType : byte
        {
            /// <summary>
            /// 未知状态
            /// </summary>
            Unknown = 0,

            /// <summary>
            /// SOCKET模式连接成功状态
            /// </summary>
            Connected = 11,

            /// <summary>
            /// SOCKET模式断开连接状态
            /// </summary>
            Disconnected = 12,

            /// <summary>
            /// SOCKET模式网络异常状态
            /// </summary>
            Exception = 13,

            /// <summary>
            /// SOCKET模式消息转发状态
            /// </summary>
            Dispatched = 14,

            /// <summary>
            /// 读取失败
            /// </summary>
            ReadError = 21,

            /// <summary>
            /// 写入失败
            /// </summary>
            WriteError = 22,

            /// <summary>
            /// HTTP模式上行请求状态
            /// </summary>
            HttpRequest = 121,

            /// <summary>
            /// HTTP模式下行响应状态
            /// </summary>
            HttpResponse = 122,

            /// <summary>
            /// HTTP模式连接异常状态
            /// </summary>
            HttpException = 123,
        }
    }
}
