/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyright (C) 2024 - 2025, Hurley, Independent Studio.
/// Copyright (C) 2025, Hainan Yuanyou Information Technology Co., Ltd. Guangzhou Branch
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
using System.Runtime.CompilerServices;

namespace GameEngine.Loader.Symbolling
{
    /// 标记对象的解析类
    internal static partial class SymClassResolver
    {
        /// <summary>
        /// 自定义实例化对象类型解析接口管理容器
        /// </summary>
        private static IList<ISymbolResolverOfInstantiationClass> _instantiationClassResolvers;

        /// <summary>
        /// 自定义实例化对象类型解析流程的初始化函数
        /// </summary>
        static void OnCustomizeInstantiationInitialize()
        {
            _instantiationClassResolvers = new List<ISymbolResolverOfInstantiationClass>();
        }

        /// <summary>
        /// 自定义实例化对象类型解析流程的清理函数
        /// </summary>
        static void OnCustomizeInstantiationCleanup()
        {
            _instantiationClassResolvers.Clear();
            _instantiationClassResolvers = null;
        }

        /// <summary>
        /// 自动填充实例对象类型的标记类的特性
        /// </summary>
        /// <param name="symClass">类标记对象</param>
        private static void AutoFillInstantiationClassFeatures(SymClass symClass)
        {
            // 服务于‘CBean’的扩展函数解析

            for (int n = 0; n < _instantiationClassResolvers.Count; ++n)
            {
                ISymbolResolverOfInstantiationClass resolver = _instantiationClassResolvers[n];
                if (resolver.Matches(symClass.ClassType))
                {
                    resolver.Resolve(symClass);
                }
            }
        }

        #region 自定义可实例化对象类型解析流程注册绑定相关接口函数

        /// <summary>
        /// 增加可实例化对象类型解析流程
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddInstantiationClassResolver<T>() where T : ISymbolResolverOfInstantiationClass
        {
            AddInstantiationClassResolver(typeof(T));
        }

        /// <summary>
        /// 增加可实例化对象类型解析流程
        /// </summary>
        /// <param name="classType">对象类型</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddInstantiationClassResolver(Type classType)
        {
            if (false == typeof(ISymbolResolverOfInstantiationClass).IsAssignableFrom(classType))
            {
                Debugger.Error(LogGroupTag.CodeLoader, "The symbol resolver '{%t}' must be inherited from 'ISymbolResolverOfInstantiationClass' interface.", classType);
                return;
            }

            AddInstantiationClassResolver(Activator.CreateInstance(classType) as ISymbolResolverOfInstantiationClass);
        }

        /// <summary>
        /// 增加可实例化对象类型解析流程
        /// </summary>
        /// <param name="resolver">解析对象</param>
        public static void AddInstantiationClassResolver(ISymbolResolverOfInstantiationClass resolver)
        {
            Debugger.Assert(resolver, NovaEngine.ErrorText.InvalidArguments);

            if (_instantiationClassResolvers.Contains(resolver))
            {
                Debugger.Error(LogGroupTag.CodeLoader, "The symbol resolver '{%t}' was already exists, repeat added it failed.", resolver);
                return;
            }

            _instantiationClassResolvers.Add(resolver);
        }

        /// <summary>
        /// 移除可实例化对象类型解析流程
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RemoveInstantiationClassResolver<T>() where T : ISymbolResolverOfInstantiationClass
        {
            RemoveInstantiationClassResolver(typeof(T));
        }

        /// <summary>
        /// 移除可实例化对象类型解析流程
        /// </summary>
        /// <param name="classType">对象类型</param>
        public static void RemoveInstantiationClassResolver(Type classType)
        {
            if (false == typeof(ISymbolResolverOfInstantiationClass).IsAssignableFrom(classType))
            {
                Debugger.Error(LogGroupTag.CodeLoader, "The symbol resolver '{%t}' must be inherited from 'ISymbolResolverOfInstantiationClass' interface.", classType);
                return;
            }

            for (int n = _instantiationClassResolvers.Count - 1; n >= 0 ; --n)
            {
                ISymbolResolverOfInstantiationClass resolver = _instantiationClassResolvers[n];
                if (classType == resolver.GetType())
                {
                    _instantiationClassResolvers.RemoveAt(n);
                }
            }
        }

        /// <summary>
        /// 移除可实例化对象类型解析流程
        /// </summary>
        /// <param name="resolver">解析对象</param>
        public static void RemoveInstantiationClassResolver(ISymbolResolverOfInstantiationClass resolver)
        {
            if (_instantiationClassResolvers.Contains(resolver))
            {
                _instantiationClassResolvers.Remove(resolver);
            }
        }

        #endregion
    }
}
