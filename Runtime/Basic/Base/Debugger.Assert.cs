/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyright (C) 2025 - 2026, Hainan Yuanyou Information Technology Co., Ltd. Guangzhou Branch
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
using System.Runtime.CompilerServices;
using UnityEditor.VersionControl;

namespace GameEngine
{
    /// 应用层提供的调试对象类
    public static partial class Debugger
    {
        /// <summary>
        /// 检测指定条件是否为TRUE的断言函数
        /// </summary>
        /// <param name="condition">断言条件</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsTrue(bool condition)
        {
            NovaEngine.CAssert.IsTrue(condition);
        }

        /// <summary>
        /// 检测指定条件是否为TRUE的断言函数
        /// </summary>
        /// <param name="condition">断言条件</param>
        /// <param name="message">消息内容</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsTrue(bool condition, string message)
        {
            NovaEngine.CAssert.IsTrue(condition, message);
        }

        /// <summary>
        /// 检测指定条件是否为TRUE的断言函数
        /// </summary>
        /// <param name="condition">断言条件</param>
        /// <param name="format">消息格式</param>
        /// <param name="args">消息参数列表</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsTrue(bool condition, string format, params object[] args)
        {
            NovaEngine.CAssert.IsTrue(condition, format, args);
        }

        /// <summary>
        /// 检测指定条件是否为FALSE的断言函数
        /// </summary>
        /// <param name="condition">断言条件</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsFalse(bool condition)
        {
            NovaEngine.CAssert.IsFalse(condition);
        }

        /// <summary>
        /// 检测指定条件是否为FALSE的断言函数
        /// </summary>
        /// <param name="condition">断言条件</param>
        /// <param name="message">消息内容</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsFalse(bool condition, string message)
        {
            NovaEngine.CAssert.IsFalse(condition, message);
        }

        /// <summary>
        /// 检测指定条件是否为FALSE的断言函数
        /// </summary>
        /// <param name="condition">断言条件</param>
        /// <param name="format">消息格式</param>
        /// <param name="args">消息参数列表</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsFalse(bool condition, string format, params object[] args)
        {
            NovaEngine.CAssert.IsFalse(condition, format, args);
        }

        /// <summary>
        /// 检测指定类对象是否为NULL的断言函数
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="value">对象实例</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsNull<T>(T value) where T : class
        {
            NovaEngine.CAssert.IsNull(value);
        }

        /// <summary>
        /// 检测指定类对象是否为NULL的断言函数
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="value">对象实例</param>
        /// <param name="message">消息内容</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsNull<T>(T value, string message) where T : class
        {
            NovaEngine.CAssert.IsNull(value, message);
        }

        /// <summary>
        /// 检测指定类对象是否为NULL的断言函数
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="value">对象实例</param>
        /// <param name="format">消息格式</param>
        /// <param name="args">消息参数列表</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsNull<T>(T value, string format, params object[] args) where T : class
        {
            NovaEngine.CAssert.IsNull(value, format, args);
        }

        /// <summary>
        /// 检测指定类对象是否为非NULL的断言函数
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="value">对象实例</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsNotNull<T>(T value) where T : class
        {
            NovaEngine.CAssert.IsNotNull(value);
        }

        /// <summary>
        /// 检测指定类对象是否为非NULL的断言函数
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="value">对象实例</param>
        /// <param name="message">消息内容</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsNotNull<T>(T value, string message) where T : class
        {
            NovaEngine.CAssert.IsNotNull(value, message);
        }

        /// <summary>
        /// 检测指定类对象是否为非NULL的断言函数
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="value">对象实例</param>
        /// <param name="format">消息格式</param>
        /// <param name="args">消息参数列表</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsNotNull<T>(T value, string format, params object[] args) where T : class
        {
            NovaEngine.CAssert.IsNotNull(value, format, args);
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
            NovaEngine.CAssert.IsTypeOf<T>(obj);
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
            NovaEngine.CAssert.IsTypeOf<T>(obj, message);
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
            NovaEngine.CAssert.IsTypeOf<T>(obj, format, args);
        }

        /// <summary>
        /// 检测指定对象类型是否继承自目标类型的断言函数
        /// </summary>
        /// <typeparam name="T">父对象类型</typeparam>
        /// <param name="type">对象类型</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsTypeOf<T>(Type type) where T : class
        {
            NovaEngine.CAssert.IsTypeOf<T>(type);
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
            NovaEngine.CAssert.IsTypeOf<T>(type, message);
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
            NovaEngine.CAssert.IsTypeOf<T>(type, format, args);
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
            NovaEngine.CAssert.IsNullOrEmpty(str);
        }

        /// <summary>
        /// 检测指定字符串对象是否为空的断言函数
        /// </summary>
        /// <param name="str">字符串对象</param>
        /// <param name="message">消息内容</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsNullOrEmpty(string str, string message)
        {
            NovaEngine.CAssert.IsNullOrEmpty(str, message);
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
            NovaEngine.CAssert.IsNullOrEmpty(str, format, args);
        }

        /// <summary>
        /// 检测指定字符串对象是否为非空的断言函数
        /// </summary>
        /// <param name="str">字符串对象</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsNotNullOrEmpty(string str)
        {
            NovaEngine.CAssert.IsNotNullOrEmpty(str);
        }

        /// <summary>
        /// 检测指定字符串对象是否为非空的断言函数
        /// </summary>
        /// <param name="str">字符串对象</param>
        /// <param name="message">消息内容</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsNotNullOrEmpty(string str, string message)
        {
            NovaEngine.CAssert.IsNotNullOrEmpty(str, message);
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
            NovaEngine.CAssert.IsNotNullOrEmpty(str, format, args);
        }

        #endregion

        #region 基于数组检测封装的断言函数

        /// <summary>
        /// 检测指定数组对象是否为空的断言函数
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="array">数组对象</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsNullOrEmpty<T>(T[] array)
        {
            NovaEngine.CAssert.IsNullOrEmpty<T>(array);
        }

        /// <summary>
        /// 检测指定数组对象是否为空的断言函数
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="array">数组对象</param>
        /// <param name="message">消息内容</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsNullOrEmpty<T>(T[] array, string message)
        {
            NovaEngine.CAssert.IsNullOrEmpty<T>(array, message);
        }

        /// <summary>
        /// 检测指定数组对象是否为空的断言函数
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="array">数组对象</param>
        /// <param name="format">消息格式</param>
        /// <param name="args">消息参数列表</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsNullOrEmpty<T>(T[] array, string format, params object[] args)
        {
            NovaEngine.CAssert.IsNullOrEmpty<T>(array, format, args);
        }

        /// <summary>
        /// 检测指定数组对象是否为非空的断言函数
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="array">数组对象</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsNotNullOrEmpty<T>(T[] array)
        {
            NovaEngine.CAssert.IsNotNullOrEmpty<T>(array);
        }

        /// <summary>
        /// 检测指定数组对象是否为非空的断言函数
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="array">数组对象</param>
        /// <param name="message">消息内容</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsNotNullOrEmpty<T>(T[] array, string message)
        {
            NovaEngine.CAssert.IsNotNullOrEmpty<T>(array, message);
        }

        /// <summary>
        /// 检测指定数组对象是否为非空的断言函数
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="array">数组对象</param>
        /// <param name="format">消息格式</param>
        /// <param name="args">消息参数列表</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsNotNullOrEmpty<T>(T[] array, string format, params object[] args)
        {
            NovaEngine.CAssert.IsNotNullOrEmpty<T>(array, format, args);
        }

        #endregion

        #region 基于容器检测封装的断言函数

        /// <summary>
        /// 检测指定容器对象是否为空的断言函数
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="collection">容器对象</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsNullOrEmpty<T>(ICollection<T> collection)
        {
            NovaEngine.CAssert.IsNullOrEmpty<T>(collection);
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
            NovaEngine.CAssert.IsNullOrEmpty<T>(collection, message);
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
            NovaEngine.CAssert.IsNullOrEmpty<T>(collection, format, args);
        }

        /// <summary>
        /// 检测指定容器对象是否为空的断言函数
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="collection">容器对象</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsNullOrEmpty<T>(IReadOnlyCollection<T> collection)
        {
            NovaEngine.CAssert.IsNullOrEmpty<T>(collection);
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
            NovaEngine.CAssert.IsNullOrEmpty<T>(collection, message);
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
            NovaEngine.CAssert.IsNullOrEmpty<T>(collection, format, args);
        }

        /// <summary>
        /// 检测指定容器对象是否为非空的断言函数
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="collection">容器对象</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsNotNullOrEmpty<T>(ICollection<T> collection)
        {
            NovaEngine.CAssert.IsNotNullOrEmpty<T>(collection);
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
            NovaEngine.CAssert.IsNotNullOrEmpty<T>(collection, message);
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
            NovaEngine.CAssert.IsNotNullOrEmpty<T>(collection, format, args);
        }

        /// <summary>
        /// 检测指定容器对象是否为非空的断言函数
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="collection">容器对象</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsNotNullOrEmpty<T>(IReadOnlyCollection<T> collection)
        {
            NovaEngine.CAssert.IsNotNullOrEmpty<T>(collection);
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
            NovaEngine.CAssert.IsNotNullOrEmpty<T>(collection, message);
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
            NovaEngine.CAssert.IsNotNullOrEmpty<T>(collection, format, args);
        }

        #endregion
    }
}
