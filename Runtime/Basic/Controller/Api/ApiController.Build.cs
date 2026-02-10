/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
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
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine.Scripting;

namespace GameEngine
{
    /// 标准定义接口的控制器类
    internal sealed partial class ApiController
    {
        /// <summary>
        /// 表达式构建的数据信息类
        /// </summary>
        private class ApiBuildInfo
        {
            /// <summary>
            /// 表达式对象实例
            /// </summary>
            public Api.Expression.ApiCallExpression Expression { get; set; }
        }

        /// <summary>
        /// 表达式构建接口的数据结构容器
        /// </summary>
        private IDictionary<int, ApiBuildInfo> _expressionBuildInfos;

        /// <summary>
        /// 编程接口构建相关内容的初始化回调函数
        /// </summary>
        [Preserve]
        [OnControllerSubmoduleInitCallback]
        private void InitializeForApiBuild()
        {
            // 数据容器初始化
            _expressionBuildInfos = new Dictionary<int, ApiBuildInfo>();
        }

        /// <summary>
        /// 编程接口构建相关内容的清理回调函数
        /// </summary>
        [Preserve]
        [OnControllerSubmoduleCleanupCallback]
        private void CleanupForApiBuild()
        {
            RemoveAllApiExpressionBuildInfos();

            // 移除数据容器
            _expressionBuildInfos = null;
        }

        /// <summary>
        /// 执行指定序列标识对应的表达式对象实例
        /// 该表达式对象实例是通过配置数据构建的，仅在第一次调用是进行构建操作
        /// </summary>
        /// <param name="obj">原型对象实例</param>
        /// <param name="serial">序列标识</param>
        /// <param name="text">配置数据</param>
        public void Exec(IBean obj, int serial, string text)
        {
            if (false == _expressionBuildInfos.TryGetValue(serial, out ApiBuildInfo info))
            {
                Api.Expression.ApiCallExpression expression = BuildExpression(text);

                if (null == expression)
                {
                    Debugger.Warn(LogGroupTag.Controller, "通过指定的配置数据‘{%s}’未能正确构建表达式对象实例，请检查配置数据是否存在非法格式！", text);
                    return;
                }

                info = AddApiExpressionBuildInfo(serial, expression);
            }

            info.Expression.Invoke(obj);
        }

        const string ApiCallMethodResolvePattern = @"^(\w+)\(([^)]*)\)$";

        /// <summary>
        /// 通过指定的配置数据构建表达式对象实例
        /// </summary>
        /// <param name="text">配置数据</param>
        /// <returns>返回构建的表达式对象实例，若构建失败则返回null</returns>
        private Api.Expression.ApiCallExpression BuildExpression(string text)
        {
            if (string.IsNullOrEmpty(text))
                return null;

            Api.Expression.ApiCallGroup group = new Api.Expression.ApiCallGroup();

            string line;
            TextReader reader = new StringReader(text);
            while (null != (line = reader.ReadLine()))
            {
                line = line.Trim();

                // MatchCollection matches = Regex.Matches(line, ApiCallMethodResolvePattern, RegexOptions.Multiline);
                // foreach (Match match in matches) {}

                // 正则表达式匹配的结果有三个值
                // 例如：line = @"function(param1, param2)"
                // 则匹配的 match 值为：
                // match.Groups[0].value = @"function(param1, param2)"
                // match.Groups[1].value = @"function"
                // match.Groups[2].value = @"param1, param2"
                Match match = Regex.Match(line, ApiCallMethodResolvePattern, RegexOptions.Multiline);
                if (match.Groups.Count < 3)
                {
                    Debugger.Warn(LogGroupTag.Controller, "编程接口传入的配置数据‘{%s}’不是一个有效的功能函数定义格式，无法正确解析为功能回调函数实例！", line);
                    continue;
                }

                string functionName = match.Groups[1].Value;
                string strParameters = match.Groups[2].Value;

                ApiCallInfo call_info = GetApiFunctionCallInfo(functionName);
                if (null == call_info)
                {
                    Debugger.Warn(LogGroupTag.Controller, "编程接口传入的配置数据‘{%s}’中含有非法的功能函数名称‘{%s}’，无法正确解析该功能回调函数实例！", line, functionName);
                    continue;
                }

                string[] strParameterArray = null;
                if (false == string.IsNullOrEmpty(strParameters))
                {
                    strParameterArray = strParameters.Split(NovaEngine.Definition.CCharacter.Comma, StringSplitOptions.RemoveEmptyEntries);
                }

                if (strParameterArray?.Length != call_info.ParameterTypes?.Count)
                {
                    Debugger.Warn(LogGroupTag.Controller, "编程接口传入的配置数据‘{%s}’中包含的参数个数‘{%d}’与功能接口函数定义的参数个数‘{%d}’不匹配，无法正确解析该功能回调函数实例！",
                            line, strParameterArray?.Length, call_info.ParameterTypes?.Count);
                    for (int n = 0; n < call_info.ParameterTypes.Count; ++n)
                    {
                        Debugger.Warn(LogGroupTag.Controller, "参数‘{%d}’的类型为‘{%t}’", n, call_info.ParameterTypes[n]);
                    }
                    continue;
                }

                Api.Expression.ApiCallMethod method = new Api.Expression.ApiCallMethod();
                method.FunctionName = functionName;

                if (null != call_info.ParameterTypes && call_info.ParameterTypes.Count > 0)
                {
                    int c = call_info.ParameterTypes.Count;
                    object[] parameterList = new object[c];
                    for (int n = 0; n < c; ++n)
                    {
                        string strValue = strParameterArray[n].Trim();
                        parameterList[n] = NovaEngine.Utility.Convertion.StringToTargetType(strValue, call_info.ParameterTypes[n]);
                    }

                    method.ParameterValues = parameterList;
                }

                group.AddExpression(method);
            }

            // 关闭文本流
            reader.Dispose();

            return group;
        }

        #region 编程接口的表达式构建相关的操作接口

        /// <summary>
        /// 新增表达式对象实例到当前构建管理容器中
        /// </summary>
        /// <param name="serial">序列标识</param>
        /// <param name="expression">表达式对象实例</param>
        /// <returns>返回表达式构建对象实例</returns>
        private ApiBuildInfo AddApiExpressionBuildInfo(int serial, Api.Expression.ApiCallExpression expression)
        {
            ApiBuildInfo info = new ApiBuildInfo();
            info.Expression = expression;

            if (_expressionBuildInfos.ContainsKey(serial))
            {
                Debugger.Warn(LogGroupTag.Controller, "当前编程接口管理器的表达式对象容器中已存在相同序列标识‘{%d}’的表达式对象实例，重复添加将覆盖旧的功能数据。", serial);
                _expressionBuildInfos.Remove(serial);
            }

            Debugger.Info(LogGroupTag.Controller, "新增编程接口表达式构建对象实例‘{%d}’到当前构建管理列表中。", serial);

            // 添加表达式对象信息
            _expressionBuildInfos.Add(serial, info);

            return info;
        }

        /// <summary>
        /// 从当前构建管理容器中移除指定标识对应的表达式对象实例
        /// </summary>
        /// <param name="serial">序列标识</param>
        private void RemoveApiExpressionBuildInfo(int serial)
        {
            if (false == _expressionBuildInfos.ContainsKey(serial))
            {
                Debugger.Warn(LogGroupTag.Controller, "当前编程接口管理器的表达式对象容器中无法检索到能匹配指定标识‘{%s}’的表达式对象实例，移除该表达式数据失败！", serial);
                return;
            }

            _expressionBuildInfos.Remove(serial);
        }

        /// <summary>
        /// 移除当前构建管理容器中的所有表达式对象实例
        /// </summary>
        private void RemoveAllApiExpressionBuildInfos()
        {
            _expressionBuildInfos.Clear();
        }

        #endregion
    }
}
