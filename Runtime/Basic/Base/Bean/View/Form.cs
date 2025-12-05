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

using System;
using Cysharp.Threading.Tasks;

namespace GameEngine
{
    /// <summary>
    /// 基于引擎封装的GUI表单结构对象类<br/>
    /// 该对象类为一个抽象基类，通过封装接口操作显示具体的子类UI<br/>
    /// <br/>
    /// 内部对具体的UI框架进行二次封装，例如：FairyGUI, UGUI, UI Toolkit
    /// </summary>
    public abstract class Form
    {
        /*
         * 
         * 当Form被创建至场景中时，会先后执行OnOpen，OnResume，OnReveal回调；
         * 
         * 当Form因层级关系变化需要被暂停时，执行OnPause回调；
         * 当Form因层级变化而不再处于最高层，导致被遮挡时，执行OnCover回调；
         * 
         * 当Form因层级关系变化不再被暂停时，执行OnResume回调；
         * 当Form因层级关系变化而变为最高层，从而即将显示时，执行OnReveal回调；
         * 
         * 当Form即将被销毁时：
         * 1. 先判断此刻是否已经被暂停，如果未暂停则执行OnPause回调；
         * 2. 再判断此刻是否在层级里处于最高层，即正处于显示状态，则执行OnCover回调；
         * 3. 最后执行OnClose回调，并继续完成实体对象剩余生命周期处理回调；
         * 
         */

        // protected abstract void OnOpen(object userData);

        // protected abstract void OnResume();

        // protected abstract void OnReveal();

        // protected abstract void OnPause();

        // protected abstract void OnCover();

        // protected abstract void OnClose();

        /// <summary>
        /// 窗口对象所属的视图类型
        /// </summary>
        protected Type _viewType = null;

        /// <summary>
        /// 窗口对象载入完成状态标识
        /// </summary>
        protected bool _isLoaded = false;

        /// <summary>
        /// 获取窗口对象的根节点实例
        /// </summary>
        public abstract object Root { get; }

        /// <summary>
        /// 获取窗口对象所属的视图类型
        /// </summary>
        public Type ViewType => _viewType;

        /// <summary>
        /// 获取窗口对象当前载入状态
        /// </summary>
        public bool IsLoaded => _isLoaded;

        protected Form(Type viewType)
        {
            _viewType = viewType;
        }

        /// <summary>
        /// 窗口实例的加载接口函数
        /// </summary>
        protected internal virtual async UniTask Load()
        {
            await UniTask.CompletedTask;

            Debugger.Throw<NotImplementedException>();
        }

        /// <summary>
        /// 窗口实例的卸载接口函数
        /// </summary>
        protected internal virtual void Unload()
        {
            Debugger.Throw<NotImplementedException>();
        }

        /// <summary>
        /// 窗口实例的显示接口函数
        /// </summary>
        protected internal virtual void Show()
        {
            Debugger.Throw<NotImplementedException>();
        }

        /// <summary>
        /// 窗口实例的隐藏接口函数
        /// </summary>
        protected internal virtual void Hide()
        {
            Debugger.Throw<NotImplementedException>();
        }

        #region 窗口对象内部节点访问相关的接口函数

        /// <summary>
        /// 通过指定路径搜索窗口对象中的节点元素
        /// </summary>
        /// <param name="path">节点路径</param>
        /// <returns>返回查找到的节点对象实例</returns>
        public virtual object GetChild(string path)
        {
            Debugger.Throw<NotImplementedException>();
            return null;
        }

        #endregion
    }
}
