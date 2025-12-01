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

using System.Reflection;
using System.Runtime.CompilerServices;

using SystemType = System.Type;

namespace GameEngine
{
    /// <summary>
    /// 输入调用函数信息封装对象类
    /// </summary>
    internal sealed class InputCallMethodInfo : BaseCallMethodInfo
    {
        /// <summary>
        /// 输入回调绑定的输入编码
        /// </summary>
        private readonly int _inputCode;
        /// <summary>
        /// 输入回调绑定的操作类型
        /// </summary>
        private readonly int _operationType;
        /// <summary>
        /// 输入回调绑定的输入数据类型
        /// </summary>
        private readonly SystemType _inputDataType;

        public int InputCode => _inputCode;
        public int OperationType => _operationType;
        public SystemType InputDataType => _inputDataType;

        #region 为静态回调函数构建委托句柄的构造函数

        public InputCallMethodInfo(string fullname, SystemType targetType, MethodInfo methodInfo, int inputCode, int operationType)
            : this(fullname, targetType, methodInfo, inputCode, operationType, false)
        { }

        public InputCallMethodInfo(string fullname, SystemType targetType, MethodInfo methodInfo, int inputCode, int operationType, bool automatically)
            : this(fullname, targetType, methodInfo, inputCode, operationType, null, automatically)
        { }

        public InputCallMethodInfo(string fullname, SystemType targetType, MethodInfo methodInfo, SystemType inputDataType)
            : this(fullname, targetType, methodInfo, inputDataType, false)
        { }

        public InputCallMethodInfo(string fullname, SystemType targetType, MethodInfo methodInfo, SystemType inputDataType, bool automatically)
            : this(fullname, targetType, methodInfo, 0, 0, inputDataType, automatically)
        { }

        private InputCallMethodInfo(string fullname, SystemType targetType, MethodInfo methodInfo, int inputCode, int operationType, SystemType inputDataType, bool automatically)
            : base(fullname, targetType, methodInfo, automatically)
        {
            _inputCode = inputCode;
            _operationType = operationType;
            _inputDataType = inputDataType;
        }

        #endregion

        #region 为普通回调函数构建委托句柄的构造函数

        public InputCallMethodInfo(IBean targetObject, string fullname, SystemType targetType, MethodInfo methodInfo, int inputCode, int operationType)
            : this(targetObject, fullname, targetType, methodInfo, inputCode, operationType, false)
        { }

        public InputCallMethodInfo(IBean targetObject, string fullname, SystemType targetType, MethodInfo methodInfo, int inputCode, int operationType, bool automatically)
            : this(targetObject, fullname, targetType, methodInfo, inputCode, operationType, null, automatically)
        { }

        public InputCallMethodInfo(IBean targetObject, string fullname, SystemType targetType, MethodInfo methodInfo, SystemType inputDataType)
            : this(targetObject, fullname, targetType, methodInfo, inputDataType, false)
        { }

        public InputCallMethodInfo(IBean targetObject, string fullname, SystemType targetType, MethodInfo methodInfo, SystemType inputDataType, bool automatically)
            : this(targetObject, fullname, targetType, methodInfo, 0, 0, inputDataType, automatically)
        { }

        private InputCallMethodInfo(IBean targetObject, string fullname, SystemType targetType, MethodInfo methodInfo, int inputCode, int operationType, SystemType inputDataType, bool automatically)
            : base(targetObject, fullname, targetType, methodInfo, automatically)
        {
            _inputCode = inputCode;
            _operationType = operationType;
            _inputDataType = inputDataType;
        }

        #endregion

        /// <summary>
        /// 检测目标函数是否为无效的格式类型
        /// </summary>
        /// <param name="methodInfo">函数对象</param>
        /// <returns>若为无效格式类型则返回true，否则返回false</returns>
        protected override sealed bool CheckFunctionFormatWasInvalidType(MethodInfo methodInfo)
        {
            // 函数格式校验
            if (NovaEngine.Debugger.Instance.IsOnDebuggingVerificationActivated())
            {
                bool verificated = false;
                if (NovaEngine.Utility.Reflection.IsTypeOfExtension(methodInfo))
                {
                    verificated = Loader.Inspecting.CodeInspector.CheckFunctionFormatOfInputCallWithBeanExtensionType(methodInfo);
                }
                else
                {
                    verificated = Loader.Inspecting.CodeInspector.CheckFunctionFormatOfInputCall(methodInfo);
                }

                // 校验失败
                if (false == verificated)
                {
                    Debugger.Error(LogGroupTag.Module, "目标对象类型‘{%t}’的‘{%s}’函数判定为非法格式的输入响应绑定回调函数，添加回调绑定操作失败！", _targetType, _fullname);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 检测目标函数是否符合当前调用类型的无参格式要求
        /// </summary>
        /// <param name="methodInfo">函数对象</param>
        /// <returns>若符合无参格式则返回true，否则返回false</returns>
        protected override sealed bool CheckFunctionFormatWasNullParameterType(MethodInfo methodInfo)
        {
            return Loader.Inspecting.CodeInspector.CheckFunctionFormatOfInputCallWithNullParameterType(methodInfo);
        }

        /// <summary>
        /// 输入回调的调度函数
        /// </summary>
        /// <param name="inputCode">输入编码</param>
        /// <param name="operationType">输入操作类型</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Invoke(int inputCode, int operationType)
        {
            if (/*_operationType == 0 ||*/ (_operationType & operationType) == 0)
            {
                // ignore
                return;
            }

            if (_isNullParameterType)
            {
                _callback.DynamicInvoke();
            }
            else
            {
                _callback.DynamicInvoke(inputCode, operationType);
            }
        }

        /// <summary>
        /// 输入回调的调度函数
        /// </summary>
        /// <param name="targetObject">对象实例</param>
        /// <param name="inputCode">输入编码</param>
        /// <param name="operationType">输入操作类型</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Invoke(IBean targetObject, int inputCode, int operationType)
        {
            Debugger.Assert(targetObject, NovaEngine.ErrorText.InvalidArguments);

            if (/*_operationType == 0 ||*/ (_operationType & operationType) == 0)
            {
                // ignore
                return;
            }

            if (_isNullParameterType)
            {
                _callback.DynamicInvoke(targetObject);
            }
            else
            {
                _callback.DynamicInvoke(targetObject, inputCode, operationType);
            }
        }

        /// <summary>
        /// 输入回调的调度函数
        /// </summary>
        /// <param name="inputData">输入数据</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Invoke(object inputData)
        {
            if (_isNullParameterType)
            {
                _callback.DynamicInvoke();
            }
            else
            {
                _callback.DynamicInvoke(inputData);
            }
        }

        /// <summary>
        /// 输入回调的调度函数
        /// </summary>
        /// <param name="targetObject">对象实例</param>
        /// <param name="inputData">输入数据</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Invoke(IBean targetObject, object inputData)
        {
            Debugger.Assert(targetObject, NovaEngine.ErrorText.InvalidArguments);

            if (_isNullParameterType)
            {
                _callback.DynamicInvoke(targetObject);
            }
            else
            {
                _callback.DynamicInvoke(targetObject, inputData);
            }
        }
    }
}
