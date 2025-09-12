/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyright (C) 2022 - 2023, Shanghai Bilibili Technology Co., Ltd.
/// Copyright (C) 2023 - 2024, Guangzhou Shiyue Network Technology Co., Ltd.
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

using SystemDateTime = System.DateTime;

namespace GameEngine.Profiler.Statistics
{
    /// <summary>
    /// 网络统计模块，对网络模块对象提供数据统计所需的接口函数
    /// </summary>
    internal sealed class NetworkStat : StatSingleton<NetworkStat>, IStat
    {
        /// <summary>
        /// 网络数据统计信息容器列表
        /// </summary>
        private IDictionary<int, NetworkStatInfo> _networkStatInfos = null;

        /// <summary>
        /// 初始化统计模块实例的回调接口
        /// </summary>
        protected override void OnInitialize()
        {
            _networkStatInfos = new Dictionary<int, NetworkStatInfo>();
        }

        /// <summary>
        /// 清理统计模块实例的回调接口
        /// </summary>
        protected override void OnCleanup()
        {
            _networkStatInfos.Clear();
            _networkStatInfos = null;
        }

        /// <summary>
        /// 卸载统计模块实例中的垃圾数据
        /// </summary>
        public void Dump()
        {
            _networkStatInfos.Clear();
        }

        /// <summary>
        /// 获取当前所有网络数据的统计信息
        /// </summary>
        /// <returns>返回所有的网络数据统计信息</returns>
        public IList<IStatInfo> GetAllStatInfos()
        {
            List<IStatInfo> results = new List<IStatInfo>();
            results.AddRange(_networkStatInfos.Values);

            return results;
        }

        [IStat.OnStatFunctionRegister(StatCode.NetworkConnected)]
        private void OnConnected(int session)
        {
            NetworkStatInfo info = null;
            if (false == Instance._networkStatInfos.TryGetValue(session, out info))
            {
                info = new NetworkStatInfo(session);
                Instance._networkStatInfos.Add(session, info);
            }

            info.ConnectTime = SystemDateTime.UtcNow;
        }

        [IStat.OnStatFunctionRegister(StatCode.NetworkDisconnected)]
        private void OnDisconnected(int session)
        {
            NetworkStatInfo info = null;
            if (false == Instance._networkStatInfos.TryGetValue(session, out info))
            {
                Debugger.Warn("Could not found any network stat info with session '{0}', disconnect it failed.", session);
                return;
            }

            info.DisconnectTime = SystemDateTime.UtcNow;
        }

        [IStat.OnStatFunctionRegister(StatCode.NetworkSend)]
        private void OnSendData(int session, int dataSize)
        {
            NetworkStatInfo info = null;
            if (false == Instance._networkStatInfos.TryGetValue(session, out info))
            {
                Debugger.Warn("Could not found any network stat info with session '{0}', send data failed.", session);
                return;
            }

            info.SendCount++;
            info.SendSize += dataSize;
        }

        [IStat.OnStatFunctionRegister(StatCode.NetworkRecv)]
        private void OnRecvData(int session, int dataSize)
        {
            NetworkStatInfo info = null;
            if (false == Instance._networkStatInfos.TryGetValue(session, out info))
            {
                Debugger.Warn("Could not found any network stat info with session '{0}', recv data failed.", session);
                return;
            }

            info.RecvCount++;
            info.RecvSize += dataSize;
        }
    }
}
