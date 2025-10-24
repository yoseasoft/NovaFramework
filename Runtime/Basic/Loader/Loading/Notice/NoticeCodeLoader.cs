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
using System.Reflection;

using SystemType = System.Type;
using SystemDelegate = System.Delegate;
using SystemAttribute = System.Attribute;
using SystemMethodInfo = System.Reflection.MethodInfo;
using SystemBindingFlags = System.Reflection.BindingFlags;

namespace GameEngine.Loader
{
    /// <summary>
    /// 程序集中通知定义对象的分析处理类，对业务层载入的所有通知定义对象类进行统一加载及分析处理
    /// </summary>
    internal static partial class NoticeCodeLoader
    {
        /// <summary>
        /// 加载通知定义类相关回调函数的管理容器
        /// </summary>
        private static IDictionary<SystemType, SystemDelegate> _classLoadCallbacks = new Dictionary<SystemType, SystemDelegate>();
        /// <summary>
        /// 清理通知定义类相关回调函数的管理容器
        /// </summary>
        private static IDictionary<SystemType, SystemDelegate> _classCleanupCallbacks = new Dictionary<SystemType, SystemDelegate>();
        /// <summary>
        /// 查找通知定义类结构信息相关回调函数的管理容器
        /// </summary>
        private static IDictionary<SystemType, SystemDelegate> _codeInfoLookupCallbacks = new Dictionary<SystemType, SystemDelegate>();

        /// <summary>
        /// 初始化针对所有通知定义类声明的全部绑定回调接口
        /// </summary>
        [CodeLoader.OnGeneralCodeLoaderInit]
        private static void InitAllNoticeClassLoadingCallbacks()
        {
            SystemType classType = typeof(NoticeCodeLoader);
            SystemMethodInfo[] methods = classType.GetMethods(SystemBindingFlags.Public | SystemBindingFlags.NonPublic | SystemBindingFlags.Static);
            for (int n = 0; n < methods.Length; ++n)
            {
                SystemMethodInfo method = methods[n];
                IEnumerable<SystemAttribute> e = method.GetCustomAttributes();
                foreach (SystemAttribute attr in e)
                {
                    SystemType attrType = attr.GetType();
                    if (typeof(OnCodeLoaderClassLoadOfTargetAttribute) == attrType)
                    {
                        OnCodeLoaderClassLoadOfTargetAttribute _attr = (OnCodeLoaderClassLoadOfTargetAttribute) attr;

                        Debugger.Assert(!_classLoadCallbacks.ContainsKey(_attr.ClassType), "Invalid notice class load type");
                        _classLoadCallbacks.Add(_attr.ClassType, method.CreateDelegate(typeof(CodeLoader.OnGeneralCodeLoaderLoadHandler)));
                    }
                    else if (typeof(OnCodeLoaderClassCleanupOfTargetAttribute) == attrType)
                    {
                        OnCodeLoaderClassCleanupOfTargetAttribute _attr = (OnCodeLoaderClassCleanupOfTargetAttribute) attr;

                        Debugger.Assert(!_classCleanupCallbacks.ContainsKey(_attr.ClassType), "Invalid notice class cleanup type");
                        _classCleanupCallbacks.Add(_attr.ClassType, method.CreateDelegate(typeof(CodeLoader.OnCleanupAllGeneralCodeLoaderHandler)));
                    }
                    else if (typeof(OnCodeLoaderClassLookupOfTargetAttribute) == attrType)
                    {
                        OnCodeLoaderClassLookupOfTargetAttribute _attr = (OnCodeLoaderClassLookupOfTargetAttribute) attr;

                        Debugger.Assert(!_codeInfoLookupCallbacks.ContainsKey(_attr.ClassType), "Invalid notice class lookup type");
                        _codeInfoLookupCallbacks.Add(_attr.ClassType, method.CreateDelegate(typeof(CodeLoader.OnGeneralCodeLoaderLookupHandler)));
                    }
                }
            }
        }

        /// <summary>
        /// 清理针对所有通知定义类声明的全部绑定回调接口
        /// </summary>
        [CodeLoader.OnGeneralCodeLoaderCleanup]
        private static void CleanupAllNoticeClassLoadingCallbacks()
        {
            IEnumerator<KeyValuePair<SystemType, SystemDelegate>> e = _classCleanupCallbacks.GetEnumerator();
            while (e.MoveNext())
            {
                CodeLoader.OnCleanupAllGeneralCodeLoaderHandler handler = e.Current.Value as CodeLoader.OnCleanupAllGeneralCodeLoaderHandler;
                Debugger.Assert(null != handler, "Invalid notice class cleanup handler.");

                handler.Invoke();
            }

            _classLoadCallbacks.Clear();
            _classCleanupCallbacks.Clear();
            _codeInfoLookupCallbacks.Clear();
        }

        /// <summary>
        /// 检测通知定义类指定的类型是否匹配当前加载器
        /// </summary>
        /// <param name="symClass">对象标记类型</param>
        /// <param name="filterType">过滤对象类型</param>
        /// <returns>若给定类型满足匹配规则则返回true，否则返回false</returns>
        [CodeLoader.OnGeneralCodeLoaderMatch]
        private static bool IsNoticeClassMatched(Symboling.SymClass symClass, SystemType filterType)
        {
            // 存在过滤类型，则直接对比过滤类型即可
            if (null != filterType)
            {
                return IsNoticeClassCallbackExist(filterType);
            }

            // 通知类必须为静态类
            if (false == symClass.IsStatic)
            {
                return false;
            }

            // IList<SystemAttribute> attrs = symClass.Attributes;
            IList<SystemType> attrTypes = symClass.FeatureTypes;
            for (int n = 0; null != attrTypes && n < attrTypes.Count; ++n)
            {
                SystemType attrType = attrTypes[n];
                // if (IsNoticeClassCallbackExist(attr.GetType()))
                if (IsNoticeClassCallbackExist(attrType))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 加载通知定义类指定的类型
        /// </summary>
        /// <param name="symClass">对象标记类型</param>
        /// <param name="reload">重载状态标识</param>
        /// <returns>若存在给定类型属性通知定义类则返回对应处理结果，否则返回false</returns>
        [CodeLoader.OnGeneralCodeLoaderLoad]
        private static bool LoadNoticeClass(Symboling.SymClass symClass, bool reload)
        {
            SystemDelegate callback = null;

            // IList<SystemAttribute> attrs = symClass.Attributes;
            IList<SystemType> attrTypes = symClass.FeatureTypes;
            for (int n = 0; null != attrTypes && n < attrTypes.Count; ++n)
            {
                SystemType attrType = attrTypes[n];
                // if (TryGetNoticeClassCallbackForTargetContainer(attr.GetType(), out callback, _classLoadCallbacks))
                if (TryGetNoticeClassCallbackForTargetContainer(attrType, out callback, _classLoadCallbacks))
                {
                    CodeLoader.OnGeneralCodeLoaderLoadHandler handler = callback as CodeLoader.OnGeneralCodeLoaderLoadHandler;
                    Debugger.Assert(null != handler, "Invalid notice class load handler.");
                    return handler.Invoke(symClass, reload);
                }
            }

            return false;
        }

        /// <summary>
        /// 查找通知定义类指定的类型对应的结构信息
        /// </summary>
        /// <param name="symClass">对象标记类型</param>
        /// <returns>返回类型对应的结构信息</returns>
        [CodeLoader.OnGeneralCodeLoaderLookup]
        private static Structuring.GeneralCodeInfo LookupNoticeCodeInfo(Symboling.SymClass symClass)
        {
            SystemDelegate callback = null;

            // IList<SystemAttribute> attrs = symClass.Attributes;
            IList<SystemType> attrTypes = symClass.FeatureTypes;
            for (int n = 0; null != attrTypes && n < attrTypes.Count; ++n)
            {
                SystemType attrType = attrTypes[n];
                // if (TryGetNoticeClassCallbackForTargetContainer(attr.GetType(), out callback, _codeInfoLookupCallbacks))
                if (TryGetNoticeClassCallbackForTargetContainer(attrType, out callback, _codeInfoLookupCallbacks))
                {
                    CodeLoader.OnGeneralCodeLoaderLookupHandler handler = callback as CodeLoader.OnGeneralCodeLoaderLookupHandler;
                    Debugger.Assert(null != handler, "Invalid notice class lookup handler.");
                    return handler.Invoke(symClass);
                }
            }

            return null;
        }

        /// <summary>
        /// 检测当前的回调管理容器中是否存在指定类型的通知定义回调
        /// </summary>
        /// <param name="targetType">对象类型</param>
        /// <returns>若存在给定类型对应的回调句柄则返回true，否则返回false</returns>
        private static bool IsNoticeClassCallbackExist(SystemType targetType)
        {
            IEnumerator<KeyValuePair<SystemType, SystemDelegate>> e = _classLoadCallbacks.GetEnumerator();
            while (e.MoveNext())
            {
                // 这里的属性类型允许继承，因此不能直接作相等比较
                if (e.Current.Key.IsAssignableFrom(targetType))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 通过指定的类型从通知定义类的回调管理容器中查找对应的回调句柄实例
        /// </summary>
        /// <param name="targetType">对象类型</param>
        /// <param name="callback">回调句柄</param>
        /// <param name="container">句柄列表容器</param>
        /// <returns>返回通过给定类型查找的回调句柄实例，若不存在则返回null</returns>
        private static bool TryGetNoticeClassCallbackForTargetContainer(SystemType targetType, out SystemDelegate callback, IDictionary<SystemType, SystemDelegate> container)
        {
            callback = null;

            IEnumerator<KeyValuePair<SystemType, SystemDelegate>> e = container.GetEnumerator();
            while (e.MoveNext())
            {
                // 这里的属性类型允许继承，因此不能直接作相等比较
                if (e.Current.Key.IsAssignableFrom(targetType))
                {
                    callback = e.Current.Value;
                    return true;
                }
            }

            return false;
        }
    }
}
