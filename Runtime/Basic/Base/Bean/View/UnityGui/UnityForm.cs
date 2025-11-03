/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
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

using SystemType = System.Type;

using UnityGameObject = UnityEngine.GameObject;
using UnityTransform = UnityEngine.Transform;
using UniTask = Cysharp.Threading.Tasks.UniTask;

namespace GameEngine
{
    /// <summary>
    /// 基于UGUI封装的窗口对象类
    /// </summary>
    public sealed class UnityForm : Form
    {
        /// <summary>
        /// 视图对象的游戏节点对象实例
        /// </summary>
        private UnityGameObject _gameObject;
        /// <summary>
        /// 视图对象的游戏节点转换组件实例
        /// </summary>
        private UnityTransform _gameTransform;

        /// <summary>
        /// 窗口根节点对象实例
        /// </summary>
        public override object Root => _gameObject;

        internal UnityForm(SystemType viewType) : base(viewType)
        {
        }

        ~UnityForm()
        { }

        /// <summary>
        /// 窗口实例的加载接口函数
        /// </summary>
        protected internal override sealed async UniTask Load()
        {
            UnityGameObject panel = await UnityFormHelper.OnWindowLoaded(_viewType);

            if (null != panel)
            {
                _isLoaded = true;

                _gameObject = panel;
                _gameTransform = panel.transform;

                // 编辑器下显示名字
                if (NovaEngine.Environment.IsDevelopmentState())
                {
                    _gameObject.name = $"{_viewType.Name}";
                }
            }
        }

        /// <summary>
        /// 窗口实例的卸载接口函数
        /// </summary>
        protected internal override sealed void Unload()
        {
            UnityGameObject.Destroy(_gameObject);
            _gameObject = null;
            _gameTransform = null;

            _isLoaded = false;
        }

        /// <summary>
        /// 窗口实例的显示接口函数
        /// </summary>
        protected internal override sealed void Show()
        {
            //_window.ShowContentPane();
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
