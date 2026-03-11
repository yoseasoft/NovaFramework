/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyright (C) 2025 - 2026, Hainan Yuanyou Information Technology Co., Ltd. Guangzhou Branch
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
    /// 数据同步管理对象类
    internal partial class ReplicateController
    {
        /// <summary>
        /// 同步分发类型的代码注册回调函数
        /// </summary>
        /// <param name="targetType">对象类型</param>
        /// <param name="codeInfo">对象结构信息数据</param>
        /// <param name="reload">重载标识</param>
        [Preserve]
        [OnReplicateBeanRegisterClassOfTarget(typeof(ReplicateSystemAttribute))]
        private static void LoadCallBindCodeType(Type targetType, Loader.Structuring.GeneralCodeInfo codeInfo, bool reload)
        {
            if (null == codeInfo)
            {
                Debugger.Warn("The load code info '{%t}' must be non-null, recv arguments invalid.", targetType);
                return;
            }

            Loader.Structuring.ReplicateCallCodeInfo replicateCodeInfo = codeInfo as Loader.Structuring.ReplicateCallCodeInfo;
            Debugger.Assert(replicateCodeInfo, "Invalid replicate call code info.");

            for (int n = 0; n < replicateCodeInfo.GetMethodTypeCount(); ++n)
            {
                Loader.Structuring.ReplicateCallMethodTypeCodeInfo callMethodInfo = replicateCodeInfo.GetMethodType(n);

                // 2025-09-28：
                // 函数委托调整为在控制器内部创建，外部只进行校验和参数传递
                // SystemDelegate callback = NovaEngine.Utility.Reflection.CreateGenericActionDelegate(callMethodInfo.Method);
                // if (null == callback)
                // {
                //     Debugger.Warn("Cannot converted from method info '{%t}' to replicate listener call, loaded this method failed.", callMethodInfo.Method);
                //     continue;
                // }

                if (reload)
                {
                    Instance.RemoveReplicateDistributeCallInfo(callMethodInfo.Fullname, callMethodInfo.TargetType, callMethodInfo.Tags);
                }

                Instance.AddReplicateDistributeCallInfo(callMethodInfo.Fullname, callMethodInfo.TargetType, callMethodInfo.Method, callMethodInfo.Tags, callMethodInfo.AnnounceType);
            }
        }

        /// <summary>
        /// 同步分发类型的全部代码的注销回调函数
        /// </summary>
        [Preserve]
        [OnReplicateBeanUnregisterClassOfTarget(typeof(ReplicateSystemAttribute))]
        private static void UnloadAllCallBindCodeTypes()
        {
            Instance.RemoveAllReplicateDistributeCalls();
        }
    }
}
