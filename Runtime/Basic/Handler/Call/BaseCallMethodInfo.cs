/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyright (C) 2023 - 2024, Guangzhou Shiyue Network Technology Co., Ltd.
/// Copyright (C) 2024 - 2025, Hurley, Independent Studio.
/// Copyright (C) 2025, Hainan Yuanyou Information Technology Co., Ltd. Guangzhou Branch
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
using System.Reflection;

namespace GameEngine
{
    /// <summary>
    /// 基础调用函数信息封装对象类
    /// </summary>
    internal abstract class BaseCallMethodInfo
    {
        /// <summary>
        /// 回调函数的目标对象实例
        /// </summary>
        protected readonly IBean _targetObject;
        /// <summary>
        /// 回调函数的完整名称
        /// </summary>
        protected readonly string _fullname;
        /// <summary>
        /// 回调函数的目标对象类型
        /// </summary>
        protected readonly Type _targetType;
        /// <summary>
        /// 回调函数的函数信息实例
        /// </summary>
        protected readonly MethodInfo _methodInfo;
        /// <summary>
        /// 回调函数的动态构建回调句柄
        /// </summary>
        protected readonly Delegate _callback;
        /// <summary>
        /// 回调函数的自动注册状态标识
        /// </summary>
        protected readonly bool _automatically;
        /// <summary>
        /// 回调函数的静态类型定义状态标识
        /// </summary>
        protected readonly bool _isStaticType;
        /// <summary>
        /// 回调函数的扩展类型定义状态标识
        /// </summary>
        protected readonly bool _isExtensionType;
        /// <summary>
        /// 回调函数的无参类型状态标识
        /// </summary>
        protected readonly bool _isNullParameterType;

        public IBean TargetObject => _targetObject;
        public string Fullname => _fullname;
        public Type TargetType => _targetType;
        public MethodInfo MethodInfo => _methodInfo;
        public Delegate Callback => _callback;
        public bool Automatically => _automatically;
        public bool IsExtensionType => _isExtensionType;
        public bool IsNullParameterType => _isNullParameterType;

        /// <summary>
        /// 构建的目标函数为静态函数时，提供的构造函数
        /// </summary>
        protected BaseCallMethodInfo(string fullname, Type targetType, MethodInfo methodInfo, bool automatically)
            : this (null, fullname, targetType, methodInfo, automatically)
        { }

        /// <summary>
        /// 构建的目标函数为普通函数时，提供的构造函数
        /// </summary>
        protected BaseCallMethodInfo(IBean targetObject, string fullname, Type targetType, MethodInfo methodInfo, bool automatically)
        {
            Debugger.Assert(null != targetType || !automatically, NovaEngine.ErrorText.InvalidArguments);

            Debugger.Assert(false == CheckFunctionFormatWasInvalidType(methodInfo), NovaEngine.ErrorText.InvalidArguments);

            _targetObject = targetObject;
            _fullname = fullname;
            _targetType = targetType;
            _methodInfo = methodInfo;
            _automatically = automatically;

            // string fullname = _Generator.GenUniqueName(methodInfo);
            // _fullname = fullname;

            // 其实这里扩展标识已经没有太大的意义，因为普通成员函数也是在后期调用时才传入实例的
            // 暂时先保留吧，后期优化的时候再删除它
            // 2025-11-30：
            // 重新修改为普通函数需要传入具体的对象实例，因为在部分平台不支持普通函数构建委托时`this`为null
            _isStaticType = methodInfo.IsStatic;
            _isExtensionType = NovaEngine.Utility.Reflection.IsTypeOfExtension(methodInfo);
            _isNullParameterType = CheckFunctionFormatWasNullParameterType(methodInfo);

            Delegate callback = NovaEngine.Utility.Reflection.CreateGenericActionDelegate(targetObject, methodInfo);
            Debugger.Assert(callback, NovaEngine.ErrorText.InvalidArguments);
            _callback = callback;
        }

        /// <summary>
        /// 检测目标函数是否为无效的格式类型
        /// </summary>
        /// <param name="methodInfo">函数对象</param>
        /// <returns>若为无效格式类型则返回true，否则返回false</returns>
        protected virtual bool CheckFunctionFormatWasInvalidType(MethodInfo methodInfo)
        {
            return Loader.Inspecting.CodeInspector.CheckFunctionFormatOfTarget(methodInfo);
        }

        /// <summary>
        /// 检测目标函数是否符合当前调用类型的无参格式要求
        /// </summary>
        /// <param name="methodInfo">函数对象</param>
        /// <returns>若符合无参格式则返回true，否则返回false</returns>
        protected virtual bool CheckFunctionFormatWasNullParameterType(MethodInfo methodInfo)
        {
            return Loader.Inspecting.CodeInspector.CheckFunctionFormatOfTargetWithNullParameterType(methodInfo);
        }
    }
}
