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

using SystemType = System.Type;
using SystemIntPtr = System.IntPtr;
using SystemStringBuilder = System.Text.StringBuilder;
using SystemRegex = System.Text.RegularExpressions.Regex;
using SystemMatch = System.Text.RegularExpressions.Match;
using SystemMatchCollection = System.Text.RegularExpressions.MatchCollection;
using SystemGCHandle = System.Runtime.InteropServices.GCHandle;
using SystemGCHandleType = System.Runtime.InteropServices.GCHandleType;

namespace NovaEngine
{
    /// <summary>
    /// 格式化接口集合工具类
    /// </summary>
    public static partial class Formatter
    {
        private const char Placeholder_D = 'd'; // 用于输出十进制整数
        private const char Placeholder_O = 'o'; // 用于输出八进制整数
        private const char Placeholder_X = 'x'; // 用于输出十六进制整数
        private const char Placeholder_F = 'f'; // 用于输出普通浮点数
        private const char Placeholder_E = 'e'; // 用于输出科学计数法的浮点数
        private const char Placeholder_C = 'c'; // 用于输出单个字符
        private const char Placeholder_S = 's'; // 用于输出字符串
        private const char Placeholder_P = 'p'; // 用于输出对象的内存指针地址
        private const char Placeholder_T = 't'; // 用于输出对象类型
        private const char Placeholder_V = 'v'; // 用于输出对象的详细信息

        /// <summary>
        /// 文本格式的参数类型分类的枚举定义
        /// </summary>
        private enum TextFormatParameterType
        {
            Unknown,
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
        private static TextFormatConvertionInfo[] s_textFormatConvertionInfos = new TextFormatConvertionInfo[(int) TextFormatParameterType.Max]
        {
            new TextFormatConvertionInfo {
                                            parameterType = TextFormatParameterType.Unknown,
                                            formatSymbol = Definition.CCharacter.Nil,
                                            convertionCallback = null,
                                         },
            new TextFormatConvertionInfo {
                                            parameterType = TextFormatParameterType.DecimalInteger,
                                            formatSymbol = Placeholder_D,
                                            convertionCallback = _TextFormatParameterConvertionCallback_DecimalInteger,
                                         },
            new TextFormatConvertionInfo {
                                            parameterType = TextFormatParameterType.OctalInteger,
                                            formatSymbol = Placeholder_O,
                                            convertionCallback = _TextFormatParameterConvertionCallback_OctalInteger,
                                         },
            new TextFormatConvertionInfo {
                                            parameterType = TextFormatParameterType.HexadecimalInteger,
                                            formatSymbol = Placeholder_X,
                                            convertionCallback = _TextFormatParameterConvertionCallback_HexadecimalInteger,
                                         },
            new TextFormatConvertionInfo {
                                            parameterType = TextFormatParameterType.CommonFloat,
                                            formatSymbol = Placeholder_F,
                                            convertionCallback = _TextFormatParameterConvertionCallback_CommonFloat,
                                         },
            new TextFormatConvertionInfo {
                                            parameterType = TextFormatParameterType.ScientificNotationFloat,
                                            formatSymbol = Placeholder_E,
                                            convertionCallback = _TextFormatParameterConvertionCallback_ScientificNotationFloat,
                                         },
            new TextFormatConvertionInfo {
                                            parameterType = TextFormatParameterType.Character,
                                            formatSymbol = Placeholder_C,
                                            convertionCallback = _TextFormatParameterConvertionCallback_Character,
                                         },
            new TextFormatConvertionInfo {
                                            parameterType = TextFormatParameterType.String,
                                            formatSymbol = Placeholder_S,
                                            convertionCallback = _TextFormatParameterConvertionCallback_String,
                                         },
            new TextFormatConvertionInfo {
                                            parameterType = TextFormatParameterType.ObjectPointer,
                                            formatSymbol = Placeholder_P,
                                            convertionCallback = _TextFormatParameterConvertionCallback_ObjectPointer,
                                         },
            new TextFormatConvertionInfo {
                                            parameterType = TextFormatParameterType.ObjectType,
                                            formatSymbol = Placeholder_T,
                                            convertionCallback = _TextFormatParameterConvertionCallback_ObjectType,
                                         },
            new TextFormatConvertionInfo {
                                            parameterType = TextFormatParameterType.ObjectInfo,
                                            formatSymbol = Placeholder_V,
                                            convertionCallback = _TextFormatParameterConvertionCallback_ObjectInfo,
                                         },
        };

        private static string _TextFormatParameterConvertionCallback_DecimalInteger(object obj)
        {
            long n = System.Convert.ToInt64(obj);
            return n.ToString();
        }

        private static string _TextFormatParameterConvertionCallback_OctalInteger(object obj)
        {
            return null;
        }

        private static string _TextFormatParameterConvertionCallback_HexadecimalInteger(object obj)
        {
            return null;
        }

        private static string _TextFormatParameterConvertionCallback_CommonFloat(object obj)
        {
            double d = System.Convert.ToDouble(obj);
            return d.ToString();
        }

        private static string _TextFormatParameterConvertionCallback_ScientificNotationFloat(object obj)
        {
            return null;
        }

        private static string _TextFormatParameterConvertionCallback_Character(object obj)
        {
            char c = System.Convert.ToChar(obj);
            return c.ToString();
        }

        private static string _TextFormatParameterConvertionCallback_String(object obj)
        {
            return obj.ToString();
        }

        private static string _TextFormatParameterConvertionCallback_ObjectPointer(object obj)
        {
            // unsafe { fixed (object *p = &obj) { Console.WriteLine((long) p); } } // 输出对象的内存地址指针值

            SystemGCHandle handle = SystemGCHandle.Alloc(obj, SystemGCHandleType.Pinned);
            SystemIntPtr address = SystemGCHandle.ToIntPtr(handle);
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
            return ToVerboseInfo(obj);
        }

        #endregion
    }
}
