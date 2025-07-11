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
    /// 视图类的结构信息
    /// </summary>
    public class ViewCodeInfo : EntityCodeInfo
    {
        /// <summary>
        /// 视图名称
        /// </summary>
        private string _viewName;
        /// <summary>
        /// 视图功能类型
        /// </summary>
        private int _funcType;

        /// <summary>
        /// 共生关系的视图名称列表
        /// </summary>
        private IList<string> _groupOfSymbioticViewNames;

        /// <summary>
        /// 视图缓存状态标识
        /// </summary>
        private bool _cachedStatus = false;
        /// <summary>
        /// 视图蒙版状态标识
        /// </summary>
        private bool _maskedStatus = false;

        public string ViewName { get { return _viewName; } internal set { _viewName = value; } }
        public int FuncType { get { return _funcType; } internal set { _funcType = value; } }

        public bool CachedStatus { get { return _cachedStatus; } internal set { _cachedStatus = value; } }
        public bool MaskedStatus { get { return _maskedStatus; } internal set { _cachedStatus = value; } }

        /// <summary>
        /// 新增与当前视图具备共生关系的目标视图名称
        /// </summary>
        /// <param name="viewName">视图名称</param>
        internal void AddGroupOfSymbioticViewName(string viewName)
        {
            if (null == _groupOfSymbioticViewNames)
            {
                _groupOfSymbioticViewNames = new List<string>();
            }

            if (_groupOfSymbioticViewNames.Contains(viewName))
            {
                Debugger.Warn("The group of symbiotic view name '{0}' was already existed, repeat added it failed.", viewName);
                return;
            }

            _groupOfSymbioticViewNames.Add(viewName);
        }

        /// <summary>
        /// 移除所有具备共生关系的视图名称记录
        /// </summary>
        internal void RemoveAllGroupOfSymbioticViewNames()
        {
            _groupOfSymbioticViewNames?.Clear();
            _groupOfSymbioticViewNames = null;
        }

        /// <summary>
        /// 检测目标视图名称是否与当前视图具备共生关系
        /// </summary>
        /// <param name="viewName">视图名称</param>
        /// <returns>若具备共生关系则返回true，否则返回false</returns>
        public bool IsGroupOfSymbioticForTargetView(string viewName)
        {
            if (null == _groupOfSymbioticViewNames || false == _groupOfSymbioticViewNames.Contains(viewName))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 获取当前具备共生关系的视图名称数量
        /// </summary>
        /// <returns>返回具备共生关系的视图名称数量</returns>
        internal int GetGroupOfSymbioticViewNamesCount()
        {
            if (null != _groupOfSymbioticViewNames)
            {
                return _groupOfSymbioticViewNames.Count;
            }

            return 0;
        }

        /// <summary>
        /// 获取当前具备共生关系的视图名称容器中指索引对应的值
        /// </summary>
        /// <param name="index">索引值</param>
        /// <returns>返回给定索引值对应的名称，若不存在对应值则返回null</returns>
        internal string GetGroupOfSymbioticViewName(int index)
        {
            if (null == _groupOfSymbioticViewNames || index < 0 || index >= _groupOfSymbioticViewNames.Count)
            {
                Debugger.Warn("Invalid index ({0}) for group of symbiotic view name list.", index);
                return null;
            }

            return _groupOfSymbioticViewNames[index];
        }

        public override string ToString()
        {
            SystemStringBuilder sb = new SystemStringBuilder();
            sb.Append("View = { ");
            sb.AppendFormat("Parent = {0}, ", base.ToString());
            sb.AppendFormat("Name = {0}, ", _viewName ?? NovaEngine.Definition.CString.Unknown);
            sb.AppendFormat("FuncType = {0}, ", _funcType);
            sb.AppendFormat("CachedStatus = {0}, ", _cachedStatus);
            sb.AppendFormat("MaskedStatus = {0}, ", _maskedStatus);

            sb.AppendFormat("GroupViews = {{{0}}}, ", NovaEngine.Utility.Text.ToString(_groupOfSymbioticViewNames));

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
        /// 视图类的结构信息管理容器
        /// </summary>
        private static IDictionary<string, ViewCodeInfo> _viewCodeInfos = new Dictionary<string, ViewCodeInfo>();

        [OnProtoClassLoadOfTarget(typeof(CView))]
        private static bool LoadViewClass(Symboling.SymClass symClass, bool reload)
        {
            if (false == typeof(CView).IsAssignableFrom(symClass.ClassType))
            {
                Debugger.Warn("The target class type '{0}' must be inherited from 'CView' interface, load it failed.", symClass.FullName);
                return false;
            }

            ViewCodeInfo info = new ViewCodeInfo();
            info.ClassType = symClass.ClassType;

            IList<SystemAttribute> attrs = symClass.Attributes;
            for (int n = 0; null != attrs && n < attrs.Count; ++n)
            {
                SystemAttribute attr = attrs[n];
                SystemType attrType = attr.GetType();
                if (typeof(DeclareViewClassAttribute) == attrType)
                {
                    DeclareViewClassAttribute _attr = (DeclareViewClassAttribute) attr;
                    info.ViewName = _attr.ViewName;
                    info.FuncType = _attr.FuncType;
                }
                else if (typeof(ViewGroupOfSymbioticRelationshipsAttribute) == attrType)
                {
                    ViewGroupOfSymbioticRelationshipsAttribute _attr = (ViewGroupOfSymbioticRelationshipsAttribute) attr;
                    info.AddGroupOfSymbioticViewName(_attr.ViewName);
                }
                else if (typeof(ViewCachedStatusEnabledAttribute) == attrType)
                {
                    info.CachedStatus = true;
                }
                else if (typeof(ViewMaskedStatusEnabledAttribute) == attrType)
                {
                    info.MaskedStatus = true;
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

            if (string.IsNullOrEmpty(info.ViewName))
            {
                const string VIEW_TAG = CView.CLASS_SUFFIX_TAG;
                string viewName = symClass.ClassName;
                if (viewName.Length > VIEW_TAG.Length)
                {
                    // 判断是否有后缀标签
                    if (viewName.Substring(viewName.Length - VIEW_TAG.Length).Equals(VIEW_TAG))
                    {
                        // 裁剪掉后缀标签
                        string prefixName = viewName.Substring(0, viewName.Length - VIEW_TAG.Length);
                        if (prefixName.Length > 0)
                        {
                            info.ViewName = prefixName;
                        }
                    }
                }
            }

            if (string.IsNullOrEmpty(info.ViewName))
            {
                Debugger.Warn(LogGroupTag.CodeLoader, "The view '{0}' name must be non-null or empty space.", symClass.FullName);
                info.ViewName = symClass.ClassName;
            }

            if (_viewCodeInfos.ContainsKey(info.ViewName))
            {
                if (reload)
                {
                    _viewCodeInfos.Remove(info.ViewName);
                }
                else
                {
                    Debugger.Warn("The view name '{0}' was already existed, repeat added it failed.", info.ViewName);
                    return false;
                }
            }

            _viewCodeInfos.Add(info.ViewName, info);
            Debugger.Log(LogGroupTag.CodeLoader, "Load 'CView' code info '{0}' succeed from target class type '{1}'.", info.ToString(), symClass.FullName);

            return true;
        }

        private static void LoadViewMethod(Symboling.SymClass symClass, ViewCodeInfo codeInfo, Symboling.SymMethod symMethod)
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

        [OnProtoClassCleanupOfTarget(typeof(CView))]
        private static void CleanupAllViewClasses()
        {
            _viewCodeInfos.Clear();
        }

        [OnProtoCodeInfoLookupOfTarget(typeof(CView))]
        private static ViewCodeInfo LookupViewCodeInfo(Symboling.SymClass symClass)
        {
            foreach (KeyValuePair<string, ViewCodeInfo> pair in _viewCodeInfos)
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
