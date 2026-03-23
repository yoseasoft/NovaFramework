/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
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
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Scripting;

namespace GameEngine
{
    /// 输入模块封装的句柄对象类
    public sealed partial class InputHandler
    {
        /// <summary>
        /// 输入虚拟按键映射管理容器
        /// </summary>
        private IDictionary<UnityEngine.KeyCode, VirtualKeyCode> _inputVirtualMapTables;

        /// <summary>
        /// 输入虚拟映射初始化回调函数
        /// </summary>
        [Preserve]
        [OnSubmoduleInitCallback]
        private void OnInputVirtualMappingInitialize()
        {
            // 初始化虚拟映射管理容器
            _inputVirtualMapTables = new Dictionary<UnityEngine.KeyCode, VirtualKeyCode>();

#if UNITY_2018_1_OR_NEWER
            InitVirtualMappingTables();
#endif
        }

        /// <summary>
        /// 输入虚拟映射清理回调函数
        /// </summary>
        [Preserve]
        [OnSubmoduleCleanupCallback]
        private void OnInputVirtualMappingCleanup()
        {
            // 清理虚拟映射管理容器
            _inputVirtualMapTables.Clear();
            _inputVirtualMapTables = null;
        }

        /// <summary>
        /// 输入虚拟映射重载回调函数
        /// </summary>
        [Preserve]
        [OnSubmoduleReloadCallback]
        private void OnInputVirtualMappingReload()
        {
        }

        #region 输入按键的虚拟映射注册、绑定相关接口函数

        /// <summary>
        /// 初始化的虚拟按键映射表
        /// </summary>
        private void InitVirtualMappingTables()
        {
            Array all_keycodes = Enum.GetValues(typeof(UnityEngine.KeyCode));
            IEnumerator keycode_enumerator = all_keycodes.GetEnumerator();
            while (keycode_enumerator.MoveNext())
            {
                UnityEngine.KeyCode keyCode = (UnityEngine.KeyCode) keycode_enumerator.Current;
                if (false == Enum.TryParse(keyCode.ToString(), out VirtualKeyCode virtualKeyCode))
                {
                    _inputVirtualMapTables.Add(keyCode, virtualKeyCode);
                }
            }
        }

        /// <summary>
        /// 通过指定的输入按键获取对应的虚拟按键
        /// </summary>
        /// <param name="keyCode">按键编码</param>
        /// <returns>返回对应的虚拟按键，若查找失败则返回空值</returns>
        private VirtualKeyCode GetVirtualKey(UnityEngine.KeyCode keyCode)
        {
            return _inputVirtualMapTables.TryGetValue(keyCode, out VirtualKeyCode virtualKeyCode) ? virtualKeyCode : VirtualKeyCode.None;
        }

        /// <summary>
        /// 设置指定的输入按键对应的虚拟按键
        /// </summary>
        /// <param name="keyCode">按键编码</param>
        /// <param name="virtualKeyCode">虚拟按键码</param>
        public void SetVirtualKey(UnityEngine.KeyCode keyCode, VirtualKeyCode virtualKeyCode)
        {
            if (_inputVirtualMapTables.ContainsKey(keyCode))
            {
                _inputVirtualMapTables[keyCode] = virtualKeyCode;
            }
            else
            {
                _inputVirtualMapTables.Add(keyCode, virtualKeyCode);
            }
        }

        #endregion
    }
}
