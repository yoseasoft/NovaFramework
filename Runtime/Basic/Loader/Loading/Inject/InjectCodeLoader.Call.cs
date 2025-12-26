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

namespace GameEngine.Loader
{
    /// 程序集中注入控制对象的分析处理类
    internal static partial class InjectCodeLoader
    {
        /// <summary>
        /// 注入实体类的结构信息管理容器
        /// </summary>
        private static IDictionary<Type, Structuring.InjectCallCodeInfo> _injectCallCodeInfos = new Dictionary<Type, Structuring.InjectCallCodeInfo>();

        [OnCodeLoaderClassLoadOfTarget(typeof(InjectAttribute))]
        private static bool LoadInjectCallClass(Symboling.SymClass symClass, bool reload)
        {
            if (false == symClass.IsInstantiate)
            {
                Debugger.Error("The inject call class '{%s}' must be instantiable class, loaded it failed.", symClass.FullName);
                return false;
            }

            Structuring.InjectCallCodeInfo info = new Structuring.InjectCallCodeInfo();
            info.ClassType = symClass.ClassType;
            info.BehaviourType = AspectBehaviour.DefaultBehaviourTypeForAutomaticallyInjectedOfBeanExtensionMethod;

            if (_injectCallCodeInfos.ContainsKey(symClass.ClassType))
            {
                if (reload)
                {
                    _injectCallCodeInfos.Remove(symClass.ClassType);
                }
                else
                {
                    Debugger.Warn("The inject call '{%s}' was already existed, repeat added it failed.", symClass.FullName);
                    return false;
                }
            }

            _injectCallCodeInfos.Add(symClass.ClassType, info);
            Debugger.Log(LogGroupTag.CodeLoader, "Load inject call code info '{%s}' succeed from target class type '{%s}'.", CodeLoaderUtils.ToString(info), symClass.FullName);

            return true;
        }

        [OnCodeLoaderClassCleanupOfTarget(typeof(InjectAttribute))]
        private static void CleanupAllInjectCallClasses()
        {
            _injectCallCodeInfos.Clear();
        }

        [OnCodeLoaderClassLookupOfTarget(typeof(InjectAttribute))]
        private static Structuring.InjectCallCodeInfo LookupInjectCallCodeInfo(Symboling.SymClass symClass)
        {
            foreach (KeyValuePair<Type, Structuring.InjectCallCodeInfo> pair in _injectCallCodeInfos)
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
