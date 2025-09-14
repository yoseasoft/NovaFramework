/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyright (C) 2024 - 2025, Hurley, Independent Studio.
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
    /// 状态管理对象类，用于对场景上下文中的所有引用对象的状态进行集中管理及分发
    /// </summary>
    internal sealed partial class HfsmController : BaseController<HfsmController>
    {
        /// <summary>
        /// 状态管理初始化通知接口函数
        /// </summary>
        protected override void OnInitialize()
        {
            // 初始化监听列表
        }

        /// <summary>
        /// 状态管理清理通知接口函数
        /// </summary>
        protected override void OnCleanup()
        {
            // 清理监听列表
        }

        /// <summary>
        /// 状态管理刷新通知接口函数
        /// </summary>
        protected override sealed void OnUpdate()
        {
        }

        /// <summary>
        /// 状态管理后置刷新通知接口函数
        /// </summary>
        protected override sealed void OnLateUpdate()
        {
        }

        /// <summary>
        /// 状态管理倾泻调度函数接口
        /// </summary>
        protected override void OnDump()
        {
        }
    }
}
