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
            // private static readonly SystemAssembly[] _assemblies = null;
            private static readonly IDictionary<string, SystemAssembly> _cachedAssemblies = null;
            private static readonly IDictionary<string, SystemType> _cachedTypes = null;

            static Assembly()
            {
                // _assemblies = SystemAppDomain.CurrentDomain.GetAssemblies();
                _cachedAssemblies = new Dictionary<string, SystemAssembly>();
                _cachedTypes = new Dictionary<string, SystemType>(SystemStringComparer.Ordinal);
            }

            /// <summary>
            /// 将指定的程序集注册到当前域程序集缓存容器中
            /// </summary>
            /// <param name="assembly">程序集实例</param>
            public static void RegisterCurrentDomainAssembly(SystemAssembly assembly)
            {
                Dictionary<string, SystemAssembly> dictionary = new Dictionary<string, SystemAssembly>();
                dictionary.Add(assembly.GetName().Name, assembly);

                RegisterCurrentDomainAssemblies(dictionary, true);
            }

            /// <summary>
            /// 将指定的多个程序集全部注册到当前域程序集缓存容器中<br/>
            /// 同时提供了一个重载标识，若该标识打开，则将清除掉当前已注册的所有程序集关联，重新开始记录新的程序集引用
            /// </summary>
            /// <param name="assemblies">目标程序集</param>
            /// <param name="reload">重载标识</param>
            public static void RegisterCurrentDomainAssemblies(IReadOnlyDictionary<string, SystemAssembly> assemblies, bool reload)
            {
                if (reload)
                {
                    // 从添加列表中注销已注册过的程序集
                    UnregisterCurrentDomainAssemblies(Collection.ToListForKeys<string, SystemAssembly>(assemblies));
                }

                foreach (KeyValuePair<string, SystemAssembly> kvp in assemblies)
                {
                    // 在非重载模式下，重复加载将移除旧包
                    if (_cachedAssemblies.ContainsKey(kvp.Key))
                    {
                        if (!reload)
                        {
                            Logger.Error("当前域中的程序集缓存列表中已存在相同名称‘{%s}’的程序包，此处正在进行错误的重复添加操作，建议在非重载模式下先移除同名程序集后再进行注册操作！", kvp.Key);
                        }

                        _cachedAssemblies.Remove(kvp.Key);
                    }

                    _cachedAssemblies.Add(kvp.Key, kvp.Value);
                }
            }

            /// <summary>
            /// 通过指定的程序集名称列表移除对应的程序集实例
            /// </summary>
            /// <param name="assemblyNames">程序集名称列表</param>
            private static void UnregisterCurrentDomainAssemblies(IList<string> assemblyNames)
            {
                bool removed = false;

                for (int n = 0; null != assemblyNames && n < assemblyNames.Count; ++n)
                {
                    removed = true;
                    _cachedAssemblies.Remove(assemblyNames[n]);
                }

                if (removed)
                {
                    _cachedTypes.Clear();
                }
            }

            /// <summary>
            /// 注销当前域缓存的所有程序集实例
            /// </summary>
            public static void UnregisterAllAssemblies()
            {
                _cachedAssemblies.Clear();
                _cachedTypes.Clear();
            }

            /// <summary>
            /// 通过程序集的名称获取对应的程序集实例
            /// </summary>
            /// <param name="assemblyName">程序集名称</param>
            /// <returns>返回该名称对应的程序集实例，若不存在则返回null</returns>
            public static SystemAssembly GetAssembly(string assemblyName)
            {
                if (_cachedAssemblies.TryGetValue(assemblyName, out SystemAssembly assembly))
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
                return Collection.ToListForValues<string, SystemAssembly>(_cachedAssemblies);
            }

            /// <summary>
            /// 获取已加载的程序集的全部类型
            /// </summary>
            /// <returns>返回当前加载的程序集全部类型集合</returns>
            public static SystemType[] GetTypes()
            {
                List<SystemType> results = new List<SystemType>();
                foreach (KeyValuePair<string, SystemAssembly> kvp in _cachedAssemblies)
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
                foreach (KeyValuePair<string, SystemAssembly> kvp in _cachedAssemblies)
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
                if (_cachedTypes.TryGetValue(typeName, out type))
                {
                    return type;
                }

                type = SystemType.GetType(typeName);
                if (null != type)
                {
                    _cachedTypes.Add(typeName, type);
                    return type;
                }

                foreach (KeyValuePair<string, SystemAssembly> kvp in _cachedAssemblies)
                {
                    type = SystemType.GetType(Utility.Text.Format("{%s}, {%s}", typeName, kvp.Value.FullName));
                    if (null != type)
                    {
                        _cachedTypes.Add(typeName, type);
                        return type;
                    }
                }

                return null;
            }
        }
    }
}
