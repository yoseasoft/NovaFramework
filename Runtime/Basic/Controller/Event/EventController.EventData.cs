/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyright (C) 2023 - 2024, Guangzhou Shiyue Network Technology Co., Ltd.
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

namespace GameEngine
{
    /// <summary>
    /// 事件管理对象类，用于对场景上下文中的所有节点对象进行事件管理及分发
    /// </summary>
    public partial class EventController
    {
        /// <summary>
        /// 事件的数据存储对象类，用于临时存放事件的参数列表
        /// </summary>
        private class EventData : NovaEngine.IReference
        {
            /// <summary>
            /// 事件对象唯一标识
            /// </summary>
            private int _eventID;
            /// <summary>
            /// 事件处理参数列表
            /// </summary>
            private object[] _params;

            public int EventID => _eventID;
            public object[] Params => _params;

            /// <summary>
            /// 事件数据对象的构造函数
            /// </summary>
            public EventData()
            {
                _eventID = 0;
                _params = null;
            }

            /// <summary>
            /// 事件数据对象的构造函数
            /// </summary>
            /// <param name="_eventID">事件唯一标识</param>
            /// <param name="_params">事件参数列表</param>
            public EventData(int _eventID, params object[] _params)
            {
                this._eventID = _eventID;
                this._params = _params;
            }

            /// <summary>
            /// 事件数据对象的构造函数
            /// </summary>
            /// <param name="param">事件数据</param>
            public EventData(object param)
            {
                _eventID = 0;
                _params = new object[] { param };
            }

            /// <summary>
            /// 事件数据对象初始化函数接口
            /// </summary>
            public void Initialize()
            {
            }

            /// <summary>
            /// 事件数据对象清理函数接口
            /// </summary>
            public void Cleanup()
            {
                _eventID = 0;
                _params = null;
            }
        }
    }
}
