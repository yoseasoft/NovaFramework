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
    /// 消息分发类型注册函数的属性类型定义
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public class OnMessageDispatchCallAttribute : Attribute
    {
        /// <summary>
        /// 派发消息的目标对象类型
        /// </summary>
        private readonly Type _classType;
        /// <summary>
        /// 消息操作码标识
        /// </summary>
        private readonly int _opcode;
        /// <summary>
        /// 消息对象类型
        /// </summary>
        private readonly Type _messageType;

        /// <summary>
        /// 目标对象类型获取函数
        /// </summary>
        public Type ClassType => _classType;
        /// <summary>
        /// 消息操作码获取函数
        /// </summary>
        public int Opcode => _opcode;
        /// <summary>
        /// 消息对象类型获取函数
        /// </summary>
        public Type MessageType => _messageType;

        public OnMessageDispatchCallAttribute(int opcode) : this(null, opcode)
        { }

        public OnMessageDispatchCallAttribute(Type messageType) : this(null, messageType)
        { }

        public OnMessageDispatchCallAttribute(Type classType, int opcode) : this(classType, opcode, null)
        { }

        public OnMessageDispatchCallAttribute(Type classType, Type messageType) : this(classType, 0, messageType)
        { }

        private OnMessageDispatchCallAttribute(Type classType, int opcode, Type messageType) : base()
        {
            _classType = classType;
            _opcode = opcode;
            _messageType = messageType;
        }

        /// <summary>
        /// 重写 Equals 方法，基于值进行比较
        /// </summary>
        public override bool Equals(object obj)
        {
            // 1. 检查是否为同一个对象（引用相等）
            if (ReferenceEquals(this, obj)) return true;
            // 2. 检查对象是否为 null 或类型不同
            if (obj is null || this.GetType() != obj.GetType()) return false;

            // 3. 转换为当前类型后，比较所有关键字段的值
            OnMessageDispatchCallAttribute other = (OnMessageDispatchCallAttribute) obj;
            return _classType == other._classType &&
                   _opcode == other._opcode &&
                   _messageType == other._messageType;
        }

        /// <summary>
        /// 重写 GetHashCode 方法，必须与 Equals 逻辑保持一致
        /// </summary>
        public override int GetHashCode()
        {
            // 使用 HashCode.Combine 来组合多个字段的哈希值，这是一个推荐的做法
            return HashCode.Combine(_classType, _opcode, _messageType);
        }
    }

    /// <summary>
    /// 消息监听绑定函数的属性类型定义
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public class MessageListenerBindingOfTargetAttribute : Attribute
    {
        /// <summary>
        /// 消息操作码标识
        /// </summary>
        private readonly int _opcode;
        /// <summary>
        /// 消息对象类型
        /// </summary>
        private readonly Type _messageType;
        /// <summary>
        /// 监听绑定的观察行为类型
        /// </summary>
        private readonly AspectBehaviourType _behaviourType;

        public int Opcode => _opcode;
        public Type MessageType => _messageType;
        public AspectBehaviourType BehaviourType => _behaviourType;

        public MessageListenerBindingOfTargetAttribute(int opcode) : this(opcode, null, AspectBehaviourType.Initialize)
        { }

        public MessageListenerBindingOfTargetAttribute(int opcode, AspectBehaviourType behaviourType) : this(opcode, null, behaviourType)
        { }

        public MessageListenerBindingOfTargetAttribute(Type messageType) : this(0, messageType, AspectBehaviourType.Initialize)
        { }

        public MessageListenerBindingOfTargetAttribute(Type messageType, AspectBehaviourType behaviourType) : this(0, messageType, behaviourType)
        { }

        private MessageListenerBindingOfTargetAttribute(int opcode, Type messageType, AspectBehaviourType behaviourType) : base()
        {
            _opcode = opcode;
            _messageType = messageType;
            _behaviourType = behaviourType;
        }

        /// <summary>
        /// 重写 Equals 方法，基于值进行比较
        /// </summary>
        public override bool Equals(object obj)
        {
            // 1. 检查是否为同一个对象（引用相等）
            if (ReferenceEquals(this, obj)) return true;
            // 2. 检查对象是否为 null 或类型不同
            if (obj is null || this.GetType() != obj.GetType()) return false;

            // 3. 转换为当前类型后，比较所有关键字段的值
            MessageListenerBindingOfTargetAttribute other = (MessageListenerBindingOfTargetAttribute) obj;
            return _opcode == other._opcode &&
                   _messageType == other._messageType; // && _behaviourType == other._behaviourType;
        }

        /// <summary>
        /// 重写 GetHashCode 方法，必须与 Equals 逻辑保持一致
        /// </summary>
        public override int GetHashCode()
        {
            // 使用 HashCode.Combine 来组合多个字段的哈希值，这是一个推荐的做法
            return HashCode.Combine(_opcode, _messageType); // , _behaviourType);
        }
    }
}
