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
    /// 无序的多值映射字典对象类
    /// 该对象类的多值采用<see cref="System.Collections.Generic.HashSet{T}"/>结构存储，因此相同键对应的值不可以重复存入
    /// 若强行存入相同元素，此前的元素将被覆盖
    /// </summary>
    public class UnorderMultiMapSet<TKey, TValue> : Dictionary<TKey, HashSet<TValue>>
    {
        /// <summary>
        /// 基于下标访问字典中的元素
        /// </summary>
        public new HashSet<TValue> this[TKey key]
        {
            get
            {
                HashSet<TValue> set;
                if (false == TryGetValue(key, out set))
                {
                    set = new HashSet<TValue>();
                }

                return set;
            }
        }

        /// <summary>
        /// 获取当前映射字典中所存储的值的总个数
        /// </summary>
        public new int Count
        {
            get
            {
                int count = 0;
                foreach (KeyValuePair<TKey, HashSet<TValue>> kv in this)
                {
                    count += kv.Value.Count;
                }
                return count;
            }
        }

        /// <summary>
        /// 向当前映射字典中新增指定的键值对
        /// </summary>
        /// <param name="key">数据键</param>
        /// <param name="value">数据值</param>
        public void Add(TKey key, TValue value)
        {
            HashSet<TValue> set;
            if (false == TryGetValue(key, out set))
            {
                // 新建值列表的实例
                set = new HashSet<TValue>();
                base[key] = set;
            }

            set.Add(value);
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
            HashSet<TValue> set;
            if (false == TryGetValue(key, out set))
            {
                return false;
            }

            if (false == set.Remove(value))
            {
                return false;
            }

            // 列表为空，移除列表实例
            if (set.Count == 0)
            {
                Remove(key);
            }

            return true;
        }

        /// <summary>
        /// 检测当前容器中是否存在指定键值对所对应的映射关联信息
        /// </summary>
        /// <param name="key">数据键</param>
        /// <param name="value">数据值</param>
        /// <returns>若当前容器中存在给定键值对所对应的映射关联信息则返回true，否则返回false</returns>
        public bool Contains(TKey key, TValue value)
        {
            HashSet<TValue> set;
            if (false == TryGetValue(key, out set))
            {
                return false;
            }

            return set.Contains(value);
        }
    }
}
