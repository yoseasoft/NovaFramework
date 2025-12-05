/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
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

using System;

namespace GameEngine
{
    /// <summary>
    /// 针对GUI的窗口对象管理所定义的接口类，通过继承该接口类，实现自定义的窗口对象管理逻辑
    /// </summary>
    public interface IFormManager
    {
        /// <summary>
        /// 管理器启动回调函数
        /// </summary>
        void Startup();

        /// <summary>
        /// 管理器关闭回调函数
        /// </summary>
        void Shutdown();

        /// <summary>
        /// 管理器更新回调函数
        /// </summary>
        void Update();

        /// <summary>
        /// 通过指定的视图类型，创建对应的窗口对象实例
        /// 若视图类型未正确注册，则创建窗口对象实例失败
        /// </summary>
        /// <param name="viewType">视图类型</param>
        /// <returns>返回创建的窗口对象实例</returns>
        Form CreateForm(Type viewType);
    }
}
