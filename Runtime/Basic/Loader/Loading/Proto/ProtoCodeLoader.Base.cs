/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyring (C) 2023 - 2024, Guangzhou Shiyue Network Technology Co., Ltd.
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
        /// 事件订阅类的函数结构信息管理容器
        /// </summary>
        private IList<EventSubscribingMethodTypeCodeInfo> m_eventSubscribingMethodTypes;

        /// <summary>
        /// 消息绑定类的函数结构信息管理容器
        /// </summary>
        private IList<MessageBindingMethodTypeCodeInfo> m_messageBindingMethodTypes;

        #region 事件订阅类结构信息操作函数

        /// <summary>
        /// 新增指定订阅事件函数的回调句柄相关的结构信息
        /// </summary>
        /// <param name="codeInfo">函数的结构信息</param>
        internal void AddEventSubscribingMethodType(EventSubscribingMethodTypeCodeInfo codeInfo)
        {
            if (null == m_eventSubscribingMethodTypes)
            {
                m_eventSubscribingMethodTypes = new List<EventSubscribingMethodTypeCodeInfo>();
            }

            if (m_eventSubscribingMethodTypes.Contains(codeInfo))
            {
                Debugger.Warn("The event subscribing class type '{0}' was already registed target method '{1}', repeat added it failed.",
                        m_classType.FullName, NovaEngine.Utility.Text.ToString(codeInfo.Method));
                return;
            }

            m_eventSubscribingMethodTypes.Add(codeInfo);
        }

        /// <summary>
        /// 移除所有订阅事件函数的回调句柄相关的结构信息
        /// </summary>
        internal void RemoveAllEventSubscribingMethodTypes()
        {
            m_eventSubscribingMethodTypes?.Clear();
            m_eventSubscribingMethodTypes = null;
        }

        /// <summary>
        /// 获取当前订阅事件函数回调句柄的结构信息数量
        /// </summary>
        /// <returns>返回函数回调句柄的结构信息数量</returns>
        internal int GetEventSubscribingMethodTypeCount()
        {
            if (null != m_eventSubscribingMethodTypes)
            {
                return m_eventSubscribingMethodTypes.Count;
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
            if (null == m_eventSubscribingMethodTypes || index < 0 || index >= m_eventSubscribingMethodTypes.Count)
            {
                Debugger.Warn("Invalid index ({0}) for event subscribing method type code info list.", index);
                return null;
            }

            return m_eventSubscribingMethodTypes[index];
        }

        #endregion

        #region 消息绑定类结构信息操作函数

        /// <summary>
        /// 新增指定消息绑定函数的回调句柄相关的结构信息
        /// </summary>
        /// <param name="codeInfo">函数的结构信息</param>
        internal void AddMessageBindingMethodType(MessageBindingMethodTypeCodeInfo codeInfo)
        {
            if (null == m_messageBindingMethodTypes)
            {
                m_messageBindingMethodTypes = new List<MessageBindingMethodTypeCodeInfo>();
            }

            if (m_messageBindingMethodTypes.Contains(codeInfo))
            {
                Debugger.Warn("The event subscribing class type '{0}' was already registed target method '{1}', repeat added it failed.",
                        m_classType.FullName, NovaEngine.Utility.Text.ToString(codeInfo.Method));
                return;
            }

            m_messageBindingMethodTypes.Add(codeInfo);
        }

        /// <summary>
        /// 移除所有消息绑定函数的回调句柄相关的结构信息
        /// </summary>
        internal void RemoveAllMessageBindingMethodTypes()
        {
            m_messageBindingMethodTypes?.Clear();
            m_messageBindingMethodTypes = null;
        }

        /// <summary>
        /// 获取当前消息绑定函数回调句柄的结构信息数量
        /// </summary>
        /// <returns>返回函数回调句柄的结构信息数量</returns>
        internal int GetMessageBindingMethodTypeCount()
        {
            if (null != m_messageBindingMethodTypes)
            {
                return m_messageBindingMethodTypes.Count;
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
            if (null == m_messageBindingMethodTypes || index < 0 || index >= m_messageBindingMethodTypes.Count)
            {
                Debugger.Warn("Invalid index ({0}) for event subscribing method type code info list.", index);
                return null;
            }

            return m_messageBindingMethodTypes[index];
        }

        #endregion

        public override string ToString()
        {
            SystemStringBuilder sb = new SystemStringBuilder();
            sb.Append("{ ");
            sb.AppendFormat("Parent = {0}, ", base.ToString());

            sb.AppendFormat("EventSubscribingMethodTypes = {{{0}}}, ", NovaEngine.Utility.Text.ToString<EventSubscribingMethodTypeCodeInfo>(m_eventSubscribingMethodTypes));
            sb.AppendFormat("MessageBindingMethodTypes = {{{0}}}, ", NovaEngine.Utility.Text.ToString<MessageBindingMethodTypeCodeInfo>(m_messageBindingMethodTypes));

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
        private AspectBehaviourType m_behaviourType;

        public AspectBehaviourType BehaviourType { get { return m_behaviourType; } internal set { m_behaviourType = value; } }

        public override string ToString()
        {
            SystemStringBuilder sb = new SystemStringBuilder();
            sb.Append("{ ");
            sb.AppendFormat("Parent = {0}, ", base.ToString());
            sb.AppendFormat("BehaviourType = {0}, ", m_behaviourType.ToString());
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
        private AspectBehaviourType m_behaviourType;

        public AspectBehaviourType BehaviourType { get { return m_behaviourType; } internal set { m_behaviourType = value; } }

        public override string ToString()
        {
            SystemStringBuilder sb = new SystemStringBuilder();
            sb.Append("{ ");
            sb.AppendFormat("Parent = {0}, ", base.ToString());
            sb.AppendFormat("BehaviourType = {0}, ", m_behaviourType.ToString());
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
            if (attribute is EventSubscribeBindingOfTargetAttribute)
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
