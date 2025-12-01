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

using System.Reflection;
using System.Runtime.CompilerServices;

using SystemType = System.Type;

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

        #region 为静态回调函数构建委托句柄的构造函数

        public MessageCallMethodInfo(string fullname, SystemType targetType, MethodInfo methodInfo, int opcode)
            : this(fullname, targetType, methodInfo, opcode, false)
        { }

        public MessageCallMethodInfo(string fullname, SystemType targetType, MethodInfo methodInfo, int opcode, bool automatically)
            : this(fullname, targetType, methodInfo, opcode, null, automatically)
        { }

        public MessageCallMethodInfo(string fullname, SystemType targetType, MethodInfo methodInfo, SystemType messageType)
            : this(fullname, targetType, methodInfo, messageType, false)
        { }

        public MessageCallMethodInfo(string fullname, SystemType targetType, MethodInfo methodInfo, SystemType messageType, bool automatically)
            : this(fullname, targetType, methodInfo, 0, messageType, automatically)
        { }

        private MessageCallMethodInfo(string fullname, SystemType targetType, MethodInfo methodInfo, int opcode, SystemType messageType, bool automatically)
            : base(fullname, targetType, methodInfo, automatically)
        {
            _opcode = opcode;
            _messageType = messageType;
        }

        #endregion

        #region 为普通回调函数构建委托句柄的构造函数

        public MessageCallMethodInfo(IBean targetObject, string fullname, SystemType targetType, MethodInfo methodInfo, int opcode)
            : this(targetObject, fullname, targetType, methodInfo, opcode, false)
        { }

        public MessageCallMethodInfo(IBean targetObject, string fullname, SystemType targetType, MethodInfo methodInfo, int opcode, bool automatically)
            : this(targetObject, fullname, targetType, methodInfo, opcode, null, automatically)
        { }

        public MessageCallMethodInfo(IBean targetObject, string fullname, SystemType targetType, MethodInfo methodInfo, SystemType messageType)
            : this(targetObject, fullname, targetType, methodInfo, messageType, false)
        { }

        public MessageCallMethodInfo(IBean targetObject, string fullname, SystemType targetType, MethodInfo methodInfo, SystemType messageType, bool automatically)
            : this(targetObject, fullname, targetType, methodInfo, 0, messageType, automatically)
        { }

        private MessageCallMethodInfo(IBean targetObject, string fullname, SystemType targetType, MethodInfo methodInfo, int opcode, SystemType messageType, bool automatically)
            : base(targetObject, fullname, targetType, methodInfo, automatically)
        {
            _opcode = opcode;
            _messageType = messageType;
        }

        #endregion

        /// <summary>
        /// 检测目标函数是否为无效的格式类型
        /// </summary>
        /// <param name="methodInfo">函数对象</param>
        /// <returns>若为无效格式类型则返回true，否则返回false</returns>
        protected override sealed bool CheckFunctionFormatWasInvalidType(MethodInfo methodInfo)
        {
            // 函数格式校验
            if (NovaEngine.Debugger.Instance.IsOnDebuggingVerificationActivated())
            {
                bool verificated = false;
                if (NovaEngine.Utility.Reflection.IsTypeOfExtension(methodInfo))
                {
                    verificated = Loader.Inspecting.CodeInspector.CheckFunctionFormatOfMessageCallWithBeanExtensionType(methodInfo);
                }
                else
                {
                    verificated = Loader.Inspecting.CodeInspector.CheckFunctionFormatOfMessageCall(methodInfo);
                }

                // 校验失败
                if (false == verificated)
                {
                    Debugger.Error(LogGroupTag.Module, "目标对象类型‘{%t}’的‘{%s}’函数判定为非法格式的消息监听绑定回调函数，添加回调绑定操作失败！", _targetType, _fullname);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 检测目标函数是否符合当前调用类型的无参格式要求
        /// </summary>
        /// <param name="methodInfo">函数对象</param>
        /// <returns>若符合无参格式则返回true，否则返回false</returns>
        protected override sealed bool CheckFunctionFormatWasNullParameterType(MethodInfo methodInfo)
        {
            return Loader.Inspecting.CodeInspector.CheckFunctionFormatOfMessageCallWithNullParameterType(methodInfo);
        }

        /// <summary>
        /// 消息回调的调度函数
        /// </summary>
        /// <param name="message">消息对象实例</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
