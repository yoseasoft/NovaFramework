/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
/// Copyright (C) 2023 - 2024, Guangzhou Shiyue Network Technology Co., Ltd.
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
    /// 复合键映射的字典数据结构对象类
    /// </summary>
    /// <typeparam name="TClassKey">对象键类型</typeparam>
    /// <typeparam name="TNameKey">名称键类型</typeparam>
    /// <typeparam name="TValue">字典的值类型</typeparam>
    public class CompositeKeyMap<TClassKey, TNameKey, TValue> : IEnumerable<TValue>, IEnumerable
    {
        private readonly DoubleMap<TClassKey, TNameKey> _keyMap;
        private readonly Dictionary<TClassKey, TValue>  _classValueMap;
        private readonly Dictionary<TNameKey, TValue>   _nameValueMap;

        /// <summary>
        /// 复合键映射字典的新实例构造函数
        /// </summary>
        public CompositeKeyMap()
        {
            _keyMap = new DoubleMap<TClassKey, TNameKey>();
            _classValueMap = new Dictionary<TClassKey, TValue>();
            _nameValueMap = new Dictionary<TNameKey, TValue>();
        }

        /// <summary>
        /// 复合键映射字典的新实例析构函数
        /// </summary>
        ~CompositeKeyMap()
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
        /// 获取复合字典中所有的类型键
        /// </summary>
        public Dictionary<TClassKey, TValue>.KeyCollection AllClassKeys
        {
            get { return _classValueMap.Keys; }
        }

        /// <summary>
        /// 获取复合字典中所有的名称键
        /// </summary>
        public Dictionary<TNameKey, TValue>.KeyCollection AllNameKeys
        {
            get { return _nameValueMap.Keys; }
        }

        /// <summary>
        /// 检查复合字典中是否包含指定主键
        /// </summary>
        /// <param name="key">要检查的主键</param>
        /// <returns>若复合字典中包含指定主键则返回true，否则返回false</returns>
        public bool ContainsKey(TClassKey key)
        {
            return _keyMap.ContainsKey(key);
        }

        /// <summary>
        /// 检查复合字典中是否包含指定主键
        /// </summary>
        /// <param name="key">要检查的主键</param>
        /// <returns>若复合字典中包含指定主键则返回true，否则返回false</returns>
        public bool ContainsKey(TNameKey key)
        {
            return _keyMap.ContainsValue(key);
        }

        /// <summary>
        /// 尝试获取复合字典中指定主键的值对象实例
        /// </summary>
        /// <param name="key">要检查的主键</param>
        /// <param name="value">值对象实例</param>
        /// <returns>若获取成功返回true，否则返回false</returns>
        public bool TryGetValue(TClassKey key, out TValue value)
        {
            return _classValueMap.TryGetValue(key, out value);
        }

        /// <summary>
        /// 尝试获取复合字典中指定主键的值对象实例
        /// </summary>
        /// <param name="key">要检查的主键</param>
        /// <param name="value">值对象实例</param>
        /// <returns>若获取成功返回true，否则返回false</returns>
        public bool TryGetValue(TNameKey key, out TValue value)
        {
            return _nameValueMap.TryGetValue(key, out value);
        }

        /// <summary>
        /// 向指定的主键增加指定的值
        /// </summary>
        /// <param name="ckey">指定的主键</param>
        /// <param name="nkey">指定的主键</param>
        /// <param name="value">指定的值</param>
        /// <returns>若数据值添加成功返回true，否则返回false</returns>
        public bool Add(TClassKey ckey, TNameKey nkey, TValue value)
        {
            if (ContainsKey(ckey) || ContainsKey(nkey))
            {
                Logger.Warn("The composite map key '{%o}, {%o}' was already exist, repeat added it failed.", ckey, nkey);
                return false;
            }

            _keyMap.Add(ckey, nkey);
            _classValueMap.Add(ckey, value);
            _nameValueMap.Add(nkey, value);
            return true;
        }

        /// <summary>
        /// 通过指定的主键中移除对应的值
        /// </summary>
        /// <param name="key">指定的主键</param>
        public void Remove(TClassKey key)
        {
            if (false == _keyMap.TryGetValueByKey(key, out TNameKey nameKey))
            {
                Logger.Warn("Could not found any value with class key '{%o}', removed it failed.", key);
                return;
            }

            _keyMap.RemoveByKey(key);
            _classValueMap.Remove(key);
            _nameValueMap.Remove(nameKey);
        }

        /// <summary>
        /// 通过指定的主键中移除对应的值
        /// </summary>
        /// <param name="key">指定的主键</param>
        public void Remove(TNameKey key)
        {
            if (false == _keyMap.TryGetKeyByValue(key, out TClassKey classKey))
            {
                Logger.Warn("Could not found any value with name key '{%o}', removed it failed.", key);
                return;
            }

            _keyMap.RemoveByValue(key);
            _classValueMap.Remove(classKey);
            _nameValueMap.Remove(key);
        }

        /// <summary>
        /// 清理复合字典全部数据
        /// </summary>
        public void Clear()
        {
            _keyMap.Clear();
            _classValueMap.Clear();
            _nameValueMap.Clear();
        }

        /// <summary>
        /// 获取复合字典中指定主键的数据值
        /// </summary>
        /// <param name="key">指定的主键</param>
        /// <returns>返回指定主键的数据值</returns>
        public TValue this[TClassKey key]
        {
            get
            {
                if (TryGetValue(key, out TValue value))
                {
                    return value;
                }

                throw new System.IndexOutOfRangeException();
            }
        }

        /// <summary>
        /// 获取复合字典中指定主键的数据值
        /// </summary>
        /// <param name="key">指定的主键</param>
        /// <returns>返回指定主键的数据值</returns>
        public TValue this[TNameKey key]
        {
            get
            {
                if (TryGetValue(key, out TValue value))
                {
                    return value;
                }

                throw new System.IndexOutOfRangeException();
            }
        }

        /// <summary>
        /// 返回循环访问集合的枚举数
        /// </summary>
        /// <returns>循环访问集合的枚举数</returns>
        public Enumerator GetEnumerator()
        {
            return new Enumerator(_classValueMap.Values);
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
            private Dictionary<TClassKey, TValue>.ValueCollection.Enumerator m_enumerator;

            internal Enumerator(Dictionary<TClassKey, TValue>.ValueCollection collection)
            {
                if (null == collection)
                {
                    throw new CFrameworkException("Dictionary's value collection is invalid.");
                }

                m_enumerator = collection.GetEnumerator();
            }

            /// <summary>
            /// 获取当前节点
            /// </summary>
            public TValue Current { get { return m_enumerator.Current; } }

            /// <summary>
            /// 获取当前的枚举数
            /// </summary>
            object IEnumerator.Current { get { return m_enumerator.Current; } }

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
                ((IEnumerator<TValue>) m_enumerator).Reset();
            }
        }
    }
}
