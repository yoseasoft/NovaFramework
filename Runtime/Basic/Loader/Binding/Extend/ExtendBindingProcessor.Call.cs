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
using UnityEngine.Scripting;

namespace GameEngine.Loader
{
    /// 扩展定义的绑定回调管理服务类
    internal static partial class ExtendBindingProcessor
    {
        /// <summary>
        /// 事件分发类型的代码注册回调函数
        /// </summary>
        /// <param name="targetType">对象类型</param>
        /// <param name="codeInfo">对象结构信息数据</param>
        /// <param name="reload">重载标识</param>
        [Preserve]
        [OnExtendDefinitionRegisterClassOfTarget(typeof(ExtendSupportedAttribute))]
        private static void LoadCallBindCodeType(Type targetType, Structuring.GeneralCodeInfo codeInfo, bool reload)
        {
            if (null == codeInfo)
            {
                Debugger.Warn("The load code info '{%t}' must be non-null, recv arguments invalid.", targetType);
                return;
            }

            Structuring.ExtendCallCodeInfo extendCodeInfo = codeInfo as Structuring.ExtendCallCodeInfo;
            Debugger.Assert(extendCodeInfo, "Invalid extend call code info.");

            for (int n = 0; n < extendCodeInfo.GetInputCallMethodTypeCount(); ++n)
            {
                Structuring.InputResponsingMethodTypeCodeInfo callMethodInfo = extendCodeInfo.GetInputCallMethodType(n);

                Debugger.Info(LogGroupTag.CodeLoader, "Load extend input call {%t} with target class type {%t}.", callMethodInfo.Method, callMethodInfo.TargetType);

                Structuring.GeneralCodeInfo _lookupCodeInfo = CodeLoader.LookupGeneralCodeInfo(callMethodInfo.TargetType, typeof(IBean));
                if (_lookupCodeInfo is Structuring.BaseBeanCodeInfo baseCodeInfo)
                {
                    baseCodeInfo.AddInputResponsingMethodType(callMethodInfo);
                }
                else
                {
                    Debugger.Warn("Could not found any general code info with target type '{%t}', binded input call failed.", callMethodInfo.TargetType);
                }
            }

            for (int n = 0; n < extendCodeInfo.GetEventCallMethodTypeCount(); ++n)
            {
                Structuring.EventSubscribingMethodTypeCodeInfo callMethodInfo = extendCodeInfo.GetEventCallMethodType(n);

                Debugger.Info(LogGroupTag.CodeLoader, "Load extend event call {%t} with target class type {%t}.", callMethodInfo.Method, callMethodInfo.TargetType);

                Structuring.GeneralCodeInfo _lookupCodeInfo = CodeLoader.LookupGeneralCodeInfo(callMethodInfo.TargetType, typeof(IBean));
                if (_lookupCodeInfo is Structuring.BaseBeanCodeInfo baseCodeInfo)
                {
                    baseCodeInfo.AddEventSubscribingMethodType(callMethodInfo);
                }
                else
                {
                    Debugger.Warn("Could not found any general code info with target type '{%t}', binded event call failed.", callMethodInfo.TargetType);
                }
            }

            for (int n = 0; n < extendCodeInfo.GetMessageCallMethodTypeCount(); ++n)
            {
                Structuring.MessageBindingMethodTypeCodeInfo callMethodInfo = extendCodeInfo.GetMessageCallMethodType(n);

                Debugger.Info(LogGroupTag.CodeLoader, "Load extend message call {%t} with target class type {%t}.", callMethodInfo.Method, callMethodInfo.TargetType);

                Structuring.GeneralCodeInfo _lookupCodeInfo = CodeLoader.LookupGeneralCodeInfo(callMethodInfo.TargetType, typeof(IBean));
                if (_lookupCodeInfo is Structuring.BaseBeanCodeInfo baseCodeInfo)
                {
                    baseCodeInfo.AddMessageBindingMethodType(callMethodInfo);
                }
                else
                {
                    Debugger.Warn("Could not found any general code info with target type '{%t}', binded message call failed.", callMethodInfo.TargetType);
                }
            }
        }

        /// <summary>
        /// 事件分发类型的全部代码的注销回调函数
        /// </summary>
        [Preserve]
        [OnExtendDefinitionUnregisterClassOfTarget(typeof(ExtendSupportedAttribute))]
        private static void UnloadAllCallBindCodeTypes()
        {
        }
    }
}
