/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyright (C) 2023, Guangzhou Shiyue Network Technology Co., Ltd.
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

namespace GameEngine.Profiler.Debugging
{
    /// <summary>
    /// 游戏调试器组件的设置对象的抽象接口类，对调试器模块相关配置的设置，存储等操作定义统一的访问调度接口函数
    /// </summary>
    internal interface IDebuggerSetting
    {
        public const string ConsoleLockScroll   = "Debugger.Console.LockScroll";
        public const string ConsoleInfoFilter   = "Debugger.Console.InfoFilter";
        public const string ConsoleWarnFilter   = "Debugger.Console.WarnFilter";
        public const string ConsoleErrorFilter  = "Debugger.Console.ErrorFilter";
        public const string ConsoleFatalFilter  = "Debugger.Console.FatalFilter";

        public const string IconX               = "Debugger.Icon.X";
        public const string IconY               = "Debugger.Icon.Y";
        public const string WindowX             = "Debugger.Window.X";
        public const string WindowY             = "Debugger.Window.Y";
        public const string WindowWidth         = "Debugger.Window.Width";
        public const string WindowHeight        = "Debugger.Window.Height";
        public const string WindowScale         = "Debugger.Window.Scale";

        /// <summary>
        /// 获取游戏配置项的数量
        /// </summary>
        int Count { get; }

        /// <summary>
        /// 获取所有配置项的名称
        /// </summary>
        /// <returns>返回所有配置项的名称</returns>
        string[] GetAllKeys();

        /// <summary>
        /// 获取所有配置项的名称，并填充到指定的链表容器中
        /// </summary>
        /// <param name="results">配置名称的集合</param>
        void GetAllKeys(IList<string> results);

        /// <summary>
        /// 检查是否存在指定名称的配置项
        /// </summary>
        /// <param name="key">配置名称</param>
        /// <returns>若存在给定名称的配置项返回true，否则返回false</returns>
        bool HasKey(string key);

        /// <summary>
        /// 从当前的配置集合中移除指定名称对应的配置项
        /// </summary>
        /// <param name="key">配置名称</param>
        bool RemoveKey(string key);

        /// <summary>
        /// 清空当前配置集合中的所有配置项
        /// </summary>
        void RemoveAllKeys();

        /// <summary>
        /// 从配置列表中查找指定名称对应的配置项，并以布尔值的方式进行读取
        /// </summary>
        /// <param name="key">配置名称</param>
        /// <returns>返回读取的布尔值</returns>
        bool GetBool(string key);

        /// <summary>
        /// 从配置列表中查找指定名称对应的配置项，并以布尔值的方式进行读取
        /// 如果读取配置失败，该函数支持返回默认值
        /// </summary>
        /// <param name="key">配置名称</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>返回读取的布尔值</returns>
        bool GetBool(string key, bool defaultValue);

        /// <summary>
        /// 将指定的配置项写入到当前配置列表中
        /// </summary>
        /// <param name="key">配置名称</param>
        /// <param name="value">配置值</param>
        void SetBool(string key, bool value);

        /// <summary>
        /// 从配置列表中查找指定名称对应的配置项，并以整型值的方式进行读取
        /// </summary>
        /// <param name="key">配置名称</param>
        /// <returns>返回读取的整型值</returns>
        int GetInt(string key);

        /// <summary>
        /// 从配置列表中查找指定名称对应的配置项，并以整型值的方式进行读取
        /// 如果读取配置失败，该函数支持返回默认值
        /// </summary>
        /// <param name="key">配置名称</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>返回读取的整型值</returns>
        int GetInt(string key, int defaultValue);

        /// <summary>
        /// 将指定的配置项写入到当前配置列表中
        /// </summary>
        /// <param name="key">配置名称</param>
        /// <param name="value">配置值</param>
        void SetInt(string key, int value);

        /// <summary>
        /// 从配置列表中查找指定名称对应的配置项，并以浮点值的方式进行读取
        /// </summary>
        /// <param name="key">配置名称</param>
        /// <returns>返回读取的浮点值</returns>
        float GetFloat(string key);

        /// <summary>
        /// 从配置列表中查找指定名称对应的配置项，并以浮点值的方式进行读取
        /// 如果读取配置失败，该函数支持返回默认值
        /// </summary>
        /// <param name="key">配置名称</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>返回读取的浮点值</returns>
        float GetFloat(string key, float defaultValue);

        /// <summary>
        /// 将指定的配置项写入到当前配置列表中
        /// </summary>
        /// <param name="key">配置名称</param>
        /// <param name="value">配置值</param>
        void SetFloat(string key, float value);

        /// <summary>
        /// 从配置列表中查找指定名称对应的配置项，并以字符串值的方式进行读取
        /// </summary>
        /// <param name="key">配置名称</param>
        /// <returns>返回读取的字符串值</returns>
        string GetString(string key);

        /// <summary>
        /// 从配置列表中查找指定名称对应的配置项，并以字符串值的方式进行读取
        /// 如果读取配置失败，该函数支持返回默认值
        /// </summary>
        /// <param name="key">配置名称</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>返回读取的字符串值</returns>
        string GetString(string key, string defaultValue);

        /// <summary>
        /// 将指定的配置项写入到当前配置列表中
        /// </summary>
        /// <param name="key">配置名称</param>
        /// <param name="value">配置值</param>
        void SetString(string key, string value);
    }
}
