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

using SystemType = System.Type;
using SystemStringBuilder = System.Text.StringBuilder;

namespace GameEngine.Loader.Symboling
{
    /// <summary>
    /// Bean对象类的属性数据的结构信息
    /// </summary>
    public class BeanProperty : BeanMember, System.IEquatable<BeanProperty>
    {
        /// <summary>
        /// 属性对象的名称
        /// </summary>
        private string m_propertyName;
        /// <summary>
        /// 属性引用的实例类型
        /// </summary>
        private SystemType m_referenceClassType;
        /// <summary>
        /// 属性引用的实例名称
        /// </summary>
        private string m_referenceBeanName;
        /// <summary>
        /// 属性引用的对象实例
        /// </summary>
        private object m_referenceValue;

        public string PropertyName { get { return m_propertyName; } internal set { m_propertyName = value; } }
        public SystemType ReferenceClassType { get { return m_referenceClassType; } internal set { m_referenceClassType = value; } }
        public string ReferenceBeanName { get { return m_referenceBeanName; } internal set { m_referenceBeanName = value; } }
        public object ReferenceValue { get { return m_referenceValue; } internal set { m_referenceValue = value; } }

        /// <summary>
        /// 获取属性配置对应的属性标记实例
        /// </summary>
        public SymProperty SymProperty
        {
            get
            {
                Debugger.Assert(null != this.BeanObject && null != this.BeanObject.TargetClass, "The bean object instance must be non-null.");

                return this.BeanObject.TargetClass.GetPropertyByName(m_propertyName);
            }
        }

        public BeanProperty(Bean beanObject) : base(beanObject)
        { }

        ~BeanProperty()
        { }

        public override string ToString()
        {
            SystemStringBuilder sb = new SystemStringBuilder();
            sb.Append("{ ");
            sb.AppendFormat("PropertyName = {0}, ", m_propertyName);
            sb.AppendFormat("ReferenceClassType = {0}, ", NovaEngine.Utility.Text.ToString(m_referenceClassType));
            sb.AppendFormat("ReferenceBeanName = {0}, ", m_referenceBeanName);
            sb.AppendFormat("ReferenceValue = {0}, ", m_referenceValue?.ToString());
            sb.Append("}");
            return sb.ToString();
        }

        public override bool Equals(object obj)
        {
            if (obj is BeanProperty) { return Equals((BeanProperty) obj); }

            return false;
        }

        public bool Equals(BeanProperty other)
        {
            return null != m_propertyName && m_propertyName == other.m_propertyName;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + m_propertyName?.GetHashCode() ?? 0;
                hash = hash * 23 + m_referenceBeanName?.GetHashCode() ?? 0;
                hash = hash * 23 + m_referenceClassType?.GetHashCode() ?? 0;
                hash = hash * 23 + m_referenceValue?.GetHashCode() ?? 0;
                return hash;
            }
        }
    }
}
