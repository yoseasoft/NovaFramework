/// <summary>
/// 基于 NovaFramework 的演示案例
/// 
/// 创建者：Hurley
/// 创建时间：2025-06-22
/// 功能描述：
/// </summary>

namespace Game.Sample
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

        internal static void AddSampleFilter(GameSampleType type)
        {
            Debugger.Assert(string.IsNullOrEmpty(FilterModuleName));

            FilterModuleName = SampleModuleName + "." + type.ToString();

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
