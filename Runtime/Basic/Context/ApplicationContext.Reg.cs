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

using System.Runtime.CompilerServices;

using SystemType = System.Type;
using SystemMemoryStream = System.IO.MemoryStream;

namespace GameEngine
{
    /// 应用程序的上下文管理器对象类
    public static partial class ApplicationContext
    {
        #region Bean配置信息的加载管理接口函数

        /// <summary>
        /// 应用程序加载指定的实体配置
        /// </summary>
        /// <param name="mstream">数据流</param>
        [System.Obsolete()]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LoadBeanConfigure(SystemMemoryStream mstream)
        {
            Loader.CodeLoader.LoadBeanConfigureInfo(mstream);
        }

        /// <summary>
        /// 应用程序通过指定的处理回调加载实体配置
        /// </summary>
        /// <param name="callback">回调句柄</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LoadBeanConfigure(NovaEngine.Definition.File.OnFileTextLoadingHandler callback)
        {
            Loader.CodeLoader.LoadBeanConfigureInfo(callback);
        }

        /// <summary>
        /// 应用程序通过指定的处理回调加载实体配置
        /// </summary>
        /// <param name="url">资源路径</param>
        /// <param name="callback">回调句柄</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LoadBeanConfigure(string url, NovaEngine.Definition.File.OnFileTextLoadingHandler callback)
        {
            Loader.CodeLoader.LoadBeanConfigureInfo(url, callback);
        }

        /// <summary>
        /// 应用程序通过指定的处理回调加载实体配置
        /// </summary>
        /// <param name="callback">回调句柄</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LoadBeanConfigure(NovaEngine.Definition.File.OnFileStreamLoadingHandler callback)
        {
            Loader.CodeLoader.LoadBeanConfigureInfo(callback);
        }

        /// <summary>
        /// 应用程序通过指定的处理回调加载实体配置
        /// </summary>
        /// <param name="url">资源路径</param>
        /// <param name="callback">回调句柄</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LoadBeanConfigure(string url, NovaEngine.Definition.File.OnFileStreamLoadingHandler callback)
        {
            Loader.CodeLoader.LoadBeanConfigureInfo(url, callback);
        }

        /// <summary>
        /// 应用程序通过指定的处理回调重载实体配置
        /// </summary>
        /// <param name="callback">回调句柄</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ReloadBeanConfigure(NovaEngine.Definition.File.OnFileTextLoadingHandler callback)
        {
            Loader.CodeLoader.ReloadBeanConfigureInfo(callback);
        }

        /// <summary>
        /// 应用程序通过指定的处理回调重载实体配置
        /// </summary>
        /// <param name="url">资源路径</param>
        /// <param name="callback">回调句柄</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ReloadBeanConfigure(string url, NovaEngine.Definition.File.OnFileTextLoadingHandler callback)
        {
            Loader.CodeLoader.ReloadBeanConfigureInfo(url, callback);
        }

        /// <summary>
        /// 应用程序通过指定的处理回调重载实体配置
        /// </summary>
        /// <param name="callback">回调句柄</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ReloadBeanConfigure(NovaEngine.Definition.File.OnFileStreamLoadingHandler callback)
        {
            Loader.CodeLoader.ReloadBeanConfigureInfo(callback);
        }

        /// <summary>
        /// 应用程序通过指定的处理回调重载实体配置
        /// </summary>
        /// <param name="url">资源路径</param>
        /// <param name="callback">回调句柄</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ReloadBeanConfigure(string url, NovaEngine.Definition.File.OnFileStreamLoadingHandler callback)
        {
            Loader.CodeLoader.ReloadBeanConfigureInfo(url, callback);
        }

        #endregion
    }
}
