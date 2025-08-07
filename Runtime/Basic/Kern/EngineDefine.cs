/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
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

namespace GameEngine
{
    /// <summary>
    /// 引擎通用参数定义类
    /// </summary>
    internal static class EngineDefine
    {
        /// <summary>
        /// 核心作用域的命名空间列表
        /// </summary>
        static IList<string> _coreScopeNamespaces = null;

        /// <summary>
        /// 获取核心作用域的命名空间列表
        /// </summary>
        static IList<string> CoreScopeNamespaces
        {
            get
            {
                if (null == _coreScopeNamespaces)
                {
                    _coreScopeNamespaces = new List<string>();

                    // _coreScopeNamespaces.Add("System");
                    // _coreScopeNamespaces.Add("UnityEngine");
                    _coreScopeNamespaces.Add(typeof(NovaEngine.Application).Namespace);
                    _coreScopeNamespaces.Add(typeof(EngineDefine).Namespace);
                }

                return _coreScopeNamespaces;
            }
        }

        /// <summary>
        /// 检测目标类型是否处于核心作用域的命名空间中
        /// </summary>
        /// <param name="classType">对象类型</param>
        /// <returns>若对象类型在核心作用域的命名空间中则返回true，否则返回false</returns>
        public static bool IsCoreScopeClassType(SystemType classType)
        {
            string ns = classType.Namespace;
            for (int n = 0; n < CoreScopeNamespaces.Count; ++n)
            {
                string cs = CoreScopeNamespaces[n];
                if (cs.Equals(ns))
                    return true;
            }

            return false;
        }
    }
}
