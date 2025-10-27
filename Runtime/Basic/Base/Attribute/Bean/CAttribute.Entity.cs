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
    /// 实体类声明属性类型定义
    /// </summary>
    [SystemAttributeUsage(SystemAttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public abstract class DeclareEntityClassAttribute : SystemAttribute
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

        protected DeclareEntityClassAttribute(string name, int priority) : base()
        {
            _name = name ?? string.Empty;
            _priority = priority;
        }
    }

    /// <summary>
    /// 实体自动挂载的目标组件的属性类型定义
    /// </summary>
    [SystemAttributeUsage(SystemAttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class CEntityAutomaticActivationComponentAttribute : SystemAttribute
    {
        /// <summary>
        /// 组件引用对象类型
        /// </summary>
        private readonly SystemType _referenceType;
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
        /// 组件优先级获取函数
        /// </summary>
        public int Priority => _priority;
        /// <summary>
        /// 组件激活行为类型获取函数
        /// </summary>
        public AspectBehaviourType ActivationBehaviourType => _activationBehaviourType;

        public CEntityAutomaticActivationComponentAttribute(SystemType referenceType) : this(referenceType, 0, AspectBehaviourType.Initialize)
        {
        }

        public CEntityAutomaticActivationComponentAttribute(SystemType referenceType, int priority) : this(referenceType, priority, AspectBehaviourType.Initialize)
        {
        }

        public CEntityAutomaticActivationComponentAttribute(SystemType referenceType, int priority, AspectBehaviourType activationBehaviourType) : base()
        {
            _referenceType = referenceType;
            _priority = priority;
            _activationBehaviourType = activationBehaviourType;
        }
    }
}
