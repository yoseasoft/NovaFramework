/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
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
using SystemMethodInfo = System.Reflection.MethodInfo;
using SystemStringBuilder = System.Text.StringBuilder;

namespace GameEngine.Loader
{
    /// <summary>
    /// 输入响应类的结构信息
    /// </summary>
    public class InputCallCodeInfo : InputCodeInfo
    {
        /// <summary>
        /// 事件调用类的数据引用对象
        /// </summary>
        private IList<InputCallMethodTypeCodeInfo> m_methodTypes;

        /// <summary>
        /// 新增指定函数的回调句柄相关的结构信息
        /// </summary>
        /// <param name="invoke">函数的结构信息</param>
        internal void AddMethodType(InputCallMethodTypeCodeInfo invoke)
        {
            if (null == m_methodTypes)
            {
                m_methodTypes = new List<InputCallMethodTypeCodeInfo>();
            }

            if (m_methodTypes.Contains(invoke))
            {
                Debugger.Warn("The input call class type '{0}' was already registed target keycode '{1}', repeat added it failed.", m_classType.FullName, invoke.Keycode);
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
        internal InputCallMethodTypeCodeInfo GetMethodType(int index)
        {
            if (null == m_methodTypes || index < 0 || index >= m_methodTypes.Count)
            {
                Debugger.Warn("Invalid index ({0}) for input call method type code info list.", index);
                return null;
            }

            return m_methodTypes[index];
        }

        public override string ToString()
        {
            SystemStringBuilder sb = new SystemStringBuilder();
            sb.Append("InputCall = { ");
            sb.AppendFormat("Parent = {0}, ", base.ToString());

            sb.AppendFormat("MethodTypes = {{{0}}}, ", NovaEngine.Utility.Text.ToString<InputCallMethodTypeCodeInfo>(m_methodTypes));

            sb.Append("}");
            return sb.ToString();
        }
    }

    /// <summary>
    /// 输入响应类的函数结构信息
    /// </summary>
    public class InputCallMethodTypeCodeInfo
    {
        /// <summary>
        /// 输入响应类的完整名称
        /// </summary>
        private string m_fullname;
        /// <summary>
        /// 输入响应类的目标对象类型
        /// </summary>
        private SystemType m_targetType;
        /// <summary>
        /// 输入响应类的监听键码标识
        /// </summary>
        private int m_keycode;
        /// <summary>
        /// 输入响应类的监听操作数据类型
        /// </summary>
        private int m_operationType;
        /// <summary>
        /// 输入响应类的监听键码集合数据类型
        /// </summary>
        private SystemType m_inputDataType;
        /// <summary>
        /// 输入响应类的回调函数
        /// </summary>
        private SystemMethodInfo m_method;

        public string Fullname { get { return m_fullname; } internal set { m_fullname = value; } }
        public SystemType TargetType { get { return m_targetType; } internal set { m_targetType = value; } }
        public int Keycode { get { return m_keycode; } internal set { m_keycode = value; } }
        public int OperationType { get { return m_operationType; } internal set { m_operationType = value; } }
        public SystemType InputDataType { get { return m_inputDataType; } internal set { m_inputDataType = value; } }
        public SystemMethodInfo Method { get { return m_method; } internal set { m_method = value; } }

        public override string ToString()
        {
            SystemStringBuilder sb = new SystemStringBuilder();
            sb.Append("{ ");
            sb.AppendFormat("Fullname = {0}, ", m_fullname);
            sb.AppendFormat("TargetType = {0}, ", NovaEngine.Utility.Text.ToString(m_targetType));
            sb.AppendFormat("Keycode = {0}, ", m_keycode);
            sb.AppendFormat("OperationType = {0}, ", m_operationType);
            sb.AppendFormat("InputDataType = {0}, ", NovaEngine.Utility.Text.ToString(m_inputDataType));
            sb.AppendFormat("Method = {0}, ", NovaEngine.Utility.Text.ToString(m_method));
            sb.Append("}");
            return sb.ToString();
        }
    }

    /// <summary>
    /// 输入响应分发调度对象的分析处理类，对业务层载入的所有输入响应调度类进行统一加载及分析处理
    /// </summary>
    internal static partial class InputCodeLoader
    {
        /// <summary>
        /// 输入响应类的结构信息管理容器
        /// </summary>
        private static IDictionary<SystemType, InputCallCodeInfo> s_inputCallCodeInfos = new Dictionary<SystemType, InputCallCodeInfo>();

        [OnInputClassLoadOfTarget(typeof(KeycodeSystemAttribute))]
        private static bool LoadInputCallClass(Symboling.SymClass symClass, bool reload)
        {
            InputCallCodeInfo info = new InputCallCodeInfo();
            info.ClassType = symClass.ClassType;

            IList<Symboling.SymMethod> symMethods = symClass.GetAllMethods();
            for (int n = 0; null != symMethods && n < symMethods.Count; ++n)
            {
                Symboling.SymMethod symMethod = symMethods[n];

                // 检查函数格式是否合法
                if (false == symMethod.IsStatic || false == Inspecting.CodeInspector.IsValidFormatOfInputCallFunction(symMethod.MethodInfo))
                {
                    Debugger.Info(LogGroupTag.CodeLoader, "The input call method '{0}.{1}' was invalid format, added it failed.", symClass.FullName, symMethod.MethodName);
                    continue;
                }

                IList<SystemAttribute> attrs = symMethod.Attributes;
                for (int m = 0; null != attrs && m < attrs.Count; ++m)
                {
                    SystemAttribute attr = attrs[m];

                    if (attr is OnKeycodeDispatchCallAttribute)
                    {
                        OnKeycodeDispatchCallAttribute _attr = (OnKeycodeDispatchCallAttribute) attr;

                        InputCallMethodTypeCodeInfo callMethodInfo = new InputCallMethodTypeCodeInfo();
                        callMethodInfo.TargetType = _attr.ClassType;
                        callMethodInfo.Keycode = _attr.Keycode;
                        callMethodInfo.OperationType = (int) _attr.OperationType;
                        callMethodInfo.InputDataType = _attr.InputDataType;

                        if (callMethodInfo.Keycode <= 0 && callMethodInfo.OperationType <= 0)
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
                                if (Inspecting.CodeInspector.IsNullParameterTypeOfInputCallFunction(symMethod.MethodInfo))
                                {
                                    // 无参类型的输入响应函数
                                    verificated = NovaEngine.Debugger.Verification.CheckGenericDelegateParameterTypeMatched(symMethod.MethodInfo);
                                }
                                else if (callMethodInfo.Keycode > 0)
                                {
                                    // 输入键码和操作类型派发
                                    verificated = NovaEngine.Debugger.Verification.CheckGenericDelegateParameterTypeMatched(symMethod.MethodInfo, typeof(int), typeof(int));
                                }
                                else
                                {
                                    // 输入键码集合数据派发
                                    verificated = NovaEngine.Debugger.Verification.CheckGenericDelegateParameterTypeMatched(symMethod.MethodInfo, callMethodInfo.InputDataType);
                                }
                            }
                            else
                            {
                                if (Inspecting.CodeInspector.IsNullParameterTypeOfInputCallFunction(symMethod.MethodInfo))
                                {
                                    // 无参类型的输入响应函数
                                    verificated = NovaEngine.Debugger.Verification.CheckGenericDelegateParameterTypeMatched(symMethod.MethodInfo, callMethodInfo.TargetType);
                                }
                                else if (callMethodInfo.Keycode > 0)
                                {
                                    // 输入键码和操作类型派发
                                    verificated = NovaEngine.Debugger.Verification.CheckGenericDelegateParameterTypeMatched(symMethod.MethodInfo, callMethodInfo.TargetType, typeof(int), typeof(int));
                                }
                                else
                                {
                                    // 输入键码集合数据派发
                                    verificated = NovaEngine.Debugger.Verification.CheckGenericDelegateParameterTypeMatched(symMethod.MethodInfo, callMethodInfo.TargetType, callMethodInfo.InputDataType);
                                }
                            }

                            // 校验失败
                            if (false == verificated)
                            {
                                Debugger.Error("Cannot verificated from method info '{0}' to input listener call, loaded this method failed.", symMethod.FullName);
                                continue;
                            }
                        }

                        // if (false == method.IsStatic)
                        // { Debugger.Warn("The input call method '{0} - {1}' must be static type, loaded it failed.", symClass.FullName, symMethod.MethodName); continue; }

                        info.AddMethodType(callMethodInfo);
                    }
                }
            }

            if (info.GetMethodTypeCount() <= 0)
            {
                Debugger.Warn("The input call method types count must be great than zero, newly added class '{0}' failed.", info.ClassType.FullName);
                return false;
            }

            if (s_inputCallCodeInfos.ContainsKey(symClass.ClassType))
            {
                if (reload)
                {
                    s_inputCallCodeInfos.Remove(symClass.ClassType);
                }
                else
                {
                    Debugger.Warn("The input call '{0}' was already existed, repeat added it failed.", symClass.FullName);
                    return false;
                }
            }

            s_inputCallCodeInfos.Add(symClass.ClassType, info);
            Debugger.Log(LogGroupTag.CodeLoader, "Load input call code info '{0}' succeed from target class type '{1}'.", info.ToString(), symClass.FullName);

            return true;
        }

        [OnInputClassCleanupOfTarget(typeof(KeycodeSystemAttribute))]
        private static void CleanupAllInputCallClasses()
        {
            s_inputCallCodeInfos.Clear();
        }

        [OnInputCodeInfoLookupOfTarget(typeof(KeycodeSystemAttribute))]
        private static InputCallCodeInfo LookupInputCallCodeInfo(Symboling.SymClass symClass)
        {
            foreach (KeyValuePair<SystemType, InputCallCodeInfo> pair in s_inputCallCodeInfos)
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
