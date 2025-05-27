/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
/// Copyring (C) 2017 - 2020, Shanghai Tommon Network Technology Co., Ltd.
/// Copyring (C) 2020 - 2022, Guangzhou Xinyuan Technology Co., Ltd.
/// Copyring (C) 2023, Guangzhou Shiyue Network Technology Co., Ltd.
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

using SystemPath = System.IO.Path;
using SystemFile = System.IO.File;

using UnityResources = UnityEngine.Resources;
using UnityTextAsset = UnityEngine.TextAsset;

namespace NovaEngine
{
    /// <summary>
    /// 软件版本加载器工具类，提供访问外部内容加载应用版本信息
    /// </summary>
    public static class VersionLoader
    {
        /// <summary>
        /// 解析目标文本串中的版本格式信息
        /// </summary>
        /// <param name="text">目标文本串</param>
        private static string[] ResolveVersionContent(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return null;
            }

            string[] items = text.Split('.');
            if (items.Length != (int) Version.EFieldType.Max)
            {
                Logger.Error("Invalid arguments: {0}", text);
                items = null;
            }

            return items;
        }

        /// <summary>
        /// 从持久化文件中载入当前应用的版本相关配置信息
        /// </summary>
        public static bool LoadApplicationVersionFromFile(string filename)
        {
            string text = Utility.Path.LoadTextAsset(filename);
            if (null == text)
            {
                return false;
            }

            return LoadApplicationVersionFromText(text);
        }

        /// <summary>
        /// 从数据流中载入当前应用的版本相关配置信息
        /// </summary>
        public static bool LoadApplicationVersionFromText(string textAsset)
        {
            string[] items = ResolveVersionContent(textAsset);
            if (null == items)
            {
                return false;
            }

            Version.SetProperty(Version.EFieldType.Major, Utility.Convertion.StringToInt(items[(int) Version.EFieldType.Major]));
            Version.SetProperty(Version.EFieldType.Minor, Utility.Convertion.StringToInt(items[(int) Version.EFieldType.Minor]));
            Version.SetProperty(Version.EFieldType.Revision, Utility.Convertion.StringToInt(items[(int) Version.EFieldType.Revision]));
            Version.SetProperty(Version.EFieldType.Pack, Utility.Convertion.StringToInt(items[(int) Version.EFieldType.Pack]));
            Version.SetProperty(Version.EFieldType.Build, Utility.Convertion.StringToLong(items[(int) Version.EFieldType.Build]));
            Version.SetProperty(Version.EFieldType.Letter, Utility.Convertion.StringToInt(items[(int) Version.EFieldType.Letter]));

            return true;
        }

        /// <summary>
        /// 将当前应用的版本相关配置信息存入外部持久化文件中
        /// </summary>
        public static void Save(string filename)
        {
            string text = string.Format("{0}.{1}.{2}.{3}.{4}.{5}",
                                        Version.APPLICATION_MAJOR,
                                        Version.APPLICATION_MINOR,
                                        Version.APPLICATION_REVISION,
                                        Version.APPLICATION_PACK,
                                        Version.APPLICATION_BUILD,
                                        Version.APPLICATION_LETTER);

            Utility.Path.SaveTextAsset(filename, text);
        }

        /// <summary>
        /// 检测外部存储路径下目标文件中的版本相关数据内容
        /// </summary>
        /// <param name="filename">目标文件名</param>
        /// <returns>若获取版本信息成功则返回对应数据内容，否则返回null</returns>
        public static string[] CheckFromDataPath(string filename)
        {
            string url = SystemPath.Combine(Utility.Resource.PersistentDataPath, filename);
            string text = SystemFile.ReadAllText(url);

            return ResolveVersionContent(text);
        }

        /// <summary>
        /// 检测原生资源路径下目标文件中的版本相关数据内容
        /// </summary>
        /// <param name="filename">目标文件名</param>
        /// <returns>若获取版本信息成功则返回对应数据内容，否则返回null</returns>
        public static string[] CheckFromApplicationPath(string filename)
        {
            string url = Utility.Path.RemoveFileSuffixName(filename);
            UnityTextAsset textAsset = UnityResources.Load<UnityTextAsset>(url);
            if (null == textAsset)
            {
                return null;
            }

            return ResolveVersionContent(textAsset.text);
        }
    }
}
