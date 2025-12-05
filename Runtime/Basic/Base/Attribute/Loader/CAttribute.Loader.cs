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
    /// 代码加载类的加载函数的属性定义
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    internal class OnCodeLoaderClassLoadOfTargetAttribute : Attribute
    {
        private readonly Type _classType;

        public Type ClassType => _classType;

        public OnCodeLoaderClassLoadOfTargetAttribute(Type classType)
        {
            _classType = classType;
        }
    }

    /// <summary>
    /// 代码加载类的清理函数的属性定义
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    internal class OnCodeLoaderClassCleanupOfTargetAttribute : Attribute
    {
        private readonly Type _classType;

        public Type ClassType => _classType;

        public OnCodeLoaderClassCleanupOfTargetAttribute(Type classType)
        {
            _classType = classType;
        }
    }

    /// <summary>
    /// 代码加载类的结构信息查找函数的属性定义
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    internal class OnCodeLoaderClassLookupOfTargetAttribute : Attribute
    {
        private readonly Type _classType;

        public Type ClassType => _classType;

        public OnCodeLoaderClassLookupOfTargetAttribute(Type classType)
        {
            _classType = classType;
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// 绑定处理接口注册函数的属性类型定义
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    internal class OnProcessRegisterClassOfTargetAttribute : Attribute
    {
        private readonly Type _classType;

        public Type ClassType => _classType;

        public OnProcessRegisterClassOfTargetAttribute(Type classType) { _classType = classType; }
    }

    /// <summary>
    /// 绑定处理接口注销函数的属性类型定义
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    internal class OnProcessUnregisterClassOfTargetAttribute : Attribute
    {
        private readonly Type _classType;

        public Type ClassType => _classType;

        public OnProcessUnregisterClassOfTargetAttribute(Type classType) { _classType = classType; }
    }
}
