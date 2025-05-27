/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
/// Copyring (C) 2017 - 2020, Shanghai Tommon Network Technology Co., Ltd.
/// Copyring (C) 2020 - 2022, Guangzhou Xinyuan Technology Co., Ltd.
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

using SystemEventHandler = System.EventHandler;

namespace NovaEngine.Network
{
    /// <summary>
    /// Web请求管理控制器定义接口类
    /// </summary>
    public interface IWebRequestController
    {
        /// <summary>
        /// 获取Web请求代理总数量
        /// </summary>
        int TotalAgentCount
        {
            get;
        }

        /// <summary>
        /// 获取可用Web请求代理数量
        /// </summary>
        int FreeAgentCount
        {
            get;
        }

        /// <summary>
        /// 获取工作中Web请求代理数量
        /// </summary>
        int WorkingAgentCount
        {
            get;
        }

        /// <summary>
        /// 获取等待中Web请求任务数量
        /// </summary>
        int WaitingTaskCount
        {
            get;
        }

        /// <summary>
        /// 获取或设置Web请求超时时长，以秒为单位
        /// </summary>
        float Timeout
        {
            get;
            set;
        }

        /// <summary>
        /// 增加Web请求任务
        /// </summary>
        /// <param name="webRequestUri">Web请求地址</param>
        /// <returns>返回新增Web请求任务的序列编号</returns>
        int AddWebRequest(string webRequestUri);

        /// <summary>
        /// 增加Web请求任务
        /// </summary>
        /// <param name="webRequestUri">Web请求地址</param>
        /// <param name="postData">要发送的数据流</param>
        /// <returns>返回新增Web请求任务的序列编号</returns>
        int AddWebRequest(string webRequestUri, byte[] postData);

        /// <summary>
        /// 增加Web请求任务
        /// </summary>
        /// <param name="webRequestUri">Web请求地址</param>
        /// <param name="priority">Web请求任务的优先级</param>
        /// <returns>返回新增Web请求任务的序列编号</returns>
        int AddWebRequest(string webRequestUri, int priority);

        /// <summary>
        /// 增加Web请求任务
        /// </summary>
        /// <param name="webRequestUri">Web请求地址</param>
        /// <param name="userData">用户自定义数据</param>
        /// <returns>返回新增Web请求任务的序列编号</returns>
        int AddWebRequest(string webRequestUri, object userData);

        /// <summary>
        /// 增加Web请求任务
        /// </summary>
        /// <param name="webRequestUri">Web请求地址</param>
        /// <param name="postData">要发送的数据流</param>
        /// <param name="priority">Web请求任务的优先级</param>
        /// <returns>返回新增Web请求任务的序列编号</returns>
        int AddWebRequest(string webRequestUri, byte[] postData, int priority);

        /// <summary>
        /// 增加Web请求任务
        /// </summary>
        /// <param name="webRequestUri">Web请求地址</param>
        /// <param name="postData">要发送的数据流</param>
        /// <param name="userData">用户自定义数据</param>
        /// <returns>返回新增Web请求任务的序列编号</returns>
        int AddWebRequest(string webRequestUri, byte[] postData, object userData);

        /// <summary>
        /// 增加Web请求任务
        /// </summary>
        /// <param name="webRequestUri">Web请求地址</param>
        /// <param name="priority">Web请求任务的优先级</param>
        /// <param name="userData">用户自定义数据</param>
        /// <returns>返回新增Web请求任务的序列编号</returns>
        int AddWebRequest(string webRequestUri, int priority, object userData);

        /// <summary>
        /// 增加Web请求任务
        /// </summary>
        /// <param name="webRequestUri">Web请求地址</param>
        /// <param name="postData">要发送的数据流</param>
        /// <param name="priority">Web请求任务的优先级</param>
        /// <param name="userData">用户自定义数据</param>
        /// <returns>返回新增Web请求任务的序列编号</returns>
        int AddWebRequest(string webRequestUri, byte[] postData, int priority, object userData);

        /// <summary>
        /// 移除Web请求任务
        /// </summary>
        /// <param name="serialId">要移除Web请求任务的序列编号</param>
        /// <returns>返回移除Web请求任务是否成功</returns>
        bool RemoveWebRequest(int serialId);

        /// <summary>
        /// 移除所有Web请求任务
        /// </summary>
        void RemoveAllWebRequests();

        /// <summary>
        /// 获取所有Web请求任务的信息
        /// </summary>
        /// <returns>所有Web请求任务的信息</returns>
        TaskInfo[] GetAllWebRequestInfos();
    }
}
