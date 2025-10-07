/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyright (C) 2020 - 2022, Guangzhou Xinyuan Technology Co., Ltd.
/// Copyright (C) 2022 - 2023, Shanghai Bilibili Technology Co., Ltd.
/// Copyright (C) 2023 - 2024, Guangzhou Shiyue Network Technology Co., Ltd.
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

using SystemType = System.Type;
using SystemMethodInfo = System.Reflection.MethodInfo;

namespace GameEngine
{
    /// <summary>
    /// 输入模块封装的句柄对象类
    /// 模块具体功能接口请参考<see cref="NovaEngine.InputModule"/>类
    /// </summary>
    public sealed partial class InputHandler
    {
        /// <summary>
        /// 针对事件标识进行分发的监听对象管理列表容器
        /// </summary>
        private IDictionary<int, IList<IInputDispatch>> _inputListenersForCode;

        /// <summary>
        /// 针对事件类型进行分发的监听对象管理列表容器
        /// </summary>
        private IDictionary<SystemType, IList<IInputDispatch>> _inputListenersForType;

        /// <summary>
        /// 按键编码分发调度接口的数据结构容器
        /// </summary>
        private IDictionary<int, IList<InputCallMethodInfo>> _inputCodeDistributeCallInfos;

        /// <summary>
        /// 输入数据分发调度接口的数据结构容器
        /// </summary>
        private IDictionary<SystemType, IList<InputCallMethodInfo>> _inputDataDistributeCallInfos;

        /// <summary>
        /// 输入回调转发接口初始化回调函数
        /// </summary>
        [UnityEngine.Scripting.Preserve]
        [OnSubmoduleInitCallback]
        private void OnInputCallDispatchingInitialize()
        {
            // 初始化实例对象调度管理容器
            _inputListenersForCode = new Dictionary<int, IList<IInputDispatch>>();
            _inputListenersForType = new Dictionary<SystemType, IList<IInputDispatch>>();
            // 初始化全局转发调度管理容器
            _inputCodeDistributeCallInfos = new Dictionary<int, IList<InputCallMethodInfo>>();
            _inputDataDistributeCallInfos = new Dictionary<SystemType, IList<InputCallMethodInfo>>();
        }

        /// <summary>
        /// 输入回调转发接口清理回调函数
        /// </summary>
        [UnityEngine.Scripting.Preserve]
        [OnSubmoduleCleanupCallback]
        private void OnInputCallDispatchingCleanup()
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
        /// 输入回调转发接口重载回调函数
        /// </summary>
        [UnityEngine.Scripting.Preserve]
        [OnSubmoduleReloadCallback]
        private void OnInputCallDispatchingReload()
        {
        }

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
            IList<InputCallMethodInfo> list = null;
            if (_inputCodeDistributeCallInfos.TryGetValue(inputCode, out list))
            {
                IEnumerator<InputCallMethodInfo> e_info = list.GetEnumerator();
                while (e_info.MoveNext())
                {
                    InputCallMethodInfo info = e_info.Current;
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

            IList<InputCallMethodInfo> list = null;
            if (_inputDataDistributeCallInfos.TryGetValue(inputDataType, out list))
            {
                IEnumerator<InputCallMethodInfo> e_info = list.GetEnumerator();
                while (e_info.MoveNext())
                {
                    InputCallMethodInfo info = e_info.Current;
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
        /// <param name="methodInfo">函数对象</param>
        /// <param name="inputCode">按键编码</param>
        /// <param name="operationType">操作类型</param>
        private void AddInputDistributeCallInfo(string fullname, SystemType targetType, SystemMethodInfo methodInfo, int inputCode, int operationType)
        {
            InputCallMethodInfo info = new InputCallMethodInfo(fullname, targetType, methodInfo, inputCode, operationType);

            Debugger.Info(LogGroupTag.Module, "新增指定的按键编码‘{%d}’及操作类型‘{%d}’对应的输入响应监听事件，其响应接口函数来自于目标类型‘{%t}’的‘{%s}’函数。",
                    inputCode, operationType, targetType, fullname);
            if (_inputCodeDistributeCallInfos.ContainsKey(inputCode))
            {
                IList<InputCallMethodInfo> list = _inputCodeDistributeCallInfos[inputCode];
                list.Add(info);
            }
            else
            {
                IList<InputCallMethodInfo> list = new List<InputCallMethodInfo>();
                list.Add(info);
                _inputCodeDistributeCallInfos.Add(inputCode, list);
            }
        }

        /// <summary>
        /// 新增指定的分发函数到当前输入响应监听管理容器中
        /// </summary>
        /// <param name="fullname">完整名称</param>
        /// <param name="targetType">目标对象类型</param>
        /// <param name="methodInfo">函数对象</param>
        /// <param name="inputDataType">输入数据类型</param>
        private void AddInputDistributeCallInfo(string fullname, SystemType targetType, SystemMethodInfo methodInfo, SystemType inputDataType)
        {
            InputCallMethodInfo info = new InputCallMethodInfo(fullname, targetType, methodInfo, inputDataType);

            Debugger.Info(LogGroupTag.Module, "新增指定的按键编码的数据类型‘{%t}’对应的输入响应监听事件，其响应接口函数来自于目标类型‘{%t}’的‘{%s}’函数。",
                    inputDataType, targetType, fullname);
            if (_inputDataDistributeCallInfos.ContainsKey(inputDataType))
            {
                IList<InputCallMethodInfo> list = _inputDataDistributeCallInfos[inputDataType];
                list.Add(info);
            }
            else
            {
                IList<InputCallMethodInfo> list = new List<InputCallMethodInfo>();
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
            IList<InputCallMethodInfo> list = _inputCodeDistributeCallInfos[inputCode];
            for (int n = list.Count - 1; n >= 0; --n)
            {
                InputCallMethodInfo info = list[n];
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
            IList<InputCallMethodInfo> list = _inputDataDistributeCallInfos[inputDataType];
            for (int n = list.Count - 1; n >= 0; --n)
            {
                InputCallMethodInfo info = list[n];
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
    }
}
