/// -------------------------------------------------------------------------------
/// GameEngine Framework
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

using SystemType = System.Type;

namespace GameEngine
{
    /// <summary>
    /// 消息对象类型加载器抽象类，用于定义消息对象类型属性访问的接口函数
    /// </summary>
    public interface IMessageObjectTypeLoader
    {
        /// <summary>
        /// 消息类型信息数据结构
        /// </summary>
        public struct MessageObjectTypeInfo
        {
            /// <summary>
            /// 操作码
            /// </summary>
            public int opcode;
            /// <summary>
            /// 响应码
            /// </summary>
            public int responseCode;
        }

        /// <summary>
        /// 获取指定消息对象类型的数据结构
        /// </summary>
        /// <param name="messageType">消息对象类型</param>
        /// <returns>返回消息对象类型的数据结构</returns>
        MessageObjectTypeInfo Parse(SystemType messageType);
    }
}
