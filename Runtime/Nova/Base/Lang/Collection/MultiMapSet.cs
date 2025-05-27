/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
/// Copyring (C) 2023, Guangzhou Shiyue Network Technology Co., Ltd.
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
using System.Linq;

using SystemArray = System.Array;

namespace NovaEngine
{
    /// <summary>
    /// 基于排序字典实现的多值映射字典对象封装类
    /// 该类继承自<see cref="System.Collections.Generic.SortedDictionary{TKey, HashSet{TValue}}"/>，并在基础上对值进行了集合封装
    /// 需要注意的是，字典内部的多值映射关系是使用<see cref="System.Collections.Generic.HashSet{T}"/>进行管理
    /// 此数据结构不支持存放重复元素，因此如果您将同一个实例多次进行添加操作时，最后一次添加的元素将覆盖之前的所有记录
    /// </summary>
    public class MultiMapSet<TKey, TValue> : Dictionary<TKey, HashSet<TValue>>
    {
        /// <summary>
        /// 空集合容器对象，用于进行默认值返回
        /// </summary>
        private readonly HashSet<TValue> Empty = new();

        /// <summary>
        /// 多值映射字典的新实例构造函数
        /// </summary>
        public MultiMapSet()
        {
        }

        /// <summary>
        /// 多值映射字典的新实例析构函数
        /// </summary>
        ~MultiMapSet()
        {
        }

        /// <summary>
        /// 基于下标访问字典中的元素
        /// </summary>
        public new HashSet<TValue> this[TKey key]
        {
            get
            {
                if (TryGetValue(key, out HashSet<TValue> set))
                {
                    return set;
                }

                // 返回默认空集合
                return Empty;
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
                set = new HashSet<TValue>();
                this.Add(key, set);
            }

            set.Add(value);
        }

        /// <summary>
        /// 通过指定的键和值移除容器中其对应的映射关联数据项
        /// 移除成功后，若指定键下对应的值集合为空，则同时移除该键及其对应的集合实例
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

            // 集合为空，移除集合实例
            if (set.Count == 0)
            {
                this.Remove(key);
            }

            return true;
        }

        /// <summary>
        /// 获取当前映射字典中指定键对应的全部值集合信息
        /// </summary>
        /// <param name="key">数据键</param>
        /// <returns>返回给定键对应的所有值集合信息</returns>
        public TValue[] GetAll(TKey key)
        {
            HashSet<TValue> set;
            if (false == TryGetValue(key, out set))
            {
                return SystemArray.Empty<TValue>();
            }

            return set.ToArray();
        }

        /// <summary>
        /// 获取当前映射字典中指定键对应的任意值信息
        /// 若该键同时对应了多个值，则默认返回首个值数据
        /// </summary>
        /// <param name="key">数据键</param>
        /// <returns>返回给定键对应的任意值数据，若一个都没找到，则返回值类型的默认值</returns>
        public TValue GetOne(TKey key)
        {
            HashSet<TValue> set;
            if (TryGetValue(key, out set) && set.Count > 0)
            {
                return set.FirstOrDefault();
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
            HashSet<TValue> set;
            if (false == TryGetValue(key, out set))
            {
                return false;
            }

            return set.Contains(value);
        }
    }
}
