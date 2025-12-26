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

namespace GameEngine.Loader.Symboling
{
    /// 标记对象的解析类
    internal static partial class SymClassResolver
    {
        /// <summary>
        /// 自定义类型解析流程的初始化函数
        /// </summary>
        static void OnCustomizeInitialize()
        {
            // 自定义可实例化对象解析业务初始化
            OnCustomizeInstantiationInitialize();
        }

        /// <summary>
        /// 自定义类型解析流程的清理函数
        /// </summary>
        static void OnCustomizeCleanup()
        {
            // 自定义可实例化对象解析业务清理
            OnCustomizeInstantiationCleanup();
        }

        /// <summary>
        /// 对标记类进行个性化定制
        /// </summary>
        /// <param name="symClass">类标记对象</param>
        static void DoPersonalizedCustomizationOfClass(SymClass symClass)
        {
            // 特性定制
            AutoFillClassFeatures(symClass);
        }

        /// <summary>
        /// 自动填充标记类的特性
        /// </summary>
        /// <param name="symClass">类标记对象</param>
        static void AutoFillClassFeatures(SymClass symClass)
        {
            if (symClass.IsStatic)
            {
                // 逻辑类的自动填充流程
                AutoFillSystemClassFeatures(symClass);
            }
            else
            {
                // 对象类的自动填充流程
                AutoFillInstantiationClassFeatures(symClass);
            }
        }

        /// <summary>
        /// 给目标符号类绑定指定的特性标签
        /// 与<see cref="AutobindFeatureTypeForTargetSymbol(SymClass, Type)"/>接口的区别在于，
        /// 该接口是针对目标符号的类型进行特性绑定的，此类型并不一定是特定的符号类，也可能是某个抽象父类或接口。
        /// 当传入类型不是一个可实例化的类型时，将对该类型的所有子类或实现类进行特性绑定
        /// </summary>
        /// <param name="targetType">符号类型</param>
        /// <param name="featureType">特性类型</param>
        private static void AutobindFeatureTypeForTargetSymbol(Type targetType, Type featureType)
        {
            // 通过目标类型，获取到对应的符号类实例
            SymClass symClass = CodeLoader.GetSymClassByType(targetType);
            Debugger.Assert(null != symClass, "Invalid target symbol type.");

            AutobindFeatureTypeForTargetSymbol(symClass, featureType);
        }

        /// <summary>
        /// 给目标符号类绑定指定的特性标签
        /// </summary>
        /// <param name="symClass">符号实例</param>
        /// <param name="featureType">特性类型</param>
        private static void AutobindFeatureTypeForTargetSymbol(SymClass symClass, Type featureType)
        {
            Debugger.Log(LogGroupTag.CodeLoader, "对象类型解析：目标符号类型‘{%s}’动态绑定新的特性‘{%t}’成功。", symClass.ClassName, featureType);

            // 为符号类添加特性
            symClass.AddFeatureType(featureType);
        }
    }
}
