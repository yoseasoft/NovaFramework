/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyright (C) 2023 - 2024, Guangzhou Shiyue Network Technology Co., Ltd.
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
    /// 消息事件分发调度对象的分析处理类，对业务层载入的所有消息事件调度类进行统一加载及分析处理
    /// </summary>
    internal static partial class EventCodeLoader
    {
        /// <summary>
        /// 事件调用类的结构信息管理容器
        /// </summary>
        private static IDictionary<SystemType, Structuring.EventCallCodeInfo> _eventCallCodeInfos = new Dictionary<SystemType, Structuring.EventCallCodeInfo>();

        [OnCodeLoaderClassLoadOfTarget(typeof(EventSystemAttribute))]
        private static bool LoadEventCallClass(Symboling.SymClass symClass, bool reload)
        {
            Structuring.EventCallCodeInfo info = new Structuring.EventCallCodeInfo();
            info.ClassType = symClass.ClassType;

            IList<Symboling.SymMethod> symMethods = symClass.GetAllMethods();
            for (int n = 0; null != symMethods && n < symMethods.Count; ++n)
            {
                Symboling.SymMethod symMethod = symMethods[n];

                // 检查函数格式是否合法
                if (false == symMethod.IsStatic || false == Inspecting.CodeInspector.CheckFunctionFormatOfEventCall(symMethod.MethodInfo))
                {
                    Debugger.Info(LogGroupTag.CodeLoader, "The event call method '{0}.{1}' was invalid format, added it failed.", symClass.FullName, symMethod.MethodName);
                    continue;
                }

                IList<SystemAttribute> attrs = symMethod.Attributes;
                for (int m = 0; null != attrs && m < attrs.Count; ++m)
                {
                    SystemAttribute attr = attrs[m];

                    if (attr is OnEventDispatchCallAttribute)
                    {
                        OnEventDispatchCallAttribute _attr = (OnEventDispatchCallAttribute) attr;

                        Structuring.EventCallMethodTypeCodeInfo callMethodInfo = new Structuring.EventCallMethodTypeCodeInfo();
                        callMethodInfo.TargetType = _attr.ClassType;
                        callMethodInfo.EventID = _attr.EventID;
                        callMethodInfo.EventDataType = _attr.EventDataType;

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
                                if (Inspecting.CodeInspector.CheckFunctionFormatOfEventCallWithNullParameterType(symMethod.MethodInfo))
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
                                if (Inspecting.CodeInspector.CheckFunctionFormatOfEventCallWithNullParameterType(symMethod.MethodInfo))
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
                }
            }

            if (info.GetMethodTypeCount() <= 0)
            {
                Debugger.Warn("The event call method types count must be great than zero, newly added class '{0}' failed.", info.ClassType.FullName);
                return false;
            }

            if (_eventCallCodeInfos.ContainsKey(symClass.ClassType))
            {
                if (reload)
                {
                    _eventCallCodeInfos.Remove(symClass.ClassType);
                }
                else
                {
                    Debugger.Warn("The event call '{0}' was already existed, repeat added it failed.", symClass.FullName);
                    return false;
                }
            }

            _eventCallCodeInfos.Add(symClass.ClassType, info);
            Debugger.Log(LogGroupTag.CodeLoader, "Load event call code info '{0}' succeed from target class type '{1}'.", Structuring.Formatter.ToString(info), symClass.FullName);

            return true;
        }

        [OnCodeLoaderClassCleanupOfTarget(typeof(EventSystemAttribute))]
        private static void CleanupAllEventCallClasses()
        {
            _eventCallCodeInfos.Clear();
        }

        [OnCodeLoaderClassLookupOfTarget(typeof(EventSystemAttribute))]
        private static Structuring.EventCallCodeInfo LookupEventCallCodeInfo(Symboling.SymClass symClass)
        {
            foreach (KeyValuePair<SystemType, Structuring.EventCallCodeInfo> pair in _eventCallCodeInfos)
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
