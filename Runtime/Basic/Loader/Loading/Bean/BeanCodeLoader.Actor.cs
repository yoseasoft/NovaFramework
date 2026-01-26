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

using System;
using System.Collections.Generic;
using System.Customize.Extension;
using UnityEngine.Scripting;

namespace GameEngine.Loader
{
    /// 程序集中原型对象的分析处理类
    internal static partial class BeanCodeLoader
    {
        /// <summary>
        /// 对象类的结构信息管理容器
        /// </summary>
        private readonly static IDictionary<string, Structuring.ActorCodeInfo> _actorCodeInfos = new Dictionary<string, Structuring.ActorCodeInfo>();

        [Preserve]
        [OnCodeLoaderClassLoadOfTarget(typeof(CActor))]
        private static bool LoadActorClass(Symbolling.SymClass symClass, bool reload)
        {
            if (false == symClass.ClassType.Is<CActor>())
            {
                Debugger.Warn("The target class type '{%s}' must be inherited from 'CActor' interface, load it failed.", symClass.FullName);
                return false;
            }

            Structuring.ActorCodeInfo info = new Structuring.ActorCodeInfo();
            info.ClassType = symClass.ClassType;

            IList<Attribute> attrs = symClass.Attributes;
            for (int n = 0; null != attrs && n < attrs.Count; ++n)
            {
                Attribute attr = attrs[n];
                if (attr is CActorClassAttribute actorClassAttribute)
                {
                    info.EntityName = actorClassAttribute.Name;
                    info.Priority = actorClassAttribute.Priority;
                }
                else
                {
                    LoadEntityClassByAttributeType(symClass, info, attr);
                }
            }

            IList<Symbolling.SymMethod> symMethods = symClass.GetAllMethods();
            for (int n = 0; null != symMethods && n < symMethods.Count; ++n)
            {
                Symbolling.SymMethod symMethod = symMethods[n];

                LoadActorMethod(symClass, info, symMethod);
            }

            if (string.IsNullOrEmpty(info.EntityName))
            {
                // Debugger.Warn(LogGroupTag.CodeLoader, "The actor '{%s}' name must be non-null or empty space.", symClass.FullName);
                info.EntityName = symClass.ClassName;
            }

            if (_actorCodeInfos.ContainsKey(info.EntityName))
            {
                if (reload)
                {
                    _actorCodeInfos.Remove(info.EntityName);
                }
                else
                {
                    Debugger.Warn("The actor name '{%s}' was already existed, repeat added it failed.", info.EntityName);
                    return false;
                }
            }

            _actorCodeInfos.Add(info.EntityName, info);
            Debugger.Log(LogGroupTag.CodeLoader, "Load 'CActor' code info '{%s}' succeed from target class type '{%s}'.", CodeLoaderUtils.ToString(info), symClass.FullName);

            return true;
        }

        private static void LoadActorMethod(Symbolling.SymClass symClass, Structuring.ActorCodeInfo codeInfo, Symbolling.SymMethod symMethod)
        {
            // 静态函数直接忽略
            if (symMethod.IsStatic)
            {
                return;
            }

            IList<Attribute> attrs = symClass.Attributes;
            for (int n = 0; null != attrs && n < attrs.Count; ++n)
            {
                Attribute attr = attrs[n];

                LoadEntityMethodByAttributeType(symClass, codeInfo, symMethod, attr);
            }
        }

        [Preserve]
        [OnCodeLoaderClassCleanupOfTarget(typeof(CActor))]
        private static void CleanupAllActorClasses()
        {
            _actorCodeInfos.Clear();
        }

        [Preserve]
        [OnCodeLoaderClassLookupOfTarget(typeof(CActor))]
        private static Structuring.ActorCodeInfo LookupActorCodeInfo(Symbolling.SymClass symClass)
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
