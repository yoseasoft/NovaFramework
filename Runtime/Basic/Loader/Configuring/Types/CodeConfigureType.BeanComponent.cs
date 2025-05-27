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

using SystemType = System.Type;
using SystemStringBuilder = System.Text.StringBuilder;

namespace GameEngine.Loader.Configuring
{
    /// <summary>
    /// 通用Bean的字段配置类型的结构信息
    /// </summary>
    public class BeanFieldConfigureInfo
    {
        /// <summary>
        /// 节点字段的完整名称
        /// </summary>
        private string m_fieldName;
        /// <summary>
        /// 节点字段的引用名称
        /// </summary>
        private string m_referenceName;
        /// <summary>
        /// 节点字段的引用类型
        /// </summary>
        private SystemType m_referenceType;
        /// <summary>
        /// 节点字段的引用值
        /// </summary>
        private object m_referenceValue;

        public string FieldName { get { return m_fieldName; } internal set { m_fieldName = value; } }
        public string ReferenceName { get { return m_referenceName; } internal set { m_referenceName = value; } }
        public SystemType ReferenceType { get { return m_referenceType; } internal set { m_referenceType = value; } }
        public object ReferenceValue { get { return m_referenceValue; } internal set { m_referenceValue = value; } }

        public override string ToString()
        {
            SystemStringBuilder sb = new SystemStringBuilder();
            sb.Append("{ ");
            sb.AppendFormat("FieldName = {0}, ", m_fieldName);
            sb.AppendFormat("ReferenceName = {0}, ", m_referenceName);
            sb.AppendFormat("ReferenceType = {0}, ", NovaEngine.Utility.Text.ToString(m_referenceType));
            sb.AppendFormat("ReferenceValue = {0}, ", m_referenceValue?.ToString());
            sb.Append("}");
            return sb.ToString();
        }
    }
}
