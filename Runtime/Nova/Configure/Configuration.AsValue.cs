/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
/// Copyright (C) 2024, Guangzhou Shiyue Network Technology Co., Ltd.
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

using System;

namespace NovaEngine
{
    /// <summary>
    /// 基础配置参数定义类，对当前引擎运行所需的初始化参数进行设置及管理
    /// </summary>
    public static partial class Configuration
    {
        /// <summary>
        /// 设置布尔类型的配置变量
        /// </summary>
        /// <param name="key">配置键</param>
        /// <param name="value">变量值</param>
        public static void SetBool(string key, bool value)
        {
            SetValue(key, Convert.ToString(value));
        }

        /// <summary>
        /// 获取布尔类型的配置变量
        /// </summary>
        /// <param name="key">配置键</param>
        /// <returns>返回变量值</returns>
        public static bool GetBool(string key)
        {
            string value = GetValue(key);

            if (null == value) return false;
            return Utility.Convertion.StringToBool(value);
        }

        /// <summary>
        /// 设置整数类型的配置变量
        /// </summary>
        /// <param name="key">配置键</param>
        /// <param name="value">变量值</param>
        public static void SetInt(string key, int value)
        {
            SetValue(key, Convert.ToString(value));
        }

        /// <summary>
        /// 获取整数类型的配置变量
        /// </summary>
        /// <param name="key">配置键</param>
        /// <returns>返回变量值</returns>
        public static int GetInt(string key)
        {
            string value = GetValue(key);

            if (null == value) return 0;
            return Utility.Convertion.StringToInt(value);
        }

        /// <summary>
        /// 设置长整数类型的配置变量
        /// </summary>
        /// <param name="key">配置键</param>
        /// <param name="value">变量值</param>
        public static void SetLong(string key, long value)
        {
            SetValue(key, Convert.ToString(value));
        }

        /// <summary>
        /// 获取长整数类型的配置变量
        /// </summary>
        /// <param name="key">配置键</param>
        /// <returns>返回变量值</returns>
        public static long GetLong(string key)
        {
            string value = GetValue(key);

            if (null == value) return 0L;
            return Utility.Convertion.StringToLong(value);
        }

        /// <summary>
        /// 设置浮点数类型的配置变量
        /// </summary>
        /// <param name="key">配置键</param>
        /// <param name="value">变量值</param>
        public static void SetFloat(string key, float value)
        {
            SetValue(key, Convert.ToString(value));
        }

        /// <summary>
        /// 获取浮点数类型的配置变量
        /// </summary>
        /// <param name="key">配置键</param>
        /// <returns>返回变量值</returns>
        public static float GetFloat(string key)
        {
            string value = GetValue(key);

            if (null == value) return 0f;
            return Utility.Convertion.StringToFloat(value);
        }

        /// <summary>
        /// 设置日期类型的配置变量
        /// </summary>
        /// <param name="key">配置键</param>
        /// <param name="value">变量值</param>
        public static void SetDateTime(string key, DateTime dateTime)
        {
            SetValue(key, Convert.ToString(dateTime));
        }

        /// <summary>
        /// 获取日期类型的配置变量
        /// </summary>
        /// <param name="key">配置键</param>
        /// <returns>返回变量值</returns>
        public static DateTime GetDateTime(string key)
        {
            string value = GetValue(key);

            if (null == value) return DateTime.MinValue;
            return Utility.Convertion.StringToDateTime(value, DateTime.MinValue);
        }
    }
}
