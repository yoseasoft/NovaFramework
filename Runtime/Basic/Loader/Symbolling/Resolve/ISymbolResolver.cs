/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
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

namespace GameEngine.Loader.Symbolling
{
    /// <summary>
    /// 针对特性标签的符号解析接口定义
    /// </summary>
    public interface ISymbolResolverOfFeatureAttribute
    {
    }

    /// <summary>
    /// 针对可实例化对象类的符号解析接口定义
    /// </summary>
    public interface ISymbolResolverOfInstantiationClass
    {
        /// <summary>
        /// 检测目标对象类型是否匹配当前解析器
        /// </summary>
        /// <param name="targetType">对象类型</param>
        /// <returns>若目标对象类型匹配则返回true，否则返回false</returns>
        bool Matches(Type targetType);

        /// <summary>
        /// 指定符号对象类的解析函数
        /// </summary>
        /// <param name="symbol">符号对象实例</param>
        void Resolve(SymClass symbol);
    }
}
