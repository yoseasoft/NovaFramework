/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyright (C) 2023 - 2024, Guangzhou Shiyue Network Technology Co., Ltd.
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

namespace GameEngine.Loader.Structuring
{
    /// <summary>
    /// 视图对象模块的结构信息
    /// </summary>
    internal sealed class ViewCodeInfo : EntityCodeInfo
    {
        /// <summary>
        /// 视图分组名称
        /// </summary>
        public string GroupName { get; internal set; }
        /// <summary>
        /// 视图窗口类型
        /// </summary>
        public ViewFormType FormType { get; internal set; }

        /// <summary>
        /// 通知接口模块的函数类型集合
        /// </summary>
        private MethodTypeList<CViewNoticeMethodTypeCodeInfo> _noticeMethodTypes;

        internal MethodTypeList<CViewNoticeMethodTypeCodeInfo> NoticeMethodTypes => _noticeMethodTypes;

        /// <summary>
        /// 共生关系的视图名称列表
        /// </summary>
        private IList<string> _groupOfSymbioticViewNames;

        internal IList<string> GroupOfSymbioticViewNames => _groupOfSymbioticViewNames;

        #region 视图对象内部通知接口函数的注册/访问相关接口

        /// <summary>
        /// 新增指定函数的回调句柄相关的结构信息
        /// </summary>
        /// <param name="invoke">函数的结构信息</param>
        public void AddNoticeMethodType(CViewNoticeMethodTypeCodeInfo invoke)
        {
            if (null == _noticeMethodTypes)
            {
                _noticeMethodTypes = new MethodTypeList<CViewNoticeMethodTypeCodeInfo>();
            }

            _noticeMethodTypes.Add(invoke);
        }

        /// <summary>
        /// 移除所有函数的回调句柄相关的结构信息
        /// </summary>
        public void RemoveAllNoticeMethodTypes()
        {
            _noticeMethodTypes?.Clear();
            _noticeMethodTypes = null;
        }

        /// <summary>
        /// 获取当前函数回调句柄的结构信息数量
        /// </summary>
        /// <returns>返回函数回调句柄的结构信息数量</returns>
        public int GetNoticeMethodTypeCount()
        {
            return _noticeMethodTypes?.Count() ?? 0;
        }

        /// <summary>
        /// 获取当前函数回调句柄的结构信息容器中指索引对应的实例
        /// </summary>
        /// <param name="index">索引值</param>
        /// <returns>返回给定索引值对应的实例，若不存在对应实例则返回null</returns>
        public CViewNoticeMethodTypeCodeInfo GetNoticeMethodType(int index)
        {
            return _noticeMethodTypes?.Get(index);
        }

        #endregion

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
    }

    /// <summary>
    /// 视图对象通知接口模块的函数结构信息
    /// </summary>
    internal sealed class CViewNoticeMethodTypeCodeInfo : MethodTypeCodeInfo
    {
        /// <summary>
        /// 视图对象接口通知类型
        /// </summary>
        public ViewNoticeType NoticeType { get; internal set; }

        /// <summary>
        /// 订阅绑定的观察行为类型
        /// </summary>
        public AspectBehaviourType BehaviourType { get; internal set; }
    }
}
