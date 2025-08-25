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
        private IList<InputCallMethodTypeCodeInfo> _methodTypes;

        /// <summary>
        /// 新增指定函数的回调句柄相关的结构信息
        /// </summary>
        /// <param name="invoke">函数的结构信息</param>
        internal void AddMethodType(InputCallMethodTypeCodeInfo invoke)
        {
            if (null == _methodTypes)
            {
                _methodTypes = new List<InputCallMethodTypeCodeInfo>();
            }

            if (_methodTypes.Contains(invoke))
            {
                Debugger.Warn("The input call class type '{0}' was already registed target keycode '{1}', repeat added it failed.", _classType.FullName, invoke.InputCode);
                return;
            }

            _methodTypes.Add(invoke);
        }

        /// <summary>
        /// 移除所有函数的回调句柄相关的结构信息
        /// </summary>
        internal void RemoveAllMethodTypes()
        {
            _methodTypes?.Clear();
            _methodTypes = null;
        }

        /// <summary>
        /// 获取当前函数回调句柄的结构信息数量
        /// </summary>
        /// <returns>返回函数回调句柄的结构信息数量</returns>
        internal int GetMethodTypeCount()
        {
            if (null != _methodTypes)
            {
                return _methodTypes.Count;
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
            if (null == _methodTypes || index < 0 || index >= _methodTypes.Count)
            {
                Debugger.Warn("Invalid index ({0}) for input call method type code info list.", index);
                return null;
            }

            return _methodTypes[index];
        }

        public override string ToString()
        {
            SystemStringBuilder sb = new SystemStringBuilder();
            sb.Append("InputCall = { ");
            sb.AppendFormat("Parent = {0}, ", base.ToString());

            sb.AppendFormat("MethodTypes = {{{0}}}, ", NovaEngine.Utility.Text.ToString<InputCallMethodTypeCodeInfo>(_methodTypes));

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
        private string _fullname;
        /// <summary>
        /// 输入响应类的目标对象类型
        /// </summary>
        private SystemType _targetType;
        /// <summary>
        /// 输入响应类的监听键码标识
        /// </summary>
        private int _inputCode;
        /// <summary>
        /// 输入响应类的监听操作数据类型
        /// </summary>
        private InputOperationType _operationType;
        /// <summary>
        /// 输入响应类的监听键码集合数据类型
        /// </summary>
        private SystemType _inputDataType;
        /// <summary>
        /// 输入响应类的回调函数
        /// </summary>
        private SystemMethodInfo _method;

        public string Fullname { get { return _fullname; } internal set { _fullname = value; } }
        public SystemType TargetType { get { return _targetType; } internal set { _targetType = value; } }
        public int InputCode { get { return _inputCode; } internal set { _inputCode = value; } }
        public InputOperationType OperationType { get { return _operationType; } internal set { _operationType = value; } }
        public SystemType InputDataType { get { return _inputDataType; } internal set { _inputDataType = value; } }
        public SystemMethodInfo Method { get { return _method; } internal set { _method = value; } }

        public override string ToString()
        {
            SystemStringBuilder sb = new SystemStringBuilder();
            sb.Append("{ ");
            sb.AppendFormat("Fullname = {0}, ", _fullname);
            sb.AppendFormat("TargetType = {0}, ", NovaEngine.Utility.Text.ToString(_targetType));
            sb.AppendFormat("InputCode = {0}, ", _inputCode);
            sb.AppendFormat("OperationType = {0}, ", _operationType);
            sb.AppendFormat("InputDataType = {0}, ", NovaEngine.Utility.Text.ToString(_inputDataType));
            sb.AppendFormat("Method = {0}, ", NovaEngine.Utility.Text.ToString(_method));
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
        private static IDictionary<SystemType, InputCallCodeInfo> _inputCallCodeInfos = new Dictionary<SystemType, InputCallCodeInfo>();

        [OnCodeLoaderClassLoadOfTarget(typeof(InputSystemAttribute))]
        private static bool LoadInputCallClass(Symboling.SymClass symClass, bool reload)
        {
            InputCallCodeInfo info = new InputCallCodeInfo();
            info.ClassType = symClass.ClassType;

            IList<Symboling.SymMethod> symMethods = symClass.GetAllMethods();
            for (int n = 0; null != symMethods && n < symMethods.Count; ++n)
            {
                Symboling.SymMethod symMethod = symMethods[n];

                // 检查函数格式是否合法
                if (false == symMethod.IsStatic || false == Inspecting.CodeInspector.CheckFunctionFormatOfInputCall(symMethod.MethodInfo))
                {
                    Debugger.Info(LogGroupTag.CodeLoader, "The input call method '{0}.{1}' was invalid format, added it failed.", symClass.FullName, symMethod.MethodName);
                    continue;
                }

                IList<SystemAttribute> attrs = symMethod.Attributes;
                for (int m = 0; null != attrs && m < attrs.Count; ++m)
                {
                    SystemAttribute attr = attrs[m];

                    if (attr is OnInputDispatchCallAttribute)
                    {
                        OnInputDispatchCallAttribute _attr = (OnInputDispatchCallAttribute) attr;

                        InputCallMethodTypeCodeInfo callMethodInfo = new InputCallMethodTypeCodeInfo();
                        callMethodInfo.TargetType = _attr.ClassType;
                        callMethodInfo.InputCode = _attr.InputCode;
                        callMethodInfo.OperationType = _attr.OperationType;
                        callMethodInfo.InputDataType = _attr.InputDataType;

                        if (callMethodInfo.InputCode <= 0 && callMethodInfo.OperationType <= 0)
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
                                if (Inspecting.CodeInspector.CheckFunctionFormatOfInputCallWithNullParameterType(symMethod.MethodInfo))
                                {
                                    // 无参类型的输入响应函数
                                    verificated = NovaEngine.Debugger.Verification.CheckGenericDelegateParameterTypeMatched(symMethod.MethodInfo);
                                }
                                else if (callMethodInfo.InputCode > 0)
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
                                if (Inspecting.CodeInspector.CheckFunctionFormatOfInputCallWithNullParameterType(symMethod.MethodInfo))
                                {
                                    // 无参类型的输入响应函数
                                    verificated = NovaEngine.Debugger.Verification.CheckGenericDelegateParameterTypeMatched(symMethod.MethodInfo, callMethodInfo.TargetType);
                                }
                                else if (callMethodInfo.InputCode > 0)
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

            if (_inputCallCodeInfos.ContainsKey(symClass.ClassType))
            {
                if (reload)
                {
                    _inputCallCodeInfos.Remove(symClass.ClassType);
                }
                else
                {
                    Debugger.Warn("The input call '{0}' was already existed, repeat added it failed.", symClass.FullName);
                    return false;
                }
            }

            _inputCallCodeInfos.Add(symClass.ClassType, info);
            Debugger.Log(LogGroupTag.CodeLoader, "Load input call code info '{0}' succeed from target class type '{1}'.", info.ToString(), symClass.FullName);

            return true;
        }

        [OnCodeLoaderClassCleanupOfTarget(typeof(InputSystemAttribute))]
        private static void CleanupAllInputCallClasses()
        {
            _inputCallCodeInfos.Clear();
        }

        [OnCodeLoaderClassLookupOfTarget(typeof(InputSystemAttribute))]
        private static InputCallCodeInfo LookupInputCallCodeInfo(Symboling.SymClass symClass)
        {
            foreach (KeyValuePair<SystemType, InputCallCodeInfo> pair in _inputCallCodeInfos)
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
