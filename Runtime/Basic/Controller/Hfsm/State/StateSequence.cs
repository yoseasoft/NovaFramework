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

using SystemTask = System.Threading.Tasks.Task;
using SystemCancellationToken = System.Threading.CancellationToken;

namespace GameEngine.HFSM
{
    /// <summary>
    /// 状态序列接口类，用于声明状态序列的访问接口
    /// </summary>
    public interface IStateSequence
    {
        /// <summary>
        /// 序列执行完成的状态标识
        /// </summary>
        bool IsDone { get; }

        /// <summary>
        /// 状态序列的启动接口函数
        /// </summary>
        void Start();

        /// <summary>
        /// 状态序列的更新接口函数，在序列启动后逐帧的调用序列刷新逻辑
        /// </summary>
        /// <returns>序列完成后返回true，否则返回false</returns>
        bool Update();
    }

    // One activity operation (activate OR deactivate) to run for this phase.
    public delegate SystemTask StatePhaseStep(SystemCancellationToken cancellationToken);

    /// <summary>
    /// 空白状态序列对象类
    /// </summary>
    public class NoopStatePhase : IStateSequence
    {
        public bool IsDone { get; private set; }

        public void Start() => IsDone = true; // completes immediately

        public bool Update() => IsDone;
    }
}
