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
using System.Reflection;
using System.Runtime.CompilerServices;

using SystemType = System.Type;

namespace GameEngine
{
    /// <summary>
    /// 视图对象抽象类，对用户界面对象上下文进行封装及调度管理
    /// </summary>
    public abstract partial class CView : CEntity
    {
        /// <summary>
        /// 基础对象内部输入编码的响应回调映射列表
        /// </summary>
        private IDictionary<ViewNoticeType, IList<string>> _noticeCalls;

        /// <summary>
        /// 视图对象的通知处理初始化函数接口
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void OnNoticeProcessingInitialize()
        {
            // 通知回调映射容器初始化
            _noticeCalls = new Dictionary<ViewNoticeType, IList<string>>();
        }

        /// <summary>
        /// 视图对象的通知处理清理函数接口
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void OnNoticeProcessingCleanup()
        {
            // 移除所有通知接口
            RemoveAllNoticeProcesses();

            _noticeCalls = null;
        }

        /// <summary>
        /// 视图对象的通知处理重载函数接口
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void OnNoticeProcessingReload()
        {
            // 移除所有通知接口
            RemoveAllNoticeProcesses();
        }

        #region 视图对象通知接口相关回调函数的操作接口定义

        /// <summary>
        /// 视图对象窗口恢复通知的接口函数
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected internal void OnResume()
        {
            if (_isResumed) return;

            _isResumed = true;
            OnInternalNoticeProcess(ViewNoticeType.Resume);
        }

        /// <summary>
        /// 视图对象窗口暂停通知的接口函数
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected internal void OnPause()
        {
            if (!_isResumed) return;

            _isResumed = false;
            OnInternalNoticeProcess(ViewNoticeType.Pause);
        }

        /// <summary>
        /// 视图对象窗口置顶通知的接口函数
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected internal void OnReveal()
        {
            if (_isRevealed) return;

            _isRevealed = true;
            OnInternalNoticeProcess(ViewNoticeType.Reveal);
        }

        /// <summary>
        /// 视图对象窗口遮挡通知的接口函数
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected internal void OnCover()
        {
            if (!_isRevealed) return;

            _isRevealed = false;
            OnInternalNoticeProcess(ViewNoticeType.Cover);
        }

        /// <summary>
        /// 视图对象的本地通知的监听回调函数
        /// </summary>
        /// <param name="noticeType">通知类型</param>
        private void OnInternalNoticeProcess(ViewNoticeType noticeType)
        {
            if (_noticeCalls.TryGetValue(noticeType, out IList<string> calls))
            {
                for (int n = 0; n < calls.Count; ++n)
                {
                    GuiHandler.Instance.InvokeViewNoticeBindingCall(calls[n], BeanType, this);
                }
            }
        }

        /// <summary>
        /// 视图对象的通知处理函数接口，将一个指定的通知绑定到给定的监听回调函数上
        /// </summary>
        /// <param name="fullname">函数名称</param>
        /// <param name="methodInfo">监听回调函数</param>
        /// <param name="noticeType">通知类型</param>
        /// <returns>若通知接口添加成功则返回true，否则返回false</returns>
        protected internal bool AddNoticeProcess(string fullname, MethodInfo methodInfo, ViewNoticeType noticeType)
        {
            GuiHandler.Instance.AddViewNoticeBindingCallInfo(fullname, BeanType, methodInfo, noticeType, true);

            if (false == _noticeCalls.TryGetValue(noticeType, out IList<string> calls))
            {
                // 创建回调列表
                calls = new List<string>();
                calls.Add(fullname);

                _noticeCalls.Add(noticeType, calls);

                return true;
            }

            if (calls.Contains(fullname))
            {
                Debugger.Warn("The '{%t}' instance's notice '{%i}' was already add same process by name '{%s}', repeat do it failed.",
                        BeanType, noticeType, fullname);
                return false;
            }

            calls.Add(fullname);

            return true;
        }

        /// <summary>
        /// 取消当前视图对象对指定通知类型的响应接口
        /// </summary>
        /// <param name="noticeType">通知类型</param>
        protected internal void RemoveNoticeProcess(ViewNoticeType noticeType)
        {
            // 若针对特定通知绑定了监听回调，则移除相应的回调句柄
            if (_noticeCalls.ContainsKey(noticeType))
            {
                _noticeCalls.Remove(noticeType);
            }
        }

        /// <summary>
        /// 取消当前视图对象对指定通知类型的响应接口
        /// </summary>
        /// <param name="fullname">函数名称</param>
        /// <param name="noticeType">通知类型</param>
        protected internal void RemoveNoticeProcess(string fullname, ViewNoticeType noticeType)
        {
            if (_noticeCalls.TryGetValue(noticeType, out IList<string> calls))
            {
                calls.Remove(fullname);

                // 当前监听列表为空时，移除该类型的监听
                if (calls.Count <= 0)
                {
                    RemoveNoticeProcess(noticeType);
                }
            }
        }

        /// <summary>
        /// 取消当前视图对象的所有通知接口
        /// </summary>
        public virtual void RemoveAllNoticeProcesses()
        {
            _noticeCalls.Clear();
        }

        #endregion
    }
}
