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

using SystemType = System.Type;
using SystemAttribute = System.Attribute;
using SystemAttributeUsageAttribute = System.AttributeUsageAttribute;
using SystemAttributeTargets = System.AttributeTargets;

namespace GameEngine
{
    /// <summary>
    /// 实体自动挂载的目标组件的属性类型定义
    /// </summary>
    [SystemAttributeUsage(SystemAttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class EntityActivationComponentAttribute : SystemAttribute
    {
        /// <summary>
        /// 组件引用对象类型
        /// </summary>
        private readonly SystemType _referenceType;
        /// <summary>
        /// 组件引用实体名称
        /// </summary>
        private readonly string _referenceName;
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
        public SystemType ReferenceType => _referenceType;
        /// <summary>
        /// 组件引用名称获取函数
        /// </summary>
        public string ReferenceName => _referenceName;
        /// <summary>
        /// 组件优先级获取函数
        /// </summary>
        public int Priority => _priority;
        /// <summary>
        /// 组件激活行为类型获取函数
        /// </summary>
        public AspectBehaviourType ActivationBehaviourType => _activationBehaviourType;

        public EntityActivationComponentAttribute(SystemType referenceType) : this(referenceType, null, 0, AspectBehaviourType.Initialize)
        {
        }

        public EntityActivationComponentAttribute(SystemType referenceType, int priority) : this(referenceType, null, priority, AspectBehaviourType.Initialize)
        {
        }

        public EntityActivationComponentAttribute(SystemType referenceType, int priority, AspectBehaviourType activationBehaviourType) : this(referenceType, null, priority, activationBehaviourType)
        {
        }

        public EntityActivationComponentAttribute(string referenceName) : this(null, referenceName, 0, AspectBehaviourType.Initialize)
        {
        }

        public EntityActivationComponentAttribute(string referenceName, int priority) : this(null, referenceName, priority, AspectBehaviourType.Initialize)
        {
        }

        public EntityActivationComponentAttribute(string referenceName, int priority, AspectBehaviourType activationBehaviourType) : this(null, referenceName, priority, activationBehaviourType)
        {
        }

        protected EntityActivationComponentAttribute(SystemType referenceType, string referenceName, int priority, AspectBehaviourType activationBehaviourType) : base()
        {
            _referenceType = referenceType;
            _referenceName = referenceName;
            _priority = priority;
            _activationBehaviourType = activationBehaviourType;
        }
    }

    /// <summary>
    /// 实体自动挂载的目标组件的属性类型定义
    /// </summary>
    [SystemAttributeUsage(SystemAttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class EntityActivationComponentOfTargetAttribute : EntityActivationComponentAttribute
    {
        /// <summary>
        /// 组件激活的目标实体名称
        /// </summary>
        private readonly string _targetBeanName;

        /// <summary>
        /// 组件激活的目标实体名称获取函数
        /// </summary>
        public string TargetBeanName => _targetBeanName;

        public EntityActivationComponentOfTargetAttribute(string beanName, SystemType referenceType) : this(beanName, referenceType, null, 0, AspectBehaviourType.Initialize)
        {
        }

        public EntityActivationComponentOfTargetAttribute(string beanName, SystemType referenceType, int priority) : this(beanName, referenceType, null, priority, AspectBehaviourType.Initialize)
        {
        }

        public EntityActivationComponentOfTargetAttribute(string beanName, SystemType referenceType, int priority, AspectBehaviourType activationBehaviourType) : this(beanName, referenceType, null, priority, activationBehaviourType)
        {
        }

        public EntityActivationComponentOfTargetAttribute(string beanName, string referenceName) : this(beanName, null, referenceName, 0, AspectBehaviourType.Initialize)
        {
        }

        public EntityActivationComponentOfTargetAttribute(string beanName, string referenceName, int priority) : this(beanName, null, referenceName, priority, AspectBehaviourType.Initialize)
        {
        }

        public EntityActivationComponentOfTargetAttribute(string beanName, string referenceName, int priority, AspectBehaviourType activationBehaviourType) : this(beanName, null, referenceName, priority, activationBehaviourType)
        {
        }

        private EntityActivationComponentOfTargetAttribute(string beanName, SystemType referenceType, string referenceName, int priority, AspectBehaviourType activationBehaviourType) : base(referenceType, referenceName, priority, activationBehaviourType)
        {
            _targetBeanName = beanName;
        }
    }
}
