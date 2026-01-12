/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
/// Copyright (C) 2017 - 2020, Shanghai Tommon Network Technology Co., Ltd.
/// Copyright (C) 2020 - 2022, Guangzhou Xinyuan Technology Co., Ltd.
/// Copyright (C) 2022 - 2023, Shanghai Bilibili Technology Co., Ltd.
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

namespace NovaEngine.Module
{
    /// <summary>
    /// 线程管理器对象类，对协程及线程进行封装，对外提供统一的调用接口
    /// </summary>
    internal sealed partial class ThreadModule : ModuleObject
    {
        /// <summary>
        /// 线程模块事件类型
        /// </summary>
        public override sealed int EventType => (int) ModuleEventType.Thread;

        /// <summary>
        /// 管理器对象初始化接口函数
        /// </summary>
        protected override sealed void OnInitialize()
        {
        }

        /// <summary>
        /// 管理器对象清理接口函数
        /// </summary>
        protected override sealed void OnCleanup()
        {
        }

        /// <summary>
        /// 管理器对象初始启动接口
        /// </summary>
        protected override sealed void OnStartup()
        {
        }

        /// <summary>
        /// 管理器对象结束关闭接口
        /// </summary>
        protected override sealed void OnShutdown()
        {
        }

        /// <summary>
        /// 管理器对象垃圾回收调度接口
        /// </summary>
        protected override sealed void OnDump()
        {
        }

        /// <summary>
        /// 线程管理器内部事务更新接口
        /// </summary>
        protected override sealed void OnUpdate()
        {
        }

        /// <summary>
        /// 线程管理器内部后置更新接口
        /// </summary>
        protected override sealed void OnLateUpdate()
        {
        }

        /// <summary>
        /// 在当前上下文以协程方式运行目标回调句柄接口
        /// </summary>
        /// <param name="handler">目标回调句柄</param>
        public void Coroutine(ICoroutinable handler)
        {
            // TODO: Facade.Instance.DoWork(handler);
        }

        /// <summary>
        /// 在当前上下文以线程方式运行回调句柄接口
        /// </summary>
        /// <param name="handler">目标回调句柄</param>
        public void Run(IRunnable handler)
        {
            Utility.Thread.DoRun(handler);
        }

        /// <summary>
        /// 在当前上下文以线程方式运行回调函数接口
        /// </summary>
        /// <param name="action">行为函数</param>
        public void RunAsync(System.Action action)
        {
            Utility.Thread.RunAsync(action);
        }
    }
}
