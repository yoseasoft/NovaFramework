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
using SystemDateTime = System.DateTime;

using UnityObject = UnityEngine.Object;
using UnityResources = UnityEngine.Resources;
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
        /// 运行时特定元素内存信息展示窗口的对象类
        /// </summary>
        private sealed partial class RuntimeMemoryInformationWindow<T> : BaseScrollableDebuggerWindow where T : UnityObject
        {
            /// <summary>
            /// 可显示的最大实例数
            /// </summary>
            private const int ShowSampleCount = 300;

            /// <summary>
            /// 实例对象的存储列表容器
            /// </summary>
            private readonly List<Sample> _samples = new List<Sample>();

            /// <summary>
            /// 实例对象的比较器函数引用
            /// </summary>
            private readonly System.Comparison<Sample> _sampleComparer = SampleComparer;

            /// <summary>
            /// 实例刷新的时间标签
            /// </summary>
            private SystemDateTime _sampleTime = SystemDateTime.MinValue;
            /// <summary>
            /// 所有实例的累计内存大小
            /// </summary>
            private long _sampleSize = 0L;
            /// <summary>
            /// 所有复用实例的累计内存大小
            /// </summary>
            private long _duplicateSampleSize = 0L;
            /// <summary>
            /// 所有复用实例的累计总数
            /// </summary>
            private int _duplicateSampleCount = 0;

            protected override void OnDrawScrollableWindow()
            {
                string typeName = typeof(T).Name;
                UnityGUILayout.Label(NovaEngine.Utility.Text.Format("<b>{0} Runtime Memory Information</b>", typeName));
                UnityGUILayout.BeginVertical("box");
                {
                    if (UnityGUILayout.Button(NovaEngine.Utility.Text.Format("Take Sample for {0}", typeName), UnityGUILayout.Height(30f)))
                    {
                        TakeSample();
                    }

                    if (_sampleTime <= SystemDateTime.MinValue)
                    {
                        UnityGUILayout.Label(NovaEngine.Utility.Text.Format("<b>Please take sample for {0} first.</b>", typeName));
                    }
                    else
                    {
                        if (_duplicateSampleCount > 0)
                        {
                            UnityGUILayout.Label(NovaEngine.Utility.Text.Format("<b>{0} {1}s ({2}) obtained at {3}, while {4} {1}s ({5}) might be duplicated.</b>",
                                                                                _samples.Count.ToString(), typeName,
                                                                                GetByteLengthString(_sampleSize),
                                                                                _sampleTime.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss"),
                                                                                _duplicateSampleCount.ToString(),
                                                                                GetByteLengthString(_duplicateSampleSize)));
                        }
                        else
                        {
                            UnityGUILayout.Label(NovaEngine.Utility.Text.Format("<b>{0} {1}s ({2}) obtained at {3}.</b>",
                                                                                _samples.Count.ToString(), typeName,
                                                                                GetByteLengthString(_sampleSize),
                                                                                _sampleTime.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss")));
                        }

                        if (_samples.Count > 0)
                        {
                            UnityGUILayout.BeginHorizontal();
                            {
                                UnityGUILayout.Label(NovaEngine.Utility.Text.Format("<b>{0} Name</b>", typeName));
                                UnityGUILayout.Label("<b>Type</b>", UnityGUILayout.Width(240f));
                                UnityGUILayout.Label("<b>Size</b>", UnityGUILayout.Width(80f));
                            }
                            UnityGUILayout.EndHorizontal();
                        }

                        int count = 0;
                        for (int n = 0; n < _samples.Count; ++n)
                        {
                            UnityGUILayout.BeginHorizontal();
                            {
                                UnityGUILayout.Label(_samples[n].Highlight ?
                                                            NovaEngine.Utility.Text.Format("<color=yellow>{0}</color>", _samples[n].Name) :
                                                            _samples[n].Name);
                                UnityGUILayout.Label(_samples[n].Highlight ?
                                                            NovaEngine.Utility.Text.Format("<color=yellow>{0}</color>", _samples[n].Type) :
                                                            _samples[n].Type,
                                                     UnityGUILayout.Width(240f));
                                UnityGUILayout.Label(_samples[n].Highlight ?
                                                            NovaEngine.Utility.Text.Format("<color=yellow>{0}</color>", GetByteLengthString(_samples[n].Size)) :
                                                            GetByteLengthString(_samples[n].Size),
                                                     UnityGUILayout.Width(80f));
                            }
                            UnityGUILayout.EndHorizontal();

                            count++;
                            if (count >= ShowSampleCount)
                            {
                                break;
                            }
                        }
                    }
                }
                UnityGUILayout.EndVertical();
            }

            /// <summary>
            /// 获取所有资源对象的统计信息
            /// </summary>
            private void TakeSample()
            {
                _sampleTime = SystemDateTime.UtcNow;
                _sampleSize = 0L;
                _duplicateSampleSize = 0L;
                _duplicateSampleCount = 0;
                _samples.Clear();

                T[] samples = UnityResources.FindObjectsOfTypeAll<T>();
                for (int n = 0; n < samples.Length; ++n)
                {
                    long sampleSize = UnityProfiler.GetRuntimeMemorySizeLong(samples[n]);

                    _sampleSize += sampleSize;
                    _samples.Add(new Sample(samples[n].name, samples[n].GetType().Name, sampleSize));
                }

                _samples.Sort(_sampleComparer);

                // 检测实例重复使用状态
                for (int n = 0; n < _samples.Count; ++n)
                {
                    if (_samples[n].Name == _samples[n - 1].Name &&
                        _samples[n].Type == _samples[n - 1].Type &&
                        _samples[n].Size == _samples[n - 1].Size)
                    {
                        _samples[n].Highlight = true;
                        _duplicateSampleSize += _samples[n].Size;
                        _duplicateSampleCount++;
                    }
                }
            }

            /// <summary>
            /// 实例对象的比较器实现函数，用于对两个实例对象进行比较排序
            /// </summary>
            /// <param name="arg0">实例对象1</param>
            /// <param name="arg1">实例对象2</param>
            /// <returns>比较两个实例对象并返回比较结果</returns>
            private static int SampleComparer(Sample arg0, Sample arg1)
            {
                int result = arg1.Size.CompareTo(arg0.Size);
                if (result != 0)
                {
                    return result;
                }

                result = arg0.Type.CompareTo(arg1.Type);
                if (result != 0)
                {
                    return result;
                }

                return arg0.Name.CompareTo(arg1.Name);
            }
        }
    }
}
