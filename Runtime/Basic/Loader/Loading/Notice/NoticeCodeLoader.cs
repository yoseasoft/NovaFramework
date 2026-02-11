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
using System.Customize.Extension;
using UnityEngine.Scripting;

namespace GameEngine.Loader
{
    /// <summary>
    /// 程序集中通知定义对象的分析处理类，对业务层载入的所有通知定义对象类进行统一加载及分析处理
    /// </summary>
    internal static partial class NoticeCodeLoader
    {
        /// <summary>
        /// 装配对象池管理类相关回调函数的管理容器
        /// </summary>
        private static readonly LoadingObjectCallbackCollector _collector = new LoadingObjectCallbackCollector();

        /// <summary>
        /// 初始化针对所有通知定义类声明的全部绑定回调接口
        /// </summary>
        [Preserve]
        [CodeLoader.OnGeneralCodeLoaderInit]
        private static void InitAllNoticeClassLoadingCallbacks()
        {
            _collector.OnInitializeContext(typeof(NoticeCodeLoader));
        }

        /// <summary>
        /// 清理针对所有通知定义类声明的全部绑定回调接口
        /// </summary>
        [Preserve]
        [CodeLoader.OnGeneralCodeLoaderCleanup]
        private static void CleanupAllNoticeClassLoadingCallbacks()
        {
            _collector.OnCleanupContext();
        }

        /// <summary>
        /// 检测通知定义类指定的类型是否匹配当前加载器
        /// </summary>
        /// <param name="symClass">对象标记类型</param>
        /// <param name="filterType">过滤对象类型</param>
        /// <returns>若给定类型满足匹配规则则返回true，否则返回false</returns>
        [Preserve]
        [CodeLoader.OnGeneralCodeLoaderMatch]
        private static bool IsNoticeClassMatched(Symbolling.SymClass symClass, Type filterType)
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

            // IList<Attribute> attrs = symClass.Attributes;
            IList<Type> attrTypes = symClass.FeatureTypes;
            for (int n = 0; null != attrTypes && n < attrTypes.Count; ++n)
            {
                Type attrType = attrTypes[n];
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
        [Preserve]
        [CodeLoader.OnGeneralCodeLoaderLoad]
        private static bool LoadNoticeClass(Symbolling.SymClass symClass, bool reload)
        {
            // IList<Attribute> attrs = symClass.Attributes;
            IList<Type> attrTypes = symClass.FeatureTypes;
            for (int n = 0; null != attrTypes && n < attrTypes.Count; ++n)
            {
                Type attrType = attrTypes[n];
                // if (_collector.TryLoadClass(IsMatchedNoticeClassFromTargetType, attr.GetType(), symClass, reload, out bool succeed))
                if (_collector.TryLoadClass(IsMatchedNoticeClassFromTargetType, attrType, symClass, reload, out bool succeed))
                {
                    return succeed;
                }
            }

            return false;
        }

        /// <summary>
        /// 查找通知定义类指定的类型对应的结构信息
        /// </summary>
        /// <param name="symClass">对象标记类型</param>
        /// <returns>返回类型对应的结构信息</returns>
        [Preserve]
        [CodeLoader.OnGeneralCodeLoaderLookup]
        private static Structuring.GeneralCodeInfo LookupNoticeCodeInfo(Symbolling.SymClass symClass)
        {
            // IList<Attribute> attrs = symClass.Attributes;
            IList<Type> attrTypes = symClass.FeatureTypes;
            for (int n = 0; null != attrTypes && n < attrTypes.Count; ++n)
            {
                Type attrType = attrTypes[n];
                // if (_collector.TryLookupCodeInfo(IsMatchedNoticeClassFromTargetType, attr.GetType(), symClass, out Structuring.GeneralCodeInfo codeInfo))
                if (_collector.TryLookupCodeInfo(IsMatchedNoticeClassFromTargetType, attrType, symClass, out Structuring.GeneralCodeInfo codeInfo))
                {
                    return codeInfo;
                }
            }

            return null;
        }

        /// <summary>
        /// 检测当前的回调管理容器中是否存在指定类型的通知定义回调
        /// </summary>
        /// <param name="targetType">对象类型</param>
        /// <returns>若存在给定类型对应的回调句柄则返回true，否则返回false</returns>
        private static bool IsNoticeClassCallbackExist(Type targetType)
        {
            foreach (Type classType in _collector.RelevanceTypes)
            {
                if (IsMatchedNoticeClassFromTargetType(targetType, classType))
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
        private static bool IsMatchedNoticeClassFromTargetType(Type targetType, Type classType)
        {
            // 这里的属性类型允许继承，因此不能直接作相等比较
            if (targetType.Is(classType))
            {
                return true;
            }

            return false;
        }
    }
}
