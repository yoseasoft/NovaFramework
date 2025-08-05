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
using SystemStringBuilder = System.Text.StringBuilder;

namespace GameEngine.Loader
{
    /// <summary>
    /// 场景类的结构信息
    /// </summary>
    public class SceneCodeInfo : EntityCodeInfo
    {
        /// <summary>
        /// 场景名称
        /// </summary>
        private string _sceneName;
        /// <summary>
        /// 场景功能类型
        /// </summary>
        private int _funcType;
        /// <summary>
        /// 自动展示的场景名称列表
        /// </summary>
        private IList<string> _autoDisplayViewNames;

        public string SceneName { get { return _sceneName; } internal set { _sceneName = value; } }
        public int FuncType { get { return _funcType; } internal set { _funcType = value; } }

        /// <summary>
        /// 新增需要自动展示在当前场景的目标视图名称
        /// </summary>
        /// <param name="viewName">视图名称</param>
        internal void AddAutoDisplayViewName(string viewName)
        {
            if (null == _autoDisplayViewNames)
            {
                _autoDisplayViewNames = new List<string>();
            }

            if (_autoDisplayViewNames.Contains(viewName))
            {
                Debugger.Warn("The auto display view name '{0}' was already existed, repeat added it failed.", viewName);
                return;
            }

            _autoDisplayViewNames.Add(viewName);
        }

        /// <summary>
        /// 移除所有自动展示在当前场景的目标视图名称记录
        /// </summary>
        internal void RemoveAllAutoDisplayViewNames()
        {
            _autoDisplayViewNames?.Clear();
            _autoDisplayViewNames = null;
        }

        /// <summary>
        /// 检测目标视图名称是否需要自动展示在当前场景
        /// </summary>
        /// <param name="viewName">视图名称</param>
        /// <returns>若需要自动展示则返回true，否则返回false</returns>
        public bool IsAutoDisplayForTargetView(string viewName)
        {
            if (null == _autoDisplayViewNames || false == _autoDisplayViewNames.Contains(viewName))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 获取当前需要自动展示在当前场景的视图名称数量
        /// </summary>
        /// <returns>返回需要自动展示在当前场景的视图名称数量</returns>
        internal int GetAutoDisplayViewNamesCount()
        {
            if (null != _autoDisplayViewNames)
            {
                return _autoDisplayViewNames.Count;
            }

            return 0;
        }

        /// <summary>
        /// 获取当前需要自动展示在当前场景的视图名称容器中指索引对应的值
        /// </summary>
        /// <param name="index">索引值</param>
        /// <returns>返回给定索引值对应的名称，若不存在对应值则返回null</returns>
        internal string GetAutoDisplayViewName(int index)
        {
            if (null == _autoDisplayViewNames || index < 0 || index >= _autoDisplayViewNames.Count)
            {
                Debugger.Warn("Invalid index ({0}) for auto display view name list.", index);
                return null;
            }

            return _autoDisplayViewNames[index];
        }

        public override string ToString()
        {
            SystemStringBuilder sb = new SystemStringBuilder();
            sb.Append("Scene = { ");
            sb.AppendFormat("Parent = {0}, ", base.ToString());
            sb.AppendFormat("Name = {0}, ", _sceneName ?? NovaEngine.Definition.CString.Unknown);
            sb.AppendFormat("FuncType = {0}, ", _funcType);

            sb.AppendFormat("AutoDisplayViews = {{{0}}}, ", NovaEngine.Utility.Text.ToString(_autoDisplayViewNames));

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
        /// 场景类的结构信息管理容器
        /// </summary>
        private static IDictionary<string, SceneCodeInfo> _sceneCodeInfos = new Dictionary<string, SceneCodeInfo>();

        [OnCodeLoaderClassLoadOfTarget(typeof(CScene))]
        private static bool LoadSceneClass(Symboling.SymClass symClass, bool reload)
        {
            if (false == typeof(CScene).IsAssignableFrom(symClass.ClassType))
            {
                Debugger.Warn("The target class type '{0}' must be inherited from 'CScene' interface, load it failed.", symClass.FullName);
                return false;
            }

            SceneCodeInfo info = new SceneCodeInfo();
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
                    info.FuncType = _attr.FuncType;
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
            Debugger.Log(LogGroupTag.CodeLoader, "Load 'CScene' code info '{0}' succeed from target class type '{1}'.", info.ToString(), symClass.FullName);

            return true;
        }

        private static void LoadSceneMethod(Symboling.SymClass symClass, SceneCodeInfo codeInfo, Symboling.SymMethod symMethod)
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
        private static SceneCodeInfo LookupSceneCodeInfo(Symboling.SymClass symClass)
        {
            foreach (KeyValuePair<string, SceneCodeInfo> pair in _sceneCodeInfos)
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
