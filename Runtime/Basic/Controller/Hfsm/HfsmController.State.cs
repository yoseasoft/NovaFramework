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

using UnityEngine.Scripting;

namespace GameEngine
{
    /// 状态管理对象类
    internal sealed partial class HfsmController
    {
        /// <summary>
        /// 原型对象状态回调相关内容的初始化回调函数
        /// </summary>
        [Preserve]
        [OnControllerSubmoduleInitCallback]
        private void InitializeForStateCall()
        {
        }

        /// <summary>
        /// 原型对象状态回调相关内容的清理回调函数
        /// </summary>
        [Preserve]
        [OnControllerSubmoduleCleanupCallback]
        private void CleanupForStateCall()
        {
        }

        /// <summary>
        /// 使用指定的根节点构建一个状态机对象实例
        /// </summary>
        /// <param name="root">根节点实例</param>
        /// <returns>返回一个新构建的状态机对象实例</returns>
        public HFSM.StateMachine Build(HFSM.State root)
        {
            return HFSM.StateMachineBuilder.Build(root);
        }
    }
}
