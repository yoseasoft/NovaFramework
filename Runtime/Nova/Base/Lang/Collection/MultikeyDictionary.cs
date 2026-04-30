/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
/// Copyright (C) 2023 - 2024, Guangzhou Shiyue Network Technology Co., Ltd.
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

using System;
using System.Collections;
using System.Collections.Generic;

namespace NovaEngine
{
    /// <summary>
    /// 复合键映射的字典数据结构对象类
    /// </summary>
    /// <typeparam name="TFirstKey">第一种键类型</typeparam>
    /// <typeparam name="TSecondKey">第二种键类型</typeparam>
    /// <typeparam name="TValue">字典的值类型</typeparam>
    public class MultikeyDictionary<TFirstKey, TSecondKey, TValue> : IEnumerable<TValue>, IEnumerable
    {
        private readonly DoubleMap<TFirstKey, TSecondKey> _keyMap;
        private readonly Dictionary<TFirstKey, TValue>  _firstKeyValueMap;
        private readonly Dictionary<TSecondKey, TValue>   _secondKeyValueMap;

        /// <summary>
        /// 复合键映射字典的新实例构造函数
        /// </summary>
        public MultikeyDictionary()
        {
            _keyMap = new DoubleMap<TFirstKey, TSecondKey>();
            _firstKeyValueMap = new Dictionary<TFirstKey, TValue>();
            _secondKeyValueMap = new Dictionary<TSecondKey, TValue>();
        }

        /// <summary>
        /// 复合键映射字典的新实例析构函数
        /// </summary>
        ~MultikeyDictionary()
        {
            Clear();
        }

        /// <summary>
        /// 获取复合字典中实际包含的主键数量
        /// </summary>
        public int Count
        {
            get { return _keyMap.Count; }
        }

        /// <summary>
        /// 获取复合字典中所有的第一种键
        /// </summary>
        public Dictionary<TFirstKey, TValue>.KeyCollection FirstKeys
        {
            get { return _firstKeyValueMap.Keys; }
        }

        /// <summary>
        /// 获取复合字典中所有的第二种键
        /// </summary>
        public Dictionary<TSecondKey, TValue>.KeyCollection SecondKeys
        {
            get { return _secondKeyValueMap.Keys; }
        }

        /// <summary>
        /// 获取所有的对象值
        /// </summary>
        public IList<TValue> Values
        {
            get { return Utility.Collection.ToList(_secondKeyValueMap.Values); }
        }

        /// <summary>
        /// 检查复合字典中是否包含指定主键
        /// </summary>
        /// <param name="key">要检查的主键</param>
        /// <returns>若复合字典中包含指定主键则返回true，否则返回false</returns>
        public bool ContainsKey(TFirstKey key)
        {
            return _firstKeyValueMap.ContainsKey(key);
        }

        /// <summary>
        /// 检查复合字典中是否包含指定主键
        /// </summary>
        /// <param name="key">要检查的主键</param>
        /// <returns>若复合字典中包含指定主键则返回true，否则返回false</returns>
        public bool ContainsKey(TSecondKey key)
        {
            return _secondKeyValueMap.ContainsKey(key);
        }

        /// <summary>
        /// 尝试获取复合字典中指定主键的值对象实例
        /// </summary>
        /// <param name="key">要检查的主键</param>
        /// <param name="value">值对象实例</param>
        /// <returns>若获取成功返回true，否则返回false</returns>
        public bool TryGetValue(TFirstKey key, out TValue value)
        {
            return _firstKeyValueMap.TryGetValue(key, out value);
        }

        /// <summary>
        /// 尝试获取复合字典中指定主键的值对象实例
        /// </summary>
        /// <param name="key">要检查的主键</param>
        /// <param name="value">值对象实例</param>
        /// <returns>若获取成功返回true，否则返回false</returns>
        public bool TryGetValue(TSecondKey key, out TValue value)
        {
            return _secondKeyValueMap.TryGetValue(key, out value);
        }

        /// <summary>
        /// 向指定的主键增加指定的值
        /// </summary>
        /// <param name="firstKey">指定的主键</param>
        /// <param name="secondKey">指定的主键</param>
        /// <param name="value">指定的值</param>
        /// <returns>若数据值添加成功返回true，否则返回false</returns>
        public bool Add(TFirstKey firstKey, TSecondKey secondKey, TValue value)
        {
            if (ContainsKey(firstKey) || ContainsKey(secondKey))
            {
                CLogger.Warn("The composite map key '{%v}, {%v}' was already exist, repeat added it failed.", firstKey, secondKey);
                return false;
            }

            _keyMap.Add(firstKey, secondKey);
            _firstKeyValueMap.Add(firstKey, value);
            _secondKeyValueMap.Add(secondKey, value);
            return true;
        }

        /// <summary>
        /// 通过指定的主键中移除对应的值
        /// </summary>
        /// <param name="firstKey">指定的主键</param>
        public void Remove(TFirstKey firstKey)
        {
            if (false == _keyMap.TryGetValueByKey(firstKey, out TSecondKey secondKey))
            {
                CLogger.Warn("Could not found any value with class key '{%v}', removed it failed.", firstKey);
                return;
            }

            _keyMap.RemoveByKey(firstKey);
            _firstKeyValueMap.Remove(firstKey);
            _secondKeyValueMap.Remove(secondKey);
        }

        /// <summary>
        /// 通过指定的主键中移除对应的值
        /// </summary>
        /// <param name="secondKey">指定的主键</param>
        public void Remove(TSecondKey secondKey)
        {
            if (false == _keyMap.TryGetKeyByValue(secondKey, out TFirstKey firstKey))
            {
                CLogger.Warn("Could not found any value with name key '{%v}', removed it failed.", secondKey);
                return;
            }

            _keyMap.RemoveByValue(secondKey);
            _firstKeyValueMap.Remove(firstKey);
            _secondKeyValueMap.Remove(secondKey);
        }

        /// <summary>
        /// 清理复合字典全部数据
        /// </summary>
        public void Clear()
        {
            _keyMap.Clear();
            _firstKeyValueMap.Clear();
            _secondKeyValueMap.Clear();
        }

        /// <summary>
        /// 获取复合字典中指定主键的数据值
        /// </summary>
        /// <param name="key">指定的主键</param>
        /// <returns>返回指定主键的数据值</returns>
        public TValue this[TFirstKey key]
        {
            get
            {
                if (TryGetValue(key, out TValue value))
                {
                    return value;
                }

                throw new IndexOutOfRangeException();
            }
        }

        /// <summary>
        /// 获取复合字典中指定主键的数据值
        /// </summary>
        /// <param name="key">指定的主键</param>
        /// <returns>返回指定主键的数据值</returns>
        public TValue this[TSecondKey key]
        {
            get
            {
                if (TryGetValue(key, out TValue value))
                {
                    return value;
                }

                throw new IndexOutOfRangeException();
            }
        }

        /// <summary>
        /// 返回循环访问集合的枚举数
        /// </summary>
        /// <returns>循环访问集合的枚举数</returns>
        public Enumerator GetEnumerator()
        {
            return new Enumerator(_firstKeyValueMap.Values);
        }

        /// <summary>
        /// 返回循环访问集合的枚举数
        /// </summary>
        /// <returns>循环访问集合的枚举数</returns>
        IEnumerator<TValue> IEnumerable<TValue>.GetEnumerator()
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
        public struct Enumerator : IEnumerator<TValue>, IEnumerator
        {
            private Dictionary<TFirstKey, TValue>.ValueCollection.Enumerator _enumerator;

            internal Enumerator(Dictionary<TFirstKey, TValue>.ValueCollection collection)
            {
                if (null == collection)
                {
                    throw new CFrameworkException("Dictionary's value collection is invalid.");
                }

                _enumerator = collection.GetEnumerator();
            }

            /// <summary>
            /// 获取当前节点
            /// </summary>
            public TValue Current { get { return _enumerator.Current; } }

            /// <summary>
            /// 获取当前的枚举数
            /// </summary>
            object IEnumerator.Current { get { return _enumerator.Current; } }

            /// <summary>
            /// 清理枚举数
            /// </summary>
            public void Dispose()
            {
                _enumerator.Dispose();
            }

            /// <summary>
            /// 获取下一个节点
            /// </summary>
            /// <returns>返回下一个节点</returns>
            public bool MoveNext()
            {
                return _enumerator.MoveNext();
            }

            /// <summary>
            /// 重置枚举数
            /// </summary>
            void IEnumerator.Reset()
            {
                ((IEnumerator<TValue>) _enumerator).Reset();
            }
        }
    }
}
