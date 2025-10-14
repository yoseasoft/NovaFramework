/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyright (C) 2023 - 2024, Guangzhou Shiyue Network Technology Co., Ltd.
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

using System.Collections.Generic;

using SystemType = System.Type;

namespace GameEngine.Loader
{
    /// <summary>
    /// 程序集的分析处理类，对业务层载入的所有对象类进行统一加载及分析处理
    /// </summary>
    public static partial class CodeLoader
    {
        /// <summary>
        /// 指定名称的程序集过滤处理回调函数接口
        /// </summary>
        /// <param name="assemblyName">程序集名称</param>
        /// <param name="classType">当前解析类</param>
        /// <returns>若目标类需要加载则返回true，否则返回false</returns>
        public delegate bool AssemblyLoadFiltingProcessor(string assemblyName, SystemType classType);

        /// <summary>
        /// 指定名称的程序集解析进度通知回调函数接口
        /// </summary>
        /// <param name="assemblyName">程序集名称</param>
        /// <param name="classType">当前解析类</param>
        /// <param name="current">当前进度值</param>
        /// <param name="max">上限值</param>
        public delegate void LoadClassProgress(string assemblyName, SystemType classType, int current, int max);

        /// <summary>
        /// 指定名称的程序集解析完成通知回调函数接口
        /// </summary>
        /// <param name="assemblyName">程序集名称</param>
        public delegate void LoadAssemblyCompleted(string assemblyName);

        /// <summary>
        /// 加载程序集的过滤处理的回调函数管理容器
        /// </summary>
        private static IList<AssemblyLoadFiltingProcessor> _assemblyLoadFiltingProcessorCallbacks;

        /// <summary>
        /// 加载程序集的进度通知的回调函数管理容器
        /// </summary>
        private static IList<LoadClassProgress> _loadClassProgressCallbacks;

        /// <summary>
        /// 加载程序集的完成通知的回调函数管理容器
        /// </summary>
        private static IList<LoadAssemblyCompleted> _loadAssemblyCompletedCallbacks;

        /// <summary>
        /// 程序集过滤处理转发
        /// </summary>
        /// <param name="assemblyName">程序集名称</param>
        /// <param name="classType">当前解析类</param>
        /// <returns>若目标类需要加载则返回true，否则返回false</returns>
        private static bool CallAssemblyLoadFiltingProcessor(string assemblyName, SystemType classType)
        {
            for (int n = 0; null != _assemblyLoadFiltingProcessorCallbacks && n < _assemblyLoadFiltingProcessorCallbacks.Count; ++n)
            {
                // 当某一个过滤条件满足时，就可以直接忽略该类
                if (false == _assemblyLoadFiltingProcessorCallbacks[n].Invoke(assemblyName, classType))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// 程序集解析进度通知转发
        /// </summary>
        /// <param name="assemblyName">程序集名称</param>
        /// <param name="classType">当前解析类</param>
        /// <param name="current">当前进度值</param>
        /// <param name="max">上限值</param>
        private static void CallLoadClassProgress(string assemblyName, SystemType classType, int current, int max)
        {
            for (int n = 0; null != _loadClassProgressCallbacks && n < _loadClassProgressCallbacks.Count; ++n)
            {
                _loadClassProgressCallbacks[n].Invoke(assemblyName, classType, current, max);
            }
        }

        /// <summary>
        /// 程序集解析完成通知转发
        /// </summary>
        /// <param name="assemblyName">程序集名称</param>
        private static void CallLoadAssemblyCompleted(string assemblyName)
        {
            for (int n = 0; null != _loadAssemblyCompletedCallbacks && n < _loadAssemblyCompletedCallbacks.Count; ++n)
            {
                _loadAssemblyCompletedCallbacks[n].Invoke(assemblyName);
            }
        }

        /// <summary>
        /// 新增程序集过滤处理回调接口
        /// </summary>
        /// <param name="callback">函数接口</param>
        public static void AddAssemblyLoadFiltingProcessorCallback(AssemblyLoadFiltingProcessor callback)
        {
            if (null == _assemblyLoadFiltingProcessorCallbacks)
            {
                _assemblyLoadFiltingProcessorCallbacks = new List<AssemblyLoadFiltingProcessor>();
            }

            if (_assemblyLoadFiltingProcessorCallbacks.Contains(callback))
            {
                Debugger.Warn("The assembly load filting processor callback '{0}' was already exist, repeat added it failed.", NovaEngine.Utility.Text.ToString(callback));
                return;
            }

            _assemblyLoadFiltingProcessorCallbacks.Add(callback);
        }

        /// <summary>
        /// 移除程序集过滤处理回调接口
        /// </summary>
        /// <param name="callback">函数接口</param>
        public static void RemoveAssemblyLoadFiltingProcessorCallback(AssemblyLoadFiltingProcessor callback)
        {
            if (null == _assemblyLoadFiltingProcessorCallbacks)
            {
                return;
            }

            if (_assemblyLoadFiltingProcessorCallbacks.Contains(callback))
            {
                _assemblyLoadFiltingProcessorCallbacks.Remove(callback);
            }
        }

        /// <summary>
        /// 新增程序集解析进度通知回调接口
        /// </summary>
        /// <param name="callback">函数接口</param>
        internal static void AddLoadClassProgressCallback(LoadClassProgress callback)
        {
            if (null == _loadClassProgressCallbacks)
            {
                _loadClassProgressCallbacks = new List<LoadClassProgress>();
            }

            if (_loadClassProgressCallbacks.Contains(callback))
            {
                Debugger.Warn("The load class progress callback '{0}' was already exist, repeat added it failed.", NovaEngine.Utility.Text.ToString(callback));
                return;
            }

            _loadClassProgressCallbacks.Add(callback);
        }

        /// <summary>
        /// 移除程序集解析进度通知回调接口
        /// </summary>
        /// <param name="callback">函数接口</param>
        internal static void RemoveLoadClassProgressCallback(LoadClassProgress callback)
        {
            if (null == _loadClassProgressCallbacks)
            {
                return;
            }

            if (_loadClassProgressCallbacks.Contains(callback))
            {
                _loadClassProgressCallbacks.Remove(callback);
            }
        }

        /// <summary>
        /// 新增程序集解析进度通知回调接口
        /// </summary>
        /// <param name="callback">函数接口</param>
        internal static void AddLoadAssemblyCompletedCallback(LoadAssemblyCompleted callback)
        {
            if (null == _loadAssemblyCompletedCallbacks)
            {
                _loadAssemblyCompletedCallbacks = new List<LoadAssemblyCompleted>();
            }

            if (_loadAssemblyCompletedCallbacks.Contains(callback))
            {
                Debugger.Warn("The load assembly completed callback '{0}' was already exist, repeat added it failed.", NovaEngine.Utility.Text.ToString(callback));
                return;
            }

            _loadAssemblyCompletedCallbacks.Add(callback);
        }

        /// <summary>
        /// 移除程序集解析进度通知回调接口
        /// </summary>
        /// <param name="callback">函数接口</param>
        internal static void RemoveLoadAssemblyCompletedCallback(LoadAssemblyCompleted callback)
        {
            if (null == _loadAssemblyCompletedCallbacks)
            {
                return;
            }

            if (_loadAssemblyCompletedCallbacks.Contains(callback))
            {
                _loadAssemblyCompletedCallbacks.Remove(callback);
            }
        }

        /// <summary>
        /// 移除所有的监听加载回调函数
        /// </summary>
        private static void RemoveAllListenerLoadedCallbacks()
        {
            if (null != _assemblyLoadFiltingProcessorCallbacks)
            {
                _assemblyLoadFiltingProcessorCallbacks.Clear();
                _assemblyLoadFiltingProcessorCallbacks = null;
            }
            if (null != _loadClassProgressCallbacks)
            {
                _loadClassProgressCallbacks.Clear();
                _loadClassProgressCallbacks = null;
            }

            if (null != _loadAssemblyCompletedCallbacks)
            {
                _loadAssemblyCompletedCallbacks.Clear();
                _loadAssemblyCompletedCallbacks = null;
            }
        }
    }
}
