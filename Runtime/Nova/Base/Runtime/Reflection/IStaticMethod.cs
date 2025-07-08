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

namespace NovaEngine
{
    /// <summary>
    /// 静态函数的抽象接口类，定义了函数的通用访问接口
    /// </summary>
    public interface IStaticMethod
    {
        /// <summary>
        /// 无参的函数访问调用接口
        /// </summary>
        void Invoke();

        /// <summary>
        /// 一个参数的函数访问调用接口
        /// </summary>
        /// <param name="arg1">参数1</param>
        void Invoke(object arg1);

        /// <summary>
        /// 两个参数的函数访问调用接口
        /// </summary>
        /// <param name="arg1">参数1</param>
        /// <param name="arg2">参数2</param>
        void Invoke(object arg1, object arg2);

        /// <summary>
        /// 三个参数的函数访问调用接口
        /// </summary>
        /// <param name="arg1">参数1</param>
        /// <param name="arg2">参数2</param>
        /// <param name="arg3">参数3</param>
        void Invoke(object arg1, object arg2, object arg3);
    }
}
