/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
/// Copyring (C) 2020 - 2022, Guangzhou Xinyuan Technology Co., Ltd.
/// Copyring (C) 2024, Guangzhou Shiyue Network Technology Co., Ltd.
/// Copyring (C) 2025, Hurley, Independent Studio.
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
using System.Reflection;

using SystemType = System.Type;
using SystemEnum = System.Enum;
using SystemAttribute = System.Attribute;
using SystemAttributeTargets = System.AttributeTargets;
using SystemAttributeUsageAttribute = System.AttributeUsageAttribute;
using SystemDelegate = System.Delegate;
using SystemBindingFlags = System.Reflection.BindingFlags;
using SystemMethodInfo = System.Reflection.MethodInfo;

namespace NovaEngine
{
    /// <summary>
    /// 日志相关函数集合工具类
    /// </summary>
    public static partial class Logger
    {
        /// <summary>
        /// 日志输出通道绑定信息管理容器
        /// </summary>
        private static IList<LogOutputChannelBindingInfo> s_logOutputChannelBindingInfos = null;

        /// <summary>
        /// 打开当前日志管理模块的输出通道
        /// </summary>
        /// <param name="channels">通道组标识</param>
        private static void OpenOutputChannels(int channels)
        {
            s_logOutputChannelBindingInfos = new List<LogOutputChannelBindingInfo>();

            SystemType classType = typeof(Logger);
            SystemType[] innerClassTypes = classType.GetNestedTypes(SystemBindingFlags.Public | SystemBindingFlags.NonPublic | SystemBindingFlags.Instance);
            for (int n = 0; null != innerClassTypes && n < innerClassTypes.Length; ++n)
            {
                SystemType innerClassType = innerClassTypes[n];
                LogOutputChannelBindingAttribute channelBindingAttribute = innerClassType.GetCustomAttribute<LogOutputChannelBindingAttribute>();
                if (null == channelBindingAttribute)
                {
                    continue;
                }

                SystemMethodInfo startupMethodInfo = innerClassType.GetMethod("Startup");
                SystemMethodInfo shutdownMethodInfo = innerClassType.GetMethod("Shutdown");
                Assert(null != startupMethodInfo && null != shutdownMethodInfo);

                Definition.Delegate.EmptyFunctionHandler startupAction = (Definition.Delegate.EmptyFunctionHandler) startupMethodInfo.CreateDelegate(typeof(Definition.Delegate.EmptyFunctionHandler));
                Definition.Delegate.EmptyFunctionHandler shutdownAction = (Definition.Delegate.EmptyFunctionHandler) shutdownMethodInfo.CreateDelegate(typeof(Definition.Delegate.EmptyFunctionHandler));

                LogOutputChannelBindingInfo info = new LogOutputChannelBindingInfo(channelBindingAttribute.ChannelType, startupAction, shutdownAction);
                s_logOutputChannelBindingInfos.Add(info);
            }

            foreach (LogOutputChannelBindingInfo info in s_logOutputChannelBindingInfos)
            {
                if ((channels & (int) info.ChannelType) == (int) info.ChannelType)
                {
                    info.OnChannelStartup();
                }
            }
        }

        /// <summary>
        /// 关闭当前日志管理模块的输出通道
        /// </summary>
        private static void CloseOutputChannels()
        {
            foreach (LogOutputChannelBindingInfo info in s_logOutputChannelBindingInfos)
            {
                info.OnChannelShutdown();
            }

            s_logOutputChannelBindingInfos.Clear();
            s_logOutputChannelBindingInfos = null;
        }

        #region 日志输出通道绑定信息数据结构及接口定义

        /// <summary>
        /// 日志输出通道的绑定属性类型定义
        /// </summary>
        [SystemAttributeUsage(SystemAttributeTargets.Class, AllowMultiple = false, Inherited = false)]
        private class LogOutputChannelBindingAttribute : SystemAttribute
        {
            /// <summary>
            /// 日志通道类型标识
            /// </summary>
            private LogOutputChannelType m_channelType;

            /// <summary>
            /// 日志通道类型标识获取函数
            /// </summary>
            public LogOutputChannelType ChannelType => m_channelType;

            public LogOutputChannelBindingAttribute(LogOutputChannelType channelType)
            {
                m_channelType = channelType;
            }
        }

        /// <summary>
        /// 日志输出通道绑定信息记录的对象类，用于记录当前日志系统所支持的通道对象绑定信息
        /// </summary>
        private class LogOutputChannelBindingInfo
        {
            /// <summary>
            /// 日志通道类型标识
            /// </summary>
            private LogOutputChannelType m_channelType;
            /// <summary>
            /// 日志通道对象的启用回调接口
            /// </summary>
            private Definition.Delegate.EmptyFunctionHandler m_startupCallback;
            /// <summary>
            /// 日志通道对象的关闭回调接口
            /// </summary>
            private Definition.Delegate.EmptyFunctionHandler m_shutdownCallback;
            /// <summary>
            /// 日志通道对象当前的启用状态标识
            /// </summary>
            private bool m_enabled;

            /// <summary>
            /// 日志通道类型标识获取函数
            /// </summary>
            public LogOutputChannelType ChannelType => m_channelType;
            /// <summary>
            /// 日志通道对象当前的启用状态标识获取函数
            /// </summary>
            public bool Enabled => m_enabled;

            public LogOutputChannelBindingInfo(LogOutputChannelType channelType,
                                               Definition.Delegate.EmptyFunctionHandler startupCallback,
                                               Definition.Delegate.EmptyFunctionHandler shutdownCallback)
            {
                m_channelType = channelType;
                m_startupCallback = startupCallback;
                m_shutdownCallback = shutdownCallback;
                m_enabled = false;
            }

            /// <summary>
            /// 启动当前对象绑定的通道
            /// </summary>
            public void OnChannelStartup()
            {
                if (m_enabled)
                {
                    // 通道已被启动，无需重复调用
                    return;
                }

                m_startupCallback?.Invoke();
                m_enabled = true;
            }

            /// <summary>
            /// 关闭当前对象绑定的通道
            /// </summary>
            public void OnChannelShutdown()
            {
                if (m_enabled)
                {
                    m_shutdownCallback?.Invoke();
                    m_enabled = false;
                }
            }
        }

        #endregion
    }
}
