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

namespace GameEngine
{
    /// <summary>
    /// 定时统计模块，对定时模块对象提供数据统计所需的接口函数
    /// </summary>
    public sealed class TimerStatModule : HandlerStatSingleton<TimerStatModule>, IStatModule
    {
        public const int ON_TIMER_STARTUP_CALL = 1;
        public const int ON_TIMER_FINISHED_CALL = 2;
        public const int ON_TIMER_DISPATCHED_CALL = 3;

        /// <summary>
        /// 定时任务统计信息容器列表
        /// </summary>
        private IDictionary<int, TimerStatInfo> _timerStatInfos = null;

        /// <summary>
        /// 初始化统计模块实例的回调接口
        /// </summary>
        protected override void OnInitialize()
        {
            _timerStatInfos = new Dictionary<int, TimerStatInfo>();
        }

        /// <summary>
        /// 清理统计模块实例的回调接口
        /// </summary>
        protected override void OnCleanup()
        {
            _timerStatInfos.Clear();
            _timerStatInfos = null;
        }

        /// <summary>
        /// 卸载统计模块实例中的垃圾数据
        /// </summary>
        public void Dump()
        {
            _timerStatInfos.Clear();
        }

        /// <summary>
        /// 获取当前所有定时任务的统计信息
        /// </summary>
        /// <returns>返回所有的定时任务统计信息</returns>
        public IList<IStatInfo> GetAllStatInfos()
        {
            List<IStatInfo> results = new List<IStatInfo>();
            results.AddRange(_timerStatInfos.Values);

            return results;
        }

        [IStatModule.OnStatModuleRegisterCallback(ON_TIMER_STARTUP_CALL)]
        private void OnTimerStartup(int session, string name)
        {
            TimerStatInfo info = null;
            if (false == _timerStatInfos.TryGetValue(session, out info))
            {
                info = new TimerStatInfo(session);
                _timerStatInfos.Add(session, info);
            }

            info.TimerName = name??NovaEngine.Definition.CString.Unknown;
            info.CreateTime = SystemDateTime.UtcNow;
            info.LastUseTime = SystemDateTime.UtcNow;
            info.ScheduleCount++;
        }

        [IStatModule.OnStatModuleRegisterCallback(ON_TIMER_FINISHED_CALL)]
        private void OnTimerFinished(int session)
        {
            TimerStatInfo info = null;
            if (false == _timerStatInfos.TryGetValue(session, out info))
            {
                Debugger.Warn("Could not found any timer stat info with session '{0}', finished it failed.", session);
                return;
            }

            info.FinishedCount++;
        }

        [IStatModule.OnStatModuleRegisterCallback(ON_TIMER_DISPATCHED_CALL)]
        private void OnTimerDispatched(int session)
        {
            TimerStatInfo info = null;
            if (false == _timerStatInfos.TryGetValue(session, out info))
            {
                Debugger.Warn("Could not found any timer stat info with session '{0}', dispatched it failed.", session);
                return;
            }

            info.BlinkCount++;
        }
    }
}
