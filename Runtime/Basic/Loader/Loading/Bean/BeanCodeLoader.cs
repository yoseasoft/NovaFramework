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

using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine.Scripting;

namespace GameEngine.Loader
{
    /// <summary>
    /// 程序集中原型对象的分析处理类，对业务层载入的所有原型对象类进行统一加载及分析处理
    /// </summary>
    internal static partial class BeanCodeLoader
    {
        /// <summary>
        /// 加载原型类相关回调函数的管理容器
        /// </summary>
        private readonly static IDictionary<Type, Delegate> _classLoadCallbacks = new Dictionary<Type, Delegate>();
        /// <summary>
        /// 清理原型类相关回调函数的管理容器
        /// </summary>
        private readonly static IDictionary<Type, Delegate> _classCleanupCallbacks = new Dictionary<Type, Delegate>();
        /// <summary>
        /// 查找原型类结构信息相关回调函数的管理容器
        /// </summary>
        private readonly static IDictionary<Type, Delegate> _codeInfoLookupCallbacks = new Dictionary<Type, Delegate>();

        /// <summary>
        /// 初始化针对所有原型类声明的全部绑定回调接口
        /// </summary>
        [Preserve]
        [CodeLoader.OnGeneralCodeLoaderInit]
        private static void InitAllBeanClassLoadingCallbacks()
        {
            Type classType = typeof(BeanCodeLoader);
            MethodInfo[] methods = classType.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
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

                        Debugger.Assert(!_classLoadCallbacks.ContainsKey(_attr.ClassType), "Invalid bean class load type");
                        _classLoadCallbacks.Add(_attr.ClassType, method.CreateDelegate(typeof(CodeLoader.OnGeneralCodeLoaderLoadHandler)));
                    }
                    else if (typeof(OnCodeLoaderClassCleanupOfTargetAttribute) == attrType)
                    {
                        OnCodeLoaderClassCleanupOfTargetAttribute _attr = (OnCodeLoaderClassCleanupOfTargetAttribute) attr;

                        Debugger.Assert(!_classCleanupCallbacks.ContainsKey(_attr.ClassType), "Invalid bean class cleanup type");
                        _classCleanupCallbacks.Add(_attr.ClassType, method.CreateDelegate(typeof(CodeLoader.OnCleanupAllGeneralCodeLoaderHandler)));
                    }
                    else if (typeof(OnCodeLoaderClassLookupOfTargetAttribute) == attrType)
                    {
                        OnCodeLoaderClassLookupOfTargetAttribute _attr = (OnCodeLoaderClassLookupOfTargetAttribute) attr;

                        Debugger.Assert(!_codeInfoLookupCallbacks.ContainsKey(_attr.ClassType), "Invalid bean class lookup type");
                        _codeInfoLookupCallbacks.Add(_attr.ClassType, method.CreateDelegate(typeof(CodeLoader.OnGeneralCodeLoaderLookupHandler)));
                    }
                }
            }
        }

        /// <summary>
        /// 清理针对所有原型类声明的全部绑定回调接口
        /// </summary>
        [Preserve]
        [CodeLoader.OnGeneralCodeLoaderCleanup]
        private static void CleanupAllBeanClassLoadingCallbacks()
        {
            IEnumerator<KeyValuePair<Type, Delegate>> e = _classCleanupCallbacks.GetEnumerator();
            while (e.MoveNext())
            {
                CodeLoader.OnCleanupAllGeneralCodeLoaderHandler handler = e.Current.Value as CodeLoader.OnCleanupAllGeneralCodeLoaderHandler;
                Debugger.Assert(null != handler, "Invalid bean class cleanup handler.");

                handler.Invoke();
            }

            _classLoadCallbacks.Clear();
            _classCleanupCallbacks.Clear();
            _codeInfoLookupCallbacks.Clear();
        }

        /// <summary>
        /// 检测原型类指定的类型是否匹配当前加载器
        /// </summary>
        /// <param name="symClass">对象标记类型</param>
        /// <param name="filterType">过滤对象类型</param>
        /// <returns>若给定类型满足匹配规则则返回true，否则返回false</returns>
        [Preserve]
        [CodeLoader.OnGeneralCodeLoaderMatch]
        private static bool IsBeanClassMatched(Symboling.SymClass symClass, Type filterType)
        {
            // 存在过滤类型，则直接对比过滤类型即可
            if (null != filterType)
            {
                // 直接将目标类型替换为过滤类型进行检测即可
                return typeof(IBean).IsAssignableFrom(filterType);
            }

            // 原型类必须为可实例化的类
            if (false == symClass.IsInstantiate)
            {
                return false;
            }

            if (typeof(IBean).IsAssignableFrom(symClass.ClassType))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 加载原型类指定的类型
        /// </summary>
        /// <param name="symClass">对象标记类型</param>
        /// <param name="reload">重载状态标识</param>
        /// <returns>若存在给定类型属性原型类则返回对应处理结果，否则返回false</returns>
        [Preserve]
        [CodeLoader.OnGeneralCodeLoaderLoad]
        private static bool LoadBeanClass(Symboling.SymClass symClass, bool reload)
        {
            if (TryGetBeanClassCallbackForTargetContainer(symClass.ClassType, out Delegate callback, _classLoadCallbacks))
            {
                CodeLoader.OnGeneralCodeLoaderLoadHandler handler = callback as CodeLoader.OnGeneralCodeLoaderLoadHandler;
                Debugger.Assert(null != handler, "Invalid bean class load handler.");
                return handler.Invoke(symClass, reload);
            }

            return false;
        }

        /// <summary>
        /// 查找原型类指定的类型对应的结构信息
        /// </summary>
        /// <param name="symClass">对象标记类型</param>
        /// <returns>返回类型对应的结构信息</returns>
        [Preserve]
        [CodeLoader.OnGeneralCodeLoaderLookup]
        private static Structuring.GeneralCodeInfo LookupBeanCodeInfo(Symboling.SymClass symClass)
        {
            if (TryGetBeanClassCallbackForTargetContainer(symClass.ClassType, out Delegate callback, _codeInfoLookupCallbacks))
            {
                CodeLoader.OnGeneralCodeLoaderLookupHandler handler = callback as CodeLoader.OnGeneralCodeLoaderLookupHandler;
                Debugger.Assert(null != handler, "Invalid bean class lookup handler.");
                return handler.Invoke(symClass);
            }

            return null;
        }

        /// <summary>
        /// 通过指定的类型从原型类的回调管理容器中查找对应的回调句柄实例
        /// </summary>
        /// <param name="targetType">对象类型</param>
        /// <param name="callback">回调句柄</param>
        /// <param name="container">句柄列表容器</param>
        /// <returns>返回通过给定类型查找的回调句柄实例，若不存在则返回null</returns>
        private static bool TryGetBeanClassCallbackForTargetContainer(Type targetType, out Delegate callback, IDictionary<Type, Delegate> container)
        {
            callback = null;

            IEnumerator<KeyValuePair<Type, Delegate>> e = container.GetEnumerator();
            while (e.MoveNext())
            {
                if (e.Current.Key.IsAssignableFrom(targetType) && e.Current.Key != targetType)
                {
                    callback = e.Current.Value;
                    return true;
                }
            }

            return false;
        }
    }
}
