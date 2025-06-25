/// <summary>
/// 基于 NovaFramework 的演示案例
/// 
/// 创建者：Hurley
/// 创建时间：2025-06-23
/// 功能描述：
/// </summary>

using System.Collections.Generic;

namespace Game.Sample
{
    /// <summary>
    /// 演示案例总控
    /// </summary>
    public static partial class GameSample
    {
        private static IList<string> WaitingLoadAssemblyNames = null;

        /// <summary>
        /// 注册待加载的程序集名称
        /// </summary>
        /// <param name="assemblyNames">程序集名称</param>
        private static void RegAssemblyNames(params string[] assemblyNames)
        {
            if (null == WaitingLoadAssemblyNames)
            {
                WaitingLoadAssemblyNames = new List<string>();
            }

            WaitingLoadAssemblyNames.Clear();

            for (int n = 0; n < assemblyNames.Length; ++n)
            {
                WaitingLoadAssemblyNames.Add(assemblyNames[n]);
            }
        }

        /// <summary>
        /// 加载所有程序集
        /// </summary>
        private static void LoadAllAssemblies(bool reload = false)
        {
            /**
             * System.Reflection.Assembly[] assemblies = System.AppDomain.CurrentDomain.GetAssemblies();
             * foreach (System.Reflection.Assembly assembly in assemblies)
             * {
             *     GameEngine.CodeLoader.LoadFromAssembly(assembly);
             * }
             */

            for (int n = 0; null != WaitingLoadAssemblyNames && n < WaitingLoadAssemblyNames.Count; ++n)
            {
                // System.Reflection.Assembly assembly = System.Reflection.Assembly.Load(WaitingLoadAssemblyNames[n]);
                System.Reflection.Assembly assembly = NovaEngine.Utility.Assembly.GetAssembly(WaitingLoadAssemblyNames[n]);
                if (null == assembly)
                {
                    Debugger.Error("通过指定名称‘{%s}’获取当前上下文中已加载的程序集实例失败！", WaitingLoadAssemblyNames[n]);
                    continue;
                }

                GameEngine.GameLibrary.LoadFromAssembly(assembly, reload);
            }
        }
    }
}
