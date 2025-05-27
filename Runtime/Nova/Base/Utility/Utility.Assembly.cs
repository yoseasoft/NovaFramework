/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
/// Copyring (C) 2022 - 2023, Shanghai Bilibili Technology Co., Ltd.
/// Copyring (C) 2023 - 2024, Guangzhou Shiyue Network Technology Co., Ltd.
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
            private static readonly SystemAssembly[] s_assemblies = null;
            private static readonly IDictionary<string, SystemType> s_cachedTypes = new Dictionary<string, SystemType>(SystemStringComparer.Ordinal);

            static Assembly()
            {
                s_assemblies = SystemAppDomain.CurrentDomain.GetAssemblies();
            }

            /// <summary>
            /// 获取已加载的程序集
            /// </summary>
            /// <returns>返回当前加载的程序集列表</returns>
            public static SystemAssembly[] GetAssemblies()
            {
                return s_assemblies;
            }

            /// <summary>
            /// 获取已加载的程序集的全部类型
            /// </summary>
            /// <returns>返回当前加载的程序集全部类型集合</returns>
            public static SystemType[] GetTypes()
            {
                List<SystemType> results = new List<SystemType>();
                foreach (SystemAssembly assembly in s_assemblies)
                {
                    results.AddRange(assembly.GetTypes());
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
                foreach (SystemAssembly assembly in s_assemblies)
                {
                    results.AddRange(assembly.GetTypes());
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

                foreach (SystemAssembly assembly in s_assemblies)
                {
                    type = SystemType.GetType(Utility.Text.Format("{0}, {1}", typeName, assembly.FullName));
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
