/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
/// Copyright (C) 2022 - 2023, Shanghai Bilibili Technology Co., Ltd.
/// Copyright (C) 2023 - 2024, Guangzhou Shiyue Network Technology Co., Ltd.
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
using System.Linq;

using SystemArray = System.Array;

namespace NovaEngine
{
    /// <summary>
    /// 实用函数集合工具类
    /// </summary>
    public static partial class Utility
    {
        /// <summary>
        /// 集合相关实用函数集合
        /// </summary>
        public static class Collection
        {
            #region 容器类型转换相关的接口函数

            /// <summary>
            /// 将枚举列表中的数据进行顺序翻转的接口函数
            /// </summary>
            /// <typeparam name="TSource">数据类型</typeparam>
            /// <param name="source">枚举列表容器</param>
            /// <returns>返回翻转后的枚举列表数据容器</returns>
            public static IEnumerable<TSource> Reverse<TSource>(IEnumerable<TSource> source)
            {
                return source.Reverse<TSource>();
            }

            /// <summary>
            /// 检测枚举列表中是否存在有效值
            /// </summary>
            /// <typeparam name="TSource">数据类型</typeparam>
            /// <param name="source">枚举列表容器</param>
            /// <returns>若存在有效值则返回true，否则返回false</returns>
            public static bool Any<TSource>(IEnumerable<TSource> source)
            {
                return source.Any();
            }

            /// <summary>
            /// 检测枚举列表中是否存在有效值
            /// </summary>
            /// <param name="source">枚举列表容器</param>
            /// <returns>若存在有效值则返回true，否则返回false</returns>
            public static bool Any(IEnumerable<object> source)
            {
                return source.Any();
            }

            /// <summary>
            /// 集合类型数据转换为数组类型数据的接口函数
            /// </summary>
            /// <typeparam name="T">数据类型</typeparam>
            /// <param name="source">集合数据容器</param>
            /// <returns>返回转换后的数组类型数据容器</returns>
            public static T[] ToArray<T>(ICollection<T> source)
            {
                return source?.ToArray<T>();
            }

            /// <summary>
            /// 通过字典类型数据的键信息转换为数组类型数据的接口函数
            /// </summary>
            /// <typeparam name="K">字典的键类型</typeparam>
            /// <typeparam name="V">字典的值类型</typeparam>
            /// <param name="dictionary">字典数据容器</param>
            /// <returns>返回转换后的数组类型数据容器</returns>
            public static K[] ToArrayForKeys<K, V>(IDictionary<K, V> dictionary)
            {
                return dictionary?.Keys.ToArray<K>();
            }

            /// <summary>
            /// 通过字典类型数据的值信息转换为数组类型数据的接口函数
            /// </summary>
            /// <typeparam name="K">字典的键类型</typeparam>
            /// <typeparam name="V">字典的值类型</typeparam>
            /// <param name="dictionary">字典数据容器</param>
            /// <returns>返回转换后的数组类型数据容器</returns>
            public static V[] ToArrayForValues<K, V>(IDictionary<K, V> dictionary)
            {
                return dictionary?.Values.ToArray<V>();
            }

            /// <summary>
            /// 集合类型数据进行类型转换后再重构为数组类型数据的接口函数
            /// </summary>
            /// <typeparam name="SourceType">源数据类型</typeparam>
            /// <typeparam name="TargetType">目标数据类型</typeparam>
            /// <param name="source">集合数据容器</param>
            /// <returns>返回转换后的数组类型数据容器</returns>
            public static TargetType[] CastAndToArray<SourceType, TargetType>(ICollection<SourceType> source)
            {
                return source?.Cast<TargetType>().ToArray();
            }

            /// <summary>
            /// 集合类型数据跳过指定数量元素后转换为数组类型数据的接口函数
            /// </summary>
            /// <typeparam name="TSource">数据类型</typeparam>
            /// <param name="source">集合数据容器</param>
            /// <param name="count">跳过元素数量</param>
            /// <returns>返回转换后的数组类型数据容器</returns>
            public static TSource[] SkipAndToArray<TSource>(ICollection<TSource> source, int count)
            {
                return source?.Skip(count).ToArray();
            }

            /// <summary>
            /// 集合类型数据转换为列表类型数据的接口函数
            /// </summary>
            /// <typeparam name="T">数据类型</typeparam>
            /// <param name="source">集合数据容器</param>
            /// <returns>返回转换后的列表类型数据容器</returns>
            public static IList<T> ToList<T>(ICollection<T> source)
            {
                return source?.ToList<T>();
            }

            /// <summary>
            /// 通过字典类型数据的键信息转换为列表类型数据的接口函数
            /// </summary>
            /// <typeparam name="K">字典的键类型</typeparam>
            /// <typeparam name="V">字典的值类型</typeparam>
            /// <param name="dictionary">字典数据容器</param>
            /// <returns>返回转换后的列表类型数据容器</returns>
            public static IList<K> ToListForKeys<K, V>(IDictionary<K, V> dictionary)
            {
                return dictionary?.Keys.ToList<K>();
            }

            /// <summary>
            /// 通过字典类型数据的键信息转换为列表类型数据的接口函数
            /// </summary>
            /// <typeparam name="K">字典的键类型</typeparam>
            /// <typeparam name="V">字典的值类型</typeparam>
            /// <param name="dictionary">字典数据容器</param>
            /// <returns>返回转换后的列表类型数据容器</returns>
            public static IList<K> ToListForKeys<K, V>(IReadOnlyDictionary<K, V> dictionary)
            {
                return dictionary?.Keys.ToList<K>();
            }

            /// <summary>
            /// 通过字典类型数据的值信息转换为列表类型数据的接口函数
            /// </summary>
            /// <typeparam name="K">字典的键类型</typeparam>
            /// <typeparam name="V">字典的值类型</typeparam>
            /// <param name="dictionary">字典数据容器</param>
            /// <returns>返回转换后的列表类型数据容器</returns>
            public static IList<V> ToListForValues<K, V>(IDictionary<K, V> dictionary)
            {
                return dictionary?.Values.ToList<V>();
            }

            /// <summary>
            /// 集合类型数据进行类型转换后再重构为列表类型数据的接口函数
            /// </summary>
            /// <typeparam name="SourceType">源数据类型</typeparam>
            /// <typeparam name="TargetType">目标数据类型</typeparam>
            /// <param name="source">集合数据容器</param>
            /// <returns>返回转换后的列表类型数据容器</returns>
            public static IList<TargetType> CastAndToList<SourceType, TargetType>(ICollection<SourceType> source)
            {
                return source?.Cast<TargetType>().ToList();
            }

            #endregion

            #region 容器类型查找相关的接口函数

            /// <summary>
            /// 获取列表容器中的最小值
            /// </summary>
            /// <typeparam name="T">值数据类型</typeparam>
            /// <typeparam name="K">比较数据类型</typeparam>
            /// <param name="array">列表容器</param>
            /// <param name="handler">回调句柄</param>
            /// <returns>返回列表中的最小值</returns>
            public static T Min<T, K>(IList<T> array, System.Func<T, K> handler) where K : System.IComparable<K>
            {
                T temp = default(T);
                temp = array[0];
                foreach (T arr in array)
                {
                    if (handler(temp).CompareTo(handler(arr)) > 0)
                    {
                        temp = arr;
                    }
                }

                return temp;
            }

            /// <summary>
            /// 获取列表容器中的最大值
            /// </summary>
            /// <typeparam name="T">值数据类型</typeparam>
            /// <typeparam name="K">比较数据类型</typeparam>
            /// <param name="array">列表容器</param>
            /// <param name="handler">回调句柄</param>
            /// <returns>返回列表中的最大值</returns>
            public static T Max<T, K>(IList<T> array, System.Func<T, K> handler) where K : System.IComparable<K>
            {
                T temp = default(T);
                temp = array[0];
                foreach (T arr in array)
                {
                    if (handler(temp).CompareTo(handler(arr)) < 0)
                    {
                        temp = arr;
                    }
                }

                return temp;
            }

            /// <summary>
            /// 获取列表容器中的最小值
            /// </summary>
            /// <typeparam name="T">数据类型</typeparam>
            /// <param name="array">列表容器</param>
            /// <param name="comparison">比较函数的回调句柄</param>
            /// <returns>返回列表中的最小值</returns>
            public static T Min<T>(IList<T> array, System.Comparison<T> comparison)
            {
                T temp = default(T);
                temp = array[0];
                foreach (T arr in array)
                {
                    if (comparison(temp, arr) > 0)
                    {
                        temp = arr;
                    }
                }

                return temp;
            }

            /// <summary>
            /// 获取列表容器中的最大值
            /// </summary>
            /// <typeparam name="T">数据类型</typeparam>
            /// <param name="array">列表容器</param>
            /// <param name="comparison">比较函数的回调句柄</param>
            /// <returns>返回列表中的最大值</returns>
            public static T Max<T>(IList<T> array, System.Comparison<T> comparison)
            {
                T temp = default(T);
                temp = array[0];
                foreach (T arr in array)
                {
                    if (comparison(temp, arr) < 0)
                    {
                        temp = arr;
                    }
                }

                return temp;
            }

            /// <summary>
            /// 获得传入元素某个符合条件的所有对象
            /// </summary>
            /// <typeparam name="T">数据类型</typeparam>
            /// <param name="array">列表容器</param>
            /// <param name="handler">条件函数的回调句柄</param>
            /// <returns>返回符合条件的目标对象实例</returns>
            public static T Find<T>(IList<T> array, System.Predicate<T> handler)
            {
                T temp = default(T);
                for (int n = 0; n < array.Count; ++n)
                {
                    if (handler(array[n]))
                    {
                        return array[n];
                    }
                }

                return temp;
            }

            /// <summary>
            /// 获得传入元素某个符合条件的所有对象
            /// </summary>
            /// <typeparam name="T">数据类型</typeparam>
            /// <param name="array">列表容器</param>
            /// <param name="handler">条件函数的回调句柄</param>
            /// <returns>返回符合条件的对象实例集合</returns>
            public static T[] FindAll<T>(IList<T> array, System.Predicate<T> handler)
            {
                var dstArray = new T[array.Count];
                int idx = 0;
                for (int n = 0; n < array.Count; ++n)
                {
                    if (handler(array[n]))
                    {
                        dstArray[idx] = array[n];
                        idx++;
                    }
                }

                SystemArray.Resize(ref dstArray, idx);
                return dstArray;
            }

            #endregion
        }
    }
}
