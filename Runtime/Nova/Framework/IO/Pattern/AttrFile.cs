/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
/// Copyright (C) 2023, Guangzhou Shiyue Network Technology Co., Ltd.
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
using System.Collections.Generic;
using System.Text;

namespace NovaEngine.IO
{
    /// <summary>
    /// 属性格式文件句柄类，用于对此类型的文件进行读写操作管理
    /// </summary>
    public sealed class AttrFile : IFile
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
        private IDictionary<string, string> _attributes = null;

        /// <summary>
        /// 获取文件名称
        /// </summary>
        public string Filename
        {
            get
            {
                return _filename;
            }
        }

        /// <summary>
        /// 获取文件内容
        /// </summary>
        public string Content
        {
            get
            {
                return _content;
            }
        }

        /// <summary>
        /// 创建属性格式文件对象实例
        /// </summary>
        /// <returns>返回文件对象实例</returns>
        public static AttrFile Create()
        {
            AttrFile f = new AttrFile();

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

            if (null != _attributes)
            {
                _attributes.Clear();
                _attributes = null;
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

            string[] contents = this._content.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);

            for (int n = 0; n < contents.GetLength(0); ++n)
            {
                string line = contents[n].Trim();

                // 注释行，直接忽略
                if (line.StartsWith("#") || line.StartsWith("//") || line.StartsWith("--"))
                {
                    continue;
                }

                // 属性列表，挨个记录
                string[] attr = line.Split(new char[] { '=' });
                if (attr.Length < 2)
                {
                    Logger.Error("文件‘{0}’存在错误的配置,请检查", _filename);
                    continue;
                }

                if (attr[0].Length < 0 || attr[1].Length < 0)
                {
                    Logger.Error("文件‘{0}’存在错误的配置key={1}, value={2}请检查", _filename, attr[0], attr[1]);
                    continue;
                }

                _attributes[attr[0].Trim()] = attr[1].Trim();
            }

            return true;
        }

        /// <summary>
        /// 获取属性映射值
        /// </summary>
        /// <param name="key">属性标识</param>
        /// <returns>返回属性映射的值</returns>
        public string GetAttributeValue(string key)
        {
            string attr = null;
            if (false == _attributes.TryGetValue(key, out attr))
            {
                return null;
            }

            return attr;
        }

        /// <summary>
        /// 属性类型数据的格式转换接口函数，将数据集格式转换为数据流格式
        /// </summary>
        /// <param name="data">数据集格式的数据</param>
        /// <returns>返回转换后的数据流格式的数据</returns>
        public static string DataSetToDataStream(IDictionary<string, string> data)
        {
            if (null == data)
            {
                return null;
            }

            StringBuilder sb = new StringBuilder();

            foreach (KeyValuePair<string, string> pair in data)
            {
                sb.AppendFormat("{0} = {1}\n", pair.Key, pair.Value);
            }

            return sb.ToString();
        }

        /// <summary>
        /// 属性类型数据的格式转换接口函数，将数据流格式转换为数据集格式
        /// </summary>
        /// <param name="data">数据流格式的数据</param>
        /// <returns>返回转换后的数据集格式的数据</returns>
        public static IDictionary<string, string> DataStreamToDataSet(string data)
        {
            IDictionary<string, string> result = new Dictionary<string, string>();

            string[] contents = data.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);

            for (int n = 0; n < contents.GetLength(0); ++n)
            {
                string line = contents[n].Trim();

                // 注释行，直接忽略
                if (line.StartsWith("#") || line.StartsWith("//") || line.StartsWith("--"))
                {
                    continue;
                }

                // 属性列表，挨个记录
                string[] attr = line.Split(new char[] { '=' });
                if (attr.Length < 2 || string.IsNullOrEmpty(attr[0]) || string.IsNullOrEmpty(attr[1]))
                {
                    Logger.Error("原始数据‘{0}’存在格式错误的问题，该行数据解析失败！", line);
                    continue;
                }

                result[attr[0].Trim()] = attr[1].Trim();
            }

            return result;
        }
    }
}
