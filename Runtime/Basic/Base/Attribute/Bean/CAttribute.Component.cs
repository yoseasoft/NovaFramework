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

namespace GameEngine
{
    /// <summary>
    /// 组件实现类声明属性类型定义
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class CComponentClassAttribute : Attribute
    {
        /// <summary>
        /// 组件名称
        /// </summary>
        private readonly string _componentName;
        /// <summary>
        /// 组件优先级
        /// </summary>
        private readonly int _priority;

        /// <summary>
        /// 组件名称获取函数
        /// </summary>
        public string ComponentName => _componentName;

        /// <summary>
        /// 组件优先级获取函数
        /// </summary>
        public int Priority => _priority;

        public CComponentClassAttribute(string componentName) : this(componentName, 0)
        {
        }

        public CComponentClassAttribute(int priority) : this(null, priority)
        {
        }

        public CComponentClassAttribute(string componentName, int priority) : base()
        {
            _componentName = componentName ?? string.Empty;
            _priority = priority;
        }
    }

    /// <summary>
    /// 组件依赖关联声明属性类型定义
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class CComponentDependenceAttribute : Attribute
    {
        /// <summary>
        /// 组件依赖关联类型
        /// </summary>
        private readonly Type _dependenceType;

        /// <summary>
        /// 组件依赖关联类型获取函数
        /// </summary>
        public Type DependenceType => _dependenceType;

        public CComponentDependenceAttribute(Type dependenceType) : base()
        {
            _dependenceType = dependenceType;
        }
    }
}
