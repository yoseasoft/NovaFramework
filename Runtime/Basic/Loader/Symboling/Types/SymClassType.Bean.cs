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
        private SymClass _targetClass;
        /// <summary>
        /// Bean对象的目标名称
        /// </summary>
        private string _beanName;
        /// <summary>
        /// 对象的单例模式
        /// </summary>
        private bool _singleton;
        /// <summary>
        /// 对象的继承模式
        /// </summary>
        private bool _inherited;

        /// <summary>
        /// 对象的数据来自于配置文件的状态标识
        /// </summary>
        private bool _fromConfigure;

        /// <summary>
        /// Bean对象包含的字段信息
        /// </summary>
        private IDictionary<string, BeanField> _fields;
        /// <summary>
        /// Bean对象包含的属性信息
        /// </summary>
        private IDictionary<string, BeanProperty> _properties;
        /// <summary>
        /// Bean对象包含的组件信息
        /// </summary>
        private IList<BeanComponent> _components;

        public SymClass TargetClass => _targetClass;

        public string BeanName { get { return _beanName; } internal set { _beanName = value; } }
        public bool Singleton { get { return _singleton; } internal set { _singleton = value; } }
        public bool Inherited { get { return _inherited; } internal set { _inherited = value; } }
        public bool FromConfigure { get { return _fromConfigure; } internal set { _fromConfigure = value; } }

        public IDictionary<string, BeanField> Fields => _fields;
        public IDictionary<string, BeanProperty> Properties => _properties;
        public IList<BeanComponent> Components => _components;

        public Bean(SymClass targetClass)
        {
            _targetClass = targetClass;
        }

        ~Bean()
        {
            _targetClass = null;

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

            if (null == _fields)
            {
                _fields = new Dictionary<string, BeanField>();
            }

            if (_fields.ContainsKey(field.FieldName))
            {
                Debugger.Warn("The bean object '{0}' field name '{1}' was already exist, repeat added it failed.", _beanName, field.FieldName);
                _fields.Remove(field.FieldName);
            }

            _fields.Add(field.FieldName, field);
        }

        /// <summary>
        /// 检测当前类字段信息列表中是否存在指定名称的字段信息实例
        /// </summary>
        /// <param name="fieldName">字段名称</param>
        /// <returns>若存在目标字段信息实例则返回true，否则返回false</returns>
        public bool HasFieldByName(string fieldName)
        {
            Debugger.Assert(false == string.IsNullOrEmpty(fieldName), "Invalid arguments.");

            if (null == _fields)
            {
                return false;
            }

            return _fields.ContainsKey(fieldName);
        }

        /// <summary>
        /// 获取Bean对象中字段信息的数量
        /// </summary>
        /// <returns>返回Bean对象中字段信息的数量</returns>
        public int GetFieldCount()
        {
            if (null == _fields)
            {
                return 0;
            }

            return _fields.Count;
        }

        /// <summary>
        /// 通过指定的字段名称查找对应的字段信息实例
        /// </summary>
        /// <param name="fieldName">字段名称</param>
        /// <returns>返回查找的字段信息实例，若查找失败返回null</returns>
        public BeanField GetFieldByName(string fieldName)
        {
            if (null == _fields)
            {
                return null;
            }

            if (_fields.TryGetValue(fieldName, out BeanField field))
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
            return _fields?.GetEnumerator();
        }

        /// <summary>
        /// 从当前的Bean对象中移除指定的类字段信息
        /// </summary>
        /// <param name="field">字段信息</param>
        public void RemoveField(BeanField field)
        {
            if (null == _fields)
            {
                return;
            }

            if (false == _fields.ContainsKey(field.FieldName))
            {
                Debugger.Warn("Could not found any field name '{0}' from target bean object '{1}', removed it failed.", field.FieldName, _beanName);
                return;
            }

            _fields.Remove(field.FieldName);
        }

        /// <summary>
        /// 移除类字段信息列表中指定名称的字段信息实例
        /// </summary>
        /// <param name="fieldName">字段名称</param>
        public void RemoveFieldByName(string fieldName)
        {
            if (null == _fields)
            {
                return;
            }

            if (_fields.ContainsKey(fieldName))
            {
                _fields.Remove(fieldName);
            }
        }

        /// <summary>
        /// 从当前的Bean对象中移除所有类字段信息
        /// </summary>
        private void RemoveAllFields()
        {
            _fields?.Clear();
            _fields = null;
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

            if (null == _properties)
            {
                _properties = new Dictionary<string, BeanProperty>();
            }

            if (_properties.ContainsKey(property.PropertyName))
            {
                Debugger.Warn("The bean object '{0}' property name '{1}' was already exist, repeat added it failed.", _beanName, property.PropertyName);
                _properties.Remove(property.PropertyName);
            }

            _properties.Add(property.PropertyName, property);
        }

        /// <summary>
        /// 检测当前类属性信息列表中是否存在指定名称的属性信息实例
        /// </summary>
        /// <param name="propertyName">属性名称</param>
        /// <returns>若存在目标属性信息实例则返回true，否则返回false</returns>
        public bool HasPropertyByName(string propertyName)
        {
            Debugger.Assert(false == string.IsNullOrEmpty(propertyName), "Invalid arguments.");

            if (null == _properties)
            {
                return false;
            }

            return _properties.ContainsKey(propertyName);
        }

        /// <summary>
        /// 获取Bean对象中属性信息的数量
        /// </summary>
        /// <returns>返回Bean对象中属性信息的数量</returns>
        public int GetPropertyCount()
        {
            if (null == _properties)
            {
                return 0;
            }

            return _properties.Count;
        }

        /// <summary>
        /// 通过指定的属性名称查找对应的属性信息实例
        /// </summary>
        /// <param name="propertyName">属性名称</param>
        /// <returns>返回查找的属性信息实例，若查找失败返回null</returns>
        public BeanProperty GetPropertyByName(string propertyName)
        {
            if (null == _properties)
            {
                return null;
            }

            if (_properties.TryGetValue(propertyName, out BeanProperty field))
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
            return _properties?.GetEnumerator();
        }

        /// <summary>
        /// 从当前的Bean对象中移除指定的类属性信息
        /// </summary>
        /// <param name="property">属性信息</param>
        public void RemoveProperty(BeanProperty property)
        {
            if (null == _properties)
            {
                return;
            }

            if (false == _properties.ContainsKey(property.PropertyName))
            {
                Debugger.Warn("Could not found any property name '{0}' from target bean object '{1}', removed it failed.", property.PropertyName, _beanName);
                return;
            }

            _properties.Remove(property.PropertyName);
        }

        /// <summary>
        /// 移除类属性信息列表中指定名称的属性信息实例
        /// </summary>
        /// <param name="propertyName">属性名称</param>
        public void RemovePropertyByName(string propertyName)
        {
            if (null == _properties)
            {
                return;
            }

            if (_properties.ContainsKey(propertyName))
            {
                _properties.Remove(propertyName);
            }
        }

        /// <summary>
        /// 从当前的Bean对象中移除所有类属性信息
        /// </summary>
        private void RemoveAllProperties()
        {
            _properties?.Clear();
            _properties = null;
        }

        #endregion

        #region Bean对象的组件列表相关访问接口函数

        /// <summary>
        /// 新增指定的类组件信息到当前的Bean对象中
        /// </summary>
        /// <param name="component">组件信息</param>
        public void AddComponent(BeanComponent component)
        {
            if (null == _components)
            {
                _components = new List<BeanComponent>();
            }

            if (HasComponentByReferenceType(component.ReferenceClassType) || HasComponentByReferenceName(component.ReferenceBeanName))
            {
                Debugger.Warn("The bean object '{0}' was already exist with target reference class type '{1}' or reference bean name '{2}', repeat added it failed.",
                        _beanName, NovaEngine.Utility.Text.ToString(component.ReferenceClassType), component.ReferenceBeanName);
                return;
            }

            _components.Add(component);
        }

        /// <summary>
        /// 检测当前类组件信息列表中是否存在指定引用类型的组件信息实例
        /// </summary>
        /// <param name="componentType">组件类型</param>
        /// <returns>若存在目标组件信息实例则返回true，否则返回false</returns>
        public bool HasComponentByReferenceType(SystemType componentType)
        {
            if (null == _components || null == componentType)
            {
                return false;
            }

            for (int n = 0; n < _components.Count; ++n)
            {
                BeanComponent componentInfo = _components[n];
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
            if (null == _components || null == beanName)
            {
                return false;
            }

            for (int n = 0; n < _components.Count; ++n)
            {
                BeanComponent componentInfo = _components[n];
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
            if (null == _components)
            {
                return 0;
            }

            return _components.Count;
        }

        /// <summary>
        /// 通过指定的索引值获取该索引上的组件信息实例
        /// </summary>
        /// <param name="index">索引值</param>
        /// <returns>返回给定索引对应的组件信息实例，若不存在实例则返回null</returns>
        public BeanComponent GetComponentAt(int index)
        {
            if (null == _components || index < 0 || index >= _components.Count)
            {
                Debugger.Warn("Invalid index ({0}) for bean object component list.", index);
                return null;
            }

            return _components[index];
        }

        /// <summary>
        /// 通过指定的引用组件类型查找对应的组件信息实例
        /// </summary>
        /// <param name="componentType">组件类型</param>
        /// <returns>返回查找的组件信息实例，若查找失败返回null</returns>
        public BeanComponent GetComponentByReferenceType(SystemType componentType)
        {
            if (null == _components || null == componentType)
            {
                return null;
            }

            for (int n = 0; n < _components.Count; ++n)
            {
                BeanComponent componentInfo = _components[n];
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
            if (null == _components || null == beanName)
            {
                return null;
            }

            for (int n = 0; n < _components.Count; ++n)
            {
                BeanComponent componentInfo = _components[n];
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
            if (null == _components)
            {
                return;
            }

            if (false == _components.Contains(component))
            {
                Debugger.Warn("Could not found any component type '{0}' from target bean object '{1}', removed it failed.",
                        NovaEngine.Utility.Text.ToString(component.ReferenceClassType), _beanName);
                return;
            }

            _components.Remove(component);
        }

        /// <summary>
        /// 移除类组件信息列表中指定引用类型的组件信息实例
        /// </summary>
        /// <param name="componentType">组件类型</param>
        public void RemoveComponentByReferenceType(SystemType componentType)
        {
            if (null == _components)
            {
                return;
            }

            for (int n = _components.Count - 1; n >= 0; --n)
            {
                BeanComponent componentInfo = _components[n];
                if (componentInfo.ReferenceClassType == componentType)
                {
                    _components.RemoveAt(n);
                }
            }
        }

        /// <summary>
        /// 移除类组件信息列表中指定引用名称的组件信息实例
        /// </summary>
        /// <param name="beanName">实体名称</param>
        public void RemoveComponentByReferenceName(string beanName)
        {
            if (null == _components)
            {
                return;
            }

            for (int n = _components.Count - 1; n >= 0; --n)
            {
                BeanComponent componentInfo = _components[n];
                if (null != componentInfo.ReferenceBeanName && componentInfo.ReferenceBeanName.Equals(beanName))
                {
                    _components.RemoveAt(n);
                }
            }
        }

        /// <summary>
        /// 从当前的Bean对象中移除所有类组件信息
        /// </summary>
        private void RemoveAllComponents()
        {
            _components?.Clear();
            _components = null;
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
        private Bean _beanObject;

        public Bean BeanObject => _beanObject;

        protected BeanMember(Bean beanObject)
        {
            _beanObject = beanObject;
        }

        ~BeanMember()
        {
            _beanObject = null;
        }
    }
}
