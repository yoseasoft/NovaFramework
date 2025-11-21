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

using SystemType = System.Type;

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
        [CodeLoader.OnConfigureResolvingCallback(XmlNodeType.Element, BeanConfigureNodeName.Bean)]
        private static BaseConfigureInfo LoadBeanElement(XmlNode node)
        {
            BeanConfigureInfo beanInfo = new BeanConfigureInfo();

            XmlAttributeCollection attrCollection = node.Attributes;
            for (int n = 0; null != attrCollection && n < attrCollection.Count; ++n)
            {
                XmlAttribute attr = attrCollection[n];
                switch (attr.Name)
                {
                    case BeanConfigureNodeAttributeName.K_NAME:
                        beanInfo.Name = attr.Value;
                        break;
                    case BeanConfigureNodeAttributeName.K_CLASS_TYPE:
                        beanInfo.ClassType = NovaEngine.Utility.Assembly.GetType(attr.Value);
                        Debugger.Assert(beanInfo.ClassType, "Invalid bean class type.");
                        break;
                    case BeanConfigureNodeAttributeName.K_PARENT_NAME:
                        beanInfo.ParentName = attr.Value;
                        break;
                    case BeanConfigureNodeAttributeName.K_SINGLETON:
                        beanInfo.Singleton = bool.Parse(attr.Value);
                        break;
                    case BeanConfigureNodeAttributeName.K_INHERITED:
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
                    Debugger.Warn("Cannot resolve target configure node '{0}' from bean info '{1}', loaded bean configure failed.", child.Name, beanInfo.Name);
                }
            }

            // 验证：有组件成员的bean，必须是entity类型
            if (beanInfo.GetComponentInfoCount() > 0)
            {
                if (false == beanInfo.ClassType.IsSubclassOf(typeof(CEntity)))
                {
                    Debugger.Warn("Could not add component to target bean class '{0}', loaded bean '{1}' failed.",
                            NovaEngine.Utility.Text.ToString(beanInfo.ClassType), beanInfo.Name);
                    return null;
                }
            }

            Debugger.Info(LogGroupTag.CodeLoader, "Load bean configure '{%s}' succeed.", CodeLoaderObject.ToString(beanInfo));
            return beanInfo;
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
                    case BeanConfigureNodeAttributeName.K_NAME:
                        fieldInfo.FieldName = attr.Value;
                        break;
                    case BeanConfigureNodeAttributeName.K_REFERENCE_NAME:
                        fieldInfo.ReferenceName = attr.Value;
                        break;
                    case BeanConfigureNodeAttributeName.K_REFERENCE_TYPE:
                        fieldInfo.ReferenceType = NovaEngine.Utility.Assembly.GetType(attr.Value);
                        Debugger.Assert(fieldInfo.ReferenceType, "Invalid bean field class type.");
                        break;
                    case BeanConfigureNodeAttributeName.K_REFERENCE_VALUE:
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
                    case BeanConfigureNodeAttributeName.K_NAME:
                        propertyInfo.PropertyName = attr.Value;
                        break;
                    case BeanConfigureNodeAttributeName.K_REFERENCE_NAME:
                        propertyInfo.ReferenceName = attr.Value;
                        break;
                    case BeanConfigureNodeAttributeName.K_REFERENCE_TYPE:
                        propertyInfo.ReferenceType = NovaEngine.Utility.Assembly.GetType(attr.Value);
                        Debugger.Assert(propertyInfo.ReferenceType, "Invalid bean property class type.");
                        break;
                    case BeanConfigureNodeAttributeName.K_REFERENCE_VALUE:
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
                    case BeanConfigureNodeAttributeName.K_REFERENCE_NAME:
                        componentInfo.ReferenceName = attr.Value;
                        break;
                    case BeanConfigureNodeAttributeName.K_REFERENCE_TYPE:
                        componentInfo.ReferenceType = NovaEngine.Utility.Assembly.GetType(attr.Value);
                        Debugger.Assert(componentInfo.ReferenceType, "Invalid bean component class type.");
                        break;
                    case BeanConfigureNodeAttributeName.K_PRIORITY:
                        componentInfo.Priority = int.Parse(attr.Value);
                        break;
                    case BeanConfigureNodeAttributeName.K_ACTIVATION_ON:
                        componentInfo.ActivationBehaviourType = NovaEngine.Utility.Convertion.GetEnumFromName<AspectBehaviourType>(attr.Value);
                        break;
                }
            }

            return componentInfo;
        }
    }
}
