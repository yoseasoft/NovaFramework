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
    /// 事件分发类型注册函数的属性类型定义
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public class OnEventDispatchCallAttribute : Attribute
    {
        /// <summary>
        /// 派发事件的目标对象类型
        /// </summary>
        private readonly Type _classType;
        /// <summary>
        /// 派发侦听的事件标识
        /// </summary>
        private readonly int _eventID;
        /// <summary>
        /// 派发侦听的事件数据类型
        /// </summary>
        private readonly Type _eventDataType;

        /// <summary>
        /// 目标对象类型获取函数
        /// </summary>
        public Type ClassType => _classType;
        /// <summary>
        /// 事件标识获取函数
        /// </summary>
        public int EventID => _eventID;
        /// <summary>
        /// 事件数据类型获取函数
        /// </summary>
        public Type EventDataType => _eventDataType;

        public OnEventDispatchCallAttribute(int eventID) : this(null, eventID)
        { }

        public OnEventDispatchCallAttribute(Type eventDataType) : this(null, eventDataType)
        { }

        public OnEventDispatchCallAttribute(Type classType, int eventID) : this(classType, eventID, null)
        { }

        public OnEventDispatchCallAttribute(Type classType, Type eventDataType) : this(classType, 0, eventDataType)
        { }

        private OnEventDispatchCallAttribute(Type classType, int eventID, Type eventDataType) : base()
        {
            _classType = classType;
            _eventID = eventID;
            _eventDataType = eventDataType;
        }
    }

    /// <summary>
    /// 事件订阅绑定函数的属性类型定义
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public class EventSubscribeBindingOfTargetAttribute : Attribute
    {
        /// <summary>
        /// 订阅绑定的事件标识
        /// </summary>
        private readonly int _eventID;
        /// <summary>
        /// 订阅绑定的事件数据类型
        /// </summary>
        private readonly Type _eventDataType;
        /// <summary>
        /// 订阅绑定的观察行为类型
        /// </summary>
        private readonly AspectBehaviourType _behaviourType;

        public int EventID => _eventID;
        public Type EventDataType => _eventDataType;
        public AspectBehaviourType BehaviourType => _behaviourType;

        public EventSubscribeBindingOfTargetAttribute(int eventID) : this(eventID, null, AspectBehaviour.AutobindBehaviourTypeOfBeanExtensionMethod)
        { }

        public EventSubscribeBindingOfTargetAttribute(int eventID, AspectBehaviourType behaviourType) : this(eventID, null, behaviourType)
        { }

        public EventSubscribeBindingOfTargetAttribute(Type eventDataType) : this(0, eventDataType, AspectBehaviour.AutobindBehaviourTypeOfBeanExtensionMethod)
        { }

        public EventSubscribeBindingOfTargetAttribute(Type eventDataType, AspectBehaviourType behaviourType) : this(0, eventDataType, behaviourType)
        { }

        private EventSubscribeBindingOfTargetAttribute(int eventID, Type eventDataType, AspectBehaviourType behaviourType) : base()
        {
            _eventID = eventID;
            _eventDataType = eventDataType;
            _behaviourType = behaviourType;
        }
    }
}
