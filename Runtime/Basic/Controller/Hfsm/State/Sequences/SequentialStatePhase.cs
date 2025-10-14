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

using System.Collections.Generic;

using SystemTask = System.Threading.Tasks.Task;
using SystemCancellationToken = System.Threading.CancellationToken;

namespace GameEngine.HFSM
{
    /// <summary>
    /// 顺序状态序列对象类
    /// 将所有过渡期间要执行的不同步骤，按照传入的顺序依次执行
    /// </summary>
    public class SequentialStatePhase : IStateSequence
    {
        private readonly IList<StatePhaseStep> _steps;
        private readonly SystemCancellationToken _cancellationToken;
        private int _index = -1;
        private SystemTask _current;

        public bool IsDone { get; private set; }

        public SequentialStatePhase(IList<StatePhaseStep> steps, SystemCancellationToken cancellationToken)
        {
            _steps = steps;
            _cancellationToken = cancellationToken;
        }

        public void Start() => NextPhase();

        public bool Update()
        {
            // 步骤结束直接返回true
            if (IsDone) return true;

            if (null == _current || _current.IsCompleted) NextPhase();

            return IsDone;
        }

        void NextPhase()
        {
            ++_index;
            // 所有步骤都已经执行完毕
            if (_index >= _steps.Count)
            {
                IsDone = true;
                return;
            }

            _current = _steps[_index](_cancellationToken);
        }
    }
}
