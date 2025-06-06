/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyring (C) 2023 - 2024, Guangzhou Shiyue Network Technology Co., Ltd.
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

namespace GameEngine
{
    /// <summary>
    /// 应用层提供的调试对象类，它是基于对<see cref="NovaEngine.Debugger"/>的便捷性接口封装
    /// </summary>
    public static partial class Debugger
    {
        /// <summary>
        /// 以分组模式开启调试的状态标识映射列表
        /// </summary>
        private static readonly IDictionary<int, DebuggingOutputGroupInfo> s_debuggingOutputGroupInfos = new Dictionary<int, DebuggingOutputGroupInfo>();

        /// <summary>
        /// 设置当前调试环境下指定分组的启用状态标识
        /// </summary>
        /// <param name="groupID">模块组标识</param>
        /// <param name="enabled">启用状态标识</param>
        internal static void SetTargetOutputGroupEnabled(int groupID, bool enabled)
        {
            if (false == s_debuggingOutputGroupInfos.ContainsKey(groupID))
            {
                Warn("Could not found any output group with target ID '{0}', setted it enabled failed.", groupID);
                return;
            }

            DebuggingOutputGroupInfo info = s_debuggingOutputGroupInfos[groupID];
            info.Enabled = enabled;
        }

        /// <summary>
        /// 设置当前调试环境下指定分组的日志输出级别
        /// </summary>
        /// <param name="groupID">模块组标识</param>
        /// <param name="level">日志输出级别</param>
        internal static void SetTargetOutputGroupLevel(int groupID, int level)
        {
            if (false == s_debuggingOutputGroupInfos.ContainsKey(groupID))
            {
                Warn("Could not found any output group with target ID '{%d}', setted it level failed.", groupID);
                return;
            }

            DebuggingOutputGroupInfo info = s_debuggingOutputGroupInfos[groupID];
            info.LogLevel = level;
        }

        /// <summary>
        /// 新增当前调试环境下指定分组的信息
        /// </summary>
        /// <param name="groupID">分组标识</param>
        /// <param name="groupName">分组名称</param>
        /// <param name="enabled">启用状态标识</param>
        /// <param name="level">日志输出级别</param>
        internal static void AddTargetOutputGroup(int groupID, string groupName, bool enabled, int level)
        {
            if (groupID <= 0 || string.IsNullOrEmpty(groupName))
            {
                Warn("Add target output group failed with invalid groupID {%d} or name {%s}, please checked your setting params.", groupID, groupName);
                return;
            }

            if (s_debuggingOutputGroupInfos.ContainsKey(groupID))
            {
                s_debuggingOutputGroupInfos.Remove(groupID);
            }

            DebuggingOutputGroupInfo info = new DebuggingOutputGroupInfo(groupID, groupName, enabled, level);
            s_debuggingOutputGroupInfos.Add(groupID, info);
        }

        /// <summary>
        /// 移除当前调试环境下指定分组的信息
        /// </summary>
        /// <param name="groupID">模块组标识</param>
        internal static void RemoveTargetOutputGroup(int groupID)
        {
            if (false == s_debuggingOutputGroupInfos.ContainsKey(groupID))
            {
                Warn("Could not found any output group with target ID '{0}', removed it failed.", groupID);
                return;
            }

            s_debuggingOutputGroupInfos.Remove(groupID);
        }

        /// <summary>
        /// 移除当前调试环境下所有分组的开启状态标识
        /// </summary>
        internal static void RemoveAllOutputGroups()
        {
            s_debuggingOutputGroupInfos?.Clear();
        }

        /// <summary>
        /// 针对特定模块组开放的调试模式日志输出接口，若模块组处于调试开启状态，则执行<see cref="Debugger.Log(object)"/>接口函数
        /// </summary>
        /// <param name="groupID">模块组标识</param>
        /// <param name="message">日志内容</param>
        public static void Log(int groupID, object message)
        {
            Output(groupID, NovaEngine.LogOutputLevelType.Debug, message);
        }

        /// <summary>
        /// 针对特定模块组开放的调试模式日志输出接口，若模块组处于调试开启状态，则执行<see cref="Debugger.Log(string)"/>接口函数
        /// </summary>
        /// <param name="groupID">模块组标识</param>
        /// <param name="message">日志内容</param>
        public static void Log(int groupID, string message)
        {
            Output(groupID, NovaEngine.LogOutputLevelType.Debug, message);
        }

        /// <summary>
        /// 针对特定模块组开放的调试模式日志输出接口，若模块组处于调试开启状态，则执行<see cref="Debugger.Log(string, object[])"/>接口函数
        /// </summary>
        /// <param name="groupID">模块组标识</param>
        /// <param name="format">日志格式内容</param>
        /// <param name="args">日志格式化参数</param>
        public static void Log(int groupID, string format, params object[] args)
        {
            Output(groupID, NovaEngine.LogOutputLevelType.Debug, format, args);
        }

        /// <summary>
        /// 针对特定模块组开放的常规模式日志输出接口，若模块组处于调试开启状态，则执行<see cref="Debugger.Info(object)"/>接口函数
        /// </summary>
        /// <param name="groupID">模块组标识</param>
        /// <param name="message">日志内容</param>
        public static void Info(int groupID, object message)
        {
            Output(groupID, NovaEngine.LogOutputLevelType.Info, message);
        }

        /// <summary>
        /// 针对特定模块组开放的常规模式日志输出接口，若模块组处于调试开启状态，则执行<see cref="Debugger.Info(string)"/>接口函数
        /// </summary>
        /// <param name="groupID">模块组标识</param>
        /// <param name="message">日志内容</param>
        public static void Info(int groupID, string message)
        {
            Output(groupID, NovaEngine.LogOutputLevelType.Info, message);
        }

        /// <summary>
        /// 针对特定模块组开放的常规模式日志输出接口，若模块组处于调试开启状态，则执行<see cref="Debugger.Info(string, object[])"/>接口函数
        /// </summary>
        /// <param name="groupID">模块组标识</param>
        /// <param name="format">日志格式内容</param>
        /// <param name="args">日志格式化参数</param>
        public static void Info(int groupID, string format, params object[] args)
        {
            Output(groupID, NovaEngine.LogOutputLevelType.Info, format, args);
        }

        /// <summary>
        /// 针对特定模块组开放的警告模式日志输出接口，若模块组处于调试开启状态，则执行<see cref="Debugger.Warn(object)"/>接口函数
        /// </summary>
        /// <param name="groupID">模块组标识</param>
        /// <param name="message">日志内容</param>
        public static void Warn(int groupID, object message)
        {
            Output(groupID, NovaEngine.LogOutputLevelType.Warning, message);
        }

        /// <summary>
        /// 针对特定模块组开放的警告模式日志输出接口，若模块组处于调试开启状态，则执行<see cref="Debugger.Warn(string)"/>接口函数
        /// </summary>
        /// <param name="groupID">模块组标识</param>
        /// <param name="message">日志内容</param>
        public static void Warn(int groupID, string message)
        {
            Output(groupID, NovaEngine.LogOutputLevelType.Warning, message);
        }

        /// <summary>
        /// 针对特定模块组开放的警告模式日志输出接口，若模块组处于调试开启状态，则执行<see cref="Debugger.Warn(string, object[])"/>接口函数
        /// </summary>
        /// <param name="groupID">模块组标识</param>
        /// <param name="format">日志格式内容</param>
        /// <param name="args">日志格式化参数</param>
        public static void Warn(int groupID, string format, params object[] args)
        {
            Output(groupID, NovaEngine.LogOutputLevelType.Warning, format, args);
        }

        /// <summary>
        /// 针对特定模块组开放的错误模式日志输出接口，若模块组处于调试开启状态，则执行<see cref="Debugger.Error(object)"/>接口函数
        /// </summary>
        /// <param name="groupID">模块组标识</param>
        /// <param name="message">日志内容</param>
        public static void Error(int groupID, object message)
        {
            Output(groupID, NovaEngine.LogOutputLevelType.Error, message);
        }

        /// <summary>
        /// 针对特定模块组开放的错误模式日志输出接口，若模块组处于调试开启状态，则执行<see cref="Debugger.Error(string)"/>接口函数
        /// </summary>
        /// <param name="groupID">模块组标识</param>
        /// <param name="message">日志内容</param>
        public static void Error(int groupID, string message)
        {
            Output(groupID, NovaEngine.LogOutputLevelType.Error, message);
        }

        /// <summary>
        /// 针对特定模块组开放的错误模式日志输出接口，若模块组处于调试开启状态，则执行<see cref="Debugger.Error(string, object[])"/>接口函数
        /// </summary>
        /// <param name="groupID">模块组标识</param>
        /// <param name="format">日志格式内容</param>
        /// <param name="args">日志格式化参数</param>
        public static void Error(int groupID, string format, params object[] args)
        {
            Output(groupID, NovaEngine.LogOutputLevelType.Error, format, args);
        }

        /// <summary>
        /// 针对特定模块组开放的崩溃模式日志输出接口，若模块组处于调试开启状态，则执行<see cref="Debugger.Fatal(object)"/>接口函数
        /// </summary>
        /// <param name="groupID">模块组标识</param>
        /// <param name="message">日志内容</param>
        public static void Fatal(int groupID, object message)
        {
            Output(groupID, NovaEngine.LogOutputLevelType.Fatal, message);
        }

        /// <summary>
        /// 针对特定模块组开放的崩溃模式日志输出接口，若模块组处于调试开启状态，则执行<see cref="Debugger.Fatal(string)"/>接口函数
        /// </summary>
        /// <param name="groupID">模块组标识</param>
        /// <param name="message">日志内容</param>
        public static void Fatal(int groupID, string message)
        {
            Output(groupID, NovaEngine.LogOutputLevelType.Fatal, message);
        }

        /// <summary>
        /// 针对特定模块组开放的崩溃模式日志输出接口，若模块组处于调试开启状态，则执行<see cref="Debugger.Fatal(string, object[])"/>接口函数
        /// </summary>
        /// <param name="groupID">模块组标识</param>
        /// <param name="format">日志格式内容</param>
        /// <param name="args">日志格式化参数</param>
        public static void Fatal(int groupID, string format, params object[] args)
        {
            Output(groupID, NovaEngine.LogOutputLevelType.Fatal, format, args);
        }

        /// <summary>
        /// 针对指定分组和日志级别的通用输出接口函数
        /// </summary>
        /// <param name="groupID">模块组标识</param>
        /// <param name="level">日志级别</param>
        /// <param name="message">日志内容</param>
        private static void Output(int groupID, NovaEngine.LogOutputLevelType level, object message)
        {
            if (s_debuggingOutputGroupInfos.TryGetValue(groupID, out DebuggingOutputGroupInfo info))
            {
                info.Output(level, message);
            }
        }

        /// <summary>
        /// 针对指定分组和日志级别的通用输出接口函数
        /// </summary>
        /// <param name="groupID">模块组标识</param>
        /// <param name="level">日志级别</param>
        /// <param name="message">日志内容</param>
        private static void Output(int groupID, NovaEngine.LogOutputLevelType level, string message)
        {
            if (s_debuggingOutputGroupInfos.TryGetValue(groupID, out DebuggingOutputGroupInfo info))
            {
                info.Output(level, message);
            }
        }

        /// <summary>
        /// 针对指定分组和日志级别的通用输出接口函数
        /// </summary>
        /// <param name="groupID">模块组标识</param>
        /// <param name="level">日志级别</param>
        /// <param name="format">日志格式内容</param>
        /// <param name="args">日志格式化参数</param>
        private static void Output(int groupID, NovaEngine.LogOutputLevelType level, string format, params object[] args)
        {
            if (s_debuggingOutputGroupInfos.TryGetValue(groupID, out DebuggingOutputGroupInfo info))
            {
                info.Output(level, format, args);
            }
        }

        #region 调试模块输出分组信息对象类

        /// <summary>
        /// 调试模块日志输出的分组记录信息封装对象类，用于管理该分组的不同级别的日志输出通道开启状态及对应的日志输出函数接口
        /// </summary>
        private sealed class DebuggingOutputGroupInfo
        {
            /// <summary>
            /// 调试日志分组标识
            /// </summary>
            private readonly int m_groupID;
            /// <summary>
            /// 调试日志分组的描述名称
            /// </summary>
            private readonly string m_groupName;
            /// <summary>
            /// 调试日志分组的开启状态标识
            /// </summary>
            private bool m_enabled;
            /// <summary>
            /// 调试日志分组的可输出日志级别
            /// </summary>
            private int m_logLevel;

            public int GroupID => m_groupID;
            public string GroupName => m_groupName;
            public bool Enabled { get { return m_enabled; } set { m_enabled = value; } }
            public int LogLevel { get { return m_logLevel; } set { m_logLevel = value; } }

            public DebuggingOutputGroupInfo(int groupID, string groupName, bool enabled, int logLevel)
            {
                m_groupID = groupID;
                m_groupName = groupName;
                m_enabled = enabled && GameMacros.DEBUGGING_OUTPUT_GROUP_POLICY_ENABLED;
                m_logLevel = logLevel;
            }

            /// <summary>
            /// 分组信息对象内部的日志输出接口函数，它将对日志级别进行过滤，最终调用底层的调试接口进行输出
            /// </summary>
            /// <param name="level">日志级别</param>
            /// <param name="message">日志内容</param>
            public void Output(NovaEngine.LogOutputLevelType level, object message)
            {
                if (false == IsOutputEnabled(level))
                {
                    return;
                }

                NovaEngine.Debugger.Output(level, $"[{m_groupName}]: {message}");
            }

            /// <summary>
            /// 分组信息对象内部的日志输出接口函数，它将对日志级别进行过滤，最终调用底层的调试接口进行输出
            /// </summary>
            /// <param name="level">日志级别</param>
            /// <param name="message">日志内容</param>
            public void Output(NovaEngine.LogOutputLevelType level, string message)
            {
                if (false == IsOutputEnabled(level))
                {
                    return;
                }

                NovaEngine.Debugger.Output(level, $"[{m_groupName}]: {message}");
            }

            /// <summary>
            /// 分组信息对象内部的日志输出接口函数，它将对日志级别进行过滤，最终调用底层的调试接口进行输出
            /// </summary>
            /// <param name="level">日志级别</param>
            /// <param name="format">日志格式内容</param>
            /// <param name="args">日志格式化参数</param>
            public void Output(NovaEngine.LogOutputLevelType level, string format, params object[] args)
            {
                if (false == IsOutputEnabled(level))
                {
                    return;
                }

                NovaEngine.Debugger.Output(level, $"[{m_groupName}]: {format}", args);
            }

            /// <summary>
            /// 检测当前的调试分组是否启用了指定级别的日志输出
            /// </summary>
            /// <param name="level">日志级别</param>
            /// <returns>若启用给定级别的调试输出返回true，否则返回false</returns>
            private bool IsOutputEnabled(NovaEngine.LogOutputLevelType level)
            {
                if (m_enabled && m_logLevel >= (int) level)
                {
                    return true;
                }

                return false;
            }
        }

        #endregion
    }
}
