/// -------------------------------------------------------------------------------
/// NovaEngine Editor Framework
///
/// Copyright (C) 2025, Hurley, Independent Studio.
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

using UnityEditor;

namespace NoveEngine.Editor
{
    /// <summary>
    /// 用户设置参数持久化管理类，通常可以采用“EditorPrefs”和“EditorUserSettings”两种形式实现
    ///
    /// EditorPrefs
    /// ‌适用范围广‌：适用于存储环境设定、位置、大小等与Unity编辑器相关的信息
    /// ‌存储方式‌：存储在Windows注册表中，以大版本划分存储，如4.x版本和5.x版本
    /// ‌查看方式‌：可以通过Windows注册表编辑器查看，路径为HKEY_CURRENT_USER\Software\Unity Technologies\UnityEditor 5.x‌
    ///
    /// EditorUserSettings
    /// ‌‌数据加密‌：保存的数据都会被加密，适合存储个人信息或密码等敏感信息
    /// ‌适用范围小‌：仅影响当前项目，不会影响到其他项目或编辑器设置‌
    /// ‌存储方式‌：以加密形式保存在项目内的文件中，如Unity2021中保存在项目的UserSettings目录下的EditorUserSettings.asset‌
    /// </summary>
    public static class UserSettings
    {
        /// <summary>
        /// 设置一个字符串值的记录信息
        /// </summary>
        /// <param name="key">记录键</param>
        /// <param name="value">记录值</param>
        public static void SetString(string key, string value)
        {
            //EditorPrefs.SetString(key, value);
            EditorUserSettings.SetConfigValue(key, value);
        }

        /// <summary>
        /// 获取一个字符串值的记录信息
        /// </summary>
        /// <param name="key">记录键</param>
        /// <returns>若存在指定键对应的记录信息，则返回其值</returns>
        public static string GetString(string key)
        {
            //return EditorPrefs.GetString(key);
            return EditorUserSettings.GetConfigValue(key);
        }

        /// <summary>
        /// 设置一个布尔值的记录信息
        /// </summary>
        /// <param name="key">记录键</param>
        /// <param name="value">记录值</param>
        public static void SetBool(string key, bool value)
        {
            SetString(key, value ? "true" : "false");
        }

        /// <summary>
        /// 获取一个布尔值的记录信息
        /// </summary>
        /// <param name="key">记录键</param>
        /// <returns>若存在指定键对应的记录信息，则返回其值</returns>
        public static bool GetBool(string key)
        {
            string value = GetString(key);

            if (null == value) return false;
            return bool.Parse(value);
        }

        /// <summary>
        /// 设置一个整型值的记录信息
        /// </summary>
        /// <param name="key">记录键</param>
        /// <param name="value">记录值</param>
        public static void SetInt(string key, int value)
        {
            SetString(key, System.Convert.ToString(value));
        }

        /// <summary>
        /// 获取一个整型值的记录信息
        /// </summary>
        /// <param name="key">记录键</param>
        /// <returns>若存在指定键对应的记录信息，则返回其值</returns>
        public static int GetInt(string key)
        {
            string value = GetString(key);

            if (null == value) return 0;
            return int.Parse(value);
        }

        /// <summary>
        /// 设置一个浮点型值的记录信息
        /// </summary>
        /// <param name="key">记录键</param>
        /// <param name="value">记录值</param>
        public static void SetFloat(string key, float value)
        {
            SetString(key, System.Convert.ToString(value));
        }

        /// <summary>
        /// 获取一个浮点型值的记录信息
        /// </summary>
        /// <param name="key">记录键</param>
        /// <returns>若存在指定键对应的记录信息，则返回其值</returns>
        public static float GetFloat(string key)
        {
            string value = GetString(key);

            if (null == value) return 0f;
            return float.Parse(value);
        }

        /// <summary>
        /// 设置一个日期型值的记录信息
        /// </summary>
        /// <param name="key">记录键</param>
        /// <param name="value">记录值</param>
        public static void SetDateTime(string key, System.DateTime dateTime)
        {
            SetString(key, System.Convert.ToString(dateTime));
        }

        /// <summary>
        /// 获取一个日期型值的记录信息
        /// </summary>
        /// <param name="key">记录键</param>
        /// <returns>若存在指定键对应的记录信息，则返回其值</returns>
        public static System.DateTime GetDateTime(string key)
        {
            string value = GetString(key);

            if (null == value) return System.DateTime.MinValue;
            return System.DateTime.Parse(value);
        }
    }
}
