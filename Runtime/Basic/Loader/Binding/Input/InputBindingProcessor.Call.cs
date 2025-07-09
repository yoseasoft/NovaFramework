/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
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

using System.Collections.Generic;

using SystemType = System.Type;
using SystemAttribute = System.Attribute;
using SystemDelegate = System.Delegate;

namespace GameEngine
{
    /// <summary>
    /// 输入模块封装的句柄对象类
    /// </summary>
    public sealed partial class InputHandler
    {
        /// <summary>
        /// 输入响应类型的代码注册回调函数
        /// </summary>
        /// <param name="targetType">对象类型</param>
        /// <param name="codeInfo">对象结构信息数据</param>
        /// <param name="reload">重载标识</param>
        [OnInputCallRegisterClassOfTarget(typeof(InputSystemAttribute))]
        private static void LoadCallBindCodeType(SystemType targetType, Loader.GeneralCodeInfo codeInfo, bool reload)
        {
            if (null == codeInfo)
            {
                Debugger.Warn("The load code info '{0}' must be non-null, recv arguments invalid.", targetType.FullName);
                return;
            }

            Loader.InputCallCodeInfo inputCodeInfo = codeInfo as Loader.InputCallCodeInfo;
            Debugger.Assert(null != inputCodeInfo, "Invalid input call code info.");

            for (int n = 0; n < inputCodeInfo.GetMethodTypeCount(); ++n)
            {
                Loader.InputCallMethodTypeCodeInfo callMethodInfo = inputCodeInfo.GetMethodType(n);

                SystemDelegate callback = NovaEngine.Utility.Reflection.CreateGenericActionDelegate(callMethodInfo.Method);
                if (null == callback)
                {
                    Debugger.Warn("Cannot converted from method info '{0}' to input listener call, loaded this method failed.", NovaEngine.Utility.Text.ToString(callMethodInfo.Method));
                    continue;
                }

                if (callMethodInfo.Keycode > 0)
                {
                    if (reload)
                    {
                        Instance.RemoveInputDistributeCallInfo(callMethodInfo.Fullname, callMethodInfo.TargetType, callMethodInfo.Keycode, callMethodInfo.OperationType);
                    }

                    Instance.AddInputDistributeCallInfo(callMethodInfo.Fullname, callMethodInfo.TargetType, callMethodInfo.Keycode, callMethodInfo.OperationType, callback);
                }
                else
                {
                    if (reload)
                    {
                        Instance.RemoveInputDistributeCallInfo(callMethodInfo.Fullname, callMethodInfo.TargetType, callMethodInfo.InputDataType);
                    }

                    Instance.AddInputDistributeCallInfo(callMethodInfo.Fullname, callMethodInfo.TargetType, callMethodInfo.InputDataType, callback);
                }
            }
        }

        /// <summary>
        /// 输入响应类型的全部代码的注销回调函数
        /// </summary>
        [OnInputCallUnregisterClassOfTarget(typeof(InputSystemAttribute))]
        private static void UnloadAllCallBindCodeTypes()
        {
            Instance.RemoveAllInputDistributeCalls();
        }
    }
}
