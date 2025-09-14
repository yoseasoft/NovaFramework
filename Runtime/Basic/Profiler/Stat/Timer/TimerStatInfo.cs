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

using SystemDateTime = System.DateTime;

namespace GameEngine.Profiler.Statistics
{
    /// <summary>
    /// 定时模块统计项对象类，对定时模块操作记录进行单项统计的数据单元
    /// </summary>
    public sealed class TimerStatInfo : IStatInfo
    {
        /// <summary>
        /// 任务的会话标识
        /// </summary>
        public int Session { get; private set; }
        /// <summary>
        /// 任务名称
        /// </summary>
        public string TimerName { get; internal set; }
        /// <summary>
        /// 任务的创建时间
        /// </summary>
        public SystemDateTime CreateTime { get; internal set; }
        /// <summary>
        /// 任务的最后使用时间
        /// </summary>
        public SystemDateTime LastUseTime { get; internal set; }
        /// <summary>
        /// 任务的调度次数
        /// </summary>
        public int ScheduleCount { get; internal set; }
        /// <summary>
        /// 任务的结束次数
        /// </summary>
        public int FinishedCount { get; internal set; }
        /// <summary>
        /// 任务的心跳次数
        /// </summary>
        public int BlinkCount { get; internal set; }

        public TimerStatInfo(int session)
        {
            this.Session = session;
            this.TimerName = string.Empty;
            this.CreateTime = SystemDateTime.MinValue;
            this.LastUseTime = SystemDateTime.MinValue;
            this.ScheduleCount = 0;
            this.FinishedCount = 0;
            this.BlinkCount = 0;
        }
    }
}
