/// -------------------------------------------------------------------------------
/// NovaEngine Editor Framework
///
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

using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

using UnityEditor;

namespace NovaEngine.Editor
{
    /// <summary>
    /// 引擎框架的宏处理接口类
    /// </summary>
    //[InitializeOnLoad]
    public class MacroProcessor : AssetPostprocessor
    {
        static string OnGeneratedCSProject(string path, string content)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(content);

            if (false == IsCSProjectReferenced(xmlDoc.DocumentElement))
                return content;

            if (false == ProcessDefineConstants(xmlDoc.DocumentElement))
                return content;

            // 将修改后的XML结构重新输出为文本
            using (MemoryStream memoryStream = new MemoryStream())
            {
                XmlWriterSettings writerSettings = new XmlWriterSettings
                {
                    Indent = true,
                    Encoding = new UTF8Encoding(false), //无BOM
                    OmitXmlDeclaration = false
                };

                using (XmlWriter xmlWriter = XmlWriter.Create(memoryStream, writerSettings))
                {
                    xmlDoc.Save(xmlWriter);
                }
                return Encoding.UTF8.GetString(memoryStream.ToArray());
            }
        }

        /// <summary>
        /// 处理宏定义
        /// </summary>
        private static bool ProcessDefineConstants(XmlElement element)
        {
            if (null == element)
                return false;

            bool processed = false;
            foreach (XmlNode node in element.ChildNodes)
            {
                if (node.Name != "PropertyGroup")
                    continue;

                foreach (XmlNode childNode in node.ChildNodes)
                {
                    if (childNode.Name != "DefineConstants")
                        continue;

                    string[] defines = childNode.InnerText.Split(';');
                    HashSet<string> hashSets = new HashSet<string>(defines);
                    foreach (string frameMacro in MacroDefine.MACROS)
                    {
                        string tmpMacro = frameMacro.Trim();
                        if (hashSets.Contains(tmpMacro) == false)
                            hashSets.Add(tmpMacro);
                    }
                    childNode.InnerText = string.Join(";", NovaEngine.Utility.Collection.ToArray<string>(hashSets));
                    processed = true;
                }
            }

            return processed;
        }

        /// <summary>
        /// 检测工程是否引用了NovaFramework
        /// </summary>
        private static bool IsCSProjectReferenced(XmlElement element)
        {
            if (null == element)
                return false;

            foreach (XmlNode node in element.ChildNodes)
            {
                if (node.Name != "ItemGroup")
                    continue;

                foreach (XmlNode childNode in node.ChildNodes)
                {
                    if (childNode.Name != "Reference" && childNode.Name != "ProjectReference")
                        continue;

                    string include = childNode.Attributes["Include"].Value;
                    if (include.Contains("NovaFramework"))
                        return true;
                }
            }

            return false;
        }
    }
}
