/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
/// Copyright (C) 2023, Guangzhou Shiyue Network Technology Co., Ltd.
/// Copyright (C) 2025, Hurley, Independent Studio.
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

namespace NovaEngine
{
    /// <summary>
    /// 调试器对象工具类，用于引擎内部调试控制及输出相关接口声明
    /// </summary>
    internal partial class Debugger
    {
        /// <summary>
        /// 日志输出规范定义代理句柄接口
        /// </summary>
        protected internal delegate void OutputHandler_object(object message);
        protected internal delegate void OutputHandler_string(string message);
        protected internal delegate void OutputHandler_cond_string(bool cond, string message);
        protected internal delegate void OutputHandler_format_args(string format, params object[] args);
        protected internal delegate void OutputHandler_cond_format_args(bool cond, string format, params object[] args);

        protected internal delegate void OutputHandler_level_object(LogOutputLevelType level, object message);
        protected internal delegate void OutputHandler_level_string(LogOutputLevelType level, string message);
        protected internal delegate void OutputHandler_level_format_args(LogOutputLevelType level, string format, params object[] args);

        /// <summary>
        /// 断言处理规范定义代理句柄接口
        /// </summary>
        protected internal delegate void AssertHandler_empty(bool condition);
        protected internal delegate void AssertHandler_object(bool condition, object message);
        protected internal delegate void AssertHandler_string(bool condition, string message);
        protected internal delegate void AssertHandler_format_args(bool condition, string format, params object[] args);

        /// <summary>
        /// 异常处理规范定义代理句柄接口
        /// </summary>
        protected internal delegate void ThrowHandler_empty();
        protected internal delegate void ThrowHandler_code(int errorCode);
        protected internal delegate void ThrowHandler_string(string message);
        protected internal delegate void ThrowHandler_format_args(string format, params object[] args);
        protected internal delegate void ThrowHandler_exception(System.Exception exception);
        protected internal delegate void ThrowHandler_type(System.Type type);
        protected internal delegate void ThrowHandler_type_string(System.Type type, string message);
        protected internal delegate void ThrowHandler_type_format_args(System.Type type, string format, params object[] args);

        /// <summary>
        /// 调试模式下的输出回调接口
        /// </summary>
        private OutputHandler_object      _log_object;
        private OutputHandler_string      _log_string;
        private OutputHandler_format_args _log_format_args;

        /// <summary>
        /// 信息模式下的输出回调接口
        /// </summary>
        private OutputHandler_object      _info_object;
        private OutputHandler_string      _info_string;
        private OutputHandler_format_args _info_format_args;

        /// <summary>
        /// 警告模式下的输出回调接口
        /// </summary>
        private OutputHandler_object      _warn_object;
        private OutputHandler_string      _warn_string;
        private OutputHandler_format_args _warn_format_args;

        /// <summary>
        /// 错误模式下的输出回调接口
        /// </summary>
        private OutputHandler_object      _error_object;
        private OutputHandler_string      _error_string;
        private OutputHandler_format_args _error_format_args;

        /// <summary>
        /// 崩溃模式下的输出回调接口
        /// </summary>
        private OutputHandler_object      _fatal_object;
        private OutputHandler_string      _fatal_string;
        private OutputHandler_format_args _fatal_format_args;

        /// <summary>
        /// 自定义模式下的输出回调接口
        /// </summary>
        private OutputHandler_level_object      _output_object;
        private OutputHandler_level_string      _output_string;
        private OutputHandler_level_format_args _output_format_args;

        /// <summary>
        /// 调试模型下的断言回调接口
        /// </summary>
        private AssertHandler_empty       _assert_empty;
        private AssertHandler_object      _assert_object;
        private AssertHandler_string      _assert_string;
        private AssertHandler_format_args _assert_format_args;

        /// <summary>
        /// 调试模型下的异常回调接口
        /// </summary>
        private ThrowHandler_empty            _throw_empty;
        private ThrowHandler_code             _throw_code;
        private ThrowHandler_string           _throw_string;
        private ThrowHandler_format_args      _throw_format_args;
        private ThrowHandler_exception        _throw_exception;
        private ThrowHandler_type             _throw_type;
        private ThrowHandler_type_string      _throw_type_string;
        private ThrowHandler_type_format_args _throw_type_format_args;

        /// <summary>
        /// 空置版本的日志输出接口，用于忽略指定级别类型对应的输出回调
        /// </summary>
        /// <param name="message">日志内容</param>
        private static void Blank_Output(object message) { }

        /// <summary>
        /// 空置版本的日志输出接口，用于忽略指定级别类型对应的输出回调
        /// </summary>
        /// <param name="message">日志内容</param>
        private static void Blank_Output(string message) { }

        /// <summary>
        /// 空置版本的日志输出接口，用于忽略指定级别类型对应的输出回调
        /// </summary>
        /// <param name="format">日志格式内容</param>
        /// <param name="args">日志格式化参数</param>
        private static void Blank_Output(string format, params object[] args) { }

        /// <summary>
        /// 控制版本的日志输出接口，用于忽略指定级别类型对应的输出回调
        /// </summary>
        /// <param name="level">日志基本</param>
        /// <param name="message">日志内容</param>
        private static void Blank_Output(LogOutputLevelType level, object message) { }

        /// <summary>
        /// 控制版本的日志输出接口，用于忽略指定级别类型对应的输出回调
        /// </summary>
        /// <param name="level">日志基本</param>
        /// <param name="message">日志内容</param>
        private static void Blank_Output(LogOutputLevelType level, string message) { }

        /// <summary>
        /// 控制版本的日志输出接口，用于忽略指定级别类型对应的输出回调
        /// </summary>
        /// <param name="level">日志基本</param>
        /// <param name="format">日志格式内容</param>
        /// <param name="args">日志格式化参数</param>
        private static void Blank_Output(LogOutputLevelType level, string format, params object[] args) { }

        /// <summary>
        /// 空置版本的日志断言接口，用于忽略对应的断言回调
        /// </summary>
        /// <param name="condition">条件表达式</param>
        private static void Blank_Assert(bool condition) { }

        /// <summary>
        /// 空置版本的日志断言接口，用于忽略对应的断言回调
        /// </summary>
        /// <param name="condition">条件表达式</param>
        /// <param name="message">日志内容</param>
        private static void Blank_Assert(bool condition, object message) { }

        /// <summary>
        /// 空置版本的日志断言接口，用于忽略对应的断言回调
        /// </summary>
        /// <param name="condition">条件表达式</param>
        /// <param name="message">日志内容</param>
        private static void Blank_Assert(bool condition, string message) { }

        /// <summary>
        /// 空置版本的日志断言接口，用于忽略对应的断言回调
        /// </summary>
        /// <param name="condition">条件表达式</param>
        /// <param name="format">日志格式内容</param>
        /// <param name="args">日志格式化参数</param>
        private static void Blank_Assert(bool condition, string format, params object[] args) { }

        /// <summary>
        /// 空置版本的日志异常接口，用于忽略对应的异常回调
        /// </summary>
        private static void Blank_Throw() { }

        /// <summary>
        /// 空置版本的日志异常接口，用于忽略对应的异常回调
        /// </summary>
        /// <param name="errorCode">错误码</param>
        private static void Blank_Throw(int errorCode) { }

        /// <summary>
        /// 空置版本的日志异常接口，用于忽略对应的异常回调
        /// </summary>
        /// <param name="message">日志内容</param>
        private static void Blank_Throw(string message) { }

        /// <summary>
        /// 空置版本的日志异常接口，用于忽略对应的异常回调
        /// </summary>
        /// <param name="format">日志格式内容</param>
        /// <param name="args">日志格式化参数</param>
        private static void Blank_Throw(string format, params object[] args) { }

        /// <summary>
        /// 空置版本的日志异常接口，用于忽略对应的异常回调
        /// </summary>
        /// <param name="exception">系统异常</param>
        private static void Blank_Throw(System.Exception exception) { }

        /// <summary>
        /// 空置版本的日志异常接口，用于忽略对应的异常回调
        /// </summary>
        /// <param name="type">异常类型</param>
        private static void Blank_Throw(System.Type type) { }

        /// <summary>
        /// 空置版本的日志异常接口，用于忽略对应的异常回调
        /// </summary>
        /// <param name="type">异常类型</param>
        /// <param name="message">日志内容</param>
        private static void Blank_Throw(System.Type type, string message) { }

        /// <summary>
        /// 空置版本的日志异常接口，用于忽略对应的异常回调
        /// </summary>
        /// <param name="type">异常类型</param>
        /// <param name="format">日志格式内容</param>
        /// <param name="args">日志格式化参数</param>
        private static void Blank_Throw(System.Type type, string format, params object[] args) { }

        /// <summary>
        /// 重新绑定全部日志输出回调接口为空置模式
        /// </summary>
        private void RebindingBlankOutputHandler()
        {
            _log_object = Blank_Output;
            _log_string = Blank_Output;
            _log_format_args = Blank_Output;

            _info_object = Blank_Output;
            _info_string = Blank_Output;
            _info_format_args = Blank_Output;

            _warn_object = Blank_Output;
            _warn_string = Blank_Output;
            _warn_format_args = Blank_Output;

            _error_object = Blank_Output;
            _error_string = Blank_Output;
            _error_format_args = Blank_Output;

            _fatal_object = Blank_Output;
            _fatal_string = Blank_Output;
            _fatal_format_args = Blank_Output;

            _output_object = Blank_Output;
            _output_string = Blank_Output;
            _output_format_args = Blank_Output;

            _assert_empty = Blank_Assert;
            _assert_object = Blank_Assert;
            _assert_string = Blank_Assert;
            _assert_format_args = Blank_Assert;

            _throw_empty = Blank_Throw;
            _throw_code = Blank_Throw;
            _throw_string = Blank_Throw;
            _throw_format_args = Blank_Throw;
            _throw_exception = Blank_Throw;
            _throw_type = Blank_Throw;
            _throw_type_string = Blank_Throw;
            _throw_type_format_args = Blank_Throw;
        }

        #region 调试器对象输出回调接口的Getter/Setter函数

        protected internal OutputHandler_object Log_object
        {
            set => _log_object = value;
            get => _log_object;
        }

        protected internal OutputHandler_string Log_string
        {
            set => _log_string = value;
            get => _log_string;
        }

        protected internal OutputHandler_format_args Log_format_args
        {
            set => _log_format_args = value;
            get => _log_format_args;
        }

        protected internal OutputHandler_object Info_object
        {
            set => _info_object = value;
            get => _info_object;
        }

        protected internal OutputHandler_string Info_string
        {
            set => _info_string = value;
            get => _info_string;
        }

        protected internal OutputHandler_format_args Info_format_args
        {
            set => _info_format_args = value;
            get => _info_format_args;
        }

        protected internal OutputHandler_object Warn_object
        {
            set => _warn_object = value;
            get => _warn_object;
        }

        protected internal OutputHandler_string Warn_string
        {
            set => _warn_string = value;
            get => _warn_string;
        }

        protected internal OutputHandler_format_args Warn_format_args
        {
            set => _warn_format_args = value;
            get => _warn_format_args;
        }

        protected internal OutputHandler_object Error_object
        {
            set => _error_object = value;
            get => _error_object;
        }

        protected internal OutputHandler_string Error_string
        {
            set => _error_string = value;
            get => _error_string;
        }

        protected internal OutputHandler_format_args Error_format_args
        {
            set => _error_format_args = value;
            get => _error_format_args;
        }

        protected internal OutputHandler_object Fatal_object
        {
            set => _fatal_object = value;
            get => _fatal_object;
        }

        protected internal OutputHandler_string Fatal_string
        {
            set => _fatal_string = value;
            get => _fatal_string;
        }

        protected internal OutputHandler_format_args Fatal_format_args
        {
            set => _fatal_format_args = value;
            get => _fatal_format_args;
        }

        protected internal OutputHandler_level_object Output_object
        {
            set => _output_object = value;
            get => _output_object;
        }

        protected internal OutputHandler_level_string Output_string
        {
            set => _output_string = value;
            get => _output_string;
        }

        protected internal OutputHandler_level_format_args Output_format_args
        {
            set => _output_format_args = value;
            get => _output_format_args;
        }

        protected internal AssertHandler_empty Assert_empty
        {
            set => _assert_empty = value;
            get => _assert_empty;
        }

        protected internal AssertHandler_object Assert_object
        {
            set => _assert_object = value;
            get => _assert_object;
        }

        protected internal AssertHandler_string Assert_string
        {
            set => _assert_string = value;
            get => _assert_string;
        }

        protected internal AssertHandler_format_args Assert_format_args
        {
            set => _assert_format_args = value;
            get => _assert_format_args;
        }

        protected internal ThrowHandler_empty Throw_empty
        {
            set => _throw_empty = value;
            get => _throw_empty;
        }

        protected internal ThrowHandler_code Throw_code
        {
            set => _throw_code = value;
            get => _throw_code;
        }

        protected internal ThrowHandler_string Throw_string
        {
            set => _throw_string = value;
            get => _throw_string;
        }

        protected internal ThrowHandler_format_args Throw_format_args
        {
            set => _throw_format_args = value;
            get => _throw_format_args;
        }

        protected internal ThrowHandler_exception Throw_exception
        {
            set => _throw_exception = value;
            get => _throw_exception;
        }

        protected internal ThrowHandler_type Throw_type
        {
            set => _throw_type = value;
            get => _throw_type;
        }

        protected internal ThrowHandler_type_string Throw_type_string
        {
            set => _throw_type_string = value;
            get => _throw_type_string;
        }

        protected internal ThrowHandler_type_format_args Throw_type_format_args
        {
            set => _throw_type_format_args = value;
            get => _throw_type_format_args;
        }

        #endregion
    }
}
