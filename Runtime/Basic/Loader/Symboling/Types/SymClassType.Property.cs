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
using System.Reflection;

using SystemType = System.Type;
using SystemStringBuilder = System.Text.StringBuilder;

namespace GameEngine.Loader.Symboling
{
    /// <summary>
    /// 通用对象属性的标记数据的结构信息
    /// </summary>
    public class SymProperty : SymBase
    {
        /// <summary>
        /// 属性的名称
        /// </summary>
        private string m_propertyName;
        /// <summary>
        /// 属性的类型
        /// </summary>
        private SystemType m_propertyType;
        /// <summary>
        /// 属性对象实例
        /// </summary>
        private PropertyInfo m_propertyInfo;

        public PropertyInfo PropertyInfo
        {
            get { return m_propertyInfo; }
            internal set
            {
                m_propertyInfo = value;

                m_propertyName = m_propertyInfo.Name;
                m_propertyType = m_propertyInfo.PropertyType;
            }
        }

        public string PropertyName => m_propertyName;
        public SystemType PropertyType => m_propertyType;

        public SymProperty() { }

        ~SymProperty()
        {
            m_propertyInfo = null;
        }

        public override string ToString()
        {
            SystemStringBuilder sb = new SystemStringBuilder();
            sb.AppendFormat("Base = {0}, ", base.ToString());
            sb.AppendFormat("PropertyName = {0}, ", m_propertyName);
            sb.AppendFormat("PropertyType = {0}, ", NovaEngine.Utility.Text.ToString(m_propertyType));
            sb.AppendFormat("PropertyInfo = {0}, ", NovaEngine.Utility.Text.ToString(m_propertyInfo));
            return sb.ToString();
        }
    }
}
