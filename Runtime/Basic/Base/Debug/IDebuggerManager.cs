/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyring (C) 2023, Guangzhou Shiyue Network Technology Co., Ltd.
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
    /// 调试管理器的辅助接口类，对外封装对调试器窗口的访问操作相关函数
    /// </summary>
    public interface IDebuggerManager
    {
        /// <summary>
        /// 获取或设置调试器窗口是否激活的状态标识
        /// </summary>
        bool ActiveWindow { get; set; }

        /// <summary>
        /// 获取调试器的设置实例
        /// </summary>
        IDebuggerSetting DebuggerSetting { get; }

        /// <summary>
        /// 获取调试器窗口的根节点
        /// </summary>
        IDebuggerWindowGroup DebuggerWindowRoot { get; }

        /// <summary>
        /// 注册指定的路径和窗口对象实例到当前的调试管理器中
        /// </summary>
        /// <param name="path">窗口路径</param>
        /// <param name="debuggerWindow">调试窗口实例</param>
        /// <param name="args">窗口初始化参数</param>
        void RegisterDebuggerWindow(string path, IDebuggerWindow debuggerWindow, params object[] args);

        /// <summary>
        /// 从当前调试管理器中注销指定路径对应的窗口对象实例
        /// </summary>
        /// <param name="path">窗口路径</param>
        /// <returns>若窗口注销成功返回true，否则返回false</returns>
        bool UnregisterDebuggerWindow(string path);

        /// <summary>
        /// 获取指定路径对应的调试窗口对象实例
        /// </summary>
        /// <param name="path">窗口路径</param>
        /// <returns>若存在路径对应的窗口实例则返回其引用，否则返回null</returns>
        IDebuggerWindow GetDebuggerWindow(string path);

        /// <summary>
        /// 选中当前调试管理器中指定路径对应的调试窗口实例
        /// </summary>
        /// <param name="path">窗口路径</param>
        /// <returns>若选中窗口实例成功返回true，否则返回false</returns>
        bool SelectedDebuggerWindow(string path);
    }
}
