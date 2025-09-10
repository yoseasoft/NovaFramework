/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyright (C) 2020 - 2022, Guangzhou Xinyuan Technology Co., Ltd.
/// Copyright (C) 2022 - 2023, Shanghai Bilibili Technology Co., Ltd.
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

namespace GameEngine
{
    /// <summary>
    /// 节点对象抽象类，对场景中的节点对象上下文进行封装及调度管理
    /// </summary>
    public abstract class CNode : CRef
    {
        /// <summary>
        /// 当前节点对象的父节点实例
        /// </summary>
        private CNode _parent = null;
        /// <summary>
        /// 当前节点对象的子节点列表
        /// </summary>
        private IList<CNode> _childrens = null;

        /// <summary>
        /// 节点初始化通知接口函数
        /// </summary>
        public override sealed void Initialize()
        {
            base.Initialize();

            // 初始化节点列表
            _childrens = new List<CNode>();

            OnInitialize();
        }

        /// <summary>
        /// 节点内部初始化通知接口函数
        /// </summary>
        protected virtual void OnInitialize() { }

        /// <summary>
        /// 节点清理通知接口函数
        /// </summary>
        public override sealed void Cleanup()
        {
            OnCleanup();

            // 清理节点列表
            RemoveAllChildrens();
            _childrens = null;

            base.Cleanup();
        }

        /// <summary>
        /// 节点内部清理通知接口函数
        /// </summary>
        protected virtual void OnCleanup() { }

        /// <summary>
        /// 节点启动通知接口函数
        /// </summary>
        public override sealed void Startup()
        {
            // base.Startup();

            OnStartup();
        }

        /// <summary>
        /// 节点内部启动通知接口函数
        /// </summary>
        protected virtual void OnStartup() { }

        /// <summary>
        /// 节点关闭通知接口函数
        /// </summary>
        public override sealed void Shutdown()
        {
            OnShutdown();

            // base.Shutdown();
        }

        /// <summary>
        /// 节点内部关闭通知接口函数
        /// </summary>
        protected virtual void OnShutdown() { }

        #region 子节点增删改查的操作函数接口

        protected internal static CNode CreateNode()
        {
            return null;
        }

        protected internal static void ReleaseNode(CNode node)
        {
        }

        /// <summary>
        /// 添加指定目标节点实例到当前节点对象的子列表中
        /// </summary>
        /// <param name="child">节点对象实例</param>
        /// <returns>若节点对象实例添加成功则返回true，否则返回false</returns>
        public bool AddChild(CNode child)
        {
            return false;
        }

        /// <summary>
        /// 获取当前节点对象实例的子节点数量
        /// </summary>
        /// <returns>返回节点对象实例的子节点数量</returns>
        public int GetChildCount()
        {
            return _childrens?.Count ?? 0;
        }

        /// <summary>
        /// 从当前节点对象中移除指定目标子节点实例
        /// </summary>
        /// <param name="child">子节点对象实例</param>
        public void RemoveChild(CNode child)
        {
            if (null == child)
            {
                Debugger.Warn(LogGroupTag.Bean, "移除的目标节点对象不能为空，移除节点操作失败！");
                return;
            }

            if (false == _childrens.Contains(child))
            {
                Debugger.Warn(LogGroupTag.Bean, "移除目标节点对象异常：当前移除的目标节点对象实例‘{%t}’不属于当前节点对象‘{%t}’的子节点元素，无法使用当前实例对目标节点对象进行移除操作。", child, this);
                return;
            }

            if (child.GetChildCount() > 0)
            {
                // 先移除其所有子节点
                child.RemoveAllChildrens();
            }

            ReleaseNode(child);
        }

        /// <summary>
        /// 移除隶属于当前节点对象的所有子节点对象实例
        /// </summary>
        public void RemoveAllChildrens()
        {
            while (_childrens.Count > 0)
            {
                RemoveChild(_childrens[0]);
            }
        }

        #endregion
    }
}
