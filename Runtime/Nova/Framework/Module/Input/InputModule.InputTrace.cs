/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
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

using UnityInput = UnityEngine.Input;
using UnityTouch = UnityEngine.Touch;
using UnityKeyCode = UnityEngine.KeyCode;

namespace NovaEngine
{
    /// <summary>
    /// 输入管理器，处理通过键盘、鼠标及触屏等方式产生的事件通知访问接口
    /// </summary>
    internal partial class InputModule
    {
        /// <summary>
        /// 当前帧按下操作产生的按键编码
        /// </summary>
        private IList<UnityKeyCode> _keycodePressedOnThisFrame;

        /// <summary>
        /// 当前帧长按操作产生的按键编码
        /// </summary>
        private IList<UnityKeyCode> _keycodeMovedOnThisFrame;

        /// <summary>
        /// 当前帧释放操作产生的按键编码
        /// </summary>
        private IList<UnityKeyCode> _keycodeReleasedOnThisFrame;

        /// <summary>
        /// 上一帧按下操作保留的按键编码
        /// </summary>
        private IList<UnityKeyCode> _keycodeChangedOnPreviousFrame;

        /// <summary>
        /// 输入信息相关属性初始化函数
        /// </summary>
        private void InitInputTrace()
        {
            _keycodePressedOnThisFrame = new List<UnityKeyCode>();
            _keycodeMovedOnThisFrame = new List<UnityKeyCode>();
            _keycodeReleasedOnThisFrame = new List<UnityKeyCode>();
            _keycodeChangedOnPreviousFrame = new List<UnityKeyCode>();
        }

        /// <summary>
        /// 输入信息相关属性清理函数
        /// </summary>
        private void CleanupInputTrace()
        {
            RemoveAllInputKeycodes();
            _keycodePressedOnThisFrame = null;
            _keycodeMovedOnThisFrame = null;
            _keycodeReleasedOnThisFrame = null;

            _keycodeChangedOnPreviousFrame.Clear();
            _keycodeChangedOnPreviousFrame = null;
        }

        #region 按键编码触发回调响应接口函数

        /// <summary>
        /// 记录当前帧按下操作触发的按键编码信息
        /// </summary>
        /// <param name="code">按键编码</param>
        private void OnKeycodePressed(UnityKeyCode code)
        {
            _keycodePressedOnThisFrame.Add(code);
        }

        /// <summary>
        /// 取消当前帧按下操作触发的按键编码信息
        /// </summary>
        /// <param name="code">按键编码</param>
        private void OnKeycodeUnpressed(UnityKeyCode code)
        {
            _keycodePressedOnThisFrame.Remove(code);
        }

        /// <summary>
        /// 记录当前帧长按操作触发的按键编码信息
        /// </summary>
        /// <param name="code">按键编码</param>
        private void OnKeycodeMoved(UnityKeyCode code)
        {
            _keycodeMovedOnThisFrame.Add(code);
        }

        /// <summary>
        /// 记录当前帧释放操作触发的按键编码信息
        /// </summary>
        /// <param name="code">按键编码</param>
        private void OnKeycodeReleased(UnityKeyCode code)
        {
            _keycodeReleasedOnThisFrame.Add(code);
        }

        /// <summary>
        /// 取消当前帧释放操作触发的按键编码信息
        /// </summary>
        /// <param name="code">按键编码</param>
        private void OnKeycodeUnreleased(UnityKeyCode code)
        {
            _keycodeReleasedOnThisFrame.Remove(code);
        }

        /// <summary>
        /// 记录上一帧和当前帧发生变化的按键编码信息
        /// </summary>
        /// <param name="code">按键编码</param>
        private void OnKeycodeChanged(UnityKeyCode code)
        {
            _keycodeChangedOnPreviousFrame.Add(code);
        }

        /// <summary>
        /// 取消上一帧和当前帧发生变化的按键编码信息
        /// </summary>
        /// <param name="code">按键编码</param>
        private void OnKeycodeUnchanged(UnityKeyCode code)
        {
            _keycodeChangedOnPreviousFrame.Remove(code);
        }

        #endregion

        /// <summary>
        /// 检测当前帧是否触发了任意按键编码的录入操作，包括按下，长按及释放
        /// </summary>
        /// <returns>若触发了任意按键编码的录入操作则返回true，否则返回false</returns>
        public bool IsAnyKeycodeInputed()
        {
            if (IsAnyKeycodePressed() || IsAnyKeycodeMoved() || IsAnyKeycodeReleased())
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 检测当前帧是否触发了指定按键编码的录入操作，包括按下，长按及释放
        /// </summary>
        /// <param name="code">按键编码</param>
        /// <returns>若触发了给定按键编码的录入操作则返回true，否则返回false</returns>
        public bool IsKeycodeInputed(UnityKeyCode code)
        {
            if (IsKeycodePressed(code) || IsKeycodeMoved(code) || IsKeycodeReleased(code))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 检测当前帧是否触发了任意按键编码的按下操作
        /// </summary>
        /// <returns>若触发了任意按键编码的按下操作则返回true，否则返回false</returns>
        public bool IsAnyKeycodePressed()
        {
            return (_keycodePressedOnThisFrame.Count > 0);
        }

        /// <summary>
        /// 检测当前帧是否触发了指定按键编码的按下操作
        /// </summary>
        /// <param name="code">按键编码</param>
        /// <returns>若触发了给定按键编码的按下操作则返回true，否则返回false</returns>
        public bool IsKeycodePressed(UnityKeyCode code)
        {
            return _keycodePressedOnThisFrame.Contains(code);
        }

        /// <summary>
        /// 检测当前帧是否触发了任意按键编码的长按操作
        /// </summary>
        /// <returns>若触发了任意按键编码的长按操作则返回true，否则返回false</returns>
        public bool IsAnyKeycodeMoved()
        {
            return (_keycodeMovedOnThisFrame.Count > 0);
        }

        /// <summary>
        /// 检测当前帧是否触发了指定按键编码的长按操作
        /// </summary>
        /// <param name="code">按键编码</param>
        /// <returns>若触发了给定按键编码的长按操作则返回true，否则返回false</returns>
        public bool IsKeycodeMoved(UnityKeyCode code)
        {
            return _keycodeMovedOnThisFrame.Contains(code);
        }

        /// <summary>
        /// 检测当前帧是否触发了任意按键编码的释放操作
        /// </summary>
        /// <returns>若触发了任意按键编码的释放操作则返回true，否则返回false</returns>
        public bool IsAnyKeycodeReleased()
        {
            return (_keycodeReleasedOnThisFrame.Count > 0);
        }

        /// <summary>
        /// 检测当前帧是否触发了指定按键编码的释放操作
        /// </summary>
        /// <param name="code">按键编码</param>
        /// <returns>若触发了给定按键编码的释放操作则返回true，否则返回false</returns>
        public bool IsKeycodeReleased(UnityKeyCode code)
        {
            return _keycodeReleasedOnThisFrame.Contains(code);
        }

        /// <summary>
        /// 检测上一帧和当前帧是否触发了任意按键编码的改变操作
        /// </summary>
        /// <returns>若触发了任意按键编码的改变操作则返回true，否则返回false</returns>
        public bool IsAnyKeycodeChanged()
        {
            return (_keycodeChangedOnPreviousFrame.Count > 0);
        }

        /// <summary>
        /// 检测上一帧和当前帧是否触发了指定按键编码的改变操作
        /// </summary>
        /// <param name="code">按键编码</param>
        /// <returns>若触发了给定按键编码的改变操作则返回true，否则返回false</returns>
        public bool IsKeycodeChanged(UnityKeyCode code)
        {
            return _keycodeChangedOnPreviousFrame.Contains(code);
        }

        /// <summary>
        /// 获取当前帧下所有按下的按键编码
        /// </summary>
        /// <returns>返回按键编码列表</returns>
        public IList<UnityKeyCode> GetAllPressedKeycodes()
        {
            return _keycodePressedOnThisFrame;
        }

        /// <summary>
        /// 获取当前帧下所有长按的按键编码
        /// </summary>
        /// <returns>返回按键编码列表</returns>
        public IList<UnityKeyCode> GetAllMovedKeycodes()
        {
            return _keycodeMovedOnThisFrame;
        }

        /// <summary>
        /// 获取当前帧下所有释放的按键编码
        /// </summary>
        /// <returns>返回按键编码列表</returns>
        public IList<UnityKeyCode> GetAllReleasedKeycodes()
        {
            return _keycodeReleasedOnThisFrame;
        }

        /// <summary>
        /// 移除当前帧记录的全部按键编码信息
        /// </summary>
        private void RemoveAllInputKeycodes()
        {
            _keycodePressedOnThisFrame.Clear();
            _keycodeMovedOnThisFrame.Clear();
            _keycodeReleasedOnThisFrame.Clear();
        }
    }
}
