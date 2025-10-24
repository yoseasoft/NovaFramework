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

using System.Collections.Generic;

using SystemType = System.Type;
using SystemAttribute = System.Attribute;

namespace GameEngine.Loader
{
    /// <summary>
    /// 程序集中通知定义对象的分析处理类，对业务层载入的所有通知定义对象类进行统一加载及分析处理
    /// </summary>
    internal static partial class NoticeCodeLoader
    {
        /// <summary>
        /// 通知定义调用类的结构信息管理容器
        /// </summary>
        private static IDictionary<SystemType, Structuring.CViewNoticeCallCodeInfo> _viewNoticeCallCodeInfos = new Dictionary<SystemType, Structuring.CViewNoticeCallCodeInfo>();

        [OnCodeLoaderClassLoadOfTarget(typeof(NoticeSupportedOnViewAttribute))]
        private static bool LoadViewNoticeCallClass(Symboling.SymClass symClass, bool reload)
        {
            Structuring.CViewNoticeCallCodeInfo info = new Structuring.CViewNoticeCallCodeInfo();
            info.ClassType = symClass.ClassType;

            IList<Symboling.SymMethod> symMethods = symClass.GetAllMethods();
            for (int n = 0; null != symMethods && n < symMethods.Count; ++n)
            {
                Symboling.SymMethod symMethod = symMethods[n];

                // 检查函数格式是否合法，通知类型的函数必须为静态类型
                if (false == symMethod.IsStatic || false == symMethod.IsExtension)
                {
                    // Debugger.Info(LogGroupTag.CodeLoader, "符号类‘{%t}’的目标函数‘{%t}’不满足‘NoticeSupported’通知回调函数格式类型的定义规则，不能进行正确的函数加载解析！", symClass.ClassType, symMethod.MethodInfo);
                    continue;
                }

                SystemType extendClassType = symMethod.ExtensionParameterType;

                IList<SystemAttribute> attrs = symMethod.Attributes;
                for (int m = 0; null != attrs && m < attrs.Count; ++m)
                {
                    SystemAttribute attr = attrs[m];

                    if (attr is CViewNoticeCallAttribute)
                    {
                        CViewNoticeCallAttribute _attr = (CViewNoticeCallAttribute) attr;

                        if (_attr.NoticeType <= 0)
                        {
                            Debugger.Warn("The notice method '{%s}' was invalid arguments from class '{%s}', added it failed.", symClass.FullName, symMethod.MethodName);
                            continue;
                        }

                        if (false == Inspecting.CodeInspector.CheckFunctionFormatOfTargetWithNullParameterType(symMethod.MethodInfo))
                        {
                            Debugger.Warn("The notice method '{%s}' was invalid format from class '{%s}', added it failed.", symClass.FullName, symMethod.MethodName);
                            continue;
                        }

                        Structuring.CViewNoticeMethodTypeCodeInfo methodTypeCodeInfo = new Structuring.CViewNoticeMethodTypeCodeInfo();
                        methodTypeCodeInfo.TargetType = extendClassType;
                        methodTypeCodeInfo.NoticeType = _attr.NoticeType;
                        methodTypeCodeInfo.BehaviourType = _attr.BehaviourType;
                        methodTypeCodeInfo.Fullname = symMethod.FullName;
                        methodTypeCodeInfo.Method = symMethod.MethodInfo;


                        info.AddMethodType(methodTypeCodeInfo);
                    }
                }
            }

            if (info.GetMethodTypeCount() <= 0)
            {
                Debugger.Warn("The notice call method types count must be great than zero, newly added class '{%t}' failed.", info.ClassType);
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
                    Debugger.Warn("The notice call '{%s}' was already existed, repeat added it failed.", symClass.FullName);
                    return false;
                }
            }

            _viewNoticeCallCodeInfos.Add(symClass.ClassType, info);
            Debugger.Log(LogGroupTag.CodeLoader, "Load notice call code info '{%s}' succeed from target class type '{%s}'.", CodeLoaderObject.ToString(info), symClass.FullName);

            return true;
        }

        [OnCodeLoaderClassCleanupOfTarget(typeof(NoticeSupportedOnViewAttribute))]
        private static void CleanupAllViewNoticeCallClasses()
        {
            _viewNoticeCallCodeInfos.Clear();
        }

        [OnCodeLoaderClassLookupOfTarget(typeof(NoticeSupportedOnViewAttribute))]
        private static Structuring.CViewNoticeCallCodeInfo LookupViewNoticeCallCodeInfo(Symboling.SymClass symClass)
        {
            foreach (KeyValuePair<SystemType, Structuring.CViewNoticeCallCodeInfo> pair in _viewNoticeCallCodeInfos)
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
