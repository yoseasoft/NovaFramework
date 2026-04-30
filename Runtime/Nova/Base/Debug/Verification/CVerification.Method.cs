/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
/// Copyright (C) 2023 - 2024, Guangzhou Shiyue Network Technology Co., Ltd.
/// Copyright (C) 2026, Hurley, Independent Studio.
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
using System.Customize.Extension;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace NovaEngine
{
    /// 验证工具类
    public static partial class CVerification
    {
        /// <summary>
        /// 检测匹配的错误信息
        /// </summary>
        private const string CheckMatchedErrorText = @"unmatched";

        /// <summary>
        /// 检查目标委托回调函数是否匹配指定的参数类型，若不匹配以断言的方式退出
        /// </summary>
        /// <param name="handler">委托回调函数</param>
        /// <param name="parameterTypes">参数类型</param>
        /// <returns>若函数格式与给定参数类型匹配则返回true，否则返回false</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool CheckGenericDelegateParameterTypeMatched(Delegate handler, params Type[] parameterTypes)
        {
            return CheckGenericDelegateParameterTypeMatched(handler.Method, parameterTypes);
        }

        /// <summary>
        /// 检查目标委托回调函数是否匹配指定的参数类型，若不匹配以断言的方式退出
        /// </summary>
        /// <param name="methodInfo">函数信息</param>
        /// <param name="parameterTypes">参数类型</param>
        /// <returns>若函数格式与给定参数类型匹配则返回true，否则返回false</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool CheckGenericDelegateParameterTypeMatched(MethodInfo methodInfo, params Type[] parameterTypes)
        {
            bool ret = Utility.Reflection.IsGenericDelegateParameterTypeMatched(methodInfo, parameterTypes);
            CDebugger.Assert(false == _isDebuggingVerificationAssertModeEnabled || ret, CheckMatchedErrorText);
            return ret;
        }

        /// <summary>
        /// 检查目标委托回调函数是否匹配指定的参数类型，若不匹配以断言的方式退出
        /// </summary>
        /// <param name="methodInfo">函数信息</param>
        /// <param name="targetType">目标对象类型</param>
        /// <param name="parameterTypes">参数类型</param>
        /// <returns>若函数格式与给定参数类型匹配则返回true，否则返回false</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool CheckGenericDelegateParameterTypeMatchedOfTargetObject(MethodInfo methodInfo, Type targetType, params Type[] parameterTypes)
        {
            if (null == targetType)
            {
                return CheckGenericDelegateParameterTypeMatched(methodInfo, parameterTypes);
            }
            else
            {
                return CheckGenericDelegateParameterTypeMatched(methodInfo, parameterTypes.Prepend(targetType));
            }
        }

        /// <summary>
        /// 检查目标委托回调函数是否匹配指定的参数类型，若不匹配以断言的方式退出
        /// </summary>
        /// <param name="handler">委托回调函数</param>
        /// <param name="returnType">函数返回类型</param>
        /// <param name="parameterTypes">参数类型</param>
        /// <returns>若函数格式与给定参数类型匹配则返回true，否则返回false</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool CheckGenericDelegateParameterAndReturnTypeMatched(Delegate handler, Type returnType, params Type[] parameterTypes)
        {
            return CheckGenericDelegateParameterAndReturnTypeMatched(handler.Method, returnType, parameterTypes);
        }

        /// <summary>
        /// 检查目标委托回调函数是否匹配指定的参数类型，若不匹配以断言的方式退出
        /// </summary>
        /// <param name="methodInfo">函数信息</param>
        /// <param name="returnType">函数返回类型</param>
        /// <param name="parameterTypes">参数类型</param>
        /// <returns>若函数格式与给定参数类型匹配则返回true，否则返回false</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool CheckGenericDelegateParameterAndReturnTypeMatched(MethodInfo methodInfo, Type returnType, params Type[] parameterTypes)
        {
            bool ret = Utility.Reflection.IsGenericDelegateParameterAndReturnTypeMatched(methodInfo, returnType, parameterTypes);
            CDebugger.Assert(false == _isDebuggingVerificationAssertModeEnabled || ret, CheckMatchedErrorText);
            return ret;
        }

        /// <summary>
        /// 检查目标委托回调函数是否匹配指定的参数类型，若不匹配以断言的方式退出
        /// </summary>
        /// <param name="methodInfo">函数信息</param>
        /// <param name="targetType">目标对象类型</param>
        /// <param name="returnType">函数返回类型</param>
        /// <param name="parameterTypes">参数类型</param>
        /// <returns>若函数格式与给定参数类型匹配则返回true，否则返回false</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool CheckGenericDelegateParameterAndReturnTypeMatchedOfTargetObject(MethodInfo methodInfo, Type targetType, Type returnType, params Type[] parameterTypes)
        {
            if (null == targetType)
            {
                return CheckGenericDelegateParameterAndReturnTypeMatched(methodInfo, returnType, parameterTypes);
            }
            else
            {
                return CheckGenericDelegateParameterAndReturnTypeMatched(methodInfo, returnType, parameterTypes.Prepend(targetType));
            }
        }
    }
}
