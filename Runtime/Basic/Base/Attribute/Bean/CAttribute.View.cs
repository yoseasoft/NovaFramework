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

using SystemAttribute = System.Attribute;
using SystemAttributeUsageAttribute = System.AttributeUsageAttribute;
using SystemAttributeTargets = System.AttributeTargets;

namespace GameEngine
{
    /// <summary>
    /// 视图实现类声明属性类型定义
    /// </summary>
    [SystemAttributeUsage(SystemAttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class DeclareViewClassAttribute : SystemAttribute
    {
        /// <summary>
        /// 视图名称
        /// </summary>
        private readonly string _viewName;
        /// <summary>
        /// 视图优先级
        /// </summary>
        private readonly int _priority;

        /// <summary>
        /// 视图名称获取函数
        /// </summary>
        public string ViewName => _viewName;

        /// <summary>
        /// 视图优先级获取函数
        /// </summary>
        public int Priority => _priority;

        public DeclareViewClassAttribute(string viewName) : this(viewName, 0)
        {
        }

        public DeclareViewClassAttribute(int priority) : this(string.Empty, priority)
        {
        }

        public DeclareViewClassAttribute(string viewName, int priority) : base()
        {
            _viewName = viewName ?? string.Empty;
            _priority = priority;
        }
    }

    /// <summary>
    /// 视图共生关系组的属性类型定义
    /// </summary>
    [SystemAttributeUsage(SystemAttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class ViewGroupOfSymbioticRelationshipsAttribute : SystemAttribute
    {
        /// <summary>
        /// 视图名称标识
        /// </summary>
        private readonly string _viewName;

        /// <summary>
        /// 视图名称获取函数
        /// </summary>
        public string ViewName => _viewName;

        public ViewGroupOfSymbioticRelationshipsAttribute(string viewName) : base()
        {
            _viewName = viewName;
        }
    }
}
