/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyright (C) 2024 - 2025, Hurley, Independent Studio.
/// Copyright (C) 2025, Hainan Yuanyou Information Tecdhnology Co., Ltd. Guangzhou Branch
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
    /// 键码响应类型注册函数的属性类型定义
    /// </summary>
    [SystemAttributeUsage(SystemAttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public class OnKeycodeDispatchResponseAttribute : SystemAttribute
    {
        /// <summary>
        /// 响应键码的目标对象类型
        /// </summary>
        private readonly SystemType m_classType;
        /// <summary>
        /// 键码唯一标识
        /// </summary>
        private readonly int m_keycode;
        /// <summary>
        /// 键码操作类型
        /// </summary>
        private readonly InputOperationType m_operationType;
        /// <summary>
        /// 派发侦听的输入数据类型
        /// </summary>
        private readonly SystemType m_inputDataType;

        /// <summary>
        /// 目标对象类型获取函数
        /// </summary>
        public SystemType ClassType => m_classType;
        /// <summary>
        /// 键码获取函数
        /// </summary>
        public int Keycode => m_keycode;
        /// <summary>
        /// 操作类型获取函数
        /// </summary>
        public InputOperationType OperationType => m_operationType;
        /// <summary>
        /// 输入数据类型获取函数
        /// </summary>
        public SystemType InputDataType => m_inputDataType;

        public OnKeycodeDispatchResponseAttribute(int keycode) : this(null, keycode, InputOperationType.Unknown, null)
        { }

        public OnKeycodeDispatchResponseAttribute(int keycode, InputOperationType operationType) : this(null, keycode, operationType, null)
        { }

        public OnKeycodeDispatchResponseAttribute(SystemType inputDataType) : this(null, 0, InputOperationType.Unknown, inputDataType)
        { }

        public OnKeycodeDispatchResponseAttribute(SystemType classType, int keycode) : this(classType, keycode, InputOperationType.Unknown, null)
        { }

        public OnKeycodeDispatchResponseAttribute(SystemType classType, int keycode, InputOperationType operationType) : this(classType, keycode, operationType, null)
        { }

        public OnKeycodeDispatchResponseAttribute(SystemType classType, SystemType inputDataType) : this(classType, 0, InputOperationType.Unknown, inputDataType)
        { }

        private OnKeycodeDispatchResponseAttribute(SystemType classType, int keycode, InputOperationType operationType, SystemType inputDataType) : base()
        {
            m_classType = classType;
            m_keycode = keycode;
            m_operationType = operationType;
            m_inputDataType = inputDataType;
        }
    }

    /// <summary>
    /// 键码监听绑定函数的属性类型定义
    /// </summary>
    [SystemAttributeUsage(SystemAttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public class KeycodeListenerBindingOfTargetAttribute : SystemAttribute
    {
        /// <summary>
        /// 键码唯一标识
        /// </summary>
        private readonly int m_keycode;
        /// <summary>
        /// 键码操作类型
        /// </summary>
        private readonly InputOperationType m_operationType;
        /// <summary>
        /// 派发侦听的输入数据类型
        /// </summary>
        private readonly SystemType m_inputDataType;
        /// <summary>
        /// 监听绑定的观察行为类型
        /// </summary>
        private readonly AspectBehaviourType m_behaviourType;

        public int Keycode => m_keycode;
        public InputOperationType OperationType => m_operationType;
        public SystemType InputDataType => m_inputDataType;
        public AspectBehaviourType BehaviourType => m_behaviourType;

        public KeycodeListenerBindingOfTargetAttribute(int keycode) : this(keycode, InputOperationType.Unknown, null, AspectBehaviourType.Initialize)
        { }

        public KeycodeListenerBindingOfTargetAttribute(int keycode, AspectBehaviourType behaviourType) : this(keycode, InputOperationType.Unknown, null, behaviourType)
        { }

        public KeycodeListenerBindingOfTargetAttribute(int keycode, InputOperationType operationType) : this(keycode, operationType, null, AspectBehaviourType.Initialize)
        { }

        public KeycodeListenerBindingOfTargetAttribute(int keycode, InputOperationType operationType, AspectBehaviourType behaviourType) : this(keycode, operationType, null, behaviourType)
        { }

        public KeycodeListenerBindingOfTargetAttribute(SystemType inputDataType) : this(0, InputOperationType.Unknown, inputDataType, AspectBehaviourType.Initialize)
        { }

        public KeycodeListenerBindingOfTargetAttribute(SystemType inputDataType, AspectBehaviourType behaviourType) : this(0, InputOperationType.Unknown, inputDataType, behaviourType)
        { }

        private KeycodeListenerBindingOfTargetAttribute(int keycode, InputOperationType operationType, SystemType inputDataType, AspectBehaviourType behaviourType) : base()
        {
            m_keycode = keycode;
            m_operationType = operationType;
            m_inputDataType = inputDataType;
            m_behaviourType = behaviourType;
        }
    }
}
