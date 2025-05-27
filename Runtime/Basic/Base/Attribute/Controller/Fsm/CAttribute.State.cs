/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyring (C) 2025, Hurley, Independent Studio.
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
using SystemAttribute = System.Attribute;
using SystemAttributeUsageAttribute = System.AttributeUsageAttribute;
using SystemAttributeTargets = System.AttributeTargets;

namespace GameEngine
{
    /// <summary>
    /// 状态声明属性类型定义
    /// </summary>
    [SystemAttributeUsage(SystemAttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class DeclareStateClassAttribute : SystemAttribute
    {
        /// <summary>
        /// 状态名称
        /// </summary>
        private readonly string m_stateName;

        /// <summary>
        /// 状态优先级
        /// </summary>
        private readonly int m_priority;

        /// <summary>
        /// 状态名称获取函数
        /// </summary>
        public string StateName => m_stateName;
        /// <summary>
        /// 状态优先级获取函数
        /// </summary>
        public int Priority => m_priority;

        public DeclareStateClassAttribute(string stateName) : this(stateName, 0)
        { }

        public DeclareStateClassAttribute(string stateName, int priority) : base()
        {
            m_stateName = stateName;
            m_priority = priority;
        }
    }
}
