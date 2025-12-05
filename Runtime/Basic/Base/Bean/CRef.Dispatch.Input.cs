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

using System;
using System.Collections.Generic;

namespace GameEngine
{
    /// 引用对象抽象类
    public abstract partial class CRef
    {
        /// <summary>
        /// 对象内部输入响应的编码管理容器
        /// </summary>
        private IList<int> _inputCodes;
        /// <summary>
        /// 对象内部输入响应的类型管理容器
        /// </summary>
        private IList<Type> _inputTypes;

        /// <summary>
        /// 引用对象的输入响应处理初始化函数接口
        /// </summary>
        private void OnInputResponseProcessingInitialize()
        {
            // 输入编码容器初始化
            _inputCodes = new List<int>();
            // 输入类型容器初始化
            _inputTypes = new List<Type>();
        }

        /// <summary>
        /// 引用对象的输入响应处理清理函数接口
        /// </summary>
        private void OnInputResponseProcessingCleanup()
        {
            // 移除所有输入响应
            Debugger.Assert(_inputCodes.Count == 0 && _inputTypes.Count == 0);
            _inputCodes = null;
            _inputTypes = null;
        }

        #region 基础对象输入响应相关处理函数的操作接口定义

        /// <summary>
        /// 发送输入编码到自己的输入管理器中进行派发
        /// </summary>
        /// <param name="inputCode">输入编码</param>
        /// <param name="operationType">输入操作类型</param>
        public void SimulateKeycodeForSelf(int inputCode, int operationType)
        {
            OnInputDispatchForCode(inputCode, operationType);
        }

        /// <summary>
        /// 发送输入数据到自己的输入管理器中进行派发
        /// </summary>
        /// <param name="inputData">输入数据</param>
        public void SimulateKeycodeForSelf<T>(T inputData) where T : struct
        {
            OnEventDispatchForType(inputData);
        }

        /// <summary>
        /// 用户自定义的输入处理函数，您可以通过重写该函数处理自定义输入行为
        /// </summary>
        /// <param name="inputCode">输入编码</param>
        /// <param name="operationType">输入操作类型</param>
        protected override void OnInput(int inputCode, int operationType) { }

        /// <summary>
        /// 用户自定义的输入处理函数，您可以通过重写该函数处理自定义输入行为
        /// </summary>
        /// <param name="inputData">输入数据</param>
        protected override void OnInput(object inputData) { }

        /// <summary>
        /// 针对指定输入编码新增输入响应的后处理程序
        /// </summary>
        /// <param name="inputCode">输入编码</param>
        /// <param name="operationType">输入操作类型</param>
        /// <returns>返回后处理的操作结果</returns>
        protected override bool OnInputResponseAddedActionPostProcess(int inputCode, int operationType)
        {
            return AddInputResponse(inputCode, operationType);
        }

        /// <summary>
        /// 针对指定输入类型新增输入响应的后处理程序
        /// </summary>
        /// <param name="inputType">输入类型</param>
        /// <returns>返回后处理的操作结果</returns>
        protected override bool OnInputResponseAddedActionPostProcess(Type inputType)
        {
            return AddInputResponse(inputType);
        }

        /// <summary>
        /// 针对指定输入编码移除输入响应的后处理程序
        /// </summary>
        /// <param name="inputCode">输入编码</param>
        /// <param name="operationType">输入操作类型</param>
        protected override void OnInputResponseRemovedActionPostProcess(int inputCode, int operationType)
        { }

        /// <summary>
        /// 针对指定输入类型移除输入响应的后处理程序
        /// </summary>
        /// <param name="inputType">输入类型</param>
        protected override void OnInputResponseRemovedActionPostProcess(Type inputType)
        { }

        /// <summary>
        /// 引用对象的输入响应函数接口，对一个指定的输入编码进行响应监听
        /// </summary>
        /// <param name="inputCode">输入编码</param>
        /// <param name="operationType">输入操作类型</param>
        /// <returns>若输入响应成功则返回true，否则返回false</returns>
        protected internal override sealed bool AddInputResponse(int inputCode, int operationType)
        {
            if (_inputCodes.Contains(inputCode))
            {
                Debugger.Warn("The 'CRef' instance input '{%d}' was already added, repeat do it failed.", inputCode);
                return true;
            }

            if (false == InputHandler.Instance.AddInputResponse(inputCode, this))
            {
                Debugger.Warn("The 'CRef' instance add input response '{%d}' failed.", inputCode);
                return false;
            }

            _inputCodes.Add(inputCode);

            return true;
        }

        /// <summary>
        /// 引用对象的输入响应函数接口，对一个指定的输入类型进行响应监听
        /// </summary>
        /// <param name="inputType">输入类型</param>
        /// <returns>若输入响应成功则返回true，否则返回false</returns>
        protected internal override sealed bool AddInputResponse(Type inputType)
        {
            if (_inputTypes.Contains(inputType))
            {
                // Debugger.Warn("The 'CRef' instance's input '{%t}' was already added, repeat do it failed.", inputType);
                return true;
            }

            if (false == InputHandler.Instance.AddInputResponse(inputType, this))
            {
                Debugger.Warn("The 'CRef' instance add input response '{%t}' failed.", inputType);
                return false;
            }

            _inputTypes.Add(inputType);

            return true;
        }

        /// <summary>
        /// 取消当前引用对象对指定输入的响应
        /// </summary>
        /// <param name="inputCode">输入编码</param>
        /// <param name="operationType">输入操作类型</param>
        protected internal override sealed void RemoveInputResponse(int inputCode, int operationType)
        {
            if (false == _inputCodes.Contains(inputCode))
            {
                // Debugger.Warn("Could not found any input '{%d}' for target 'CRef' instance with on added, do removed it failed.", inputCode);
                return;
            }

            InputHandler.Instance.RemoveInputResponse(inputCode, this);
            _inputCodes.Remove(inputCode);

            base.RemoveInputResponse(inputCode, operationType);
        }

        /// <summary>
        /// 取消当前引用对象对指定输入的响应
        /// </summary>
        /// <param name="inputType">输入类型</param>
        protected internal override sealed void RemoveInputResponse(Type inputType)
        {
            if (false == _inputTypes.Contains(inputType))
            {
                // Debugger.Warn("Could not found any input '{%t}' for target 'CRef' instance with on added, do removed it failed.", inputType);
                return;
            }

            InputHandler.Instance.RemoveInputResponse(inputType, this);
            _inputTypes.Remove(inputType);

            base.RemoveInputResponse(inputType);
        }

        /// <summary>
        /// 取消当前引用对象的所有输入响应
        /// </summary>
        public override sealed void RemoveAllInputResponses()
        {
            base.RemoveAllInputResponses();

            InputHandler.Instance.RemoveInputResponseForTarget(this);

            _inputCodes.Clear();
            _inputTypes.Clear();
        }

        #endregion
    }
}
