/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
/// Copyright (C) 2023, Guangzhou Shiyue Network Technology Co., Ltd.
/// Copyright (C) 2025, Hurley, Independent Studio.
/// Copyright (C) 2025, Hainan Yuanyou Information Technology Co., Ltd. Guangzhou Branch
/// Copyright (C) 2026, Hurley, Independent Studio.
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
using System.Collections.Generic;
using System.Customize.Extension;
using System.Diagnostics;
using System.Runtime.CompilerServices;

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
        [Conditional("UNITY_ASSERTIONS")]
        public static void IsTrue(bool condition)
        {
            if (Environment.IsDevelopmentState())
            {
                UnityAssert.IsTrue(condition);
            }
        }

        /// <summary>
        /// 检测指定条件是否为TRUE的断言函数
        /// </summary>
        /// <param name="condition">断言条件</param>
        /// <param name="message">消息内容</param>
        [Conditional("UNITY_ASSERTIONS")]
        public static void IsTrue(bool condition, string message)
        {
            if (Environment.IsDevelopmentState())
            {
                UnityAssert.IsTrue(condition, message);
            }
        }

        /// <summary>
        /// 检测指定条件是否为TRUE的断言函数
        /// </summary>
        /// <param name="condition">断言条件</param>
        /// <param name="format">消息格式</param>
        /// <param name="args">消息参数列表</param>
        [Conditional("UNITY_ASSERTIONS")]
        public static void IsTrue(bool condition, string format, params object[] args)
        {
            if (Environment.IsDevelopmentState())
            {
                UnityAssert.IsTrue(condition, Utility.Text.Format(format, args));
            }
        }

        /// <summary>
        /// 检测指定条件是否为FALSE的断言函数
        /// </summary>
        /// <param name="condition">断言条件</param>
        [Conditional("UNITY_ASSERTIONS")]
        public static void IsFalse(bool condition)
        {
            if (Environment.IsDevelopmentState())
            {
                UnityAssert.IsFalse(condition);
            }
        }

        /// <summary>
        /// 检测指定条件是否为FALSE的断言函数
        /// </summary>
        /// <param name="condition">断言条件</param>
        /// <param name="message">消息内容</param>
        [Conditional("UNITY_ASSERTIONS")]
        public static void IsFalse(bool condition, string message)
        {
            if (Environment.IsDevelopmentState())
            {
                UnityAssert.IsFalse(condition, message);
            }
        }

        /// <summary>
        /// 检测指定条件是否为FALSE的断言函数
        /// </summary>
        /// <param name="condition">断言条件</param>
        /// <param name="format">消息格式</param>
        /// <param name="args">消息参数列表</param>
        [Conditional("UNITY_ASSERTIONS")]
        public static void IsFalse(bool condition, string format, params object[] args)
        {
            if (Environment.IsDevelopmentState())
            {
                UnityAssert.IsFalse(condition, Utility.Text.Format(format, args));
            }
        }

        /// <summary>
        /// 检测指定对象是否为NULL的断言函数
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="value">对象实例</param>
        [Conditional("UNITY_ASSERTIONS")]
        public static void IsNull<T>(T value) where T : class
        {
            UnityAssert.IsNull<T>(value);
        }

        /// <summary>
        /// 检测指定对象是否为NULL的断言函数
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="value">对象实例</param>
        /// <param name="message">消息内容</param>
        [Conditional("UNITY_ASSERTIONS")]
        public static void IsNull<T>(T value, string message) where T : class
        {
            UnityAssert.IsNull<T>(value, message);
        }

        /// <summary>
        /// 检测指定对象是否为NULL的断言函数
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="value">对象实例</param>
        /// <param name="format">消息格式</param>
        /// <param name="args">消息参数列表</param>
        [Conditional("UNITY_ASSERTIONS")]
        public static void IsNull<T>(T value, string format, params object[] args) where T : class
        {
            UnityAssert.IsNull<T>(value, Utility.Text.Format(format, args));
        }

        /// <summary>
        /// 检测指定对象是否为非NULL的断言函数
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="value">对象实例</param>
        [Conditional("UNITY_ASSERTIONS")]
        public static void IsNotNull<T>(T value) where T : class
        {
            UnityAssert.IsNotNull<T>(value);
        }

        /// <summary>
        /// 检测指定对象是否为非NULL的断言函数
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="value">对象实例</param>
        /// <param name="message">消息内容</param>
        [Conditional("UNITY_ASSERTIONS")]
        public static void IsNotNull<T>(T value, string message) where T : class
        {
            UnityAssert.IsNotNull<T>(value, message);
        }

        /// <summary>
        /// 检测指定对象是否为非NULL的断言函数
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="value">对象实例</param>
        /// <param name="format">消息格式</param>
        /// <param name="args">消息参数列表</param>
        [Conditional("UNITY_ASSERTIONS")]
        public static void IsNotNull<T>(T value, string format, params object[] args) where T : class
        {
            UnityAssert.IsNotNull<T>(value, Utility.Text.Format(format, args));
        }

        #region 基于类型匹配检测封装的断言函数

        /// <summary>
        /// 检测指定对象是否继承自目标类型的断言函数
        /// </summary>
        /// <typeparam name="T">父对象类型</typeparam>
        /// <param name="obj">对象实例</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsTypeOf<T>(object obj) where T : class
        {
            IsTypeOf<T>(obj.GetType());
        }

        /// <summary>
        /// 检测指定对象是否继承自目标类型的断言函数
        /// </summary>
        /// <typeparam name="T">父对象类型</typeparam>
        /// <param name="obj">对象实例</param>
        /// <param name="message">消息内容</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsTypeOf<T>(object obj, string message) where T : class
        {
            IsTypeOf<T>(obj.GetType(), message);
        }

        /// <summary>
        /// 检测指定对象是否继承自目标类型的断言函数
        /// </summary>
        /// <typeparam name="T">父对象类型</typeparam>
        /// <param name="obj">对象实例</param>
        /// <param name="format">消息格式</param>
        /// <param name="args">消息参数列表</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsTypeOf<T>(object obj, string format, params object[] args) where T : class
        {
            IsTypeOf<T>(obj.GetType(), format, args);
        }

        /// <summary>
        /// 检测指定对象类型是否继承自目标类型的断言函数
        /// </summary>
        /// <typeparam name="T">父对象类型</typeparam>
        /// <param name="type">对象类型</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsTypeOf<T>(Type type) where T : class
        {
            IsTrue(typeof(T).IsAssignableFrom(type));
        }

        /// <summary>
        /// 检测指定对象类型是否继承自目标类型的断言函数
        /// </summary>
        /// <typeparam name="T">父对象类型</typeparam>
        /// <param name="type">对象类型</param>
        /// <param name="message">消息内容</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsTypeOf<T>(Type type, string message) where T : class
        {
            IsTrue(typeof(T).IsAssignableFrom(type), message);
        }

        /// <summary>
        /// 检测指定对象类型是否继承自目标类型的断言函数
        /// </summary>
        /// <typeparam name="T">父对象类型</typeparam>
        /// <param name="type">对象类型</param>
        /// <param name="format">消息格式</param>
        /// <param name="args">消息参数列表</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsTypeOf<T>(Type type, string format, params object[] args) where T : class
        {
            IsTrue(typeof(T).IsAssignableFrom(type), format, args);
        }

        #endregion

        #region 基于空字符串检测封装的断言函数

        /// <summary>
        /// 检测指定字符串对象是否为空的断言函数
        /// </summary>
        /// <param name="str">字符串对象</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsNullOrEmpty(string str)
        {
            IsTrue(str.IsNullOrEmpty());
        }

        /// <summary>
        /// 检测指定字符串对象是否为空的断言函数
        /// </summary>
        /// <param name="str">字符串对象</param>
        /// <param name="message">消息内容</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsNullOrEmpty(string str, string message)
        {
            IsTrue(str.IsNullOrEmpty(), message);
        }

        /// <summary>
        /// 检测指定字符串对象是否为空的断言函数
        /// </summary>
        /// <param name="str">字符串对象</param>
        /// <param name="format">消息格式</param>
        /// <param name="args">消息参数列表</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsNullOrEmpty(string str, string format, params object[] args)
        {
            IsTrue(str.IsNullOrEmpty(), format, args);
        }

        /// <summary>
        /// 检测指定字符串对象是否为非空的断言函数
        /// </summary>
        /// <param name="str">字符串对象</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsNotNullOrEmpty(string str)
        {
            IsTrue(str.IsNotNullOrEmpty());
        }

        /// <summary>
        /// 检测指定字符串对象是否为非空的断言函数
        /// </summary>
        /// <param name="str">字符串对象</param>
        /// <param name="message">消息内容</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsNotNullOrEmpty(string str, string message)
        {
            IsTrue(str.IsNotNullOrEmpty(), message);
        }

        /// <summary>
        /// 检测指定字符串对象是否为非空的断言函数
        /// </summary>
        /// <param name="str">字符串对象</param>
        /// <param name="format">消息格式</param>
        /// <param name="args">消息参数列表</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsNotNullOrEmpty(string str, string format, params object[] args)
        {
            IsTrue(str.IsNotNullOrEmpty(), format, args);
        }

        #endregion

        #region 基于空容器检测封装的断言函数

        /// <summary>
        /// 检测指定容器对象是否为空的断言函数
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="collection">容器对象</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsNullOrEmpty<T>(ICollection<T> collection)
        {
            IsTrue(collection.IsNull() || 0 == collection.Count);
        }

        /// <summary>
        /// 检测指定容器对象是否为空的断言函数
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="collection">容器对象</param>
        /// <param name="message">消息内容</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsNullOrEmpty<T>(ICollection<T> collection, string message)
        {
            IsTrue(collection.IsNull() || 0 == collection.Count, message);
        }

        /// <summary>
        /// 检测指定字符串对象是否为空的断言函数
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="collection">容器对象</param>
        /// <param name="format">消息格式</param>
        /// <param name="args">消息参数列表</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsNullOrEmpty<T>(ICollection<T> collection, string format, params object[] args)
        {
            IsTrue(collection.IsNull() || 0 == collection.Count, format, args);
        }

        /// <summary>
        /// 检测指定容器对象是否为空的断言函数
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="collection">容器对象</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsNullOrEmpty<T>(IReadOnlyCollection<T> collection)
        {
            IsTrue(collection.IsNull() || 0 == collection.Count);
        }

        /// <summary>
        /// 检测指定容器对象是否为空的断言函数
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="collection">容器对象</param>
        /// <param name="message">消息内容</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsNullOrEmpty<T>(IReadOnlyCollection<T> collection, string message)
        {
            IsTrue(collection.IsNull() || 0 == collection.Count, message);
        }

        /// <summary>
        /// 检测指定字符串对象是否为空的断言函数
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="collection">容器对象</param>
        /// <param name="format">消息格式</param>
        /// <param name="args">消息参数列表</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsNullOrEmpty<T>(IReadOnlyCollection<T> collection, string format, params object[] args)
        {
            IsTrue(collection.IsNull() || 0 == collection.Count, format, args);
        }

        /// <summary>
        /// 检测指定容器对象是否为非空的断言函数
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="collection">容器对象</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsNotNullOrEmpty<T>(ICollection<T> collection)
        {
            IsTrue(collection.IsNotNull() && collection.Count > 0);
        }

        /// <summary>
        /// 检测指定容器对象是否为非空的断言函数
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="collection">容器对象</param>
        /// <param name="message">消息内容</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsNotNullOrEmpty<T>(ICollection<T> collection, string message)
        {
            IsTrue(collection.IsNotNull() && collection.Count > 0, message);
        }

        /// <summary>
        /// 检测指定字符串对象是否为非空的断言函数
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="collection">容器对象</param>
        /// <param name="format">消息格式</param>
        /// <param name="args">消息参数列表</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsNotNullOrEmpty<T>(ICollection<T> collection, string format, params object[] args)
        {
            IsTrue(collection.IsNotNull() && collection.Count > 0, format, args);
        }

        /// <summary>
        /// 检测指定容器对象是否为非空的断言函数
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="collection">容器对象</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsNotNullOrEmpty<T>(IReadOnlyCollection<T> collection)
        {
            IsTrue(collection.IsNotNull() && collection.Count > 0);
        }

        /// <summary>
        /// 检测指定容器对象是否为非空的断言函数
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="collection">容器对象</param>
        /// <param name="message">消息内容</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsNotNullOrEmpty<T>(IReadOnlyCollection<T> collection, string message)
        {
            IsTrue(collection.IsNotNull() && collection.Count > 0, message);
        }

        /// <summary>
        /// 检测指定字符串对象是否为非空的断言函数
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="collection">容器对象</param>
        /// <param name="format">消息格式</param>
        /// <param name="args">消息参数列表</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsNotNullOrEmpty<T>(IReadOnlyCollection<T> collection, string format, params object[] args)
        {
            IsTrue(collection.IsNotNull() && collection.Count > 0, format, args);
        }

        #endregion
    }
}
