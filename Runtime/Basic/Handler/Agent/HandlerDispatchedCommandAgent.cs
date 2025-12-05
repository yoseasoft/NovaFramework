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

namespace GameEngine
{
    /// <summary>
    /// 管理句柄分发模式的指令对象代理接口，用于对程序句柄管理器关心的事件消息进行分发
    /// </summary>
    public sealed class HandlerDispatchedCommandAgent : NovaEngine.ICommandAgent
    {
        /// <summary>
        /// 代理对象名称
        /// </summary>
        public const string COMMAND_AGENT_NAME = "HandlerDispatched";

        /// <summary>
        /// 指令代理初始化接口
        /// </summary>
        public void Initialize()
        {
        }

        /// <summary>
        /// 指令代理清理接口
        /// </summary>
        public void Cleanup()
        {
        }

        /// <summary>
        /// 指令代理处理目标指令参数调度接口
        /// </summary>
        /// <param name="command">指令对象实例</param>
        public void Call(NovaEngine.CommandArgs command)
        {
            NovaEngine.Module.ModuleCommandArgs moduleCommand = command as NovaEngine.Module.ModuleCommandArgs;

            if (false == HandlerManagement.OnEventDispatch(moduleCommand.Data))
            {
                Debugger.Error("Could not found any mached handler instance with current command type {%d}.", moduleCommand.Type);
            }

            NovaEngine.ReferencePool.Release(moduleCommand);
        }
    }
}
