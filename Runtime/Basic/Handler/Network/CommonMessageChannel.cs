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
    /// 对TCP模式的网络通道进行封装后的消息通信对象类
    /// </summary>
    public sealed class TcpMessageChannel : SocketMessageChannel
    {
        public TcpMessageChannel(int channelID) : this(channelID, string.Empty)
        { }

        public TcpMessageChannel(int channelID, string name) : this(channelID, name, string.Empty)
        { }

        public TcpMessageChannel(int channelID, string name, string url) : base(channelID, (int) NovaEngine.Module.NetworkServiceType.Tcp, name, url)
        { }
    }

    /// <summary>
    /// 对WebSocket模式的网络通道进行封装后的消息通信对象类
    /// </summary>
    public sealed class WebSocketMessageChannel : SocketMessageChannel
    {
        public WebSocketMessageChannel(int channelID) : this(channelID, string.Empty)
        { }

        public WebSocketMessageChannel(int channelID, string name) : this(channelID, name, string.Empty)
        { }

        public WebSocketMessageChannel(int channelID, string name, string url) : base(channelID, (int) NovaEngine.Module.NetworkServiceType.WebSocket, name, url)
        { }
    }

    /// <summary>
    /// 对Http模式的网络通道进行封装后的消息通信对象类
    /// </summary>
    public sealed class HttpMessageChannel : WebMessageChannel
    {
        public HttpMessageChannel(int channelID) : this(channelID, string.Empty)
        { }

        public HttpMessageChannel(int channelID, string name) : this(channelID, name, string.Empty)
        { }

        public HttpMessageChannel(int channelID, string name, string url) : base(channelID, (int) NovaEngine.Module.NetworkServiceType.Http, name, url)
        { }
    }
}
