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

using UnityObject = UnityEngine.Object;
using UnityGUILayout = UnityEngine.GUILayout;
using UnityProfiler = UnityEngine.Profiling.Profiler;

namespace GameEngine.Debug
{
    /// <summary>
    /// 游戏调试器组件对象类，用于定义调试器对象的基础属性及访问操作函数
    /// </summary>
    public sealed partial class DebuggerComponent
    {
        /// <summary>
        /// 运行时特定元素内存信息展示窗口的对象类
        /// </summary>
        private sealed partial class RuntimeMemoryInformationWindow<T>
        {
            /// <summary>
            /// 用于对资源实例进行封装的实例对象类，提供对单个资源实例的基础属性的信息封装
            /// </summary>
            private sealed class Sample
            {
                /// <summary>
                /// 实例的名称
                /// </summary>
                private readonly string m_name;
                /// <summary>
                /// 实例的类型
                /// </summary>
                private readonly string m_type;
                /// <summary>
                /// 实例的内存大小
                /// </summary>
                private readonly long m_size;
                /// <summary>
                /// 实例重复使用的状态标识
                /// </summary>
                private bool m_highlight;

                /// <summary>
                /// 获取实例的名称
                /// </summary>
                public string Name
                {
                    get { return m_name; }
                }

                /// <summary>
                /// 获取实例的类型
                /// </summary>
                public string Type
                {
                    get { return m_type; }
                }

                /// <summary>
                /// 获取实例的内存大小
                /// </summary>
                public long Size
                {
                    get { return m_size; }
                }

                /// <summary>
                /// 获取或设置实例的重复使用状态
                /// </summary>
                public bool Highlight
                {
                    get { return m_highlight; }
                    set { m_highlight = value; }
                }

                public Sample(string name, string type, long size)
                {
                    m_name = name;
                    m_type = type;
                    m_size = size;
                    m_highlight = false;
                }
            }
        }
    }
}
