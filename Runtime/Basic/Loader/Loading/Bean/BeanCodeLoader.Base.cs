/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyright (C) 2023 - 2024, Guangzhou Shiyue Network Technology Co., Ltd.
/// Copyright (C) 2024 - 2025, Hurley, Independent Studio.
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
using System.Reflection;

using SystemType = System.Type;
using SystemAttribute = System.Attribute;

namespace GameEngine.Loader
{
    /// <summary>
    /// 程序集中原型对象的分析处理类，对业务层载入的所有原型对象类进行统一加载及分析处理
    /// </summary>
    internal static partial class BeanCodeLoader
    {
        /// <summary>
        /// 引用对象指定类型的属性解析接口
        /// </summary>
        /// <param name="symClass">对象标记类型</param>
        /// <param name="codeInfo">对象结构信息</param>
        /// <param name="attribute">属性对象</param>
        private static void LoadBaseClassByAttributeType(Symboling.SymClass symClass, Structuring.BaseBeanCodeInfo codeInfo, SystemAttribute attribute)
        {
        }

        /// <summary>
        /// 引用对象指定函数的属性解析接口
        /// </summary>
        /// <param name="symClass">对象标记类型</param>
        /// <param name="codeInfo">对象结构信息</param>
        /// <param name="symMethod">函数对象</param>
        /// <param name="attribute">属性对象</param>
        private static void LoadBaseMethodByAttributeType(Symboling.SymClass symClass, Structuring.BaseBeanCodeInfo codeInfo, Symboling.SymMethod symMethod, SystemAttribute attribute)
        {
            if (attribute is InputResponseBindingOfTargetAttribute)
            {
                InputResponseBindingOfTargetAttribute _attr = (InputResponseBindingOfTargetAttribute) attribute;

                if (symMethod.IsStatic || false == Inspecting.CodeInspector.CheckFunctionFormatOfInputCall(symMethod.MethodInfo))
                {
                    Debugger.Warn("The input responsing method '{0}.{1}' was invalid format, added it failed.", symClass.FullName, symMethod.MethodName);
                    return;
                }

                Structuring.InputResponsingMethodTypeCodeInfo methodTypeCodeInfo = new Structuring.InputResponsingMethodTypeCodeInfo();
                methodTypeCodeInfo.InputCode = _attr.InputCode;
                methodTypeCodeInfo.OperationType = _attr.OperationType;
                methodTypeCodeInfo.InputDataType = _attr.InputDataType;
                methodTypeCodeInfo.BehaviourType = _attr.BehaviourType;
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
                        Debugger.Error("Cannot verificated from method info '{0}' to input responsing call, loaded this method failed.", symMethod.FullName);
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
                    Debugger.Warn("The event subscribing method '{0}.{1}' was invalid format, added it failed.", symClass.FullName, symMethod.MethodName);
                    return;
                }

                Structuring.EventSubscribingMethodTypeCodeInfo methodTypeCodeInfo = new Structuring.EventSubscribingMethodTypeCodeInfo();
                methodTypeCodeInfo.EventID = _attr.EventID;
                methodTypeCodeInfo.EventDataType = _attr.EventDataType;
                methodTypeCodeInfo.BehaviourType = _attr.BehaviourType;
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
                        Debugger.Error("Cannot verificated from method info '{0}' to event subscribing call, loaded this method failed.", symMethod.FullName);
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
                    Debugger.Warn("The message binding method '{0}.{1}' was invalid format, added it failed.", symClass.FullName, symMethod.MethodName);
                    return;
                }

                Structuring.MessageBindingMethodTypeCodeInfo methodTypeCodeInfo = new Structuring.MessageBindingMethodTypeCodeInfo();
                methodTypeCodeInfo.Opcode = _attr.Opcode;
                methodTypeCodeInfo.MessageType = _attr.MessageType;
                methodTypeCodeInfo.BehaviourType = _attr.BehaviourType;
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
                                            symMethod.MethodInfo, typeof(ProtoBuf.Extension.IMessage));
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
                        Debugger.Error("Cannot verificated from method info '{0}' to message binding call, loaded this method failed.", symMethod.FullName);
                        return;
                    }
                }

                codeInfo.AddMessageBindingMethodType(methodTypeCodeInfo);
            }
        }
    }
}
