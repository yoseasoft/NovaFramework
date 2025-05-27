/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
/// Copyring (C) 2017 - 2020, Shanghai Tommon Network Technology Co., Ltd.
/// Copyring (C) 2020 - 2022, Guangzhou Xinyuan Technology Co., Ltd.
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

using SystemEnum = System.Enum;
using SystemLanguage = UnityEngine.SystemLanguage;

namespace NovaEngine
{
    /// <summary>
    /// 本地化管理类
    /// </summary>
    public static partial class Localization
    {
        /// <summary>
        /// 将目标平台语言类型转换为程序语言类型
        /// </summary>
        /// <param name="lang">平台语言参数</param>
        /// <returns>若转换成功则返回对应程序语言类型，否则返回未指定类型</returns>
        private static Language PlatformLanguageToApplicationLanguage(SystemLanguage lang)
        {
            string name = SystemEnum.GetName(typeof(SystemLanguage), lang);
            Language local_lang;
            if (SystemEnum.TryParse<Language>(name, out local_lang))
            {
                return local_lang;
            }

            return Language.Unspecified;
        }

        /// <summary>
        /// 根据内置的语言枚举值，获取对应的语言扩展类型
        /// </summary>
        /// <param name="language">系统语言枚举值</param>
        /// <returns>返回对应的地区语言扩展类型，若没有与之对应的语言，则返回默认语言</returns>
        private static RegionalLanguageType GetLanguageExtensionType(Language language)
        {
            switch (language)
            {
                /// 南非荷兰语
                case Language.Afrikaans:
                /// 阿拉伯语
                case Language.Arabic:
                /// 巴斯克语
                case Language.Basque:
                /// 白俄罗斯语
                case Language.Belarusian:
                /// 保加利亚语
                case Language.Bulgarian:
                /// 加泰罗尼亚语
                case Language.Catalan:
                    return RegionalLanguageType.English;
                /// 繁体中文
                case Language.ChineseTraditional:
                    return RegionalLanguageType.Taiwan;
                /// 简体中文
                case Language.ChineseSimplified:
                    return RegionalLanguageType.Chinese;
                /// 捷克语
                case Language.Czech:
                /// 丹麦语
                case Language.Danish:
                /// 荷兰语
                case Language.Dutch:
                /// 英语
                case Language.English:
                /// 爱沙尼亚语
                case Language.Estonian:
                /// 法罗语
                case Language.Faroese:
                /// 芬兰语
                case Language.Finnish:
                    return RegionalLanguageType.English;
                /// 法语
                case Language.French:
                    return RegionalLanguageType.French;
                /// 德语
                case Language.German:
                    return RegionalLanguageType.German;
                /// 希腊语
                case Language.Greek:
                /// 希伯来语
                case Language.Hebrew:
                /// 冰岛语
                case Language.Icelandic:
                /// 印尼语
                case Language.Indonesian:
                    return RegionalLanguageType.English;
                /// 意大利语
                case Language.Italian:
                    return RegionalLanguageType.Italy;
                /// 日语
                case Language.Japanese:
                    return RegionalLanguageType.Japanese;
                /// 韩语
                case Language.Korean:
                    return RegionalLanguageType.Korea;
                /// 拉脱维亚语
                case Language.Latvian:
                /// 立陶宛语
                case Language.Lithuanian:
                /// 挪威语
                case Language.Norwegian:
                /// 波兰语
                case Language.Polish:
                /// 罗马尼亚语
                case Language.Romanian:
                    return RegionalLanguageType.English;
                /// 葡萄牙语
                case Language.PortuguesePortugal:
                    return RegionalLanguageType.Portuguese;
                /// 俄语
                case Language.Russian:
                    return RegionalLanguageType.Russia;
                /// 塞尔维亚克罗地亚语
                case Language.SerboCroatian:
                /// 斯洛伐克语
                case Language.Slovak:
                /// 斯洛文尼亚语
                case Language.Slovenian:
                    return RegionalLanguageType.English;
                /// 西班牙语
                case Language.Spanish:
                    return RegionalLanguageType.Spanish;
                /// 瑞典语
                case Language.Swedish:
                /// 泰语
                case Language.Thai:
                /// 土耳其语
                case Language.Turkish:
                /// 乌克兰语
                case Language.Ukrainian:
                /// 越南语
                case Language.Vietnamese:
                    return RegionalLanguageType.English;
                case Language.Unspecified:
                default:
                    return RegionalLanguageType.English;
            }
        }

        /// <summary>
        /// 根据当前系统平台语言版本，获取对应的语言扩展名称
        /// </summary>
        /// <returns>返回对应的地区语言扩展名称，若没有与之对应的语言，则返回默认语言</returns>
        public static string GetLanguageExtensionName()
        {
            SystemLanguage lang = UnityEngine.Application.systemLanguage;
            return GetLanguageExtensionName(PlatformLanguageToApplicationLanguage(lang));
        }

        /// <summary>
        /// 根据内置的语言枚举值，获取对应的语言扩展名称
        /// </summary>
        /// <param name="language">系统语言枚举值</param>
        /// <returns>返回对应的地区语言扩展名称，若没有与之对应的语言，则返回默认语言</returns>
        public static string GetLanguageExtensionName(Language language)
        {
            RegionalLanguageType extensionType = GetLanguageExtensionType(language);
            return EnumDescriptionAttribute.GetDescription(extensionType);
        }
    }
}
