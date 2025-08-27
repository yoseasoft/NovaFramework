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

namespace GameEngine.Loader
{
    /// <summary>
    /// 程序集中切面控制对象的分析处理类，对业务层载入的所有切面控制类进行统一加载及分析处理
    /// </summary>
    internal static partial class AspectCodeLoader
    {
        /// <summary>
        /// 切面调用类的结构信息管理容器
        /// </summary>
        private static IDictionary<SystemType, Structuring.AspectCallCodeInfo> _aspectCallCodeInfos = new Dictionary<SystemType, Structuring.AspectCallCodeInfo>();

        [OnCodeLoaderClassLoadOfTarget(typeof(AspectAttribute))]
        private static bool LoadAspectCallClass(Symboling.SymClass symClass, bool reload)
        {
            Structuring.AspectCallCodeInfo info = new Structuring.AspectCallCodeInfo();
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

                Structuring.AspectCallMethodTypeCodeInfo callMethodInfo = new Structuring.AspectCallMethodTypeCodeInfo();
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
                if (false == Inspecting.CodeInspector.CheckFunctionFormatOfAspect(symMethod.MethodInfo))
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
        private static Structuring.AspectCallCodeInfo LookupAspectCallCodeInfo(Symboling.SymClass symCLass)
        {
            foreach (KeyValuePair<SystemType, Structuring.AspectCallCodeInfo> pair in _aspectCallCodeInfos)
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
