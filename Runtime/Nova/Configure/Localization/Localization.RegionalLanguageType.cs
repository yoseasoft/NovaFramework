/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
/// Copyright (C) 2017 - 2020, Shanghai Tommon Network Technology Co., Ltd.
/// Copyright (C) 2020 - 2022, Guangzhou Xinyuan Technology Co., Ltd.
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
    /// 本地化管理类
    /// </summary>
    public static partial class Localization
    {
        /// <summary>
        /// 语言扩展枚举类型定义
        /// </summary>
        internal enum RegionalLanguageType : byte
        {
            /// <summary>
            /// 未知类型
            /// </summary>
            Unknown = 0,

            /// <summary>
            /// 中文
            /// </summary>
            [EnumDescription("CN")]
            Chinese,

            /// <summary>
            /// 繁体
            /// </summary>
            [EnumDescription("TW")]
            Taiwan,

            /// <summary>
            /// 英语
            /// </summary>
            [EnumDescription("EN")]
            English,

            /// <summary>
            /// 日语
            /// </summary>
            [EnumDescription("JP")]
            Japanese,

            /// <summary>
            /// 法语
            /// </summary>
            [EnumDescription("FR")]
            French,

            /// <summary>
            /// 德语
            /// </summary>
            [EnumDescription("GE")]
            German,

            /// <summary>
            /// 意大利语
            /// </summary>
            [EnumDescription("IT")]
            Italy,

            /// <summary>
            /// 朝鲜语
            /// </summary>
            [EnumDescription("KR")]
            Korea,

            /// <summary>
            /// 俄语
            /// </summary>
            [EnumDescription("RU")]
            Russia,

            /// <summary>
            /// 西班牙语
            /// </summary>
            [EnumDescription("SP")]
            Spanish,

            /// <summary>
            /// 葡萄牙语
            /// </summary>
            [EnumDescription("POR")]
            Portuguese,

            /// <summary>
            /// 阿拉伯语
            /// </summary>
            [EnumDescription("AR")]
            Arabic,
        }
    }
}
