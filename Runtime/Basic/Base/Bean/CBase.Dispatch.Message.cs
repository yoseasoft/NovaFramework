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

using System.Collections.Generic;

using SystemType = System.Type;
using SystemDelegate = System.Delegate;
using SystemMethodInfo = System.Reflection.MethodInfo;

namespace GameEngine
{
    /// <summary>
    /// 基础对象抽象类，对需要进行对象定义的场景提供一个通用的基类
    /// </summary>
    public abstract partial class CBase
    {
        /// <summary>
        /// 基础对象内部网络消息的监听回调映射列表
        /// </summary>
        private IDictionary<int, IDictionary<string, bool>> _messageListenerCallForType;

        /// <summary>
        /// 基础对象的消息监听回调初始化函数接口
        /// </summary>
        private void OnMessageListenerCallInitialize()
        {
            // 消息监听回调映射容器初始化
            _messageListenerCallForType = new Dictionary<int, IDictionary<string, bool>>();
        }

        /// <summary>
        /// 基础对象的消息监听回调清理函数接口
        /// </summary>
        private void OnMessageListenerCallCleanup()
        {
            // 移除所有消息通知
            RemoveAllMessageListeners();

            _messageListenerCallForType = null;
        }

        #region 基础对象消息监听相关回调函数的操作接口定义

        /// <summary>
        /// 基础对象的消息通知的监听回调函数<br/>
        /// 该函数针对消息转发接口的标准实现，禁止子类重写该函数<br/>
        /// 若子类需要根据需要自行处理消息，可以通过重写<see cref="GameEngine.CBase.OnMessage(ProtoBuf.Extension.IMessage)"/>实现消息的自定义处理逻辑
        /// </summary>
        /// <param name="opcode">协议操作码</param>
        /// <param name="message">消息对象实例</param>
        public virtual void OnMessageDispatch(int opcode, ProtoBuf.Extension.IMessage message)
        {
            if (_messageListenerCallForType.TryGetValue(opcode, out IDictionary<string, bool> calls))
            {
                SystemType classType = GetType();
                foreach (KeyValuePair<string, bool> kvp in calls)
                {
                    NetworkHandler.Instance.InvokeMessageListenerBindingCall(kvp.Key, classType, this, message);
                }
            }

            OnMessage(opcode, message);
        }

        /// <summary>
        /// 用户自定义的消息处理函数，您可以通过重写该函数处理自定义消息通知
        /// </summary>
        /// <param name="opcode">协议操作码</param>
        /// <param name="message">消息对象实例</param>
        protected abstract void OnMessage(int opcode, ProtoBuf.Extension.IMessage message);

        /// <summary>
        /// 针对指定消息标识新增消息监听的后处理程序
        /// </summary>
        /// <param name="opcode">协议操作码</param>
        /// <returns>返回后处理的操作结果</returns>
        protected abstract bool OnMessageListenerAddedActionPostProcess(int opcode);
        /// <summary>
        /// 针对指定消息标识移除消息监听的后处理程序
        /// </summary>
        /// <param name="opcode">协议操作码</param>
        protected abstract void OnMessageListenerRemovedActionPostProcess(int opcode);

        /// <summary>
        /// 检测当前基础对象是否监听了目标消息类型
        /// </summary>
        /// <param name="opcode">协议操作码</param>
        /// <returns>若监听了给定消息类型则返回true，否则返回false</returns>
        protected internal virtual bool IsMessageListenedOfTargetType(int opcode)
        {
            if (_messageListenerCallForType.ContainsKey(opcode) && _messageListenerCallForType[opcode].Count > 0)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 基础对象的消息监听函数接口，对一个指定的消息进行转发监听
        /// </summary>
        /// <param name="opcode">协议操作码</param>
        /// <returns>若消息监听成功则返回true，否则返回false</returns>
        protected internal virtual bool AddMessageListener(int opcode)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// 基础对象的消息监听函数接口，将一个指定的协议码绑定到给定的监听回调函数上
        /// </summary>
        /// <param name="fullname">函数名称</param>
        /// <param name="methodInfo">监听回调函数</param>
        /// <param name="opcode">协议操作码</param>
        /// <returns>若消息监听成功则返回true，否则返回false</returns>
        protected internal bool AddMessageListener(string fullname, SystemMethodInfo methodInfo, int opcode)
        {
            return AddMessageListener(fullname, methodInfo, opcode, false);
        }

        /// <summary>
        /// 基础对象的消息监听函数接口，将一个指定的协议码绑定到给定的监听回调函数上
        /// </summary>
        /// <param name="fullname">函数名称</param>
        /// <param name="methodInfo">监听回调函数</param>
        /// <param name="opcode">协议操作码</param>
        /// <param name="automatically">自动装载状态标识</param>
        /// <returns>若消息监听成功则返回true，否则返回false</returns>
        protected internal bool AddMessageListener(string fullname, SystemMethodInfo methodInfo, int opcode, bool automatically)
        {
            NetworkHandler.Instance.AddMessageListenerBindingCallInfo(fullname, GetType(), methodInfo, opcode, automatically);

            if (false == _messageListenerCallForType.TryGetValue(opcode, out IDictionary<string, bool> calls))
            {
                // 创建回调列表
                calls = new Dictionary<string, bool>();
                calls.Add(fullname, automatically);

                _messageListenerCallForType.Add(opcode, calls);

                // 新增消息监听的后处理程序
                return OnMessageListenerAddedActionPostProcess(opcode);
            }

            if (calls.ContainsKey(fullname))
            {
                Debugger.Warn("The '{0}' instance's message type '{1}' was already add same listener by name '{2}', repeat added it failed.",
                    NovaEngine.Utility.Text.ToString(GetType()), opcode, fullname);
                return false;
            }

            calls.Add(fullname, automatically);

            return true;
        }

        /// <summary>
        /// 基础对象的消息监听函数接口，将一个指定的消息类型绑定到给定的监听回调函数上
        /// </summary>
        /// <typeparam name="T">消息类型</typeparam>
        /// <returns>若消息监听成功则返回true，否则返回false</returns>
        public bool AddMessageListener<T>() where T : ProtoBuf.Extension.IMessage
        {
            return AddMessageListener(typeof(T));
        }

        /// <summary>
        /// 基础对象的消息监听函数接口，将一个指定的消息类型绑定到给定的监听回调函数上
        /// </summary>
        /// <param name="messageType">消息类型</param>
        /// <returns>若消息监听成功则返回true，否则返回false</returns>
        public bool AddMessageListener(SystemType messageType)
        {
            int opcode = NetworkHandler.Instance.GetOpcodeByMessageType(messageType);

            return AddMessageListener(opcode);
        }

        /// <summary>
        /// 基础对象的消息监听函数接口，将一个指定的消息类型绑定到给定的监听回调函数上
        /// </summary>
        /// <typeparam name="T">消息类型</typeparam>
        /// <param name="func">监听回调函数</param>
        /// <returns>若消息监听成功则返回true，否则返回false</returns>
        public bool AddMessageListener<T>(System.Action<T> func) where T : ProtoBuf.Extension.IMessage
        {
            string fullname = NovaEngine.Utility.Text.GetFullName(func.Method);
            return AddMessageListener(fullname, func.Method, typeof(T));
        }

        /// <summary>
        /// 基础对象的消息监听函数接口，将一个指定的消息类型绑定到给定的监听回调函数上
        /// </summary>
        /// <param name="fullname">函数名称</param>
        /// <param name="methodInfo">监听回调函数</param>
        /// <param name="messageType">消息类型</param>
        /// <returns>若消息监听成功则返回true，否则返回false</returns>
        public bool AddMessageListener(string fullname, SystemMethodInfo methodInfo, SystemType messageType)
        {
            int opcode = NetworkHandler.Instance.GetOpcodeByMessageType(messageType);

            return AddMessageListener(fullname, methodInfo, opcode);
        }

        /// <summary>
        /// 基础对象的消息监听函数接口，将一个指定的消息类型绑定到给定的监听回调函数上
        /// </summary>
        /// <param name="fullname">函数名称</param>
        /// <param name="methodInfo">监听回调函数</param>
        /// <param name="messageType">消息类型</param>
        /// <param name="automatically">自动装载状态标识</param>
        /// <returns>若消息监听成功则返回true，否则返回false</returns>
        protected internal bool AddMessageListener(string fullname, SystemMethodInfo methodInfo, SystemType messageType, bool automatically)
        {
            int opcode = NetworkHandler.Instance.GetOpcodeByMessageType(messageType);

            return AddMessageListener(fullname, methodInfo, opcode, automatically);
        }

        /// <summary>
        /// 取消当前基础对象对指定协议码的监听回调
        /// </summary>
        /// <param name="opcode">协议操作码</param>
        public virtual void RemoveMessageListener(int opcode)
        {
            // 若针对特定消息绑定了监听回调，则移除相应的回调句柄
            if (_messageListenerCallForType.ContainsKey(opcode))
            {
                _messageListenerCallForType.Remove(opcode);
            }

            // 移除消息监听的后处理程序
            OnMessageListenerRemovedActionPostProcess(opcode);
        }

        /// <summary>
        /// 取消当前基础对象对指定协议码的监听回调
        /// </summary>
        /// <param name="methodInfo">监听回调函数</param>
        /// <param name="opcode">协议操作码</param>
        protected internal void RemoveMessageListener(SystemMethodInfo methodInfo, int opcode)
        {
            string fullname = _Generator.GenUniqueName(methodInfo);

            RemoveMessageListener(fullname, opcode);
        }

        /// <summary>
        /// 取消当前基础对象对指定协议码的监听回调
        /// </summary>
        /// <param name="opcode">协议操作码</param>
        /// <param name="fullname">函数名称</param>
        protected internal void RemoveMessageListener(string fullname, int opcode)
        {
            if (_messageListenerCallForType.TryGetValue(opcode, out IDictionary<string, bool> calls))
            {
                if (calls.ContainsKey(fullname))
                {
                    calls.Remove(fullname);
                }
            }

            // 当前监听列表为空时，移除该消息的监听
            if (false == IsMessageListenedOfTargetType(opcode))
            {
                RemoveMessageListener(opcode);
            }
        }

        /// <summary>
        /// 取消当前基础对象对指定消息类型的监听回调
        /// </summary>
        /// <typeparam name="T">消息类型</typeparam>
        public void RemoveMessageListener<T>()
        {
            RemoveMessageListener(typeof(T));
        }

        /// <summary>
        /// 取消当前基础对象对指定消息类型的监听回调
        /// </summary>
        /// <param name="messageType">消息类型</param>
        public void RemoveMessageListener(SystemType messageType)
        {
            int opcode = NetworkHandler.Instance.GetOpcodeByMessageType(messageType);

            RemoveMessageListener(opcode);
        }

        /// <summary>
        /// 取消当前基础对象对指定消息类型的监听回调
        /// </summary>
        /// <typeparam name="T">消息类型</typeparam>
        /// <param name="methodInfo">监听回调函数</param>
        public void RemoveMessageListener<T>(SystemMethodInfo methodInfo)
        {
            RemoveMessageListener(methodInfo, typeof(T));
        }

        /// <summary>
        /// 取消当前基础对象对指定消息类型的监听回调
        /// </summary>
        /// <param name="methodInfo">监听回调函数</param>
        /// <param name="messageType">消息类型</param>
        public void RemoveMessageListener(SystemMethodInfo methodInfo, SystemType messageType)
        {
            int opcode = NetworkHandler.Instance.GetOpcodeByMessageType(messageType);

            RemoveMessageListener(methodInfo, opcode);
        }

        /// <summary>
        /// 取消当前基础对象对指定消息类型的监听回调
        /// </summary>
        /// <typeparam name="T">消息类型</typeparam>
        /// <param name="fullname">函数名称</param>
        protected internal void RemoveMessageListener<T>(string fullname)
        {
            RemoveMessageListener(fullname, typeof(T));
        }

        /// <summary>
        /// 取消当前基础对象对指定消息类型的监听回调
        /// </summary>
        /// <param name="fullname">函数名称</param>
        /// <param name="messageType">消息类型</param>
        protected internal void RemoveMessageListener(string fullname, SystemType messageType)
        {
            int opcode = NetworkHandler.Instance.GetOpcodeByMessageType(messageType);

            RemoveMessageListener(fullname, opcode);
        }

        /// <summary>
        /// 移除所有自动注册的消息监听回调接口
        /// </summary>
        protected internal void RemoveAllAutomaticallyMessageListeners()
        {
            OnAutomaticallyCallSyntaxInfoProcessHandler<int>(_messageListenerCallForType, RemoveMessageListener);
        }

        /// <summary>
        /// 取消当前基础对象的所有注册的消息监听回调
        /// </summary>
        public virtual void RemoveAllMessageListeners()
        {
            IList<int> id_keys = NovaEngine.Utility.Collection.ToListForKeys<int, IDictionary<string, bool>>(_messageListenerCallForType);
            if (null != id_keys)
            {
                int c = id_keys.Count;
                for (int n = 0; n < c; ++n) { RemoveMessageListener(id_keys[n]); }
            }

            _messageListenerCallForType.Clear();
        }

        #endregion
    }
}
