/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
/// Copyright (C) 2020 - 2022, Guangzhou Xinyuan Technology Co., Ltd.
/// Copyright (C) 2022 - 2023, Shanghai Bilibili Technology Co., Ltd.
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

namespace NovaEngine.IO
{
    /// <summary>
    /// INI格式文件句柄类，用于对此类型的文件进行读写操作管理
    /// </summary>
    public sealed class IniFile : IFile
    {
        /// <summary>
        /// 当前打开的文件名称
        /// </summary>
        private string _filename = null;

        /// <summary>
        /// 当前打开的文件内容
        /// </summary>
        private string _content = null;

        /// <summary>
        /// 文件段属性映射表
        /// </summary>
        private IDictionary<string, IDictionary<string, string>> _sections = null;

        /// <summary>
        /// 获取文件名称
        /// </summary>
        public string Filename
        {
            get { return _filename; }
        }

        /// <summary>
        /// 获取文件内容
        /// </summary>
        public string Content
        {
            get { return _content; }
        }

        /// <summary>
        /// 创建INI格式文件对象实例
        /// </summary>
        /// <returns>返回文件对象实例</returns>
        public static IniFile Create()
        {
            IniFile f = new IniFile();

            return f;
        }

        /// <summary>
        /// 以路径方式加载文件内容
        /// </summary>
        /// <param name="filename">文件路径</param>
        /// <returns>若加载文件成功则返回true，否则返回false</returns>
        public bool LoadFromFile(string filename)
        {
            string text = Utility.Path.LoadTextAsset(filename);
            if (null == text)
            {
                return false;
            }

            _filename = filename;

            return LoadFromText(text);
        }

        /// <summary>
        /// 以文本方式加载文件内容
        /// </summary>
        /// <param name="text">文本内容</param>
        /// <returns>若加载文件成功则返回true，否则返回false</returns>
        public bool LoadFromText(string text)
        {
            _content = text;

            return __ResolveFileContent();
        }

        /// <summary>
        /// 文件关闭操作接口
        /// </summary>
        public void Close()
        {
            _filename = null;
            _content = null;

            if (null != _sections)
            {
                _sections.Clear();
                _sections = null;
            }
        }

        /// <summary>
        /// 解析文件内容
        /// </summary>
        /// <returns>若解析成功则返回true，否则返回false</returns>
        private bool __ResolveFileContent()
        {
            if (string.IsNullOrEmpty(this._content))
            {
                return false;
            }

            string[] contents = this._content.Split(new string[] { "\n" }, System.StringSplitOptions.RemoveEmptyEntries);

            string sectionName = "default";
            IDictionary<string, string> dict = null;
            if (null == _sections)
            {
                dict = new Dictionary<string, string>();
                _sections = new Dictionary<string, IDictionary<string, string>>();
                _sections.Add(sectionName, dict);
            }

            for (int n = 0; n < contents.GetLength(0); ++n)
            {
                string line = contents[n].Trim();

                // 注释行，直接忽略
                if (line.StartsWith("#") || line.StartsWith("//") || line.StartsWith("--"))
                {
                    continue;
                }

                // 段落标识，进入新段区
                if (line.StartsWith("[") && line.EndsWith("]"))
                {
                    sectionName = line.Substring(1, line.Length - 2).Trim();
                    if (false == _sections.ContainsKey(sectionName))
                    {
                        dict = new Dictionary<string, string>();
                        _sections.Add(sectionName, dict);
                    }
                    continue;
                }

                // 属性列表，挨个记录
                string[] property = line.Split(new char[] { '=' });
                if (property.Length < 2)
                {
                    Logger.Error("文件‘{0}’存在错误的配置,请检查", _filename);
                    continue;
                }

                if (property[0].Length < 0 || property[1].Length < 0)
                {
                    Logger.Error("文件‘{0}’存在错误的配置key={1}, value={2}请检查", _filename, property[0], property[1]);
                    continue;
                }

                dict[property[0].Trim()] = property[1].Trim();
            }

            return true;
        }

        /// <summary>
        /// 获取段落映射值
        /// </summary>
        /// <param name="sectionName">段名称</param>
        /// <returns>返回段落映射的字典表</returns>
        public IReadOnlyDictionary<string, string> GetSectionValue(string sectionName = "default")
        {
            if (false == _sections.TryGetValue(sectionName, out IDictionary<string, string> dict))
            {
                return null;
            }

            return new System.Collections.ObjectModel.ReadOnlyDictionary<string, string>(dict);
        }

        /// <summary>
        /// 获取属性映射值
        /// </summary>
        /// <param name="key">属性标识</param>
        /// <param name="sectionName">段名称</param>
        /// <returns>返回属性映射的值</returns>
        public string GetPropertyValue(string key, string sectionName = "default")
        {
            IReadOnlyDictionary<string, string> dict = this.GetSectionValue(sectionName);
            if (null == dict)
            {
                return null;
            }

            string prop = null;
            if (false == dict.TryGetValue(key, out prop))
            {
                return null;
            }

            return prop;
        }
    }
}
