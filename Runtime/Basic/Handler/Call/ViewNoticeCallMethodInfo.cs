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

using SystemType = System.Type;
using SystemMethodInfo = System.Reflection.MethodInfo;

namespace GameEngine
{
    /// <summary>
    /// 视图通知调用函数信息封装对象类
    /// </summary>
    internal sealed class ViewNoticeCallMethodInfo : BaseCallMethodInfo
    {
        /// <summary>
        /// 视图通知类型
        /// </summary>
        private readonly ViewNoticeType _noticeType;

        public ViewNoticeType NoticeType => _noticeType;

        public ViewNoticeCallMethodInfo(string fullname, SystemType targetType, SystemMethodInfo methodInfo, ViewNoticeType noticeType)
            : this(fullname, targetType, methodInfo, noticeType, false)
        { }

        public ViewNoticeCallMethodInfo(string fullname, SystemType targetType, SystemMethodInfo methodInfo, ViewNoticeType noticeType, bool automatically)
            : base(fullname, targetType, methodInfo, automatically)
        {
            _noticeType = noticeType;
        }

        /// <summary>
        /// 通知回调的调度函数
        /// </summary>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public void Invoke()
        {
            // _callback.DynamicInvoke();

            Debugger.Throw<System.NotImplementedException>();
        }

        /// <summary>
        /// 通知回调的调度函数
        /// </summary>
        /// <param name="targetObject">对象实例</param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public void Invoke(IBean targetObject)
        {
            Debugger.Assert(targetObject, "Invalid arguments.");

            _callback.DynamicInvoke(targetObject);
        }
    }
}
