/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyright (C) 2025 - 2026, Hainan Yuanyou Information Technology Co., Ltd. Guangzhou Branch
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
    /// 数据同步的目标实体对象启用标记的属性类型定义
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public abstract class OnReplicateObjectAttribute : Attribute
    {
        /// <summary>
        /// 同步数据的关联标签
        /// </summary>
        private readonly string _tag;

        /// <summary>
        /// 获取同步数据的关联标签
        /// </summary>
        public string Tag => _tag;

        protected OnReplicateObjectAttribute() : this(null) { }

        protected OnReplicateObjectAttribute(string tag) : base()
        {
            _tag = tag;
        }
    }

    /// <summary>
    /// 数据同步的目标对象类型启用标记的属性类型定义
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class OnReplicateClassAttribute : OnReplicateObjectAttribute
    {
        public OnReplicateClassAttribute() : base() { }

        public OnReplicateClassAttribute(string tag) : base(tag) { }
    }

    /// <summary>
    /// 数据同步的目标实体对象唯一标识的属性类型定义
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class OnReplicateIdAttribute : OnReplicateObjectAttribute
    {
        public OnReplicateIdAttribute() : base() { }

        public OnReplicateIdAttribute(string tag) : base(tag) { }
    }

    /// <summary>
    /// 数据同步的目标对象字段启用标记的属性类型定义
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public sealed class OnReplicateFieldAttribute : OnReplicateObjectAttribute
    {
        public OnReplicateFieldAttribute() : base() { }

        public OnReplicateFieldAttribute(string tag) : base(tag) { }
    }

    /// <summary>
    /// 数据同步的目标对象属性启用标记的属性类型定义
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class OnReplicatePropertyAttribute : OnReplicateObjectAttribute
    {
        public OnReplicatePropertyAttribute() : base() { }

        public OnReplicatePropertyAttribute(string tag) : base(tag) { }
    }
}
