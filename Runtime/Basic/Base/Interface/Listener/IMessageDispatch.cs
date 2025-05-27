/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyring (C) 2023, Guangzhou Shiyue Network Technology Co., Ltd.
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
    /// 消息监听接口类，用于定义接收监听消息的函数接口
    /// </summary>
    public interface IMessageDispatch : IListener
    {
        /// <summary>
        /// 对象内部消息通知的监听回调接口，通过该类型函数对指定消息进行监听处理
        /// </summary>
        // public delegate void MessageDispatchingListenerForNullParameter();

        /// <summary>
        /// 对象内部消息通知的监听回调接口，通过该类型函数对指定类型消息进行监听处理
        /// </summary>
        /// <param name="message">消息对象实例</param>
        // public delegate void EventDispatchingListener(ProtoBuf.Extension.IMessage message);

        /// <summary>
        /// 接收监听指定类型的消息的回调接口函数
        /// </summary>
        /// <param name="opcode">协议操作码</param>
        /// <param name="message">消息对象实例</param>
        void OnMessageDispatch(int opcode, ProtoBuf.Extension.IMessage message);
    }
}
