/// -------------------------------------------------------------------------------
/// NovaEngine Framework Samples
///
/// Copyright (C) 2024 - 2025, Hurley, Independent Studio.
/// Copyright (C) 2025, Hainan Yuanyou Information Tecdhnology Co., Ltd. Guangzhou Branch
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

namespace Game.Sample.PerformanceAnalysis
{
    /// <summary>
    /// 通过系统原生object实现的属性组件类
    /// </summary>
    public class NativeAttributeComponent : object
    {
        public int level;

        public static int attribute_lifecycle_count;

        public void OnInitialize()
        {
            attribute_lifecycle_count++;
        }

        public void OnStartup()
        {
            attribute_lifecycle_count++;
        }

        public void OnAwake()
        {
            attribute_lifecycle_count++;
        }

        public void OnStart()
        {
            attribute_lifecycle_count++;
        }

        public void OnDestroy()
        {
            attribute_lifecycle_count++;
        }

        public void OnShutdown()
        {
            attribute_lifecycle_count++;
        }

        public void OnCleanup()
        {
            attribute_lifecycle_count++;
        }

        public void OnUpdate()
        {
            level++;
        }

        public void OnLateUpdate()
        {
            level++;
        }

        public void Reset()
        {
            level = 0;

            attribute_lifecycle_count = 0;
        }
    }
}
