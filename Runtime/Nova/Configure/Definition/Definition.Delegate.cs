/// -------------------------------------------------------------------------------
/// NovaEngine Framework
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

namespace NovaEngine
{
    /// <summary>
    /// 基础常量数据定义类，将字符和字符串中的一些通用常量在此统一进行定义使用
    /// </summary>
    public static partial class Definition
    {
        /// <summary>
        /// 代理委托相关函数接口定义
        /// </summary>
        public static class Delegate
        {
            /// <summary>
            /// 空函数的接口句柄定义
            /// </summary>
            public delegate void EmptyFunctionHandler();

            /// <summary>
            /// 获取布尔值的空参函数的接口句柄定义
            /// </summary>
            /// <returns>返回布尔值</returns>
            public delegate bool EmptyFunctionHandlerForReturnBool();

            /// <summary>
            /// 获取对象值的空参函数的接口句柄定义
            /// </summary>
            /// <returns>返回对象值</returns>
            public delegate object EmptyFunctionHandlerForReturnObject();
        }
    }
}
