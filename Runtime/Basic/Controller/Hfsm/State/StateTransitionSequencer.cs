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

using SystemAction = System.Action;

namespace GameEngine.HFSM
{
    /// <summary>
    /// 状态转换序列对象类，用于管理状态转换序列调度相关逻辑
    /// </summary>
    internal class StateTransitionSequencer
    {
        /// <summary>
        /// 转换序列所属的状态机对象实例
        /// </summary>
        private StateMachine _machine;

        private IStateSequence _sequencer; // current phase (deactivate or activate)
        private SystemAction _nextPhase; // switch structure between phases
        private (State from, State to)? _pending; // coalesce a single pending request
        /// <summary>
        /// 用于跟踪上一次过渡请求的临时引用
        /// </summary>
        private State _lastFrom, _lastTo;

        private CancellationTokenSource _cancellationTokenSource;

        private readonly bool _useSequential = true; // set false to use parallel

        internal StateTransitionSequencer(StateMachine machine)
        {
            _machine = machine;
        }

        ~StateTransitionSequencer()
        {
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = null;
        }

        // Request a transition from one state to another
        public void RequestTransition(State from, State to)
        {
            // _machine.ChangeState(from, to);

            // 忽略无效过渡
            if (null == to || from == to) return;

            // 若当前已经处于过渡中，则将此请求存为待处理
            // 由于待处理请求会被覆盖，所以只有最后请求的那个会被执行
            if (null != _sequencer)
            {
                _pending = (from, to);
                return;
            }

            // 调用开始过渡方法来启动这个过渡流程
            BeginTransition(from, to);
        }

        void BeginTransition(State from, State to)
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource = new CancellationTokenSource();

            State lca = Lca(from, to);
            IList<State> exitChain = StatesToExit(from, lca);
            IList<State> enterChain = StatesToEnter(to, lca);

            // 为了妥善处理一次过渡行为，我们会将其拆分为三个不同的步骤：
            // 第一步是停用操作，目前我们会用无操作阶段来暂代这个步骤；
            // 在停用完成后调用过渡的下一步，通过告知状态机来改变状态；
            // 接着是第三步，我们会激活目标分支，因为当前分支只是个占位符；
            // 之后我们会在此按需触发相应的活动或延迟行为

            // 1. Deactivate the "old branch"
            IList<StatePhaseStep> exitSteps = GatherStatePhaseSteps(exitChain, deactivate: true);
            // 补充了顺序和并行步骤处理逻辑后，这里不需要再用空阶段代替了
            // _sequencer = new NoopStatePhase();
            // 选择使用顺序还是并行的方式执行所有活动步骤
            _sequencer = _useSequential
                ? new SequentialStatePhase(exitSteps, _cancellationTokenSource.Token)
                : new ParallelStatePhase(exitSteps, _cancellationTokenSource.Token);
            _sequencer.Start();

            _nextPhase = () =>
            {
                // 2. ChangeState
                _machine.ChangeState(from, to);
                // 3. Activate the "new branch"
                IList<StatePhaseStep> enterSteps = GatherStatePhaseSteps(enterChain, deactivate: false);
                // 这里也同样不需要空阶段步骤了
                // _sequencer = new NoopStatePhase();
                _sequencer = _useSequential
                    ? new SequentialStatePhase(enterSteps, _cancellationTokenSource.Token)
                    : new ParallelStatePhase(enterSteps, _cancellationTokenSource.Token);
                _sequencer.Start();
            };
        }

        void EndTransition()
        {
            _sequencer = null;

            // 检测是否有新的过渡请求
            if (_pending.HasValue)
            {
                (State from, State to) p = _pending.Value;
                _pending = null;
                BeginTransition(p.from, p.to);
            }
        }

        public void Tick()
        {
            if (null != _sequencer)
            {
                if (_sequencer.Update())
                {
                    // 当前序列已完成，则开始下一个序列
                    if (null != _nextPhase)
                    {
                        // 激活新的分支
                        SystemAction f = _nextPhase;
                        _nextPhase = null;
                        f();
                    }
                    else
                    {
                        EndTransition();
                    }
                }

                return; // while transitioning, we don't run normal updates
            }

            // 当前没有序列工作时，才进行状态机更新

            _machine.InternalTick();
        }

        /// <summary>
        /// 收集状态链表上的所有激活或停用步骤
        /// </summary>
        /// <param name="chain">状态链表</param>
        /// <param name="deactivate">激活标识</param>
        /// <returns>返回对应的步骤列表</returns>
        static IList<StatePhaseStep> GatherStatePhaseSteps(IList<State> chain, bool deactivate)
        {
            IList<StatePhaseStep> steps = new List<StatePhaseStep>();

            for (int m = 0; m < chain.Count; ++m)
            {
                IReadOnlyList<IStateActivity> activities = chain[m].Activities;
                for (int n = 0; n < activities.Count; ++n)
                {
                    IStateActivity activity = activities[n];

                    if (deactivate) // 停用状态
                    {
                        if (StateActivityMode.Active == activity.ActivityMode)
                        {
                            steps.Add(cancellationToken => activity.DeactivateAsync(cancellationToken));
                        }
                    }
                    else // 激活状态
                    {
                        if (StateActivityMode.Inactive == activity.ActivityMode)
                        {
                            steps.Add(cancellationToken => activity.ActivateAsync(cancellationToken));
                        }
                    }
                }
            }

            return steps;
        }

        // States to exit: from -> ... up to (but excluding) lca; bottom->up order.
        static IList<State> StatesToExit(State from, State lca)
        {
            IList<State> list = new List<State>();

            for (State s = from; null != s && s != lca; s = s.Parent)
                list.Add(s);

            return list;
        }

        // States to enter: path from 'to' up to (but excluding) lca; returned in enter order (top->down).
        static IList<State> StatesToEnter(State to, State lca)
        {
            Stack<State> stack = new Stack<State>();

            for (State s = to; s != lca; s = s.Parent)
                stack.Push(s);

            return new List<State>(stack);
        }

        // Compute the Lowest Common Ancestor of two states.
        internal static State Lca(State arg0, State arg1)
        {
            // Create a set of all parents of 'arg0'
            HashSet<State> parents = new HashSet<State>();

            for (State s = arg0; null != s; s = s.Parent)
                parents.Add(s);

            // Find the first parent of 'arg1' that is also a parent of 'arg0'
            for (State s = arg1; null != s; s = s.Parent)
                if (parents.Contains(s))
                    return s;

            // If no common ancestor found, return null
            return null;
        }
    }
}
