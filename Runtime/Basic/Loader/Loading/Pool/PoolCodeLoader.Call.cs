/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyright (C) 2023 - 2024, Guangzhou Shiyue Network Technology Co., Ltd.
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

using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

namespace GameEngine.Loader
{
    /// 对象池容器管理对象的分析处理类
    internal static partial class PoolCodeLoader
    {
        /// <summary>
        /// 对象池管理类的结构信息管理容器
        /// </summary>
        private readonly static IDictionary<Type, Structuring.PoolCallCodeInfo> _poolCallCodeInfos = new Dictionary<Type, Structuring.PoolCallCodeInfo>();

        [Preserve]
        [OnCodeLoaderClassLoadOfTarget(typeof(PoolSupportedAttribute))]
        private static bool LoadPoolCallClass(Symbolling.SymClass symClass, bool reload)
        {
            Structuring.PoolCallCodeInfo info = new Structuring.PoolCallCodeInfo();
            info.ClassType = symClass.ClassType;

            if (false == symClass.IsInstantiate)
            {
                Debugger.Warn(LogGroupTag.CodeLoader, "The pool supported class '{%t}' must be was instantiable, newly added it failed.", info.ClassType);
                return false;
            }

            if (_poolCallCodeInfos.ContainsKey(symClass.ClassType))
            {
                if (reload)
                {
                    // 重载模式下，先移除旧的记录
                    _poolCallCodeInfos.Remove(symClass.ClassType);
                }
                else
                {
                    Debugger.Warn(LogGroupTag.CodeLoader, "The pool call type '{%s}' was already existed, repeat added it failed.", symClass.FullName);
                    return false;
                }
            }

            _poolCallCodeInfos.Add(symClass.ClassType, info);
            Debugger.Log(LogGroupTag.CodeLoader, "Load pool call code info '{%s}' succeed from target class type '{%s}'.", CodeLoaderUtils.ToString(info), symClass.FullName);

            return true;
        }

        [Preserve]
        [OnCodeLoaderClassCleanupOfTarget(typeof(PoolSupportedAttribute))]
        private static void CleanupAllPoolCallClasses()
        {
            _poolCallCodeInfos.Clear();
        }

        [Preserve]
        [OnCodeLoaderClassLookupOfTarget(typeof(PoolSupportedAttribute))]
        private static Structuring.PoolCallCodeInfo LookupPoolCallCodeInfo(Symbolling.SymClass symClass)
        {
            if (_poolCallCodeInfos.TryGetValue(symClass.ClassType, out Structuring.PoolCallCodeInfo codeInfo))
            {
                return codeInfo;
            }

            return null;
        }
    }
}
