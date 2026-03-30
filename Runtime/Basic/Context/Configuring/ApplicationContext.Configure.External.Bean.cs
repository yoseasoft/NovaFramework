/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyright (C) 2025 - 2026, Hainan Yuanyou Information Technology Co., Ltd. Guangzhou Branch
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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;

using Cysharp.Threading.Tasks;

namespace GameEngine
{
    /// 应用程序的上下文管理器对象类
    public static partial class ApplicationContext
    {
        /// 针对当前应用程序上下文的配置管理封装类
        public static partial class Configure
        {
            #region 实体配置信息的加载管理接口函数

            /// <summary>
            /// 应用程序加载指定的实体配置
            /// </summary>
            /// <param name="mstream">数据流</param>
            [Obsolete]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void LoadBeanConfigure(MemoryStream mstream)
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
            public static async UniTask LoadBeanConfigure(NovaEngine.Definition.File.OnFileTextLoadingAsyncHandler callback)
            {
                await Loader.CodeLoader.LoadBeanConfigureInfo(callback);
            }

            /// <summary>
            /// 应用程序通过指定的处理回调加载实体配置
            /// </summary>
            /// <param name="url">资源路径</param>
            /// <param name="callback">回调句柄</param>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static async UniTask LoadBeanConfigure(string url, NovaEngine.Definition.File.OnFileTextLoadingAsyncHandler callback)
            {
                await Loader.CodeLoader.LoadBeanConfigureInfo(url, callback);
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
            /// 应用程序通过指定的处理回调加载实体配置
            /// </summary>
            /// <param name="callback">回调句柄</param>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static async UniTask LoadBeanConfigure(NovaEngine.Definition.File.OnFileStreamLoadingAsyncHandler callback)
            {
                await Loader.CodeLoader.LoadBeanConfigureInfo(callback);
            }

            /// <summary>
            /// 应用程序通过指定的处理回调加载实体配置
            /// </summary>
            /// <param name="url">资源路径</param>
            /// <param name="callback">回调句柄</param>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static async UniTask LoadBeanConfigure(string url, NovaEngine.Definition.File.OnFileStreamLoadingAsyncHandler callback)
            {
                await Loader.CodeLoader.LoadBeanConfigureInfo(url, callback);
            }

            /// <summary>
            /// 重新绑定符号对象实例中关于实体数据部分的配置信息
            /// </summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void RebindingBeanConfigureOfSymbols()
            {
                // 重新绑定Bean实例
                Loader.CodeLoader.RebindingBeanConfigureOfSymbols();
            }

            /// <summary>
            /// 卸载当前所有解析登记的配置数据对象实例<br/>
            /// 请注意，该接口不建议由用户自定调用，因为这需要充分了解代码加载的流程<br/>
            /// 并在合适的节点进行调度，否则将导致不可预知的问题
            /// </summary>
            [Obsolete]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void UnloadAllBeanConfigureInfos()
            {
                Loader.CodeLoader.UnloadAllBeanConfigureInfos();
            }

            #endregion

            #region 实体配置信息的自动装配接口函数

            /// <summary>
            /// 自动加载Bean的导入配置数据<br/>
            /// 该函数会将当前已记录的所有Bean文件路径，通过<see cref="LoadBeanConfigure(string, NovaEngine.Definition.File.OnFileStreamLoadingHandler)"/>全部重新加载一次<br/>
            /// 所以如果在使用该函数之前已提前加载过配置数据，则需要先调用<see cref="Loader.CodeLoader.UnloadAllBeanConfigureInfos()"/>方法进行全部配置数据卸载
            /// </summary>
            /// <param name="callback">回调句柄</param>
            private static void AutoLoadBeanImportConfigure(NovaEngine.Definition.File.OnFileStreamLoadingHandler callback)
            {
                IList<string> list = Context.Configuring.ApplicationConfigureInfo.BeanUrlPaths;
                for (int n = 0; null != list && n < list.Count; ++n)
                {
                    LoadBeanConfigure(list[n], callback);
                }
            }

            /// <summary>
            /// 自动加载Bean的导入配置数据<br/>
            /// 该函数会将当前已记录的所有Bean文件路径，通过<see cref="LoadBeanConfigure(string, NovaEngine.Definition.File.OnFileStreamLoadingHandler)"/>全部重新加载一次<br/>
            /// 所以如果在使用该函数之前已提前加载过配置数据，则需要先调用<see cref="Loader.CodeLoader.UnloadAllBeanConfigureInfos()"/>方法进行全部配置数据卸载
            /// </summary>
            /// <param name="callback">回调句柄</param>
            private static async UniTask AutoLoadBeanImportConfigure(NovaEngine.Definition.File.OnFileStreamLoadingAsyncHandler callback)
            {
                IList<string> list = Context.Configuring.ApplicationConfigureInfo.BeanUrlPaths;
                for (int n = 0; null != list && n < list.Count; ++n)
                {
                    await LoadBeanConfigure(list[n], callback);
                }
            }

            /// <summary>
            /// 自动加载Bean的导入配置数据<br/>
            /// 该函数会将当前已记录的所有Bean导入文件路径，通过<see cref="LoadBeanConfigure(string, NovaEngine.Definition.File.OnFileTextLoadingHandler)"/>全部重新加载一次<br/>
            /// 所以如果在使用该函数之前已提前加载过配置数据，则需要先调用<see cref="Loader.CodeLoader.UnloadAllBeanConfigureInfos()"/>方法进行全部配置数据卸载
            /// </summary>
            /// <param name="callback">回调句柄</param>
            private static void AutoLoadBeanImportConfigure(NovaEngine.Definition.File.OnFileTextLoadingHandler callback)
            {
                IList<string> list = Context.Configuring.ApplicationConfigureInfo.BeanUrlPaths;
                for (int n = 0; null != list && n < list.Count; ++n)
                {
                    LoadBeanConfigure(list[n], callback);
                }
            }

            /// <summary>
            /// 自动加载Bean的导入配置数据<br/>
            /// 该函数会将当前已记录的所有Bean导入文件路径，通过<see cref="LoadBeanConfigure(string, NovaEngine.Definition.File.OnFileTextLoadingHandler)"/>全部重新加载一次<br/>
            /// 所以如果在使用该函数之前已提前加载过配置数据，则需要先调用<see cref="Loader.CodeLoader.UnloadAllBeanConfigureInfos()"/>方法进行全部配置数据卸载
            /// </summary>
            /// <param name="callback">回调句柄</param>
            private static async UniTask AutoLoadBeanImportConfigure(NovaEngine.Definition.File.OnFileTextLoadingAsyncHandler callback)
            {
                IList<string> list = Context.Configuring.ApplicationConfigureInfo.BeanUrlPaths;
                for (int n = 0; null != list && n < list.Count; ++n)
                {
                    await LoadBeanConfigure(list[n], callback);
                }
            }

            #endregion
        }
    }
}
