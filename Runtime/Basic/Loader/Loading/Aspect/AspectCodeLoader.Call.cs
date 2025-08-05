/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyright (C) 2023 - 2024, Guangzhou Shiyue Network Technology Co., Ltd.
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
using SystemDelegate = System.Delegate;
using SystemMethodInfo = System.Reflection.MethodInfo;
using SystemStringBuilder = System.Text.StringBuilder;

using SystemAction_object = System.Action<object>;
using SystemExpression_Action_object = System.Linq.Expressions.Expression<System.Action<object>>;

namespace GameEngine.Loader
{
    /// <summary>
    /// 切面调用类的结构信息
    /// </summary>
    public class AspectCallCodeInfo : AspectCodeInfo
    {
        /// <summary>
        /// 切面调用类的函数结构信息管理容器
        /// </summary>
        private IList<AspectCallMethodTypeCodeInfo> _methodTypes;

        /// <summary>
        /// 新增指定函数的回调句柄相关的结构信息
        /// </summary>
        /// <param name="codeInfo">函数的结构信息</param>
        internal void AddMethodType(AspectCallMethodTypeCodeInfo codeInfo)
        {
            if (null == _methodTypes)
            {
                _methodTypes = new List<AspectCallMethodTypeCodeInfo>();
            }

            if (_methodTypes.Contains(codeInfo))
            {
                Debugger.Warn("The aspect call class type '{0}' was already registed target method '{1}', repeat added it failed.",
                        NovaEngine.Utility.Text.ToString(_classType), codeInfo.MethodName);
                return;
            }

            _methodTypes.Add(codeInfo);
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
        internal AspectCallMethodTypeCodeInfo GetMethodType(int index)
        {
            if (null == _methodTypes || index < 0 || index >= _methodTypes.Count)
            {
                Debugger.Warn("Invalid index ({0}) for aspect call method type code info list.", index);
                return null;
            }

            return _methodTypes[index];
        }

        public override string ToString()
        {
            SystemStringBuilder sb = new SystemStringBuilder();
            sb.Append("AspectCall = { ");
            sb.AppendFormat("Parent = {0}, ", base.ToString());

            sb.AppendFormat("MethodTypes = {{{0}}}, ", NovaEngine.Utility.Text.ToString<AspectCallMethodTypeCodeInfo>(_methodTypes));

            sb.Append("}");
            return sb.ToString();
        }
    }

    /// <summary>
    /// 切面调用类的函数结构信息
    /// </summary>
    public class AspectCallMethodTypeCodeInfo
    {
        /// <summary>
        /// 切面调用类的完整名称
        /// </summary>
        private string _fullname;
        /// <summary>
        /// 切面调用类的目标对象类型
        /// </summary>
        private SystemType _targetType;
        /// <summary>
        /// 切面调用类的目标函数名称
        /// </summary>
        private string _methodName;
        /// <summary>
        /// 切面调用类的接入访问方式
        /// </summary>
        private AspectAccessType _accessType;
        /// <summary>
        /// 切面调用类的回调函数信息
        /// </summary>
        private SystemMethodInfo _methodInfo;
        /// <summary>
        /// 切面调用类的回调句柄实例
        /// </summary>
        private SystemAction_object _callback;

        public string Fullname { get { return _fullname; } internal set { _fullname = value; } }
        public SystemType TargetType { get { return _targetType; } internal set { _targetType = value; } }
        public string MethodName { get { return _methodName; } internal set { _methodName = value; } }
        public AspectAccessType AccessType { get { return _accessType; } internal set { _accessType = value; } }
        public SystemMethodInfo MethodInfo { get { return _methodInfo; } internal set { _methodInfo = value; } }
        public SystemAction_object Callback { get { return _callback; } internal set { _callback = value; } }

        public override string ToString()
        {
            SystemStringBuilder sb = new SystemStringBuilder();
            sb.Append("{ ");
            sb.AppendFormat("Fullname = {0}, ", _fullname);
            sb.AppendFormat("TargetType = {0}, ", NovaEngine.Utility.Text.ToString(_targetType));
            sb.AppendFormat("MethodName = {0}, ", _methodName ?? NovaEngine.Definition.CString.Unknown);
            sb.AppendFormat("AccessType = {0}, ", _accessType.ToString());
            sb.AppendFormat("MethodInfo = {0}, ", NovaEngine.Utility.Text.ToString(_methodInfo));
            sb.Append("}");
            return sb.ToString();
        }
    }

    /// <summary>
    /// 程序集中切面控制对象的分析处理类，对业务层载入的所有切面控制类进行统一加载及分析处理
    /// </summary>
    internal static partial class AspectCodeLoader
    {
        /// <summary>
        /// 切面调用类的结构信息管理容器
        /// </summary>
        private static IDictionary<SystemType, AspectCallCodeInfo> _aspectCallCodeInfos = new Dictionary<SystemType, AspectCallCodeInfo>();

        [OnCodeLoaderClassLoadOfTarget(typeof(AspectAttribute))]
        private static bool LoadAspectCallClass(Symboling.SymClass symClass, bool reload)
        {
            AspectCallCodeInfo info = new AspectCallCodeInfo();
            info.ClassType = symClass.ClassType;

            IList<Symboling.SymMethod> symMethods = symClass.GetAllMethods();
            for (int n = 0; null != symMethods && n < symMethods.Count; ++n)
            {
                Symboling.SymMethod symMethod = symMethods[n];

                // 非静态方法直接跳过
                if (false == symMethod.IsStatic)
                {
                    continue;
                }

                // 获取切面特性标签
                OnAspectCallAttribute aspectCallAttribute = symMethod.GetAttribute<OnAspectCallAttribute>(true);
                if (null == aspectCallAttribute)
                {
                    continue;
                }

                SystemType interruptedSource = null;
                if (symMethod.IsExtension)
                {
                    interruptedSource = symMethod.ExtensionParameterType;
                }
                else // 暂时考虑非扩展方法，但是第一个参数是服务对象类型的情况
                {
                    interruptedSource = symMethod.GetParameterType(0);
                }

                AspectCallMethodTypeCodeInfo callMethodInfo = new AspectCallMethodTypeCodeInfo();
                callMethodInfo.TargetType = interruptedSource;
                callMethodInfo.MethodName = aspectCallAttribute.MethodName;
                callMethodInfo.AccessType = aspectCallAttribute.AccessType;

                if (null == callMethodInfo.TargetType || string.IsNullOrEmpty(callMethodInfo.MethodName))
                {
                    // 未进行合法标识的函数忽略它
                    Debugger.Info(LogGroupTag.CodeLoader, "The aspect call '{0}.{1}' interrupted source and function names cannot be null, added it failed.",
                            symClass.FullName, symMethod.FullName);
                    continue;
                }

                // 检查函数格式是否合法
                if (false == Inspecting.CodeInspector.IsValidFormatOfAspectFunction(symMethod.MethodInfo))
                {
                    Debugger.Warn("The aspect call method '{0}.{1}' was invalid format, added it failed.", symClass.FullName, symMethod.FullName);
                    continue;
                }

                // 排除掉对象默认自带的其它函数，这里不进行参数检查，直接通过转换是否成功来进行判断
                callMethodInfo.Fullname = symMethod.FullName;
                // callMethodInfo.Callback = NovaEngine.Utility.Reflection.CreateGenericActionDelegate(symMethod.MethodInfo);
                callMethodInfo.MethodInfo = symMethod.MethodInfo;

                callMethodInfo.Callback = NovaEngine.Utility.Reflection.CreateGenericAction<object>(symMethod.MethodInfo, callMethodInfo.TargetType);
                if (null == callMethodInfo.Callback)
                {
                    Debugger.Warn("Cannot converted from method info '{0}' to aspect standard call, loaded this method failed.", symMethod.MethodName);
                    continue;
                }

                // 函数参数类型的格式检查，仅在调试模式下执行，正式环境可跳过该处理
                // NovaEngine.Debugger.Verification.CheckGenericDelegateParameterTypeMatched(callMethodInfo.Callback, callMethodInfo.TargetType);
                NovaEngine.Debugger.Verification.CheckGenericDelegateParameterTypeMatched(symMethod.MethodInfo, callMethodInfo.TargetType);

                // if (false == method.IsStatic)
                // { Debugger.Warn("The aspect call method '{0} - {1}' must be static type, loaded it failed.", symClass.FullName, method.Name); continue; }

                info.AddMethodType(callMethodInfo);
            }

            if (info.GetMethodTypeCount() <= 0)
            {
                Debugger.Warn("The aspect call method types count must be great than zero, newly added class '{0}' failed.", symClass.FullName);
                return false;
            }

            if (_aspectCallCodeInfos.ContainsKey(symClass.ClassType))
            {
                if (reload)
                {
                    _aspectCallCodeInfos.Remove(symClass.ClassType);
                }
                else
                {
                    Debugger.Warn("The aspect call '{0}' was already existed, repeat added it failed.", symClass.FullName);
                    return false;
                }
            }

            _aspectCallCodeInfos.Add(symClass.ClassType, info);
            Debugger.Log(LogGroupTag.CodeLoader, "Load aspect call code info '{0}' succeed from target class type '{1}'.", info.ToString(), symClass.FullName);

            return true;
        }

        [OnCodeLoaderClassCleanupOfTarget(typeof(AspectAttribute))]
        private static void CleanupAllAspectCallClasses()
        {
            _aspectCallCodeInfos.Clear();
        }

        [OnCodeLoaderClassLookupOfTarget(typeof(AspectAttribute))]
        private static AspectCallCodeInfo LookupAspectCallCodeInfo(Symboling.SymClass symCLass)
        {
            foreach (KeyValuePair<SystemType, AspectCallCodeInfo> pair in _aspectCallCodeInfos)
            {
                if (pair.Value.ClassType == symCLass.ClassType)
                {
                    return pair.Value;
                }
            }

            return null;
        }
    }
}
