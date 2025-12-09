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

using System;
using System.Collections.Generic;

namespace GameEngine.Loader.Configuring
{
    /// <summary>
    /// 通用Bean配置类型的结构信息
    /// </summary>
    public sealed class BeanConfigureInfo : BaseConfigureInfo
    {
        /// <summary>
        /// 节点对应的对象类型
        /// </summary>
        private Type _classType;
        /// <summary>
        /// 节点对象的父节点名称
        /// </summary>
        private string _parentName;
        /// <summary>
        /// 节点对象的单例模式
        /// </summary>
        private bool _singleton;
        /// <summary>
        /// 节点对象的继承模式
        /// </summary>
        private bool _inherited;
        /// <summary>
        /// 节点对象的字段列表
        /// </summary>
        private IDictionary<string, BeanFieldConfigureInfo> _fields;
        /// <summary>
        /// 节点对象的属性列表
        /// </summary>
        private IDictionary<string, BeanPropertyConfigureInfo> _properties;
        /// <summary>
        /// 节点对象的组件列表
        /// </summary>
        private IList<BeanComponentConfigureInfo> _components;

        public override ConfigureInfoType Type => ConfigureInfoType.Bean;
        public Type ClassType { get { return _classType; } internal set { _classType = value; } }
        public string ParentName { get { return _parentName; } internal set { _parentName = value; } }
        public bool Singleton { get { return _singleton; } internal set { _singleton = value; } }
        public bool Inherited { get { return _inherited; } internal set { _inherited = value; } }
        public IDictionary<string, BeanFieldConfigureInfo> Fields { get { return _fields; } }
        public IDictionary<string, BeanPropertyConfigureInfo> Properties { get { return _properties; } }
        public IList<BeanComponentConfigureInfo> Components { get { return _components; } }

        ~BeanConfigureInfo()
        {
            RemoveAllFieldInfos();
            RemoveAllPropertyInfos();
            RemoveAllComponentInfos();
        }

        /// <summary>
        /// 添加指定的字段信息实例到当前节点对象中
        /// </summary>
        /// <param name="fieldInfo">字段信息</param>
        /// <returns>若字段信息添加成功返回true，否则返回false</returns>
        public bool AddFieldInfo(BeanFieldConfigureInfo fieldInfo)
        {
            if (null == fieldInfo)
            {
                return false;
            }

            if (null == _fields)
            {
                _fields = new Dictionary<string, BeanFieldConfigureInfo>();
            }
            else if (_fields.ContainsKey(fieldInfo.FieldName))
            {
                Debugger.Warn("The bean '{%t}' class's field '{%s}' was already exist, repeat added it will be override old value.", _classType, fieldInfo.FieldName);
                _fields.Remove(fieldInfo.FieldName);
            }

            _fields.Add(fieldInfo.FieldName, fieldInfo);
            return true;
        }

        /// <summary>
        /// 移除当前节点对象中指定名称的字段信息
        /// </summary>
        /// <param name="fieldName">字段名称</param>
        public void RemoveFieldInfo(string fieldName)
        {
            if (null != _fields && _fields.ContainsKey(fieldName))
            {
                _fields.Remove(fieldName);
            }
        }

        /// <summary>
        /// 移除当前节点对象中注册的所有字段信息
        /// </summary>
        private void RemoveAllFieldInfos()
        {
            _fields?.Clear();
            _fields = null;
        }

        /// <summary>
        /// 获取节点对象的字段迭代器
        /// </summary>
        /// <returns>返回节点对象的字段迭代器</returns>
        public IEnumerator<KeyValuePair<string, BeanFieldConfigureInfo>> GetFieldInfoEnumerator()
        {
            return _fields?.GetEnumerator();
        }

        /// <summary>
        /// 添加指定的属性信息实例到当前节点对象中
        /// </summary>
        /// <param name="propertyInfo">属性信息</param>
        /// <returns>若属性信息添加成功返回true，否则返回false</returns>
        public bool AddPropertyInfo(BeanPropertyConfigureInfo propertyInfo)
        {
            if (null == propertyInfo)
            {
                return false;
            }

            if (null == _properties)
            {
                _properties = new Dictionary<string, BeanPropertyConfigureInfo>();
            }
            else if (_properties.ContainsKey(propertyInfo.PropertyName))
            {
                Debugger.Warn("The bean '{%t}' class's property '{%s}' was already exist, repeat added it will be override old value.", _classType, propertyInfo.PropertyName);
                _properties.Remove(propertyInfo.PropertyName);
            }

            _properties.Add(propertyInfo.PropertyName, propertyInfo);
            return true;
        }

        /// <summary>
        /// 移除当前节点对象中指定名称的属性信息
        /// </summary>
        /// <param name="propertyName">属性名称</param>
        public void RemovePropertyInfo(string propertyName)
        {
            if (null != _properties && _properties.ContainsKey(propertyName))
            {
                _properties.Remove(propertyName);
            }
        }

        /// <summary>
        /// 移除当前节点对象中注册的所有属性信息
        /// </summary>
        private void RemoveAllPropertyInfos()
        {
            _properties?.Clear();
            _properties = null;
        }

        /// <summary>
        /// 获取节点对象的属性迭代器
        /// </summary>
        /// <returns>返回节点对象的属性迭代器</returns>
        public IEnumerator<KeyValuePair<string, BeanPropertyConfigureInfo>> GetPropertyInfoEnumerator()
        {
            return _properties?.GetEnumerator();
        }

        /// <summary>
        /// 添加指定的组件信息实例到当前节点对象中
        /// </summary>
        /// <param name="componentInfo">组件信息</param>
        /// <returns>若组件信息添加成功返回true，否则返回false</returns>
        public bool AddComponentInfo(BeanComponentConfigureInfo componentInfo)
        {
            if (null == componentInfo)
            {
                return false;
            }

            if (null == _components)
            {
                _components = new List<BeanComponentConfigureInfo>();
            }
            else if (_components.Contains(componentInfo))
            {
                Debugger.Warn("The bean '{%t}' class's component '{%i}' was already exist, repeat added it will be override old value.", _classType, componentInfo);
                _components.Remove(componentInfo);
            }

            _components.Add(componentInfo);
            return true;
        }

        /// <summary>
        /// 移除当前节点对象中注册的所有组件信息
        /// </summary>
        private void RemoveAllComponentInfos()
        {
            _components?.Clear();
            _components = null;
        }

        /// <summary>
        /// 获取当前节点对象的组件解析数据的结构信息数量
        /// </summary>
        /// <returns>返回组件解析数据的结构信息数量</returns>
        internal int GetComponentInfoCount()
        {
            if (null != _components)
            {
                return _components.Count;
            }

            return 0;
        }

        /// <summary>
        /// 获取当前节点对象的组件信息容器中指索引对应的实例
        /// </summary>
        /// <param name="index">索引值</param>
        /// <returns>返回给定索引值对应的实例，若不存在对应实例则返回null</returns>
        internal BeanComponentConfigureInfo GetComponentInfo(int index)
        {
            if (null == _components || index < 0 || index >= _components.Count)
            {
                Debugger.Warn("Invalid index ({%d}) for configure bean component info list.", index);
                return null;
            }

            return _components[index];
        }
    }
}
