/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyring (C) 2025, Hurley, Independent Studio.
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

namespace GameEngine
{
    /// <summary>
    /// 状态对象接口类，内部定义了一个标准状态对象所需的接口函数<br/>
    /// 您可以通过继承该接口类，来实现一个状态对象实例，并通过状态管理器进行调度控制<br/>
    /// </summary>
    public interface IState
    {
        /// <summary>
        /// 状态对象进入操作调度函数
        /// </summary>
        /// <param name="target">目标引用对象实例</param>
        /// <returns>若状态进入成功则返回true，否则返回false</returns>
        bool Enter(CRef target);

        /// <summary>
        /// 状态对象执行操作调度函数
        /// </summary>
        /// <param name="target">目标引用对象实例</param>
        void Execute(CRef target);

        /// <summary>
        /// 状态对象退出操作调度函数
        /// </summary>
        /// <param name="target">目标引用对象实例</param>
        void Exit(CRef target);
    }
}
