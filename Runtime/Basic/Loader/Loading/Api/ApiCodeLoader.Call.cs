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
    /// 程序集中编程接口对象的分析处理类
    internal static partial class ApiCodeLoader
    {
        /// <summary>
        /// 切面调用类的结构信息管理容器
        /// </summary>
        private readonly static IDictionary<Type, Structuring.ApiCallCodeInfo> _apiCallCodeInfos = new Dictionary<Type, Structuring.ApiCallCodeInfo>();

        [Preserve]
        [OnCodeLoaderClassLoadOfTarget(typeof(ApiSystemAttribute))]
        private static bool LoadApiCallClass(Symbolling.SymClass symClass, bool reload)
        {
            Structuring.ApiCallCodeInfo info = new Structuring.ApiCallCodeInfo();
            info.ClassType = symClass.ClassType;

            IList<Symbolling.SymMethod> symMethods = symClass.GetAllMethods();
            for (int n = 0; null != symMethods && n < symMethods.Count; ++n)
            {
                Symbolling.SymMethod symMethod = symMethods[n];

                // 非静态方法直接跳过
                if (false == symMethod.IsStatic)
                {
                    continue;
                }

                // 获取编程接口特性标签
                OnApiDispatchCallAttribute _attr = symMethod.GetAttribute<OnApiDispatchCallAttribute>(true);
                if (null == _attr)
                {
                    continue;
                }

                Structuring.ApiCallMethodTypeCodeInfo callMethodInfo = new Structuring.ApiCallMethodTypeCodeInfo();
                callMethodInfo.TargetType = _attr.ClassType;
                callMethodInfo.FunctionName = _attr.FunctionName;

                if (string.IsNullOrEmpty(callMethodInfo.FunctionName))
                {
                    // 未进行合法标识的函数忽略它
                    continue;
                }

                // 先记录函数信息并检查函数格式
                // 在绑定环节在进行委托的格式转换
                callMethodInfo.Fullname = symMethod.FullName;
                callMethodInfo.Method = symMethod.MethodInfo;

                // if (false == method.IsStatic)
                // { Debugger.Warn("The api call method '{%s}.{%s}' must be static type, loaded it failed.", symClass.FullName, symMethod.MethodName); continue; }

                info.AddMethodType(callMethodInfo);
            }

            if (info.GetMethodTypeCount() <= 0)
            {
                Debugger.Warn("The api call method types count must be great than zero, newly added class '{%t}' failed.", info.ClassType);
                return false;
            }

            if (_apiCallCodeInfos.ContainsKey(symClass.ClassType))
            {
                if (reload)
                {
                    _apiCallCodeInfos.Remove(symClass.ClassType);
                }
                else
                {
                    Debugger.Warn("The api call '{%s}' was already existed, repeat added it failed.", symClass.FullName);
                    return false;
                }
            }

            _apiCallCodeInfos.Add(symClass.ClassType, info);
            Debugger.Log(LogGroupTag.CodeLoader, "Load api call code info '{%s}' succeed from target class type '{%s}'.", CodeLoaderUtils.ToString(info), symClass.FullName);

            return true;
        }

        [Preserve]
        [OnCodeLoaderClassCleanupOfTarget(typeof(ApiSystemAttribute))]
        private static void CleanupAllApiCallClasses()
        {
            _apiCallCodeInfos.Clear();
        }

        [Preserve]
        [OnCodeLoaderClassLookupOfTarget(typeof(ApiSystemAttribute))]
        private static Structuring.ApiCallCodeInfo LookupApiCallCodeInfo(Symbolling.SymClass symCLass)
        {
            foreach (KeyValuePair<Type, Structuring.ApiCallCodeInfo> pair in _apiCallCodeInfos)
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
