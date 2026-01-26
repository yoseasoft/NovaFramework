/// -------------------------------------------------------------------------------
/// GameEngine Framework
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

using System.Xml;
using UnityEngine.Scripting;

namespace GameEngine.Context.Configuring
{
    /// 程序配置数据的解析类
    internal static partial class ApplicationConfigureResolver
    {
        /// <summary>
        /// 实体配置导入节点的配置数据
        /// </summary>
        /// <param name="node">节点实例</param>
        [Preserve]
        [OnXmlConfigureResolvingCallback(XmlNodeType.Element, ApplicationConfigureNodeName.BeanImport)]
        private static void LoadBeanImportElement(XmlNode node)
        {
            string url = null;
            XmlAttributeCollection attrCollection = node.Attributes;
            for (int n = 0; null != attrCollection && n < attrCollection.Count; ++n)
            {
                XmlAttribute attr = attrCollection[n];
                switch (attr.Name)
                {
                    case ApplicationConfigureAttributeName.Url:
                        url = attr.Value;
                        break;
                }
            }

            if (string.IsNullOrEmpty(url))
            {
                Debugger.Warn(LogGroupTag.Basic, "应用配置文件的‘{%s}’节点导入信息不能为空，该节点解析处理异常！", node.Name);
                return;
            }

            Debugger.Info(LogGroupTag.Basic, "Load bean-import configure url '{%s}' succeed.", url);
            ApplicationConfigureInfo.AddBeanUrlPath(url);
        }
    }
}
