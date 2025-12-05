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
    /// 引擎框架模块中控台管理类
    internal static partial class ModuleController
    {
        /// <summary>
        /// 模块对象通用事件参数基类定义
        /// </summary>
        public sealed class CommonEventArgs : ModuleEventArgs
        {
            /// <summary>
            /// 模块通用事件处理类型编号
            /// </summary>
            private int _eventID;

            /// <summary>
            /// 模块通用事件处理类型标识
            /// </summary>
            private int _eventType;

            /// <summary>
            /// 模块通用事件参数对象的新实例构建接口
            /// </summary>
            public CommonEventArgs() : base()
            {
            }

            /// <summary>
            /// 获取通用事件参数类型编号
            /// </summary>
            public override int ID
            {
                get { return _eventID; }
            }

            public int EventID
            {
                get { return _eventID; }
                set { _eventID = value; }
            }

            /// <summary>
            /// 获取或设置通用事件处理类型
            /// </summary>
            public int EventType
            {
                get { return _eventType; }
                set { _eventType = value; }
            }

            /// <summary>
            /// 通用事件参数对象初始化接口
            /// </summary>
            public override void Initialize()
            {
                _eventID = 0;
                _eventType = 0;
            }

            /// <summary>
            /// 通用事件参数对象清理接口
            /// </summary>
            public override void Cleanup()
            {
                _eventID = 0;
                _eventType = 0;
            }

            /// <summary>
            /// 通用事件参数克隆接口
            /// </summary>
            /// <param name="args">事件参数实例</param>
            public override void CopyTo(ModuleEventArgs args)
            {
                Logger.Assert(args.GetType().IsAssignableFrom(typeof(CommonEventArgs)));

                CommonEventArgs e = (CommonEventArgs) args;
                e._eventID = _eventID;
                e._eventType = _eventType;
            }
        }
    }
}
