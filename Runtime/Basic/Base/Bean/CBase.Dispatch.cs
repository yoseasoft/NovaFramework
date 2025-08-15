/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyright (C) 2023 - 2024, Guangzhou Shiyue Network Technology Co., Ltd.
/// Copyright (C) 2024 - 2025, Hurley, Independent Studio.
/// Copyright (C) 2025, Hainan Yuanyou Information Tecdhnology Co., Ltd. Guangzhou Branch
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
        /// 基础对象内部输入编码的监听回调容器列表
        /// </summary>
        private IDictionary<int, IDictionary<string, InputCallSyntaxInfo>> _inputCallInfosForCode;
        /// <summary>
        /// 基础对象内部输入编码的监听回调容器列表
        /// </summary>
        private IDictionary<SystemType, IDictionary<string, InputCallSyntaxInfo>> _inputCallInfosForType;

        /// <summary>
        /// 基础对象内部订阅事件的监听回调容器列表
        /// </summary>
        private IDictionary<int, IDictionary<string, EventCallSyntaxInfo>> _eventCallInfosForId;
        /// <summary>
        /// 基础对象内部订阅事件的监听回调容器列表
        /// </summary>
        private IDictionary<SystemType, IDictionary<string, EventCallSyntaxInfo>> _eventCallInfosForType;

        /// <summary>
        /// 基础对象内部消息通知的监听回调的映射容器列表
        /// </summary>
        private IDictionary<int, IDictionary<string, MessageCallSyntaxInfo>> _messageCallInfosForType;

        /// <summary>
        /// 基础对象的转发回调初始化函数接口
        /// </summary>
        private void OnDispatchCallInitialize()
        {
            // 输入监听回调映射容器初始化
            _inputCallInfosForCode = new Dictionary<int, IDictionary<string, InputCallSyntaxInfo>>();
            _inputCallInfosForType = new Dictionary<SystemType, IDictionary<string, InputCallSyntaxInfo>>();

            // 事件监听回调映射容器初始化
            _eventCallInfosForId = new Dictionary<int, IDictionary<string, EventCallSyntaxInfo>>();
            _eventCallInfosForType = new Dictionary<SystemType, IDictionary<string, EventCallSyntaxInfo>>();

            // 消息监听回调映射容器初始化
            _messageCallInfosForType = new Dictionary<int, IDictionary<string, MessageCallSyntaxInfo>>();
        }

        /// <summary>
        /// 基础对象的转发回调清理函数接口
        /// </summary>
        private void OnDispatchCallCleanup()
        {
            // 移除所有输入响应
            RemoveAllInputResponses();
            _inputCallInfosForCode = null;
            _inputCallInfosForType = null;

            // 移除所有订阅事件
            UnsubscribeAllEvents();
            _eventCallInfosForId = null;
            _eventCallInfosForType = null;

            // 移除所有消息通知
            RemoveAllMessageListeners();
            _messageCallInfosForType = null;
        }

        #region 基础对象输入响应相关操作函数合集

        /// <summary>
        /// 基础对象的输入编码的监听回调函数<br/>
        /// 该函数针对输入响应接口的标准实现，禁止子类重写该函数<br/>
        /// 若子类需要根据需要自行解析输入编码，可以通过重写<see cref="GameEngine.CBase.OnInput(int, int)"/>实现输入编码的自定义处理逻辑
        /// </summary>
        /// <param name="inputCode">输入编码</param>
        /// <param name="operationType">输入操作类型</param>
        public virtual void OnInputDispatchForCode(int inputCode, int operationType)
        {
            if (_inputCallInfosForCode.TryGetValue(inputCode, out IDictionary<string, InputCallSyntaxInfo> infos))
            {
                IEnumerator<InputCallSyntaxInfo> e = infos.Values.GetEnumerator();
                while (e.MoveNext())
                {
                    e.Current.Invoke(inputCode, operationType);
                }
            }

            OnInput(inputCode, operationType);
        }

        /// <summary>
        /// 基础对象的输入编码的监听回调函数<br/>
        /// 该函数针对输入响应接口的标准实现，禁止子类重写该函数<br/>
        /// 若子类需要根据需要自行解析输入编码，可以通过重写<see cref="GameEngine.CBase.OnInput(object)"/>实现输入编码的自定义处理逻辑
        /// </summary>
        /// <param name="inputData">事件数据</param>
        public virtual void OnInputDispatchForType(object inputData)
        {
            if (_inputCallInfosForType.TryGetValue(inputData.GetType(), out IDictionary<string, InputCallSyntaxInfo> infos))
            {
                IEnumerator<InputCallSyntaxInfo> e = infos.Values.GetEnumerator();
                while (e.MoveNext())
                {
                    e.Current.Invoke(inputData);
                }
            }

            OnInput(inputData);
        }

        /// <summary>
        /// 用户自定义的输入处理函数，您可以通过重写该函数处理自定义输入行为
        /// </summary>
        /// <param name="inputCode">事件标识</param>
        /// <param name="operationType">事件数据参数</param>
        protected abstract void OnInput(int inputCode, int operationType);

        /// <summary>
        /// 用户自定义的输入处理函数，您可以通过重写该函数处理自定义输入行为
        /// </summary>
        /// <param name="inputData">事件数据</param>
        protected abstract void OnInput(object inputData);

        /// <summary>
        /// 针对指定编码标识新增输入监听的后处理程序
        /// </summary>
        /// <param name="inputCode">输入编码</param>
        /// <param name="operationType">事件数据参数</param>
        /// <returns>返回后处理的操作结果</returns>
        protected abstract bool OnInputResponseAddedActionPostProcess(int inputCode, int operationType);
        /// <summary>
        /// 针对指定编码类型新增输入监听的后处理程序
        /// </summary>
        /// <param name="inputType">输入类型</param>
        /// <returns>返回后处理的操作结果</returns>
        protected abstract bool OnInputResponseAddedActionPostProcess(SystemType inputType);
        /// <summary>
        /// 针对指定编码标识移除输入监听的后处理程序
        /// </summary>
        /// <param name="inputCode">输入编码</param>
        /// <param name="operationType">事件数据参数</param>
        protected abstract void OnInputResponseRemovedActionPostProcess(int inputCode, int operationType);
        /// <summary>
        /// 针对指定编码类型移除输入监听的后处理程序
        /// </summary>
        /// <param name="inputType">输入类型</param>
        protected abstract void OnInputResponseRemovedActionPostProcess(SystemType inputType);

        /// <summary>
        /// 检测当前基础对象是否响应了目标输入标识
        /// </summary>
        /// <param name="inputCode">输入编码</param>
        /// <param name="operationType">输入操作类型</param>
        /// <returns>若响应了给定编码标识则返回true，否则返回false</returns>
        protected internal virtual bool IsInputResponsedOfTargetCode(int inputCode, int operationType)
        {
            if (_inputCallInfosForCode.ContainsKey(inputCode) && _inputCallInfosForCode[inputCode].Count > 0)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 检测当前基础对象是否响应了目标输入类型
        /// </summary>
        /// <param name="inputType">输入类型</param>
        /// <returns>若响应了给定输入类型则返回true，否则返回false</returns>
        protected internal virtual bool IsInputResponsedOfTargetType(SystemType inputType)
        {
            if (_inputCallInfosForType.ContainsKey(inputType) && _inputCallInfosForType[inputType].Count > 0)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 基础对象的输入响应函数接口，对一个指定的编码进行响应监听
        /// </summary>
        /// <param name="inputCode">输入编码</param>
        /// <param name="operationType">输入操作类型</param>
        /// <returns>若输入响应成功则返回true，否则返回false</returns>
        public virtual bool AddInputResponse(int inputCode, int operationType)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// 基础对象的输入响应函数接口，将一个指定的输入绑定到给定的监听回调函数上
        /// </summary>
        /// <param name="inputCode">输入编码</param>
        /// <param name="operationType">输入操作类型</param>
        /// <param name="methodInfo">监听回调函数</param>
        /// <returns>若输入响应成功则返回true，否则返回false</returns>
        public bool AddInputResponse(int inputCode, int operationType, SystemMethodInfo methodInfo)
        {
            return AddInputResponse(inputCode, operationType, methodInfo, false);
        }

        /// <summary>
        /// 基础对象的输入响应函数接口，将一个指定的输入绑定到给定的监听回调函数上
        /// </summary>
        /// <param name="inputCode">输入编码</param>
        /// <param name="operationType">输入操作类型</param>
        /// <param name="methodInfo">监听回调函数</param>
        /// <param name="automatically">自动装载状态标识</param>
        /// <returns>若输入响应成功则返回true，否则返回false</returns>
        protected internal bool AddInputResponse(int inputCode, int operationType, SystemMethodInfo methodInfo, bool automatically)
        {
            // 函数格式校验
            if (NovaEngine.Debugger.Instance.IsOnDebuggingVerificationActivated())
            {
                bool verificated = false;
                if (NovaEngine.Utility.Reflection.IsTypeOfExtension(methodInfo))
                {
                    verificated = Loader.Inspecting.CodeInspector.IsValidFormatOfBeanExtendInputCallFunction(methodInfo);
                }
                else
                {
                    verificated = Loader.Inspecting.CodeInspector.IsValidFormatOfInputCallFunction(methodInfo);
                }

                // 校验失败
                if (false == verificated)
                {
                    Debugger.Error("The input listener '{0}' was invalid format, added it failed.", NovaEngine.Utility.Text.ToString(methodInfo));
                    return false;
                }
            }

            InputCallSyntaxInfo info = new InputCallSyntaxInfo(this, inputCode, operationType, methodInfo, automatically);

            if (false == _inputCallInfosForCode.TryGetValue(inputCode, out IDictionary<string, InputCallSyntaxInfo> infos))
            {
                // 创建监听列表
                infos = new Dictionary<string, InputCallSyntaxInfo>();
                infos.Add(info.Fullname, info);

                _inputCallInfosForCode.Add(inputCode, infos);

                // 新增输入响应的后处理程序
                return OnInputResponseAddedActionPostProcess(inputCode, operationType);
            }

            if (infos.ContainsKey(info.Fullname))
            {
                Debugger.Warn("The '{0}' instance's input '{1}' was already add same listener by name '{2}', repeat do it failed.",
                        NovaEngine.Utility.Text.ToString(GetType()), inputCode, info.Fullname);
                return false;
            }

            infos.Add(info.Fullname, info);

            return true;
        }

        /// <summary>
        /// 基础对象的输入响应函数接口，对一个指定的类型进行响应监听
        /// </summary>
        /// <typeparam name="T">输入类型</typeparam>
        /// <returns>若输入响应成功则返回true，否则返回false</returns>
        public bool AddInputResponse<T>() where T : struct
        {
            return AddInputResponse(typeof(T));
        }

        /// <summary>
        /// 基础对象的输入响应函数接口，对一个指定的类型进行响应监听
        /// </summary>
        /// <param name="inputType">输入类型</param>
        /// <returns>若输入响应成功则返回true，否则返回false</returns>
        public virtual bool AddInputResponse(SystemType inputType)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// 基础对象的输入响应函数接口，将一个指定的输入绑定到给定的监听回调函数上
        /// </summary>
        /// <typeparam name="T">输入类型</typeparam>
        /// <param name="func">监听回调函数</param>
        /// <returns>若输入响应成功则返回true，否则返回false</returns>
        public bool AddInputResponse<T>(System.Action<T> func) where T : struct
        {
            return AddInputResponse(typeof(T), func.Method);
        }

        /// <summary>
        /// 基础对象的输入响应函数接口，将一个指定的输入绑定到给定的监听回调函数上
        /// </summary>
        /// <param name="inputType">输入类型</param>
        /// <param name="methodInfo">监听回调函数</param>
        /// <returns>若输入响应成功则返回true，否则返回false</returns>
        public bool AddInputResponse(SystemType inputType, SystemMethodInfo methodInfo)
        {
            return AddInputResponse(inputType, methodInfo, false);
        }

        /// <summary>
        /// 基础对象的输入响应函数接口，将一个指定的输入绑定到给定的监听回调函数上
        /// </summary>
        /// <param name="inputType">输入类型</param>
        /// <param name="methodInfo">监听回调函数</param>
        /// <param name="automatically">自动装载状态标识</param>
        /// <returns>若输入响应成功则返回true，否则返回false</returns>
        protected internal bool AddInputResponse(SystemType inputType, SystemMethodInfo methodInfo, bool automatically)
        {
            // 函数格式校验
            if (NovaEngine.Debugger.Instance.IsOnDebuggingVerificationActivated())
            {
                bool verificated = false;
                if (NovaEngine.Utility.Reflection.IsTypeOfExtension(methodInfo))
                {
                    verificated = Loader.Inspecting.CodeInspector.IsValidFormatOfBeanExtendInputCallFunction(methodInfo);
                }
                else
                {
                    verificated = Loader.Inspecting.CodeInspector.IsValidFormatOfInputCallFunction(methodInfo);
                }

                // 校验失败
                if (false == verificated)
                {
                    Debugger.Error("The input listener '{0}' was invalid format, added it failed.", NovaEngine.Utility.Text.ToString(methodInfo));
                    return false;
                }
            }

            InputCallSyntaxInfo info = new InputCallSyntaxInfo(this, inputType, methodInfo, automatically);

            if (false == _inputCallInfosForType.TryGetValue(inputType, out IDictionary<string, InputCallSyntaxInfo> infos))
            {
                // 创建监听列表
                infos = new Dictionary<string, InputCallSyntaxInfo>();
                infos.Add(info.Fullname, info);

                _inputCallInfosForType.Add(inputType, infos);

                // 新增输入响应的后处理程序
                return OnInputResponseAddedActionPostProcess(inputType);
            }

            if (infos.ContainsKey(info.Fullname))
            {
                Debugger.Warn("The '{0}' instance's input '{1}' was already add same listener by name '{2}', repeat do it failed.",
                        NovaEngine.Utility.Text.ToString(GetType()), inputType.FullName, info.Fullname);
                return false;
            }

            infos.Add(info.Fullname, info);

            return true;
        }

        /// <summary>
        /// 取消当前基础对象对指定编码的响应
        /// </summary>
        /// <param name="inputCode">输入编码</param>
        public void RemoveInputResponse(int inputCode)
        {
            RemoveInputResponse(inputCode, 0);
        }

        /// <summary>
        /// 取消当前基础对象对指定编码的响应
        /// </summary>
        /// <param name="inputCode">输入编码</param>
        /// <param name="operationType">输入操作类型</param>
        public virtual void RemoveInputResponse(int inputCode, int operationType)
        {
            // 若针对特定编码绑定了监听回调，则移除相应的回调句柄
            if (_inputCallInfosForCode.ContainsKey(inputCode))
            {
                _inputCallInfosForCode.Remove(inputCode);
            }

            // 移除输入响应的后处理程序
            OnInputResponseRemovedActionPostProcess(inputCode, operationType);
        }

        /// <summary>
        /// 取消当前基础对象对指定编码的监听回调函数
        /// </summary>
        /// <param name="inputCode">输入编码</param>
        /// <param name="operationType">输入操作类型</param>
        /// <param name="methodInfo">监听回调函数</param>
        public void RemoveInputResponse(int inputCode, int operationType, SystemMethodInfo methodInfo)
        {
            string funcName = GenTools.GenUniqueName(methodInfo);

            RemoveInputResponse(inputCode, operationType, funcName);
        }

        /// <summary>
        /// 取消当前基础对象对指定编码的监听回调函数
        /// </summary>
        /// <param name="inputCode">输入编码</param>
        /// <param name="funcName">函数名称</param>
        protected internal void RemoveInputResponse(int inputCode, string funcName)
        {
            RemoveInputResponse(inputCode, 0, funcName);
        }

        /// <summary>
        /// 取消当前基础对象对指定编码的监听回调函数
        /// </summary>
        /// <param name="inputCode">输入编码</param>
        /// <param name="operationType">输入操作类型</param>
        /// <param name="funcName">函数名称</param>
        protected internal void RemoveInputResponse(int inputCode, int operationType, string funcName)
        {
            if (_inputCallInfosForCode.TryGetValue(inputCode, out IDictionary<string, InputCallSyntaxInfo> infos))
            {
                if (infos.ContainsKey(funcName))
                {
                    infos.Remove(funcName);
                }
            }

            // 当前监听列表为空时，移除该编码的监听
            if (false == IsInputResponsedOfTargetCode(inputCode, operationType))
            {
                RemoveInputResponse(inputCode, operationType);
            }
        }

        /// <summary>
        /// 取消当前基础对象对指定输入类型的响应
        /// </summary>
        /// <typeparam name="T">输入类型</typeparam>
        public void RemoveInputResponse<T>()
        {
            RemoveInputResponse(typeof(T));
        }

        /// <summary>
        /// 取消当前基础对象对指定输入类型的响应
        /// </summary>
        /// <param name="inputType">输入类型</param>
        public virtual void RemoveInputResponse(SystemType inputType)
        {
            // 若针对特定输入绑定了监听回调，则移除相应的回调句柄
            if (_eventCallInfosForType.ContainsKey(inputType))
            {
                _eventCallInfosForType.Remove(inputType);
            }

            // 移除输入响应的后处理程序
            OnInputResponseRemovedActionPostProcess(inputType);
        }

        /// <summary>
        /// 取消当前基础对象对指定输入类型的监听回调函数
        /// </summary>
        /// <typeparam name="T">输入类型</typeparam>
        /// <param name="methodInfo">监听回调函数</param>
        public void RemoveInputResponse<T>(SystemMethodInfo methodInfo)
        {
            RemoveInputResponse(typeof(T), methodInfo);
        }

        /// <summary>
        /// 取消当前基础对象对指定输入类型的响应
        /// </summary>
        /// <param name="inputType">输入类型</param>
        /// <param name="methodInfo">监听回调函数</param>
        public void RemoveInputResponse(SystemType inputType, SystemMethodInfo methodInfo)
        {
            string funcName = GenTools.GenUniqueName(methodInfo);

            RemoveInputResponse(inputType, funcName);
        }

        /// <summary>
        /// 取消当前基础对象对指定输入类型的监听回调函数
        /// </summary>
        /// <typeparam name="T">输入类型</typeparam>
        /// <param name="funcName">函数名称</param>
        protected internal void RemoveInputResponse<T>(string funcName)
        {
            RemoveInputResponse(typeof(T), funcName);
        }

        /// <summary>
        /// 取消当前基础对象对指定输入类型的监听回调函数
        /// </summary>
        /// <param name="inputType">输入类型</param>
        /// <param name="funcName">函数名称</param>
        protected internal void RemoveInputResponse(SystemType inputType, string funcName)
        {
            if (_inputCallInfosForType.TryGetValue(inputType, out IDictionary<string, InputCallSyntaxInfo> infos))
            {
                if (infos.ContainsKey(funcName))
                {
                    infos.Remove(funcName);
                }
            }

            // 当前监听列表为空时，移除该事件的监听
            if (false == IsInputResponsedOfTargetType(inputType))
            {
                RemoveInputResponse(inputType);
            }
        }

        /// <summary>
        /// 移除所有自动注册的输入响应回调接口
        /// </summary>
        protected internal void RemoveAllAutomaticallyInputResponses()
        {
            OnAutomaticallyCallSyntaxInfoProcessHandler<int, InputCallSyntaxInfo>(_inputCallInfosForCode, RemoveInputResponse);
            OnAutomaticallyCallSyntaxInfoProcessHandler<SystemType, InputCallSyntaxInfo>(_inputCallInfosForType, RemoveInputResponse);
        }

        /// <summary>
        /// 取消当前基础对象的所有输入响应
        /// </summary>
        public virtual void RemoveAllInputResponses()
        {
            IList<int> id_keys = NovaEngine.Utility.Collection.ToListForKeys<int, IDictionary<string, InputCallSyntaxInfo>>(_inputCallInfosForCode);
            for (int n = 0; null != id_keys && n < id_keys.Count; ++n) { RemoveInputResponse(id_keys[n]); }

            IList<SystemType> type_keys = NovaEngine.Utility.Collection.ToListForKeys<SystemType, IDictionary<string, InputCallSyntaxInfo>>(_inputCallInfosForType);
            for (int n = 0; null != type_keys && n < type_keys.Count; ++n) { RemoveInputResponse(type_keys[n]); }

            _inputCallInfosForCode.Clear();
            _inputCallInfosForType.Clear();
        }

        #endregion

        #region 基础对象事件转发相关操作函数合集

        /// <summary>
        /// 基础对象的订阅事件的监听回调函数<br/>
        /// 该函数针对事件转发接口的标准实现，禁止子类重写该函数<br/>
        /// 若子类需要根据需要自行解析事件，可以通过重写<see cref="GameEngine.CBase.OnEvent(int, object[])"/>实现事件的自定义处理逻辑
        /// </summary>
        /// <param name="eventID">事件标识</param>
        /// <param name="args">事件数据参数</param>
        public virtual void OnEventDispatchForId(int eventID, params object[] args)
        {
            if (_eventCallInfosForId.TryGetValue(eventID, out IDictionary<string, EventCallSyntaxInfo> infos))
            {
                IEnumerator<EventCallSyntaxInfo> e = infos.Values.GetEnumerator();
                while (e.MoveNext())
                {
                    e.Current.Invoke(eventID, args);
                }
            }

            OnEvent(eventID, args);
        }

        /// <summary>
        /// 基础对象的订阅事件的监听回调函数<br/>
        /// 该函数针对事件转发接口的标准实现，禁止子类重写该函数<br/>
        /// 若子类需要根据需要自行解析事件，可以通过重写<see cref="GameEngine.CBase.OnEvent(object)"/>实现事件的自定义处理逻辑
        /// </summary>
        /// <param name="eventData">事件数据</param>
        public virtual void OnEventDispatchForType(object eventData)
        {
            if (_eventCallInfosForType.TryGetValue(eventData.GetType(), out IDictionary<string, EventCallSyntaxInfo> infos))
            {
                IEnumerator<EventCallSyntaxInfo> e = infos.Values.GetEnumerator();
                while (e.MoveNext())
                {
                    e.Current.Invoke(eventData);
                }
            }

            OnEvent(eventData);
        }

        /// <summary>
        /// 用户自定义的事件处理函数，您可以通过重写该函数处理自定义事件行为
        /// </summary>
        /// <param name="eventID">事件标识</param>
        /// <param name="args">事件数据参数</param>
        protected abstract void OnEvent(int eventID, params object[] args);

        /// <summary>
        /// 用户自定义的事件处理函数，您可以通过重写该函数处理自定义事件行为
        /// </summary>
        /// <param name="eventData">事件数据</param>
        protected abstract void OnEvent(object eventData);

        /// <summary>
        /// 针对指定事件标识新增事件订阅的后处理程序
        /// </summary>
        /// <param name="eventID">事件标识</param>
        /// <returns>返回后处理的操作结果</returns>
        protected abstract bool OnSubscribeActionPostProcess(int eventID);
        /// <summary>
        /// 针对指定事件类型新增事件订阅的后处理程序
        /// </summary>
        /// <param name="eventType">事件类型</param>
        /// <returns>返回后处理的操作结果</returns>
        protected abstract bool OnSubscribeActionPostProcess(SystemType eventType);
        /// <summary>
        /// 针对指定事件标识移除事件订阅的后处理程序
        /// </summary>
        /// <param name="eventID">事件标识</param>
        protected abstract void OnUnsubscribeActionPostProcess(int eventID);
        /// <summary>
        /// 针对指定事件类型移除事件订阅的后处理程序
        /// </summary>
        /// <param name="eventType">事件类型</param>
        protected abstract void OnUnsubscribeActionPostProcess(SystemType eventType);

        /// <summary>
        /// 检测当前基础对象是否订阅了目标事件标识
        /// </summary>
        /// <param name="eventID">事件标识</param>
        /// <returns>若订阅了给定事件标识则返回true，否则返回false</returns>
        protected internal virtual bool IsSubscribedOfTargetId(int eventID)
        {
            if (_eventCallInfosForId.ContainsKey(eventID) && _eventCallInfosForId[eventID].Count > 0)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 检测当前基础对象是否订阅了目标事件类型
        /// </summary>
        /// <param name="eventType">事件类型</param>
        /// <returns>若订阅了给定事件类型则返回true，否则返回false</returns>
        protected internal virtual bool IsSubscribedOfTargetType(SystemType eventType)
        {
            if (_eventCallInfosForType.ContainsKey(eventType) && _eventCallInfosForType[eventType].Count > 0)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 基础对象的事件订阅函数接口，对一个指定的事件进行订阅监听
        /// </summary>
        /// <param name="eventID">事件标识</param>
        /// <returns>若事件订阅成功则返回true，否则返回false</returns>
        public virtual bool Subscribe(int eventID)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// 基础对象的事件订阅函数接口，将一个指定的事件绑定到给定的监听回调函数上
        /// </summary>
        /// <param name="eventID">事件标识</param>
        /// <param name="methodInfo">监听回调函数</param>
        /// <returns>若事件订阅成功则返回true，否则返回false</returns>
        public bool Subscribe(int eventID, SystemMethodInfo methodInfo)
        {
            return Subscribe(eventID, methodInfo, false);
        }

        /// <summary>
        /// 基础对象的事件订阅函数接口，将一个指定的事件绑定到给定的监听回调函数上
        /// </summary>
        /// <param name="eventID">事件标识</param>
        /// <param name="methodInfo">监听回调函数</param>
        /// <param name="automatically">自动装载状态标识</param>
        /// <returns>若事件订阅成功则返回true，否则返回false</returns>
        protected internal bool Subscribe(int eventID, SystemMethodInfo methodInfo, bool automatically)
        {
            // 函数格式校验
            if (NovaEngine.Debugger.Instance.IsOnDebuggingVerificationActivated())
            {
                bool verificated = false;
                if (NovaEngine.Utility.Reflection.IsTypeOfExtension(methodInfo))
                {
                    verificated = Loader.Inspecting.CodeInspector.IsValidFormatOfBeanExtendEventCallFunction(methodInfo);
                }
                else
                {
                    verificated = Loader.Inspecting.CodeInspector.IsValidFormatOfEventCallFunction(methodInfo);
                }

                // 校验失败
                if (false == verificated)
                {
                    Debugger.Error("The event listener '{0}' was invalid format, subscribed it failed.", NovaEngine.Utility.Text.ToString(methodInfo));
                    return false;
                }
            }

            EventCallSyntaxInfo info = new EventCallSyntaxInfo(this, eventID, methodInfo, automatically);

            if (false == _eventCallInfosForId.TryGetValue(eventID, out IDictionary<string, EventCallSyntaxInfo> infos))
            {
                // 创建监听列表
                infos = new Dictionary<string, EventCallSyntaxInfo>();
                infos.Add(info.Fullname, info);

                _eventCallInfosForId.Add(eventID, infos);

                // 新增事件订阅的后处理程序
                return OnSubscribeActionPostProcess(eventID);
            }

            if (infos.ContainsKey(info.Fullname))
            {
                Debugger.Warn("The '{0}' instance's event '{1}' was already add same listener by name '{2}', repeat do it failed.",
                        NovaEngine.Utility.Text.ToString(GetType()), eventID, info.Fullname);
                return false;
            }

            infos.Add(info.Fullname, info);

            return true;
        }

        /// <summary>
        /// 基础对象的事件订阅函数接口，对一个指定的事件进行订阅监听
        /// </summary>
        /// <typeparam name="T">事件类型</typeparam>
        /// <returns>若事件订阅成功则返回true，否则返回false</returns>
        public bool Subscribe<T>() where T : struct
        {
            return Subscribe(typeof(T));
        }

        /// <summary>
        /// 基础对象的事件订阅函数接口，对一个指定的事件进行订阅监听
        /// </summary>
        /// <param name="eventType">事件类型</param>
        /// <returns>若事件订阅成功则返回true，否则返回false</returns>
        public virtual bool Subscribe(SystemType eventType)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// 基础对象的事件订阅函数接口，将一个指定的事件绑定到给定的监听回调函数上
        /// </summary>
        /// <typeparam name="T">事件类型</typeparam>
        /// <param name="func">监听回调函数</param>
        /// <returns>若事件订阅成功则返回true，否则返回false</returns>
        public bool Subscribe<T>(System.Action<T> func) where T : struct
        {
            return Subscribe(typeof(T), func.Method);
        }

        /// <summary>
        /// 基础对象的事件订阅函数接口，将一个指定的事件绑定到给定的监听回调函数上
        /// </summary>
        /// <param name="eventType">事件类型</param>
        /// <param name="methodInfo">监听回调函数</param>
        /// <returns>若事件订阅成功则返回true，否则返回false</returns>
        public bool Subscribe(SystemType eventType, SystemMethodInfo methodInfo)
        {
            return Subscribe(eventType, methodInfo, false);
        }

        /// <summary>
        /// 基础对象的事件订阅函数接口，将一个指定的事件绑定到给定的监听回调函数上
        /// </summary>
        /// <param name="eventType">事件类型</param>
        /// <param name="methodInfo">监听回调函数</param>
        /// <param name="automatically">自动装载状态标识</param>
        /// <returns>若事件订阅成功则返回true，否则返回false</returns>
        protected internal bool Subscribe(SystemType eventType, SystemMethodInfo methodInfo, bool automatically)
        {
            // 函数格式校验
            if (NovaEngine.Debugger.Instance.IsOnDebuggingVerificationActivated())
            {
                bool verificated = false;
                if (NovaEngine.Utility.Reflection.IsTypeOfExtension(methodInfo))
                {
                    verificated = Loader.Inspecting.CodeInspector.IsValidFormatOfBeanExtendEventCallFunction(methodInfo);
                }
                else
                {
                    verificated = Loader.Inspecting.CodeInspector.IsValidFormatOfEventCallFunction(methodInfo);
                }

                // 校验失败
                if (false == verificated)
                {
                    Debugger.Error("The event listener '{0}' was invalid format, subscribed it failed.", NovaEngine.Utility.Text.ToString(methodInfo));
                    return false;
                }
            }

            EventCallSyntaxInfo info = new EventCallSyntaxInfo(this, eventType, methodInfo, automatically);

            if (false == _eventCallInfosForType.TryGetValue(eventType, out IDictionary<string, EventCallSyntaxInfo> infos))
            {
                // 创建监听列表
                infos = new Dictionary<string, EventCallSyntaxInfo>();
                infos.Add(info.Fullname, info);

                _eventCallInfosForType.Add(eventType, infos);

                // 新增事件订阅的后处理程序
                return OnSubscribeActionPostProcess(eventType);
            }

            if (infos.ContainsKey(info.Fullname))
            {
                Debugger.Warn("The '{0}' instance's event '{1}' was already add same listener by name '{2}', repeat do it failed.",
                        NovaEngine.Utility.Text.ToString(GetType()), eventType.FullName, info.Fullname);
                return false;
            }

            infos.Add(info.Fullname, info);

            return true;
        }

        /// <summary>
        /// 取消当前基础对象对指定事件的订阅
        /// </summary>
        /// <param name="eventID">事件标识</param>
        public virtual void Unsubscribe(int eventID)
        {
            // 若针对特定事件绑定了监听回调，则移除相应的回调句柄
            if (_eventCallInfosForId.ContainsKey(eventID))
            {
                _eventCallInfosForId.Remove(eventID);
            }

            // 移除事件订阅的后处理程序
            OnUnsubscribeActionPostProcess(eventID);
        }

        /// <summary>
        /// 取消当前基础对象对指定扩展事件对应的监听回调函数
        /// </summary>
        /// <param name="eventID">事件标识</param>
        /// <param name="methodInfo">监听回调函数</param>
        public void Unsubscribe(int eventID, SystemMethodInfo methodInfo)
        {
            string funcName = GenTools.GenUniqueName(methodInfo);

            Unsubscribe(eventID, funcName);
        }

        /// <summary>
        /// 取消当前基础对象对指定扩展事件对应的监听回调函数
        /// </summary>
        /// <param name="eventID">事件标识</param>
        /// <param name="funcName">函数名称</param>
        protected internal void Unsubscribe(int eventID, string funcName)
        {
            if (_eventCallInfosForId.TryGetValue(eventID, out IDictionary<string, EventCallSyntaxInfo> infos))
            {
                if (infos.ContainsKey(funcName))
                {
                    infos.Remove(funcName);
                }
            }

            // 当前监听列表为空时，移除该事件的监听
            if (false == IsSubscribedOfTargetId(eventID))
            {
                Unsubscribe(eventID);
            }
        }

        /// <summary>
        /// 取消当前基础对象对指定事件的订阅
        /// </summary>
        /// <typeparam name="T">事件类型</typeparam>
        public void Unsubscribe<T>()
        {
            Unsubscribe(typeof(T));
        }

        /// <summary>
        /// 取消当前基础对象对指定事件的订阅
        /// </summary>
        /// <param name="eventType">事件类型</param>
        public virtual void Unsubscribe(SystemType eventType)
        {
            // 若针对特定事件绑定了监听回调，则移除相应的回调句柄
            if (_eventCallInfosForType.ContainsKey(eventType))
            {
                _eventCallInfosForType.Remove(eventType);
            }

            // 移除事件订阅的后处理程序
            OnUnsubscribeActionPostProcess(eventType);
        }

        /// <summary>
        /// 取消当前基础对象对指定事件对应的监听回调函数
        /// </summary>
        /// <typeparam name="T">事件类型</typeparam>
        /// <param name="methodInfo">监听回调函数</param>
        public void Unsubscribe<T>(SystemMethodInfo methodInfo)
        {
            Unsubscribe(typeof(T), methodInfo);
        }

        /// <summary>
        /// 取消当前基础对象对指定事件的订阅
        /// </summary>
        /// <param name="eventType">事件类型</param>
        /// <param name="methodInfo">监听回调函数</param>
        public void Unsubscribe(SystemType eventType, SystemMethodInfo methodInfo)
        {
            string funcName = GenTools.GenUniqueName(methodInfo);

            Unsubscribe(eventType, funcName);
        }

        /// <summary>
        /// 取消当前基础对象对指定事件对应的监听回调函数
        /// </summary>
        /// <typeparam name="T">事件类型</typeparam>
        /// <param name="funcName">函数名称</param>
        protected internal void Unsubscribe<T>(string funcName)
        {
            Unsubscribe(typeof(T), funcName);
        }

        /// <summary>
        /// 取消当前基础对象对指定事件对应的监听回调函数
        /// </summary>
        /// <param name="eventType">事件类型</param>
        /// <param name="funcName">函数名称</param>
        protected internal void Unsubscribe(SystemType eventType, string funcName)
        {
            if (_eventCallInfosForType.TryGetValue(eventType, out IDictionary<string, EventCallSyntaxInfo> infos))
            {
                if (infos.ContainsKey(funcName))
                {
                    infos.Remove(funcName);
                }
            }

            // 当前监听列表为空时，移除该事件的监听
            if (false == IsSubscribedOfTargetType(eventType))
            {
                Unsubscribe(eventType);
            }
        }

        /// <summary>
        /// 移除所有自动注册的事件订阅回调接口
        /// </summary>
        protected internal void UnsubscribeAllAutomaticallyEvents()
        {
            OnAutomaticallyCallSyntaxInfoProcessHandler<int, EventCallSyntaxInfo>(_eventCallInfosForId, Unsubscribe);
            OnAutomaticallyCallSyntaxInfoProcessHandler<SystemType, EventCallSyntaxInfo>(_eventCallInfosForType, Unsubscribe);
        }

        /// <summary>
        /// 取消当前基础对象的所有事件订阅
        /// </summary>
        public virtual void UnsubscribeAllEvents()
        {
            IList<int> id_keys = NovaEngine.Utility.Collection.ToListForKeys<int, IDictionary<string, EventCallSyntaxInfo>>(_eventCallInfosForId);
            for (int n = 0; null != id_keys && n < id_keys.Count; ++n) { Unsubscribe(id_keys[n]); }

            IList<SystemType> type_keys = NovaEngine.Utility.Collection.ToListForKeys<SystemType, IDictionary<string, EventCallSyntaxInfo>>(_eventCallInfosForType);
            for (int n = 0; null != type_keys && n < type_keys.Count; ++n) { Unsubscribe(type_keys[n]); }

            _eventCallInfosForId.Clear();
            _eventCallInfosForType.Clear();
        }

        #endregion

        #region 基础对象消息通知相关操作函数合集

        /// <summary>
        /// 基础对象的消息通知的监听回调函数<br/>
        /// 该函数针对消息转发接口的标准实现，禁止子类重写该函数<br/>
        /// 若子类需要根据需要自行处理消息，可以通过重写<see cref="GameEngine.CBase.OnMessage(ProtoBuf.Extension.IMessage)"/>实现消息的自定义处理逻辑
        /// </summary>
        /// <param name="opcode">协议操作码</param>
        /// <param name="message">消息对象实例</param>
        public virtual void OnMessageDispatch(int opcode, ProtoBuf.Extension.IMessage message)
        {
            if (_messageCallInfosForType.TryGetValue(opcode, out IDictionary<string, MessageCallSyntaxInfo> infos))
            {
                IEnumerator<MessageCallSyntaxInfo> e = infos.Values.GetEnumerator();
                while (e.MoveNext())
                {
                    e.Current.Invoke(message);
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
            if (_messageCallInfosForType.ContainsKey(opcode) && _messageCallInfosForType[opcode].Count > 0)
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
        public virtual bool AddMessageListener(int opcode)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// 基础对象的消息监听函数接口，将一个指定的协议码绑定到给定的监听回调函数上
        /// </summary>
        /// <param name="opcode">协议操作码</param>
        /// <param name="methodInfo">监听回调函数</param>
        /// <returns>若消息监听成功则返回true，否则返回false</returns>
        public bool AddMessageListener(int opcode, SystemMethodInfo methodInfo)
        {
            return AddMessageListener(opcode, methodInfo, false);
        }

        /// <summary>
        /// 基础对象的消息监听函数接口，将一个指定的协议码绑定到给定的监听回调函数上
        /// </summary>
        /// <param name="opcode">协议操作码</param>
        /// <param name="methodInfo">监听回调函数</param>
        /// <param name="automatically">自动装载状态标识</param>
        /// <returns>若消息监听成功则返回true，否则返回false</returns>
        protected internal bool AddMessageListener(int opcode, SystemMethodInfo methodInfo, bool automatically)
        {
            // 函数格式校验
            if (NovaEngine.Debugger.Instance.IsOnDebuggingVerificationActivated())
            {
                bool verificated = false;
                if (NovaEngine.Utility.Reflection.IsTypeOfExtension(methodInfo))
                {
                    verificated = Loader.Inspecting.CodeInspector.IsValidFormatOfBeanExtendMessageCallFunction(methodInfo);
                }
                else
                {
                    verificated = Loader.Inspecting.CodeInspector.IsValidFormatOfMessageCallFunction(methodInfo);
                }

                // 校验失败
                if (false == verificated)
                {
                    Debugger.Error("The message dispatch listener '{0}' was invalid format, added it failed.", NovaEngine.Utility.Text.ToString(methodInfo));
                    return false;
                }
            }

            MessageCallSyntaxInfo info = new MessageCallSyntaxInfo(this, opcode, methodInfo, automatically);

            if (false == _messageCallInfosForType.TryGetValue(opcode, out IDictionary<string, MessageCallSyntaxInfo> infos))
            {
                // 创建监听列表
                infos = new Dictionary<string, MessageCallSyntaxInfo>();
                infos.Add(info.Fullname, info);

                _messageCallInfosForType.Add(opcode, infos);

                // 新增消息监听的后处理程序
                return OnMessageListenerAddedActionPostProcess(opcode);
            }

            if (infos.ContainsKey(info.Fullname))
            {
                Debugger.Warn("The '{0}' instance's message type '{1}' was already add same listener by name '{2}', repeat added it failed.",
                    NovaEngine.Utility.Text.ToString(GetType()), opcode, info.Fullname);
                return false;
            }

            infos.Add(info.Fullname, info);

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
            return AddMessageListener(typeof(T), func.Method);
        }

        /// <summary>
        /// 基础对象的消息监听函数接口，将一个指定的消息类型绑定到给定的监听回调函数上
        /// </summary>
        /// <param name="messageType">消息类型</param>
        /// <param name="methodInfo">监听回调函数</param>
        /// <returns>若消息监听成功则返回true，否则返回false</returns>
        public bool AddMessageListener(SystemType messageType, SystemMethodInfo methodInfo)
        {
            int opcode = NetworkHandler.Instance.GetOpcodeByMessageType(messageType);

            return AddMessageListener(opcode, methodInfo);
        }

        /// <summary>
        /// 基础对象的消息监听函数接口，将一个指定的消息类型绑定到给定的监听回调函数上
        /// </summary>
        /// <param name="messageType">消息类型</param>
        /// <param name="methodInfo">监听回调函数</param>
        /// <param name="automatically">自动装载状态标识</param>
        /// <returns>若消息监听成功则返回true，否则返回false</returns>
        protected internal bool AddMessageListener(SystemType messageType, SystemMethodInfo methodInfo, bool automatically)
        {
            int opcode = NetworkHandler.Instance.GetOpcodeByMessageType(messageType);

            return AddMessageListener(opcode, methodInfo, automatically);
        }

        /// <summary>
        /// 取消当前基础对象对指定协议码的监听回调
        /// </summary>
        /// <param name="opcode">协议操作码</param>
        public virtual void RemoveMessageListener(int opcode)
        {
            // 若针对特定消息绑定了监听回调，则移除相应的回调句柄
            if (_messageCallInfosForType.ContainsKey(opcode))
            {
                _messageCallInfosForType.Remove(opcode);
            }

            // 移除消息监听的后处理程序
            OnMessageListenerRemovedActionPostProcess(opcode);
        }

        /// <summary>
        /// 取消当前基础对象对指定协议码的监听回调
        /// </summary>
        /// <param name="opcode">协议操作码</param>
        /// <param name="methodInfo">监听回调函数</param>
        public void RemoveMessageListener(int opcode, SystemMethodInfo methodInfo)
        {
            string funcName = GenTools.GenUniqueName(methodInfo);

            RemoveMessageListener(opcode, funcName);
        }

        /// <summary>
        /// 取消当前基础对象对指定协议码的监听回调
        /// </summary>
        /// <param name="opcode">协议操作码</param>
        /// <param name="funcName">函数名称</param>
        protected internal void RemoveMessageListener(int opcode, string funcName)
        {
            if (_messageCallInfosForType.TryGetValue(opcode, out IDictionary<string, MessageCallSyntaxInfo> infos))
            {
                if (infos.ContainsKey(funcName))
                {
                    infos.Remove(funcName);
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
            RemoveMessageListener(typeof(T), methodInfo);
        }

        /// <summary>
        /// 取消当前基础对象对指定消息类型的监听回调
        /// </summary>
        /// <param name="messageType">消息类型</param>
        /// <param name="methodInfo">监听回调函数</param>
        public void RemoveMessageListener(SystemType messageType, SystemMethodInfo methodInfo)
        {
            int opcode = NetworkHandler.Instance.GetOpcodeByMessageType(messageType);

            RemoveMessageListener(opcode, methodInfo);
        }

        /// <summary>
        /// 取消当前基础对象对指定消息类型的监听回调
        /// </summary>
        /// <typeparam name="T">消息类型</typeparam>
        /// <param name="funcName">函数名称</param>
        protected internal void RemoveMessageListener<T>(string funcName)
        {
            RemoveMessageListener(typeof(T), funcName);
        }

        /// <summary>
        /// 取消当前基础对象对指定消息类型的监听回调
        /// </summary>
        /// <param name="messageType">消息类型</param>
        /// <param name="funcName">函数名称</param>
        protected internal void RemoveMessageListener(SystemType messageType, string funcName)
        {
            int opcode = NetworkHandler.Instance.GetOpcodeByMessageType(messageType);

            RemoveMessageListener(opcode, funcName);
        }

        /// <summary>
        /// 移除所有自动注册的消息监听回调接口
        /// </summary>
        protected internal void RemoveAllAutomaticallyMessageListeners()
        {
            OnAutomaticallyCallSyntaxInfoProcessHandler<int, MessageCallSyntaxInfo>(_messageCallInfosForType, RemoveMessageListener);
        }

        /// <summary>
        /// 取消当前基础对象的所有注册的消息监听回调
        /// </summary>
        public virtual void RemoveAllMessageListeners()
        {
            IList<int> id_keys = NovaEngine.Utility.Collection.ToListForKeys<int, IDictionary<string, MessageCallSyntaxInfo>>(_messageCallInfosForType);
            for (int n = 0; null != id_keys && n < id_keys.Count; ++n) { RemoveMessageListener(id_keys[n]); }

            _messageCallInfosForType.Clear();
        }

        #endregion

        #region 基础回调接口包装结构及处理函数声明（包含回调接口函数的通用数据结构）

        protected class BaseCallSyntaxInfo
        {
            /// <summary>
            /// 回调函数的目标对象实例
            /// </summary>
            protected readonly CBase _targetObject;
            /// <summary>
            /// 回调函数的完整名称
            /// </summary>
            protected readonly string _fullname;
            /// <summary>
            /// 回调函数的函数信息实例
            /// </summary>
            protected readonly SystemMethodInfo _methodInfo;
            /// <summary>
            /// 回调函数的动态构建回调句柄
            /// </summary>
            protected readonly SystemDelegate _callback;
            /// <summary>
            /// 回调函数的自动注册状态标识
            /// </summary>
            protected readonly bool _automatically;
            /// <summary>
            /// 回调函数的扩展定义状态标识
            /// </summary>
            protected readonly bool _isExtensionType;
            /// <summary>
            /// 回调函数的无参状态标识
            /// </summary>
            protected readonly bool _isNullParameterType;

            public string Fullname => _fullname;
            public SystemMethodInfo MethodInfo => _methodInfo;
            public SystemDelegate Callback => _callback;
            public bool Automatically => _automatically;
            public bool IsExtensionType => _isExtensionType;
            public bool IsNullParameterType => _isNullParameterType;

            protected BaseCallSyntaxInfo(CBase targetObject, SystemMethodInfo methodInfo, bool automatically)
            {
                _targetObject = targetObject;
                _methodInfo = methodInfo;
                _automatically = automatically;
                _isExtensionType = NovaEngine.Utility.Reflection.IsTypeOfExtension(methodInfo);
                _isNullParameterType = Loader.Inspecting.CodeInspector.IsNullParameterTypeOfMessageCallFunction(methodInfo);

                object obj = targetObject;
                if (_isExtensionType)
                {
                    // 扩展函数在构建委托时不需要传入运行时对象实例，而是在调用时传入
                    obj = null;
                }

                string fullname = GenTools.GenUniqueName(methodInfo);

                SystemDelegate callback = NovaEngine.Utility.Reflection.CreateGenericActionDelegate(obj, methodInfo);
                Debugger.Assert(null != callback, "Invalid method type.");

                _fullname = fullname;
                _callback = callback;
            }
        }

        #endregion

        #region 输入回调接口包装结构及处理函数声明

        /// <summary>
        /// 输入回调接口的包装对象类
        /// </summary>
        protected class InputCallSyntaxInfo : BaseCallSyntaxInfo
        {
            /// <summary>
            /// 输入回调绑定的输入编码
            /// </summary>
            private readonly int _inputCode;
            /// <summary>
            /// 输入回调绑定的操作类型
            /// </summary>
            private readonly int _operationType;
            /// <summary>
            /// 输入回调绑定的输入数据类型
            /// </summary>
            private readonly SystemType _inputDataType;

            public int InputCode => _inputCode;
            public int OperationType => _operationType;
            public SystemType InputDataType => _inputDataType;

            public InputCallSyntaxInfo(CBase targetObject, int inputCode, int operationType, SystemMethodInfo methodInfo) :
                this(targetObject, inputCode, operationType, methodInfo, false)
            { }

            public InputCallSyntaxInfo(CBase targetObject, int inputCode, int operationType, SystemMethodInfo methodInfo, bool automatically) :
                this(targetObject, inputCode, operationType, null, methodInfo, automatically)
            { }

            public InputCallSyntaxInfo(CBase targetObject, SystemType inputDataType, SystemMethodInfo methodInfo) :
                this(targetObject, inputDataType, methodInfo, false)
            { }

            public InputCallSyntaxInfo(CBase targetObject, SystemType inputDataType, SystemMethodInfo methodInfo, bool automatically) :
                this(targetObject, 0, 0, inputDataType, methodInfo, automatically)
            { }

            private InputCallSyntaxInfo(CBase targetObject, int inputCode, int operationType, SystemType inputDataType, SystemMethodInfo methodInfo, bool automatically) :
                base(targetObject, methodInfo, automatically)
            {
                _inputCode = inputCode;
                _operationType = operationType;
                _inputDataType = inputDataType;
            }

            /// <summary>
            /// 输入回调的调度函数
            /// </summary>
            /// <param name="inputCode">输入编码</param>
            /// <param name="operationType">输入操作类型</param>
            public void Invoke(int inputCode, int operationType)
            {
                if (_operationType == 0 || (_operationType & operationType) == 0)
                {
                    // ignore
                    return;
                }

                if (_isExtensionType)
                {
                    if (_isNullParameterType)
                    {
                        _callback.DynamicInvoke(_targetObject);
                    }
                    else
                    {
                        _callback.DynamicInvoke(_targetObject, inputCode, operationType);
                    }
                }
                else
                {
                    if (_isNullParameterType)
                    {
                        _callback.DynamicInvoke();
                    }
                    else
                    {
                        _callback.DynamicInvoke(inputCode, operationType);
                    }
                }
            }

            /// <summary>
            /// 输入回调的调度函数
            /// </summary>
            /// <param name="inputData">输入数据</param>
            public void Invoke(object inputData)
            {
                if (_isExtensionType)
                {
                    if (_isNullParameterType)
                    {
                        _callback.DynamicInvoke(_targetObject);
                    }
                    else
                    {
                        _callback.DynamicInvoke(_targetObject, inputData);
                    }
                }
                else
                {
                    if (_isNullParameterType)
                    {
                        _callback.DynamicInvoke();
                    }
                    else
                    {
                        _callback.DynamicInvoke(inputData);
                    }
                }
            }
        }

        #endregion

        #region 事件回调接口包装结构及处理函数声明

        /// <summary>
        /// 事件回调接口的包装对象类
        /// </summary>
        protected class EventCallSyntaxInfo : BaseCallSyntaxInfo
        {
            /// <summary>
            /// 事件回调绑定的事件标识
            /// </summary>
            private readonly int _eventID;
            /// <summary>
            /// 事件回调绑定的事件数据类型
            /// </summary>
            private readonly SystemType _eventType;

            public int EventID => _eventID;
            public SystemType EventType => _eventType;

            public EventCallSyntaxInfo(CBase targetObject, int eventID, SystemMethodInfo methodInfo) : this(targetObject, eventID, methodInfo, false)
            { }

            public EventCallSyntaxInfo(CBase targetObject, int eventID, SystemMethodInfo methodInfo, bool automatically) : this(targetObject, eventID, null, methodInfo, automatically)
            { }

            public EventCallSyntaxInfo(CBase targetObject, SystemType eventType, SystemMethodInfo methodInfo) : this(targetObject, eventType, methodInfo, false)
            { }

            public EventCallSyntaxInfo(CBase targetObject, SystemType eventType, SystemMethodInfo methodInfo, bool automatically) : this(targetObject, 0, eventType, methodInfo, automatically)
            { }

            private EventCallSyntaxInfo(CBase targetObject, int eventID, SystemType eventType, SystemMethodInfo methodInfo, bool automatically) : base(targetObject, methodInfo, automatically)
            {
                _eventID = eventID;
                _eventType = eventType;
            }

            /// <summary>
            /// 事件回调的调度函数
            /// </summary>
            /// <param name="eventID">事件标识</param>
            /// <param name="args">事件数据参数</param>
            public void Invoke(int eventID, params object[] args)
            {
                if (_isExtensionType)
                {
                    if (_isNullParameterType)
                    {
                        _callback.DynamicInvoke(_targetObject);
                    }
                    else
                    {
                        _callback.DynamicInvoke(_targetObject, eventID, args);
                    }
                }
                else
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
            }

            /// <summary>
            /// 事件回调的调度函数
            /// </summary>
            /// <param name="eventData">事件数据</param>
            public void Invoke(object eventData)
            {
                if (_isExtensionType)
                {
                    if (_isNullParameterType)
                    {
                        _callback.DynamicInvoke(_targetObject);
                    }
                    else
                    {
                        _callback.DynamicInvoke(_targetObject, eventData);
                    }
                }
                else
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
            }
        }

        #endregion

        #region 消息回调接口包装结构及处理函数声明

        /// <summary>
        /// 消息回调接口的包装对象类
        /// </summary>
        protected class MessageCallSyntaxInfo : BaseCallSyntaxInfo
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

            public MessageCallSyntaxInfo(CBase targetObject, int opcode, SystemMethodInfo methodInfo) : this(targetObject, opcode, methodInfo, false)
            { }

            public MessageCallSyntaxInfo(CBase targetObject, int opcode, SystemMethodInfo methodInfo, bool automatically) : this(targetObject, opcode, null, methodInfo, automatically)
            { }

            public MessageCallSyntaxInfo(CBase targetObject, SystemType messageType, SystemMethodInfo methodInfo) : this(targetObject, messageType, methodInfo, false)
            { }

            public MessageCallSyntaxInfo(CBase targetObject, SystemType messageType, SystemMethodInfo methodInfo, bool automatically) : this(targetObject, 0, messageType, methodInfo, automatically)
            { }

            private MessageCallSyntaxInfo(CBase targetObject, int opcode, SystemType messageType, SystemMethodInfo methodInfo, bool automatically) : base(targetObject, methodInfo, automatically)
            {
                _opcode = opcode;
                _messageType = messageType;
            }

            /// <summary>
            /// 消息回调的调度函数
            /// </summary>
            /// <param name="message">消息对象实例</param>
            public void Invoke(ProtoBuf.Extension.IMessage message)
            {
                if (_isExtensionType)
                {
                    if (_isNullParameterType)
                    {
                        _callback.DynamicInvoke(_targetObject);
                    }
                    else
                    {
                        _callback.DynamicInvoke(_targetObject, message);
                    }
                }
                else
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
            }
        }

        #endregion

        #region 包装类型回调信息类自动绑定数据管理函数接口

        /// <summary>
        /// 处理所有包装类型回调信息数据
        /// </summary>
        /// <typeparam name="RegType">注册类型</typeparam>
        /// <typeparam name="CallInfoType">回调信息类型</typeparam>
        /// <param name="container">回调信息数据容器</param>
        /// <param name="func">操作回调接口</param>
        private void OnAutomaticallyCallSyntaxInfoProcessHandler<RegType, CallInfoType>(IDictionary<RegType, IDictionary<string, CallInfoType>> container,
                                                                                        System.Action<RegType, string> func)
                                                                                        where CallInfoType : BaseCallSyntaxInfo
        {
            IDictionary<RegType, IList<string>> list = new Dictionary<RegType, IList<string>>();

            foreach (KeyValuePair<RegType, IDictionary<string, CallInfoType>> kvp in container)
            {
                IDictionary<string, CallInfoType> callInfos = kvp.Value;
                foreach (KeyValuePair<string, CallInfoType> kvp_info in callInfos)
                {
                    if (kvp_info.Value.Automatically)
                    {
                        if (false == list.TryGetValue(kvp.Key, out IList<string> infos))
                        {
                            infos = new List<string>();
                            list.Add(kvp.Key, infos);
                        }

                        if (infos.Contains(kvp_info.Key))
                        {
                            Debugger.Warn("The call info was already exist with target type '{0}' and name '{1}', repeat added it will override old value.", kvp.Key, kvp_info.Key);
                            infos.Remove(kvp_info.Key);
                        }

                        infos.Add(kvp_info.Key);
                    }
                }
            }

            if (list.Count > 0)
            {
                foreach (KeyValuePair<RegType, IList<string>> kvp in list)
                {
                    IList<string> infos = kvp.Value;
                    for (int n = 0; n < infos.Count; ++n)
                    {
                        func(kvp.Key, infos[n]);
                    }
                }
            }
        }

        #endregion
    }
}
