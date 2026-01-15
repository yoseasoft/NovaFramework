/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyright (C) 2020 - 2022, Guangzhou Xinyuan Technology Co., Ltd.
/// Copyright (C) 2022 - 2023, Shanghai Bilibili Technology Co., Ltd.
/// Copyright (C) 2023 - 2024, Guangzhou Shiyue Network Technology Co., Ltd.
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
using System.Collections.Generic;
using System.Customize.Extension;
using System.Runtime.CompilerServices;

namespace GameEngine
{
    /// <summary>
    /// 网络模块封装的句柄对象类
    /// 模块具体功能接口请参考<see cref="NovaEngine.Module.NetworkModule"/>类
    /// </summary>
    public sealed partial class NetworkHandler : BaseHandler
    {
        /// <summary>
        /// 消息处理回调句柄
        /// </summary>
        /// <param name="message">消息内容</param>
        public delegate void MessageProcessHandler(object message);

        /// <summary>
        /// 网络消息协议对象类映射字典
        /// </summary>
        private IDictionary<int, Type> _messageClassTypes;

        /// <summary>
        /// 网络消息通道对象管理容器
        /// </summary>
        private IDictionary<int, Type> _messageChannelTypes;
        /// <summary>
        /// 网络消息解析服务对象管理容器
        /// </summary>
        private IDictionary<int, IMessageTranslator> _messageTranslators;

        /// <summary>
        /// 网络连接监听对象实例管理容器
        /// </summary>
        private IList<INetworkConnectionListener> _networkConnectionListeners;
        /// <summary>
        /// 消息传输监听对象实例管理容器
        /// </summary>
        private IList<INetworkTransmissionListener> _networkTransmissionListeners;

        /// <summary>
        /// 消息通道对象实例管理容器
        /// </summary>
        private IDictionary<int, MessageChannel> _messageChannels;

        /// <summary>
        /// 消息协议类型
        /// </summary>
        private Type _messageProtocolType;

        /// <summary>
        /// 消息的调用队列
        /// </summary>
        private Queue<Action> _messageInvokeQueue;

        /// <summary>
        /// 句柄对象的单例访问获取接口
        /// </summary>
        public static NetworkHandler Instance => HandlerManagement.NetworkHandler;

        /// <summary>
        /// 句柄对象默认构造函数
        /// </summary>
        internal NetworkHandler() : base()
        { }

        /// <summary>
        /// 句柄对象析构函数
        /// </summary>
        ~NetworkHandler()
        { }

        /// <summary>
        /// 句柄对象内置初始化接口函数
        /// </summary>
        /// <returns>若句柄对象初始化成功则返回true，否则返回false</returns>
        protected override bool OnInitialize()
        {
            // 初始化协议对象映射字典
            _messageClassTypes = new Dictionary<int, Type>();
            // 初始化网络消息通道对象管理容器
            _messageChannelTypes = new Dictionary<int, Type>();
            // 初始化网络消息解析对象管理容器
            _messageTranslators = new Dictionary<int, IMessageTranslator>();

            // 初始化网络连接监听对象管理容器
            _networkConnectionListeners = new List<INetworkConnectionListener>();
            // 初始化消息传输监听对象管理容器
            _networkTransmissionListeners = new List<INetworkTransmissionListener>();

            // 初始化消息通道对象管理容器
            _messageChannels = new Dictionary<int, MessageChannel>();

            // 初始化消息调用队列
            _messageInvokeQueue = new Queue<Action>();

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

            // 移除所有网络连接监听实例
            RemoveAllNetworkConnectionListeners();
            // 移除所有消息转发监听实例
            RemoveAllNetworkTransmissionListeners();

            // 清理网络连接监听对象管理容器
            _networkConnectionListeners = null;
            // 清理消息传输监听对象管理容器
            _networkTransmissionListeners = null;

            // 移除消息通道数据
            RemoveAllMessageChannelTypes();

            // 清理网络消息协议类型
            _messageProtocolType = null;

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
        /// 句柄对象内置重载接口函数
        /// </summary>
        protected override void OnReload()
        {
        }

        /// <summary>
        /// 句柄对象内置执行接口
        /// </summary>
        protected override void OnExecute()
        {
        }

        /// <summary>
        /// 句柄对象内置延迟执行接口
        /// </summary>
        protected override void OnLateExecute()
        {
        }

        /// <summary>
        /// 句柄对象内置刷新接口
        /// </summary>
        protected override void OnUpdate()
        {
            if (_messageInvokeQueue.Count > 0)
            {
                Queue<Action> queue = new Queue<Action>(_messageInvokeQueue);
                _messageInvokeQueue.Clear();

                while (queue.Count > 0)
                {
                    Action f = queue.Dequeue();
                    try { f(); } catch(Exception e) { Debugger.Error(e); }
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
        public override void OnEventDispatch(NovaEngine.Module.ModuleEventArgs e)
        {
            NovaEngine.Module.NetworkEventArgs networkEventArgs = e as NovaEngine.Module.NetworkEventArgs;
            if (null == networkEventArgs)
            {
                Debugger.Error("The ModuleEventArgs unabled convert to NetworkEventArgs, dispatched it {%d} failed.", e.ID);
                return;
            }

            // Debugger.Info("On network event dispatched for protocol type '{%d}' with target channel '{%d}'.", networkEventArgs.Type, networkEventArgs.ChannelID);

            switch (networkEventArgs.Type)
            {
                case (int) NovaEngine.Module.NetworkModule.ProtocolType.Connected:
                    {
                        OnNetworkChannelConnection(networkEventArgs.ChannelID);
                    }
                    break;
                case (int) NovaEngine.Module.NetworkModule.ProtocolType.Disconnected:
                    {
                        OnNetworkChannelDisconnection(networkEventArgs.ChannelID);
                    }
                    break;
                case (int) NovaEngine.Module.NetworkModule.ProtocolType.Exception:
                case (int) NovaEngine.Module.NetworkModule.ProtocolType.ReadError:
                case (int) NovaEngine.Module.NetworkModule.ProtocolType.WriteError:
                    {
                        OnNetworkChannelConnectError(networkEventArgs.ChannelID);
                    }
                    break;
                case (int) NovaEngine.Module.NetworkModule.ProtocolType.Dispatched:
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
        /// </summary>
        /// <param name="serviceType">网络协议类型</param>
        /// <param name="name">网络名称</param>
        /// <param name="url">网络地址</param>
        /// <returns>若网络连接请求发送成功返回对应的通道实例，否则返回null</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public MessageChannel Connect(NovaEngine.Module.NetworkServiceType serviceType, string name, string url)
        {
            return Connect((int) serviceType, name, url);
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
                    Debugger.Warn("The network connect name '{%s}' was already existed, don't repeat used it.", name);
                    return channels.Current;
                }
            }

            int channelID = NetworkModule.Connect(protocol, name, url);
            if (channelID <= 0)
            {
                Debugger.Warn("Connect target address '{%s}' with protocol type '{%d}' failed.", url, protocol);
                return null;
            }

            MessageChannel channel = MessageChannel.Create(channelID, protocol, name, url);
            if (null == channel)
            {
                Debugger.Error("Create message channel with protocol type '{%d}' failed.", protocol);
                return null;
            }

            Debugger.Info(LogGroupTag.Module, "The network connect new channel [ID={%d},Type={%d},Name={%s}] on target url '{%s}'.", channelID, protocol, name, url);
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

                Debugger.Info(LogGroupTag.Module, "The network disconnect target channel [ID={%d},Type={%d},Name={%s}] now.", channel.ChannelID, channel.ChannelType, channel.Name);
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
        public void SendMessage(int channelID, object message)
        {
            // byte[] buffer = ProtoBuf.Extension.ProtobufHelper.ToBytes(message);
            // SendMessage(channelID, buffer);

            Debugger.Throw<NotImplementedException>();
        }

        /// <summary>
        /// 通过指定的消息类型获取其对应的操作码
        /// </summary>
        /// <param name="clsType">消息类型</param>
        /// <returns>返回消息类型对应的操作码，若类型非法则返回0</returns>
        public int GetOpcodeByMessageType(Type clsType)
        {
            foreach (KeyValuePair<int, Type> pair in _messageClassTypes)
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
                    Debugger.Error("Could not found any message channel instance with channel ID '{%d}', the connect was invalid type.", channelID);
                    Disconnect(channelID);
                    return;
                }

                channel.OnConnected();

                IEnumerator<INetworkConnectionListener> e = _networkConnectionListeners.GetEnumerator();
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
            // _messageInvokeQueue?.Enqueue(() =>
            // {
            MessageChannel channel = GetChannel(channelID);
            if (null != channel)
            {
                IEnumerator<INetworkConnectionListener> e = _networkConnectionListeners.GetEnumerator();
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
            // _messageInvokeQueue.Enqueue(() =>
            // {
            MessageChannel channel = GetChannel(channelID);
            if (null != channel)
            {
                IEnumerator<INetworkConnectionListener> e = _networkConnectionListeners.GetEnumerator();
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
            // 2025-12-04：
            // 新增消息传输监听接口，允许业务自行实现派发策略
            if (_networkTransmissionListeners.Count > 0)
            {
                bool processed = false;
                for (int n = 0; n < _networkTransmissionListeners.Count; ++n)
                {
                    processed = processed || _networkTransmissionListeners[n].OnRecvMessage(channelID, buffer);
                }

                // 若监听器处理了该消息，则不再进行后续处理
                if (processed)
                    return;
            }

            _messageInvokeQueue.Enqueue(() =>
            {
                MessageChannel channel = GetChannel(channelID);
                if (null == channel)
                {
                    Debugger.Error("Could not found any message channel instance with channel ID '{%d}', the recv message and processing was failed.", channelID);
                    Disconnect(channelID);
                    return;
                }

                if (false == channel.IsConnected)
                {
                    Debugger.Warn("The message channel instance '{%d}' was not connected, the recv message and processing was failed.", channelID);
                    return;
                }

                object message = channel.MessageTranslator.Decode(buffer);
                if (null == message)
                {
                    Debugger.Warn("Decode received message failed with target channel '{%t}', please checked the channel translator is running normally.", channel);
                }
                else
                {
                    // Debugger.Info("message:" + message.GetType() + ", Content:" + message);

                    Debugger.Assert(typeof(SocketMessageChannel).IsAssignableFrom(channel.GetType()), "无效的网络通道类型：{%t}", channel);

                    OnReceiveMessage(channel as SocketMessageChannel, message);
                }
            });
        }

        /// <summary>
        /// 通过模拟的方式接收特定格式协议构建的网络消息的接口函数<br/>
        /// 警告：该函数需要谨慎使用，因为它破坏了正常的消息转发流程<br/>
        /// 正因如此，所以在正式的发布环境中，该接口将被禁用<br/>
        /// 因此，我们建议外部业务代码仅在测试情况下使用该接口，且完成测试后尽快移除相关逻辑
        /// </summary>
        /// <param name="message">消息对象实例</param>
        public void OnSimulationReceiveMessage(object message)
        {
            if (false == NovaEngine.Environment.IsDevelopmentState())
            {
                Debugger.Error("不能在正式环境下进行模拟接收消息处理‘{%d} - {%t}’，请检查您是否正确调用了预期的函数接口！", GetOpcodeByMessageType(message.GetType()), message);
                return;
            }

            OnReceiveMessage(null, message);
        }

        /// <summary>
        /// 接收基于特定格式协议构建的网络消息的接口函数
        /// </summary>
        /// <param name="channel">通道对象实例</param>
        /// <param name="message">消息对象实例</param>
        private void OnReceiveMessage(SocketMessageChannel channel, object message)
        {
            int opcode = GetOpcodeByMessageType(message.GetType());

            bool v = _messageDistributeCallInfos.ContainsKey(opcode);
            if (v)
            {
                // 消息分发调度
                OnMessageDistributeCallDispatched(opcode, message);
            }

            if (_messageDispatchListeners.TryGetValue(opcode, out IList<IMessageDispatch> listeners))
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
        private const string MessageChannelClassUnifiedStandardName = "MessageChannel";
        /// <summary>
        /// 消息解析器类的后缀名称常量定义
        /// </summary>
        // private const string MessageTranslatorClassUnifiedStandardName = "MessageTranslator";

        /// <summary>
        /// 网络消息通道对象类型的加载接口函数
        /// 此函数将同时对通道的解析器对象进行实例化
        /// </summary>
        private void LoadAllMessageChannelTypes()
        {
            string namespaceTag = GetType().Namespace;

            foreach (NovaEngine.Module.NetworkServiceType enumValue in Enum.GetValues(typeof(NovaEngine.Module.NetworkServiceType)))
            {
                if (NovaEngine.Module.NetworkServiceType.Unknown == enumValue)
                {
                    continue;
                }

                string enumName = enumValue.ToString();
                // 类名反射时需要包含命名空间前缀
                string channelClassName = NovaEngine.FormatString.Format("{%s}.{%s}{%s}", namespaceTag, enumName, MessageChannelClassUnifiedStandardName);
                // string translatorClassName = NovaEngine.FormatString.Format("{%s}.{%s}{%s}", namespaceTag, enumName, MessageTranslatorClassUnifiedStandardName);

                Type channelClassType = Type.GetType(channelClassName);
                // Type translatorClassType = Type.GetType(translatorClassName);

                if (null == channelClassType)
                {
                    Debugger.Info(LogGroupTag.Module, "Could not found any message channel class with target name {%s}.", channelClassName);
                    continue;
                }

                //
                // 2025-11-05：
                // 此处将消息转译对象的实例化操作调整到外部模块中进行
                // 因为不同业务对消息编码/解码的方式亦有所不同，有个性化定制的需求
                // 所以该步骤由外部模块自行注册管理
                //

                // if (null == translatorClassType)
                // { Debugger.Info(LogGroupTag.Module, "Could not found any message translator class with target name {%s}.", translatorClassName); continue; }

                // if (false == typeof(IMessageTranslator).IsAssignableFrom(translatorClassType))
                // { Debugger.Warn("The message translator class type {%s} must be inherited from 'IMessageTranslator' interface.", translatorClassName); continue; }

                // 创建消息转译对象的实例
                // IMessageTranslator messageTranslator = Activator.CreateInstance(translatorClassType) as IMessageTranslator;
                // if (null == messageTranslator)
                // { Debugger.Error("The message translator class type {%s} created failed.", translatorClassName); continue; }

                Debugger.Info(LogGroupTag.Module, "Register new message channel type '{%s}' to target service type '{%s}'.", channelClassName, enumName);
                // Debugger.Info(LogGroupTag.Module, "Register new message translator class '{%s}' to target service type '{%s}'.", translatorClassName, enumName);

                _messageChannelTypes.Add((int) enumValue, channelClassType);
                // _messageTranslators.Add((int) enumValue, messageTranslator);
            }
        }

        /// <summary>
        /// 注册指定服务类型对应的消息解析器对象实例
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="serviceType">服务类型</param>
        /// <returns>若注册解析器对象实例成功则返回true，否则返回false</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool RegMessageTranslator<T>(int serviceType) where T : IMessageTranslator, new()
        {
            return RegMessageTranslator(serviceType, typeof(T));
        }

        /// <summary>
        /// 注册指定服务类型对应的消息解析器对象实例
        /// </summary>
        /// <param name="serviceType">服务类型</param>
        /// <param name="classType">对象类型</param>
        /// <returns>若注册解析器对象实例成功则返回true，否则返回false</returns>
        public bool RegMessageTranslator(int serviceType, Type classType)
        {
            if (false == _messageChannelTypes.ContainsKey(serviceType))
            {
                Debugger.Error(LogGroupTag.Module, "The network service type '{%d}' was invalid value, register matched message translator failed.", serviceType);
                return false;
            }

            if (_messageTranslators.ContainsKey(serviceType))
            {
                Debugger.Error(LogGroupTag.Module, "The message translator class was already exists by service type '{%d}', you must delete old value before adding new instance.", serviceType);
                return false;
            }

            if (false == classType.Is<IMessageTranslator>())
            {
                Debugger.Error(LogGroupTag.Module, "The message translator class type {%t} must be inherited from 'IMessageTranslator' interface.", classType);
                return false;
            }

            // 创建实例
            IMessageTranslator messageTranslator = Activator.CreateInstance(classType) as IMessageTranslator;
            if (null == messageTranslator)
            {
                Debugger.Error(LogGroupTag.Module, "The message translator class type {%t} created instance failed.", classType);
                return false;
            }
            _messageTranslators.Add(serviceType, messageTranslator);
            return true;
        }

        /// <summary>
        /// 删除指定服务类型对应的消息解析器对象实例
        /// </summary>
        /// <param name="serviceType">服务类型</param>
        public void UnregMessageTranslator(int serviceType)
        {
            if (_messageTranslators.ContainsKey(serviceType))
            {
                _messageTranslators.Remove(serviceType);
            }
        }

        /// <summary>
        /// 通过指定的服务类型获取对应的消息通道对象类型
        /// </summary>
        /// <param name="serviceType">服务类型</param>
        /// <returns>返回对应类型的消息通道对象，若不存在对应类型的通道对象则返回null</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Type GetMessageChannelTypeByServiceType(int serviceType)
        {
            if (false == _messageChannelTypes.ContainsKey(serviceType))
            {
                Debugger.Warn("Could not found any message channel class by service type '{%d}', please checked it was loaded failed.", serviceType);
            }

            return _messageChannelTypes[serviceType];
        }

        /// <summary>
        /// 通过指定的服务类型获取对应的消息解析接口实例
        /// </summary>
        /// <param name="serviceType">服务类型</param>
        /// <returns>返回对应类型的消息解析实例，若不存在对应类型的解析接口则返回null</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IMessageTranslator GetMessageTranslatorByServiceType(int serviceType)
        {
            if (false == _messageTranslators.ContainsKey(serviceType))
            {
                Debugger.Warn("Could not found any message translator class by service type '{%d}', please checked it was loaded failed.", serviceType);
            }

            return _messageTranslators[serviceType];
        }

        /// <summary>
        /// 设置当前句柄的消息协议类型
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetMessageProtocolType<T>() where T : class
        {
            SetMessageProtocolType(typeof(T));
        }

        /// <summary>
        /// 设置当前句柄的消息协议类型
        /// </summary>
        /// <param name="classType">对象类型</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetMessageProtocolType(Type classType)
        {
            if (null != _messageProtocolType)
            {
                Debugger.Warn(LogGroupTag.Module, "网络管理句柄中已绑定消息协议类型‘{%t}’，重复添加新的协议类型‘{%t}’将覆盖旧的类型值。", _messageProtocolType, classType);
            }

            _messageProtocolType = classType;
        }

        /// <summary>
        /// 获取当前句柄注册的消息协议类型
        /// </summary>
        /// <returns>返回当前网络支持的消息协议类型</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Type GetMessageProtocolType()
        {
            Debugger.Assert(_messageProtocolType, NovaEngine.ErrorText.NullObjectReference);

            return _messageProtocolType;
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

        #region 网络消息定义对象注册绑定接口函数

        /// <summary>
        /// 向网络管理句柄中注册指定消息类型对应的对象类
        /// </summary>
        /// <param name="msgType">消息类型</param>
        /// <param name="classType">对象类</param>
        private void RegMessageClassTypes(int msgType, Type classType)
        {
            Debugger.Assert(msgType > 0, NovaEngine.ErrorText.InvalidArguments);

            if (_messageClassTypes.ContainsKey(msgType))
            {
                Debugger.Warn(LogGroupTag.Module, "The message proto object code '{%d}' was already exist, repeat add will be override old handler.", msgType);
                _messageClassTypes.Remove(msgType);
            }

            // Debugger.Info(LogGroupTag.Module, "Register new message class type '{%t}' with target opcode '{%d}'.", classType, msgType);
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
                Debugger.Warn(LogGroupTag.Module, "Could not found any message class type with target opcode '{%d}', unregistered it failed.", msgType);
                return;
            }

            // Debugger.Info(LogGroupTag.Module, "Unregister message class type with target opcode '{%d}'.", msgType);
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Type GetMessageClassByType(int msgType)
        {
            if (_messageClassTypes.ContainsKey(msgType))
            {
                return _messageClassTypes[msgType];
            }

            Debugger.Warn(LogGroupTag.Module, "Could not found any message class with target type '{%d}'!", msgType);
            return null;
        }

        #endregion

        #region 网络事件转发监听对象注册绑定接口函数

        /// <summary>
        /// 添加指定的网络连接监听对象实例到当前网络管理句柄中
        /// </summary>
        /// <param name="listener">监听对象实例</param>
        /// <returns>若给定的实例添加成功则返回true，否则返回false</returns>
        public bool AddNetworkConnectionListener(INetworkConnectionListener listener)
        {
            if (_networkConnectionListeners.Contains(listener))
            {
                Debugger.Warn("The target network connection listener instance was already existed, added it failed.");
                return false;
            }

            _networkConnectionListeners.Add(listener);
            return true;
        }

        /// <summary>
        /// 从当前网络管理句柄中移除指定的网络连接监听对象实例
        /// </summary>
        /// <param name="listener">监听对象实例</param>
        public void RemoveNetworkConnectionListener(INetworkConnectionListener listener)
        {
            if (false == _networkConnectionListeners.Contains(listener))
            {
                Debugger.Warn("Could not found any network connection listener instance from current manage container, removed it failed.");
                return;
            }

            _networkConnectionListeners.Remove(listener);
        }

        /// <summary>
        /// 清除当前网络管理句柄中的所有网络连接监听对象实例
        /// </summary>
        private void RemoveAllNetworkConnectionListeners()
        {
            _networkConnectionListeners.Clear();
        }

        /// <summary>
        /// 添加指定的消息传输监听对象实例到当前网络管理句柄中
        /// </summary>
        /// <param name="listener">消息传输监听对象实例</param>
        /// <returns>若给定的实例添加成功则返回true，否则返回false</returns>
        public bool AddNetworkTransmissionListener(INetworkTransmissionListener listener)
        {
            if (_networkTransmissionListeners.Contains(listener))
            {
                Debugger.Warn("The target network transmission listener instance was already existed, added it failed.");
                return false;
            }

            _networkTransmissionListeners.Add(listener);
            return true;
        }

        /// <summary>
        /// 从当前网络管理句柄中移除指定的消息传输监听对象实例
        /// </summary>
        /// <param name="listener">消息传输监听对象实例</param>
        public void RemoveNetworkTransmissionListener(INetworkTransmissionListener listener)
        {
            if (false == _networkTransmissionListeners.Contains(listener))
            {
                Debugger.Warn("Could not found any network transmission listener instance from current manage container, removed it failed.");
                return;
            }

            _networkTransmissionListeners.Remove(listener);
        }

        /// <summary>
        /// 清除当前网络管理句柄中的所有消息传输监听对象实例
        /// </summary>
        private void RemoveAllNetworkTransmissionListeners()
        {
            _networkTransmissionListeners.Clear();
        }

        #endregion
    }
}
