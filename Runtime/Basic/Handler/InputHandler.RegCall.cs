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
        /// 输入响应回调绑定接口的缓存容器
        /// </summary>
        private IDictionary<Type, IDictionary<string, InputCallMethodInfo>> _inputResponseBindingCaches;

        /// <summary>
        /// 输入响应绑定接口初始化回调函数
        /// </summary>
        [Preserve]
        [OnSubmoduleInitCallback]
        private void OnInputResponseBindingInitialize()
        {
            // 初始化回调绑定缓存容器
            _inputResponseBindingCaches = new Dictionary<Type, IDictionary<string, InputCallMethodInfo>>();
        }

        /// <summary>
        /// 输入响应绑定接口清理回调函数
        /// </summary>
        [Preserve]
        [OnSubmoduleCleanupCallback]
        private void OnInputResponseBindingCleanup()
        {
            // 清理回调绑定缓存容器
            _inputResponseBindingCaches.Clear();
            _inputResponseBindingCaches = null;
        }

        /// <summary>
        /// 输入响应绑定接口重载回调函数
        /// </summary>
        [Preserve]
        [OnSubmoduleReloadCallback]
        private void OnInputResponseBindingReload()
        {
            // 移除全部输入响应回调函数
            RemoveAllInputResponseBindingCalls();
        }

        #region 实体对象输入响应绑定的回调函数注册/注销相关的接口函数

        /// <summary>
        /// 新增指定的回调绑定函数到当前输入响应缓存管理容器中
        /// </summary>
        /// <param name="fullname">完整名称</param>
        /// <param name="targetType">目标对象类型</param>
        /// <param name="methodInfo">函数对象</param>
        /// <param name="inputCode">按键编码</param>
        /// <param name="operationType">操作类型</param>
        /// <param name="automatically">自动装载状态标识</param>
        internal void AddInputResponseBindingCallInfo(string fullname, Type targetType, MethodInfo methodInfo, int inputCode, int operationType, bool automatically)
        {
            if (false == _inputResponseBindingCaches.TryGetValue(targetType, out IDictionary<string, InputCallMethodInfo> inputCallMethodInfos))
            {
                inputCallMethodInfos = new Dictionary<string, InputCallMethodInfo>();
                _inputResponseBindingCaches.Add(targetType, inputCallMethodInfos);
            }

            if (inputCallMethodInfos.ContainsKey(fullname))
            {
                return;
            }

            Debugger.Info(LogGroupTag.Module, "新增指定的按键编码‘{%d}’及操作类型‘{%d}’对应的输入响应绑定回调函数，其响应接口函数来自于目标类型‘{%t}’的‘{%s}’函数。",
                    inputCode, operationType, targetType, fullname);

            InputCallMethodInfo inputCallMethodInfo = new InputCallMethodInfo(fullname, targetType, methodInfo, inputCode, operationType, automatically);
            inputCallMethodInfos.Add(fullname, inputCallMethodInfo);
        }

        /// <summary>
        /// 新增指定的回调绑定函数到当前输入响应缓存管理容器中
        /// </summary>
        /// <param name="fullname">完整名称</param>
        /// <param name="targetType">目标对象类型</param>
        /// <param name="methodInfo">函数对象</param>
        /// <param name="inputDataType">输入数据类型</param>
        /// <param name="automatically">自动装载状态标识</param>
        internal void AddInputResponseBindingCallInfo(string fullname, Type targetType, MethodInfo methodInfo, Type inputDataType, bool automatically)
        {
            if (false == _inputResponseBindingCaches.TryGetValue(targetType, out IDictionary<string, InputCallMethodInfo> inputCallMethodInfos))
            {
                inputCallMethodInfos = new Dictionary<string, InputCallMethodInfo>();
                _inputResponseBindingCaches.Add(targetType, inputCallMethodInfos);
            }

            if (inputCallMethodInfos.ContainsKey(fullname))
            {
                return;
            }

            Debugger.Info(LogGroupTag.Module, "新增指定的按键编码的数据类型‘{%t}’对应的输入响应绑定回调函数，其响应接口函数来自于目标类型‘{%t}’的‘{%s}’函数。",
                    inputDataType, targetType, fullname);

            InputCallMethodInfo inputCallMethodInfo = new InputCallMethodInfo(fullname, targetType, methodInfo, inputDataType, automatically);
            inputCallMethodInfos.Add(fullname, inputCallMethodInfo);
        }

        /// <summary>
        /// 从当前输入响应缓存管理容器中移除指定标识的回调绑定函数
        /// </summary>
        /// <param name="fullname">完整名称</param>
        /// <param name="targetType">目标对象类型</param>
        internal void RemoveInputResponseBindingCallInfo(string fullname, Type targetType)
        {
            Debugger.Info(LogGroupTag.Module, "移除指定的输入响应绑定回调函数，其响应接口函数来自于目标类型‘{%t}’的‘{%s}’函数。", targetType, fullname);

            if (_inputResponseBindingCaches.TryGetValue(targetType, out IDictionary<string, InputCallMethodInfo> inputCallMethodInfos))
            {
                if (inputCallMethodInfos.ContainsKey(fullname))
                {
                    inputCallMethodInfos.Remove(fullname);
                }

                if (inputCallMethodInfos.Count <= 0)
                {
                    _inputResponseBindingCaches.Remove(targetType);
                }
            }
        }

        /// <summary>
        /// 移除当前输入响应缓存管理容器中登记的所有回调绑定函数
        /// </summary>
        private void RemoveAllInputResponseBindingCalls()
        {
            _inputResponseBindingCaches.Clear();
        }

        /// <summary>
        /// 针对按键编码调用指定的回调绑定函数
        /// </summary>
        /// <param name="targetObject">对象实例</param>
        /// <param name="fullname">完整名称</param>
        /// <param name="targetType">目标对象类型</param>
        /// <param name="inputCode">按键编码</param>
        /// <param name="operationType">按键操作类型</param>
        internal void InvokeInputResponseBindingCall(IBean targetObject, string fullname, Type targetType, int inputCode, int operationType)
        {
            InputCallMethodInfo inputCallMethodInfo = FindInputResponseBindingCallByName(fullname, targetType);
            if (null == inputCallMethodInfo)
            {
                Debugger.Warn(LogGroupTag.Module, "当前的输入响应缓存管理容器中无法检索到指定类型‘{%t}’及名称‘{%s}’对应的回调绑定函数，此次按键编码‘{%d}’转发通知失败！", targetType, fullname, inputCode);
                return;
            }

            inputCallMethodInfo.Invoke(targetObject, inputCode, operationType);
        }

        /// <summary>
        /// 针对输入数据调用指定的回调绑定函数
        /// </summary>
        /// <param name="targetObject">对象实例</param>
        /// <param name="fullname">完整名称</param>
        /// <param name="targetType">目标对象类型</param>
        /// <param name="inputData">输入数据</param>
        internal void InvokeInputResponseBindingCall(IBean targetObject, string fullname, Type targetType, object inputData)
        {
            InputCallMethodInfo inputCallMethodInfo = FindInputResponseBindingCallByName(fullname, targetType);
            if (null == inputCallMethodInfo)
            {
                Debugger.Warn(LogGroupTag.Module, "当前的输入响应缓存管理容器中无法检索到指定类型‘{%t}’及名称‘{%s}’对应的回调绑定函数，此次输入数据‘{%t}’转发通知失败！", targetType, fullname, inputData);
                return;
            }

            inputCallMethodInfo.Invoke(targetObject, inputData);
        }

        /// <summary>
        /// 通过指定的名称及对象类型，在当前的缓存容器中查找对应的回调绑定函数
        /// </summary>
        /// <param name="fullname">完整名称</param>
        /// <param name="targetType">目标对象类型</param>
        /// <returns>返回绑定函数实例</returns>
        private InputCallMethodInfo FindInputResponseBindingCallByName(string fullname, Type targetType)
        {
            if (_inputResponseBindingCaches.TryGetValue(targetType, out IDictionary<string, InputCallMethodInfo> inputCallMethodInfos))
            {
                if (inputCallMethodInfos.TryGetValue(fullname, out InputCallMethodInfo inputCallMethodInfo))
                {
                    return inputCallMethodInfo;
                }
            }

            return null;
        }

        #endregion
    }
}
