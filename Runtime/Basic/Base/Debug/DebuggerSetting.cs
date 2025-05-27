/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
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

using System.Collections.Generic;

namespace GameEngine.Debug
{
    /// <summary>
    /// 游戏调试器组件的设置对象类，用于定义调试器模块相关配置的设置及存储接口函数
    /// </summary>
    public sealed class DebuggerSetting : IDebuggerSetting
    {
        private readonly IDictionary<string, string> m_settings = new Dictionary<string, string>();

        /// <summary>
        /// 获取游戏配置项的数量
        /// </summary>
        public int Count
        {
            get { return m_settings.Count; }
        }

        /// <summary>
        /// 获取所有配置项的名称
        /// </summary>
        /// <returns>返回所有配置项的名称</returns>
        public string[] GetAllKeys()
        {
            int index = 0;
            string[] keys = new string[m_settings.Count];
            foreach (KeyValuePair<string, string> setting in m_settings)
            {
                keys[index++] = setting.Key;
            }

            return keys;
        }

        /// <summary>
        /// 获取所有配置项的名称，并填充到指定的链表容器中
        /// </summary>
        /// <param name="results">配置名称的集合</param>
        public void GetAllKeys(IList<string> results)
        {
            if (null == results)
            {
                throw new NovaEngine.CFrameworkException("Results is invalid.");
            }

            results.Clear();
            foreach (KeyValuePair<string, string> setting in m_settings)
            {
                results.Add(setting.Key);
            }
        }

        /// <summary>
        /// 检查是否存在指定名称的配置项
        /// </summary>
        /// <param name="key">配置名称</param>
        /// <returns>若存在给定名称的配置项返回true，否则返回false</returns>
        public bool HasKey(string key)
        {
            return m_settings.ContainsKey(key);
        }

        /// <summary>
        /// 从当前的配置集合中移除指定名称对应的配置项
        /// </summary>
        /// <param name="key">配置名称</param>
        public bool RemoveKey(string key)
        {
            return m_settings.Remove(key);
        }

        /// <summary>
        /// 清空当前配置集合中的所有配置项
        /// </summary>
        public void RemoveAllKeys()
        {
            m_settings.Clear();
        }

        /// <summary>
        /// 从配置列表中查找指定名称对应的配置项，并以布尔值的方式进行读取
        /// </summary>
        /// <param name="key">配置名称</param>
        /// <returns>返回读取的布尔值</returns>
        public bool GetBool(string key)
        {
            string value = null;
            if (false == m_settings.TryGetValue(key, out value))
            {
                Debugger.Warn("Setting '{0}' is not exist.", key);
                return false;
            }

            return (int.Parse(value) != 0);
        }

        /// <summary>
        /// 从配置列表中查找指定名称对应的配置项，并以布尔值的方式进行读取
        /// 如果读取配置失败，该函数支持返回默认值
        /// </summary>
        /// <param name="key">配置名称</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>返回读取的布尔值</returns>
        public bool GetBool(string key, bool defaultValue)
        {
            string value = null;
            if (false == m_settings.TryGetValue(key, out value))
            {
                return defaultValue;
            }

            return (int.Parse(value) != 0);
        }

        /// <summary>
        /// 将指定的配置项写入到当前配置列表中
        /// </summary>
        /// <param name="key">配置名称</param>
        /// <param name="value">配置值</param>
        public void SetBool(string key, bool value)
        {
            m_settings[key] = value ? "1" : "0";
        }

        /// <summary>
        /// 从配置列表中查找指定名称对应的配置项，并以整型值的方式进行读取
        /// </summary>
        /// <param name="key">配置名称</param>
        /// <returns>返回读取的整型值</returns>
        public int GetInt(string key)
        {
            string value = null;
            if (false == m_settings.TryGetValue(key, out value))
            {
                Debugger.Warn("Setting '{0}' is not exist.", key);
                return 0;
            }

            return int.Parse(value);
        }

        /// <summary>
        /// 从配置列表中查找指定名称对应的配置项，并以整型值的方式进行读取
        /// 如果读取配置失败，该函数支持返回默认值
        /// </summary>
        /// <param name="key">配置名称</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>返回读取的整型值</returns>
        public int GetInt(string key, int defaultValue)
        {
            string value = null;
            if (false == m_settings.TryGetValue(key, out value))
            {
                return defaultValue;
            }

            return int.Parse(value);
        }

        /// <summary>
        /// 将指定的配置项写入到当前配置列表中
        /// </summary>
        /// <param name="key">配置名称</param>
        /// <param name="value">配置值</param>
        public void SetInt(string key, int value)
        {
            m_settings[key] = value.ToString();
        }

        /// <summary>
        /// 从配置列表中查找指定名称对应的配置项，并以浮点值的方式进行读取
        /// </summary>
        /// <param name="key">配置名称</param>
        /// <returns>返回读取的浮点值</returns>
        public float GetFloat(string key)
        {
            string value = null;
            if (false == m_settings.TryGetValue(key, out value))
            {
                Debugger.Warn("Setting '{0}' is not exist.", key);
                return 0f;
            }

            return float.Parse(value);
        }

        /// <summary>
        /// 从配置列表中查找指定名称对应的配置项，并以浮点值的方式进行读取
        /// 如果读取配置失败，该函数支持返回默认值
        /// </summary>
        /// <param name="key">配置名称</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>返回读取的浮点值</returns>
        public float GetFloat(string key, float defaultValue)
        {
            string value = null;
            if (false == m_settings.TryGetValue(key, out value))
            {
                return defaultValue;
            }

            return float.Parse(value);
        }

        /// <summary>
        /// 将指定的配置项写入到当前配置列表中
        /// </summary>
        /// <param name="key">配置名称</param>
        /// <param name="value">配置值</param>
        public void SetFloat(string key, float value)
        {
            m_settings[key] = value.ToString();
        }

        /// <summary>
        /// 从配置列表中查找指定名称对应的配置项，并以字符串值的方式进行读取
        /// </summary>
        /// <param name="key">配置名称</param>
        /// <returns>返回读取的字符串值</returns>
        public string GetString(string key)
        {
            string value = null;
            if (false == m_settings.TryGetValue(key, out value))
            {
                Debugger.Warn("Setting '{0}' is not exist.", key);
                return null;
            }

            return value;
        }

        /// <summary>
        /// 从配置列表中查找指定名称对应的配置项，并以字符串值的方式进行读取
        /// 如果读取配置失败，该函数支持返回默认值
        /// </summary>
        /// <param name="key">配置名称</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>返回读取的字符串值</returns>
        public string GetString(string key, string defaultValue)
        {
            string value = null;
            if (false == m_settings.TryGetValue(key, out value))
            {
                return defaultValue;
            }

            return value;
        }

        /// <summary>
        /// 将指定的配置项写入到当前配置列表中
        /// </summary>
        /// <param name="key">配置名称</param>
        /// <param name="value">配置值</param>
        public void SetString(string key, string value)
        {
            m_settings[key] = value;
        }
    }
}
