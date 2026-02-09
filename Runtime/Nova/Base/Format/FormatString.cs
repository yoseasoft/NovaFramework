/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
/// Copyright (C) 2024 - 2025, Hurley, Independent Studio.
/// Copyright (C) 2025 - 2026, Hainan Yuanyou Information Technology Co., Ltd. Guangzhou Branch
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
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using UnityEngine.Scripting;

namespace NovaEngine
{
    /// <summary>
    /// 格式化字符串对象类，此类是无法继承的。
    /// 基于 <b>System.String</b> 类，用于存储和操作文本数据。
    /// 字符串是不可变的（Immutable），意味着一旦创建，其内容无法更改。
    /// 引擎提供了丰富的字符串操作方法，适合处理文本、格式化和数据转换等任务。
    /// </summary>
    public static class FormatString
    {
        #region 格式化字符串内容相关的接口函数

        [Preserve]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Format(object value) { return ObjectFormatter.ToString(value); }

        [Preserve]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Format(string format, params object[] args)
        { return Utility.Text.Format(format, args); }

        [Preserve]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Format(Array array, Func<object, string> callback)
        { return Utility.Text.ToString(array, callback); }

        [Preserve]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Format(Array array, Func<int, object, string> callback = null)
        { return Utility.Text.ToString(array, callback); }

        [Preserve]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Format(string format, Array array, Func<object, string> callback)
        { return Utility.Text.Format(format, Utility.Text.ToString(array, callback)); }

        [Preserve]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Format(string format, Array array, Func<int, object, string> callback = null)
        { return Utility.Text.Format(format, Utility.Text.ToString(array, callback)); }

        [Preserve]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Format<T>(T[] array, Func<T, string> callback)
        { return Utility.Text.ToString(array, callback); }

        [Preserve]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Format<T>(T[] array, Func<int, T, string> callback = null)
        { return Utility.Text.ToString(array, callback); }

        [Preserve]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Format<T>(string format, T[] array, Func<T, string> callback)
        { return Utility.Text.Format(format, Utility.Text.ToString(array, callback)); }

        [Preserve]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Format<T>(string format, T[] array, Func<int, T, string> callback = null)
        { return Utility.Text.Format(format, Utility.Text.ToString(array, callback)); }

        [Preserve]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Format(ICollection collection, Func<object, string> callback)
        { return Utility.Text.ToString(collection, callback); }

        [Preserve]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Format(ICollection collection, Func<int, object, string> callback = null)
        { return Utility.Text.ToString(collection, callback); }

        [Preserve]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Format(string format, ICollection collection, Func<object, string> callback)
        { return Utility.Text.Format(format, Utility.Text.ToString(collection, callback)); }

        [Preserve]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Format(string format, ICollection collection, Func<int, object, string> callback = null)
        { return Utility.Text.Format(format, Utility.Text.ToString(collection, callback)); }

        [Preserve]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Format<T>(ICollection<T> collection, Func<T, string> callback)
        { return Utility.Text.ToString(collection, callback); }

        [Preserve]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Format<T>(IReadOnlyCollection<T> collection, Func<T, string> callback)
        { return Utility.Text.ToString(collection, callback); }

        [Preserve]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Format<T>(ICollection<T> collection, Func<int, T, string> callback = null)
        { return Utility.Text.ToString(collection, callback); }

        [Preserve]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Format<T>(IReadOnlyCollection<T> collection, Func<int, T, string> callback = null)
        { return Utility.Text.ToString(collection, callback); }

        [Preserve]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Format<T>(string format, ICollection<T> collection, Func<T, string> callback)
        { return Utility.Text.Format(format, Utility.Text.ToString(collection, callback)); }

        [Preserve]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Format<T>(string format, IReadOnlyCollection<T> collection, Func<T, string> callback)
        { return Utility.Text.Format(format, Utility.Text.ToString(collection, callback)); }

        [Preserve]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Format<T>(string format, ICollection<T> collection, Func<int, T, string> callback = null)
        { return Utility.Text.Format(format, Utility.Text.ToString(collection, callback)); }

        [Preserve]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Format<T>(string format, IReadOnlyCollection<T> collection, Func<int, T, string> callback = null)
        { return Utility.Text.Format(format, Utility.Text.ToString(collection, callback)); }

        [Preserve]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Format<K, V>(ICollection<KeyValuePair<K, V>> collection, Func<K, V, string> callback = null)
        { return Utility.Text.ToString(collection, callback); }

        [Preserve]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Format<K, V>(IReadOnlyCollection<KeyValuePair<K, V>> collection, Func<K, V, string> callback = null)
        { return Utility.Text.ToString(collection, callback); }

        [Preserve]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Format<K, V>(string format, ICollection<KeyValuePair<K, V>> collection, Func<K, V, string> callback = null)
        { return Utility.Text.Format(format, Utility.Text.ToString(collection, callback)); }

        [Preserve]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Format<K, V>(string format, IReadOnlyCollection<KeyValuePair<K, V>> collection, Func<K, V, string> callback = null)
        { return Utility.Text.Format(format, Utility.Text.ToString(collection, callback)); }

        [Preserve]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Format(IList list, Func<object, string> callback)
        { return Utility.Text.ToString(list, callback); }

        [Preserve]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Format(IList list, Func<int, object, string> callback = null)
        { return Utility.Text.ToString(list, callback); }

        [Preserve]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Format(string format, IList list, Func<object, string> callback)
        { return Utility.Text.Format(format, Utility.Text.ToString(list, callback)); }

        [Preserve]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Format(string format, IList list, Func<int, object, string> callback = null)
        { return Utility.Text.Format(format, Utility.Text.ToString(list, callback)); }

        [Preserve]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Format<T>(IList<T> list, Func<T, string> callback)
        { return Utility.Text.ToString(list, callback); }

        [Preserve]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Format<T>(IReadOnlyList<T> list, Func<T, string> callback)
        { return Utility.Text.ToString(list, callback); }

        [Preserve]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Format<T>(IList<T> list, Func<int, T, string> callback = null)
        { return Utility.Text.ToString(list, callback); }

        [Preserve]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Format<T>(IReadOnlyList<T> list, Func<int, T, string> callback = null)
        { return Utility.Text.ToString(list, callback); }

        [Preserve]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Format<T>(string format, IList<T> list, Func<T, string> callback)
        { return Utility.Text.Format(format, Utility.Text.ToString(list, callback)); }

        [Preserve]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Format<T>(string format, IReadOnlyList<T> list, Func<T, string> callback)
        { return Utility.Text.Format(format, Utility.Text.ToString(list, callback)); }

        [Preserve]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Format<T>(string format, IList<T> list, Func<int, T, string> callback = null)
        { return Utility.Text.Format(format, Utility.Text.ToString(list, callback)); }

        [Preserve]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Format<T>(string format, IReadOnlyList<T> list, Func<int, T, string> callback = null)
        { return Utility.Text.Format(format, Utility.Text.ToString(list, callback)); }

        [Preserve]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Format<T>(ISet<T> set, Func<T, string> callback)
        { return Utility.Text.ToString(set, callback); }

        [Preserve]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Format<T>(ISet<T> set, Func<int, T, string> callback = null)
        { return Utility.Text.ToString(set, callback); }

        [Preserve]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Format<T>(string format, ISet<T> set, Func<T, string> callback)
        { return Utility.Text.Format(format, Utility.Text.ToString(set, callback)); }

        [Preserve]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Format<T>(string format, ISet<T> set, Func<int, T, string> callback = null)
        { return Utility.Text.Format(format, Utility.Text.ToString(set, callback)); }

        [Preserve]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Format(IDictionary dictionary, Func<object, object, string> callback = null)
        { return Utility.Text.ToString(dictionary, callback); }

        [Preserve]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Format(string format, IDictionary dictionary, Func<object, object, string> callback = null)
        { return Utility.Text.Format(format, Utility.Text.ToString(dictionary, callback)); }

        [Preserve]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Format<K, V>(IDictionary<K, V> dictionary, Func<K, V, string> callback = null)
        { return Utility.Text.ToString(dictionary, callback); }

        [Preserve]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Format<K, V>(IReadOnlyDictionary<K, V> dictionary, Func<K, V, string> callback = null)
        { return Utility.Text.ToString(dictionary, callback); }

        [Preserve]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Format<K, V>(string format, IDictionary<K, V> dictionary, Func<K, V, string> callback = null)
        { return Utility.Text.Format(format, Utility.Text.ToString(dictionary, callback)); }

        [Preserve]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Format<K, V>(string format, IReadOnlyDictionary<K, V> dictionary, Func<K, V, string> callback = null)
        { return Utility.Text.Format(format, Utility.Text.ToString(dictionary, callback)); }

        #endregion
    }
}
