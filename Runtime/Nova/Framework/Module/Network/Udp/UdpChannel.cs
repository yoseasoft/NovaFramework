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

namespace NovaEngine
{
    /// <summary>
    /// UDP模式网络通道对象抽象基类
    /// </summary>
    internal sealed partial class UdpChannel : NetworkChannel
    {

        /// <summary>
        /// TCP网络通道对象的新实例构建接口
        /// </summary>
        /// <param name="name">通道名称</param>
        /// <param name="url">网络地址参数</param>
        /// <param name="service">服务对象实例</param>
        public UdpChannel(string name, string url, UdpService service) : base(name, url, service)
        {
        }

        /// <summary>
        /// 网络通道关闭操作回调接口
        /// </summary>
        protected override void OnClose()
        {
            base.OnClose();
        }

        /// <summary>
        /// 网络通道连接操作接口
        /// </summary>
        public override void Connect()
        {
        }

        /// <summary>
        /// 网络通道数据下行操作接口
        /// </summary>
        /// <param name="message">消息内容</param>
        public override void Send(string message)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// 网络通道数据下行操作接口
        /// </summary>
        /// <param name="message">消息内容</param>
        public override void Send(byte[] message)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// 网络通道数据下行操作接口
        /// </summary>
        /// <param name="memoryStream">消息数据流</param>
        public override void Send(System.IO.MemoryStream memoryStream)
        {
            throw new System.NotImplementedException();
        }
    }
}
