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

using SystemType = System.Type;
using SystemStringBuilder = System.Text.StringBuilder;

namespace GameEngine.Loader.Configuring
{
    /// <summary>
    /// 通用Bean配置类型的结构信息
    /// </summary>
    public class BeanConfigureInfo : BaseConfigureInfo
    {
        /// <summary>
        /// 节点对应的对象类型
        /// </summary>
        private SystemType m_classType;
        /// <summary>
        /// 节点对象的父节点名称
        /// </summary>
        private string m_parentName;
        /// <summary>
        /// 节点对象的单例模式
        /// </summary>
        private bool m_singleton;
        /// <summary>
        /// 节点对象的继承模式
        /// </summary>
        private bool m_inherited;
        /// <summary>
        /// 节点对象的字段列表
        /// </summary>
        private IDictionary<string, BeanFieldConfigureInfo> m_fields;
        /// <summary>
        /// 节点对象的组件列表
        /// </summary>
        private IList<BeanComponentConfigureInfo> m_components;

        public override ConfigureInfoType Type => ConfigureInfoType.Bean;
        public SystemType ClassType { get { return m_classType; } internal set { m_classType = value; } }
        public string ParentName { get { return m_parentName; } internal set { m_parentName = value; } }
        public bool Singleton { get { return m_singleton; } internal set { m_singleton = value; } }
        public bool Inherited { get { return m_inherited; } internal set { m_inherited = value; } }
        public IDictionary<string, BeanFieldConfigureInfo> Fields { get { return m_fields; } }
        public IList<BeanComponentConfigureInfo> Components { get { return m_components; } }

        ~BeanConfigureInfo()
        {
            RemoveAllFieldInfos();
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

            if (null == m_fields)
            {
                m_fields = new Dictionary<string, BeanFieldConfigureInfo>();
            }
            else if (m_fields.ContainsKey(fieldInfo.FieldName))
            {
                Debugger.Warn("The bean '{0}' class's field '{1}' was already exist, repeat added it will be override old value.",
                        NovaEngine.Utility.Text.ToString(m_classType), fieldInfo.FieldName);
                m_fields.Remove(fieldInfo.FieldName);
            }

            m_fields.Add(fieldInfo.FieldName, fieldInfo);
            return true;
        }

        /// <summary>
        /// 移除当前节点对象中指定名称的字段信息
        /// </summary>
        /// <param name="fieldName"></param>
        public void RemoveFieldInfo(string fieldName)
        {
            if (null != m_fields && m_fields.ContainsKey(fieldName))
            {
                m_fields.Remove(fieldName);
            }
        }

        /// <summary>
        /// 移除当前节点对象中注册的所有字段信息
        /// </summary>
        private void RemoveAllFieldInfos()
        {
            m_fields?.Clear();
            m_fields = null;
        }

        /// <summary>
        /// 获取节点对象的字段迭代器
        /// </summary>
        /// <returns>返回节点对象的字段迭代器</returns>
        public IEnumerator<KeyValuePair<string, BeanFieldConfigureInfo>> GetFieldInfoEnumerator()
        {
            return m_fields?.GetEnumerator();
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

            if (null == m_components)
            {
                m_components = new List<BeanComponentConfigureInfo>();
            }
            else if (m_components.Contains(componentInfo))
            {
                Debugger.Warn("The bean '{0}' class's component '{1}' was already exist, repeat added it will be override old value.",
                        NovaEngine.Utility.Text.ToString(m_classType), componentInfo.ToString());
                m_components.Remove(componentInfo);
            }

            m_components.Add(componentInfo);
            return true;
        }

        /// <summary>
        /// 移除当前节点对象中注册的所有组件信息
        /// </summary>
        private void RemoveAllComponentInfos()
        {
            m_components?.Clear();
            m_components = null;
        }

        /// <summary>
        /// 获取当前节点对象的组件解析数据的结构信息数量
        /// </summary>
        /// <returns>返回组件解析数据的结构信息数量</returns>
        internal int GetComponentInfoCount()
        {
            if (null != m_components)
            {
                return m_components.Count;
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
            if (null == m_components || index < 0 || index >= m_components.Count)
            {
                Debugger.Warn("Invalid index ({0}) for configure bean component info list.", index);
                return null;
            }

            return m_components[index];
        }

        public override string ToString()
        {
            SystemStringBuilder sb = new SystemStringBuilder();
            sb.Append("Bean = { ");
            sb.AppendFormat("Base = {{{0}}}, ", base.ToString());
            sb.AppendFormat("ClassType = {0}, ", NovaEngine.Utility.Text.ToString(m_classType));
            sb.AppendFormat("ParentName = {0}, ", m_parentName);
            sb.AppendFormat("Singleton = {0}, ", m_singleton);
            sb.AppendFormat("Inherited = {0}, ", m_inherited);
            sb.AppendFormat("Fields = {{{0}}}, ", NovaEngine.Utility.Text.ToString<string, BeanFieldConfigureInfo>(m_fields));
            sb.AppendFormat("Components = {{{0}}}, ", NovaEngine.Utility.Text.ToString<BeanComponentConfigureInfo>(m_components));
            sb.Append("}");
            return sb.ToString();
        }
    }
}
