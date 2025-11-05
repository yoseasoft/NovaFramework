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

using NovaEngine;

namespace GameEngine
{
    /// <summary>
    /// 套接字类型通道的消息解析器对象类，用于对套接字通道的网络消息数据进行加工
    /// </summary>
    public abstract class SocketMessageTranslator : IMessageTranslator
    {
        /// <summary>
        /// 消息操作码的长度，以字节为单位
        /// </summary>
        private const int OpcodeSize = 2;

        /// <summary>
        /// 将指定的消息内容编码为可发送的消息字节流
        /// </summary>
        /// <param name="message">消息内容</param>
        /// <returns>若编码有效的数据则返回其对应的字节流，否则返回null</returns>
        public byte[] Encode(object message)
        {
            ProtoBuf.Extension.IMessage msg = message as ProtoBuf.Extension.IMessage;
            Debugger.Assert(null != msg, NovaEngine.ErrorText.InvalidArguments);

            int opcode = NetworkHandler.Instance.GetOpcodeByMessageType(message.GetType());
            byte[] msgBytes = ProtoBuf.Extension.ProtobufHelper.ToBytes(message);

            byte[] buffer = new byte[msgBytes.Length + OpcodeSize];
            buffer.WriteToBig(0, (ushort) opcode);
            for (int n = 0; n < msgBytes.Length; ++n)
            {
                buffer[n + OpcodeSize] = msgBytes[n];
            }

            return buffer;
        }

        /// <summary>
        /// 将指定的消息字节流解码成消息内容
        /// </summary>
        /// <param name="buffer">消息字节流</param>
        /// <returns>返回解码后的消息内容，若解码失败则返回null</returns>
        public object Decode(byte[] buffer)
        {
            // short opcode = System.BitConverter.ToInt16(buffer, 0);
            // opcode = System.Net.IPAddress.NetworkToHostOrder(opcode);
            int opcode = buffer.ReadBigUInt16(0);
            System.Type messageType = NetworkHandler.Instance.GetMessageClassByType(opcode);

            object message = ProtoBuf.Extension.ProtobufHelper.FromBytes(messageType, buffer, OpcodeSize, buffer.Length - OpcodeSize);

            return message;
        }
    }
}
