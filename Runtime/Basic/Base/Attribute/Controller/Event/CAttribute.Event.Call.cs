/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyring (C) 2023 - 2024, Guangzhou Shiyue Network Technology Co., Ltd.
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
    /// 事件分发类型注册函数的属性类型定义
    /// </summary>
    [SystemAttributeUsage(SystemAttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class OnEventDispatchCallAttribute : SystemAttribute
    {
        /// <summary>
        /// 派发事件的目标对象类型
        /// </summary>
        private readonly SystemType m_classType;
        /// <summary>
        /// 派发侦听的事件标识
        /// </summary>
        private readonly int m_eventID;
        /// <summary>
        /// 派发侦听的事件数据类型
        /// </summary>
        private readonly SystemType m_eventDataType;

        /// <summary>
        /// 目标对象类型获取函数
        /// </summary>
        public SystemType ClassType => m_classType;
        /// <summary>
        /// 事件标识获取函数
        /// </summary>
        public int EventID => m_eventID;
        /// <summary>
        /// 事件数据类型获取函数
        /// </summary>
        public SystemType EventDataType => m_eventDataType;

        public OnEventDispatchCallAttribute(int eventID) : this(null, eventID)
        { }

        public OnEventDispatchCallAttribute(SystemType eventDataType) : this(null, eventDataType)
        { }

        public OnEventDispatchCallAttribute(SystemType classType, int eventID) : this(classType, eventID, null)
        { }

        public OnEventDispatchCallAttribute(SystemType classType, SystemType eventDataType) : this(classType, 0, eventDataType)
        { }

        private OnEventDispatchCallAttribute(SystemType classType, int eventID, SystemType eventDataType) : base()
        {
            m_classType = classType;
            m_eventID = eventID;
            m_eventDataType = eventDataType;
        }
    }

    /// <summary>
    /// 事件订阅绑定函数的属性类型定义
    /// </summary>
    [SystemAttributeUsage(SystemAttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class EventSubscribeBindingOfTargetAttribute : SystemAttribute
    {
        /// <summary>
        /// 订阅绑定的事件标识
        /// </summary>
        private readonly int m_eventID;
        /// <summary>
        /// 订阅绑定的事件数据类型
        /// </summary>
        private readonly SystemType m_eventDataType;
        /// <summary>
        /// 订阅绑定的观察行为类型
        /// </summary>
        private readonly AspectBehaviourType m_behaviourType;

        public int EventID => m_eventID;
        public SystemType EventDataType => m_eventDataType;
        public AspectBehaviourType BehaviourType => m_behaviourType;

        public EventSubscribeBindingOfTargetAttribute(int eventID) : this(eventID, null, AspectBehaviourType.Initialize)
        { }

        public EventSubscribeBindingOfTargetAttribute(int eventID, AspectBehaviourType behaviourType) : this(eventID, null, behaviourType)
        { }

        public EventSubscribeBindingOfTargetAttribute(SystemType eventDataType) : this(0, eventDataType, AspectBehaviourType.Initialize)
        { }

        public EventSubscribeBindingOfTargetAttribute(SystemType eventDataType, AspectBehaviourType behaviourType) : this(0, eventDataType, behaviourType)
        { }

        private EventSubscribeBindingOfTargetAttribute(int eventID, SystemType eventDataType, AspectBehaviourType behaviourType) : base()
        {
            m_eventID = eventID;
            m_eventDataType = eventDataType;
            m_behaviourType = behaviourType;
        }
    }
}
