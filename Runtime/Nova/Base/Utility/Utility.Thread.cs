/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
/// Copyright (C) 2022 - 2023, Shanghai Bilibili Technology Co., Ltd.
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
using System.Threading;

using Cysharp.Threading.Tasks;

namespace NovaEngine
{
    /// 实用函数集合工具类
    public static partial class Utility
    {
        /// <summary>
        /// 线程相关实用函数集合
        /// </summary>
        public static class Thread
        {
            // 当前运行线程的最大限制数
            private const int MAX_RUNNING_THREAD_COUNTS = 8;

            // 当前运行线程的实际计数
            private static int _runningThreads;

            /// <summary>
            /// 当前线程休眠指定的毫秒值
            /// </summary>
            /// <param name="milliseconds">毫秒值</param>
            public static void Sleep(int milliseconds)
            {
                Thread.Sleep(milliseconds);
            }

            /// <summary>
            /// 执行协程调度接口
            /// </summary>
            /// <param name="behaviour">mono组件</param>
            /// <param name="routine">协程对象实例</param>
            /// <returns>协程调度返回信息</returns>
            // public static UnityCoroutine StartCoroutine(UnityBehaviour behaviour, IEnumerator routine)
            public static void DoWork(ICoroutinable coroutine)
            {
                Logger.Assert(null != coroutine);

                // return behaviour.StartCoroutine(routine);
                WorkAsync(delegate () { coroutine.Work(); });
            }

            /// <summary>
            /// 异步方式执行目标任务逻辑
            /// </summary>
            /// <param name="action">目标任务项</param>
            public static async void WorkAsync(Action action)
            {
                await WorkAction(action);
            }

            /// <summary>
            /// 异步方式执行目标行为函数
            /// </summary>
            /// <param name="action">行为函数</param>
            private static async UniTask WorkAction(Action action)
            {
                action();
                await UniTask.CompletedTask;
            }

            /// <summary>
            /// 执行线程调度接口
            /// </summary>
            /// <param name="runnable">线程对象实例</param>
            public static void DoRun(IRunnable runnable)
            {
                Logger.Assert(null != runnable);

                RunAsync(delegate () { runnable.Run(); });
            }

            /// <summary>
            /// 异步方式执行目标任务逻辑
            /// </summary>
            /// <param name="action">目标任务项</param>
            public static void RunAsync(Action action)
            {
                while (_runningThreads >= MAX_RUNNING_THREAD_COUNTS)
                {
                    Thread.Sleep(100);
                }

                Interlocked.Increment(ref _runningThreads);
                ThreadPool.QueueUserWorkItem(RunAction, action);
            }

            /// <summary>
            /// 异步方式执行目标行为函数
            /// </summary>
            /// <param name="action">行为函数</param>
            private static void RunAction(object action)
            {
                try
                {
                    ((Action) action)();
                }
                catch (Exception e)
                {
                    Logger.Error(e.ToString());
                }
                finally
                {
                    Interlocked.Decrement(ref _runningThreads);
                }
            }
        }
    }
}
