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

namespace GameEngine.Loader.Configuring
{
    /// <summary>
    /// 通用Bean的字段配置类型的结构信息
    /// </summary>
    public class BeanFieldConfigureInfo : ICodeConfigureEffectivateVerification
    {
        /// <summary>
        /// 节点字段的完整名称
        /// </summary>
        private string _fieldName;
        /// <summary>
        /// 节点字段的引用名称
        /// </summary>
        private string _referenceName;
        /// <summary>
        /// 节点字段的引用类型
        /// </summary>
        private SystemType _referenceType;
        /// <summary>
        /// 节点字段的配置值
        /// </summary>
        private string _referenceValue;

        public string FieldName { get { return _fieldName; } internal set { _fieldName = value; } }
        public string ReferenceName { get { return _referenceName; } internal set { _referenceName = value; } }
        public SystemType ReferenceType { get { return _referenceType; } internal set { _referenceType = value; } }
        public string ReferenceValue { get { return _referenceValue; } internal set { _referenceValue = value; } }

        /// <summary>
        /// 该配置对象是否有效的检测接口函数
        /// </summary>
        /// <returns>若配置有效则返回true，否则返回false</returns>
        public bool IsEffectivationCodeConfigure()
        {
            int c = 0;

            // 字段的引用名称，引用类型或引用值，必须要配置一项

            if (null == _referenceName)
                ++c;

            if (null != _referenceType)
                ++c;

            if (null == _referenceValue)
                ++c;

            if (c <= 0 || c > 1)
                return false;

            return true;
        }

        public override string ToString()
        {
            SystemStringBuilder sb = new SystemStringBuilder();
            sb.Append("{ ");
            sb.AppendFormat("FieldName = {0}, ", _fieldName);
            sb.AppendFormat("ReferenceName = {0}, ", _referenceName);
            sb.AppendFormat("ReferenceType = {0}, ", NovaEngine.Utility.Text.ToString(_referenceType));
            sb.AppendFormat("ReferenceValue = {0}, ", _referenceValue);
            sb.Append("}");
            return sb.ToString();
        }
    }
}
