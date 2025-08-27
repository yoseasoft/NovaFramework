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
using SystemDelegate = System.Delegate;
using SystemMethodInfo = System.Reflection.MethodInfo;
using SystemStringBuilder = System.Text.StringBuilder;

namespace GameEngine.Loader.Structuring
{
    /// <summary>
    /// 场景对象模块的结构信息
    /// </summary>
    internal sealed class SceneCodeInfo : EntityCodeInfo
    {
        /// <summary>
        /// 场景名称
        /// </summary>
        private string _sceneName;
        /// <summary>
        /// 场景优先级
        /// </summary>
        private int _priority;
        /// <summary>
        /// 自动展示的场景名称列表
        /// </summary>
        private IList<string> _autoDisplayViewNames;

        public string SceneName { get { return _sceneName; } internal set { _sceneName = value; } }
        public int Priority { get { return _priority; } internal set { _priority = value; } }

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
            sb.AppendFormat("Priority = {0}, ", _priority);

            sb.AppendFormat("AutoDisplayViews = {{{0}}}, ", NovaEngine.Utility.Text.ToString(_autoDisplayViewNames));

            sb.Append("}");
            return sb.ToString();
        }
    }
}
