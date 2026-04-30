/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
/// Copyright (C) 2023 - 2024, Guangzhou Shiyue Network Technology Co., Ltd.
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

namespace NovaEngine
{
    /// 调试器对象工具类
    internal partial class CDebugger
    {
        /// <summary>
        /// 调试器对象配置加载接口函数
        /// </summary>
        public void LoadConfig()
        {
            int lv = Environment.DebugLevel;

            // 未开启调试模式的情况下，调试级别设置为错误级
            if (!Environment.DebugMode) lv = (int) CLogOutputLevelType.Error;

            // 重新绑定为空置模式
            RebindingBlankOutputHandler();

            CLogger.Info("The platform current debug level was {%d}.", lv);

            // 自定义模式在任意级别下均会加载
            Output_object = CLogger.__Output;
            Output_string = CLogger.__Output;
            Output_format_args = CLogger.__Output;

            Assert_empty = CLogger.__Assert_ImplementedOnSystem;
            Assert_object = CLogger.__Assert_ImplementedOnSystem;
            Assert_string = CLogger.__Assert_ImplementedOnSystem;
            Assert_format_args = CLogger.__Assert_ImplementedOnSystem;

            Throw_empty = CLogger.__Throw_ImplementedOnSystem;
            Throw_code = CLogger.__Throw_ImplementedOnSystem;
            Throw_string = CLogger.__Throw_ImplementedOnSystem;
            Throw_format_args = CLogger.__Throw_ImplementedOnSystem;
            Throw_exception = CLogger.__Throw_ImplementedOnSystem;
            Throw_type = CLogger.__Throw_ImplementedOnSystem;
            Throw_type_string = CLogger.__Throw_ImplementedOnSystem;
            Throw_type_format_args = CLogger.__Throw_ImplementedOnSystem;

            // 非调试模式下，断言和异常都作为崩溃输出处理
            if (!Environment.DebugMode)
            {
                Assert_empty = CLogger.__Assert_ImplementedOnOutput;
                Assert_object = CLogger.__Assert_ImplementedOnOutput;
                Assert_string = CLogger.__Assert_ImplementedOnOutput;
                Assert_format_args = CLogger.__Assert_ImplementedOnOutput;

                Throw_empty = CLogger.__Throw_ImplementedOnOutput;
                Throw_code = CLogger.__Throw_ImplementedOnOutput;
                Throw_string = CLogger.__Throw_ImplementedOnOutput;
                Throw_format_args = CLogger.__Throw_ImplementedOnOutput;
                Throw_exception = CLogger.__Throw_ImplementedOnOutput;
                Throw_type = CLogger.__Throw_ImplementedOnOutput;
                Throw_type_string = CLogger.__Throw_ImplementedOnOutput;
                Throw_type_format_args = CLogger.__Throw_ImplementedOnOutput;
            }

            // if (lv <= 4)
            {
                Error_object = CLogger.Error;
                Error_string = CLogger.Error;
                Error_format_args = CLogger.Error;

                Fatal_object = CLogger.Fatal;
                Fatal_string = CLogger.Fatal;
                Fatal_format_args = CLogger.Fatal;
            }

            if (lv <= 3)
            {
                Warn_object = CLogger.Warn;
                Warn_string = CLogger.Warn;
                Warn_format_args = CLogger.Warn;

                Error_object = CLogger.TraceError;
                Error_string = CLogger.TraceError;
                Error_format_args = CLogger.TraceError;

                Fatal_object = CLogger.TraceFatal;
                Fatal_string = CLogger.TraceFatal;
                Fatal_format_args = CLogger.TraceFatal;
            }

            if (lv <= 2)
            {
                Info_object = CLogger.Info;
                Info_string = CLogger.Info;
                Info_format_args = CLogger.Info;

                Warn_object = CLogger.TraceWarn;
                Warn_string = CLogger.TraceWarn;
                Warn_format_args = CLogger.TraceWarn;
            }

            if (lv <= 1)
            {
                Log_object = CLogger.Debug;
                Log_string = CLogger.Debug;
                Log_format_args = CLogger.Debug;
            }

            if (lv <= 0)
            {
                Log_object = CLogger.TraceDebug;
                Log_string = CLogger.TraceDebug;
                Log_format_args = CLogger.TraceDebug;

                Info_object = CLogger.TraceInfo;
                Info_string = CLogger.TraceInfo;
                Info_format_args = CLogger.TraceInfo;
            }
        }

        /// <summary>
        /// 调试器对象配置卸载接口函数
        /// </summary>
        public void UnloadConfig()
        {
            // 重新绑定为空置模式
            RebindingBlankOutputHandler();
        }
    }
}
