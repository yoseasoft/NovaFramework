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

using SystemType = System.Type;
using SystemAttribute = System.Attribute;

namespace GameEngine.Loader
{
    /// <summary>
    /// 程序集中扩展定义对象的分析处理类，对业务层载入的所有扩展定义对象类进行统一加载及分析处理
    /// </summary>
    internal static partial class ExtendCodeLoader
    {
        /// <summary>
        /// 扩展定义调用类的结构信息管理容器
        /// </summary>
        private static IDictionary<SystemType, Structuring.ExtendCallCodeInfo> _extendCallCodeInfos = new Dictionary<SystemType, Structuring.ExtendCallCodeInfo>();

        [OnCodeLoaderClassLoadOfTarget(typeof(ExtendSupportedAttribute))]
        private static bool LoadExtendCallClass(Symboling.SymClass symClass, bool reload)
        {
            Structuring.ExtendCallCodeInfo info = new Structuring.ExtendCallCodeInfo();
            info.ClassType = symClass.ClassType;

            IList<Symboling.SymMethod> symMethods = symClass.GetAllMethods();
            for (int n = 0; null != symMethods && n < symMethods.Count; ++n)
            {
                Symboling.SymMethod symMethod = symMethods[n];

                // 检查函数格式是否合法，扩展类型的函数必须为静态类型
                if (false == symMethod.IsStatic || false == symMethod.IsExtension)
                {
                    Debugger.Info(LogGroupTag.CodeLoader, "符号类‘{%t}’的目标函数‘{%t}’不满足‘ExtendSupport’扩展回调函数格式类型的定义规则，不能进行正确的函数加载解析！", symClass.ClassType, symMethod.MethodInfo);
                    continue;
                }

                IList<SystemAttribute> attrs = symMethod.Attributes;
                for (int m = 0; null != attrs && m < attrs.Count; ++m)
                {
                    SystemAttribute attr = attrs[m];

                    if (attr is InputResponseBindingOfTargetAttribute)
                    {
                        InputResponseBindingOfTargetAttribute _attr = (InputResponseBindingOfTargetAttribute) attr;

                        if (_attr.InputCode <= 0 && null == _attr.InputDataType)
                        {
                            Debugger.Warn("The extend input response method '{0}.{1}' was invalid arguments, added it failed.", symClass.FullName, symMethod.MethodName);
                            continue;
                        }

                        if (false == Inspecting.CodeInspector.CheckFunctionFormatOfInputCallWithBeanExtensionType(symMethod.MethodInfo))
                        {
                            Debugger.Warn("The extend input response method '{0}.{1}' was invalid format, added it failed.", symClass.FullName, symMethod.MethodName);
                            continue;
                        }

                        SystemType extendClassType = symMethod.GetParameter(0).ParameterType;

                        Structuring.InputResponsingMethodTypeCodeInfo methodTypeCodeInfo = new Structuring.InputResponsingMethodTypeCodeInfo();
                        methodTypeCodeInfo.TargetType = extendClassType;
                        methodTypeCodeInfo.InputCode = _attr.InputCode;
                        methodTypeCodeInfo.OperationType = _attr.OperationType;
                        methodTypeCodeInfo.InputDataType = _attr.InputDataType;
                        methodTypeCodeInfo.BehaviourType = _attr.BehaviourType;
                        methodTypeCodeInfo.Method = symMethod.MethodInfo;

                        // 函数参数类型的格式检查，仅在调试模式下执行，正式环境可跳过该处理
                        if (NovaEngine.Debugger.Instance.IsOnDebuggingVerificationActivated())
                        {
                            bool verificated = NovaEngine.Debugger.Verification.CheckGenericDelegateParameterTypeMatched(
                                                    Inspecting.CodeInspector.CheckFunctionFormatOfInputCallWithNullParameterType(symMethod.MethodInfo),
                                                    symMethod.MethodInfo, methodTypeCodeInfo.TargetType);

                            if (Inspecting.CodeInspector.CheckFunctionFormatOfInputCallWithNullParameterType(symMethod.MethodInfo))
                            {
                                // null parameter type, skip other check process
                            }
                            else if (methodTypeCodeInfo.InputCode > 0)
                            {
                                verificated = NovaEngine.Debugger.Verification.CheckGenericDelegateParameterTypeMatched(
                                                    false == Inspecting.CodeInspector.CheckFunctionFormatOfInputCallWithNullParameterType(symMethod.MethodInfo),
                                                    symMethod.MethodInfo, methodTypeCodeInfo.TargetType, typeof(int), typeof(int));
                            }
                            else
                            {
                                verificated = NovaEngine.Debugger.Verification.CheckGenericDelegateParameterTypeMatched(
                                                    false == Inspecting.CodeInspector.CheckFunctionFormatOfInputCallWithNullParameterType(symMethod.MethodInfo),
                                                    symMethod.MethodInfo, methodTypeCodeInfo.TargetType, methodTypeCodeInfo.InputDataType);
                            }

                            // 校验失败
                            if (false == verificated)
                            {
                                Debugger.Error("Cannot verificated from method info '{0}' to extend input responsing call, loaded this method failed.", symMethod.MethodName);
                                continue;
                            }
                        }

                        info.AddInputCallMethodType(methodTypeCodeInfo);
                    }
                    else if (attr is EventSubscribeBindingOfTargetAttribute)
                    {
                        EventSubscribeBindingOfTargetAttribute _attr = (EventSubscribeBindingOfTargetAttribute) attr;

                        if (_attr.EventID <= 0 && null == _attr.EventDataType)
                        {
                            Debugger.Warn("The extend event subscribe method '{0}.{1}' was invalid arguments, added it failed.", symClass.FullName, symMethod.MethodName);
                            continue;
                        }

                        if (false == Inspecting.CodeInspector.CheckFunctionFormatOfEventCallWithBeanExtensionType(symMethod.MethodInfo))
                        {
                            Debugger.Warn("The extend event subscribe method '{0}.{1}' was invalid format, added it failed.", symClass.FullName, symMethod.MethodName);
                            continue;
                        }

                        SystemType extendClassType = symMethod.GetParameter(0).ParameterType;

                        Structuring.EventSubscribingMethodTypeCodeInfo methodTypeCodeInfo = new Structuring.EventSubscribingMethodTypeCodeInfo();
                        methodTypeCodeInfo.TargetType = extendClassType;
                        methodTypeCodeInfo.EventID = _attr.EventID;
                        methodTypeCodeInfo.EventDataType = _attr.EventDataType;
                        methodTypeCodeInfo.BehaviourType = _attr.BehaviourType;
                        methodTypeCodeInfo.Method = symMethod.MethodInfo;

                        // 函数参数类型的格式检查，仅在调试模式下执行，正式环境可跳过该处理
                        if (NovaEngine.Debugger.Instance.IsOnDebuggingVerificationActivated())
                        {
                            bool verificated = NovaEngine.Debugger.Verification.CheckGenericDelegateParameterTypeMatched(
                                                    Inspecting.CodeInspector.CheckFunctionFormatOfEventCallWithNullParameterType(symMethod.MethodInfo),
                                                    symMethod.MethodInfo, methodTypeCodeInfo.TargetType);

                            if (Inspecting.CodeInspector.CheckFunctionFormatOfEventCallWithNullParameterType(symMethod.MethodInfo))
                            {
                                // null parameter type, skip other check process
                            }
                            else if (methodTypeCodeInfo.EventID > 0)
                            {
                                verificated = NovaEngine.Debugger.Verification.CheckGenericDelegateParameterTypeMatched(
                                                    false == Inspecting.CodeInspector.CheckFunctionFormatOfEventCallWithNullParameterType(symMethod.MethodInfo),
                                                    symMethod.MethodInfo, methodTypeCodeInfo.TargetType, typeof(int), typeof(object[]));
                            }
                            else
                            {
                                verificated = NovaEngine.Debugger.Verification.CheckGenericDelegateParameterTypeMatched(
                                                    false == Inspecting.CodeInspector.CheckFunctionFormatOfEventCallWithNullParameterType(symMethod.MethodInfo),
                                                    symMethod.MethodInfo, methodTypeCodeInfo.TargetType, methodTypeCodeInfo.EventDataType);
                            }

                            // 校验失败
                            if (false == verificated)
                            {
                                Debugger.Error("Cannot verificated from method info '{0}' to extend event subscribing call, loaded this method failed.", symMethod.MethodName);
                                continue;
                            }
                        }

                        info.AddEventCallMethodType(methodTypeCodeInfo);
                    }
                    else if (attr is MessageListenerBindingOfTargetAttribute)
                    {
                        MessageListenerBindingOfTargetAttribute _attr = (MessageListenerBindingOfTargetAttribute) attr;

                        if (_attr.Opcode <= 0 && null == _attr.MessageType)
                        {
                            Debugger.Warn("The extend message listener method '{0}.{1}' was invalid arguments, added it failed.", symClass.FullName, symMethod.MethodName);
                            continue;
                        }

                        if (false == Inspecting.CodeInspector.CheckFunctionFormatOfMessageCallWithBeanExtensionType(symMethod.MethodInfo))
                        {
                            Debugger.Warn("The extend message recv method '{0}.{1}' was invalid format, added it failed.", symClass.FullName, symMethod.MethodName);
                            continue;
                        }

                        SystemType extendClassType = symMethod.GetParameter(0).ParameterType;

                        Structuring.MessageBindingMethodTypeCodeInfo methodTypeCodeInfo = new Structuring.MessageBindingMethodTypeCodeInfo();
                        methodTypeCodeInfo.TargetType = extendClassType;
                        methodTypeCodeInfo.Opcode = _attr.Opcode;
                        methodTypeCodeInfo.MessageType = _attr.MessageType;
                        methodTypeCodeInfo.BehaviourType = _attr.BehaviourType;
                        methodTypeCodeInfo.Method = symMethod.MethodInfo;

                        // 函数参数类型的格式检查，仅在调试模式下执行，正式环境可跳过该处理
                        if (NovaEngine.Debugger.Instance.IsOnDebuggingVerificationActivated())
                        {
                            bool verificated = NovaEngine.Debugger.Verification.CheckGenericDelegateParameterTypeMatched(
                                                    Inspecting.CodeInspector.CheckFunctionFormatOfMessageCallWithNullParameterType(symMethod.MethodInfo),
                                                    symMethod.MethodInfo, methodTypeCodeInfo.TargetType);

                            if (Inspecting.CodeInspector.CheckFunctionFormatOfMessageCallWithNullParameterType(symMethod.MethodInfo))
                            {
                                // null parameter type, skip other check process
                            }
                            else if (methodTypeCodeInfo.Opcode > 0)
                            {
                                verificated = NovaEngine.Debugger.Verification.CheckGenericDelegateParameterTypeMatched(
                                                    false == Inspecting.CodeInspector.CheckFunctionFormatOfMessageCallWithNullParameterType(symMethod.MethodInfo),
                                                    symMethod.MethodInfo, methodTypeCodeInfo.TargetType, typeof(ProtoBuf.Extension.IMessage));
                            }
                            else
                            {
                                verificated = NovaEngine.Debugger.Verification.CheckGenericDelegateParameterTypeMatched(
                                                    false == Inspecting.CodeInspector.CheckFunctionFormatOfMessageCallWithNullParameterType(symMethod.MethodInfo),
                                                    symMethod.MethodInfo, methodTypeCodeInfo.TargetType, methodTypeCodeInfo.MessageType);
                            }

                            // 校验失败
                            if (false == verificated)
                            {
                                Debugger.Error("Cannot verificated from method info '{0}' to extend message binding call, loaded this method failed.", symMethod.MethodName);
                                continue;
                            }
                        }

                        info.AddMessageCallMethodType(methodTypeCodeInfo);
                    }
                    else if (attr is StateTransitionBindingOfTargetAttribute)
                    {
                        StateTransitionBindingOfTargetAttribute _attr = (StateTransitionBindingOfTargetAttribute) attr;

                        if (string.IsNullOrEmpty(_attr.StateName))
                        {
                            Debugger.Warn("The extend state transition method '{0}.{1}' was invalid arguments, added it failed.", symClass.FullName, symMethod.MethodName);
                            continue;
                        }

                        if (false == Inspecting.CodeInspector.CheckFunctionFormatOfStateCallWithBeanExtensionType(symMethod.MethodInfo))
                        {
                            Debugger.Warn("The extend state transition method '{0}.{1}' was invalid format, added it failed.", symClass.FullName, symMethod.MethodName);
                            continue;
                        }

                        SystemType extendClassType = symMethod.GetParameter(0).ParameterType;

                        Structuring.StateTransitioningMethodTypeCodeInfo methodTypeCodeInfo = new Structuring.StateTransitioningMethodTypeCodeInfo();
                        methodTypeCodeInfo.TargetType = extendClassType;
                        methodTypeCodeInfo.StateName = _attr.StateName;
                        methodTypeCodeInfo.AccessType = _attr.AccessType;
                        methodTypeCodeInfo.BehaviourType = AspectBehaviourType.Initialize;
                        methodTypeCodeInfo.Method = symMethod.MethodInfo;

                        // 函数参数类型的格式检查，仅在调试模式下执行，正式环境可跳过该处理
                        if (NovaEngine.Debugger.Instance.IsOnDebuggingVerificationActivated())
                        {
                            bool verificated = false;

                            if (Inspecting.CodeInspector.CheckFunctionFormatOfStateCallWithNullParameterType(symMethod.MethodInfo))
                            {
                                verificated = NovaEngine.Debugger.Verification.CheckGenericDelegateParameterTypeMatched(
                                                    symMethod.MethodInfo, methodTypeCodeInfo.TargetType);
                            }
                            else
                            {
                                verificated = NovaEngine.Debugger.Verification.CheckGenericDelegateParameterTypeMatched(
                                        symMethod.MethodInfo, methodTypeCodeInfo.TargetType, typeof(StateGraph));
                            }

                            // 校验失败
                            if (false == verificated)
                            {
                                Debugger.Error("Cannot verificated from method info '{0}' to extend state transitioning call, loaded this method failed.", symMethod.MethodName);
                                continue;
                            }
                        }

                        info.AddStateCallMethodType(methodTypeCodeInfo);
                    }
                }
            }

            if (info.GetInputCallMethodTypeCount() <= 0 &&
                info.GetEventCallMethodTypeCount() <= 0 &&
                info.GetMessageCallMethodTypeCount() <= 0 &&
                info.GetStateCallMethodTypeCount() <= 0)
            {
                Debugger.Warn("The extend call method types count must be great than zero, newly added class '{0}' failed.", info.ClassType.FullName);
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
                    Debugger.Warn("The extend call '{0}' was already existed, repeat added it failed.", symClass.FullName);
                    return false;
                }
            }

            _extendCallCodeInfos.Add(symClass.ClassType, info);
            Debugger.Log(LogGroupTag.CodeLoader, "Load extend call code info '{0}' succeed from target class type '{1}'.", info.ToString(), symClass.FullName);

            return true;
        }

        [OnCodeLoaderClassCleanupOfTarget(typeof(ExtendSupportedAttribute))]
        private static void CleanupAllExtendCallClasses()
        {
            _extendCallCodeInfos.Clear();
        }

        [OnCodeLoaderClassLookupOfTarget(typeof(ExtendSupportedAttribute))]
        private static Structuring.ExtendCallCodeInfo LookupExtendCallCodeInfo(Symboling.SymClass symClass)
        {
            foreach (KeyValuePair<SystemType, Structuring.ExtendCallCodeInfo> pair in _extendCallCodeInfos)
            {
                if (pair.Value.ClassType == symClass.ClassType)
                {
                    return pair.Value;
                }
            }

            return null;
        }
    }
}
