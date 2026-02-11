/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyright (C) 2024 - 2025, Hurley, Independent Studio.
/// Copyright (C) 2025, Hainan Yuanyou Information Technology Co., Ltd. Guangzhou Branch
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
using System.Collections.Generic;
using UnityEngine.Scripting;

namespace GameEngine.Loader
{
    /// 程序集中通知定义对象的分析处理类
    internal static partial class NoticeCodeLoader
    {
        /// <summary>
        /// 通知定义调用类的结构信息管理容器
        /// </summary>
        private readonly static IDictionary<Type, Structuring.CViewNoticeCallCodeInfo> _viewNoticeCallCodeInfos = new Dictionary<Type, Structuring.CViewNoticeCallCodeInfo>();

        [Preserve]
        [OnCodeLoaderClassLoadOfTarget(typeof(NoticeSupportedOnViewAttribute))]
        private static bool LoadViewNoticeCallClass(Symbolling.SymClass symClass, bool reload)
        {
            Structuring.CViewNoticeCallCodeInfo info = new Structuring.CViewNoticeCallCodeInfo();
            info.ClassType = symClass.ClassType;

            IList<Symbolling.SymMethod> symMethods = symClass.GetAllMethods();
            for (int n = 0; null != symMethods && n < symMethods.Count; ++n)
            {
                Symbolling.SymMethod symMethod = symMethods[n];

                // 检查函数格式是否合法，通知类型的函数必须为静态类型
                if (false == symMethod.IsStatic || false == symMethod.IsExtension)
                {
                    // Debugger.Info(LogGroupTag.CodeLoader, "符号类‘{%t}’的目标函数‘{%t}’不满足‘NoticeSupported’通知回调函数格式类型的定义规则，不能进行正确的函数加载解析！", symClass.ClassType, symMethod.MethodInfo);
                    continue;
                }

                Type extendClassType = symMethod.ExtensionParameterType;

                IList<Attribute> attrs = symMethod.Attributes;
                for (int m = 0; null != attrs && m < attrs.Count; ++m)
                {
                    Attribute attr = attrs[m];

                    if (attr is CViewNoticeCallAttribute viewNoticeCallAttribute)
                    {
                        if (viewNoticeCallAttribute.NoticeType <= 0)
                        {
                            Debugger.Warn(LogGroupTag.CodeLoader, "The notice method '{%s}' was invalid arguments from class '{%s}', added it failed.", symClass.FullName, symMethod.MethodName);
                            continue;
                        }

                        if (false == Inspecting.CodeInspector.CheckFunctionFormatOfTargetWithNullParameterType(symMethod.MethodInfo))
                        {
                            Debugger.Warn(LogGroupTag.CodeLoader, "The notice method '{%s}' was invalid format from class '{%s}', added it failed.", symClass.FullName, symMethod.MethodName);
                            continue;
                        }

                        Structuring.CViewNoticeMethodTypeCodeInfo methodTypeCodeInfo = new Structuring.CViewNoticeMethodTypeCodeInfo();
                        methodTypeCodeInfo.TargetType = extendClassType;
                        methodTypeCodeInfo.NoticeType = viewNoticeCallAttribute.NoticeType;
                        methodTypeCodeInfo.BehaviourType = viewNoticeCallAttribute.BehaviourType;
                        methodTypeCodeInfo.Fullname = symMethod.FullName;
                        methodTypeCodeInfo.Method = symMethod.MethodInfo;


                        info.AddMethodType(methodTypeCodeInfo);
                    }
                }
            }

            if (info.GetMethodTypeCount() <= 0)
            {
                Debugger.Warn(LogGroupTag.CodeLoader, "The notice call method types count must be great than zero, newly added class '{%t}' failed.", info.ClassType);
                return false;
            }

            if (_viewNoticeCallCodeInfos.ContainsKey(symClass.ClassType))
            {
                if (reload)
                {
                    _viewNoticeCallCodeInfos.Remove(symClass.ClassType);
                }
                else
                {
                    Debugger.Warn(LogGroupTag.CodeLoader, "The notice call '{%s}' was already existed, repeat added it failed.", symClass.FullName);
                    return false;
                }
            }

            _viewNoticeCallCodeInfos.Add(symClass.ClassType, info);
            Debugger.Log(LogGroupTag.CodeLoader, "Load notice call code info '{%s}' succeed from target class type '{%s}'.", CodeLoaderUtils.ToString(info), symClass.FullName);

            return true;
        }

        [Preserve]
        [OnCodeLoaderClassCleanupOfTarget(typeof(NoticeSupportedOnViewAttribute))]
        private static void CleanupAllViewNoticeCallClasses()
        {
            _viewNoticeCallCodeInfos.Clear();
        }

        [Preserve]
        [OnCodeLoaderClassLookupOfTarget(typeof(NoticeSupportedOnViewAttribute))]
        private static Structuring.CViewNoticeCallCodeInfo LookupViewNoticeCallCodeInfo(Symbolling.SymClass symClass)
        {
            if (_viewNoticeCallCodeInfos.TryGetValue(symClass.ClassType, out Structuring.CViewNoticeCallCodeInfo codeInfo))
            {
                return codeInfo;
            }

            return null;
        }
    }
}
