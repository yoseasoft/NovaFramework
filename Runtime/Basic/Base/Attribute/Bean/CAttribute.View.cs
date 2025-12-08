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

using System;

namespace GameEngine
{
    /// <summary>
    /// 视图实现类声明属性类型定义
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class CViewClassAttribute : CEntityDeclareClassAttribute
    {
        public CViewClassAttribute(string viewName) : this(viewName, 0)
        { }

        public CViewClassAttribute(int priority) : this(null, priority)
        { }

        public CViewClassAttribute(string viewName, int priority) : base(viewName, priority)
        { }
    }

    /// <summary>
    /// 视图分组策略声明属性类型定义
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class CViewGroupAttribute : Attribute
    {
        /// <summary>
        /// 视图分组名称
        /// </summary>
        private readonly string _groupName;

        /// <summary>
        /// 视图分组名称获取函数
        /// </summary>
        public string GroupName => _groupName;

        public CViewGroupAttribute(string groupName) : base()
        {
            _groupName = groupName;
        }
    }

    /// <summary>
    /// 视图通知函数的属性类型定义
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public class CViewNoticeCallAttribute : Attribute
    {
        /// <summary>
        /// 视图通知类型
        /// </summary>
        private readonly ViewNoticeType _noticeType;

        /// <summary>
        /// 监听绑定的观察行为类型
        /// </summary>
        private readonly AspectBehaviourType _behaviourType;

        public ViewNoticeType NoticeType => _noticeType;
        public AspectBehaviourType BehaviourType => _behaviourType;

        public CViewNoticeCallAttribute(ViewNoticeType noticeType) : this(noticeType, AspectBehaviourType.Initialize)
        { }

        public CViewNoticeCallAttribute(ViewNoticeType noticeType, AspectBehaviourType behaviourType) : base()
        {
            _noticeType = noticeType;
            _behaviourType = behaviourType;
        }
    }

    /// <summary>
    /// 视图共生关系组的属性类型定义
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class ViewGroupOfSymbioticRelationshipsAttribute : Attribute
    {
        /// <summary>
        /// 视图名称标识
        /// </summary>
        private readonly string _viewName;

        /// <summary>
        /// 视图名称获取函数
        /// </summary>
        public string ViewName => _viewName;

        public ViewGroupOfSymbioticRelationshipsAttribute(string viewName) : base()
        {
            _viewName = viewName;
        }
    }
}
