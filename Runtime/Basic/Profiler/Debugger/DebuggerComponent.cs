/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyright (C) 2023, Guangzhou Shiyue Network Technology Co., Ltd.
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

using UnityColor = UnityEngine.Color;
using UnityColor32 = UnityEngine.Color32;
using UnityRect = UnityEngine.Rect;
using UnityVector3 = UnityEngine.Vector3;
using UnityTime = UnityEngine.Time;
using UnityTextEditor = UnityEngine.TextEditor;
using UnityGUILayout = UnityEngine.GUILayout;
using UnityLogType = UnityEngine.LogType;
using UnityObject = UnityEngine.Object;
using UnityTexture = UnityEngine.Texture;
using UnityMesh = UnityEngine.Mesh;
using UnityMaterial = UnityEngine.Material;
using UnityShader = UnityEngine.Shader;
using UnityAnimationClip = UnityEngine.AnimationClip;
using UnityAudioClip = UnityEngine.AudioClip;
using UnityFont = UnityEngine.Font;
using UnityTextAsset = UnityEngine.TextAsset;
using UnityScriptableObject = UnityEngine.ScriptableObject;

namespace GameEngine.Profiler.Debugging
{
    /// <summary>
    /// 游戏调试器组件对象类，用于定义调试器对象的基础属性及访问操作函数
    /// </summary>
    [UnityEngine.DisallowMultipleComponent]
    [UnityEngine.AddComponentMenu("Framework/Debugger")]
    public sealed partial class DebuggerComponent : NovaEngine.CFrameworkComponent
    {
        /// <summary>
        /// 调试器组件的挂载名称
        /// </summary>
        public const string MOUNTING_GAMEOBJECT_NAME = "GameDebugger";

        /// <summary>
        /// 默认调试器漂浮窗口大小
        /// </summary>
        internal static readonly UnityRect DefaultIconRect = new UnityRect(10f, 10f, 60f, 60f);

        /// <summary>
        /// 默认调试器信息窗口大小
        /// </summary>
        internal static readonly UnityRect DefaultWindowRect = new UnityRect(10f, 10f, 640f, 480f);

        /// <summary>
        /// 默认调试器信息窗口缩放比例
        /// </summary>
        internal static readonly float DefaultWindowScale = 1.0f;

        /// <summary>
        /// 调试器组件通用的文本编辑对象实例
        /// </summary>
        private static readonly UnityTextEditor _textEditor = new UnityTextEditor();
        /// <summary>
        /// 调试管理器对象实例
        /// </summary>
        private IDebuggerManager _debuggerManager = null;
        private UnityRect _dragRect = new UnityRect(0f, 0f, float.MaxValue, 25f);
        private UnityRect _iconRect = DefaultIconRect;
        private UnityRect _windowRect = DefaultWindowRect;
        private float _windowScale = DefaultWindowScale;

        [UnityEngine.SerializeField]
        private UnityEngine.GUISkin _skin = null;

        [UnityEngine.SerializeField]
        private ActiveWindowType _activeWindowType = ActiveWindowType.AlwaysOpen;

        [UnityEngine.SerializeField]
        private bool _showFullWindow = false;

        [UnityEngine.SerializeField]
        private ConsoleWindow _consoleWindow = new ConsoleWindow();

        private readonly SystemInformationWindow _systemInformationWindow = new SystemInformationWindow();
        private EnvironmentInformationWindow _environmentInformationWindow = new EnvironmentInformationWindow();
        private ScreenInformationWindow _screenInformationWindow = new ScreenInformationWindow();
        private GraphicsInformationWindow _graphicsInformationWindow = new GraphicsInformationWindow();
        private InputSummaryInformationWindow _inputSummaryInformationWindow = new InputSummaryInformationWindow();
        private InputTouchInformationWindow _inputTouchInformationWindow = new InputTouchInformationWindow();
        private InputLocationInformationWindow _inputLocationInformationWindow = new InputLocationInformationWindow();
        private InputAccelerationInformationWindow _inputAccelerationInformationWindow = new InputAccelerationInformationWindow();
        private InputGyroscopeInformationWindow _inputGyroscopeInformationWindow = new InputGyroscopeInformationWindow();
        private InputCompassInformationWindow _inputCompassInformationWindow = new InputCompassInformationWindow();
        private PathInformationWindow _pathInformationWindow = new PathInformationWindow();
        private SceneInformationWindow _sceneInformationWindow = new SceneInformationWindow();
        private TimeInformationWindow _timeInformationWindow = new TimeInformationWindow();
        private QualityInformationWindow _qualityInformationWindow = new QualityInformationWindow();
        private ProfilerInformationWindow _profilerInformationWindow = new ProfilerInformationWindow();
        private RuntimeMemorySummaryWindow _runtimeMemorySummaryWindow = new RuntimeMemorySummaryWindow();
        private RuntimeMemoryInformationWindow<UnityObject> _runtimeMemoryAllInformationWindow = new RuntimeMemoryInformationWindow<UnityObject>();
        private RuntimeMemoryInformationWindow<UnityTexture> _runtimeMemoryTextureInformationWindow = new RuntimeMemoryInformationWindow<UnityTexture>();
        private RuntimeMemoryInformationWindow<UnityMesh> _runtimeMemoryMeshInformationWindow = new RuntimeMemoryInformationWindow<UnityMesh>();
        private RuntimeMemoryInformationWindow<UnityMaterial> _runtimeMemoryMaterialInformationWindow = new RuntimeMemoryInformationWindow<UnityMaterial>();
        private RuntimeMemoryInformationWindow<UnityShader> _runtimeMemoryShaderInformationWindow = new RuntimeMemoryInformationWindow<UnityShader>();
        private RuntimeMemoryInformationWindow<UnityAnimationClip> _runtimeMemoryAnimationClipInformationWindow = new RuntimeMemoryInformationWindow<UnityAnimationClip>();
        private RuntimeMemoryInformationWindow<UnityAudioClip> _runtimeMemoryAudioClipInformationWindow = new RuntimeMemoryInformationWindow<UnityAudioClip>();
        private RuntimeMemoryInformationWindow<UnityFont> _runtimeMemoryFontInformationWindow = new RuntimeMemoryInformationWindow<UnityFont>();
        private RuntimeMemoryInformationWindow<UnityTextAsset> _runtimeMemoryTextAssetInformationWindow = new RuntimeMemoryInformationWindow<UnityTextAsset>();
        private RuntimeMemoryInformationWindow<UnityScriptableObject> _runtimeMemoryScriptableObjectInformationWindow = new RuntimeMemoryInformationWindow<UnityScriptableObject>();
        private RuntimeTimerModuleStatInformationWindow _runtimeTimerModuleStatInformationWindow = new RuntimeTimerModuleStatInformationWindow();
        private RuntimeNetworkModuleStatInformationWindow _runtimeNetworkModuleStatInformationWindow = new RuntimeNetworkModuleStatInformationWindow();
        private RuntimeSceneModuleStatInformationWindow _runtimeSceneModuleStatInformationWindow = new RuntimeSceneModuleStatInformationWindow();
        private RuntimeActorModuleStatInformationWindow _runtimeActorModuleStatInformationWindow = new RuntimeActorModuleStatInformationWindow();
        private RuntimeViewModuleStatInformationWindow _runtimeViewModuleStatInformationWindow = new RuntimeViewModuleStatInformationWindow();
        private SettingsWindow _settingsWindow = new SettingsWindow();
        private OperationsWindow _operationsWindow = new OperationsWindow();

        /// <summary>
        /// 帧率计数器对象实例
        /// </summary>
        private FpsCounter _fpsCounter = null;

        /// <summary>
        /// 获取或设置调试器的窗口实例是否激活
        /// </summary>
        public bool ActiveWindow
        {
            get { return _debuggerManager.ActiveWindow; }
            set
            {
                _debuggerManager.ActiveWindow = value;
                enabled = value;
            }
        }

        /// <summary>
        /// 获取或设置是否显示完整的调试器界面
        /// </summary>
        public bool ShowFullWindow
        {
            get { return _showFullWindow; }
            set { _showFullWindow = value; }
        }

        /// <summary>
        /// 获取或设置调试器漂浮框大小
        /// </summary>
        public UnityRect IconRect
        {
            get { return _iconRect; }
            set { _iconRect = value; }
        }

        /// <summary>
        /// 获取或设置调试器窗口大小
        /// </summary>
        public UnityRect WindowRect
        {
            get { return _windowRect; }
            set { _windowRect = value; }
        }

        /// <summary>
        /// 获取或设置调试器窗口缩放比例
        /// </summary>
        public float WindowScale
        {
            get { return _windowScale; }
            set { _windowScale = value; }
        }

        /// <summary>
        /// 调试器组件的初始唤醒回调函数
        /// </summary>
        private void Awake()
        {
            _debuggerManager = NovaEngine.AppEntry.GetManager<IDebuggerManager>();
            if (null == _debuggerManager)
            {
                Debugger.Fatal("Debugger manager is invalid.");
                return;
            }

            // 帧率计数器对象初始化
            _fpsCounter = new FpsCounter(0.5f);
        }

        private void Start()
        {
            RegisterDebuggerWindow("Console", _consoleWindow);
            RegisterDebuggerWindow("Information/System", _systemInformationWindow);
            RegisterDebuggerWindow("Information/Environment", _environmentInformationWindow);
            RegisterDebuggerWindow("Information/Screen", _screenInformationWindow);
            RegisterDebuggerWindow("Information/Graphics", _graphicsInformationWindow);
            RegisterDebuggerWindow("Information/Input/Summary", _inputSummaryInformationWindow);
            RegisterDebuggerWindow("Information/Input/Touch", _inputTouchInformationWindow);
            RegisterDebuggerWindow("Information/Input/Location", _inputLocationInformationWindow);
            RegisterDebuggerWindow("Information/Input/Acceleration", _inputAccelerationInformationWindow);
            RegisterDebuggerWindow("Information/Input/Gyroscope", _inputGyroscopeInformationWindow);
            RegisterDebuggerWindow("Information/Input/Compass", _inputCompassInformationWindow);
            RegisterDebuggerWindow("Information/Other/Scene", _sceneInformationWindow);
            RegisterDebuggerWindow("Information/Other/Path", _pathInformationWindow);
            RegisterDebuggerWindow("Information/Other/Time", _timeInformationWindow);
            RegisterDebuggerWindow("Information/Other/Quality", _qualityInformationWindow);
            RegisterDebuggerWindow("Profiler/Summary", _profilerInformationWindow);
            RegisterDebuggerWindow("Profiler/Memory/Summary", _runtimeMemorySummaryWindow);
            RegisterDebuggerWindow("Profiler/Memory/All", _runtimeMemoryAllInformationWindow);
            RegisterDebuggerWindow("Profiler/Memory/Texture", _runtimeMemoryTextureInformationWindow);
            RegisterDebuggerWindow("Profiler/Memory/Mesh", _runtimeMemoryMeshInformationWindow);
            RegisterDebuggerWindow("Profiler/Memory/Material", _runtimeMemoryMaterialInformationWindow);
            RegisterDebuggerWindow("Profiler/Memory/Shader", _runtimeMemoryShaderInformationWindow);
            RegisterDebuggerWindow("Profiler/Memory/AnimationClip", _runtimeMemoryAnimationClipInformationWindow);
            RegisterDebuggerWindow("Profiler/Memory/AudioClip", _runtimeMemoryAudioClipInformationWindow);
            RegisterDebuggerWindow("Profiler/Memory/Font", _runtimeMemoryFontInformationWindow);
            RegisterDebuggerWindow("Profiler/Memory/TextAsset", _runtimeMemoryTextAssetInformationWindow);
            RegisterDebuggerWindow("Profiler/Memory/ScriptableObject", _runtimeMemoryScriptableObjectInformationWindow);
            RegisterDebuggerWindow("Profiler/Module/Timer", _runtimeTimerModuleStatInformationWindow);
            RegisterDebuggerWindow("Profiler/Module/Network", _runtimeNetworkModuleStatInformationWindow);
            RegisterDebuggerWindow("Profiler/Module/Scene", _runtimeSceneModuleStatInformationWindow);
            RegisterDebuggerWindow("Profiler/Module/Object", _runtimeActorModuleStatInformationWindow);
            RegisterDebuggerWindow("Profiler/Module/View", _runtimeViewModuleStatInformationWindow);
            RegisterDebuggerWindow("Other/Settings", _settingsWindow);
            RegisterDebuggerWindow("Other/Operations", _operationsWindow);

            switch (_activeWindowType)
            {
                case ActiveWindowType.AlwaysOpen:
                    ActiveWindow = true;
                    break;
                case ActiveWindowType.OnlyOpenWhenDevelopment:
                    ActiveWindow = UnityEngine.Debug.isDebugBuild;
                    break;
                case ActiveWindowType.OnlyOpenInEditor:
                    ActiveWindow = UnityEngine.Application.isEditor;
                    break;
                default:
                    ActiveWindow = false;
                    break;
            }
        }

        private void Update()
        {
            _fpsCounter.Update(UnityTime.deltaTime, UnityTime.unscaledDeltaTime);
        }

        private void OnDestroy()
        {
            NovaEngine.AppEntry.RemoveManager<IDebuggerManager>();
        }

        private void OnGUI()
        {
            if (null == _debuggerManager || !_debuggerManager.ActiveWindow)
            {
                return;
            }

            UnityEngine.GUISkin cachedGuiSkin = UnityEngine.GUI.skin;
            UnityEngine.Matrix4x4 cachedMatrix = UnityEngine.GUI.matrix;

            UnityEngine.GUI.skin = _skin;
            UnityEngine.GUI.matrix = UnityEngine.Matrix4x4.Scale(new UnityVector3(_windowScale, _windowScale, 1f));

            if (_showFullWindow)
            {
                _windowRect = UnityGUILayout.Window(0, _windowRect, DrawWindow, "<b>GAME FRAMEWORK DEBUGGER</b>");
            }
            else
            {
                _iconRect = UnityGUILayout.Window(0, _iconRect, DrawDebuggerWindowIcon, "<b>DEBUGGER</b>");
            }

            UnityEngine.GUI.skin = cachedGuiSkin;
            UnityEngine.GUI.matrix = cachedMatrix;
        }

        /// <summary>
        /// 注册指定的名称和窗口对象实例到当前组件的调试环境中
        /// </summary>
        /// <param name="path">窗口名称</param>
        /// <param name="window">窗口实例</param>
        /// <param name="args">窗口初始化参数</param>
        public void RegisterDebuggerWindow(string path, IDebuggerWindow window, params object[] args)
        {
            Debugger.Assert(null != _debuggerManager);

            _debuggerManager.RegisterDebuggerWindow(path, window, args);
        }

        /// <summary>
        /// 从当前组件的调试环境中注销指定名称对应的窗口对象实例
        /// </summary>
        /// <param name="path">窗口名称</param>
        /// <returns>若窗口注销成功返回true，否则返回false</returns>
        public bool UnregisterDebuggerWindow(string path)
        {
            Debugger.Assert(null != _debuggerManager);

            return _debuggerManager.UnregisterDebuggerWindow(path);
        }

        /// <summary>
        /// 获取指定名称对应的调试窗口对象实例
        /// </summary>
        /// <param name="path">窗口名称</param>
        /// <returns>若存在名称对应的窗口实例则返回其引用，否则返回null</returns>
        public IDebuggerWindow GetDebuggerWindow(string path)
        {
            Debugger.Assert(null != _debuggerManager);

            return _debuggerManager.GetDebuggerWindow(path);
        }

        /// <summary>
        /// 选中当前调试管理器中指定名称对应的调试窗口实例
        /// </summary>
        /// <param name="path">窗口名称</param>
        /// <returns>若选中窗口实例成功返回true，否则返回false</returns>
        public bool SelectedDebuggerWindow(string path)
        {
            Debugger.Assert(null != _debuggerManager);

            return _debuggerManager.SelectedDebuggerWindow(path);
        }

        /// <summary>
        /// 还原调试器组件的窗口布局
        /// </summary>
        public void ResetLayout()
        {
            IconRect = DefaultIconRect;
            WindowRect = DefaultWindowRect;
            WindowScale = DefaultWindowScale;
        }

        /// <summary>
        /// 获取最近新增的日志记录，通过传入的列表实例进行返回
        /// 该函数将返回记录的全部日志节点
        /// </summary>
        /// <param name="results">日志记录列表</param>
        public void GetRecentLogs(List<LogNode> results)
        {
            _consoleWindow.GetRecentLogs(results);
        }

        /// <summary>
        /// 获取最近新增的日志记录，通过传入的列表实例进行返回
        /// 此处限定了获取日志记录的数量
        /// </summary>
        /// <param name="results"></param>
        /// <param name="count"></param>
        public void GetRecentLogs(List<LogNode> results, int count)
        {
            _consoleWindow.GetRecentLogs(results, count);
        }

        private void DrawWindow(int windowId)
        {
            UnityEngine.GUI.DragWindow(_dragRect);
            DrawDebuggerWindowGroup(_debuggerManager.DebuggerWindowRoot);
        }

        private void DrawDebuggerWindowGroup(IDebuggerWindowGroup debuggerWindowGroup)
        {
            if (null == debuggerWindowGroup)
            {
                return;
            }

            List<string> names = new List<string>();
            string[] windowNames = debuggerWindowGroup.GetAllDebuggerWindowNames();
            for (int n = 0; n < windowNames.Length; ++n)
            {
                names.Add(NovaEngine.Utility.Text.Format("<b>{0}</b>", windowNames[n]));
            }

            if (_debuggerManager.DebuggerWindowRoot == debuggerWindowGroup)
            {
                names.Add("<b>Close</b>");
            }

            int toolbarIndex = UnityGUILayout.Toolbar(debuggerWindowGroup.SelectedIndex, names.ToArray(),
                    UnityGUILayout.Height(30f), UnityGUILayout.MaxWidth(UnityEngine.Screen.width));
            if (debuggerWindowGroup.DebuggerWindowCount <= toolbarIndex)
            {
                _showFullWindow = false;
                return;
            }

            if (null == debuggerWindowGroup.SelectedWindow)
            {
                return;
            }

            if (debuggerWindowGroup.SelectedIndex != toolbarIndex)
            {
                debuggerWindowGroup.SelectedWindow.OnExit();
                debuggerWindowGroup.SelectedIndex = toolbarIndex;
                debuggerWindowGroup.SelectedWindow.OnEnter();
            }

            IDebuggerWindowGroup subWindowGroup = debuggerWindowGroup.SelectedWindow as IDebuggerWindowGroup;
            if (null != subWindowGroup)
            {
                DrawDebuggerWindowGroup(subWindowGroup);
            }

            debuggerWindowGroup.SelectedWindow.OnDraw();
        }

        private void DrawDebuggerWindowIcon(int windowId)
        {
            UnityEngine.GUI.DragWindow(_dragRect);
            UnityGUILayout.Space(5);
            UnityColor32 color = UnityColor.white;

            _consoleWindow.RefreshLogCount();
            if (_consoleWindow.FatalCount > 0)
            {
                color = _consoleWindow.GetLogStringColor(UnityLogType.Exception);
            }
            else if (_consoleWindow.ErrorCount > 0)
            {
                color = _consoleWindow.GetLogStringColor(UnityLogType.Error);
            }
            else if (_consoleWindow.WarnCount > 0)
            {
                color = _consoleWindow.GetLogStringColor(UnityLogType.Warning);
            }
            else if (_consoleWindow.InfoCount > 0)
            {
                color = _consoleWindow.GetLogStringColor(UnityLogType.Log);
            }

            string title = NovaEngine.Utility.Text.Format("<color=#{0}{1}{2}{3}><b>FPS: {4}</b></color>",
                    color.r.ToString("x2"), color.g.ToString("x2"), color.b.ToString("x2"), color.a.ToString("x2"), _fpsCounter.CurrentFps.ToString("F2"));
            if (UnityGUILayout.Button(title, UnityGUILayout.Width(100f), UnityGUILayout.Height(40f)))
            {
                _showFullWindow = true;
            }
        }

        /// <summary>
        /// 将指定文本内容拷贝到临时剪切板中<br/>
        /// 注意此处的剪切板非系统及剪切板，而是专属于该调试器组件的自定义剪切板空间
        /// </summary>
        /// <param name="content">文本内容</param>
        private static void CopyToClipboard(string content)
        {
            _textEditor.text = content;
            _textEditor.OnFocus();
            _textEditor.Copy();
            _textEditor.text = string.Empty;
        }
    }
}
