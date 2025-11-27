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
    internal partial class Debugger
    {
        /// <summary>
        /// 调试器对象配置加载接口函数
        /// </summary>
        public void LoadConfig()
        {
            int lv = Environment.debugLevel;

            // 未开启调试模式的情况下，调试级别设置为错误级
            if (!Environment.debugMode) lv = (int) LogOutputLevelType.Error;

            // 重新绑定为空置模式
            RebindingBlankOutputHandler();

            Logger.Info("The platform current debug level was {%d}.", lv);

            // 自定义模式在任意级别下均会加载
            Output_object = Logger.__Output;
            Output_string = Logger.__Output;
            Output_format_args = Logger.__Output;

            Assert_empty = Logger.__Assert_ImplementedOnSystem;
            Assert_object = Logger.__Assert_ImplementedOnSystem;
            Assert_string = Logger.__Assert_ImplementedOnSystem;
            Assert_format_args = Logger.__Assert_ImplementedOnSystem;

            Throw_empty = Logger.__Throw_ImplementedOnSystem;
            Throw_code = Logger.__Throw_ImplementedOnSystem;
            Throw_string = Logger.__Throw_ImplementedOnSystem;
            Throw_format_args = Logger.__Throw_ImplementedOnSystem;
            Throw_exception = Logger.__Throw_ImplementedOnSystem;
            Throw_type = Logger.__Throw_ImplementedOnSystem;
            Throw_type_string = Logger.__Throw_ImplementedOnSystem;
            Throw_type_format_args = Logger.__Throw_ImplementedOnSystem;

            // 非调试模式下，断言和异常都作为崩溃输出处理
            if (!Environment.debugMode)
            {
                Assert_empty = Logger.__Assert_ImplementedOnOutput;
                Assert_object = Logger.__Assert_ImplementedOnOutput;
                Assert_string = Logger.__Assert_ImplementedOnOutput;
                Assert_format_args = Logger.__Assert_ImplementedOnOutput;

                Throw_empty = Logger.__Throw_ImplementedOnOutput;
                Throw_code = Logger.__Throw_ImplementedOnOutput;
                Throw_string = Logger.__Throw_ImplementedOnOutput;
                Throw_format_args = Logger.__Throw_ImplementedOnOutput;
                Throw_exception = Logger.__Throw_ImplementedOnOutput;
                Throw_type = Logger.__Throw_ImplementedOnOutput;
                Throw_type_string = Logger.__Throw_ImplementedOnOutput;
                Throw_type_format_args = Logger.__Throw_ImplementedOnOutput;
            }

            // if (lv <= 4)
            {
                Error_object = Logger.Error;
                Error_string = Logger.Error;
                Error_format_args = Logger.Error;

                Fatal_object = Logger.Fatal;
                Fatal_string = Logger.Fatal;
                Fatal_format_args = Logger.Fatal;
            }

            if (lv <= 3)
            {
                Warn_object = Logger.Warn;
                Warn_string = Logger.Warn;
                Warn_format_args = Logger.Warn;

                Error_object = Logger.TraceError;
                Error_string = Logger.TraceError;
                Error_format_args = Logger.TraceError;

                Fatal_object = Logger.TraceFatal;
                Fatal_string = Logger.TraceFatal;
                Fatal_format_args = Logger.TraceFatal;
            }

            if (lv <= 2)
            {
                Info_object = Logger.Info;
                Info_string = Logger.Info;
                Info_format_args = Logger.Info;

                Warn_object = Logger.TraceWarn;
                Warn_string = Logger.TraceWarn;
                Warn_format_args = Logger.TraceWarn;
            }

            if (lv <= 1)
            {
                Log_object = Logger.Debug;
                Log_string = Logger.Debug;
                Log_format_args = Logger.Debug;
            }

            if (lv <= 0)
            {
                Log_object = Logger.TraceDebug;
                Log_string = Logger.TraceDebug;
                Log_format_args = Logger.TraceDebug;

                Info_object = Logger.TraceInfo;
                Info_string = Logger.TraceInfo;
                Info_format_args = Logger.TraceInfo;
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
