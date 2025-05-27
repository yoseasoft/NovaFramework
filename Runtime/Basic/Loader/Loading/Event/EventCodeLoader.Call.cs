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
using SystemMethodInfo = System.Reflection.MethodInfo;
using SystemStringBuilder = System.Text.StringBuilder;

namespace GameEngine.Loader
{
    /// <summary>
    /// 事件调用类的结构信息
    /// </summary>
    public class EventCallCodeInfo : EventCodeInfo
    {
        /// <summary>
        /// 事件调用类的数据引用对象
        /// </summary>
        private IList<EventCallMethodTypeCodeInfo> m_methodTypes;

        /// <summary>
        /// 新增指定函数的回调句柄相关的结构信息
        /// </summary>
        /// <param name="invoke">函数的结构信息</param>
        internal void AddMethodType(EventCallMethodTypeCodeInfo invoke)
        {
            if (null == m_methodTypes)
            {
                m_methodTypes = new List<EventCallMethodTypeCodeInfo>();
            }

            if (m_methodTypes.Contains(invoke))
            {
                Debugger.Warn("The event call class type '{0}' was already registed target event '{1}', repeat added it failed.", m_classType.FullName, invoke.EventID);
                return;
            }

            m_methodTypes.Add(invoke);
        }

        /// <summary>
        /// 移除所有函数的回调句柄相关的结构信息
        /// </summary>
        internal void RemoveAllMethodTypes()
        {
            m_methodTypes?.Clear();
            m_methodTypes = null;
        }

        /// <summary>
        /// 获取当前函数回调句柄的结构信息数量
        /// </summary>
        /// <returns>返回函数回调句柄的结构信息数量</returns>
        internal int GetMethodTypeCount()
        {
            if (null != m_methodTypes)
            {
                return m_methodTypes.Count;
            }

            return 0;
        }

        /// <summary>
        /// 获取当前函数回调句柄的结构信息容器中指索引对应的实例
        /// </summary>
        /// <param name="index">索引值</param>
        /// <returns>返回给定索引值对应的实例，若不存在对应实例则返回null</returns>
        internal EventCallMethodTypeCodeInfo GetMethodType(int index)
        {
            if (null == m_methodTypes || index < 0 || index >= m_methodTypes.Count)
            {
                Debugger.Warn("Invalid index ({0}) for event call method type code info list.", index);
                return null;
            }

            return m_methodTypes[index];
        }

        public override string ToString()
        {
            SystemStringBuilder sb = new SystemStringBuilder();
            sb.Append("EventCall = { ");
            sb.AppendFormat("Parent = {0}, ", base.ToString());

            sb.AppendFormat("MethodTypes = {{{0}}}, ", NovaEngine.Utility.Text.ToString<EventCallMethodTypeCodeInfo>(m_methodTypes));

            sb.Append("}");
            return sb.ToString();
        }
    }

    /// <summary>
    /// 事件调用类的函数结构信息
    /// </summary>
    public class EventCallMethodTypeCodeInfo
    {
        /// <summary>
        /// 事件调用类的完整名称
        /// </summary>
        private string m_fullname;
        /// <summary>
        /// 事件调用类的目标对象类型
        /// </summary>
        private SystemType m_targetType;
        /// <summary>
        /// 事件调用类的监听事件标识
        /// </summary>
        private int m_eventID;
        /// <summary>
        /// 事件调用类的监听事件数据类型
        /// </summary>
        private SystemType m_eventDataType;
        /// <summary>
        /// 事件调用类的回调函数
        /// </summary>
        private SystemMethodInfo m_method;

        public string Fullname { get { return m_fullname; } internal set { m_fullname = value; } }
        public SystemType TargetType { get { return m_targetType; } internal set { m_targetType = value; } }
        public int EventID { get { return m_eventID; } internal set { m_eventID = value; } }
        public SystemType EventDataType { get { return m_eventDataType; } internal set { m_eventDataType = value; } }
        public SystemMethodInfo Method { get { return m_method; } internal set { m_method = value; } }

        public override string ToString()
        {
            SystemStringBuilder sb = new SystemStringBuilder();
            sb.Append("{ ");
            sb.AppendFormat("Fullname = {0}, ", m_fullname);
            sb.AppendFormat("TargetType = {0}, ", NovaEngine.Utility.Text.ToString(m_targetType));
            sb.AppendFormat("EventID = {0}, ", m_eventID);
            sb.AppendFormat("EventDataType = {0}, ", NovaEngine.Utility.Text.ToString(m_eventDataType));
            sb.AppendFormat("Method = {0}, ", NovaEngine.Utility.Text.ToString(m_method));
            sb.Append("}");
            return sb.ToString();
        }
    }

    /// <summary>
    /// 消息事件分发调度对象的分析处理类，对业务层载入的所有消息事件调度类进行统一加载及分析处理
    /// </summary>
    internal static partial class EventCodeLoader
    {
        /// <summary>
        /// 事件调用类的结构信息管理容器
        /// </summary>
        private static IDictionary<SystemType, EventCallCodeInfo> s_eventCallCodeInfos = new Dictionary<SystemType, EventCallCodeInfo>();

        [OnEventClassLoadOfTarget(typeof(EventSystemAttribute))]
        private static bool LoadEventCallClass(Symboling.SymClass symClass, bool reload)
        {
            EventCallCodeInfo info = new EventCallCodeInfo();
            info.ClassType = symClass.ClassType;

            IList<Symboling.SymMethod> symMethods = symClass.GetAllMethods();
            for (int n = 0; null != symMethods && n < symMethods.Count; ++n)
            {
                Symboling.SymMethod symMethod = symMethods[n];

                // 检查函数格式是否合法
                if (false == symMethod.IsStatic || false == Inspecting.CodeInspector.IsValidFormatOfEventCallFunction(symMethod.MethodInfo))
                {
                    Debugger.Info(LogGroupTag.CodeLoader, "The event call method '{0}.{1}' was invalid format, added it failed.", symClass.FullName, symMethod.MethodName);
                    continue;
                }

                EventCallMethodTypeCodeInfo callMethodInfo = new EventCallMethodTypeCodeInfo();

                IList<SystemAttribute> attrs = symMethod.Attributes;
                for (int m = 0; null != attrs && m < attrs.Count; ++m)
                {
                    SystemAttribute attr = attrs[m];

                    if (attr is OnEventDispatchCallAttribute)
                    {
                        OnEventDispatchCallAttribute _attr = (OnEventDispatchCallAttribute) attr;

                        callMethodInfo.TargetType = _attr.ClassType;
                        callMethodInfo.EventID = _attr.EventID;
                        callMethodInfo.EventDataType = _attr.EventDataType;
                    }
                }

                if (callMethodInfo.EventID <= 0 && null == callMethodInfo.EventDataType)
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
                        if (Inspecting.CodeInspector.IsNullParameterTypeOfEventCallFunction(symMethod.MethodInfo))
                        {
                            // 无参类型的事件函数
                            verificated = NovaEngine.Debugger.Verification.CheckGenericDelegateParameterTypeMatched(symMethod.MethodInfo);
                        }
                        else if (callMethodInfo.EventID > 0)
                        {
                            // 事件ID派发
                            verificated = NovaEngine.Debugger.Verification.CheckGenericDelegateParameterTypeMatched(symMethod.MethodInfo, typeof(int), typeof(object[]));
                        }
                        else
                        {
                            // 事件数据派发
                            verificated = NovaEngine.Debugger.Verification.CheckGenericDelegateParameterTypeMatched(symMethod.MethodInfo, callMethodInfo.EventDataType);
                        }
                    }
                    else
                    {
                        if (Inspecting.CodeInspector.IsNullParameterTypeOfEventCallFunction(symMethod.MethodInfo))
                        {
                            // 无参类型的事件函数
                            verificated = NovaEngine.Debugger.Verification.CheckGenericDelegateParameterTypeMatched(symMethod.MethodInfo, callMethodInfo.TargetType);
                        }
                        else if (callMethodInfo.EventID > 0)
                        {
                            // 事件ID派发
                            verificated = NovaEngine.Debugger.Verification.CheckGenericDelegateParameterTypeMatched(symMethod.MethodInfo, callMethodInfo.TargetType, typeof(int), typeof(object[]));
                        }
                        else
                        {
                            // 事件数据派发
                            verificated = NovaEngine.Debugger.Verification.CheckGenericDelegateParameterTypeMatched(symMethod.MethodInfo, callMethodInfo.TargetType, callMethodInfo.EventDataType);
                        }
                    }

                    // 校验失败
                    if (false == verificated)
                    {
                        Debugger.Error("Cannot verificated from method info '{0}' to event standard call, loaded this method failed.", symMethod.FullName);
                        continue;
                    }
                }

                // if (false == method.IsStatic)
                // { Debugger.Warn("The event call method '{0} - {1}' must be static type, loaded it failed.", symClass.FullName, symMethod.MethodName); continue; }

                info.AddMethodType(callMethodInfo);
            }

            if (info.GetMethodTypeCount() <= 0)
            {
                Debugger.Warn("The event call method types count must be great than zero, newly added class '{0}' failed.", info.ClassType.FullName);
                return false;
            }

            if (s_eventCallCodeInfos.ContainsKey(symClass.ClassType))
            {
                if (reload)
                {
                    s_eventCallCodeInfos.Remove(symClass.ClassType);
                }
                else
                {
                    Debugger.Warn("The event call '{0}' was already existed, repeat added it failed.", symClass.FullName);
                    return false;
                }
            }

            s_eventCallCodeInfos.Add(symClass.ClassType, info);
            Debugger.Log(LogGroupTag.CodeLoader, "Load event call code info '{0}' succeed from target class type '{1}'.", info.ToString(), symClass.FullName);

            return true;
        }

        [OnEventClassCleanupOfTarget(typeof(EventSystemAttribute))]
        private static void CleanupAllEventCallClasses()
        {
            s_eventCallCodeInfos.Clear();
        }

        [OnEventCodeInfoLookupOfTarget(typeof(EventSystemAttribute))]
        private static EventCallCodeInfo LookupEventCallCodeInfo(Symboling.SymClass symClass)
        {
            foreach (KeyValuePair<SystemType, EventCallCodeInfo> pair in s_eventCallCodeInfos)
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
