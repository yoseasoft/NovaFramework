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
using System.Text;

namespace GameEngine.Profiler.Debugging
{
    /// 控制台组件对象类
    internal sealed partial class ConsoleComponent
    {
        [HandleCommandType("help")]
        private int CommandOfHelp(string command, out string response)
        {
            StringBuilder help = new StringBuilder();
            help.AppendLine("Welcome to NovaFramework debug console");
            help.AppendLine("");
            help.AppendLine("Available commands:");
            help.AppendLine("  help                    - Show this help message");
            help.AppendLine("  list                    - List all services");
            help.AppendLine("  stat [timeout]          - Dump all stats (message queue, tasks)");
            help.AppendLine("  mem [timeout]           - Show memory status");
            help.AppendLine("  gc [timeout]            - Force garbage collection on all services");
            help.AppendLine("  ping <address>          - Measure response time");
            help.AppendLine("  kill <address>          - Kill a service");
            help.AppendLine("  exit <address>          - Send exit message to service");
            help.AppendLine("  info <address>          - Get service information");
            help.AppendLine("  start <service_name>    - Launch a new service by module name");
            help.AppendLine("  service                 - List unique services");
            help.AppendLine("  task <address>          - Show service task details");
            help.AppendLine("  netstat                 - Show network connection status");
            help.AppendLine("  signal <address> [sig]  - Send signal to service");
            help.AppendLine("  clearcache              - Clear code cache");
            help.AppendLine("  getenv <name>           - Get environment variable");
            help.AppendLine("  setenv <name> <value>   - Set environment variable");
            help.AppendLine("  logon <address>         - Turn on service log");
            help.AppendLine("  logoff <address>        - Turn off service log");
            help.AppendLine("  quit                    - Exit telnet console");
            help.AppendLine("");
            help.AppendLine("Address format for commands (ping, kill, exit, info, task, signal, logon, logoff):");
            help.AppendLine("  :00000001   - 8 hex digit handle (use 'list' command to see available handles)");
            help.AppendLine("  .servicename - Named service (e.g., .launcher, .logger)");

            response = help.ToString();
            return 0;
        }

        [HandleCommandType("getenv")]
        private int CommandOfGetenv(string command, out string response)
        {
            response = NovaEngine.Environment.GetValue(command.Trim());
            return 0;
        }

        [HandleCommandType("setenv")]
        private int CommandOfSetenv(string command, out string response)
        {
            response = null;
            return 0;
        }

        [HandleCommandType("quit")]
        private int CommandOfQuit(string command, out string response)
        {
            response = null;
            return ConsoleErrorCode.Quit;
        }
    }
}
