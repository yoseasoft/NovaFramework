/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
/// Copyright (C) 2020 - 2022, Guangzhou Xinyuan Technology Co., Ltd.
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

using SystemFile = System.IO.File;
using SystemFileInfo = System.IO.FileInfo;
using SystemStreamWriter = System.IO.StreamWriter;

namespace NovaEngine
{
    /// <summary>
    /// 日志相关函数集合工具类
    /// </summary>
    internal static partial class Logger
    {
        /// <summary>
        /// 日志输出文件模式操作管理类
        /// </summary>
        [LogOutputChannelBinding(LogOutputChannelType.File)]
        private sealed class LogFile : Singleton<LogFile>, ILogOutput
        {
            private SystemFileInfo _fileHandler = null;
            private SystemStreamWriter _fileWriter = null;

            /// <summary>
            /// 启动日志输出文件模式
            /// </summary>
            public static void Startup()
            {
                LogFile f = Instance;
                AddOutputChannel(f);
            }

            /// <summary>
            /// 关闭日志输出文件模式
            /// </summary>
            public static void Shutdown()
            {
                LogFile f = Instance;
                RemoveOutputChannel(f);
                Destroy();
            }

            /// <summary>
            /// 日志文件类初始化接口
            /// </summary>
            protected override void Initialize()
            {
                //if (false == f.Open(filename, backup))
                //{
                //    throw new CException("Log file open failed.");
                //}
            }

            /// <summary>
            /// 日志文件类清理接口
            /// </summary>
            protected override void Cleanup()
            {
                this.Close();
            }

            /// <summary>
            /// 日志管理器打开当前日志文件句柄接口，若指定备份文件路径，则将当前目标日志文件的历史记录写入备份文件中
            /// </summary>
            /// <param name="filename">目标日志文件</param>
            /// <param name="backup">备份日志文件</param>
            /// <returns>若打开日志文件句柄成功则返回true，否则返回false</returns>
            public bool Open(string filename, string backup)
            {
                if (null != _fileHandler)
                {
                    Close();
                }

                if (null == filename)
                {
                    return false;
                }

                filename = Utility.Resource.PersistentDataPath + "/" + filename;

                _fileHandler = new SystemFileInfo(filename);
                if (_fileHandler.Exists)
                {
                    if (null != backup)
                    {
                        // 已存在旧日志文件的情况下，将其内容进行备份处理
                        backup = Utility.Resource.PersistentDataPath + "/" + backup;
                        _fileHandler.CopyTo(backup, true);
                    }

                    // 清空原日志文件内容
                    _fileHandler.Delete();
                    _fileWriter = _fileHandler.CreateText();
                }
                else
                {
                    // 创建新的日志文件
                    _fileWriter = new SystemStreamWriter(filename, true, System.Text.Encoding.Default);
                }

                return true;
            }

            /// <summary>
            /// 日志管理器关闭当前日志文件句柄接口
            /// </summary>
            private void Close()
            {
                if (null != _fileWriter)
                {
                    _fileWriter.Close();
                    _fileWriter.Dispose();
                    _fileWriter = null;
                }

                _fileHandler = null;
            }

            /// <summary>
            /// 日志管理器对当前已打开的日志文件进行写入操作
            /// </summary>
            /// <param name="text">待写入的日志文本内容串</param>
            private void Write(string text)
            {
                if (null != _fileWriter)
                {
                    _fileWriter.WriteLine(text);
                }
            }

            /// <summary>
            /// 日志输入记录接口
            /// </summary>
            /// <param name="level">日志等级</param>
            /// <param name="message">日志内容</param>
            public void Output(LogOutputLevelType level, object message)
            {
                // 写入日志内容
                Write(message.ToString());
            }
        }
    }
}
