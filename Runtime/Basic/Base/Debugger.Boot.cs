/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyright (C) 2023 - 2024, Guangzhou Shiyue Network Technology Co., Ltd.
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
    /// 应用层提供的调试对象类，它是基于对<see cref="NovaEngine.Debugger"/>的便捷性接口封装
    /// </summary>
    public static partial class Debugger
    {
        /// <summary>
        /// 调试器对象启动函数
        /// </summary>
        internal static void Startup()
        {
            LoadConfig();

            // 初始化分组设置参数
            InitLogOutputGroupSettings();

            // 绑定调试输出
            BindingDebugOutputHandler();
        }

        /// <summary>
        /// 调试器对象关闭函数
        /// </summary>
        internal static void Shutdown()
        {
            // 重置调试输出
            ResettingDebugOutputHandler();

            // 移除分组设置参数
            RemoveAllLogOutputGroupSettings();

            UnloadConfig();
        }

        /// <summary>
        /// 应用层调试器对象的配置加载函数，用于对调试器内部状态进行初始设置
        /// </summary>
        private static void LoadConfig()
        {
            NovaEngine.Debugger.Instance.LoadConfig();

            if (NovaEngine.Environment.debugMode)
            {
                NovaEngine.Debugger.Instance.IsClassTypeVerificationEnabled = true;
                NovaEngine.Debugger.Instance.IsMethodInfoVerificationEnabled = true;
                NovaEngine.Debugger.Instance.IsParameterInfoVerificationEnabled = true;
                NovaEngine.Debugger.Instance.IsDebuggingVerificationAssertModeEnabled = true;
            }
        }

        /// <summary>
        /// 应用层调试器对象的配置卸载函数
        /// </summary>
        private static void UnloadConfig()
        {
            NovaEngine.Debugger.Instance.UnloadConfig();

            NovaEngine.Debugger.Instance.IsClassTypeVerificationEnabled = false;
            NovaEngine.Debugger.Instance.IsMethodInfoVerificationEnabled = false;
            NovaEngine.Debugger.Instance.IsParameterInfoVerificationEnabled = false;
            NovaEngine.Debugger.Instance.IsDebuggingVerificationAssertModeEnabled = false;
        }

        #region 调试器类的访问代理属性定义

        /// <summary>
        /// 调试模式下的输出回调接口
        /// </summary>
        private static NovaEngine.Debugger.OutputHandler_object s_logForObject;
        private static NovaEngine.Debugger.OutputHandler_string s_logForString;
        private static NovaEngine.Debugger.OutputHandler_cond_string s_logForCondString;
        private static NovaEngine.Debugger.OutputHandler_format_args s_logForFormatArgs;
        private static NovaEngine.Debugger.OutputHandler_cond_format_args s_logForCondFormatArgs;

        /// <summary>
        /// 信息模式下的输出回调接口
        /// </summary>
        private static NovaEngine.Debugger.OutputHandler_object s_infoForObject;
        private static NovaEngine.Debugger.OutputHandler_string s_infoForString;
        private static NovaEngine.Debugger.OutputHandler_cond_string s_infoForCondString;
        private static NovaEngine.Debugger.OutputHandler_format_args s_infoForFormatArgs;
        private static NovaEngine.Debugger.OutputHandler_cond_format_args s_infoForCondFormatArgs;

        /// <summary>
        /// 警告模式下的输出回调接口
        /// </summary>
        private static NovaEngine.Debugger.OutputHandler_object s_warnForObject;
        private static NovaEngine.Debugger.OutputHandler_string s_warnForString;
        private static NovaEngine.Debugger.OutputHandler_cond_string s_warnForCondString;
        private static NovaEngine.Debugger.OutputHandler_format_args s_warnForFormatArgs;
        private static NovaEngine.Debugger.OutputHandler_cond_format_args s_warnForCondFormatArgs;

        /// <summary>
        /// 错误模式下的输出回调接口
        /// </summary>
        private static NovaEngine.Debugger.OutputHandler_object s_errorForObject;
        private static NovaEngine.Debugger.OutputHandler_string s_errorForString;
        private static NovaEngine.Debugger.OutputHandler_cond_string s_errorForCondString;
        private static NovaEngine.Debugger.OutputHandler_format_args s_errorForFormatArgs;
        private static NovaEngine.Debugger.OutputHandler_cond_format_args s_errorForCondFormatArgs;

        /// <summary>
        /// 崩溃模式下的输出回调接口
        /// </summary>
        private static NovaEngine.Debugger.OutputHandler_object s_fatalForObject;
        private static NovaEngine.Debugger.OutputHandler_string s_fatalForString;
        private static NovaEngine.Debugger.OutputHandler_cond_string s_fatalForCondString;
        private static NovaEngine.Debugger.OutputHandler_format_args s_fatalForFormatArgs;
        private static NovaEngine.Debugger.OutputHandler_cond_format_args s_fatalForCondFormatArgs;

        /// <summary>
        /// 重置全部日志输出回调接口
        /// </summary>
        private static void ResettingDebugOutputHandler()
        {
            s_logForObject = Unity_Output;
            s_logForString = Unity_Output;
            s_logForCondString = Unity_Output;
            s_logForFormatArgs = Unity_Output;
            s_logForCondFormatArgs = Unity_Output;

            s_infoForObject = Unity_Output;
            s_infoForString = Unity_Output;
            s_infoForCondString = Unity_Output;
            s_infoForFormatArgs = Unity_Output;
            s_infoForCondFormatArgs = Unity_Output;

            s_warnForObject = Unity_Output;
            s_warnForString = Unity_Output;
            s_warnForCondString = Unity_Output;
            s_warnForFormatArgs = Unity_Output;
            s_warnForCondFormatArgs = Unity_Output;

            s_errorForObject = Unity_Output;
            s_errorForString = Unity_Output;
            s_errorForCondString = Unity_Output;
            s_errorForFormatArgs = Unity_Output;
            s_errorForCondFormatArgs = Unity_Output;

            s_fatalForObject = Unity_Output;
            s_fatalForString = Unity_Output;
            s_fatalForCondString = Unity_Output;
            s_fatalForFormatArgs = Unity_Output;
            s_fatalForCondFormatArgs = Unity_Output;
        }

        /// <summary>
        /// 绑定全部日志输出回调接口
        /// </summary>
        private static void BindingDebugOutputHandler()
        {
            s_logForObject = NovaEngine.Debugger.Log;
            s_logForString = NovaEngine.Debugger.Log;
            s_logForCondString = NovaEngine.Debugger.Log;
            s_logForFormatArgs = NovaEngine.Debugger.Log;
            s_logForCondFormatArgs = NovaEngine.Debugger.Log;

            s_infoForObject = NovaEngine.Debugger.Info;
            s_infoForString = NovaEngine.Debugger.Info;
            s_infoForCondString = NovaEngine.Debugger.Info;
            s_infoForFormatArgs = NovaEngine.Debugger.Info;
            s_infoForCondFormatArgs = NovaEngine.Debugger.Info;

            s_warnForObject = NovaEngine.Debugger.Warn;
            s_warnForString = NovaEngine.Debugger.Warn;
            s_warnForCondString = NovaEngine.Debugger.Warn;
            s_warnForFormatArgs = NovaEngine.Debugger.Warn;
            s_warnForCondFormatArgs = NovaEngine.Debugger.Warn;

            s_errorForObject = NovaEngine.Debugger.Error;
            s_errorForString = NovaEngine.Debugger.Error;
            s_errorForCondString = NovaEngine.Debugger.Error;
            s_errorForFormatArgs = NovaEngine.Debugger.Error;
            s_errorForCondFormatArgs = NovaEngine.Debugger.Error;

            s_fatalForObject = NovaEngine.Debugger.Fatal;
            s_fatalForString = NovaEngine.Debugger.Fatal;
            s_fatalForCondString = NovaEngine.Debugger.Fatal;
            s_fatalForFormatArgs = NovaEngine.Debugger.Fatal;
            s_fatalForCondFormatArgs = NovaEngine.Debugger.Fatal;
        }

        #endregion
    }
}
