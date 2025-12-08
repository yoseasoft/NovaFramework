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

namespace NovaEngine.Module
{
    /// <summary>
    /// 场景对象事件参数基类定义
    /// </summary>
    internal sealed class SceneEventArgs : ModuleEventArgs
    {
        /// <summary>
        /// 事件参数类型标识
        /// </summary>
        public override sealed int ID => (int) ModuleObject.ModuleEventType.Scene;

        /// <summary>
        /// 场景协议类型
        /// </summary>
        private int _protocol = 0;

        /// <summary>
        /// 场景数据实体
        /// </summary>
        private object _data = null;

        public int Protocol
        {
            get { return _protocol; }
            set { _protocol = value; }
        }

        public object Data
        {
            get { return _data; }
            set { _data = value; }
        }

        /// <summary>
        /// 场景事件参数对象的新实例构建接口
        /// </summary>
        public SceneEventArgs()
        {
        }

        /// <summary>
        /// 场景参数对象初始化接口
        /// </summary>
        public override sealed void Initialize()
        {
            _data = null;
        }

        /// <summary>
        /// 场景参数对象清理接口
        /// </summary>
        public override sealed void Cleanup()
        {
            _data = null;
        }

        /// <summary>
        /// 场景参数克隆接口
        /// </summary>
        /// <param name="args">模块事件参数对象</param>
        public override sealed void CopyTo(ModuleEventArgs args)
        {
            throw new System.NotImplementedException();
        }
    }
}
