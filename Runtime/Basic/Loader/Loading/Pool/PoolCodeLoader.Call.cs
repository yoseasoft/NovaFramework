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

using System.Collections.Generic;

using SystemType = System.Type;

namespace GameEngine.Loader
{
    /// <summary>
    /// 对象池容器管理对象的分析处理类，对业务层载入的所有对象池支持类进行统一加载及分析处理
    /// </summary>
    internal static partial class PoolCodeLoader
    {
        /// <summary>
        /// 对象池管理类的结构信息管理容器
        /// </summary>
        private static IDictionary<SystemType, Structuring.PoolCallCodeInfo> _poolCallCodeInfos = new Dictionary<SystemType, Structuring.PoolCallCodeInfo>();

        [OnCodeLoaderClassLoadOfTarget(typeof(PoolSupportedAttribute))]
        private static bool LoadPoolCallClass(Symboling.SymClass symClass, bool reload)
        {
            Structuring.PoolCallCodeInfo info = new Structuring.PoolCallCodeInfo();
            info.ClassType = symClass.ClassType;

            if (false == symClass.IsInstantiate)
            {
                Debugger.Warn("The pool supported class '{0}' must be was instantiable, newly added it failed.", info.ClassType.FullName);
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
                    Debugger.Warn("The pool call type '{0}' was already existed, repeat added it failed.", symClass.FullName);
                    return false;
                }
            }

            _poolCallCodeInfos.Add(symClass.ClassType, info);
            Debugger.Log(LogGroupTag.CodeLoader, "Load pool call code info '{0}' succeed from target class type '{1}'.", CodeLoaderUtils.ToString(info), symClass.FullName);

            return true;
        }

        [OnCodeLoaderClassCleanupOfTarget(typeof(PoolSupportedAttribute))]
        private static void CleanupAllPoolCallClasses()
        {
            _poolCallCodeInfos.Clear();
        }

        [OnCodeLoaderClassLookupOfTarget(typeof(PoolSupportedAttribute))]
        private static Structuring.PoolCallCodeInfo LookupPoolCallCodeInfo(Symboling.SymClass symClass)
        {
            foreach (KeyValuePair<SystemType, Structuring.PoolCallCodeInfo> pair in _poolCallCodeInfos)
            {
                if (pair.Value.ClassType == symClass.ClassType)
                {
                    return pair.Value;
                }
            }

            return null;
        }
    }
}
