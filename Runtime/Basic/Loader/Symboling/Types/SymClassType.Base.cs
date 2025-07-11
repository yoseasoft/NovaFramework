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
using System.Reflection;

using SystemType = System.Type;
using SystemAttribute = System.Attribute;
using SystemStringBuilder = System.Text.StringBuilder;

namespace GameEngine.Loader.Symboling
{
    /// <summary>
    /// 通用标记数据的基础结构信息
    /// </summary>
    public abstract class SymBase
    {
        /// <summary>
        /// 标记的唯一声明ID
        /// </summary>
        private int _uid;

        /// <summary>
        /// 标记包含的属性信息
        /// </summary>
        private IList<SystemAttribute> _attributes;

        /// <summary>
        /// 唯一声明ID获取函数
        /// </summary>
        public int Uid => _uid;

        /// <summary>
        /// 属性信息获取函数
        /// </summary>
        public IList<SystemAttribute> Attributes => _attributes;

        protected SymBase() { }

        ~SymBase()
        {
            RemoveAllAttributes();
        }

        #region 标记对象的属性列表相关访问接口函数

        /// <summary>
        /// 获取标记中属性实例的数量
        /// </summary>
        /// <returns>返回标记中属性实例的数量</returns>
        public int GetAttributeCount()
        {
            if (null == _attributes)
            {
                return 0;
            }

            return _attributes.Count;
        }

        /// <summary>
        /// 检测标记的属性列表中是否包含指定类型的属性实例
        /// </summary>
        /// <param name="attributeType">属性类型</param>
        /// <returns>若属性列表中存在给定类型的实例则返回true，否则返回false</returns>
        public bool HasAttribute(SystemType attributeType)
        {
            if (null == _attributes)
            {
                return false;
            }

            for (int n = 0; n < _attributes.Count; ++n)
            {
                if (attributeType == _attributes[n].GetType())
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 通过指定的属性名称获取对应的属性对象实例
        /// </summary>
        /// <param name="attributeName">属性名称</param>
        /// <returns>若类对象存在给定属性则返回其实例，否则返回null</returns>
        public SystemAttribute GetAttribute(string attributeName)
        {
            if (null == _attributes || string.IsNullOrEmpty(attributeName))
            {
                return null;
            }

            for (int n = 0; n < _attributes.Count; ++n)
            {
                SystemAttribute attribute = _attributes[n];
                if (attributeName.Equals(attribute.GetType().Name))
                {
                    return attribute;
                }
            }

            return null;
        }

        /// <summary>
        /// 通过指定的属性名称获取所有匹配该名称的属性对象实例
        /// </summary>
        /// <param name="attributeName">属性名称</param>
        /// <returns>返回属性对象实例列表，若不存在则返回null</returns>
        public IList<SystemAttribute> GetAttributes(string attributeName)
        {
            if (null == _attributes || string.IsNullOrEmpty(attributeName))
            {
                return null;
            }

            IList<SystemAttribute> attributes = null;
            for (int n = 0; n < _attributes.Count; ++n)
            {
                SystemAttribute attribute = _attributes[n];
                if (attributeName.Equals(attribute.GetType().Name))
                {
                    if (null == attributes)
                    {
                        attributes = new List<SystemAttribute>();
                    }

                    attributes.Add(attribute);
                }
            }

            return attributes;
        }

        /// <summary>
        /// 通过指定的属性类型获取对应的属性对象实例
        /// </summary>
        /// <typeparam name="T">属性类型</typeparam>
        /// <returns>若类对象存在给定属性则返回其实例，否则返回null</returns>
        public T GetAttribute<T>() where T : SystemAttribute
        {
            return GetAttribute(typeof(T)) as T;
        }

        /// <summary>
        /// 通过指定的属性类型获取对应的属性对象实例
        /// </summary>
        /// <param name="attributeType">属性类型</param>
        /// <returns>若类对象存在给定属性则返回其实例，否则返回null</returns>
        public SystemAttribute GetAttribute(SystemType attributeType)
        {
            if (null == _attributes || null == attributeType)
            {
                return null;
            }

            for (int n = 0; n < _attributes.Count; ++n)
            {
                SystemAttribute attribute = _attributes[n];
                if (attribute.GetType() == attributeType)
                {
                    return attribute;
                }
            }

            return null;
        }

        /// <summary>
        /// 通过指定的属性类型获取所有匹配该类型的属性对象实例
        /// </summary>
        /// <typeparam name="T">属性类型</typeparam>
        /// <returns>返回属性对象实例列表，若不存在则返回null</returns>
        public IList<T> GetAttributes<T>() where T : SystemAttribute
        {
            return NovaEngine.Utility.Collection.CastAndToList<SystemAttribute, T>(GetAttributes(typeof(T)));
        }

        /// <summary>
        /// 通过指定的属性类型获取所有匹配该类型的属性对象实例
        /// </summary>
        /// <param name="attributeType">属性类型</param>
        /// <returns>返回属性对象实例列表，若不存在则返回null</returns>
        public IList<SystemAttribute> GetAttributes(SystemType attributeType)
        {
            if (null == _attributes || null == attributeType)
            {
                return null;
            }

            IList<SystemAttribute> attributes = null;
            for (int n = 0; n < _attributes.Count; ++n)
            {
                SystemAttribute attribute = _attributes[n];
                if (attribute.GetType() == attributeType)
                {
                    if (null == attributes)
                    {
                        attributes = new List<SystemAttribute>();
                    }

                    attributes.Add(attribute);
                }
            }

            return attributes;
        }

        /// <summary>
        /// 尝试通过指定的属性名称，在当前属性列表中查找其实例
        /// </summary>
        /// <param name="attributeName">属性名称</param>
        /// <param name="attribute">属性实例</param>
        /// <returns>若存在对应属性实例则返回true，否则返回false</returns>
        public bool TryGetAttribute(string attributeName, out SystemAttribute attribute)
        {
            attribute = GetAttribute(attributeName);
            if (null == attribute)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 尝试通过指定的属性类型，在当前属性列表中查找其实例
        /// </summary>
        /// <typeparam name="T">属性类型</typeparam>
        /// <param name="attribute">属性实例</param>
        /// <returns>若存在对应属性实例则返回true，否则返回false</returns>
        public bool TryGetAttribute<T>(out T attribute) where T : SystemAttribute
        {
            attribute = GetAttribute<T>();
            if (null == attribute)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 尝试通过指定的属性类型，在当前属性列表中查找其实例
        /// </summary>
        /// <param name="attributeType">属性类型</param>
        /// <param name="attribute">属性实例</param>
        /// <returns>若存在对应属性实例则返回true，否则返回false</returns>
        public bool TryGetAttribute(SystemType attributeType, out SystemAttribute attribute)
        {
            attribute = GetAttribute(attributeType);
            if (null == attribute)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 新增指定的属性实例到当前的标记对象中
        /// </summary>
        /// <param name="attribute">属性实例</param>
        public void AddAttribute(SystemAttribute attribute)
        {
            if (null == _attributes)
            {
                _attributes = new List<SystemAttribute>();
            }

            if (_attributes.Contains(attribute))
            {
                Debugger.Warn("The attribute '{0}' was already exist within symbol instance, repeat added it failed.", NovaEngine.Utility.Text.ToString(attribute.GetType()));
                return;
            }

            _attributes.Add(attribute);
        }

        /// <summary>
        /// 从当前的标记对象中移除指定的属性实例
        /// </summary>
        /// <param name="attribute">属性实例</param>
        public void RemoveAttribute(SystemAttribute attribute)
        {
            if (null == _attributes)
            {
                return;
            }

            if (false == _attributes.Contains(attribute))
            {
                Debugger.Warn("Could not found any attribute '{0}' from symbol instance, removed it failed.", NovaEngine.Utility.Text.ToString(attribute.GetType()));
                return;
            }

            _attributes.Remove(attribute);
        }

        /// <summary>
        /// 从当前的标记对象中移除所有属性实例
        /// </summary>
        public void RemoveAllAttributes()
        {
            _attributes?.Clear();
            _attributes = null;
        }

        #endregion
    }
}
