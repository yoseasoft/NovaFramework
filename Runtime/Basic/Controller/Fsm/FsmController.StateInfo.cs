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

namespace GameEngine
{
    /// <summary>
    /// 状态管理对象类，用于对场景上下文中的所有引用对象的状态进行集中管理及分发
    /// </summary>
    internal sealed partial class FsmController
    {
        /// <summary>
        /// 原型对象状态行为映射管理容器
        /// </summary>
        private IDictionary<SystemType, BeanStateCallInfo> _beanStateCallTables = null;

        /// <summary>
        /// 原型对象状态回调相关内容的初始化回调函数
        /// </summary>
        [OnControllerSubmoduleInitCallback]
        private void InitializeForStateCall()
        {
            // 数据容器初始化
            _beanStateCallTables = new Dictionary<SystemType, BeanStateCallInfo>();
        }

        /// <summary>
        /// 原型对象状态回调相关内容的清理回调函数
        /// </summary>
        [OnControllerSubmoduleCleanupCallback]
        private void CleanupForStateCall()
        {
            RemoveAllBeanStateCalls();

            // 移除数据容器
            _beanStateCallTables = null;
        }

        /// <summary>
        /// 新增原型对象指定名称和访问类型的状态行为调度函数
        /// </summary>
        /// <param name="targetType">原型对象类型</param>
        /// <param name="stateName">状态名称</param>
        /// <param name="accessType">状态访问类型</param>
        /// <param name="methodInfo">执行函数接口</param>
        private void AddBeanStateCall(SystemType targetType, string stateName, StateAccessType accessType, SystemMethodInfo methodInfo)
        {
            if (false == _beanStateCallTables.TryGetValue(targetType, out BeanStateCallInfo beanStateCallInfo))
            {
                if (false == typeof(IBean).IsAssignableFrom(targetType))
                {
                    Debugger.Error("新增状态调度函数异常：当前添加状态行为的目标对象类型‘{%t}’匹配错误（需要原型对象类型），请检查状态行为函数‘{%t}’定义的目标对象类型是否合法！", targetType, methodInfo);
                    return;
                }

                beanStateCallInfo = new BeanStateCallInfo(targetType);
                _beanStateCallTables.Add(targetType, beanStateCallInfo);
            }

            beanStateCallInfo.AddStateCallInfo(stateName, accessType, methodInfo);
        }

        /// <summary>
        /// 新增原型对象指定名称和访问类型的状态行为调度函数
        /// </summary>
        /// <param name="fullname">完整名称</param>
        /// <param name="targetType">原型对象类型</param>
        /// <param name="stateName">状态名称</param>
        /// <param name="accessType">状态访问类型</param>
        /// <param name="methodInfo">执行函数接口</param>
        private void AddBeanStateCall(string fullname, SystemType targetType, string stateName, StateAccessType accessType, SystemMethodInfo methodInfo)
        {
            if (false == _beanStateCallTables.TryGetValue(targetType, out BeanStateCallInfo beanStateCallInfo))
            {
                if (false == typeof(IBean).IsAssignableFrom(targetType))
                {
                    Debugger.Error("新增状态调度函数异常：当前添加状态行为的目标对象类型‘{%t}’匹配错误（需要原型对象类型），请检查状态行为函数‘{%t}’定义的目标对象类型是否合法！", targetType, methodInfo);
                    return;
                }

                beanStateCallInfo = new BeanStateCallInfo(targetType);
                _beanStateCallTables.Add(targetType, beanStateCallInfo);
            }

            beanStateCallInfo.AddStateCallInfo(fullname, stateName, accessType, methodInfo);
        }

        /// <summary>
        /// 移除指定原型对象类型对应的全部状态行为调度函数
        /// </summary>
        /// <param name="targetType">原型对象类型</param>
        private void RemoveBeanStateCall(SystemType targetType)
        {
            if (false == _beanStateCallTables.ContainsKey(targetType))
            {
                Debugger.Warn(LogGroupTag.Controller, "当前的对象状态调度管理缓存容器中无法找到与指导类型‘{%t}’相应的状态行为数据实例，请确认该数据是否从未添加或在之前的流程中已经被移除！", targetType);
                return;
            }

            _beanStateCallTables.Remove(targetType);
        }

        /// <summary>
        /// 移除当前状态管理器中登记的所有访问函数回调句柄
        /// </summary>
        private void RemoveAllBeanStateCalls()
        {
            _beanStateCallTables.Clear();
        }

        #region 状态对象内部的数据结构封装及针对状态执行接口函数的构建

        /// <summary>
        /// 状态函数信息记录对象类，用于记录状态类的函数信息
        /// </summary>
        private sealed class StateMethodInfo
        {
            /// <summary>
            /// 状态对象的标识名称
            /// </summary>
            private readonly string _fullname;
            /// <summary>
            /// 状态对象的访问类型
            /// </summary>
            private readonly StateAccessType _accessType;
            /// <summary>
            /// 状态对象的函数实例
            /// </summary>
            private readonly SystemMethodInfo _methodInfo;
            /// <summary>
            /// 状态对象的动态构建回调句柄
            /// </summary>
            private readonly SystemDelegate _callback;

            public string Fullname => _fullname;
            public StateAccessType AccessType => _accessType;

            internal StateMethodInfo(StateAccessType accessType, SystemMethodInfo methodInfo) :
                    this(GenTools.GenUniqueName(methodInfo), accessType, methodInfo)
            { }

            internal StateMethodInfo(string fullname, StateAccessType accessType, SystemMethodInfo methodInfo)
            {
                _fullname = fullname;
                _methodInfo = methodInfo;

                bool isExtensionType = NovaEngine.Utility.Reflection.IsTypeOfExtension(methodInfo);
                bool isNullParameterType = Loader.Inspecting.CodeInspector.CheckFunctionFormatOfMessageCallWithNullParameterType(methodInfo);

                Debugger.Assert(isExtensionType && isNullParameterType, "Invalid arguments.");

                SystemDelegate callback = NovaEngine.Utility.Reflection.CreateGenericActionDelegate(null, methodInfo);
                Debugger.Assert(null != callback, "Invalid method type.");

                _callback = callback;
            }

            /// <summary>
            /// 当前状态函数实例调用接口函数
            /// </summary>
            /// <param name="bean">原型对象实例</param>
            internal void Invoke(IBean bean)
            {
                _callback.DynamicInvoke(bean);
            }
        }

        /// <summary>
        /// 状态分组信息记录对象类，用于记录状态类的分组信息
        /// </summary>
        private sealed class StateGroupInfo
        {
            /// <summary>
            /// 状态对象的服务名称
            /// </summary>
            private readonly string _stateName;
            /// <summary>
            /// 状态对象的函数实例
            /// </summary>
            private readonly IList<StateMethodInfo> _stateMethodList;

            public string StateName => _stateName;

            internal StateGroupInfo(string stateName)
            {
                _stateName = stateName;

                _stateMethodList = new List<StateMethodInfo>();
            }

            /// <summary>
            /// 将指定访问类型对应的执行函数接口添加到当前的目标对象状态管理列表中
            /// </summary>
            /// <param name="accessType">状态访问类型</param>
            /// <param name="methodInfo">执行函数接口</param>
            internal void AddStateMethodInfo(StateAccessType accessType, SystemMethodInfo methodInfo)
            {
                string fullname = GenTools.GenUniqueName(methodInfo);

                AddStateMethodInfo(fullname, accessType, methodInfo);
            }

            /// <summary>
            /// 将指定访问类型对应的执行函数接口添加到当前的目标对象状态管理列表中
            /// </summary>
            /// <param name="fullname">完整名称</param>
            /// <param name="accessType">状态访问类型</param>
            /// <param name="methodInfo">执行函数接口</param>
            internal void AddStateMethodInfo(string fullname, StateAccessType accessType, SystemMethodInfo methodInfo)
            {
                for (int n = 0; n < _stateMethodList.Count; n++)
                {
                    StateMethodInfo info = _stateMethodList[n];
                    if (info.Fullname.Equals(fullname))
                    {
                        Debugger.Error("新增状态调度函数异常：当前状态实例‘{%s}’中已存在同名的函数实例‘{%s}’，不可重复添加相同的函数实例给同一个目标状态对象！", _stateName, fullname);
                        return;
                    }
                }

                StateMethodInfo stateMethodInfo = new StateMethodInfo(fullname, accessType, methodInfo);
                _stateMethodList.Add(stateMethodInfo);
            }

            /// <summary>
            /// 当前状态分组在指定的访问类型下进行状态行为执行逻辑的接口函数
            /// </summary>
            /// <param name="bean">原型对象实例</param>
            /// <param name="accessType">状态访问类型</param>
            internal void OnState(IBean bean, StateAccessType accessType)
            {
                for (int n = 0; n < _stateMethodList.Count; ++n)
                {
                    StateMethodInfo info = _stateMethodList[n];
                    if (info.AccessType == accessType)
                    {
                        info.Invoke(bean);
                    }
                }
            }
        }

        /// <summary>
        /// 基于原型对象的状态行为调度信息对象类，用于分管状态类的调度逻辑
        /// </summary>
        private sealed class BeanStateCallInfo
        {
            /// <summary>
            /// 状态对象的服务目标类型
            /// </summary>
            private readonly SystemType _targetType;
            /// <summary>
            /// 状态对象的函数实例
            /// </summary>
            private readonly IDictionary<string, StateGroupInfo> _stateGroupTable;

            public SystemType TargetType => _targetType;

            internal BeanStateCallInfo(SystemType targetType)
            {
                Debugger.Assert(typeof(IBean).IsAssignableFrom(targetType), "Invalid arguments.");

                _targetType = targetType;
                _stateGroupTable = new Dictionary<string, StateGroupInfo>();
            }

            /// <summary>
            /// 新增指定的状态标识，及其相应的调度管理容器类
            /// </summary>
            /// <param name="stateName">状态名称</param>
            internal void AddStateCallInfo(string stateName)
            {
                if (false == _stateGroupTable.ContainsKey(stateName))
                {
                    StateGroupInfo stateGroupInfo = new StateGroupInfo(stateName);
                    _stateGroupTable.Add(stateName, stateGroupInfo);
                }
            }

            /// <summary>
            /// 新增指定的状态标识，并绑定相应的访问类型及执行函数接口到该状态的调度管理列表中
            /// </summary>
            /// <param name="stateName">状态名称</param>
            /// <param name="accessType">状态访问类型</param>
            /// <param name="methodInfo">执行函数接口</param>
            internal void AddStateCallInfo(string stateName, StateAccessType accessType, SystemMethodInfo methodInfo)
            {
                if (false == _stateGroupTable.TryGetValue(stateName, out StateGroupInfo stateGroupInfo))
                {
                    stateGroupInfo = new StateGroupInfo(stateName);
                    _stateGroupTable.Add(stateName, stateGroupInfo);
                }

                stateGroupInfo.AddStateMethodInfo(accessType, methodInfo);
            }

            /// <summary>
            /// 新增指定的状态标识，并绑定相应的访问类型及执行函数接口到该状态的调度管理列表中
            /// </summary>
            /// <param name="fullname">完整名称</param>
            /// <param name="stateName">状态名称</param>
            /// <param name="accessType">状态访问类型</param>
            /// <param name="methodInfo">执行函数接口</param>
            internal void AddStateCallInfo(string fullname, string stateName, StateAccessType accessType, SystemMethodInfo methodInfo)
            {
                if (false == _stateGroupTable.TryGetValue(stateName, out StateGroupInfo stateGroupInfo))
                {
                    stateGroupInfo = new StateGroupInfo(stateName);
                    _stateGroupTable.Add(stateName, stateGroupInfo);
                }

                stateGroupInfo.AddStateMethodInfo(fullname, accessType, methodInfo);
            }

            /// <summary>
            /// 执行指定原型对象下的给定名称及访问类型的状态实例
            /// </summary>
            /// <param name="bean">原型对象实例</param>
            /// <param name="stateName">状态名称</param>
            /// <param name="accessType">状态访问类型</param>
            internal void OnState(IBean bean, string stateName, StateAccessType accessType)
            {
                if (NovaEngine.Environment.IsDevelopmentState())
                {
                    // 仅在开发模式下进行类型检查
                    if (false == _targetType.IsAssignableFrom(bean.GetType()))
                    {
                        Debugger.Error("对象状态行为执行失败：当前状态实例归属于‘{%t}’对象类型，而实际调度的对象实例为‘{%t}’类型，实际调度对象与定义类型不匹配！", _targetType, bean);
                        return;
                    }
                }

                if (false == _stateGroupTable.TryGetValue(stateName, out StateGroupInfo stateGroupInfo))
                {
                    Debugger.Error("对象状态行为执行异常：无法从当前注册的对象类型‘{%t}’中查找到名称为‘{%s}’的状态数据，请检查该状态是否被正确定义！", _targetType, stateName);
                    return;
                }

                stateGroupInfo.OnState(bean, accessType);
            }
        }

        #endregion
    }
}
