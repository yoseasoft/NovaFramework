/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
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

namespace GameEngine
{
    /// <summary>
    /// 网络管理器的连接状态监听接口类，用于对网络模块相关连接状态事件的转发操作进行监听回调
    /// </summary>
    public interface INetworkConnectionListener
    {
        /// <summary>
        /// 网络连接成功的回调通知接口函数
        /// </summary>
        /// <param name="channel">消息通信对象实例</param>
        void OnConnection(MessageChannel channel);

        /// <summary>
        /// 网络连接断开的回调通知接口函数
        /// </summary>
        /// <param name="channel">消息通信对象实例</param>
        void OnDisconnection(MessageChannel channel);

        /// <summary>
        /// 网络连接异常的回调通知接口函数
        /// </summary>
        /// <param name="channel">消息通信对象实例</param>
        void OnConnectError(MessageChannel channel);
    }
}
