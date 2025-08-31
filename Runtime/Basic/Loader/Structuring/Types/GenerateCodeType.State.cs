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

using SystemType = System.Type;

namespace GameEngine.Loader.Structuring
{
    /// <summary>
    /// 状态调用模块的函数结构信息
    /// </summary>
    internal class StateCallMethodTypeCodeInfo : MethodTypeCodeInfo
    {
        /// <summary>
        /// 状态调用模块的状态名称
        /// </summary>
        public string StateName { get; internal set; }
        /// <summary>
        /// 状态调用模块的访问类型
        /// </summary>
        public StateAccessType AccessType { get; internal set; }
    }

    /// <summary>
    /// 切面管理的状态转换函数结构信息
    /// </summary>
    internal sealed class StateTransitioningMethodTypeCodeInfo : StateCallMethodTypeCodeInfo
    {
        /// <summary>
        /// 状态转换的观察行为类型
        /// </summary>
        public AspectBehaviourType BehaviourType { get; internal set; }
    }
}
