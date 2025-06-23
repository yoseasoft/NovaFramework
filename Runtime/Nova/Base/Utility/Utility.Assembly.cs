/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
/// Copyright (C) 2022 - 2023, Shanghai Bilibili Technology Co., Ltd.
/// Copyright (C) 2023 - 2024, Guangzhou Shiyue Network Technology Co., Ltd.
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

using SystemType = System.Type;
using SystemStringComparer = System.StringComparer;
using SystemAppDomain = System.AppDomain;
using SystemAssembly = System.Reflection.Assembly;

namespace NovaEngine
{
    /// <summary>
    /// 实用函数集合工具类
    /// </summary>
    public static partial class Utility
    {
        /// <summary>
        /// 程序集相关实用函数集合
        /// </summary>
        public static class Assembly
        {
            // private static readonly SystemAssembly[] s_assemblies = null;
            private static readonly IDictionary<string, SystemAssembly> s_cachedAssemblies = null;
            private static readonly IDictionary<string, SystemType> s_cachedTypes = new Dictionary<string, SystemType>(SystemStringComparer.Ordinal);

            static Assembly()
            {
                // s_assemblies = SystemAppDomain.CurrentDomain.GetAssemblies();
                s_cachedAssemblies = new Dictionary<string, SystemAssembly>();
                s_cachedTypes = new Dictionary<string, SystemType>(SystemStringComparer.Ordinal);
            }

            /// <summary>
            /// 将指定的程序集注册到当前域程序集缓存容器中
            /// </summary>
            /// <param name="assembly">程序集实例</param>
            public static void RegisterCurrentDomainAssembly(SystemAssembly assembly)
            {
                Dictionary<string, SystemAssembly> dictionary = new Dictionary<string, SystemAssembly>();
                dictionary.Add(assembly.GetName().Name, assembly);

                RegisterCurrentDomainAssemblies(dictionary);
            }

            /// <summary>
            /// 将指定的多个程序集全部注册到当前域程序集缓存容器中<br/>
            /// 同时提供了一个重载标识，若该标识打开，则将清除掉当前已注册的所有程序集关联，重新开始记录新的程序集引用
            /// </summary>
            /// <param name="assemblies">目标程序集</param>
            public static void RegisterCurrentDomainAssemblies(IReadOnlyDictionary<string, SystemAssembly> assemblies)
            {
                foreach (KeyValuePair<string, SystemAssembly> kvp in assemblies)
                {
                    // 在非重载模式下，重复加载将移除旧包
                    if (s_cachedAssemblies.ContainsKey(kvp.Key))
                    {
                        Logger.Warn("当前域下的程序集缓存列表中已存在相同名称‘{%s}’的程序包，重复加载将覆盖旧的引用记录！", kvp.Key);
                        s_cachedAssemblies.Remove(kvp.Key);
                    }

                    s_cachedAssemblies.Add(kvp.Key, kvp.Value);
                }
            }

            /// <summary>
            /// 注销当前域缓存的所有程序集实例
            /// </summary>
            public static void UnregisterAllAssemblies()
            {
                s_cachedAssemblies.Clear();
                s_cachedTypes.Clear();
            }

            /// <summary>
            /// 通过程序集的名称获取对应的程序集实例
            /// </summary>
            /// <param name="assemblyName">程序集名称</param>
            /// <returns>返回该名称对应的程序集实例，若不存在则返回null</returns>
            public static SystemAssembly GetAssembly(string assemblyName)
            {
                if (s_cachedAssemblies.TryGetValue(assemblyName, out SystemAssembly assembly))
                {
                    return assembly;
                }

                return null;
            }

            /// <summary>
            /// 获取已加载的程序集
            /// </summary>
            /// <returns>返回当前加载的程序集列表</returns>
            public static IList<SystemAssembly> GetAllAssemblies()
            {
                return Collection.ToListForValues<string, SystemAssembly>(s_cachedAssemblies);
            }

            /// <summary>
            /// 获取已加载的程序集的全部类型
            /// </summary>
            /// <returns>返回当前加载的程序集全部类型集合</returns>
            public static SystemType[] GetTypes()
            {
                List<SystemType> results = new List<SystemType>();
                foreach (KeyValuePair<string, SystemAssembly> kvp in s_cachedAssemblies)
                {
                    results.AddRange(kvp.Value.GetTypes());
                }

                return results.ToArray();
            }

            /// <summary>
            /// 获取已加载的程序集的全部类型
            /// </summary>
            /// <param name="results">类型集合输出实例</param>
            public static void GetTypes(List<SystemType> results)
            {
                if (null == results)
                {
                    throw new CFrameworkException("Results is invalid.");
                }

                results.Clear();
                foreach (KeyValuePair<string, SystemAssembly> kvp in s_cachedAssemblies)
                {
                    results.AddRange(kvp.Value.GetTypes());
                }
            }

            /// <summary>
            /// 获取已加载的程序集中的指定类型
            /// </summary>
            /// <param name="typeName">类型名称</param>
            /// <returns>返回程序集中指定类型名称的对应定义类型</returns>
            public static SystemType GetType(string typeName)
            {
                if (string.IsNullOrEmpty(typeName))
                {
                    throw new CFrameworkException("Type name is invalid.");
                }

                SystemType type = null;
                if (s_cachedTypes.TryGetValue(typeName, out type))
                {
                    return type;
                }

                type = SystemType.GetType(typeName);
                if (null != type)
                {
                    s_cachedTypes.Add(typeName, type);
                    return type;
                }

                foreach (KeyValuePair<string, SystemAssembly> kvp in s_cachedAssemblies)
                {
                    type = SystemType.GetType(Utility.Text.Format("{%s}, {%s}", typeName, kvp.Value.FullName));
                    if (null != type)
                    {
                        s_cachedTypes.Add(typeName, type);
                        return type;
                    }
                }

                return null;
            }
        }
    }
}
