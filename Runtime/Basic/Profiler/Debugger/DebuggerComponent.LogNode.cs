/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyright (C) 2023, Guangzhou Shiyue Network Technology Co., Ltd.
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

using UnityLogType = UnityEngine.LogType;

namespace GameEngine.Profiler.Debugging
{
    /// <summary>
    /// 游戏调试器组件对象类，用于定义调试器对象的基础属性及访问操作函数
    /// </summary>
    public sealed partial class DebuggerComponent
    {
        /// <summary>
        /// 调试日志的记录节点对象类，用于记录单条日志记录
        /// </summary>
        public sealed class LogNode : NovaEngine.IReference
        {
            /// <summary>
            /// 日志记录发生的时间
            /// </summary>
            private DateTime _time;
            /// <summary>
            /// 日志记录发生的帧数
            /// </summary>
            private int _frameCount;
            /// <summary>
            /// 日志记录的类型
            /// </summary>
            private UnityLogType _type;
            /// <summary>
            /// 日志记录的内容
            /// </summary>
            private string _message;
            /// <summary>
            /// 日志记录的栈信息
            /// </summary>
            private string _stackTrace;

            /// <summary>
            /// 获取当前日志记录发生的时间
            /// </summary>
            public DateTime Time { get { return _time; } }
            /// <summary>
            /// 获取当前日志记录发生的帧数
            /// </summary>
            public int FrameCount { get { return _frameCount; } }
            /// <summary>
            /// 获取当前日志记录的类型
            /// </summary>
            public UnityLogType Type { get { return _type; } }
            /// <summary>
            /// 获取当前日志记录的内容
            /// </summary>
            public string Message { get { return _message; } }
            /// <summary>
            /// 获取当前日志记录的栈信息
            /// </summary>
            public string StackTrace { get { return _stackTrace; } }

            public LogNode()
            {
                _time = default;
                _frameCount = 0;
                _type = UnityLogType.Error;
                _message = null;
                _stackTrace = null;
            }

            /// <summary>
            /// 日志节点的初始化回调函数
            /// </summary>
            public void Initialize()
            {
            }

            /// <summary>
            /// 日志节点的清理回调函数
            /// </summary>
            public void Cleanup()
            {
                _time = default;
                _frameCount = 0;
                _type = UnityLogType.Error;
                _message = null;
                _stackTrace = null;
            }

            /// <summary>
            /// 创建一个日志节点对象实例<br/>
            /// 该接口将从缓冲池中进行新对象实例的分派，它比每次通过“new”进行新对象创建的方式性能更好，推荐使用该接口进行新节点实例的创建<br/>
            /// 但如果使用了该接口创建新对象，则必须使用<see cref="DebuggerComponent.LogNode.Release(LogNode)"/>接口进行实例的回收
            /// </summary>
            /// <param name="type"></param>
            /// <param name="message"></param>
            /// <param name="stackTrace"></param>
            /// <returns></returns>
            public static LogNode Create(UnityLogType type, string message, string stackTrace)
            {
                LogNode node = NovaEngine.ReferencePool.Acquire<LogNode>();
                node._time = DateTime.UtcNow;
                node._frameCount = NovaEngine.Timestamp.FrameCount;
                node._type = type;
                node._message = message;
                node._stackTrace = stackTrace;
                return node;
            }

            /// <summary>
            /// 回收指定的日志节点对象实例<br/>
            /// 回收的目标节点对象实例，必须是通过<see cref="DebuggerComponent.LogNode.Create(UnityLogType, string, string)"/>函数分配的
            /// </summary>
            /// <param name="node">日志节点对象实例</param>
            public static void Release(LogNode node)
            {
                NovaEngine.ReferencePool.Release(node);
            }
        }
    }
}
