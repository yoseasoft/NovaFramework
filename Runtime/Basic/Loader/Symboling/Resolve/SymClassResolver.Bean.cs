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

using System.Collections.Generic;

using SystemType = System.Type;
using SystemAttribute = System.Attribute;

namespace GameEngine.Loader.Symboling
{
    /// <summary>
    /// 标记对象的解析类，对基础对象类的注入标记进行解析和构建
    /// </summary>
    internal static partial class SymClassResolver
    {
        /// <summary>
        /// 从指定的类标记创建类对象的默认Bean对象实例
        /// </summary>
        /// <param name="symClass">类标记对象</param>
        /// <returns>若创建Bean对象成功则返回对应实例，否则返回null</returns>
        private static Bean CreateDefaultBeanObjectFromSymClass(SymClass symClass)
        {
            // 不可实例化的类类型，无需进行Bean实体的构建
            if (false == symClass.IsInstantiate)
            {
                return null;
            }

            string defaultBeanName = symClass.DefaultBeanName;

            // 默认配置的Bean信息
            Bean defaultBeanInstance = new Bean(symClass);

            defaultBeanInstance.BeanName = defaultBeanName;
            defaultBeanInstance.Singleton = false;

            IList<SystemAttribute> classTypeAttrs = symClass.Attributes;
            for (int n = 0; null != classTypeAttrs && n < classTypeAttrs.Count; ++n)
            {
                SystemAttribute attr = classTypeAttrs[n];
                SystemType attrType = attr.GetType();
                if (typeof(EntityActivationComponentAttribute) == attrType)
                {
                    Debugger.Assert(typeof(CEntity).IsAssignableFrom(symClass.ClassType), "Invalid symbol class type '{0}'.", symClass.FullName);

                    EntityActivationComponentAttribute _attr = (EntityActivationComponentAttribute) attr;

                    BeanComponent component = new BeanComponent(defaultBeanInstance);
                    component.ReferenceClassType = _attr.ReferenceType;
                    component.ReferenceBeanName = _attr.ReferenceName;
                    component.Priority = _attr.Priority;
                    component.ActivationBehaviourType = _attr.ActivationBehaviourType;
                    defaultBeanInstance.AddComponent(component);
                }
            }

            IDictionary<string, SymField> classTypeFields = symClass.Fields;
            IEnumerator<KeyValuePair<string, SymField>> fieldInfoEnumerator = symClass.GetFieldEnumerator();
            if (null != fieldInfoEnumerator)
            {
                while (fieldInfoEnumerator.MoveNext())
                {
                    SymField symField = fieldInfoEnumerator.Current.Value;

                    IList<SystemAttribute> fieldTypeAttrs = symClass.Attributes;
                    for (int n = 0; null != fieldTypeAttrs && n < fieldTypeAttrs.Count; ++n)
                    {
                        SystemAttribute attr = classTypeAttrs[n];
                        SystemType attrType = attr.GetType();

                        if (typeof(OnBeanAutowiredAttribute) == attrType)
                        {
                            OnBeanAutowiredAttribute _attr = (OnBeanAutowiredAttribute) attr;

                            if (null == _attr.ReferenceType && string.IsNullOrEmpty(_attr.ReferenceName))
                            {
                                Debugger.Warn("Could not found any reference type or value with target bean field '{0}', resolved field configure failed.", symField.FieldName);
                                continue;
                            }

                            BeanField beanField = new BeanField(defaultBeanInstance);
                            beanField.FieldName = symField.FieldName;
                            beanField.ReferenceClassType = _attr.ReferenceType;
                            beanField.ReferenceBeanName = _attr.ReferenceName;
                            defaultBeanInstance.AddField(beanField);
                        }
                    }
                }
            }

            return defaultBeanInstance;
        }
    }
}
