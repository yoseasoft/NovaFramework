/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyright (C) 2020 - 2022, Guangzhou Xinyuan Technology Co., Ltd.
/// Copyright (C) 2022 - 2023, Shanghai Bilibili Technology Co., Ltd.
/// Copyright (C) 2023 - 2024, Guangzhou Shiyue Network Technology Co., Ltd.
/// Copyright (C) 2025, Hainan Yuanyou Information Tecdhnology Co., Ltd. Guangzhou Branch
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

using SystemEnum = System.Enum;
using SystemType = System.Type;
using SystemDelegate = System.Delegate;
using SystemAction = System.Action;

namespace GameEngine
{
    /// <summary>
    /// 网络模块封装的句柄对象类
    /// 模块具体功能接口请参考<see cref="NovaEngine.NetworkModule"/>类
    /// </summary>
    public sealed partial class NetworkHandler : BaseHandler
    {
        /// <summary>
        /// 消息处理回调句柄
        /// </summary>
        /// <param name="message">消息内容</param>
        public delegate void MessageProcessHandler(ProtoBuf.Extension.IMessage message);

        /// <summary>
        /// 网络消息协议对象类映射字典
        /// </summary>
        private IDictionary<int, SystemType> _messageClassTypes;

        /// <summary>
        /// 网络消息通道对象管理容器
        /// </summary>
        private IDictionary<int, SystemType> _messageChannelTypes;
        /// <summary>
        /// 网络消息解析服务对象管理容器
        /// </summary>
        private IDictionary<int, IMessageTranslator> _messageTranslators;

        /// <summary>
        /// 通过协议编码分发调度接口的数据结构容器
        /// </summary>
        private IDictionary<int, IList<MessageCallInfo>> _messageDistributeCallInfos = null;

        /// <summary>
        /// 网络消息转发监听回调管理容器
        /// </summary>
        private IDictionary<int, IList<IMessageDispatch>> _messageDispatchListeners;

        /// <summary>
        /// 消息转发监听对象实例管理容器
        /// </summary>
        private IList<INetworkDispatchListener> _networkEventDispatchListeners;

        /// <summary>
        /// 消息通道对象实例管理容器
        /// </summary>
        private IDictionary<int, MessageChannel> _messageChannels;

        /// <summary>
        /// 消息的调用队列
        /// </summary>
        private Queue<SystemAction> _messageInvokeQueue;

        /// <summary>
        /// 句柄对象的单例访问获取接口
        /// </summary>
        public static NetworkHandler Instance => HandlerManagement.NetworkHandler;

        /// <summary>
        /// 句柄对象内置初始化接口函数
        /// </summary>
        /// <returns>若句柄对象初始化成功则返回true，否则返回false</returns>
        protected override bool OnInitialize()
        {
            // 初始化协议对象映射字典
            _messageClassTypes = new Dictionary<int, SystemType>();
            // 初始化网络消息通道对象管理容器
            _messageChannelTypes = new Dictionary<int, SystemType>();
            // 初始化网络消息解析对象管理容器
            _messageTranslators = new Dictionary<int, IMessageTranslator>();
            // 初始化回调句柄映射字典
            _messageDistributeCallInfos = new Dictionary<int, IList<MessageCallInfo>>();
            // 初始化消息转发监听回调映射字典
            _messageDispatchListeners = new Dictionary<int, IList<IMessageDispatch>>();

            // 初始化消息转发监听对象管理容器
            _networkEventDispatchListeners = new List<INetworkDispatchListener>();

            // 初始化消息通道对象管理容器
            _messageChannels = new Dictionary<int, MessageChannel>();

            // 初始化消息调用队列
            _messageInvokeQueue = new Queue<SystemAction>();

            // 加载消息通道数据
            LoadAllMessageChannelTypes();

            return true;
        }

        /// <summary>
        /// 句柄对象内置清理接口函数
        /// </summary>
        protected override void OnCleanup()
        {
            // 关闭所有连接
            DisconnectAllChannels();

            // 清理消息调用队列
            _messageInvokeQueue.Clear();
            _messageInvokeQueue = null;

            // 清理消息通道对象管理容器
            _messageChannels.Clear();
            _messageChannels = null;

            // 移除所有消息事件转发监听实例
            RemoveAllNetworkEventDispatchListeners();

            // 清理消息转发监听对象管理容器
            _networkEventDispatchListeners = null;

            // 清理消息通知转发监听管理容器
            _messageDispatchListeners.Clear();
            _messageDispatchListeners = null;

            // 移除所有消息分发回调信息
            RemoveAllMessageDistributeCalls();

            // 销毁回调句柄映射字典
            _messageDistributeCallInfos.Clear();
            _messageDistributeCallInfos = null;

            // 移除消息通道数据
            RemoveAllMessageChannelTypes();

            // 清理网络消息解析对象管理容器
            _messageTranslators.Clear();
            _messageTranslators = null;
            // 清理网络消息通道对象管理容器
            _messageChannelTypes.Clear();
            _messageChannelTypes = null;
            // 销毁协议对象映射字典
            _messageClassTypes.Clear();
            _messageClassTypes = null;
        }

        /// <summary>
        /// 句柄对象内置刷新接口
        /// </summary>
        protected override void OnUpdate()
        {
            if (_messageInvokeQueue.Count > 0)
            {
                Queue<SystemAction> queue = new Queue<SystemAction>(_messageInvokeQueue);
                _messageInvokeQueue.Clear();

                while (queue.Count > 0)
                {
                    SystemAction f = queue.Dequeue();
                    try { f(); } catch(System.Exception e) { Debugger.Error(e); }
                }
            }
        }

        /// <summary>
        /// 句柄对象内置延迟刷新接口
        /// </summary>
        protected override void OnLateUpdate()
        {
        }

        /// <summary>
        /// 句柄对象的模块事件转发回调接口
        /// </summary>
        /// <param name="e">模块事件参数</param>
        public override void OnEventDispatch(NovaEngine.ModuleEventArgs e)
        {
            NovaEngine.NetworkEventArgs networkEventArgs = e as NovaEngine.NetworkEventArgs;
            if (null == networkEventArgs)
            {
                Debugger.Error("The ModuleEventArgs unabled convert to NetworkEventArgs, dispatched it {0} failed.", e.ID);
                return;
            }

            // Debugger.Info("On network event dispatched for protocol type '{0}' with target channel '{1}'.", networkEventArgs.Type, networkEventArgs.ChannelID);

            switch (networkEventArgs.Type)
            {
                case (int) NovaEngine.NetworkModule.ProtocolType.Connected:
                    {
                        OnNetworkChannelConnection(networkEventArgs.ChannelID);
                    }
                    break;
                case (int) NovaEngine.NetworkModule.ProtocolType.Disconnected:
                    {
                        OnNetworkChannelDisconnection(networkEventArgs.ChannelID);
                    }
                    break;
                case (int) NovaEngine.NetworkModule.ProtocolType.Exception:
                case (int) NovaEngine.NetworkModule.ProtocolType.ReadError:
                case (int) NovaEngine.NetworkModule.ProtocolType.WriteError:
                    {
                        OnNetworkChannelConnectError(networkEventArgs.ChannelID);
                    }
                    break;
                case (int) NovaEngine.NetworkModule.ProtocolType.Dispatched:
                    {
                        NovaEngine.IO.ByteStreamBuffer streamBuffer = networkEventArgs.Data as NovaEngine.IO.ByteStreamBuffer;
                        byte[] buffer = streamBuffer.ReadBytes();
                        OnNetworkChannelReceiveMessage(networkEventArgs.ChannelID, buffer);
                    }
                    break;
            }
        }

        /// <summary>
        /// 网络连接请求函数
        /// 具体连接的网络类型参数设置方式可以参考<see cref="NovaEngine.NetworkServiceType"/>
        /// </summary>
        /// <param name="protocol">网络协议类型</param>
        /// <param name="name">网络名称</param>
        /// <param name="url">网络地址</param>
        /// <returns>若网络连接请求发送成功返回对应的通道实例，否则返回null</returns>
        public MessageChannel Connect(int protocol, string name, string url)
        {
            IEnumerator<MessageChannel> channels = _messageChannels.Values.GetEnumerator();
            while (channels.MoveNext())
            {
                // 如果有重名的网络通道，则直接返回已有的实例
                if (channels.Current.Name.Equals(name))
                {
                    Debugger.Warn("The network connect name '{0}' was already existed, don't repeat used it.", name);
                    return channels.Current;
                }
            }

            int channelID = NetworkModule.Connect(protocol, name, url);
            if (channelID <= 0)
            {
                Debugger.Warn("Connect target address '{0}' with protocol type '{1}' failed.", url, protocol);
                return null;
            }

            MessageChannel channel = MessageChannel.Create(channelID, protocol, name, url);
            if (null == channel)
            {
                Debugger.Error("Create message channel with protocol type '{0}' failed.", protocol);
                return null;
            }

            Debugger.Info(LogGroupTag.Module, "The network connect new channel [ID = {0}, Type = {1}, Name = {2}] on target url '{3}'.", channelID, protocol, name, url);
            _messageChannels.Add(channelID, channel);

            return channel;
        }

        /// <summary>
        /// 关闭网络连接函数
        /// 通过指定的通道标识，关闭该通道对应的网络连接
        /// </summary>
        /// <param name="channelID">网络通道标识</param>
        public void Disconnect(int channelID)
        {
            if (_messageChannels.ContainsKey(channelID))
            {
                MessageChannel channel = _messageChannels[channelID];

                if (channel.IsConnected)
                {
                    NetworkModule.Disconnect(channelID);
                }

                Debugger.Info(LogGroupTag.Module, "The network disconnect target channel [ID = {0}, Type = {1}, Name = {2}] now.", channel.ChannelID, channel.ChannelType, channel.Name);
                _messageChannels.Remove(channelID);

                // 销毁消息通道
                channel.Destroy();
            }
        }

        /// <summary>
        /// 关闭网络连接函数
        /// 通过指定的消息通道实例，关闭该通道对应的网络连接
        /// </summary>
        /// <param name="channel">网络通道对象实例</param>
        public void Disconnect(MessageChannel channel)
        {
            Disconnect(channel.ChannelID);
        }

        /// <summary>
        /// 关闭当前所有已打开的网络连接
        /// </summary>
        public void DisconnectAllChannels()
        {
            IList<int> keys = NovaEngine.Utility.Collection.ToListForKeys<int, MessageChannel>(_messageChannels);
            for (int n = 0; null != keys && n < keys.Count; ++n)
            {
                Disconnect(keys[n]);
            }
        }

        /// <summary>
        /// 在当前网络管理句柄中查找指定通道标识对应的消息通道对象实例
        /// </summary>
        /// <param name="channelID">通道标识</param>
        /// <returns>返回给定标识对应的网络通道对象实例</returns>
        public MessageChannel GetChannel(int channelID)
        {
            if (_messageChannels.ContainsKey(channelID))
            {
                return _messageChannels[channelID];
            }

            return null;
        }

        /// <summary>
        /// 向指定的网络通道对应的连接发送消息
        /// </summary>
        /// <param name="channelID">网络通道标识</param>
        /// <param name="buffer">消息内容</param>
        public void SendMessage(int channelID, string buffer)
        {
            NetworkModule.WriteMessage(channelID, buffer);
        }

        /// <summary>
        /// 向指定的网络通道对应的连接发送消息
        /// </summary>
        /// <param name="channelID">网络通道标识</param>
        /// <param name="message">消息字节流</param>
        public void SendMessage(int channelID, byte[] message)
        {
            NetworkModule.WriteMessage(channelID, message);
        }

        /// <summary>
        /// 向指定的网络通道对应的连接发送消息
        /// </summary>
        /// <param name="channelID">网络通道标识</param>
        /// <param name="message">消息对象</param>
        public void SendMessage(int channelID, ProtoBuf.Extension.IMessage message)
        {
            byte[] buffer = ProtoBuf.Extension.ProtobufHelper.ToBytes(message);

            SendMessage(channelID, buffer);
        }

        /// <summary>
        /// 通过指定的消息类型获取其对应的操作码
        /// </summary>
        /// <param name="clsType">消息类型</param>
        /// <returns>返回消息类型对应的操作码，若类型非法则返回0</returns>
        public int GetOpcodeByMessageType(SystemType clsType)
        {
            foreach (KeyValuePair<int, SystemType> pair in _messageClassTypes)
            {
                if (pair.Value == clsType)
                {
                    return pair.Key;
                }
            }

            return 0;
        }

        #region 网络管理句柄事件监听接口回调函数

        /// <summary>
        /// 网络通道连接成功的通知回调接口
        /// </summary>
        /// <param name="channelID">通道标识</param>
        private void OnNetworkChannelConnection(int channelID)
        {
            _messageInvokeQueue.Enqueue(() =>
            {
                MessageChannel channel = GetChannel(channelID);
                if (null == channel)
                {
                    Debugger.Error("Could not found any message channel instance with channel ID '{0}', the connect was invalid type.", channelID);
                    Disconnect(channelID);
                    return;
                }

                channel.OnConnected();

                IEnumerator<INetworkDispatchListener> e = _networkEventDispatchListeners.GetEnumerator();
                while (e.MoveNext())
                {
                    e.Current.OnConnection(channel);
                }
            });
        }

        /// <summary>
        /// 网络通道连接断开的通知回调接口
        /// </summary>
        /// <param name="channelID">通道标识</param>
        private void OnNetworkChannelDisconnection(int channelID)
        {
            // m_messageInvokeQueue?.Enqueue(() =>
            // {
            MessageChannel channel = GetChannel(channelID);
            if (null != channel)
            {
                IEnumerator<INetworkDispatchListener> e = _networkEventDispatchListeners.GetEnumerator();
                while (e.MoveNext())
                {
                    e.Current.OnDisconnection(channel);
                }

                Disconnect(channel);
            }
            // });
        }

        /// <summary>
        /// 网络通道连接错误的通知回调接口
        /// </summary>
        /// <param name="channelID">通道标识</param>
        private void OnNetworkChannelConnectError(int channelID)
        {
            // m_messageInvokeQueue.Enqueue(() =>
            // {
            MessageChannel channel = GetChannel(channelID);
            if (null != channel)
            {
                IEnumerator<INetworkDispatchListener> e = _networkEventDispatchListeners.GetEnumerator();
                while (e.MoveNext())
                {
                    e.Current.OnConnectError(channel);
                }

                Disconnect(channel);
            }
            // });
        }

        /// <summary>
        /// 网络通道接收消息的通知回调接口
        /// </summary>
        /// <param name="channelID">通道标识</param>
        /// <param name="buffer">消息数据流</param>
        private void OnNetworkChannelReceiveMessage(int channelID, byte[] buffer)
        {
            _messageInvokeQueue.Enqueue(() =>
            {
                MessageChannel channel = GetChannel(channelID);
                if (null == channel)
                {
                    Debugger.Error("Could not found any message channel instance with channel ID '{0}', the recv message and processing was failed.", channelID);
                    Disconnect(channelID);
                    return;
                }

                if (false == channel.IsConnected)
                {
                    Debugger.Warn("The message channel instance '{0}' was not connected, the recv message and processing was failed.", channelID);
                    return;
                }

                object message = channel.MessageTranslator.Decode(buffer);
                if (null == message)
                {
                    Debugger.Warn("Decode received message failed with target channel '{0}', please checked the channel translator is running normally.", NovaEngine.Utility.Text.ToString(channel.GetType()));
                }
                else if (typeof(ProtoBuf.Extension.IMessage).IsAssignableFrom(message.GetType()))
                {
                    // Debugger.Info("message:" + message.GetType() + ", Content:" + message);

                    Debugger.Assert(typeof(SocketMessageChannel).IsAssignableFrom(channel.GetType()), "无效的网络通道类型：{%t}", channel);

                    OnReceiveMessageComposedOfProtoBuf(channel as SocketMessageChannel, message as ProtoBuf.Extension.IMessage);
                }
                else
                {
                    Debugger.Error("无法找到任何匹配目标消息类型‘{%t}’的处理通道，对目标消息的接收和转发操作处理失败！", message);
                }
            });
        }

        /// <summary>
        /// 通过模拟的方式接收基于ProtoBuf协议构建的网络消息的接口函数<br/>
        /// 警告：该函数需要谨慎使用，因为它破坏了正常的消息转发流程<br/>
        /// 正因如此，所以在正式的发布环境中，该接口将被禁用<br/>
        /// 因此，我们建议外部业务代码仅在测试情况下使用该接口，且完成测试后尽快移除相关逻辑
        /// </summary>
        /// <param name="message">消息对象实例</param>
        public void OnSimulationReceiveMessageComposedOfProtoBuf(ProtoBuf.Extension.IMessage message)
        {
            if (false == NovaEngine.Environment.IsDevelopmentState())
            {
                Debugger.Error("不能在正式环境下进行模拟接收消息处理‘{%d} - {%t}’，请检查您是否正确调用了预期的函数接口！", GetOpcodeByMessageType(message.GetType()), message);
                return;
            }

            OnReceiveMessageComposedOfProtoBuf(null, message);
        }

        /// <summary>
        /// 接收基于ProtoBuf协议构建的网络消息的接口函数
        /// </summary>
        /// <param name="channel">通道对象实例</param>
        /// <param name="message">消息对象实例</param>
        private void OnReceiveMessageComposedOfProtoBuf(SocketMessageChannel channel, ProtoBuf.Extension.IMessage message)
        {
            int opcode = GetOpcodeByMessageType(message.GetType());

            bool v = _messageDistributeCallInfos.ContainsKey(opcode);
            if (v)
            {
                // 消息分发调度
                OnMessageDistributeCallDispatched(opcode, message);
            }

            IList<IMessageDispatch> listeners = null;
            if (_messageDispatchListeners.TryGetValue(opcode, out listeners))
            {
                v = true;

                // 2024-06-22:
                // 存在处理消息逻辑过程中删除实体对象的情况，
                // 此时队列将发生改变，但应用层逻辑依赖注册顺序，不能使用逆向遍历的方式处理
                // 所以此处创建一个临时列表来存放数据进行遍历
                // 为了优化性能，只有一条数据的列表直接赋值，不进行容器创建行为
                IList<IMessageDispatch> list;
                if (listeners.Count > 1)
                {
                    list = new List<IMessageDispatch>();
                    ((List<IMessageDispatch>) list).AddRange(listeners);
                }
                else
                {
                    list = listeners;
                }

                for (int n = 0; n < list.Count; ++n)
                {
                    IMessageDispatch listener = list[n];
                    listener.OnMessageDispatch(opcode, message);
                }

                list = null;
            }

            if (null != channel && channel.IsConnected && channel.IsWaitingForTargetCode(opcode))
            {
                v = true;
                channel.OnMessageDispatched(opcode, message);
            }

            if (!v)
            {
                Debugger.Warn("无法找到任何匹配目标操作码‘{%d}’的网络消息接收处理器，请针对该操作码注册对应的处理回调句柄！", opcode);
            }
        }

        #endregion

        #region 网络消息通道类及解析服务加载/清理接口函数

        /// <summary>
        /// 消息通道类的后缀名称常量定义
        /// </summary>
        private const string MESSAGE_CHANNEL_CLASS_SUFFIX_NAME = "MessageChannel";
        /// <summary>
        /// 消息解析器类的后缀名称常量定义
        /// </summary>
        private const string MESSAGE_TRANSLATOR_CLASS_SUFFIX_NAME = "MessageTranslator";

        /// <summary>
        /// 网络消息通道对象类型的加载接口函数
        /// 此函数将同时对通道的解析器对象进行实例化
        /// </summary>
        private void LoadAllMessageChannelTypes()
        {
            string namespaceTag = GetType().Namespace;

            foreach (NovaEngine.NetworkServiceType enumValue in SystemEnum.GetValues(typeof(NovaEngine.NetworkServiceType)))
            {
                if (NovaEngine.NetworkServiceType.Unknown == enumValue)
                {
                    continue;
                }

                string enumName = enumValue.ToString();
                // 类名反射时需要包含命名空间前缀
                string channelClassName = NovaEngine.Utility.Text.Format("{0}.{1}{2}", namespaceTag, enumName, MESSAGE_CHANNEL_CLASS_SUFFIX_NAME);
                string translatorClassName = NovaEngine.Utility.Text.Format("{0}.{1}{2}", namespaceTag, enumName, MESSAGE_TRANSLATOR_CLASS_SUFFIX_NAME);

                SystemType channelClassType = SystemType.GetType(channelClassName);
                SystemType translatorClassType = SystemType.GetType(translatorClassName);

                if (null == channelClassType)
                {
                    Debugger.Info(LogGroupTag.Module, "Could not found any message channel class with target name {0}.", channelClassName);
                    continue;
                }

                if (null == translatorClassType)
                {
                    Debugger.Info(LogGroupTag.Module, "Could not found any message translator class with target name {0}.", translatorClassName);
                    continue;
                }

                if (false == typeof(IMessageTranslator).IsAssignableFrom(translatorClassType))
                {
                    Debugger.Warn("The message translator class type {0} must be inherited from 'IMessageTranslator' interface.", translatorClassName);
                    continue;
                }

                // 创建实例
                IMessageTranslator messageTranslator = System.Activator.CreateInstance(translatorClassType) as IMessageTranslator;
                if (null == messageTranslator)
                {
                    Debugger.Error("The message translator class type {0} created failed.", translatorClassName);
                    continue;
                }

                Debugger.Info(LogGroupTag.Module, "Register new message channel type '{0}' and translator class '{1}' to target service type '{2}'.",
                        channelClassName, translatorClassName, enumName);

                _messageChannelTypes.Add((int) enumValue, channelClassType);
                _messageTranslators.Add((int) enumValue, messageTranslator);
            }
        }

        /// <summary>
        /// 通过指定的服务类型获取对应的消息通道对象类型
        /// </summary>
        /// <param name="serviceType">服务类型</param>
        /// <returns>返回对应类型的消息通道对象，若不存在对应类型的通道对象则返回null</returns>
        public SystemType GetMessageChannelTypeByServiceType(int serviceType)
        {
            if (false == _messageChannelTypes.ContainsKey(serviceType))
            {
                Debugger.Warn("Could not found any message channel class by service type '{0}', please checked it was loaded failed.", serviceType);
            }

            return _messageChannelTypes[serviceType];
        }

        /// <summary>
        /// 通过指定的服务类型获取对应的消息解析接口实例
        /// </summary>
        /// <param name="serviceType">服务类型</param>
        /// <returns>返回对应类型的消息解析实例，若不存在对应类型的解析接口则返回null</returns>
        public IMessageTranslator GetMessageTranslatorByServiceType(int serviceType)
        {
            if (false == _messageTranslators.ContainsKey(serviceType))
            {
                Debugger.Warn("Could not found any message translator class by service type '{0}', please checked it was loaded failed.", serviceType);
            }

            return _messageTranslators[serviceType];
        }

        /// <summary>
        /// 网络消息通道对象类型的移除接口函数
        /// 此函数将同时移除掉所有为通道而实例化的消息解析器对象
        /// </summary>
        private void RemoveAllMessageChannelTypes()
        {
            _messageChannelTypes.Clear();
            _messageTranslators.Clear();
        }

        #endregion

        #region 网络管理句柄对象注册绑定接口函数

        /// <summary>
        /// 向网络管理句柄中注册指定消息类型对应的对象类
        /// </summary>
        /// <param name="msgType">消息类型</param>
        /// <param name="classType">对象类</param>
        private void RegMessageClassTypes(int msgType, SystemType classType)
        {
            if (_messageClassTypes.ContainsKey(msgType))
            {
                Debugger.Warn("The message proto object code '{0}' was already exist, repeat add will be override old handler.", msgType);
                _messageClassTypes.Remove(msgType);
            }

            // Debugger.Info("Register new message class type '{0}' with target opcode '{1}'.", classType.FullName, msgType);
            _messageClassTypes.Add(msgType, classType);
        }

        /// <summary>
        /// 从网络管理句柄中注销指定消息类型对应的对象类
        /// </summary>
        /// <param name="msgType">消息类型</param>
        private void UnregMessageClassTypes(int msgType)
        {
            if (false == _messageClassTypes.ContainsKey(msgType))
            {
                Debugger.Warn("Could not found any message class type with target opcode '{0}', unregisted it failed.", msgType);
                return;
            }

            // Debugger.Info("Unregister message class type with target opcode '{0}'.", msgType);
            _messageClassTypes.Remove(msgType);
        }

        /// <summary>
        /// 从网络管理句柄中注销所有的消息对象类
        /// </summary>
        private void UnregAllMessageClassTypes()
        {
            _messageClassTypes.Clear();
        }

        /// <summary>
        /// 通过指定的消息类型从网络管理句柄中获取对应的消息对象类
        /// </summary>
        /// <param name="msgType">消息类型</param>
        /// <returns>返回给定类型对应的消息对象类，若不存在则返回null</returns>
        public SystemType GetMessageClassByType(int msgType)
        {
            if (_messageClassTypes.ContainsKey(msgType))
            {
                return _messageClassTypes[msgType];
            }

            Debugger.Info("Could not found any message class with target type '{0}'!", msgType);
            return null;
        }

        /// <summary>
        /// 针对网络数据进行消息分发的调度入口函数
        /// </summary>
        /// <param name="opcode">协议编码</param>
        /// <param name="message">消息内容</param>
        private void OnMessageDistributeCallDispatched(int opcode, object message)
        {
            IList<MessageCallInfo> list = null;
            if (_messageDistributeCallInfos.TryGetValue(opcode, out list))
            {
                IEnumerator<MessageCallInfo> e_info = list.GetEnumerator();
                while (e_info.MoveNext())
                {
                    MessageCallInfo info = e_info.Current;
                    if (null == info.TargetType)
                    {
                        info.Invoke(message);
                    }
                    else
                    {
                        IList<IProto> protos = ProtoController.Instance.FindAllProtos(info.TargetType);
                        if (null != protos)
                        {
                            IEnumerator<IProto> e_proto = protos.GetEnumerator();
                            while (e_proto.MoveNext())
                            {
                                IProto proto = e_proto.Current;
                                info.Invoke(proto, message);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 新增指定的分发函数到当前消息监听管理容器中
        /// </summary>
        /// <param name="fullname">完整名称</param>
        /// <param name="targetType">目标对象类型</param>
        /// <param name="opcode">协议编码</param>
        /// <param name="callback">函数回调句柄</param>
        private void AddMessageDistributeCallInfo(string fullname, SystemType targetType, int opcode, SystemDelegate callback)
        {
            MessageCallInfo info = new MessageCallInfo(fullname, targetType, opcode, callback);

            Debugger.Info(LogGroupTag.Module, "Add new message distribute call '{0}' to target opcode '{1}' of the class type '{2}'.",
                    NovaEngine.Utility.Text.ToString(callback), opcode, NovaEngine.Utility.Text.ToString(targetType));
            if (_messageDistributeCallInfos.ContainsKey(opcode))
            {
                IList<MessageCallInfo> list = _messageDistributeCallInfos[opcode];
                list.Add(info);
            }
            else
            {
                IList<MessageCallInfo> list = new List<MessageCallInfo>();
                list.Add(info);
                _messageDistributeCallInfos.Add(opcode, list);
            }
        }

        /// <summary>
        /// 新增指定的分发函数到当前消息监听管理容器中
        /// </summary>
        /// <param name="fullname">完整名称</param>
        /// <param name="targetType">目标对象类型</param>
        /// <param name="messageType">消息对象类型</param>
        /// <param name="callback">函数回调句柄</param>
        private void AddMessageDistributeCallInfo(string fullname, SystemType targetType, SystemType messageType, SystemDelegate callback)
        {
            int opcode = GetOpcodeByMessageType(messageType);

            AddMessageDistributeCallInfo(fullname, targetType, opcode, callback);
        }

        /// <summary>
        /// 从当前消息监听管理容器中移除指定标识的分发函数
        /// </summary>
        /// <param name="fullname">完整名称</param>
        /// <param name="targetType">目标对象类型</param>
        /// <param name="opcode">协议编码</param>
        private void RemoveMessageDistributeCallInfo(string fullname, SystemType targetType, int opcode)
        {
            Debugger.Info(LogGroupTag.Module, "Remove message distribute call '{0}' with target opcode '{1}' and class type '{2}'.",
                    fullname, opcode, NovaEngine.Utility.Text.ToString(targetType));
            if (false == _messageDistributeCallInfos.ContainsKey(opcode))
            {
                Debugger.Warn("Could not found any message distribute call '{0}' with target opcode '{1}', removed it failed.", fullname, opcode);
                return;
            }

            IList<MessageCallInfo> list = _messageDistributeCallInfos[opcode];
            for (int n = 0; n < list.Count; ++n)
            {
                MessageCallInfo info = list[n];
                if (info.Fullname.Equals(fullname))
                {
                    Debugger.Assert(info.TargetType == targetType && info.Opcode == opcode, "Invalid arguments.");

                    list.RemoveAt(n);
                    if (list.Count <= 0)
                    {
                        _messageDistributeCallInfos.Remove(opcode);
                    }

                    return;
                }
            }

            Debugger.Warn("Could not found any message distribute call '{0}' with target opcode '{1}' and class type '{2}', removed it failed.",
                    fullname, opcode, NovaEngine.Utility.Text.ToString(targetType));
        }

        /// <summary>
        /// 从当前消息监听管理容器中移除指定类型的分发函数
        /// </summary>
        /// <param name="fullname">完整名称</param>
        /// <param name="targetType">目标对象类型</param>
        /// <param name="messageType">消息对象类型</param>
        private void RemoveMessageDistributeCallInfo(string fullname, SystemType targetType, SystemType messageType)
        {
            int opcode = GetOpcodeByMessageType(messageType);

            RemoveMessageDistributeCallInfo(fullname, targetType, opcode);
        }

        /// <summary>
        /// 移除当前消息监听管理器中登记的所有分发函数回调句柄
        /// </summary>
        private void RemoveAllMessageDistributeCalls()
        {
            _messageDistributeCallInfos.Clear();
        }

        #endregion

        #region 网络消息监听的数据信息结构对象声明

        /// <summary>
        /// 消息监听的数据信息类
        /// </summary>
        private class MessageCallInfo
        {
            /// <summary>
            /// 消息监听类的完整名称
            /// </summary>
            private string m_fullname;
            /// <summary>
            /// 消息监听类的目标对象类型
            /// </summary>
            private SystemType m_targetType;
            /// <summary>
            /// 消息监听类的协议编码
            /// </summary>
            protected int m_opcode;
            /// <summary>
            /// 消息监听类的回调句柄
            /// </summary>
            private SystemDelegate m_callback;
            /// <summary>
            /// 消息监听回调函数的无参状态标识
            /// </summary>
            private bool m_isNullParameterType;

            public string Fullname => m_fullname;
            public SystemType TargetType => m_targetType;
            public int Opcode => m_opcode;
            public SystemDelegate Callback => m_callback;
            public bool IsNullParameterType => m_isNullParameterType;

            public MessageCallInfo(string fullname, SystemType targetType, int opcode, SystemDelegate callback)
            {
                Debugger.Assert(null != callback, "Invalid arguments.");

                m_fullname = fullname;
                m_targetType = targetType;
                m_opcode = opcode;
                m_callback = callback;
                m_isNullParameterType = Loader.Inspecting.CodeInspector.IsNullParameterTypeOfMessageCallFunction(callback.Method);
            }

            /// <summary>
            /// 基于网络数据类型的消息回调转发函数
            /// </summary>
            /// <param name="message">消息内容</param>
            public void Invoke(object message)
            {
                if (m_isNullParameterType)
                {
                    m_callback.DynamicInvoke();
                }
                else
                {
                    m_callback.DynamicInvoke(message);
                }
            }

            /// <summary>
            /// 基于网络数据类型的消息回调转发函数
            /// </summary>
            /// <param name="proto">目标原型对象</param>
            /// <param name="message">消息内容</param>
            public void Invoke(IProto proto, object message)
            {
                if (m_isNullParameterType)
                {
                    m_callback.DynamicInvoke(proto);
                }
                else
                {
                    m_callback.DynamicInvoke(proto, message);
                }
            }
        }

        #endregion

        #region 网络消息转发监听接口注册绑定接口函数

        /// <summary>
        /// 新增一个指定协议码对应的消息转发通知的监听回调接口
        /// </summary>
        /// <param name="opcode">协议操作码</param>
        /// <param name="listener">消息监听回调接口</param>
        /// <returns>若添加监听接口成功则返回true，否则返回false</returns>
        public bool AddMessageDispatchListener(int opcode, IMessageDispatch listener)
        {
            IList<IMessageDispatch> list;
            if (false == _messageDispatchListeners.TryGetValue(opcode, out list))
            {
                list = new List<IMessageDispatch>();
                list.Add(listener);

                _messageDispatchListeners.Add(opcode, list);
                return true;
            }

            // 检查是否重复注册
            if (list.Contains(listener))
            {
                Debugger.Warn("The listener for target message '{0}' was already exist, cannot repeat add it.", opcode);
                return false;
            }

            list.Add(listener);

            return true;
        }

        /// <summary>
        /// 新增一个指定消息类型对应的消息转发通知的监听回调接口
        /// </summary>
        /// <param name="messageType">消息类型</param>
        /// <param name="listener">消息监听回调接口</param>
        /// <returns>若添加监听接口成功则返回true，否则返回false</returns>
        public bool AddMessageDispatchListener(SystemType messageType, IMessageDispatch listener)
        {
            int opcode = GetOpcodeByMessageType(messageType);

            return AddMessageDispatchListener(opcode, listener);
        }

        /// <summary>
        /// 移除指定协议码对应的消息转发监听回调接口
        /// </summary>
        /// <param name="opcode">协议操作码</param>
        /// <param name="listener">消息监听回调接口</param>
        public void RemoveMessageDispatchListener(int opcode, IMessageDispatch listener)
        {
            IList<IMessageDispatch> list;
            if (false == _messageDispatchListeners.TryGetValue(opcode, out list))
            {
                Debugger.Warn("Could not found any listener for target message '{0}' in dispatch container, removed it failed.", opcode);
                return;
            }

            list.Remove(listener);
            // 列表为空则移除对应的消息监听列表实例
            if (list.Count == 0)
            {
                _messageDispatchListeners.Remove(opcode);
            }
        }

        /// <summary>
        /// 移除指定消息类型对应的消息转发监听回调接口
        /// </summary>
        /// <param name="messageType">消息类型</param>
        /// <param name="listener">消息监听回调接口</param>
        public void RemoveMessageDispatchListener(SystemType messageType, IMessageDispatch listener)
        {
            int opcode = GetOpcodeByMessageType(messageType);

            RemoveMessageDispatchListener(opcode, listener);
        }

        /// <summary>
        /// 移除指定监听接口的所有注册消息通知
        /// </summary>
        public void RemoveAllMessageDispatchListeners(IMessageDispatch listener)
        {
            IList<int> ids = NovaEngine.Utility.Collection.ToListForKeys<int, IList<IMessageDispatch>>(_messageDispatchListeners);
            for (int n = 0; null != ids && n < ids.Count; ++n)
            {
                RemoveMessageDispatchListener(ids[n], listener);
            }
        }

        #endregion

        #region 网络事件转发监听对象注册绑定接口函数

        /// <summary>
        /// 添加指定的事件转发监听对象实例到当前网络管理句柄中
        /// </summary>
        /// <param name="listener">事件转发监听对象实例</param>
        /// <returns>若给定的实例添加成功则返回true，否则返回false</returns>
        public bool AddNetworkEventDispatchListener(INetworkDispatchListener listener)
        {
            if (_networkEventDispatchListeners.Contains(listener))
            {
                Debugger.Warn("The target network event dispatch listener instance was already existed, added it failed.");
                return false;
            }

            _networkEventDispatchListeners.Add(listener);
            return true;
        }

        /// <summary>
        /// 从当前网络管理句柄中移除指定的事件转发监听对象实例
        /// </summary>
        /// <param name="listener">事件转发监听对象实例</param>
        public void RemoveNetworkEventDispatchListener(INetworkDispatchListener listener)
        {
            if (false == _networkEventDispatchListeners.Contains(listener))
            {
                Debugger.Warn("Could not found any network event dispatch listener instance from current manage container, removed it failed.");
                return;
            }

            _networkEventDispatchListeners.Remove(listener);
        }

        /// <summary>
        /// 清除当前网络管理句柄中的所有事件转发监听对象实例
        /// </summary>
        private void RemoveAllNetworkEventDispatchListeners()
        {
            _networkEventDispatchListeners.Clear();
        }

        #endregion
    }
}
