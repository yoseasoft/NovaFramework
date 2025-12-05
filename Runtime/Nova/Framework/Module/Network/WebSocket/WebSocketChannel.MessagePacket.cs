/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
/// Copyright (C) 2025, Hainan Yuanyou Information Technology Co., Ltd. Guangzhou Branch
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

using System;
using System.IO;
using System.Net;

namespace NovaEngine.Module
{
    /// <summary>
    /// WebSocket模式网络通道对象抽象基类
    /// </summary>
    internal sealed partial class WebSocketChannel
    {
        /// <summary>
        /// WebSocket模式网络通道数据包对象实体类
        /// </summary>
        private sealed class MessagePacket
        {
            /// <summary>
            /// 消息包的包头长度
            /// </summary>
            private readonly int _headerSize = 0;

            /// <summary>
            /// 数据包的包头缓冲区
            /// </summary>
            private readonly byte[] _packetHeaderCached = null;

            private MemoryStream _memoryStream = null;

            private Action<MemoryStream> _callback = null;

            public MessagePacket(int headerSize, MemoryStream memoryStream, Action<MemoryStream> callback)
            {
                this._headerSize = headerSize;
                this._packetHeaderCached = new byte[headerSize];

                this._memoryStream = memoryStream;

                this._callback = callback;
            }

            /// <summary>
            /// 数据包分发处理
            /// </summary>
            /// <param name="buffer">数据流</param>
            public void OnDispatched(IO.MemoryBuffer buffer)
            {
                bool finished = false;
                int offsetSize = 0;
                while (false == finished)
                {
                    if (buffer.Length - offsetSize < this._headerSize)
                    {
                        Logger.Error("network packet buffer length '{%d}' error.", buffer.Length - offsetSize);
                        break;
                    }

                    buffer.Read(this._packetHeaderCached, 0, this._headerSize);
                    offsetSize += this._headerSize;

                    int packetSize = 0;
                    switch (this._headerSize)
                    {
                        case MessageConstant.HeaderSize_4:
                            packetSize = BitConverter.ToInt32(this._packetHeaderCached, 0);
                            if (packetSize > MessageConstant.MaxPacketSize_4 || packetSize < MessageConstant.MinPacketSize_4)
                            {
                                throw new CFrameworkException("receive header size '{%d}' out of the range.", packetSize);
                            }
                            break;
                        case MessageConstant.HeaderSize_2:
                            short messageLength = BitConverter.ToInt16(this._packetHeaderCached, 0);
                            packetSize = IPAddress.NetworkToHostOrder(messageLength);
                            if (packetSize > MessageConstant.MaxPacketSize_2 || packetSize < MessageConstant.MinPacketSize_2)
                            {
                                throw new CFrameworkException("receive header size '{%d}' out of the range.", packetSize);
                            }
                            break;
                        default:
                            throw new CFrameworkException("network packet header size '{%d}' error.", this._headerSize);
                    }

                    if (buffer.Length - offsetSize < packetSize)
                    {
                        Logger.Error("network packet buffer length '{%d}' error.", buffer.Length - offsetSize);
                        break;
                    }

                    this._memoryStream.Seek(0, SeekOrigin.Begin);
                    this._memoryStream.SetLength(packetSize);

                    byte[] bytes = this._memoryStream.GetBuffer();
                    buffer.Read(bytes, 0, packetSize);
                    offsetSize += packetSize;

                    this._callback(this._memoryStream);

                    if (offsetSize == buffer.Length)
                    {
                        finished = true;
                    }
                }
            }
        }
    }
}
