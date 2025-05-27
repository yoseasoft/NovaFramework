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

using SystemAttribute = System.Attribute;
using SystemAttributeUsageAttribute = System.AttributeUsageAttribute;
using SystemAttributeTargets = System.AttributeTargets;

namespace GameEngine
{
    /// <summary>
    /// 角色实现类声明属性类型定义
    /// </summary>
    [SystemAttributeUsage(SystemAttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class DeclareActorClassAttribute : SystemAttribute
    {
        /// <summary>
        /// 角色名称
        /// </summary>
        private readonly string m_actorName;
        /// <summary>
        /// 角色功能类型标识
        /// </summary>
        private readonly int m_funcType;

        /// <summary>
        /// 角色名称获取函数
        /// </summary>
        public string ActorName => m_actorName;

        /// <summary>
        /// 角色功能类型获取函数
        /// </summary>
        public int FuncType => m_funcType;

        public DeclareActorClassAttribute(string actorName) : this(actorName, 0)
        {
        }

        public DeclareActorClassAttribute(int funcType) : this(string.Empty, funcType)
        {
        }

        public DeclareActorClassAttribute(string actorName, int funcType) : base()
        {
            m_actorName = actorName ?? string.Empty;
            m_funcType = funcType;
        }
    }
}
