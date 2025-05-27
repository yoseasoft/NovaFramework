/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
/// Copyring (C) 2020 - 2022, Guangzhou Xinyuan Technology Co., Ltd.
/// Copyring (C) 2022 - 2023, Shanghai Bilibili Technology Co., Ltd.
/// Copyring (C) 2023 - 2024, Guangzhou Shiyue Network Technology Co., Ltd.
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

using System.Collections.Generic;

using SystemException = System.Exception;
using SystemIPEndPoint = System.Net.IPEndPoint;

namespace NovaEngine
{
    /// <summary>
    /// TCP模式网络服务接口管理基类
    /// </summary>
    public class TcpService : NetworkService
    {
        /// <summary>
        /// 等待发送的记录队列
        /// </summary>
        private readonly IList<int> m_waitingSendQueue = new List<int>();

        /// <summary>
        /// 获取网络通道的服务类型
        /// </summary>
        public override int ServiceType => (int) NetworkServiceType.Tcp;

        /// <summary>
        /// TCP模式网络服务对象的新实例构建接口
        /// </summary>
        public TcpService()
        {
        }

        /// <summary>
        /// TCP模式网络服务对象的实例析构接口
        /// </summary>
        ~TcpService()
        {
            RemoveAllWaitingSendChannels();
        }

        /// <summary>
        /// 创建一个指定名称和地址的通道对象实例
        /// </summary>
        /// <param name="name">通道名称</param>
        /// <param name="url">通道地址</param>
        /// <returns>返回新创建的通道实例，若实例创建失败则返回null</returns>
        public override NetworkChannel CreateChannel(string name, string url)
        {
            TcpChannel channel = new TcpChannel(name, url, this);

            return channel;
        }

        /// <summary>
        /// 释放指定标识对应的通道对象实例
        /// </summary>
        /// <param name="channelID">通道标识</param>
        public override void ReleaseChannel(int channelID)
        {
            this.RemoveWaitingSendChannel(channelID);
        }

        public override void Update()
        {
            if (m_waitingSendQueue.Count > 0)
            {
                for (int n = 0; n < m_waitingSendQueue.Count; ++n)
                {
                    int channelID = m_waitingSendQueue[n];
                    TcpChannel channel = (TcpChannel) GetChannel(channelID);

                    if (channel.IsOnWriting)
                    {
                        continue;
                    }

                    try
                    {
                        channel.OnSend();
                    }
                    catch (SystemException e)
                    {
                        throw new CFrameworkException("Wait for send failed.", e);
                    }
                }

                // 重置待命列表
                RemoveAllWaitingSendChannels();
            }
        }

        /// <summary>
        /// 标识指定通道实例为待发送状态
        /// </summary>
        /// <param name="channelID">网络通道标识</param>
        public void WaitingForSend(int channelID)
        {
            this.AddWaitingSendChannel(channelID);
        }

        /// <summary>
        /// 添加指定通道标识到当前的待发送记录列表中
        /// </summary>
        /// <param name="channelID">网络通道标识</param>
        private void AddWaitingSendChannel(int channelID)
        {
            for (int n = 0; n < m_waitingSendQueue.Count; ++n)
            {
                int v = m_waitingSendQueue[n];
                if (v == channelID)
                {
                    return;
                }
            }

            m_waitingSendQueue.Add(channelID);
        }

        /// <summary>
        /// 移除指定通道标识的待发送记录
        /// </summary>
        /// <param name="channelID">网络通道标识</param>
        private void RemoveWaitingSendChannel(int channelID)
        {
            for (int n = m_waitingSendQueue.Count - 1; n >= 0; --n)
            {
                int v = m_waitingSendQueue[n];
                if (v == channelID)
                {
                    m_waitingSendQueue.RemoveAt(n);
                }
            }
        }

        /// <summary>
        /// 移除全部待发送记录的通道标识
        /// </summary>
        private void RemoveAllWaitingSendChannels()
        {
            m_waitingSendQueue.Clear();
        }
    }
}
