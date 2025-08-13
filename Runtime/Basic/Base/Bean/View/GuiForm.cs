/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyright (C) 2024 - 2025, Hurley, Independent Studio.
/// Copyright (C) 2025, Hainan Yuanyou Information Tecdhnology Co., Ltd. Guangzhou Branch
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

#define ON_FAIRYGUI_SUPPORTED   // 开启FairyGUI框架支持
#define ON_UGUI_SUPPORTED       // 开启UGUI框架支持
#define ON_UITOOLKIT_SUPPORTED  // 开启UIToolkit框架支持

namespace GameEngine
{
    /// <summary>
    /// 基于引擎封装的GUI表单结构对象类<br/>
    /// 该对象类为一个抽象基类，通过封装接口操作显示具体的子类UI<br/>
    /// <br/>
    /// 目前封装的UI包括：FairyGUI, UGUI, UI Toolkit
    /// </summary>
    public abstract class GuiForm
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



        protected abstract void OnOpen(object userData);

        protected abstract void OnResume();

        protected abstract void OnReveal();

        protected abstract void OnPause();

        protected abstract void OnCover();

        protected abstract void OnClose();
    }
}
