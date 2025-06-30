/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyright (C) 2023 - 2024, Guangzhou Shiyue Network Technology Co., Ltd.
/// Copyright (C) 2024 - 2025, Hurley, Independent Studio.
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
using System.Reflection;

using SystemType = System.Type;
using SystemStringBuilder = System.Text.StringBuilder;

namespace GameEngine.Loader.Symboling
{
    /// <summary>
    /// 通用对象类的标记数据的结构信息
    /// </summary>
    public class SymClass : SymBase
    {
        /// <summary>
        /// 对象类的名称
        /// </summary>
        private string m_className;
        /// <summary>
        /// 对象类的完整名称
        /// </summary>
        private string m_fullName;
        /// <summary>
        /// 对象类的类型
        /// </summary>
        private SystemType m_classType;
        /// <summary>
        /// 对象类的父类类型
        /// </summary>
        private SystemType m_baseType;

        /// <summary>
        /// 对象类的默认Bean实例名称，该名称由类标记对象自动生成，不可主动赋值
        /// </summary>
        private string m_defaultBeanName;

        /// <summary>
        /// 对象是否为接口类型
        /// </summary>
        private bool m_isInterface;
        /// <summary>
        /// 对象是否为类类型
        /// </summary>
        private bool m_isClass;
        /// <summary>
        /// 对象是否为抽象类型
        /// </summary>
        private bool m_isAbstract;
        /// <summary>
        /// 对象是否为静态类型
        /// </summary>
        private bool m_isStatic;
        /// <summary>
        /// 对象是否为可实例化的类型
        /// </summary>
        private bool m_isInstantiate;

        /// <summary>
        /// 对象类包含的特性信息
        /// </summary>
        private IList<SystemType> m_featureTypes;
        /// <summary>
        /// 对象类包含的接口信息
        /// </summary>
        private IList<SystemType> m_interfaceTypes;

        /// <summary>
        /// 对象类包含的字段信息
        /// </summary>
        private IDictionary<string, SymField> m_fields;
        /// <summary>
        /// 对象类包含的属性信息
        /// </summary>
        private IDictionary<string, SymProperty> m_properties;
        /// <summary>
        /// 对象类包含的函数信息
        /// </summary>
        private IDictionary<string, SymMethod> m_methods;

        /// <summary>
        /// 对象类的Bean实例列表
        /// </summary>
        private IDictionary<string, Bean> m_beans;

        public SystemType ClassType
        {
            get { return m_classType; }
            internal set
            {
                m_classType = value;

                // 对象类的名称及它的完整类名
                m_className = m_classType.Name;
                m_fullName = NovaEngine.Utility.Text.GetFullName(m_classType);

                m_baseType = m_classType.BaseType;

                m_isInterface = m_classType.IsInterface;
                m_isClass = m_classType.IsClass;
                m_isAbstract = m_classType.IsAbstract;
                m_isStatic = NovaEngine.Utility.Reflection.IsTypeOfStaticClass(m_classType);
                m_isInstantiate = NovaEngine.Utility.Reflection.IsTypeOfInstantiableClass(m_classType);
            }
        }

        public string DefaultBeanName
        {
            get
            {
                if (null == m_defaultBeanName)
                {
                    // 前缀+类的完整名称
                    m_defaultBeanName = string.Format("__internal_{0}", NovaEngine.Utility.Text.GetFullName(m_classType));
                }

                return m_defaultBeanName;
            }
        }

        public string ClassName => m_className;
        public string FullName => m_fullName;
        public SystemType BaseType => m_baseType;

        public bool IsInterface => m_isInterface;
        public bool IsClass => m_isClass;
        public bool IsAbstract => m_isAbstract;
        public bool IsStatic => m_isStatic;
        public bool IsInstantiate => m_isInstantiate;

        public IList<SystemType> FeatureTypes => m_featureTypes;
        public IList<SystemType> InterfaceTypes => m_interfaceTypes;

        public IDictionary<string, SymField> Fields => m_fields;
        public IDictionary<string, SymProperty> Properties => m_properties;
        public IDictionary<string, SymMethod> Methods => m_methods;

        public SymClass() : base() { }

        ~SymClass()
        {
            RemoveAllFeatureTypes();
            RemoveAllInterfaceTypes();

            RemoveAllFields();
            RemoveAllProperties();
            RemoveAllMethods();

            RemoveAllBeans();
        }

        #region 类标记对象的特性列表相关访问接口函数

        /// <summary>
        /// 新增指定的类特性实例到当前的类标记对象中
        /// </summary>
        /// <param name="featureType">类特性实例</param>
        public void AddFeatureType(SystemType featureType)
        {
            if (null == m_featureTypes)
            {
                m_featureTypes = new List<SystemType>();
            }

            if (m_featureTypes.Contains(featureType))
            {
                Debugger.Warn("The symbol class '{%f}' feature type '{%f}' was already exist, repeat added it failed.", m_classType, featureType);
                return;
            }

            m_featureTypes.Add(featureType);
        }

        /// <summary>
        /// 检测当前类标记中是否存在指定类型的特性实例
        /// </summary>
        /// <param name="featureType">特性类型</param>
        /// <returns>若存在目标特性实例则返回true，否则返回false</returns>
        public bool HasFeatureType(SystemType featureType)
        {
            if (null == m_featureTypes)
            {
                return false;
            }

            return m_featureTypes.Contains(featureType);
        }

        /// <summary>
        /// 从当前的类标记对象中移除指定类型的类特性实例
        /// </summary>
        /// <param name="featureType">特性类型</param>
        public void RemoveFeatureType(SystemType featureType)
        {
            if (null == m_featureTypes)
            {
                return;
            }

            if (false == m_featureTypes.Contains(featureType))
            {
                Debugger.Warn("Could not found any feature type '{%f}' from target symbol class '{%f}', removed it failed.", featureType, m_classType);
                return;
            }

            m_featureTypes.Remove(featureType);
        }

        /// <summary>
        /// 从当前的类标记对象中移除所有类特性实例
        /// </summary>
        private void RemoveAllFeatureTypes()
        {
            m_featureTypes?.Clear();
            m_featureTypes = null;
        }

        #endregion

        #region 类标记对象的接口列表相关访问接口函数

        /// <summary>
        /// 新增指定的类接口实例到当前的类标记对象中
        /// </summary>
        /// <param name="interfaceType">类接口实例</param>
        public void AddInterfaceType(SystemType interfaceType)
        {
            if (null == m_interfaceTypes)
            {
                m_interfaceTypes = new List<SystemType>();
            }

            if (m_interfaceTypes.Contains(interfaceType))
            {
                Debugger.Warn("The symbol class '{%f}' interface type '{%f}' was already exist, repeat added it failed.", m_classType, interfaceType);
                return;
            }

            m_interfaceTypes.Add(interfaceType);
        }

        /// <summary>
        /// 检测当前类标记中是否存在指定类型的接口实例
        /// </summary>
        /// <param name="interfaceType">接口类型</param>
        /// <returns>若存在目标接口实例则返回true，否则返回false</returns>
        public bool HasInterfaceType(SystemType interfaceType)
        {
            if (null == m_interfaceTypes)
            {
                return false;
            }

            return m_interfaceTypes.Contains(interfaceType);
        }

        /// <summary>
        /// 从当前的类标记对象中移除指定类型的接口实例
        /// </summary>
        /// <param name="interfaceType">接口类型</param>
        public void RemoveInterfaceType(SystemType interfaceType)
        {
            if (null == m_interfaceTypes)
            {
                return;
            }

            if (false == m_interfaceTypes.Contains(interfaceType))
            {
                Debugger.Warn("Could not found any interface type '{%f}' from target symbol class '{%f}', removed it failed.", interfaceType, m_classType);
                return;
            }

            m_interfaceTypes.Remove(interfaceType);
        }

        /// <summary>
        /// 从当前的类标记对象中移除所有类接口实例
        /// </summary>
        private void RemoveAllInterfaceTypes()
        {
            m_interfaceTypes?.Clear();
            m_interfaceTypes = null;
        }

        #endregion

        #region 类标记对象的字段列表相关访问接口函数

        /// <summary>
        /// 新增指定的类字段实例到当前的类标记对象中
        /// </summary>
        /// <param name="interfaceType">类字段实例</param>
        public void AddField(SymField field)
        {
            if (null == m_fields)
            {
                m_fields = new Dictionary<string, SymField>();
            }

            if (m_fields.ContainsKey(field.FieldName))
            {
                Debugger.Warn("The symbol class '{0}' field '{1}' was already exist, repeat added it failed.",
                        NovaEngine.Utility.Text.ToString(m_classType), field.FieldName);
                return;
            }

            m_fields.Add(field.FieldName, field);
        }

        /// <summary>
        /// 检测当前类标记中是否存在指定名称的字段实例
        /// </summary>
        /// <param name="fieldName">字段名称</param>
        /// <returns>若存在目标字段实例则返回true，否则返回false</returns>
        public bool HasFieldByName(string fieldName)
        {
            Debugger.Assert(false == string.IsNullOrEmpty(fieldName), "Invalid arguments.");

            if (null == m_fields)
            {
                return false;
            }

            return m_fields.ContainsKey(fieldName);
        }

        /// <summary>
        /// 获取类标记中字段实例的数量
        /// </summary>
        /// <returns>返回类标记中字段实例的数量</returns>
        public int GetFieldCount()
        {
            if (null == m_fields)
            {
                return 0;
            }

            return m_fields.Count;
        }

        /// <summary>
        /// 通过指定的字段名称查找对应的字段标记对象实例
        /// </summary>
        /// <param name="fieldName">字段名称</param>
        /// <returns>若查找标记对象成功则返回该实例，否则返回null</returns>
        public SymField GetFieldByName(string fieldName)
        {
            if (null == m_fields || false == m_fields.ContainsKey(fieldName))
            {
                return null;
            }

            return m_fields[fieldName];
        }

        /// <summary>
        /// 尝试通过指定的字段名称，获取对应的字段标记对象实例
        /// </summary>
        /// <param name="fieldName">字段名称</param>
        /// <param name="symField">字段标记对象实例</param>
        /// <returns>若查找标记对象成功则返回true，否则返回false</returns>
        public bool TryGetFieldByName(string fieldName, out SymField symField)
        {
            if (null == m_fields)
            {
                symField = null;
                return false;
            }

            return m_fields.TryGetValue(fieldName, out symField);
        }

        /// <summary>
        /// 获取类标记对象的字段列表
        /// </summary>
        /// <returns>返回类标记对象的字段列表</returns>
        public IList<SymField> GetAllFields()
        {
            return NovaEngine.Utility.Collection.ToListForValues<string, SymField>(m_fields);
        }

        /// <summary>
        /// 获取类标记对象的字段迭代器
        /// </summary>
        /// <returns>返回类标记对象的字段迭代器</returns>
        public IEnumerator<KeyValuePair<string, SymField>> GetFieldEnumerator()
        {
            return m_fields?.GetEnumerator();
        }

        /// <summary>
        /// 从当前的类标记对象中移除指定的类字段实例
        /// </summary>
        /// <param name="field">类字段实例</param>
        public void RemoveField(SymField field)
        {
            RemoveField(field.FieldName);
        }

        /// <summary>
        /// 从当前的类标记对象中移除指定名称的字段实例
        /// </summary>
        /// <param name="fieldName">字段名称</param>
        public void RemoveField(string fieldName)
        {
            if (null == m_fields)
            {
                return;
            }

            if (false == m_fields.ContainsKey(fieldName))
            {
                Debugger.Warn("Could not found any field instance '{0}' from target symbol class '{1}', removed it failed.",
                        fieldName, NovaEngine.Utility.Text.ToString(m_classType));
                return;
            }

            m_fields.Remove(fieldName);
        }

        /// <summary>
        /// 从当前的类标记对象中移除所有类字段实例
        /// </summary>
        private void RemoveAllFields()
        {
            m_fields?.Clear();
            m_fields = null;
        }

        #endregion

        #region 类标记对象的属性列表相关访问接口函数

        /// <summary>
        /// 新增指定的类属性实例到当前的类标记对象中
        /// </summary>
        /// <param name="property">类属性实例</param>
        public void AddProperty(SymProperty property)
        {
            if (null == m_properties)
            {
                m_properties = new Dictionary<string, SymProperty>();
            }

            if (m_properties.ContainsKey(property.PropertyName))
            {
                Debugger.Warn("The symbol class '{0}' property '{1}' was already exist, repeat added it failed.",
                        NovaEngine.Utility.Text.ToString(m_classType), property.PropertyName);
                return;
            }

            m_properties.Add(property.PropertyName, property);
        }

        /// <summary>
        /// 获取类标记中属性实例的数量
        /// </summary>
        /// <returns>返回类标记中属性实例的数量</returns>
        public int GetPropertyCount()
        {
            if (null == m_properties)
            {
                return 0;
            }

            return m_properties.Count;
        }

        /// <summary>
        /// 尝试通过指定的属性名称，获取对应的属性标记对象实例
        /// </summary>
        /// <param name="propertyName">属性名称</param>
        /// <param name="symProperty">属性标记对象实例</param>
        /// <returns>若查找标记对象成功则返回true，否则返回false</returns>
        public bool TryGetPropertyByName(string propertyName, out SymProperty symProperty)
        {
            if (null == m_properties)
            {
                symProperty = null;
                return false;
            }

            return m_properties.TryGetValue(propertyName, out symProperty);
        }

        /// <summary>
        /// 获取类标记对象的属性列表
        /// </summary>
        /// <returns>返回类标记对象的属性列表</returns>
        public IList<SymProperty> GetAllProperties()
        {
            return NovaEngine.Utility.Collection.ToListForValues<string, SymProperty>(m_properties);
        }

        /// <summary>
        /// 获取类标记对象的属性迭代器
        /// </summary>
        /// <returns>返回类标记对象的属性迭代器</returns>
        public IEnumerator<KeyValuePair<string, SymProperty>> GetPropertyEnumerator()
        {
            return m_properties?.GetEnumerator();
        }

        /// <summary>
        /// 从当前的类标记对象中移除指定的类属性实例
        /// </summary>
        /// <param name="property">类属性实例</param>
        public void RemoveProperty(SymProperty property)
        {
            RemoveField(property.PropertyName);
        }

        /// <summary>
        /// 从当前的类标记对象中移除指定名称的属性实例
        /// </summary>
        /// <param name="propertyName">属性名称</param>
        public void RemoveProperty(string propertyName)
        {
            if (null == m_properties)
            {
                return;
            }

            if (false == m_properties.ContainsKey(propertyName))
            {
                Debugger.Warn("Could not found any property instance '{0}' from target symbol class '{1}', removed it failed.",
                        propertyName, NovaEngine.Utility.Text.ToString(m_classType));
                return;
            }

            m_properties.Remove(propertyName);
        }

        /// <summary>
        /// 从当前的类标记对象中移除所有类属性实例
        /// </summary>
        private void RemoveAllProperties()
        {
            m_properties?.Clear();
            m_properties = null;
        }

        #endregion

        #region 类标记对象的函数列表相关访问接口函数

        /// <summary>
        /// 新增指定的类函数实例到当前的类标记对象中
        /// </summary>
        /// <param name="method">类函数实例</param>
        public void AddMethod(SymMethod method)
        {
            if (null == m_methods)
            {
                m_methods = new Dictionary<string, SymMethod>();
            }

            if (m_methods.ContainsKey(method.FullName))
            {
                Debugger.Warn("The symbol class '{0}' method '{1}' was already exist, repeat added it failed.",
                        NovaEngine.Utility.Text.ToString(m_classType), method.FullName);
                return;
            }

            m_methods.Add(method.FullName, method);
        }

        /// <summary>
        /// 获取类标记中函数实例的数量
        /// </summary>
        /// <returns>返回类标记中函数实例的数量</returns>
        public int GetMethodCount()
        {
            if (null == m_methods)
            {
                return 0;
            }

            return m_methods.Count;
        }

        /// <summary>
        /// 尝试通过指定的函数名称，获取对应的函数标记对象实例
        /// </summary>
        /// <param name="methodName">函数名称</param>
        /// <param name="symMethod">函数标记对象实例</param>
        /// <returns>若查找标记对象成功则返回true，否则返回false</returns>
        public bool TryGetMethodByName(string methodName, out SymMethod symMethod)
        {
            if (null == m_methods)
            {
                symMethod = null;
                return false;
            }

            return m_methods.TryGetValue(methodName, out symMethod);
        }

        /// <summary>
        /// 获取类标记对象的函数列表
        /// </summary>
        /// <returns>返回类标记对象的函数列表</returns>
        public IList<SymMethod> GetAllMethods()
        {
            return NovaEngine.Utility.Collection.ToListForValues<string, SymMethod>(m_methods);
        }

        /// <summary>
        /// 获取类标记对象的函数迭代器
        /// </summary>
        /// <returns>返回类标记对象的函数迭代器</returns>
        public IEnumerator<KeyValuePair<string, SymMethod>> GetMethodEnumerator()
        {
            return m_methods?.GetEnumerator();
        }

        /// <summary>
        /// 从当前的类标记对象中移除指定的类函数实例
        /// </summary>
        /// <param name="method">类函数实例</param>
        public void RemoveMethod(SymMethod method)
        {
            RemoveField(method.MethodName);
        }

        /// <summary>
        /// 从当前的类标记对象中移除指定名称的函数实例
        /// </summary>
        /// <param name="methodName">函数名称</param>
        public void RemoveMethod(string methodName)
        {
            if (null == m_methods)
            {
                return;
            }

            if (false == m_methods.ContainsKey(methodName))
            {
                Debugger.Warn("Could not found any method instance '{0}' from target symbol class '{1}', removed it failed.",
                        methodName, NovaEngine.Utility.Text.ToString(m_classType));
                return;
            }

            m_methods.Remove(methodName);
        }

        /// <summary>
        /// 从当前的类标记对象中移除所有类函数实例
        /// </summary>
        private void RemoveAllMethods()
        {
            m_methods?.Clear();
            m_methods = null;
        }

        #endregion

        #region 类标记对象的Bean实例相关访问接口函数

        /// <summary>
        /// 新增指定的Bean实例到当前的类标记管理列表中
        /// </summary>
        /// <param name="bean">Bean实例</param>
        public void AddBean(Bean bean)
        {
            if (string.IsNullOrEmpty(bean.BeanName))
            {
                bean.BeanName = this.DefaultBeanName;
            }

            if (null == m_beans)
            {
                m_beans = new Dictionary<string, Bean>();
            }

            if (m_beans.ContainsKey(bean.BeanName))
            {
                Debugger.Warn("The bean object '{0}' was already exist in symbol class '{1}', repeat added it will be override old value.", bean.BeanName, m_className);
                m_beans.Remove(bean.BeanName);
            }

            m_beans.Add(bean.BeanName, bean);
        }

        /// <summary>
        /// 检测当前类标记中是否存在指定名称的Bean实例
        /// </summary>
        /// <param name="beanName">Bean名称</param>
        /// <returns>若存在目标Bean实例则返回true，否则返回false</returns>
        public bool HasBeanByName(string beanName)
        {
            if (string.IsNullOrEmpty(beanName))
            {
                beanName = this.DefaultBeanName;
            }

            if (null != m_beans && m_beans.ContainsKey(beanName))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 获取对象类默认解析的Bean实例
        /// </summary>
        /// <returns>返回默认的Bean实例</returns>
        public Bean GetBean()
        {
            return GetBean(this.DefaultBeanName);
        }

        /// <summary>
        /// 获取对象类指定名称的Bean实例
        /// </summary>
        /// <param name="beanName">Bean名称</param>
        /// <returns>返回给定名称对应的Bean实例，若不存在则返回null</returns>
        public Bean GetBean(string beanName)
        {
            if (string.IsNullOrEmpty(beanName))
            {
                return GetBean();
            }

            if (null == m_beans)
            {
                return null;
            }

            if (m_beans.TryGetValue(beanName, out Bean bean))
            {
                return bean;
            }

            return null;
        }

        /// <summary>
        /// 获取类标记对象的Bean实例列表
        /// </summary>
        /// <returns>返回类标记对象的Bean实例列表</returns>
        public IList<Bean> GetAllBeans()
        {
            return NovaEngine.Utility.Collection.ToListForValues<string, Bean>(m_beans);
        }

        /// <summary>
        /// 获取类标记对象的配置类Bean的实例迭代器
        /// </summary>
        /// <returns>返回类标记对象的函数迭代器</returns>
        public IEnumerator<KeyValuePair<string, Bean>> GetBeanEnumerator()
        {
            return m_beans?.GetEnumerator();
        }

        /// <summary>
        /// 从当前的类标记对象中移除所有Bean实例
        /// </summary>
        private void RemoveAllBeans()
        {
            m_beans?.Clear();
            m_beans = null;
        }

        #endregion

    }
}
