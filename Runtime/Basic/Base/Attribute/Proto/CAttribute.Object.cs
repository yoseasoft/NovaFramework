/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyright (C) 2024 - 2025, Hurley, Independent Studio.
/// Copyright (C) 2025, Hainan Yuanyou Information Tecdhnology Co., Ltd. Guangzhou Branch
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

using SystemAttribute = System.Attribute;
using SystemAttributeUsageAttribute = System.AttributeUsageAttribute;
using SystemAttributeTargets = System.AttributeTargets;

namespace GameEngine
{
    /// <summary>
    /// 对象实现类声明属性类型定义
    /// </summary>
    [SystemAttributeUsage(SystemAttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class DeclareObjectClassAttribute : SystemAttribute
    {
        /// <summary>
        /// 对象名称
        /// </summary>
        private readonly string _objectName;
        /// <summary>
        /// 对象功能类型标识
        /// </summary>
        private readonly int _funcType;

        /// <summary>
        /// 对象名称获取函数
        /// </summary>
        public string ObjectName => _objectName;

        /// <summary>
        /// 对象功能类型获取函数
        /// </summary>
        public int FuncType => _funcType;

        public DeclareObjectClassAttribute(string objectName) : this(objectName, 0)
        {
        }

        public DeclareObjectClassAttribute(int funcType) : this(string.Empty, funcType)
        {
        }

        public DeclareObjectClassAttribute(string objectName, int funcType) : base()
        {
            _objectName = objectName ?? string.Empty;
            _funcType = funcType;
        }
    }
}
