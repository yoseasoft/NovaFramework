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

using System.Collections.Generic;
using System.Reflection;

using SystemType = System.Type;
using SystemDelegate = System.Delegate;
using SystemAttribute = System.Attribute;
using SystemAttributeTargets = System.AttributeTargets;
using SystemMethodInfo = System.Reflection.MethodInfo;
using SystemBindingFlags = System.Reflection.BindingFlags;

namespace GameEngine.Loader
{
    /// <summary>
    /// 扩展定义的绑定回调管理服务类，对扩展模块的回调接口绑定/注销操作进行统一定义管理
    /// </summary>
    internal static partial class ExtendBindingProcessor
    {
        /// <summary>
        /// 事件分发类型的代码注册回调函数
        /// </summary>
        /// <param name="targetType">对象类型</param>
        /// <param name="codeInfo">对象结构信息数据</param>
        /// <param name="reload">重载标识</param>
        [OnExtendDefinitionRegisterClassOfTarget(typeof(ExtendSupportedAttribute))]
        private static void LoadCallBindCodeType(SystemType targetType, GeneralCodeInfo codeInfo, bool reload)
        {
            if (null == codeInfo)
            {
                Debugger.Warn("The load code info '{0}' must be non-null, recv arguments invalid.", targetType.FullName);
                return;
            }

            ExtendCallCodeInfo extendCodeInfo = codeInfo as ExtendCallCodeInfo;
            Debugger.Assert(null != extendCodeInfo, "Invalid extend call code info.");

            for (int n = 0; n < extendCodeInfo.GetInputCallMethodTypeCount(); ++n)
            {
                InputResponsingMethodTypeCodeInfo callMethodInfo = extendCodeInfo.GetInputCallMethodType(n);

                Debugger.Info(LogGroupTag.CodeLoader, "Load extend input call {0} with target class type {1}.", callMethodInfo.Method.Name, callMethodInfo.TargetType.FullName);

                GeneralCodeInfo _lookupCodeInfo = CodeLoader.LookupGeneralCodeInfo(callMethodInfo.TargetType, typeof(IProto));
                if (_lookupCodeInfo is ProtoBaseCodeInfo baseCodeInfo)
                {
                    baseCodeInfo.AddInputResponsingMethodType(callMethodInfo);
                }
                else
                {
                    Debugger.Warn("Could not found any general code info with target type '{0}', binded input call failed.", callMethodInfo.TargetType.FullName);
                }
            }

            for (int n = 0; n < extendCodeInfo.GetEventCallMethodTypeCount(); ++n)
            {
                EventSubscribingMethodTypeCodeInfo callMethodInfo = extendCodeInfo.GetEventCallMethodType(n);

                Debugger.Info(LogGroupTag.CodeLoader, "Load extend event call {0} with target class type {1}.", callMethodInfo.Method.Name, callMethodInfo.TargetType.FullName);

                GeneralCodeInfo _lookupCodeInfo = CodeLoader.LookupGeneralCodeInfo(callMethodInfo.TargetType, typeof(IProto));
                if (_lookupCodeInfo is ProtoBaseCodeInfo baseCodeInfo)
                {
                    baseCodeInfo.AddEventSubscribingMethodType(callMethodInfo);
                }
                else
                {
                    Debugger.Warn("Could not found any general code info with target type '{0}', binded event call failed.", callMethodInfo.TargetType.FullName);
                }
            }

            for (int n = 0; n < extendCodeInfo.GetMessageCallMethodTypeCount(); ++n)
            {
                MessageBindingMethodTypeCodeInfo callMethodInfo = extendCodeInfo.GetMessageCallMethodType(n);

                Debugger.Info(LogGroupTag.CodeLoader, "Load extend message call {0} with target class type {1}.", callMethodInfo.Method.Name, callMethodInfo.TargetType.FullName);

                GeneralCodeInfo _lookupCodeInfo = CodeLoader.LookupGeneralCodeInfo(callMethodInfo.TargetType, typeof(IProto));
                if (_lookupCodeInfo is ProtoBaseCodeInfo baseCodeInfo)
                {
                    baseCodeInfo.AddMessageBindingMethodType(callMethodInfo);
                }
                else
                {
                    Debugger.Warn("Could not found any general code info with target type '{0}', binded message call failed.", callMethodInfo.TargetType.FullName);
                }
            }
        }

        /// <summary>
        /// 事件分发类型的全部代码的注销回调函数
        /// </summary>
        [OnExtendDefinitionUnregisterClassOfTarget(typeof(ExtendSupportedAttribute))]
        private static void UnloadAllCallBindCodeTypes()
        {
        }
    }
}
