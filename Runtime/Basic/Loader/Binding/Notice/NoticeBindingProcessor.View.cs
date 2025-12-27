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

namespace GameEngine.Loader
{
    /// 通知定义的绑定回调管理服务类
    internal static partial class NoticeBindingProcessor
    {
        /// <summary>
        /// 通知定义类型的代码注册回调函数
        /// </summary>
        /// <param name="targetType">对象类型</param>
        /// <param name="codeInfo">对象结构信息数据</param>
        /// <param name="reload">重载标识</param>
        [Preserve]
        [OnNoticeDefinitionRegisterClassOfTarget(typeof(NoticeSupportedOnViewAttribute))]
        private static void LoadCallBindCodeType(Type targetType, Structuring.GeneralCodeInfo codeInfo, bool reload)
        {
            if (null == codeInfo)
            {
                Debugger.Warn("The load code info '{%t}' must be non-null, recv arguments invalid.", targetType);
                return;
            }

            Structuring.CViewNoticeCallCodeInfo noticeCodeInfo = codeInfo as Structuring.CViewNoticeCallCodeInfo;
            Debugger.Assert(null != noticeCodeInfo, "Invalid notice call code info.");

            for (int n = 0; n < noticeCodeInfo.GetMethodTypeCount(); ++n)
            {
                Structuring.CViewNoticeMethodTypeCodeInfo callMethodInfo = noticeCodeInfo.GetMethodType(n);

                Debugger.Info(LogGroupTag.CodeLoader, "Load notice call {%t} with target class type {%t}.", callMethodInfo.Method, callMethodInfo.TargetType);

                Structuring.GeneralCodeInfo _lookupCodeInfo = CodeLoader.LookupGeneralCodeInfo(callMethodInfo.TargetType, typeof(IBean));
                if (_lookupCodeInfo is Structuring.ViewCodeInfo viewCodeInfo)
                {
                    viewCodeInfo.AddNoticeMethodType(callMethodInfo);
                }
                else
                {
                    Debugger.Warn("Could not found any general code info with target type '{%t}', binded notice call failed.", callMethodInfo.TargetType);
                }
            }
        }

        /// <summary>
        /// 通知定义类型的全部代码的注销回调函数
        /// </summary>
        [Preserve]
        [OnNoticeDefinitionUnregisterClassOfTarget(typeof(NoticeSupportedOnViewAttribute))]
        private static void UnloadAllCallBindCodeTypes()
        {
        }
    }
}
