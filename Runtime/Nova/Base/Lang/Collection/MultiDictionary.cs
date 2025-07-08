/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
/// Copyright (C) 2020 - 2022, Guangzhou Xinyuan Technology Co., Ltd.
/// Copyright (C) 2022 - 2023, Shanghai Bilibili Technology Co., Ltd.
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

using System.Collections;
using System.Collections.Generic;

namespace NovaEngine
{
    /// <summary>
    /// 多值字典数据结构对象类
    /// </summary>
    /// <typeparam name="TKey">指定多值字典的主键类型</typeparam>
    /// <typeparam name="TValue">指定多值字典的值类型</typeparam>
    public sealed class MultiDictionary<TKey, TValue> : IEnumerable<KeyValuePair<TKey, DoubleLinkedList<TValue>>>, IEnumerable
    {
        private readonly CacheLinkedList<TValue> _linkedList;
        private readonly Dictionary<TKey, DoubleLinkedList<TValue>> _dictionary;

        /// <summary>
        /// 多值字典的新实例构造函数
        /// </summary>
        public MultiDictionary()
        {
            _linkedList = new CacheLinkedList<TValue>();
            _dictionary = new Dictionary<TKey, DoubleLinkedList<TValue>>();
        }

        /// <summary>
        /// 多值字典的新实例析构函数
        /// </summary>
        ~MultiDictionary()
        {
            Clear();
        }

        /// <summary>
        /// 获取多值字典中实际包含的主键数量
        /// </summary>
        public int Count
        {
            get { return _dictionary.Count; }
        }

        /// <summary>
        /// 检查多值字典中是否包含指定主键
        /// </summary>
        /// <param name="key">要检查的主键</param>
        /// <returns>若多值字典中包含指定主键则返回true，否则返回false</returns>
        public bool Contains(TKey key)
        {
            return _dictionary.ContainsKey(key);
        }

        /// <summary>
        /// 检查多值字典中是否包含指定值
        /// </summary>
        /// <param name="key">要检查的主键</param>
        /// <param name="value">要检查的值</param>
        /// <returns>若多值字典中包含指定值则返回true，否则返回false</returns>
        public bool Contains(TKey key, TValue value)
        {
            DoubleLinkedList<TValue> range = default(DoubleLinkedList<TValue>);
            if (_dictionary.TryGetValue(key, out range))
            {
                return range.Contains(value);
            }

            return false;
        }

        /// <summary>
        /// 尝试获取多值字典中指定主键的范围链表实例
        /// </summary>
        /// <param name="key">指定的主键</param>
        /// <param name="range">指定主键的范围链表引用</param>
        /// <returns>若获取成功返回true，否则返回false</returns>
        public bool TryGetValue(TKey key, out DoubleLinkedList<TValue> range)
        {
            return _dictionary.TryGetValue(key, out range);
        }

        /// <summary>
        /// 向指定的主键增加指定的值
        /// </summary>
        /// <param name="key">指定的主键</param>
        /// <param name="value">指定的值</param>
        public void Add(TKey key, TValue value)
        {
            DoubleLinkedList<TValue> range = default(DoubleLinkedList<TValue>);
            if (_dictionary.TryGetValue(key, out range))
            {
                _linkedList.AddBefore(range.Terminal, value);
            }
            else
            {
                LinkedListNode<TValue> first = _linkedList.AddLast(value);
                LinkedListNode<TValue> terminal = _linkedList.AddLast(default(TValue));
                _dictionary.Add(key, new DoubleLinkedList<TValue>(first, terminal));
            }
        }

        /// <summary>
        /// 从指定的主键中移除指定的值
        /// </summary>
        /// <param name="key">指定的主键</param>
        /// <param name="value">指定的值</param>
        /// <returns>若移除成功则返回true，否则返回false</returns>
        public bool Remove(TKey key, TValue value)
        {
            DoubleLinkedList<TValue> range = default(DoubleLinkedList<TValue>);
            if (_dictionary.TryGetValue(key, out range))
            {
                for (LinkedListNode<TValue> current = range.First; current != null && current != range.Terminal; current = current.Next)
                {
                    if (current.Value.Equals(value))
                    {
                        if (current == range.First)
                        {
                            LinkedListNode<TValue> next = current.Next;
                            if (next == range.Terminal)
                            {
                                _linkedList.Remove(next);
                                _dictionary.Remove(key);
                            }
                            else
                            {
                                _dictionary[key] = new DoubleLinkedList<TValue>(next, range.Terminal);
                            }
                        }

                        _linkedList.Remove(current);
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// 从指定的主键中移除所有的值
        /// </summary>
        /// <param name="key">指定的主键</param>
        /// <returns>若移除成功则返回true，否则返回false</returns>
        public bool RemoveAll(TKey key)
        {
            DoubleLinkedList<TValue> range = default(DoubleLinkedList<TValue>);
            if (_dictionary.TryGetValue(key, out range))
            {
                _dictionary.Remove(key);

                LinkedListNode<TValue> current = range.First;
                while (null != current)
                {
                    LinkedListNode<TValue> next = current != range.Terminal ? current.Next : null;
                    _linkedList.Remove(current);
                    current = next;
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// 清理多值字典全部数据
        /// </summary>
        public void Clear()
        {
            _dictionary.Clear();
            _linkedList.Clear();
        }

        /// <summary>
        /// 获取多值字典中指定主键的范围链表实例
        /// </summary>
        /// <param name="key">指定的主键</param>
        /// <returns>返回指定主键的范围链表实例</returns>
        public DoubleLinkedList<TValue> this[TKey key]
        {
            get
            {
                DoubleLinkedList<TValue> range = default(DoubleLinkedList<TValue>);
                _dictionary.TryGetValue(key, out range);
                return range;
            }
        }

        /// <summary>
        /// 返回循环访问集合的枚举数
        /// </summary>
        /// <returns>循环访问集合的枚举数</returns>
        public Enumerator GetEnumerator()
        {
            return new Enumerator(_dictionary);
        }

        /// <summary>
        /// 返回循环访问集合的枚举数
        /// </summary>
        /// <returns>循环访问集合的枚举数</returns>
        IEnumerator<KeyValuePair<TKey, DoubleLinkedList<TValue>>> IEnumerable<KeyValuePair<TKey, DoubleLinkedList<TValue>>>.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// 返回循环访问集合的枚举数
        /// </summary>
        /// <returns>循环访问集合的枚举数</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// 循环访问集合的枚举数
        /// </summary>
        public struct Enumerator : IEnumerator<KeyValuePair<TKey, DoubleLinkedList<TValue>>>, IEnumerator
        {
            private Dictionary<TKey, DoubleLinkedList<TValue>>.Enumerator m_enumerator;

            internal Enumerator(Dictionary<TKey, DoubleLinkedList<TValue>> dictionary)
            {
                if (null == dictionary)
                {
                    throw new CFrameworkException("Dictionary is invalid.");
                }

                m_enumerator = dictionary.GetEnumerator();
            }

            /// <summary>
            /// 获取当前节点
            /// </summary>
            public KeyValuePair<TKey, DoubleLinkedList<TValue>> Current
            {
                get
                {
                    return m_enumerator.Current;
                }
            }

            /// <summary>
            /// 获取当前的枚举数
            /// </summary>
            object IEnumerator.Current
            {
                get
                {
                    return m_enumerator.Current;
                }
            }

            /// <summary>
            /// 清理枚举数
            /// </summary>
            public void Dispose()
            {
                m_enumerator.Dispose();
            }

            /// <summary>
            /// 获取下一个节点
            /// </summary>
            /// <returns>返回下一个节点</returns>
            public bool MoveNext()
            {
                return m_enumerator.MoveNext();
            }

            /// <summary>
            /// 重置枚举数
            /// </summary>
            void IEnumerator.Reset()
            {
                ((IEnumerator<KeyValuePair<TKey, DoubleLinkedList<TValue>>>) m_enumerator).Reset();
            }
        }
    }
}
