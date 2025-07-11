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

namespace GameEngine
{
    /// <summary>
    /// 基于ECS模式定义的句柄对象类，针对实体类型访问操作的接口进行封装
    /// 该句柄对象提供实体相关的操作访问接口
    /// </summary>
    public abstract partial class EcsmHandler
    {
        /// <summary>
        /// 组件类型的代码注册回调函数
        /// </summary>
        /// <param name="targetType">对象类型</param>
        /// <param name="codeInfo">对象结构信息数据</param>
        /// <param name="reload">重载标识</param>
        [OnProtoRegisterClassOfTarget(typeof(CComponent))]
        private static void LoadComponentCodeType(SystemType targetType, Loader.GeneralCodeInfo codeInfo, bool reload)
        {
            if (targetType.IsInterface || targetType.IsAbstract)
            {
                Debugger.Log("The load code type '{0}' cannot be interface or abstract class, recv arguments invalid.", targetType.FullName);
                return;
            }

            if (null == codeInfo)
            {
                Debugger.Warn("The load code info '{0}' must be non-null, recv arguments invalid.", targetType.FullName);
                return;
            }

            Loader.ComponentCodeInfo componentCodeInfo = codeInfo as Loader.ComponentCodeInfo;
            Debugger.Assert(null != componentCodeInfo, "Invalid component code info.");

            if (reload)
            {
                // ProtoController.Instance.UnregisterComponentClass(componentCodeInfo.ComponentName);
                // 重载模式下，无需重复注册视图信息
                return;
            }

            ProtoController.Instance.RegisterComponentClass(componentCodeInfo.ComponentName, componentCodeInfo.ClassType);
        }

        /// <summary>
        /// 组件类型的全部代码的注销回调函数
        /// </summary>
        [OnProtoUnregisterClassOfTarget(typeof(CComponent))]
        private static void UnloadAllComponentCodeTypes()
        {
            ProtoController.Instance.UnregisterAllComponentClasses();
        }
    }
}
