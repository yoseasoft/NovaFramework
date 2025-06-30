/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
/// Copyring (C) 2022 - 2023, Shanghai Bilibili Technology Co., Ltd.
/// Copyring (C) 2023 - 2024, Guangzhou Shiyue Network Technology Co., Ltd.
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

using SystemType = System.Type;
using SystemAttribute = System.Attribute;
using SystemDelegate = System.Delegate;
using SystemIntPtr = System.IntPtr;
using SystemStringBuilder = System.Text.StringBuilder;
using SystemRegex = System.Text.RegularExpressions.Regex;
using SystemMatch = System.Text.RegularExpressions.Match;
using SystemMatchCollection = System.Text.RegularExpressions.MatchCollection;
using SystemFieldInfo = System.Reflection.FieldInfo;
using SystemPropertyInfo = System.Reflection.PropertyInfo;
using SystemMethodBase = System.Reflection.MethodBase;
using SystemMethodInfo = System.Reflection.MethodInfo;
using SystemParameterInfo = System.Reflection.ParameterInfo;
using SystemGCHandle = System.Runtime.InteropServices.GCHandle;
using SystemGCHandleType = System.Runtime.InteropServices.GCHandleType;

namespace NovaEngine
{
    /// <summary>
    /// 实用函数集合工具类
    /// </summary>
    public static partial class Utility
    {
        /// <summary>
        /// 字符串相关实用函数集合
        /// </summary>
        public static class Text
        {
            [System.ThreadStatic]
            private static SystemStringBuilder _cachedStringBuilder = new SystemStringBuilder(4096);

            /// <summary>
            /// 使用指定的格式及参数获取对应的格式化字符串
            /// </summary>
            /// <param name="format">字符串格式</param>
            /// <param name="args">字符串参数</param>
            /// <returns>返回格式化后的字符串</returns>
            public static string Format(string format, params object[] args)
            {
                return TextFormatConvertionProcess(format, args);
            }

            /// <summary>
            /// 使用指定的格式及参数获取对应的格式化字符串，并存放到目标缓冲区中
            /// </summary>
            /// <param name="buff">目标缓冲区</param>
            /// <param name="format">字符串格式</param>
            /// <param name="args">字符串参数</param>
            public static void FormatToBuffer(SystemStringBuilder buff, string format, params object[] args)
            {
                if (buff == null)
                {
                    throw new CFrameworkException("Invalid arguments.");
                }

                buff.Length = 0;
                buff.Append(TextFormatConvertionProcess(format, args));
            }

            #region 自定义格式化解析逻辑封装接口函数

            /// <summary>
            /// 文本格式的参数类型转换回调函数声明
            /// </summary>
            /// <param name="obj">目标参数</param>
            /// <returns>返回转换后的参数对象实例</returns>
            private delegate object TextFormatParameterConvertionCallback(object obj);

            /// <summary>
            /// 文本格式的参数类型分类的枚举定义
            /// </summary>
            private enum TextFormatParameterType
            {
                Unknown,
                Digit,
                String,
                Object,
                ObjectPtr,
                ObjectType,
                ObjectInfo,
                ClassType,
                Verbose,
                Max,
            }

            /// <summary>
            /// 文本格式转换的处理信息数据结构定义<br/>
            /// 用于记录参数类型所对应的解析标识符和处理回调函数
            /// </summary>
            private struct TextFormatConvertionInfo
            {
                public TextFormatParameterType parameterType;
                public string formatSymbol;
                public TextFormatParameterConvertionCallback convertionCallback;
            }

            /// <summary>
            /// 文本格式转换的参数默认分隔符
            /// </summary>
            private const string TEXT_FORMAT_CONVERTION_ARGUMENTS_SEPARATOR = "    ";

            /// <summary>
            /// 文本格式转换的处理信息对象集合
            /// </summary>
            private static TextFormatConvertionInfo[] s_textFormatConvertionInfos = new TextFormatConvertionInfo[(int) TextFormatParameterType.Max]
            {
            new TextFormatConvertionInfo {
                                            parameterType = TextFormatParameterType.Unknown,
                                            formatSymbol = string.Empty,
                                            convertionCallback = null,
                                        },
            new TextFormatConvertionInfo {
                                            parameterType = TextFormatParameterType.Digit,
                                            formatSymbol = "d",
                                            convertionCallback = _TextFormatConvertionCallback_BasicType,
                                        },
            new TextFormatConvertionInfo {
                                            parameterType = TextFormatParameterType.String,
                                            formatSymbol = "s",
                                            convertionCallback = _TextFormatConvertionCallback_BasicType,
                                        },
            new TextFormatConvertionInfo {
                                            parameterType = TextFormatParameterType.Object,
                                            formatSymbol = "o",
                                            convertionCallback = _TextFormatConvertionCallback_Object,
                                        },
            new TextFormatConvertionInfo {
                                            parameterType = TextFormatParameterType.ObjectPtr,
                                            formatSymbol = "p",
                                            convertionCallback = _TextFormatConvertionCallback_ObjectPtr,
                                        },
            new TextFormatConvertionInfo {
                                            parameterType = TextFormatParameterType.ObjectType,
                                            formatSymbol = "t",
                                            convertionCallback = _TextFormatConvertionCallback_ObjectType,
                                        },
            new TextFormatConvertionInfo {
                                            parameterType = TextFormatParameterType.ObjectInfo,
                                            formatSymbol = "i",
                                            convertionCallback = _TextFormatConvertionCallback_ObjectInfo,
                                        },
            new TextFormatConvertionInfo {
                                            parameterType = TextFormatParameterType.ClassType,
                                            formatSymbol = "f",
                                            convertionCallback = _TextFormatConvertionCallback_ClassType,
                                        },
            new TextFormatConvertionInfo {
                                            parameterType = TextFormatParameterType.Verbose,
                                            formatSymbol = "v",
                                            convertionCallback = _TextFormatConvertionCallback_Verbose,
                                        },
            };

            /// <summary>
            /// 通过指定的解析标识符，查找对应的格式转换参数类型
            /// </summary>
            /// <param name="symbolName">解析标识符</param>
            /// <returns>返回格式转换的参数类型，若解析标识符非法，则返回Unknown类型</returns>
            private static TextFormatParameterType GetTextFormatParameterTypeBySymbolName(string symbolName)
            {
                if (string.IsNullOrEmpty(symbolName))
                {
                    return TextFormatParameterType.Unknown;
                }

                // 转换为小写字符
                symbolName = symbolName.ToLower();

                for (int n = 0; n < s_textFormatConvertionInfos.Length; ++n)
                {
                    if (symbolName.Equals(s_textFormatConvertionInfos[n].formatSymbol))
                    {
                        return s_textFormatConvertionInfos[n].parameterType;
                    }
                }

                return TextFormatParameterType.Unknown;
            }

            /// <summary>
            /// 对指定的格式文本及参数列表进行格式化处理的接口函数
            /// </summary>
            /// <param name="text">格式文本</param>
            /// <param name="args">参数列表</param>
            /// <returns>返回格式化处理后的文本字符串</returns>
            /// <exception cref="CFrameworkException">格式文件解析异常</exception>
            private static string TextFormatConvertionProcess(string text, params object[] args)
            {
                // 格式内容不能为空
                if (string.IsNullOrEmpty(text))
                {
                    return string.Empty;
                }

                const string format_pattern = @"\{([^\{\}]+)\}";
                // const string digit_pattern = @"(\d+)";

                SystemMatchCollection matches = SystemRegex.Matches(text, format_pattern);

                // 若没有需要转换的格式化参数，则直接返回文本内容
                if (matches.Count <= 0)
                {
                    return text;
                }

                // 需要格式化的参数和实际传入的参数不一致
                if (null == args || matches.Count > args.Length)
                {
                    throw new CFrameworkException("The arguments length '{0}' must be great than format parameter match count '{1}'.", args?.Length, matches?.Count);
                }

                object[] parameters = new object[args.Length];

                SystemStringBuilder sb = new SystemStringBuilder();
                int pos = 0;
                int index = 0;
                for (index = 0; index < matches.Count; ++index)
                {
                    SystemMatch match = matches[index];

                    sb.Append(text.Substring(pos, match.Index - pos));
                    pos = match.Index + match.Value.Length;

                    // 大括号内为空
                    if (match.Value.Length <= 2)
                    {
                        throw new CFrameworkException("Invalid format parameter type '{0}' within text context '{1}'.", match.Value, text);
                    }

                    string substr = match.Value.Substring(1, match.Value.Length - 2);
                    // bool is_digit = SystemRegex.IsMatch(substr, digit_pattern);
                    // 数字类型
                    if (int.TryParse(substr, out int num_value))
                    {
                        if (num_value != index)
                        {
                            throw new CFrameworkException("The convertion index '{0}' doesnot match format location '{1}' within text context '{2}'.", num_value, index, text);
                        }

                        parameters[index] = args[index];

                        sb.Append(match.Value);
                        continue;
                    }

                    string symbol_name = null;
                    if (substr.Length > 1 && Definition.CCharacter.Percent == substr[0])
                    {
                        symbol_name = substr.Substring(1);
                    }

                    TextFormatParameterType parameterType = GetTextFormatParameterTypeBySymbolName(symbol_name);
                    if (TextFormatParameterType.Unknown == parameterType)
                    {
                        throw new CFrameworkException("Invalid format parameter type '{0}' within text \"{1}\" position '{2}'.", substr, text, match.Index);
                    }

                    TextFormatConvertionInfo convertionInfo = s_textFormatConvertionInfos[(int) parameterType];
                    parameters[index] = convertionInfo.convertionCallback(args[index]);

                    sb.Append($"{{{index}}}");
                }

                // 将最后一个格式化参数之后的文本内容添加到队列中
                if (text.Length > pos)
                {
                    sb.Append(text.Substring(pos));
                }

                // 传入的实际参数个数超过格式化匹配的参数个数，
                // 则将多余参数以字符串形式追加至末尾输出
                while (index < args.Length)
                {
                    sb.Append($"{TEXT_FORMAT_CONVERTION_ARGUMENTS_SEPARATOR}{{{index}}}");
                    parameters[index] = args[index].ToString();
                    index++;
                }

                // UnityEngine.Debug.LogWarning($"the convertion text content = \"{sb.ToString()}\" and arguments length = \"{parameters.Length}\".");

                _cachedStringBuilder.Length = 0;
                _cachedStringBuilder.AppendFormat(sb.ToString(), parameters);
                return _cachedStringBuilder.ToString();
            }

            private static object _TextFormatConvertionCallback_BasicType(object obj)
            {
                // SystemType targetType = obj?.GetType();
                // 基础类型仅包含数值类型
                // if (null != targetType && targetType.IsPrimitive && targetType != typeof(bool) && targetType != typeof(char))

                // 包含所有基础类型
                // if (null == targetType || false == targetType.IsPrimitive) { return Definition.CString.Null; }

                return obj;
            }

            private static object _TextFormatConvertionCallback_Object(object obj)
            {
                if (null == obj)
                {
                    return Definition.CString.Null;
                }

                return obj.ToString();
            }

            private static object _TextFormatConvertionCallback_ObjectPtr(object obj)
            {
                if (null == obj)
                {
                    return 0L;
                }

                SystemGCHandle handle = SystemGCHandle.Alloc(obj, SystemGCHandleType.Pinned);
                SystemIntPtr address = SystemGCHandle.ToIntPtr(handle);
                handle.Free();

                return address.ToInt64();
            }

            private static object _TextFormatConvertionCallback_ObjectType(object obj)
            {
                if (null == obj)
                {
                    return Definition.CString.Null;
                }

                return GetFullName(obj.GetType());
            }

            private static object _TextFormatConvertionCallback_ObjectInfo(object obj)
            {
                if (null == obj)
                {
                    return Definition.CString.Null;
                }

                return obj.ToString();
            }

            private static object _TextFormatConvertionCallback_ClassType(object obj)
            {
                if (null == obj)
                {
                    return Definition.CString.Null;
                }

                if (obj is SystemType targetType)
                {
                    return GetFullName(targetType);
                }
                else if (obj is SystemAttribute attribute)
                {
                    return GetFullName(attribute);
                }
                else if (obj is SystemFieldInfo fieldInfo)
                {
                    return GetFullName(fieldInfo);
                }
                else if (obj is SystemPropertyInfo propertyInfo)
                {
                    return GetFullName(propertyInfo);
                }
                else if (obj is SystemDelegate callback)
                {
                    return GetFullName(callback);
                }
                else if (obj is SystemMethodInfo methodInfo)
                {
                    return GetFullName(methodInfo);
                }
                else if (obj is SystemParameterInfo parameterInfo)
                {
                    return GetFullName(parameterInfo);
                }

                throw new CFrameworkException("Invalid format convertion class type '{%s}'.", GetFullName(obj.GetType()));
            }

            private static object _TextFormatConvertionCallback_Verbose(object obj)
            {
                if (null == obj)
                {
                    return Definition.CString.Null;
                }

                SystemType targetType = obj.GetType();
                if (typeof(System.Collections.IList).IsAssignableFrom(targetType))
                {
                    return ToString(obj as System.Collections.IList);
                }
                else if (typeof(System.Collections.IDictionary).IsAssignableFrom(targetType))
                {
                    return ToString(obj as System.Collections.IDictionary);
                }

                throw new CFrameworkException("Invalid format convertion enumerable type '{%s}'.", GetFullName(obj.GetType()));
            }

            #endregion

            #region 对象的字符串输出封装接口函数

            /// <summary>
            /// 对象类型的字符串描述输出函数
            /// </summary>
            /// <param name="classType">对象类型</param>
            /// <returns>返回对象类型对应的字符串输出结果</returns>
            public static string ToString(SystemType classType)
            {
                return null == classType ? Definition.CString.Null : GetFullName(classType);
            }

            /// <summary>
            /// 特性对象的字符串描述输出函数
            /// </summary>
            /// <param name="attribute">特性对象</param>
            /// <returns>返回特性对象对应的字符串输出结果</returns>
            public static string ToString(SystemAttribute attribute)
            {
                return null == attribute ? Definition.CString.Null : GetFullName(attribute);
            }

            /// <summary>
            /// 字段对象的字符串描述输出函数
            /// </summary>
            /// <param name="field">字段对象</param>
            /// <returns>返回字段对象对应的字符串输出结果</returns>
            public static string ToString(SystemFieldInfo field)
            {
                return null == field ? Definition.CString.Null : GetFullName(field);
            }

            /// <summary>
            /// 属性对象的字符串描述输出函数
            /// </summary>
            /// <param name="property">属性对象</param>
            /// <returns>返回属性对象对应的字符串输出结果</returns>
            public static string ToString(SystemPropertyInfo property)
            {
                return null == property ? Definition.CString.Null : GetFullName(property);
            }

            /// <summary>
            /// 委托回调的字符串描述输出函数
            /// </summary>
            /// <param name="callback">委托回调</param>
            /// <returns>返回委托回调对应的字符串输出结果</returns>
            public static string ToString(SystemDelegate callback)
            {
                return null == callback ? Definition.CString.Null : GetFullName(callback);
            }

            /// <summary>
            /// 函数对象的字符串描述输出函数
            /// </summary>
            /// <param name="method">函数对象</param>
            /// <returns>返回函数对象对应的字符串输出结果</returns>
            public static string ToString(SystemMethodBase method)
            {
                return null == method ? Definition.CString.Null : GetFullName(method);
            }

            /// <summary>
            /// 参数对象的字符串描述输出函数
            /// </summary>
            /// <param name="parameter">参数对象</param>
            /// <returns>返回参数对象对应的字符串输出结果</returns>
            public static string ToString(SystemParameterInfo parameter)
            {
                return null == parameter ? Definition.CString.Null : GetFullName(parameter);
            }

            /// <summary>
            /// 返回指定类型的全名字符串信息
            /// </summary>
            /// <param name="targetType">对象类型</param>
            /// <returns>返回字符串信息</returns>
            public static string GetFullName(SystemType targetType)
            {
                return targetType.FullName;
            }

            /// <summary>
            /// 返回指定特性的全名字符串信息
            /// </summary>
            /// <param name="attribute">特性对象</param>
            /// <returns>返回字符串信息</returns>
            public static string GetFullName(SystemAttribute attribute)
            {
                return attribute.GetType().FullName;
            }

            /// <summary>
            /// 返回指定字段的全名字符串信息
            /// </summary>
            /// <param name="field">字段对象</param>
            /// <returns>返回字符串信息</returns>
            public static string GetFullName(SystemFieldInfo field)
            {
                SystemStringBuilder stringBuilder = new SystemStringBuilder();

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
            public static string GetFullName(SystemPropertyInfo property)
            {
                SystemStringBuilder stringBuilder = new SystemStringBuilder();

                stringBuilder.Append(GetFullName(property.PropertyType));
                stringBuilder.Append(Definition.CCharacter.Space);
                stringBuilder.Append(property.Name);
                stringBuilder.Append(Definition.CCharacter.LeftParen);
                stringBuilder.Append(property.DeclaringType.FullName);
                stringBuilder.Append(Definition.CCharacter.RightParen);
                stringBuilder.Append(Definition.CCharacter.Space);

                stringBuilder.Append(Definition.CCharacter.LeftBracket);
                SystemMethodInfo getMethodInfo = property.GetGetMethod(true);
                if (null != getMethodInfo)
                {
                    stringBuilder.Append(GetFullName(getMethodInfo));
                }

                SystemMethodInfo setMethodInfo = property.GetSetMethod(true);
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
            public static string GetFullName(SystemDelegate callback)
            {
                SystemStringBuilder stringBuilder = new SystemStringBuilder();

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
            public static string GetFullName(SystemMethodBase method)
            {
                SystemStringBuilder stringBuilder = new SystemStringBuilder();

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
            private static string __GetFullName(SystemMethodBase method)
            {
                SystemStringBuilder stringBuilder = new SystemStringBuilder();

                stringBuilder.Append(method.Name);
                if (method.IsGenericMethod)
                {
                    SystemType[] genericArguments = method.GetGenericArguments();
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
            private static string __GetMethodParamsNames(SystemMethodBase method)
            {
                SystemParameterInfo[] array = (Reflection.IsTypeOfExtension(method) ? Collection.SkipAndToArray<SystemParameterInfo>(method.GetParameters(), 1) : method.GetParameters());
                SystemStringBuilder stringBuilder = new SystemStringBuilder();
                for (int n = 0, num = array.Length; n < num; ++n)
                {
                    if (n > 0)
                    {
                        stringBuilder.Append(", ");
                    }

                    SystemParameterInfo parameterInfo = array[n];

                    stringBuilder.Append(__GetFullName(parameterInfo));
                }

                return stringBuilder.ToString();
            }

            /// <summary>
            /// 返回指定参数的全名字符串信息
            /// </summary>
            /// <param name="parameter">参数对象</param>
            /// <returns>返回字符串信息</returns>
            public static string GetFullName(SystemParameterInfo parameter)
            {
                return __GetFullName(parameter);
            }

            /// <summary>
            /// 返回指定参数的全名字符串信息
            /// </summary>
            /// <param name="parameter">参数对象</param>
            /// <returns>返回字符串信息</returns>
            private static string __GetFullName(SystemParameterInfo parameter)
            {
                SystemStringBuilder stringBuilder = new SystemStringBuilder();

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
            /// <typeparam name="T">容器内的元素类型</typeparam>
            /// <param name="array">数组容器对象实例</param>
            /// <returns>返回数组容器对应的字符串输出结果</returns>
            public static string ToString<T>(T[] array)
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

                        sb.AppendFormat("[{0}]={1}", n, array[n].ToString());
                    }
                }

                return sb.ToString();
            }

            /// <summary>
            /// 列表容器的字符串描述输出函数
            /// </summary>
            /// <param name="list">列表容器对象实例</param>
            /// <returns>返回列表容器对应的字符串输出结果</returns>
            private static string ToString(System.Collections.IList list)
            {
                // return "[" + string.Join(',', list) + "]";
                SystemStringBuilder sb = new SystemStringBuilder();

                if (null == list)
                {
                    sb.Append(Definition.CString.Null);
                }
                else
                {
                    for (int n = 0; n < list.Count; ++n)
                    {
                        if (n > 0) sb.Append(Definition.CCharacter.Comma);

                        sb.AppendFormat("[{0}]={1}", n, list[n].ToString());
                    }
                }

                return sb.ToString();
            }

            /// <summary>
            /// 列表容器的字符串描述输出函数
            /// </summary>
            /// <typeparam name="T">容器内的元素类型</typeparam>
            /// <param name="list">列表容器对象实例</param>
            /// <returns>返回列表容器对应的字符串输出结果</returns>
            public static string ToString<T>(IList<T> list)
            {
                return ToString(list as System.Collections.IList);
            }

            /// <summary>
            /// 列表容器的字符串描述输出函数
            /// </summary>
            /// <param name="list">列表容器对象实例</param>
            /// <returns>返回列表容器对应的字符串输出结果</returns>
            public static string ToString(IList<int> list)
            {
                return ToString<int>(list);
            }

            /// <summary>
            /// 列表容器的字符串描述输出函数
            /// </summary>
            /// <param name="list">列表容器对象实例</param>
            /// <returns>返回列表容器对应的字符串输出结果</returns>
            public static string ToString(IList<string> list)
            {
                return ToString<string>(list);
            }

            /// <summary>
            /// 字典容器的字符串描述输出函数
            /// </summary>
            /// <param name="dictionary">字典容器对象实例</param>
            /// <returns>返回字典容器对应的字符串输出结果</returns>
            private static string ToString(System.Collections.IDictionary dictionary)
            {
                SystemStringBuilder sb = new SystemStringBuilder();
                if (null == dictionary || dictionary.Count <= 0)
                {
                    sb.Append(Definition.CString.Null);
                }
                else
                {
                    System.Collections.IDictionaryEnumerator e = dictionary.GetEnumerator();
                    int c = 0;
                    while (e.MoveNext())
                    {
                        if (c > 0) sb.Append(Definition.CCharacter.Comma);

                        sb.AppendFormat("[{0}]={1}", e.Key.ToString(), e.Value.ToString());

                        ++c;
                    }
                }

                return sb.ToString();
            }

            /// <summary>
            /// 字典容器的字符串描述输出函数
            /// </summary>
            /// <typeparam name="K">字典映射的键类型</typeparam>
            /// <typeparam name="V">字典映射的值类型</typeparam>
            /// <param name="dictionary">字典容器对象实例</param>
            /// <returns>返回字典容器对应的字符串输出结果</returns>
            public static string ToString<K, V>(IDictionary<K, V> dictionary)
            {
                return ToString(dictionary as System.Collections.IDictionary);
            }

            #endregion
        }
    }
}
