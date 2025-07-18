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

using System.Collections.Generic;

using SystemType = System.Type;

using UniTaskForMessage = Cysharp.Threading.Tasks.UniTask<ProtoBuf.Extension.IMessage>;
using UniTaskCompletionSourceForMessage = Cysharp.Threading.Tasks.UniTaskCompletionSource<ProtoBuf.Extension.IMessage>;

namespace GameEngine
{
    /// <summary>
    /// 对Web服务模式的网络通道进行封装后的消息通信对象类
    /// </summary>
    public abstract class WebMessageChannel : MessageChannel
    {
        protected WebMessageChannel(int channelID, int channelType) : base(channelID, channelType)
        { }

        protected WebMessageChannel(int channelID, int channelType, string name) : base(channelID, channelType, name)
        { }

        protected WebMessageChannel(int channelID, int channelType, string name, string url) : base(channelID, channelType, name, url)
        { }

        /// <summary>
        /// 通道对象初始化函数接口
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
        }

        /// <summary>
        /// 通道对象清理函数接口
        /// </summary>
        protected override void Cleanup()
        {
            base.Cleanup();
        }

        public void Send(int type, string message)
        {
        }

        public void Send(int type, byte[] data)
        {
        }
    }
}
