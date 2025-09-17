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
    /// 基于分层调度的有限状态对象类，可以在 <see cref="GameEngine.StateMachine"/> 中作为对象实例进行调度驱动 <br/>
    /// 每个独立个体以有限状态机的形式存在，并提供状态切换、状态执行、状态退出等接口函数供逻辑层使用： <br/>
    /// <code>
    /// OnEnter  // 状态进入接口函数
    /// OnUpdate // 状态更新接口函数
    /// OnExit   // 状态退出接口函数
    /// </code>
    /// </summary>
    public abstract class State
    {
        /// <summary>
        /// 状态机对象实例
        /// </summary>
        private readonly StateMachine _machine = null;
        /// <summary>
        /// 当前状态对象的父节点引用对象实例
        /// </summary>
        private readonly State _parent = null;
        /// <summary>
        /// 当前状态对象的激活子节点引用对象实例
        /// </summary>
        private State _activeChild = null;

        /// <summary>
        /// 状态内部的活动列表
        /// </summary>
        private readonly IList<IStateActivity> _activities = null;

        public State Parent => _parent;

        public State ActiveChild => _activeChild;

        public IReadOnlyList<IStateActivity> Activities => (List<IStateActivity>) _activities;

        public State(StateMachine machine, State parent = null)
        {
            _machine = machine;
            _parent = parent;

            _activities = new List<IStateActivity>();
        }

        // Initial child to enter when this state starts (null = this is the leaf)
        protected virtual State GetInitialState() => null;

        // Target state to switch to this frame (null = stay in current state)
        protected virtual State GetTransition() => null;

        /// <summary>
        /// 新增指定的状态活动到当前的状态实例中
        /// </summary>
        /// <param name="activity">活动对象</param>
        protected internal void Add(IStateActivity activity)
        {
            if (null == activity || _activities.Contains(activity))
            {
                Debugger.Warn(LogGroupTag.Controller, "目标状态对象‘{%t}’内部已存在相同的活动实例‘{%t}’，请勿对同一活动对象进行多次重复添加操作！", this, activity);
                return;
            }

            _activities.Add(activity);
        }

        #region Lifecycle hooks

        protected virtual void OnEnter() { }

        protected virtual void OnExit() { }

        protected virtual void OnUpdate() { }

        #endregion

        internal void Enter()
        {
            if (null != _parent) _parent._activeChild = this;

            OnEnter();

            State init = GetInitialState();
            if (null != init) init.Enter();
        }

        internal void Exit()
        {
            if (null != _activeChild) _activeChild.Exit();
            _activeChild = null;

            OnExit();
        }

        internal void Update()
        {
            State state = GetTransition();
            if (null != state)
            {
                _machine.Sequencer.RequestTransition(this, state);
                return;
            }

            if (null != _activeChild) _activeChild.Update();

            OnUpdate();
        }

        // Returns the deepest currently-active descendant state (the leaf of the active path).
        public State Leaf()
        {
            State state = this;
            while (null != state._activeChild)
                state = state._activeChild;

            return state;
        }

        // Yields this state and then each ancestor up to the root (self -> parent -> ... -> root).
        public IEnumerable<State> PathToRoot()
        {
            for (State state = this; null != state; state = state._parent)
                yield return state;
        }
    }
}
