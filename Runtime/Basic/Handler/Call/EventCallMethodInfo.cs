/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
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

using SystemType = System.Type;
using SystemMethodInfo = System.Reflection.MethodInfo;

namespace GameEngine
{
    /// <summary>
    /// 事件调用函数信息封装对象类
    /// </summary>
    internal sealed class EventCallMethodInfo : BaseCallMethodInfo
    {
        /// <summary>
        /// 事件回调绑定的事件标识
        /// </summary>
        private readonly int _eventID;
        /// <summary>
        /// 事件回调绑定的事件数据类型
        /// </summary>
        private readonly SystemType _eventDataType;

        public int EventID => _eventID;
        public SystemType EventDataType => _eventDataType;

        public EventCallMethodInfo(string fullname, SystemType targetType, SystemMethodInfo methodInfo, int eventID)
            : this(fullname, targetType, methodInfo, eventID, false)
        { }

        public EventCallMethodInfo(string fullname, SystemType targetType, SystemMethodInfo methodInfo, int eventID, bool automatically)
            : this(fullname, targetType, methodInfo, eventID, null, automatically)
        { }

        public EventCallMethodInfo(string fullname, SystemType targetType, SystemMethodInfo methodInfo, SystemType eventDataType)
            : this(fullname, targetType, methodInfo, eventDataType, false)
        { }

        public EventCallMethodInfo(string fullname, SystemType targetType, SystemMethodInfo methodInfo, SystemType eventDataType, bool automatically)
            : this(fullname, targetType, methodInfo, 0, eventDataType, automatically)
        { }

        private EventCallMethodInfo(string fullname, SystemType targetType, SystemMethodInfo methodInfo, int eventID, SystemType eventDataType, bool automatically)
            : base(fullname, targetType, methodInfo, automatically)
        {
            _eventID = eventID;
            _eventDataType = eventDataType;
        }

        /// <summary>
        /// 检测目标函数是否符合当前调用类型的无参格式要求
        /// </summary>
        /// <param name="methodInfo">函数对象</param>
        /// <returns>若符合无参格式则返回true，否则返回false</returns>
        protected override sealed bool CheckFunctionFormatWasNullParameterType(SystemMethodInfo methodInfo)
        {
            return Loader.Inspecting.CodeInspector.CheckFunctionFormatOfEventCallWithNullParameterType(methodInfo);
        }

        /// <summary>
        /// 事件回调的调度函数
        /// </summary>
        /// <param name="eventID">事件标识</param>
        /// <param name="args">事件数据参数</param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public void Invoke(int eventID, params object[] args)
        {
            if (_isNullParameterType)
            {
                _callback.DynamicInvoke();
            }
            else
            {
                _callback.DynamicInvoke(eventID, args);
            }
        }

        /// <summary>
        /// 事件回调的调度函数
        /// </summary>
        /// <param name="targetObject">对象实例</param>
        /// <param name="eventID">事件标识</param>
        /// <param name="args">事件数据参数</param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public void Invoke(IBean targetObject, int eventID, params object[] args)
        {
            Debugger.Assert(targetObject, NovaEngine.ErrorText.InvalidArguments);

            if (_isNullParameterType)
            {
                _callback.DynamicInvoke(targetObject);
            }
            else
            {
                _callback.DynamicInvoke(targetObject, eventID, args);
            }
        }

        /// <summary>
        /// 事件回调的调度函数
        /// </summary>
        /// <param name="eventData">事件数据</param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public void Invoke(object eventData)
        {
            if (_isNullParameterType)
            {
                _callback.DynamicInvoke();
            }
            else
            {
                _callback.DynamicInvoke(eventData);
            }
        }

        /// <summary>
        /// 事件回调的调度函数
        /// </summary>
        /// <param name="targetObject">对象实例</param>
        /// <param name="eventData">事件数据</param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public void Invoke(IBean targetObject, object eventData)
        {
            Debugger.Assert(targetObject, NovaEngine.ErrorText.InvalidArguments);

            if (_isNullParameterType)
            {
                _callback.DynamicInvoke(targetObject);
            }
            else
            {
                _callback.DynamicInvoke(targetObject, eventData);
            }
        }
    }
}
