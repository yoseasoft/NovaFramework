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
using System.Reflection;

namespace GameEngine.Loader
{
    /// 程序集中原型对象的分析处理类
    internal static partial class BeanCodeLoader
    {
        /// <summary>
        /// 搜索对象嵌套类型所使用的绑定标识
        /// </summary>
        const BindingFlags SearchBindingFlagsOfClassNestedTypes = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

        /// <summary>
        /// 基础对象的对象类型解析接口
        /// </summary>
        /// <param name="symClass">对象标记类型</param>
        /// <param name="codeInfo">对象结构信息</param>
        private static void LoadBaseClass(Symbolling.SymClass symClass, Structuring.BaseBeanCodeInfo codeInfo)
        {
            Type targetType = symClass.ClassType;
            Type[] nestedTypes = targetType.GetNestedTypes(SearchBindingFlagsOfClassNestedTypes);
            if (null != nestedTypes && nestedTypes.Length > 0)
            {
                for (int n = 0; n < nestedTypes.Length; ++n)
                {
                    Type nestedType = nestedTypes[n];
                    // do anything ...
                }
            }
        }

        /// <summary>
        /// 基础对象指定类型的属性解析接口
        /// </summary>
        /// <param name="symClass">对象标记类型</param>
        /// <param name="codeInfo">对象结构信息</param>
        /// <param name="attribute">属性对象</param>
        private static void LoadBaseClassByAttributeType(Symbolling.SymClass symClass, Structuring.BaseBeanCodeInfo codeInfo, Attribute attribute)
        {
        }

        /// <summary>
        /// 基础对象指定函数的属性解析接口
        /// </summary>
        /// <param name="symClass">对象标记类型</param>
        /// <param name="codeInfo">对象结构信息</param>
        /// <param name="symMethod">函数对象</param>
        /// <param name="attribute">属性对象</param>
        private static void LoadBaseMethodByAttributeType(Symbolling.SymClass symClass, Structuring.BaseBeanCodeInfo codeInfo, Symbolling.SymMethod symMethod, Attribute attribute)
        {
            if (attribute is InputResponseBindingOfTargetAttribute)
            {
                InputResponseBindingOfTargetAttribute _attr = (InputResponseBindingOfTargetAttribute) attribute;

                if (symMethod.IsStatic || false == Inspecting.CodeInspector.CheckFunctionFormatOfInputCall(symMethod.MethodInfo))
                {
                    Debugger.Warn("The input responsing method '{%s}.{%s}' was invalid format, added it failed.", symClass.FullName, symMethod.MethodName);
                    return;
                }

                Structuring.InputResponsingMethodTypeCodeInfo methodTypeCodeInfo = new Structuring.InputResponsingMethodTypeCodeInfo();
                methodTypeCodeInfo.InputCode = _attr.InputCode;
                methodTypeCodeInfo.OperationType = _attr.OperationType;
                methodTypeCodeInfo.InputDataType = _attr.InputDataType;
                methodTypeCodeInfo.BehaviourType = _attr.BehaviourType;
                methodTypeCodeInfo.Fullname = symMethod.FullName;
                methodTypeCodeInfo.Method = symMethod.MethodInfo;

                // 函数参数类型的格式检查，仅在调试模式下执行，正式环境可跳过该处理
                if (NovaEngine.Debugger.Instance.IsOnDebuggingVerificationActivated())
                {
                    bool verificated = NovaEngine.Debugger.Verification.CheckGenericDelegateParameterTypeMatched(
                                            Inspecting.CodeInspector.CheckFunctionFormatOfInputCallWithNullParameterType(symMethod.MethodInfo), symMethod.MethodInfo);

                    if (Inspecting.CodeInspector.CheckFunctionFormatOfInputCallWithNullParameterType(symMethod.MethodInfo))
                    {
                        // null parameter type, skip other check process
                    }
                    else if (methodTypeCodeInfo.InputCode > 0)
                    {
                        verificated = NovaEngine.Debugger.Verification.CheckGenericDelegateParameterTypeMatched(
                                            false == Inspecting.CodeInspector.CheckFunctionFormatOfInputCallWithNullParameterType(symMethod.MethodInfo),
                                            symMethod.MethodInfo, typeof(int), typeof(int));
                    }
                    else
                    {
                        verificated = NovaEngine.Debugger.Verification.CheckGenericDelegateParameterTypeMatched(
                                            false == Inspecting.CodeInspector.CheckFunctionFormatOfInputCallWithNullParameterType(symMethod.MethodInfo),
                                            symMethod.MethodInfo, methodTypeCodeInfo.InputDataType);
                    }

                    // 校验失败
                    if (false == verificated)
                    {
                        Debugger.Error("Cannot verificated from method info '{%s}' to input responsing call, loaded this method failed.", symMethod.FullName);
                        return;
                    }
                }

                codeInfo.AddInputResponsingMethodType(methodTypeCodeInfo);
            }
            else if (attribute is EventSubscribeBindingOfTargetAttribute)
            {
                EventSubscribeBindingOfTargetAttribute _attr = (EventSubscribeBindingOfTargetAttribute) attribute;

                if (symMethod.IsStatic || false == Inspecting.CodeInspector.CheckFunctionFormatOfEventCall(symMethod.MethodInfo))
                {
                    Debugger.Warn("The event subscribing method '{%s}.{%s}' was invalid format, added it failed.", symClass.FullName, symMethod.MethodName);
                    return;
                }

                Structuring.EventSubscribingMethodTypeCodeInfo methodTypeCodeInfo = new Structuring.EventSubscribingMethodTypeCodeInfo();
                methodTypeCodeInfo.EventID = _attr.EventID;
                methodTypeCodeInfo.EventDataType = _attr.EventDataType;
                methodTypeCodeInfo.BehaviourType = _attr.BehaviourType;
                methodTypeCodeInfo.Fullname = symMethod.FullName;
                methodTypeCodeInfo.Method = symMethod.MethodInfo;

                // 函数参数类型的格式检查，仅在调试模式下执行，正式环境可跳过该处理
                if (NovaEngine.Debugger.Instance.IsOnDebuggingVerificationActivated())
                {
                    bool verificated = NovaEngine.Debugger.Verification.CheckGenericDelegateParameterTypeMatched(
                                            Inspecting.CodeInspector.CheckFunctionFormatOfEventCallWithNullParameterType(symMethod.MethodInfo), symMethod.MethodInfo);

                    if (Inspecting.CodeInspector.CheckFunctionFormatOfEventCallWithNullParameterType(symMethod.MethodInfo))
                    {
                        // null parameter type, skip other check process
                    }
                    else if (methodTypeCodeInfo.EventID > 0)
                    {
                        verificated = NovaEngine.Debugger.Verification.CheckGenericDelegateParameterTypeMatched(
                                            false == Inspecting.CodeInspector.CheckFunctionFormatOfEventCallWithNullParameterType(symMethod.MethodInfo),
                                            symMethod.MethodInfo, typeof(int), typeof(object[]));
                    }
                    else
                    {
                        verificated = NovaEngine.Debugger.Verification.CheckGenericDelegateParameterTypeMatched(
                                            false == Inspecting.CodeInspector.CheckFunctionFormatOfEventCallWithNullParameterType(symMethod.MethodInfo),
                                            symMethod.MethodInfo, methodTypeCodeInfo.EventDataType);
                    }

                    // 校验失败
                    if (false == verificated)
                    {
                        Debugger.Error("Cannot verificated from method info '{%s}' to event subscribing call, loaded this method failed.", symMethod.FullName);
                        return;
                    }
                }

                codeInfo.AddEventSubscribingMethodType(methodTypeCodeInfo);
            }
            else if (attribute is MessageListenerBindingOfTargetAttribute)
            {
                MessageListenerBindingOfTargetAttribute _attr = (MessageListenerBindingOfTargetAttribute) attribute;

                if (symMethod.IsStatic || false == Inspecting.CodeInspector.CheckFunctionFormatOfMessageCall(symMethod.MethodInfo))
                {
                    Debugger.Warn("The message binding method '{%s}.{%s}' was invalid format, added it failed.", symClass.FullName, symMethod.MethodName);
                    return;
                }

                Structuring.MessageListeningMethodTypeCodeInfo methodTypeCodeInfo = new Structuring.MessageListeningMethodTypeCodeInfo();
                methodTypeCodeInfo.Opcode = _attr.Opcode;
                methodTypeCodeInfo.MessageType = _attr.MessageType;
                methodTypeCodeInfo.BehaviourType = _attr.BehaviourType;
                methodTypeCodeInfo.Fullname = symMethod.FullName;
                methodTypeCodeInfo.Method = symMethod.MethodInfo;

                // 函数参数类型的格式检查，仅在调试模式下执行，正式环境可跳过该处理
                if (NovaEngine.Debugger.Instance.IsOnDebuggingVerificationActivated())
                {
                    bool verificated = NovaEngine.Debugger.Verification.CheckGenericDelegateParameterTypeMatched(
                                            Inspecting.CodeInspector.CheckFunctionFormatOfMessageCallWithNullParameterType(symMethod.MethodInfo), symMethod.MethodInfo);

                    if (Inspecting.CodeInspector.CheckFunctionFormatOfMessageCallWithNullParameterType(symMethod.MethodInfo))
                    {
                        // null parameter type, skip other check process
                    }
                    else if (methodTypeCodeInfo.Opcode > 0)
                    {
                        verificated = NovaEngine.Debugger.Verification.CheckGenericDelegateParameterTypeMatched(
                                            false == Inspecting.CodeInspector.CheckFunctionFormatOfMessageCallWithNullParameterType(symMethod.MethodInfo),
                                            symMethod.MethodInfo, typeof(object));
                    }
                    else
                    {
                        verificated = NovaEngine.Debugger.Verification.CheckGenericDelegateParameterTypeMatched(
                                            false == Inspecting.CodeInspector.CheckFunctionFormatOfMessageCallWithNullParameterType(symMethod.MethodInfo),
                                            symMethod.MethodInfo, methodTypeCodeInfo.MessageType);
                    }

                    // 校验失败
                    if (false == verificated)
                    {
                        Debugger.Error("Cannot verificated from method info '{%s}' to message binding call, loaded this method failed.", symMethod.FullName);
                        return;
                    }
                }

                codeInfo.AddMessageListeningMethodType(methodTypeCodeInfo);
            }
            else if (attribute is ReplicateCommunicateBindingOfTargetAttribute)
            {
                ReplicateCommunicateBindingOfTargetAttribute _attr = (ReplicateCommunicateBindingOfTargetAttribute) attribute;

                if (symMethod.IsStatic || false == Inspecting.CodeInspector.CheckFunctionFormatOfReplicateCall(symMethod.MethodInfo))
                {
                    Debugger.Warn("The replicate communicating method '{%s}.{%s}' was invalid format, added it failed.", symClass.FullName, symMethod.MethodName);
                    return;
                }

                Structuring.ReplicateCommunicatingMethodTypeCodeInfo methodTypeCodeInfo = new Structuring.ReplicateCommunicatingMethodTypeCodeInfo();
                methodTypeCodeInfo.Tags = _attr.Tags;
                methodTypeCodeInfo.AnnounceType = _attr.AnnounceType;
                methodTypeCodeInfo.BehaviourType = _attr.BehaviourType;
                methodTypeCodeInfo.Fullname = symMethod.FullName;
                methodTypeCodeInfo.Method = symMethod.MethodInfo;

                // 函数参数类型的格式检查，仅在调试模式下执行，正式环境可跳过该处理
                if (NovaEngine.Debugger.Instance.IsOnDebuggingVerificationActivated())
                {
                    bool verificated = NovaEngine.Debugger.Verification.CheckGenericDelegateParameterTypeMatched(
                                            Inspecting.CodeInspector.CheckFunctionFormatOfReplicateCallWithNullParameterType(symMethod.MethodInfo), symMethod.MethodInfo);

                    if (Inspecting.CodeInspector.CheckFunctionFormatOfReplicateCallWithNullParameterType(symMethod.MethodInfo))
                    {
                        // null parameter type, skip other check process
                    }
                    else
                    {
                        verificated = NovaEngine.Debugger.Verification.CheckGenericDelegateParameterTypeMatched(
                                            false == Inspecting.CodeInspector.CheckFunctionFormatOfReplicateCallWithNullParameterType(symMethod.MethodInfo),
                                            symMethod.MethodInfo, typeof(string), typeof(ReplicateAnnounceType));
                    }

                    // 校验失败
                    if (false == verificated)
                    {
                        Debugger.Error("Cannot verificated from method info '{%s}' to replicate communicating call, loaded this method failed.", symMethod.FullName);
                        return;
                    }
                }

                codeInfo.AddReplicateCommunicatingMethodType(methodTypeCodeInfo);
            }
        }
    }
}
