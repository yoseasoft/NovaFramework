/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
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

namespace NovaEngine
{
    /// <summary>
    /// 基础常量数据定义类，将基础数据类型中的一些通用常量在此统一进行定义使用
    /// </summary>
    public static partial class Definition
    {
        /// <summary>
        /// 字符串相关常量数据定义
        /// </summary>
        public static class CString
        {
            // 字符串类型常量，值为制表符“\t”
            public const string TAB = "\t";
            // 字符串类型常量，值为回车符“\r”
            public const string CR = "\r";
            // 字符串类型常量，值为换行符“\n”
            public const string LF = "\n";
            // 字符串类型常量，值为回车换行符“\r\n”
            public const string CRLF = "\r\n";

            // 字符串类型常量，值为斜杠“/”
            public const string Slash = "/";
            // 字符串类型常量，值为双斜杠“//”
            public const string DoubleSlash = "//";
            // 字符串类型常量，值为反斜杠“\”
            public const string Backslash = "\\";
            // 字符串类型常量，值为双反斜杠“\\”
            public const string DoubleBackslash = "\\\\";
            // 字符串类型常量，值为左小括号“(”
            public const string LeftParen = "(";
            // 字符串类型常量，值为右小括号“)”
            public const string RightParen = ")";
            // 字符串类型常量，值为左中括号“[”
            public const string LeftBracket = "[";
            // 字符串类型常量，值为右中括号“]”
            public const string RightBracket = "]";
            // 字符串类型常量，值为左花括号“{”
            public const string LeftCurlyBracket = "{";
            // 字符串类型常量，值为右花括号“}”
            public const string RightCurlyBracket = "}";
            // 字符串类型常量，值为冒号“:”
            public const string Colon = ":";
            // 字符串类型常量，值为分号“;”
            public const string Semicolon = ";";
            // 字符串类型常量，值为单引号“'”
            public const string Quote = "'";
            // 字符串类型常量，值为双引号“"”
            public const string DoubleQuote = "\"";
            // 字符串类型常量，值为逗号“,”
            public const string Comma = ",";
            // 字符串类型常量，值为点号“.”
            public const string Dot = ".";
            // 字符串类型常量，值为双点号“..”
            public const string DoubleDot = "..";
            // 字符串类型常量，值为空格符“ ”
            public const string Space = " ";
            // 字符串类型常量，值为下划线“_”
            public const string Underline = "_";
            // 字符串类型常量，值为波浪号“~”
            public const string Tilde = "~";
            // 字符串类型常量，值为艾特符“@”
            public const string At = "@";
            // 字符串类型常量，值为井号符“#”
            public const string Hash = "#";
            // 字符串类型常量，值为美元符“$”
            public const string Dollar = "$";
            // 字符串类型常量，值为百分号“%”
            public const string Percent = "%";

            // 字符串类型常量，值为与运算符“&”
            public const string Ampersand = "&";
            // 字符串类型常量，值为或运算符“|”
            public const string Pipe = "|";

            //字符串类型常量，值为加号“+”
            public const string Plus = "+";
            //字符串类型常量，值为减号“-”
            public const string Minus = "-";
            //字符串类型常量，值为乘号“*”
            public const string Asterisk = "*";
            //字符串类型常量，值为小于号“<”
            public const string Less = "<";
            //字符串类型常量，值为大于号“>”
            public const string Greater = ">";
            //字符串类型常量，值为问号“?”
            public const string Question = "?";

            // 字符串类型常量，值为“y”，用于表示字符类型真值
            public const string Y = "y";
            // 字符串类型常量，值为“n”，用于表示字符类型假值
            public const string N = "n";

            // 字符串类型常量，值为“yes”，用于表示字符类型真值
            public const string Yes = "yes";
            // 字符串类型常量，值为“no”，用于表示字符类型假值
            public const string No = "no";

            // 字符串类型常量，值为“ok”，用于表示字符类型真值
            public const string Ok = "ok";
            // 字符串类型常量，值为“error”，用于表示字符类型假值
            public const string Error = "error";

            // 字符串类型常量，值为“true”，用于表示字符类型真值
            public const string True = "true";
            // 字符串类型常量，值为“false”，用于表示字符类型假值
            public const string False = "false";

            // 字符串类型常量，值为“null”，用于表示字符类型空引用值
            public const string Null = "null";
            // 字符串类型常量，值为“none”，用于表示字符类型空值
            public const string None = "none";
            // 字符串类型常量，值为“unknown”，用于表示字符类型无效值
            public const string Unknown = "unknown";
            // 字符串类型常量，值为“default”，用于表示字符类型默认值
            public const string Default = "default";

            // 字符串类型常量，值为HTML中的And符转译“&amp;” -> “&”
            public const string HtmlAmp = "&amp;";
            // 字符串类型常量，值为HTML中的单引号转译“&apos;” -> “'”
            public const string HtmlApos = "&apos;";
            // 字符串类型常量，值为HTML中的双引号转译“&quot;” -> “\"”
            public const string HtmlQuote = "&quot;";
            // 字符串类型常量，值为HTML中的大于号转译“&gt;” -> “>”
            public const string HtmlGt = "&gt;";
            // 字符串类型常量，值为HTML中的小于号转译“&lt;” -> “<”
            public const string HtmlLt = "&lt;";
            // 字符串类型常量，值为HTML中的空格符转译“&nbsp;” -> “ ”
            public const string HtmlNbsp = "&nbsp;";
        }
    }
}
