/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
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

using System.Collections.Generic;

using SystemType = System.Type;

using UniTaskForMessage = Cysharp.Threading.Tasks.UniTask<ProtoBuf.Extension.IMessage>;
using UniTaskCompletionSourceForMessage = Cysharp.Threading.Tasks.UniTaskCompletionSource<ProtoBuf.Extension.IMessage>;

namespace GameEngine
{
    /// <summary>
    /// 对网络通道进行封装后的消息通信对象类
    /// </summary>
    public abstract class MessageChannel
    {
        /// <summary>
        /// 用于映射对应通道的唯一标识
        /// </summary>
        protected int m_channelID;

        /// <summary>
        /// 网络通道的类型标识
        /// </summary>
        protected int m_channelType;

        /// <summary>
        /// 消息通道的名称
        /// </summary>
        protected string m_name;

        /// <summary>
        /// 消息通道的远程访问地址
        /// </summary>
        protected string m_url;

        /// <summary>
        /// 消息通道的数据解析实例
        /// </summary>
        protected IMessageTranslator m_messageTranslator;

        /// <summary>
        /// 消息通信对象当前连接状态标识
        /// </summary>
        protected bool m_isConnected;

        public int ChannelID { get { return m_channelID; } }
        public int ChannelType { get { return m_channelType; } }
        public string Name { get { return m_name; } internal set { m_name = value; } }
        public string Url { get { return m_url; } internal set { m_url = value; } }
        public IMessageTranslator MessageTranslator { get { return m_messageTranslator; } }
        public bool IsConnected { get { return m_isConnected; } }

        protected MessageChannel(int channelID, int channelType) : this(channelID, channelType, string.Empty)
        { }

        protected MessageChannel(int channelID, int channelType, string name) : this(channelID, channelType, name, string.Empty)
        { }

        protected MessageChannel(int channelID, int channelType, string name, string url)
        {
            m_channelID = channelID;
            m_channelType = channelType;
            m_name = name;
            m_url = url;

            m_isConnected = false;
        }

        /// <summary>
        /// 通道对象初始化函数接口
        /// </summary>
        protected virtual void Initialize()
        {
            m_messageTranslator = NetworkHandler.Instance.GetMessageTranslatorByServiceType(m_channelType);
            if (null == m_messageTranslator)
            {
                Debugger.Warn("Could not found any valid message translator with target service type '{0}', initialized channel instance failed.", m_channelType);
            }
        }

        /// <summary>
        /// 通道对象清理函数接口
        /// </summary>
        protected virtual void Cleanup()
        {
            // 重置消息解析器
            m_messageTranslator = null;
        }

        /// <summary>
        /// 通道对象的实例销毁接口函数
        /// </summary>
        public void Destroy()
        {
            OnDisconnected();

            Cleanup();
        }

        /// <summary>
        /// 通过指定的通道标识和类型创建一个通道对象实例
        /// </summary>
        /// <param name="channelID">通道标识</param>
        /// <param name="channelType">通道类型</param>
        /// <returns>返回创建的通道对象实例，若创建失败则返回null</returns>
        public static MessageChannel Create(int channelID, int channelType)
        {
            return Create(channelID, channelType, string.Empty);
        }

        /// <summary>
        /// 通过指定的通道标识和类型创建一个通道对象实例
        /// </summary>
        /// <param name="channelID">通道标识</param>
        /// <param name="channelType">通道类型</param>
        /// <param name="name">通道名称</param>
        /// <returns>返回创建的通道对象实例，若创建失败则返回null</returns>
        public static MessageChannel Create(int channelID, int channelType, string name)
        {
            return Create(channelID, channelType, name, string.Empty);
        }

        /// <summary>
        /// 通过指定的通道标识和类型创建一个通道对象实例
        /// </summary>
        /// <param name="channelID">通道标识</param>
        /// <param name="channelType">通道类型</param>
        /// <param name="name">通道名称</param>
        /// <param name="url">目标地址</param>
        /// <returns>返回创建的通道对象实例，若创建失败则返回null</returns>
        public static MessageChannel Create(int channelID, int channelType, string name, string url)
        {
            SystemType classType = NetworkHandler.Instance.GetMessageChannelTypeByServiceType(channelType);
            Debugger.Assert(null != classType, "Invalid channel type '{0}'.", channelType);

            MessageChannel channel = System.Activator.CreateInstance(classType, new object[] { channelID, name, url }) as MessageChannel;
            channel.Initialize();

            return channel;
        }

        /// <summary>
        /// 销毁指定的通道对象实例
        /// </summary>
        /// <param name="channel">通道对象实例</param>
        public static void Destroy(MessageChannel channel)
        {
            channel.Destroy();
        }

        /// <summary>
        /// 消息通道连接成功的回调函数
        /// </summary>
        internal void OnConnected()
        {
            m_isConnected = true;
        }

        /// <summary>
        /// 消息通道连接断开的回调函数
        /// </summary>
        internal void OnDisconnected()
        {
            m_isConnected = false;
        }

        /// <summary>
        /// 通道的消息发送标准接口函数
        /// </summary>
        /// <param name="buffer">消息字符串</param>
        protected void Send(string buffer)
        {
            NetworkHandler.Instance.SendMessage(m_channelID, buffer);
        }

        /// <summary>
        /// 通道的消息发送标准接口函数
        /// </summary>
        /// <param name="buffer">消息字节流</param>
        protected void Send(byte[] buffer)
        {
            NetworkHandler.Instance.SendMessage(m_channelID, buffer);
        }
    }
}
