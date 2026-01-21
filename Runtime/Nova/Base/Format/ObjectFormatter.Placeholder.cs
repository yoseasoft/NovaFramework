/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
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

using System;
using System.Customize.Extension;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;

namespace NovaEngine
{
    /// 格式化接口集合工具类
    internal static partial class ObjectFormatter
    {
        private const char Placeholder_B = 'b'; // 用于输出布尔值
        private const char Placeholder_D = 'd'; // 用于输出十进制整数
        private const char Placeholder_O = 'o'; // 用于输出八进制整数
        private const char Placeholder_X = 'x'; // 用于输出十六进制整数
        private const char Placeholder_F = 'f'; // 用于输出普通浮点数
        private const char Placeholder_E = 'e'; // 用于输出科学计数法的浮点数
        private const char Placeholder_C = 'c'; // 用于输出单个字符
        private const char Placeholder_S = 's'; // 用于输出字符串
        private const char Placeholder_P = 'p'; // 用于输出对象的内存指针地址
        private const char Placeholder_T = 't'; // 用于输出对象类型
        private const char Placeholder_I = 'i'; // 用于输出对象的详细信息

        /// <summary>
        /// 文本格式的参数类型分类的枚举定义
        /// </summary>
        private enum TextFormatParameterType
        {
            Unknown,
            Boolean,
            DecimalInteger,
            OctalInteger,
            HexadecimalInteger,
            CommonFloat,
            ScientificNotationFloat,
            Character,
            String,
            ObjectPointer,
            ObjectType,
            ObjectInfo,
            Max,
        }

        /// <summary>
        /// 文本格式的参数类型转换回调函数声明
        /// </summary>
        /// <param name="obj">目标参数</param>
        /// <returns>返回转换后的参数对象实例</returns>
        private delegate string TextFormatParameterConvertionCallback(object obj);

        [ThreadStatic]
        private static StringBuilder _cachedStringBuilder = new StringBuilder(4096);

        #region 文本格式的参数类型解析及转换处理接口函数定义

        /// <summary>
        /// 文本格式转换的参数默认分隔符
        /// </summary>
        private const string TEXT_FORMAT_CONVERTION_ARGUMENTS_SEPARATOR = " ";

        /// <summary>
        /// 通过指定的解析标识符，查找对应的格式转换参数类型
        /// </summary>
        /// <param name="symbolName">解析标识符</param>
        /// <returns>返回格式转换的参数类型，若解析标识符非法，则返回Unknown类型</returns>
        private static TextFormatParameterType GetTextFormatParameterTypeBySymbolName(string symbolName)
        {
            if (symbolName.IsNullOrEmpty())
            {
                return TextFormatParameterType.Unknown;
            }

            // 转换为小写字符
            symbolName = symbolName.ToLower();

            for (int n = 0; n < _textFormatConvertionInfos.Length; ++n)
            {
                if (_textFormatConvertionInfos[n].parameterType != TextFormatParameterType.Unknown &&
                    _textFormatConvertionInfos[n].formatSymbol == char.Parse(symbolName))
                {
                    return _textFormatConvertionInfos[n].parameterType;
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
        public static string TextFormatConvertionProcess(string text, params object[] args)
        {
            // 格式内容不能为空
            if (text.IsNullOrEmpty())
            {
                return string.Empty;
            }

            const string format_pattern = @"\{([^\{\}]+)\}";
            // const string digit_pattern = @"(\d+)";

            MatchCollection matches = Regex.Matches(text, format_pattern);

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

            StringBuilder sb = new StringBuilder();
            int pos = 0;
            int index;
            for (index = 0; index < matches.Count; ++index)
            {
                Match match = matches[index];

                sb.Append(text.Substring(pos, match.Index - pos));
                pos = match.Index + match.Value.Length;

                // 大括号内为空
                if (match.Value.Length <= 2)
                {
                    throw new CFrameworkException("Invalid format parameter type '{0}' within text context '{1}'.", match.Value, text);
                }

                string substr = match.Value.Substring(1, match.Value.Length - 2);
                // bool is_digit = Regex.IsMatch(substr, digit_pattern);
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

                if (null == args[index])
                {
                    parameters[index] = Definition.CString.Null;
                }
                else
                {
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

                    TextFormatConvertionInfo convertionInfo = _textFormatConvertionInfos[(int) parameterType];
                    parameters[index] = convertionInfo.convertionCallback(args[index]);
                }

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
                parameters[index] = ToString(args[index]);
                index++;
            }

            // UnityEngine.Debug.LogWarning($"替换掉占位符后的格式化文本串：\"{sb.ToString()}\"，传入的格式化参数数量为：\"{parameters.Length}\"。");

            _cachedStringBuilder.Length = 0;
            _cachedStringBuilder.AppendFormat(sb.ToString(), parameters);
            return _cachedStringBuilder.ToString();
        }

        #endregion

        #region 文本格式的参数类型转换回调函数针对不同参数类型的具体实现

        /// <summary>
        /// 文本格式转换的处理信息数据结构定义<br/>
        /// 用于记录参数类型所对应的解析标识符和处理回调函数
        /// </summary>
        private struct TextFormatConvertionInfo
        {
            public TextFormatParameterType parameterType;
            public char formatSymbol;
            public TextFormatParameterConvertionCallback convertionCallback;
        }

        /// <summary>
        /// 文本格式转换的处理信息对象集合
        /// </summary>
        private static TextFormatConvertionInfo[] _textFormatConvertionInfos = new TextFormatConvertionInfo[(int) TextFormatParameterType.Max]
        {
            new ()
            {
                parameterType = TextFormatParameterType.Unknown,
                formatSymbol = Definition.CCharacter.Nil,
                convertionCallback = null,
            },
            new ()
            {
                parameterType = TextFormatParameterType.Boolean,
                formatSymbol = Placeholder_B,
                convertionCallback = _TextFormatParameterConvertionCallback_Boolean,
            },
            new ()
            {
                parameterType = TextFormatParameterType.DecimalInteger,
                formatSymbol = Placeholder_D,
                convertionCallback = _TextFormatParameterConvertionCallback_DecimalInteger,
            },
            new ()
            {
                parameterType = TextFormatParameterType.OctalInteger,
                formatSymbol = Placeholder_O,
                convertionCallback = _TextFormatParameterConvertionCallback_OctalInteger,
            },
            new ()
            {
                parameterType = TextFormatParameterType.HexadecimalInteger,
                formatSymbol = Placeholder_X,
                convertionCallback = _TextFormatParameterConvertionCallback_HexadecimalInteger,
            },
            new ()
            {
                parameterType = TextFormatParameterType.CommonFloat,
                formatSymbol = Placeholder_F,
                convertionCallback = _TextFormatParameterConvertionCallback_CommonFloat,
            },
            new ()
            {
                parameterType = TextFormatParameterType.ScientificNotationFloat,
                formatSymbol = Placeholder_E,
                convertionCallback = _TextFormatParameterConvertionCallback_ScientificNotationFloat,
            },
            new ()
            {
                parameterType = TextFormatParameterType.Character,
                formatSymbol = Placeholder_C,
                convertionCallback = _TextFormatParameterConvertionCallback_Character,
            },
            new ()
            {
                parameterType = TextFormatParameterType.String,
                formatSymbol = Placeholder_S,
                convertionCallback = _TextFormatParameterConvertionCallback_String,
            },
            new ()
            {
                parameterType = TextFormatParameterType.ObjectPointer,
                formatSymbol = Placeholder_P,
                convertionCallback = _TextFormatParameterConvertionCallback_ObjectPointer,
            },
            new ()
            {
                parameterType = TextFormatParameterType.ObjectType,
                formatSymbol = Placeholder_T,
                convertionCallback = _TextFormatParameterConvertionCallback_ObjectType,
            },
            new ()
            {
                parameterType = TextFormatParameterType.ObjectInfo,
                formatSymbol = Placeholder_I,
                convertionCallback = _TextFormatParameterConvertionCallback_ObjectInfo,
            },
        };

        private static string _TextFormatParameterConvertionCallback_Boolean(object obj)
        {
            bool b = Convert.ToBoolean(obj);
            return b ? Definition.CString.True : Definition.CString.False;
        }

        private static string _TextFormatParameterConvertionCallback_DecimalInteger(object obj)
        {
            long n = Convert.ToInt64(obj);
            return n.ToString();
        }

        private static string _TextFormatParameterConvertionCallback_OctalInteger(object obj)
        {
            return Convert.ToString(Convert.ToInt64(obj), 8);
        }

        private static string _TextFormatParameterConvertionCallback_HexadecimalInteger(object obj)
        {
            // return Convert.ToString(Convert.ToInt64(obj), 16);
            return string.Format("{0:X8}", obj);
        }

        private static string _TextFormatParameterConvertionCallback_CommonFloat(object obj)
        {
            double d = Convert.ToDouble(obj);
            return d.ToString();
        }

        private static string _TextFormatParameterConvertionCallback_ScientificNotationFloat(object obj)
        {
            double d = Convert.ToDouble(obj);
            return d.ToString("E");
        }

        private static string _TextFormatParameterConvertionCallback_Character(object obj)
        {
            if (obj is char)
            {
                return Convert.ToChar(obj).ToString();
            }

            string str = obj.ToString();
            if (str.IsNullOrEmpty())
            {
                return Definition.CString.Null;
            }

            char c = str[0];
            return c.ToString();
        }

        private static string _TextFormatParameterConvertionCallback_String(object obj)
        {
            Logger.Assert(obj is string, ErrorText.InvalidArguments);

            return obj.ToString();
        }

        private static string _TextFormatParameterConvertionCallback_ObjectPointer(object obj)
        {
            // unsafe { fixed (object *p = &obj) { Console.WriteLine((long) p); } } // 输出对象的内存地址指针值

            GCHandle handle = GCHandle.Alloc(obj); // GCHandle.Alloc(obj, GCHandleType.Pinned);
            IntPtr address = GCHandle.ToIntPtr(handle);
            handle.Free();

            // 将内存地址转换为十六进制整型数值的表示方式再返回
            return _TextFormatParameterConvertionCallback_HexadecimalInteger(address.ToInt64());
        }

        private static string _TextFormatParameterConvertionCallback_ObjectType(object obj)
        {
            // 处理反射对象实例的情况
            if (TryGetSystemReflectionObjectInfo(obj, out string text))
            {
                return text;
            }

            // 处理普通对象的情况
            return Utility.Text.GetFullName(obj.GetType());
        }

        private static string _TextFormatParameterConvertionCallback_ObjectInfo(object obj)
        {
            return ToString(obj);
        }

        #endregion
    }
}
