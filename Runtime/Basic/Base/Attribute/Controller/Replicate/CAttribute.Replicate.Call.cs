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
using System.Customize.Extension;

namespace GameEngine
{
    /// <summary>
    /// 同步转发类型注册函数的属性类型定义
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public class OnReplicateDispatchCallAttribute : Attribute
    {
        /// <summary>
        /// 自动同步的目标对象类型
        /// </summary>
        private readonly Type _classType;
        /// <summary>
        /// 自动同步的数据标签链条
        /// </summary>
        private readonly string _tags;
        /// <summary>
        /// 自动同步的数据播报类型
        /// </summary>
        private readonly ReplicateAnnounceType _announceType;
        /// <summary>
        /// 自动同步的观察行为类型
        /// </summary>
        private readonly AspectBehaviourType _behaviourType;

        /// <summary>
        /// 目标对象类型获取函数
        /// </summary>
        public Type ClassType => _classType;
        /// <summary>
        /// 数据标签链条获取函数
        /// </summary>
        public string Tags => _tags;
        /// <summary>
        /// 数据播报类型获取函数
        /// </summary>
        public ReplicateAnnounceType AnnounceType => _announceType;
        /// <summary>
        /// 观察行为类型获取函数
        /// </summary>
        public AspectBehaviourType BehaviourType => _behaviourType;

        public OnReplicateDispatchCallAttribute(string tags)
            : this(tags, AspectBehaviour.DefaultBehaviourTypeForAutomaticallyDispatchedProcessingNode)
        { }

        public OnReplicateDispatchCallAttribute(string tags, AspectBehaviourType behaviourType)
            : this(null, tags, ReplicateAnnounceType.All, behaviourType)
        { }

        public OnReplicateDispatchCallAttribute(string tags, ReplicateAnnounceType announceType)
            : this(tags, announceType, AspectBehaviour.DefaultBehaviourTypeForAutomaticallyDispatchedProcessingNode)
        { }

        public OnReplicateDispatchCallAttribute(string tags, ReplicateAnnounceType announceType, AspectBehaviourType behaviourType)
            : this(null, tags, announceType, behaviourType)
        { }

        public OnReplicateDispatchCallAttribute(Type classType, string tags)
            : this(classType, tags, ReplicateAnnounceType.All, AspectBehaviourType.Unknown)
        { }

        public OnReplicateDispatchCallAttribute(Type classType, string tags, ReplicateAnnounceType announceType)
            : this(classType, tags, announceType, AspectBehaviourType.Unknown)
        { }

        private OnReplicateDispatchCallAttribute(Type classType, string tags, ReplicateAnnounceType announceType, AspectBehaviourType behaviourType) : base()
        {
            _classType = classType;
            _tags = tags;
            _announceType = announceType;
            _behaviourType = behaviourType;
        }

        /// <summary>
        /// 重写 Equals 方法，基于值进行比较
        /// </summary>
        public override bool Equals(object obj)
        {
            // 1. 检查是否为同一个对象（引用相等）
            if (ReferenceEquals(this, obj)) return true;
            // 2. 检查对象是否为 null 或类型不同
            if (obj is null || this.GetType() != obj.GetType()) return false;

            // 3. 转换为当前类型后，比较所有关键字段的值
            OnReplicateDispatchCallAttribute other = (OnReplicateDispatchCallAttribute) obj;
            return _classType == other._classType &&
                   _tags.EqualsIgnoreCase(other._tags) && // _tags == other._tags &&
                   _announceType == other._announceType; // && _behaviourType == other._behaviourType;
        }

        /// <summary>
        /// 重写 GetHashCode 方法，必须与 Equals 逻辑保持一致
        /// </summary>
        public override int GetHashCode()
        {
            // 使用 HashCode.Combine 来组合多个字段的哈希值，这是一个推荐的做法
            return HashCode.Combine(_classType, _tags, _announceType); // , _behaviourType);
        }
    }
}
