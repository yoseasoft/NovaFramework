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
    /// 切面调用的属性类型定义
    /// </summary>
    [SystemAttributeUsage(SystemAttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class OnAspectCallAttribute : SystemAttribute
    {
        /// <summary>
        /// 定义切点的函数名称
        /// </summary>
        private readonly string _methodName;
        /// <summary>
        /// 定义切点的访问类型
        /// </summary>
        private readonly AspectAccessType _accessType;

        public string MethodName => _methodName;
        public AspectAccessType AccessType => _accessType;

        public OnAspectCallAttribute(string methodName, AspectAccessType accessType)
        {
            _methodName = methodName;
            _accessType = accessType;
        }

        public OnAspectCallAttribute(AspectBehaviourType behaviourType, AspectAccessType accessType)
        {
            if (false == NovaEngine.Utility.Convertion.IsCorrectedEnumValue<AspectBehaviourType>((int) behaviourType))
            {
                Debugger.Error("Invalid aspect behaviour type ({0}).", behaviourType.ToString());
                return;
            }

            _methodName = behaviourType.ToString();
            _accessType = accessType;
        }
    }

    /// <summary>
    /// 针对特定类型的切面调用的属性类型定义
    /// </summary>
    [SystemAttributeUsage(SystemAttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class OnAspectCallOfTargetAttribute : SystemAttribute
    {
        /// <summary>
        /// 匹配切点的目标对象类型
        /// </summary>
        private readonly SystemType _classType;
        /// <summary>
        /// 定义切点的函数名称
        /// </summary>
        private readonly string _methodName;
        /// <summary>
        /// 定义切点的访问类型
        /// </summary>
        private readonly AspectAccessType _accessType;

        public SystemType ClassType => _classType;
        public string MethodName => _methodName;
        public AspectAccessType AccessType => _accessType;

        public OnAspectCallOfTargetAttribute(SystemType classType, string methodName, AspectAccessType accessType)
        {
            _classType = classType;
            _methodName = methodName;
            _accessType = accessType;
        }

        public OnAspectCallOfTargetAttribute(SystemType classType, AspectBehaviourType behaviourType, AspectAccessType accessType)
        {
            if (false == NovaEngine.Utility.Convertion.IsCorrectedEnumValue<AspectBehaviourType>((int) behaviourType))
            {
                Debugger.Error("Invalid aspect behaviour type ({0}).", behaviourType.ToString());
                return;
            }

            _classType = classType;
            _methodName = behaviourType.ToString();
            _accessType = accessType;
        }
    }

    /// <summary>
    /// 切面扩展调用的属性类型定义
    /// </summary>
    [SystemAttributeUsage(SystemAttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class OnAspectExtendCallAttribute : OnAspectCallAttribute
    {
        public OnAspectExtendCallAttribute(string methodName) : base(methodName, AspectAccessType.Extend)
        { }

        public OnAspectExtendCallAttribute(AspectBehaviourType behaviourType) : base(behaviourType, AspectAccessType.Extend)
        { }
    }

    /// <summary>
    /// 针对特定类型的切面扩展调用的属性类型定义
    /// </summary>
    [SystemAttributeUsage(SystemAttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class OnAspectExtendCallOfTargetAttribute : OnAspectCallOfTargetAttribute
    {
        public OnAspectExtendCallOfTargetAttribute(SystemType classType, string methodName) : base(classType, methodName, AspectAccessType.Extend)
        { }

        public OnAspectExtendCallOfTargetAttribute(SystemType classType, AspectBehaviourType behaviourType) : base(classType, behaviourType, AspectAccessType.Extend)
        { }
    }

    /// <summary>
    /// 切面前置调用的属性类型定义
    /// </summary>
    [SystemAttributeUsage(SystemAttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class OnAspectBeforeCallAttribute : OnAspectCallAttribute
    {
        public OnAspectBeforeCallAttribute(string methodName) : base(methodName, AspectAccessType.Before)
        { }

        public OnAspectBeforeCallAttribute(AspectBehaviourType behaviourType) : base(behaviourType, AspectAccessType.Before)
        { }
    }

    /// <summary>
    /// 针对特定类型的切面前置调用的属性类型定义
    /// </summary>
    [SystemAttributeUsage(SystemAttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class OnAspectBeforeCallOfTargetAttribute : OnAspectCallOfTargetAttribute
    {
        public OnAspectBeforeCallOfTargetAttribute(SystemType classType, string methodName) : base(classType, methodName, AspectAccessType.Before)
        { }

        public OnAspectBeforeCallOfTargetAttribute(SystemType classType, AspectBehaviourType behaviourType) : base(classType, behaviourType, AspectAccessType.Before)
        { }
    }

    /// <summary>
    /// 切面后置调用的属性类型定义
    /// </summary>
    [SystemAttributeUsage(SystemAttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class OnAspectAfterCallAttribute : OnAspectCallAttribute
    {
        public OnAspectAfterCallAttribute(string methodName) : base(methodName, AspectAccessType.After)
        { }

        public OnAspectAfterCallAttribute(AspectBehaviourType behaviourType) : base(behaviourType, AspectAccessType.After)
        { }
    }

    /// <summary>
    /// 针对特定类型的切面后置调用的属性类型定义
    /// </summary>
    [SystemAttributeUsage(SystemAttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class OnAspectAfterCallOfTargetAttribute : OnAspectCallOfTargetAttribute
    {
        public OnAspectAfterCallOfTargetAttribute(SystemType classType, string methodName) : base(classType, methodName, AspectAccessType.After)
        { }

        public OnAspectAfterCallOfTargetAttribute(SystemType classType, AspectBehaviourType behaviourType) : base(classType, behaviourType, AspectAccessType.After)
        { }
    }

    /// <summary>
    /// 切面返回调用的属性类型定义
    /// </summary>
    [SystemAttributeUsage(SystemAttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class OnAspectAfterReturningCallAttribute : OnAspectCallAttribute
    {
        public OnAspectAfterReturningCallAttribute(string methodName) : base(methodName, AspectAccessType.AfterReturning)
        { }

        public OnAspectAfterReturningCallAttribute(AspectBehaviourType behaviourType) : base(behaviourType, AspectAccessType.AfterReturning)
        { }
    }

    /// <summary>
    /// 针对特定类型的切面返回调用的属性类型定义
    /// </summary>
    [SystemAttributeUsage(SystemAttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class OnAspectAfterReturningCallOfTargetAttribute : OnAspectCallOfTargetAttribute
    {
        public OnAspectAfterReturningCallOfTargetAttribute(SystemType classType, string methodName) : base(classType, methodName, AspectAccessType.AfterReturning)
        { }

        public OnAspectAfterReturningCallOfTargetAttribute(SystemType classType, AspectBehaviourType behaviourType) : base(classType, behaviourType, AspectAccessType.AfterReturning)
        { }
    }

    /// <summary>
    /// 切面异常调用的属性类型定义
    /// </summary>
    [SystemAttributeUsage(SystemAttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class OnAspectAfterThrowingCallAttribute : OnAspectCallAttribute
    {
        public OnAspectAfterThrowingCallAttribute(string methodName) : base(methodName, AspectAccessType.AfterThrowing)
        { }

        public OnAspectAfterThrowingCallAttribute(AspectBehaviourType behaviourType) : base(behaviourType, AspectAccessType.AfterThrowing)
        { }
    }

    /// <summary>
    /// 针对特定类型的切面异常调用的属性类型定义
    /// </summary>
    [SystemAttributeUsage(SystemAttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class OnAspectAfterThrowingCallOfTargetAttribute : OnAspectCallOfTargetAttribute
    {
        public OnAspectAfterThrowingCallOfTargetAttribute(SystemType classType, string methodName) : base(classType, methodName, AspectAccessType.AfterThrowing)
        { }

        public OnAspectAfterThrowingCallOfTargetAttribute(SystemType classType, AspectBehaviourType behaviourType) : base(classType, behaviourType, AspectAccessType.AfterThrowing)
        { }
    }

    /// <summary>
    /// 切面环绕调用的属性类型定义
    /// </summary>
    [SystemAttributeUsage(SystemAttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class OnAspectAroundCallAttribute : OnAspectCallAttribute
    {
        public OnAspectAroundCallAttribute(string methodName) : base(methodName, AspectAccessType.Around)
        { }

        public OnAspectAroundCallAttribute(AspectBehaviourType behaviourType) : base(behaviourType, AspectAccessType.Around)
        { }
    }

    /// <summary>
    /// 针对特定类型的切面环绕调用的属性类型定义
    /// </summary>
    [SystemAttributeUsage(SystemAttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class OnAspectAroundCallOfTargetAttribute : OnAspectCallOfTargetAttribute
    {
        public OnAspectAroundCallOfTargetAttribute(SystemType classType, string methodName) : base(classType, methodName, AspectAccessType.Around)
        { }

        public OnAspectAroundCallOfTargetAttribute(SystemType classType, AspectBehaviourType behaviourType) : base(classType, behaviourType, AspectAccessType.Around)
        { }
    }
}
