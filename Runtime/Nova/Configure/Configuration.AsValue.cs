/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
/// Copyring (C) 2024, Guangzhou Shiyue Network Technology Co., Ltd.
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

namespace NovaEngine
{
    /// <summary>
    /// 基础配置参数定义类，对当前引擎运行所需的初始化参数进行设置及管理
    /// </summary>
    public static partial class Configuration
    {
        /// <summary>
        /// 检测当前容器中是否存在指定键对应的属性值
        /// </summary>
        /// <param name="key">属性键</param>
        /// <returns>若存在对应属性值则返回true，否则返回false</returns>
        public static bool HasProperty(string key)
        {
            if (s_variables.ContainsKey(key))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 通过指定键获取布尔类型的属性值
        /// </summary>
        /// <param name="key">属性键</param>
        /// <returns>返回给定键对应的布尔值，若不存在则返回false</returns>
        public static bool GetPropertyAsBool(string key)
        {
            if (false == s_variables.TryGetValue(key, out string value))
            {
                return false;
            }

            return Utility.Convertion.StringToBool(value);
        }

        /// <summary>
        /// 通过指定键获取整数类型的属性值
        /// </summary>
        /// <param name="key">属性键</param>
        /// <returns>返回给定键对应的整数值，若不存在则返回0</returns>
        public static int GetPropertyAsInt(string key)
        {
            if (false == s_variables.TryGetValue(key, out string value))
            {
                return 0;
            }

            return Utility.Convertion.StringToInt(value);
        }
    }
}
