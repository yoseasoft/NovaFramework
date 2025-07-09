/// -------------------------------------------------------------------------------
/// GameEngine Framework
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

namespace GameEngine.Debug
{
    /// <summary>
    /// 调试器窗口组的抽象接口类，对调试器窗口进行编组及批量调度操作
    /// </summary>
    public interface IDebuggerWindowGroup : IDebuggerWindow
    {
        /// <summary>
        /// 获取调试器窗口的数量
        /// </summary>
        int DebuggerWindowCount { get; }

        /// <summary>
        /// 获取或设置当前选中的调试器窗口的索引
        /// </summary>
        int SelectedIndex { get; set; }

        /// <summary>
        /// 获取当前选中的调试器窗口
        /// </summary>
        IDebuggerWindow SelectedWindow { get; }

        /// <summary>
        /// 获取该调试组的所有调试器窗口的名称集合
        /// </summary>
        /// <returns>返回调试器窗口名称的集合，若没有则返回空</returns>
        string[] GetAllDebuggerWindowNames();

        /// <summary>
        /// 获取指定路径对应的调试器窗口实例
        /// </summary>
        /// <param name="path">窗口路径</param>
        /// <returns>返回路径对应的窗口实例，若不存在则返回null</returns>
        IDebuggerWindow GetDebuggerWindow(string path);

        /// <summary>
        /// 注册指定的路径和窗口对象实例到当前的调试组中
        /// </summary>
        /// <param name="path">窗口路径</param>
        /// <param name="debuggerWindow">调试窗口实例</param>
        void RegisterDebuggerWindow(string path, IDebuggerWindow debuggerWindow);
    }
}
