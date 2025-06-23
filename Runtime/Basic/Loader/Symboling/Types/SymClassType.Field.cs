/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyright (C) 2023 - 2024, Guangzhou Shiyue Network Technology Co., Ltd.
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

using System.Collections.Generic;
using System.Reflection;

using SystemType = System.Type;
using SystemStringBuilder = System.Text.StringBuilder;

namespace GameEngine.Loader.Symboling
{
    /// <summary>
    /// 通用对象字段的标记数据的结构信息
    /// </summary>
    public class SymField : SymBase
    {
        /// <summary>
        /// 字段的名称
        /// </summary>
        private string m_fieldName;
        /// <summary>
        /// 字段的类型
        /// </summary>
        private SystemType m_fieldType;
        /// <summary>
        /// 字段对象实例
        /// </summary>
        private FieldInfo m_fieldInfo;

        public FieldInfo FieldInfo
        {
            get { return m_fieldInfo; }
            internal set
            {
                m_fieldInfo = value;

                m_fieldName = m_fieldInfo.Name;
                m_fieldType = m_fieldInfo.FieldType;
            }
        }

        public string FieldName => m_fieldName;
        public SystemType FieldType => m_fieldType;

        public SymField() { }

        ~SymField()
        {
            m_fieldInfo = null;
        }

        public override string ToString()
        {
            SystemStringBuilder sb = new SystemStringBuilder();
            sb.AppendFormat("{0}, ", base.ToString());
            sb.AppendFormat("FieldName = {0}, ", m_fieldName);
            sb.AppendFormat("FieldType = {0}, ", NovaEngine.Utility.Text.ToString(m_fieldType));
            sb.AppendFormat("FieldInfo = {0}, ", NovaEngine.Utility.Text.ToString(m_fieldInfo));
            return sb.ToString();
        }
    }
}
