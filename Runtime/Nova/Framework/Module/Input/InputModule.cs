/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
/// Copyright (C) 2017 - 2020, Shanghai Tommon Network Technology Co., Ltd.
/// Copyright (C) 2020 - 2022, Guangzhou Xinyuan Technology Co., Ltd.
/// Copyright (C) 2022 - 2023, Shanghai Bilibili Technology Co., Ltd.
/// Copyright (C) 2023 - 2024, Guangzhou Shiyue Network Technology Co., Ltd.
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

using SystemEnum = System.Enum;
using SystemArray = System.Array;
using SystemStringBuilder = System.Text.StringBuilder;

using UnityInput = UnityEngine.Input;
using UnityTouch = UnityEngine.Touch;
using UnityKeyCode = UnityEngine.KeyCode;

namespace NovaEngine
{
    /// <summary>
    /// 输入管理器，处理通过键盘、鼠标及触屏等方式产生的事件通知访问接口
    /// </summary>
    public sealed partial class InputModule : ModuleObject
    {
        /// <summary>
        /// 按键编码列表
        /// </summary>
        private SystemArray _keyCodeArray = null;

        public InputModule() : base()
        {
        }

        /// <summary>
        /// 输入模块事件类型
        /// </summary>
        public override int EventType => (int) ModuleEventType.Input;

        /// <summary>
        /// 管理器对象初始化接口函数
        /// </summary>
        protected override void OnInitialize()
        {
            SystemArray all_keycodes = SystemEnum.GetValues(typeof(UnityKeyCode));
            IList<UnityKeyCode> keycode_list = new List<UnityKeyCode>();
            System.Collections.IEnumerator keycode_enumerator = all_keycodes.GetEnumerator();
            while (keycode_enumerator.MoveNext())
            {
                UnityKeyCode keycode = (UnityKeyCode) keycode_enumerator.Current;
                if (IsKeyboardCode(keycode))
                {
                    keycode_list.Add(keycode);
                }
            }
            _keyCodeArray = Utility.Collection.ToArray<UnityKeyCode>(keycode_list);

            InitInputTrace();
        }

        /// <summary>
        /// 管理器对象清理接口函数
        /// </summary>
        protected override void OnCleanup()
        {
            CleanupInputTrace();

            _keyCodeArray = null;
        }

        /// <summary>
        /// 管理器对象初始启动接口
        /// </summary>
        protected override void OnStartup()
        {
        }

        /// <summary>
        /// 管理器对象结束关闭接口
        /// </summary>
        protected override void OnShutdown()
        {
        }

        /// <summary>
        /// 管理器对象垃圾回收调度接口
        /// </summary>
        protected override void OnDump()
        {
        }

        /// <summary>
        /// 交互管理器内部事务更新接口
        /// </summary>
        protected override void OnUpdate()
        {
#if UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_EDITOR
            //if (Environment.ON_KEYBOARD_ENABLED)
            {
                // 若支持按键交互，则在此响应按键反馈
                if (UnityInput.anyKey || this.IsAnyKeycodeChanged())
                {
                    UnityKeyCode code;
                    bool pressed, moved, released;
                    for (int n = 0; n < _keyCodeArray.Length; ++n)
                    {
                        code = (UnityKeyCode) _keyCodeArray.GetValue(n);

                        // 操作状态标识
                        pressed = false;
                        moved = false;
                        released = false;

                        if (UnityInput.GetKeyDown(code))
                        {
                            // 按键按下事件
                            this.OnKeycodePressed(code);
                            // 记录按下状态
                            pressed = true;
                        }
                        if (UnityInput.GetKeyUp(code))
                        {
                            // 按键释放事件
                            this.OnKeycodeReleased(code);
                            // 记录释放状态
                            released = true;
                        }
                        if (UnityInput.GetKey(code))
                        {
                            // 按键长按事件
                            this.OnKeycodeMoved(code);
                            // 记录长按状态
                            moved = true;
                        }

                        if (pressed && released)
                        {
                            // 按下与释放操作同时触发时，相互抵消
                            this.OnKeycodeUnpressed(code);
                            this.OnKeycodeUnreleased(code);
                        }
                        else if (pressed)
                        {
                            Logger.Assert(!IsKeycodeChanged(code), "Could not found any pressed operation record with keycode '{0}'.", code.ToString());

                            // 记录变化按键
                            this.OnKeycodeChanged(code);
                        }
                        else if (released)
                        {
                            Logger.Assert(IsKeycodeChanged(code), "Could not found any released operation record with keycode '{0}'.", code.ToString());

                            // 取消变化按键
                            this.OnKeycodeUnchanged(code);
                        }
                        else if (moved)
                        {
                            // 变化队列中没有该编码，但是长按事件中有该编码的通知
                            // 这种情况可能是由于外部按键没有释放的情况下重新激活了当前程序的焦点而导致的
                            // 因此，需要重新修补相关数据，追加按下通知和变化通知产生的数据
                            if (false == IsKeycodeChanged(code))
                            {
                                this.OnKeycodePressed(code);
                                this.OnKeycodeChanged(code);
                            }
                        }
                        else
                        {
                            // 没有该编码的任意事件信息，但变化队列中却存在该编码的数据
                            // 这种情况可能是由于当前程序在长按行为的同时丢失了焦点而导致的
                            // 因此，需要重新修补相关数据，追加释放通知和变化通知产生的数据
                            if (IsKeycodeChanged(code))
                            {
                                this.OnKeycodeReleased(code);
                                this.OnKeycodeUnchanged(code);
                            }
                        }
                    }

                    // PrintStackTrace();
                }
            }
#endif
        }

        /// <summary>
        /// 基于标准Input体系的输入检测逻辑刷新函数
        /// </summary>
        private void UpdateForInput()
        {
        }

        /// <summary>
        /// 基于InputSystem体系的输入检测逻辑刷新函数
        /// </summary>
        private void UpdateForInputSystem()
        {
            /*
            // 按键输入
            UnityEngine.InputSystem.Keyboard keyboard = UnityEngine.InputSystem.Keyboard.current;

            if (null != keyboard && keyboard.wasUpdatedThisFrame)
            {
                foreach (UnityEngine.InputSystem.Controls.KeyControl key in keyboard.allKeys.Where(key => key.wasPressedThisFrame))
                {
                }
            }

            // 控制摇杆输入
            UnityEngine.InputSystem.Gamepad gamepad = UnityEngine.InputSystem.Gamepad.current;

            if (null != gamepad && gamepad.wasUpdatedThisFrame)
            {
                foreach (UnityEngine.InputSystem.Controls.KeyControl control in gamepad.allControls.Where(control => control.IsPressed()))
                {
                }
            }
            */
        }

        /// <summary>
        /// 交互管理器内部后置更新接口
        /// </summary>
        protected override void OnLateUpdate()
        {
            this.RemoveAllInputKeycodes();
        }

        /// <summary>
        /// 检测指定按键编码是否为键盘类型的按键信息
        /// </summary>
        /// <param name="code">按键编码</param>
        /// <returns>若目标按键编码为键盘按键信息则返回true，否则返回false</returns>
        private bool IsKeyboardCode(UnityKeyCode code)
        {
            if (UnityKeyCode.Menu >= code)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 打印当前输入的按键编码信息
        /// </summary>
        public void PrintStackTrace()
        {
            SystemStringBuilder sb = new SystemStringBuilder();

            // 当前帧按下行为通知的编码信息
            sb.Append("KeyCode Pressed = { ");
            foreach (UnityKeyCode keyCode in _keycodePressedOnThisFrame)
            {
                sb.Append(keyCode.ToString() + ", ");
            }
            sb.Append("}, ");

            // 当前帧长按行为通知的编码信息
            sb.Append("KeyCode Moved = { ");
            foreach (UnityKeyCode keyCode in _keycodeMovedOnThisFrame)
            {
                sb.Append(keyCode.ToString() + ", ");
            }
            sb.Append("}, ");

            // 当前帧释放行为通知的编码信息
            sb.Append("KeyCode Released = { ");
            foreach (UnityKeyCode keyCode in _keycodeReleasedOnThisFrame)
            {
                sb.Append(keyCode.ToString() + ", ");
            }
            sb.Append("}, ");

            // 上一帧和当前帧之间的变化行为通知的编码信息
            sb.Append("KeyCode Changed = { ");
            foreach (UnityKeyCode keyCode in _keycodeChangedOnPreviousFrame)
            {
                sb.Append(keyCode.ToString() + ", ");
            }
            sb.Append("}");

            Debugger.Log(sb.ToString());
        }
    }
}
