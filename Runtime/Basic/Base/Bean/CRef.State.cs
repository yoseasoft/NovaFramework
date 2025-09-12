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

using SystemType = System.Type;
using SystemDelegate = System.Delegate;
using SystemMethodInfo = System.Reflection.MethodInfo;
using System.Security.Cryptography.X509Certificates;

namespace GameEngine
{
    /// <summary>
    /// 引用对象抽象类，对场景中的引用对象上下文进行封装及调度管理
    /// </summary>
    public abstract partial class CRef
    {
        /// <summary>
        /// 引用对象内部状态监控的监听回调容器列表
        /// </summary>
        private IDictionary<string, IList<StateCallMethodInfo>> _stateCallInfos = null;

        /// <summary>
        /// 状态构筑对象实例
        /// </summary>
        private StateBuilder _stateBuilder = null;

        /// <summary>
        /// 状态流程图对象实例
        /// </summary>
        private StateGraph _stateGraph = null;

        /// <summary>
        /// 引用对象的状态轮询初始化函数接口
        /// </summary>
        private void OnStatePollInitialize()
        {
            _stateCallInfos = new Dictionary<string, IList<StateCallMethodInfo>>();
        }

        /// <summary>
        /// 引用对象的状态轮询清理函数接口
        /// </summary>
        private void OnStatePollCleanup()
        {
            UnregisterAllStateTransitions();

            _stateCallInfos = null;

            _stateBuilder = null;
            _stateGraph = null;
        }

        #region 引用对象状态构筑相关操作函数合集

        /// <summary>
        /// 创建一个新的状态构筑对象实例
        /// 若该对象实例已存在，则将重置状态图实例，以便重新配置新的流程图
        /// </summary>
        /// <returns>返回状态构筑对象实例</returns>
        public StateBuilder CreateStateBuilder()
        {
            if (null == _stateBuilder)
            {
                if (null == _stateGraph)
                {
                    Debugger.Warn("创建状态构筑对象异常：当前对象类型‘{%t}’没有合法的状态图对象实例，请检查该对象是否在注册状态接口时发生错误！", this);
                    return null;
                }

                _stateBuilder = new StateBuilder(_stateGraph);
            }

            _stateGraph.Reset();

            return _stateBuilder;
        }

        #endregion

        #region 引用对象状态监控相关操作函数合集

        /// <summary>
        /// 引用对象进行指定状态转换的接口函数
        /// </summary>
        /// <param name="stateName">状态名称</param>
        public void StateTransition(string stateName)
        {
            Debugger.Assert(_stateGraph, "The state graph must be non-null.");

            _stateGraph.ChangeState(stateName, true);
        }

        /// <summary>
        /// 引用对象将当前运行状态结束的接口函数
        /// </summary>
        public void StateFinished()
        {
            Debugger.Assert(_stateGraph, "The state graph must be non-null.");

            _stateGraph.OnFinished();
        }

        /// <summary>
        /// 引用对象的状态转换的调度接口函数<br/>
        /// 该函数针对状态转换接口接口的标准实现，禁止子类重写该函数<br/>
        /// 若子类需要根据需要自行解析状态，可以通过重写<see cref="GameEngine.CRef.OnState(string, StateAccessType)"/>实现状态的自定义处理逻辑
        /// </summary>
        private void OnStateDispatch()
        {
            if (null == _stateGraph)
            {
                return;
            }

            if (false == string.IsNullOrEmpty(_stateGraph.CurrentState))
            {
                bool interrupted = false;
                if (false == InvokeStateTransition(_stateGraph.CurrentState, StateAccessType.Update))
                {
                    interrupted = true;
                }

                if (_stateGraph.IsFinished || interrupted)
                {
                    InvokeStateTransition(_stateGraph.CurrentState, StateAccessType.Exit);

                    if (null == _stateGraph.NextState)
                    {
                        string nextState = _stateGraph.GetNextTransitionState();
                        if (null != nextState)
                        {
                            _stateGraph.ChangeState(nextState);
                        }
                    }

                    _stateGraph.CurrentState = null;
                }
            }

            if (false == string.IsNullOrEmpty(_stateGraph.NextState))
            {
                // 目前的方案是如果有后续状态，则立即结束当前状态
                // 直接开启后续状态跳转
                // 如果需要等待当前状态结束后才开始后续流程
                // 就重新梳理这个切换流程
                InvokeStateTransition(_stateGraph.CurrentState, StateAccessType.Exit);
                _stateGraph.CurrentState = _stateGraph.NextState;
                _stateGraph.NextState = null;

                InvokeStateTransition(_stateGraph.CurrentState, StateAccessType.Enter);
            }
        }

        /// <summary>
        /// 对指定的状态实例的特定访问方式进行调度的接口函数
        /// </summary>
        /// <param name="stateName">状态名称</param>
        /// <param name="accessType">状态访问类型</param>
        /// <returns>若成功调度指定状态则返回true，否则返回false</returns>
        private bool InvokeStateTransition(string stateName, StateAccessType accessType)
        {
            bool result = false;
            if (string.IsNullOrEmpty(stateName))
            {
                return result;
            }

            OnState(stateName, accessType);

            if (_stateCallInfos.TryGetValue(stateName, out IList<StateCallMethodInfo> infos))
            {
                for (int n = 0; n < infos.Count; ++n)
                {
                    StateCallMethodInfo info = infos[n];
                    if (info.AccessType == accessType)
                    {
                        info.Invoke();
                        result = true;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// 用户自定义的状态切换函数，您可以通过重写该函数处理自定义状态行为
        /// </summary>
        /// <param name="stateName">状态名称</param>
        /// <param name="accessType">状态访问类型</param>
        protected virtual void OnState(string stateName, StateAccessType accessType) { }

        /// <summary>
        /// 检测当前对象实例是否存在状态图需要调度
        /// </summary>
        /// <returns>若需要调度状态图则返回true，否则返回false</returns>
        private bool IsOnDispatchingOfStateGraph()
        {
            if (null != _stateGraph && _stateGraph.IsNumberOfStateGreatThanZero())
            {
                return true;
            }

            return false;
        }

        private bool IsTargetStateTransitionExists(StateCallMethodInfo targetInfo, IList<StateCallMethodInfo> callMethodInfos)
        {
            for (int n = 0; null != callMethodInfos && n < callMethodInfos.Count; ++n)
            {
                StateCallMethodInfo info = callMethodInfos[n];
                if (info.Fullname.Equals(targetInfo.Fullname))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 引用对象的状态转换注册函数接口，将一个指定的状态访问方式绑定到给定的监听回调函数上
        /// </summary>
        /// <param name="stateName">状态名称</param>
        /// <param name="accessType">状态访问类型</param>
        /// <param name="methodInfo">监听回调函数</param>
        /// <returns>若状态注册成功则返回true，否则返回false</returns>
        internal bool RegisterStateTransition(string stateName, StateAccessType accessType, SystemMethodInfo methodInfo)
        {
            // 函数格式校验
            if (NovaEngine.Debugger.Instance.IsOnDebuggingVerificationActivated())
            {
                bool verificated = false;
                if (NovaEngine.Utility.Reflection.IsTypeOfExtension(methodInfo))
                {
                    verificated = Loader.Inspecting.CodeInspector.CheckFunctionFormatOfStateCallWithBeanExtensionType(methodInfo);
                }
                else
                {
                    verificated = Loader.Inspecting.CodeInspector.CheckFunctionFormatOfStateCall(methodInfo);
                }

                // 校验失败
                if (false == verificated)
                {
                    Debugger.Error("The state transition '{0}' was invalid format, added it failed.", NovaEngine.Utility.Text.ToString(methodInfo));
                    return false;
                }
            }

            if (null == _stateGraph)
            {
                _stateGraph = new StateGraph();
            }
            _stateGraph.AddState(stateName);

            StateCallMethodInfo info = new StateCallMethodInfo(this, stateName, accessType, methodInfo);

            if (false == _stateCallInfos.TryGetValue(stateName, out IList<StateCallMethodInfo> infos))
            {
                // 创建监听列表
                infos = new List<StateCallMethodInfo>();
                infos.Add(info);

                _stateCallInfos.Add(stateName, infos);

                return true;
            }

            if (IsTargetStateTransitionExists(info, infos))
            {
                Debugger.Warn("注册状态转换回调函数异常：当前实体对象‘{%t}’指定状态‘{%s}’的转换列表中已存在名称为‘{%s}’的目标状态信息实例，不能重复添加相同的状态回调函数！", this, stateName, info.Fullname);
                return false;
            }

            infos.Add(info);

            return true;
        }

        /// <summary>
        /// 注销当前引用对象指定的状态实例
        /// </summary>
        /// <param name="stateName">状态名称</param>
        public void UnregisterStateTransition(string stateName)
        {
            if (_stateCallInfos.TryGetValue(stateName, out IList<StateCallMethodInfo> infos))
            {
                while (infos.Count > 0)
                {
                    UnregisterStateTransition(stateName, infos[0].MethodInfo);
                }
            }
        }

        /// <summary>
        /// 注销当前引用对象对指定状态的扩展回调函数
        /// </summary>
        /// <param name="stateName">状态名称</param>
        /// <param name="methodInfo">监听回调函数</param>
        public void UnregisterStateTransition(string stateName, SystemMethodInfo methodInfo)
        {
            string fullname = _Generator.GenUniqueName(methodInfo);

            if (_stateCallInfos.TryGetValue(stateName, out IList<StateCallMethodInfo> infos))
            {
                for (int n = 0; n < infos.Count; ++n)
                {
                    StateCallMethodInfo info = infos[n];
                    if (info.Fullname.Equals(fullname))
                    {
                        infos.Remove(info);
                        break;
                    }
                }

                if (infos.Count <= 0)
                {
                    _stateCallInfos.Remove(stateName);

                    _stateGraph.RemoveState(stateName);
                }
            }
        }

        /// <summary>
        /// 注销当前引用对象的所有状态转换
        /// </summary>
        public virtual void UnregisterAllStateTransitions()
        {
            IList<string> id_keys = NovaEngine.Utility.Collection.ToListForKeys<string, IList<StateCallMethodInfo>>(_stateCallInfos);
            for (int n = 0; null != id_keys && n < id_keys.Count; ++n) { UnregisterStateTransition(id_keys[n]); }

            _stateCallInfos.Clear();
        }

        #endregion

        #region 状态回调接口包装结构及处理函数声明

        protected class StateCallMethodInfo
        {
            /// <summary>
            /// 回调函数的目标对象实例
            /// </summary>
            protected readonly CRef _targetObject;
            /// <summary>
            /// 回调函数的完整名称
            /// </summary>
            protected readonly string _fullname;
            /// <summary>
            /// 回调函数的目标状态名称
            /// </summary>
            protected readonly string _stateName;
            /// <summary>
            /// 回调函数的目标状态访问类型
            /// </summary>
            protected readonly StateAccessType _accessType;
            /// <summary>
            /// 回调函数的函数信息实例
            /// </summary>
            protected readonly SystemMethodInfo _methodInfo;
            /// <summary>
            /// 回调函数的动态构建回调句柄
            /// </summary>
            protected readonly SystemDelegate _callback;
            /// <summary>
            /// 回调函数的扩展定义状态标识
            /// </summary>
            protected readonly bool _isExtensionType;
            /// <summary>
            /// 回调函数的无参状态标识
            /// </summary>
            protected readonly bool _isNullParameterType;

            public string Fullname => _fullname;
            public string StateName => _stateName;
            public StateAccessType AccessType => _accessType;
            public SystemMethodInfo MethodInfo => _methodInfo;
            public SystemDelegate Callback => _callback;
            public bool IsExtensionType => _isExtensionType;

            public StateCallMethodInfo(CRef targetObject, string stateName, StateAccessType accessType, SystemMethodInfo methodInfo)
            {
                _targetObject = targetObject;
                _stateName = stateName;
                _accessType = accessType;
                _methodInfo = methodInfo;
                _isExtensionType = NovaEngine.Utility.Reflection.IsTypeOfExtension(methodInfo);
                _isNullParameterType = Loader.Inspecting.CodeInspector.CheckFunctionFormatOfStateCallWithNullParameterType(methodInfo);

                object obj = targetObject;
                if (_isExtensionType)
                {
                    // 扩展函数在构建委托时不需要传入运行时对象实例，而是在调用时传入
                    obj = null;
                }

                string fullname = _Generator.GenUniqueName(methodInfo);

                SystemDelegate callback = NovaEngine.Utility.Reflection.CreateGenericActionDelegate(obj, methodInfo);
                Debugger.Assert(null != callback, "Invalid method type.");

                _fullname = fullname;
                _callback = callback;
            }

            /// <summary>
            /// 状态回调的调度函数
            /// </summary>
            public void Invoke()
            {
                if (_isExtensionType)
                {
                    if (_isNullParameterType)
                    {
                        _callback.DynamicInvoke(_targetObject);
                    }
                    else
                    {
                        _callback.DynamicInvoke(_targetObject, _targetObject._stateGraph);
                    }
                }
                else
                {
                    if (_isNullParameterType)
                    {
                        _callback.DynamicInvoke();
                    }
                    else
                    {
                        _callback.DynamicInvoke(_targetObject._stateGraph);
                    }
                }
            }
        }

        #endregion
    }
}
