/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyright (C) 2024 - 2025, Hurley, Independent Studio.
/// Copyright (C) 2025, Hainan Yuanyou Information Technology Co., Ltd. Guangzhou Branch
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
    /// 编程接口分发类型注册函数的属性类型定义
    /// </summary>
    [SystemAttributeUsage(SystemAttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class OnApiDispatchCallAttribute : SystemAttribute
    {
        /// <summary>
        /// 编程接口通知的目标对象类型
        /// </summary>
        private readonly SystemType _classType;
        /// <summary>
        /// 编程接口通知的功能名称
        /// </summary>
        private readonly string _functionName;

        /// <summary>
        /// 目标对象类型获取函数
        /// </summary>
        public SystemType ClassType => _classType;
        /// <summary>
        /// 功能名称获取函数
        /// </summary>
        public string FunctionName => _functionName;

        public OnApiDispatchCallAttribute(string functionName) : this(null, functionName)
        { }

        public OnApiDispatchCallAttribute(SystemType classType, string functionName)
        {
            _classType = classType;
            _functionName = functionName;
        }
    }

    /// <summary>
    /// 编程接口功能绑定函数的属性类型定义
    /// </summary>
    [SystemAttributeUsage(SystemAttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class ApiFunctionBindingOfTargetAttribute : SystemAttribute
    {
        /// <summary>
        /// 编程接口通知的功能名称
        /// </summary>
        private readonly string _functionName;

        public string FunctionName => _functionName;

        public ApiFunctionBindingOfTargetAttribute(string functionName)
        {
            _functionName = functionName;
        }
    }
}
