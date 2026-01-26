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
using System.Customize.Extension;

namespace GameEngine.Loader.Symbolling
{
    /// 标记对象的解析类
    internal static partial class SymClassResolver
    {
        /// <summary>
        /// 自动填充实体类型的系统对象的标记类的特性
        /// </summary>
        /// <param name="symClass">类标记对象</param>
        private static void AutoFillEntityExtensionMethodFeatures(SymClass symClass, SymMethod symMethod)
        {
            if (symMethod.ExtensionParameterType.Is<CView>())
            {
                AutoFillViewExtensionMethodFeatures(symClass, symMethod);
            }
        }

        /// <summary>
        /// 自动填充视图类型的系统对象的标记类的特性
        /// </summary>
        /// <param name="symClass">类标记对象</param>
        private static void AutoFillViewExtensionMethodFeatures(SymClass symClass, SymMethod symMethod)
        {
            if (symMethod.HasAttribute(typeof(CViewNoticeCallAttribute)))
            {
                // 装配通知支持
                AutobindFeatureTypeForTargetSymbol(symClass, typeof(NoticeSupportedOnViewAttribute));
            }
        }
    }
}
