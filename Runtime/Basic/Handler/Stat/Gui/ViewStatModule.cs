/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
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

using SystemDateTime = System.DateTime;

namespace GameEngine
{
    /// <summary>
    /// 视图统计模块，对视图模块对象提供数据统计所需的接口函数
    /// </summary>
    public sealed class ViewStatModule : HandlerStatSingleton<ViewStatModule>, IStatModule
    {
        public const int ON_VIEW_CREATE_CALL = 1;
        public const int ON_VIEW_CLOSE_CALL = 2;

        /// <summary>
        /// 视图访问统计信息容器列表
        /// </summary>
        private IList<ViewStatInfo> _viewStatInfos = null;

        /// <summary>
        /// 初始化统计模块实例的回调接口
        /// </summary>
        protected override void OnInitialize()
        {
            _viewStatInfos = new List<ViewStatInfo>();
        }

        /// <summary>
        /// 清理统计模块实例的回调接口
        /// </summary>
        protected override void OnCleanup()
        {
            _viewStatInfos.Clear();
            _viewStatInfos = null;
        }

        /// <summary>
        /// 卸载统计模块实例中的垃圾数据
        /// </summary>
        public void Dump()
        {
            _viewStatInfos.Clear();
        }

        /// <summary>
        /// 获取当前所有视图访问的统计信息
        /// </summary>
        /// <returns>返回所有的操作访问统计信息</returns>
        public IList<IStatInfo> GetAllStatInfos()
        {
            List<IStatInfo> results = new List<IStatInfo>();
            results.AddRange(_viewStatInfos);

            return results;
        }

        [IStatModule.OnStatModuleRegisterCallback(ON_VIEW_CREATE_CALL)]
        private void OnViewCreate(CView view)
        {
            ViewStatInfo info = null;

            int uid = _viewStatInfos.Count + 1;

            info = new ViewStatInfo(uid, view.Name, view.GetHashCode());
            info.CreateTime = SystemDateTime.UtcNow;
            _viewStatInfos.Add(info);
        }

        [IStatModule.OnStatModuleRegisterCallback(ON_VIEW_CLOSE_CALL)]
        private void OnViewClose(CView view)
        {
            ViewStatInfo info = null;
            if (false == TryGetViewStatInfoByHashCode(view.GetHashCode(), out info))
            {
                Debugger.Warn("Could not found any view stat info with name '{0}', exited it failed.", view.Name);
                return;
            }

            info.CloseTime = SystemDateTime.UtcNow;
        }

        private bool TryGetViewStatInfoByHashCode(int hashCode, out ViewStatInfo info)
        {
            for (int n = _viewStatInfos.Count - 1; n >= 0; --n)
            {
                ViewStatInfo found = _viewStatInfos[n];
                if (found.HashCode == hashCode)
                {
                    info = found;
                    return true;
                }
            }

            info = null;
            return false;
        }
    }
}
