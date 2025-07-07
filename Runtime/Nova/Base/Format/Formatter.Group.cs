/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
/// Copyright (C) 2024 - 2025, Hurley, Independent Studio.
/// Copyright (C) 2025, Hainan Yuanyou Information Tecdhnology Co., Ltd. Guangzhou Branch
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

using SystemStringBuilder = System.Text.StringBuilder;

namespace NovaEngine
{
    /// <summary>
    /// 格式化接口集合工具类
    /// </summary>
    public static partial class Formatter
    {
        /// <summary>
        /// 数组容器的字符串描述输出函数
        /// </summary>
        /// <typeparam name="T">容器内的元素类型</typeparam>
        /// <param name="array">数组容器对象实例</param>
        /// <param name="callback">回调句柄</param>
        /// <returns>返回数组容器对应的字符串输出结果</returns>
        public static string ToString<T>(T[] array, System.Func<int, T, string> callback)
        {
            // return "[" + string.Join(',', array) + "]";
            SystemStringBuilder sb = new SystemStringBuilder();

            if (null == array)
            {
                sb.Append(Definition.CString.Null);
            }
            else
            {
                for (int n = 0; n < array.Length; ++n)
                {
                    if (n > 0) sb.Append(Definition.CCharacter.Comma);

                    sb.AppendFormat("{0}", callback(n, array[n]));
                }
            }

            return sb.ToString();
        }

        private static string ToString_Array(System.Array array)
        {
            SystemStringBuilder sb = new SystemStringBuilder();

            if (null == array)
            {
                sb.Append(Definition.CString.Null);
            }
            else
            {
                int n = 0;
                foreach (object item in array)
                {
                    if (n > 0) sb.Append(Definition.CCharacter.Comma);

                    sb.Append(ToString(item));
                    n++;
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// 集合容器的字符串描述输出函数
        /// </summary>
        /// <param name="collection">集合容器对象实例</param>
        /// <param name="callback">回调句柄</param>
        /// <returns>返回集合容器对应的字符串输出结果</returns>
        public static string ToString(System.Collections.ICollection collection, System.Func<int, object, string> callback)
        {
            SystemStringBuilder sb = new SystemStringBuilder();

            if (null == collection)
            {
                sb.Append(Definition.CString.Null);
            }
            else
            {
                int n = 0;
                System.Collections.IEnumerator e = collection.GetEnumerator();
                while (e.MoveNext())
                {
                    if (n > 0) sb.Append(Definition.CCharacter.Comma);

                    sb.Append(callback(n, e.Current));
                    n++;
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// 集合容器的字符串描述输出函数
        /// </summary>
        /// <typeparam name="T">数据值类型</typeparam>
        /// <param name="collection">集合容器对象实例</param>
        /// <param name="callback">回调句柄</param>
        /// <returns>返回集合容器对应的字符串输出结果</returns>
        public static string ToString<T>(ICollection<T> collection, System.Func<int, T, string> callback)
        {
            SystemStringBuilder sb = new SystemStringBuilder();

            if (null == collection)
            {
                sb.Append(Definition.CString.Null);
            }
            else
            {
                int n = 0;
                IEnumerator<T> e = collection.GetEnumerator();
                while (e.MoveNext())
                {
                    if (n > 0) sb.Append(Definition.CCharacter.Comma);

                    sb.Append(callback(n, e.Current));
                    n++;
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// 集合容器的字符串描述输出函数
        /// </summary>
        /// <typeparam name="K">数据键类型</typeparam>
        /// <typeparam name="V">数据值类型</typeparam>
        /// <param name="collection">集合容器对象实例</param>
        /// <param name="callback">回调句柄</param>
        /// <returns>返回集合容器对应的字符串输出结果</returns>
        public static string ToString<K, V>(ICollection<KeyValuePair<K, V>> collection, System.Func<K, V, string> callback)
        {
            SystemStringBuilder sb = new SystemStringBuilder();

            if (null == collection)
            {
                sb.Append(Definition.CString.Null);
            }
            else
            {
                int n = 0;
                IEnumerator<KeyValuePair<K, V>> e = collection.GetEnumerator();
                while (e.MoveNext())
                {
                    if (n > 0) sb.Append(Definition.CCharacter.Comma);

                    sb.Append(callback(e.Current.Key, e.Current.Value));
                    n++;
                }
            }

            return sb.ToString();
        }
    }
}
