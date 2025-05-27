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
using SystemAttribute = System.Attribute;
using SystemAttributeUsageAttribute = System.AttributeUsageAttribute;
using SystemAttributeTargets = System.AttributeTargets;

namespace GameEngine
{
    /// <summary>
    /// 代码加载类的加载函数的属性定义
    /// </summary>
    [SystemAttributeUsage(SystemAttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class OnCodeLoaderClassLoadOfTargetAttribute : SystemAttribute
    {
        private readonly SystemType m_classType;

        public SystemType ClassType => m_classType;

        public OnCodeLoaderClassLoadOfTargetAttribute(SystemType classType)
        {
            m_classType = classType;
        }
    }

    /// <summary>
    /// 代码加载类的清理函数的属性定义
    /// </summary>
    [SystemAttributeUsage(SystemAttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class OnCodeLoaderClassCleanupOfTargetAttribute : SystemAttribute
    {
        private readonly SystemType m_classType;

        public SystemType ClassType => m_classType;

        public OnCodeLoaderClassCleanupOfTargetAttribute(SystemType classType)
        {
            m_classType = classType;
        }
    }

    /// <summary>
    /// 代码加载类的结构信息查找函数的属性定义
    /// </summary>
    [SystemAttributeUsage(SystemAttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class OnCodeLoaderClassLookupOfTargetAttribute : SystemAttribute
    {
        private readonly SystemType m_classType;

        public SystemType ClassType => m_classType;

        public OnCodeLoaderClassLookupOfTargetAttribute(SystemType classType)
        {
            m_classType = classType;
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// 绑定处理接口注册函数的属性类型定义
    /// </summary>
    [SystemAttributeUsage(SystemAttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class OnProcessRegisterClassOfTargetAttribute : SystemAttribute
    {
        private readonly SystemType m_classType;

        public SystemType ClassType => m_classType;

        public OnProcessRegisterClassOfTargetAttribute(SystemType classType) { m_classType = classType; }
    }

    /// <summary>
    /// 绑定处理接口注销函数的属性类型定义
    /// </summary>
    [SystemAttributeUsage(SystemAttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class OnProcessUnregisterClassOfTargetAttribute : SystemAttribute
    {
        private readonly SystemType m_classType;

        public SystemType ClassType => m_classType;

        public OnProcessUnregisterClassOfTargetAttribute(SystemType classType) { m_classType = classType; }
    }
}
