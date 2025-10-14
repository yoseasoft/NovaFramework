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
    /// 程序集中编程接口对象的分析处理类，对业务层载入的所有编程接口类进行统一加载及分析处理
    /// </summary>
    internal static partial class ApiCodeLoader
    {
        /// <summary>
        /// 切面调用类的结构信息管理容器
        /// </summary>
        private static IDictionary<SystemType, Structuring.ApiCallCodeInfo> _apiCallCodeInfos = new Dictionary<SystemType, Structuring.ApiCallCodeInfo>();

        [OnCodeLoaderClassLoadOfTarget(typeof(ApiSystemAttribute))]
        private static bool LoadApiCallClass(Symboling.SymClass symClass, bool reload)
        {
            Structuring.ApiCallCodeInfo info = new Structuring.ApiCallCodeInfo();
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
                // { Debugger.Warn("The api call method '{0} - {1}' must be static type, loaded it failed.", symClass.FullName, symMethod.MethodName); continue; }

                info.AddMethodType(callMethodInfo);
            }

            if (info.GetMethodTypeCount() <= 0)
            {
                Debugger.Warn("The api call method types count must be great than zero, newly added class '{0}' failed.", info.ClassType.FullName);
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
                    Debugger.Warn("The api call '{0}' was already existed, repeat added it failed.", symClass.FullName);
                    return false;
                }
            }

            _apiCallCodeInfos.Add(symClass.ClassType, info);
            Debugger.Log(LogGroupTag.CodeLoader, "Load api call code info '{0}' succeed from target class type '{1}'.", CodeLoaderObject.ToString(info), symClass.FullName);

            return true;
        }

        [OnCodeLoaderClassCleanupOfTarget(typeof(ApiSystemAttribute))]
        private static void CleanupAllApiCallClasses()
        {
            _apiCallCodeInfos.Clear();
        }

        [OnCodeLoaderClassLookupOfTarget(typeof(ApiSystemAttribute))]
        private static Structuring.ApiCallCodeInfo LookupApiCallCodeInfo(Symboling.SymClass symCLass)
        {
            foreach (KeyValuePair<SystemType, Structuring.ApiCallCodeInfo> pair in _apiCallCodeInfos)
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
