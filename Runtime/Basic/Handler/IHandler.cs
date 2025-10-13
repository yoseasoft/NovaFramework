/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyright (C) 2023 - 2024, Guangzhou Shiyue Network Technology Co., Ltd.
/// Copyright (C) 2025, Hainan Yuanyou Information Technology Co., Ltd. Guangzhou Branch
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
    /// 针对<see cref="NovaEngine.ModuleObject"/>进行上层管理接口封装的句柄对象类的预定义接口<br/>
    /// 所有实现了该接口的子类，均由<see cref="GameEngine.HandlerManagement"/>实例化，并进行统一调度管理<br/>
    /// 子类必需包含的接口包括：<br/>
    ///   - Initialize: 初始化接口<br/>
    ///   - Cleanup:    清理接口<br/>
    ///   - Update:     刷新接口<br/>
    ///   - LateUpdate: 后置刷新接口<br/>
    /// </summary>
    public interface IHandler
    {
        /// <summary>
        /// 设置句柄的类型标识
        /// </summary>
        int HandlerType { get; }

        /// <summary>
        /// 句柄对象初始化接口函数
        /// </summary>
        /// <returns>若句柄对象初始化成功则返回true，否则返回false</returns>
        bool Initialize();

        /// <summary>
        /// 句柄对象清理接口函数
        /// </summary>
        void Cleanup();

        /// <summary>
        /// 句柄对象重载接口函数
        /// </summary>
        void Reload();

        /// <summary>
        /// 句柄对象执行接口
        /// </summary>
        void Execute();

        /// <summary>
        /// 句柄对象后置执行接口
        /// </summary>
        void LateExecute();

        /// <summary>
        /// 句柄对象刷新接口
        /// </summary>
        void Update();

        /// <summary>
        /// 句柄对象后置刷新接口
        /// </summary>
        void LateUpdate();

        /// <summary>
        /// 句柄对象的模块事件转发回调接口
        /// </summary>
        /// <param name="e">模块事件参数</param>
        void OnEventDispatch(NovaEngine.ModuleEventArgs e);
    }
}
