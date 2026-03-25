/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyright (C) 2020 - 2022, Guangzhou Xinyuan Technology Co., Ltd.
/// Copyright (C) 2022 - 2023, Shanghai Bilibili Technology Co., Ltd.
/// Copyright (C) 2023 - 2024, Guangzhou Shiyue Network Technology Co., Ltd.
/// Copyright (C) 2024 - 2025, Hurley, Independent Studio.
/// Copyright (C) 2025 - 2026, Hainan Yuanyou Information Technology Co., Ltd. Guangzhou Branch
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

using System;
using System.Collections.Generic;
using System.Reflection;

using UnityEngine.Scripting;

namespace GameEngine
{
    /// 输入模块封装的句柄对象类
    public sealed partial class InputHandler
    {
        /// <summary>
        /// 针对按键编码进行分发的监听对象管理列表容器
        /// </summary>
        private IDictionary<VirtualKeyCode, IList<IInputDispatch>> _inputListenersForCode;

        /// <summary>
        /// 针对输入类型进行分发的监听对象管理列表容器
        /// </summary>
        private IDictionary<Type, IList<IInputDispatch>> _inputListenersForType;

        /// <summary>
        /// 按键编码分发调度接口的数据结构容器
        /// </summary>
        private IDictionary<VirtualKeyCode, IDictionary<string, InputCallMethodInfo>> _inputCodeDistributeCallInfos;

        /// <summary>
        /// 输入数据分发调度接口的数据结构容器
        /// </summary>
        private IDictionary<Type, IDictionary<string, InputCallMethodInfo>> _inputDataDistributeCallInfos;

        /// <summary>
        /// 输入回调转发接口初始化回调函数
        /// </summary>
        [Preserve]
        [OnSubmoduleInitCallback]
        private void OnInputCallDispatchingInitialize()
        {
            // 初始化实例对象调度管理容器
            _inputListenersForCode = new Dictionary<VirtualKeyCode, IList<IInputDispatch>>();
            _inputListenersForType = new Dictionary<Type, IList<IInputDispatch>>();
            // 初始化全局转发调度管理容器
            _inputCodeDistributeCallInfos = new Dictionary<VirtualKeyCode, IDictionary<string, InputCallMethodInfo>>();
            _inputDataDistributeCallInfos = new Dictionary<Type, IDictionary<string, InputCallMethodInfo>>();
        }

        /// <summary>
        /// 输入回调转发接口清理回调函数
        /// </summary>
        [Preserve]
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
        [Preserve]
        [OnSubmoduleReloadCallback]
        private void OnInputCallDispatchingReload()
        {
        }

        #region 针对实例对象输入响应的分发调度管理接口函数

        /// <summary>
        /// 输入分发对象的输入响应函数接口，指派一个指定的监听回调接口到目标输入编码
        /// </summary>
        /// <param name="keyCode">按键编码</param>
        /// <param name="listener">监听回调接口</param>
        /// <returns>若输入响应成功则返回true，否则返回false</returns>
        public bool AddInputResponse(VirtualKeyCode keyCode, IInputDispatch listener)
        {
            // Debugger.Info(LogGroupTag.Module, "新增目标对象类型‘{%t}’的输入转发监听回调接口到指定的输入编码‘{%v}’对应的输入响应管理容器中！", listener, keyCode);

            if (false == _inputListenersForCode.TryGetValue(keyCode, out IList<IInputDispatch> list))
            {
                list = new List<IInputDispatch>() { listener };

                _inputListenersForCode.Add(keyCode, list);
                return true;
            }

            // 检查是否重复添加
            if (list.Contains(listener))
            {
                Debugger.Warn("The listener for target input '{%v}' was already added, cannot repeat do it.", keyCode);
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
        public bool AddInputResponse(Type inputType, IInputDispatch listener)
        {
            // Debugger.Info(LogGroupTag.Module, "新增目标对象类型‘{%t}’的输入转发监听回调接口到指定的输入数据类型‘{%t}’对应的输入响应管理容器中！", listener, inputType);

            if (false == _inputListenersForType.TryGetValue(inputType, out IList<IInputDispatch> list))
            {
                list = new List<IInputDispatch>() { listener };

                _inputListenersForType.Add(inputType, list);
                return true;
            }

            // 检查是否重复添加
            if (list.Contains(listener))
            {
                Debugger.Warn("The listener for target input '{%t}' was already added, cannot repeat do it.", inputType);
                return false;
            }

            list.Add(listener);

            return true;
        }

        /// <summary>
        /// 取消指定输入编码的响应监听回调接口
        /// </summary>
        /// <param name="keyCode">按键编码</param>
        /// <param name="listener">监听回调接口</param>
        public void RemoveInputResponse(VirtualKeyCode keyCode, IInputDispatch listener)
        {
            if (false == _inputListenersForCode.TryGetValue(keyCode, out IList<IInputDispatch> list))
            {
                Debugger.Warn("Could not found any listener for target input '{%v}' with on added, do removed it failed.", keyCode);
                return;
            }

            list.Remove(listener);
            // 列表为空则移除对应的输入监听列表实例
            if (list.Count == 0)
            {
                _inputListenersForCode.Remove(keyCode);
            }

            // Debugger.Info(LogGroupTag.Module, "从输入响应管理容器中移除指定的输入编码‘{%v}’对应的目标对象类型‘{%t}’的输入转发监听回调接口！", keyCode, listener);
        }

        /// <summary>
        /// 取消指定输入类型的响应监听回调接口
        /// </summary>
        /// <param name="inputType">输入类型</param>
        /// <param name="listener">监听回调接口</param>
        public void RemoveInputResponse(Type inputType, IInputDispatch listener)
        {
            if (false == _inputListenersForType.TryGetValue(inputType, out IList<IInputDispatch> list))
            {
                Debugger.Warn("Could not found any listener for target input '{%t}' with on added, do removed it failed.", inputType);
                return;
            }

            list.Remove(listener);
            // 列表为空则移除对应的输入监听列表实例
            if (list.Count == 0)
            {
                _inputListenersForType.Remove(inputType);
            }

            // Debugger.Info(LogGroupTag.Module, "从输入响应管理容器中移除指定的输入数据类型‘{%t}’对应的目标对象类型‘{%t}’的输入转发监听回调接口！", inputType, listener);
        }

        /// <summary>
        /// 取消指定的监听回调接口对应的所有输入响应
        /// </summary>
        public void RemoveInputResponseForTarget(IInputDispatch listener)
        {
            IList<VirtualKeyCode> ids = NovaEngine.Utility.Collection.ToListForKeys<VirtualKeyCode, IList<IInputDispatch>>(_inputListenersForCode);
            for (int n = 0; null != ids && n < ids.Count; ++n)
            {
                RemoveInputResponse(ids[n], listener);
            }

            IList<Type> types = NovaEngine.Utility.Collection.ToListForKeys<Type, IList<IInputDispatch>>(_inputListenersForType);
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
        /// <param name="keyCode">按键编码</param>
        /// <param name="operationType">按键操作类型</param>
        private void OnInputDistributeCallDispatched(VirtualKeyCode keyCode, InputOperationType operationType)
        {
            if (_inputCodeDistributeCallInfos.TryGetValue(keyCode, out IDictionary<string, InputCallMethodInfo> infos))
            {
                IEnumerator<KeyValuePair<string, InputCallMethodInfo>> e_info = infos.GetEnumerator();
                while (e_info.MoveNext())
                {
                    InputCallMethodInfo info = e_info.Current.Value;
                    if (null == info.TargetType)
                    {
                        info.Invoke(keyCode, operationType);
                    }
                    else
                    {
                        IReadOnlyList<IBean> beans = BeanController.Instance.FindAllBeans(info.TargetType);
                        if (null != beans)
                        {
                            IEnumerator<IBean> e_bean = beans.GetEnumerator();
                            while (e_bean.MoveNext())
                            {
                                IBean bean = e_bean.Current;
                                info.Invoke(bean, keyCode, operationType);
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
            Type inputDataType = inputData.GetType();

            if (_inputDataDistributeCallInfos.TryGetValue(inputDataType, out IDictionary<string, InputCallMethodInfo> infos))
            {
                IEnumerator<KeyValuePair<string, InputCallMethodInfo>> e_info = infos.GetEnumerator();
                while (e_info.MoveNext())
                {
                    InputCallMethodInfo info = e_info.Current.Value;
                    if (null == info.TargetType)
                    {
                        info.Invoke(inputData);
                    }
                    else
                    {
                        IReadOnlyList<IBean> beans = BeanController.Instance.FindAllBeans(info.TargetType);
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
        /// <param name="keyCode">按键编码</param>
        /// <param name="operationType">操作类型</param>
        private void AddInputDistributeCallInfo(string fullname, Type targetType, MethodInfo methodInfo, VirtualKeyCode keyCode, InputOperationType operationType)
        {
            if (false == _inputCodeDistributeCallInfos.TryGetValue(keyCode, out IDictionary<string, InputCallMethodInfo> infos))
            {
                infos = new Dictionary<string, InputCallMethodInfo>();
                _inputCodeDistributeCallInfos.Add(keyCode, infos);
            }

            if (infos.TryGetValue(fullname, out InputCallMethodInfo info))
            {
                Debugger.Info(LogGroupTag.Module, "Update input distribute call '{%s}' to target code '{%v}' and operation type '{%v}' of the class type '{%t}'.",
                    fullname, keyCode, operationType, targetType);
                info.RegisterOperationType(keyCode, operationType);
                return;
            }

            info = new InputCallMethodInfo(fullname, targetType, methodInfo, keyCode, operationType);

            Debugger.Info(LogGroupTag.Module, "Add new input distribute call '{%s}' to target code '{%v}' and operation type '{%v}' of the class type '{%t}'.",
                    fullname, keyCode, operationType, targetType);

            infos.Add(fullname, info);
        }

        /// <summary>
        /// 新增指定的分发函数到当前输入响应监听管理容器中
        /// </summary>
        /// <param name="fullname">完整名称</param>
        /// <param name="targetType">目标对象类型</param>
        /// <param name="methodInfo">函数对象</param>
        /// <param name="inputDataType">输入数据类型</param>
        private void AddInputDistributeCallInfo(string fullname, Type targetType, MethodInfo methodInfo, Type inputDataType)
        {
            if (false == _inputDataDistributeCallInfos.TryGetValue(inputDataType, out IDictionary<string, InputCallMethodInfo> infos))
            {
                infos = new Dictionary<string, InputCallMethodInfo>();
                _inputDataDistributeCallInfos.Add(inputDataType, infos);
            }

            if (infos.TryGetValue(fullname, out InputCallMethodInfo info))
            {
                Debugger.Warn(LogGroupTag.Module, "The input distribute call '{%s}' to target data type '{%t}' was already exists of the class type '{%t}', repeat added it failed.",
                    fullname, inputDataType, targetType);
                return;
            }

            info = new InputCallMethodInfo(fullname, targetType, methodInfo, inputDataType);

            Debugger.Info(LogGroupTag.Module, "Add new input distribute call '{%s}' to target data type '{%t}' of the class type '{%t}'.",
                    fullname, inputDataType, targetType);

            infos.Add(fullname, info);
        }

        /// <summary>
        /// 从当前输入响应监听管理容器中移除指定标识的分发函数信息
        /// </summary>
        /// <param name="fullname">完整名称</param>
        /// <param name="targetType">目标对象类型</param>
        /// <param name="keyCode">按键编码</param>
        /// <param name="operationType">操作类型</param>
        private void RemoveInputDistributeCallInfo(string fullname, Type targetType, VirtualKeyCode keyCode, InputOperationType operationType)
        {
            Debugger.Info(LogGroupTag.Module, "Remove input distribute call '{%s}' with target code '{%v}' of the class type '{%t}'.",
                    fullname, keyCode, targetType);
            if (false == _inputCodeDistributeCallInfos.TryGetValue(keyCode, out IDictionary<string, InputCallMethodInfo> infos))
            {
                Debugger.Warn(LogGroupTag.Module, "Could not found any input distribute call '{%s}' with target code '{%v}', removed it failed.", fullname, keyCode);
                return;
            }

            infos.Remove(fullname);
            if (infos.Count <= 0)
            {
                _inputCodeDistributeCallInfos.Remove(keyCode);
            }
        }

        /// <summary>
        /// 从当前输入响应监听管理容器中移除指定类型的分发函数信息
        /// </summary>
        /// <param name="fullname">完整名称</param>
        /// <param name="targetType">目标对象类型</param>
        /// <param name="inputDataType">输入数据类型</param>
        private void RemoveInputDistributeCallInfo(string fullname, Type targetType, Type inputDataType)
        {
            Debugger.Info(LogGroupTag.Module, "Remove input distribute call '{%s}' with target data type '{%t}' of the class type '{%t}'.",
                    fullname, inputDataType, targetType);
            if (false == _inputDataDistributeCallInfos.TryGetValue(inputDataType, out IDictionary<string, InputCallMethodInfo> infos))
            {
                Debugger.Warn(LogGroupTag.Module, "Could not found any input distribute call '{%s}' with target data type '{%t}', removed it failed.", fullname, inputDataType);
                return;
            }

            infos.Remove(fullname);
            if (infos.Count <= 0)
            {
                _inputDataDistributeCallInfos.Remove(inputDataType);
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
