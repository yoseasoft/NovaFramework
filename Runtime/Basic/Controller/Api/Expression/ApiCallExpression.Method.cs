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

namespace GameEngine.Api.Expression
{
    /// <summary>
    /// 编程接口的回调函数对象类，用于提供对指定功能函数的调用
    /// </summary>
    internal sealed class ApiCallMethod : ApiCallExpression
    {
        /// <summary>
        /// 回调函数的功能名称
        /// </summary>
        public string FunctionName { get; set; }
        /// <summary>
        /// 回调函数的参数列表
        /// </summary>
        public object[] ParameterValues { get; set; }

        /// <summary>
        /// 对象功能执行函数
        /// </summary>
        /// <param name="obj">目标对象实例</param>
        public override void Invoke(IBean obj)
        {
            ApiController.Instance.CallFunction(obj, FunctionName, ParameterValues);
        }
    }
}
