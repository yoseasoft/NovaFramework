/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyright (C) 2024 - 2025, Hurley, Independent Studio.
/// Copyright (C) 2025 - 2026, Hainan Yuanyou Information Technology Co., Ltd. Guangzhou Branch
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
    /// 输入转发类型注册函数的属性类型定义
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public class OnInputDispatchCallAttribute : Attribute
    {
        /// <summary>
        /// 输入响应的目标对象类型
        /// </summary>
        private readonly Type _classType;
        /// <summary>
        /// 输入响应的目标按键编码
        /// </summary>
        private readonly VirtualKeyCode _keyCode;
        /// <summary>
        /// 输入响应的目标操作类型
        /// </summary>
        private readonly InputOperationType _operationType;
        /// <summary>
        /// 输入响应的目标数据类型
        /// </summary>
        private readonly Type _inputDataType;
        /// <summary>
        /// 输入响应的观察行为类型
        /// </summary>
        private readonly AspectBehaviourType _behaviourType;

        /// <summary>
        /// 目标对象类型获取函数
        /// </summary>
        public Type ClassType => _classType;
        /// <summary>
        /// 输入按键编码获取函数
        /// </summary>
        public VirtualKeyCode KeyCode => _keyCode;
        /// <summary>
        /// 输入操作类型获取函数
        /// </summary>
        public InputOperationType OperationType => _operationType;
        /// <summary>
        /// 输入数据类型获取函数
        /// </summary>
        public Type InputDataType => _inputDataType;
        /// <summary>
        /// 输入观察行为类型获取函数
        /// </summary>
        public AspectBehaviourType BehaviourType => _behaviourType;

        public OnInputDispatchCallAttribute(VirtualKeyCode keyCode)
            : this(keyCode, AspectBehaviour.DefaultBehaviourTypeForAutomaticallyDispatchedProcessingNode)
        { }

        public OnInputDispatchCallAttribute(VirtualKeyCode keyCode, AspectBehaviourType behaviourType)
            : this(null, keyCode, InputOperationType.Unknown, null, behaviourType)
        { }

        public OnInputDispatchCallAttribute(VirtualKeyCode keyCode, InputOperationType operationType)
            : this(keyCode, operationType, AspectBehaviour.DefaultBehaviourTypeForAutomaticallyDispatchedProcessingNode)
        { }

        public OnInputDispatchCallAttribute(VirtualKeyCode keyCode, InputOperationType operationType, AspectBehaviourType behaviourType)
            : this(null, keyCode, operationType, null, behaviourType)
        { }

        public OnInputDispatchCallAttribute(Type inputDataType)
            : this(inputDataType, AspectBehaviour.DefaultBehaviourTypeForAutomaticallyDispatchedProcessingNode)
        { }

        public OnInputDispatchCallAttribute(Type inputDataType, AspectBehaviourType behaviourType)
            : this(null, VirtualKeyCode.None, InputOperationType.Unknown, inputDataType, behaviourType)
        { }

        public OnInputDispatchCallAttribute(Type classType, VirtualKeyCode keyCode)
            : this(classType, keyCode, InputOperationType.Unknown, null, AspectBehaviourType.Unknown)
        { }

        public OnInputDispatchCallAttribute(Type classType, VirtualKeyCode keyCode, InputOperationType operationType)
            : this(classType, keyCode, operationType, null, AspectBehaviourType.Unknown)
        { }

        public OnInputDispatchCallAttribute(Type classType, Type inputDataType)
            : this(classType, VirtualKeyCode.None, InputOperationType.Unknown, inputDataType, AspectBehaviourType.Unknown)
        { }

        private OnInputDispatchCallAttribute(Type classType,
                                             VirtualKeyCode keyCode,
                                             InputOperationType operationType,
                                             Type inputDataType,
                                             AspectBehaviourType behaviourType) : base()
        {
            _classType = classType;
            _keyCode = keyCode;
            _operationType = operationType;
            _inputDataType = inputDataType;
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
            OnInputDispatchCallAttribute other = (OnInputDispatchCallAttribute) obj;
            return _classType == other._classType &&
                   _keyCode == other._keyCode &&
                   _operationType == other._operationType &&
                   _inputDataType == other._inputDataType; // && _behaviourType == other._behaviourType;
        }

        /// <summary>
        /// 重写 GetHashCode 方法，必须与 Equals 逻辑保持一致
        /// </summary>
        public override int GetHashCode()
        {
            // 使用 HashCode.Combine 来组合多个字段的哈希值，这是一个推荐的做法
            return HashCode.Combine(_classType, _keyCode, _operationType, _inputDataType); // , _behaviourType);
        }
    }
}
