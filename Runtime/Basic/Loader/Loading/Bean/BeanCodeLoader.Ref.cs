/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyright (C) 2023 - 2024, Guangzhou Shiyue Network Technology Co., Ltd.
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

using SystemType = System.Type;
using SystemAttribute = System.Attribute;

namespace GameEngine.Loader
{
    /// <summary>
    /// 程序集中原型对象的分析处理类，对业务层载入的所有原型对象类进行统一加载及分析处理
    /// </summary>
    internal static partial class BeanCodeLoader
    {
        /// <summary>
        /// 引用对象指定类型的属性解析接口
        /// </summary>
        /// <param name="symClass">对象标记类型</param>
        /// <param name="codeInfo">对象结构信息</param>
        /// <param name="attribute">属性对象</param>
        private static void LoadRefClassByAttributeType(Symboling.SymClass symClass, Structuring.RefCodeInfo codeInfo, SystemAttribute attribute)
        {
            LoadBaseClassByAttributeType(symClass, codeInfo, attribute);
        }

        /// <summary>
        /// 引用对象指定函数的属性解析接口
        /// </summary>
        /// <param name="symClass">对象标记类型</param>
        /// <param name="codeInfo">对象结构信息</param>
        /// <param name="symMethod">函数标记对象</param>
        /// <param name="attribute">属性对象</param>
        private static void LoadRefMethodByAttributeType(Symboling.SymClass symClass, Structuring.RefCodeInfo codeInfo, Symboling.SymMethod symMethod, SystemAttribute attribute)
        {

            LoadBaseMethodByAttributeType(symClass, codeInfo, symMethod, attribute);
        }
    }
}
