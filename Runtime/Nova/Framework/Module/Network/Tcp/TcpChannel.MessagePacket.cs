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
            private readonly int _headerSize = 0;

            private readonly IO.CircularLinkedBuffer _buffer = null;

            private SystemMemoryStream _memoryStream = null;

            private ParseStateType _stateType;
            private int _packetSize = 0;
            private bool _isCompleted = false;

            public MessagePacket(int headerSize, IO.CircularLinkedBuffer buffer, SystemMemoryStream memoryStream)
            {
                this._headerSize = headerSize;
                this._buffer = buffer;
                this._memoryStream = memoryStream;

                this._stateType = ParseStateType.Header;
                this._packetSize = 0;
                this._isCompleted = false;
            }

            /// <summary>
            /// 解析数据包
            /// </summary>
            /// <returns>返回解析数据结果</returns>
            public bool ParsePacket()
            {
                if (this._isCompleted)
                {
                    return true;
                }

                bool finished = false;
                while (false == finished)
                {
                    switch (this._stateType)
                    {
                        case ParseStateType.Header:
                            if (this._buffer.Length < this._headerSize)
                            {
                                finished = true;
                            }
                            else
                            {
                                this._buffer.Read(this._memoryStream.GetBuffer(), 0, this._headerSize);

                                switch (this._headerSize)
                                {
                                    case MessageConstant.HeaderSize4:
                                        this._packetSize = SystemBitConverter.ToInt32(this._memoryStream.GetBuffer(), 0);
                                        if (this._packetSize > ushort.MaxValue * 16 || this._packetSize < 2)
                                        {
                                            throw new CFrameworkException("receive header size '{0}' out of the range.", this._packetSize);
                                        }
                                        break;
                                    case MessageConstant.HeaderSize2:
                                        short messageLength = SystemBitConverter.ToInt16(this._memoryStream.GetBuffer(), 0);
                                        this._packetSize = SystemIPAddress.NetworkToHostOrder(messageLength);
                                        if (this._packetSize > ushort.MaxValue || this._packetSize < 2)
                                        {
                                            throw new CFrameworkException("receive header size '{0}' out of the range.", this._packetSize);
                                        }
                                        break;
                                    default:
                                        throw new CFrameworkException("network packet header size '{0}' error.", this._headerSize);
                                }

                                this._stateType = ParseStateType.Body;
                            }
                            break;

                        case ParseStateType.Body:
                            if (this._buffer.Length < this._packetSize)
                            {
                                finished = true;
                            }
                            else
                            {
                                this._memoryStream.Seek(0, SystemSeekOrigin.Begin);
                                this._memoryStream.SetLength(this._packetSize);

                                byte[] bytes = this._memoryStream.GetBuffer();
                                this._buffer.Read(bytes, 0, this._packetSize);
                                this._isCompleted = true;
                                this._stateType = ParseStateType.Header;

                                finished = true;
                            }
                            break;
                    }
                }

                return this._isCompleted;
            }

            /// <summary>
            /// 提取解析后的数据包
            /// </summary>
            /// <returns>返回数据流</returns>
            public SystemMemoryStream GetPacket()
            {
                if (false == this._isCompleted)
                {
                    return null;
                }

                this._isCompleted = false;
                return this._memoryStream;
            }
        }
    }
}
