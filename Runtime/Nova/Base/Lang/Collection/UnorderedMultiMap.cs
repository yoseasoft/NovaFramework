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
using SystemArray = System.Array;

namespace NovaEngine
{
    /// <summary>
    /// 无序的多值映射字典对象类
    /// 该对象类的多值采用<see cref="System.Collections.Generic.List{T}"/>结构存储，因此相同键对应的值可以重复存入
    /// 若存入的值为基础类型，则移除时将选择首次存入的相同值进行删除操作
    /// </summary>
    public class UnorderedMultiMap<TKey, TValue> : Dictionary<TKey, List<TValue>>
    {
        /// <summary>
        /// 基于下标访问字典中的元素
        /// </summary>
        public new List<TValue> this[TKey key]
        {
            get
            {
                List<TValue> list;
                TryGetValue(key, out list);
                return list;
            }
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
                // 新建值列表的实例
                list = new List<TValue>();
                base[key] = list;
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
                Remove(key);
            }

            return true;
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
                return SystemArray.Empty<TValue>();
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
