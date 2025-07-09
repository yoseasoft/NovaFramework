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
using UnitySceneManager = UnityEngine.SceneManagement.SceneManager;
using UnityScene = UnityEngine.SceneManagement.Scene;

namespace GameEngine.Debug
{
    /// <summary>
    /// 游戏调试器组件对象类，用于定义调试器对象的基础属性及访问操作函数
    /// </summary>
    public sealed partial class DebuggerComponent
    {
        /// <summary>
        /// 场景信息展示窗口的对象类
        /// </summary>
        private sealed class SceneInformationWindow : BaseScrollableDebuggerWindow
        {
            protected override void OnDrawScrollableWindow()
            {
                UnityGUILayout.Label("<b>Scene Information</b>");
                UnityGUILayout.BeginVertical("box");
                {
                    DrawItem("Scene Count", UnitySceneManager.sceneCount.ToString());
                    DrawItem("Scene Count In Build Settings", UnitySceneManager.sceneCountInBuildSettings.ToString());

                    UnityScene activeScene = UnitySceneManager.GetActiveScene();
                    DrawItem("Active Scene Handle", activeScene.handle.ToString());
                    DrawItem("Active Scene Name", activeScene.name);
                    DrawItem("Active Scene Path", activeScene.path);
                    DrawItem("Active Scene Build Index", activeScene.buildIndex.ToString());
                    DrawItem("Active Scene Is Dirty", activeScene.isDirty.ToString());
                    DrawItem("Active Scene Is Loaded", activeScene.isLoaded.ToString());
                    DrawItem("Active Scene Is Valid", activeScene.IsValid().ToString());
                    DrawItem("Active Scene Root Count", activeScene.rootCount.ToString());
                    DrawItem("Active Scene Is Sub Scene", activeScene.isSubScene.ToString());
                }
                UnityGUILayout.EndVertical();
            }
        }
    }
}
