/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyright (C) 2023 - 2024, Guangzhou Shiyue Network Technology Co., Ltd.
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

using SystemType = System.Type;

namespace GameEngine.Loader.Configuring
{
    /// 对象配置数据的解析类
    internal static partial class CodeConfigureResolver
    {
        /// <summary>
        /// 加载文件导入节点的配置数据
        /// </summary>
        /// <param name="node">节点实例</param>
        /// <remarks>返回配置数据的对象实例</remarks>
        [CodeLoader.OnConfigureResolvingCallback(XmlNodeType.Element, BeanConfigureNodeName.File)]
        private static BaseConfigureInfo LoadFileElement(XmlNode node)
        {
            FileConfigureInfo fileInfo = new FileConfigureInfo();

            XmlAttributeCollection attrCollection = node.Attributes;
            for (int n = 0; null != attrCollection && n < attrCollection.Count; ++n)
            {
                XmlAttribute attr = attrCollection[n];
                switch (attr.Name)
                {
                    case BeanConfigureNodeAttributeName.K_INCLUDE:
                        fileInfo.Include = attr.Value;
                        break;
                }
            }

            if (string.IsNullOrEmpty(fileInfo.Include))
            {
                Debugger.Warn(LogGroupTag.CodeLoader, "实体配置文件的‘{%s}’节点导入信息不能为空，该节点解析处理异常！", node.Name);
                return null;
            }

            Debugger.Info(LogGroupTag.CodeLoader, "Load file configure '{%s}' succeed.", CodeLoaderObject.ToString(fileInfo));
            return fileInfo;
        }
    }
}
