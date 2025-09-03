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

using System.Collections.Generic;

using SystemType = System.Type;
using SystemDelegate = System.Delegate;

namespace GameEngine
{
    /// <summary>
    /// 标准定义接口的控制器类，对整个程序所有标准定义函数进行统一的整合和管理
    /// </summary>
    internal sealed partial class ApiController
    {
        /// <summary>
        /// 接口调用的数据信息类
        /// </summary>
        private class ApiCallInfo
        {
            /// <summary>
            /// 接口调用类的完整名称
            /// </summary>
            public string Fullname { get; set; }
            /// <summary>
            /// 接口调用类的目标对象类型
            /// </summary>
            public SystemType TargetType { get; set; }
            /// <summary>
            /// 接口调用类的功能名称
            /// </summary>
            public string FunctionName { get; set; }
            /// <summary>
            /// 接口调用类的回调函数句柄
            /// </summary>
            public SystemDelegate Callback { get; set; }
        }

        /// <summary>
        /// 功能调度接口的数据结构容器
        /// </summary>
        private IDictionary<string, ApiCallInfo> _functionCallInfos = null;

        /// <summary>
        /// 编程接口调度相关内容的初始化回调函数
        /// </summary>
        [OnControllerSubmoduleInitCallback]
        private void InitializeForApiCall()
        {
            // 数据容器初始化
            _functionCallInfos = new Dictionary<string, ApiCallInfo>();
        }

        /// <summary>
        /// 编程接口调度相关内容的清理回调函数
        /// </summary>
        [OnControllerSubmoduleCleanupCallback]
        private void CleanupForApiCall()
        {
            RemoveAllApiFunctionCallInfos();

            // 移除数据容器
            _functionCallInfos = null;
        }

        /// <summary>
        /// 功能模块回调接口函数
        /// </summary>
        /// <param name="functionName">功能名称</param>
        /// <param name="args">调用参数</param>
        public void Call(string functionName, params object[] args)
        {
            if (false == _functionCallInfos.TryGetValue(functionName, out ApiCallInfo info))
            {
                Debugger.Warn(LogGroupTag.Controller, "调用编程接口回调函数异常：当前编程接口管理器的对象容器中无法检索到能匹配指定功能名称‘{%s}’的回调函数实例，调用该功能接口失败！", functionName);
                return;
            }

            info.Callback.DynamicInvoke(args);
        }

        /// <summary>
        /// 功能模块回调接口函数
        /// </summary>
        /// <param name="obj">目标对象实例</param>
        /// <param name="functionName">功能名称</param>
        /// <param name="args">调用参数</param>
        public void Call(IBean obj, string functionName, params object[] args)
        {
            if (false == _functionCallInfos.TryGetValue(functionName, out ApiCallInfo info))
            {
                Debugger.Warn(LogGroupTag.Controller, "调用编程接口回调函数异常：当前编程接口管理器的对象容器中无法检索到能匹配指定功能名称‘{%s}’的回调函数实例，调用该功能接口失败！", functionName);
                return;
            }

            if (null == args || args.Length == 0)
            {
                info.Callback.DynamicInvoke(obj);
                return;
            }

            object[] new_args = new object[args.Length + 1];
            new_args[0] = obj;
            System.Array.Copy(args, 0, new_args, 1, args.Length);
            info.Callback.DynamicInvoke(new_args);
        }

        #region 编程接口的功能回调函数相关的管理接口（新增，移除操作）

        private void AddApiFunctionCallInfo(string fullname, SystemType targetType, string functionName, SystemDelegate callback)
        {
            ApiCallInfo info = new ApiCallInfo();
            info.Fullname = fullname;
            info.TargetType = targetType;
            info.FunctionName = functionName;
            info.Callback = callback;

            Debugger.Info(LogGroupTag.Controller, "Add new api call '{%s}' to target function '{%s}' of the class type '{%t}'.",
                    fullname, functionName, targetType);

            if (_functionCallInfos.ContainsKey(functionName))
            {
                Debugger.Warn(LogGroupTag.Controller, "新增编程接口回调函数异常：当前编程接口管理器的对象容器中已存在相同功能名称‘{%s}’的回调函数实例，重复添加将覆盖旧的功能数据。", functionName);
                _functionCallInfos.Remove(functionName);
            }

            // 添加切面回调信息
            _functionCallInfos.Add(functionName, info);
        }

        /// <summary>
        /// 从当前接口管理容器中移除指定功能对应的回调函数
        /// </summary>
        /// <param name="functionName">功能名称</param>
        private void RemoveApiFunctionCallInfo(string functionName)
        {
            if (false == _functionCallInfos.ContainsKey(functionName))
            {
                Debugger.Warn(LogGroupTag.Controller, "移除编程接口回调函数异常：当前编程接口管理器的对象容器中无法检索到能匹配指定功能名称‘{%s}’的回调函数实例，移除该功能数据失败！", functionName);
                return;
            }

            _functionCallInfos.Remove(functionName);
        }

        /// <summary>
        /// 移除当前接口管理容器中的所有功能接口回调函数
        /// </summary>
        private void RemoveAllApiFunctionCallInfos()
        {
            _functionCallInfos.Clear();
        }

        #endregion
    }
}
