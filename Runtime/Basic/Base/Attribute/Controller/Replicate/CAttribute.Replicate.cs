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
    /// 对象的数据同步功能支持的属性类型定义
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class ReplicateSupportedAttribute : Attribute
    {
        public ReplicateSupportedAttribute() : base() { }
    }

    /// <summary>
    /// 数据同步的目标实体对象启用标记的属性类型定义
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class OnReplicateObjectAttribute : Attribute
    {
        /// <summary>
        /// 同步数据的关联标签
        /// </summary>
        private readonly string _tag;

        /// <summary>
        /// 获取同步数据的关联标签
        /// </summary>
        public string Tag => _tag;

        public OnReplicateObjectAttribute() : this(null) { }

        public OnReplicateObjectAttribute(string tag) : base()
        {
            _tag = tag;
        }
    }

    /// <summary>
    /// 数据同步的目标实体对象唯一标识的属性类型定义
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class OnReplicateIdAttribute : Attribute
    {
        public OnReplicateIdAttribute() : base() { }
    }

    /// <summary>
    /// 数据同步的目标对象字段启用标记的属性类型定义
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class OnReplicateFieldAttribute : Attribute
    {
        /// <summary>
        /// 同步数据的关联标签
        /// </summary>
        private readonly string _tag;

        /// <summary>
        /// 获取同步数据的关联标签
        /// </summary>
        public string Tag => _tag;

        public OnReplicateFieldAttribute() : this(null) { }

        public OnReplicateFieldAttribute(string tag) : base()
        {
            _tag = tag;
        }
    }
}
