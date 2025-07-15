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

using System.Collections.Generic;

using SystemType = System.Type;
using SystemAttribute = System.Attribute;
using SystemDelegate = System.Delegate;

namespace GameEngine
{
    /// <summary>
    /// 事件管理对象类，用于对场景上下文中的所有节点对象进行事件管理及分发
    /// </summary>
    internal partial class EventController
    {
        /// <summary>
        /// 事件分发类型的代码注册回调函数
        /// </summary>
        /// <param name="targetType">对象类型</param>
        /// <param name="codeInfo">对象结构信息数据</param>
        /// <param name="reload">重载标识</param>
        [OnEventCallRegisterClassOfTarget(typeof(EventSystemAttribute))]
        private static void LoadCallBindCodeType(SystemType targetType, Loader.GeneralCodeInfo codeInfo, bool reload)
        {
            if (null == codeInfo)
            {
                Debugger.Warn("The load code info '{0}' must be non-null, recv arguments invalid.", targetType.FullName);
                return;
            }

            Loader.EventCallCodeInfo eventCodeInfo = codeInfo as Loader.EventCallCodeInfo;
            Debugger.Assert(null != eventCodeInfo, "Invalid event call code info.");

            for (int n = 0; n < eventCodeInfo.GetMethodTypeCount(); ++n)
            {
                Loader.EventCallMethodTypeCodeInfo callMethodInfo = eventCodeInfo.GetMethodType(n);

                SystemDelegate callback = NovaEngine.Utility.Reflection.CreateGenericActionDelegate(callMethodInfo.Method);
                if (null == callback)
                {
                    Debugger.Warn("Cannot converted from method info '{0}' to event standard call, loaded this method failed.", NovaEngine.Utility.Text.ToString(callMethodInfo.Method));
                    continue;
                }

                if (callMethodInfo.EventID > 0)
                {
                    if (reload)
                    {
                        Instance.RemoveEventDistributeCallInfo(callMethodInfo.Fullname, callMethodInfo.TargetType, callMethodInfo.EventID);
                    }

                    Instance.AddEventDistributeCallInfo(callMethodInfo.Fullname, callMethodInfo.TargetType, callMethodInfo.EventID, callback);
                }
                else
                {
                    if (reload)
                    {
                        Instance.RemoveEventDistributeCallInfo(callMethodInfo.Fullname, callMethodInfo.TargetType, callMethodInfo.EventDataType);
                    }

                    Instance.AddEventDistributeCallInfo(callMethodInfo.Fullname, callMethodInfo.TargetType, callMethodInfo.EventDataType, callback);
                }
            }
        }

        /// <summary>
        /// 事件分发类型的全部代码的注销回调函数
        /// </summary>
        [OnEventCallUnregisterClassOfTarget(typeof(EventSystemAttribute))]
        private static void UnloadAllCallBindCodeTypes()
        {
            Instance.RemoveAllEventDistributeCalls();
        }
    }
}
