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
        /// 原型对象事件订阅的扩展定义调用类的数据管理容器
        /// </summary>
        private IList<EventSubscribingMethodTypeCodeInfo> m_eventCallMethodTypes;

        /// <summary>
        /// 原型对象消息处理的扩展定义调用类的数据管理容器
        /// </summary>
        private IList<MessageBindingMethodTypeCodeInfo> m_messageCallMethodTypes;

        #region 扩展事件调用类结构信息操作函数

        /// <summary>
        /// 新增指定函数的回调句柄相关的结构信息
        /// </summary>
        /// <param name="invoke">函数的结构信息</param>
        public void AddEventCallMethodType(EventSubscribingMethodTypeCodeInfo invoke)
        {
            if (null == m_eventCallMethodTypes)
            {
                m_eventCallMethodTypes = new List<EventSubscribingMethodTypeCodeInfo>();
            }

            if (m_eventCallMethodTypes.Contains(invoke))
            {
                Debugger.Warn("The extend event call class type '{0}' was already registed target event '{1}', repeat added it failed.", m_classType.FullName, invoke.EventID);
                return;
            }

            m_eventCallMethodTypes.Add(invoke);
        }

        /// <summary>
        /// 移除所有函数的回调句柄相关的结构信息
        /// </summary>
        public void RemoveAllEventCallMethodTypes()
        {
            m_eventCallMethodTypes?.Clear();
            m_eventCallMethodTypes = null;
        }

        /// <summary>
        /// 获取当前函数回调句柄的结构信息数量
        /// </summary>
        /// <returns>返回函数回调句柄的结构信息数量</returns>
        public int GetEventCallMethodTypeCount()
        {
            if (null != m_eventCallMethodTypes)
            {
                return m_eventCallMethodTypes.Count;
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
            if (null == m_eventCallMethodTypes || index < 0 || index >= m_eventCallMethodTypes.Count)
            {
                Debugger.Warn("Invalid index ({0}) for extend event call method type code info list.", index);
                return null;
            }

            return m_eventCallMethodTypes[index];
        }

        #endregion

        #region 扩展消息调用类结构信息操作函数

        /// <summary>
        /// 新增指定函数的回调句柄相关的结构信息
        /// </summary>
        /// <param name="invoke">函数的结构信息</param>
        public void AddMessageCallMethodType(MessageBindingMethodTypeCodeInfo invoke)
        {
            if (null == m_messageCallMethodTypes)
            {
                m_messageCallMethodTypes = new List<MessageBindingMethodTypeCodeInfo>();
            }

            if (m_messageCallMethodTypes.Contains(invoke))
            {
                Debugger.Warn("The extend message call class type '{0}' was already registed target event '{1}', repeat added it failed.", m_classType.FullName, invoke.Opcode);
                return;
            }

            m_messageCallMethodTypes.Add(invoke);
        }

        /// <summary>
        /// 移除所有函数的回调句柄相关的结构信息
        /// </summary>
        public void RemoveAllMessageCallMethodTypes()
        {
            m_messageCallMethodTypes?.Clear();
            m_messageCallMethodTypes = null;
        }

        /// <summary>
        /// 获取当前函数回调句柄的结构信息数量
        /// </summary>
        /// <returns>返回函数回调句柄的结构信息数量</returns>
        public int GetMessageCallMethodTypeCount()
        {
            if (null != m_messageCallMethodTypes)
            {
                return m_messageCallMethodTypes.Count;
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
            if (null == m_messageCallMethodTypes || index < 0 || index >= m_messageCallMethodTypes.Count)
            {
                Debugger.Warn("Invalid index ({0}) for extend message call method type code info list.", index);
                return null;
            }

            return m_messageCallMethodTypes[index];
        }

        #endregion

        public override string ToString()
        {
            SystemStringBuilder sb = new SystemStringBuilder();
            sb.Append("EventCall = { ");
            sb.AppendFormat("Parent = {0}, ", base.ToString());

            sb.AppendFormat("EventCallMethodTypes = {{{0}}}, ", NovaEngine.Utility.Text.ToString<EventSubscribingMethodTypeCodeInfo>(m_eventCallMethodTypes));
            sb.AppendFormat("MessageCallMethodTypes = {{{0}}}, ", NovaEngine.Utility.Text.ToString<MessageBindingMethodTypeCodeInfo>(m_messageCallMethodTypes));

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
        private static IDictionary<SystemType, ExtendCallCodeInfo> s_extendCallCodeInfos = new Dictionary<SystemType, ExtendCallCodeInfo>();

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
                if (false == symMethod.IsStatic || false == symMethod.IsExtense)
                {
                    Debugger.Info(LogGroupTag.CodeLoader, "The extend call method '{0}.{1}' was invalid format, added it failed.", symClass.FullName, symMethod.MethodName);
                    continue;
                }

                IList<SystemAttribute> attrs = symMethod.Attributes;
                for (int m = 0; null != attrs && m < attrs.Count; ++m)
                {
                    SystemAttribute attr = attrs[m];

                    if (attr is EventSubscribeBindingOfTargetAttribute)
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

            if (info.GetEventCallMethodTypeCount() <= 0 && info.GetMessageCallMethodTypeCount() <= 0)
            {
                Debugger.Warn("The extend call method types count must be great than zero, newly added class '{0}' failed.", info.ClassType.FullName);
                return false;
            }

            if (s_extendCallCodeInfos.ContainsKey(symClass.ClassType))
            {
                if (reload)
                {
                    s_extendCallCodeInfos.Remove(symClass.ClassType);
                }
                else
                {
                    Debugger.Warn("The extend call '{0}' was already existed, repeat added it failed.", symClass.FullName);
                    return false;
                }
            }

            s_extendCallCodeInfos.Add(symClass.ClassType, info);
            Debugger.Log(LogGroupTag.CodeLoader, "Load extend call code info '{0}' succeed from target class type '{1}'.", info.ToString(), symClass.FullName);

            return true;
        }

        [OnExtendClassCleanupOfTarget(typeof(ExtendSupportedAttribute))]
        private static void CleanupAllExtendCallClasses()
        {
            s_extendCallCodeInfos.Clear();
        }

        [OnExtendCodeInfoLookupOfTarget(typeof(ExtendSupportedAttribute))]
        private static ExtendCallCodeInfo LookupExtendCallCodeInfo(Symboling.SymClass symClass)
        {
            foreach (KeyValuePair<SystemType, ExtendCallCodeInfo> pair in s_extendCallCodeInfos)
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
