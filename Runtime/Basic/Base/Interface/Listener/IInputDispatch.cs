/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
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

namespace GameEngine
{
    /// <summary>
    /// 输入监听接口类，用于定义接收监听输入行为的函数接口
    /// </summary>
    public interface IInputDispatch : IListener
    {
        /// <summary>
        /// 对象内部输入的监听回调接口，通过该类型函数对指定输入编码进行监听处理
        /// </summary>
        /// <param name="inputCode">输入编码</param>
        /// <param name="operationType">输入操作类型</param>
        // public delegate void InputDispatchingListenerForCode(int inputCode, int operationType);

        /// <summary>
        /// 对象内部输入的监听回调接口，通过该类型函数对指定输入类型进行监听处理
        /// </summary>
        /// <param name="inputData">输入数据</param>
        // public delegate void InputDispatchingListenerForType(object inputData);

        /// <summary>
        /// 接收监听指定输入编码的回调接口函数
        /// </summary>
        /// <param name="inputCode">输入编码</param>
        /// <param name="operationType">输入操作类型</param>
        void OnInputDispatchForCode(int inputCode, int operationType);

        /// <summary>
        /// 接收监听指定输入类型的回调接口函数
        /// </summary>
        /// <param name="inputData">输入数据</param>
        void OnInputDispatchForType(object inputData);
    }
}
