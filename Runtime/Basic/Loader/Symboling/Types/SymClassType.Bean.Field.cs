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
    /// Bean对象类的字段数据的结构信息
    /// </summary>
    public class BeanField : BeanMember, System.IEquatable<BeanField>
    {
        /// <summary>
        /// 字段对象的名称
        /// </summary>
        private string m_fieldName;
        /// <summary>
        /// 字段引用的实例类型
        /// </summary>
        private SystemType m_referenceClassType;
        /// <summary>
        /// 字段引用的实例名称
        /// </summary>
        private string m_referenceBeanName;
        /// <summary>
        /// 字段引用的对象实例
        /// </summary>
        private object m_referenceValue;

        public string FieldName { get { return m_fieldName; } internal set { m_fieldName = value; } }
        public SystemType ReferenceClassType { get { return m_referenceClassType; } internal set { m_referenceClassType = value; } }
        public string ReferenceBeanName { get { return m_referenceBeanName; } internal set { m_referenceBeanName = value; } }
        public object ReferenceValue { get { return m_referenceValue; } internal set { m_referenceValue = value; } }

        /// <summary>
        /// 获取字段配置对应的字段标记实例
        /// </summary>
        public SymField SymField
        {
            get
            {
                Debugger.Assert(null != this.BeanObject && null != this.BeanObject.TargetClass, "The bean object instance must be non-null.");

                return this.BeanObject.TargetClass.GetFieldByName(m_fieldName);
            }
        }

        public BeanField(Bean beanObject) : base(beanObject)
        { }

        ~BeanField()
        { }

        public override string ToString()
        {
            SystemStringBuilder sb = new SystemStringBuilder();
            sb.Append("{ ");
            sb.AppendFormat("FieldName = {0}, ", m_fieldName);
            sb.AppendFormat("ReferenceClassType = {0}, ", NovaEngine.Utility.Text.ToString(m_referenceClassType));
            sb.AppendFormat("ReferenceBeanName = {0}, ", m_referenceBeanName);
            sb.AppendFormat("ReferenceValue = {0}, ", m_referenceValue?.ToString());
            sb.Append("}");
            return sb.ToString();
        }

        public override bool Equals(object obj)
        {
            if (obj is BeanField) { return Equals((BeanField) obj); }

            return false;
        }

        public bool Equals(BeanField other)
        {
            return null != m_fieldName && m_fieldName == other.m_fieldName;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + m_fieldName?.GetHashCode() ?? 0;
                hash = hash * 23 + m_referenceBeanName?.GetHashCode() ?? 0;
                hash = hash * 23 + m_referenceClassType?.GetHashCode() ?? 0;
                hash = hash * 23 + m_referenceValue?.GetHashCode() ?? 0;
                return hash;
            }
        }
    }
}
