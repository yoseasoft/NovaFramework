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

namespace GameEngine.Sample
{
    /// <summary>
    /// 演示案例过滤接口定义
    /// </summary>
    internal static class SampleFiltingProcessor
    {
        /// <summary>
        /// 是否忽略Game模块下的所有内容，演示案例目录下的不计入忽略清单
        /// </summary>
        private readonly static bool IgnoreExternalGameModuleEnabled = true;

        private readonly static string GameModuleName = typeof(SampleFiltingProcessor).Namespace.Substring(0, typeof(SampleFiltingProcessor).Namespace.IndexOf('.'));
        private readonly static string SampleModuleName = typeof(SampleFiltingProcessor).Namespace;
        private static string FilterModuleName = null;

        internal static void AddSampleFilter(string type)
        {
            Debugger.Assert(string.IsNullOrEmpty(FilterModuleName));

            FilterModuleName = $"{SampleModuleName}.{type}";

            GameEngine.Loader.CodeLoader.AddAssemblyLoadFiltingProcessorCallback(AssemblyLoadFiltingProcessor);
        }

        internal static void RemoveSampleFilter()
        {
            GameEngine.Loader.CodeLoader.RemoveAssemblyLoadFiltingProcessorCallback(AssemblyLoadFiltingProcessor);

            FilterModuleName = null;
        }

        internal static string GetFilterModuleName()
        {
            return FilterModuleName;
        }

        /// <summary>
        /// 参考<see cref="GameEngine.Loader.CodeLoader"/>类中定义的过滤处理回调接口<see cref="GameEngine.Loader.CodeLoader.AssemblyLoadFiltingProcessor"/>格式<br/>
        /// 需要注意的是，该过滤器仅过滤Samples目录下的代码，其它接入代码不在该过滤器考虑范围内
        /// </summary>
        /// <param name="assemblyName">程序集名称</param>
        /// <param name="classType">当前解析类</param>
        /// <returns>若目标类需要加载则返回true，否则返回false</returns>
        public static bool AssemblyLoadFiltingProcessor(string assemblyName, System.Type classType)
        {
            string ns = classType.Namespace;

            if (IgnoreExternalGameModuleEnabled)
            {
                if (null == ns)
                {
                    return false;
                }
                if (ns.StartsWith(GameModuleName) && false == ns.StartsWith(SampleModuleName))
                {
                    return false;
                }
            }

            // 排除掉 Game.Sample 中的类
            // if (ns.StartsWith(SampleModuleName) && (string.Equals(ns, SampleModuleName, System.StringComparison.Ordinal) || false == string.Equals(ns, FilterModuleName, System.StringComparison.Ordinal))) { }
            if (ns.StartsWith(SampleModuleName) && false == string.Equals(ns, FilterModuleName, System.StringComparison.Ordinal))
            {
                return false;
            }

            return true;
        }
    }
}
