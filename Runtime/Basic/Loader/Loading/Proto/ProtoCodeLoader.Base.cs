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
using SystemDelegate = System.Delegate;
using SystemMethodInfo = System.Reflection.MethodInfo;
using SystemStringBuilder = System.Text.StringBuilder;

namespace GameEngine.Loader
{
    /// <summary>
    /// 引用基类的结构信息
    /// </summary>
    public abstract class ProtoBaseCodeInfo : ProtoCodeInfo
    {
        /// <summary>
        /// 输入响应类的函数结构信息管理容器
        /// </summary>
        private IList<InputResponsingMethodTypeCodeInfo> _inputResponsingMethodTypes;

        /// <summary>
        /// 事件订阅类的函数结构信息管理容器
        /// </summary>
        private IList<EventSubscribingMethodTypeCodeInfo> _eventSubscribingMethodTypes;

        /// <summary>
        /// 消息绑定类的函数结构信息管理容器
        /// </summary>
        private IList<MessageBindingMethodTypeCodeInfo> _messageBindingMethodTypes;

        #region 输入响应类结构信息操作函数

        /// <summary>
        /// 新增指定响应输入函数的回调句柄相关的结构信息
        /// </summary>
        /// <param name="codeInfo">函数的结构信息</param>
        internal void AddInputResponsingMethodType(InputResponsingMethodTypeCodeInfo codeInfo)
        {
            if (null == _inputResponsingMethodTypes)
            {
                _inputResponsingMethodTypes = new List<InputResponsingMethodTypeCodeInfo>();
            }

            if (_inputResponsingMethodTypes.Contains(codeInfo))
            {
                Debugger.Warn("The input responsing class type '{0}' was already registed target method '{1}', repeat added it failed.",
                        _classType.FullName, NovaEngine.Utility.Text.ToString(codeInfo.Method));
                return;
            }

            _inputResponsingMethodTypes.Add(codeInfo);
        }

        /// <summary>
        /// 移除所有响应输入函数的回调句柄相关的结构信息
        /// </summary>
        internal void RemoveAllInputResponsingMethodTypes()
        {
            _inputResponsingMethodTypes?.Clear();
            _inputResponsingMethodTypes = null;
        }

        /// <summary>
        /// 获取当前响应输入函数回调句柄的结构信息数量
        /// </summary>
        /// <returns>返回函数回调句柄的结构信息数量</returns>
        internal int GetInputResponsingMethodTypeCount()
        {
            if (null != _inputResponsingMethodTypes)
            {
                return _inputResponsingMethodTypes.Count;
            }

            return 0;
        }

        /// <summary>
        /// 获取当前响应输入函数回调句柄的结构信息容器中指索引对应的实例
        /// </summary>
        /// <param name="index">索引值</param>
        /// <returns>返回给定索引值对应的实例，若不存在对应实例则返回null</returns>
        internal InputResponsingMethodTypeCodeInfo GetInputResponsingMethodType(int index)
        {
            if (null == _inputResponsingMethodTypes || index < 0 || index >= _inputResponsingMethodTypes.Count)
            {
                Debugger.Warn("Invalid index ({0}) for input responsing method type code info list.", index);
                return null;
            }

            return _inputResponsingMethodTypes[index];
        }

        #endregion

        #region 事件订阅类结构信息操作函数

        /// <summary>
        /// 新增指定订阅事件函数的回调句柄相关的结构信息
        /// </summary>
        /// <param name="codeInfo">函数的结构信息</param>
        internal void AddEventSubscribingMethodType(EventSubscribingMethodTypeCodeInfo codeInfo)
        {
            if (null == _eventSubscribingMethodTypes)
            {
                _eventSubscribingMethodTypes = new List<EventSubscribingMethodTypeCodeInfo>();
            }

            if (_eventSubscribingMethodTypes.Contains(codeInfo))
            {
                Debugger.Warn("The event subscribing class type '{0}' was already registed target method '{1}', repeat added it failed.",
                        _classType.FullName, NovaEngine.Utility.Text.ToString(codeInfo.Method));
                return;
            }

            _eventSubscribingMethodTypes.Add(codeInfo);
        }

        /// <summary>
        /// 移除所有订阅事件函数的回调句柄相关的结构信息
        /// </summary>
        internal void RemoveAllEventSubscribingMethodTypes()
        {
            _eventSubscribingMethodTypes?.Clear();
            _eventSubscribingMethodTypes = null;
        }

        /// <summary>
        /// 获取当前订阅事件函数回调句柄的结构信息数量
        /// </summary>
        /// <returns>返回函数回调句柄的结构信息数量</returns>
        internal int GetEventSubscribingMethodTypeCount()
        {
            if (null != _eventSubscribingMethodTypes)
            {
                return _eventSubscribingMethodTypes.Count;
            }

            return 0;
        }

        /// <summary>
        /// 获取当前订阅事件函数回调句柄的结构信息容器中指索引对应的实例
        /// </summary>
        /// <param name="index">索引值</param>
        /// <returns>返回给定索引值对应的实例，若不存在对应实例则返回null</returns>
        internal EventSubscribingMethodTypeCodeInfo GetEventSubscribingMethodType(int index)
        {
            if (null == _eventSubscribingMethodTypes || index < 0 || index >= _eventSubscribingMethodTypes.Count)
            {
                Debugger.Warn("Invalid index ({0}) for event subscribing method type code info list.", index);
                return null;
            }

            return _eventSubscribingMethodTypes[index];
        }

        #endregion

        #region 消息绑定类结构信息操作函数

        /// <summary>
        /// 新增指定消息绑定函数的回调句柄相关的结构信息
        /// </summary>
        /// <param name="codeInfo">函数的结构信息</param>
        internal void AddMessageBindingMethodType(MessageBindingMethodTypeCodeInfo codeInfo)
        {
            if (null == _messageBindingMethodTypes)
            {
                _messageBindingMethodTypes = new List<MessageBindingMethodTypeCodeInfo>();
            }

            if (_messageBindingMethodTypes.Contains(codeInfo))
            {
                Debugger.Warn("The event subscribing class type '{0}' was already registed target method '{1}', repeat added it failed.",
                        _classType.FullName, NovaEngine.Utility.Text.ToString(codeInfo.Method));
                return;
            }

            _messageBindingMethodTypes.Add(codeInfo);
        }

        /// <summary>
        /// 移除所有消息绑定函数的回调句柄相关的结构信息
        /// </summary>
        internal void RemoveAllMessageBindingMethodTypes()
        {
            _messageBindingMethodTypes?.Clear();
            _messageBindingMethodTypes = null;
        }

        /// <summary>
        /// 获取当前消息绑定函数回调句柄的结构信息数量
        /// </summary>
        /// <returns>返回函数回调句柄的结构信息数量</returns>
        internal int GetMessageBindingMethodTypeCount()
        {
            if (null != _messageBindingMethodTypes)
            {
                return _messageBindingMethodTypes.Count;
            }

            return 0;
        }

        /// <summary>
        /// 获取当前消息绑定函数回调句柄的结构信息容器中指索引对应的实例
        /// </summary>
        /// <param name="index">索引值</param>
        /// <returns>返回给定索引值对应的实例，若不存在对应实例则返回null</returns>
        internal MessageBindingMethodTypeCodeInfo GetMessageBindingMethodType(int index)
        {
            if (null == _messageBindingMethodTypes || index < 0 || index >= _messageBindingMethodTypes.Count)
            {
                Debugger.Warn("Invalid index ({0}) for event subscribing method type code info list.", index);
                return null;
            }

            return _messageBindingMethodTypes[index];
        }

        #endregion

        public override string ToString()
        {
            SystemStringBuilder sb = new SystemStringBuilder();
            sb.Append("{ ");
            sb.AppendFormat("Parent = {0}, ", base.ToString());

            sb.AppendFormat("InputResponsingMethodTypes = {{{0}}}, ", NovaEngine.Utility.Text.ToString<InputResponsingMethodTypeCodeInfo>(_inputResponsingMethodTypes));
            sb.AppendFormat("EventSubscribingMethodTypes = {{{0}}}, ", NovaEngine.Utility.Text.ToString<EventSubscribingMethodTypeCodeInfo>(_eventSubscribingMethodTypes));
            sb.AppendFormat("MessageBindingMethodTypes = {{{0}}}, ", NovaEngine.Utility.Text.ToString<MessageBindingMethodTypeCodeInfo>(_messageBindingMethodTypes));

            sb.Append("}");
            return sb.ToString();
        }
    }

    /// <summary>
    /// 标准输入响应函数结构信息
    /// </summary>
    public class InputResponsingMethodTypeCodeInfo : InputCallMethodTypeCodeInfo
    {
        /// <summary>
        /// 响应绑定的观察行为类型
        /// </summary>
        private AspectBehaviourType _behaviourType;

        public AspectBehaviourType BehaviourType { get { return _behaviourType; } internal set { _behaviourType = value; } }

        public override string ToString()
        {
            SystemStringBuilder sb = new SystemStringBuilder();
            sb.Append("{ ");
            sb.AppendFormat("Parent = {0}, ", base.ToString());
            sb.AppendFormat("BehaviourType = {0}, ", _behaviourType.ToString());
            sb.Append("}");
            return sb.ToString();
        }
    }

    /// <summary>
    /// 标准订阅事件函数结构信息
    /// </summary>
    public class EventSubscribingMethodTypeCodeInfo : EventCallMethodTypeCodeInfo
    {
        /// <summary>
        /// 订阅绑定的观察行为类型
        /// </summary>
        private AspectBehaviourType _behaviourType;

        public AspectBehaviourType BehaviourType { get { return _behaviourType; } internal set { _behaviourType = value; } }

        public override string ToString()
        {
            SystemStringBuilder sb = new SystemStringBuilder();
            sb.Append("{ ");
            sb.AppendFormat("Parent = {0}, ", base.ToString());
            sb.AppendFormat("BehaviourType = {0}, ", _behaviourType.ToString());
            sb.Append("}");
            return sb.ToString();
        }
    }

    /// <summary>
    /// 标准消息绑定函数结构信息
    /// </summary>
    public class MessageBindingMethodTypeCodeInfo : MessageCallMethodTypeCodeInfo
    {
        /// <summary>
        /// 消息绑定的观察行为类型
        /// </summary>
        private AspectBehaviourType _behaviourType;

        public AspectBehaviourType BehaviourType { get { return _behaviourType; } internal set { _behaviourType = value; } }

        public override string ToString()
        {
            SystemStringBuilder sb = new SystemStringBuilder();
            sb.Append("{ ");
            sb.AppendFormat("Parent = {0}, ", base.ToString());
            sb.AppendFormat("BehaviourType = {0}, ", _behaviourType.ToString());
            sb.Append("}");
            return sb.ToString();
        }
    }

    /// <summary>
    /// 程序集中原型对象的分析处理类，对业务层载入的所有原型对象类进行统一加载及分析处理
    /// </summary>
    internal static partial class ProtoCodeLoader
    {
        /// <summary>
        /// 引用对象指定类型的属性解析接口
        /// </summary>
        /// <param name="symClass">对象标记类型</param>
        /// <param name="codeInfo">对象结构信息</param>
        /// <param name="attribute">属性对象</param>
        private static void LoadBaseClassByAttributeType(Symboling.SymClass symClass, ProtoBaseCodeInfo codeInfo, SystemAttribute attribute)
        {
        }

        /// <summary>
        /// 引用对象指定函数的属性解析接口
        /// </summary>
        /// <param name="symClass">对象标记类型</param>
        /// <param name="codeInfo">对象结构信息</param>
        /// <param name="symMethod">函数对象</param>
        /// <param name="attribute">属性对象</param>
        private static void LoadBaseMethodByAttributeType(Symboling.SymClass symClass, ProtoBaseCodeInfo codeInfo, Symboling.SymMethod symMethod, SystemAttribute attribute)
        {
            if (attribute is InputResponseBindingOfTargetAttribute)
            {
                InputResponseBindingOfTargetAttribute _attr = (InputResponseBindingOfTargetAttribute) attribute;

                if (symMethod.IsStatic || false == Inspecting.CodeInspector.IsValidFormatOfInputCallFunction(symMethod.MethodInfo))
                {
                    Debugger.Warn("The input responsing method '{0}.{1}' was invalid format, added it failed.", symClass.FullName, symMethod.MethodName);
                    return;
                }

                InputResponsingMethodTypeCodeInfo methodTypeCodeInfo = new InputResponsingMethodTypeCodeInfo();
                methodTypeCodeInfo.InputCode = _attr.InputCode;
                methodTypeCodeInfo.OperationType = _attr.OperationType;
                methodTypeCodeInfo.InputDataType = _attr.InputDataType;
                methodTypeCodeInfo.BehaviourType = _attr.BehaviourType;
                methodTypeCodeInfo.Method = symMethod.MethodInfo;

                // 函数参数类型的格式检查，仅在调试模式下执行，正式环境可跳过该处理
                if (NovaEngine.Debugger.Instance.IsOnDebuggingVerificationActivated())
                {
                    bool verificated = NovaEngine.Debugger.Verification.CheckGenericDelegateParameterTypeMatched(
                                            Inspecting.CodeInspector.IsNullParameterTypeOfInputCallFunction(symMethod.MethodInfo), symMethod.MethodInfo);

                    if (Inspecting.CodeInspector.IsNullParameterTypeOfInputCallFunction(symMethod.MethodInfo))
                    {
                        // null parameter type, skip other check process
                    }
                    else if (methodTypeCodeInfo.InputCode > 0)
                    {
                        verificated = NovaEngine.Debugger.Verification.CheckGenericDelegateParameterTypeMatched(
                                            false == Inspecting.CodeInspector.IsNullParameterTypeOfInputCallFunction(symMethod.MethodInfo),
                                            symMethod.MethodInfo, typeof(int), typeof(int));
                    }
                    else
                    {
                        verificated = NovaEngine.Debugger.Verification.CheckGenericDelegateParameterTypeMatched(
                                            false == Inspecting.CodeInspector.IsNullParameterTypeOfInputCallFunction(symMethod.MethodInfo),
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

                if (symMethod.IsStatic || false == Inspecting.CodeInspector.IsValidFormatOfEventCallFunction(symMethod.MethodInfo))
                {
                    Debugger.Warn("The event subscribing method '{0}.{1}' was invalid format, added it failed.", symClass.FullName, symMethod.MethodName);
                    return;
                }

                EventSubscribingMethodTypeCodeInfo methodTypeCodeInfo = new EventSubscribingMethodTypeCodeInfo();
                methodTypeCodeInfo.EventID = _attr.EventID;
                methodTypeCodeInfo.EventDataType = _attr.EventDataType;
                methodTypeCodeInfo.BehaviourType = _attr.BehaviourType;
                methodTypeCodeInfo.Method = symMethod.MethodInfo;

                // 函数参数类型的格式检查，仅在调试模式下执行，正式环境可跳过该处理
                if (NovaEngine.Debugger.Instance.IsOnDebuggingVerificationActivated())
                {
                    bool verificated = NovaEngine.Debugger.Verification.CheckGenericDelegateParameterTypeMatched(
                                            Inspecting.CodeInspector.IsNullParameterTypeOfEventCallFunction(symMethod.MethodInfo), symMethod.MethodInfo);

                    if (Inspecting.CodeInspector.IsNullParameterTypeOfEventCallFunction(symMethod.MethodInfo))
                    {
                        // null parameter type, skip other check process
                    }
                    else if (methodTypeCodeInfo.EventID > 0)
                    {
                        verificated = NovaEngine.Debugger.Verification.CheckGenericDelegateParameterTypeMatched(
                                            false == Inspecting.CodeInspector.IsNullParameterTypeOfEventCallFunction(symMethod.MethodInfo),
                                            symMethod.MethodInfo, typeof(int), typeof(object[]));
                    }
                    else
                    {
                        verificated = NovaEngine.Debugger.Verification.CheckGenericDelegateParameterTypeMatched(
                                            false == Inspecting.CodeInspector.IsNullParameterTypeOfEventCallFunction(symMethod.MethodInfo),
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

                if (symMethod.IsStatic || false == Inspecting.CodeInspector.IsValidFormatOfMessageCallFunction(symMethod.MethodInfo))
                {
                    Debugger.Warn("The message binding method '{0}.{1}' was invalid format, added it failed.", symClass.FullName, symMethod.MethodName);
                    return;
                }

                MessageBindingMethodTypeCodeInfo methodTypeCodeInfo = new MessageBindingMethodTypeCodeInfo();
                methodTypeCodeInfo.Opcode = _attr.Opcode;
                methodTypeCodeInfo.MessageType = _attr.MessageType;
                methodTypeCodeInfo.BehaviourType = _attr.BehaviourType;
                methodTypeCodeInfo.Method = symMethod.MethodInfo;

                // 函数参数类型的格式检查，仅在调试模式下执行，正式环境可跳过该处理
                if (NovaEngine.Debugger.Instance.IsOnDebuggingVerificationActivated())
                {
                    bool verificated = NovaEngine.Debugger.Verification.CheckGenericDelegateParameterTypeMatched(
                                            Inspecting.CodeInspector.IsNullParameterTypeOfMessageCallFunction(symMethod.MethodInfo), symMethod.MethodInfo);

                    if (Inspecting.CodeInspector.IsNullParameterTypeOfMessageCallFunction(symMethod.MethodInfo))
                    {
                        // null parameter type, skip other check process
                    }
                    else if (methodTypeCodeInfo.Opcode > 0)
                    {
                        verificated = NovaEngine.Debugger.Verification.CheckGenericDelegateParameterTypeMatched(
                                            false == Inspecting.CodeInspector.IsNullParameterTypeOfMessageCallFunction(symMethod.MethodInfo),
                                            symMethod.MethodInfo, typeof(ProtoBuf.Extension.IMessage));
                    }
                    else
                    {
                        verificated = NovaEngine.Debugger.Verification.CheckGenericDelegateParameterTypeMatched(
                                            false == Inspecting.CodeInspector.IsNullParameterTypeOfMessageCallFunction(symMethod.MethodInfo),
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
