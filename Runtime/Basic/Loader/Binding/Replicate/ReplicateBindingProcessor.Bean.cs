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
        /// 实体对象数据同步管理类型的代码注册回调函数
        /// </summary>
        /// <param name="targetType">对象类型</param>
        /// <param name="codeInfo">对象结构信息数据</param>
        /// <param name="reload">重载标识</param>
        [Preserve]
        [OnReplicateBeanRegisterClassOfTarget(typeof(ReplicateConfigureAttribute))]
        private static void LoadBeanBindCodeType(Type targetType, Loader.Structuring.GeneralCodeInfo codeInfo, bool reload)
        {
            if (null == codeInfo)
            {
                Debugger.Warn("The load code info '{%t}' must be non-null, recv arguments invalid.", targetType);
                return;
            }

            Loader.Structuring.ReplicateBeanCodeInfo replicateCodeInfo = codeInfo as Loader.Structuring.ReplicateBeanCodeInfo;
            Debugger.IsNotNull(replicateCodeInfo);

            if (reload)
            {
                Instance.UnregisterReplicateObject(replicateCodeInfo.ClassType);
            }

            if (false == string.IsNullOrEmpty(replicateCodeInfo.DataLabel))
            {
                Instance.RegisterReplicateObject(replicateCodeInfo.ClassType, replicateCodeInfo.DataLabel);
            }

            if (replicateCodeInfo.GetMemberCount() > 0)
            {
                for (int i = 0; i < replicateCodeInfo.GetMemberCount(); i++)
                {
                    Loader.Structuring.ReplicateBeanMemberCodeInfo memberCodeInfo = replicateCodeInfo.GetMember(i);
                    // Debugger.Assert(!memberCodeInfo.IsProperty, "Only supported field member!");

                    Instance.RegisterReplicateMember(replicateCodeInfo.ClassType, memberCodeInfo.MemberName, memberCodeInfo.DataLabel);
                }
            }
        }

        /// <summary>
        /// 实体对象数据同步管理类型的全部代码的注销回调函数
        /// </summary>
        [Preserve]
        [OnReplicateBeanUnregisterClassOfTarget(typeof(ReplicateConfigureAttribute))]
        private static void UnloadAllBeanBindCodeTypes()
        {
            Instance.UnregisterAllReplicateInfos();
        }
    }
}
