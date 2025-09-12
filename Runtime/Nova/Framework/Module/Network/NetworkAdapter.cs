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

using System.Collections.Generic;

using SystemException = System.Exception;
using SystemEnum = System.Enum;
using SystemType = System.Type;
using SystemASCIIEncoding = System.Text.ASCIIEncoding;
using SystemMemoryStream = System.IO.MemoryStream;
using SystemBinaryReader = System.IO.BinaryReader;
using SystemSeekOrigin = System.IO.SeekOrigin;

namespace NovaEngine
{
    /// <summary>
    /// 通用模式下的网络适配组件操作接口类
    /// </summary>
    public static class NetworkAdapter
    {
        /// <summary>
        /// 网络服务的后缀名称
        /// </summary>
        private const string NetworkServiceClassUnifiedStandardName = "Service";

        /// <summary>
        /// 网络服务对象注册管理容器
        /// </summary>
        private static readonly IDictionary<int, NetworkService> _services = new Dictionary<int, NetworkService>();

        /// <summary>
        /// 网络通道实例管理容器
        /// </summary>
        private static readonly IDictionary<int, NetworkChannel> _channels = new Dictionary<int, NetworkChannel>();

        /// <summary>
        /// ASCII字符集编码
        /// </summary>
        private static readonly SystemASCIIEncoding ASCII = new SystemASCIIEncoding();

        /// <summary>
        /// 网络适配器启动操作
        /// </summary>
        public static void Startup()
        {
            string namespaceTag = typeof(NetworkAdapter).Namespace;
            foreach (string enumName in SystemEnum.GetNames(typeof(NetworkServiceType)))
            {
                if (enumName.Equals(NetworkServiceType.Unknown.ToString()))
                {
                    continue;
                }

                // 类名反射时需要包含命名空间前缀
                string serviceName = Utility.Text.Format("{%s}.{%s}{%s}", namespaceTag, enumName, NetworkServiceClassUnifiedStandardName);

                SystemType serviceType = SystemType.GetType(serviceName);
                if (null == serviceType)
                {
                    Debugger.Info("Could not found any network service class with target name {%s}.", serviceName);
                    continue;
                }

                if (false == typeof(NetworkService).IsAssignableFrom(serviceType))
                {
                    Debugger.Warn("The service type {%s} must be inherited from 'NetworkService' type.", serviceName);
                    continue;
                }

                AddService(serviceType);
            }
        }

        /// <summary>
        /// 网络适配器关闭操作
        /// </summary>
        public static void Shutdown()
        {
            RemoveAllChannels();

            RemoveAllServices();
        }

        /// <summary>
        /// 网络适配器刷新操作
        /// </summary>
        public static void Update()
        {
            foreach (KeyValuePair<int, NetworkService> pair in _services)
            {
                NetworkService service = pair.Value;
                service.Update();
            }
        }

        /// <summary>
        /// 添加指定类型的网络服务接口实例
        /// </summary>
        /// <typeparam name="T">服务类型</typeparam>
        private static void AddService<T>() where T : NetworkService, new()
        {
            NetworkService service = new T();
            int type = service.ServiceType;

            if (_services.ContainsKey(type))
            {
                Logger.Warn("The target service instance '{%d}' was already exists, repeat added it will be override old value.", type);

                RemoveService(type);
            }

            _services.Add(type, service);
        }

        /// <summary>
        /// 添加指定类型的网络服务接口实例
        /// </summary>
        /// <param name="serviceType">对象类型</param>
        private static void AddService(SystemType serviceType)
        {
            if (false == typeof(NetworkService).IsAssignableFrom(serviceType))
            {
                Debugger.Warn("The service type '{%t}' must be inherited from 'NetworkService' type.", serviceType);
                return;
            }

            // 创建实例
            NetworkService service = System.Activator.CreateInstance(serviceType) as NetworkService;
            int type = service.ServiceType;

            if (_services.ContainsKey(type))
            {
                Logger.Warn("The target service instance '{%d}' was already exists, repeat added it will be override old value.", type);

                RemoveService(type);
            }

            _services.Add(type, service);
        }

        /// <summary>
        /// 移除指定类型的网络服务接口实例
        /// </summary>
        /// <param name="type">网络类型</param>
        private static void RemoveService(int type)
        {
            if (_services.ContainsKey(type))
            {
                _services.Remove(type);
            }
        }

        /// <summary>
        /// 移除全部网络服务接口实例
        /// </summary>
        private static void RemoveAllServices()
        {
            int[] keys = Utility.Collection.ToArray(_services.Keys);
            foreach (int key in keys)
            {
                RemoveService(key);
            }

            _services.Clear();
        }

        /// <summary>
        /// 通过指定服务类型获取对应的网络服务注册实例
        /// </summary>
        /// <param name="type">网络服务类型</param>
        /// <returns>返回网络服务注册实例</returns>
        public static NetworkService GetService(int type)
        {
            if (_services.ContainsKey(type))
            {
                return _services[type];
            }

            return null;
        }

        /// <summary>
        /// 添加网络通道实例到管理容器中
        /// </summary>
        /// <param name="channel">网络通道对象实例</param>
        private static void AddChannel(NetworkChannel channel)
        {
            if (_channels.ContainsKey(channel.ChannelID))
            {
                throw new CFrameworkException("The target channel '{%d}' was already exists.", channel.ChannelID);
            }

            _channels.Add(channel.ChannelID, channel);
        }

        /// <summary>
        /// 从管理容器中移除指定标识的网络通道实例
        /// </summary>
        /// <param name="channelID">网络通道对象标识</param>
        private static void RemoveChannel(int channelID)
        {
            if (_channels.TryGetValue(channelID, out NetworkChannel channel))
            {
                NetworkService service = GetService(channel.ServiceType);
                service.ReleaseChannel(channelID);

                // 网络通道关闭操作
                channel.Close();

                _channels.Remove(channelID);
            }
            else
            {
                Logger.Warn("Could not found channel instance by ID '{%d}'.", channelID);
            }
        }

        /// <summary>
        /// 从管理容器中移除全部网络通道实例
        /// </summary>
        private static void RemoveAllChannels()
        {
            int[] keys = Utility.Collection.ToArray(_channels.Keys);
            foreach (int key in keys)
            {
                RemoveChannel(key);
            }

            _channels.Clear();
        }

        /// <summary>
        /// 通过指定的网络通道标识在当前管理容器中查找对应的实例
        /// </summary>
        /// <param name="channelID">网络通道对象标识</param>
        /// <returns>返回网络通道对象实例，若查找失败则返回null</returns>
        public static NetworkChannel GetChannel(int channelID)
        {
            if (_channels.TryGetValue(channelID, out NetworkChannel channel))
            {
                return channel;
            }

            return null;
        }

        /// <summary>
        /// 创建指定服务类型的网络通道，并对其发起连接
        /// </summary>
        /// <param name="ctype">网络服务类型</param>
        /// <param name="name">网络名称</param>
        /// <param name="url">网络地址</param>
        /// <returns>返回网络通道对象实例的唯一标识</returns>
        public static int Connect(int ctype, string name, string url)
        {
            NetworkService service = GetService(ctype);
            if (null == service)
            {
                return 0;
            }

            NetworkChannel channel = service.CreateChannel(name, url);
            // 注册回调
            RegisterChannelCallback(channel);

            AddChannel(channel);

            // 执行连接操作
            channel.Connect();

            return channel.ChannelID;
        }

        /// <summary>
        /// 注册网络通道的回调接口
        /// </summary>
        /// <param name="channel">网络通道实例</param>
        private static void RegisterChannelCallback(NetworkChannel channel)
        {
            channel.ConnectionCallback += (c) =>
            {
                ModuleController.GetModule<NetworkModule>().OnConnection(channel.ChannelID);
            };

            channel.DisconnectionCallback += (c) =>
            {
                ModuleController.GetModule<NetworkModule>().OnDisconnection(channel.ChannelID);
            };

            channel.ErrorCallback += (c, e) =>
            {
                ModuleController.GetModule<NetworkModule>().OnConnectError(channel.ChannelID);
            };

            channel.ReadCallback += (stream, e) =>
            {
                OnReadCallback(channel.ChannelID, stream, e);
            };

            channel.WriteFailedCallback += (stream, e) =>
            {
                OnWriteFailedCallback(channel.ChannelID, stream, e);
            };
        }

        /// <summary>
        /// 检索指定标识的网络通道执行断开连接操作
        /// </summary>
        /// <param name="channelID">网络通道对象唯一标识</param>
        public static void Disconnect(int channelID)
        {
            NetworkChannel channel = GetChannel(channelID);
            if (null == channel /* || channel.IsClosed */ )
            {
                return;
            }

            RemoveChannel(channelID);
        }

        /// <summary>
        /// 在目标网络通道上进行消息发送操作
        /// </summary>
        /// <param name="channelID">网络通道对象唯一标识</param>
        /// <param name="message">消息内容</param>
        public static void Send(int channelID, string message)
        {
            NetworkChannel channel = GetChannel(channelID);
            if (null == channel || channel.IsClosed)
            {
                throw new CFrameworkException("channel '{%d}' was not accessible.", channelID);
            }

            channel.Send(message);
        }

        /// <summary>
        /// 在目标网络通道上进行消息发送操作
        /// </summary>
        /// <param name="channelID">网络通道对象唯一标识</param>
        /// <param name="message">消息内容</param>
        public static void Send(int channelID, byte[] message)
        {
            NetworkChannel channel = GetChannel(channelID);
            if (null == channel || channel.IsClosed)
            {
                return;
            }

            channel.Send(message);
        }

        /// <summary>
        /// 在目标网络通道上进行消息发送操作
        /// </summary>
        /// <param name="channelID">网络通道对象唯一标识</param>
        /// <param name="stream">数据流</param>
        public static void Send(int channelID, SystemMemoryStream stream)
        {
            NetworkChannel channel = GetChannel(channelID);
            if (null == channel || channel.IsClosed)
            {
                throw new CFrameworkException("channel '{%d}' was not accessible.", channelID);
            }

            channel.Send(stream);
        }

        private static void OnReadCallback(int channelID, SystemMemoryStream memoryStream, int streamType)
        {
            memoryStream.Seek(0, SystemSeekOrigin.Begin);

            // ushort opcode = 0;
            SystemBinaryReader reader = null;
            try
            {
                reader = new SystemBinaryReader(memoryStream);
                byte[] message = reader.ReadBytes((int) (memoryStream.Length - memoryStream.Position));

                switch (streamType)
                {
                    case MessageStreamCode.Byte:
                        ModuleController.GetModule<NetworkModule>().OnReceivedMessage(channelID, message);
                        break;
                    case MessageStreamCode.String:
                        ModuleController.GetModule<NetworkModule>().OnReceivedMessage(channelID, ASCII.GetString(message));
                        break;
                    default:
                        Logger.Error("unknown stream type '{%d}'.", streamType);
                        break;
                }
            }
            catch (SystemException e)
            {
                Logger.Error(e);
                NetworkChannel channel = GetChannel(channelID);
                channel.ErrorCode = NetworkErrorCode.PacketReadError;
                ModuleController.GetModule<NetworkModule>().OnConnectError(channelID);
            }
        }

        private static void OnWriteFailedCallback(int channelID, SystemMemoryStream memoryStream, int streamType)
        {
            SystemBinaryReader reader = null;
            try
            {
                memoryStream.Seek(0, SystemSeekOrigin.Begin);

                reader = new SystemBinaryReader(memoryStream);
                byte[] message = reader.ReadBytes((int) (memoryStream.Length - memoryStream.Position));

                switch (streamType)
                {
                    case MessageStreamCode.Byte:
                        ModuleController.GetModule<NetworkModule>().OnSendFailedMessage(channelID, message);
                        break;
                    case MessageStreamCode.String:
                        ModuleController.GetModule<NetworkModule>().OnSendFailedMessage(channelID, ASCII.GetString(message));
                        break;
                    default:
                        Logger.Error("unknown stream type '{%d}'.", streamType);
                        break;
                }
            }
            catch (SystemException e)
            {
                Logger.Error(e);
            }
            finally
            {
                if (null != reader)
                {
                    reader.Close();
                }
            }
        }
    }
}
