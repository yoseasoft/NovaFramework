/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
/// Copyright (C) 2022 - 2023, Shanghai Bilibili Technology Co., Ltd.
/// Copyright (C) 2023, Guangzhou Shiyue Network Technology Co., Ltd.
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

using System.Globalization;

using SystemMath = System.Math;
using SystemConvert = System.Convert;
using SystemTimeSpan = System.TimeSpan;
using SystemDateTime = System.DateTime;
using SystemDateTimeKind = System.DateTimeKind;
using SystemDateTimeOffset = System.DateTimeOffset;
using SystemDayOfWeek = System.DayOfWeek;

namespace NovaEngine
{
    /// 实用函数集合工具类
    public static partial class Utility
    {
        /// <summary>
        /// 日期时间相关实用函数集合
        /// </summary>
        public static class DateTime
        {
            /*
             * 1s  = 1000ms
             * 1ms = 1000us
             */

            private readonly static SystemDateTime Epoch = new SystemDateTime(1970, 1, 1, 0, 0, 0, 0, SystemDateTimeKind.Utc);
            private readonly static long EpochTicks = new SystemDateTime(1970, 1, 1, 0, 0, 0, 0, SystemDateTimeKind.Utc).Ticks;
            private readonly static long SecDivisor = (long) SystemMath.Pow(10, 7);
            private readonly static long MsecDivisor = (long) SystemMath.Pow(10, 4);

            /// <summary>
            /// 获取秒级别的UTC时间戳
            /// </summary>
            /// <returns>返回秒级别时间戳</returns>
            public static long SecondTimestamp()
            {
                return (SystemDateTime.UtcNow.Ticks - EpochTicks) / SecDivisor;
            }

            /// <summary>
            /// 获取毫秒级别的UTC时间戳
            /// </summary>
            /// <returns>返回毫秒级别时间戳</returns>
            public static long MillisecondTimestamp()
            {
                return (SystemDateTime.UtcNow.Ticks - EpochTicks) / MsecDivisor;
            }

            /// <summary>
            /// 获取微秒级别的UTC时间戳
            /// </summary>
            /// <returns>返回微秒级别时间戳</returns>
            public static long MicrosecondTimeStamp()
            {
                return SystemDateTime.UtcNow.Ticks - EpochTicks;
            }

            /// <summary>
            /// 获取秒级别的UTC时间
            /// </summary>
            /// <returns>返回秒级别 long 类型时间</returns>
            public static long SecondNow()
            {
                return SystemDateTime.UtcNow.Ticks / SecDivisor;
            }

            /// <summary>
            /// 获取毫秒级别的UTC时间
            /// </summary>
            /// <returns>返回毫秒级别 long 类型时间</returns>
            public static long MillisecondNow()
            {
                return SystemDateTime.UtcNow.Ticks / MsecDivisor;
            }

            /// <summary>
            /// 获取微秒级别的UTC时间
            /// </summary>
            /// <returns>返回微秒级别 long 类型时间</returns>
            public static long MicrosecondNow()
            {
                return SystemDateTime.UtcNow.Ticks;
            }

            /// <summary>
            /// 将时间戳转换为UTC时间对象实例
            /// </summary>
            /// <param name="timestamp">时间戳</param>
            /// <returns>返回UTC时间对象实例</returns>
            public static SystemDateTime TimestampToDateTime(long timestamp)
            {
                int length = (int) SystemMath.Floor(SystemMath.Log10(timestamp));
                SystemDateTime dateTime = new SystemDateTime(1970, 1, 1, 0, 0, 0, 0, SystemDateTimeKind.Utc);
                if (length == 9)
                    return dateTime.AddSeconds(timestamp);
                if (length == 12)
                    return dateTime.AddMilliseconds(timestamp);
                if (length == 16)
                    return dateTime.AddTicks(timestamp);
                return SystemDateTime.MinValue;
            }

            /// <summary>
            /// 获取当前时间相对于纪元时间的秒数
            /// </summary>
            /// <returns>返回时间秒数</returns>
            public static long GetTotalSeconds()
            {
                SystemTimeSpan ts = SystemDateTime.UtcNow - Epoch;
                return SystemConvert.ToInt64(ts.TotalSeconds);
            }

            /// <summary>
            /// 获取当前时间相对于纪元时间的毫秒数
            /// </summary>
            /// <returns>返回时间毫秒数</returns>
            public static long GetTotalMilliseconds()
            {
                SystemTimeSpan ts = SystemDateTime.UtcNow - Epoch;
                return SystemConvert.ToInt64(ts.TotalMilliseconds);
            }

            /// <summary>
            /// 获取当前时间相对于纪元时间的微秒数
            /// </summary>
            /// <returns>返回时间微秒数</returns>
            public static long GetTotalMicroseconds()
            {
                SystemTimeSpan ts = SystemDateTime.UtcNow - Epoch;
                return SystemConvert.ToInt64(ts.TotalMilliseconds) / 10;
            }

            /// <summary>
            /// 获取当前时间相对于纪元时间的分钟数
            /// </summary>
            /// <returns>返回时间分钟数</returns>
            public static double GetTotalMinutes()
            {
                SystemTimeSpan ts = SystemDateTime.UtcNow - Epoch;
                return SystemConvert.ToInt64(ts.TotalMinutes);
            }

            /// <summary>
            /// 获取当前时间相对于纪元时间的小时数
            /// </summary>
            /// <returns>返回时间小时数</returns>
            public static double GetTotalHours()
            {
                SystemTimeSpan ts = SystemDateTime.UtcNow - Epoch;
                return SystemConvert.ToInt64(ts.TotalHours);
            }

            /// <summary>
            /// 获取当前时间相对于纪元时间的天数
            /// </summary>
            /// <returns>返回时间天数</returns>
            public static double GetTotalDays()
            {
                SystemTimeSpan ts = SystemDateTime.UtcNow - Epoch;
                return SystemConvert.ToInt64(ts.TotalDays);
            }

            /// <summary>
            /// 获取某一年内总共有多少周
            /// </summary>
            /// <param name="year">年份</param>
            /// <returns>返回该年周数</returns>
            public static int GetWeekAmount(int year)
            {
                SystemDateTime end = new SystemDateTime(year, 12, 31); // 该年最后一天
                GregorianCalendar gc = new GregorianCalendar();
                return gc.GetWeekOfYear(end, CalendarWeekRule.FirstDay, SystemDayOfWeek.Monday); // 该年星期数
            }

            /// <summary>
            /// 获得一年中的某一周的起始日期和截止日期
            /// </summary>
            /// <param name="year">年份</param>
            /// <param name="numWeek">第几周</param>
            /// <param name="dtWeekStart">开始日期</param>
            /// <param name="dtWeekeEnd">结束日期</param>
            public static void GetWeekTime(int year, int numWeek, out SystemDateTime dtWeekStart, out SystemDateTime dtWeekeEnd)
            {
                SystemDateTime dt = new SystemDateTime(year, 1, 1);
                dt += new SystemTimeSpan((numWeek - 1) * 7, 0, 0, 0);
                dtWeekStart = dt.AddDays((int) SystemDayOfWeek.Monday - (int) dt.DayOfWeek);
                dtWeekeEnd = dt.AddDays((int) SystemDayOfWeek.Saturday - (int) dt.DayOfWeek + 1);
            }

            /// <summary>
            /// 获取一年中的某周的起始日期和截止日期<br/>
            /// 仅限周一到周五的工作日时间
            /// </summary>
            /// <param name="year">年份</param>
            /// <param name="numWeek">第几周</param>
            /// <param name="dtWeekStart">开始日期</param>
            /// <param name="dtWeekeEnd">结束日期</param>
            public static void GetWeekWorkTime(int year, int numWeek, out SystemDateTime dtWeekStart, out SystemDateTime dtWeekeEnd)
            {
                SystemDateTime dt = new SystemDateTime(year, 1, 1);
                dt += new SystemTimeSpan((numWeek - 1) * 7, 0, 0, 0);
                dtWeekStart = dt.AddDays((int) SystemDayOfWeek.Monday - (int) dt.DayOfWeek);
                dtWeekeEnd = dt.AddDays((int) SystemDayOfWeek.Saturday - (int) dt.DayOfWeek + 1).AddDays(-2);
            }

            /// <summary>
            /// 获取某年某月的最后一天
            /// </summary>
            /// <param name="year">年份</param>
            /// <param name="month">月份</param>
            /// <returns>返回指定日期</returns>
            public static int GetMonthLastDate(int year, int month)
            {
                SystemDateTime lastDay = new SystemDateTime(year, month, new GregorianCalendar().GetDaysInMonth(year, month));
                int day = lastDay.Day;
                return day;
            }

            /// <summary>
            /// 获得一段时间内有多少小时
            /// </summary>
            /// <param name="lhs">起始时间</param>
            /// <param name="rhs">终止时间</param>
            /// <returns>返回小时差的字符串信息</returns>
            public static string GetTimeDelay(SystemDateTime lhs, SystemDateTime rhs)
            {
                long lTicks = (rhs.Ticks - lhs.Ticks) / 10000000;
                string sTemp = (lTicks / 3600).ToString().PadLeft(2, '0') + ":";
                sTemp += (lTicks % 3600 / 60).ToString().PadLeft(2, '0') + ":";
                sTemp += (lTicks % 3600 % 60).ToString().PadLeft(2, '0');
                return sTemp;
            }

            /// <summary>
            /// 获取指定年份中指定月有多少天
            /// </summary>
            /// <param name="year">年份</param>
            /// <param name="month">月份</param>
            /// <returns>返回该月的天数</returns>
            public static int GetDaysOfMonth(int year, int month)
            {
                switch (month)
                {
                    case 1:
                        return 31;
                    case 2:
                        return IsLeapYear(year) ? 29 : 28;
                    case 3:
                        return 31;
                    case 4:
                        return 30;
                    case 5:
                        return 31;
                    case 6:
                        return 30;
                    case 7:
                        return 31;
                    case 8:
                        return 31;
                    case 9:
                        return 30;
                    case 10:
                        return 31;
                    case 11:
                        return 30;
                    case 12:
                        return 31;
                    default:
                        return 0;
                }
            }

            /// <summary>
            /// 检查指定年份是否为闰年
            /// </summary>
            /// <param name="year">年份</param>
            /// <returns>若该年为闰年则返回true，否则返回false</returns>
            public static bool IsLeapYear(int year)
            {
                // 形式参数为年份
                // 例如：2023
                return year % 400 == 0 || year % 4 == 0 && year % 100 != 0;
            }

            /// <summary>
            /// 获取第二天的午夜零时的时间
            /// </summary>
            /// <returns>返回UTC午夜零时时间</returns>
            public static SystemDateTime GetTomorrowMidnightDateTime()
            {
                SystemDateTime todayDate = SystemDateTime.UtcNow.Date;
                return todayDate.AddDays(1);
            }

            /// <summary>
            /// 获取明日的午夜零时偏移量
            /// <para>转换时间戳可使用方法：<see cref="DateTimeOffset.ToUnixTimeSeconds"/> </para>
            /// </summary>
            /// <returns>返回日期偏移量</returns>
            public static SystemDateTimeOffset GetTomorrowMidnightDateTimeOffset()
            {
                SystemDateTime tomorrowDateTime = GetTomorrowMidnightDateTime();
                return new SystemDateTimeOffset(tomorrowDateTime);
            }

            /// <summary>
            /// 获取指定时间的秒级别时间戳
            /// </summary>
            /// <param name="dateTime">指定日期</param>
            /// <returns>返回秒级别时间戳</returns>
            public static long GetSecondTimestamp(SystemDateTime dateTime)
            {
                return (dateTime.Ticks - EpochTicks) / SecDivisor;
            }

            /// <summary>
            /// 获取指定时间的毫秒级别时间戳
            /// </summary>
            /// <param name="dateTime">指定日期</param>
            /// <returns>返回毫秒级别时间戳</returns>
            public static long GetMillisecondTimestamp(SystemDateTime dateTime)
            {
                return (dateTime.Ticks - EpochTicks) / MsecDivisor;
            }

            /// <summary>
            /// 获取指定时间的微秒级别时间戳
            /// </summary>
            /// <param name="dateTime">指定日期</param>
            /// <returns>返回微秒级别时间戳</returns>
            public static long GetMicrosecondTimestamp(SystemDateTime dateTime)
            {
                return (dateTime.Ticks - EpochTicks);
            }
        }
    }
}
