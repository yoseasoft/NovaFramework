/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyright (C) 2022 - 2023, Shanghai Bilibili Technology Co., Ltd.
/// Copyright (C) 2023 - 2024, Guangzhou Shiyue Network Technology Co., Ltd.
/// Copyright (C) 2025, Hainan Yuanyou Information Technology Co., Ltd. Guangzhou Branch
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

namespace GameEngine.Profiler.Statistics
{
    /// <summary>
    /// 网络统计模块，对网络模块对象提供数据统计所需的接口函数
    /// </summary>
    internal sealed class NetworkStat : BaseStat<NetworkStat, NetworkStatInfo>
    {
        [IStat.OnStatFunctionRegister(StatCode.NetworkConnected)]
        private void OnConnected(int session)
        {
            NetworkStatInfo info = TryGetValue(session);
            if (null == info)
            {
                info = new NetworkStatInfo(session);
                TryAddValue(info);
            }
        }

        [IStat.OnStatFunctionRegister(StatCode.NetworkDisconnected)]
        private void OnDisconnected(int session)
        {
            TryCloseValue(session);
        }

        [IStat.OnStatFunctionRegister(StatCode.NetworkSend)]
        private void OnSendData(int session, int dataSize)
        {
            NetworkStatInfo info = TryGetValue(session);
            if (null == info)
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
            NetworkStatInfo info = TryGetValue(session);
            if (null == info)
            {
                Debugger.Warn("Could not found any network stat info with session '{0}', recv data failed.", session);
                return;
            }

            info.RecvCount++;
            info.RecvSize += dataSize;
        }
    }
}
