/// -------------------------------------------------------------------------------
/// NovaEngine Framework
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

using SystemAttribute = System.Attribute;
using SystemAttributeUsageAttribute = System.AttributeUsageAttribute;
using SystemAttributeTargets = System.AttributeTargets;

namespace NovaEngine
{
    /// <summary>
    /// 测试用例函数属性类型定义
    /// </summary>
    [SystemAttributeUsage(SystemAttributeTargets.Method)]
    public class TestingCaseAttribute : SystemAttribute
    {
        public TestingCaseAttribute() : base()
        {
        }
    }

    /// <summary>
    /// 测试用例静态函数属性类型定义
    /// </summary>
    [SystemAttributeUsage(SystemAttributeTargets.Method)]
    public class StaticTestingCaseAttribute : SystemAttribute
    {
        public StaticTestingCaseAttribute() : base()
        {
        }
    }

    /// <summary>
    /// 测试用例类属性类型定义
    /// </summary>
    [SystemAttributeUsage(SystemAttributeTargets.Class)]
    public class TestingClassAttribute : SystemAttribute
    {
        /// <summary>
        /// 测试用例类级别标识
        /// </summary>
        private readonly int _level;

        /// <summary>
        /// 测试用例类级别的获取函数
        /// </summary>
        public int Level => _level;

        public TestingClassAttribute(int level) : base()
        {
            _level = level;
        }
    }
}
