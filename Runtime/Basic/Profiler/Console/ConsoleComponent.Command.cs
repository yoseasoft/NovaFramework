/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyright (C) 2026, Hurley, Independent Studio.
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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace GameEngine.Profiler.Debugging
{
    /// 控制台组件对象类
    internal sealed partial class ConsoleComponent
    {
        private delegate int DebugConsoleHandleCommand(string command, out string response);

        /// <summary>
        /// 句柄指令类型的特性标签类
        /// </summary>
        private class HandleCommandTypeAttribute : Attribute
        {
            /// <summary>指令名称</summary>
            private readonly string _command;
            /// <summary>获取指令名称属性</summary>
            public string Command => _command;

            public HandleCommandTypeAttribute(string command) { _command = command; }
        }

        /// <summary>句柄指令缓存容器</summary>
        private IDictionary<string, DebugConsoleHandleCommand> _handleCommands;

        /// <summary>
        /// 初始化所有句柄指令委托
        /// </summary>
        private void InitializeHandleCommands()
        {
            _handleCommands = new Dictionary<string, DebugConsoleHandleCommand>();

            Type targetType = typeof(ConsoleComponent);
            MethodInfo[] methods = targetType.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            for (int n = 0; n < methods.Length; ++n)
            {
                MethodInfo method = methods[n];
                HandleCommandTypeAttribute handleCommandTypeAttr = method.GetCustomAttribute<HandleCommandTypeAttribute>();
                if (null != handleCommandTypeAttr)
                {
                    Debugger.Assert(method.IsStatic);

                    DebugConsoleHandleCommand callback = method.CreateDelegate(typeof(DebugConsoleHandleCommand), this) as DebugConsoleHandleCommand;
                    _handleCommands.Add(handleCommandTypeAttr.Command, callback);
                }
            }
        }

        /// <summary>
        /// 清理所有句柄指令委托
        /// </summary>
        private void CleanupHandleCommands()
        {
            _handleCommands.Clear();
            _handleCommands = null;
        }

        /// <summary>
        /// 指令分发调度接口函数
        /// </summary>
        /// <param name="input">命令行</param>
        /// <param name="response">响应字符串</param>
        /// <returns>返回指令处理的错误码，若执行正常则返回0</returns>
        private int CommandDispatched(string input, out string response)
        {
            // 1. 查找第一个空格的位置
            int spaceIndex = input.IndexOf(NovaEngine.Definition.CCharacter.Space);

            string cmd;
            string data;

            if (spaceIndex >= 0)
            {
                // 2. 截取从索引 0 开始，长度为 spaceIndex 的字符串
                cmd = input.Substring(0, spaceIndex);
                data = input.Substring(spaceIndex);
            }
            else
            {
                // 如果没有空格，根据业务需求返回原字符串或空字符串
                cmd = input;
                data = null;
            }

            // 转换为小写
            cmd = cmd.ToLower();

            if (false == _handleCommands.TryGetValue(cmd, out DebugConsoleHandleCommand handle))
            {
                Debugger.Warn(LogGroupTag.Profiler, "Could not found any handle command with target name '{%s}', dispatched it failed.", cmd);
                response = null;
                return ConsoleErrorCode.InvalidCommand;
            }

            return handle.Invoke(data, out response);
        }

        /// <summary>
        /// 指令事件对象类
        /// </summary>
        private sealed class CommandEvent // : NovaEngine.IReference
        {
            /// <summary>会话标识</summary>
            private readonly int _sessionId;
            /// <summary>输入文本</summary>
            private readonly string _line;

            public int SessionId => _sessionId;
            public string Line => _line;

            public CommandEvent(int sessionId, string line)
            {
                _sessionId = sessionId;
                _line = line;
            }

            /// <summary>
            /// 指令事件对象的默认初始化回调函数
            /// </summary>
            // public void Initialize() { }

            /// <summary>
            /// 指令事件对象的默认清理回调函数
            /// </summary>
            // public void Cleanup() { }
        }
    }
}
