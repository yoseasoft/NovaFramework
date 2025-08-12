/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
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

using SystemType = System.Type;

namespace GameEngine
{
    /// <summary>
    /// 对象池管理类，用于对场景上下文中使用的对象池提供通用的访问操作接口
    /// </summary>
    internal sealed partial class PoolController
    {
        /// <summary>
        /// 基础对象处理回调管理模块的初始化函数
        /// </summary>
        [OnControllerSubmoduleInitCallback]
        private void InitializeForObjectProcess()
        {
            NovaEngine.ReferencePool.AddReferencePostProcess(typeof(CObject), OnPostProcessAfterObjectCreate, OnPostProcessBeforeObjectRelease);

            AddPoolObjectProcessInfo<CObject>(CreateObjectInstance, ReleaseObjectInstance);
        }

        /// <summary>
        /// 基础对象处理回调管理模块的清理函数
        /// </summary>
        [OnControllerSubmoduleCleanupCallback]
        private void CleanupForObjectProcess()
        {
            RemovePoolObjectProcessInfo<CObject>();

            NovaEngine.ReferencePool.RemoveReferencePostProcess(typeof(CObject));
        }

        #region 基础对象的创建/释放接口函数

        /// <summary>
        /// 通过指定的对象类型创建一个实例
        /// </summary>
        /// <param name="classType">对象类型</param>
        /// <returns>返回对象实例，若创建失败则返回null</returns>
        private CObject CreateObjectInstance(SystemType classType)
        {
            NovaEngine.IReference reference = NovaEngine.ReferencePool.Acquire(classType);

            return reference as CObject;
        }

        /// <summary>
        /// 释放指定的对象实例
        /// </summary>
        /// <param name="obj">对象实例</param>
        private void ReleaseObjectInstance(CObject obj)
        {
            NovaEngine.ReferencePool.Release(obj);
        }

        /// <summary>
        /// 创建基础对象的后处理接口函数
        /// </summary>
        /// <param name="reference">对象实例</param>
        private void OnPostProcessAfterObjectCreate(NovaEngine.IReference reference)
        {
            CObject obj = reference as CObject;
            Debugger.Assert(null != obj, "Invalid arguments");

            Debugger.Log(LogGroupTag.Controller, "Acquire object class '{0}' from the pool.", NovaEngine.Utility.Text.ToString(obj.GetType()));
            // obj.Call(obj.Initialize);
        }

        /// <summary>
        /// 释放基础对象的后处理接口函数
        /// </summary>
        /// <param name="reference">对象实例</param>
        private void OnPostProcessBeforeObjectRelease(NovaEngine.IReference reference)
        {
            CObject obj = reference as CObject;
            Debugger.Assert(null != obj, "Invalid arguments");

            Debugger.Log(LogGroupTag.Controller, "Release object class '{0}' to the pool.", NovaEngine.Utility.Text.ToString(obj.GetType()));
            // obj.Call(obj.Cleanup);
        }

        #endregion
    }
}
