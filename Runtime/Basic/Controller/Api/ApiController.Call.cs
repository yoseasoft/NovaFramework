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
using SystemMethodInfo = System.Reflection.MethodInfo;
using SystemParameterInfo = System.Reflection.ParameterInfo;

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

            /// <summary>
            /// 接口调用类的扩展状态标识
            /// </summary>
            public bool IsExtension { get; set; }
            /// <summary>
            /// 接口调用类的扩展对象类型
            /// </summary>
            public SystemType ExtensionObjectType { get; set; }
            /// <summary>
            /// 接口调用类的参数类型列表
            /// </summary>
            public IList<SystemType> ParameterTypes { get; set; }
            /// <summary>
            /// 接口调用类的返回值类型
            /// </summary>
            public SystemType ReturnType { get; set; }
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
        /// <param name="obj">目标对象实例</param>
        /// <param name="functionName">功能名称</param>
        /// <param name="args">调用参数</param>
        public void CallAction(IBean obj, string functionName, params object[] args)
        {
            if (false == _functionCallInfos.TryGetValue(functionName, out ApiCallInfo info))
            {
                Debugger.Warn(LogGroupTag.Controller, "当前编程接口管理器的对象容器中无法检索到能匹配指定功能名称‘{%s}’的回调函数实例，调用该功能接口失败！", functionName);
                return;
            }

            if (info.ParameterTypes?.Count != args?.Length)
            {
                Debugger.Warn(LogGroupTag.Controller, "当前传入指定功能名称‘{%s}’的编程接口回调函数实例的参数与函数实际定义参数个数不匹配，调用该功能接口失败！", functionName);
                return;
            }

            if (info.IsExtension) // 扩展接口类型
            {
                if (null == obj)
                {
                    Debugger.Warn(LogGroupTag.Controller, "当前指定功能名称‘{%s}’的编程接口回调函数实例为扩展函数类型，需要指定扩展目标对象才可以正确调用该功能接口！", functionName);
                    return;
                }

                if (false == info.ExtensionObjectType.IsAssignableFrom(obj.GetType()))
                {
                    Debugger.Warn(LogGroupTag.Controller, "当前指定功能名称‘{%s}’的编程接口回调函数实例扩展的目标对象类型‘{%t}’与实际传入的实体对象类型‘{%t}’不匹配，调用该功能接口失败！",
                            functionName, info.ExtensionObjectType, obj);
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
            } // info.IsExtension
            else // 全局接口类型
            {
                info.Callback.DynamicInvoke(args);
            }
        }

        #region 编程接口的功能回调函数相关的操作接口

        /// <summary>
        /// 新增功能接口到当前接口管理容器中
        /// </summary>
        /// <param name="fullname">完整名称</param>
        /// <param name="targetType">目标类型</param>
        /// <param name="functionName">功能名称</param>
        /// <param name="callback">回调接口</param>
        /// <returns>返回功能接口对象实例</returns>
        private ApiCallInfo AddApiFunctionCallInfo(string fullname, SystemType targetType, string functionName, SystemDelegate callback)
        {
            ApiCallInfo info = new ApiCallInfo();
            info.Fullname = fullname;
            info.TargetType = targetType;
            info.FunctionName = functionName;
            info.Callback = callback;

            info.IsExtension = NovaEngine.Utility.Reflection.IsTypeOfExtension(callback.Method);

            int pos = 0;
            SystemParameterInfo[] parameters = callback.Method.GetParameters();
            if (info.IsExtension)
            {
                // 如果是扩展类型函数，则记录一下扩展的目标对象类型
                Debugger.Assert(parameters.Length > 0, "Invalid arguments.");
                info.ExtensionObjectType = parameters[pos].ParameterType;
                pos++;
            }

            if (null != parameters && parameters.Length > pos)
            {
                // 如果是扩展函数，这里记录的参数列表从第二个参数开始算
                info.ParameterTypes = new List<SystemType>();
                for (int n = pos; n < parameters.Length; ++n)
                {
                    info.ParameterTypes.Add(parameters[n].ParameterType);
                }
            }

            info.ReturnType = callback.Method.ReturnType;
            if (typeof(void) == info.ReturnType)
            {
                info.ReturnType = null;
            }

            if (_functionCallInfos.ContainsKey(functionName))
            {
                Debugger.Warn(LogGroupTag.Controller, "当前编程接口管理器的函数对象容器中已存在相同功能名称‘{%s}’的回调函数实例，重复添加将覆盖旧的功能数据。", functionName);
                _functionCallInfos.Remove(functionName);
            }

            Debugger.Info(LogGroupTag.Controller, "新增目标对象类型‘{%t}’的回调函数‘{%s}’到指定的功能名称‘{%s}’中。",
                    fullname, functionName, targetType);

            // 添加功能回调信息
            _functionCallInfos.Add(functionName, info);

            return info;
        }

        /// <summary>
        /// 从当前接口管理容器中查找指定名称的回调函数
        /// </summary>
        /// <param name="functionName">功能名称</param>
        /// <returns>返回功能接口函数对象实例</returns>
        private ApiCallInfo GetApiFunctionCallInfo(string functionName)
        {
            if (_functionCallInfos.ContainsKey(functionName))
            {
                return _functionCallInfos[functionName];
            }

            return null;
        }

        /// <summary>
        /// 从当前接口管理容器中移除指定功能对应的回调函数
        /// </summary>
        /// <param name="functionName">功能名称</param>
        private void RemoveApiFunctionCallInfo(string functionName)
        {
            if (false == _functionCallInfos.ContainsKey(functionName))
            {
                Debugger.Warn(LogGroupTag.Controller, "当前编程接口管理器的函数对象容器中无法检索到能匹配指定功能名称‘{%s}’的回调函数实例，移除该功能数据失败！", functionName);
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
