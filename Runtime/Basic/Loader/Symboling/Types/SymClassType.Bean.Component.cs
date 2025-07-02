/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyring (C) 2023 - 2024, Guangzhou Shiyue Network Technology Co., Ltd.
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

using System.Collections.Generic;

using SystemType = System.Type;
using SystemStringBuilder = System.Text.StringBuilder;

namespace GameEngine.Loader.Symboling
{
    /// <summary>
    /// Bean对象类的组件数据的结构信息
    /// </summary>
    public class BeanComponent : BeanMember, System.IEquatable<BeanComponent>
    {
        /// <summary>
        /// 组件引用的实例类型
        /// </summary>
        private SystemType m_referenceClassType;
        /// <summary>
        /// 组件引用的实体名称
        /// </summary>
        private string m_referenceBeanName;
        /// <summary>
        /// 组件的调度优先级
        /// </summary>
        private int m_priority;
        /// <summary>
        /// 组件的激活行为类型
        /// </summary>
        private AspectBehaviourType m_activationBehaviourType;

        public SystemType ReferenceClassType { get { return m_referenceClassType; } internal set { m_referenceClassType = value; } }
        public string ReferenceBeanName { get { return m_referenceBeanName; } internal set { m_referenceBeanName = value; } }
        public int Priority { get { return m_priority; } internal set { m_priority = value; } }
        public AspectBehaviourType ActivationBehaviourType { get { return m_activationBehaviourType; } internal set { m_activationBehaviourType = value; } }

        public BeanComponent(Bean beanObject) : base(beanObject)
        { }

        ~BeanComponent()
        { }

        public override bool Equals(object obj)
        {
            if (obj is BeanComponent) { return Equals((BeanComponent) obj); }

            return false;
        }

        public bool Equals(BeanComponent other)
        {
            return (null != m_referenceClassType && m_referenceClassType == other.m_referenceClassType) ||
                   (null != m_referenceBeanName && m_referenceBeanName.Equals(other.m_referenceBeanName));
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + m_referenceClassType?.GetHashCode() ?? 0;
                hash = hash * 23 + m_referenceBeanName?.GetHashCode() ?? 0;
                hash = hash * 23 + m_priority;
                hash = hash * 23 + m_activationBehaviourType.GetHashCode();
                return hash;
            }
        }
    }
}
