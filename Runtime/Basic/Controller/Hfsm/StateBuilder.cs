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

namespace GameEngine
{
    /// <summary>
    /// 状态构建封装对象类，用于对实体对象内部的状态图构建提供统一的操作访问接口
    /// </summary>
    public class StateBuilder
    {
        /// <summary>
        /// 状态图对象实例
        /// </summary>
        private StateGraph _stateGraph = null;

        internal StateBuilder(StateGraph stateGraph)
        {
            _stateGraph = stateGraph;
        }

        ~StateBuilder()
        {
            _stateGraph = null;
        }

        /// <summary>
        /// 构建类的清理回调接口函数
        /// </summary>
        internal void Clear()
        {
        }

        /// <summary>
        /// 设置状态图的默认启动状态实例
        /// </summary>
        /// <param name="stateName">状态名称</param>
        /// <returns>返回构建对象实例</returns>
        public StateBuilder Launch(string stateName)
        {
            _stateGraph.SetDefaultLaunchState(stateName);

            return this;
        }

        /// <summary>
        /// 设置状态图的序列起始状态实例
        /// </summary>
        /// <param name="stateName">状态名称</param>
        /// <returns>返回构建对象实例</returns>
        public StateBuilder Start(string stateName)
        {
            _stateGraph.PushStateSequenceForStart(stateName);

            return this;
        }

        /// <summary>
        /// 设置状态图的序列转换状态实例
        /// </summary>
        /// <param name="stateName">状态名称</param>
        /// <returns>返回构建对象实例</returns>
        public StateBuilder Transition(string stateName)
        {
            _stateGraph.PushStateSequenceForTransition(stateName);

            return this;
        }
    }
}
