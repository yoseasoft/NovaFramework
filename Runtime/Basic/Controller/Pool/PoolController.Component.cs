/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyring (C) 2023 - 2024, Guangzhou Shiyue Network Technology Co., Ltd.
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
    /// 对象池管理类，用于对场景上下文中使用的对象池提供通用的访问操作接口
    /// </summary>
    public sealed partial class PoolController
    {
        /// <summary>
        /// 组件对象处理回调管理模块的初始化函数
        /// </summary>
        [OnControllerSubmoduleInitCallback]
        private void InitializeForComponentProcess()
        {
            NovaEngine.ReferencePool.AddReferencePostProcess(typeof(CComponent), OnPostProcessAfterComponentCreate, OnPostProcessBeforeComponentRelease);

            AddPoolObjectProcessInfo<CComponent>(CreateComponentInstance, ReleaseComponentInstance);
        }

        /// <summary>
        /// 组件对象处理回调管理模块的清理函数
        /// </summary>
        [OnControllerSubmoduleCleanupCallback]
        private void CleanupForComponentProcess()
        {
            RemovePoolObjectProcessInfo<IProto>();

            NovaEngine.ReferencePool.RemoveReferencePostProcess(typeof(IProto));
        }

        #region 组件对象的创建/释放接口函数

        /// <summary>
        /// 通过指定的对象类型创建一个实例
        /// </summary>
        /// <param name="classType">对象类型</param>
        /// <returns>返回对象实例，若创建失败则返回null</returns>
        private CComponent CreateComponentInstance(SystemType classType)
        {
            NovaEngine.IReference reference = NovaEngine.ReferencePool.Acquire(classType);

            //CComponent component = reference as CComponent;
            //component.Entity = entity;

            //component.Call(component.Initialize);

            return reference as CComponent;
        }

        /// <summary>
        /// 释放指定的对象实例
        /// </summary>
        /// <param name="component">对象实例</param>
        private void ReleaseComponentInstance(CComponent component)
        {
            //component.Call(component.Cleanup);

            //component.Entity = null;

            NovaEngine.ReferencePool.Release(component);
        }

        /// <summary>
        /// 创建组件对象的后处理接口函数
        /// </summary>
        /// <param name="reference">对象实例</param>
        private void OnPostProcessAfterComponentCreate(NovaEngine.IReference reference)
        {
            CComponent component = reference as CComponent;
            Debugger.Assert(null != component, "Invalid arguments");

            Debugger.Log(LogGroupTag.Controller, "Acquire component class '{0}' from the pool.", NovaEngine.Utility.Text.ToString(component.GetType()));
        }

        /// <summary>
        /// 释放组件对象的后处理接口函数
        /// </summary>
        /// <param name="reference">对象实例</param>
        private void OnPostProcessBeforeComponentRelease(NovaEngine.IReference reference)
        {
            CComponent component = reference as CComponent;
            Debugger.Assert(null != component, "Invalid arguments");

            Debugger.Log(LogGroupTag.Controller, "Release component class '{0}' to the pool.", NovaEngine.Utility.Text.ToString(component.GetType()));
        }

        #endregion
    }
}
