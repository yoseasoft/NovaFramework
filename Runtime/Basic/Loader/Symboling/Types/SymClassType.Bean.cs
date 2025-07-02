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

using SystemType = System.Type;
using SystemStringBuilder = System.Text.StringBuilder;

namespace GameEngine.Loader.Symboling
{
    /// <summary>
    /// Bean对象类的类数据的结构信息
    /// </summary>
    public class Bean
    {
        /// <summary>
        /// Bean对象的宿主标记对象
        /// </summary>
        private SymClass m_targetClass;
        /// <summary>
        /// Bean对象的目标名称
        /// </summary>
        private string m_beanName;
        /// <summary>
        /// 对象的单例模式
        /// </summary>
        private bool m_singleton;
        /// <summary>
        /// 对象的继承模式
        /// </summary>
        private bool m_inherited;

        /// <summary>
        /// 对象的数据来自于配置文件的状态标识
        /// </summary>
        private bool m_fromConfigure;

        /// <summary>
        /// Bean对象包含的字段信息
        /// </summary>
        private IDictionary<string, BeanField> m_fields;
        /// <summary>
        /// Bean对象包含的属性信息
        /// </summary>
        private IDictionary<string, BeanProperty> m_properties;
        /// <summary>
        /// Bean对象包含的组件信息
        /// </summary>
        private IList<BeanComponent> m_components;

        public SymClass TargetClass => m_targetClass;

        public string BeanName { get { return m_beanName; } internal set { m_beanName = value; } }
        public bool Singleton { get { return m_singleton; } internal set { m_singleton = value; } }
        public bool Inherited { get { return m_inherited; } internal set { m_inherited = value; } }
        public bool FromConfigure { get { return m_fromConfigure; } internal set { m_fromConfigure = value; } }

        public IDictionary<string, BeanField> Fields => m_fields;
        public IDictionary<string, BeanProperty> Properties => m_properties;
        public IList<BeanComponent> Components => m_components;

        public Bean(SymClass targetClass)
        {
            m_targetClass = targetClass;
        }

        ~Bean()
        {
            m_targetClass = null;

            RemoveAllFields();
            RemoveAllProperties();
            RemoveAllComponents();
        }

        #region Bean对象的字段列表相关访问接口函数

        /// <summary>
        /// 新增指定的类字段信息到当前的Bean对象中
        /// </summary>
        /// <param name="field">字段信息</param>
        public void AddField(BeanField field)
        {
            Debugger.Assert(null != field && false == string.IsNullOrEmpty(field.FieldName), "Invalid arguments.");

            if (null == m_fields)
            {
                m_fields = new Dictionary<string, BeanField>();
            }

            if (m_fields.ContainsKey(field.FieldName))
            {
                Debugger.Warn("The bean object '{0}' field name '{1}' was already exist, repeat added it failed.", m_beanName, field.FieldName);
                m_fields.Remove(field.FieldName);
            }

            m_fields.Add(field.FieldName, field);
        }

        /// <summary>
        /// 检测当前类字段信息列表中是否存在指定名称的字段信息实例
        /// </summary>
        /// <param name="fieldName">字段名称</param>
        /// <returns>若存在目标字段信息实例则返回true，否则返回false</returns>
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
        /// 获取Bean对象中字段信息的数量
        /// </summary>
        /// <returns>返回Bean对象中字段信息的数量</returns>
        public int GetFieldCount()
        {
            if (null == m_fields)
            {
                return 0;
            }

            return m_fields.Count;
        }

        /// <summary>
        /// 通过指定的字段名称查找对应的字段信息实例
        /// </summary>
        /// <param name="fieldName">字段名称</param>
        /// <returns>返回查找的字段信息实例，若查找失败返回null</returns>
        public BeanField GetFieldByName(string fieldName)
        {
            if (null == m_fields)
            {
                return null;
            }

            if (m_fields.TryGetValue(fieldName, out BeanField field))
            {
                return field;
            }

            return null;
        }

        /// <summary>
        /// 获取Bean对象的字段迭代器
        /// </summary>
        /// <returns>返回Bean对象的字段迭代器</returns>
        public IEnumerator<KeyValuePair<string, BeanField>> GetFieldEnumerator()
        {
            return m_fields?.GetEnumerator();
        }

        /// <summary>
        /// 从当前的Bean对象中移除指定的类字段信息
        /// </summary>
        /// <param name="field">字段信息</param>
        public void RemoveField(BeanField field)
        {
            if (null == m_fields)
            {
                return;
            }

            if (false == m_fields.ContainsKey(field.FieldName))
            {
                Debugger.Warn("Could not found any field name '{0}' from target bean object '{1}', removed it failed.", field.FieldName, m_beanName);
                return;
            }

            m_fields.Remove(field.FieldName);
        }

        /// <summary>
        /// 移除类字段信息列表中指定名称的字段信息实例
        /// </summary>
        /// <param name="fieldName">字段名称</param>
        public void RemoveFieldByName(string fieldName)
        {
            if (null == m_fields)
            {
                return;
            }

            if (m_fields.ContainsKey(fieldName))
            {
                m_fields.Remove(fieldName);
            }
        }

        /// <summary>
        /// 从当前的Bean对象中移除所有类字段信息
        /// </summary>
        private void RemoveAllFields()
        {
            m_fields?.Clear();
            m_fields = null;
        }

        #endregion

        #region Bean对象的属性列表相关访问接口函数

        /// <summary>
        /// 新增指定的类属性信息到当前的Bean对象中
        /// </summary>
        /// <param name="property">属性信息</param>
        public void AddProperty(BeanProperty property)
        {
            Debugger.Assert(null != property && false == string.IsNullOrEmpty(property.PropertyName), "Invalid arguments.");

            if (null == m_properties)
            {
                m_properties = new Dictionary<string, BeanProperty>();
            }

            if (m_properties.ContainsKey(property.PropertyName))
            {
                Debugger.Warn("The bean object '{0}' property name '{1}' was already exist, repeat added it failed.", m_beanName, property.PropertyName);
                m_properties.Remove(property.PropertyName);
            }

            m_properties.Add(property.PropertyName, property);
        }

        /// <summary>
        /// 检测当前类属性信息列表中是否存在指定名称的属性信息实例
        /// </summary>
        /// <param name="propertyName">属性名称</param>
        /// <returns>若存在目标属性信息实例则返回true，否则返回false</returns>
        public bool HasPropertyByName(string propertyName)
        {
            Debugger.Assert(false == string.IsNullOrEmpty(propertyName), "Invalid arguments.");

            if (null == m_properties)
            {
                return false;
            }

            return m_properties.ContainsKey(propertyName);
        }

        /// <summary>
        /// 获取Bean对象中属性信息的数量
        /// </summary>
        /// <returns>返回Bean对象中属性信息的数量</returns>
        public int GetPropertyCount()
        {
            if (null == m_properties)
            {
                return 0;
            }

            return m_properties.Count;
        }

        /// <summary>
        /// 通过指定的属性名称查找对应的属性信息实例
        /// </summary>
        /// <param name="propertyName">属性名称</param>
        /// <returns>返回查找的属性信息实例，若查找失败返回null</returns>
        public BeanProperty GetPropertyByName(string propertyName)
        {
            if (null == m_properties)
            {
                return null;
            }

            if (m_properties.TryGetValue(propertyName, out BeanProperty field))
            {
                return field;
            }

            return null;
        }

        /// <summary>
        /// 获取Bean对象的属性迭代器
        /// </summary>
        /// <returns>返回Bean对象的属性迭代器</returns>
        public IEnumerator<KeyValuePair<string, BeanProperty>> GetPropertyEnumerator()
        {
            return m_properties?.GetEnumerator();
        }

        /// <summary>
        /// 从当前的Bean对象中移除指定的类属性信息
        /// </summary>
        /// <param name="property">属性信息</param>
        public void RemoveProperty(BeanProperty property)
        {
            if (null == m_properties)
            {
                return;
            }

            if (false == m_properties.ContainsKey(property.PropertyName))
            {
                Debugger.Warn("Could not found any property name '{0}' from target bean object '{1}', removed it failed.", property.PropertyName, m_beanName);
                return;
            }

            m_properties.Remove(property.PropertyName);
        }

        /// <summary>
        /// 移除类属性信息列表中指定名称的属性信息实例
        /// </summary>
        /// <param name="propertyName">属性名称</param>
        public void RemovePropertyByName(string propertyName)
        {
            if (null == m_properties)
            {
                return;
            }

            if (m_properties.ContainsKey(propertyName))
            {
                m_properties.Remove(propertyName);
            }
        }

        /// <summary>
        /// 从当前的Bean对象中移除所有类属性信息
        /// </summary>
        private void RemoveAllProperties()
        {
            m_properties?.Clear();
            m_properties = null;
        }

        #endregion

        #region Bean对象的组件列表相关访问接口函数

        /// <summary>
        /// 新增指定的类组件信息到当前的Bean对象中
        /// </summary>
        /// <param name="component">组件信息</param>
        public void AddComponent(BeanComponent component)
        {
            if (null == m_components)
            {
                m_components = new List<BeanComponent>();
            }

            if (HasComponentByReferenceType(component.ReferenceClassType) || HasComponentByReferenceName(component.ReferenceBeanName))
            {
                Debugger.Warn("The bean object '{0}' was already exist with target reference class type '{1}' or reference bean name '{2}', repeat added it failed.",
                        m_beanName, NovaEngine.Utility.Text.ToString(component.ReferenceClassType), component.ReferenceBeanName);
                return;
            }

            m_components.Add(component);
        }

        /// <summary>
        /// 检测当前类组件信息列表中是否存在指定引用类型的组件信息实例
        /// </summary>
        /// <param name="componentType">组件类型</param>
        /// <returns>若存在目标组件信息实例则返回true，否则返回false</returns>
        public bool HasComponentByReferenceType(SystemType componentType)
        {
            if (null == m_components || null == componentType)
            {
                return false;
            }

            for (int n = 0; n < m_components.Count; ++n)
            {
                BeanComponent componentInfo = m_components[n];
                if (componentInfo.ReferenceClassType == componentType)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 检测当前类组件信息列表中是否存在指定引用名称的组件信息实例
        /// </summary>
        /// <param name="beanName">实体名称</param>
        /// <returns>若存在目标组件信息实例则返回true，否则返回false</returns>
        public bool HasComponentByReferenceName(string beanName)
        {
            if (null == m_components || null == beanName)
            {
                return false;
            }

            for (int n = 0; n < m_components.Count; ++n)
            {
                BeanComponent componentInfo = m_components[n];
                if (null != componentInfo.ReferenceBeanName && componentInfo.ReferenceBeanName.Equals(beanName))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 获取Bean对象中组件信息的数量
        /// </summary>
        /// <returns>返回Bean对象中组件信息的数量</returns>
        public int GetComponentCount()
        {
            if (null == m_components)
            {
                return 0;
            }

            return m_components.Count;
        }

        /// <summary>
        /// 通过指定的索引值获取该索引上的组件信息实例
        /// </summary>
        /// <param name="index">索引值</param>
        /// <returns>返回给定索引对应的组件信息实例，若不存在实例则返回null</returns>
        public BeanComponent GetComponentAt(int index)
        {
            if (null == m_components || index < 0 || index >= m_components.Count)
            {
                Debugger.Warn("Invalid index ({0}) for bean object component list.", index);
                return null;
            }

            return m_components[index];
        }

        /// <summary>
        /// 通过指定的引用组件类型查找对应的组件信息实例
        /// </summary>
        /// <param name="componentType">组件类型</param>
        /// <returns>返回查找的组件信息实例，若查找失败返回null</returns>
        public BeanComponent GetComponentByReferenceType(SystemType componentType)
        {
            if (null == m_components || null == componentType)
            {
                return null;
            }

            for (int n = 0; n < m_components.Count; ++n)
            {
                BeanComponent componentInfo = m_components[n];
                if (componentInfo.ReferenceClassType == componentType)
                {
                    return componentInfo;
                }
            }

            return null;
        }

        /// <summary>
        /// 通过指定的引用实体名称查找对应的组件信息实例
        /// </summary>
        /// <param name="beanName">实体名称</param>
        /// <returns>返回查找的组件信息实例，若查找失败返回null</returns>
        public BeanComponent GetComponentByReferenceName(string beanName)
        {
            if (null == m_components || null == beanName)
            {
                return null;
            }

            for (int n = 0; n < m_components.Count; ++n)
            {
                BeanComponent componentInfo = m_components[n];
                if (null != componentInfo.ReferenceBeanName && componentInfo.ReferenceBeanName.Equals(beanName))
                {
                    return componentInfo;
                }
            }

            return null;
        }

        /// <summary>
        /// 从当前的Bean对象中移除指定的类组件信息
        /// </summary>
        /// <param name="component">组件信息</param>
        public void RemoveComponent(BeanComponent component)
        {
            if (null == m_components)
            {
                return;
            }

            if (false == m_components.Contains(component))
            {
                Debugger.Warn("Could not found any component type '{0}' from target bean object '{1}', removed it failed.",
                        NovaEngine.Utility.Text.ToString(component.ReferenceClassType), m_beanName);
                return;
            }

            m_components.Remove(component);
        }

        /// <summary>
        /// 移除类组件信息列表中指定引用类型的组件信息实例
        /// </summary>
        /// <param name="componentType">组件类型</param>
        public void RemoveComponentByReferenceType(SystemType componentType)
        {
            if (null == m_components)
            {
                return;
            }

            for (int n = m_components.Count - 1; n >= 0; --n)
            {
                BeanComponent componentInfo = m_components[n];
                if (componentInfo.ReferenceClassType == componentType)
                {
                    m_components.RemoveAt(n);
                }
            }
        }

        /// <summary>
        /// 移除类组件信息列表中指定引用名称的组件信息实例
        /// </summary>
        /// <param name="beanName">实体名称</param>
        public void RemoveComponentByReferenceName(string beanName)
        {
            if (null == m_components)
            {
                return;
            }

            for (int n = m_components.Count - 1; n >= 0; --n)
            {
                BeanComponent componentInfo = m_components[n];
                if (null != componentInfo.ReferenceBeanName && componentInfo.ReferenceBeanName.Equals(beanName))
                {
                    m_components.RemoveAt(n);
                }
            }
        }

        /// <summary>
        /// 从当前的Bean对象中移除所有类组件信息
        /// </summary>
        private void RemoveAllComponents()
        {
            m_components?.Clear();
            m_components = null;
        }

        #endregion

    }

    /// <summary>
    /// Bean对象的内部成员基类定义
    /// </summary>
    public abstract class BeanMember
    {
        /// <summary>
        /// 依赖的Bean实例载体
        /// </summary>
        private Bean m_beanObject;

        public Bean BeanObject => m_beanObject;

        protected BeanMember(Bean beanObject)
        {
            m_beanObject = beanObject;
        }

        ~BeanMember()
        {
            m_beanObject = null;
        }
    }
}
