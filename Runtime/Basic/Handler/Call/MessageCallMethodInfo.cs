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
    /// 消息调用函数信息封装对象类
    /// </summary>
    internal sealed class MessageCallMethodInfo : BaseCallMethodInfo
    {
        /// <summary>
        /// 消息回调绑定的协议标识
        /// </summary>
        private readonly int _opcode;
        /// <summary>
        /// 消息回调绑定的协议对象类型
        /// </summary>
        private readonly SystemType _messageType;

        public int Opcode => _opcode;
        public SystemType MessageType => _messageType;

        public MessageCallMethodInfo(string fullname, SystemType targetType, SystemMethodInfo methodInfo, int opcode)
            : this(fullname, targetType, methodInfo, opcode, false)
        { }

        public MessageCallMethodInfo(string fullname, SystemType targetType, SystemMethodInfo methodInfo, int opcode, bool automatically)
            : this(fullname, targetType, methodInfo, opcode, null, automatically)
        { }

        public MessageCallMethodInfo(string fullname, SystemType targetType, SystemMethodInfo methodInfo, SystemType messageType)
            : this(fullname, targetType, methodInfo, messageType, false)
        { }

        public MessageCallMethodInfo(string fullname, SystemType targetType, SystemMethodInfo methodInfo, SystemType messageType, bool automatically)
            : this(fullname, targetType, methodInfo, 0, messageType, automatically)
        { }

        private MessageCallMethodInfo(string fullname, SystemType targetType, SystemMethodInfo methodInfo, int opcode, SystemType messageType, bool automatically)
            : base(fullname, targetType, methodInfo, automatically)
        {
            _opcode = opcode;
            _messageType = messageType;
        }

        /// <summary>
        /// 检测目标函数是否符合当前调用类型的无参格式要求
        /// </summary>
        /// <param name="methodInfo">函数对象</param>
        /// <returns>若符合无参格式则返回true，否则返回false</returns>
        protected override sealed bool CheckFunctionFormatWasNullParameterType(SystemMethodInfo methodInfo)
        {
            return Loader.Inspecting.CodeInspector.CheckFunctionFormatOfMessageCallWithNullParameterType(methodInfo);
        }

        /// <summary>
        /// 消息回调的调度函数
        /// </summary>
        /// <param name="message">消息对象实例</param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public void Invoke(object message)
        {
            if (_isNullParameterType)
            {
                _callback.DynamicInvoke();
            }
            else
            {
                _callback.DynamicInvoke(message);
            }
        }

        /// <summary>
        /// 消息回调的调度函数
        /// </summary>
        /// <param name="targetObject">对象实例</param>
        /// <param name="message">消息对象实例</param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public void Invoke(IBean targetObject, object message)
        {
            Debugger.Assert(targetObject, NovaEngine.ErrorText.InvalidArguments);

            if (_isNullParameterType)
            {
                _callback.DynamicInvoke(targetObject);
            }
            else
            {
                _callback.DynamicInvoke(targetObject, message);
            }
        }
    }
}
