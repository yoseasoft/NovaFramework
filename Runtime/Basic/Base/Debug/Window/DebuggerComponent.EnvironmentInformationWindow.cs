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

using UnityGUILayout = UnityEngine.GUILayout;
using UnityApplication = UnityEngine.Application;

namespace GameEngine.Debug
{
    /// <summary>
    /// 游戏调试器组件对象类，用于定义调试器对象的基础属性及访问操作函数
    /// </summary>
    public sealed partial class DebuggerComponent
    {
        /// <summary>
        /// 环境信息展示窗口的对象类
        /// </summary>
        private sealed class EnvironmentInformationWindow : BaseScrollableDebuggerWindow
        {
            public override void Initialize(params object[] args)
            {
                base.Initialize(args);
            }

            protected override void OnDrawScrollableWindow()
            {
                UnityGUILayout.Label("<b>Environment Information</b>");
                UnityGUILayout.BeginVertical("box");
                {
                    DrawItem("Product Name", UnityApplication.productName);
                    DrawItem("Company Name", UnityApplication.companyName);
                    DrawItem("Game Identifier", UnityApplication.identifier);
                    // DrawItem("Game Framework Version", Version.GameFrameworkVersion);
                    // DrawItem("Game Version", NovaEngine.Utility.Text.Format("{0} ({1})", Version.GameVersion, Version.InternalGameVersion.ToString()));
                    // DrawItem("Resource Version", _BaseComponent.EditorResourceMode ? "Unavailable in editor resource mode" : (string.IsNullOrEmpty(_ResourceComponent.ApplicableGameVersion) ? "Unknown" : Utility.Text.Format("{0} ({1})", _ResourceComponent.ApplicableGameVersion, _ResourceComponent.InternalResourceVersion.ToString())));
                    DrawItem("Application Version", UnityApplication.version);
                    DrawItem("Unity Version", UnityApplication.unityVersion);
                    DrawItem("Platform", UnityApplication.platform.ToString());
                    DrawItem("System Language", UnityApplication.systemLanguage.ToString());
                    DrawItem("Cloud Project Id", UnityApplication.cloudProjectId);
                    DrawItem("Build Guid", UnityApplication.buildGUID);
                    DrawItem("Target Frame Rate", UnityApplication.targetFrameRate.ToString());
                    DrawItem("Internet Reachability", UnityApplication.internetReachability.ToString());
                    DrawItem("Background Loading Priority", UnityApplication.backgroundLoadingPriority.ToString());
                    DrawItem("Is Playing", UnityApplication.isPlaying.ToString());
                    DrawItem("Splash Screen Is Finished", UnityEngine.Rendering.SplashScreen.isFinished.ToString());
                    DrawItem("Run In Background", UnityApplication.runInBackground.ToString());
                    DrawItem("Install Name", UnityApplication.installerName);
                    DrawItem("Install Mode", UnityApplication.installMode.ToString());
                    DrawItem("Sandbox Type", UnityApplication.sandboxType.ToString());
                    DrawItem("Is Mobile Platform", UnityApplication.isMobilePlatform.ToString());
                    DrawItem("Is Console Platform", UnityApplication.isConsolePlatform.ToString());
                    DrawItem("Is Editor", UnityApplication.isEditor.ToString());
                    DrawItem("Is Debug Build", UnityEngine.Debug.isDebugBuild.ToString());
                    DrawItem("Is Focused", UnityApplication.isFocused.ToString());
                    DrawItem("Is Batch Mode", UnityApplication.isBatchMode.ToString());
                }
                UnityGUILayout.EndVertical();
            }
        }
    }
}
