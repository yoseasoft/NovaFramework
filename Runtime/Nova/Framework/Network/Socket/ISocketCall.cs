/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
/// Copyring (C) 2017 - 2020, Shanghai Tommon Network Technology Co., Ltd.
/// Copyring (C) 2020 - 2022, Guangzhou Xinyuan Technology Co., Ltd.
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

namespace NovaEngine.Network
{
    /// <summary>
    /// 基于SOCKET网络模式下的回调通知接口
    /// </summary>
    public interface ISocketCall
    {
        /// <summary>
        /// 网络SOCKET连接成功回调通知接口
        /// </summary>
        /// <param name="fd">对应网络通道标识</param>
        void OnConnection(int fd);

        /// <summary>
        /// 网络SOCKET断开连接回调通知接口
        /// </summary>
        /// <param name="fd">对应网络通道标识</param>
        void OnDisconnection(int fd);

        /// <summary>
        /// 网络SOCKET连接异常回调通知接口
        /// </summary>
        /// <param name="fd">对应网络通道标识</param>
        void OnConnectError(int fd);

        /// <summary>
        /// 网络SOCKET数据下行回调通知接口
        /// </summary>
        /// <param name="fd">对应网络通道标识</param>
        /// <param name="message">下行数据字节流</param>
        void OnReceivedMessage(int fd, byte[] message);
    }
}
