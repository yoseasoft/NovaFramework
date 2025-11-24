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
        /// 组件类的结构信息管理容器
        /// </summary>
        private static IDictionary<string, Structuring.ComponentCodeInfo> _componentCodeInfos = new Dictionary<string, Structuring.ComponentCodeInfo>();

        [OnCodeLoaderClassLoadOfTarget(typeof(CComponent))]
        private static bool LoadComponentClass(Symboling.SymClass symClass, bool reload)
        {
            if (false == typeof(CComponent).IsAssignableFrom(symClass.ClassType))
            {
                Debugger.Warn("The target class type '{0}' must be inherited from 'CComponent' interface, load it failed.", symClass.ClassName);
                return false;
            }

            Structuring.ComponentCodeInfo info = new Structuring.ComponentCodeInfo();
            info.ClassType = symClass.ClassType;

            IList<SystemAttribute> attrs = symClass.Attributes;
            for (int n = 0; null != attrs && n < attrs.Count; ++n)
            {
                SystemAttribute attr = attrs[n];
                SystemType attrType = attr.GetType();
                if (typeof(CComponentClassAttribute) == attrType)
                {
                    CComponentClassAttribute _attr = (CComponentClassAttribute) attr;
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
                // Debugger.Warn(LogGroupTag.CodeLoader, "The component '{%s}' name must be non-null or empty space.", symClass.FullName);
                info.ComponentName = symClass.ClassName;
            }

            if (_componentCodeInfos.ContainsKey(info.ComponentName))
            {
                if (reload)
                {
                    _componentCodeInfos.Remove(info.ComponentName);
                }
                else
                {
                    Debugger.Warn("The component name '{0}' was already existed, repeat added it failed.", info.ComponentName);
                    return false;
                }
            }

            _componentCodeInfos.Add(info.ComponentName, info);
            Debugger.Log(LogGroupTag.CodeLoader, "Load 'CComponent' code info '{0}' succeed from target class type '{1}'.", CodeLoaderUtils.ToString(info), symClass.FullName);

            return true;
        }

        private static void LoadComponentMethod(Symboling.SymClass symClass, Structuring.ComponentCodeInfo codeInfo, Symboling.SymMethod symMethod)
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

        [OnCodeLoaderClassCleanupOfTarget(typeof(CComponent))]
        private static void CleanupAllComponentClasses()
        {
            _componentCodeInfos.Clear();
        }

        [OnCodeLoaderClassLookupOfTarget(typeof(CComponent))]
        private static Structuring.ComponentCodeInfo LookupComponentCodeInfo(Symboling.SymClass symClass)
        {
            foreach (KeyValuePair<string, Structuring.ComponentCodeInfo> pair in _componentCodeInfos)
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
