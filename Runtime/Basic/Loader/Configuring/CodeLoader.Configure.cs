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
using System.Customize.Extension;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml;
using Cysharp.Threading.Tasks;

namespace GameEngine.Loader
{
    /// 程序集的分析处理类
    public static partial class CodeLoader
    {
        /// <summary>
        /// 初始化针对所有配置解析类声明的全部绑定回调接口
        /// </summary>
        [OnClassSubmoduleInitializeCallback]
        private static void InitAllCodeConfigureLoadingCallbacks()
        {
            // 配置解析器初始化
            Configuring.CodeConfigureResolver.Initialize();
        }

        /// <summary>
        /// 清理针对所有配置解析类声明的全部绑定回调接口
        /// </summary>
        [OnClassSubmoduleCleanupCallback]
        private static void CleanupAllCodeConfigureLoadingCallbacks()
        {
            // 配置解析器清理
            Configuring.CodeConfigureResolver.Cleanup();
        }

        /// <summary>
        /// 加载通用类库的配置数据
        /// </summary>
        /// <param name="buffer">数据流</param>
        /// <param name="offset">数据偏移</param>
        /// <param name="length">数据长度</param>
        private static void LoadGeneralConfigure(byte[] buffer, int offset, int length)
        {
            MemoryStream memoryStream = new MemoryStream();
            memoryStream.Write(buffer, offset, length);
            memoryStream.Seek(0, SeekOrigin.Begin);

            LoadGeneralConfigure(memoryStream);

            memoryStream.Dispose();
        }

        /// <summary>
        /// 加载通用类库的配置数据
        /// </summary>
        /// <param name="memoryStream">数据流</param>
        private static void LoadGeneralConfigure(MemoryStream memoryStream)
        {
            XmlDocument document = new XmlDocument();
            document.Load(memoryStream);

            XmlElement element = document.DocumentElement;
            XmlNodeList nodeList = element.ChildNodes;
            for (int n = 0; null != nodeList && n < nodeList.Count; ++n)
            {
                XmlNode node = nodeList[n];

                Configuring.CodeConfigureResolver.LoadConfigureContent(node);
            }
        }

        /// <summary>
        /// 重载通用类库的配置数据
        /// </summary>
        /// <param name="callback">回调句柄</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void LoadGeneralConfigure(NovaEngine.Definition.File.OnFileStreamLoadingHandler callback)
        {
            LoadGeneralConfigure(null, callback);
        }

        /// <summary>
        /// 重载通用类库的配置数据
        /// </summary>
        /// <param name="url">资源路径</param>
        /// <param name="callback">回调句柄</param>
        private static void LoadGeneralConfigure(string url, NovaEngine.Definition.File.OnFileStreamLoadingHandler callback)
        {
            string path = url;
            if (null == callback)
            {
                Debugger.Error(LogGroupTag.CodeLoader, "The configure file load handler must be non-null, reload general configure failed!");
                return;
            }

            do
            {
                MemoryStream ms = new MemoryStream();
                Debugger.Info(LogGroupTag.CodeLoader, "指定的实体配置文件‘{%s}’开始进行进入加载队列中……", path);
                if (false == callback(path, ms))
                {
                    Debugger.Error(LogGroupTag.CodeLoader, "载入Bean配置数据失败：指定路径‘{%s}’下的配置文件加载回调接口执行异常！", path);
                    return;
                }

                // 加载配置
                LoadGeneralConfigure(ms);

                ms.Dispose();

                // 获取下一个文件路径
                path = Configuring.CodeConfigureResolver.PopNextConfigureFileLoadPath();
            } while (null != path);
        }

        /// <summary>
        /// 重载通用类库的配置数据
        /// </summary>
        /// <param name="callback">回调句柄</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static async UniTask LoadGeneralConfigure(NovaEngine.Definition.File.OnFileStreamLoadingAsyncHandler callback)
        {
            await LoadGeneralConfigure(null, callback);
        }

        /// <summary>
        /// 重载通用类库的配置数据
        /// </summary>
        /// <param name="url">资源路径</param>
        /// <param name="callback">回调句柄</param>
        private static async UniTask LoadGeneralConfigure(string url, NovaEngine.Definition.File.OnFileStreamLoadingAsyncHandler callback)
        {
            string path = url;
            if (null == callback)
            {
                Debugger.Error(LogGroupTag.CodeLoader, "The configure file load handler must be non-null, reload general configure failed!");
                return;
            }

            do
            {
                MemoryStream ms = new MemoryStream();
                Debugger.Info(LogGroupTag.CodeLoader, "指定的实体配置文件‘{%s}’开始进行进入加载队列中……", path);
                if (false == await callback(path, ms))
                {
                    Debugger.Error(LogGroupTag.CodeLoader, "载入Bean配置数据失败：指定路径‘{%s}’下的配置文件加载回调接口执行异常！", path);
                    return;
                }

                // 加载配置
                LoadGeneralConfigure(ms);

                ms.Dispose();

                // 获取下一个文件路径
                path = Configuring.CodeConfigureResolver.PopNextConfigureFileLoadPath();
            } while (null != path);
        }

        /// <summary>
        /// 重载通用类库的配置数据
        /// </summary>
        /// <param name="callback">回调句柄</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void LoadGeneralConfigure(NovaEngine.Definition.File.OnFileTextLoadingHandler callback)
        {
            LoadGeneralConfigure(null, callback);
        }

        /// <summary>
        /// 重载通用类库的配置数据
        /// </summary>
        /// <param name="url">资源路径</param>
        /// <param name="callback">回调句柄</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void LoadGeneralConfigure(string url, NovaEngine.Definition.File.OnFileTextLoadingHandler callback)
        {
            LoadGeneralConfigure(url, (url, ms) =>
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
        /// 重载通用类库的配置数据
        /// </summary>
        /// <param name="callback">回调句柄</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static async UniTask LoadGeneralConfigure(NovaEngine.Definition.File.OnFileTextLoadingAsyncHandler callback)
        {
            await LoadGeneralConfigure(null, callback);
        }

        /// <summary>
        /// 重载通用类库的配置数据
        /// </summary>
        /// <param name="url">资源路径</param>
        /// <param name="callback">回调句柄</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static async UniTask LoadGeneralConfigure(string url, NovaEngine.Definition.File.OnFileTextLoadingAsyncHandler callback)
        {
            await LoadGeneralConfigure(url, async (url, ms) =>
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
        /// 通过指定的配置名称，获取对应的配置数据结构信息
        /// </summary>
        /// <param name="name">配置名称</param>
        /// <returns>返回配置数据实例，若查找失败返回null</returns>
        private static Configuring.BaseConfigureInfo GetConfigureInfoByName(string name)
        {
            return Configuring.CodeConfigureResolver.GetNodeConfigureInfoByName(name);
        }

        /// <summary>
        /// 获取当前注册的所有配置数据结构信息对象实例
        /// </summary>
        /// <returns>返回当前所有配置数据实例</returns>
        private static IReadOnlyList<Configuring.BaseConfigureInfo> GetAllConfigureInfos()
        {
            return Configuring.CodeConfigureResolver.GetAllNodeConfigureInfos();
        }

        /// <summary>
        /// 卸载当前所有解析登记的配置数据对象实例
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void UnloadAllConfigureContents()
        {
            Configuring.CodeConfigureResolver.UnloadAllConfigureContents();
        }
    }
}
