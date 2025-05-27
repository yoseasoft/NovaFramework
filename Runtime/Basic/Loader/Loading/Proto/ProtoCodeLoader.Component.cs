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
using SystemAttribute = System.Attribute;
using SystemStringBuilder = System.Text.StringBuilder;

namespace GameEngine.Loader
{
    /// <summary>
    /// 组件类的结构信息
    /// </summary>
    public class ComponentCodeInfo : ProtoBaseCodeInfo
    {
        /// <summary>
        /// 组件名称
        /// </summary>
        private string m_componentName;

        public string ComponentName { get { return m_componentName; } internal set { m_componentName = value; } }

        public override string ToString()
        {
            SystemStringBuilder sb = new SystemStringBuilder();
            sb.Append("Component = { ");
            sb.AppendFormat("Parent = {0}, ", base.ToString());
            sb.AppendFormat("Name = {0}, ", m_componentName ?? NovaEngine.Definition.CString.Unknown);
            sb.Append("}");
            return sb.ToString();
        }
    }

    /// <summary>
    /// 程序集中原型对象的分析处理类，对业务层载入的所有原型对象类进行统一加载及分析处理
    /// </summary>
    internal static partial class ProtoCodeLoader
    {
        /// <summary>
        /// 组件类的结构信息管理容器
        /// </summary>
        private static IDictionary<string, ComponentCodeInfo> s_componentCodeInfos = new Dictionary<string, ComponentCodeInfo>();

        [OnProtoClassLoadOfTarget(typeof(CComponent))]
        private static bool LoadComponentClass(Symboling.SymClass symClass, bool reload)
        {
            if (false == typeof(CComponent).IsAssignableFrom(symClass.ClassType))
            {
                Debugger.Warn("The target class type '{0}' must be inherited from 'CComponent' interface, load it failed.", symClass.ClassName);
                return false;
            }

            ComponentCodeInfo info = new ComponentCodeInfo();
            info.ClassType = symClass.ClassType;

            IList<SystemAttribute> attrs = symClass.Attributes;
            for (int n = 0; null != attrs && n < attrs.Count; ++n)
            {
                SystemAttribute attr = attrs[n];
                SystemType attrType = attr.GetType();
                if (typeof(DeclareComponentClassAttribute) == attrType)
                {
                    DeclareComponentClassAttribute _attr = (DeclareComponentClassAttribute) attr;
                    info.ComponentName = _attr.ComponentName;
                }
            }

            IList<Symboling.SymMethod> symMethods = symClass.GetAllMethods();
            for (int n = 0; null != symMethods && n < symMethods.Count; ++n)
            {
                Symboling.SymMethod symMethod = symMethods[n];

                LoadComponentMethod(symClass, info, symMethod);
            }

            if (string.IsNullOrEmpty(info.ComponentName))
            {
                const string COMPONENT_TAG = "Component";
                string componentName = symClass.ClassName;
                if (componentName.Length > COMPONENT_TAG.Length)
                {
                    // 判断是否为“Component”后缀
                    if (componentName.Substring(componentName.Length - COMPONENT_TAG.Length).Equals(COMPONENT_TAG))
                    {
                        // 裁剪掉“Component”后缀
                        string prefixName = componentName.Substring(0, componentName.Length - COMPONENT_TAG.Length);
                        if (prefixName.Length > 0)
                        {
                            info.ComponentName = prefixName;
                        }
                    }
                }
            }

            if (string.IsNullOrEmpty(info.ComponentName))
            {
                Debugger.Warn(LogGroupTag.CodeLoader, "The component '{0}' name must be non-null or empty space.", symClass.FullName);
                info.ComponentName = symClass.ClassName;
            }

            if (s_componentCodeInfos.ContainsKey(info.ComponentName))
            {
                if (reload)
                {
                    s_componentCodeInfos.Remove(info.ComponentName);
                }
                else
                {
                    Debugger.Warn("The component name '{0}' was already existed, repeat added it failed.", info.ComponentName);
                    return false;
                }
            }

            s_componentCodeInfos.Add(info.ComponentName, info);
            Debugger.Log(LogGroupTag.CodeLoader, "Load 'CComponent' code info '{0}' succeed from target class type '{1}'.", info.ToString(), symClass.FullName);

            return true;
        }

        private static void LoadComponentMethod(Symboling.SymClass symClass, ComponentCodeInfo codeInfo, Symboling.SymMethod symMethod)
        {
            // 静态函数直接忽略
            if (symMethod.IsStatic)
            {
                return;
            }

            IList<SystemAttribute> attrs = symMethod.Attributes;
            for (int n = 0; null != attrs && n < attrs.Count; ++n)
            {
                SystemAttribute attr = attrs[n];

                LoadBaseMethodByAttributeType(symClass, codeInfo, symMethod, attr);
            }
        }

        [OnProtoClassCleanupOfTarget(typeof(CComponent))]
        private static void CleanupAllComponentClasses()
        {
            s_componentCodeInfos.Clear();
        }

        [OnProtoCodeInfoLookupOfTarget(typeof(CComponent))]
        private static ComponentCodeInfo LookupComponentCodeInfo(Symboling.SymClass symClass)
        {
            foreach (KeyValuePair<string, ComponentCodeInfo> pair in s_componentCodeInfos)
            {
                if (pair.Value.ClassType == symClass.ClassType)
                {
                    return pair.Value;
                }
            }

            return null;
        }
    }
}
