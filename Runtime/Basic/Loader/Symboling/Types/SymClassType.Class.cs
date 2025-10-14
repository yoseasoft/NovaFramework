/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyright (C) 2023 - 2024, Guangzhou Shiyue Network Technology Co., Ltd.
/// Copyright (C) 2024 - 2025, Hurley, Independent Studio.
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

using System.Collections.Generic;

using SystemType = System.Type;

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
        private string _className;
        /// <summary>
        /// 对象类的完整名称
        /// </summary>
        private string _fullName;
        /// <summary>
        /// 对象类的类型
        /// </summary>
        private SystemType _classType;
        /// <summary>
        /// 对象类的父类类型
        /// </summary>
        private SystemType _baseType;

        /// <summary>
        /// 对象类的默认Bean实例名称，该名称由类标记对象自动生成，不可主动赋值
        /// </summary>
        private string _defaultBeanName;

        /// <summary>
        /// 对象是否为接口类型
        /// </summary>
        private bool _isInterface;
        /// <summary>
        /// 对象是否为类类型
        /// </summary>
        private bool _isClass;
        /// <summary>
        /// 对象是否为抽象类型
        /// </summary>
        private bool _isAbstract;
        /// <summary>
        /// 对象是否为静态类型
        /// </summary>
        private bool _isStatic;
        /// <summary>
        /// 对象是否为可实例化的类型
        /// </summary>
        private bool _isInstantiate;
        /// <summary>
        /// 对象是否为扩展类型
        /// </summary>
        private bool _isExtension;

        /// <summary>
        /// 对象类包含的特性信息
        /// </summary>
        private IList<SystemType> _featureTypes;
        /// <summary>
        /// 对象类包含的接口信息
        /// </summary>
        private IList<SystemType> _interfaceTypes;
        /// <summary>
        /// 对象类包含的切面行为类型信息
        /// </summary>
        private IList<AspectBehaviourType> _aspectBehaviourTypes;

        /// <summary>
        /// 对象类包含的字段信息
        /// </summary>
        private IDictionary<string, SymField> _fields;
        /// <summary>
        /// 对象类包含的属性信息
        /// </summary>
        private IDictionary<string, SymProperty> _properties;
        /// <summary>
        /// 对象类包含的函数信息
        /// </summary>
        private IDictionary<string, SymMethod> _methods;

        /// <summary>
        /// 对象类的Bean实例列表
        /// </summary>
        private IDictionary<string, Bean> _beans;

        public SystemType ClassType
        {
            get { return _classType; }
            internal set
            {
                _classType = value;

                // 对象类的名称及它的完整类名
                _className = _classType.Name;
                _fullName = NovaEngine.Utility.Text.GetFullName(_classType);

                _baseType = _classType.BaseType;

                _isInterface = _classType.IsInterface;
                _isClass = _classType.IsClass;
                _isAbstract = _classType.IsAbstract;
                _isStatic = NovaEngine.Utility.Reflection.IsTypeOfStaticClass(_classType);
                _isInstantiate = NovaEngine.Utility.Reflection.IsTypeOfInstantiableClass(_classType);
                _isExtension = NovaEngine.Utility.Reflection.IsTypeOfExtension(_classType);
            }
        }

        public string DefaultBeanName
        {
            get
            {
                if (null == _defaultBeanName)
                {
                    // 前缀+类的完整名称
                    _defaultBeanName = string.Format("__internal_{0}", NovaEngine.Utility.Text.GetFullName(_classType));
                }

                return _defaultBeanName;
            }
        }

        public string ClassName => _className;
        public string FullName => _fullName;
        public SystemType BaseType => _baseType;

        public bool IsInterface => _isInterface;
        public bool IsClass => _isClass;
        public bool IsAbstract => _isAbstract;
        public bool IsStatic => _isStatic;
        public bool IsInstantiate => _isInstantiate;
        public bool IsExtension => _isExtension;

        public IList<SystemType> FeatureTypes => _featureTypes;
        public IList<SystemType> InterfaceTypes => _interfaceTypes;
        public IList<AspectBehaviourType> AspectBehaviourTypes => _aspectBehaviourTypes;

        public IDictionary<string, SymField> Fields => _fields;
        public IDictionary<string, SymProperty> Properties => _properties;
        public IDictionary<string, SymMethod> Methods => _methods;

        public SymClass() : base() { }

        ~SymClass()
        {
            RemoveAllFeatureTypes();
            RemoveAllInterfaceTypes();
            RemoveAllAspectBehaviourTypes();

            RemoveAllFields();
            RemoveAllProperties();
            RemoveAllMethods();

            RemoveAllBeans();
        }

        /// <summary>
        /// 判断该符号类型是否继承自指定的目标对象类型
        /// </summary>
        /// <param name="parentType">父类型</param>
        /// <returns>若继承自给定类型则返回true，否则返回false</returns>
        public bool IsInheritedFrom(SystemType parentType)
        {
            if (null != parentType && parentType.IsAssignableFrom(_classType))
            {
                return true;
            }

            return false;
        }

        #region 类标记对象的特性列表相关访问接口函数

        /// <summary>
        /// 新增指定的类特性实例到当前的类标记对象中
        /// </summary>
        /// <param name="featureType">类特性实例</param>
        public void AddFeatureType(SystemType featureType)
        {
            if (null == _featureTypes)
            {
                _featureTypes = new List<SystemType>();
            }

            if (_featureTypes.Contains(featureType))
            {
                // Debugger.Warn("The symbol class '{%t}' feature type '{%t}' was already exist, repeat added it failed.", _classType, featureType);
                return;
            }

            _featureTypes.Add(featureType);
        }

        /// <summary>
        /// 检测当前类标记中是否存在指定类型的特性实例
        /// </summary>
        /// <param name="featureType">特性类型</param>
        /// <returns>若存在目标特性实例则返回true，否则返回false</returns>
        public bool HasFeatureType(SystemType featureType)
        {
            if (null == _featureTypes)
            {
                return false;
            }

            return _featureTypes.Contains(featureType);
        }

        /// <summary>
        /// 从当前的类标记对象中移除指定类型的类特性实例
        /// </summary>
        /// <param name="featureType">特性类型</param>
        public void RemoveFeatureType(SystemType featureType)
        {
            if (null == _featureTypes)
            {
                return;
            }

            if (false == _featureTypes.Contains(featureType))
            {
                Debugger.Warn("Could not found any feature type '{%t}' from target symbol class '{%t}', removed it failed.", featureType, _classType);
                return;
            }

            _featureTypes.Remove(featureType);
        }

        /// <summary>
        /// 从当前的类标记对象中移除所有类特性实例
        /// </summary>
        private void RemoveAllFeatureTypes()
        {
            _featureTypes?.Clear();
            _featureTypes = null;
        }

        #endregion

        #region 类标记对象的接口列表相关访问接口函数

        /// <summary>
        /// 新增指定的类接口实例到当前的类标记对象中
        /// </summary>
        /// <param name="interfaceType">类接口实例</param>
        public void AddInterfaceType(SystemType interfaceType)
        {
            if (null == _interfaceTypes)
            {
                _interfaceTypes = new List<SystemType>();
            }

            if (_interfaceTypes.Contains(interfaceType))
            {
                Debugger.Warn("The symbol class '{%t}' interface type '{%t}' was already exist, repeat added it failed.", _classType, interfaceType);
                return;
            }

            _interfaceTypes.Add(interfaceType);
        }

        /// <summary>
        /// 检测当前类标记中是否存在指定类型的接口实例
        /// </summary>
        /// <param name="interfaceType">接口类型</param>
        /// <returns>若存在目标接口实例则返回true，否则返回false</returns>
        public bool HasInterfaceType(SystemType interfaceType)
        {
            if (null == _interfaceTypes)
            {
                return false;
            }

            return _interfaceTypes.Contains(interfaceType);
        }

        /// <summary>
        /// 从当前的类标记对象中移除指定类型的接口实例
        /// </summary>
        /// <param name="interfaceType">接口类型</param>
        public void RemoveInterfaceType(SystemType interfaceType)
        {
            if (null == _interfaceTypes)
            {
                return;
            }

            if (false == _interfaceTypes.Contains(interfaceType))
            {
                Debugger.Warn("Could not found any interface type '{%t}' from target symbol class '{%t}', removed it failed.", interfaceType, _classType);
                return;
            }

            _interfaceTypes.Remove(interfaceType);
        }

        /// <summary>
        /// 从当前的类标记对象中移除所有类接口实例
        /// </summary>
        private void RemoveAllInterfaceTypes()
        {
            _interfaceTypes?.Clear();
            _interfaceTypes = null;
        }

        #endregion

        #region 类标记对象的切面行为列表相关访问接口函数

        /// <summary>
        /// 新增指定的切面行为类型到当前的类标记对象中
        /// </summary>
        /// <param name="aspectBehaviourType">切面行为类型</param>
        public void AddAspectBehaviourType(AspectBehaviourType aspectBehaviourType)
        {
            if (null == _aspectBehaviourTypes)
            {
                _aspectBehaviourTypes = new List<AspectBehaviourType>();
            }

            if (_aspectBehaviourTypes.Contains(aspectBehaviourType))
            {
                // Debugger.Warn("The symbol class '{%t}' aspect behaviour type '{%s}' was already exist, repeat added it failed.", _classType, aspectBehaviourType.ToString());
                return;
            }

            _aspectBehaviourTypes.Add(aspectBehaviourType);
        }

        /// <summary>
        /// 检测当前类标记中是否存在指定的切面行为类型
        /// </summary>
        /// <param name="aspectBehaviourType">切面行为类型</param>
        /// <returns>若存在给定切面行为类型则返回true，否则返回false</returns>
        public bool HasAspectBehaviourType(AspectBehaviourType aspectBehaviourType)
        {
            if (null == _aspectBehaviourTypes)
            {
                return false;
            }

            return _aspectBehaviourTypes.Contains(aspectBehaviourType);
        }

        /// <summary>
        /// 从当前的类标记对象中移除指定的切面行为类型
        /// </summary>
        /// <param name="aspectBehaviourType">切面行为类型</param>
        public void RemoveAspectBehaviourType(AspectBehaviourType aspectBehaviourType)
        {
            if (null == _aspectBehaviourTypes)
            {
                return;
            }

            if (false == _aspectBehaviourTypes.Contains(aspectBehaviourType))
            {
                Debugger.Warn("Could not found any aspect behaviour type '{%s}' from target symbol class '{%t}', removed it failed.", aspectBehaviourType.ToString(), _classType);
                return;
            }

            _aspectBehaviourTypes.Remove(aspectBehaviourType);
        }

        /// <summary>
        /// 从当前的类标记对象中移除所有切面行为类型
        /// </summary>
        private void RemoveAllAspectBehaviourTypes()
        {
            _aspectBehaviourTypes?.Clear();
            _aspectBehaviourTypes = null;
        }

        #endregion

        #region 类标记对象的字段列表相关访问接口函数

        /// <summary>
        /// 新增指定的类字段实例到当前的类标记对象中
        /// </summary>
        /// <param name="interfaceType">类字段实例</param>
        public void AddField(SymField field)
        {
            if (null == _fields)
            {
                _fields = new Dictionary<string, SymField>();
            }

            if (_fields.ContainsKey(field.FieldName))
            {
                Debugger.Warn("The symbol class '{0}' field '{1}' was already exist, repeat added it failed.",
                        NovaEngine.Utility.Text.ToString(_classType), field.FieldName);
                return;
            }

            _fields.Add(field.FieldName, field);
        }

        /// <summary>
        /// 检测当前类标记中是否存在指定名称的字段实例
        /// </summary>
        /// <param name="fieldName">字段名称</param>
        /// <returns>若存在目标字段实例则返回true，否则返回false</returns>
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
        /// 获取类标记中字段实例的数量
        /// </summary>
        /// <returns>返回类标记中字段实例的数量</returns>
        public int GetFieldCount()
        {
            if (null == _fields)
            {
                return 0;
            }

            return _fields.Count;
        }

        /// <summary>
        /// 通过指定的字段名称查找对应的字段标记对象实例
        /// </summary>
        /// <param name="fieldName">字段名称</param>
        /// <returns>若查找标记对象成功则返回该实例，否则返回null</returns>
        public SymField GetFieldByName(string fieldName)
        {
            if (null == _fields || false == _fields.ContainsKey(fieldName))
            {
                return null;
            }

            return _fields[fieldName];
        }

        /// <summary>
        /// 尝试通过指定的字段名称，获取对应的字段标记对象实例
        /// </summary>
        /// <param name="fieldName">字段名称</param>
        /// <param name="symField">字段标记对象实例</param>
        /// <returns>若查找标记对象成功则返回true，否则返回false</returns>
        public bool TryGetFieldByName(string fieldName, out SymField symField)
        {
            if (null == _fields)
            {
                symField = null;
                return false;
            }

            return _fields.TryGetValue(fieldName, out symField);
        }

        /// <summary>
        /// 获取类标记对象的字段列表
        /// </summary>
        /// <returns>返回类标记对象的字段列表</returns>
        public IList<SymField> GetAllFields()
        {
            return NovaEngine.Utility.Collection.ToListForValues<string, SymField>(_fields);
        }

        /// <summary>
        /// 获取类标记对象的字段迭代器
        /// </summary>
        /// <returns>返回类标记对象的字段迭代器</returns>
        public IEnumerator<KeyValuePair<string, SymField>> GetFieldEnumerator()
        {
            return _fields?.GetEnumerator();
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
            if (null == _fields)
            {
                return;
            }

            if (false == _fields.ContainsKey(fieldName))
            {
                Debugger.Warn("Could not found any field instance '{0}' from target symbol class '{1}', removed it failed.",
                        fieldName, NovaEngine.Utility.Text.ToString(_classType));
                return;
            }

            _fields.Remove(fieldName);
        }

        /// <summary>
        /// 从当前的类标记对象中移除所有类字段实例
        /// </summary>
        private void RemoveAllFields()
        {
            _fields?.Clear();
            _fields = null;
        }

        #endregion

        #region 类标记对象的属性列表相关访问接口函数

        /// <summary>
        /// 新增指定的类属性实例到当前的类标记对象中
        /// </summary>
        /// <param name="property">类属性实例</param>
        public void AddProperty(SymProperty property)
        {
            if (null == _properties)
            {
                _properties = new Dictionary<string, SymProperty>();
            }

            if (_properties.ContainsKey(property.PropertyName))
            {
                Debugger.Warn("The symbol class '{0}' property '{1}' was already exist, repeat added it failed.",
                        NovaEngine.Utility.Text.ToString(_classType), property.PropertyName);
                return;
            }

            _properties.Add(property.PropertyName, property);
        }

        /// <summary>
        /// 获取类标记中属性实例的数量
        /// </summary>
        /// <returns>返回类标记中属性实例的数量</returns>
        public int GetPropertyCount()
        {
            if (null == _properties)
            {
                return 0;
            }

            return _properties.Count;
        }

        /// <summary>
        /// 通过指定的属性名称查找对应的属性标记对象实例
        /// </summary>
        /// <param name="propertyName">属性名称</param>
        /// <returns>若查找标记对象成功则返回该实例，否则返回null</returns>
        public SymProperty GetPropertyByName(string propertyName)
        {
            if (null == _properties || false == _properties.ContainsKey(propertyName))
            {
                return null;
            }

            return _properties[propertyName];
        }

        /// <summary>
        /// 尝试通过指定的属性名称，获取对应的属性标记对象实例
        /// </summary>
        /// <param name="propertyName">属性名称</param>
        /// <param name="symProperty">属性标记对象实例</param>
        /// <returns>若查找标记对象成功则返回true，否则返回false</returns>
        public bool TryGetPropertyByName(string propertyName, out SymProperty symProperty)
        {
            if (null == _properties)
            {
                symProperty = null;
                return false;
            }

            return _properties.TryGetValue(propertyName, out symProperty);
        }

        /// <summary>
        /// 获取类标记对象的属性列表
        /// </summary>
        /// <returns>返回类标记对象的属性列表</returns>
        public IList<SymProperty> GetAllProperties()
        {
            return NovaEngine.Utility.Collection.ToListForValues<string, SymProperty>(_properties);
        }

        /// <summary>
        /// 获取类标记对象的属性迭代器
        /// </summary>
        /// <returns>返回类标记对象的属性迭代器</returns>
        public IEnumerator<KeyValuePair<string, SymProperty>> GetPropertyEnumerator()
        {
            return _properties?.GetEnumerator();
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
            if (null == _properties)
            {
                return;
            }

            if (false == _properties.ContainsKey(propertyName))
            {
                Debugger.Warn("Could not found any property instance '{0}' from target symbol class '{1}', removed it failed.",
                        propertyName, NovaEngine.Utility.Text.ToString(_classType));
                return;
            }

            _properties.Remove(propertyName);
        }

        /// <summary>
        /// 从当前的类标记对象中移除所有类属性实例
        /// </summary>
        private void RemoveAllProperties()
        {
            _properties?.Clear();
            _properties = null;
        }

        #endregion

        #region 类标记对象的函数列表相关访问接口函数

        /// <summary>
        /// 新增指定的类函数实例到当前的类标记对象中
        /// </summary>
        /// <param name="method">类函数实例</param>
        public void AddMethod(SymMethod method)
        {
            if (null == _methods)
            {
                _methods = new Dictionary<string, SymMethod>();
            }

            if (_methods.ContainsKey(method.FullName))
            {
                Debugger.Warn("The symbol class '{0}' method '{1}' was already exist, repeat added it failed.",
                        NovaEngine.Utility.Text.ToString(_classType), method.FullName);
                return;
            }

            _methods.Add(method.FullName, method);
        }

        /// <summary>
        /// 获取类标记中函数实例的数量
        /// </summary>
        /// <returns>返回类标记中函数实例的数量</returns>
        public int GetMethodCount()
        {
            if (null == _methods)
            {
                return 0;
            }

            return _methods.Count;
        }

        /// <summary>
        /// 尝试通过指定的函数名称，获取对应的函数标记对象实例
        /// </summary>
        /// <param name="methodName">函数名称</param>
        /// <param name="symMethod">函数标记对象实例</param>
        /// <returns>若查找标记对象成功则返回true，否则返回false</returns>
        public bool TryGetMethodByName(string methodName, out SymMethod symMethod)
        {
            if (null == _methods)
            {
                symMethod = null;
                return false;
            }

            return _methods.TryGetValue(methodName, out symMethod);
        }

        /// <summary>
        /// 获取类标记对象的函数列表
        /// </summary>
        /// <returns>返回类标记对象的函数列表</returns>
        public IList<SymMethod> GetAllMethods()
        {
            return NovaEngine.Utility.Collection.ToListForValues<string, SymMethod>(_methods);
        }

        /// <summary>
        /// 获取类标记对象的函数迭代器
        /// </summary>
        /// <returns>返回类标记对象的函数迭代器</returns>
        public IEnumerator<KeyValuePair<string, SymMethod>> GetMethodEnumerator()
        {
            return _methods?.GetEnumerator();
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
            if (null == _methods)
            {
                return;
            }

            if (false == _methods.ContainsKey(methodName))
            {
                Debugger.Warn("Could not found any method instance '{0}' from target symbol class '{1}', removed it failed.",
                        methodName, NovaEngine.Utility.Text.ToString(_classType));
                return;
            }

            _methods.Remove(methodName);
        }

        /// <summary>
        /// 从当前的类标记对象中移除所有类函数实例
        /// </summary>
        private void RemoveAllMethods()
        {
            _methods?.Clear();
            _methods = null;
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

            if (null == _beans)
            {
                _beans = new Dictionary<string, Bean>();
            }

            if (_beans.ContainsKey(bean.BeanName))
            {
                Debugger.Warn("The bean object '{0}' was already exist in symbol class '{1}', repeat added it will be override old value.", bean.BeanName, _className);
                _beans.Remove(bean.BeanName);
            }

            _beans.Add(bean.BeanName, bean);
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

            if (null != _beans && _beans.ContainsKey(beanName))
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

            if (null == _beans)
            {
                return null;
            }

            if (_beans.TryGetValue(beanName, out Bean bean))
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
            return NovaEngine.Utility.Collection.ToListForValues<string, Bean>(_beans);
        }

        /// <summary>
        /// 获取类标记对象的配置类Bean的实例迭代器
        /// </summary>
        /// <returns>返回类标记对象的函数迭代器</returns>
        public IEnumerator<KeyValuePair<string, Bean>> GetBeanEnumerator()
        {
            return _beans?.GetEnumerator();
        }

        /// <summary>
        /// 从当前的类标记对象中移除所有通过配置注册的Bean实例
        /// </summary>
        public void RemoveAllConfigureBeans()
        {
            IList<string> keys = NovaEngine.Utility.Collection.ToList<string>(_beans?.Keys);
            for (int n = 0; null != keys && n < keys.Count; ++n)
            {
                string k = keys[n];
                if (DefaultBeanName.Equals(k))
                    continue;

                _beans.Remove(k);
            }
        }

        /// <summary>
        /// 从当前的类标记对象中移除所有Bean实例
        /// </summary>
        private void RemoveAllBeans()
        {
            _beans?.Clear();
            _beans = null;
        }

        #endregion

    }
}
