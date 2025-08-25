/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyright (C) 2020 - 2022, Guangzhou Xinyuan Technology Co., Ltd.
/// Copyright (C) 2022 - 2023, Shanghai Bilibili Technology Co., Ltd.
/// Copyright (C) 2023 - 2024, Guangzhou Shiyue Network Technology Co., Ltd.
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

using UnityKeyCode = UnityEngine.KeyCode;

namespace GameEngine
{
    /// <summary>
    /// 输入模块封装的句柄对象类
    /// 模块具体功能接口请参考<see cref="NovaEngine.InputModule"/>类
    /// </summary>
    public sealed partial class InputHandler : BaseHandler
    {
        /// <summary>
        /// 句柄对象的单例访问获取接口
        /// </summary>
        public static InputHandler Instance => HandlerManagement.InputHandler;

        /// <summary>
        /// 针对事件标识进行分发的监听对象管理列表容器
        /// </summary>
        private IDictionary<int, IList<IInputDispatch>> _inputListenersForCode = null;

        /// <summary>
        /// 针对事件类型进行分发的监听对象管理列表容器
        /// </summary>
        private IDictionary<SystemType, IList<IInputDispatch>> _inputListenersForType = null;

        /// <summary>
        /// 通过按键编码分发调度接口的数据结构容器
        /// </summary>
        private IDictionary<int, IList<InputCallInfo>> _inputCodeDistributeCallInfos = null;

        /// <summary>
        /// 通过输入数据分发调度接口的数据结构容器
        /// </summary>
        private IDictionary<SystemType, IList<InputCallInfo>> _inputDataDistributeCallInfos = null;

        /// <summary>
        /// 句柄对象内置初始化接口函数
        /// </summary>
        /// <returns>若句柄对象初始化成功则返回true，否则返回false</returns>
        protected override bool OnInitialize()
        {
            // 初始化实例对象调度管理容器
            _inputListenersForCode = new Dictionary<int, IList<IInputDispatch>>();
            _inputListenersForType = new Dictionary<SystemType, IList<IInputDispatch>>();
            // 初始化全局转发调度管理容器
            _inputCodeDistributeCallInfos = new Dictionary<int, IList<InputCallInfo>>();
            _inputDataDistributeCallInfos = new Dictionary<SystemType, IList<InputCallInfo>>();

            return true;
        }

        /// <summary>
        /// 句柄对象内置清理接口函数
        /// </summary>
        protected override void OnCleanup()
        {
            // 清理实例对象调度管理容器
            _inputListenersForCode.Clear();
            _inputListenersForCode = null;
            _inputListenersForType.Clear();
            _inputListenersForType = null;

            // 清理全局转发调度管理容器
            _inputCodeDistributeCallInfos.Clear();
            _inputCodeDistributeCallInfos = null;
            _inputDataDistributeCallInfos.Clear();
            _inputDataDistributeCallInfos = null;
        }

        /// <summary>
        /// 句柄对象内置刷新接口
        /// </summary>
        protected override void OnUpdate()
        {
            // 没有任何按键输入的情况下，无需进行任何转发处理
            if (false == InputModule.IsAnyKeycodeInputed())
            {
                return;
            }

            IList<UnityKeyCode> list = null;

            // 按下操作分发调度流程
            if (InputModule.IsAnyKeycodePressed())
            {
                list = InputModule.GetAllPressedKeycodes();
                for (int n = 0; n < list.Count; ++n)
                {
                    OnInputDispatched((int) list[n], (int) InputOperationType.Pressed);
                }
            }

            // 长按操作分发调度流程
            if (InputModule.IsAnyKeycodeMoved())
            {
                list = InputModule.GetAllMovedKeycodes();
                for (int n = 0; n < list.Count; ++n)
                {
                    OnInputDispatched((int) list[n], (int) InputOperationType.Moved);
                }
            }

            // 释放操作分发调度流程
            if (InputModule.IsAnyKeycodeReleased())
            {
                list = InputModule.GetAllReleasedKeycodes();
                for (int n = 0; n < list.Count; ++n)
                {
                    OnInputDispatched((int) list[n], (int) InputOperationType.Released);
                }
            }
        }

        /// <summary>
        /// 句柄对象内置延迟刷新接口
        /// </summary>
        protected override void OnLateUpdate()
        {
        }

        /// <summary>
        /// 句柄对象的模块事件转发回调接口
        /// </summary>
        /// <param name="e">模块事件参数</param>
        public override void OnEventDispatch(NovaEngine.ModuleEventArgs e)
        {
        }

        /// <summary>
        /// 检测当前帧是否触发了任意按键编码的录入操作，包括按下，长按及释放
        /// </summary>
        /// <returns>若触发了任意按键编码的录入操作则返回true，否则返回false</returns>
        public bool IsAnyKeycodeInputed()
        {
            return InputModule.IsAnyKeycodeInputed();
        }

        /// <summary>
        /// 检测当前帧是否触发了指定按键编码的录入操作，包括按下，长按及释放
        /// </summary>
        /// <param name="code">按键编码</param>
        /// <returns>若触发了给定按键编码的录入操作则返回true，否则返回false</returns>
        public bool IsKeycodeInputed(UnityKeyCode code)
        {
            return InputModule.IsKeycodeInputed(code);
        }

        /// <summary>
        /// 检测当前帧是否触发了任意按键编码的按下操作
        /// </summary>
        /// <returns>若触发了任意按键编码的按下操作则返回true，否则返回false</returns>
        public bool IsAnyKeycodePressed()
        {
            return InputModule.IsAnyKeycodePressed();
        }

        /// <summary>
        /// 检测当前帧是否触发了指定按键编码的按下操作
        /// </summary>
        /// <param name="code">按键编码</param>
        /// <returns>若触发了给定按键编码的按下操作则返回true，否则返回false</returns>
        public bool IsKeycodePressed(UnityKeyCode code)
        {
            return InputModule.IsKeycodePressed(code);
        }

        /// <summary>
        /// 检测当前帧是否触发了任意按键编码的长按操作
        /// </summary>
        /// <returns>若触发了任意按键编码的长按操作则返回true，否则返回false</returns>
        public bool IsAnyKeycodeMoved()
        {
            return InputModule.IsAnyKeycodeMoved();
        }

        /// <summary>
        /// 检测当前帧是否触发了指定按键编码的长按操作
        /// </summary>
        /// <param name="code">按键编码</param>
        /// <returns>若触发了给定按键编码的长按操作则返回true，否则返回false</returns>
        public bool IsKeycodeMoved(UnityKeyCode code)
        {
            return InputModule.IsKeycodeMoved(code);
        }

        /// <summary>
        /// 检测当前帧是否触发了任意按键编码的释放操作
        /// </summary>
        /// <returns>若触发了任意按键编码的释放操作则返回true，否则返回false</returns>
        public bool IsAnyKeycodeReleased()
        {
            return InputModule.IsAnyKeycodeReleased();
        }

        /// <summary>
        /// 检测当前帧是否触发了指定按键编码的释放操作
        /// </summary>
        /// <param name="code">按键编码</param>
        /// <returns>若触发了给定按键编码的释放操作则返回true，否则返回false</returns>
        public bool IsKeycodeReleased(UnityKeyCode code)
        {
            return InputModule.IsKeycodeReleased(code);
        }

        #region 输入响应的编码数据分发调度管理接口函数

        /// <summary>
        /// 通过模拟输入操作的方式发送自定义按键编码的接口函数
        /// </summary>
        /// <param name="inputCode">按键编码</param>
        /// <param name="operationType">按键操作类型</param>
        public void OnSimulationInputOperation(int inputCode, int operationType)
        {
            OnInputDispatched(inputCode, operationType);
        }

        /// <summary>
        /// 通过模拟输入操作的方式发送自定义数据的接口函数
        /// </summary>
        /// <param name="inputData">输入数据</param>
        public void OnSimulationInputOperation(object inputData)
        {
            OnInputDispatched(inputData);
        }

        /// <summary>
        /// 针对按键编码进行响应分发的调度入口函数
        /// </summary>
        /// <param name="inputCode">按键编码</param>
        /// <param name="operationType">按键操作类型</param>
        private void OnInputDispatched(int inputCode, int operationType)
        {
            // 输入分发调度
            OnInputDistributeCallDispatched(inputCode, operationType);

            IList<IInputDispatch> listeners;
            if (_inputListenersForCode.TryGetValue(inputCode, out listeners))
            {
                IList<IInputDispatch> list;
                if (listeners.Count > 1)
                {
                    list = new List<IInputDispatch>();
                    ((List<IInputDispatch>) list).AddRange(listeners);
                }
                else
                {
                    list = listeners;
                }

                for (int n = 0; n < list.Count; ++n)
                {
                    IInputDispatch listener = list[n];
                    listener.OnInputDispatchForCode(inputCode, operationType);
                }

                list = null;
            }
        }

        /// <summary>
        /// 针对输入数据进行响应分发的调度入口函数
        /// </summary>
        /// <param name="inputData">输入数据</param>
        private void OnInputDispatched(object inputData)
        {
            // 输入分发调度
            OnInputDistributeCallDispatched(inputData);

            IList<IInputDispatch> listeners;
            if (_inputListenersForType.TryGetValue(inputData.GetType(), out listeners))
            {
                IList<IInputDispatch> list;
                if (listeners.Count > 1)
                {
                    list = new List<IInputDispatch>();
                    ((List<IInputDispatch>) list).AddRange(listeners);
                }
                else
                {
                    list = listeners;
                }

                for (int n = 0; n < list.Count; ++n)
                {
                    IInputDispatch listener = list[n];
                    listener.OnInputDispatchForType(inputData);
                }

                list = null;
            }
        }

        #endregion

        #region 针对实例对象输入响应的分发调度管理接口函数

        /// <summary>
        /// 输入分发对象的输入响应函数接口，指派一个指定的监听回调接口到目标输入编码
        /// </summary>
        /// <param name="inputCode">输入编码</param>
        /// <param name="listener">监听回调接口</param>
        /// <returns>若输入响应成功则返回true，否则返回false</returns>
        public bool AddInputResponse(int inputCode, IInputDispatch listener)
        {
            //Debugger.Info(LogGroupTag.Module, "新增目标对象类型‘{%s}’的输入转发监听回调接口到指定的输入编码‘{%d}’对应的输入响应管理容器中！",
            //        NovaEngine.Utility.Text.ToString(listener.GetType()), inputCode);

            IList<IInputDispatch> list;
            if (false == _inputListenersForCode.TryGetValue(inputCode, out list))
            {
                list = new List<IInputDispatch>();
                list.Add(listener);

                _inputListenersForCode.Add(inputCode, list);
                return true;
            }

            // 检查是否重复添加
            if (list.Contains(listener))
            {
                Debugger.Warn("The listener for target input '{0}' was already added, cannot repeat do it.", inputCode);
                return false;
            }

            list.Add(listener);

            return true;
        }

        /// <summary>
        /// 输入分发对象的输入响应函数接口，指派一个指定的监听回调接口到目标输入类型
        /// </summary>
        /// <param name="inputType">输入类型</param>
        /// <param name="listener">监听回调接口</param>
        /// <returns>若输入响应成功则返回true，否则返回false</returns>
        public bool AddInputResponse(SystemType inputType, IInputDispatch listener)
        {
            //Debugger.Info(LogGroupTag.Module, "新增目标对象类型‘{%s}’的输入转发监听回调接口到指定的输入数据类型‘{%s}’对应的输入响应管理容器中！",
            //        NovaEngine.Utility.Text.ToString(listener.GetType()), NovaEngine.Utility.Text.ToString(inputType));

            IList<IInputDispatch> list;
            if (false == _inputListenersForType.TryGetValue(inputType, out list))
            {
                list = new List<IInputDispatch>();
                list.Add(listener);

                _inputListenersForType.Add(inputType, list);
                return true;
            }

            // 检查是否重复添加
            if (list.Contains(listener))
            {
                Debugger.Warn("The listener for target input '{0}' was already added, cannot repeat do it.", inputType.FullName);
                return false;
            }

            list.Add(listener);

            return true;
        }

        /// <summary>
        /// 取消指定输入编码的响应监听回调接口
        /// </summary>
        /// <param name="inputCode">输入编码</param>
        /// <param name="listener">监听回调接口</param>
        public void RemoveInputResponse(int inputCode, IInputDispatch listener)
        {
            IList<IInputDispatch> list;
            if (false == _inputListenersForCode.TryGetValue(inputCode, out list))
            {
                Debugger.Warn("Could not found any listener for target input '{0}' with on added, do removed it failed.", inputCode);
                return;
            }

            list.Remove(listener);
            // 列表为空则移除对应的输入监听列表实例
            if (list.Count == 0)
            {
                _inputListenersForCode.Remove(inputCode);
            }

            //Debugger.Info(LogGroupTag.Module, "从输入响应管理容器中移除指定的输入编码‘{%d}’对应的目标对象类型‘{%s}’的输入转发监听回调接口！",
            //        inputCode, NovaEngine.Utility.Text.ToString(listener.GetType()));
        }

        /// <summary>
        /// 取消指定输入类型的响应监听回调接口
        /// </summary>
        /// <param name="inputType">输入类型</param>
        /// <param name="listener">监听回调接口</param>
        public void RemoveInputResponse(SystemType inputType, IInputDispatch listener)
        {
            IList<IInputDispatch> list;
            if (false == _inputListenersForType.TryGetValue(inputType, out list))
            {
                Debugger.Warn("Could not found any listener for target input '{0}' with on added, do removed it failed.", inputType.FullName);
                return;
            }

            list.Remove(listener);
            // 列表为空则移除对应的输入监听列表实例
            if (list.Count == 0)
            {
                _inputListenersForType.Remove(inputType);
            }

            //Debugger.Info(LogGroupTag.Module, "从输入响应管理容器中移除指定的输入数据类型‘{%s}’对应的目标对象类型‘{%s}’的输入转发监听回调接口！",
            //        NovaEngine.Utility.Text.ToString(inputType), NovaEngine.Utility.Text.ToString(listener.GetType()));
        }

        /// <summary>
        /// 取消指定的监听回调接口对应的所有输入响应
        /// </summary>
        public void RemoveInputResponseForTarget(IInputDispatch listener)
        {
            IList<int> ids = NovaEngine.Utility.Collection.ToListForKeys<int, IList<IInputDispatch>>(_inputListenersForCode);
            for (int n = 0; null != ids && n < ids.Count; ++n)
            {
                RemoveInputResponse(ids[n], listener);
            }

            IList<SystemType> types = NovaEngine.Utility.Collection.ToListForKeys<SystemType, IList<IInputDispatch>>(_inputListenersForType);
            for (int n = 0; null != types && n < types.Count; ++n)
            {
                RemoveInputResponse(types[n], listener);
            }
        }

        #endregion

        #region 针对全局转发输入响应的分发调度管理接口函数

        /// <summary>
        /// 针对按键编码进行响应分发的调度入口函数
        /// </summary>
        /// <param name="inputCode">按键编码</param>
        /// <param name="operationType">按键操作类型</param>
        private void OnInputDistributeCallDispatched(int inputCode, int operationType)
        {
            IList<InputCallInfo> list = null;
            if (_inputCodeDistributeCallInfos.TryGetValue(inputCode, out list))
            {
                IEnumerator<InputCallInfo> e_info = list.GetEnumerator();
                while (e_info.MoveNext())
                {
                    InputCallInfo info = e_info.Current;
                    if (null == info.TargetType)
                    {
                        info.Invoke(inputCode, operationType);
                    }
                    else
                    {
                        IList<IBean> beans = BeanController.Instance.FindAllBeans(info.TargetType);
                        if (null != beans)
                        {
                            IEnumerator<IBean> e_bean = beans.GetEnumerator();
                            while (e_bean.MoveNext())
                            {
                                IBean bean = e_bean.Current;
                                info.Invoke(bean, inputCode, operationType);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 针对输入数据进行响应分发的调度入口函数
        /// </summary>
        /// <param name="inputData">输入数据</param>
        private void OnInputDistributeCallDispatched(object inputData)
        {
            SystemType inputDataType = inputData.GetType();

            IList<InputCallInfo> list = null;
            if (_inputDataDistributeCallInfos.TryGetValue(inputDataType, out list))
            {
                IEnumerator<InputCallInfo> e_info = list.GetEnumerator();
                while (e_info.MoveNext())
                {
                    InputCallInfo info = e_info.Current;
                    if (null == info.TargetType)
                    {
                        info.Invoke(inputData);
                    }
                    else
                    {
                        IList<IBean> beans = BeanController.Instance.FindAllBeans(info.TargetType);
                        if (null != beans)
                        {
                            IEnumerator<IBean> e_bean = beans.GetEnumerator();
                            while (e_bean.MoveNext())
                            {
                                IBean bean = e_bean.Current;
                                info.Invoke(bean, inputData);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 新增指定的分发函数到当前输入响应监听管理容器中
        /// </summary>
        /// <param name="fullname">完整名称</param>
        /// <param name="targetType">目标对象类型</param>
        /// <param name="inputCode">按键编码</param>
        /// <param name="operationType">操作类型</param>
        /// <param name="callback">函数回调句柄</param>
        private void AddInputDistributeCallInfo(string fullname, SystemType targetType, int inputCode, int operationType, SystemDelegate callback)
        {
            InputCallInfo info = new InputCallInfo(fullname, targetType, inputCode, operationType, callback);

            Debugger.Info(LogGroupTag.Module, "新增指定的按键编码‘{%d}’及操作类型‘{%d}’对应的输入响应监听事件，其响应接口函数来自于目标类型‘{%t}’的‘{%s}’函数。",
                    inputCode, operationType, targetType, fullname);
            if (_inputCodeDistributeCallInfos.ContainsKey(inputCode))
            {
                IList<InputCallInfo> list = _inputCodeDistributeCallInfos[inputCode];
                list.Add(info);
            }
            else
            {
                IList<InputCallInfo> list = new List<InputCallInfo>();
                list.Add(info);
                _inputCodeDistributeCallInfos.Add(inputCode, list);
            }
        }

        /// <summary>
        /// 新增指定的分发函数到当前输入响应监听管理容器中
        /// </summary>
        /// <param name="fullname">完整名称</param>
        /// <param name="targetType">目标对象类型</param>
        /// <param name="inputDataType">输入数据类型</param>
        /// <param name="callback">函数回调句柄</param>
        private void AddInputDistributeCallInfo(string fullname, SystemType targetType, SystemType inputDataType, SystemDelegate callback)
        {
            InputCallInfo info = new InputCallInfo(fullname, targetType, inputDataType, callback);

            Debugger.Info(LogGroupTag.Module, "新增指定的按键编码的数据类型‘{%t}’对应的输入响应监听事件，其响应接口函数来自于目标类型‘{%t}’的‘{%s}’函数。",
                    inputDataType, targetType, fullname);
            if (_inputDataDistributeCallInfos.ContainsKey(inputDataType))
            {
                IList<InputCallInfo> list = _inputDataDistributeCallInfos[inputDataType];
                list.Add(info);
            }
            else
            {
                IList<InputCallInfo> list = new List<InputCallInfo>();
                list.Add(info);
                _inputDataDistributeCallInfos.Add(inputDataType, list);
            }
        }

        /// <summary>
        /// 从当前输入响应监听管理容器中移除指定标识的分发函数信息
        /// </summary>
        /// <param name="fullname">完整名称</param>
        /// <param name="targetType">目标对象类型</param>
        /// <param name="inputCode">按键编码</param>
        private void RemoveInputDistributeCallInfo(string fullname, SystemType targetType, int inputCode, int operationType)
        {
            Debugger.Info(LogGroupTag.Module, "移除指定的按键编码‘{%d}’及操作类型‘{%d}’对应的全部输入响应监听事件，其响应接口函数来自于目标类型‘{%t}’的‘{%s}’函数。",
                    inputCode, operationType, targetType, fullname);
            if (false == _inputCodeDistributeCallInfos.ContainsKey(inputCode))
            {
                Debugger.Warn(LogGroupTag.Module, "从当前的输入响应管理容器中无法找到任何与目标按键编码‘{%d}’匹配的响应登记信息，移除输入响应分发信息失败！", inputCode);
                return;
            }

            bool succ = false;
            IList<InputCallInfo> list = _inputCodeDistributeCallInfos[inputCode];
            for (int n = list.Count - 1; n >= 0; --n)
            {
                InputCallInfo info = list[n];
                if (info.Fullname.Equals(fullname))
                {
                    Debugger.Assert(info.TargetType == targetType && info.InputCode == inputCode, "Invalid arguments.");

                    list.RemoveAt(n);
                    if (list.Count <= 0)
                    {
                        _inputCodeDistributeCallInfos.Remove(inputCode);
                    }

                    succ = true;
                }
            }

            if (!succ)
            {
                Debugger.Warn(LogGroupTag.Module, "从目标对象类型‘{%t}’的‘{%s}’函数中无法检索到任何与指定的按键编码‘{%d}’匹配的输入响应分发接口，对给定编码的移除操作执行失败！",
                    targetType, fullname, inputCode);
            }
        }

        /// <summary>
        /// 从当前输入响应监听管理容器中移除指定类型的分发函数信息
        /// </summary>
        /// <param name="fullname">完整名称</param>
        /// <param name="targetType">目标对象类型</param>
        /// <param name="inputDataType">输入数据类型</param>
        private void RemoveInputDistributeCallInfo(string fullname, SystemType targetType, SystemType inputDataType)
        {
            Debugger.Info(LogGroupTag.Module, "移除指定的按键编码的数据类型‘{%t}’对应的全部输入响应监听事件，其响应接口函数来自于目标类型‘{%t}’的‘{%s}’函数。",
                    inputDataType, targetType, fullname);
            if (false == _inputDataDistributeCallInfos.ContainsKey(inputDataType))
            {
                Debugger.Warn(LogGroupTag.Module, "从当前的输入响应管理容器中无法找到任何与目标数据类型‘{%t}’匹配的响应登记信息，移除输入响应分发信息失败！", inputDataType);
                return;
            }

            bool succ = false;
            IList<InputCallInfo> list = _inputDataDistributeCallInfos[inputDataType];
            for (int n = list.Count - 1; n >= 0; --n)
            {
                InputCallInfo info = list[n];
                if (info.Fullname.Equals(fullname))
                {
                    Debugger.Assert(info.TargetType == targetType && info.InputDataType == inputDataType, "Invalid arguments.");

                    list.RemoveAt(n);
                    if (list.Count <= 0)
                    {
                        _inputDataDistributeCallInfos.Remove(inputDataType);
                    }

                    succ = true;
                }
            }

            if (!succ)
            {
                Debugger.Warn(LogGroupTag.Module, "从目标对象类型‘{%t}’的‘{%s}’函数中无法检索到任何与指定的输入数据类型‘{%t}’匹配的输入响应分发接口，对给定类型的移除操作执行失败！",
                    targetType, fullname, inputDataType);
            }
        }

        /// <summary>
        /// 移除当前输入响应管理器中登记的所有分发函数回调句柄
        /// </summary>
        private void RemoveAllInputDistributeCalls()
        {
            _inputCodeDistributeCallInfos.Clear();
            _inputDataDistributeCallInfos.Clear();
        }

        #endregion

        #region 全局转发类型的输入响应调度接口的数据信息结构对象声明

        /// <summary>
        /// 输入响应的数据信息类
        /// </summary>
        private class InputCallInfo
        {
            /// <summary>
            /// 输入响应类的完整名称
            /// </summary>
            private string _fullname;
            /// <summary>
            /// 输入响应类的目标对象类型
            /// </summary>
            private SystemType _targetType;
            /// <summary>
            /// 输入响应类的编码信息
            /// </summary>
            private int _inputCode;
            /// <summary>
            /// 输入响应类的操作类型
            /// </summary>
            private int _operationType;
            /// <summary>
            /// 输入响应类的键码数据类型
            /// </summary>
            private SystemType _inputDataType;
            /// <summary>
            /// 输入响应类的回调句柄
            /// </summary>
            private SystemDelegate _callback;
            /// <summary>
            /// 输入响应回调函数的无参状态标识
            /// </summary>
            private bool _isNullParameterType;

            public string Fullname => _fullname;
            public SystemType TargetType => _targetType;
            public int InputCode => _inputCode;
            public int OperationType => _operationType;
            public SystemType InputDataType => _inputDataType;
            public SystemDelegate Callback => _callback;
            public bool IsNullParameterType => _isNullParameterType;

            public InputCallInfo(string fullname, SystemType targetType, int inputCode, int operationType, SystemDelegate callback)
                : this(fullname, targetType, inputCode, operationType, null, callback)
            {
            }

            public InputCallInfo(string fullname, SystemType targetType, SystemType inputDataType, SystemDelegate callback)
                : this(fullname, targetType, 0, 0, inputDataType, callback)
            {
            }

            private InputCallInfo(string fullname, SystemType targetType, int inputCode, int operationType, SystemType inputDataType, SystemDelegate callback)
            {
                Debugger.Assert(null != callback, "Invalid arguments.");

                _fullname = fullname;
                _targetType = targetType;
                _inputCode = inputCode;
                _operationType = operationType;
                _inputDataType = inputDataType;
                _callback = callback;
                _isNullParameterType = Loader.Inspecting.CodeInspector.CheckFunctionFormatOfInputCallWithNullParameterType(callback.Method);
            }

            /// <summary>
            /// 基于按键编码的输入回调转发函数
            /// </summary>
            /// <param name="inputCode">按键编码</param>
            /// <param name="operationType">按键操作类型</param>
            public void Invoke(int inputCode, int operationType)
            {
                if (_operationType == 0 || (_operationType & operationType) == 0)
                {
                    // ignore
                    return;
                }

                if (_isNullParameterType)
                {
                    _callback.DynamicInvoke();
                }
                else
                {
                    _callback.DynamicInvoke(inputCode, operationType);
                }
            }

            /// <summary>
            /// 基于按键编码的输入回调转发函数
            /// </summary>
            /// <param name="bean">目标原型对象</param>
            /// <param name="inputCode">按键编码</param>
            /// <param name="operationType">按键操作类型</param>
            public void Invoke(IBean bean, int inputCode, int operationType)
            {
                if (_operationType == 0 || (_operationType & operationType) == 0)
                {
                    // ignore
                    return;
                }

                if (_isNullParameterType)
                {
                    _callback.DynamicInvoke(bean);
                }
                else
                {
                    _callback.DynamicInvoke(bean, inputCode, operationType);
                }
            }

            /// <summary>
            /// 基于数据集合类型的输入回调转发函数
            /// </summary>
            /// <param name="inputData">输入数据</param>
            public void Invoke(object inputData)
            {
                if (_isNullParameterType)
                {
                    _callback.DynamicInvoke();
                }
                else
                {
                    _callback.DynamicInvoke(inputData);
                }
            }

            /// <summary>
            /// 基于数据集合类型的输入回调转发函数
            /// </summary>
            /// <param name="bean">目标原型对象</param>
            /// <param name="inputData">输入数据</param>
            public void Invoke(IBean bean, object inputData)
            {
                if (_isNullParameterType)
                {
                    _callback.DynamicInvoke(bean);
                }
                else
                {
                    _callback.DynamicInvoke(bean, inputData);
                }
            }
        }

        #endregion
    }
}
