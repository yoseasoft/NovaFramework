/// -------------------------------------------------------------------------------
/// NovaEngine Framework Samples
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

namespace GameEngine.Sample.TextFormat
{
    /// <summary>
    /// 操作任务接口类
    /// </summary>
    static partial class OpsTask
    {
        [GameEngine.OnInputDispatchCall(TaskCode_LogFormat, GameEngine.InputOperationType.Released)]
        static void TestLogFormat(int keycode, int operationType)
        {
            int num = 1976123159;
            float num2 = 200101.5959337f;
            string str = "hello，中国";
            IDictionary<int, string> dict = new Dictionary<int, string>();
            dict.Add(1, "good");
            dict.Add(2, "pool");
            dict.Add(3, "door");
            dict.Add(4, "room");
            dict.Add(5, "zoon");

            string format = @"测试格式化参数，包括了整型{%d}，八进制{%o}，十六进制{%x}，浮点型{%f}，科学计数法{%e}，字符型{%c}，字符串型""{%s}""，指针型""{%p}""，对象类型{%t}，对象描述{{{%v}}}";
            Debugger.Warn(NovaEngine.Formatter.TextFormatConvertionProcess(format, num, num, num, num2, num2, str, str, dict, dict, dict, num, num2, str, dict));
        }
    }
}
