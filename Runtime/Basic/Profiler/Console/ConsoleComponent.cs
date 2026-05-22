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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace GameEngine.Profiler.Debugging
{
    /// <summary>
    /// 控制台组件对象类，用于启动远程调试后台及命令响应处理
    /// </summary>
    [UnityEngine.DisallowMultipleComponent]
    [UnityEngine.AddComponentMenu("Framework/Console")]
    internal sealed partial class ConsoleComponent : NovaEngine.CFrameworkComponent
    {
        /// <summary>
        /// 控制台组件的挂载名称
        /// </summary>
        public const string MOUNTING_GAMEOBJECT_NAME = "GameConsole";

        /// <summary>机器本地回环地址</summary>
        private const string LocalLoopbackAddress = @"127.0.0.1";

        /// <summary>控制台服务运行地址</summary>
        private string _ip;
        /// <summary>控制台服务运行端口</summary>
        private int _port;
        /// <summary>控制台服务监听实例</summary>
        private TcpListener _listener;

        /// <summary>控制台会话实例管理容器</summary>
        private ConcurrentDictionary<int, ClientSession> _sessions;
        /// <summary>控制台指令事件缓存队列</summary>
        private ConcurrentQueue<CommandEvent> _commands;
        /// <summary>控制台会话标识计数器</summary>
        private int _sessionIdCounter;

        private readonly object _lock = new ();

        void Start()
        {
            Debugger.IsNull(_listener);

            _ip = LocalLoopbackAddress;
            _port = 8000;

            _sessions = new ConcurrentDictionary<int, ClientSession>();
            _commands = new ConcurrentQueue<CommandEvent>();
            _sessionIdCounter = 0;

            // 启动调试服务器
            StartListener();

            // 句柄指令初始化
            InitializeHandleCommands();
        }

        void Update()
        {
            if (_commands.Count > 0)
            {
                while (_commands.TryDequeue(out CommandEvent e))
                {
                    ProcessCommand(e.SessionId, e.Line);
                }
            }
        }

        void Destroy()
        {
            Debugger.IsNotNull(_listener);

            // 句柄指令清理
            CleanupHandleCommands();

            foreach (var session in _sessions.Values)
            {
                session.Close();
            }
            _sessions.Clear();
            _sessions = null;

            _listener?.Stop();
            _listener = null;
        }

        private void StartListener()
        {
            try
            {
                Debugger.Info(LogGroupTag.Profiler, "Starting debug console listener on {%s}:{%d}", _ip, _port);
                IPAddress ipAddress = IPAddress.Parse(_ip);

                _listener = new TcpListener(ipAddress, _port);
                _listener.Start();
                Debugger.Info(LogGroupTag.Profiler, "Debug console tcp listener started successfully.");

                _listener.BeginAcceptTcpClient(OnClientConnected, null);
            }
            catch (Exception ex)
            {
                Debugger.Error(LogGroupTag.Profiler, "Debug console service failed to start listener: {%s}, and the exception stack trace: {%s}.", ex.Message, ex.StackTrace);
            }
        }

        private void OnClientConnected(IAsyncResult ar)
        {
            try
            {
                if (null == _listener)
                    return;

                TcpClient client = _listener.EndAcceptTcpClient(ar);
                _listener.BeginAcceptTcpClient(OnClientConnected, null);

                int sessionId = Interlocked.Increment(ref _sessionIdCounter);
                var session = new ClientSession(this, sessionId, client);
                _sessions[sessionId] = session;

                Debugger.Info(LogGroupTag.Profiler, "Debug console client connected, session={%d}.", sessionId);
            }
            catch (Exception ex)
            {
                Debugger.Error(LogGroupTag.Profiler, "Debug console accept client error: {%s}.", ex.Message);
            }
        }

        /// <summary>
        /// 控制台的指令事件接收函数
        /// </summary>
        /// <param name="sessionId">会话标识</param>
        /// <param name="input">输入数据</param>
        private void OnCommandEventReceived(int sessionId, string input)
        {
            CommandEvent e = new CommandEvent(sessionId, input);
            _commands.Enqueue(e);

            // Thread.MemoryBarrier();
        }

        /// <summary>
        /// 控制台命令执行处理函数
        /// </summary>
        /// <param name="sessionId">会话标识</param>
        /// <param name="input">命令行</param>
        private void ProcessCommand(int sessionId, string input)
        {
            int errorCode = ExecuteCommand(input, out string response);

            if (_sessions.TryGetValue(sessionId, out var session))
            {
                session.SendResponse(response);

                if (ConsoleErrorCode.Quit == errorCode)
                {
                    session.Close();
                    ((IDictionary<int, ClientSession>) _sessions).Remove(sessionId);

                    Debugger.Info(LogGroupTag.Profiler, "Debug console client disconnected, session={%d}.", sessionId);
                }
            }
        }

        /// <summary>
        /// 控制台指定命令执行函数
        /// </summary>
        /// <param name="input">命令行</param>
        /// <param name="response">响应字符串</param>
        /// <returns>返回命令执行返回的错误码</returns>
        private int ExecuteCommand(string input, out string response)
        {
            int errorCode = CommandDispatched(input, out response);

            if (errorCode > 0)
            {
                response = GetErrorMessage(errorCode);
            }

            return errorCode;
        }

        /// <summary>
        /// 控制台的客户端会话对象类
        /// </summary>
        private sealed class ClientSession
        {
            /// <summary>控制台组件对象实例</summary>
            private readonly ConsoleComponent _service;
            /// <summary>会话唯一标识</summary>
            private readonly int _sessionId;
            /// <summary>客户端连接</summary>
            private readonly TcpClient _client;
            private readonly NetworkStream _stream;
            private readonly byte[] _buffer = new byte[4096];
            private readonly StringBuilder _lineBuffer = new StringBuilder();
            private bool _isClosed;

            public ClientSession(ConsoleComponent service, int sessionId, TcpClient client)
            {
                _service = service;
                _sessionId = sessionId;
                _client = client;
                _stream = _client.GetStream();
                _stream.BeginRead(_buffer, 0, _buffer.Length, OnDataReceived, null);

                SendResponse(@"Welcome to NovaFramework console:");
            }

            /// <summary>
            /// 网络数据接收回调接口函数
            /// </summary>
            private void OnDataReceived(IAsyncResult ar)
            {
                try
                {
                    if (null == _stream || false == _client.Connected)
                    {
                        Close();
                        return;
                    }

                    int bytesRead = _stream.EndRead(ar);
                    if (bytesRead == 0)
                    {
                        Close();
                        return;
                    }

                    string data = Encoding.UTF8.GetString(_buffer, 0, bytesRead);
                    ProcessData(data);

                    _stream.BeginRead(_buffer, 0, _buffer.Length, OnDataReceived, null);
                }
                catch
                {
                    Close();
                }
            }

            private void ProcessData(string data)
            {
                foreach (char c in data)
                {
                    if (c == NovaEngine.Definition.CCharacter.LF)
                    {
                        string line = _lineBuffer.ToString().Trim();
                        _lineBuffer.Clear();
                        if (false == string.IsNullOrWhiteSpace(line))
                        {
                            _service.OnCommandEventReceived(_sessionId, line);
                        }
                    }
                    else if (c != NovaEngine.Definition.CCharacter.CR)
                    {
                        _lineBuffer.Append(c);
                    }
                }
            }

            public void SendResponse(string message)
            {
                try
                {
                    if (null != _stream && _client.Connected)
                    {
                        byte[] data = Encoding.UTF8.GetBytes(message + NovaEngine.Definition.CString.CRLF);
                        _stream.Write(data, 0, data.Length);
                        _stream.Flush();
                    }
                }
                catch
                {
                    Close();
                }
            }

            /// <summary>
            /// 客户端会话关闭函数
            /// </summary>
            public void Close()
            {
                if (_isClosed) return;

                _isClosed = true;
                _stream.Close();
                _client.Close();
            }
        }
    }
}
