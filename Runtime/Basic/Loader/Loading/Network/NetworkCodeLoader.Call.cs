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

namespace GameEngine.Loader
{
    /// <summary>
    /// 程序集中网络消息对象的分析处理类，对业务层载入的所有网络消息类进行统一加载及分析处理
    /// </summary>
    internal static partial class NetworkCodeLoader
    {
        /// <summary>
        /// 网络消息接收类的结构信息管理容器
        /// </summary>
        private static IDictionary<SystemType, Structuring.MessageCallCodeInfo> _messageCallCodeInfos = new Dictionary<SystemType, Structuring.MessageCallCodeInfo>();

        [OnCodeLoaderClassLoadOfTarget(typeof(MessageSystemAttribute))]
        private static bool LoadMessageCallClass(Symboling.SymClass symClass, bool reload)
        {
            Structuring.MessageCallCodeInfo info = new Structuring.MessageCallCodeInfo();
            info.ClassType = symClass.ClassType;

            IList<Symboling.SymMethod> symMethods = symClass.GetAllMethods();
            for (int n = 0; null != symMethods && n < symMethods.Count; ++n)
            {
                Symboling.SymMethod symMethod = symMethods[n];

                // 检查函数格式是否合法
                if (false == symMethod.IsStatic || false == Inspecting.CodeInspector.CheckFunctionFormatOfMessageCall(symMethod.MethodInfo))
                {
                    Debugger.Info(LogGroupTag.CodeLoader, "The message call method '{0}.{1}' was invalid format, added it failed.", symClass.FullName, symMethod.MethodName);
                    continue;
                }

                IList<SystemAttribute> attrs = symMethod.Attributes;
                for (int m = 0; null != attrs && m < attrs.Count; ++m)
                {
                    SystemAttribute attr = attrs[m];

                    if (attr is OnMessageDispatchCallAttribute)
                    {
                        OnMessageDispatchCallAttribute _attr = (OnMessageDispatchCallAttribute) attr;

                        Structuring.MessageCallMethodTypeCodeInfo callMethodInfo = new Structuring.MessageCallMethodTypeCodeInfo();
                        callMethodInfo.TargetType = _attr.ClassType;
                        callMethodInfo.Opcode = _attr.Opcode;
                        callMethodInfo.MessageType = _attr.MessageType;

                        if (callMethodInfo.Opcode <= 0 && null == callMethodInfo.MessageType)
                        {
                            // 未进行合法标识的函数忽略它
                            continue;
                        }

                        // 先记录函数信息并检查函数格式
                        // 在绑定环节在进行委托的格式转换
                        callMethodInfo.Fullname = symMethod.FullName;
                        callMethodInfo.Method = symMethod.MethodInfo;

                        // 函数参数类型的格式检查，仅在调试模式下执行，正式环境可跳过该处理
                        if (NovaEngine.Debugger.Instance.IsOnDebuggingVerificationActivated())
                        {
                            bool verificated = false;
                            if (null == callMethodInfo.TargetType)
                            {
                                if (Inspecting.CodeInspector.CheckFunctionFormatOfMessageCallWithNullParameterType(symMethod.MethodInfo))
                                {
                                    // 无参类型的事件函数
                                    verificated = NovaEngine.Debugger.Verification.CheckGenericDelegateParameterTypeMatched(symMethod.MethodInfo);
                                }
                                else if (callMethodInfo.Opcode > 0)
                                {
                                    // 协议编码派发
                                    verificated = NovaEngine.Debugger.Verification.CheckGenericDelegateParameterTypeMatched(symMethod.MethodInfo, NetworkHandler.Instance.GetMessageProtocolType());
                                }
                                else
                                {
                                    // 协议类型派发
                                    verificated = NovaEngine.Debugger.Verification.CheckGenericDelegateParameterTypeMatched(symMethod.MethodInfo, callMethodInfo.MessageType);
                                }
                            }
                            else
                            {
                                if (Inspecting.CodeInspector.CheckFunctionFormatOfMessageCallWithNullParameterType(symMethod.MethodInfo))
                                {
                                    // 无参类型的事件函数
                                    verificated = NovaEngine.Debugger.Verification.CheckGenericDelegateParameterTypeMatched(symMethod.MethodInfo, callMethodInfo.TargetType);
                                }
                                else if (callMethodInfo.Opcode > 0)
                                {
                                    // 协议编码派发
                                    verificated = NovaEngine.Debugger.Verification.CheckGenericDelegateParameterTypeMatched(symMethod.MethodInfo, callMethodInfo.TargetType, NetworkHandler.Instance.GetMessageProtocolType());
                                }
                                else
                                {
                                    // 协议类型派发
                                    verificated = NovaEngine.Debugger.Verification.CheckGenericDelegateParameterTypeMatched(symMethod.MethodInfo, callMethodInfo.TargetType, callMethodInfo.MessageType);
                                }
                            }

                            // 校验失败
                            if (false == verificated)
                            {
                                Debugger.Error("Cannot verificated from method info '{0}' to message listener call, loaded this method failed.", symMethod.FullName);
                                continue;
                            }
                        }

                        info.AddMethodType(callMethodInfo);
                    }
                }
            }

            if (info.GetMethodTypeCount() <= 0)
            {
                Debugger.Warn("The message call method types count must be great than zero, newly added class '{0}' failed.", info.ClassType.FullName);
                return false;
            }

            if (_messageCallCodeInfos.ContainsKey(symClass.ClassType))
            {
                if (reload)
                {
                    // 重载模式下，先移除旧的记录
                    _messageCallCodeInfos.Remove(symClass.ClassType);
                }
                else
                {
                    Debugger.Warn("The network message call '{0}' was already existed, repeat added it failed.", symClass.FullName);
                    return false;
                }
            }

            _messageCallCodeInfos.Add(symClass.ClassType, info);
            Debugger.Log(LogGroupTag.CodeLoader, "Load message call code info '{0}' succeed from target class type '{1}'.", CodeLoaderObject.ToString(info), symClass.FullName);

            return true;
        }

        [OnCodeLoaderClassCleanupOfTarget(typeof(MessageSystemAttribute))]
        private static void CleanupAllMessageCallClasses()
        {
            _messageCallCodeInfos.Clear();
        }

        [OnCodeLoaderClassLookupOfTarget(typeof(MessageSystemAttribute))]
        private static Structuring.MessageCallCodeInfo LookupMessageCallCodeInfo(Symboling.SymClass symClass)
        {
            foreach (KeyValuePair<SystemType, Structuring.MessageCallCodeInfo> pair in _messageCallCodeInfos)
            {
                if (pair.Key == symClass.ClassType)
                {
                    return pair.Value;
                }
            }

            return null;
        }
    }
}
