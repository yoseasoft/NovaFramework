/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyright (C) 2023 - 2024, Guangzhou Shiyue Network Technology Co., Ltd.
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
    /// Bean对象类的字段数据的结构信息
    /// </summary>
    public class BeanField : BeanMember, System.IEquatable<BeanField>
    {
        /// <summary>
        /// 字段对象的名称
        /// </summary>
        private string _fieldName;
        /// <summary>
        /// 字段引用的实例类型
        /// </summary>
        private SystemType _referenceClassType;
        /// <summary>
        /// 字段引用的实例名称
        /// </summary>
        private string _referenceBeanName;
        /// <summary>
        /// 字段引用的对象实例
        /// </summary>
        private object _referenceValue;

        public string FieldName { get { return _fieldName; } internal set { _fieldName = value; } }
        public SystemType ReferenceClassType { get { return _referenceClassType; } internal set { _referenceClassType = value; } }
        public string ReferenceBeanName { get { return _referenceBeanName; } internal set { _referenceBeanName = value; } }
        public object ReferenceValue { get { return _referenceValue; } internal set { _referenceValue = value; } }

        /// <summary>
        /// 获取字段配置对应的字段标记实例
        /// </summary>
        public SymField SymField
        {
            get
            {
                Debugger.Assert(null != this.BeanObject && null != this.BeanObject.TargetClass, "The bean object instance must be non-null.");

                return this.BeanObject.TargetClass.GetFieldByName(_fieldName);
            }
        }

        public BeanField(Bean beanObject) : base(beanObject)
        { }

        ~BeanField()
        { }

        public override bool Equals(object obj)
        {
            if (obj is BeanField) { return Equals((BeanField) obj); }

            return false;
        }

        public bool Equals(BeanField other)
        {
            return null != _fieldName && _fieldName == other._fieldName;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + _fieldName?.GetHashCode() ?? 0;
                hash = hash * 23 + _referenceBeanName?.GetHashCode() ?? 0;
                hash = hash * 23 + _referenceClassType?.GetHashCode() ?? 0;
                hash = hash * 23 + _referenceValue?.GetHashCode() ?? 0;
                return hash;
            }
        }
    }
}
