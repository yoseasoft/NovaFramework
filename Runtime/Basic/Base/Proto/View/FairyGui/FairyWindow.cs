/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyright (C) 2020 - 2022, Guangzhou Xinyuan Technology Co., Ltd.
/// Copyright (C) 2022 - 2023, Shanghai Bilibili Technology Co., Ltd.
/// Copyright (C) 2023 - 2024, Guangzhou Shiyue Network Technology Co., Ltd.
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

using UnityVector2 = UnityEngine.Vector2;
using UnityRect = UnityEngine.Rect;
using UnityScreen = UnityEngine.Screen;
using UnityScreenOrientation = UnityEngine.ScreenOrientation;

using FairyGObject = FairyGUI.GObject;
using FairyGRoot = FairyGUI.GRoot;
using FairyIUISource = FairyGUI.IUISource;
using FairyUILoadCallback = FairyGUI.UILoadCallback;
using FairyRelationType = FairyGUI.RelationType;

using UniTask = Cysharp.Threading.Tasks.UniTask;
using UniTaskCompletionSource = Cysharp.Threading.Tasks.UniTaskCompletionSource;

namespace GameEngine
{
    /// <summary>
    /// UI基础窗口组件
    /// </summary>
    public class BaseWindow : FairyGUI.Window
    {
        /// <summary>
        /// 窗口设置
        /// </summary>
        readonly WindowSettings _settings;

        /// <summary>
        /// 窗口加载支持
        /// </summary>
        readonly BaseWindowLoaded _baseWindowLoaded;

        /// <summary>
        /// 自定义安全区
        /// </summary>
        static UnityRect _customSafeArea;

        public BaseWindow(WindowSettings settings)
        {
            _settings = settings;
            _baseWindowLoaded = new BaseWindowLoaded(this, settings);
            AddUISource(_baseWindowLoaded);
        }

        /// <summary>
        /// 基类初始化完成处理
        /// </summary>
        protected override void OnInit()
        {
            sortingOrder = _settings.sortingOrder;
            if (_settings.isFullScreen)
            {
                RefreshWindowSize();
                FairyGRoot.inst.onSizeChanged.Add(RefreshWindowSize);
            }
        }

        /// <summary>
        /// 窗口销毁处理
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();
            if (_settings.isFullScreen)
            {
                FairyGRoot.inst.touchable = true;
            }
            FairyGRoot.inst.onSizeChanged.Remove(RefreshWindowSize);
            FairyGuiHelper.RemoveWindowPackage(_settings.pkgName);
        }

        /// <summary>
        /// 显示窗口内容
        /// </summary>
        public void ShowContentPane()
        {
            base.OnShown();

            if (contentPane != null)
            {
                contentPane.visible = true;
            }

            // 全屏UI加载完成并显示时, 启用UI点击
            if (_settings.isFullScreen)
            {
                FairyGRoot.inst.touchable = true;
            }
        }

        /// <summary>
        /// 等待加载完成
        /// </summary>
        public async UniTask WaitLoadAsync()
        {
            if (_baseWindowLoaded != null)
            {
                await _baseWindowLoaded.task;
            }
        }

        /// <summary>
        /// 设置自定义安全区, 并刷新所有窗口(目前由Editor层反射调用)
        /// </summary>
        static void SetCustomSafeArea(UnityRect rect)
        {
            _customSafeArea = rect;

            for (int i = 0; i < FairyGRoot.inst.numChildren; i++)
            {
                if (FairyGRoot.inst.GetChildAt(i) is BaseWindow baseWindow && baseWindow._settings.isFullScreen)
                {
                    baseWindow.RefreshWindowSize();
                }
            }
        }

        /// <summary>
        /// 刷新窗口的大小(默认全屏, 若有安全区则另外处理)
        /// </summary>
        public void RefreshWindowSize()
        {
            // 先还原默认设置
            x = 0f;
            y = 0f;
            MakeFullScreen();

            // 获取安全区参数, 若是编辑器下且有自定义安全区, 则使用自定义安全区
            UnityRect safeArea;
            if (!UnityEngine.Application.isEditor)
            {
                safeArea = UnityScreen.safeArea;
            }
            else
            {
                if (_customSafeArea.x != 0 || _customSafeArea.y != 0 || _customSafeArea.width != 0 || _customSafeArea.height != 0)
                {
                    safeArea = _customSafeArea;
                }
                else
                {
                    safeArea = UnityScreen.safeArea;
                }
            }

            // 竖屏游戏处理
            if (UnityScreen.orientation is UnityScreenOrientation.Portrait or UnityScreenOrientation.PortraitUpsideDown)
            {
                // 顶部安全区处理
                if ((int)safeArea.yMax < UnityScreen.height)
                {
                    UnityVector2 localPos = FairyGRoot.inst.GlobalToLocal(new UnityVector2(0, UnityScreen.height - safeArea.yMax)); // 先转换为逻辑坐标
                    y = localPos.y;
                    height -= y;
                }

                // 底部安全区处理
                if (safeArea.y > 0)
                {
                    UnityVector2 localPos = FairyGRoot.inst.GlobalToLocal(new UnityVector2(0, safeArea.y)); // 先转换为逻辑坐标
                    height -= localPos.y;
                }

                // 经过和美术沟通, 此项目UI操作上不适合完全适配宽屏, 最后选择以下方案:UI居中后两边留黑, 配置了FullScreenWidth的界面除外
                if (contentPane is { baseUserData: "FullScreenWidth" })
                {
                    return;
                }

                const float normalRate = 1080f / 1920;
                if (FairyGRoot.inst.width / FairyGRoot.inst.height > normalRate)
                {
                    width = 1080f;
                    Center();
                }
            }
            // 横屏游戏处理
            else if (UnityScreen.orientation is UnityScreenOrientation.LandscapeLeft or UnityScreenOrientation.LandscapeRight)
            {
                // 左边安全区处理
                if (safeArea.x > 0)
                {
                    UnityVector2 localPos = FairyGRoot.inst.GlobalToLocal(new UnityVector2(safeArea.x, 0)); // 先转换为逻辑坐标
                    x = localPos.x;
                    width -= x;
                }

                // 右边安全区处理
                if ((int)safeArea.xMax < UnityScreen.width)
                {
                    UnityVector2 localPos = FairyGRoot.inst.GlobalToLocal(new UnityVector2(UnityScreen.width - safeArea.xMax, 0)); // 先转换为逻辑坐标
                    width -= localPos.x;
                }
            }
        }
    }

    /// <summary>
    /// 视图窗口设置参数的数据结构
    /// </summary>
    public struct WindowSettings
    {
        /// <summary>
        /// 视图窗口的包名
        /// </summary>
        public string pkgName;

        /// <summary>
        /// 视图窗口的组件名
        /// </summary>
        public string comName;

        /// <summary>
        /// 视图窗口的层级
        /// </summary>
        public int sortingOrder;

        /// <summary>
        /// 视图窗口的全屏状态标识
        /// </summary>
        public bool isFullScreen;

        public WindowSettings(string pkgName, string comName = "Main", int sortingOrder = 0, bool isFullScreen = true)
        {
            this.pkgName = pkgName;
            this.comName = comName;
            this.sortingOrder = sortingOrder;
            this.isFullScreen = isFullScreen;
        }
    }

    /// <summary>
    /// 视图窗口加载操作管理对象类
    /// </summary>
    class BaseWindowLoaded : FairyIUISource
    {
        /// <summary>
        /// 视图窗口实例
        /// </summary>
        readonly BaseWindow _window;

        /// <summary>
        /// 视图窗口设置参数数据结构
        /// </summary>
        readonly WindowSettings _settings;

        /// <summary>
        /// 视图窗口加载回调句柄
        /// </summary>
        FairyUILoadCallback _loadedCallback;

        /// <summary>
        /// 异步任务结果
        /// </summary>
        readonly UniTaskCompletionSource _taskCompletionSource = new();

        /// <summary>
        /// 异步任务(提供给await使用)
        /// </summary>
        public UniTask task => _taskCompletionSource.Task;

        public string fileName
        {
            get => _settings.pkgName;
            set { }
        }

        public bool loaded { get; private set; }

        public BaseWindowLoaded(BaseWindow window, WindowSettings settings)
        {
            _window = window;
            _settings = settings;
        }

        public void Load(FairyUILoadCallback callback)
        {
            // 全屏UI加载时, 屏蔽UI点击
            if (_settings.isFullScreen)
            {
                FairyGRoot.inst.touchable = false;
            }

            _loadedCallback = callback;

            try
            {
                FairyGuiHelper.CreateContentPaneAsync(_settings.pkgName, _settings.comName, CreateObjectFinish);
            }
            catch (System.Exception e)
            {
                Debugger.Error(e);
                if (_settings.isFullScreen)
                {
                    FairyGRoot.inst.touchable = true;
                }
            }
        }

        public void Cancel()
        {
        }

        /// <summary>
        /// UI加载完成处理回调函数
        /// </summary>
        void CreateObjectFinish(FairyGObject obj)
        {
            // 加载失败需要打开触摸
            if (obj is null && _settings.isFullScreen)
            {
                FairyGRoot.inst.touchable = true;
            }

            if (_window.isDisposed)
            {
                obj?.Dispose();
                return;
            }

            // 若obj为空则视为加载失败
            if (obj != null)
            {
                _window.contentPane = obj.asCom;
            }

            loaded = true;
            _loadedCallback();

            _taskCompletionSource.TrySetResult();
        }
    }
}
