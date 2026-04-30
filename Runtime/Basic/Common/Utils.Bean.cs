/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyright (C) 2026, Hurley, Independent Studio.
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
using System.Customize.Extension;

namespace GameEngine
{
    /// 框架内部的辅助工具类
    static partial class Utils
    {
        /// <summary>
        /// 通过指定的对象类型获取与之匹配的实体分类标签，<br/>
        /// 若类型不属于实体对象的子类，则返回<see cref="CBeanClassificationLabel.Unknown"/>值
        /// </summary>
        /// <param name="classType">对象类型</param>
        /// <returns>返回该类型匹配的实体分类标签</returns>
        public static CBeanClassificationLabel GetBeanClassificationLabelByClassType(Type classType)
        {
            return classType switch
            {
                _ when classType.Is<CObject>()    => CBeanClassificationLabel.Object,
                _ when classType.Is<CScene>()     => CBeanClassificationLabel.Scene,
                _ when classType.Is<CActor>()     => CBeanClassificationLabel.Actor,
                _ when classType.Is<CView>()      => CBeanClassificationLabel.View,
                _ when classType.Is<CComponent>() => CBeanClassificationLabel.Component,
                _                                 => CBeanClassificationLabel.Unknown
            };
        }
    }
}
