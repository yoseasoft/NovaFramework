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

using System.Collections;
using System.Collections.Generic;
using System.Customize.Extension;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml;

using Cysharp.Threading.Tasks;

namespace GameEngine
{
    /// 应用程序的上下文管理器对象类
    public static partial class ApplicationContext
    {
        /// 针对当前应用程序上下文的配置管理封装类
        public static partial class Configure
        {
            #region 模组配置信息的加载管理接口函数

            /// <summary>
            /// 加载模组的配置数据
            /// </summary>
            /// <param name="memoryStream">数据流</param>
            private static void LoadModuleConfigure(MemoryStream memoryStream)
            {
                XmlDocument document = new XmlDocument();
                document.Load(memoryStream);

                XmlElement element = document.DocumentElement;
                XmlNodeList nodeList = element.ChildNodes;
                for (int n = 0; null != nodeList && n < nodeList.Count; ++n)
                {
                    XmlNode node = nodeList[n];

                    Context.Configuring.ModuleConfigureResolver.LoadConfigureContent(node);
                }
            }

            /// <summary>
            /// 应用程序通过指定的处理回调加载模组配置
            /// </summary>
            /// <param name="callback">回调句柄</param>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void LoadModuleConfigure(NovaEngine.Definition.File.OnFileTextLoadingHandler callback)
            {
                LoadModuleConfigure(null, callback);
            }

            /// <summary>
            /// 应用程序通过指定的处理回调加载模组配置
            /// </summary>
            /// <param name="url">资源路径</param>
            /// <param name="callback">回调句柄</param>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void LoadModuleConfigure(string url, NovaEngine.Definition.File.OnFileTextLoadingHandler callback)
            {
                LoadModuleConfigure(url, (url, ms) =>
                {
                    string text = callback(url);
                    if (text.IsNullOrEmpty())
                        return false;

                    byte[] buffer = Encoding.UTF8.GetBytes(text);
                    ms.Write(buffer, 0, buffer.Length);
                    ms.Seek(0, SeekOrigin.Begin);

                    return true;
                });
            }

            /// <summary>
            /// 应用程序通过指定的处理回调加载模组配置
            /// </summary>
            /// <param name="callback">回调句柄</param>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static async UniTask LoadModuleConfigure(NovaEngine.Definition.File.OnFileTextLoadingAsyncHandler callback)
            {
                await LoadModuleConfigure(null, callback);
            }

            /// <summary>
            /// 应用程序通过指定的处理回调加载模组配置
            /// </summary>
            /// <param name="url">资源路径</param>
            /// <param name="callback">回调句柄</param>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static async UniTask LoadModuleConfigure(string url, NovaEngine.Definition.File.OnFileTextLoadingAsyncHandler callback)
            {
                await LoadModuleConfigure(url, async (url, ms) =>
                {
                    string text = await callback(url);
                    if (text.IsNullOrEmpty())
                        return false;

                    byte[] buffer = Encoding.UTF8.GetBytes(text);
                    ms.Write(buffer, 0, buffer.Length);
                    ms.Seek(0, SeekOrigin.Begin);

                    return true;
                });
            }

            /// <summary>
            /// 应用程序通过指定的处理回调加载模组配置
            /// </summary>
            /// <param name="callback">回调句柄</param>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void LoadModuleConfigure(NovaEngine.Definition.File.OnFileStreamLoadingHandler callback)
            {
                LoadModuleConfigure(null, callback);
            }

            /// <summary>
            /// 应用程序通过指定的处理回调加载模组配置
            /// </summary>
            /// <param name="url">资源路径</param>
            /// <param name="callback">回调句柄</param>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void LoadModuleConfigure(string url, NovaEngine.Definition.File.OnFileStreamLoadingHandler callback)
            {
                if (null == callback)
                {
                    Debugger.Error(LogGroupTag.Basic, "The configure file load handler must be non-null, reload module configure failed!");
                    return;
                }

                MemoryStream ms = new MemoryStream();
                Debugger.Info(LogGroupTag.Basic, "指定的模组配置文件‘{%s}’开始进行进入加载队列中……", url);
                if (false == callback(url, ms))
                {
                    Debugger.Error(LogGroupTag.Basic, "载入模组配置数据失败：指定路径‘{%s}’下的配置文件加载回调接口执行异常！", url);
                    return;
                }

                // 加载配置
                LoadModuleConfigure(ms);

                ms.Dispose();
            }

            /// <summary>
            /// 应用程序通过指定的处理回调加载模组配置
            /// </summary>
            /// <param name="callback">回调句柄</param>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static async UniTask LoadModuleConfigure(NovaEngine.Definition.File.OnFileStreamLoadingAsyncHandler callback)
            {
                await LoadModuleConfigure(null, callback);
            }

            /// <summary>
            /// 应用程序通过指定的处理回调加载模组配置
            /// </summary>
            /// <param name="url">资源路径</param>
            /// <param name="callback">回调句柄</param>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static async UniTask LoadModuleConfigure(string url, NovaEngine.Definition.File.OnFileStreamLoadingAsyncHandler callback)
            {
                if (null == callback)
                {
                    Debugger.Error(LogGroupTag.Basic, "The configure file load handler must be non-null, reload module configure failed!");
                    return;
                }

                MemoryStream ms = new MemoryStream();
                Debugger.Info(LogGroupTag.Basic, "指定的模组配置文件‘{%s}’开始进行进入加载队列中……", url);
                if (false == await callback(url, ms))
                {
                    Debugger.Error(LogGroupTag.Basic, "载入模组配置数据失败：指定路径‘{%s}’下的配置文件加载回调接口执行异常！", url);
                    return;
                }

                // 加载配置
                LoadModuleConfigure(ms);

                ms.Dispose();
            }

            /// <summary>
            /// 卸载当前所有解析登记的模组配置数据
            /// </summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void UnloadAllModuleConfigureInfos()
            {
                Context.Configuring.ModuleConfigureResolver.UnloadAllConfigureContents();
            }

            #endregion

            #region 模组配置信息的自动装配接口函数

            /// <summary>
            /// 自动加载模组的导入配置数据<br/>
            /// 该函数会将当前已记录的所有模组文件路径，通过<see cref="LoadModuleConfigure(string, NovaEngine.Definition.File.OnFileStreamLoadingHandler)"/>全部重新加载一次<br/>
            /// 所以如果在使用该函数之前已提前加载过配置数据，则需要先调用<see cref="UnloadAllModuleConfigureInfos()"/>方法进行全部配置数据卸载
            /// </summary>
            /// <param name="callback">回调句柄</param>
            private static void AutoLoadModuleImportConfigure(NovaEngine.Definition.File.OnFileStreamLoadingHandler callback)
            {
                IList<string> list = Context.Configuring.ApplicationConfigureInfo.ModuleUrlPaths;
                for (int n = 0; null != list && n < list.Count; ++n)
                {
                    LoadModuleConfigure(list[n], callback);
                }
            }

            /// <summary>
            /// 自动加载模组的导入配置数据<br/>
            /// 该函数会将当前已记录的所有模组文件路径，通过<see cref="LoadModuleConfigure(string, NovaEngine.Definition.File.OnFileStreamLoadingHandler)"/>全部重新加载一次<br/>
            /// 所以如果在使用该函数之前已提前加载过配置数据，则需要先调用<see cref="UnloadAllModuleConfigureInfos()"/>方法进行全部配置数据卸载
            /// </summary>
            /// <param name="callback">回调句柄</param>
            private static async UniTask AutoLoadModuleImportConfigure(NovaEngine.Definition.File.OnFileStreamLoadingAsyncHandler callback)
            {
                IList<string> list = Context.Configuring.ApplicationConfigureInfo.ModuleUrlPaths;
                for (int n = 0; null != list && n < list.Count; ++n)
                {
                    await LoadModuleConfigure(list[n], callback);
                }
            }

            /// <summary>
            /// 自动加载模组的导入配置数据<br/>
            /// 该函数会将当前已记录的所有模组导入文件路径，通过<see cref="LoadModuleConfigure(string, NovaEngine.Definition.File.OnFileTextLoadingHandler)"/>全部重新加载一次<br/>
            /// 所以如果在使用该函数之前已提前加载过配置数据，则需要先调用<see cref="UnloadAllModuleConfigureInfos()"/>方法进行全部配置数据卸载
            /// </summary>
            /// <param name="callback">回调句柄</param>
            private static void AutoLoadModuleImportConfigure(NovaEngine.Definition.File.OnFileTextLoadingHandler callback)
            {
                IList<string> list = Context.Configuring.ApplicationConfigureInfo.ModuleUrlPaths;
                for (int n = 0; null != list && n < list.Count; ++n)
                {
                    LoadModuleConfigure(list[n], callback);
                }
            }

            /// <summary>
            /// 自动加载模组的导入配置数据<br/>
            /// 该函数会将当前已记录的所有模组导入文件路径，通过<see cref="LoadModuleConfigure(string, NovaEngine.Definition.File.OnFileTextLoadingHandler)"/>全部重新加载一次<br/>
            /// 所以如果在使用该函数之前已提前加载过配置数据，则需要先调用<see cref="UnloadAllModuleConfigureInfos()"/>方法进行全部配置数据卸载
            /// </summary>
            /// <param name="callback">回调句柄</param>
            private static async UniTask AutoLoadModuleImportConfigure(NovaEngine.Definition.File.OnFileTextLoadingAsyncHandler callback)
            {
                IList<string> list = Context.Configuring.ApplicationConfigureInfo.ModuleUrlPaths;
                for (int n = 0; null != list && n < list.Count; ++n)
                {
                    await LoadModuleConfigure(list[n], callback);
                }
            }

            #endregion
        }
    }
}
