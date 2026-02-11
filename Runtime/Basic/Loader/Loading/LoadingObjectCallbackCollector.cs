/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyright (C) 2024 - 2025, Hurley, Independent Studio.
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
using System.Customize.Extension;
using System.Reflection;

namespace GameEngine.Loader
{
    /// <summary>
    /// 分析处理对象的回调收集器对象类，用于对分析处理对象内部的回调句柄统一管理
    /// </summary>
    internal sealed class LoadingObjectCallbackCollector
    {
        /// <summary>
        /// 服务的目标加载对象类型
        /// </summary>
        private Type _targetType;

        /// <summary>
        /// 加载对象池管理类相关回调函数的管理容器
        /// </summary>
        private readonly IDictionary<Type, Delegate> _classLoadCallbacks;
        /// <summary>
        /// 清理对象池管理类相关回调函数的管理容器
        /// </summary>
        private readonly IDictionary<Type, Delegate> _classCleanupCallbacks;
        /// <summary>
        /// 查找对象池管理类结构信息相关回调函数的管理容器
        /// </summary>
        private readonly IDictionary<Type, Delegate> _codeInfoLookupCallbacks;

        /// <summary>
        /// 获取全部的加载对象类型
        /// </summary>
        public IReadOnlyCollection<Type> RelevanceTypes => _classLoadCallbacks.As<Dictionary<Type, Delegate>>().Keys;

        public LoadingObjectCallbackCollector()
        {
            _classLoadCallbacks = new Dictionary<Type, Delegate>();
            _classCleanupCallbacks = new Dictionary<Type, Delegate>();
            _codeInfoLookupCallbacks = new Dictionary<Type, Delegate>();
        }

        /// <summary>
        /// 目标加载对象的初始化函数
        /// </summary>
        /// <param name="targetType">对象类型</param>
        public void OnInitializeContext(Type targetType)
        {
            _targetType = targetType;
            MethodInfo[] methods = targetType.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            for (int n = 0; n < methods.Length; ++n)
            {
                MethodInfo method = methods[n];
                IEnumerable<Attribute> e = method.GetCustomAttributes();
                foreach (Attribute attr in e)
                {
                    Type attrType = attr.GetType();
                    if (typeof(OnCodeLoaderClassLoadOfTargetAttribute) == attrType)
                    {
                        OnCodeLoaderClassLoadOfTargetAttribute _attr = (OnCodeLoaderClassLoadOfTargetAttribute) attr;

                        Debugger.Assert(!_classLoadCallbacks.ContainsKey(_attr.ClassType), "Invalid class '{%t}' load type", targetType);
                        _classLoadCallbacks.Add(_attr.ClassType, method.CreateDelegate(typeof(CodeLoader.OnGeneralCodeLoaderLoadHandler)));
                    }
                    else if (typeof(OnCodeLoaderClassCleanupOfTargetAttribute) == attrType)
                    {
                        OnCodeLoaderClassCleanupOfTargetAttribute _attr = (OnCodeLoaderClassCleanupOfTargetAttribute) attr;

                        Debugger.Assert(!_classCleanupCallbacks.ContainsKey(_attr.ClassType), "Invalid class '{%t}' cleanup type", targetType);
                        _classCleanupCallbacks.Add(_attr.ClassType, method.CreateDelegate(typeof(CodeLoader.OnCleanupAllGeneralCodeLoaderHandler)));
                    }
                    else if (typeof(OnCodeLoaderClassLookupOfTargetAttribute) == attrType)
                    {
                        OnCodeLoaderClassLookupOfTargetAttribute _attr = (OnCodeLoaderClassLookupOfTargetAttribute) attr;

                        Debugger.Assert(!_codeInfoLookupCallbacks.ContainsKey(_attr.ClassType), "Invalid class '{%t}' lookup type", targetType);
                        _codeInfoLookupCallbacks.Add(_attr.ClassType, method.CreateDelegate(typeof(CodeLoader.OnGeneralCodeLoaderLookupHandler)));
                    }
                }
            }
        }

        /// <summary>
        /// 目标加载对象的清理函数
        /// </summary>
        public void OnCleanupContext()
        {
            foreach (Delegate callback in _classCleanupCallbacks.Values)
            {
                CodeLoader.OnCleanupAllGeneralCodeLoaderHandler handler = callback as CodeLoader.OnCleanupAllGeneralCodeLoaderHandler;
                Debugger.Assert(handler, "Invalid class '{%t}' cleanup handler.", _targetType);

                handler.Invoke();
            }

            _targetType = null;

            _classLoadCallbacks.Clear();
            _classCleanupCallbacks.Clear();
            _codeInfoLookupCallbacks.Clear();
        }

        /// <summary>
        /// 尝试加载指定标记类型所对应的类对象<br/>
        /// 通过匹配回调函数对指定标记类型进行检测，查找是否注册了对应的加载回调函数<br/>
        /// 若存在对应的加载回调函数，则使用该函数对指定标记类型进行加载，并返回加载成功的状态标识
        /// </summary>
        /// <param name="onMatched">类型匹配回调函数</param>
        /// <param name="matchedType">匹配的目标类型</param>
        /// <param name="symClass">对象标记类型</param>
        /// <param name="reload">重载标识</param>
        /// <param name="succeed">加载成功状态标识</param>
        /// <returns>若成功匹配到指定标记类型对应的加载回调则返回true，否则返回false</returns>
        public bool TryLoadClass(Func<Type, Type, bool> onMatched, Type matchedType, Symbolling.SymClass symClass, bool reload, out bool succeed)
        {
            foreach (KeyValuePair<Type, Delegate> kvp in _classLoadCallbacks)
            {
                if (onMatched(matchedType, kvp.Key))
                {
                    CodeLoader.OnGeneralCodeLoaderLoadHandler handler = kvp.Value as CodeLoader.OnGeneralCodeLoaderLoadHandler;
                    Debugger.Assert(handler, "Invalid class '{%t}' load handler.", _targetType);
                    succeed = handler.Invoke(symClass, reload);
                    return true;
                }
            }

            succeed = false;
            return false;
        }

        /// <summary>
        /// 尝试查找指定标记类型所对应的结构信息<br/>
        /// 通过匹配回调函数对指定标记类型进行检测，查找是否注册了对应的检索回调函数<br/>
        /// 若存在对应的检索回调函数，则使用该函数对指定标记类型进行查找，并返回查找成功的结构信息
        /// </summary>
        /// <param name="onMatched">类型匹配回调函数</param>
        /// <param name="matchedType">匹配的目标类型</param>
        /// <param name="symClass">对象标记类型</param>
        /// <param name="codeInfo">结构信息</param>
        /// <returns>若成功匹配到指定标记类型对应的检索回调则返回true，否则返回false</returns>
        public bool TryLookupCodeInfo(Func<Type, Type, bool> onMatched, Type matchedType, Symbolling.SymClass symClass, out Structuring.GeneralCodeInfo codeInfo)
        {
            foreach (KeyValuePair<Type, Delegate> kvp in _codeInfoLookupCallbacks)
            {
                if (onMatched(matchedType, kvp.Key))
                {
                    CodeLoader.OnGeneralCodeLoaderLookupHandler handler = kvp.Value as CodeLoader.OnGeneralCodeLoaderLookupHandler;
                    Debugger.Assert(handler, "Invalid class '{%t}' lookup handler.", _targetType);
                    codeInfo = handler.Invoke(symClass);
                    return true;
                }
            }

            codeInfo = null;
            return false;
        }
    }
}
