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
using System.Threading;
using System.Threading.Tasks;

namespace GameEngine.HFSM
{
    /// <summary>
    /// 并行状态序列对象类
    /// 将所有过渡期间要执行的不同步骤，按照并行方式同时执行
    /// </summary>
    public class ParallelStatePhase : IStateSequence
    {
        private readonly IList<StatePhaseStep> _steps;
        private readonly CancellationToken _cancellationToken;
        private IList<Task> _tasks;

        public bool IsDone { get; private set; }

        public ParallelStatePhase(IList<StatePhaseStep> steps, CancellationToken cancellationToken)
        {
            _steps = steps;
            _cancellationToken = cancellationToken;
        }

        public void Start()
        {
            // 无事可做就直接结束
            if (null == _steps || _steps.Count == 0)
            {
                IsDone = true;
                return;
            }

            _tasks = new List<Task>(_steps.Count);
            for (int n = 0; n < _steps.Count; ++n)
            {
                _tasks.Add(_steps[n](_cancellationToken));
            }
        }

        public bool Update()
        {
            // 步骤结束直接返回true
            if (IsDone) return true;

            IsDone = null == _tasks || ((List<Task>) _tasks).TrueForAll(t => t.IsCompleted);

            return IsDone;
        }
    }
}
