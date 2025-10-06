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

using SystemAttribute = System.Attribute;
using SystemAttributeUsageAttribute = System.AttributeUsageAttribute;
using SystemAttributeTargets = System.AttributeTargets;

namespace GameEngine
{
    /// <summary>
    /// 控制器类的子模块初始化回调句柄的声明属性类型定义
    /// </summary>
    [SystemAttributeUsage(SystemAttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    internal class OnControllerSubmoduleInitCallbackAttribute : SystemAttribute
    {
        public OnControllerSubmoduleInitCallbackAttribute() : base() { }
    }

    /// <summary>
    /// 控制器类的子模块清理回调句柄的声明属性类型定义
    /// </summary>
    [SystemAttributeUsage(SystemAttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    internal class OnControllerSubmoduleCleanupCallbackAttribute : SystemAttribute
    {
        public OnControllerSubmoduleCleanupCallbackAttribute() : base() { }
    }

    /// <summary>
    /// 控制器类的子模块刷新回调句柄的声明属性类型定义
    /// </summary>
    [SystemAttributeUsage(SystemAttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    internal class OnControllerSubmoduleUpdateCallbackAttribute : SystemAttribute
    {
        public OnControllerSubmoduleUpdateCallbackAttribute() : base() { }
    }

    /// <summary>
    /// 控制器类的子模块后置刷新回调句柄的声明属性类型定义
    /// </summary>
    [SystemAttributeUsage(SystemAttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    internal class OnControllerSubmoduleLateUpdateCallbackAttribute : SystemAttribute
    {
        public OnControllerSubmoduleLateUpdateCallbackAttribute() : base() { }
    }

    /// <summary>
    /// 控制器类的子模块重载回调句柄的声明属性类型定义
    /// </summary>
    [SystemAttributeUsage(SystemAttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    internal class OnControllerSubmoduleReloadCallbackAttribute : SystemAttribute
    {
        public OnControllerSubmoduleReloadCallbackAttribute() : base() { }
    }

    /// <summary>
    /// 控制器类的子模块倾泻回调句柄的声明属性类型定义
    /// </summary>
    [SystemAttributeUsage(SystemAttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    internal class OnControllerSubmoduleDumpCallbackAttribute : SystemAttribute
    {
        public OnControllerSubmoduleDumpCallbackAttribute() : base() { }
    }
}
