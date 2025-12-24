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

using SystemType = System.Type;
using SystemAttribute = System.Attribute;

namespace GameEngine.Loader
{
    /// <summary>
    /// 程序集中原型对象的分析处理类，对业务层载入的所有原型对象类进行统一加载及分析处理
    /// </summary>
    internal static partial class BeanCodeLoader
    {
        /// <summary>
        /// 视图类的结构信息管理容器
        /// </summary>
        private static IDictionary<string, Structuring.ViewCodeInfo> _viewCodeInfos = new Dictionary<string, Structuring.ViewCodeInfo>();

        [OnCodeLoaderClassLoadOfTarget(typeof(CView))]
        private static bool LoadViewClass(Symboling.SymClass symClass, bool reload)
        {
            if (false == typeof(CView).IsAssignableFrom(symClass.ClassType))
            {
                Debugger.Warn("The target class type '{0}' must be inherited from 'CView' interface, load it failed.", symClass.FullName);
                return false;
            }

            Structuring.ViewCodeInfo info = new Structuring.ViewCodeInfo();
            info.ClassType = symClass.ClassType;

            IList<SystemAttribute> attrs = symClass.Attributes;
            for (int n = 0; null != attrs && n < attrs.Count; ++n)
            {
                SystemAttribute attr = attrs[n];
                SystemType attrType = attr.GetType();
                if (attr is CViewClassAttribute viewClassAttribute)
                {
                    info.EntityName = viewClassAttribute.Name;
                    info.Priority = viewClassAttribute.Priority;
                }
                else if (attr is CViewGroupAttribute viewGroupAttribute)
                {
                    info.GroupName = viewGroupAttribute.GroupName;
                }
                else if (typeof(ViewGroupOfSymbioticRelationshipsAttribute) == attrType)
                {
                    ViewGroupOfSymbioticRelationshipsAttribute _attr = (ViewGroupOfSymbioticRelationshipsAttribute) attr;
                    info.AddGroupOfSymbioticViewName(_attr.ViewName);
                }
                else
                {
                    LoadEntityClassByAttributeType(symClass, info, attr);
                }
            }

            IList<Symboling.SymMethod> symMethods = symClass.GetAllMethods();
            for (int n = 0; null != symMethods && n < symMethods.Count; ++n)
            {
                Symboling.SymMethod symMethod = symMethods[n];

                LoadViewMethod(symClass, info, symMethod);
            }

            if (string.IsNullOrEmpty(info.EntityName))
            {
                // Debugger.Warn(LogGroupTag.CodeLoader, "The view '{%s}' name must be non-null or empty space.", symClass.FullName);
                info.EntityName = symClass.ClassName;
            }

            if (_viewCodeInfos.ContainsKey(info.EntityName))
            {
                if (reload)
                {
                    _viewCodeInfos.Remove(info.EntityName);
                }
                else
                {
                    Debugger.Warn("The view name '{%s}' was already existed, repeat added it failed.", info.EntityName);
                    return false;
                }
            }

            _viewCodeInfos.Add(info.EntityName, info);
            Debugger.Log(LogGroupTag.CodeLoader, "Load 'CView' code info '{%s}' succeed from target class type '{%s}'.", CodeLoaderUtils.ToString(info), symClass.FullName);

            return true;
        }

        private static void LoadViewMethod(Symboling.SymClass symClass, Structuring.ViewCodeInfo codeInfo, Symboling.SymMethod symMethod)
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

                LoadEntityMethodByAttributeType(symClass, codeInfo, symMethod, attr);
            }
        }

        [OnCodeLoaderClassCleanupOfTarget(typeof(CView))]
        private static void CleanupAllViewClasses()
        {
            _viewCodeInfos.Clear();
        }

        [OnCodeLoaderClassLookupOfTarget(typeof(CView))]
        private static Structuring.ViewCodeInfo LookupViewCodeInfo(Symboling.SymClass symClass)
        {
            foreach (KeyValuePair<string, Structuring.ViewCodeInfo> pair in _viewCodeInfos)
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
