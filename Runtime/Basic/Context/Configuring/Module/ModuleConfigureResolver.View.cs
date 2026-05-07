/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyright (C) 2025 - 2026, Hainan Yuanyou Information Technology Co., Ltd. Guangzhou Branch
/// Copyright (C) 2026, Hurley, Independent Studio.
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
    /// 模组配置数据的解析类
    internal static partial class ModuleConfigureResolver
    {
        /// <summary>
        /// 热加载模块导入节点的配置数据
        /// </summary>
        /// <param name="node">节点实例</param>
        [Preserve]
        [OnXmlConfigureResolvingCallback(XmlNodeType.Element, ModuleConfigureNodeName.ViewGroup)]
        private static void LoadViewGroupElement(XmlNode node)
        {
            XmlNodeList childs = node.ChildNodes;
            for (int n = 0; null != childs && n < childs.Count; ++n)
            {
                XmlNode child = childs[n];

                switch (child.Name)
                {
                    case ModuleConfigureNodeName.Group:
                        LoadViewGroupContentInfo(child);
                        break;
                    default:
                        Debugger.Warn("Invalid child node '{%s}' within 'view-group' element, resolved it failed.", child.Name);
                        break;
                }
            }
        }

        private static void LoadViewGroupContentInfo(XmlNode node)
        {
            string groupName = null;
            int level = 0;
            ViewGroupStrategyType strategyType = ViewGroupStrategyType.None;

            XmlAttributeCollection attrCollection = node.Attributes;
            for (int n = 0; null != attrCollection && n < attrCollection.Count; ++n)
            {
                XmlAttribute attr = attrCollection[n];
                switch (attr.Name)
                {
                    case ModuleConfigureAttributeName.Name:
                        groupName = attr.Value;
                        break;
                    case ModuleConfigureAttributeName.Level:
                        level = NovaEngine.Utility.Convertion.StringToInt(attr.Value);
                        break;
                }
            }

            XmlNodeList childs = node.ChildNodes;
            for (int n = 0; null != childs && n < childs.Count; ++n)
            {
                XmlNode child = childs[n];

                switch (child.Name)
                {
                    case ModuleConfigureNodeName.StrategyType:
                        strategyType |= NovaEngine.Utility.Convertion.GetEnumFromName<ViewGroupStrategyType>(child.InnerText);
                        break;
                    default:
                        Debugger.Warn("Invalid child node '{%s}' within 'group' element, resolved it failed.", child.Name);
                        break;
                }
            }

            if (string.IsNullOrEmpty(groupName) || level <= 0)
            {
                Debugger.Warn(LogGroupTag.Basic, "应用配置文件的‘{%s}’节点导入信息不能为空，该节点解析处理异常！", node.Name);
                return;
            }

            Debugger.Info(LogGroupTag.Basic, "Load view group configure 'name={%s}, level={%d}, strategy={%v}' succeed.", groupName, level, strategyType);

            GuiHandler.Instance.AddViewGroup(groupName, level, strategyType);
        }
    }
}
