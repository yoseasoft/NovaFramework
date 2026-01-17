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

using System;

namespace GameEngine
{
    /// <summary>
    /// 消息系统基于对象分发的属性类型定义
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    internal sealed class MessageActivationAttribute : Attribute
    {
        public MessageActivationAttribute()
        {
        }
    }

    /// <summary>
    /// 消息系统基于全局分发的属性类型定义
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class MessageSystemAttribute : Attribute
    {
        public MessageSystemAttribute()
        {
        }
    }

    /// <summary>
    /// 消息对象类声明属性类型定义
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class MessageObjectAttribute : Attribute
    {
        /// <summary>
        /// 消息操作码
        /// </summary>
        private readonly int _opcode;
        /// <summary>
        /// 消息响应码
        /// </summary>
        private readonly int _responseCode;

        /// <summary>
        /// 消息操作码获取函数
        /// </summary>
        public int Opcode => _opcode;
        /// <summary>
        /// 消息响应码获取函数
        /// </summary>
        public int ResponseCode => _responseCode;

        public MessageObjectAttribute() : this(0)
        { }

        public MessageObjectAttribute(int opcode) : this(opcode, 0)
        { }

        public MessageObjectAttribute(int opcode, int responseCode) : base()
        {
            _opcode = opcode;
            _responseCode = responseCode;
        }
    }
}
