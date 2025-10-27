/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyright (C) 2024 - 2025, Hurley, Independent Studio.
/// Copyright (C) 2025, Hainan Yuanyou Information Technology Co., Ltd. Guangzhou Branch
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

namespace GameEngine
{
    /// <summary>
    /// 视图分组策略相关枚举类型定义
    /// </summary>
    [System.Flags]
    public enum ViewGroupStrategyType : byte
    {
        /// <summary>
        /// 空白
        /// </summary>
        None = 0,

        /// <summary>
        /// 堆叠
        /// </summary>
        Overlap = 0x01,
        /// <summary>
        /// 平铺
        /// </summary>
        Tile = 0x02,

        /// <summary>
        /// 单一展示
        /// </summary>
        Single = 0x04,

        /// <summary>
        /// 遮罩
        /// </summary>
        Mask = 0x10,
    }

    /// <summary>
    /// 用户界面模块封装的句柄对象类
    /// 模块具体功能接口请参考<see cref="NovaEngine.GuiModule"/>类
    /// </summary>
    public sealed partial class GuiHandler : EntityHandler
    {
        /// <summary>
        /// 视图分组对象管理列表
        /// </summary>
        private IDictionary<string, ViewGroup> _viewGroups;
        /// <summary>
        /// 视图分组对象排序列表
        /// </summary>
        private IList<ViewGroup> _sortingGroupList;

        /// <summary>
        /// 默认视图分组名称
        /// </summary>
        private string _defaultViewGroupName;

        /// <summary>
        /// 视图类型绑定分组名称映射容器
        /// </summary>
        private IDictionary<SystemType, string> _viewBindingGroupNames;

        /// <summary>
        /// 视图分组管理接口初始化回调函数
        /// </summary>
        [UnityEngine.Scripting.Preserve]
        [OnSubmoduleInitCallback]
        private void OnViewGroupingInitialize()
        {
            // 初始化视图分组对象管理容器
            _viewGroups = new Dictionary<string, ViewGroup>();
            // 初始化视图分组排序容器
            _sortingGroupList = new List<ViewGroup>();

            _defaultViewGroupName = $"{NovaEngine.Definition.CString.Default}_{typeof(ViewGroup).Name.ToLower()}";

            // 初始化视图绑定分组名称映射容器
            _viewBindingGroupNames = new Dictionary<SystemType, string>();

            // 注册默认分组
            AddViewGroup(_defaultViewGroupName, 0, ViewGroupStrategyType.None);
        }

        /// <summary>
        /// 视图分组管理接口清理回调函数
        /// </summary>
        [UnityEngine.Scripting.Preserve]
        [OnSubmoduleCleanupCallback]
        private void OnViewGroupingCleanup()
        {
            // 清理视图绑定分组名称映射容器
            _viewBindingGroupNames.Clear();
            _viewBindingGroupNames = null;

            // 清理视图分组对象管理容器
            RemoveAllViewGroups();
            _viewGroups = null;
            _sortingGroupList = null;
        }

        /// <summary>
        /// 视图分组管理接口重载回调函数
        /// </summary>
        [UnityEngine.Scripting.Preserve]
        [OnSubmoduleReloadCallback]
        private void OnViewGroupingReload()
        {
        }

        #region 视图元素分组策略管理对象数据类型及接口函数

        /// <summary>
        /// 注册指定名称的视图分组对象到当前句柄容器中
        /// </summary>
        /// <param name="groupName">分组名称</param>
        /// <param name="level">分组层级</param>
        public void AddViewGroup(string groupName, int level, ViewGroupStrategyType strategyType)
        {
            if (_viewGroups.ContainsKey(groupName))
            {
                Debugger.Warn(LogGroupTag.Module, "视图管理句柄中已存在名称为‘{%s}’的视图分组对象实例，重复进行相同名称的分组对象添加操作执行失败！", groupName);
                return;
            }

            // 在此处进行层级检测，要求所有分组对象的层级不能重复
            foreach (KeyValuePair<string, ViewGroup> kvp in _viewGroups)
            {
                if (kvp.Value.Level == level)
                {
                    Debugger.Warn(LogGroupTag.Module, "视图管理句柄中已存在相同层级‘{%d}’的视图分组对象实例‘{%s}’，建议当前分组对象‘{%s}’更换其它层级后再进行添加操作！",
                        level, kvp.Value.GroupName, groupName);
                    return;
                }
            }

            // 创建视图分组对象
            ViewGroup viewGroup = new ViewGroup(groupName, level, strategyType);

            // 添加视图分组对象
            _viewGroups.Add(groupName, viewGroup);
            // 分组对象添加到排序列表
            _sortingGroupList.Add(viewGroup);

            // 视图分组排序
            SortingViewGroups();
        }

        /// <summary>
        /// 从当前句柄容器中移除指定名称的视图分组对象
        /// </summary>
        /// <param name="groupName">分组名称</param>
        public void RemoveViewGroup(string groupName)
        {
            if (false == _viewGroups.TryGetValue(groupName, out ViewGroup viewGroup))
            {
                Debugger.Warn(LogGroupTag.Module, "在视图管理句柄未找到名称为‘{%s}’的视图分组对象实例，对目标分组对象删除操作执行失败！", groupName);
                return;
            }

            // 从排序列表移除
            _sortingGroupList.Remove(viewGroup);
            // 移除视图分组对象
            _viewGroups.Remove(groupName);

            // 视图分组排序
            SortingViewGroups();
        }

        /// <summary>
        /// 移除当前句柄容器中注册的所有视图分组对象实例
        /// </summary>
        private void RemoveAllViewGroups()
        {
            // 移除所有视图分组对象
            _viewGroups.Clear();
            // 移除所有视图分组排序信息
            _sortingGroupList.Clear();
        }

        /// <summary>
        /// 对当前句柄容器中注册的所有视图分组对象实例进行排序
        /// </summary>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        private void SortingViewGroups()
        {
            ((List<ViewGroup>) _sortingGroupList).Sort();
        }

        /// <summary>
        /// 视图分组对象类，用于对窗口对象进行分组标记，以及同组内的变化通知
        /// </summary>
        private sealed class ViewGroup : System.IComparable<ViewGroup>
        {
            /// <summary>
            /// 分组对象的名称
            /// </summary>
            private readonly string _groupName;
            /// <summary>
            /// 分组对象的挂载层级
            /// </summary>
            private readonly int _level;
            /// <summary>
            /// 分组对象策略类型
            /// </summary>
            private readonly ViewGroupStrategyType _strategyType;

            /// <summary>
            /// 分组对象内部管理的视图实例
            /// </summary>
            private readonly IList<CView> _groupViews;

            public string GroupName => _groupName;
            public int Level => _level;
            public ViewGroupStrategyType StrategyType => _strategyType;

            public ViewGroup(string groupName, int level, ViewGroupStrategyType strategyType)
            {
                _groupName = groupName;
                _level = level;
                _strategyType = strategyType;
                _groupViews = new List<CView>();
            }

            ~ViewGroup()
            {
                Debugger.Assert(0 == _groupViews.Count, "The view list has more instance was not removed.");
                // 这里应该已经为空表了，但还是再清理一次吧
                _groupViews.Clear();
                // _groupViews = null;
            }

            public int CompareTo(ViewGroup other)
            {
                return other.Level.CompareTo(this.Level);
            }

            /// <summary>
            /// 视图对象分组绑定操作的管理接口函数
            /// </summary>
            /// <param name="view">视图对象实例</param>
            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            public void OnViewGroupBinding(CView view)
            {
                if (_groupViews.Contains(view))
                {
                    Debugger.Error(LogGroupTag.Module, "目标视图对象实例‘{%t}’已经被注册到分组管理容器‘{%s}’中，请勿对相同视图对象进行重复添加！", view, _groupName);
                    return;
                }

                _groupViews.Add(view);

                // 视图对象恢复通知
                view.OnResume();
                // 视图对象置顶通知
                view.OnReveal();
            }

            /// <summary>
            /// 视图对象分组解绑操作的管理接口函数
            /// </summary>
            /// <param name="view">视图对象实例</param>
            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            public void OnViewGroupUnbinding(CView view)
            {
                if (false == _groupViews.Contains(view))
                {
                    Debugger.Error(LogGroupTag.Module, "目标视图对象实例‘{%t}’从未被注册到分组管理容器‘{%s}’中，对无效视图对象进行移除操作失败！", view, _groupName);
                    return;
                }

                // 视图对象遮挡通知
                view.OnCover();
                // 视图对象暂停通知
                view.OnPause();

                _groupViews.Remove(view);
            }
        }

        #endregion

        #region 视图类型绑定分组相关的接口函数

        /// <summary>
        /// 注册指定视图类型及分组名称到当前分组绑定映射容器中<br/>
        /// 若该分组名称尚未在分组容器中进行注册，将自动修改为默认分组
        /// </summary>
        /// <param name="viewType">视图类型</param>
        /// <param name="groupName">分组名称</param>
        private void AddViewBindingGroupName(SystemType viewType, string groupName)
        {
            if (_viewBindingGroupNames.ContainsKey(viewType))
            {
                Debugger.Warn(LogGroupTag.Module, "视图管理句柄的分组绑定映射容器中已存在类型为‘{%t}’的视图记录，重复进行相同类型视图的分组绑定工作执行失败！", viewType);
                return;
            }

            if (false == _viewGroups.ContainsKey(groupName))
            {
                Debugger.Warn(LogGroupTag.Module, "视图管理句柄的分组容器中不存在名称为‘{%s}’的分组对象实例，视图类型‘{%t}’将自动添加到默认分组中。", groupName, viewType);

                // groupName = _defaultViewGroupName;
                // 默认分组无需注册，直接返回即可
                return;
            }

            // 添加视图类型绑定分组名称
            _viewBindingGroupNames.Add(viewType, groupName);
        }

        /// <summary>
        /// 从当前分组绑定映射容器中移除指定视图类型的绑定关系
        /// </summary>
        /// <param name="viewType">视图类型</param>
        private void RemoveViewBindingGroupName(SystemType viewType)
        {
            if (false == _viewBindingGroupNames.ContainsKey(viewType))
            {
                Debugger.Warn(LogGroupTag.Module, "在视图管理句柄的分组绑定映射容器中未找到类型为‘{%t}’的视图记录，对目标视图类型的分组解绑工作执行失败！", viewType);
                return;
            }

            // 移除视图类型绑定分组名称
            _viewBindingGroupNames.Remove(viewType);
        }

        /// <summary>
        /// 通过指定视图类型查找对应的分组对象实例
        /// </summary>
        /// <param name="viewType">视图类型</param>
        /// <returns>返回视图类型对应的分组对象实例</returns>
        private ViewGroup FindGroupByViewType(SystemType viewType)
        {
            if (null == _viewGroups)
            {
                // 这里补充一个检查，只是因为在销毁阶段子模块先被清除的原因
                // 是否有必要因为这个，在每次调用该接口时都多一步检查？
                return null;
            }

            if (false == _viewBindingGroupNames.TryGetValue(viewType, out string groupName))
            {
                groupName = _defaultViewGroupName;
            }

            if (_viewGroups.TryGetValue(groupName, out ViewGroup viewGroup))
            {
                return viewGroup;
            }

            Debugger.Warn(LogGroupTag.Module, "在视图管理句柄的分组绑定映射容器中未找到类型‘{%t}’及名称‘{%s}’对应的视图记录，对目标视图类型的分组解绑工作执行失败！", viewType, groupName);

            return null;
        }

        #endregion
    }
}
