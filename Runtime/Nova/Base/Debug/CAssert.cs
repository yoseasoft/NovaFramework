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

using UnityAssert = UnityEngine.Assertions.Assert;

namespace NovaEngine
{
    /// <summary>
    /// 断言对象工具类，用于引擎内部用例测试时的调试断言提供的操作函数
    /// </summary>
    public static class CAssert
    {
        /// <summary>
        /// 检测指定条件是否为TRUE的断言函数
        /// </summary>
        /// <param name="condition">断言条件</param>
        public static void IsTrue(bool condition)
        {
#if UNITY_EDITOR
            UnityAssert.IsTrue(condition);
#endif
        }

        /// <summary>
        /// 检测指定条件是否为TRUE的断言函数
        /// </summary>
        /// <param name="condition">断言条件</param>
        /// <param name="message">消息内容</param>
        public static void IsTrue(bool condition, string message)
        {
#if UNITY_EDITOR
            UnityAssert.IsTrue(condition, message);
#endif
        }

        /// <summary>
        /// 检测指定条件是否为TRUE的断言函数
        /// </summary>
        /// <param name="condition">断言条件</param>
        /// <param name="format">消息格式</param>
        /// <param name="args">消息参数列表</param>
        public static void IsTrue(bool condition, string format, params object[] args)
        {
#if UNITY_EDITOR
            UnityAssert.IsTrue(condition, Utility.Text.Format(format, args));
#endif
        }

        /// <summary>
        /// 检测指定条件是否为FALSE的断言函数
        /// </summary>
        /// <param name="condition">断言条件</param>
        public static void IsFalse(bool condition)
        {
#if UNITY_EDITOR
            UnityAssert.IsFalse(condition);
#endif
        }

        /// <summary>
        /// 检测指定条件是否为FALSE的断言函数
        /// </summary>
        /// <param name="condition">断言条件</param>
        /// <param name="message">消息内容</param>
        public static void IsFalse(bool condition, string message)
        {
#if UNITY_EDITOR
            UnityAssert.IsFalse(condition, message);
#endif
        }

        /// <summary>
        /// 检测指定条件是否为FALSE的断言函数
        /// </summary>
        /// <param name="condition">断言条件</param>
        /// <param name="format">消息格式</param>
        /// <param name="args">消息参数列表</param>
        public static void IsFalse(bool condition, string format, params object[] args)
        {
#if UNITY_EDITOR
            UnityAssert.IsFalse(condition, Utility.Text.Format(format, args));
#endif
        }

        /// <summary>
        /// 检测指定类对象是否为NULL的断言函数
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="value">对象实例</param>
        public static void IsNull<T>(T value) where T : class
        {
            UnityAssert.IsNull<T>(value);
        }

        /// <summary>
        /// 检测指定类对象是否为NULL的断言函数
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="value">对象实例</param>
        /// <param name="message">消息内容</param>
        public static void IsNull<T>(T value, string message) where T : class
        {
            UnityAssert.IsNull<T>(value, message);
        }

        /// <summary>
        /// 检测指定类对象是否为NULL的断言函数
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="value">对象实例</param>
        /// <param name="format">消息格式</param>
        /// <param name="args">消息参数列表</param>
        public static void IsNull<T>(T value, string format, params object[] args) where T : class
        {
            UnityAssert.IsNull<T>(value, Utility.Text.Format(format, args));
        }

        /// <summary>
        /// 检测指定类对象是否为非NULL的断言函数
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="value">对象实例</param>
        public static void IsNotNull<T>(T value) where T : class
        {
            UnityAssert.IsNotNull<T>(value);
        }

        /// <summary>
        /// 检测指定类对象是否为非NULL的断言函数
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="value">对象实例</param>
        /// <param name="message">消息内容</param>
        public static void IsNotNull<T>(T value, string message) where T : class
        {
            UnityAssert.IsNotNull<T>(value, message);
        }

        /// <summary>
        /// 检测指定类对象是否为非NULL的断言函数
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="value">对象实例</param>
        /// <param name="format">消息格式</param>
        /// <param name="args">消息参数列表</param>
        public static void IsNotNull<T>(T value, string format, params object[] args) where T : class
        {
            UnityAssert.IsNotNull<T>(value, Utility.Text.Format(format, args));
        }
    }
}
