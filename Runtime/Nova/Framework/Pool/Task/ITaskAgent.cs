/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
/// Copyring (C) 2020 - 2022, Guangzhou Xinyuan Technology Co., Ltd.
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
    /// 任务对象代理接口
    /// </summary>
    /// <typeparam name="T">任务类型</typeparam>
    internal interface ITaskAgent<T> where T : TaskArgs
    {
        /// <summary>
        /// 获取任务对象
        /// </summary>
        T Task
        {
            get;
        }

        /// <summary>
        /// 任务代理初始化接口
        /// </summary>
        void Initialize();

        /// <summary>
        /// 任务代理清理接口
        /// </summary>
        void Cleanup();

        /// <summary>
        /// 任务启动操作处理接口
        /// </summary>
        /// <param name="task">待处理任务对象</param>
        /// <returns>返回开始处理任务的状态类型</returns>
        TaskStartupResultType Startup(T task);

        /// <summary>
        /// 任务关闭操作处理接口
        /// </summary>
        void Shutdown();

        /// <summary>
        /// 任务代理轮询操作调度接口
        /// </summary>
        void Update();

        /// <summary>
        /// 停止当前处理的任务并重置任务代理
        /// </summary>
        void Reset();
    }
}
