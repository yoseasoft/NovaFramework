/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyright (C) 2023 - 2024, Guangzhou Shiyue Network Technology Co., Ltd.
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
using SystemMethodInfo = System.Reflection.MethodInfo;

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

        public InputCallMethodInfo(string fullname, SystemType targetType, SystemMethodInfo methodInfo, int inputCode, int operationType)
            : this(fullname, targetType, methodInfo, inputCode, operationType, false)
        { }

        public InputCallMethodInfo(string fullname, SystemType targetType, SystemMethodInfo methodInfo, int inputCode, int operationType, bool automatically)
            : this(fullname, targetType, methodInfo, inputCode, operationType, null, automatically)
        { }

        public InputCallMethodInfo(string fullname, SystemType targetType, SystemMethodInfo methodInfo, SystemType inputDataType)
            : this(fullname, targetType, methodInfo, inputDataType, false)
        { }

        public InputCallMethodInfo(string fullname, SystemType targetType, SystemMethodInfo methodInfo, SystemType inputDataType, bool automatically)
            : this(fullname, targetType, methodInfo, 0, 0, inputDataType, automatically)
        { }

        private InputCallMethodInfo(string fullname, SystemType targetType, SystemMethodInfo methodInfo, int inputCode, int operationType, SystemType inputDataType, bool automatically)
            : base(fullname, targetType, methodInfo, automatically)
        {
            _inputCode = inputCode;
            _operationType = operationType;
            _inputDataType = inputDataType;
        }

        /// <summary>
        /// 检测目标函数是否符合当前调用类型的无参格式要求
        /// </summary>
        /// <param name="methodInfo">函数对象</param>
        /// <returns>若符合无参格式则返回true，否则返回false</returns>
        protected override sealed bool CheckFunctionFormatWasNullParameterType(SystemMethodInfo methodInfo)
        {
            return Loader.Inspecting.CodeInspector.CheckFunctionFormatOfInputCallWithNullParameterType(methodInfo);
        }

        /// <summary>
        /// 输入回调的调度函数
        /// </summary>
        /// <param name="inputCode">输入编码</param>
        /// <param name="operationType">输入操作类型</param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
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
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public void Invoke(IBean targetObject, int inputCode, int operationType)
        {
            Debugger.Assert(targetObject, "Invalid arguments.");

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
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
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
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public void Invoke(IBean targetObject, object inputData)
        {
            Debugger.Assert(targetObject, "Invalid arguments.");

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
