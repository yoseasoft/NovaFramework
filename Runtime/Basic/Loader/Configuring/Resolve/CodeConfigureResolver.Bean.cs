/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyright (C) 2023 - 2024, Guangzhou Shiyue Network Technology Co., Ltd.
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

namespace GameEngine.Loader.Configuring
{
    /// 对象配置数据的解析类
    internal static partial class CodeConfigureResolver
    {
        /// <summary>
        /// 加载基础Bean节点的配置数据
        /// </summary>
        /// <param name="node">节点实例</param>
        /// <remarks>返回配置数据的对象实例</remarks>
        [Preserve]
        [OnXmlConfigureResolvingCallback(XmlNodeType.Element, BeanConfigureNodeName.Bean)]
        private static void LoadBeanElement(XmlNode node)
        {
            BeanConfigureInfo beanInfo = new BeanConfigureInfo();

            XmlAttributeCollection attrCollection = node.Attributes;
            for (int n = 0; null != attrCollection && n < attrCollection.Count; ++n)
            {
                XmlAttribute attr = attrCollection[n];
                switch (attr.Name)
                {
                    case BeanConfigureNodeAttributeName.Name:
                        beanInfo.Name = attr.Value;
                        break;
                    case BeanConfigureNodeAttributeName.ClassType:
                        beanInfo.ClassType = NovaEngine.Utility.Assembly.GetType(attr.Value);
                        Debugger.Assert(beanInfo.ClassType, "Invalid bean class type.");
                        break;
                    case BeanConfigureNodeAttributeName.ParentName:
                        beanInfo.ParentName = attr.Value;
                        break;
                    case BeanConfigureNodeAttributeName.Singleton:
                        beanInfo.Singleton = bool.Parse(attr.Value);
                        break;
                    case BeanConfigureNodeAttributeName.Inherited:
                        beanInfo.Inherited = bool.Parse(attr.Value);
                        break;
                }
            }

            XmlNodeList nodeList = node.ChildNodes;
            for (int n = 0; null != nodeList && n < nodeList.Count; ++n)
            {
                XmlNode child = nodeList.Item(n);
                if (BeanConfigureNodeName.Field.Equals(child.Name))
                {
                    BeanFieldConfigureInfo fieldInfo = LoadBeanFieldElement(child);
                    beanInfo.AddFieldInfo(fieldInfo);
                }
                else if (BeanConfigureNodeName.Property.Equals(child.Name))
                {
                    BeanPropertyConfigureInfo propertyInfo = LoadBeanPropertyElement(child);
                    beanInfo.AddPropertyInfo(propertyInfo);
                }
                else if (BeanConfigureNodeName.Component.Equals(child.Name))
                {
                    BeanComponentConfigureInfo componentInfo = LoadBeanComponentElement(child);
                    beanInfo.AddComponentInfo(componentInfo);
                }
                else
                {
                    Debugger.Warn(LogGroupTag.CodeLoader, "Cannot resolve target configure node '{%s}' from bean info '{%s}', loaded bean configure failed.", child.Name, beanInfo.Name);
                }
            }

            // 验证：有组件成员的bean，必须是entity类型
            if (beanInfo.GetComponentInfoCount() > 0)
            {
                if (false == beanInfo.ClassType.IsSubclassOf(typeof(CEntity)))
                {
                    Debugger.Warn(LogGroupTag.CodeLoader, "Could not add component to target bean class '{%t}', loaded bean '{%s}' failed.", beanInfo.ClassType, beanInfo.Name);
                    return;
                }
            }

            Debugger.Info(LogGroupTag.CodeLoader, "Load bean configure '{%s}' succeed.", CodeLoaderUtils.ToString(beanInfo));
            AddNodeConfigureInfo(beanInfo);
        }

        private static BeanFieldConfigureInfo LoadBeanFieldElement(XmlNode node)
        {
            BeanFieldConfigureInfo fieldInfo = new BeanFieldConfigureInfo();

            XmlAttributeCollection attrCollection = node.Attributes;
            for (int n = 0; null != attrCollection && n < attrCollection.Count; ++n)
            {
                XmlAttribute attr = attrCollection[n];
                switch (attr.Name)
                {
                    case BeanConfigureNodeAttributeName.Name:
                        fieldInfo.FieldName = attr.Value;
                        break;
                    case BeanConfigureNodeAttributeName.ReferenceName:
                        fieldInfo.ReferenceName = attr.Value;
                        break;
                    case BeanConfigureNodeAttributeName.ReferenceType:
                        fieldInfo.ReferenceType = NovaEngine.Utility.Assembly.GetType(attr.Value);
                        Debugger.Assert(fieldInfo.ReferenceType, "Invalid bean field class type.");
                        break;
                    case BeanConfigureNodeAttributeName.ReferenceValue:
                        fieldInfo.ReferenceValue = attr.Value;
                        break;
                }
            }

            return fieldInfo;
        }

        private static BeanPropertyConfigureInfo LoadBeanPropertyElement(XmlNode node)
        {
            BeanPropertyConfigureInfo propertyInfo = new BeanPropertyConfigureInfo();

            XmlAttributeCollection attrCollection = node.Attributes;
            for (int n = 0; null != attrCollection && n < attrCollection.Count; ++n)
            {
                XmlAttribute attr = attrCollection[n];
                switch (attr.Name)
                {
                    case BeanConfigureNodeAttributeName.Name:
                        propertyInfo.PropertyName = attr.Value;
                        break;
                    case BeanConfigureNodeAttributeName.ReferenceName:
                        propertyInfo.ReferenceName = attr.Value;
                        break;
                    case BeanConfigureNodeAttributeName.ReferenceType:
                        propertyInfo.ReferenceType = NovaEngine.Utility.Assembly.GetType(attr.Value);
                        Debugger.Assert(propertyInfo.ReferenceType, "Invalid bean property class type.");
                        break;
                    case BeanConfigureNodeAttributeName.ReferenceValue:
                        propertyInfo.ReferenceValue = attr.Value;
                        break;
                }
            }

            return propertyInfo;
        }

        private static BeanComponentConfigureInfo LoadBeanComponentElement(XmlNode node)
        {
            BeanComponentConfigureInfo componentInfo = new BeanComponentConfigureInfo();

            XmlAttributeCollection attrCollection = node.Attributes;
            for (int n = 0; null != attrCollection && n < attrCollection.Count; ++n)
            {
                XmlAttribute attr = attrCollection[n];
                switch (attr.Name)
                {
                    case BeanConfigureNodeAttributeName.ReferenceName:
                        componentInfo.ReferenceName = attr.Value;
                        break;
                    case BeanConfigureNodeAttributeName.ReferenceType:
                        componentInfo.ReferenceType = NovaEngine.Utility.Assembly.GetType(attr.Value);
                        Debugger.Assert(componentInfo.ReferenceType, "Invalid bean component class type.");
                        break;
                    case BeanConfigureNodeAttributeName.Priority:
                        componentInfo.Priority = int.Parse(attr.Value);
                        break;
                    case BeanConfigureNodeAttributeName.ActivationOn:
                        componentInfo.ActivationBehaviourType = NovaEngine.Utility.Convertion.GetEnumFromName<AspectBehaviourType>(attr.Value);
                        break;
                }
            }

            return componentInfo;
        }
    }
}
