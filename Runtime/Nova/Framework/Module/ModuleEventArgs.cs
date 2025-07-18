/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
/// Copyright (C) 2020 - 2022, Guangzhou Xinyuan Technology Co., Ltd.
/// Copyright (C) 2022 - 2023, Shanghai Bilibili Technology Co., Ltd.
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

namespace NovaEngine
{
    /// <summary>
    /// 模块对象事件参数基类定义
    /// </summary>
    public abstract class ModuleEventArgs : EventArgs
    {
        /// <summary>
        /// 模块事件参数对象的新实例构建接口
        /// </summary>
        public ModuleEventArgs() : base()
        {
        }

        /*
        /// <summary>
        /// 获取事件参数类型编号
        /// </summary>
        public abstract int ID
        {
            get;
        }
        */

        /// <summary>
        /// 事件参数对象初始化接口
        /// </summary>
        // public abstract void Initialize();

        /// <summary>
        /// 事件参数对象清理接口
        /// </summary>
        // public abstract void Cleanup();

        public abstract void CopyTo(ModuleEventArgs args);
    }
}
