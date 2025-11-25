/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
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
using System.Runtime.CompilerServices;

namespace GameEngine.Context.Configuring
{
    /// <summary>
    /// 应用配置信息的对象类，用于存放加载的应用配置信息，并提供对外访问操作接口
    /// </summary>
    internal static class ApplicationConfigureInfo
    {
        /// <summary>
        /// 热加载模块类型列表
        /// </summary>
        private static IList<string> _hotModuleTypes;
        /// <summary>
        /// 实体配置路径列表
        /// </summary>
        private static IList<string> _beanUrlPaths;

        /// <summary>
        /// 应用配置包含的文件列表
        /// </summary>
        private static IList<string> _configureFilePaths;

        public static IList<string> HotModuleTypes => _hotModuleTypes;
        public static IList<string> BeanUrlPaths => _beanUrlPaths;

        /// <summary>
        /// 配置信息初始化函数
        /// </summary>
        public static void Initialize()
        {
            // 初始化容器
            _hotModuleTypes = new List<string>();
            _beanUrlPaths = new List<string>();
        }

        /// <summary>
        /// 配置信息清理函数
        /// </summary>
        public static void Cleanup()
        {
            // 清理容器
            RemoveAllConfigureInfos();

            _hotModuleTypes = null;
            _beanUrlPaths = null;

            _configureFilePaths = null;
        }

        /// <summary>
        /// 移除所有的配置信息
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RemoveAllConfigureInfos()
        {
            RemoveAllHotModuleTypes();
            RemoveAllBeanUrlPaths();
        }

        /// <summary>
        /// 新增热加载模块的对象类型
        /// </summary>
        /// <param name="hotModuleType">对象类型</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddHotModuleType(string hotModuleType)
        {
            if (_hotModuleTypes.Contains(hotModuleType))
            {
                Debugger.Warn(LogGroupTag.Basic, "The hot module type '{%s}' was already exists, repeat added it failed.", hotModuleType);
                return;
            }

            _hotModuleTypes.Add(hotModuleType);
        }

        /// <summary>
        /// 移除当前记录的所有热加载模块对象类型
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RemoveAllHotModuleTypes()
        {
            _hotModuleTypes.Clear();
        }

        /// <summary>
        /// 新增实体对象配置文件的资源路径
        /// </summary>
        /// <param name="beanUrlPath">资源路径</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddBeanUrlPath(string beanUrlPath)
        {
            if (_beanUrlPaths.Contains(beanUrlPath))
            {
                Debugger.Warn(LogGroupTag.Basic, "The bean url path '{%s}' was already exists, repeat added it failed.", beanUrlPath);
                return;
            }

            _beanUrlPaths.Add(beanUrlPath);
        }

        /// <summary>
        /// 移除当前记录的所有实体对象配置文件资源路径
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RemoveAllBeanUrlPaths()
        {
            _beanUrlPaths.Clear();
        }

        #region 内联配置文件的访问记录登记/管理接口函数

        /// <summary>
        /// 添加新的配置文件路径到当前的加载上下文中
        /// </summary>
        /// <param name="path">文件路径</param>
        public static void AddConfigureFilePath(string path)
        {
            if (string.IsNullOrEmpty(path))
                return;

            if (null == _configureFilePaths)
                _configureFilePaths = new List<string>();

            if (_configureFilePaths.Contains(path))
            {
                Debugger.Warn(LogGroupTag.Basic, "The configure file path '{%s}' was already exists, repeat added it will be override old value.", path);
                return;
            }

            _configureFilePaths.Add(path);
        }

        /// <summary>
        /// 获取下一个未处理的配置文件加载路径
        /// </summary>
        /// <returns>返回文件路径地址</returns>
        public static string PopNextConfigureFileLoadPath()
        {
            if (null != _configureFilePaths && _configureFilePaths.Count > 0)
            {
                string path = _configureFilePaths[0];
                _configureFilePaths.RemoveAt(0);
                return path;
            }

            return null;
        }

        #endregion
    }
}
