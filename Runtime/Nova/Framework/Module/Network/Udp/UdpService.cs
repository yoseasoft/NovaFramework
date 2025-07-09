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
    /// UDP模式网络服务接口管理基类
    /// </summary>
    public class UdpService : NetworkService
    {
        /// <summary>
        /// 获取网络通道的服务类型
        /// </summary>
        public override int ServiceType => (int) NetworkServiceType.Udp;

        /// <summary>
        /// UDP模式网络服务对象的新实例构建接口
        /// </summary>
        public UdpService()
        {
        }

        /// <summary>
        /// UDP模式网络服务对象的实例析构接口
        /// </summary>
        ~UdpService()
        {
        }

        /// <summary>
        /// 创建一个指定名称和地址的通道对象实例
        /// </summary>
        /// <param name="name">通道名称</param>
        /// <param name="url">通道地址</param>
        /// <returns>返回新创建的通道实例，若实例创建失败则返回null</returns>
        public override NetworkChannel CreateChannel(string name, string url)
        {
            UdpChannel channel = new UdpChannel(name, url, this);

            return channel;
        }

        /// <summary>
        /// 释放指定标识对应的通道对象实例
        /// </summary>
        /// <param name="channelID">通道标识</param>
        public override void ReleaseChannel(int channelID)
        {
        }

        public override void Update()
        {
        }
    }
}
