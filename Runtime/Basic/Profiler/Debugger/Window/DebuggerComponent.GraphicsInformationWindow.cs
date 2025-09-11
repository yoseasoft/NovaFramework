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

using UnitySystemInfo = UnityEngine.SystemInfo;
using UnityShader = UnityEngine.Shader;
using UnityGraphics = UnityEngine.Graphics;
using UnityGUILayout = UnityEngine.GUILayout;

namespace GameEngine.Profiler.Debugging
{
    /// <summary>
    /// 游戏调试器组件对象类，用于定义调试器对象的基础属性及访问操作函数
    /// </summary>
    public sealed partial class DebuggerComponent
    {
        /// <summary>
        /// 图像展示窗口的对象类
        /// </summary>
        private sealed class GraphicsInformationWindow : BaseScrollableDebuggerWindow
        {
            protected override void OnDrawScrollableWindow()
            {
                UnityGUILayout.Label("<b>Graphics Information</b>");
                UnityGUILayout.BeginVertical("box");
                {
                    DrawItem("Device ID", UnitySystemInfo.graphicsDeviceID.ToString());
                    DrawItem("Device Name", UnitySystemInfo.graphicsDeviceName);
                    DrawItem("Device Vendor ID", UnitySystemInfo.graphicsDeviceVendorID.ToString());
                    DrawItem("Device Vendor", UnitySystemInfo.graphicsDeviceVendor);
                    DrawItem("Device Type", UnitySystemInfo.graphicsDeviceType.ToString());
                    DrawItem("Device Version", UnitySystemInfo.graphicsDeviceVersion);
                    DrawItem("Memory Size", NovaEngine.Utility.Text.Format("{0} MB", UnitySystemInfo.graphicsMemorySize.ToString()));
                    DrawItem("Multi Threaded", UnitySystemInfo.graphicsMultiThreaded.ToString());
                    DrawItem("Rendering Threading Mode", UnitySystemInfo.renderingThreadingMode.ToString());
                    DrawItem("HRD Display Support Flags", UnitySystemInfo.hdrDisplaySupportFlags.ToString());
                    DrawItem("Shader Level", GetShaderLevelString(UnitySystemInfo.graphicsShaderLevel));
                    DrawItem("Global Maximum LOD", UnityShader.globalMaximumLOD.ToString());
                    DrawItem("Global Render Pipeline", UnityShader.globalRenderPipeline);
                    DrawItem("Min OpenGLES Version", UnityGraphics.minOpenGLESVersion.ToString());
                    DrawItem("Active Tier", UnityGraphics.activeTier.ToString());
                    DrawItem("Active Color Gamut", UnityGraphics.activeColorGamut.ToString());
                    DrawItem("Preserve Frame Buffer Alpha", UnityGraphics.preserveFramebufferAlpha.ToString());
                    DrawItem("NPOT Support", UnitySystemInfo.npotSupport.ToString());
                    DrawItem("Max Texture Size", UnitySystemInfo.maxTextureSize.ToString());
                    DrawItem("Supported Render Target Count", UnitySystemInfo.supportedRenderTargetCount.ToString());
                    DrawItem("Supported Random Write Target Count", UnitySystemInfo.supportedRandomWriteTargetCount.ToString());
                    DrawItem("Copy Texture Support", UnitySystemInfo.copyTextureSupport.ToString());
                    DrawItem("Uses Reversed ZBuffer", UnitySystemInfo.usesReversedZBuffer.ToString());
                    DrawItem("Max Cubemap Size", UnitySystemInfo.maxCubemapSize.ToString());
                    DrawItem("Graphics UV Starts At Top", UnitySystemInfo.graphicsUVStartsAtTop.ToString());
                    DrawItem("Constant Buffer Offset Alignment", UnitySystemInfo.constantBufferOffsetAlignment.ToString());
                    DrawItem("Has Hidden Surface Removal On GPU", UnitySystemInfo.hasHiddenSurfaceRemovalOnGPU.ToString());
                    DrawItem("Has Dynamic Uniform Array Indexing In Fragment Shaders", UnitySystemInfo.hasDynamicUniformArrayIndexingInFragmentShaders.ToString());
                    DrawItem("Has Mip Max Level", UnitySystemInfo.hasMipMaxLevel.ToString());
                    DrawItem("Uses Load Store Actions", UnitySystemInfo.usesLoadStoreActions.ToString());
                    DrawItem("Max Compute Buffer Inputs Compute", UnitySystemInfo.maxComputeBufferInputsCompute.ToString());
                    DrawItem("Max Compute Buffer Inputs Domain", UnitySystemInfo.maxComputeBufferInputsDomain.ToString());
                    DrawItem("Max Compute Buffer Inputs Fragment", UnitySystemInfo.maxComputeBufferInputsFragment.ToString());
                    DrawItem("Max Compute Buffer Inputs Geometry", UnitySystemInfo.maxComputeBufferInputsGeometry.ToString());
                    DrawItem("Max Compute Buffer Inputs Hull", UnitySystemInfo.maxComputeBufferInputsHull.ToString());
                    DrawItem("Max Compute Buffer Inputs Vertex", UnitySystemInfo.maxComputeBufferInputsVertex.ToString());
                    DrawItem("Max Compute Work Group Size", UnitySystemInfo.maxComputeWorkGroupSize.ToString());
                    DrawItem("Max Compute Work Group Size X", UnitySystemInfo.maxComputeWorkGroupSizeX.ToString());
                    DrawItem("Max Compute Work Group Size Y", UnitySystemInfo.maxComputeWorkGroupSizeY.ToString());
                    DrawItem("Max Compute Work Group Size Z", UnitySystemInfo.maxComputeWorkGroupSizeZ.ToString());
                    DrawItem("Supports Sparse Textures", UnitySystemInfo.supportsSparseTextures.ToString());
                    DrawItem("Supports 3D Textures", UnitySystemInfo.supports3DTextures.ToString());
                    DrawItem("Supports Shadows", UnitySystemInfo.supportsShadows.ToString());
                    DrawItem("Supports Raw Shadow Depth Sampling", UnitySystemInfo.supportsRawShadowDepthSampling.ToString());
                    DrawItem("Supports Compute Shader", UnitySystemInfo.supportsComputeShaders.ToString());
                    DrawItem("Supports Instancing", UnitySystemInfo.supportsInstancing.ToString());
                    DrawItem("Supports 2D Array Textures", UnitySystemInfo.supports2DArrayTextures.ToString());
                    DrawItem("Supports Motion Vectors", UnitySystemInfo.supportsMotionVectors.ToString());
                    DrawItem("Supports Cubemap Array Textures", UnitySystemInfo.supportsCubemapArrayTextures.ToString());
                    DrawItem("Supports 3D Render Textures", UnitySystemInfo.supports3DRenderTextures.ToString());
                    DrawItem("Supports Texture Wrap Mirror Once", UnitySystemInfo.supportsTextureWrapMirrorOnce.ToString());
                    DrawItem("Supports Graphics Fence", UnitySystemInfo.supportsGraphicsFence.ToString());
                    DrawItem("Supports Async Compute", UnitySystemInfo.supportsAsyncCompute.ToString());
                    DrawItem("Supports Multi-sampled Textures", UnitySystemInfo.supportsMultisampledTextures.ToString());
                    DrawItem("Supports Async GPU Readback", UnitySystemInfo.supportsAsyncGPUReadback.ToString());
                    DrawItem("Supports 32bits Index Buffer", UnitySystemInfo.supports32bitsIndexBuffer.ToString());
                    DrawItem("Supports Hardware Quad Topology", UnitySystemInfo.supportsHardwareQuadTopology.ToString());
                    DrawItem("Supports Mip Streaming", UnitySystemInfo.supportsMipStreaming.ToString());
                    DrawItem("Supports Multi-sample Auto Resolve", UnitySystemInfo.supportsMultisampleAutoResolve.ToString());
                    DrawItem("Supports Separated Render Targets Blend", UnitySystemInfo.supportsSeparatedRenderTargetsBlend.ToString());
                    DrawItem("Supports Set Constant Buffer", UnitySystemInfo.supportsSetConstantBuffer.ToString());
                    DrawItem("Supports Geometry Shaders", UnitySystemInfo.supportsGeometryShaders.ToString());
                    DrawItem("Supports Ray Tracing", UnitySystemInfo.supportsRayTracing.ToString());
                    DrawItem("Supports Tessellation Shaders", UnitySystemInfo.supportsTessellationShaders.ToString());
                    DrawItem("Supports Compressed 3D Textures", UnitySystemInfo.supportsCompressed3DTextures.ToString());
                    DrawItem("Supports Conservative Raster", UnitySystemInfo.supportsConservativeRaster.ToString());
                    DrawItem("Supports GPU Recorder", UnitySystemInfo.supportsGpuRecorder.ToString());
                    DrawItem("Supports Multi-sampled 2D Array Textures", UnitySystemInfo.supportsMultisampled2DArrayTextures.ToString());
                    DrawItem("Supports Multiview", UnitySystemInfo.supportsMultiview.ToString());
                    DrawItem("Supports Render Target Array Index From Vertex Shader", UnitySystemInfo.supportsRenderTargetArrayIndexFromVertexShader.ToString());
                }
                UnityGUILayout.EndVertical();
            }

            /// <summary>
            /// 获取shader层级对应的字符串信息
            /// </summary>
            /// <param name="shaderLevel">shader层级</param>
            /// <returns>返回转换后的字符串信息</returns>
            private string GetShaderLevelString(int shaderLevel)
            {
                return NovaEngine.Utility.Text.Format("Shader Model {0}.{1}", (shaderLevel / 10).ToString(), (shaderLevel % 10).ToString());
            }
        }
    }
}
