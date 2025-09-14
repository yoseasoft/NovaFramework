/// -------------------------------------------------------------------------------
/// NovaEngine Editor Framework
///
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

using System.Collections.Generic;

using UnityEditor;

using SystemEncoding = System.Text.Encoding;
using SystemUTF8Encoding = System.Text.UTF8Encoding;
using SystemMemoryStream = System.IO.MemoryStream;
using SystemXmlDocument = System.Xml.XmlDocument;
using SystemXmlElement = System.Xml.XmlElement;
using SystemXmlNode = System.Xml.XmlNode;
using SystemXmlWriter = System.Xml.XmlWriter;
using SystemXmlWriterSettings = System.Xml.XmlWriterSettings;

namespace NoveEngine.Editor
{
    /// <summary>
    /// 引擎框架的宏处理接口类
    /// </summary>
    //[InitializeOnLoad]
    public class MacroProcessor : AssetPostprocessor
    {
        static string OnGeneratedCSProject(string path, string content)
        {
            SystemXmlDocument xmlDoc = new SystemXmlDocument();
            xmlDoc.LoadXml(content);

            if (false == IsCSProjectReferenced(xmlDoc.DocumentElement))
                return content;

            if (false == ProcessDefineConstants(xmlDoc.DocumentElement))
                return content;

            // 将修改后的XML结构重新输出为文本
            using (SystemMemoryStream memoryStream = new SystemMemoryStream())
            {
                SystemXmlWriterSettings writerSettings = new SystemXmlWriterSettings
                {
                    Indent = true,
                    Encoding = new SystemUTF8Encoding(false), //无BOM
                    OmitXmlDeclaration = false
                };

                using (SystemXmlWriter xmlWriter = SystemXmlWriter.Create(memoryStream, writerSettings))
                {
                    xmlDoc.Save(xmlWriter);
                }
                return SystemEncoding.UTF8.GetString(memoryStream.ToArray());
            }
        }

        /// <summary>
        /// 处理宏定义
        /// </summary>
        private static bool ProcessDefineConstants(SystemXmlElement element)
        {
            if (null == element)
                return false;

            bool processed = false;
            foreach (SystemXmlNode node in element.ChildNodes)
            {
                if (node.Name != "PropertyGroup")
                    continue;

                foreach (SystemXmlNode childNode in node.ChildNodes)
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
        private static bool IsCSProjectReferenced(SystemXmlElement element)
        {
            if (null == element)
                return false;

            foreach (SystemXmlNode node in element.ChildNodes)
            {
                if (node.Name != "ItemGroup")
                    continue;

                foreach (SystemXmlNode childNode in node.ChildNodes)
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
