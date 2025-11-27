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
    /// 应用层提供的调试对象类
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
        private static NovaEngine.Debugger.OutputHandler_object _logForObject;
        private static NovaEngine.Debugger.OutputHandler_string _logForString;
        private static NovaEngine.Debugger.OutputHandler_cond_string _logForCondString;
        private static NovaEngine.Debugger.OutputHandler_format_args _logForFormatArgs;
        private static NovaEngine.Debugger.OutputHandler_cond_format_args _logForCondFormatArgs;

        /// <summary>
        /// 信息模式下的输出回调接口
        /// </summary>
        private static NovaEngine.Debugger.OutputHandler_object _infoForObject;
        private static NovaEngine.Debugger.OutputHandler_string _infoForString;
        private static NovaEngine.Debugger.OutputHandler_cond_string _infoForCondString;
        private static NovaEngine.Debugger.OutputHandler_format_args _infoForFormatArgs;
        private static NovaEngine.Debugger.OutputHandler_cond_format_args _infoForCondFormatArgs;

        /// <summary>
        /// 警告模式下的输出回调接口
        /// </summary>
        private static NovaEngine.Debugger.OutputHandler_object _warnForObject;
        private static NovaEngine.Debugger.OutputHandler_string _warnForString;
        private static NovaEngine.Debugger.OutputHandler_cond_string _warnForCondString;
        private static NovaEngine.Debugger.OutputHandler_format_args _warnForFormatArgs;
        private static NovaEngine.Debugger.OutputHandler_cond_format_args _warnForCondFormatArgs;

        /// <summary>
        /// 错误模式下的输出回调接口
        /// </summary>
        private static NovaEngine.Debugger.OutputHandler_object _errorForObject;
        private static NovaEngine.Debugger.OutputHandler_string _errorForString;
        private static NovaEngine.Debugger.OutputHandler_cond_string _errorForCondString;
        private static NovaEngine.Debugger.OutputHandler_format_args _errorForFormatArgs;
        private static NovaEngine.Debugger.OutputHandler_cond_format_args _errorForCondFormatArgs;

        /// <summary>
        /// 崩溃模式下的输出回调接口
        /// </summary>
        private static NovaEngine.Debugger.OutputHandler_object _fatalForObject;
        private static NovaEngine.Debugger.OutputHandler_string _fatalForString;
        private static NovaEngine.Debugger.OutputHandler_cond_string _fatalForCondString;
        private static NovaEngine.Debugger.OutputHandler_format_args _fatalForFormatArgs;
        private static NovaEngine.Debugger.OutputHandler_cond_format_args _fatalForCondFormatArgs;

        /// <summary>
        /// 重置全部日志输出回调接口
        /// </summary>
        private static void ResettingDebugOutputHandler()
        {
            _logForObject = Unity_Output;
            _logForString = Unity_Output;
            _logForCondString = Unity_Output;
            _logForFormatArgs = Unity_Output;
            _logForCondFormatArgs = Unity_Output;

            _infoForObject = Unity_Output;
            _infoForString = Unity_Output;
            _infoForCondString = Unity_Output;
            _infoForFormatArgs = Unity_Output;
            _infoForCondFormatArgs = Unity_Output;

            _warnForObject = Unity_Output;
            _warnForString = Unity_Output;
            _warnForCondString = Unity_Output;
            _warnForFormatArgs = Unity_Output;
            _warnForCondFormatArgs = Unity_Output;

            _errorForObject = Unity_Output;
            _errorForString = Unity_Output;
            _errorForCondString = Unity_Output;
            _errorForFormatArgs = Unity_Output;
            _errorForCondFormatArgs = Unity_Output;

            _fatalForObject = Unity_Output;
            _fatalForString = Unity_Output;
            _fatalForCondString = Unity_Output;
            _fatalForFormatArgs = Unity_Output;
            _fatalForCondFormatArgs = Unity_Output;
        }

        /// <summary>
        /// 绑定全部日志输出回调接口
        /// </summary>
        private static void BindingDebugOutputHandler()
        {
            _logForObject = NovaEngine.Debugger.Log;
            _logForString = NovaEngine.Debugger.Log;
            _logForCondString = NovaEngine.Debugger.Log;
            _logForFormatArgs = NovaEngine.Debugger.Log;
            _logForCondFormatArgs = NovaEngine.Debugger.Log;

            _infoForObject = NovaEngine.Debugger.Info;
            _infoForString = NovaEngine.Debugger.Info;
            _infoForCondString = NovaEngine.Debugger.Info;
            _infoForFormatArgs = NovaEngine.Debugger.Info;
            _infoForCondFormatArgs = NovaEngine.Debugger.Info;

            _warnForObject = NovaEngine.Debugger.Warn;
            _warnForString = NovaEngine.Debugger.Warn;
            _warnForCondString = NovaEngine.Debugger.Warn;
            _warnForFormatArgs = NovaEngine.Debugger.Warn;
            _warnForCondFormatArgs = NovaEngine.Debugger.Warn;

            _errorForObject = NovaEngine.Debugger.Error;
            _errorForString = NovaEngine.Debugger.Error;
            _errorForCondString = NovaEngine.Debugger.Error;
            _errorForFormatArgs = NovaEngine.Debugger.Error;
            _errorForCondFormatArgs = NovaEngine.Debugger.Error;

            _fatalForObject = NovaEngine.Debugger.Fatal;
            _fatalForString = NovaEngine.Debugger.Fatal;
            _fatalForCondString = NovaEngine.Debugger.Fatal;
            _fatalForFormatArgs = NovaEngine.Debugger.Fatal;
            _fatalForCondFormatArgs = NovaEngine.Debugger.Fatal;
        }

        #endregion
    }
}
