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
        /// 对象类的结构信息管理容器
        /// </summary>
        private static IDictionary<string, Structuring.ActorCodeInfo> _actorCodeInfos = new Dictionary<string, Structuring.ActorCodeInfo>();

        [OnCodeLoaderClassLoadOfTarget(typeof(CActor))]
        private static bool LoadActorClass(Symboling.SymClass symClass, bool reload)
        {
            if (false == typeof(CActor).IsAssignableFrom(symClass.ClassType))
            {
                Debugger.Warn("The target class type '{0}' must be inherited from 'CActor' interface, load it failed.", symClass.FullName);
                return false;
            }

            Structuring.ActorCodeInfo info = new Structuring.ActorCodeInfo();
            info.ClassType = symClass.ClassType;

            IList<SystemAttribute> attrs = symClass.Attributes;
            for (int n = 0; null != attrs && n < attrs.Count; ++n)
            {
                SystemAttribute attr = attrs[n];
                SystemType attrType = attr.GetType();
                if (typeof(DeclareActorClassAttribute) == attrType)
                {
                    DeclareActorClassAttribute _attr = (DeclareActorClassAttribute) attr;
                    info.ActorName = _attr.Name;
                    info.Priority = _attr.Priority;
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

                LoadActorMethod(symClass, info, symMethod);
            }

            if (string.IsNullOrEmpty(info.ActorName))
            {
                // Debugger.Warn(LogGroupTag.CodeLoader, "The actor '{%s}' name must be non-null or empty space.", symClass.FullName);
                info.ActorName = symClass.ClassName;
            }

            if (_actorCodeInfos.ContainsKey(info.ActorName))
            {
                if (reload)
                {
                    _actorCodeInfos.Remove(info.ActorName);
                }
                else
                {
                    Debugger.Warn("The actor name '{0}' was already existed, repeat added it failed.", info.ActorName);
                    return false;
                }
            }

            _actorCodeInfos.Add(info.ActorName, info);
            Debugger.Log(LogGroupTag.CodeLoader, "Load 'CActor' code info '{0}' succeed from target class type '{1}'.", CodeLoaderObject.ToString(info), symClass.FullName);

            return true;
        }

        private static void LoadActorMethod(Symboling.SymClass symClass, Structuring.ActorCodeInfo codeInfo, Symboling.SymMethod symMethod)
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

        [OnCodeLoaderClassCleanupOfTarget(typeof(CActor))]
        private static void CleanupAllActorClasses()
        {
            _actorCodeInfos.Clear();
        }

        [OnCodeLoaderClassLookupOfTarget(typeof(CActor))]
        private static Structuring.ActorCodeInfo LookupActorCodeInfo(Symboling.SymClass symClass)
        {
            foreach (KeyValuePair<string, Structuring.ActorCodeInfo> pair in _actorCodeInfos)
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
