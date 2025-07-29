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
    /// 定时对象事件参数基类定义
    /// </summary>
    public sealed class TimerEventArgs : ModuleEventArgs
    {
        /// <summary>
        /// 事件参数类型标识
        /// </summary>
        public override int ID => (int) ModuleObject.ModuleEventType.Timer;

        private int _type;

        private int _session;

        /// <summary>
        /// 定时事件参数对象的新实例构建接口
        /// </summary>
        public TimerEventArgs()
        {
        }

        public int Type
        {
            get { return _type; }
            set { _type = value; }
        }

        public int Session
        {
            get { return _session; }
            set { _session = value; }
        }

        /// <summary>
        /// 定时参数对象初始化接口
        /// </summary>
        public override void Initialize()
        {
            _type = 0;
            _session = 0;
        }

        /// <summary>
        /// 定时参数对象清理接口
        /// </summary>
        public override void Cleanup()
        {
            _type = 0;
            _session = 0;
        }

        /// <summary>
        /// 定时参数克隆接口
        /// </summary>
        /// <param name="args">模块事件参数对象</param>
        public override void CopyTo(ModuleEventArgs args)
        {
            throw new System.NotImplementedException();
        }
    }
}