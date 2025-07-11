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
using SystemStringBuilder = System.Text.StringBuilder;

namespace GameEngine.Loader
{
    /// <summary>
    /// 扩展定义调用类的结构信息
    /// </summary>
    public class ExtendCallCodeInfo : ExtendCodeInfo
    {
        /// <summary>
        /// 原型对象输入响应的扩展定义调用类的数据管理容器
        /// </summary>
        private IList<InputResponsingMethodTypeCodeInfo> _inputCallMethodTypes;

        /// <summary>
        /// 原型对象事件订阅的扩展定义调用类的数据管理容器
        /// </summary>
        private IList<EventSubscribingMethodTypeCodeInfo> _eventCallMethodTypes;

        /// <summary>
        /// 原型对象消息处理的扩展定义调用类的数据管理容器
        /// </summary>
        private IList<MessageBindingMethodTypeCodeInfo> _messageCallMethodTypes;

        #region 扩展输入调用类结构信息操作函数

        /// <summary>
        /// 新增指定函数的回调句柄相关的结构信息
        /// </summary>
        /// <param name="invoke">函数的结构信息</param>
        public void AddInputCallMethodType(InputResponsingMethodTypeCodeInfo invoke)
        {
            if (null == _inputCallMethodTypes)
            {
                _inputCallMethodTypes = new List<InputResponsingMethodTypeCodeInfo>();
            }

            if (_inputCallMethodTypes.Contains(invoke))
            {
                Debugger.Warn("The extend input call class type '{0}' was already registed target input '{1}', repeat added it failed.", _classType.FullName, invoke.InputCode);
                return;
            }

            _inputCallMethodTypes.Add(invoke);
        }

        /// <summary>
        /// 移除所有函数的回调句柄相关的结构信息
        /// </summary>
        public void RemoveAllInputCallMethodTypes()
        {
            _inputCallMethodTypes?.Clear();
            _inputCallMethodTypes = null;
        }

        /// <summary>
        /// 获取当前函数回调句柄的结构信息数量
        /// </summary>
        /// <returns>返回函数回调句柄的结构信息数量</returns>
        public int GetInputCallMethodTypeCount()
        {
            if (null != _inputCallMethodTypes)
            {
                return _inputCallMethodTypes.Count;
            }

            return 0;
        }

        /// <summary>
        /// 获取当前函数回调句柄的结构信息容器中指索引对应的实例
        /// </summary>
        /// <param name="index">索引值</param>
        /// <returns>返回给定索引值对应的实例，若不存在对应实例则返回null</returns>
        public InputResponsingMethodTypeCodeInfo GetInputCallMethodType(int index)
        {
            if (null == _inputCallMethodTypes || index < 0 || index >= _inputCallMethodTypes.Count)
            {
                Debugger.Warn("Invalid index ({0}) for extend input call method type code info list.", index);
                return null;
            }

            return _inputCallMethodTypes[index];
        }

        #endregion

        #region 扩展事件调用类结构信息操作函数

        /// <summary>
        /// 新增指定函数的回调句柄相关的结构信息
        /// </summary>
        /// <param name="invoke">函数的结构信息</param>
        public void AddEventCallMethodType(EventSubscribingMethodTypeCodeInfo invoke)
        {
            if (null == _eventCallMethodTypes)
            {
                _eventCallMethodTypes = new List<EventSubscribingMethodTypeCodeInfo>();
            }

            if (_eventCallMethodTypes.Contains(invoke))
            {
                Debugger.Warn("The extend event call class type '{0}' was already registed target event '{1}', repeat added it failed.", _classType.FullName, invoke.EventID);
                return;
            }

            _eventCallMethodTypes.Add(invoke);
        }

        /// <summary>
        /// 移除所有函数的回调句柄相关的结构信息
        /// </summary>
        public void RemoveAllEventCallMethodTypes()
        {
            _eventCallMethodTypes?.Clear();
            _eventCallMethodTypes = null;
        }

        /// <summary>
        /// 获取当前函数回调句柄的结构信息数量
        /// </summary>
        /// <returns>返回函数回调句柄的结构信息数量</returns>
        public int GetEventCallMethodTypeCount()
        {
            if (null != _eventCallMethodTypes)
            {
                return _eventCallMethodTypes.Count;
            }

            return 0;
        }

        /// <summary>
        /// 获取当前函数回调句柄的结构信息容器中指索引对应的实例
        /// </summary>
        /// <param name="index">索引值</param>
        /// <returns>返回给定索引值对应的实例，若不存在对应实例则返回null</returns>
        public EventSubscribingMethodTypeCodeInfo GetEventCallMethodType(int index)
        {
            if (null == _eventCallMethodTypes || index < 0 || index >= _eventCallMethodTypes.Count)
            {
                Debugger.Warn("Invalid index ({0}) for extend event call method type code info list.", index);
                return null;
            }

            return _eventCallMethodTypes[index];
        }

        #endregion

        #region 扩展消息调用类结构信息操作函数

        /// <summary>
        /// 新增指定函数的回调句柄相关的结构信息
        /// </summary>
        /// <param name="invoke">函数的结构信息</param>
        public void AddMessageCallMethodType(MessageBindingMethodTypeCodeInfo invoke)
        {
            if (null == _messageCallMethodTypes)
            {
                _messageCallMethodTypes = new List<MessageBindingMethodTypeCodeInfo>();
            }

            if (_messageCallMethodTypes.Contains(invoke))
            {
                Debugger.Warn("The extend message call class type '{0}' was already registed target event '{1}', repeat added it failed.", _classType.FullName, invoke.Opcode);
                return;
            }

            _messageCallMethodTypes.Add(invoke);
        }

        /// <summary>
        /// 移除所有函数的回调句柄相关的结构信息
        /// </summary>
        public void RemoveAllMessageCallMethodTypes()
        {
            _messageCallMethodTypes?.Clear();
            _messageCallMethodTypes = null;
        }

        /// <summary>
        /// 获取当前函数回调句柄的结构信息数量
        /// </summary>
        /// <returns>返回函数回调句柄的结构信息数量</returns>
        public int GetMessageCallMethodTypeCount()
        {
            if (null != _messageCallMethodTypes)
            {
                return _messageCallMethodTypes.Count;
            }

            return 0;
        }

        /// <summary>
        /// 获取当前函数回调句柄的结构信息容器中指索引对应的实例
        /// </summary>
        /// <param name="index">索引值</param>
        /// <returns>返回给定索引值对应的实例，若不存在对应实例则返回null</returns>
        public MessageBindingMethodTypeCodeInfo GetMessageCallMethodType(int index)
        {
            if (null == _messageCallMethodTypes || index < 0 || index >= _messageCallMethodTypes.Count)
            {
                Debugger.Warn("Invalid index ({0}) for extend message call method type code info list.", index);
                return null;
            }

            return _messageCallMethodTypes[index];
        }

        #endregion

        public override string ToString()
        {
            SystemStringBuilder sb = new SystemStringBuilder();
            sb.Append("ExtendCall = { ");
            sb.AppendFormat("Parent = {0}, ", base.ToString());

            sb.AppendFormat("InputCallMethodTypes = {{{0}}}, ", NovaEngine.Utility.Text.ToString<InputResponsingMethodTypeCodeInfo>(_inputCallMethodTypes));
            sb.AppendFormat("EventCallMethodTypes = {{{0}}}, ", NovaEngine.Utility.Text.ToString<EventSubscribingMethodTypeCodeInfo>(_eventCallMethodTypes));
            sb.AppendFormat("MessageCallMethodTypes = {{{0}}}, ", NovaEngine.Utility.Text.ToString<MessageBindingMethodTypeCodeInfo>(_messageCallMethodTypes));

            sb.Append("}");
            return sb.ToString();
        }
    }

    /// <summary>
    /// 程序集中扩展定义对象的分析处理类，对业务层载入的所有扩展定义对象类进行统一加载及分析处理
    /// </summary>
    internal static partial class ExtendCodeLoader
    {
        /// <summary>
        /// 扩展定义调用类的结构信息管理容器
        /// </summary>
        private static IDictionary<SystemType, ExtendCallCodeInfo> _extendCallCodeInfos = new Dictionary<SystemType, ExtendCallCodeInfo>();

        [OnExtendClassLoadOfTarget(typeof(ExtendSupportedAttribute))]
        private static bool LoadExtendCallClass(Symboling.SymClass symClass, bool reload)
        {
            ExtendCallCodeInfo info = new ExtendCallCodeInfo();
            info.ClassType = symClass.ClassType;

            IList<Symboling.SymMethod> symMethods = symClass.GetAllMethods();
            for (int n = 0; null != symMethods && n < symMethods.Count; ++n)
            {
                Symboling.SymMethod symMethod = symMethods[n];

                // 检查函数格式是否合法，扩展类型的函数必须为静态类型
                if (false == symMethod.IsStatic || false == symMethod.IsExtension)
                {
                    Debugger.Info(LogGroupTag.CodeLoader, "The extend call method '{0}.{1}' was invalid format, added it failed.", symClass.FullName, symMethod.MethodName);
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

                        if (false == Inspecting.CodeInspector.IsValidFormatOfProtoExtendInputCallFunction(symMethod.MethodInfo))
                        {
                            Debugger.Warn("The extend input response method '{0}.{1}' was invalid format, added it failed.", symClass.FullName, symMethod.MethodName);
                            continue;
                        }

                        SystemType extendClassType = symMethod.GetParameter(0).ParameterType;

                        InputResponsingMethodTypeCodeInfo methodTypeCodeInfo = new InputResponsingMethodTypeCodeInfo();
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
                                                    Inspecting.CodeInspector.IsNullParameterTypeOfInputCallFunction(symMethod.MethodInfo),
                                                    symMethod.MethodInfo, methodTypeCodeInfo.TargetType);

                            if (Inspecting.CodeInspector.IsNullParameterTypeOfInputCallFunction(symMethod.MethodInfo))
                            {
                                // null parameter type, skip other check process
                            }
                            else if (methodTypeCodeInfo.InputCode > 0)
                            {
                                verificated = NovaEngine.Debugger.Verification.CheckGenericDelegateParameterTypeMatched(
                                                    false == Inspecting.CodeInspector.IsNullParameterTypeOfInputCallFunction(symMethod.MethodInfo),
                                                    symMethod.MethodInfo, methodTypeCodeInfo.TargetType, typeof(int), typeof(int));
                            }
                            else
                            {
                                verificated = NovaEngine.Debugger.Verification.CheckGenericDelegateParameterTypeMatched(
                                                    false == Inspecting.CodeInspector.IsNullParameterTypeOfInputCallFunction(symMethod.MethodInfo),
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

                        if (false == Inspecting.CodeInspector.IsValidFormatOfProtoExtendEventCallFunction(symMethod.MethodInfo))
                        {
                            Debugger.Warn("The extend event subscribe method '{0}.{1}' was invalid format, added it failed.", symClass.FullName, symMethod.MethodName);
                            continue;
                        }

                        SystemType extendClassType = symMethod.GetParameter(0).ParameterType;

                        EventSubscribingMethodTypeCodeInfo methodTypeCodeInfo = new EventSubscribingMethodTypeCodeInfo();
                        methodTypeCodeInfo.TargetType = extendClassType;
                        methodTypeCodeInfo.EventID = _attr.EventID;
                        methodTypeCodeInfo.EventDataType = _attr.EventDataType;
                        methodTypeCodeInfo.BehaviourType = _attr.BehaviourType;
                        methodTypeCodeInfo.Method = symMethod.MethodInfo;

                        // 函数参数类型的格式检查，仅在调试模式下执行，正式环境可跳过该处理
                        if (NovaEngine.Debugger.Instance.IsOnDebuggingVerificationActivated())
                        {
                            bool verificated = NovaEngine.Debugger.Verification.CheckGenericDelegateParameterTypeMatched(
                                                    Inspecting.CodeInspector.IsNullParameterTypeOfEventCallFunction(symMethod.MethodInfo),
                                                    symMethod.MethodInfo, methodTypeCodeInfo.TargetType);

                            if (Inspecting.CodeInspector.IsNullParameterTypeOfEventCallFunction(symMethod.MethodInfo))
                            {
                                // null parameter type, skip other check process
                            }
                            else if (methodTypeCodeInfo.EventID > 0)
                            {
                                verificated = NovaEngine.Debugger.Verification.CheckGenericDelegateParameterTypeMatched(
                                                    false == Inspecting.CodeInspector.IsNullParameterTypeOfEventCallFunction(symMethod.MethodInfo),
                                                    symMethod.MethodInfo, methodTypeCodeInfo.TargetType, typeof(int), typeof(object[]));
                            }
                            else
                            {
                                verificated = NovaEngine.Debugger.Verification.CheckGenericDelegateParameterTypeMatched(
                                                    false == Inspecting.CodeInspector.IsNullParameterTypeOfEventCallFunction(symMethod.MethodInfo),
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

                        if (false == Inspecting.CodeInspector.IsValidFormatOfProtoExtendMessageCallFunction(symMethod.MethodInfo))
                        {
                            Debugger.Warn("The extend message recv method '{0}.{1}' was invalid format, added it failed.", symClass.FullName, symMethod.MethodName);
                            continue;
                        }

                        SystemType extendClassType = symMethod.GetParameter(0).ParameterType;

                        MessageBindingMethodTypeCodeInfo methodTypeCodeInfo = new MessageBindingMethodTypeCodeInfo();
                        methodTypeCodeInfo.TargetType = extendClassType;
                        methodTypeCodeInfo.Opcode = _attr.Opcode;
                        methodTypeCodeInfo.MessageType = _attr.MessageType;
                        methodTypeCodeInfo.BehaviourType = _attr.BehaviourType;
                        methodTypeCodeInfo.Method = symMethod.MethodInfo;

                        // 函数参数类型的格式检查，仅在调试模式下执行，正式环境可跳过该处理
                        if (NovaEngine.Debugger.Instance.IsOnDebuggingVerificationActivated())
                        {
                            bool verificated = NovaEngine.Debugger.Verification.CheckGenericDelegateParameterTypeMatched(
                                                    Inspecting.CodeInspector.IsNullParameterTypeOfMessageCallFunction(symMethod.MethodInfo),
                                                    symMethod.MethodInfo, methodTypeCodeInfo.TargetType);

                            if (Inspecting.CodeInspector.IsNullParameterTypeOfMessageCallFunction(symMethod.MethodInfo))
                            {
                                // null parameter type, skip other check process
                            }
                            else if (methodTypeCodeInfo.Opcode > 0)
                            {
                                verificated = NovaEngine.Debugger.Verification.CheckGenericDelegateParameterTypeMatched(
                                                    false == Inspecting.CodeInspector.IsNullParameterTypeOfMessageCallFunction(symMethod.MethodInfo),
                                                    symMethod.MethodInfo, methodTypeCodeInfo.TargetType, typeof(ProtoBuf.Extension.IMessage));
                            }
                            else
                            {
                                verificated = NovaEngine.Debugger.Verification.CheckGenericDelegateParameterTypeMatched(
                                                    false == Inspecting.CodeInspector.IsNullParameterTypeOfMessageCallFunction(symMethod.MethodInfo),
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
                }
            }

            if (info.GetInputCallMethodTypeCount() <= 0 &&
                info.GetEventCallMethodTypeCount() <= 0 &&
                info.GetMessageCallMethodTypeCount() <= 0)
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

        [OnExtendClassCleanupOfTarget(typeof(ExtendSupportedAttribute))]
        private static void CleanupAllExtendCallClasses()
        {
            _extendCallCodeInfos.Clear();
        }

        [OnExtendCodeInfoLookupOfTarget(typeof(ExtendSupportedAttribute))]
        private static ExtendCallCodeInfo LookupExtendCallCodeInfo(Symboling.SymClass symClass)
        {
            foreach (KeyValuePair<SystemType, ExtendCallCodeInfo> pair in _extendCallCodeInfos)
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
