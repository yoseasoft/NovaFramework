/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyright (C) 2024 - 2025, Hurley, Independent Studio.
/// Copyright (C) 2025, Hainan Yuanyou Information Technology Co., Ltd. Guangzhou Branch
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

namespace GameEngine.Loader.Symboling
{
    /// <summary>
    /// Bean对象类的属性数据的结构信息
    /// </summary>
    public class BeanProperty : BeanMember, IEquatable<BeanProperty>
    {
        /// <summary>
        /// 属性对象的名称
        /// </summary>
        private string _propertyName;
        /// <summary>
        /// 属性引用的实例类型
        /// </summary>
        private Type _referenceClassType;
        /// <summary>
        /// 属性引用的实例名称
        /// </summary>
        private string _referenceBeanName;
        /// <summary>
        /// 属性引用的对象实例
        /// </summary>
        private object _referenceValue;

        public string PropertyName { get { return _propertyName; } internal set { _propertyName = value; } }
        public Type ReferenceClassType { get { return _referenceClassType; } internal set { _referenceClassType = value; } }
        public string ReferenceBeanName { get { return _referenceBeanName; } internal set { _referenceBeanName = value; } }
        public object ReferenceValue { get { return _referenceValue; } internal set { _referenceValue = value; } }

        /// <summary>
        /// 获取属性配置对应的属性标记实例
        /// </summary>
        public SymProperty SymProperty
        {
            get
            {
                Debugger.Assert(null != this.BeanObject && null != this.BeanObject.Symbol, "The bean object instance must be non-null.");

                return this.BeanObject.Symbol.GetPropertyByName(_propertyName);
            }
        }

        public BeanProperty(Bean beanObject) : base(beanObject)
        { }

        ~BeanProperty()
        { }

        public override bool Equals(object obj)
        {
            if (obj is BeanProperty) { return Equals((BeanProperty) obj); }

            return false;
        }

        public bool Equals(BeanProperty other)
        {
            return null != _propertyName && _propertyName == other._propertyName;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + _propertyName?.GetHashCode() ?? 0;
                hash = hash * 23 + _referenceBeanName?.GetHashCode() ?? 0;
                hash = hash * 23 + _referenceClassType?.GetHashCode() ?? 0;
                hash = hash * 23 + _referenceValue?.GetHashCode() ?? 0;
                return hash;
            }
        }
    }
}
