/// -------------------------------------------------------------------------------
/// GameEngine Framework
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

namespace GameEngine
{
    /// <summary>
    /// 用于框架内部的统计调用，集中控制业务统计相关的调度转发流程<br/>
    /// <br/>
    /// 注意：当正式版发布时，将忽略所有的统计信息
    /// </summary>
    internal static class _Profiler
    {
        /// <summary>
        /// 统计模块启动函数
        /// </summary>
        public static void Startup()
        {
            if (GameMacros.DEBUGGING_PROFILER_ONLINE_WINDOW_ENABLED)
            {
                NovaEngine.AppEntry.RegisterComponent<Profiler.Debugging.DebuggerComponent>(Profiler.Debugging.DebuggerComponent.MOUNTING_GAMEOBJECT_NAME);
            }

            if (GameMacros.DEBUGGING_PROFILER_STAT_MODULE_ENABLED)
            {
                Profiler.Statistics.StatisticalCenter.Startup();
            }
        }

        /// <summary>
        /// 统计模块关闭函数
        /// </summary>
        public static void Shutdown()
        {
            if (GameMacros.DEBUGGING_PROFILER_STAT_MODULE_ENABLED)
            {
                Profiler.Statistics.StatisticalCenter.Shutdown();
            }

            if (GameMacros.DEBUGGING_PROFILER_ONLINE_WINDOW_ENABLED)
            {
                NovaEngine.AppEntry.UnregisterComponent(Profiler.Debugging.DebuggerComponent.MOUNTING_GAMEOBJECT_NAME);
            }
        }

        /// <summary>
        /// 统计模块处理调用函数
        /// </summary>
        /// <param name="funcType">功能类型</param>
        /// <param name="args">参数列表</param>
        public static void CallStat(int funcType, params object[] args)
        {
            if (GameMacros.DEBUGGING_PROFILER_STAT_MODULE_ENABLED)
            {
                Profiler.Statistics.StatisticalCenter.Call(funcType, args);
            }
        }
    }
}
