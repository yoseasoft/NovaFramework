/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
/// Copyright (C) 2024 - 2025, Hurley, Independent Studio.
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

using SystemDateTime = System.DateTime;
using SystemDateTimeOffset = System.DateTimeOffset;
using SystemDayOfWeek = System.DayOfWeek;
using SystemGregorianCalendar = System.Globalization.GregorianCalendar;
using SystemCalendarWeekRule = System.Globalization.CalendarWeekRule;

namespace NovaEngine
{
    /// <summary>
    /// 为系统默认的日期时间类型提供扩展接口支持
    /// </summary>
    public static class __DateTime
    {
        /// <summary>
        /// 获取年度第几个星期，默认星期日是第一天
        /// </summary>
        /// <param name="self">日期时间对象实例</param>
        /// <returns>返回年度第几个星期</returns>
        public static int WeekOfYear(this in SystemDateTime self)
        {
            SystemGregorianCalendar gc = new SystemGregorianCalendar();
            return gc.GetWeekOfYear(self, SystemCalendarWeekRule.FirstDay, SystemDayOfWeek.Sunday);
        }

        /// <summary>
        /// 获取年度第几个星期
        /// </summary>
        /// <param name="self">日期时间对象实例</param>
        /// <param name="week">星期类型</param>
        /// <returns>返回年度第几个星期</returns>
        public static int WeekOfYear(this in SystemDateTime self, SystemDayOfWeek week)
        {
            SystemGregorianCalendar gc = new SystemGregorianCalendar();
            return gc.GetWeekOfYear(self, SystemCalendarWeekRule.FirstDay, week);
        }

        /// <summary>
        /// 获取相对于当前时间的相对天数后的时间格式字符串信息
        /// </summary>
        /// <param name="self">日期时间对象实例</param>
        /// <param name="relativeday">叠加天数</param>
        /// <returns>返回时间格式字符串</returns>
        // public static string GetDateTime(this in SystemDateTime self, int relativeday) { return self.AddDays(relativeday).ToString("yyyy-MM-dd HH:mm:ss"); }

        /// <summary>
        /// 获取标准时间格式字符串信息
        /// </summary>
        /// <param name="self">日期时间对象实例</param>
        /// <returns>返回时间格式字符串</returns>
        // public static string GetDateTimeF(this in SystemDateTime self) { return self.ToString("yyyy-MM-dd HH:mm:ss:fffffff"); }

        /// <summary>
        /// 获取该时间相对于纪元时间的分钟数
        /// </summary>
        /// <param name="self">日期时间对象实例</param>
        /// <returns>返回时间值</returns>
        public static double GetTotalMinutes(this in SystemDateTime self)
        {
            return new SystemDateTimeOffset(self).Offset.TotalMinutes;
        }

        /// <summary>
        /// 获取该时间相对于纪元时间的小时数
        /// </summary>
        /// <param name="self">日期时间对象实例</param>
        /// <returns>返回时间值</returns>
        public static double GetTotalHours(this in SystemDateTime self)
        {
            return new SystemDateTimeOffset(self).Offset.TotalHours;
        }

        /// <summary>
        /// 获取该时间相对于纪元时间的天数
        /// </summary>
        /// <param name="self">日期时间对象实例</param>
        /// <returns>返回时间值</returns>
        public static double GetTotalDays(this in SystemDateTime self)
        {
            return new SystemDateTimeOffset(self).Offset.TotalDays;
        }
    }
}
