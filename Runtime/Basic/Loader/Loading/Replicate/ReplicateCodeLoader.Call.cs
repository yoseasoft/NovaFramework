/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyright (C) 2025 - 2026, Hainan Yuanyou Information Technology Co., Ltd. Guangzhou Branch
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

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Scripting;

namespace GameEngine.Loader
{
    /// 数据同步管理对象的分析处理类
    static partial class ReplicateCodeLoader
    {
        /// <summary>
        /// 同步调用类的结构信息管理容器
        /// </summary>
        private readonly static IDictionary<Type, Structuring.ReplicateCallCodeInfo> _replicateCallCodeInfos = new Dictionary<Type, Structuring.ReplicateCallCodeInfo>();

        [Preserve]
        [OnCodeLoaderClassLoadOfTarget(typeof(ReplicateSystemAttribute))]
        private static bool LoadReplicateCallClass(Symbolling.SymClass symClass, bool reload)
        {
            Structuring.ReplicateCallCodeInfo info = new Structuring.ReplicateCallCodeInfo();
            info.ClassType = symClass.ClassType;

            IList<Symbolling.SymMethod> symMethods = symClass.GetAllMethods();
            for (int n = 0; null != symMethods && n < symMethods.Count; ++n)
            {
                Symbolling.SymMethod symMethod = symMethods[n];

                if (false == symMethod.IsStatic || symMethod.IsExtension)
                {
                    continue;
                }

                // 检查函数格式是否合法
                if (false == Inspecting.CodeInspector.CheckFunctionFormatOfReplicateCall(symMethod.MethodInfo))
                {
                    Debugger.Info(LogGroupTag.CodeLoader, "The replicate call method '{%s}.{%s}' was invalid format, added it failed.", symClass.FullName, symMethod.MethodName);
                    continue;
                }

                IList<Attribute> attrs = symMethod.Attributes;
                for (int m = 0; null != attrs && m < attrs.Count; ++m)
                {
                    Attribute attr = attrs[m];

                    if (attr is OnReplicateDispatchCallAttribute)
                    {
                        OnReplicateDispatchCallAttribute _attr = (OnReplicateDispatchCallAttribute) attr;

                        Structuring.ReplicateCallMethodTypeCodeInfo callMethodInfo = new Structuring.ReplicateCallMethodTypeCodeInfo();
                        callMethodInfo.TargetType = _attr.ClassType;
                        callMethodInfo.Tags = _attr.Tags;
                        callMethodInfo.AnnounceType = _attr.AnnounceType;

                        if (string.IsNullOrEmpty(callMethodInfo.Tags))
                        {
                            // 未进行合法标识的函数忽略它
                            continue;
                        }

                        // 先记录函数信息并检查函数格式
                        // 在绑定环节在进行委托的格式转换
                        callMethodInfo.Fullname = symMethod.FullName;
                        callMethodInfo.Method = symMethod.MethodInfo;

                        // 函数参数类型的格式检查，仅在调试模式下执行，正式环境可跳过该处理
                        if (NovaEngine.CVerification.IsOnDebuggingVerificationActivated())
                        {
                            bool verificated = false;

                            if (Inspecting.CodeInspector.CheckFunctionFormatOfReplicateCallWithNullParameterType(symMethod.MethodInfo))
                            {
                                // 无参类型的事件函数
                                verificated = NovaEngine.CVerification.CheckGenericDelegateParameterTypeMatchedOfTargetObject(symMethod.MethodInfo, callMethodInfo.TargetType);
                            }
                            else
                            {
                                // 事件数据派发
                                verificated = NovaEngine.CVerification.CheckGenericDelegateParameterTypeMatchedOfTargetObject(symMethod.MethodInfo, callMethodInfo.TargetType, typeof(string), typeof(ReplicateAnnounceType));
                            }

                            // 校验失败
                            if (false == verificated)
                            {
                                Debugger.Error(LogGroupTag.CodeLoader, "Cannot verificated from method info '{%s}' to replicate standard call, loaded this method failed.", symMethod.FullName);
                                continue;
                            }
                        }

                        // if (false == method.IsStatic)
                        // { Debugger.Warn(LogGroupTag.CodeLoader, "The replicate call method '{%s}.{%s}' must be static type, loaded it failed.", symClass.FullName, symMethod.MethodName); continue; }

                        info.AddMethodType(callMethodInfo);
                    }
                }
            }

            if (info.GetMethodTypeCount() <= 0)
            {
                Debugger.Warn(LogGroupTag.CodeLoader, "The replicate call method types count must be great than zero, newly added class '{%t}' failed.", info.ClassType);
                return false;
            }

            if (_replicateCallCodeInfos.ContainsKey(symClass.ClassType))
            {
                if (reload)
                {
                    _replicateCallCodeInfos.Remove(symClass.ClassType);
                }
                else
                {
                    Debugger.Warn(LogGroupTag.CodeLoader, "The replicate call '{%s}' was already existed, repeat added it failed.", symClass.FullName);
                    return false;
                }
            }

            _replicateCallCodeInfos.Add(symClass.ClassType, info);
            Debugger.Log(LogGroupTag.CodeLoader, "Load replicate call code info '{%s}' succeed from target class type '{%s}'.", CodeLoaderUtils.ToString(info), symClass.FullName);

            return true;
        }

        [Preserve]
        [OnCodeLoaderClassCleanupOfTarget(typeof(ReplicateSystemAttribute))]
        private static void CleanupAllReplicateCallClasses()
        {
            _replicateCallCodeInfos.Clear();
        }

        [Preserve]
        [OnCodeLoaderClassLookupOfTarget(typeof(ReplicateSystemAttribute))]
        private static Structuring.ReplicateCallCodeInfo LookupReplicateCallCodeInfo(Symbolling.SymClass symClass)
        {
            if (_replicateCallCodeInfos.TryGetValue(symClass.ClassType, out Structuring.ReplicateCallCodeInfo codeInfo))
            {
                return codeInfo;
            }

            return null;
        }
    }
}
