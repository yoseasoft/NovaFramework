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

using System.Collections.Generic;

using SystemType = System.Type;
using SystemAttribute = System.Attribute;
using SystemDelegate = System.Delegate;

namespace GameEngine
{
    /// <summary>
    /// 网络模块封装的句柄对象类
    /// </summary>
    public sealed partial class NetworkHandler
    {
        /// <summary>
        /// 网络消息类型的代码注册回调函数
        /// </summary>
        /// <param name="targetType">对象类型</param>
        /// <param name="codeInfo">对象结构信息数据</param>
        /// <param name="reload">重载标识</param>
        [OnNetworkMessageRegisterClassOfTarget(typeof(ProtoBuf.Extension.MessageAttribute))]
        private static void LoadMessageBindCodeType(SystemType targetType, Loader.GeneralCodeInfo codeInfo, bool reload)
        {
            if (targetType.IsInterface || targetType.IsAbstract)
            {
                Debugger.Log("The load code type '{0}' cannot be interface or abstract class, recv arguments invalid.", targetType.FullName);
                return;
            }

            if (null == codeInfo)
            {
                Debugger.Warn("The load code info '{0}' must be non-null, recv arguments invalid.", targetType.FullName);
                return;
            }

            Loader.NetworkMessageCodeInfo networkCodeInfo = codeInfo as Loader.NetworkMessageCodeInfo;
            Debugger.Assert(null != networkCodeInfo, "Invalid network message code info.");

            if (reload)
            {
                Instance.UnregMessageClassTypes(networkCodeInfo.Opcode);
            }

            Instance.RegMessageClassTypes(networkCodeInfo.Opcode, targetType);
        }

        /// <summary>
        /// 网络消息类型的全部代码的注销回调函数
        /// </summary>
        [OnNetworkMessageUnregisterClassOfTarget(typeof(ProtoBuf.Extension.MessageAttribute))]
        private static void UnloadAllMessageBindCodeTypes()
        {
            Instance.UnregAllMessageClassTypes();
        }

        /// <summary>
        /// 网络消息类型的代码注册回调函数
        /// </summary>
        /// <param name="targetType">对象类型</param>
        /// <param name="codeInfo">对象结构信息数据</param>
        /// <param name="reload">重载标识</param>
        [OnNetworkMessageRegisterClassOfTarget(typeof(MessageSystemAttribute))]
        private static void LoadMessageCallbackBindCodeType(SystemType targetType, Loader.GeneralCodeInfo codeInfo, bool reload)
        {
            if (null == codeInfo)
            {
                Debugger.Warn("The load code info '{0}' must be non-null, recv arguments invalid.", targetType.FullName);
                return;
            }

            Loader.MessageCallCodeInfo messageCodeInfo = codeInfo as Loader.MessageCallCodeInfo;
            Debugger.Assert(null != messageCodeInfo, "Invalid message call code info.");

            for (int n = 0; n < messageCodeInfo.GetMethodTypeCount(); ++n)
            {
                Loader.MessageCallMethodTypeCodeInfo callMethodInfo = messageCodeInfo.GetMethodType(n);

                SystemDelegate callback = NovaEngine.Utility.Reflection.CreateGenericActionDelegate(callMethodInfo.Method);
                if (null == callback)
                {
                    Debugger.Warn("Cannot converted from method info '{0}' to network message call, loaded this method failed.", NovaEngine.Utility.Text.ToString(callMethodInfo.Method));
                    continue;
                }

                if (callMethodInfo.Opcode > 0)
                {
                    if (reload)
                    {
                        Instance.RemoveMessageDistributeCallInfo(callMethodInfo.Fullname, callMethodInfo.TargetType, callMethodInfo.Opcode);
                    }

                    Instance.AddMessageDistributeCallInfo(callMethodInfo.Fullname, callMethodInfo.TargetType, callMethodInfo.Opcode, callback);
                }
                else
                {
                    if (reload)
                    {
                        Instance.RemoveMessageDistributeCallInfo(callMethodInfo.Fullname, callMethodInfo.TargetType, callMethodInfo.MessageType);
                    }

                    Instance.AddMessageDistributeCallInfo(callMethodInfo.Fullname, callMethodInfo.TargetType, callMethodInfo.MessageType, callback);
                }
            }
        }

        /// <summary>
        /// 网络消息类型的全部代码的注销回调函数
        /// </summary>
        [OnNetworkMessageUnregisterClassOfTarget(typeof(MessageSystemAttribute))]
        private static void UnloadAllMessageCallbackBindCodeTypes()
        {
            Instance.RemoveAllMessageDistributeCalls();
        }
    }
}
