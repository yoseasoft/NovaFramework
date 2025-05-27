/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
/// Copyring (C) 2020 - 2022, Guangzhou Xinyuan Technology Co., Ltd.
/// Copyring (C) 2022 - 2023, Shanghai Bilibili Technology Co., Ltd.
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

namespace NovaEngine
{
    /// <summary>
    /// 模块类指令参数基类定义
    /// </summary>
    public class ModuleCommandArgs : CommandArgs
    {
        /// <summary>
        /// 当前指令参数的类型
        /// </summary>
        private int m_type;

        /// <summary>
        /// 当前指令参数的数据实例
        /// </summary>
        private ModuleEventArgs m_data;

        /// <summary>
        /// 模块类指令参数对象的新实例构建接口
        /// </summary>
        public ModuleCommandArgs()
        {
        }

        /// <summary>
        /// 获取模块指令参数类型编号
        /// </summary>
        public int Type
        {
            get { return m_type; }
        }

        /// <summary>
        /// 获取模块指令参数数据实例
        /// </summary>
        public ModuleEventArgs Data
        {
            get { return m_data; }
        }

        /// <summary>
        /// 模块类指令参数对象初始化接口
        /// </summary>
        public override void Initialize()
        {
        }

        /// <summary>
        /// 模块类指令参数对象清理接口
        /// </summary>
        public override void Cleanup()
        {
            m_type = 0;

            m_data = null;
        }

        /// <summary>
        /// 设置模块指令参数的类型
        /// </summary>
        /// <param name="type">模块参数类型</param>
        public void SetType(int type)
        {
            m_type = type;
        }

        /// <summary>
        /// 设置模块指令参数的数据实例
        /// </summary>
        /// <param name="e">事件参数对象实例</param>
        public void SetData(ModuleEventArgs e)
        {
            // m_data = ReferencePool.Acquire<T>();
            // e.CopyTo(m_data);

            m_data = e;
        }
    }
}
