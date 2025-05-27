/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyring (C) 2023 - 2024, Guangzhou Shiyue Network Technology Co., Ltd.
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
using System.Reflection;

using SystemType = System.Type;
using SystemAttribute = System.Attribute;
using SystemFieldInfo = System.Reflection.FieldInfo;
using SystemPropertyInfo = System.Reflection.PropertyInfo;
using SystemMethodInfo = System.Reflection.MethodInfo;
using SystemBindingFlags = System.Reflection.BindingFlags;

namespace GameEngine.Loader.Symboling
{
    /// <summary>
    /// 标记对象的解析类，对基础对象类的注入标记进行解析和构建
    /// </summary>
    internal static partial class SymClassResolver
    {
        /// <summary>
        /// 从指定的配置信息创建类对象的Bean
        /// </summary>
        /// <param name="symClass">类标记实例</param>
        /// <param name="beanConfigureInfo">对象配置信息</param>
        /// <returns>若创建Bean对象成功则返回对应实例，否则返回null</returns>
        private static Bean CreateBeanObjectFromConfigureInfo(SymClass symClass, Configuring.BeanConfigureInfo beanConfigureInfo)
        {
            // 不可实例化的类类型，不能进行Bean实体的构建
            if (false == symClass.IsInstantiate)
            {
                Debugger.Warn("The target symbol class '{0}' was not instantiable type, created bean object failed.", symClass.FullName);
                return null;
            }

            Bean bean = new Bean(symClass);

            bean.BeanName = beanConfigureInfo.Name;
            bean.Singleton = beanConfigureInfo.Singleton;
            bean.Inherited = beanConfigureInfo.Inherited;

            // 标识该Bean实例是通过配置文件加载的
            bean.FromConfigure = true;

            // 递归所有的父类，把具备继承标识的配置都加载进来
            SystemType parentType = symClass.ClassType;
            Configuring.BeanConfigureInfo parentBeanInfo = beanConfigureInfo;
            while (null != parentType)
            {
                LoadClassBeanFromConfigure(symClass, parentBeanInfo, bean);

                parentType = parentType.BaseType;
                if (null != parentBeanInfo && false == string.IsNullOrEmpty(parentBeanInfo.ParentName))
                {
                    parentBeanInfo = CodeLoader.GetConfigureBeanByName(parentBeanInfo.ParentName);
                }
                else
                {
                    parentBeanInfo = null; // 重置
                    IList<Configuring.BeanConfigureInfo> list = CodeLoader.GetConfigureBeanByType(parentType);
                    if (null != list && list.Count > 0)
                    {
                        if (list.Count > 1)
                        {
                            Debugger.Warn("The bean info '{0}' has multiple parent object with target type '{1}', only chosed first bean and used it.",
                                    parentBeanInfo.Name, NovaEngine.Utility.Text.ToString(parentType));
                        }
                        parentBeanInfo = list[0];
                    }
                }

                if (null != parentBeanInfo)
                {
                    if (false == parentBeanInfo.Inherited)
                    {
                        parentBeanInfo = null;
                    }
                }
            }

            return bean;
        }

        private static void LoadClassBeanFromConfigure(SymClass symClass, Configuring.BeanConfigureInfo configureBeanInfo, Bean bean)
        {
            // 若该实体配置信息不支持继承，则会将其赋值为null
            // 此处将忽略该bean的配置信息
            if (null == configureBeanInfo)
            {
                return;
            }

            IEnumerator<KeyValuePair<string, Configuring.BeanFieldConfigureInfo>> fieldInfoEnumerator = configureBeanInfo.GetFieldInfoEnumerator();
            if (null != fieldInfoEnumerator)
            {
                while (fieldInfoEnumerator.MoveNext())
                {
                    Configuring.BeanFieldConfigureInfo configureBeanFieldInfo = fieldInfoEnumerator.Current.Value;

                    Symboling.SymField symField = symClass.GetFieldByName(configureBeanFieldInfo.FieldName);
                    if (null == symField)
                    {
                        Debugger.Error("Could not found any symbol field with target name '{0}', please rechecked your configure bean '{1}' and repair it.",
                                configureBeanFieldInfo.FieldName, configureBeanInfo.Name);
                        continue;
                    }

                    if (bean.HasFieldByName(configureBeanFieldInfo.FieldName))
                    {
                        Debugger.Warn("The target bean field '{0}' was already exist within bean object '{1}', repeat added it failed.",
                                configureBeanFieldInfo.FieldName, bean.BeanName);
                        continue;
                    }

                    BeanField beanField = new BeanField(bean);
                    beanField.FieldName = configureBeanFieldInfo.FieldName;

                    if (false == string.IsNullOrEmpty(configureBeanFieldInfo.ReferenceName))
                    {
                        Configuring.BeanConfigureInfo referenceConfigureBeanInfo = CodeLoader.GetConfigureBeanByName(configureBeanFieldInfo.ReferenceName);
                        Debugger.Assert(null != referenceConfigureBeanInfo, "Invalid configure reference name '{0}'.", configureBeanFieldInfo.ReferenceName);

                        beanField.ReferenceBeanName = configureBeanFieldInfo.ReferenceName;
                    }
                    else if (null != configureBeanFieldInfo.ReferenceType)
                    {
                        beanField.ReferenceClassType = configureBeanFieldInfo.ReferenceType;
                    }
                    else
                    {
                        Debugger.Warn("The defination reference value cannot be null, setted the field symbol failed.");
                        continue;
                    }

                    bean.AddField(beanField);
                }
            }

            for (int n = 0; n < configureBeanInfo.GetComponentInfoCount(); ++n)
            {
                Configuring.BeanComponentConfigureInfo configureBeanComponentInfo = configureBeanInfo.GetComponentInfo(n);

                BeanComponent beanComponent = new BeanComponent(bean);

                if (false == string.IsNullOrEmpty(configureBeanComponentInfo.ReferenceName))
                {
                    Configuring.BeanConfigureInfo referenceConfigureBeanInfo = CodeLoader.GetConfigureBeanByName(configureBeanComponentInfo.ReferenceName);
                    Debugger.Assert(null != referenceConfigureBeanInfo, "Invalid configure reference name '{0}'.", configureBeanComponentInfo.ReferenceName);

                    beanComponent.ReferenceBeanName = configureBeanComponentInfo.ReferenceName;
                }
                else if (null != configureBeanComponentInfo.ReferenceType)
                {
                    beanComponent.ReferenceClassType = configureBeanComponentInfo.ReferenceType;
                }
                else
                {
                    Debugger.Warn("The defination reference value cannot be null, setted the component info failed.");
                    continue;
                }

                beanComponent.Priority = configureBeanComponentInfo.Priority;
                beanComponent.ActivationBehaviourType = configureBeanComponentInfo.ActivationBehaviourType;
                bean.AddComponent(beanComponent);
            }
        }
    }
}
