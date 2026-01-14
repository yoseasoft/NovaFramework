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
        /// 场景类的结构信息管理容器
        /// </summary>
        private readonly static IDictionary<string, Structuring.SceneCodeInfo> _sceneCodeInfos = new Dictionary<string, Structuring.SceneCodeInfo>();

        [Preserve]
        [OnCodeLoaderClassLoadOfTarget(typeof(CScene))]
        private static bool LoadSceneClass(Symboling.SymClass symClass, bool reload)
        {
            if (false == symClass.ClassType.Is<CScene>())
            {
                Debugger.Warn("The target class type '{%s}' must be inherited from 'CScene' interface, load it failed.", symClass.FullName);
                return false;
            }

            Structuring.SceneCodeInfo info = new Structuring.SceneCodeInfo();
            info.ClassType = symClass.ClassType;

            IList<Attribute> attrs = symClass.Attributes;
            for (int n = 0; null != attrs && n < attrs.Count; ++n)
            {
                Attribute attr = attrs[n];
                Type attrType = attr.GetType();
                if (attr is CSceneClassAttribute sceneClassAttribute)
                {
                    info.EntityName = sceneClassAttribute.Name;
                    info.Priority = sceneClassAttribute.Priority;
                }
                else if (attr is SceneAutoDisplayOnTargetViewAttribute sceneAutoDisplayOnTargetViewAttribute)
                {
                    info.AddAutoDisplayViewName(sceneAutoDisplayOnTargetViewAttribute.ViewName);
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

                LoadSceneMethod(symClass, info, symMethod);
            }

            if (string.IsNullOrEmpty(info.EntityName))
            {
                // Debugger.Warn(LogGroupTag.CodeLoader, "The scene '{%s}' name must be non-null or empty space.", symClass.FullName);
                info.EntityName = symClass.ClassName;
            }

            if (_sceneCodeInfos.ContainsKey(info.EntityName))
            {
                if (reload)
                {
                    _sceneCodeInfos.Remove(info.EntityName);
                }
                else
                {
                    Debugger.Warn("The scene name '{%s}' was already existed, repeat added it failed.", info.EntityName);
                    return false;
                }
            }

            _sceneCodeInfos.Add(info.EntityName, info);
            Debugger.Log(LogGroupTag.CodeLoader, "Load 'CScene' code info '{%s}' succeed from target class type '{%s}'.", CodeLoaderUtils.ToString(info), symClass.FullName);

            return true;
        }

        private static void LoadSceneMethod(Symboling.SymClass symClass, Structuring.SceneCodeInfo codeInfo, Symboling.SymMethod symMethod)
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
        [OnCodeLoaderClassCleanupOfTarget(typeof(CScene))]
        private static void CleanupAllSceneClasses()
        {
            _sceneCodeInfos.Clear();
        }

        [Preserve]
        [OnCodeLoaderClassLookupOfTarget(typeof(CScene))]
        private static Structuring.SceneCodeInfo LookupSceneCodeInfo(Symboling.SymClass symClass)
        {
            foreach (KeyValuePair<string, Structuring.SceneCodeInfo> pair in _sceneCodeInfos)
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
