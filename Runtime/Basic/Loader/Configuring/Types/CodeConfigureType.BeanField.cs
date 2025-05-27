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
    /// 通用Bean的组件配置类型的结构信息
    /// </summary>
    public class BeanComponentConfigureInfo
    {
        /// <summary>
        /// 节点组件的引用名称
        /// </summary>
        private string m_referenceName;
        /// <summary>
        /// 节点组件的引用类型
        /// </summary>
        private SystemType m_referenceType;
        /// <summary>
        /// 节点组件的优先级
        /// </summary>
        private int m_priority;
        /// <summary>
        /// 节点组件的激活阶段类型标识
        /// </summary>
        private AspectBehaviourType m_activationBehaviourType;

        public string ReferenceName { get { return m_referenceName; } internal set { m_referenceName = value; } }
        public SystemType ReferenceType { get { return m_referenceType; } internal set { m_referenceType = value; } }
        public int Priority { get { return m_priority; } internal set { m_priority = value; } }
        public AspectBehaviourType ActivationBehaviourType { get { return m_activationBehaviourType; } internal set { m_activationBehaviourType = value; } }

        public override string ToString()
        {
            SystemStringBuilder sb = new SystemStringBuilder();
            sb.Append("{ ");
            sb.AppendFormat("ReferenceName = {0}, ", m_referenceName);
            sb.AppendFormat("ReferenceType = {0}, ", NovaEngine.Utility.Text.ToString(m_referenceType));
            sb.AppendFormat("Priority = {0}, ", m_priority);
            sb.AppendFormat("ActivationBehaviourType = {0}, ", m_activationBehaviourType);
            sb.Append("}");
            return sb.ToString();
        }
    }
}
