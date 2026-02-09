/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
/// Copyright (C) 2022 - 2023, Shanghai Bilibili Technology Co., Ltd.
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
using System.Text;
using System.Reflection;
using System.Runtime.CompilerServices;

using UnityEngine.Scripting;

namespace NovaEngine
{
    /// 实用函数集合工具类
    public static partial class Utility
    {
        /// <summary>
        /// 字符串相关实用函数集合
        /// </summary>
        public static class Text
        {
            /// <summary>
            /// 字符串构建器缓存对象实例，用于内部提供字符串文本组装
            /// </summary>
            [ThreadStatic] // 每个静态类型字段对于每一个线程都是唯一的
            private static StringBuilder _stringBuilderCache = new StringBuilder(4096);

            /// <summary>
            /// 将指定内存尺寸转换为字符串形式，且使用最大单位值
            /// </summary>
            /// <param name="size">内存尺寸</param>
            /// <returns>返回转换后的字符串信息</returns>
            public static string GetSizeText(long size)
            {
                const double KB = 1024;
                const double MB = 1024 * KB;
                const double GB = 1024 * MB;

                double gb = (double) size / (double) GB;
                double mb = (double) size / (double) MB;
                double kb = (double) size / (double) KB;

                if (gb > 1)
                {
                    return string.Format("{0:#.3}gb", gb);
                }
                else if (mb > 1)
                {
                    return string.Format("{0:#.#}mb", mb);
                }
                else if (kb > 1)
                {
                    return string.Format("{0:#.#}kb", kb);
                }

                return string.Format("{0:0.#}b", kb);
            }

            /// <summary>
            /// 生成指定长度的随机字符串接口函数
            /// </summary>
            /// <param name="length">字符串长度</param>
            /// <returns>返回生成的随机字符串</returns>
            public static string GenerateRandomString(int length)
            {
                _stringBuilderCache.Clear();
                for (int n = 0; n < length; ++n)
                {
                    _stringBuilderCache.Append(Definition.CCharacter.KEYCODE_ARRAY[Random.Next(62)]);
                }

                return _stringBuilderCache.ToString();
            }

            /// <summary>
            /// 对象 <see cref="object.ToString"/> 信息连接接口函数
            /// </summary>
            /// <param name="args">对象数组</param>
            /// <returns>返回连接后的字符串</returns>
            public static string Append(params object[] args)
            {
                if (null == args)
                    Logger.Throw<ArgumentNullException>("Append is invalid.");

                _stringBuilderCache.Clear();
                int length = args.Length;
                for (int n = 0; n < length; ++n)
                {
                    _stringBuilderCache.Append(args[n]);
                }

                return _stringBuilderCache.ToString();
            }

            /// <summary>
            /// 字符串合并接口函数
            /// </summary>
            /// <param name="strings">字符串数组</param>
            /// <returns>返回合并后的字符串</returns>
            public static string Combine(params string[] strings)
            {
                if (null == strings)
                    Logger.Throw<ArgumentNullException>("Combine is invalid.");

                _stringBuilderCache.Length = 0;
                int length = strings.Length;
                for (int n = 0; n < length; ++n)
                {
                    _stringBuilderCache.Append(strings[n]);
                }

                return _stringBuilderCache.ToString();
            }

            /// <summary>
            /// 使用指定的格式及参数获取对应的格式化字符串
            /// </summary>
            /// <param name="format">字符串格式</param>
            /// <param name="args">字符串参数</param>
            /// <returns>返回格式化后的字符串</returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static string Format(string format, params object[] args)
            {
                return ObjectFormatter.TextFormatConvertionProcess(format, args);
            }

            /// <summary>
            /// 使用指定的格式及参数获取对应的格式化字符串，并存放到目标缓冲区中
            /// </summary>
            /// <param name="buff">目标缓冲区</param>
            /// <param name="format">字符串格式</param>
            /// <param name="args">字符串参数</param>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void FormatToBuffer(StringBuilder buff, string format, params object[] args)
            {
                if (buff == null)
                {
                    throw new CFrameworkException("Invalid arguments.");
                }

                buff.Length = 0;
                buff.Append(ObjectFormatter.TextFormatConvertionProcess(format, args));
            }

            #region 对象的字符串输出封装接口函数

            /// <summary>
            /// 对象类型的字符串描述输出函数
            /// </summary>
            /// <param name="classType">对象类型</param>
            /// <returns>返回对象类型对应的字符串输出结果</returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static string ToString(Type classType)
            {
                return null == classType ? Definition.CString.Null : GetFullName(classType);
            }

            /// <summary>
            /// 特性对象的字符串描述输出函数
            /// </summary>
            /// <param name="attribute">特性对象</param>
            /// <returns>返回特性对象对应的字符串输出结果</returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static string ToString(Attribute attribute)
            {
                return null == attribute ? Definition.CString.Null : GetFullName(attribute);
            }

            /// <summary>
            /// 字段对象的字符串描述输出函数
            /// </summary>
            /// <param name="field">字段对象</param>
            /// <returns>返回字段对象对应的字符串输出结果</returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static string ToString(FieldInfo field)
            {
                return null == field ? Definition.CString.Null : GetFullName(field);
            }

            /// <summary>
            /// 属性对象的字符串描述输出函数
            /// </summary>
            /// <param name="property">属性对象</param>
            /// <returns>返回属性对象对应的字符串输出结果</returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static string ToString(PropertyInfo property)
            {
                return null == property ? Definition.CString.Null : GetFullName(property);
            }

            /// <summary>
            /// 委托回调的字符串描述输出函数
            /// </summary>
            /// <param name="callback">委托回调</param>
            /// <returns>返回委托回调对应的字符串输出结果</returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static string ToString(Delegate callback)
            {
                return null == callback ? Definition.CString.Null : GetFullName(callback);
            }

            /// <summary>
            /// 函数对象的字符串描述输出函数
            /// </summary>
            /// <param name="method">函数对象</param>
            /// <returns>返回函数对象对应的字符串输出结果</returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static string ToString(MethodBase method)
            {
                return null == method ? Definition.CString.Null : GetFullName(method);
            }

            /// <summary>
            /// 参数对象的字符串描述输出函数
            /// </summary>
            /// <param name="parameter">参数对象</param>
            /// <returns>返回参数对象对应的字符串输出结果</returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static string ToString(ParameterInfo parameter)
            {
                return null == parameter ? Definition.CString.Null : GetFullName(parameter);
            }

            /// <summary>
            /// 返回指定类型的全名字符串信息
            /// </summary>
            /// <param name="targetType">对象类型</param>
            /// <returns>返回字符串信息</returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static string GetFullName(Type targetType)
            {
                return targetType.FullName;
            }

            /// <summary>
            /// 返回指定特性的全名字符串信息
            /// </summary>
            /// <param name="attribute">特性对象</param>
            /// <returns>返回字符串信息</returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static string GetFullName(Attribute attribute)
            {
                return attribute.GetType().FullName;
            }

            /// <summary>
            /// 返回指定字段的全名字符串信息
            /// </summary>
            /// <param name="field">字段对象</param>
            /// <returns>返回字符串信息</returns>
            public static string GetFullName(FieldInfo field)
            {
                StringBuilder stringBuilder = new StringBuilder();

                stringBuilder.Append(GetFullName(field.FieldType));
                stringBuilder.Append(Definition.CCharacter.Space);
                stringBuilder.Append(field.Name);
                stringBuilder.Append(Definition.CCharacter.LeftParen);
                stringBuilder.Append(field.DeclaringType.FullName);
                stringBuilder.Append(Definition.CCharacter.RightParen);

                return stringBuilder.ToString();
            }

            /// <summary>
            /// 返回指定属性的全名字符串信息
            /// </summary>
            /// <param name="property">属性对象</param>
            /// <returns>返回字符串信息</returns>
            public static string GetFullName(PropertyInfo property)
            {
                StringBuilder stringBuilder = new StringBuilder();

                stringBuilder.Append(GetFullName(property.PropertyType));
                stringBuilder.Append(Definition.CCharacter.Space);
                stringBuilder.Append(property.Name);
                stringBuilder.Append(Definition.CCharacter.LeftParen);
                stringBuilder.Append(property.DeclaringType.FullName);
                stringBuilder.Append(Definition.CCharacter.RightParen);
                stringBuilder.Append(Definition.CCharacter.Space);

                stringBuilder.Append(Definition.CCharacter.LeftBracket);
                MethodInfo getMethodInfo = property.GetGetMethod(true);
                if (null != getMethodInfo)
                {
                    stringBuilder.Append(GetFullName(getMethodInfo));
                }

                MethodInfo setMethodInfo = property.GetSetMethod(true);
                if (null != setMethodInfo)
                {
                    if (null != getMethodInfo) stringBuilder.Append(Definition.CCharacter.Semicolon);
                    stringBuilder.Append(GetFullName(setMethodInfo));
                }

                if (null == getMethodInfo && null == setMethodInfo)
                {
                    stringBuilder.Append(Definition.CString.Null);
                }
                stringBuilder.Append(Definition.CCharacter.RightBracket);

                return stringBuilder.ToString();
            }

            /// <summary>
            /// 返回指定委托的全名字符串信息
            /// </summary>
            /// <param name="callback">委托对象</param>
            /// <returns>返回字符串信息</returns>
            public static string GetFullName(Delegate callback)
            {
                StringBuilder stringBuilder = new StringBuilder();

                if (null != callback.Target)
                {
                    stringBuilder.Append(GetFullName(callback.Target.GetType()));
                }
                else
                {
                    // 静态函数
                    stringBuilder.Append(Definition.CCharacter.LeftBracket);
                    stringBuilder.Append(callback.Method.DeclaringType.FullName);
                    stringBuilder.Append(Definition.CCharacter.RightBracket);
                }

                stringBuilder.Append(Definition.CCharacter.Dot);
                stringBuilder.Append(__GetFullName(callback.Method));
                return stringBuilder.ToString();
            }

            /// <summary>
            /// 返回指定函数的全名字符串信息
            /// </summary>
            /// <param name="method">函数对象</param>
            /// <returns>返回字符串信息</returns>
            public static string GetFullName(MethodBase method)
            {
                StringBuilder stringBuilder = new StringBuilder();

                stringBuilder.Append(method.DeclaringType.FullName);
                stringBuilder.Append(Definition.CCharacter.Dot);
                stringBuilder.Append(__GetFullName(method));
                return stringBuilder.ToString();
            }

            /// <summary>
            /// 返回指定函数的全名字符串信息
            /// </summary>
            /// <param name="method">函数对象</param>
            /// <returns>返回字符串信息</returns>
            private static string __GetFullName(MethodBase method)
            {
                StringBuilder stringBuilder = new StringBuilder();

                stringBuilder.Append(method.Name);
                if (method.IsGenericMethod)
                {
                    Type[] genericArguments = method.GetGenericArguments();
                    stringBuilder.Append("<");
                    for (int n = 0; n < genericArguments.Length; n++)
                    {
                        if (n != 0)
                        {
                            stringBuilder.Append(", ");
                        }

                        stringBuilder.Append(genericArguments[n].FullName);
                    }

                    stringBuilder.Append(">");
                }

                stringBuilder.Append("(");
                stringBuilder.Append(__GetMethodParamsNames(method));
                stringBuilder.Append(")");
                return stringBuilder.ToString();
            }

            /// <summary>
            /// 返回指定函数的参数名称列表字符串信息
            /// </summary>
            /// <param name="method">函数对象</param>
            /// <returns>返回字符串信息</returns>
            private static string __GetMethodParamsNames(MethodBase method)
            {
                ParameterInfo[] array = (Reflection.IsTypeOfExtension(method) ? Collection.SkipAndToArray(method.GetParameters(), 1) : method.GetParameters());
                StringBuilder stringBuilder = new StringBuilder();
                for (int n = 0, num = array.Length; n < num; ++n)
                {
                    if (n > 0)
                    {
                        stringBuilder.Append(", ");
                    }

                    ParameterInfo parameterInfo = array[n];

                    stringBuilder.Append(__GetFullName(parameterInfo));
                }

                return stringBuilder.ToString();
            }

            /// <summary>
            /// 返回指定参数的全名字符串信息
            /// </summary>
            /// <param name="parameter">参数对象</param>
            /// <returns>返回字符串信息</returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static string GetFullName(ParameterInfo parameter)
            {
                return __GetFullName(parameter);
            }

            /// <summary>
            /// 返回指定参数的全名字符串信息
            /// </summary>
            /// <param name="parameter">参数对象</param>
            /// <returns>返回字符串信息</returns>
            private static string __GetFullName(ParameterInfo parameter)
            {
                StringBuilder stringBuilder = new StringBuilder();

                string niceName = parameter.ParameterType.FullName;

                // 检测参数是否为输出类型
                if (parameter.IsOut)
                {
                    stringBuilder.Append("out ");
                }

                stringBuilder.Append(niceName);
                stringBuilder.Append(Definition.CCharacter.Space);
                stringBuilder.Append(parameter.Name);

                // 添加参数的默认值
                if (parameter.HasDefaultValue)
                {
                    stringBuilder.Append(" = ");
                    stringBuilder.Append(parameter.DefaultValue?.ToString());
                }

                return stringBuilder.ToString();
            }

            #endregion

            #region 容器类对象的字符串输出封装接口函数

            /// <summary>
            /// 数组容器的字符串描述输出函数
            /// </summary>
            /// <param name="array">数组容器对象实例</param>
            /// <param name="callback">输出回调句柄</param>
            /// <returns>返回数组容器对应的字符串输出结果</returns>
            [Preserve]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static string ToString(Array array, Func<object, string> callback)
            {
                return ToString(array, (n, obj) =>
                {
                    return $"{n}={{{callback(obj)}}}";
                });
            }

            /// <summary>
            /// 数组容器的字符串描述输出函数
            /// </summary>
            /// <param name="array">数组容器对象实例</param>
            /// <param name="callback">输出回调句柄</param>
            /// <returns>返回数组容器对应的字符串输出结果</returns>
            [Preserve]
            public static string ToString(Array array, Func<int, object, string> callback = null)
            {
                if (null == array)
                {
                    return Definition.CString.Null;
                }

                StringBuilder sb = new StringBuilder();

                int n = 0;
                foreach (object item in array)
                {
                    if (n > 0) sb.Append(Definition.CCharacter.Comma);

                    if (null == callback)
                    {
                        sb.AppendFormat("{0}={1}", n, item.ToString());
                    }
                    else
                    {
                        sb.Append(callback(n, item));
                    }
                    n++;
                }

                return sb.ToString();
            }

            /// <summary>
            /// 数组容器的字符串描述输出函数
            /// </summary>
            /// <typeparam name="T">容器内的元素类型</typeparam>
            /// <param name="array">数组容器对象实例</param>
            /// <param name="callback">输出回调句柄</param>
            /// <returns>返回数组容器对应的字符串输出结果</returns>
            [Preserve]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static string ToString<T>(T[] array, Func<T, string> callback)
            {
                return ToString<T>(array, (n, obj) =>
                {
                    return $"{n}={{{callback(obj)}}}";
                });
            }

            /// <summary>
            /// 数组容器的字符串描述输出函数
            /// </summary>
            /// <typeparam name="T">容器内的元素类型</typeparam>
            /// <param name="array">数组容器对象实例</param>
            /// <param name="callback">输出回调句柄</param>
            /// <returns>返回数组容器对应的字符串输出结果</returns>
            [Preserve]
            public static string ToString<T>(T[] array, Func<int, T, string> callback = null)
            {
                if (null == array)
                {
                    return Definition.CString.Null;
                }

                // return "[" + string.Join(',', array) + "]";
                StringBuilder sb = new StringBuilder();

                for (int n = 0; n < array.Length; ++n)
                {
                    if (n > 0) sb.Append(Definition.CCharacter.Comma);

                    if (null == callback)
                    {
                        sb.AppendFormat("{0}={1}", n, array[n].ToString());
                    }
                    else
                    {
                        sb.Append(callback(n, array[n]));
                    }
                }

                return sb.ToString();
            }

            /// <summary>
            /// 集合容器的字符串描述输出函数
            /// </summary>
            /// <param name="collection">集合容器对象实例</param>
            /// <param name="callback">输出回调句柄</param>
            /// <returns>返回集合容器对应的字符串输出结果</returns>
            [Preserve]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static string ToString(ICollection collection, Func<object, string> callback)
            {
                return ToString(collection?.GetEnumerator(), callback);
            }

            /// <summary>
            /// 集合容器的字符串描述输出函数
            /// </summary>
            /// <param name="collection">集合容器对象实例</param>
            /// <param name="callback">输出回调句柄</param>
            /// <returns>返回集合容器对应的字符串输出结果</returns>
            [Preserve]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static string ToString(ICollection collection, Func<int, object, string> callback = null)
            {
                return ToString(collection?.GetEnumerator(), callback);
            }

            /// <summary>
            /// 迭代器的字符串描述输出函数
            /// </summary>
            /// <param name="enumerator">迭代器对象实例</param>
            /// <param name="callback">输出回调句柄</param>
            /// <returns>返回迭代器对应的字符串输出结果</returns>
            [Preserve]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static string ToString(IEnumerator enumerator, Func<object, string> callback)
            {
                return ToString(enumerator, (n, obj) =>
                {
                    return $"{n}={{{callback(obj)}}}";
                });
            }

            /// <summary>
            /// 迭代器的字符串描述输出函数
            /// </summary>
            /// <param name="enumerator">迭代器对象实例</param>
            /// <param name="callback">输出回调句柄</param>
            /// <returns>返回迭代器对应的字符串输出结果</returns>
            [Preserve]
            public static string ToString(IEnumerator enumerator, Func<int, object, string> callback = null)
            {
                if (null == enumerator)
                {
                    return Definition.CString.Null;
                }

                StringBuilder sb = new StringBuilder();

                int n = 0;
                while (enumerator.MoveNext())
                {
                    if (n > 0) sb.Append(Definition.CCharacter.Comma);

                    if (null == callback)
                    {
                        if (enumerator.Current is DictionaryEntry entry)
                        {
                            sb.AppendFormat("{0}={1}", entry.Key.ToString(), entry.Value.ToString());
                        }
                        else
                        {
                            sb.AppendFormat("{0}={1}", n, enumerator.Current.ToString());
                        }
                    }
                    else
                    {
                        sb.Append(callback(n, enumerator.Current));
                    }
                    n++;
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
            [Preserve]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static string ToString<T>(ICollection<T> collection, Func<T, string> callback)
            {
                return ToString(collection?.GetEnumerator(), callback);
            }

            /// <summary>
            /// 只读集合容器的字符串描述输出函数
            /// </summary>
            /// <typeparam name="T">数据值类型</typeparam>
            /// <param name="collection">只读集合容器对象实例</param>
            /// <param name="callback">回调句柄</param>
            /// <returns>返回只读集合容器对应的字符串输出结果</returns>
            [Preserve]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static string ToString<T>(IReadOnlyCollection<T> collection, Func<T, string> callback)
            {
                return ToString(collection?.GetEnumerator(), callback);
            }

            /// <summary>
            /// 集合容器的字符串描述输出函数
            /// </summary>
            /// <typeparam name="T">数据值类型</typeparam>
            /// <param name="collection">集合容器对象实例</param>
            /// <param name="callback">回调句柄</param>
            /// <returns>返回集合容器对应的字符串输出结果</returns>
            [Preserve]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static string ToString<T>(ICollection<T> collection, Func<int, T, string> callback = null)
            {
                return ToString(collection?.GetEnumerator(), callback);
            }

            /// <summary>
            /// 只读集合容器的字符串描述输出函数
            /// </summary>
            /// <typeparam name="T">数据值类型</typeparam>
            /// <param name="collection">只读集合容器对象实例</param>
            /// <param name="callback">回调句柄</param>
            /// <returns>返回只读集合容器对应的字符串输出结果</returns>
            [Preserve]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static string ToString<T>(IReadOnlyCollection<T> collection, Func<int, T, string> callback = null)
            {
                return ToString(collection?.GetEnumerator(), callback);
            }

            /// <summary>
            /// 迭代器的字符串描述输出函数
            /// </summary>
            /// <typeparam name="T">数据值类型</typeparam>
            /// <param name="enumerator">迭代器对象实例</param>
            /// <param name="callback">回调句柄</param>
            /// <returns>返回迭代器对应的字符串输出结果</returns>
            [Preserve]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static string ToString<T>(IEnumerator<T> enumerator, Func<T, string> callback)
            {
                return ToString(enumerator, (n, obj) =>
                {
                    return $"{n}={{{callback(obj)}}}";
                });
            }

            /// <summary>
            /// 迭代器的字符串描述输出函数
            /// </summary>
            /// <typeparam name="T">数据值类型</typeparam>
            /// <param name="enumerator">迭代器对象实例</param>
            /// <param name="callback">回调句柄</param>
            /// <returns>返回迭代器对应的字符串输出结果</returns>
            [Preserve]
            public static string ToString<T>(IEnumerator<T> enumerator, Func<int, T, string> callback = null)
            {
                if (null == enumerator)
                {
                    return Definition.CString.Null;
                }

                StringBuilder sb = new StringBuilder();

                int n = 0;
                while (enumerator.MoveNext())
                {
                    if (n > 0) sb.Append(Definition.CCharacter.Comma);

                    if (null == callback)
                    {
                        sb.AppendFormat("{0}={1}", n, enumerator.Current.ToString());
                    }
                    else
                    {
                        sb.Append(callback(n, enumerator.Current));

                    }
                    n++;
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
            [Preserve]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static string ToString<K, V>(ICollection<KeyValuePair<K, V>> collection, Func<K, V, string> callback = null)
            {
                return ToString(collection?.GetEnumerator(), callback);
            }

            /// <summary>
            /// 集合容器的字符串描述输出函数
            /// </summary>
            /// <typeparam name="K">数据键类型</typeparam>
            /// <typeparam name="V">数据值类型</typeparam>
            /// <param name="collection">集合容器对象实例</param>
            /// <param name="callback">回调句柄</param>
            /// <returns>返回集合容器对应的字符串输出结果</returns>
            [Preserve]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static string ToString<K, V>(IReadOnlyCollection<KeyValuePair<K, V>> collection, Func<K, V, string> callback = null)
            {
                return ToString(collection?.GetEnumerator(), callback);
            }

            /// <summary>
            /// 迭代器的字符串描述输出函数
            /// </summary>
            /// <typeparam name="K">数据键类型</typeparam>
            /// <typeparam name="V">数据值类型</typeparam>
            /// <param name="enumerator">迭代器对象实例</param>
            /// <param name="callback">回调句柄</param>
            /// <returns>返回迭代器对应的字符串输出结果</returns>
            [Preserve]
            public static string ToString<K, V>(IEnumerator<KeyValuePair<K, V>> enumerator, Func<K, V, string> callback = null)
            {
                if (null == enumerator)
                {
                    return Definition.CString.Null;
                }

                StringBuilder sb = new StringBuilder();

                int n = 0;
                while (enumerator.MoveNext())
                {
                    if (n > 0) sb.Append(Definition.CCharacter.Comma);

                    if (null == callback)
                    {
                        sb.AppendFormat("{0}={1}", enumerator.Current.Key.ToString(), enumerator.Current.Value.ToString());
                    }
                    else
                    {
                        sb.Append(callback(enumerator.Current.Key, enumerator.Current.Value));

                    }
                    n++;
                }

                return sb.ToString();
            }

            /// <summary>
            /// 列表容器的字符串描述输出函数
            /// </summary>
            /// <param name="list">列表容器对象实例</param>
            /// <param name="callback">输出回调句柄</param>
            /// <returns>返回列表容器对应的字符串输出结果</returns>
            [Preserve]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static string ToString(IList list, Func<object, string> callback)
            {
                return ToString(list as ICollection, callback);
            }

            /// <summary>
            /// 列表容器的字符串描述输出函数
            /// </summary>
            /// <param name="list">列表容器对象实例</param>
            /// <param name="callback">输出回调句柄</param>
            /// <returns>返回列表容器对应的字符串输出结果</returns>
            [Preserve]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static string ToString(IList list, Func<int, object, string> callback = null)
            {
                return ToString(list as ICollection, callback);
            }

            /// <summary>
            /// 列表容器的字符串描述输出函数
            /// </summary>
            /// <typeparam name="T">容器内的元素类型</typeparam>
            /// <param name="list">列表容器对象实例</param>
            /// <param name="callback">输出回调句柄</param>
            /// <returns>返回列表容器对应的字符串输出结果</returns>
            [Preserve]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static string ToString<T>(IList<T> list, Func<T, string> callback)
            {
                return ToString(list as ICollection<T>, callback);
            }

            /// <summary>
            /// 只读列表容器的字符串描述输出函数
            /// </summary>
            /// <typeparam name="T">容器内的元素类型</typeparam>
            /// <param name="list">只读列表容器对象实例</param>
            /// <param name="callback">输出回调句柄</param>
            /// <returns>返回只读列表容器对应的字符串输出结果</returns>
            [Preserve]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static string ToString<T>(IReadOnlyList<T> list, Func<T, string> callback)
            {
                return ToString(list as IReadOnlyCollection<T>, callback);
            }

            /// <summary>
            /// 列表容器的字符串描述输出函数
            /// </summary>
            /// <typeparam name="T">容器内的元素类型</typeparam>
            /// <param name="list">列表容器对象实例</param>
            /// <param name="callback">输出回调句柄</param>
            /// <returns>返回列表容器对应的字符串输出结果</returns>
            [Preserve]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static string ToString<T>(IList<T> list, Func<int, T, string> callback = null)
            {
                return ToString(list as ICollection<T>, callback);
            }

            /// <summary>
            /// 只读列表容器的字符串描述输出函数
            /// </summary>
            /// <typeparam name="T">容器内的元素类型</typeparam>
            /// <param name="list">只读列表容器对象实例</param>
            /// <param name="callback">输出回调句柄</param>
            /// <returns>返回只读列表容器对应的字符串输出结果</returns>
            [Preserve]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static string ToString<T>(IReadOnlyList<T> list, Func<int, T, string> callback = null)
            {
                return ToString(list as IReadOnlyCollection<T>, callback);
            }

            /// <summary>
            /// 集合容器的字符串描述输出函数
            /// </summary>
            /// <typeparam name="T">容器内的元素类型</typeparam>
            /// <param name="set">集合容器对象实例</param>
            /// <param name="callback">输出回调句柄</param>
            /// <returns>返回集合容器对应的字符串输出结果</returns>
            [Preserve]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static string ToString<T>(ISet<T> set, Func<T, string> callback)
            {
                return ToString(set as ICollection<T>, callback);
            }

            /// <summary>
            /// 集合容器的字符串描述输出函数
            /// </summary>
            /// <typeparam name="T">容器内的元素类型</typeparam>
            /// <param name="set">集合容器对象实例</param>
            /// <param name="callback">输出回调句柄</param>
            /// <returns>返回集合容器对应的字符串输出结果</returns>
            [Preserve]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static string ToString<T>(ISet<T> set, Func<int, T, string> callback = null)
            {
                return ToString(set as ICollection<T>, callback);
            }

            /// <summary>
            /// 列表容器的字符串描述输出函数
            /// </summary>
            /// <param name="list">列表容器对象实例</param>
            /// <param name="callback">输出回调句柄</param>
            /// <returns>返回列表容器对应的字符串输出结果</returns>
            [Preserve]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static string ToString(IList<int> list, Func<int, string> callback)
            {
                return ToString<int>(list, callback);
            }

            /// <summary>
            /// 只读列表容器的字符串描述输出函数
            /// </summary>
            /// <param name="list">只读列表容器对象实例</param>
            /// <param name="callback">输出回调句柄</param>
            /// <returns>返回只读列表容器对应的字符串输出结果</returns>
            [Preserve]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static string ToString(IReadOnlyList<int> list, Func<int, string> callback)
            {
                return ToString<int>(list, callback);
            }

            /// <summary>
            /// 列表容器的字符串描述输出函数
            /// </summary>
            /// <param name="list">列表容器对象实例</param>
            /// <param name="callback">输出回调句柄</param>
            /// <returns>返回列表容器对应的字符串输出结果</returns>
            [Preserve]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static string ToString(IList<int> list, Func<int, int, string> callback = null)
            {
                return ToString<int>(list, callback);
            }

            /// <summary>
            /// 只读列表容器的字符串描述输出函数
            /// </summary>
            /// <param name="list">只读列表容器对象实例</param>
            /// <param name="callback">输出回调句柄</param>
            /// <returns>返回只读列表容器对应的字符串输出结果</returns>
            [Preserve]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static string ToString(IReadOnlyList<int> list, Func<int, int, string> callback = null)
            {
                return ToString<int>(list, callback);
            }

            /// <summary>
            /// 列表容器的字符串描述输出函数
            /// </summary>
            /// <param name="list">列表容器对象实例</param>
            /// <param name="callback">输出回调句柄</param>
            /// <returns>返回列表容器对应的字符串输出结果</returns>
            [Preserve]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static string ToString(IList<string> list, Func<string, string> callback)
            {
                return ToString<string>(list, callback);
            }

            /// <summary>
            /// 只读列表容器的字符串描述输出函数
            /// </summary>
            /// <param name="list">只读列表容器对象实例</param>
            /// <param name="callback">输出回调句柄</param>
            /// <returns>返回只读列表容器对应的字符串输出结果</returns>
            [Preserve]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static string ToString(IReadOnlyList<string> list, Func<string, string> callback)
            {
                return ToString<string>(list, callback);
            }

            /// <summary>
            /// 列表容器的字符串描述输出函数
            /// </summary>
            /// <param name="list">列表容器对象实例</param>
            /// <param name="callback">输出回调句柄</param>
            /// <returns>返回列表容器对应的字符串输出结果</returns>
            [Preserve]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static string ToString(IList<string> list, Func<int, string, string> callback = null)
            {
                return ToString<string>(list, callback);
            }

            /// <summary>
            /// 只读列表容器的字符串描述输出函数
            /// </summary>
            /// <param name="list">只读列表容器对象实例</param>
            /// <param name="callback">输出回调句柄</param>
            /// <returns>返回只读列表容器对应的字符串输出结果</returns>
            [Preserve]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static string ToString(IReadOnlyList<string> list, Func<int, string, string> callback = null)
            {
                return ToString<string>(list, callback);
            }

            /// <summary>
            /// 字典容器的字符串描述输出函数
            /// </summary>
            /// <param name="dictionary">字典容器对象实例</param>
            /// <param name="callback">输出回调句柄</param>
            /// <returns>返回字典容器对应的字符串输出结果</returns>
            [Preserve]
            public static string ToString(IDictionary dictionary, Func<object, object, string> callback = null)
            {
                if (null == callback)
                {
                    return ToString(dictionary as ICollection);
                }

                return ToString(dictionary as ICollection, (n, obj) =>
                {
                    if (obj is DictionaryEntry entry)
                    {
                        return callback(entry.Key, entry.Value);
                    }

                    Type targetType = obj.GetType();

                    if (targetType.IsGenericType && typeof(KeyValuePair<,>) == targetType.GetGenericTypeDefinition())
                    {
                        PropertyInfo getKeyPropertyInfo = targetType.GetProperty("Key");
                        PropertyInfo getValuePropertyInfo = targetType.GetProperty("Value");

                        Logger.Assert(null != getKeyPropertyInfo && null != getValuePropertyInfo, "Invalid arguments.");
                        return callback(getKeyPropertyInfo.GetValue(obj), getValuePropertyInfo.GetValue(obj));
                    }

                    Logger.Warn("Resolve dictionary value type '{%t}' failed.", targetType);
                    return callback(obj, obj);
                });
            }

            /// <summary>
            /// 字典容器的字符串描述输出函数
            /// </summary>
            /// <typeparam name="K">字典映射的键类型</typeparam>
            /// <typeparam name="V">字典映射的值类型</typeparam>
            /// <param name="dictionary">字典容器对象实例</param>
            /// <param name="callback">输出回调句柄</param>
            /// <returns>返回字典容器对应的字符串输出结果</returns>
            [Preserve]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static string ToString<K, V>(IDictionary<K, V> dictionary, Func<K, V, string> callback = null)
            {
                return ToString(dictionary as ICollection<KeyValuePair<K, V>>, callback);
            }

            /// <summary>
            /// 只读字典容器的字符串描述输出函数
            /// </summary>
            /// <typeparam name="K">字典映射的键类型</typeparam>
            /// <typeparam name="V">字典映射的值类型</typeparam>
            /// <param name="dictionary">只读字典容器对象实例</param>
            /// <param name="callback">输出回调句柄</param>
            /// <returns>返回只读字典容器对应的字符串输出结果</returns>
            [Preserve]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static string ToString<K, V>(IReadOnlyDictionary<K, V> dictionary, Func<K, V, string> callback = null)
            {
                return ToString(dictionary as IReadOnlyCollection<KeyValuePair<K, V>>, callback);
            }

            ////////////////////////////////////////////////////////////////////////////////////////////////////

            /// <summary>
            /// 列表容器的字符串描述输出函数
            /// </summary>
            /// <typeparam name="T">容器内的元素类型</typeparam>
            /// <param name="list">列表容器对象实例</param>
            /// <param name="callback">输出回调句柄</param>
            /// <returns>返回列表容器对应的字符串输出结果</returns>
            [Preserve]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static string ToString<T>(List<T> list, Func<T, string> callback)
            {
                return ToString((IReadOnlyList<T>) list, callback);
            }

            /// <summary>
            /// 列表容器的字符串描述输出函数
            /// </summary>
            /// <typeparam name="T">容器内的元素类型</typeparam>
            /// <param name="list">列表容器对象实例</param>
            /// <param name="callback">输出回调句柄</param>
            /// <returns>返回列表容器对应的字符串输出结果</returns>
            [Preserve]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static string ToString<T>(List<T> list, Func<int, T, string> callback = null)
            {
                return ToString((IReadOnlyList<T>) list, callback);
            }

            /// <summary>
            /// 集合容器的字符串描述输出函数
            /// </summary>
            /// <typeparam name="T">容器内的元素类型</typeparam>
            /// <param name="set">集合容器对象实例</param>
            /// <param name="callback">输出回调句柄</param>
            /// <returns>返回集合容器对应的字符串输出结果</returns>
            [Preserve]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static string ToString<T>(HashSet<T> set, Func<T, string> callback)
            {
                return ToString((IReadOnlyCollection<T>) set, callback);
            }

            /// <summary>
            /// 集合容器的字符串描述输出函数
            /// </summary>
            /// <typeparam name="T">容器内的元素类型</typeparam>
            /// <param name="set">集合容器对象实例</param>
            /// <param name="callback">输出回调句柄</param>
            /// <returns>返回集合容器对应的字符串输出结果</returns>
            [Preserve]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static string ToString<T>(HashSet<T> set, Func<int, T, string> callback = null)
            {
                return ToString((IReadOnlyCollection<T>) set, callback);
            }

            /// <summary>
            /// 队列容器的字符串描述输出函数
            /// </summary>
            /// <typeparam name="T">容器内的元素类型</typeparam>
            /// <param name="queue">队列容器对象实例</param>
            /// <param name="callback">输出回调句柄</param>
            /// <returns>返回队列容器对应的字符串输出结果</returns>
            [Preserve]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static string ToString<T>(Queue<T> queue, Func<T, string> callback)
            {
                return ToString((IReadOnlyCollection<T>) queue, callback);
            }

            /// <summary>
            /// 队列容器的字符串描述输出函数
            /// </summary>
            /// <typeparam name="T">容器内的元素类型</typeparam>
            /// <param name="queue">队列容器对象实例</param>
            /// <param name="callback">输出回调句柄</param>
            /// <returns>返回队列容器对应的字符串输出结果</returns>
            [Preserve]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static string ToString<T>(Queue<T> queue, Func<int, T, string> callback = null)
            {
                return ToString((IReadOnlyCollection<T>) queue, callback);
            }

            /// <summary>
            /// 堆栈容器的字符串描述输出函数
            /// </summary>
            /// <typeparam name="T">数据值类型</typeparam>
            /// <param name="stack">堆栈容器对象实例</param>
            /// <param name="callback">回调句柄</param>
            /// <returns>返回堆栈容器对应的字符串输出结果</returns>
            [Preserve]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static string ToString<T>(Stack<T> stack, Func<T, string> callback)
            {
                return ToString((IReadOnlyCollection<T>) stack, callback);
            }

            /// <summary>
            /// 堆栈容器的字符串描述输出函数
            /// </summary>
            /// <typeparam name="T">数据值类型</typeparam>
            /// <param name="stack">堆栈容器对象实例</param>
            /// <param name="callback">回调句柄</param>
            /// <returns>返回堆栈容器对应的字符串输出结果</returns>
            [Preserve]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static string ToString<T>(Stack<T> stack, Func<int, T, string> callback = null)
            {
                return ToString((IReadOnlyCollection<T>) stack, callback);
            }

            /// <summary>
            /// 字典容器的字符串描述输出函数
            /// </summary>
            /// <typeparam name="K">字典映射的键类型</typeparam>
            /// <typeparam name="V">字典映射的值类型</typeparam>
            /// <param name="dictionary">字典容器对象实例</param>
            /// <param name="callback">输出回调句柄</param>
            /// <returns>返回字典容器对应的字符串输出结果</returns>
            [Preserve]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static string ToString<K, V>(Dictionary<K, V> dictionary, Func<K, V, string> callback)
            {
                return ToString((IReadOnlyDictionary<K, V>) dictionary, callback);
            }

            #endregion
        }
    }
}
