/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
/// Copyright (C) 2024 - 2025, Hurley, Independent Studio.
/// Copyright (C) 2025, Hainan Yuanyou Information Technology Co., Ltd. Guangzhou Branch
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
using System.Text;
using System.Runtime.CompilerServices;

namespace NovaEngine
{
    /// <summary>
    /// 格式化可变字符串的构建器对象类，此类是无法继承的。
    /// 对于执行大量字符串操作的例程（例如在循环中多次修改字符串的应用），反复修改字符串可能会产生显著的性能损失。
    /// 替代方法是使用 <b>FormatStringBuilder</b>，这是可变字符串类。
    /// 可变性意味着，在创建类的实例后，可以通过追加、删除、替换或插入字符来修改该类。
    ///
    /// 请考虑在这些条件下使用该 <b>FormatString</b> 类：
    /// 当代码对字符串所做的更改数较小时。在这些情况下， <b>FormatStringBuilder</b> 可能会提供微不足道或没有性能改进 <b>FormatString</b>。
    /// 执行固定数量的串联操作时，尤其是字符串文本。在这种情况下，编译器可以将串联操作合并为单个操作。
    /// 在生成字符串时必须执行广泛的搜索操作。类 <b>FormatStringBuilder</b> 缺少搜索方法，例如 <b>IndexOf</b> 或 <b>StartsWith</b>。你必须将 <b>FormatStringBuilder</b> 对象转换为这些操作的对象 <b>FormatString</b> ，这可以抵消使用 <b>FormatStringBuilder</b> 的性能优势。有关详细信息，请参阅 “在 <b>FormatStringBuilder</b> 对象 部分中搜索文本”。
    ///
    /// 请考虑在这些条件下使用该 <b>FormatStringBuilder</b> 类：
    /// 当你期望代码在设计时对字符串进行未知数量的更改（例如，使用循环连接包含用户输入的随机字符串时）。
    /// 当你期望代码对字符串进行大量更改时。
    /// </summary>
    public sealed class FormatStringBuilder
    {
        /// <summary>
        /// 构建器默认的预分配空间
        /// </summary>
        private const int DefaultCapacity = 256;
        /// <summary>
        /// 构建器缓存列表最大容量
        /// </summary>
        private const int MaxCacheSize = 16;

        /// <summary>
        /// 内置默认系统提供的可变字符串构建器对象实例
        /// </summary>
        private readonly StringBuilder _sb;

        /// <summary>
        /// 当前构建器对象实例是否处于活动状态
        /// </summary>
        private bool _isActive;

        /// <summary>
        /// 可变字符串构建器的缓存列表
        /// </summary>
        private static IList<FormatStringBuilder> _caches = new List<FormatStringBuilder>();

        private FormatStringBuilder() : this(DefaultCapacity)
        { }

        private FormatStringBuilder(int capacity)
        {
            _sb = new StringBuilder(capacity);
        }

        #region 格式化可变字符串内容添加接口函数

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [UnityEngine.Scripting.Preserve]
        public FormatStringBuilder Append(char value, int repeatCount) { _sb.Append(value, repeatCount); return this; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [UnityEngine.Scripting.Preserve]
        public FormatStringBuilder Append(byte value) { _sb.Append(value); return this; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [UnityEngine.Scripting.Preserve]
        public FormatStringBuilder Append(bool value) { _sb.Append(value); return this; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [UnityEngine.Scripting.Preserve]
        public FormatStringBuilder Append(char value) { _sb.Append(value); return this; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [UnityEngine.Scripting.Preserve]
        public FormatStringBuilder Append(long value) { _sb.Append(value); return this; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [UnityEngine.Scripting.Preserve]
        public FormatStringBuilder Append(int value) { _sb.Append(value); return this; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [UnityEngine.Scripting.Preserve]
        public FormatStringBuilder Append(short value) { _sb.Append(value); return this; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [UnityEngine.Scripting.Preserve]
        public FormatStringBuilder Append(double value) { _sb.Append(value); return this; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [UnityEngine.Scripting.Preserve]
        public FormatStringBuilder Append(char[] value) { _sb.Append(value); return this; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [UnityEngine.Scripting.Preserve]
        public FormatStringBuilder Append(char[] value, int startIndex, int charCount) { _sb.Append(value, startIndex, charCount); return this; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [UnityEngine.Scripting.Preserve]
        public FormatStringBuilder Append(float value) { _sb.Append(value); return this; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [UnityEngine.Scripting.Preserve]
        public FormatStringBuilder Append(decimal value) { _sb.Append(value); return this; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [UnityEngine.Scripting.Preserve]
        public FormatStringBuilder Append(object value) { _sb.Append(value); return this; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [UnityEngine.Scripting.Preserve]
        public FormatStringBuilder Append(string value) { _sb.Append(value); return this; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [UnityEngine.Scripting.Preserve]
        public FormatStringBuilder Append(string format, params object[] args)
        { _sb.Append(Utility.Text.Format(format, args)); return this; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [UnityEngine.Scripting.Preserve]
        public FormatStringBuilder Append(System.Array array, System.Func<object, string> callback)
        { _sb.Append(Utility.Text.ToString(array, callback)); return this; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [UnityEngine.Scripting.Preserve]
        public FormatStringBuilder Append(System.Array array, System.Func<int, object, string> callback = null)
        { _sb.Append(Utility.Text.ToString(array, callback)); return this; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [UnityEngine.Scripting.Preserve]
        public FormatStringBuilder Append(string format, System.Array array, System.Func<object, string> callback)
        { _sb.Append(Utility.Text.Format(format, Utility.Text.ToString(array, callback))); return this; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [UnityEngine.Scripting.Preserve]
        public FormatStringBuilder Append(string format, System.Array array, System.Func<int, object, string> callback = null)
        { _sb.Append(Utility.Text.Format(format, Utility.Text.ToString(array, callback))); return this; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [UnityEngine.Scripting.Preserve]
        public FormatStringBuilder Append<T>(T[] array, System.Func<T, string> callback)
        { _sb.Append(Utility.Text.ToString<T>(array, callback)); return this; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [UnityEngine.Scripting.Preserve]
        public FormatStringBuilder Append<T>(T[] array, System.Func<int, T, string> callback = null)
        { _sb.Append(Utility.Text.ToString<T>(array, callback)); return this; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [UnityEngine.Scripting.Preserve]
        public FormatStringBuilder Append<T>(string format, T[] array, System.Func<T, string> callback)
        { _sb.Append(Utility.Text.Format(format, Utility.Text.ToString<T>(array, callback))); return this; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [UnityEngine.Scripting.Preserve]
        public FormatStringBuilder Append<T>(string format, T[] array, System.Func<int, T, string> callback = null)
        { _sb.Append(Utility.Text.Format(format, Utility.Text.ToString<T>(array, callback))); return this; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [UnityEngine.Scripting.Preserve]
        public FormatStringBuilder Append(System.Collections.ICollection collection, System.Func<object, string> callback)
        { _sb.Append(Utility.Text.ToString(collection, callback)); return this; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [UnityEngine.Scripting.Preserve]
        public FormatStringBuilder Append(System.Collections.ICollection collection, System.Func<int, object, string> callback = null)
        { _sb.Append(Utility.Text.ToString(collection, callback)); return this; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [UnityEngine.Scripting.Preserve]
        public FormatStringBuilder Append(string format, System.Collections.ICollection collection, System.Func<object, string> callback)
        { _sb.Append(Utility.Text.Format(format, Utility.Text.ToString(collection, callback))); return this; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [UnityEngine.Scripting.Preserve]
        public FormatStringBuilder Append(string format, System.Collections.ICollection collection, System.Func<int, object, string> callback = null)
        { _sb.Append(Utility.Text.Format(format, Utility.Text.ToString(collection, callback))); return this; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [UnityEngine.Scripting.Preserve]
        public FormatStringBuilder Append<T>(ICollection<T> collection, System.Func<T, string> callback)
        { _sb.Append(Utility.Text.ToString<T>(collection, callback)); return this; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [UnityEngine.Scripting.Preserve]
        public FormatStringBuilder Append<T>(ICollection<T> collection, System.Func<int, T, string> callback = null)
        { _sb.Append(Utility.Text.ToString<T>(collection, callback)); return this; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [UnityEngine.Scripting.Preserve]
        public FormatStringBuilder Append<T>(string format, ICollection<T> collection, System.Func<T, string> callback)
        { _sb.Append(Utility.Text.Format(format, Utility.Text.ToString<T>(collection, callback))); return this; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [UnityEngine.Scripting.Preserve]
        public FormatStringBuilder Append<T>(string format, ICollection<T> collection, System.Func<int, T, string> callback = null)
        { _sb.Append(Utility.Text.Format(format, Utility.Text.ToString<T>(collection, callback))); return this; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [UnityEngine.Scripting.Preserve]
        public FormatStringBuilder Append<K, V>(ICollection<KeyValuePair<K, V>> collection, System.Func<K, V, string> callback = null)
        { _sb.Append(Utility.Text.ToString<K, V>(collection, callback)); return this; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [UnityEngine.Scripting.Preserve]
        public FormatStringBuilder Append<K, V>(string format, ICollection<KeyValuePair<K, V>> collection, System.Func<K, V, string> callback = null)
        { _sb.Append(Utility.Text.Format(format, Utility.Text.ToString<K, V>(collection, callback))); return this; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [UnityEngine.Scripting.Preserve]
        public FormatStringBuilder Append(System.Collections.IList list, System.Func<object, string> callback)
        { _sb.Append(Utility.Text.ToString(list, callback)); return this; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [UnityEngine.Scripting.Preserve]
        public FormatStringBuilder Append(System.Collections.IList list, System.Func<int, object, string> callback = null)
        { _sb.Append(Utility.Text.ToString(list, callback)); return this; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [UnityEngine.Scripting.Preserve]
        public FormatStringBuilder Append(string format, System.Collections.IList list, System.Func<object, string> callback)
        { _sb.Append(Utility.Text.Format(format, Utility.Text.ToString(list, callback))); return this; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [UnityEngine.Scripting.Preserve]
        public FormatStringBuilder Append(string format, System.Collections.IList list, System.Func<int, object, string> callback = null)
        { _sb.Append(Utility.Text.Format(format, Utility.Text.ToString(list, callback))); return this; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [UnityEngine.Scripting.Preserve]
        public FormatStringBuilder Append<T>(IList<T> list, System.Func<T, string> callback)
        { _sb.Append(Utility.Text.ToString<T>(list, callback)); return this; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [UnityEngine.Scripting.Preserve]
        public FormatStringBuilder Append<T>(IList<T> list, System.Func<int, T, string> callback = null)
        { _sb.Append(Utility.Text.ToString<T>(list, callback)); return this; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [UnityEngine.Scripting.Preserve]
        public FormatStringBuilder Append<T>(string format, IList<T> list, System.Func<T, string> callback)
        { _sb.Append(Utility.Text.Format(format, Utility.Text.ToString<T>(list, callback))); return this; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [UnityEngine.Scripting.Preserve]
        public FormatStringBuilder Append<T>(string format, IList<T> list, System.Func<int, T, string> callback = null)
        { _sb.Append(Utility.Text.Format(format, Utility.Text.ToString<T>(list, callback))); return this; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [UnityEngine.Scripting.Preserve]
        public FormatStringBuilder Append(System.Collections.IDictionary dictionary, System.Func<object, object, string> callback = null)
        { _sb.Append(Utility.Text.ToString(dictionary, callback)); return this; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [UnityEngine.Scripting.Preserve]
        public FormatStringBuilder Append(string format, System.Collections.IDictionary dictionary, System.Func<object, object, string> callback = null)
        { _sb.Append(Utility.Text.Format(format, Utility.Text.ToString(dictionary, callback))); return this; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [UnityEngine.Scripting.Preserve]
        public FormatStringBuilder Append<K, V>(IDictionary<K, V> dictionary, System.Func<K, V, string> callback = null)
        { _sb.Append(Utility.Text.ToString<K, V>(dictionary, callback)); return this; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [UnityEngine.Scripting.Preserve]
        public FormatStringBuilder Append<K, V>(string format, IDictionary<K, V> dictionary, System.Func<K, V, string> callback = null)
        { _sb.Append(Utility.Text.Format(format, Utility.Text.ToString<K, V>(dictionary, callback))); return this; }

        #endregion

        /// <summary>
        /// 清空字符串
        /// </summary>
        private void Clear()
        {
            _sb.Clear();
        }

        public override string ToString()
        {
            Logger.Assert(_isActive);

            _isActive = false;
            if (_caches.Count < MaxCacheSize)
                _caches.Add(this);

            return _sb.ToString();
        }

        /// <summary>
        /// 创建一个可变字符串构建器实例
        /// 若缓存中有空闲的实例，则优先从缓存中提取
        /// </summary>
        /// <returns>返回可变字符串构建器实例</returns>
        public static FormatStringBuilder Create()
        {
            FormatStringBuilder fsb = null;

            if (_caches.Count > 0)
            {
                fsb = _caches[0];
                fsb.Clear();
                _caches.RemoveAt(0);
            }
            else
            {
                fsb = new FormatStringBuilder();
            }

            // 标记为活动状态
            fsb._isActive = true;

            return fsb;
        }
    }
}
