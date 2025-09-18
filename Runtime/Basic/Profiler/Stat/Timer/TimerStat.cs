/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyright (C) 2022 - 2023, Shanghai Bilibili Technology Co., Ltd.
/// Copyright (C) 2023 - 2024, Guangzhou Shiyue Network Technology Co., Ltd.
/// Copyright (C) 2025, Hainan Yuanyou Information Tecdhnology Co., Ltd. Guangzhou Branch
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
    /// 定时统计模块，对定时模块对象提供数据统计所需的接口函数
    /// </summary>
    internal sealed class TimerStat : BaseStat<TimerStat, TimerStatInfo>
    {
        [IStat.OnStatFunctionRegister(StatCode.TimerStartup)]
        private void OnTimerStartup(int session, string name)
        {
            TimerStatInfo info = TryGetValue(session);
            if (null == info)
            {
                info = new TimerStatInfo(session);
                TryAddValue(info);
            }

            info.TimerName = name ?? NovaEngine.Definition.CString.Unknown;
            info.ScheduleCount++;
        }

        [IStat.OnStatFunctionRegister(StatCode.TimerFinished)]
        private void OnTimerFinished(int session)
        {
            TimerStatInfo info = TryGetValue(session);
            if (null == info)
            {
                Debugger.Warn("Could not found any timer stat info with session '{0}', finished it failed.", session);
                return;
            }

            info.FinishedCount++;
        }

        [IStat.OnStatFunctionRegister(StatCode.TimerDispatched)]
        private void OnTimerDispatched(int session)
        {
            TimerStatInfo info = TryGetValue(session);
            if (null == info)
            {
                Debugger.Warn("Could not found any timer stat info with session '{0}', dispatched it failed.", session);
                return;
            }

            info.BlinkCount++;
        }
    }
}
