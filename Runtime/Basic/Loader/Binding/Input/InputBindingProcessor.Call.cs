/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
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
using UnityEngine.Scripting;

namespace GameEngine
{
    /// 输入模块封装的句柄对象类
    public sealed partial class InputHandler
    {
        /// <summary>
        /// 输入响应类型的代码注册回调函数
        /// </summary>
        /// <param name="targetType">对象类型</param>
        /// <param name="codeInfo">对象结构信息数据</param>
        /// <param name="reload">重载标识</param>
        [Preserve]
        [OnInputCallRegisterClassOfTarget(typeof(InputSystemAttribute))]
        private static void LoadCallBindCodeType(Type targetType, Loader.Structuring.GeneralCodeInfo codeInfo, bool reload)
        {
            if (null == codeInfo)
            {
                Debugger.Warn("The load code info '{%t}' must be non-null, recv arguments invalid.", targetType);
                return;
            }

            Loader.Structuring.InputCallCodeInfo inputCodeInfo = codeInfo as Loader.Structuring.InputCallCodeInfo;
            Debugger.Assert(inputCodeInfo, "Invalid input call code info.");

            for (int n = 0; n < inputCodeInfo.GetMethodTypeCount(); ++n)
            {
                Loader.Structuring.InputCallMethodTypeCodeInfo callMethodInfo = inputCodeInfo.GetMethodType(n);

                // 2025-09-28：
                // 函数委托调整为在控制器内部创建，外部只进行校验和参数传递
                // SystemDelegate callback = NovaEngine.Utility.Reflection.CreateGenericActionDelegate(callMethodInfo.Method);
                // if (null == callback)
                // {
                //     Debugger.Warn("Cannot converted from method info '{%t}' to input listener call, loaded this method failed.", callMethodInfo.Method);
                //     continue;
                // }

                if (callMethodInfo.InputCode > 0)
                {
                    if (reload)
                    {
                        Instance.RemoveInputDistributeCallInfo(callMethodInfo.Fullname, callMethodInfo.TargetType, callMethodInfo.InputCode, (int) callMethodInfo.OperationType);
                    }

                    Instance.AddInputDistributeCallInfo(callMethodInfo.Fullname, callMethodInfo.TargetType, callMethodInfo.Method, callMethodInfo.InputCode, (int) callMethodInfo.OperationType);
                }
                else
                {
                    if (reload)
                    {
                        Instance.RemoveInputDistributeCallInfo(callMethodInfo.Fullname, callMethodInfo.TargetType, callMethodInfo.InputDataType);
                    }

                    Instance.AddInputDistributeCallInfo(callMethodInfo.Fullname, callMethodInfo.TargetType, callMethodInfo.Method, callMethodInfo.InputDataType);
                }
            }
        }

        /// <summary>
        /// 输入响应类型的全部代码的注销回调函数
        /// </summary>
        [Preserve]
        [OnInputCallUnregisterClassOfTarget(typeof(InputSystemAttribute))]
        private static void UnloadAllCallBindCodeTypes()
        {
            Instance.RemoveAllInputDistributeCalls();

            Instance.RemoveAllInputResponseBindingCalls();
        }
    }
}
