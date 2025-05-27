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
    /// 引擎框架模块中控台管理类
    /// </summary>
    public static partial class ModuleController
    {
        /// <summary>
        /// 模块对象通用事件参数基类定义
        /// </summary>
        public sealed class CommonEventArgs : ModuleEventArgs
        {
            /// <summary>
            /// 模块通用事件处理类型编号
            /// </summary>
            private int m_eventID;

            /// <summary>
            /// 模块通用事件处理类型标识
            /// </summary>
            private int m_eventType;

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
                get { return m_eventID; }
            }

            public int EventID
            {
                get { return m_eventID; }
                set { m_eventID = value; }
            }

            /// <summary>
            /// 获取或设置通用事件处理类型
            /// </summary>
            public int EventType
            {
                get { return m_eventType; }
                set { m_eventType = value; }
            }

            /// <summary>
            /// 通用事件参数对象初始化接口
            /// </summary>
            public override void Initialize()
            {
                m_eventID = 0;
                m_eventType = 0;
            }

            /// <summary>
            /// 通用事件参数对象清理接口
            /// </summary>
            public override void Cleanup()
            {
                m_eventID = 0;
                m_eventType = 0;
            }

            /// <summary>
            /// 通用事件参数克隆接口
            /// </summary>
            /// <param name="args">事件参数实例</param>
            public override void CopyTo(ModuleEventArgs args)
            {
                Logger.Assert(args.GetType().IsAssignableFrom(typeof(CommonEventArgs)));

                CommonEventArgs e = (CommonEventArgs) args;
                e.m_eventID = m_eventID;
                e.m_eventType = m_eventType;
            }
        }
    }
}
