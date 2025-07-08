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
    /// 框架模块管理器抽象类，用于定义一个在框架层进行调度的管理器对象类<br/>
    /// 该类归属于管理器接口，但默认实现了初始化及刷新回调，用于快速实现一个基础版的管理器对象
    /// </summary>
    public abstract class CFrameworkManager : IManager, IInitializable, IUpdatable
    {
        /// <summary>
        /// 获取管理器实例的优先级
        /// </summary>
        public virtual int Priority
        {
            get { return 0; }
        }

        /// <summary>
        /// 管理器对象的初始化回调函数
        /// </summary>
        public virtual void Initialize()
        {
        }

        /// <summary>
        /// 管理器对象的清理回调函数
        /// </summary>
        public virtual void Cleanup()
        {
        }

        /// <summary>
        ///管理器对象的刷新回调函数
        /// </summary>
        public virtual void Update()
        {
        }

        /// <summary>
        ///管理器对象的后置刷新回调函数
        /// </summary>
        public virtual void LateUpdate()
        {
        }
    }
}
