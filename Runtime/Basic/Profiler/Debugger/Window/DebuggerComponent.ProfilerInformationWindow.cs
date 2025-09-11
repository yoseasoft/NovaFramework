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
using UnityProfiler = UnityEngine.Profiling.Profiler;

namespace GameEngine.Profiler.Debugging
{
    /// <summary>
    /// 游戏调试器组件对象类，用于定义调试器对象的基础属性及访问操作函数
    /// </summary>
    public sealed partial class DebuggerComponent
    {
        /// <summary>
        /// 分析工具信息展示窗口的对象类
        /// </summary>
        private sealed class ProfilerInformationWindow : BaseScrollableDebuggerWindow
        {
            protected override void OnDrawScrollableWindow()
            {
                UnityGUILayout.Label("<b>Profiler Information</b>");
                UnityGUILayout.BeginVertical("box");
                {
                    DrawItem("Supported", UnityProfiler.supported.ToString());
                    DrawItem("Enabled", UnityProfiler.enabled.ToString());
                    DrawItem("Enable Binary Log", UnityProfiler.enableBinaryLog ? NovaEngine.Utility.Text.Format("True, {0}", UnityProfiler.logFile) : "False");
                    DrawItem("Enable Allocation Callstacks", UnityProfiler.enableAllocationCallstacks.ToString());
                    DrawItem("Area Count", UnityProfiler.areaCount.ToString());
                    DrawItem("Max Used Memory", GetByteLengthString(UnityProfiler.maxUsedMemory));
                    DrawItem("Mono Used Size", GetByteLengthString(UnityProfiler.GetMonoUsedSizeLong()));
                    DrawItem("Mono Heap Size", GetByteLengthString(UnityProfiler.GetMonoHeapSizeLong()));
                    DrawItem("Used Heap Size", GetByteLengthString(UnityProfiler.usedHeapSizeLong));
                    DrawItem("Total Allocated Memory", GetByteLengthString(UnityProfiler.GetTotalAllocatedMemoryLong()));
                    DrawItem("Total Reserved Memory", GetByteLengthString(UnityProfiler.GetTotalReservedMemoryLong()));
                    DrawItem("Total Unused Reserved Memory", GetByteLengthString(UnityProfiler.GetTotalUnusedReservedMemoryLong()));
                    DrawItem("Allocated Memory For Graphics Driver", GetByteLengthString(UnityProfiler.GetAllocatedMemoryForGraphicsDriver()));
                    DrawItem("Temp Allocator Size", GetByteLengthString(UnityProfiler.GetTempAllocatorSize()));
                    DrawItem("Marshal Cached HGlobal Size", GetByteLengthString(NovaEngine.Utility.Marshal.CachedHGlobalSize));
                }
                UnityGUILayout.EndVertical();
            }
        }
    }
}
