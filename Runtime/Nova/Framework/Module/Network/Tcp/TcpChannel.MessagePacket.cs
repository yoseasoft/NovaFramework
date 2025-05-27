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

using SystemBitConverter = System.BitConverter;
using SystemMemoryStream = System.IO.MemoryStream;
using SystemSeekOrigin = System.IO.SeekOrigin;
using SystemIPAddress = System.Net.IPAddress;

namespace NovaEngine
{
    /// <summary>
    /// TCP模式网络通道对象抽象基类
    /// </summary>
    public sealed partial class TcpChannel
    {
        /// <summary>
        /// TCP模式网络通道数据包对象实体类
        /// </summary>
        private sealed class MessagePacket
        {
            /// <summary>
            /// 解析状态类型定义
            /// </summary>
            private enum ParseStateType : byte
            {
                Header,
                Body,
            }

            /// <summary>
            /// 消息包的包头长度
            /// </summary>
            private readonly int m_headerSize = 0;

            private readonly IO.CircularLinkedBuffer m_buffer = null;

            private SystemMemoryStream m_memoryStream = null;

            private ParseStateType m_stateType;
            private int m_packetSize = 0;
            private bool m_isCompleted = false;

            public MessagePacket(int headerSize, IO.CircularLinkedBuffer buffer, SystemMemoryStream memoryStream)
            {
                this.m_headerSize = headerSize;
                this.m_buffer = buffer;
                this.m_memoryStream = memoryStream;

                this.m_stateType = ParseStateType.Header;
                this.m_packetSize = 0;
                this.m_isCompleted = false;
            }

            /// <summary>
            /// 解析数据包
            /// </summary>
            /// <returns>返回解析数据结果</returns>
            public bool ParsePacket()
            {
                if (this.m_isCompleted)
                {
                    return true;
                }

                bool finished = false;
                while (false == finished)
                {
                    switch (this.m_stateType)
                    {
                        case ParseStateType.Header:
                            if (this.m_buffer.Length < this.m_headerSize)
                            {
                                finished = true;
                            }
                            else
                            {
                                this.m_buffer.Read(this.m_memoryStream.GetBuffer(), 0, this.m_headerSize);

                                switch (this.m_headerSize)
                                {
                                    case MessageConstant.HeaderSize4:
                                        this.m_packetSize = SystemBitConverter.ToInt32(this.m_memoryStream.GetBuffer(), 0);
                                        if (this.m_packetSize > ushort.MaxValue * 16 || this.m_packetSize < 2)
                                        {
                                            throw new CFrameworkException("receive header size '{0}' out of the range.", this.m_packetSize);
                                        }
                                        break;
                                    case MessageConstant.HeaderSize2:
                                        short messageLength = SystemBitConverter.ToInt16(this.m_memoryStream.GetBuffer(), 0);
                                        this.m_packetSize = SystemIPAddress.NetworkToHostOrder(messageLength);
                                        if (this.m_packetSize > ushort.MaxValue || this.m_packetSize < 2)
                                        {
                                            throw new CFrameworkException("receive header size '{0}' out of the range.", this.m_packetSize);
                                        }
                                        break;
                                    default:
                                        throw new CFrameworkException("network packet header size '{0}' error.", this.m_headerSize);
                                }

                                this.m_stateType = ParseStateType.Body;
                            }
                            break;

                        case ParseStateType.Body:
                            if (this.m_buffer.Length < this.m_packetSize)
                            {
                                finished = true;
                            }
                            else
                            {
                                this.m_memoryStream.Seek(0, SystemSeekOrigin.Begin);
                                this.m_memoryStream.SetLength(this.m_packetSize);

                                byte[] bytes = this.m_memoryStream.GetBuffer();
                                this.m_buffer.Read(bytes, 0, this.m_packetSize);
                                this.m_isCompleted = true;
                                this.m_stateType = ParseStateType.Header;

                                finished = true;
                            }
                            break;
                    }
                }

                return this.m_isCompleted;
            }

            /// <summary>
            /// 提取解析后的数据包
            /// </summary>
            /// <returns>返回数据流</returns>
            public SystemMemoryStream GetPacket()
            {
                if (false == this.m_isCompleted)
                {
                    return null;
                }

                this.m_isCompleted = false;
                return this.m_memoryStream;
            }
        }
    }
}
