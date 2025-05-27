/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
/// Copyring (C) 2023, Guangzhou Shiyue Network Technology Co., Ltd.
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

using System.Threading.Tasks;

using SystemPath = System.IO.Path;
using SystemProcess = System.Diagnostics.Process;
using SystemProcessStartInfo = System.Diagnostics.ProcessStartInfo;
using SystemRuntimeInformation = System.Runtime.InteropServices.RuntimeInformation;
using SystemOSPlatform = System.Runtime.InteropServices.OSPlatform;

using UniTaskVoid = Cysharp.Threading.Tasks.UniTaskVoid;

namespace NovaEngine
{
    /// <summary>
    /// 进程管理封装对象类，对系统进程相关操作提供函数接口
    /// </summary>
    public static class Process
    {
        /// <summary>
        /// 运行指定工作目录下的可执行程序
        /// </summary>
        /// <param name="exe">可执行程序名称</param>
        /// <param name="arguments">参数列表</param>
        /// <param name="workingDirectory">工作目录路径</param>
        /// <param name="waitExit">等待退出状态标识</param>
        /// <returns>返回给定名称的可执行程序运行句柄</returns>
        public static SystemProcess Run(string exe, string arguments, string workingDirectory = ".", bool waitExit = false)
        {
            Logger.Debug($"Process Run exe: {exe}, arguments: {arguments}, workingDirectory: {workingDirectory}.");
            try
            {
                bool redirectStandardOutput = true;
                bool redirectStandardError = true;
                bool useShellExecute = false;
                if (SystemRuntimeInformation.IsOSPlatform(SystemOSPlatform.Windows))
                {
                    redirectStandardOutput = false;
                    redirectStandardError = false;
                    useShellExecute = true;
                }

                if (waitExit)
                {
                    redirectStandardOutput = true;
                    redirectStandardError = true;
                    useShellExecute = false;
                }

                SystemProcessStartInfo info = new SystemProcessStartInfo
                {
                    FileName = exe,
                    Arguments = arguments,
                    CreateNoWindow = true,
                    UseShellExecute = useShellExecute,
                    WorkingDirectory = workingDirectory,
                    RedirectStandardOutput = redirectStandardOutput,
                    RedirectStandardError = redirectStandardError,
                };

                SystemProcess process = SystemProcess.Start(info);
                if (waitExit)
                {
                    WaitExitAsync(process).Forget();
                }

                return process;
            }
            catch (System.Exception e)
            {
                throw new CFrameworkException($"dir: {SystemPath.GetFullPath(workingDirectory)}, command: {exe} {arguments}.", e);
            }
        }

        /// <summary>
        /// 以异步方式等待进程退出
        /// </summary>
        /// <param name="process">进程实例</param>
        private static async UniTaskVoid WaitExitAsync(SystemProcess process)
        {
            await process.WaitForExitAsync();

            Logger.Info("process exit, exit code: {0} {1} {2}",
                process.ExitCode, process.StandardOutput.ReadToEnd(), process.StandardError.ReadToEnd());
        }

        /// <summary>
        /// 扩展<see cref="System.Diagnostics.Process"/>类的接口，增加等待进程退出的异步操作函数
        /// </summary>
        /// <param name="self">进程对象类</param>
        private static async Task WaitForExitAsync(this SystemProcess self)
        {
            if (false == self.HasExited)
            {
                return;
            }

            try
            {
                self.EnableRaisingEvents = true;
            }
            catch (System.InvalidOperationException)
            {
                if (self.HasExited)
                {
                    return;
                }

                throw;
            }

            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
            void Handler(object obj, System.EventArgs e) => tcs.TrySetResult(true);
            self.Exited += Handler;

            try
            {
                if (self.HasExited)
                {
                    return;
                }

                await tcs.Task;
            }
            finally
            {
                // 确保移除为该进程捆绑的回调句柄
                self.Exited -= Handler;
            }
        }
    }
}
