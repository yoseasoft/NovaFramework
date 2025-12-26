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
    /// 实体类声明属性类型定义
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public abstract class CEntityDeclareClassAttribute : Attribute
    {
        /// <summary>
        /// 实体名称
        /// </summary>
        private readonly string _name;
        /// <summary>
        /// 实体优先级
        /// </summary>
        private readonly int _priority;

        /// <summary>
        /// 实体名称获取函数
        /// </summary>
        protected internal string Name => _name;

        /// <summary>
        /// 实体优先级获取函数
        /// </summary>
        protected internal int Priority => _priority;

        protected CEntityDeclareClassAttribute(string name, int priority) : base()
        {
            _name = name ?? string.Empty;
            _priority = priority;
        }
    }

    /// <summary>
    /// 实体自动挂载的目标组件的属性类型定义
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class CComponentAutomaticActivationOfEntityAttribute : Attribute
    {
        /// <summary>
        /// 组件引用对象类型
        /// </summary>
        private readonly Type _referenceType;
        /// <summary>
        /// 组件优先级
        /// </summary>
        private readonly int _priority;
        /// <summary>
        /// 组件的激活行为类型
        /// </summary>
        private readonly AspectBehaviourType _activationBehaviourType;

        /// <summary>
        /// 组件引用类型获取函数
        /// </summary>
        public Type ReferenceType => _referenceType;
        /// <summary>
        /// 组件优先级获取函数
        /// </summary>
        public int Priority => _priority;
        /// <summary>
        /// 组件激活行为类型获取函数
        /// </summary>
        public AspectBehaviourType ActivationBehaviourType => _activationBehaviourType;

        public CComponentAutomaticActivationOfEntityAttribute(Type referenceType) : this(referenceType, 0, AspectBehaviourType.Initialize)
        {
        }

        public CComponentAutomaticActivationOfEntityAttribute(Type referenceType, int priority) : this(referenceType, priority, AspectBehaviourType.Initialize)
        {
        }

        public CComponentAutomaticActivationOfEntityAttribute(Type referenceType, int priority, AspectBehaviourType activationBehaviourType) : base()
        {
            _referenceType = referenceType;
            _priority = priority;
            _activationBehaviourType = activationBehaviourType;
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
            CComponentAutomaticActivationOfEntityAttribute other = (CComponentAutomaticActivationOfEntityAttribute) obj;
            return _referenceType == other._referenceType &&
                   _priority == other._priority &&
                   _activationBehaviourType == other._activationBehaviourType;
        }

        /// <summary>
        /// 重写 GetHashCode 方法，必须与 Equals 逻辑保持一致
        /// </summary>
        public override int GetHashCode()
        {
            // 使用 HashCode.Combine 来组合多个字段的哈希值，这是一个推荐的做法
            return HashCode.Combine(_referenceType, _priority, _activationBehaviourType);
        }
    }
}
