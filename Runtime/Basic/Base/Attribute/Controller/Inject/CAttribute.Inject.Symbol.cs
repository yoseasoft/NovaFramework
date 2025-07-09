/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyright (C) 2023 - 2024, Guangzhou Shiyue Network Technology Co., Ltd.
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
    /// 对象注入管理的配置数据相关的属性类型定义
    /// </summary>
    [SystemAttributeUsage(SystemAttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class OnBeanConfiguredAttribute : SystemAttribute
    {
        /// <summary>
        /// 对象实例的配置名称
        /// </summary>
        private readonly string _beanName;
        /// <summary>
        /// 对象实例的单例模式状态标识
        /// </summary>
        private readonly bool _singleton;

        /// <summary>
        /// 对象配置名称获取函数
        /// </summary>
        public string BeanName => _beanName;
        /// <summary>
        /// 对象单例模式状态标识获取函数
        /// </summary>
        public bool Singleton => _singleton;

        public OnBeanConfiguredAttribute(string beanName) : this(beanName, false)
        {
        }

        public OnBeanConfiguredAttribute(bool singleton) : this(null, singleton)
        {
        }

        public OnBeanConfiguredAttribute(string beanName, bool singleton) : base()
        {
            _beanName = beanName;
            _singleton = singleton;
        }
    }

    /// <summary>
    /// 对象注入管理的对象链接标记的属性类型定义
    /// </summary>
    [SystemAttributeUsage(SystemAttributeTargets.Field | SystemAttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class OnBeanAutowiredAttribute : SystemAttribute
    {
        /// <summary>
        /// 对象实例的引用类型
        /// </summary>
        private readonly SystemType _referenceType;
        /// <summary>
        /// 对象实例的引用名称
        /// </summary>
        private readonly string _referenceName;

        /// <summary>
        /// 对象引用类型获取函数
        /// </summary>
        public SystemType ReferenceType => _referenceType;
        /// <summary>
        /// 对象引用名称获取函数
        /// </summary>
        public string ReferenceName => _referenceName;

        public OnBeanAutowiredAttribute(SystemType referenceType) : this(referenceType, null)
        {
        }

        public OnBeanAutowiredAttribute(string referenceName) : this(null, referenceName)
        {
        }

        public OnBeanAutowiredAttribute(SystemType referenceType, string referenceName) : base()
        {
            _referenceType= referenceType;
            _referenceName = referenceName;
        }
    }

    /// <summary>
    /// 对象注入管理的对象链接标记的属性类型定义
    /// </summary>
    [SystemAttributeUsage(SystemAttributeTargets.Field | SystemAttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    public class OnBeanAutowiredOfTargetAttribute : OnBeanAutowiredAttribute
    {
        /// <summary>
        /// 对象实例的实体名称
        /// </summary>
        private readonly string _beanName;

        /// <summary>
        /// 对象实体名称获取函数
        /// </summary>
        public string BeanName => _beanName;

        public OnBeanAutowiredOfTargetAttribute(string beanName, SystemType referenceType) : this(beanName, referenceType, null)
        {
        }

        public OnBeanAutowiredOfTargetAttribute(string beanName, string referenceName) : this(beanName, null, referenceName)
        {
        }

        public OnBeanAutowiredOfTargetAttribute(string beanName, SystemType referenceType, string referenceName) : base(referenceType, referenceName)
        {
            _beanName = beanName;
        }
    }
}
