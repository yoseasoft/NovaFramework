/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyright (C) 2020 - 2022, Guangzhou Xinyuan Technology Co., Ltd.
/// Copyright (C) 2022 - 2023, Shanghai Bilibili Technology Co., Ltd.
/// Copyright (C) 2023 - 2024, Guangzhou Shiyue Network Technology Co., Ltd.
/// Copyright (C) 2025, Hurley, Independent Studio.
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
        /// 通过按键编码分发调度接口的数据结构容器
        /// </summary>
        private IDictionary<int, IList<InputResponseInfo>> m_inputCodeDistributeResponseInfos = null;

        /// <summary>
        /// 通过输入数据分发调度接口的数据结构容器
        /// </summary>
        private IDictionary<SystemType, IList<InputResponseInfo>> m_inputDataDistributeResponseInfos = null;

        /// <summary>
        /// 句柄对象内置初始化接口函数
        /// </summary>
        /// <returns>若句柄对象初始化成功则返回true，否则返回false</returns>
        protected override bool OnInitialize()
        {
            // 初始化转发调度管理容器
            m_inputCodeDistributeResponseInfos = new Dictionary<int, IList<InputResponseInfo>>();
            m_inputDataDistributeResponseInfos = new Dictionary<SystemType, IList<InputResponseInfo>>();

            return true;
        }

        /// <summary>
        /// 句柄对象内置清理接口函数
        /// </summary>
        protected override void OnCleanup()
        {
            // 清理转发调度管理容器
            m_inputCodeDistributeResponseInfos.Clear();
            m_inputCodeDistributeResponseInfos = null;
            m_inputDataDistributeResponseInfos.Clear();
            m_inputDataDistributeResponseInfos = null;
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
                    OnInputDistributeResponseDispatched((int) list[n], (int) InputOperationType.Pressed);
                }
            }

            // 长按操作分发调度流程
            if (InputModule.IsAnyKeycodeMoved())
            {
                list = InputModule.GetAllMovedKeycodes();
                for (int n = 0; n < list.Count; ++n)
                {
                    OnInputDistributeResponseDispatched((int) list[n], (int) InputOperationType.Moved);
                }
            }

            // 释放操作分发调度流程
            if (InputModule.IsAnyKeycodeReleased())
            {
                list = InputModule.GetAllReleasedKeycodes();
                for (int n = 0; n < list.Count; ++n)
                {
                    OnInputDistributeResponseDispatched((int) list[n], (int) InputOperationType.Released);
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
        /// 针对按键编码进行响应分发的调度入口函数
        /// </summary>
        /// <param name="keycode">按键编码</param>
        /// <param name="operationType">按键操作类型</param>
        private void OnInputDistributeResponseDispatched(int keycode, int operationType)
        {
            IList<InputResponseInfo> list = null;
            if (m_inputCodeDistributeResponseInfos.TryGetValue(keycode, out list))
            {
                IEnumerator<InputResponseInfo> e_info = list.GetEnumerator();
                while (e_info.MoveNext())
                {
                    InputResponseInfo info = e_info.Current;
                    if (null == info.TargetType)
                    {
                        info.Invoke(keycode, operationType);
                    }
                    else
                    {
                        IList<IProto> protos = ProtoController.Instance.FindAllProtos(info.TargetType);
                        if (null != protos)
                        {
                            IEnumerator<IProto> e_proto = protos.GetEnumerator();
                            while (e_proto.MoveNext())
                            {
                                IProto proto = e_proto.Current;
                                info.Invoke(proto, keycode, operationType);
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
        private void OnInputDistributeResponseDispatched(object inputData)
        {
            SystemType inputDataType = inputData.GetType();

            IList<InputResponseInfo> list = null;
            if (m_inputDataDistributeResponseInfos.TryGetValue(inputDataType, out list))
            {
                IEnumerator<InputResponseInfo> e_info = list.GetEnumerator();
                while (e_info.MoveNext())
                {
                    InputResponseInfo info = e_info.Current;
                    if (null == info.TargetType)
                    {
                        info.Invoke(inputData);
                    }
                    else
                    {
                        IList<IProto> protos = ProtoController.Instance.FindAllProtos(info.TargetType);
                        if (null != protos)
                        {
                            IEnumerator<IProto> e_proto = protos.GetEnumerator();
                            while (e_proto.MoveNext())
                            {
                                IProto proto = e_proto.Current;
                                info.Invoke(proto, inputData);
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
        /// <param name="keycode">按键编码</param>
        /// <param name="operationType">操作类型</param>
        /// <param name="callback">函数回调句柄</param>
        private void AddInputDistributeResponseInfo(string fullname, SystemType targetType, int keycode, int operationType, SystemDelegate callback)
        {
            InputResponseInfo info = new InputResponseInfo(fullname, targetType, keycode, operationType, callback);

            Debugger.Info(LogGroupTag.Module, "新增指定的按键编码‘{%d}’及操作类型‘{%d}’对应的输入响应监听事件，其响应接口函数来自于目标类型‘{%f}’的‘{%s}’函数。",
                    keycode, operationType, targetType, fullname);
            if (m_inputCodeDistributeResponseInfos.ContainsKey(keycode))
            {
                IList<InputResponseInfo> list = m_inputCodeDistributeResponseInfos[keycode];
                list.Add(info);
            }
            else
            {
                IList<InputResponseInfo> list = new List<InputResponseInfo>();
                list.Add(info);
                m_inputCodeDistributeResponseInfos.Add(keycode, list);
            }
        }

        /// <summary>
        /// 新增指定的分发函数到当前输入响应监听管理容器中
        /// </summary>
        /// <param name="fullname">完整名称</param>
        /// <param name="targetType">目标对象类型</param>
        /// <param name="inputDataType">输入数据类型</param>
        /// <param name="callback">函数回调句柄</param>
        private void AddInputDistributeResponseInfo(string fullname, SystemType targetType, SystemType inputDataType, SystemDelegate callback)
        {
            InputResponseInfo info = new InputResponseInfo(fullname, targetType, inputDataType, callback);

            Debugger.Info(LogGroupTag.Module, "新增指定的按键编码的数据类型‘{%f}’对应的输入响应监听事件，其响应接口函数来自于目标类型‘{%f}’的‘{%s}’函数。",
                    inputDataType, targetType, fullname);
            if (m_inputDataDistributeResponseInfos.ContainsKey(inputDataType))
            {
                IList<InputResponseInfo> list = m_inputDataDistributeResponseInfos[inputDataType];
                list.Add(info);
            }
            else
            {
                IList<InputResponseInfo> list = new List<InputResponseInfo>();
                list.Add(info);
                m_inputDataDistributeResponseInfos.Add(inputDataType, list);
            }
        }

        /// <summary>
        /// 从当前输入响应监听管理容器中移除指定标识的分发函数信息
        /// </summary>
        /// <param name="fullname">完整名称</param>
        /// <param name="targetType">目标对象类型</param>
        /// <param name="keycode">按键编码</param>
        private void RemoveInputDistributeResponseInfo(string fullname, SystemType targetType, int keycode, int operationType)
        {
            Debugger.Info(LogGroupTag.Module, "移除指定的按键编码‘{%d}’及操作类型‘{%d}’对应的全部输入响应监听事件，其响应接口函数来自于目标类型‘{%f}’的‘{%s}’函数。",
                    keycode, operationType, targetType, fullname);
            if (false == m_inputCodeDistributeResponseInfos.ContainsKey(keycode))
            {
                Debugger.Warn(LogGroupTag.Module, "从当前的输入响应管理容器中无法找到任何与目标按键编码‘{%d}’匹配的响应登记信息，移除输入响应分发信息失败！", keycode);
                return;
            }

            bool succ = false;
            IList<InputResponseInfo> list = m_inputCodeDistributeResponseInfos[keycode];
            for (int n = list.Count - 1; n >= 0; --n)
            {
                InputResponseInfo info = list[n];
                if (info.Fullname.Equals(fullname))
                {
                    Debugger.Assert(info.TargetType == targetType && info.Keycode == keycode, "Invalid arguments.");

                    list.RemoveAt(n);
                    if (list.Count <= 0)
                    {
                        m_inputCodeDistributeResponseInfos.Remove(keycode);
                    }

                    succ = true;
                }
            }

            if (!succ)
            {
                Debugger.Warn(LogGroupTag.Module, "从目标对象类型‘{%f}’的‘{%s}’函数中无法检索到任何与指定的按键编码‘{%d}’匹配的输入响应分发接口，对给定编码的移除操作执行失败！",
                    targetType, fullname, keycode);
            }
        }

        /// <summary>
        /// 从当前输入响应监听管理容器中移除指定类型的分发函数信息
        /// </summary>
        /// <param name="fullname">完整名称</param>
        /// <param name="targetType">目标对象类型</param>
        /// <param name="inputDataType">输入数据类型</param>
        private void RemoveInputDistributeResponseInfo(string fullname, SystemType targetType, SystemType inputDataType)
        {
            Debugger.Info(LogGroupTag.Module, "移除指定的按键编码的数据类型‘{%f}’对应的全部输入响应监听事件，其响应接口函数来自于目标类型‘{%f}’的‘{%s}’函数。",
                    inputDataType, targetType, fullname);
            if (false == m_inputDataDistributeResponseInfos.ContainsKey(inputDataType))
            {
                Debugger.Warn(LogGroupTag.Module, "从当前的输入响应管理容器中无法找到任何与目标数据类型‘{%f}’匹配的响应登记信息，移除输入响应分发信息失败！", inputDataType);
                return;
            }

            bool succ = false;
            IList<InputResponseInfo> list = m_inputDataDistributeResponseInfos[inputDataType];
            for (int n = list.Count - 1; n >= 0; --n)
            {
                InputResponseInfo info = list[n];
                if (info.Fullname.Equals(fullname))
                {
                    Debugger.Assert(info.TargetType == targetType && info.InputDataType == inputDataType, "Invalid arguments.");

                    list.RemoveAt(n);
                    if (list.Count <= 0)
                    {
                        m_inputDataDistributeResponseInfos.Remove(inputDataType);
                    }

                    succ = true;
                }
            }

            if (!succ)
            {
                Debugger.Warn(LogGroupTag.Module, "从目标对象类型‘{%f}’的‘{%s}’函数中无法检索到任何与指定的输入数据类型‘{%f}’匹配的输入响应分发接口，对给定类型的移除操作执行失败！",
                    targetType, fullname, inputDataType);
            }
        }

        /// <summary>
        /// 移除当前输入响应管理器中登记的所有分发函数回调句柄
        /// </summary>
        private void RemoveAllInputDistributeResponses()
        {
            m_inputCodeDistributeResponseInfos.Clear();
            m_inputDataDistributeResponseInfos.Clear();
        }

        #endregion

        #region 输入响应的数据信息结构对象声明

        /// <summary>
        /// 输入响应的数据信息类
        /// </summary>
        private class InputResponseInfo
        {
            /// <summary>
            /// 输入响应类的完整名称
            /// </summary>
            private string m_fullname;
            /// <summary>
            /// 输入响应类的目标对象类型
            /// </summary>
            private SystemType m_targetType;
            /// <summary>
            /// 输入响应类的编码信息
            /// </summary>
            private int m_keycode;
            /// <summary>
            /// 输入响应类的操作类型
            /// </summary>
            private int m_operationType;
            /// <summary>
            /// 输入响应类的键码数据类型
            /// </summary>
            private SystemType m_inputDataType;
            /// <summary>
            /// 输入响应类的回调句柄
            /// </summary>
            private SystemDelegate m_callback;
            /// <summary>
            /// 输入响应回调函数的无参状态标识
            /// </summary>
            private bool m_isNullParameterType;

            public string Fullname => m_fullname;
            public SystemType TargetType => m_targetType;
            public int Keycode => m_keycode;
            public int OperationType => m_operationType;
            public SystemType InputDataType => m_inputDataType;
            public SystemDelegate Callback => m_callback;
            public bool IsNullParameterType => m_isNullParameterType;

            public InputResponseInfo(string fullname, SystemType targetType, int keycode, int operationType, SystemDelegate callback)
                : this(fullname, targetType, keycode, operationType, null, callback)
            {
            }

            public InputResponseInfo(string fullname, SystemType targetType, SystemType inputDataType, SystemDelegate callback)
                : this(fullname, targetType, 0, 0, inputDataType, callback)
            {
            }

            private InputResponseInfo(string fullname, SystemType targetType, int keycode, int operationType, SystemType inputDataType, SystemDelegate callback)
            {
                Debugger.Assert(null != callback, "Invalid arguments.");

                m_fullname = fullname;
                m_targetType = targetType;
                m_keycode = keycode;
                m_operationType = operationType;
                m_inputDataType = inputDataType;
                m_callback = callback;
                m_isNullParameterType = Loader.Inspecting.CodeInspector.IsNullParameterTypeOfInputResponseFunction(callback.Method);
            }

            /// <summary>
            /// 基于按键编码的输入回调转发函数
            /// </summary>
            /// <param name="keycode">按键编码</param>
            /// <param name="operationType">按键操作类型</param>
            public void Invoke(int keycode, int operationType)
            {
                if (m_operationType == 0 || (m_operationType & operationType) == 0)
                {
                    // ignore
                    return;
                }

                if (m_isNullParameterType)
                {
                    m_callback.DynamicInvoke();
                }
                else
                {
                    m_callback.DynamicInvoke(keycode, operationType);
                }
            }

            /// <summary>
            /// 基于按键编码的输入回调转发函数
            /// </summary>
            /// <param name="proto">目标原型对象</param>
            /// <param name="keycode">按键编码</param>
            /// <param name="operationType">按键操作类型</param>
            public void Invoke(IProto proto, int keycode, int operationType)
            {
                if (m_operationType == 0 || (m_operationType & operationType) == 0)
                {
                    // ignore
                    return;
                }

                if (m_isNullParameterType)
                {
                    m_callback.DynamicInvoke(proto);
                }
                else
                {
                    m_callback.DynamicInvoke(proto, keycode, operationType);
                }
            }

            /// <summary>
            /// 基于数据集合类型的输入回调转发函数
            /// </summary>
            /// <param name="inputData">输入数据</param>
            public void Invoke(object inputData)
            {
                if (m_isNullParameterType)
                {
                    m_callback.DynamicInvoke();
                }
                else
                {
                    m_callback.DynamicInvoke(inputData);
                }
            }

            /// <summary>
            /// 基于数据集合类型的输入回调转发函数
            /// </summary>
            /// <param name="proto">目标原型对象</param>
            /// <param name="inputData">输入数据</param>
            public void Invoke(IProto proto, object inputData)
            {
                if (m_isNullParameterType)
                {
                    m_callback.DynamicInvoke(proto);
                }
                else
                {
                    m_callback.DynamicInvoke(proto, inputData);
                }
            }
        }

        #endregion
    }
}
