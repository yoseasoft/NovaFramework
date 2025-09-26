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
        /// 场景类的结构信息管理容器
        /// </summary>
        private static IDictionary<string, Structuring.SceneCodeInfo> _sceneCodeInfos = new Dictionary<string, Structuring.SceneCodeInfo>();

        [OnCodeLoaderClassLoadOfTarget(typeof(CScene))]
        private static bool LoadSceneClass(Symboling.SymClass symClass, bool reload)
        {
            if (false == typeof(CScene).IsAssignableFrom(symClass.ClassType))
            {
                Debugger.Warn("The target class type '{0}' must be inherited from 'CScene' interface, load it failed.", symClass.FullName);
                return false;
            }

            Structuring.SceneCodeInfo info = new Structuring.SceneCodeInfo();
            info.ClassType = symClass.ClassType;

            IList<SystemAttribute> attrs = symClass.Attributes;
            for (int n = 0; null != attrs && n < attrs.Count; ++n)
            {
                SystemAttribute attr = attrs[n];
                SystemType attrType = attr.GetType();
                if (typeof(DeclareSceneClassAttribute) == attrType)
                {
                    DeclareSceneClassAttribute _attr = (DeclareSceneClassAttribute) attr;
                    info.SceneName = _attr.SceneName;
                    info.Priority = _attr.Priority;
                }
                else if (typeof(SceneAutoDisplayOnTargetViewAttribute) == attrType)
                {
                    SceneAutoDisplayOnTargetViewAttribute _attr = (SceneAutoDisplayOnTargetViewAttribute) attr;
                    info.AddAutoDisplayViewName(_attr.ViewName);
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

            if (string.IsNullOrEmpty(info.SceneName))
            {
                // Debugger.Warn(LogGroupTag.CodeLoader, "The scene '{%s}' name must be non-null or empty space.", symClass.FullName);
                info.SceneName = symClass.ClassName;
            }

            if (_sceneCodeInfos.ContainsKey(info.SceneName))
            {
                if (reload)
                {
                    _sceneCodeInfos.Remove(info.SceneName);
                }
                else
                {
                    Debugger.Warn("The scene name '{0}' was already existed, repeat added it failed.", info.SceneName);
                    return false;
                }
            }

            _sceneCodeInfos.Add(info.SceneName, info);
            Debugger.Log(LogGroupTag.CodeLoader, "Load 'CScene' code info '{0}' succeed from target class type '{1}'.", CodeLoaderObject.ToString(info), symClass.FullName);

            return true;
        }

        private static void LoadSceneMethod(Symboling.SymClass symClass, Structuring.SceneCodeInfo codeInfo, Symboling.SymMethod symMethod)
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

        [OnCodeLoaderClassCleanupOfTarget(typeof(CScene))]
        private static void CleanupAllSceneClasses()
        {
            _sceneCodeInfos.Clear();
        }

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
