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

using SystemType = System.Type;
using SystemDelegate = System.Delegate;
using SystemMethodInfo = System.Reflection.MethodInfo;

namespace GameEngine
{
    /// <summary>
    /// 基础调用函数信息封装对象类
    /// </summary>
    internal abstract class BaseCallMethodInfo
    {
        /// <summary>
        /// 回调函数的目标对象类型
        /// </summary>
        protected readonly SystemType _targetType;
        /// <summary>
        /// 回调函数的完整名称
        /// </summary>
        protected readonly string _fullname;
        /// <summary>
        /// 回调函数的函数信息实例
        /// </summary>
        protected readonly SystemMethodInfo _methodInfo;
        /// <summary>
        /// 回调函数的动态构建回调句柄
        /// </summary>
        protected readonly SystemDelegate _callback;
        /// <summary>
        /// 回调函数的自动注册状态标识
        /// </summary>
        protected readonly bool _automatically;
        /// <summary>
        /// 回调函数的扩展定义状态标识
        /// </summary>
        protected readonly bool _isExtensionType;
        /// <summary>
        /// 回调函数的无参状态标识
        /// </summary>
        protected readonly bool _isNullParameterType;

        public SystemType TargetType => _targetType;
        public string Fullname => _fullname;
        public SystemMethodInfo MethodInfo => _methodInfo;
        public SystemDelegate Callback => _callback;
        public bool Automatically => _automatically;
        public bool IsExtensionType => _isExtensionType;
        public bool IsNullParameterType => _isNullParameterType;

        protected BaseCallMethodInfo(string fullname, SystemType targetType, SystemMethodInfo methodInfo, bool automatically)
        {
            Debugger.Assert(null != targetType || !automatically, NovaEngine.ErrorText.InvalidArguments);

            _targetType = targetType;
            _fullname = fullname;
            _methodInfo = methodInfo;
            _automatically = automatically;

            // 其实这里扩展标识已经没有太大的意义，因为普通成员函数也是在后期调用时才传入实例的
            // 暂时先保留吧，后期优化的时候再删除它
            _isExtensionType = NovaEngine.Utility.Reflection.IsTypeOfExtension(methodInfo);
            _isNullParameterType = CheckFunctionFormatWasNullParameterType(methodInfo);

            // string fullname = _Generator.GenUniqueName(methodInfo);
            // _fullname = fullname;

            SystemDelegate callback = NovaEngine.Utility.Reflection.CreateGenericActionDelegate(null, methodInfo);
            Debugger.Assert(callback, NovaEngine.ErrorText.InvalidArguments);
            _callback = callback;
        }

        /// <summary>
        /// 检测目标函数是否符合当前调用类型的无参格式要求
        /// </summary>
        /// <param name="methodInfo">函数对象</param>
        /// <returns>若符合无参格式则返回true，否则返回false</returns>
        protected virtual bool CheckFunctionFormatWasNullParameterType(SystemMethodInfo methodInfo)
        {
            return Loader.Inspecting.CodeInspector.CheckFunctionFormatOfTargetWithNullParameterType(methodInfo);
        }
    }
}
