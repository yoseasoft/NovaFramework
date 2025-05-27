/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
/// Copyring (C) 2020 - 2022, Guangzhou Xinyuan Technology Co., Ltd.
/// Copyring (C) 2022 - 2023, Shanghai Bilibili Technology Co., Ltd.
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

namespace NovaEngine
{
    /// <summary>
    /// 指令转发操作管理类
    /// </summary>
    public sealed class CommandDispatcher
    {
        private readonly Dictionary<string, ICommandAgent> m_commandAgents = null;

        /// <summary>
        /// 指令管理类的新实例构建接口
        /// </summary>
        public CommandDispatcher()
        {
            m_commandAgents = new Dictionary<string, ICommandAgent>();
        }

        /// <summary>
        /// 指令转发对象初始化函数
        /// </summary>
        public void Initialize()
        {
        }

        /// <summary>
        /// 指令转发对象清理函数
        /// </summary>
        public void Cleanup()
        {
            foreach (KeyValuePair<string, ICommandAgent> pair in m_commandAgents)
            {
                pair.Value.Cleanup();
            }

            m_commandAgents.Clear();
        }

        /// <summary>
        /// 增加指令代理对象实例
        /// </summary>
        /// <param name="cname">代理接口名称</param>
        /// <param name="agent">指令代理对象实例</param>
        public void AddAgent(string cname, ICommandAgent agent)
        {
            if (null == cname || null == agent)
            {
                throw new CFrameworkException("Command agent is invalid.");
            }

            if (m_commandAgents.ContainsKey(cname))
            {
                throw new CFrameworkException("Agent name '{%s}' is already exist.", cname);
            }

            agent.Initialize();
            m_commandAgents.Add(cname, agent);
        }

        /// <summary>
        /// 移除指令代理对象实例
        /// </summary>
        /// <param name="cname">代理接口名称</param>
        public void RemoveAgent(string cname)
        {
            if (null == cname)
            {
                throw new CFrameworkException("Command agent is invalid.");
            }

            if (false == m_commandAgents.ContainsKey(cname))
            {
                throw new CFrameworkException("Agent name '{%s}' is not exist.", cname);
            }

            ICommandAgent agent = m_commandAgents[cname];
            agent.Cleanup();

            m_commandAgents.Remove(cname);
        }

        /// <summary>
        /// 指令管理器处理目标指令参数调度接口
        /// </summary>
        /// <param name="command">指令对象实例</param>
        public void Call(CommandArgs command)
        {
            foreach (KeyValuePair<string, ICommandAgent> pair in m_commandAgents)
            {
                pair.Value.Call(command);
            }
        }
    }
}
