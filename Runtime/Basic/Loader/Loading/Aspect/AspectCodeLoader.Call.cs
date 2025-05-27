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
using SystemDelegate = System.Delegate;
using SystemMethodInfo = System.Reflection.MethodInfo;
using SystemStringBuilder = System.Text.StringBuilder;

using SystemExpression = System.Linq.Expressions.Expression;
using SystemParameterExpression = System.Linq.Expressions.ParameterExpression;
using SystemUnaryExpression = System.Linq.Expressions.UnaryExpression;
using SystemMethodCallExpression = System.Linq.Expressions.MethodCallExpression;

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
        private IList<AspectCallMethodTypeCodeInfo> m_methodTypes;

        /// <summary>
        /// 新增指定函数的回调句柄相关的结构信息
        /// </summary>
        /// <param name="codeInfo">函数的结构信息</param>
        internal void AddMethodType(AspectCallMethodTypeCodeInfo codeInfo)
        {
            if (null == m_methodTypes)
            {
                m_methodTypes = new List<AspectCallMethodTypeCodeInfo>();
            }

            if (m_methodTypes.Contains(codeInfo))
            {
                Debugger.Warn("The aspect call class type '{0}' was already registed target method '{1}', repeat added it failed.",
                        NovaEngine.Utility.Text.ToString(m_classType), codeInfo.MethodName);
                return;
            }

            m_methodTypes.Add(codeInfo);
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
        internal AspectCallMethodTypeCodeInfo GetMethodType(int index)
        {
            if (null == m_methodTypes || index < 0 || index >= m_methodTypes.Count)
            {
                Debugger.Warn("Invalid index ({0}) for aspect call method type code info list.", index);
                return null;
            }

            return m_methodTypes[index];
        }

        public override string ToString()
        {
            SystemStringBuilder sb = new SystemStringBuilder();
            sb.Append("AspectCall = { ");
            sb.AppendFormat("Parent = {0}, ", base.ToString());

            sb.AppendFormat("MethodTypes = {{{0}}}, ", NovaEngine.Utility.Text.ToString<AspectCallMethodTypeCodeInfo>(m_methodTypes));

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
        private string m_fullname;
        /// <summary>
        /// 切面调用类的目标对象类型
        /// </summary>
        private SystemType m_targetType;
        /// <summary>
        /// 切面调用类的目标函数名称
        /// </summary>
        private string m_methodName;
        /// <summary>
        /// 切面调用类的接入访问方式
        /// </summary>
        private AspectAccessType m_accessType;
        /// <summary>
        /// 切面调用类的回调函数信息
        /// </summary>
        private SystemMethodInfo m_methodInfo;
        /// <summary>
        /// 切面调用类的回调句柄实例
        /// </summary>
        private SystemAction_object m_callback;

        public string Fullname { get { return m_fullname; } internal set { m_fullname = value; } }
        public SystemType TargetType { get { return m_targetType; } internal set { m_targetType = value; } }
        public string MethodName { get { return m_methodName; } internal set { m_methodName = value; } }
        public AspectAccessType AccessType { get { return m_accessType; } internal set { m_accessType = value; } }
        public SystemMethodInfo MethodInfo { get { return m_methodInfo; } internal set { m_methodInfo = value; } }
        public SystemAction_object Callback { get { return m_callback; } internal set { m_callback = value; } }

        public override string ToString()
        {
            SystemStringBuilder sb = new SystemStringBuilder();
            sb.Append("{ ");
            sb.AppendFormat("Fullname = {0}, ", m_fullname);
            sb.AppendFormat("TargetType = {0}, ", NovaEngine.Utility.Text.ToString(m_targetType));
            sb.AppendFormat("MethodName = {0}, ", m_methodName ?? NovaEngine.Definition.CString.Unknown);
            sb.AppendFormat("AccessType = {0}, ", m_accessType.ToString());
            sb.AppendFormat("MethodInfo = {0}, ", NovaEngine.Utility.Text.ToString(m_methodInfo));
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
        private static IDictionary<SystemType, AspectCallCodeInfo> s_aspectCallCodeInfos = new Dictionary<SystemType, AspectCallCodeInfo>();

        [OnAspectClassLoadOfTarget(typeof(AspectAttribute))]
        private static bool LoadAspectCallClass(Symboling.SymClass symClass, bool reload)
        {
            AspectCallCodeInfo info = new AspectCallCodeInfo();
            info.ClassType = symClass.ClassType;

            SystemType interruptedSource = null;
            if (symClass.IsClass && false == symClass.IsAbstract)
            {
                // 为对象类，且非抽象类或静态类
                interruptedSource = symClass.ClassType;
            }

            AspectOfTargetAttribute aspectOfTargetAttr = symClass.GetAttribute<AspectOfTargetAttribute>(); ;
            if (null != aspectOfTargetAttr)
            {
                interruptedSource = aspectOfTargetAttr.ClassType;
            }

            IList<Symboling.SymMethod> symMethods = symClass.GetAllMethods();
            for (int n = 0; null != symMethods && n < symMethods.Count; ++n)
            {
                Symboling.SymMethod symMethod = symMethods[n];

                // 非静态方法直接跳过
                if (false == symMethod.IsStatic)
                {
                    continue;
                }

                AspectCallMethodTypeCodeInfo callMethodInfo = new AspectCallMethodTypeCodeInfo();

                IList<SystemAttribute> attrs = symMethod.Attributes;
                for (int m = 0; null != attrs && m < attrs.Count; ++m)
                {
                    SystemAttribute attr = attrs[m];
                    if (attr is OnAspectCallAttribute)
                    {
                        OnAspectCallAttribute _attr = (OnAspectCallAttribute) attr;

                        callMethodInfo.TargetType = interruptedSource;
                        callMethodInfo.MethodName = _attr.MethodName;
                        callMethodInfo.AccessType = _attr.AccessType;
                    }
                    else if (attr is OnAspectCallOfTargetAttribute)
                    {
                        OnAspectCallOfTargetAttribute _attr = (OnAspectCallOfTargetAttribute) attr;

                        callMethodInfo.TargetType = _attr.ClassType;
                        callMethodInfo.MethodName = _attr.MethodName;
                        callMethodInfo.AccessType = _attr.AccessType;
                    }
                }

                // 当设置了全局的切面观察源后，函数不可再单独设置自己的观察目标
                if (null != callMethodInfo.TargetType && null != interruptedSource && interruptedSource != callMethodInfo.TargetType)
                {
                    Debugger.Warn("The aspect call '{0}.{1}' interrupted source was setting repeatedly, added it failed.", symClass.FullName, symMethod.MethodName);
                    continue;
                }

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

            if (s_aspectCallCodeInfos.ContainsKey(symClass.ClassType))
            {
                if (reload)
                {
                    s_aspectCallCodeInfos.Remove(symClass.ClassType);
                }
                else
                {
                    Debugger.Warn("The aspect call '{0}' was already existed, repeat added it failed.", symClass.FullName);
                    return false;
                }
            }

            s_aspectCallCodeInfos.Add(symClass.ClassType, info);
            Debugger.Log(LogGroupTag.CodeLoader, "Load aspect call code info '{0}' succeed from target class type '{1}'.", info.ToString(), symClass.FullName);

            return true;
        }

        [OnAspectClassCleanupOfTarget(typeof(AspectAttribute))]
        private static void CleanupAllAspectCallClasses()
        {
            s_aspectCallCodeInfos.Clear();
        }

        [OnAspectCodeInfoLookupOfTarget(typeof(AspectAttribute))]
        private static AspectCallCodeInfo LookupAspectCallCodeInfo(Symboling.SymClass symCLass)
        {
            foreach (KeyValuePair<SystemType, AspectCallCodeInfo> pair in s_aspectCallCodeInfos)
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
