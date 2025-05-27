/// -------------------------------------------------------------------------------
/// NovaEngine Framework
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

namespace NovaEngine
{
    /// <summary>
    /// 基础环境属性定义类，对当前引擎运行所需的环境成员属性进行设置及管理
    /// </summary>
    public static partial class Environment
    {
        /// <summary>
        /// 通过指定键名获取对应布尔类型的环境参数值
        /// </summary>
        /// <param name="key">环境参数键</param>
        /// <returns>若存在指定环境参数则返回对应值，否则返回false</returns>
        public static bool GetVariableAsBool(string key)
        {
            string v = GetVariable(key);

            return Utility.Convertion.StringToBool(v);
        }

        /// <summary>
        /// 通过指定键名获取对应整数类型的环境参数值
        /// </summary>
        /// <param name="key">环境参数键</param>
        /// <returns>若存在指定环境参数则返回对应值，否则返回0</returns>
        public static int GetVariableAsInt(string key)
        {
            string v = GetVariable(key);

            return Utility.Convertion.StringToInt(v);
        }

        /// <summary>
        /// 通过指定键名获取对应长整数类型的环境参数值
        /// </summary>
        /// <param name="key">环境参数键</param>
        /// <returns>若存在指定环境参数则返回对应值，否则返回0</returns>
        public static long GetVariableAsLong(string key)
        {
            string v = GetVariable(key);

            return Utility.Convertion.StringToLong(v);
        }

        /// <summary>
        /// 通过指定键名获取对应浮点数类型的环境参数值
        /// </summary>
        /// <param name="key">环境参数键</param>
        /// <returns>若存在指定环境参数则返回对应值，否则返回0.0f</returns>
        public static float GetVariableAsFloat(string key)
        {
            string v = GetVariable(key);

            return Utility.Convertion.StringToFloat(v);
        }

        /// <summary>
        /// 通过指定键名获取对应双精度浮点数类型的环境参数值
        /// </summary>
        /// <param name="key">环境参数键</param>
        /// <returns>若存在指定环境参数则返回对应值，否则返回0.0f</returns>
        public static double GetVariableAsDouble(string key)
        {
            string v = GetVariable(key);

            return Utility.Convertion.StringToDouble(v);
        }
    }
}
