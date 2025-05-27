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

using UnityGUILayout = UnityEngine.GUILayout;
using UnityQualitySettings = UnityEngine.QualitySettings;

namespace GameEngine.Debug
{
    /// <summary>
    /// 游戏调试器组件对象类，用于定义调试器对象的基础属性及访问操作函数
    /// </summary>
    public sealed partial class DebuggerComponent
    {
        /// <summary>
        /// 性能信息展示窗口的对象类
        /// </summary>
        private sealed class QualityInformationWindow : BaseScrollableDebuggerWindow
        {
            private bool m_applyExpensiveChanges = false;

            protected override void OnDrawScrollableWindow()
            {
                UnityGUILayout.Label("<b>Quality Information</b>");
                UnityGUILayout.BeginVertical("box");
                {
                    int currentQualityLevel = UnityQualitySettings.GetQualityLevel();

                    DrawItem("Current Quality Level", UnityQualitySettings.names[currentQualityLevel]);
                    m_applyExpensiveChanges = UnityGUILayout.Toggle(m_applyExpensiveChanges, "Apply expensive changes on quality level change.");

                    int newQualityLevel = UnityGUILayout.SelectionGrid(currentQualityLevel, UnityQualitySettings.names, 3, "toggle");
                    if (newQualityLevel != currentQualityLevel)
                    {
                        UnityQualitySettings.SetQualityLevel(newQualityLevel, m_applyExpensiveChanges);
                    }
                }
                UnityGUILayout.EndVertical();

                UnityGUILayout.Label("<b>Rendering Information</b>");
                UnityGUILayout.BeginVertical("box");
                {
                    DrawItem("Active Color Space", UnityQualitySettings.activeColorSpace.ToString());
                    DrawItem("Desired Color Space", UnityQualitySettings.desiredColorSpace.ToString());
                    DrawItem("Max Queued Frames", UnityQualitySettings.maxQueuedFrames.ToString());
                    DrawItem("Pixel Light Count", UnityQualitySettings.pixelLightCount.ToString());
                    DrawItem("Master Texture Limit", UnityQualitySettings.globalTextureMipmapLimit.ToString());
                    DrawItem("Anisotropic Filtering", UnityQualitySettings.anisotropicFiltering.ToString());
                    DrawItem("Anti Aliasing", UnityQualitySettings.antiAliasing.ToString());
                    DrawItem("Soft Particles", UnityQualitySettings.softParticles.ToString());
                    DrawItem("Soft Vegetation", UnityQualitySettings.softVegetation.ToString());
                    DrawItem("Realtime Reflection Probes", UnityQualitySettings.realtimeReflectionProbes.ToString());
                    DrawItem("Billboards Face Camera Position", UnityQualitySettings.billboardsFaceCameraPosition.ToString());
                    DrawItem("Resolution Scaling Fixed DPI Factor", UnityQualitySettings.resolutionScalingFixedDPIFactor.ToString());
                    DrawItem("Texture Streaming Enabled", UnityQualitySettings.streamingMipmapsActive.ToString());
                    DrawItem("Texture Streaming Add All Cameras", UnityQualitySettings.streamingMipmapsAddAllCameras.ToString());
                    DrawItem("Texture Streaming Memory Budget", UnityQualitySettings.streamingMipmapsMemoryBudget.ToString());
                    DrawItem("Texture Streaming Renderers Per Frame", UnityQualitySettings.streamingMipmapsRenderersPerFrame.ToString());
                    DrawItem("Texture Streaming Max Level Reduction", UnityQualitySettings.streamingMipmapsMaxLevelReduction.ToString());
                    DrawItem("Texture Streaming Max File IO Requests", UnityQualitySettings.streamingMipmapsMaxFileIORequests.ToString());
                }
                UnityGUILayout.EndVertical();

                UnityGUILayout.Label("<b>Shadows Information</b>");
                UnityGUILayout.BeginVertical("box");
                {
                    DrawItem("Shadowmask Mode", UnityQualitySettings.shadowmaskMode.ToString());
                    DrawItem("Shadow Quality", UnityQualitySettings.shadows.ToString());
                    DrawItem("Shadow Resolution", UnityQualitySettings.shadowResolution.ToString());
                    DrawItem("Shadow Projection", UnityQualitySettings.shadowProjection.ToString());
                    DrawItem("Shadow Distance", UnityQualitySettings.shadowDistance.ToString());
                    DrawItem("Shadow Near Plane Offset", UnityQualitySettings.shadowNearPlaneOffset.ToString());
                    DrawItem("Shadow Cascades", UnityQualitySettings.shadowCascades.ToString());
                    DrawItem("Shadow Cascade 2 Split", UnityQualitySettings.shadowCascade2Split.ToString());
                    DrawItem("Shadow Cascade 4 Split", UnityQualitySettings.shadowCascade4Split.ToString());
                }
                UnityGUILayout.EndVertical();

                UnityGUILayout.Label("<b>Other Information</b>");
                UnityGUILayout.BeginVertical("box");
                {
                    DrawItem("Skin Weights", UnityQualitySettings.skinWeights.ToString());
                    DrawItem("VSync Count", UnityQualitySettings.vSyncCount.ToString());
                    DrawItem("LOD Bias", UnityQualitySettings.lodBias.ToString());
                    DrawItem("Maximum LOD Level", UnityQualitySettings.maximumLODLevel.ToString());
                    DrawItem("Particle Raycast Budget", UnityQualitySettings.particleRaycastBudget.ToString());
                    DrawItem("Async Upload Time Slice", NovaEngine.Utility.Text.Format("{0} ms", UnityQualitySettings.asyncUploadTimeSlice.ToString()));
                    DrawItem("Async Upload Buffer Size", NovaEngine.Utility.Text.Format("{0} MB", UnityQualitySettings.asyncUploadBufferSize.ToString()));
                    DrawItem("Async Upload Persistent Buffer", UnityQualitySettings.asyncUploadPersistentBuffer.ToString());
                }
                UnityGUILayout.EndVertical();
            }
        }
    }
}
