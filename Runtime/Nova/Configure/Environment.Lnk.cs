/// -------------------------------------------------------------------------------
/// NovaEngine Framework
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

namespace NovaEngine
{
    /// <summary>
    /// 基础环境属性定义类，对当前引擎运行所需的环境成员属性进行设置及管理
    /// </summary>
    public static partial class Environment
    {
        /// <summary>
        /// 检测当前程序上下文运行环境是否处于编辑模式<br/>
        /// 目前设置为只要处于编辑器状态下，就必定处于编辑模式
        /// </summary>
        /// <returns>若当前程序处于编辑模式则返回true，否则返回false</returns>
        public static bool IsEditorMode()
        {
            // 理论上当editorMode为true时，IsEditor()函数的返回值也必定为true
            if (Application.IsEditor() || editorMode)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 检测当前程序上下文运行环境是否为开发模式<br/>
        /// 当编辑模式被打开，或处于调试模式时，默认此状态为开发模式
        /// </summary>
        /// <returns>若当前程序处于开发模式则返回true，否则返回false</returns>
        public static bool IsDevelopmentState()
        {
            // 理论上当editorMode为true时，IsEditor()函数的返回值也必定为true
            if (Application.IsEditor() || editorMode || debugMode)
            {
                return true;
            }

            return false;
        }
    }
}
