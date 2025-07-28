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
