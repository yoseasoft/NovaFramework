/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
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

namespace NovaEngine
{
    /// <summary>
    /// 基础常量数据定义类，将字符和字符串中的一些通用常量在此统一进行定义使用
    /// </summary>
    public static partial class Definition
    {
        /// <summary>
        /// 字符相关常量数据定义
        /// </summary>
        public static class CCharacter
        {
            /// <summary>
            /// 字符键数组列表
            /// </summary>
            public static readonly char[] KEYCODE_ARRAY = new char[] {
                'a','b','d','c','e','f','g','h','i','j','k','l','m','n','o','p','q','r','s','t','u','v','w','x','y','z',
                '0','1','2','3','4','5','6','7','8','9',
                'A','B','C','D','E','F','G','H','I','J','K','L','M','N','O','P','Q','R','S','T','U','V','W','X','Y','Z',
            };

            // 字符类型常量，值为制表符‘\t’
            public const char TAB = '\t';
            // 字符类型常量，值为回车符‘\r’
            public const char CR = '\r';
            // 字符类型常量，值为换行符‘\n’
            public const char LF = '\n';

            // 字符类型常量，值为斜杠‘/’
            public const char Slash = '/';
            // 字符类型常量，值为反斜杠‘\’
            public const char Backslash = '\\';
            // 字符类型常量，值为左小括号‘(’
            public const char LeftParen = '(';
            // 字符类型常量，值为右小括号‘)’
            public const char RightParen = ')';
            // 字符类型常量，值为左中括号‘[’
            public const char LeftBracket = '[';
            // 字符类型常量，值为右中括号‘]’
            public const char RightBracket = ']';
            // 字符类型常量，值为左花括号‘{’
            public const char LeftCurlyBracket = '{';
            // 字符类型常量，值为右花括号‘}’
            public const char RightCurlyBracket = '}';
            // 字符类型常量，值为冒号‘:’
            public const char Colon = ':';
            // 字符类型常量，值为分号‘;’
            public const char Semicolon = ';';
            // 字符类型常量，值为单引号‘'’
            public const char Quote = '\'';
            // 字符类型常量，值为双引号‘"’
            public const char DoubleQuote = '"';
            // 字符类型常量，值为逗号‘,’
            public const char Comma = ',';
            // 字符类型常量，值为点号‘.’
            public const char Dot = '.';
            // 字符类型常量，值为空格符‘ ’
            public const char Space = ' ';
            // 字符类型常量，值为下划线‘_’
            public const char Underline = '_';
            // 字符类型常量，值为波浪号‘~’
            public const char Tilde = '~';
            // 字符类型常量，值为艾特符‘@’
            public const char At = '@';
            // 字符类型常量，值为井号符‘#’
            public const char Hash = '#';
            // 字符类型常量，值为美元符‘$’
            public const char Dollar = '$';
            // 字符类型常量，值为百分号‘%’
            public const char Percent = '%';

            // 字符类型常量，值为与运算符‘&’
            public const char Ampersand = '&';
            // 字符类型常量，值为或运算符‘|’
            public const char Pipe = '|';

            // 字符类型常量，值为加号‘+’
            public const char Plus = '+';
            // 字符类型常量，值为减号‘-’
            public const char Minus = '-';
            // 字符类型常量，值为乘号‘*’
            public const char Asterisk = '*';
            // 字符类型常量，值为小于号‘<’
            public const char Less = '<';
            // 字符类型常量，值为大于号‘>’
            public const char Greater = '>';
            // 字符类型常量，值为等于号‘=’
            public const char Equal = '=';
            // 字符类型常量，值为问号‘?’
            public const char Question = '?';

            // 字符类型常量，值为‘y’，用于表示字符类型真值
            public const char Y = 'y';
            // 字符类型常量，值为‘n’，用于表示字符类型假值
            public const char N = 'n';
        }
    }
}
