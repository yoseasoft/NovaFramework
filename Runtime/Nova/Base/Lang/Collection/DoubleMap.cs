/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
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

using System.Collections.Generic;

namespace NovaEngine
{
    /// <summary>
    /// 双向索引的映射数据结构对象类
    /// </summary>
    public class DoubleMap<TKey, TValue>
    {
        /// <summary>
        /// 正向关联的映射对象容器实例
        /// </summary>
        private readonly IDictionary<TKey, TValue> _kv = null;
        /// <summary>
        /// 反向关联的映射对象容器实例
        /// </summary>
        private readonly IDictionary<TValue, TKey> _vk = null;

        /// <summary>
        /// 获取当前映射容器所有的键列表
        /// </summary>
        public IList<TKey> Keys
        {
            get { return new List<TKey>(_kv.Keys); }
        }

        /// <summary>
        /// 获取当前映射容器所有的值列表
        /// </summary>
        public IList<TValue> Values
        {
            get { return new List<TValue>(_vk.Keys); }
        }

        /// <summary>
        /// 获取映射容器中实际包含的主键数量
        /// </summary>
        public int Count
        {
            get { return _kv.Count; }
        }

        /// <summary>
        /// 双向映射的新实例构造函数
        /// </summary>
        public DoubleMap()
        {
            _kv = new Dictionary<TKey, TValue>();
            _vk = new Dictionary<TValue, TKey>();
        }

        /// <summary>
        /// 双向映射的新实例带参构造函数
        /// </summary>
        /// <param name="capacity">初始容器空间大小</param>
        public DoubleMap(int capacity)
        {
            _kv = new Dictionary<TKey, TValue>(capacity);
            _vk = new Dictionary<TValue, TKey>(capacity);
        }

        /// <summary>
        /// 双向映射的新实例析构函数
        /// </summary>
        ~DoubleMap()
        {
            Clear();
        }

        /// <summary>
        /// 遍历当前的映射容器，同时通过指定的委托行为处理每一次遍历的数据项
        /// </summary>
        /// <param name="action">委托行为</param>
        public void ForEach(System.Action<TKey, TValue> action)
        {
            if (null == action)
            {
                Logger.Warn("The ForEach call action must be non-null.");
                return;
            }

            // Dictionary<TKey, TValue>.KeyCollection keys = m_kv.Keys;
            ICollection<TKey> keys = _kv.Keys;
            foreach (TKey key in keys)
            {
                action(key, _kv[key]);
            }
        }

        /// <summary>
        /// 向当前映射容器中新增指定的键值对
        /// </summary>
        /// <param name="key">数据键</param>
        /// <param name="value">数据值</param>
        public void Add(TKey key, TValue value)
        {
            if (null == key || null == value ||
                _kv.ContainsKey(key) || _vk.ContainsKey(value))
            {
                Logger.Warn("Invalid arguments.");
                return;
            }

            // 在两个容器中分别进行数据新增
            _kv.Add(key, value);
            _vk.Add(value, key);
        }

        /// <summary>
        /// 通过指定键获取对应的映射值
        /// </summary>
        /// <param name="key">数据键</param>
        /// <returns>返回给定键对应的映射值，若不存在该键则返回值类型的默认值</returns>
        public TValue GetValueByKey(TKey key)
        {
            if (null != key && _kv.ContainsKey(key))
            {
                return _kv[key];
            }

            return default(TValue);
        }

        /// <summary>
        /// 通过指定键获取对应的映射值
        /// </summary>
        /// <param name="key">数据键</param>
        /// <param name="value">数据值</param>
        /// <returns>若查找映射值成功则返回true，否则返回false</returns>
        public bool TryGetValueByKey(TKey key, out TValue value)
        {
            if (null != key && _kv.ContainsKey(key))
            {
                value = _kv[key];
                return true;
            }

            value = default(TValue);

            return false;
        }

        /// <summary>
        /// 通过指定的映射值获取对应的键信息
        /// </summary>
        /// <param name="value">数据值</param>
        /// <returns>返回给定映射值对应的键信息，若不存在该值则返回键类型的默认值</returns>
        public TKey GetKeyByValue(TValue value)
        {
            if (null != value && _vk.ContainsKey(value))
            {
                return _vk[value];
            }

            return default(TKey);
        }

        /// <summary>
        /// 通过指定的映射值获取对应的键信息
        /// </summary>
        /// <param name="value">数据值</param>
        /// <param name="key">数据键</param>
        /// <returns>若查找键信息成功则返回true，否则返回false</returns>
        public bool TryGetKeyByValue(TValue value, out TKey key)
        {
            if (null != value && _vk.ContainsKey(value))
            {
                key = _vk[value];
                return true;
            }

            key = default(TKey);

            return false;
        }

        /// <summary>
        /// 通过指定的键移除其对应的映射关联数据项
        /// </summary>
        /// <param name="key">数据键</param>
        public void RemoveByKey(TKey key)
        {
            TValue value;
            if (null == key || !_kv.TryGetValue(key, out value))
            {
                return;
            }

            // 在两个容器中分别进行数据移除操作
            _kv.Remove(key);
            _vk.Remove(value);
        }

        /// <summary>
        /// 通过指定的映射值移除其对应的映射关联数据项
        /// </summary>
        /// <param name="value">数据值</param>
        public void RemoveByValue(TValue value)
        {
            TKey key;
            if (null == value || !_vk.TryGetValue(value, out key))
            {
                return;
            }

            // 在两个容器中分别进行数据移除操作
            _kv.Remove(key);
            _vk.Remove(value);
        }

        /// <summary>
        /// 检测当前容器中是否存在指定键对应的映射关联信息
        /// </summary>
        /// <param name="key">数据键</param>
        /// <returns>若当前容器中存在给定键对应的映射关联信息则返回true，否则返回false</returns>
        public bool ContainsKey(TKey key)
        {
            if (null == key)
            {
                return false;
            }

            return _kv.ContainsKey(key);
        }

        /// <summary>
        /// 检测当前容器中是否存在指定映射值对应的映射关联信息
        /// </summary>
        /// <param name="value">数据值</param>
        /// <returns>若当前容器中存在给定映射值对应的映射关联信息则返回true，否则返回false</returns>
        public bool ContainsValue(TValue value)
        {
            if (null == value)
            {
                return false;
            }

            return _vk.ContainsKey(value);
        }

        /// <summary>
        /// 检测当前容器中是否存在指定键值对所对应的映射关联信息
        /// </summary>
        /// <param name="key">数据键</param>
        /// <param name="value">数据值</param>
        /// <returns>若当前容器中存在给定键值对所对应的映射关联信息则返回true，否则返回false</returns>
        public bool Contains(TKey key, TValue value)
        {
            if (null == key || null == value)
            {
                return false;
            }

            return _kv.ContainsKey(key) && _vk.ContainsKey(value);
        }

        /// <summary>
        /// 清理双向索引映射容器中的全部数据
        /// </summary>
        public void Clear()
        {
            _kv.Clear();
            _vk.Clear();
        }
    }
}
