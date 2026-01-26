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
using UnityEngine.Scripting;

namespace GameEngine
{
    /// 切面注入访问接口的控制器类
    internal partial class AspectController
    {
        /// <summary>
        /// 切面控制类型的代码注册回调函数
        /// </summary>
        /// <param name="targetType">对象类型</param>
        /// <param name="codeInfo">对象结构信息数据</param>
        /// <param name="reload">重载标识</param>
        [Preserve]
        [OnAspectCallRegisterClassOfTarget(typeof(AspectAttribute))]
        private static void LoadCallBindCodeType(Type targetType, Loader.Structuring.GeneralCodeInfo codeInfo, bool reload)
        {
            if (null == codeInfo)
            {
                Debugger.Warn("The load code info '{%t}' must be non-null, recv arguments invalid.", targetType);
                return;
            }

            Loader.Structuring.AspectCallCodeInfo aspectCodeInfo = codeInfo as Loader.Structuring.AspectCallCodeInfo;
            Debugger.Assert(aspectCodeInfo, "Invalid aspect call code info.");

            for (int n = 0; n < aspectCodeInfo.GetMethodTypeCount(); ++n)
            {
                Loader.Structuring.AspectCallMethodTypeCodeInfo callMethodInfo = aspectCodeInfo.GetMethodType(n);

                if (reload)
                {
                    Instance.RemoveAspectCallForGenericType(callMethodInfo.Fullname, callMethodInfo.TargetType, callMethodInfo.MethodName, callMethodInfo.AccessType);
                }

                Instance.AddAspectCallForGenericType(callMethodInfo.Fullname,
                                                     callMethodInfo.TargetType,
                                                     callMethodInfo.MethodName,
                                                     callMethodInfo.AccessType,
                                                     callMethodInfo.Callback);
            }
        }

        /// <summary>
        /// 切面控制类型的全部代码的注销回调函数
        /// </summary>
        [Preserve]
        [OnAspectCallUnregisterClassOfTarget(typeof(AspectAttribute))]
        private static void UnloadAllCallBindCodeTypes()
        {
            Instance.RemoveAllAspectCalls();
        }
    }
}
