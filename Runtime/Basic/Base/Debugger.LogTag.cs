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

using System.Collections.Generic;
using System.Reflection;

using SystemType = System.Type;
using SystemAttribute = System.Attribute;
using SystemBindingFlags = System.Reflection.BindingFlags;

namespace GameEngine
{
    /// <summary>
    /// 日志输出的通道标签，用于进行分组过滤使用
    /// </summary>
    internal sealed class LogGroupTag
    {
        [Debugger.LogOutputGroup]
        public const int Basic = 1;

        [Debugger.LogOutputGroup]
        public const int Profiler = 2;

        [Debugger.LogOutputGroup]
        public const int Bean = 11;

        [Debugger.LogOutputGroup]
        public const int Module = 12;

        [Debugger.LogOutputGroup]
        public const int Controller = 21;

        [Debugger.LogOutputGroup]
        public const int CodeLoader = 22;

        /// <summary>
        /// 用户组的分割线
        /// </summary>
        public const int User = 10000;
    }

    /// <summary>
    /// 应用层提供的调试对象类，它是基于对<see cref="NovaEngine.Debugger"/>的便捷性接口封装
    /// </summary>
    public static partial class Debugger
    {
        /// <summary>
        /// 调试日志分组输出通道的默认开启状态
        /// </summary>
        private static bool DefaultLogOutputGroupEnabled = true;
        /// <summary>
        /// 调试日志分组输出通道的默认日志输出级别
        /// </summary>
        private static int DefaultLogOutputGroupLevel = 5;

        /// <summary>
        /// 日志输出通道分组的属性类型定义
        /// </summary>
        internal class LogOutputGroupAttribute : SystemAttribute
        {
            /// <summary>
            /// 日志分组启用状态标识
            /// </summary>
            private readonly bool _enabled;
            /// <summary>
            /// 日志分组的输出级别
            /// </summary>
            private readonly int _logLevel;

            public bool Enabled => _enabled;
            public int LogLevel => _logLevel;

            public LogOutputGroupAttribute() : this(DefaultLogOutputGroupEnabled) { }

            public LogOutputGroupAttribute(bool enabled) : this(enabled, DefaultLogOutputGroupLevel) { }

            public LogOutputGroupAttribute(int level) : this(DefaultLogOutputGroupEnabled, level) { }

            private LogOutputGroupAttribute(bool enabled, int level) : base()
            {
                _enabled = enabled;
                _logLevel = level;
            }
        }

        /// <summary>
        /// 初始化日志输出通道分组设置参数
        /// </summary>
        private static void InitLogOutputGroupSettings()
        {
            SystemType classType = typeof(LogGroupTag);
            FieldInfo[] fieldInfos = classType.GetFields(SystemBindingFlags.Public | SystemBindingFlags.Static);
            for (int n = 0; null != fieldInfos && n < fieldInfos.Length; ++n)
            {
                FieldInfo fieldInfo = fieldInfos[n];
                if (fieldInfo.IsLiteral)
                {
                    IEnumerable<SystemAttribute> e = fieldInfo.GetCustomAttributes();
                    foreach (SystemAttribute attr in e)
                    {
                        if (typeof(LogOutputGroupAttribute) == attr.GetType())
                        {
                            LogOutputGroupAttribute _attr = (LogOutputGroupAttribute) attr;

                            AddTargetOutputGroup(System.Convert.ToInt32(fieldInfo.GetValue(null)), fieldInfo.Name, _attr.Enabled, _attr.LogLevel);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 清除全部日志输出通道分组设置参数
        /// </summary>
        private static void RemoveAllLogOutputGroupSettings()
        {
            RemoveAllOutputGroups();
        }
    }
}
