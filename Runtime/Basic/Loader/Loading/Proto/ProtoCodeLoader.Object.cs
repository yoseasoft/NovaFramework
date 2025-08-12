/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
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
using SystemAttribute = System.Attribute;
using SystemStringBuilder = System.Text.StringBuilder;

namespace GameEngine.Loader
{
    /// <summary>
    /// 对象类的结构信息
    /// </summary>
    public class ObjectCodeInfo : RefCodeInfo
    {
        /// <summary>
        /// 对象名称
        /// </summary>
        private string _objectName;
        /// <summary>
        /// 对象优先级
        /// </summary>
        private int _priority;

        public string ObjectName { get { return _objectName; } internal set { _objectName = value; } }
        public int Priority { get { return _priority; } internal set { _priority = value; } }

        public override string ToString()
        {
            SystemStringBuilder sb = new SystemStringBuilder();
            sb.Append("Object = { ");
            sb.AppendFormat("Parent = {0}, ", base.ToString());
            sb.AppendFormat("Name = {0}, ", _objectName ?? NovaEngine.Definition.CString.Unknown);
            sb.AppendFormat("Priority = {0}, ", _priority);
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
        /// 对象类的结构信息管理容器
        /// </summary>
        private static IDictionary<string, ObjectCodeInfo> _objectCodeInfos = new Dictionary<string, ObjectCodeInfo>();

        [OnCodeLoaderClassLoadOfTarget(typeof(CObject))]
        private static bool LoadObjectClass(Symboling.SymClass symClass, bool reload)
        {
            if (false == typeof(CObject).IsAssignableFrom(symClass.ClassType))
            {
                Debugger.Warn("The target class type '{0}' must be inherited from 'CObject' interface, load it failed.", symClass.FullName);
                return false;
            }

            ObjectCodeInfo info = new ObjectCodeInfo();
            info.ClassType = symClass.ClassType;

            IList<SystemAttribute> attrs = symClass.Attributes;
            for (int n = 0; null != attrs && n < attrs.Count; ++n)
            {
                SystemAttribute attr = attrs[n];
                SystemType attrType = attr.GetType();
                if (typeof(DeclareObjectClassAttribute) == attrType)
                {
                    DeclareObjectClassAttribute _attr = (DeclareObjectClassAttribute) attr;
                    info.ObjectName = _attr.ObjectName;
                    info.Priority = _attr.Priority;
                }
                else
                {
                    LoadRefClassByAttributeType(symClass, info, attr);
                }
            }

            IList<Symboling.SymMethod> symMethods = symClass.GetAllMethods();
            for (int n = 0; null != symMethods && n < symMethods.Count; ++n)
            {
                Symboling.SymMethod symMethod = symMethods[n];

                LoadObjectMethod(symClass, info, symMethod);
            }

            if (string.IsNullOrEmpty(info.ObjectName))
            {
                // Debugger.Warn(LogGroupTag.CodeLoader, "The object '{%s}' name must be non-null or empty space.", symClass.FullName);
                info.ObjectName = symClass.ClassName;
            }

            if (_objectCodeInfos.ContainsKey(info.ObjectName))
            {
                if (reload)
                {
                    _objectCodeInfos.Remove(info.ObjectName);
                }
                else
                {
                    Debugger.Warn("The object name '{0}' was already existed, repeat added it failed.", info.ObjectName);
                    return false;
                }
            }

            _objectCodeInfos.Add(info.ObjectName, info);
            Debugger.Log(LogGroupTag.CodeLoader, "Load 'CObject' code info '{0}' succeed from target class type '{1}'.", info.ToString(), symClass.FullName);

            return true;
        }

        private static void LoadObjectMethod(Symboling.SymClass symClass, ObjectCodeInfo codeInfo, Symboling.SymMethod symMethod)
        {
            // 静态函数直接忽略
            if (symMethod.IsStatic)
            {
                return;
            }

            IList<SystemAttribute> attrs = symClass.Attributes;
            for (int n = 0; null != attrs && n < attrs.Count; ++n)
            {
                SystemAttribute attr = attrs[n];

                LoadRefMethodByAttributeType(symClass, codeInfo, symMethod, attr);
            }
        }

        [OnCodeLoaderClassCleanupOfTarget(typeof(CObject))]
        private static void CleanupAllObjectClasses()
        {
            _objectCodeInfos.Clear();
        }

        [OnCodeLoaderClassLookupOfTarget(typeof(CObject))]
        private static ObjectCodeInfo LookupObjectCodeInfo(Symboling.SymClass symClass)
        {
            foreach (KeyValuePair<string, ObjectCodeInfo> pair in _objectCodeInfos)
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
