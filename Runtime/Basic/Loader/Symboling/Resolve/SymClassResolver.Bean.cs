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
using SystemFieldInfo = System.Reflection.FieldInfo;
using SystemPropertyInfo = System.Reflection.PropertyInfo;
using SystemMethodInfo = System.Reflection.MethodInfo;

namespace GameEngine.Loader.Symboling
{
    /// <summary>
    /// 标记对象的解析类，对基础对象类的注入标记进行解析和构建
    /// </summary>
    internal static partial class SymClassResolver
    {
        /// <summary>
        /// 从指定的类标记创建类对象的Bean实例列表
        /// </summary>
        /// <param name="symClass">类标记对象</param>
        /// <returns>若创建Bean对象成功则返回对应实例列表，否则返回null</returns>
        private static IList<Bean> CreateBeanObjectsFromSymClass(SymClass symClass)
        {
            // 不可实例化的类类型，无需进行Bean实体的构建
            if (false == symClass.IsInstantiate)
            {
                return null;
            }

            string defaultBeanName = symClass.DefaultBeanName;
            IDictionary<string, Bean> beanMaps = new Dictionary<string, Bean>();

            IList<OnBeanConfiguredAttribute> configureAttrs = symClass.GetAttributes<OnBeanConfiguredAttribute>();
            for (int n = 0; null != configureAttrs && n < configureAttrs.Count; ++n)
            {
                OnBeanConfiguredAttribute attr = configureAttrs[n];

                string beanName = null;
                if (string.IsNullOrEmpty(attr.BeanName))
                {
                    beanName = defaultBeanName;
                }
                else
                {
                    beanName = attr.BeanName;
                }

                if (beanMaps.ContainsKey(beanName))
                {
                    Debugger.Warn("The target bean name '{0}' was already exist with symbol class '{1}', repeat registed it failed.", beanName, symClass.FullName);
                    continue;
                }

                Bean bean = new Bean(symClass);

                bean.BeanName = beanName;
                bean.Singleton = attr.Singleton;

                // 标记该Bean实例是通过类信息加载的
                bean.FromConfigure = false;

                beanMaps.Add(beanName, bean);
            }

            // 没有默认配置的Bean信息，则自行添加一个默认实例
            if (beanMaps.Count <= 0)
            {
                Bean bean = new Bean(symClass);

                bean.BeanName = defaultBeanName;
                bean.Singleton = false;

                // 标记该Bean实例是通过类信息加载的
                bean.FromConfigure = false;

                beanMaps.Add(bean.BeanName, bean);
            }

            // 如果没有对实体进行配置，则添加一个默认的实例
            if (false == beanMaps.ContainsKey(defaultBeanName))
            {
                Bean bean = new Bean(symClass);

                bean.BeanName = defaultBeanName;
                bean.Singleton = false;

                // 标记该Bean实例是通过类信息加载的
                bean.FromConfigure = false;

                beanMaps.Add(defaultBeanName, bean);
            }

            IList<SystemAttribute> classTypeAttrs = symClass.Attributes;
            for (int n = 0; null != classTypeAttrs && n < classTypeAttrs.Count; ++n)
            {
                SystemAttribute attr = classTypeAttrs[n];
                SystemType attrType = attr.GetType();
                if (typeof(EntityActivationComponentAttribute) == attrType)
                {
                    Debugger.Assert(typeof(CEntity).IsAssignableFrom(symClass.ClassType), "Invalid symbol class type '{0}'.", symClass.FullName);

                    EntityActivationComponentAttribute _attr = (EntityActivationComponentAttribute) attr;

                    if (false == beanMaps.TryGetValue(defaultBeanName, out Bean bean))
                    {
                        Debugger.Warn("Could not found any bean instance with default name '{0}', resolved attribute configure failed.", defaultBeanName);
                        continue;
                    }

                    BeanComponent component = new BeanComponent(bean);
                    component.ReferenceClassType = _attr.ReferenceType;
                    component.ReferenceBeanName = _attr.ReferenceName;
                    component.Priority = _attr.Priority;
                    component.ActivationBehaviourType = _attr.ActivationBehaviourType;
                    bean.AddComponent(component);
                }
                else if (typeof(EntityActivationComponentOfTargetAttribute) == attrType)
                {
                    Debugger.Assert(typeof(CEntity).IsAssignableFrom(symClass.ClassType), "Invalid symbol class type '{0}'.", symClass.FullName);

                    EntityActivationComponentOfTargetAttribute _attr = (EntityActivationComponentOfTargetAttribute) attr;

                    if (false == beanMaps.TryGetValue(_attr.TargetBeanName, out Bean bean))
                    {
                        Debugger.Warn("Could not found any bean instance with default name '{0}', resolved attribute configure failed.", _attr.TargetBeanName);
                        continue;
                    }

                    BeanComponent component = new BeanComponent(bean);
                    component.ReferenceClassType = _attr.ReferenceType;
                    component.ReferenceBeanName = _attr.ReferenceName;
                    component.Priority = _attr.Priority;
                    component.ActivationBehaviourType = _attr.ActivationBehaviourType;
                    bean.AddComponent(component);
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

                            if (false == beanMaps.TryGetValue(defaultBeanName, out Bean bean))
                            {
                                Debugger.Warn("Could not found any bean instance with default name '{0}', resolved field configure failed.", defaultBeanName);
                                continue;
                            }

                            if (/*string.IsNullOrEmpty(_attr.ReferenceName) || */null == _attr.ReferenceType)
                            {
                                Debugger.Warn("Could not found any reference type or value with target bean field '{0}', resolved field configure failed.", symField.FieldName);
                                continue;
                            }

                            BeanField beanField = new BeanField(bean);
                            beanField.FieldName = symField.FieldName;
                            beanField.ReferenceClassType = _attr.ReferenceType;
                            beanField.ReferenceBeanName = _attr.ReferenceName;
                            bean.AddField(beanField);
                        }
                        else if (typeof(OnBeanAutowiredOfTargetAttribute) == attrType)
                        {
                            OnBeanAutowiredOfTargetAttribute _attr = (OnBeanAutowiredOfTargetAttribute) attr;

                            if (false == beanMaps.TryGetValue(_attr.BeanName, out Bean bean))
                            {
                                Debugger.Warn("Could not found any bean instance with default name '{0}', resolved field configure failed.", _attr.BeanName);
                                continue;
                            }

                            if (/*string.IsNullOrEmpty(_attr.ReferenceName) || */null == _attr.ReferenceType)
                            {
                                Debugger.Warn("Could not found any reference type or value with target bean field '{0}', resolved field configure failed.", symField.FieldName);
                                continue;
                            }

                            BeanField beanField = new BeanField(bean);
                            beanField.FieldName = symField.FieldName;
                            beanField.ReferenceClassType = _attr.ReferenceType;
                            beanField.ReferenceBeanName = _attr.ReferenceName;
                            bean.AddField(beanField);
                        }
                    }
                }
            }

            return NovaEngine.Utility.Collection.ToListForValues<string, Bean>(beanMaps);
        }
    }
}
