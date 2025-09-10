/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyright (C) 2025, Hurley, Independent Studio.
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

using SystemType = System.Type;

using FairyGComponent = FairyGUI.GComponent;

using UniTask = Cysharp.Threading.Tasks.UniTask;

namespace GameEngine
{
    /// <summary>
    /// 基于FairyGui封装的窗口对象类
    /// </summary>
    public sealed class FairyForm : Form
    {
        /// <summary>
        /// 视图对象挂载的窗口实例
        /// </summary>
        private BaseWindow _window;

        /// <summary>
        /// 窗口设置相关数据结构
        /// </summary>
        private WindowSettings _settings;

        /// <summary>
        /// 视图对象的模型根节点
        /// </summary>
        public FairyGComponent ContentPane => _window?.contentPane;

        /// <summary>
        /// 窗口根节点对象实例
        /// </summary>
        public override object Root => _window?.contentPane;

        internal FairyForm(SystemType viewType) : base(viewType)
        {
            _settings = new WindowSettings(viewType?.Name);
        }

        /// <summary>
        /// 窗口实例的加载接口函数
        /// </summary>
        protected internal override sealed async UniTask Load()
        {
            _window = new BaseWindow(_settings);
            _window.Show();

            await _window.WaitLoadAsync();

            if (null != _window.contentPane)
            {
                _window.contentPane.visible = false;

                // 编辑器下显示名字
                if (NovaEngine.Environment.IsDevelopmentState())
                {
                    _window.gameObjectName = $"{_viewType.Name}(Pkg:{_settings.pkgName},Com:{_settings.comName})";
                }
            }
        }

        /// <summary>
        /// 窗口实例的卸载接口函数
        /// </summary>
        protected internal override sealed void Unload()
        {
            _window?.Dispose();
            _window = null;
        }

        /// <summary>
        /// 窗口实例的显示接口函数
        /// </summary>
        protected internal override sealed void Show()
        {
            // if (null != ContentPane) ContentPane.visible = true;

            _window.ShowContentPane();
        }

        /// <summary>
        /// 窗口实例的隐藏接口函数
        /// </summary>
        protected internal override sealed void Hide()
        {
            Debugger.Throw<System.NotImplementedException>();
        }
    }
}
