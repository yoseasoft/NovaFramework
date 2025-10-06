/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyright (C) 2023 - 2024, Guangzhou Shiyue Network Technology Co., Ltd.
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

using SystemType = System.Type;

namespace GameEngine
{
    /// <summary>
    /// 原型对象管理类，用于对场景上下文中的所有原型对象提供通用的访问操作接口
    /// </summary>
    internal sealed partial class BeanController : BaseController<BeanController>
    {
        /// <summary>
        /// 原型管理对象初始化通知接口函数
        /// </summary>
        protected override sealed void OnInitialize()
        {
        }

        /// <summary>
        /// 原型管理对象清理通知接口函数
        /// </summary>
        protected override sealed void OnCleanup()
        {
        }

        /// <summary>
        /// 原型管理对象刷新调度函数接口
        /// </summary>
        protected override sealed void OnUpdate()
        {
        }

        /// <summary>
        /// 原型管理对象后置刷新调度函数接口
        /// </summary>
        protected override sealed void OnLateUpdate()
        {
        }

        /// <summary>
        /// 原型管理对象重载调度函数接口
        /// </summary>
        protected override sealed void OnReload()
        {
        }

        /// <summary>
        /// 原型管理对象倾泻调度函数接口
        /// </summary>
        protected override void OnDump()
        {
        }
    }
}
