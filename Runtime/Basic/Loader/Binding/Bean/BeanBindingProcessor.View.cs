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
    /// 视图模块封装的句柄对象类
    public sealed partial class GuiHandler
    {
        /// <summary>
        /// 视图类型的代码注册回调函数
        /// </summary>
        /// <param name="targetType">对象类型</param>
        /// <param name="codeInfo">对象结构信息数据</param>
        /// <param name="reload">重载标识</param>
        [Preserve]
        [OnBeanRegisterClassOfTarget(typeof(CView))]
        private static void LoadCodeType(Type targetType, Loader.Structuring.GeneralCodeInfo codeInfo, bool reload)
        {
            if (targetType.IsInterface || targetType.IsAbstract)
            {
                Debugger.Log("The load code type '{%t}' cannot be interface or abstract class, recv arguments invalid.", targetType);
                return;
            }

            if (null == codeInfo)
            {
                Debugger.Warn("The load code info '{%t}' must be non-null, recv arguments invalid.", targetType);
                return;
            }

            Loader.Structuring.ViewCodeInfo viewCodeInfo = codeInfo as Loader.Structuring.ViewCodeInfo;
            Debugger.Assert(null != viewCodeInfo, "Invalid view code info.");

            if (reload)
            {
                // 重载模式下，无需重复注册视图信息
                return;
            }

            Instance.RegisterViewClass(viewCodeInfo.EntityName, viewCodeInfo.ClassType, viewCodeInfo.Priority);

            // 添加视图绑定分组信息
            if (false == string.IsNullOrEmpty(viewCodeInfo.GroupName))
            {
                Instance.AddViewBindingGroupName(viewCodeInfo.ClassType, viewCodeInfo.GroupName);
            }
        }

        /// <summary>
        /// 视图类型的全部代码的注销回调函数
        /// </summary>
        [Preserve]
        [OnBeanUnregisterClassOfTarget(typeof(CView))]
        private static void UnloadAllCodeTypes()
        {
            // 注销所有的视图类映射
            Instance.UnregisterAllViewClasses();
            // 移除所有视图分组对象
            Instance.RemoveAllViewGroups();
        }
    }
}
