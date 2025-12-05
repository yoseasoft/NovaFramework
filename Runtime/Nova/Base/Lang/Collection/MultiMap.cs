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

using System;
using System.Collections.Generic;

namespace NovaEngine
{
    /// <summary>
    /// 基于排序字典实现的带缓存功能的多值映射字典对象封装类
    /// 该类继承自<see cref="System.Collections.Generic.SortedDictionary{TKey, List{TValue}}"/>，并在基础上对值进行了列表封装
    /// 同时该类提供了列表对象的缓存机制，适用于大量且高频率对列表进行增删的逻辑
    /// 需要注意的是，字典内部的多值映射关系是使用<see cref="System.Collections.Generic.List{T}"/>进行管理
    /// 此数据结构支持存放重复元素，因此您可以将同一个实例多次进行添加操作
    /// </summary>
    public class MultiMap<TKey, TValue> : SortedDictionary<TKey, List<TValue>>
    {
        /// <summary>
        /// 空列表容器对象，用于进行默认值返回
        /// </summary>
        private readonly List<TValue> Empty = new();
        /// <summary>
        /// 最大缓冲区长度
        /// </summary>
        private readonly int _maxPoolSize = 0;
        /// <summary>
        /// 列表实例的缓冲区队列
        /// </summary>
        private readonly Queue<List<TValue>> _pool;

        /// <summary>
        /// 多值映射字典的新实例构造函数
        /// </summary>
        /// <param name="maxPoolSize">最大缓冲区长度</param>
        public MultiMap(int maxPoolSize)
        {
            _maxPoolSize = maxPoolSize;
            _pool = new Queue<List<TValue>>(maxPoolSize);
        }

        /// <summary>
        /// 多值映射字典的新实例析构函数
        /// </summary>
        ~MultiMap()
        {
        }

        /// <summary>
        /// 基于下标访问字典中的元素
        /// </summary>
        public new List<TValue> this[TKey key]
        {
            get
            {
                if (TryGetValue(key, out List<TValue> list))
                {
                    return list;
                }

                // 返回默认空列表
                return Empty;
            }
        }

        /// <summary>
        /// 从缓冲池中分配列表对象实例的操作函数
        /// 若缓冲池为空，则新创建一个列表对象实例
        /// </summary>
        /// <returns>返回有效的列表对象实例</returns>
        private List<TValue> Fetch()
        {
            if (_pool.Count > 0)
            {
                // 弹出缓存实例
                return _pool.Dequeue();
            }

            return new List<TValue>(16);
        }

        /// <summary>
        /// 回收列表对象实例到缓冲池中的操作函数
        /// 若缓冲池中存储的实例已超过最大上限，则直接丢弃
        /// </summary>
        /// <param name="list">列表对象实例</param>
        private void Recycle(List<TValue> list)
        {
            if (null == list)
            { return; }

            // 缓冲区达到上限，丢弃实例
            if (_pool.Count >= _maxPoolSize)
            { return; }

            // 重置列表数据
            list.Clear();

            // 推入缓存实例
            _pool.Enqueue(list);
        }

        /// <summary>
        /// 向当前映射字典中新增指定的键值对
        /// </summary>
        /// <param name="key">数据键</param>
        /// <param name="value">数据值</param>
        public void Add(TKey key, TValue value)
        {
            List<TValue> list;
            if (false == TryGetValue(key, out list))
            {
                // 申请新的列表实例
                list = Fetch();
                this.Add(key, list);
            }

            list.Add(value);
        }

        /// <summary>
        /// 通过指定的键和值移除容器中其对应的映射关联数据项
        /// 移除成功后，若指定键下对应的值列表为空，则同时移除该键及其对应的列表实例
        /// </summary>
        /// <param name="key">数据键</param>
        /// <param name="value">数据值</param>
        /// <returns>从当前映射容器中移除数据项成功则返回true，否则返回false</returns>
        public bool Remove(TKey key, TValue value)
        {
            List<TValue> list;
            if (false == TryGetValue(key, out list))
            {
                return false;
            }

            if (false == list.Remove(value))
            {
                return false;
            }

            // 列表为空，移除列表实例
            if (list.Count == 0)
            {
                this.Remove(key);
            }

            return true;
        }

        /// <summary>
        /// 移除指定键对应的整个列表所有数据项
        /// </summary>
        /// <param name="key">数据键</param>
        /// <returns>若移除给定键对应的列表实例成功返回true，否则返回false</returns>
        public new bool Remove(TKey key)
        {
            List<TValue> list;
            if (false == TryGetValue(key, out list))
            {
                return false;
            }

            // 回收列表实例
            Recycle(list);

            return base.Remove(key);
        }

        /// <summary>
        /// 获取当前映射字典中指定键对应的全部值列表信息
        /// </summary>
        /// <param name="key">数据键</param>
        /// <returns>返回给定键对应的所有值列表信息</returns>
        public TValue[] GetAll(TKey key)
        {
            List<TValue> list;
            if (false == TryGetValue(key, out list))
            {
                return Array.Empty<TValue>();
            }

            return list.ToArray();
        }

        /// <summary>
        /// 获取当前映射字典中指定键对应的任意值信息
        /// 若该键同时对应了多个值，则默认返回首个值数据
        /// </summary>
        /// <param name="key">数据键</param>
        /// <returns>返回给定键对应的任意值数据，若一个都没找到，则返回值类型的默认值</returns>
        public TValue GetOne(TKey key)
        {
            List<TValue> list;
            if (TryGetValue(key, out list) && list.Count > 0)
            {
                return list[0];
            }

            return default(TValue);
        }

        /// <summary>
        /// 检测当前容器中是否存在指定键值对所对应的映射关联信息
        /// </summary>
        /// <param name="key">数据键</param>
        /// <param name="value">数据值</param>
        /// <returns>若当前容器中存在给定键值对所对应的映射关联信息则返回true，否则返回false</returns>
        public bool Contains(TKey key, TValue value)
        {
            List<TValue> list;
            if (false == TryGetValue(key, out list))
            {
                return false;
            }

            return list.Contains(value);
        }
    }
}
