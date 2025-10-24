/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
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
    /// 用户界面模块封装的句柄对象类
    /// 模块具体功能接口请参考<see cref="NovaEngine.GuiModule"/>类
    /// </summary>
    public sealed partial class GuiHandler : EntityHandler
    {
        /// <summary>
        /// 视图通知回调绑定接口的缓存容器
        /// </summary>
        private IDictionary<SystemType, IDictionary<string, ViewNoticeCallMethodInfo>> _viewNoticeBindingCaches;

        /// <summary>
        /// 视图通知绑定接口初始化回调函数
        /// </summary>
        [UnityEngine.Scripting.Preserve]
        [OnSubmoduleInitCallback]
        private void OnViewNoticeBindingInitialize()
        {
            // 初始化回调绑定缓存容器
            _viewNoticeBindingCaches = new Dictionary<SystemType, IDictionary<string, ViewNoticeCallMethodInfo>>();
        }

        /// <summary>
        /// 视图通知绑定接口清理回调函数
        /// </summary>
        [UnityEngine.Scripting.Preserve]
        [OnSubmoduleCleanupCallback]
        private void OnViewNoticeBindingCleanup()
        {
            // 清理回调绑定缓存容器
            _viewNoticeBindingCaches.Clear();
            _viewNoticeBindingCaches = null;
        }

        /// <summary>
        /// 视图通知绑定接口重载回调函数
        /// </summary>
        [UnityEngine.Scripting.Preserve]
        [OnSubmoduleReloadCallback]
        private void OnViewNoticeBindingReload()
        {
            // 移除全部通知回调函数
            RemoveAllViewNoticeBindingCalls();
        }

        #region 视图对象通知接口绑定的回调函数注册/注销相关的接口函数

        /// <summary>
        /// 新增指定的回调绑定函数到当前通知接口缓存管理容器中
        /// </summary>
        /// <param name="fullname">完整名称</param>
        /// <param name="targetType">目标对象类型</param>
        /// <param name="methodInfo">函数对象</param>
        /// <param name="noticeType">通知类型</param>
        /// <param name="automatically">自动装载状态标识</param>
        internal void AddViewNoticeBindingCallInfo(string fullname, SystemType targetType, SystemMethodInfo methodInfo, ViewNoticeType noticeType, bool automatically)
        {
            if (false == _viewNoticeBindingCaches.TryGetValue(targetType, out IDictionary<string, ViewNoticeCallMethodInfo> viewNoticeCallMethodInfos))
            {
                viewNoticeCallMethodInfos = new Dictionary<string, ViewNoticeCallMethodInfo>();
                _viewNoticeBindingCaches.Add(targetType, viewNoticeCallMethodInfos);
            }

            if (viewNoticeCallMethodInfos.TryGetValue(fullname, out ViewNoticeCallMethodInfo viewNoticeCallMethodInfo))
            {
                return;
            }

            // 函数格式校验
            if (NovaEngine.Debugger.Instance.IsOnDebuggingVerificationActivated())
            {
                bool verificated = Loader.Inspecting.CodeInspector.CheckFunctionFormatOfTargetWithNullParameterType(methodInfo);

                // 校验失败
                if (false == verificated)
                {
                    Debugger.Error(LogGroupTag.Module, "目标对象类型‘{%t}’的‘{%s}’函数判定为非法格式的通知接口绑定回调函数，添加回调绑定操作失败！", targetType, fullname);
                    return;
                }
            }

            Debugger.Info(LogGroupTag.Module, "新增指定类型的‘{%i}’对应的通知接口绑定回调函数，其接口函数来自于目标类型‘{%t}’的‘{%s}’函数。",
                    noticeType, targetType, fullname);

            viewNoticeCallMethodInfo = new ViewNoticeCallMethodInfo(fullname, targetType, methodInfo, noticeType, automatically);
            viewNoticeCallMethodInfos.Add(fullname, viewNoticeCallMethodInfo);
        }

        /// <summary>
        /// 从当前通知接口缓存管理容器中移除指定标识的回调绑定函数
        /// </summary>
        /// <param name="fullname">完整名称</param>
        /// <param name="targetType">目标对象类型</param>
        internal void RemoveViewNoticeBindingCallInfo(string fullname, SystemType targetType)
        {
            Debugger.Info(LogGroupTag.Module, "移除指定的通知接口绑定回调函数，其接口函数来自于目标类型‘{%t}’的‘{%s}’函数。", targetType, fullname);

            if (_viewNoticeBindingCaches.TryGetValue(targetType, out IDictionary<string, ViewNoticeCallMethodInfo> viewNoticeCallMethodInfos))
            {
                if (viewNoticeCallMethodInfos.ContainsKey(fullname))
                {
                    viewNoticeCallMethodInfos.Remove(fullname);
                }

                if (viewNoticeCallMethodInfos.Count <= 0)
                {
                    _viewNoticeBindingCaches.Remove(targetType);
                }
            }
        }

        /// <summary>
        /// 移除当前通知接口缓存管理容器中登记的所有回调绑定函数
        /// </summary>
        private void RemoveAllViewNoticeBindingCalls()
        {
            _viewNoticeBindingCaches.Clear();
        }

        /// <summary>
        /// 针对视图通知调用指定的回调绑定函数
        /// </summary>
        /// <param name="fullname">完整名称</param>
        /// <param name="targetType">目标对象类型</param>
        /// <param name="targetObject">对象实例</param>
        internal void InvokeViewNoticeBindingCall(string fullname, SystemType targetType, IBean targetObject)
        {
            ViewNoticeCallMethodInfo viewNoticeCallMethodInfo = FindViewNoticeBindingCallByName(fullname, targetType);
            if (null == viewNoticeCallMethodInfo)
            {
                Debugger.Warn(LogGroupTag.Module, "当前的通知接口缓存管理容器中无法检索到指定类型‘{%t}’及名称‘{%s}’对应的回调绑定函数，此次转发通知调度失败！", targetType, fullname);
                return;
            }

            viewNoticeCallMethodInfo.Invoke(targetObject);
        }

        /// <summary>
        /// 通过指定的名称及对象类型，在当前的缓存容器中查找对应的回调绑定函数
        /// </summary>
        /// <param name="fullname">完整名称</param>
        /// <param name="targetType">目标对象类型</param>
        /// <returns>返回绑定函数实例</returns>
        private ViewNoticeCallMethodInfo FindViewNoticeBindingCallByName(string fullname, SystemType targetType)
        {
            if (_viewNoticeBindingCaches.TryGetValue(targetType, out IDictionary<string, ViewNoticeCallMethodInfo> viewNoticeCallMethodInfos))
            {
                if (viewNoticeCallMethodInfos.TryGetValue(fullname, out ViewNoticeCallMethodInfo viewNoticeCallMethodInfo))
                {
                    return viewNoticeCallMethodInfo;
                }
            }

            return null;
        }

        #endregion
    }
}
