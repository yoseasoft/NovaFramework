/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
/// Copyring (C) 2022 - 2023, Shanghai Bilibili Technology Co., Ltd.
/// Copyring (C) 2023, Guangzhou Shiyue Network Technology Co., Ltd.
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

using UnityPlayerPrefs = UnityEngine.PlayerPrefs;

namespace NovaEngine
{
    /// <summary>
    /// 实用函数集合工具类
    /// </summary>
    public static partial class Utility
    {
        /// <summary>
        /// 持久化相关实用函数集合
        /// 使用轻量级数据存储检索方式“PlayerPrefs”来实现，该方式只允许存储少量数据
        /// 您可以使用该方式对音量控制，亮度设置，字符选择等游戏设置类数据进行存储管理
        /// </summary>
        public static class Persistence
        {
            /// <summary>
            /// 获取指定标识对应的整型存储数据
            /// </summary>
            /// <param name="key">唯一标识</param>
            /// <returns>返回该标识对应的整型存储数据</returns>
            public static int GetInt(string key)
            {
                return UnityPlayerPrefs.GetInt(key);
            }

            /// <summary>
            /// 获取指定标识对应的整型存储数据，若不存在则返回给定的默认值
            /// </summary>
            /// <param name="key">唯一标识</param>
            /// <param name="defaultValue">默认值</param>
            /// <returns>返回该标识对应的整型存储数据</returns>
            public static int GetInt(string key, int defaultValue)
            {
                return UnityPlayerPrefs.GetInt(key, defaultValue);
            }

            /// <summary>
            /// 获取指定标识对应的浮点型存储数据
            /// </summary>
            /// <param name="key">唯一标识</param>
            /// <returns>返回该标识对应的浮点型存储数据</returns>
            public static float GetFloat(string key)
            {
                return UnityPlayerPrefs.GetFloat(key);
            }

            /// <summary>
            /// 获取指定标识对应的浮点型存储数据，若不存在则返回给定的默认值
            /// </summary>
            /// <param name="key">唯一标识</param>
            /// <param name="defaultValue">默认值</param>
            /// <returns>返回该标识对应的浮点型存储数据</returns>
            public static float GetFloat(string key, float defaultValue)
            {
                return UnityPlayerPrefs.GetFloat(key, defaultValue);
            }

            /// <summary>
            /// 获取指定标识对应的字符型存储数据
            /// </summary>
            /// <param name="key">唯一标识</param>
            /// <returns>返回该标识对应的字符型存储数据</returns>
            public static string GetString(string key)
            {
                return UnityPlayerPrefs.GetString(key);
            }

            /// <summary>
            /// 获取指定标识对应的字符型存储数据，若不存在则返回给定的默认值
            /// </summary>
            /// <param name="key">唯一标识</param>
            /// <param name="defaultValue">默认值</param>
            /// <returns>返回该标识对应的字符型存储数据</returns>
            public static string GetString(string key, string defaultValue)
            {
                return UnityPlayerPrefs.GetString(key, defaultValue);
            }

            /// <summary>
            /// 设置指定标识对应的整型存储数据
            /// </summary>
            /// <param name="key">唯一标识</param>
            /// <param name="value">整型数据</param>
            public static void SetInt(string key, int value)
            {
                UnityPlayerPrefs.SetInt(key, value);
            }

            /// <summary>
            /// 设置指定标识对应的浮点型存储数据
            /// </summary>
            /// <param name="key">唯一标识</param>
            /// <param name="value">浮点型数据</param>
            public static void SetFloat(string key, float value)
            {
                UnityPlayerPrefs.SetFloat(key, value);
            }

            /// <summary>
            /// 设置指定标识对应的字符型存储数据
            /// </summary>
            /// <param name="key">唯一标识</param>
            /// <param name="value">字符型数据</param>
            public static void SetString(string key, string value)
            {
                UnityPlayerPrefs.SetString(key, value);
            }

            /// <summary>
            /// 检测当前容器中是否存在指定标识对应的数据
            /// </summary>
            /// <param name="key">唯一标识</param>
            /// <returns>若该容器中存在对应标识的数据则返回true，否则返回false</returns>
            public static bool HasKey(string key)
            {
                return UnityPlayerPrefs.HasKey(key);
            }

            /// <summary>
            /// 将当前存储仓库中的全部记录进行持久化操作
            /// </summary>
            public static void Save()
            {
                UnityPlayerPrefs.Save();
            }

            /// <summary>
            /// 移除存储仓库中指定标识对应的记录
            /// </summary>
            /// <param name="key">数据记录标识</param>
            public static void DeleteKey(string key)
            {
                UnityPlayerPrefs.DeleteKey(key);
            }

            /// <summary>
            /// 移除存储仓库中的全部记录
            /// </summary>
            public static void DeleteAll()
            {
                UnityPlayerPrefs.DeleteAll();
            }
        }
    }
}
