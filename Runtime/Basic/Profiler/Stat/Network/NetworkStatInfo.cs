/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
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

using SystemDateTime = System.DateTime;

namespace GameEngine
{
    /// <summary>
    /// 网络模块统计项对象类，对网络模块操作记录进行单项统计的数据单元
    /// </summary>
    public sealed class NetworkStatInfo : IStatInfo
    {
        /// <summary>
        /// 通道的会话标识
        /// </summary>
        private readonly int _session;
        /// <summary>
        /// 通道连接的地址
        /// </summary>
        private string _url;
        /// <summary>
        /// 通道连接的时间
        /// </summary>
        private SystemDateTime _connectTime;
        /// <summary>
        /// 通道断开连接的时间
        /// </summary>
        private SystemDateTime _disconnectTime;
        /// <summary>
        /// 通道发送数据的次数
        /// </summary>
        private int _sendCount;
        /// <summary>
        /// 通道接收数据的次数
        /// </summary>
        private int _recvCount;
        /// <summary>
        /// 通道发送数据的大小
        /// </summary>
        private int _sendSize;
        /// <summary>
        /// 通道接收数据的大小
        /// </summary>
        private int _recvSize;

        public NetworkStatInfo(int session)
        {
            _session = session;
            _url = string.Empty;
            _connectTime = SystemDateTime.MinValue;
            _disconnectTime = SystemDateTime.MinValue;
            _sendCount = 0;
            _recvCount = 0;
            _sendSize = 0;
            _recvSize = 0;
        }

        public int Session
        {
            get { return _session; }
        }

        public string Url
        {
            get { return _url; }
            set { _url = value; }
        }

        public SystemDateTime ConnectTime
        {
            get { return _connectTime; }
            set { _connectTime = value; }
        }

        public SystemDateTime DisconnectTime
        {
            get { return _disconnectTime; }
            set { _disconnectTime = value; }
        }

        public int SendCount
        {
            get { return _sendCount; }
            set { _sendCount = value; }
        }

        public int RecvCount
        {
            get { return _recvCount; }
            set { _recvCount = value; }
        }

        public int SendSize
        {
            get { return _sendSize; }
            set { _sendSize = value; }
        }

        public int RecvSize
        {
            get { return _recvSize; }
            set { _recvSize = value; }
        }
    }
}
