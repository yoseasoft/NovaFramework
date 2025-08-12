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
    /// 对象池管理类，用于对场景上下文中使用的对象池提供通用的访问操作接口
    /// </summary>
    internal sealed partial class PoolController
    {
        /// <summary>
        /// 托管对象处理回调管理模块的初始化函数
        /// </summary>
        [OnControllerSubmoduleInitCallback]
        private void InitializeForGameObjectProcess()
        {
            AddPoolObjectProcessInfo<Pool.GameObject>(CreateGameObjectInstance, ReleaseGameObjectInstance);
        }

        /// <summary>
        /// 托管对象处理回调管理模块的清理函数
        /// </summary>
        [OnControllerSubmoduleCleanupCallback]
        private void CleanupForGameObjectProcess()
        {
            RemovePoolObjectProcessInfo<Pool.GameObject>();
        }

        #region 托管对象的创建/释放接口函数

        /// <summary>
        /// 通过指定的对象类型创建一个实例
        /// </summary>
        /// <param name="classType">对象类型</param>
        /// <returns>返回对象实例，若创建失败则返回null</returns>
        public Pool.GameObject CreateGameObjectInstance(SystemType classType)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// 释放指定的对象实例
        /// </summary>
        /// <param name="obj">对象实例</param>
        public void ReleaseGameObjectInstance(Pool.GameObject obj)
        {
            throw new System.NotImplementedException();
        }

        #endregion
    }
}
