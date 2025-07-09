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
    /// 输入转发类型注册函数的属性类型定义
    /// </summary>
    [SystemAttributeUsage(SystemAttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public class OnInputDispatchCallAttribute : SystemAttribute
    {
        /// <summary>
        /// 输入响应的目标对象类型
        /// </summary>
        private readonly SystemType _classType;
        /// <summary>
        /// 输入编码唯一标识
        /// </summary>
        private readonly int _keycode;
        /// <summary>
        /// 输入操作类型
        /// </summary>
        private readonly InputOperationType _operationType;
        /// <summary>
        /// 派发侦听的输入数据类型
        /// </summary>
        private readonly SystemType _inputDataType;

        /// <summary>
        /// 目标对象类型获取函数
        /// </summary>
        public SystemType ClassType => _classType;
        /// <summary>
        /// 输入编码获取函数
        /// </summary>
        public int Keycode => _keycode;
        /// <summary>
        /// 输入操作类型获取函数
        /// </summary>
        public InputOperationType OperationType => _operationType;
        /// <summary>
        /// 输入数据类型获取函数
        /// </summary>
        public SystemType InputDataType => _inputDataType;

        public OnInputDispatchCallAttribute(int keycode) : this(null, keycode, InputOperationType.Unknown, null)
        { }

        public OnInputDispatchCallAttribute(int keycode, InputOperationType operationType) : this(null, keycode, operationType, null)
        { }

        public OnInputDispatchCallAttribute(SystemType inputDataType) : this(null, 0, InputOperationType.Unknown, inputDataType)
        { }

        public OnInputDispatchCallAttribute(SystemType classType, int keycode) : this(classType, keycode, InputOperationType.Unknown, null)
        { }

        public OnInputDispatchCallAttribute(SystemType classType, int keycode, InputOperationType operationType) : this(classType, keycode, operationType, null)
        { }

        public OnInputDispatchCallAttribute(SystemType classType, SystemType inputDataType) : this(classType, 0, InputOperationType.Unknown, inputDataType)
        { }

        private OnInputDispatchCallAttribute(SystemType classType, int keycode, InputOperationType operationType, SystemType inputDataType) : base()
        {
            _classType = classType;
            _keycode = keycode;
            _operationType = operationType;
            _inputDataType = inputDataType;
        }
    }

    /// <summary>
    /// 输入的监听绑定函数的属性类型定义
    /// </summary>
    [SystemAttributeUsage(SystemAttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public class InputListenerBindingOfTargetAttribute : SystemAttribute
    {
        /// <summary>
        /// 输入编码唯一标识
        /// </summary>
        private readonly int _keycode;
        /// <summary>
        /// 输入操作类型
        /// </summary>
        private readonly InputOperationType _operationType;
        /// <summary>
        /// 派发侦听的输入数据类型
        /// </summary>
        private readonly SystemType _inputDataType;
        /// <summary>
        /// 监听绑定的观察行为类型
        /// </summary>
        private readonly AspectBehaviourType _behaviourType;

        public int Keycode => _keycode;
        public InputOperationType OperationType => _operationType;
        public SystemType InputDataType => _inputDataType;
        public AspectBehaviourType BehaviourType => _behaviourType;

        public InputListenerBindingOfTargetAttribute(int keycode) : this(keycode, InputOperationType.Unknown, null, AspectBehaviourType.Initialize)
        { }

        public InputListenerBindingOfTargetAttribute(int keycode, AspectBehaviourType behaviourType) : this(keycode, InputOperationType.Unknown, null, behaviourType)
        { }

        public InputListenerBindingOfTargetAttribute(int keycode, InputOperationType operationType) : this(keycode, operationType, null, AspectBehaviourType.Initialize)
        { }

        public InputListenerBindingOfTargetAttribute(int keycode, InputOperationType operationType, AspectBehaviourType behaviourType) : this(keycode, operationType, null, behaviourType)
        { }

        public InputListenerBindingOfTargetAttribute(SystemType inputDataType) : this(0, InputOperationType.Unknown, inputDataType, AspectBehaviourType.Initialize)
        { }

        public InputListenerBindingOfTargetAttribute(SystemType inputDataType, AspectBehaviourType behaviourType) : this(0, InputOperationType.Unknown, inputDataType, behaviourType)
        { }

        private InputListenerBindingOfTargetAttribute(int keycode, InputOperationType operationType, SystemType inputDataType, AspectBehaviourType behaviourType) : base()
        {
            _keycode = keycode;
            _operationType = operationType;
            _inputDataType = inputDataType;
            _behaviourType = behaviourType;
        }
    }
}
