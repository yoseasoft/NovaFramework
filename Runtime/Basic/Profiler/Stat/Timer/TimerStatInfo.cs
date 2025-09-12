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
        private readonly int _session;
        /// <summary>
        /// 任务名称
        /// </summary>
        private string _timerName;
        /// <summary>
        /// 任务的创建时间
        /// </summary>
        private SystemDateTime _createTime;
        /// <summary>
        /// 任务的最后使用时间
        /// </summary>
        private SystemDateTime _lastUseTime;
        /// <summary>
        /// 任务的调度次数
        /// </summary>
        private int _scheduleCount;
        /// <summary>
        /// 任务的结束次数
        /// </summary>
        private int _finishedCount;
        /// <summary>
        /// 任务的心跳次数
        /// </summary>
        private int _blinkCount;

        public TimerStatInfo(int session)
        {
            _session = session;
            _timerName = string.Empty;
            _createTime = SystemDateTime.MinValue;
            _lastUseTime = SystemDateTime.MinValue;
            _scheduleCount = 0;
            _finishedCount = 0;
            _blinkCount = 0;
        }

        public int Session
        {
            get { return _session; }
        }

        public string TimerName
        {
            get { return _timerName; }
            set { _timerName = value; }
        }

        public SystemDateTime CreateTime
        {
            get { return _createTime; }
            set { _createTime = value; }
        }

        public SystemDateTime LastUseTime
        {
            get { return _lastUseTime; }
            set { _lastUseTime = value; }
        }

        public int ScheduleCount
        {
            get { return _scheduleCount; }
            set { _scheduleCount = value; }
        }

        public int FinishedCount
        {
            get { return _finishedCount; }
            set { _finishedCount = value; }
        }

        public int BlinkCount
        {
            get { return _blinkCount; }
            set { _blinkCount = value; }
        }
    }
}
