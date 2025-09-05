/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
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

namespace GameEngine.Api.Expression
{
    /// <summary>
    /// 编程接口的表达式组对象类，用于提供对表达式对象进行分组管理
    /// </summary>
    internal sealed class ApiCallGroup : ApiCallExpression
    {
        /// <summary>
        /// 分组类的表达式对象管理容器
        /// </summary>
        private IList<ApiCallExpression> _expressions = null;

        #region 表达式对象实例的元素管理接口函数

        /// <summary>
        /// 新增指定的表达式对象实例到当前分组中
        /// </summary>
        /// <param name="expression">表达式对象实例</param>
        public void AddExpression(ApiCallExpression expression)
        {
            if (null == _expressions)
            {
                _expressions = new List<ApiCallExpression>();
            }

            if (_expressions.Contains(expression))
            {
                Debugger.Warn(LogGroupTag.Controller, "新增“ApiCallExpression”异常：当前编程接口管理器的分组容器中已存在相同的表达式对象实例，重复添加相同实例的操作无效。");
                return;
            }

            _expressions.Add(expression);
        }

        /// <summary>
        /// 从当前分组中移除全部表达式对象实例
        /// </summary>
        public void RemoveAllExpressions()
        {
            _expressions?.Clear();
            _expressions = null;
        }

        #endregion

        /// <summary>
        /// 分组对象执行函数
        /// </summary>
        /// <param name="obj">目标对象实例</param>
        public override void Invoke(IBean obj)
        {
            if (null != _expressions)
            {
                int c = _expressions.Count;
                for (int n = 0; n < c; ++n)
                {
                    _expressions[n].Invoke(obj);
                }
            }
        }
    }
}
