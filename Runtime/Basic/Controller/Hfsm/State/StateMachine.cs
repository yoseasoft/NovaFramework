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

namespace GameEngine.HFSM
{
    /// <summary>
    /// 状态机管理对象类，提供对引擎内部的有限状态对象实例进行调度管理
    /// </summary>
    public class StateMachine
    {
        /// <summary>
        /// 状态机当前的根节点对象实例
        /// </summary>
        private State _root;
        /// <summary>
        /// 状态机当前的状态转换序列管理对象实例
        /// </summary>
        private StateTransitionSequencer _sequencer;

        /// <summary>
        /// 状态机对象运行状态标识
        /// </summary>
        private bool _isRunning;

        public State Root => _root;

        public StateTransitionSequencer Sequencer => _sequencer;

        public StateMachine(State root)
        {
            _root = root;
            _sequencer = new StateTransitionSequencer(this);
        }

        /// <summary>
        /// 状态机对象启动函数
        /// </summary>
        public void Start()
        {
            if (_isRunning) return;

            _isRunning = true;

            _root.Enter();
        }

        /// <summary>
        /// 状态机对象停止函数
        /// </summary>
        public void Stop()
        {
            if (!_isRunning) return;

            _isRunning = false;

            _root.Exit();
        }

        public void Tick()
        {
            if (!_isRunning)
            {
                Debugger.Warn(LogGroupTag.Controller, "当前状态机对象实例尚未启动，不能正确进行‘Tick’操作！");
                return;
            }

            // InternalTick();

            // 这里运行序列器的刷新逻辑，而不直接运行状态机刷新逻辑，是因为序列器中可能存在过渡操作，
            // 在过渡操作运行的过程中，需要先停止当前激活状态自身的刷新逻辑。
            _sequencer.Tick();
        }

        internal void InternalTick() => _root.Update();

        // Perform the actual switch from 'from' to 'to' by exiting up to the shared ancestor, then entering down to the target.
        public void ChangeState(State from, State to)
        {
            if (null == from || null == to || from == to) return;

            State lca = StateTransitionSequencer.Lca(from, to);

            // Exit current branch up to (but not including) LCA
            for (State s = from; s != lca; s = s.Parent)
                s.Exit();

            // Enter target branch from LCA down to target
            Stack<State> stack = new Stack<State>();
            for (State s = to; s != lca; s = s.Parent)
                stack.Push(s);

            while (stack.Count > 0)
                stack.Pop().Enter();
        }
    }
}
