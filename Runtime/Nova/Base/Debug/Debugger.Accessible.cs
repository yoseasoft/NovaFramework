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
    public partial class Debugger
    {
        /// <summary>
        /// 日志输出规范定义代理句柄接口
        /// </summary>
        public delegate void OutputHandler_object(object message);
        public delegate void OutputHandler_string(string message);
        public delegate void OutputHandler_cond_string(bool cond, string message);
        public delegate void OutputHandler_format_args(string format, params object[] args);
        public delegate void OutputHandler_cond_format_args(bool cond, string format, params object[] args);

        public delegate void OutputHandler_level_object(LogOutputLevelType level, object message);
        public delegate void OutputHandler_level_string(LogOutputLevelType level, string message);
        public delegate void OutputHandler_level_format_args(LogOutputLevelType level, string format, params object[] args);

        /// <summary>
        /// 断言处理规范定义代理句柄接口
        /// </summary>
        public delegate void AssertHandler_empty(bool condition);
        public delegate void AssertHandler_object(bool condition, object message);
        public delegate void AssertHandler_string(bool condition, string message);
        public delegate void AssertHandler_format_args(bool condition, string format, params object[] args);

        /// <summary>
        /// 异常处理规范定义代理句柄接口
        /// </summary>
        public delegate void ThrowHandler_empty();
        public delegate void ThrowHandler_code(int errorCode);
        public delegate void ThrowHandler_string(string message);
        public delegate void ThrowHandler_format_args(string format, params object[] args);
        public delegate void ThrowHandler_exception(System.Exception exception);
        public delegate void ThrowHandler_type(System.Type type);
        public delegate void ThrowHandler_type_string(System.Type type, string message);
        public delegate void ThrowHandler_type_format_args(System.Type type, string format, params object[] args);

        /// <summary>
        /// 调试模式下的输出回调接口
        /// </summary>
        private OutputHandler_object m_log_object;
        private OutputHandler_string m_log_string;
        private OutputHandler_format_args m_log_format_args;

        /// <summary>
        /// 信息模式下的输出回调接口
        /// </summary>
        private OutputHandler_object m_info_object;
        private OutputHandler_string m_info_string;
        private OutputHandler_format_args m_info_format_args;

        /// <summary>
        /// 警告模式下的输出回调接口
        /// </summary>
        private OutputHandler_object m_warn_object;
        private OutputHandler_string m_warn_string;
        private OutputHandler_format_args m_warn_format_args;

        /// <summary>
        /// 错误模式下的输出回调接口
        /// </summary>
        private OutputHandler_object m_error_object;
        private OutputHandler_string m_error_string;
        private OutputHandler_format_args m_error_format_args;

        /// <summary>
        /// 崩溃模式下的输出回调接口
        /// </summary>
        private OutputHandler_object m_fatal_object;
        private OutputHandler_string m_fatal_string;
        private OutputHandler_format_args m_fatal_format_args;

        /// <summary>
        /// 自定义模式下的输出回调接口
        /// </summary>
        private OutputHandler_level_object m_output_object;
        private OutputHandler_level_string m_output_string;
        private OutputHandler_level_format_args m_output_format_args;

        /// <summary>
        /// 调试模型下的断言回调接口
        /// </summary>
        private AssertHandler_empty m_assert_empty;
        private AssertHandler_object m_assert_object;
        private AssertHandler_string m_assert_string;
        private AssertHandler_format_args m_assert_format_args;

        /// <summary>
        /// 调试模型下的异常回调接口
        /// </summary>
        private ThrowHandler_empty m_throw_empty;
        private ThrowHandler_code m_throw_code;
        private ThrowHandler_string m_throw_string;
        private ThrowHandler_format_args m_throw_format_args;
        private ThrowHandler_exception m_throw_exception;
        private ThrowHandler_type m_throw_type;
        private ThrowHandler_type_string m_throw_type_string;
        private ThrowHandler_type_format_args m_throw_type_format_args;

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
            m_log_object = Blank_Output;
            m_log_string = Blank_Output;
            m_log_format_args = Blank_Output;

            m_info_object = Blank_Output;
            m_info_string = Blank_Output;
            m_info_format_args = Blank_Output;

            m_warn_object = Blank_Output;
            m_warn_string = Blank_Output;
            m_warn_format_args = Blank_Output;

            m_error_object = Blank_Output;
            m_error_string = Blank_Output;
            m_error_format_args = Blank_Output;

            m_fatal_object = Blank_Output;
            m_fatal_string = Blank_Output;
            m_fatal_format_args = Blank_Output;

            m_output_object = Blank_Output;
            m_output_string = Blank_Output;
            m_output_format_args = Blank_Output;

            m_assert_empty = Blank_Assert;
            m_assert_object = Blank_Assert;
            m_assert_string = Blank_Assert;
            m_assert_format_args = Blank_Assert;

            m_throw_empty = Blank_Throw;
            m_throw_code = Blank_Throw;
            m_throw_string = Blank_Throw;
            m_throw_format_args = Blank_Throw;
            m_throw_exception = Blank_Throw;
            m_throw_type = Blank_Throw;
            m_throw_type_string = Blank_Throw;
            m_throw_type_format_args = Blank_Throw;
        }

        #region 调试器对象输出回调接口的Getter/Setter函数

        protected internal OutputHandler_object Log_object
        {
            set => m_log_object = value;
            get => m_log_object;
        }

        protected internal OutputHandler_string Log_string
        {
            set => m_log_string = value;
            get => m_log_string;
        }

        protected internal OutputHandler_format_args Log_format_args
        {
            set => m_log_format_args = value;
            get => m_log_format_args;
        }

        protected internal OutputHandler_object Info_object
        {
            set => m_info_object = value;
            get => m_info_object;
        }

        protected internal OutputHandler_string Info_string
        {
            set => m_info_string = value;
            get => m_info_string;
        }

        protected internal OutputHandler_format_args Info_format_args
        {
            set => m_info_format_args = value;
            get => m_info_format_args;
        }

        protected internal OutputHandler_object Warn_object
        {
            set => m_warn_object = value;
            get => m_warn_object;
        }

        protected internal OutputHandler_string Warn_string
        {
            set => m_warn_string = value;
            get => m_warn_string;
        }

        protected internal OutputHandler_format_args Warn_format_args
        {
            set => m_warn_format_args = value;
            get => m_warn_format_args;
        }

        protected internal OutputHandler_object Error_object
        {
            set => m_error_object = value;
            get => m_error_object;
        }

        protected internal OutputHandler_string Error_string
        {
            set => m_error_string = value;
            get => m_error_string;
        }

        protected internal OutputHandler_format_args Error_format_args
        {
            set => m_error_format_args = value;
            get => m_error_format_args;
        }

        protected internal OutputHandler_object Fatal_object
        {
            set => m_fatal_object = value;
            get => m_fatal_object;
        }

        protected internal OutputHandler_string Fatal_string
        {
            set => m_fatal_string = value;
            get => m_fatal_string;
        }

        protected internal OutputHandler_format_args Fatal_format_args
        {
            set => m_fatal_format_args = value;
            get => m_fatal_format_args;
        }

        protected internal OutputHandler_level_object Output_object
        {
            set => m_output_object = value;
            get => m_output_object;
        }

        protected internal OutputHandler_level_string Output_string
        {
            set => m_output_string = value;
            get => m_output_string;
        }

        protected internal OutputHandler_level_format_args Output_format_args
        {
            set => m_output_format_args = value;
            get => m_output_format_args;
        }

        protected internal AssertHandler_empty Assert_empty
        {
            set => m_assert_empty = value;
            get => m_assert_empty;
        }

        protected internal AssertHandler_object Assert_object
        {
            set => m_assert_object = value;
            get => m_assert_object;
        }

        protected internal AssertHandler_string Assert_string
        {
            set => m_assert_string = value;
            get => m_assert_string;
        }

        protected internal AssertHandler_format_args Assert_format_args
        {
            set => m_assert_format_args = value;
            get => m_assert_format_args;
        }

        protected internal ThrowHandler_empty Throw_empty
        {
            set => m_throw_empty = value;
            get => m_throw_empty;
        }

        protected internal ThrowHandler_code Throw_code
        {
            set => m_throw_code = value;
            get => m_throw_code;
        }

        protected internal ThrowHandler_string Throw_string
        {
            set => m_throw_string = value;
            get => m_throw_string;
        }

        protected internal ThrowHandler_format_args Throw_format_args
        {
            set => m_throw_format_args = value;
            get => m_throw_format_args;
        }

        protected internal ThrowHandler_exception Throw_exception
        {
            set => m_throw_exception = value;
            get => m_throw_exception;
        }

        protected internal ThrowHandler_type Throw_type
        {
            set => m_throw_type = value;
            get => m_throw_type;
        }

        protected internal ThrowHandler_type_string Throw_type_string
        {
            set => m_throw_type_string = value;
            get => m_throw_type_string;
        }

        protected internal ThrowHandler_type_format_args Throw_type_format_args
        {
            set => m_throw_type_format_args = value;
            get => m_throw_type_format_args;
        }

        #endregion
    }
}
