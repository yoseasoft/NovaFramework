/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
/// Copyright (C) 2023, Guangzhou Shiyue Network Technology Co., Ltd.
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

using System;

namespace NovaEngine
{
    /// <summary>
    /// 错误码描述属性类型定义
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class ErrorCodeDescriptionAttribute : Attribute
    {
        /// <summary>
        /// 错误码描述内容
        /// </summary>
        private readonly string _description;

        public ErrorCodeDescriptionAttribute(string description) : base()
        {
            _description = description;
        }

        /// <summary>
        /// 描述属性值的访问操作接口
        /// </summary>
        public string Description
        {
            get { return _description; }
        }
    }
}
