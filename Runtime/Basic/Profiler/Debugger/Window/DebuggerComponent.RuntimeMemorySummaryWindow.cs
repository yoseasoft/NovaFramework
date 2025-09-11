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
        /// 运行时内存概览信息展示窗口的对象类
        /// </summary>
        private sealed partial class RuntimeMemorySummaryWindow : BaseScrollableDebuggerWindow
        {
            /// <summary>
            /// 记录对象的存储列表容器
            /// </summary>
            private readonly List<Record> _records = new List<Record>();

            /// <summary>
            /// 记录对象的比较器函数引用
            /// </summary>
            private readonly System.Comparison<Record> _recordComparer = RecordComparer;

            /// <summary>
            /// 记录刷新的时间标签
            /// </summary>
            private SystemDateTime _sampleTime = SystemDateTime.MinValue;
            /// <summary>
            /// 所有记录的累计总数
            /// </summary>
            private int _sampleCount = 0;
            /// <summary>
            /// 所有记录的累计内存大小
            /// </summary>
            private long _sampleSize = 0L;

            protected override void OnDrawScrollableWindow()
            {
                UnityGUILayout.Label("<b>Runtime Memory Summary</b>");
                UnityGUILayout.BeginVertical("box");
                {
                    if (UnityGUILayout.Button("Take Sample", UnityGUILayout.Height(30f)))
                    {
                        TakeSample();
                    }

                    if (_sampleTime <= SystemDateTime.MinValue)
                    {
                        UnityGUILayout.Label("<b>Please take sample at first.</b>");
                    }
                    else
                    {
                        UnityGUILayout.Label(NovaEngine.Utility.Text.Format("<b>{0} Objects ({1}) obtained at {2}.</b>",
                                                                            _sampleCount.ToString(),
                                                                            GetByteLengthString(_sampleSize),
                                                                            _sampleTime.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss")));

                        UnityGUILayout.BeginHorizontal();
                        {
                            UnityGUILayout.Label("<b>Type</b>");
                            UnityGUILayout.Label("<b>Count</b>", UnityGUILayout.Width(120f));
                            UnityGUILayout.Label("<b>Size</b>", UnityGUILayout.Width(120f));
                        }
                        UnityGUILayout.EndHorizontal();

                        for (int n = 0; n < _records.Count; ++n)
                        {
                            UnityGUILayout.BeginHorizontal();
                            {
                                UnityGUILayout.Label(_records[n].Name);
                                UnityGUILayout.Label(_records[n].Count.ToString(), UnityGUILayout.Width(120f));
                                UnityGUILayout.Label(GetByteLengthString(_records[n].Size), UnityGUILayout.Width(120f));
                            }
                            UnityGUILayout.EndHorizontal();
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
                _records.Clear();
                _sampleTime = SystemDateTime.UtcNow;
                _sampleCount = 0;
                _sampleSize = 0L;

                UnityObject[] samples = UnityResources.FindObjectsOfTypeAll<UnityObject>();
                for (int n = 0; n < samples.Length; ++n)
                {
                    long sampleSize = UnityProfiler.GetRuntimeMemorySizeLong(samples[n]);

                    string name = samples[n].GetType().Name;
                    _sampleCount++;
                    _sampleSize += sampleSize;

                    Record record = null;
                    foreach (Record r in _records)
                    {
                        // 已存在同名资源的记录
                        if (r.Name == name)
                        {
                            record = r;
                            break;
                        }
                    }

                    if (null == record)
                    {
                        record = new Record(name);
                        _records.Add(record);
                    }

                    record.Count++;
                    record.Size += sampleSize;
                }

                _records.Sort(_recordComparer);
            }

            /// <summary>
            /// 记录对象的比较器实现函数，用于对两个记录对象实例进行比较排序
            /// </summary>
            /// <param name="arg0">记录对象1</param>
            /// <param name="arg1">记录对象2</param>
            /// <returns>比较两个记录对象实例并返回比较结果</returns>
            private static int RecordComparer(Record arg0, Record arg1)
            {
                int result = arg1.Size.CompareTo(arg0.Size);
                if (result != 0)
                {
                    return result;
                }

                result = arg0.Count.CompareTo(arg1.Count);
                if (result != 0)
                {
                    return result;
                }

                return arg0.Name.CompareTo(arg1.Name);
            }
        }
    }
}
