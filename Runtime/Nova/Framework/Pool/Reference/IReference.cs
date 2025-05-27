/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
/// Copyring (C) 2022 - 2023, Shanghai Bilibili Technology Co., Ltd.
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
    /// 引用类对象的抽象父类，声明一个简易的引用对象类<br/>
    /// 该类可以认为是所有动态创建的引用对象的通用父类，它同时提供了一个引用缓存池作为所有引用对象的缓存管理<br/>
    /// 当您需要创建一个对象类时，若此类不是一个静态类，且可能存在多次创建的情况时，建议您以该类的子类的方式去实现它
    /// </summary>
    public interface IReference : IInitializable
    {
        /// <summary>
        /// 引用对象的默认初始化回调函数<br/>
        /// 您可以在这里对引用对象的属性及上下文环境进行初始化<br/>
        /// 因为引用对象是可以被缓存管理的，因此该函数可能存在多次被调用的情况<br/>
        /// 您在使用时需要注意，针对某些只能初始化一次的数据（例如全局静态属性，或仅构造时赋值一次的成员等），不能在此处进行处理
        /// </summary>
        // void Initialize();

        /// <summary>
        /// 引用对象的默认清理回调函数<br/>
        /// 若您在引用对象的初始化函数中加载了某些实例，请记得在这里移除它们，避免内存泄漏或循环引用等其它问题<br/>
        /// 因为引用对象是可以被缓存管理的，因此该函数可能存在多次被调用的情况<br/>
        /// 您在使用时需要注意，针对某些伴随对象整个生命周期的数据（例如全局静态属性，或仅销毁时清理的成员等），不能在此处进行处理
        /// </summary>
        // void Cleanup();
    }
}
