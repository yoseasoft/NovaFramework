/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
/// Copyring (C) 2023, Guangzhou Shiyue Network Technology Co., Ltd.
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
    /// 单例类对象的抽象接口，声明一个简易的单例对象类<br/>
    /// 当您需要创建一个单例对象类时，需要继承自该接口，否则在单例对象类型检测时将发生异常<br/>
    /// 这里建议您直接选择继承<see cref="NovaEngine.Singleton{T}"/>抽象类<br/>
    /// 它也继承自该接口，同时提供了单例的创建及访问的函数接口
    /// </summary>
    public interface ISingleton
    {
        /// <summary>
        /// 单例类的创建函数句柄
        /// </summary>
        public delegate object SingletonCreateHandler();

        /// <summary>
        /// 单例类的销毁函数句柄
        /// </summary>
        public delegate void SingletonDestroyHandler();
    }
}
