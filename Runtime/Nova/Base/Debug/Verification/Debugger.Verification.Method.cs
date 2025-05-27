/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
/// Copyring (C) 2023 - 2024, Guangzhou Shiyue Network Technology Co., Ltd.
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

namespace NovaEngine
{
    /// <summary>
    /// 调试器对象工具类，用于引擎内部调试控制及输出相关接口声明
    /// </summary>
    public partial class Debugger
    {
        /// <summary>
        /// 验证工具类，对类型，函数等定义进行格式校验
        /// </summary>
        public static partial class Verification
        {
            /// <summary>
            /// 检查目标委托回调函数是否匹配指定的参数类型，若不匹配以断言的方式退出
            /// </summary>
            /// <param name="handler">委托回调函数</param>
            /// <param name="parameterTypes">参数类型</param>
            /// <returns>若函数格式与给定参数类型匹配则返回true，否则返回false</returns>
            public static bool CheckGenericDelegateParameterTypeMatched(SystemDelegate handler, params SystemType[] parameterTypes)
            {
                return CheckGenericDelegateParameterTypeMatched(true, handler, parameterTypes);
            }

            /// <summary>
            /// 检查目标委托回调函数是否匹配指定的参数类型，若不匹配以断言的方式退出
            /// </summary>
            /// <param name="condition">条件表达式</param>
            /// <param name="handler">委托回调函数</param>
            /// <param name="parameterTypes">参数类型</param>
            /// <returns>若函数格式与给定参数类型匹配则返回true，否则返回false</returns>
            public static bool CheckGenericDelegateParameterTypeMatched(bool condition, SystemDelegate handler, params SystemType[] parameterTypes)
            {
                if (condition)
                {
                    if (Instance.m_isDebuggingVerificationAssertModeEnabled)
                    {
                        Assert(Utility.Reflection.IsGenericDelegateParameterTypeMatched(handler, parameterTypes), "Invalid method format.");
                    }
                    else
                    {
                        return Utility.Reflection.IsGenericDelegateParameterTypeMatched(handler, parameterTypes);
                    }
                }

                return true;
            }

            /// <summary>
            /// 检查目标委托回调函数是否匹配指定的参数类型，若不匹配以断言的方式退出
            /// </summary>
            /// <param name="methodInfo">函数信息</param>
            /// <param name="parameterTypes">参数类型</param>
            /// <returns>若函数格式与给定参数类型匹配则返回true，否则返回false</returns>
            public static bool CheckGenericDelegateParameterTypeMatched(SystemMethodInfo methodInfo, params SystemType[] parameterTypes)
            {
                return CheckGenericDelegateParameterTypeMatched(true, methodInfo, parameterTypes);
            }

            /// <summary>
            /// 检查目标委托回调函数是否匹配指定的参数类型，若不匹配以断言的方式退出
            /// </summary>
            /// <param name="condition">条件表达式</param>
            /// <param name="methodInfo">函数信息</param>
            /// <param name="parameterTypes">参数类型</param>
            /// <returns>若函数格式与给定参数类型匹配则返回true，否则返回false</returns>
            public static bool CheckGenericDelegateParameterTypeMatched(bool condition, SystemMethodInfo methodInfo, params SystemType[] parameterTypes)
            {
                if (condition)
                {
                    if (Instance.m_isDebuggingVerificationAssertModeEnabled)
                    {
                        Assert(Utility.Reflection.IsGenericDelegateParameterTypeMatched(methodInfo, parameterTypes), "Invalid method format.");
                    }
                    else
                    {
                        return Utility.Reflection.IsGenericDelegateParameterTypeMatched(methodInfo, parameterTypes);
                    }
                }

                return true;
            }

            /// <summary>
            /// 检查目标委托回调函数是否匹配指定的参数类型，若不匹配以断言的方式退出
            /// </summary>
            /// <param name="handler">委托回调函数</param>
            /// <param name="returnType">函数返回类型</param>
            /// <param name="parameterTypes">参数类型</param>
            /// <returns>若函数格式与给定参数类型匹配则返回true，否则返回false</returns>
            public static bool CheckGenericDelegateParameterAndReturnTypeMatched(SystemDelegate handler, SystemType returnType, params SystemType[] parameterTypes)
            {
                return CheckGenericDelegateParameterAndReturnTypeMatched(true, handler, returnType, parameterTypes);
            }

            /// <summary>
            /// 检查目标委托回调函数是否匹配指定的参数类型，若不匹配以断言的方式退出
            /// </summary>
            /// <param name="condition">条件表达式</param>
            /// <param name="handler">委托回调函数</param>
            /// <param name="returnType">函数返回类型</param>
            /// <param name="parameterTypes">参数类型</param>
            /// <returns>若函数格式与给定参数类型匹配则返回true，否则返回false</returns>
            public static bool CheckGenericDelegateParameterAndReturnTypeMatched(bool condition, SystemDelegate handler, SystemType returnType, params SystemType[] parameterTypes)
            {
                if (condition)
                {
                    if (Instance.m_isDebuggingVerificationAssertModeEnabled)
                    {
                        Assert(Utility.Reflection.IsGenericDelegateParameterAndReturnTypeMatched(handler, returnType, parameterTypes), "Invalid method format.");
                    }
                    else
                    {
                        return Utility.Reflection.IsGenericDelegateParameterAndReturnTypeMatched(handler, returnType, parameterTypes);
                    }
                }

                return true;
            }

            /// <summary>
            /// 检查目标委托回调函数是否匹配指定的参数类型，若不匹配以断言的方式退出
            /// </summary>
            /// <param name="methodInfo">函数信息</param>
            /// <param name="returnType">函数返回类型</param>
            /// <param name="parameterTypes">参数类型</param>
            /// <returns>若函数格式与给定参数类型匹配则返回true，否则返回false</returns>
            public static bool CheckGenericDelegateParameterAndReturnTypeMatched(SystemMethodInfo methodInfo, SystemType returnType, params SystemType[] parameterTypes)
            {
                return CheckGenericDelegateParameterAndReturnTypeMatched(true, methodInfo, returnType, parameterTypes);
            }

            /// <summary>
            /// 检查目标委托回调函数是否匹配指定的参数类型，若不匹配以断言的方式退出
            /// </summary>
            /// <param name="condition">条件表达式</param>
            /// <param name="methodInfo">函数信息</param>
            /// <param name="returnType">函数返回类型</param>
            /// <param name="parameterTypes">参数类型</param>
            /// <returns>若函数格式与给定参数类型匹配则返回true，否则返回false</returns>
            public static bool CheckGenericDelegateParameterAndReturnTypeMatched(bool condition, SystemMethodInfo methodInfo, SystemType returnType, params SystemType[] parameterTypes)
            {
                if (condition)
                {
                    if (Instance.m_isDebuggingVerificationAssertModeEnabled)
                    {
                        Assert(Utility.Reflection.IsGenericDelegateParameterAndReturnTypeMatched(methodInfo, returnType, parameterTypes), "Invalid method format.");
                    }
                    else
                    {
                        return Utility.Reflection.IsGenericDelegateParameterAndReturnTypeMatched(methodInfo, returnType, parameterTypes);
                    }
                }

                return true;
            }
        }
    }
}
