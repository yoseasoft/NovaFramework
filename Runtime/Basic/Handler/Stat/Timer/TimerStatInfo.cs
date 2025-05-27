/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyring (C) 2022 - 2023, Shanghai Bilibili Technology Co., Ltd.
/// Copyring (C) 2023 - 2024, Guangzhou Shiyue Network Technology Co., Ltd.
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

namespace GameEngine
{
    /// <summary>
    /// 定时模块统计项对象类，对定时模块操作记录进行单项统计的数据单元
    /// </summary>
    public sealed class TimerStatInfo : IStatInfo
    {
        /// <summary>
        /// 任务的会话标识
        /// </summary>
        private readonly int m_session;
        /// <summary>
        /// 任务名称
        /// </summary>
        private string m_timerName;
        /// <summary>
        /// 任务的创建时间
        /// </summary>
        private SystemDateTime m_createTime;
        /// <summary>
        /// 任务的最后使用时间
        /// </summary>
        private SystemDateTime m_lastUseTime;
        /// <summary>
        /// 任务的调度次数
        /// </summary>
        private int m_scheduleCount;
        /// <summary>
        /// 任务的结束次数
        /// </summary>
        private int m_finishedCount;
        /// <summary>
        /// 任务的心跳次数
        /// </summary>
        private int m_blinkCount;

        public TimerStatInfo(int session)
        {
            m_session = session;
            m_timerName = string.Empty;
            m_createTime = SystemDateTime.MinValue;
            m_lastUseTime = SystemDateTime.MinValue;
            m_scheduleCount = 0;
            m_finishedCount = 0;
            m_blinkCount = 0;
        }

        public int Session
        {
            get { return m_session; }
        }

        public string TimerName
        {
            get { return m_timerName; }
            set { m_timerName = value; }
        }

        public SystemDateTime CreateTime
        {
            get { return m_createTime; }
            set { m_createTime = value; }
        }

        public SystemDateTime LastUseTime
        {
            get { return m_lastUseTime; }
            set { m_lastUseTime = value; }
        }

        public int ScheduleCount
        {
            get { return m_scheduleCount; }
            set { m_scheduleCount = value; }
        }

        public int FinishedCount
        {
            get { return m_finishedCount; }
            set { m_finishedCount = value; }
        }

        public int BlinkCount
        {
            get { return m_blinkCount; }
            set { m_blinkCount = value; }
        }
    }
}
