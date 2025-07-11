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
    /// 消息解析器抽象类，用于定义通道消息的编码/解码的接口函数，提供对应的网络服务使用
    /// </summary>
    public interface IMessageTranslator
    {
        /// <summary>
        /// 将指定的消息内容编码为可发送的消息字节流
        /// </summary>
        /// <param name="message">消息内容</param>
        /// <returns>若数据编码成功则返回其对应的字节流，否则返回null</returns>
        byte[] Encode(object message);

        /// <summary>
        /// 将指定的消息字节流解码成消息内容
        /// </summary>
        /// <param name="buffer">消息字节流</param>
        /// <returns>返回解码后的消息内容，若解码失败则返回null</returns>
        object Decode(byte[] buffer);
    }
}
