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

namespace GameEngine
{
    /// <summary>
    /// 状态流程图封装对象类，用于管理状态对象的关联关系及过渡方式
    /// </summary>
    public sealed class StateGraph
    {
        /// <summary>
        /// 默认加载的状态实例名称
        /// </summary>
        private string _defaultOperatingStateName;
        /// <summary>
        /// 状态名称容器
        /// </summary>
        private IList<string> _stateNames;
        /// <summary>
        /// 操作状态实例的顺序链表
        /// </summary>
        private IDictionary<string, IList<string>> _transitionSequences;

        /// <summary>
        /// 当前运行状态名称
        /// </summary>
        private string _currentState;
        /// <summary>
        /// 后续转换的目标状态名称
        /// </summary>
        private string _nextState;
        /// <summary>
        /// 当前运行状态是否结束的标识
        /// </summary>
        private bool _isFinished;

        /// <summary>
        /// 启动时指定的状态名称（单次）
        /// </summary>
        private string _launchState;

        /// <summary>
        /// 当前推送的状态名称
        /// </summary>
        private string _pushState;

        /// <summary>
        /// 状态图的用户数据
        /// </summary>
        private object _userData;

        public string DefaultOperatingStateName => _defaultOperatingStateName;
        public object UserData => _userData;

        public string CurrentState { get { return _currentState; } internal set { _currentState = value; } }
        public string NextState { get { return _nextState; } internal set { _nextState = value; } }
        public bool IsFinished { get { return _isFinished; } internal set { _isFinished = value; } }

        public StateGraph()
        {
            // 容器初始化
            _stateNames = new List<string>();
            _transitionSequences = new Dictionary<string, IList<string>>();
        }

        ~StateGraph()
        {
            // 清理容器
            RemoveAllStateNames();
            RemoveAllStateSequences();

            _stateNames = null;
            _transitionSequences = null;

            _userData = null;
        }

        /// <summary>
        /// 状态图的重置操作接口函数
        /// </summary>
        public void Reset()
        {
            _defaultOperatingStateName = null;
            // RemoveAllStateNames();
            RemoveAllStateSequences();

            _currentState = null;
            _nextState = null;
            _isFinished = false;

            _launchState = null;

            _userData = null;
        }

        #region 状态实例相关操作的接口函数

        /// <summary>
        /// 当前状态图的实例容器中新增指定的状态实例
        /// 若该实例已存在，则直接忽略
        /// </summary>
        /// <param name="stateName">状态名称</param>
        internal void AddState(string stateName)
        {
            if (false == _stateNames.Contains(stateName))
            {
                _stateNames.Add(stateName);
            }
        }

        /// <summary>
        /// 从当前状态图中移除指定名称的状态实例
        /// </summary>
        /// <param name="stateName">状态名称</param>
        internal void RemoveState(string stateName)
        {
            if (_stateNames.Contains(stateName))
            {
                _stateNames.Remove(stateName);
            }
        }

        /// <summary>
        /// 移除当前状态图中注册的所有状态实例
        /// </summary>
        private void RemoveAllStateNames()
        {
            _stateNames.Clear();
        }

        /// <summary>
        /// 检查指定的状态名称是否是一个合法的状态实例
        /// </summary>
        /// <param name="stateName">状态名称</param>
        /// <returns>若状态有效则返回true，否则返回false</returns>
        private bool IsValidState(string stateName)
        {
            if (_stateNames.Contains(stateName))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 检查当前状态图中是否拥有超过一个以上的有效状态实例
        /// </summary>
        /// <returns>若对象有状态实例则返回true，否则返回false</returns>
        public bool IsNumberOfStateGreatThanZero()
        {
            if (_stateNames.Count > 0)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 设置当前状态图的用户数据
        /// </summary>
        /// <param name="data">用户数据实例</param>
        public void SetUserData(object data)
        {
            _userData = data;
        }

        #endregion

        #region 状态序列相关操作的接口函数

        /// <summary>
        /// 设置当前状态图默认启动的状态实例
        /// </summary>
        /// <param name="stateName">状态名称</param>
        internal void SetDefaultLaunchState(string stateName)
        {
            if (!IsValidState(stateName))
            {
                Debugger.Warn("设置默认启用状态对象异常：当前对象类型指定的目标状态‘{%s}’为非法值，请检查该状态是否在注册时发生错误！", stateName);
                return;
            }

            _defaultOperatingStateName = stateName;
        }

        /// <summary>
        /// 通过指定状态获取以该状态进行开启的状态序列并进行激活操作
        /// </summary>
        /// <param name="stateName">状态名称</param>
        internal void OnStateSequenceActivationByHeaderName(string stateName)
        {
            if (!IsValidState(stateName))
            {
                Debugger.Warn("激活状态序列异常：当前对象类型指定的目标状态‘{%s}’为非法值，请检查该状态是否在注册时发生错误！", stateName);
                return;
            }

            if (false == _transitionSequences.ContainsKey(stateName))
            {
                Debugger.Warn("激活状态序列异常：当前对象类型中不存在以指定目标状态‘{%s}’开启的状态序列，激活该状态序列操作失败！", stateName);
                return;
            }

            _pushState = stateName;
        }

        /// <summary>
        /// 新增一条从指定状态开始的状态序列，并将该序列指定为当前激活序列
        /// </summary>
        /// <param name="stateName">状态名称</param>
        internal void PushStateSequenceForStart(string stateName)
        {
            if (!IsValidState(stateName))
            {
                Debugger.Warn("配置状态序列启动参数异常：当前对象类型指定的目标状态‘{%s}’为非法值，请检查该状态是否在注册时发生错误！", stateName);
                return;
            }

            if (_transitionSequences.ContainsKey(stateName))
            {
                Debugger.Warn("配置状态序列启动参数异常：当前对象类型已开启指定的状态‘{%s}’的转换序列，重复推送将覆盖旧的配置流程！", stateName);
                _transitionSequences.Remove(stateName);
            }

            IList<string> sequences = new List<string>();
            _transitionSequences.Add(stateName, sequences);

            _pushState = stateName;
            PushStateSequenceForTransition(stateName);
        }

        /// <summary>
        /// 在当前激活序列的链表末尾新增一个指定的状态作为转换目标
        /// 如果您需要在其它序列链表中增加转换目标，则需要先将目标状态序列激活
        /// 激活状态序列的方法请参考<see cref="StateGraph.OnStateSequenceActivationByHeaderName(string)"/>
        /// </summary>
        /// <param name="stateName">状态名称</param>
        internal void PushStateSequenceForTransition(string stateName)
        {
            if (string.IsNullOrEmpty(_pushState) || string.IsNullOrEmpty(stateName))
            {
                Debugger.Warn("配置状态序列转换参数异常：当前对象类型的状态序列转换流程下的启动状态‘{%s}’和转换目标状态‘{%s}’均不能为空，请检查流程和参数后重新执行该操作！", _pushState, stateName);
                return;
            }

            if (!IsValidState(stateName))
            {
                Debugger.Warn("配置状态序列转换参数异常：当前对象类型指定的目标状态‘{%s}’为非法值，请检查该状态是否在注册时发生错误！", stateName);
                return;
            }

            if (false == _transitionSequences.TryGetValue(_pushState, out IList<string> sequences))
            {
                Debugger.Error("配置状态序列转换参数异常：当前对象类型需要指定一个激活的状态序列后才可以对目标状态‘{%s}’进行该序列的转换管理，请先调用状态序列启动函数激活一个新的序列流程！", stateName);
                return;
            }

            // 这里待定，有可能在一条业务线中多次进入同一状态的情况
            // 如果需要支持这种流程，就屏蔽下面的代码，支持同一状态的多次插入
            if (sequences.Contains(stateName))
            {
                Debugger.Warn("配置状态序列转换参数异常：当前对象类型的激活状态序列链表中已存在目标状态‘{%s}’的转换信息，不能对相同的状态在同一序列流程中进行多次指定。", stateName);
                return;
            }

            sequences.Add(stateName);
        }

        /// <summary>
        /// 移除当前状态图中的全部状态序列
        /// </summary>
        private void RemoveAllStateSequences()
        {
            _transitionSequences.Clear();
        }

        #endregion

        #region 状态转换相关操作的接口函数

        /// <summary>
        /// 切换运行状态为指定的状态实例
        /// 若启动标识开启，则表示此状态为序列中的起始状态实例
        /// </summary>
        /// <param name="stateName">状态名称</param>
        /// <param name="launch">序列启动标识</param>
        public void ChangeState(string stateName, bool launch = false)
        {
            if (!IsValidState(stateName))
            {
                Debugger.Warn("状态变更操作异常：当前对象类型指定的目标状态‘{%s}’为非法值，请检查该状态是否在注册时发生错误！", stateName);
                return;
            }

            if (false == string.IsNullOrEmpty(_currentState) && _currentState.Equals(stateName))
            {
                Debugger.Warn("状态变更操作异常：当前对象类型指定转换的目标状态‘{%s}’与调度中的当前状态实例相同，无法对同一状态实例进行转换操作！", stateName);
                return;
            }

            _nextState = stateName;
            // 将当前运行状态实例标记为结束
            _isFinished = false;

            if (launch)
            {
                _launchState = stateName;
            }
        }

        /// <summary>
        /// 当前运行状态实例结束标识
        /// </summary>
        public void OnFinished()
        {
            if (string.IsNullOrEmpty(_currentState))
            {
                Debugger.Warn("状态结束操作异常：当前对象类型尚未激活任何状态实例，无法对空状态进行结束操作！");
                return;
            }

            _isFinished = true;
        }

        /// <summary>
        /// 当前运行状态实例关闭标识
        /// 该接口与<see cref="StateGraph.OnFinished"/>的区别是：
        /// 状态结束只是关闭当前运行状态，会继续序列中的后续状态
        /// 而状态关闭会同时终结序列的后续状态，跳出整个序列流程
        /// </summary>
        public void OnTerminated()
        {
            OnFinished();

            _launchState = null;
        }

        /// <summary>
        /// 获取下一个转换状态实例，该搜索来自于状态序列链表
        /// </summary>
        /// <returns>返回序列中的后续状态实例，若不存在则返回null</returns>
        internal string GetNextTransitionState()
        {
            if (string.IsNullOrEmpty(_currentState) || string.IsNullOrEmpty(_launchState))
            {
                return null;
            }

            if (_transitionSequences.TryGetValue(_launchState, out IList<string> sequences))
            {
                for (int n = 0; n < sequences.Count - 1; ++n)
                {
                    if (_currentState.Equals(sequences[n]))
                    {
                        // 返回下一个转换状态
                        return sequences[n + 1];
                    }
                }
            }

            return null;
        }

        #endregion
    }
}
