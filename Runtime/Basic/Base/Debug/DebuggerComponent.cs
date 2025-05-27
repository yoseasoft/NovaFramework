/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyring (C) 2023, Guangzhou Shiyue Network Technology Co., Ltd.
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

namespace GameEngine.Debug
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
        private static readonly UnityTextEditor s_textEditor = new UnityTextEditor();
        /// <summary>
        /// 调试管理器对象实例
        /// </summary>
        private IDebuggerManager m_debuggerManager = null;
        private UnityRect m_dragRect = new UnityRect(0f, 0f, float.MaxValue, 25f);
        private UnityRect m_iconRect = DefaultIconRect;
        private UnityRect m_windowRect = DefaultWindowRect;
        private float m_windowScale = DefaultWindowScale;

        [UnityEngine.SerializeField]
        private UnityEngine.GUISkin m_skin = null;

        [UnityEngine.SerializeField]
        private ActiveWindowType m_activeWindowType = ActiveWindowType.AlwaysOpen;

        [UnityEngine.SerializeField]
        private bool m_showFullWindow = false;

        [UnityEngine.SerializeField]
        private ConsoleWindow m_consoleWindow = new ConsoleWindow();

        private readonly SystemInformationWindow m_systemInformationWindow = new SystemInformationWindow();
        private EnvironmentInformationWindow m_environmentInformationWindow = new EnvironmentInformationWindow();
        private ScreenInformationWindow m_screenInformationWindow = new ScreenInformationWindow();
        private GraphicsInformationWindow m_graphicsInformationWindow = new GraphicsInformationWindow();
        private InputSummaryInformationWindow m_inputSummaryInformationWindow = new InputSummaryInformationWindow();
        private InputTouchInformationWindow m_inputTouchInformationWindow = new InputTouchInformationWindow();
        private InputLocationInformationWindow m_inputLocationInformationWindow = new InputLocationInformationWindow();
        private InputAccelerationInformationWindow m_inputAccelerationInformationWindow = new InputAccelerationInformationWindow();
        private InputGyroscopeInformationWindow m_inputGyroscopeInformationWindow = new InputGyroscopeInformationWindow();
        private InputCompassInformationWindow m_inputCompassInformationWindow = new InputCompassInformationWindow();
        private PathInformationWindow m_pathInformationWindow = new PathInformationWindow();
        private SceneInformationWindow m_sceneInformationWindow = new SceneInformationWindow();
        private TimeInformationWindow m_timeInformationWindow = new TimeInformationWindow();
        private QualityInformationWindow m_qualityInformationWindow = new QualityInformationWindow();
        private ProfilerInformationWindow m_profilerInformationWindow = new ProfilerInformationWindow();
        private RuntimeMemorySummaryWindow m_runtimeMemorySummaryWindow = new RuntimeMemorySummaryWindow();
        private RuntimeMemoryInformationWindow<UnityObject> m_runtimeMemoryAllInformationWindow = new RuntimeMemoryInformationWindow<UnityObject>();
        private RuntimeMemoryInformationWindow<UnityTexture> m_runtimeMemoryTextureInformationWindow = new RuntimeMemoryInformationWindow<UnityTexture>();
        private RuntimeMemoryInformationWindow<UnityMesh> m_runtimeMemoryMeshInformationWindow = new RuntimeMemoryInformationWindow<UnityMesh>();
        private RuntimeMemoryInformationWindow<UnityMaterial> m_runtimeMemoryMaterialInformationWindow = new RuntimeMemoryInformationWindow<UnityMaterial>();
        private RuntimeMemoryInformationWindow<UnityShader> m_runtimeMemoryShaderInformationWindow = new RuntimeMemoryInformationWindow<UnityShader>();
        private RuntimeMemoryInformationWindow<UnityAnimationClip> m_runtimeMemoryAnimationClipInformationWindow = new RuntimeMemoryInformationWindow<UnityAnimationClip>();
        private RuntimeMemoryInformationWindow<UnityAudioClip> m_runtimeMemoryAudioClipInformationWindow = new RuntimeMemoryInformationWindow<UnityAudioClip>();
        private RuntimeMemoryInformationWindow<UnityFont> m_runtimeMemoryFontInformationWindow = new RuntimeMemoryInformationWindow<UnityFont>();
        private RuntimeMemoryInformationWindow<UnityTextAsset> m_runtimeMemoryTextAssetInformationWindow = new RuntimeMemoryInformationWindow<UnityTextAsset>();
        private RuntimeMemoryInformationWindow<UnityScriptableObject> m_runtimeMemoryScriptableObjectInformationWindow = new RuntimeMemoryInformationWindow<UnityScriptableObject>();
        private RuntimeTimerModuleStatInformationWindow m_runtimeTimerModuleStatInformationWindow = new RuntimeTimerModuleStatInformationWindow();
        private RuntimeNetworkModuleStatInformationWindow m_runtimeNetworkModuleStatInformationWindow = new RuntimeNetworkModuleStatInformationWindow();
        private RuntimeSceneModuleStatInformationWindow m_runtimeSceneModuleStatInformationWindow = new RuntimeSceneModuleStatInformationWindow();
        private RuntimeActorModuleStatInformationWindow m_runtimeActorModuleStatInformationWindow = new RuntimeActorModuleStatInformationWindow();
        private RuntimeViewModuleStatInformationWindow m_runtimeViewModuleStatInformationWindow = new RuntimeViewModuleStatInformationWindow();
        private SettingsWindow m_settingsWindow = new SettingsWindow();
        private OperationsWindow m_operationsWindow = new OperationsWindow();

        /// <summary>
        /// 帧率计数器对象实例
        /// </summary>
        private FpsCounter m_fpsCounter = null;

        /// <summary>
        /// 获取或设置调试器的窗口实例是否激活
        /// </summary>
        public bool ActiveWindow
        {
            get { return m_debuggerManager.ActiveWindow; }
            set
            {
                m_debuggerManager.ActiveWindow = value;
                enabled = value;
            }
        }

        /// <summary>
        /// 获取或设置是否显示完整的调试器界面
        /// </summary>
        public bool ShowFullWindow
        {
            get { return m_showFullWindow; }
            set { m_showFullWindow = value; }
        }

        /// <summary>
        /// 获取或设置调试器漂浮框大小
        /// </summary>
        public UnityRect IconRect
        {
            get { return m_iconRect; }
            set { m_iconRect = value; }
        }

        /// <summary>
        /// 获取或设置调试器窗口大小
        /// </summary>
        public UnityRect WindowRect
        {
            get { return m_windowRect; }
            set { m_windowRect = value; }
        }

        /// <summary>
        /// 获取或设置调试器窗口缩放比例
        /// </summary>
        public float WindowScale
        {
            get { return m_windowScale; }
            set { m_windowScale = value; }
        }

        /// <summary>
        /// 调试器组件的初始唤醒回调函数
        /// </summary>
        private void Awake()
        {
            m_debuggerManager = NovaEngine.AppEntry.GetManager<IDebuggerManager>();
            if (null == m_debuggerManager)
            {
                Debugger.Fatal("Debugger manager is invalid.");
                return;
            }

            // 帧率计数器对象初始化
            m_fpsCounter = new FpsCounter(0.5f);
        }

        private void Start()
        {
            RegisterDebuggerWindow("Console", m_consoleWindow);
            RegisterDebuggerWindow("Information/System", m_systemInformationWindow);
            RegisterDebuggerWindow("Information/Environment", m_environmentInformationWindow);
            RegisterDebuggerWindow("Information/Screen", m_screenInformationWindow);
            RegisterDebuggerWindow("Information/Graphics", m_graphicsInformationWindow);
            RegisterDebuggerWindow("Information/Input/Summary", m_inputSummaryInformationWindow);
            RegisterDebuggerWindow("Information/Input/Touch", m_inputTouchInformationWindow);
            RegisterDebuggerWindow("Information/Input/Location", m_inputLocationInformationWindow);
            RegisterDebuggerWindow("Information/Input/Acceleration", m_inputAccelerationInformationWindow);
            RegisterDebuggerWindow("Information/Input/Gyroscope", m_inputGyroscopeInformationWindow);
            RegisterDebuggerWindow("Information/Input/Compass", m_inputCompassInformationWindow);
            RegisterDebuggerWindow("Information/Other/Scene", m_sceneInformationWindow);
            RegisterDebuggerWindow("Information/Other/Path", m_pathInformationWindow);
            RegisterDebuggerWindow("Information/Other/Time", m_timeInformationWindow);
            RegisterDebuggerWindow("Information/Other/Quality", m_qualityInformationWindow);
            RegisterDebuggerWindow("Profiler/Summary", m_profilerInformationWindow);
            RegisterDebuggerWindow("Profiler/Memory/Summary", m_runtimeMemorySummaryWindow);
            RegisterDebuggerWindow("Profiler/Memory/All", m_runtimeMemoryAllInformationWindow);
            RegisterDebuggerWindow("Profiler/Memory/Texture", m_runtimeMemoryTextureInformationWindow);
            RegisterDebuggerWindow("Profiler/Memory/Mesh", m_runtimeMemoryMeshInformationWindow);
            RegisterDebuggerWindow("Profiler/Memory/Material", m_runtimeMemoryMaterialInformationWindow);
            RegisterDebuggerWindow("Profiler/Memory/Shader", m_runtimeMemoryShaderInformationWindow);
            RegisterDebuggerWindow("Profiler/Memory/AnimationClip", m_runtimeMemoryAnimationClipInformationWindow);
            RegisterDebuggerWindow("Profiler/Memory/AudioClip", m_runtimeMemoryAudioClipInformationWindow);
            RegisterDebuggerWindow("Profiler/Memory/Font", m_runtimeMemoryFontInformationWindow);
            RegisterDebuggerWindow("Profiler/Memory/TextAsset", m_runtimeMemoryTextAssetInformationWindow);
            RegisterDebuggerWindow("Profiler/Memory/ScriptableObject", m_runtimeMemoryScriptableObjectInformationWindow);
            RegisterDebuggerWindow("Profiler/Module/Timer", m_runtimeTimerModuleStatInformationWindow);
            RegisterDebuggerWindow("Profiler/Module/Network", m_runtimeNetworkModuleStatInformationWindow);
            RegisterDebuggerWindow("Profiler/Module/Scene", m_runtimeSceneModuleStatInformationWindow);
            RegisterDebuggerWindow("Profiler/Module/Object", m_runtimeActorModuleStatInformationWindow);
            RegisterDebuggerWindow("Profiler/Module/View", m_runtimeViewModuleStatInformationWindow);
            RegisterDebuggerWindow("Other/Settings", m_settingsWindow);
            RegisterDebuggerWindow("Other/Operations", m_operationsWindow);

            switch (m_activeWindowType)
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
            m_fpsCounter.Update(UnityTime.deltaTime, UnityTime.unscaledDeltaTime);
        }

        private void OnDestroy()
        {
            NovaEngine.AppEntry.RemoveManager<IDebuggerManager>();
        }

        private void OnGUI()
        {
            if (null == m_debuggerManager || !m_debuggerManager.ActiveWindow)
            {
                return;
            }

            UnityEngine.GUISkin cachedGuiSkin = UnityEngine.GUI.skin;
            UnityEngine.Matrix4x4 cachedMatrix = UnityEngine.GUI.matrix;

            UnityEngine.GUI.skin = m_skin;
            UnityEngine.GUI.matrix = UnityEngine.Matrix4x4.Scale(new UnityVector3(m_windowScale, m_windowScale, 1f));

            if (m_showFullWindow)
            {
                m_windowRect = UnityGUILayout.Window(0, m_windowRect, DrawWindow, "<b>GAME FRAMEWORK DEBUGGER</b>");
            }
            else
            {
                m_iconRect = UnityGUILayout.Window(0, m_iconRect, DrawDebuggerWindowIcon, "<b>DEBUGGER</b>");
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
            Debugger.Assert(null != m_debuggerManager);

            m_debuggerManager.RegisterDebuggerWindow(path, window, args);
        }

        /// <summary>
        /// 从当前组件的调试环境中注销指定名称对应的窗口对象实例
        /// </summary>
        /// <param name="path">窗口名称</param>
        /// <returns>若窗口注销成功返回true，否则返回false</returns>
        public bool UnregisterDebuggerWindow(string path)
        {
            Debugger.Assert(null != m_debuggerManager);

            return m_debuggerManager.UnregisterDebuggerWindow(path);
        }

        /// <summary>
        /// 获取指定名称对应的调试窗口对象实例
        /// </summary>
        /// <param name="path">窗口名称</param>
        /// <returns>若存在名称对应的窗口实例则返回其引用，否则返回null</returns>
        public IDebuggerWindow GetDebuggerWindow(string path)
        {
            Debugger.Assert(null != m_debuggerManager);

            return m_debuggerManager.GetDebuggerWindow(path);
        }

        /// <summary>
        /// 选中当前调试管理器中指定名称对应的调试窗口实例
        /// </summary>
        /// <param name="path">窗口名称</param>
        /// <returns>若选中窗口实例成功返回true，否则返回false</returns>
        public bool SelectedDebuggerWindow(string path)
        {
            Debugger.Assert(null != m_debuggerManager);

            return m_debuggerManager.SelectedDebuggerWindow(path);
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
            m_consoleWindow.GetRecentLogs(results);
        }

        /// <summary>
        /// 获取最近新增的日志记录，通过传入的列表实例进行返回
        /// 此处限定了获取日志记录的数量
        /// </summary>
        /// <param name="results"></param>
        /// <param name="count"></param>
        public void GetRecentLogs(List<LogNode> results, int count)
        {
            m_consoleWindow.GetRecentLogs(results, count);
        }

        private void DrawWindow(int windowId)
        {
            UnityEngine.GUI.DragWindow(m_dragRect);
            DrawDebuggerWindowGroup(m_debuggerManager.DebuggerWindowRoot);
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

            if (m_debuggerManager.DebuggerWindowRoot == debuggerWindowGroup)
            {
                names.Add("<b>Close</b>");
            }

            int toolbarIndex = UnityGUILayout.Toolbar(debuggerWindowGroup.SelectedIndex, names.ToArray(),
                    UnityGUILayout.Height(30f), UnityGUILayout.MaxWidth(UnityEngine.Screen.width));
            if (debuggerWindowGroup.DebuggerWindowCount <= toolbarIndex)
            {
                m_showFullWindow = false;
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
            UnityEngine.GUI.DragWindow(m_dragRect);
            UnityGUILayout.Space(5);
            UnityColor32 color = UnityColor.white;

            m_consoleWindow.RefreshLogCount();
            if (m_consoleWindow.FatalCount > 0)
            {
                color = m_consoleWindow.GetLogStringColor(UnityLogType.Exception);
            }
            else if (m_consoleWindow.ErrorCount > 0)
            {
                color = m_consoleWindow.GetLogStringColor(UnityLogType.Error);
            }
            else if (m_consoleWindow.WarnCount > 0)
            {
                color = m_consoleWindow.GetLogStringColor(UnityLogType.Warning);
            }
            else if (m_consoleWindow.InfoCount > 0)
            {
                color = m_consoleWindow.GetLogStringColor(UnityLogType.Log);
            }

            string title = NovaEngine.Utility.Text.Format("<color=#{0}{1}{2}{3}><b>FPS: {4}</b></color>",
                    color.r.ToString("x2"), color.g.ToString("x2"), color.b.ToString("x2"), color.a.ToString("x2"), m_fpsCounter.CurrentFps.ToString("F2"));
            if (UnityGUILayout.Button(title, UnityGUILayout.Width(100f), UnityGUILayout.Height(40f)))
            {
                m_showFullWindow = true;
            }
        }

        /// <summary>
        /// 将指定文本内容拷贝到临时剪切板中<br/>
        /// 注意此处的剪切板非系统及剪切板，而是专属于该调试器组件的自定义剪切板空间
        /// </summary>
        /// <param name="content">文本内容</param>
        private static void CopyToClipboard(string content)
        {
            s_textEditor.text = content;
            s_textEditor.OnFocus();
            s_textEditor.Copy();
            s_textEditor.text = string.Empty;
        }
    }
}
