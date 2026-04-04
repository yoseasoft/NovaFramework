/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyright (C) 2025 - 2026, Hainan Yuanyou Information Technology Co., Ltd. Guangzhou Branch
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
    /// 基于ECS模式的组件对象封装类
    public abstract partial class CComponent
    {
        #region 组件对象数据同步分发调度接口函数

        /// <summary>
        /// 属性变化的分发调度接口函数
        /// </summary>
        /// <param name="propertyName">变化属性名称</param>
        internal override sealed void OnPropertyChangedDispatch(string propertyName)
        {
            string tags = ReplicateController.Instance.RetrievingCompleteReplicateTags(Entity.BeanType, BeanType, propertyName);
            if (null == tags)
            {
                Debugger.Warn(LogGroupTag.Bean, "");
                return;
            }

            SendToSelf(tags, ReplicateAnnounceType.Changed);
        }

        #endregion
    }
}
