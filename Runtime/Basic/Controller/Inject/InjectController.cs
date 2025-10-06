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
    /// 反射注入接口的控制器类，对整个程序所有反射注入函数进行统一的整合和管理
    /// </summary>
    internal sealed partial class InjectController : BaseController<InjectController>
    {
        /// <summary>
        /// 注入控制器对象初始化通知接口函数
        /// </summary>
        protected override sealed void OnInitialize()
        {
            // 注入服务的初始化
            InjectBeanService.InitAllServiceProcessingCallbacks();
        }

        /// <summary>
        /// 注入控制器对象清理通知接口函数
        /// </summary>
        protected override sealed void OnCleanup()
        {
            // 注入服务的清理
            InjectBeanService.CleanupAllServiceProcessingCallbacks();
        }

        /// <summary>
        /// 注入控制器对象刷新调度函数接口
        /// </summary>
        protected override sealed void OnUpdate()
        {
        }

        /// <summary>
        /// 注入控制器对象后置刷新调度函数接口
        /// </summary>
        protected override sealed void OnLateUpdate()
        {
        }

        /// <summary>
        /// 注入控制器对象重载调度函数接口
        /// </summary>
        protected override sealed void OnReload()
        {
        }

        /// <summary>
        /// 注入控制器对象倾泻调度函数接口
        /// </summary>
        protected override void OnDump()
        {
        }

        #region 反射注入函数调用接口

        #endregion
    }
}
