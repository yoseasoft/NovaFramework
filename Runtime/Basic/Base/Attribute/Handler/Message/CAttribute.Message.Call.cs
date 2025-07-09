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

using SystemType = System.Type;
using SystemAttribute = System.Attribute;
using SystemAttributeUsageAttribute = System.AttributeUsageAttribute;
using SystemAttributeTargets = System.AttributeTargets;

namespace GameEngine
{
    /// <summary>
    /// 消息分发类型注册函数的属性类型定义
    /// </summary>
    [SystemAttributeUsage(SystemAttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public class OnMessageDispatchCallAttribute : SystemAttribute
    {
        /// <summary>
        /// 派发消息的目标对象类型
        /// </summary>
        private readonly SystemType _classType;
        /// <summary>
        /// 消息操作码标识
        /// </summary>
        private readonly int _opcode;
        /// <summary>
        /// 消息对象类型
        /// </summary>
        private readonly SystemType _messageType;

        /// <summary>
        /// 目标对象类型获取函数
        /// </summary>
        public SystemType ClassType => _classType;
        /// <summary>
        /// 消息操作码获取函数
        /// </summary>
        public int Opcode => _opcode;
        /// <summary>
        /// 消息对象类型获取函数
        /// </summary>
        public SystemType MessageType => _messageType;

        public OnMessageDispatchCallAttribute(int opcode) : this(null, opcode)
        { }

        public OnMessageDispatchCallAttribute(SystemType messageType) : this(null, messageType)
        { }

        public OnMessageDispatchCallAttribute(SystemType classType, int opcode) : this(classType, opcode, null)
        { }

        public OnMessageDispatchCallAttribute(SystemType classType, SystemType messageType) : this(classType, 0, messageType)
        { }

        private OnMessageDispatchCallAttribute(SystemType classType, int opcode, SystemType messageType) : base()
        {
            _classType = classType;
            _opcode = opcode;
            _messageType = messageType;
        }
    }

    /// <summary>
    /// 消息监听绑定函数的属性类型定义
    /// </summary>
    [SystemAttributeUsage(SystemAttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public class MessageListenerBindingOfTargetAttribute : SystemAttribute
    {
        /// <summary>
        /// 消息操作码标识
        /// </summary>
        private readonly int _opcode;
        /// <summary>
        /// 消息对象类型
        /// </summary>
        private readonly SystemType _messageType;
        /// <summary>
        /// 监听绑定的观察行为类型
        /// </summary>
        private readonly AspectBehaviourType _behaviourType;

        public int Opcode => _opcode;
        public SystemType MessageType => _messageType;
        public AspectBehaviourType BehaviourType => _behaviourType;

        public MessageListenerBindingOfTargetAttribute(int opcode) : this(opcode, null, AspectBehaviourType.Initialize)
        { }

        public MessageListenerBindingOfTargetAttribute(int opcode, AspectBehaviourType behaviourType) : this(opcode, null, behaviourType)
        { }

        public MessageListenerBindingOfTargetAttribute(SystemType messageType) : this(0, messageType, AspectBehaviourType.Initialize)
        { }

        public MessageListenerBindingOfTargetAttribute(SystemType messageType, AspectBehaviourType behaviourType) : this(0, messageType, behaviourType)
        { }

        private MessageListenerBindingOfTargetAttribute(int opcode, SystemType messageType, AspectBehaviourType behaviourType) : base()
        {
            _opcode = opcode;
            _messageType = messageType;
            _behaviourType = behaviourType;
        }
    }
}
