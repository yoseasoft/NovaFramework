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
        /// 基础对象内部输入编码的响应回调映射列表
        /// </summary>
        private IDictionary<int, IDictionary<string, bool>> _inputResponseCallsForCode;
        /// <summary>
        /// 基础对象内部输入类型的响应回调映射列表
        /// </summary>
        private IDictionary<SystemType, IDictionary<string, bool>> _inputResponseCallsForType;

        /// <summary>
        /// 基础对象的输入响应回调初始化函数接口
        /// </summary>
        private void OnInputResponseCallInitialize()
        {
            // 输入响应回调映射容器初始化
            _inputResponseCallsForCode = new Dictionary<int, IDictionary<string, bool>>();
            _inputResponseCallsForType = new Dictionary<SystemType, IDictionary<string, bool>>();
        }

        /// <summary>
        /// 基础对象的输入响应回调清理函数接口
        /// </summary>
        private void OnInputResponseCallCleanup()
        {
            // 移除所有输入响应
            RemoveAllInputResponses();

            _inputResponseCallsForCode = null;
            _inputResponseCallsForType = null;
        }

        #region 基础对象输入响应相关回调函数的操作接口定义

        /// <summary>
        /// 基础对象的输入编码的监听回调函数<br/>
        /// 该函数针对输入响应接口的标准实现，禁止子类重写该函数<br/>
        /// 若子类需要根据需要自行解析输入编码，可以通过重写<see cref="GameEngine.CBase.OnInput(int, int)"/>实现输入编码的自定义处理逻辑
        /// </summary>
        /// <param name="inputCode">输入编码</param>
        /// <param name="operationType">输入操作类型</param>
        public virtual void OnInputDispatchForCode(int inputCode, int operationType)
        {
            if (_inputResponseCallsForCode.TryGetValue(inputCode, out IDictionary<string, bool> calls))
            {
                SystemType classType = GetType();
                foreach (KeyValuePair<string, bool> kvp in calls)
                {
                    InputHandler.Instance.InvokeInputResponseBindingCall(kvp.Key, classType, this, inputCode, operationType);
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
            if (_inputResponseCallsForType.TryGetValue(inputData.GetType(), out IDictionary<string, bool> calls))
            {
                SystemType classType = GetType();
                foreach (KeyValuePair<string, bool> kvp in calls)
                {
                    InputHandler.Instance.InvokeInputResponseBindingCall(kvp.Key, classType, this, inputData);
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
            if (_inputResponseCallsForCode.ContainsKey(inputCode) && _inputResponseCallsForCode[inputCode].Count > 0)
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
            if (_inputResponseCallsForType.ContainsKey(inputType) && _inputResponseCallsForType[inputType].Count > 0)
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
        protected internal virtual bool AddInputResponse(int inputCode, int operationType)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// 基础对象的输入响应函数接口，将一个指定的输入绑定到给定的监听回调函数上
        /// </summary>
        /// <param name="fullname">函数名称</param>
        /// <param name="methodInfo">监听回调函数</param>
        /// <param name="inputCode">输入编码</param>
        /// <param name="operationType">输入操作类型</param>
        /// <returns>若输入响应成功则返回true，否则返回false</returns>
        public bool AddInputResponse(string fullname, SystemMethodInfo methodInfo, int inputCode, int operationType)
        {
            return AddInputResponse(fullname, methodInfo, inputCode, operationType, false);
        }

        /// <summary>
        /// 基础对象的输入响应函数接口，将一个指定的输入绑定到给定的监听回调函数上
        /// </summary>
        /// <param name="fullname">函数名称</param>
        /// <param name="methodInfo">监听回调函数</param>
        /// <param name="inputCode">输入编码</param>
        /// <param name="operationType">输入操作类型</param>
        /// <param name="automatically">自动装载状态标识</param>
        /// <returns>若输入响应成功则返回true，否则返回false</returns>
        protected internal bool AddInputResponse(string fullname, SystemMethodInfo methodInfo, int inputCode, int operationType, bool automatically)
        {
            InputHandler.Instance.AddInputResponseBindingCallInfo(fullname, GetType(), methodInfo, inputCode, operationType, automatically);

            if (false == _inputResponseCallsForCode.TryGetValue(inputCode, out IDictionary<string, bool> calls))
            {
                // 创建回调列表
                calls = new Dictionary<string, bool>();
                calls.Add(fullname, automatically);

                _inputResponseCallsForCode.Add(inputCode, calls);

                // 新增输入响应的后处理程序
                return OnInputResponseAddedActionPostProcess(inputCode, operationType);
            }

            if (calls.ContainsKey(fullname))
            {
                Debugger.Warn("The '{0}' instance's input '{1}' was already add same listener by name '{2}', repeat do it failed.",
                        NovaEngine.Utility.Text.ToString(GetType()), inputCode, fullname);
                return false;
            }

            calls.Add(fullname, automatically);

            return true;
        }

        /// <summary>
        /// 基础对象的输入响应函数接口，对一个指定的类型进行响应监听
        /// </summary>
        /// <typeparam name="T">输入类型</typeparam>
        /// <returns>若输入响应成功则返回true，否则返回false</returns>
        protected internal bool AddInputResponse<T>() where T : struct
        {
            return AddInputResponse(typeof(T));
        }

        /// <summary>
        /// 基础对象的输入响应函数接口，对一个指定的类型进行响应监听
        /// </summary>
        /// <param name="inputType">输入类型</param>
        /// <returns>若输入响应成功则返回true，否则返回false</returns>
        protected internal virtual bool AddInputResponse(SystemType inputType)
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
            string fullname = NovaEngine.Utility.Text.GetFullName(func.Method);
            return AddInputResponse(fullname, func.Method, typeof(T));
        }

        /// <summary>
        /// 基础对象的输入响应函数接口，将一个指定的输入绑定到给定的监听回调函数上
        /// </summary>
        /// <param name="fullname">函数名称</param>
        /// <param name="methodInfo">监听回调函数</param>
        /// <param name="inputType">输入类型</param>
        /// <returns>若输入响应成功则返回true，否则返回false</returns>
        public bool AddInputResponse(string fullname, SystemMethodInfo methodInfo, SystemType inputType)
        {
            return AddInputResponse(fullname, methodInfo, inputType, false);
        }

        /// <summary>
        /// 基础对象的输入响应函数接口，将一个指定的输入绑定到给定的监听回调函数上
        /// </summary>
        /// <param name="fullname">函数名称</param>
        /// <param name="methodInfo">监听回调函数</param>
        /// <param name="inputType">输入类型</param>
        /// <param name="automatically">自动装载状态标识</param>
        /// <returns>若输入响应成功则返回true，否则返回false</returns>
        protected internal bool AddInputResponse(string fullname, SystemMethodInfo methodInfo, SystemType inputType, bool automatically)
        {
            InputHandler.Instance.AddInputResponseBindingCallInfo(fullname, GetType(), methodInfo, inputType, automatically);

            if (false == _inputResponseCallsForType.TryGetValue(inputType, out IDictionary<string, bool> calls))
            {
                // 创建回调列表
                calls = new Dictionary<string, bool>();
                calls.Add(fullname, automatically);

                _inputResponseCallsForType.Add(inputType, calls);

                // 新增输入响应的后处理程序
                return OnInputResponseAddedActionPostProcess(inputType);
            }

            if (calls.ContainsKey(fullname))
            {
                Debugger.Warn("The '{0}' instance's input '{1}' was already add same listener by name '{2}', repeat do it failed.",
                        NovaEngine.Utility.Text.ToString(GetType()), inputType.FullName, fullname);
                return false;
            }

            calls.Add(fullname, automatically);

            return true;
        }

        /// <summary>
        /// 取消当前基础对象对指定编码的响应
        /// </summary>
        /// <param name="inputCode">输入编码</param>
        protected internal void RemoveInputResponse(int inputCode)
        {
            RemoveInputResponse(inputCode, 0);
        }

        /// <summary>
        /// 取消当前基础对象对指定编码的响应
        /// </summary>
        /// <param name="inputCode">输入编码</param>
        /// <param name="operationType">输入操作类型</param>
        protected internal virtual void RemoveInputResponse(int inputCode, int operationType)
        {
            // 若针对特定编码绑定了监听回调，则移除相应的回调句柄
            if (_inputResponseCallsForCode.ContainsKey(inputCode))
            {
                _inputResponseCallsForCode.Remove(inputCode);
            }

            // 移除输入响应的后处理程序
            OnInputResponseRemovedActionPostProcess(inputCode, operationType);
        }

        /// <summary>
        /// 取消当前基础对象对指定编码的监听回调函数
        /// </summary>
        /// <param name="inputCode">输入编码</param>
        /// <param name="fullname">函数名称</param>
        protected internal void RemoveInputResponse(string fullname, int inputCode)
        {
            RemoveInputResponse(fullname, inputCode, 0);
        }

        /// <summary>
        /// 取消当前基础对象对指定编码的监听回调函数
        /// </summary>
        /// <param name="fullname">函数名称</param>
        /// <param name="inputCode">输入编码</param>
        /// <param name="operationType">输入操作类型</param>
        protected internal void RemoveInputResponse(string fullname, int inputCode, int operationType)
        {
            if (_inputResponseCallsForCode.TryGetValue(inputCode, out IDictionary<string, bool> calls))
            {
                if (calls.ContainsKey(fullname))
                {
                    calls.Remove(fullname);
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
        protected internal void RemoveInputResponse<T>()
        {
            RemoveInputResponse(typeof(T));
        }

        /// <summary>
        /// 取消当前基础对象对指定输入类型的响应
        /// </summary>
        /// <param name="inputType">输入类型</param>
        protected internal virtual void RemoveInputResponse(SystemType inputType)
        {
            // 若针对特定输入绑定了监听回调，则移除相应的回调句柄
            if (_inputResponseCallsForType.ContainsKey(inputType))
            {
                _inputResponseCallsForType.Remove(inputType);
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
            RemoveInputResponse(methodInfo, typeof(T));
        }

        /// <summary>
        /// 取消当前基础对象对指定输入类型的响应
        /// </summary>
        /// <param name="methodInfo">监听回调函数</param>
        /// <param name="inputType">输入类型</param>
        public void RemoveInputResponse(SystemMethodInfo methodInfo, SystemType inputType)
        {
            string fullname = _Generator.GenUniqueName(methodInfo);

            RemoveInputResponse(fullname, inputType);
        }

        /// <summary>
        /// 取消当前基础对象对指定输入类型的监听回调函数
        /// </summary>
        /// <typeparam name="T">输入类型</typeparam>
        /// <param name="fullname">函数名称</param>
        protected internal void RemoveInputResponse<T>(string fullname)
        {
            RemoveInputResponse(fullname, typeof(T));
        }

        /// <summary>
        /// 取消当前基础对象对指定输入类型的监听回调函数
        /// </summary>
        /// <param name="fullname">函数名称</param>
        /// <param name="inputType">输入类型</param>
        protected internal void RemoveInputResponse(string fullname, SystemType inputType)
        {
            if (_inputResponseCallsForType.TryGetValue(inputType, out IDictionary<string, bool> calls))
            {
                if (calls.ContainsKey(fullname))
                {
                    calls.Remove(fullname);
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
            OnAutomaticallyCallSyntaxInfoProcessHandler<int>(_inputResponseCallsForCode, RemoveInputResponse);
            OnAutomaticallyCallSyntaxInfoProcessHandler<SystemType>(_inputResponseCallsForType, RemoveInputResponse);
        }

        /// <summary>
        /// 取消当前基础对象的所有输入响应
        /// </summary>
        public virtual void RemoveAllInputResponses()
        {
            IList<int> id_keys = NovaEngine.Utility.Collection.ToListForKeys<int, IDictionary<string, bool>>(_inputResponseCallsForCode);
            if (null != id_keys)
            {
                int c = id_keys.Count;
                for (int n = 0; n < c; ++n) { RemoveInputResponse(id_keys[n]); }
            }

            IList<SystemType> type_keys = NovaEngine.Utility.Collection.ToListForKeys<SystemType, IDictionary<string, bool>>(_inputResponseCallsForType);
            if (null != type_keys)
            {
                int c = type_keys.Count;
                for (int n = 0; n < c; ++n) { RemoveInputResponse(type_keys[n]); }
            }

            _inputResponseCallsForCode.Clear();
            _inputResponseCallsForType.Clear();
        }

        #endregion
    }
}
