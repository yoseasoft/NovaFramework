/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyright (C) 2023 - 2024, Guangzhou Shiyue Network Technology Co., Ltd.
/// Copyright (C) 2024 - 2025, Hurley, Independent Studio.
/// Copyright (C) 2025 - 2026, Hainan Yuanyou Information Technology Co., Ltd. Guangzhou Branch
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
using UnityEngine.Scripting;

namespace GameEngine.Loader
{
    /// 程序集中扩展定义对象的分析处理类
    internal static partial class ExtendCodeLoader
    {
        /// <summary>
        /// 扩展定义调用类的结构信息管理容器
        /// </summary>
        private readonly static IDictionary<Type, Structuring.ExtendCallCodeInfo> _extendCallCodeInfos = new Dictionary<Type, Structuring.ExtendCallCodeInfo>();

        [Preserve]
        [OnCodeLoaderClassLoadOfTarget(typeof(ExtendSupportedAttribute))]
        private static bool LoadExtendCallClass(Symbolling.SymClass symClass, bool reload)
        {
            Structuring.ExtendCallCodeInfo info = new Structuring.ExtendCallCodeInfo();
            info.ClassType = symClass.ClassType;

            IList<Symbolling.SymMethod> symMethods = symClass.GetAllMethods();
            for (int n = 0; null != symMethods && n < symMethods.Count; ++n)
            {
                Symbolling.SymMethod symMethod = symMethods[n];

                // 检查函数格式是否合法，扩展类型的函数必须为静态类型
                if (false == symMethod.IsStatic || false == symMethod.IsExtension)
                {
                    Debugger.Info(LogGroupTag.CodeLoader, "符号类‘{%t}’的目标函数‘{%t}’不满足‘ExtendSupport’扩展回调函数格式类型的定义规则，不能进行正确的函数加载解析！", symClass.ClassType, symMethod.MethodInfo);
                    continue;
                }

                Type extendClassType = symMethod.ExtensionParameterType;

                IList<Attribute> attrs = symMethod.Attributes;
                for (int m = 0; null != attrs && m < attrs.Count; ++m)
                {
                    Attribute attr = attrs[m];

                    if (attr is OnInputDispatchCallAttribute inputDispatchCallAttribute)
                    {
                        if (inputDispatchCallAttribute.KeyCode <= VirtualKeyCode.None && null == inputDispatchCallAttribute.InputDataType)
                        {
                            Debugger.Warn(LogGroupTag.CodeLoader, "The extend input dispatch method '{%s}.{%s}' was invalid arguments, added it failed.", symClass.FullName, symMethod.MethodName);
                            continue;
                        }

                        if (false == Inspecting.CodeInspector.CheckFunctionFormatOfInputCallWithBeanExtensionType(symMethod.MethodInfo))
                        {
                            Debugger.Warn(LogGroupTag.CodeLoader, "The extend input dispatch method '{%s}.{%s}' was invalid format, added it failed.", symClass.FullName, symMethod.MethodName);
                            continue;
                        }

                        Structuring.InputCallMethodTypeCodeInfo methodTypeCodeInfo = new Structuring.InputCallMethodTypeCodeInfo();
                        methodTypeCodeInfo.TargetType = extendClassType;
                        methodTypeCodeInfo.KeyCode = inputDispatchCallAttribute.KeyCode;
                        methodTypeCodeInfo.OperationType = inputDispatchCallAttribute.OperationType;
                        methodTypeCodeInfo.InputDataType = inputDispatchCallAttribute.InputDataType;
                        methodTypeCodeInfo.BehaviourType = inputDispatchCallAttribute.BehaviourType;
                        methodTypeCodeInfo.Fullname = symMethod.FullName;
                        methodTypeCodeInfo.Method = symMethod.MethodInfo;
                        Debugger.IsNull(inputDispatchCallAttribute.ClassType);

                        // 函数参数类型的格式检查，仅在调试模式下执行，正式环境可跳过该处理
                        if (NovaEngine.Debugger.Instance.IsOnDebuggingVerificationActivated())
                        {
                            bool verificated;

                            if (Inspecting.CodeInspector.CheckFunctionFormatOfInputCallWithNullParameterType(symMethod.MethodInfo))
                            {
                                verificated = NovaEngine.Debugger.Verification.CheckGenericDelegateParameterTypeMatched(
                                                    symMethod.MethodInfo, methodTypeCodeInfo.TargetType);
                            }
                            else if (methodTypeCodeInfo.KeyCode > VirtualKeyCode.None)
                            {
                                verificated = NovaEngine.Debugger.Verification.CheckGenericDelegateParameterTypeMatched(
                                                    symMethod.MethodInfo, methodTypeCodeInfo.TargetType, typeof(VirtualKeyCode), typeof(InputOperationType));
                            }
                            else
                            {
                                verificated = NovaEngine.Debugger.Verification.CheckGenericDelegateParameterTypeMatched(
                                                    symMethod.MethodInfo, methodTypeCodeInfo.TargetType, methodTypeCodeInfo.InputDataType);
                            }

                            // 校验失败
                            if (false == verificated)
                            {
                                Debugger.Error(LogGroupTag.CodeLoader, "Cannot verificated from method info '{%s}' to extend input dispatch call, loaded this method failed.", symMethod.MethodName);
                                continue;
                            }
                        }

                        info.AddInputCallMethodType(methodTypeCodeInfo);
                    }
                    else if (attr is OnEventDispatchCallAttribute eventDispatchCallAttribute)
                    {
                        if (eventDispatchCallAttribute.EventID <= 0 && null == eventDispatchCallAttribute.EventDataType)
                        {
                            Debugger.Warn(LogGroupTag.CodeLoader, "The extend event dispatch method '{%s}.{%s}' was invalid arguments, added it failed.", symClass.FullName, symMethod.MethodName);
                            continue;
                        }

                        if (false == Inspecting.CodeInspector.CheckFunctionFormatOfEventCallWithBeanExtensionType(symMethod.MethodInfo))
                        {
                            Debugger.Warn(LogGroupTag.CodeLoader, "The extend event dispatch method '{%s}.{%s}' was invalid format, added it failed.", symClass.FullName, symMethod.MethodName);
                            continue;
                        }

                        Structuring.EventCallMethodTypeCodeInfo methodTypeCodeInfo = new Structuring.EventCallMethodTypeCodeInfo();
                        methodTypeCodeInfo.TargetType = extendClassType;
                        methodTypeCodeInfo.EventID = eventDispatchCallAttribute.EventID;
                        methodTypeCodeInfo.EventDataType = eventDispatchCallAttribute.EventDataType;
                        methodTypeCodeInfo.BehaviourType = eventDispatchCallAttribute.BehaviourType;
                        methodTypeCodeInfo.Fullname = symMethod.FullName;
                        methodTypeCodeInfo.Method = symMethod.MethodInfo;
                        Debugger.IsNull(eventDispatchCallAttribute.ClassType);

                        // 函数参数类型的格式检查，仅在调试模式下执行，正式环境可跳过该处理
                        if (NovaEngine.Debugger.Instance.IsOnDebuggingVerificationActivated())
                        {
                            bool verificated;

                            if (Inspecting.CodeInspector.CheckFunctionFormatOfEventCallWithNullParameterType(symMethod.MethodInfo))
                            {
                                verificated = NovaEngine.Debugger.Verification.CheckGenericDelegateParameterTypeMatched(
                                                    symMethod.MethodInfo, methodTypeCodeInfo.TargetType);
                            }
                            else if (methodTypeCodeInfo.EventID > 0)
                            {
                                verificated = NovaEngine.Debugger.Verification.CheckGenericDelegateParameterTypeMatched(
                                                    symMethod.MethodInfo, methodTypeCodeInfo.TargetType, typeof(int), typeof(object[]));
                            }
                            else
                            {
                                verificated = NovaEngine.Debugger.Verification.CheckGenericDelegateParameterTypeMatched(
                                                    symMethod.MethodInfo, methodTypeCodeInfo.TargetType, methodTypeCodeInfo.EventDataType);
                            }

                            // 校验失败
                            if (false == verificated)
                            {
                                Debugger.Error(LogGroupTag.CodeLoader, "Cannot verificated from method info '{%s}' to extend event dispatch call, loaded this method failed.", symMethod.MethodName);
                                continue;
                            }
                        }

                        info.AddEventCallMethodType(methodTypeCodeInfo);
                    }
                    else if (attr is OnMessageDispatchCallAttribute messageDispatchCallAttribute)
                    {
                        if (messageDispatchCallAttribute.Opcode <= 0 && null == messageDispatchCallAttribute.MessageType)
                        {
                            Debugger.Warn(LogGroupTag.CodeLoader, "The extend message dispatch method '{%s}.{%s}' was invalid arguments, added it failed.", symClass.FullName, symMethod.MethodName);
                            continue;
                        }

                        if (false == Inspecting.CodeInspector.CheckFunctionFormatOfMessageCallWithBeanExtensionType(symMethod.MethodInfo))
                        {
                            Debugger.Warn(LogGroupTag.CodeLoader, "The extend message dispatch method '{%s}.{%s}' was invalid format, added it failed.", symClass.FullName, symMethod.MethodName);
                            continue;
                        }

                        Structuring.MessageCallMethodTypeCodeInfo methodTypeCodeInfo = new Structuring.MessageCallMethodTypeCodeInfo();
                        methodTypeCodeInfo.TargetType = extendClassType;
                        methodTypeCodeInfo.Opcode = messageDispatchCallAttribute.Opcode;
                        methodTypeCodeInfo.MessageType = messageDispatchCallAttribute.MessageType;
                        methodTypeCodeInfo.BehaviourType = messageDispatchCallAttribute.BehaviourType;
                        methodTypeCodeInfo.Fullname = symMethod.FullName;
                        methodTypeCodeInfo.Method = symMethod.MethodInfo;
                        Debugger.IsNull(messageDispatchCallAttribute.ClassType);

                        // 函数参数类型的格式检查，仅在调试模式下执行，正式环境可跳过该处理
                        if (NovaEngine.Debugger.Instance.IsOnDebuggingVerificationActivated())
                        {
                            bool verificated;

                            if (Inspecting.CodeInspector.CheckFunctionFormatOfMessageCallWithNullParameterType(symMethod.MethodInfo))
                            {
                                verificated = NovaEngine.Debugger.Verification.CheckGenericDelegateParameterTypeMatched(
                                                    symMethod.MethodInfo, methodTypeCodeInfo.TargetType);
                            }
                            else if (methodTypeCodeInfo.Opcode > 0)
                            {
                                verificated = NovaEngine.Debugger.Verification.CheckGenericDelegateParameterTypeMatched(
                                                    symMethod.MethodInfo, methodTypeCodeInfo.TargetType, NetworkHandler.Instance.GetMessageProtocolType());
                            }
                            else
                            {
                                verificated = NovaEngine.Debugger.Verification.CheckGenericDelegateParameterTypeMatched(
                                                    symMethod.MethodInfo, methodTypeCodeInfo.TargetType, methodTypeCodeInfo.MessageType);
                            }

                            // 校验失败
                            if (false == verificated)
                            {
                                Debugger.Error(LogGroupTag.CodeLoader, "Cannot verificated from method info '{%s}' to extend message dispatch call, loaded this method failed.", symMethod.MethodName);
                                continue;
                            }
                        }

                        info.AddMessageCallMethodType(methodTypeCodeInfo);
                    }
                    else if (attr is OnReplicateDispatchCallAttribute replicateDispatchCallAttribute)
                    {
                        if (string.IsNullOrEmpty(replicateDispatchCallAttribute.Tags) || ReplicateAnnounceType.None == replicateDispatchCallAttribute.AnnounceType)
                        {
                            Debugger.Warn(LogGroupTag.CodeLoader, "The extend replicate dispatch method '{%s}.{%s}' was invalid arguments, added it failed.", symClass.FullName, symMethod.MethodName);
                            continue;
                        }

                        if (false == Inspecting.CodeInspector.CheckFunctionFormatOfReplicateCallWithBeanExtensionType(symMethod.MethodInfo))
                        {
                            Debugger.Warn(LogGroupTag.CodeLoader, "The extend replicate dispatch method '{%s}.{%s}' was invalid format, added it failed.", symClass.FullName, symMethod.MethodName);
                            continue;
                        }

                        Structuring.ReplicateCallMethodTypeCodeInfo methodTypeCodeInfo = new Structuring.ReplicateCallMethodTypeCodeInfo();
                        methodTypeCodeInfo.TargetType = extendClassType;
                        methodTypeCodeInfo.Tags = replicateDispatchCallAttribute.Tags;
                        methodTypeCodeInfo.AnnounceType = replicateDispatchCallAttribute.AnnounceType;
                        methodTypeCodeInfo.BehaviourType = replicateDispatchCallAttribute.BehaviourType;
                        methodTypeCodeInfo.Fullname = symMethod.FullName;
                        methodTypeCodeInfo.Method = symMethod.MethodInfo;
                        Debugger.IsNull(replicateDispatchCallAttribute.ClassType);

                        // 函数参数类型的格式检查，仅在调试模式下执行，正式环境可跳过该处理
                        if (NovaEngine.Debugger.Instance.IsOnDebuggingVerificationActivated())
                        {
                            bool verificated;

                            if (Inspecting.CodeInspector.CheckFunctionFormatOfReplicateCallWithNullParameterType(symMethod.MethodInfo))
                            {
                                verificated = NovaEngine.Debugger.Verification.CheckGenericDelegateParameterTypeMatched(
                                                    symMethod.MethodInfo, methodTypeCodeInfo.TargetType);
                            }
                            else
                            {
                                verificated = NovaEngine.Debugger.Verification.CheckGenericDelegateParameterTypeMatched(
                                                    symMethod.MethodInfo, methodTypeCodeInfo.TargetType, typeof(string), typeof(ReplicateAnnounceType));
                            }

                            // 校验失败
                            if (false == verificated)
                            {
                                Debugger.Error(LogGroupTag.CodeLoader, "Cannot verificated from method info '{%s}' to extend replicate dispatch call, loaded this method failed.", symMethod.MethodName);
                                continue;
                            }
                        }

                        info.AddReplicateCallMethodType(methodTypeCodeInfo);
                    }
                }
            }

            if (info.GetInputCallMethodTypeCount() <= 0 &&
                info.GetEventCallMethodTypeCount() <= 0 &&
                info.GetMessageCallMethodTypeCount() <= 0 &&
                info.GetReplicateCallMethodTypeCount() <= 0)
            {
                Debugger.Warn(LogGroupTag.CodeLoader, "The extend call method types count must be great than zero, newly added class '{%t}' failed.", info.ClassType);
                return false;
            }

            if (_extendCallCodeInfos.ContainsKey(symClass.ClassType))
            {
                if (reload)
                {
                    _extendCallCodeInfos.Remove(symClass.ClassType);
                }
                else
                {
                    Debugger.Warn(LogGroupTag.CodeLoader, "The extend call '{%s}' was already existed, repeat added it failed.", symClass.FullName);
                    return false;
                }
            }

            _extendCallCodeInfos.Add(symClass.ClassType, info);
            Debugger.Log(LogGroupTag.CodeLoader, "Load extend call code info '{%s}' succeed from target class type '{%s}'.", CodeLoaderUtils.ToString(info), symClass.FullName);

            return true;
        }

        [Preserve]
        [OnCodeLoaderClassCleanupOfTarget(typeof(ExtendSupportedAttribute))]
        private static void CleanupAllExtendCallClasses()
        {
            _extendCallCodeInfos.Clear();
        }

        [Preserve]
        [OnCodeLoaderClassLookupOfTarget(typeof(ExtendSupportedAttribute))]
        private static Structuring.ExtendCallCodeInfo LookupExtendCallCodeInfo(Symbolling.SymClass symClass)
        {
            if (_extendCallCodeInfos.TryGetValue(symClass.ClassType, out Structuring.ExtendCallCodeInfo codeInfo))
            {
                return codeInfo;
            }

            return null;
        }
    }
}
