/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyright (C) 2022 - 2023, Shanghai Bilibili Technology Co., Ltd.
/// Copyright (C) 2023 - 2024, Guangzhou Shiyue Network Technology Co., Ltd.
/// Copyright (C) 2025, Hainan Yuanyou Information Tecdhnology Co., Ltd. Guangzhou Branch
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

namespace GameEngine.Profiler.Statistics
{
    /// <summary>
    /// 网络模块统计项对象类，对网络模块操作记录进行单项统计的数据单元
    /// </summary>
    public sealed class NetworkStatInfo : StatInfo
    {
        /// <summary>
        /// 通道连接的地址
        /// </summary>
        public string Url { get; internal set; }
        /// <summary>
        /// 通道发送数据的次数
        /// </summary>
        public int SendCount { get; internal set; }
        /// <summary>
        /// 通道接收数据的次数
        /// </summary>
        public int RecvCount { get; internal set; }
        /// <summary>
        /// 通道发送数据的大小
        /// </summary>
        public int SendSize { get; internal set; }
        /// <summary>
        /// 通道接收数据的大小
        /// </summary>
        public int RecvSize { get; internal set; }

        public NetworkStatInfo(int uid) : base(uid)
        {
            this.Url = string.Empty;
            this.SendCount = 0;
            this.RecvCount = 0;
            this.SendSize = 0;
            this.RecvSize = 0;
        }
    }
}
