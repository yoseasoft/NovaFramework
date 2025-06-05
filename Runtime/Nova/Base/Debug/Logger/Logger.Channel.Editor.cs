/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
/// Copyright (C) 2020 - 2022, Guangzhou Xinyuan Technology Co., Ltd.
/// Copyright (C) 2023 - 2024, Guangzhou Shiyue Network Technology Co., Ltd.
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

using SystemDateTime = System.DateTime;
using SystemStringBuilder = System.Text.StringBuilder;
using SystemStackTrace = System.Diagnostics.StackTrace;
using SystemStackFrame = System.Diagnostics.StackFrame;

namespace NovaEngine
{
    /// <summary>
    /// 日志相关函数集合工具类
    /// </summary>
    public static partial class Logger
    {
        /// <summary>
        /// 日志输出编辑器模式操作管理类
        /// </summary>
        [LogOutputChannelBinding(LogOutputChannelType.Editor)]
        public sealed class LogEditor : Singleton<LogEditor>, ILogOutput
        {
            private const string Key_UsingCustomColor = "LogTextUsingCustomColor";
            private const string Key_UsingSystemColor = "LogTextUsingSystemColor";

            /// <summary>
            /// 日志文本使用自定义颜色
            /// </summary>
            private static bool UsingCustomColor = false;
            /// <summary>
            /// 日志文本使用系统设置颜色
            /// </summary>
            private static bool UsingSystemColor = true;

            private const string LOG_COLOR_WHITE        = "FFFFFF";
            private const string LOG_COLOR_BLACK        = "000000";
            private const string LOG_COLOR_RED          = "FF0000";
            private const string LOG_COLOR_GREEN        = "00FF18";
            private const string LOG_COLOR_BLUE         = "0010FF";
            private const string LOG_COLOR_ORINGE       = "FF9400";
            private const string LOG_COLOR_EXCEPTION    = "FF00BD";

            [System.ThreadStatic]
            private static SystemStringBuilder _cachedStringBuilder = new SystemStringBuilder(8192);

            /// <summary>
            /// 启动日志输出编辑器模式
            /// </summary>
            public static void Startup()
            {
                LogEditor c = Instance;
                AddOutputChannel(c);
            }

            /// <summary>
            /// 关闭日志输出编辑器模式
            /// </summary>
            public static void Shutdown()
            {
                LogEditor c = Instance;
                RemoveOutputChannel(c);
                Destroy();
            }

            /// <summary>
            /// 日志编辑器类初始化接口
            /// </summary>
            protected override void Initialize()
            {
                if (Configuration.HasProperty(Key_UsingCustomColor))
                {
                    UsingCustomColor = Configuration.GetPropertyAsBool(Key_UsingCustomColor);
                }

                if (Configuration.HasProperty(Key_UsingSystemColor))
                {
                    UsingSystemColor = Configuration.GetPropertyAsBool(Key_UsingSystemColor);
                }
            }

            /// <summary>
            /// 日志编辑器类清理接口
            /// </summary>
            protected override void Cleanup()
            {
            }

            /// <summary>
            /// 日志输入记录接口
            /// </summary>
            /// <param name="level">日志等级</param>
            /// <param name="message">日志内容</param>
            public void Output(LogOutputLevelType level, object message)
            {
                SystemStringBuilder sb = GetHighlightedLogText(level, message.ToString());

                // 获取C#堆栈,Warning以上级别日志才获取堆栈
                if (level >= LogOutputLevelType.Warning)
                {
                    sb.Append("\n");

                    SystemStackFrame[] stackFrames = new SystemStackTrace(true).GetFrames();
                    for (int i = 0; i < stackFrames.Length; i++)
                    {
                        SystemStackFrame frame = stackFrames[i];
                        string declaringTypeName = frame.GetMethod().DeclaringType.FullName;
                        string methodName = frame.GetMethod().Name;

                        string fileName = frame.GetFileName();
                        int lineNumber = frame.GetFileLineNumber();

                        sb.AppendFormat("({0}:{1}) (at {2}:{3})\n", declaringTypeName, methodName, fileName, lineNumber);
                    }
                }

                switch (level)
                {
                    case LogOutputLevelType.Debug:
                    case LogOutputLevelType.Info:
                        UnityEngine.Debug.Log(sb.ToString());
                        break;
                    case LogOutputLevelType.Warning:
                        UnityEngine.Debug.LogWarning(sb.ToString());
                        break;
                    case LogOutputLevelType.Error:
                    case LogOutputLevelType.Fatal:
                        UnityEngine.Debug.LogError(sb.ToString());
                        break;
                    case LogOutputLevelType.Assert:
                    case LogOutputLevelType.Exception:
                        UnityEngine.Debug.LogAssertion(sb.ToString());
                        break;
                }
            }

            private SystemStringBuilder GetHighlightedLogText(LogOutputLevelType level, string message)
            {
                _cachedStringBuilder.Clear();

                switch (level)
                {
                    case LogOutputLevelType.Debug:
                        if (UsingCustomColor)
                        {
                            _cachedStringBuilder.AppendFormat(
                                "<color=#0099bc><b>[NovaEngine] ► </b></color><color=#00FF18><b>[DEBUG] ► </b></color>[{0}] - <color=#{2}>{1}</color>",
                                SystemDateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff"), message, LOG_COLOR_GREEN);
                        }
                        else
                        {
                            _cachedStringBuilder.AppendFormat(
                                UsingSystemColor ?
                                "<color=#0099bc><b>[NovaEngine] ► </b></color><color=gray><b>[DEBUG] ► </b></color>[{0}] - <color=#00FF18>{1}</color>" :
                                "<color=#0099bc><b>[NovaEngine] ► </b></color><color=#00FF18><b>[DEBUG] ► </b></color>[{0}] - {1}",
                                SystemDateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff"), message);
                        }
                        break;
                    case LogOutputLevelType.Info:
                        if (UsingCustomColor)
                        {
                            _cachedStringBuilder.AppendFormat(
                                "<color=#0099bc><b>[NovaEngine] ► </b></color><color=gray><b>[INFO] ► </b></color>[{0}] - <color=#{2}>{1}</color>",
                                SystemDateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff"), message, LOG_COLOR_BLACK);
                        }
                        else
                        {
                            _cachedStringBuilder.AppendFormat(
                                UsingSystemColor ?
                                "<color=#0099bc><b>[NovaEngine] ► </b></color><color=gray><b>[INFO] ► </b></color>[{0}] - <color=gray>{1}</color>" :
                                "<color=#0099bc><b>[NovaEngine] ► </b></color><color=gray><b>[INFO] ► </b></color>[{0}] - {1}",
                                SystemDateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff"), message);
                        }
                        break;
                    case LogOutputLevelType.Warning:
                        if (UsingCustomColor)
                        {
                            _cachedStringBuilder.AppendFormat(
                                "<color=#0099bc><b>[NovaEngine] ► </b></color><color=#FF9400><b>[WARNING] ► </b></color>[{0}] - <color=#{2}>{1}</color>",
                                SystemDateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff"), message, LOG_COLOR_ORINGE);
                        }
                        else
                        {
                            _cachedStringBuilder.AppendFormat(
                                UsingSystemColor ?
                                "<color=#0099bc><b>[NovaEngine] ► </b></color><color=#FF9400><b>[WARNING] ► </b></color>[{0}] - <color=yellow>{1}</color>" :
                                "<color=#0099bc><b>[NovaEngine] ► </b></color><color=#FF9400><b>[WARNING] ► </b></color>[{0}] - {1}",
                                SystemDateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff"), message);
                        }
                        break;
                    case LogOutputLevelType.Error:
                        if (UsingCustomColor)
                        {
                            _cachedStringBuilder.AppendFormat(
                                "<color=#0099bc><b>[NovaEngine] ► </b></color><color=#FF9400><b>[WARNING] ► </b></color>[{0}] - <color=#{2}>{1}</color>",
                                SystemDateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff"), message, LOG_COLOR_RED);
                        }
                        else
                        {
                            _cachedStringBuilder.AppendFormat(
                                UsingSystemColor ?
                                "<color=#0099bc><b>[NovaEngine] ► </b></color><color=red><b>[ERROR] ► </b></color>[{0}] - <color=red>{1}</color>" :
                                "<color=#0099bc><b>[NovaEngine] ► </b></color><color=red><b>[ERROR] ► </b></color>[{0}] - {1}",
                                SystemDateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff"), message);
                        }
                        break;
                    case LogOutputLevelType.Fatal:
                        if (UsingCustomColor)
                        {
                            _cachedStringBuilder.AppendFormat(
                                "<color=#0099bc><b>[NovaEngine] ► </b></color><color=#FF00BD><b>[FATAL] ► </b></color>[{0}] - <color=#{2}>{1}</color>",
                                SystemDateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff"), message, LOG_COLOR_EXCEPTION);
                        }
                        else
                        {
                            _cachedStringBuilder.AppendFormat(
                                UsingSystemColor ?
                                "<color=#0099bc><b>[NovaEngine] ► </b></color><color=#FF00BD><b>[FATAL] ► </b></color>[{0}] - <color=green>{1}</color>" :
                                "<color=#0099bc><b>[NovaEngine] ► </b></color><color=#FF00BD><b>[FATAL] ► </b></color>[{0}] - {1}",
                                SystemDateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff"), message);
                        }
                        break;
                    case LogOutputLevelType.Assert:
                        if (UsingCustomColor)
                        {
                            _cachedStringBuilder.AppendFormat(
                                "<color=#0099bc><b>[NovaEngine] ► </b></color><color=#FF00BD><b>[ASSERT] ► </b></color>[{0}] - <color=#{2}>{1}</color>",
                                SystemDateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff"), message, LOG_COLOR_EXCEPTION);
                        }
                        else
                        {
                            _cachedStringBuilder.AppendFormat(
                                UsingSystemColor ?
                                "<color=#0099bc><b>[NovaEngine] ► </b></color><color=#FF00BD><b>[ASSERT] ► </b></color>[{0}] - <color=green>{1}</color>" :
                                "<color=#0099bc><b>[NovaEngine] ► </b></color><color=#FF00BD><b>[ASSERT] ► </b></color>[{0}] - {1}",
                                SystemDateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff"), message);
                        }
                        break;
                    case LogOutputLevelType.Exception:
                        if (UsingCustomColor)
                        {
                            _cachedStringBuilder.AppendFormat(
                                "<color=#0099bc><b>[NovaEngine] ► </b></color><color=red><b>[EXCEPTION] ► </b></color>[{0}] - <color=#{2}>{1}</color>",
                                SystemDateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff"), message, LOG_COLOR_EXCEPTION);
                        }
                        else
                        {
                            _cachedStringBuilder.AppendFormat(
                                UsingSystemColor ?
                                "<color=#0099bc><b>[NovaEngine] ► </b></color><color=red><b>[EXCEPTION] ► </b></color>[{0}] - <color=red>{1}</color>" :
                                "<color=#0099bc><b>[NovaEngine] ► </b></color><color=red><b>[EXCEPTION] ► </b></color>[{0}] - {1}",
                                SystemDateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff"), message);
                        }
                        break;
                }

                return _cachedStringBuilder;
            }
        }
    }
}
