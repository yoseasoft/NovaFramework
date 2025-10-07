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
        /// 句柄对象内置初始化接口函数
        /// </summary>
        /// <returns>若句柄对象初始化成功则返回true，否则返回false</returns>
        protected override bool OnInitialize()
        {
            return true;
        }

        /// <summary>
        /// 句柄对象内置清理接口函数
        /// </summary>
        protected override void OnCleanup()
        {
        }

        /// <summary>
        /// 句柄对象内置重载接口函数
        /// </summary>
        protected override void OnReload()
        {
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
    }
}
