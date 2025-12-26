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

namespace GameEngine
{
    /// 对象模块封装的句柄对象类
    public sealed partial class ObjectHandler
    {
        /// <summary>
        /// 对象类型的代码注册回调函数
        /// </summary>
        /// <param name="targetType">对象类型</param>
        /// <param name="codeInfo">对象结构信息数据</param>
        /// <param name="reload">重载标识</param>
        [OnBeanRegisterClassOfTarget(typeof(CObject))]
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

            Loader.Structuring.ObjectCodeInfo objectCodeInfo = codeInfo as Loader.Structuring.ObjectCodeInfo;
            Debugger.Assert(null != objectCodeInfo, "Invalid object code info.");

            if (reload)
            {
                // 重载模式下，无需重复注册对象信息
                return;
            }

            Instance.RegisterObjectClass(objectCodeInfo.ObjectName, objectCodeInfo.ClassType, objectCodeInfo.Priority);
        }

        /// <summary>
        /// 对象类型的全部代码的注销回调函数
        /// </summary>
        [OnBeanUnregisterClassOfTarget(typeof(CObject))]
        private static void UnloadAllCodeTypes()
        {
            Instance.UnregisterAllObjectClasses();
        }
    }
}
