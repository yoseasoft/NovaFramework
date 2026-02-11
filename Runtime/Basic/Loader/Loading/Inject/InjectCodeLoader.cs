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
using UnityEngine.Scripting;

namespace GameEngine.Loader
{
    /// <summary>
    /// 程序集中注入控制对象的分析处理类，对业务层载入的所有注入控制类进行统一加载及分析处理
    /// </summary>
    internal static partial class InjectCodeLoader
    {
        /// <summary>
        /// 装配对象池管理类相关回调函数的管理容器
        /// </summary>
        private static readonly LoadingObjectCallbackCollector _collector = new LoadingObjectCallbackCollector();

        /// <summary>
        /// 初始化针对所有注入控制类声明的全部绑定回调接口
        /// </summary>
        [Preserve]
        [CodeLoader.OnGeneralCodeLoaderInit]
        private static void InitAllInjectClassLoadingCallbacks()
        {
            _collector.OnInitializeContext(typeof(InjectCodeLoader));
        }

        /// <summary>
        /// 清理针对所有注入控制类声明的全部绑定回调接口
        /// </summary>
        [Preserve]
        [CodeLoader.OnGeneralCodeLoaderCleanup]
        private static void CleanupAllInjectClassLoadingCallbacks()
        {
            _collector.OnCleanupContext();
        }

        /// <summary>
        /// 检测注入控制类指定的类型是否匹配当前加载器
        /// </summary>
        /// <param name="symClass">对象标记类型</param>
        /// <param name="filterType">过滤对象类型</param>
        /// <returns>若给定类型满足匹配规则则返回true，否则返回false</returns>
        [Preserve]
        [CodeLoader.OnGeneralCodeLoaderMatch]
        private static bool IsInjectClassMatched(Symbolling.SymClass symClass, Type filterType)
        {
            // 存在过滤类型，则直接对比过滤类型即可
            if (null != filterType)
            {
                return IsInjectClassCallbackExist(filterType);
            }

            IList<Attribute> attrs = symClass.Attributes;
            for (int n = 0; null != attrs && n < attrs.Count; ++n)
            {
                Attribute attr = attrs[n];
                if (IsInjectClassCallbackExist(attr.GetType()))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 加载注入控制类指定的类型
        /// </summary>
        /// <param name="symClass">对象标记类型</param>
        /// <param name="reload">重载状态标识</param>
        /// <returns>若存在给定类型属性注入控制类则返回对应处理结果，否则返回false</returns>
        [Preserve]
        [CodeLoader.OnGeneralCodeLoaderLoad]
        private static bool LoadInjectClass(Symbolling.SymClass symClass, bool reload)
        {
            IList<Attribute> attrs = symClass.Attributes;
            for (int n = 0; null != attrs && n < attrs.Count; ++n)
            {
                Attribute attr = attrs[n];
                if (_collector.TryLoadClass(IsMatchedInjectClassFromTargetType, attr.GetType(), symClass, reload, out bool succeed))
                {
                    return succeed;
                }
            }

            return false;
        }

        /// <summary>
        /// 查找注入控制类指定的类型对应的结构信息
        /// </summary>
        /// <param name="symClass">对象标记类型</param>
        /// <returns>返回类型对应的结构信息</returns>
        [Preserve]
        [CodeLoader.OnGeneralCodeLoaderLookup]
        private static Structuring.GeneralCodeInfo LookupInjectCodeInfo(Symbolling.SymClass symClass)
        {
            IList<Attribute> attrs = symClass.Attributes;
            for (int n = 0; null != attrs && n < attrs.Count; ++n)
            {
                Attribute attr = attrs[n];
                if (_collector.TryLookupCodeInfo(IsMatchedInjectClassFromTargetType, attr.GetType(), symClass, out Structuring.GeneralCodeInfo codeInfo))
                {
                    return codeInfo;
                }
            }

            return null;
        }

        /// <summary>
        /// 检测当前的回调管理容器中是否存在指定类型的注入控制回调
        /// </summary>
        /// <param name="targetType">对象类型</param>
        /// <returns>若存在给定类型对应的回调句柄则返回true，否则返回false</returns>
        private static bool IsInjectClassCallbackExist(Type targetType)
        {
            foreach (Type classType in _collector.RelevanceTypes)
            {
                if (IsMatchedInjectClassFromTargetType(targetType, classType))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 检查目标类型与指定的对象类型是否匹配
        /// </summary>
        /// <param name="targetType">目标类型</param>
        /// <param name="classType">对象类型</param>
        /// <returns>若对象类型匹配则返回true，否则返回false</returns>
        private static bool IsMatchedInjectClassFromTargetType(Type targetType, Type classType)
        {
            // 这里的类型为属性定义的类型，因此直接作相等比较即可
            if (classType == targetType)
            {
                return true;
            }

            return false;
        }
    }
}
