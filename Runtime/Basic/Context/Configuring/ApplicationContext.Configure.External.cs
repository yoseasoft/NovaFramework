/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyright (C) 2025 - 2026, Hainan Yuanyou Information Technology Co., Ltd. Guangzhou Branch
/// Copyright (C) 2026, Hurley, Independent Studio.
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

using Cysharp.Threading.Tasks;

namespace GameEngine
{
    /// 应用程序的上下文管理器对象类
    public static partial class ApplicationContext
    {
        /// 针对当前应用程序上下文的配置管理封装类
        public static partial class Configure
        {
            #region 外部配置信息的自动装配接口函数

            /// <summary>
            /// 外部配置初始化回调函数
            /// </summary>
            private static void OnExternalConfigureInitialize()
            {
                // 模组配置解析器初始化
                Context.Configuring.ModuleConfigureResolver.Initialize();
            }

            /// <summary>
            /// 外部配置清理回调函数
            /// </summary>
            private static void OnExternalConfigureCleanup()
            {
                // 模组配置解析器清理
                Context.Configuring.ModuleConfigureResolver.Cleanup();
            }

            /// <summary>
            /// 自动加载扩展的配置数据
            /// </summary>
            /// <param name="callback">回调句柄</param>
            private static void AutoLoadExternalConfigure(NovaEngine.Definition.File.OnFileStreamLoadingHandler callback)
            {
                // 设置模组配置数据
                AutoLoadModuleSettingConfigure(callback);
                // 导入Bean配置数据
                AutoLoadBeanImportConfigure(callback);
            }

            /// <summary>
            /// 自动加载扩展的配置数据
            /// </summary>
            /// <param name="callback">回调句柄</param>
            private static async UniTask AutoLoadExternalConfigure(NovaEngine.Definition.File.OnFileStreamLoadingAsyncHandler callback)
            {
                // 设置模组配置数据
                await AutoLoadModuleSettingConfigure(callback);
                // 导入Bean配置数据
                await AutoLoadBeanImportConfigure(callback);
            }

            /// <summary>
            /// 自动加载扩展的配置数据
            /// </summary>
            /// <param name="callback">回调句柄</param>
            private static void AutoLoadExternalConfigure(NovaEngine.Definition.File.OnFileTextLoadingHandler callback)
            {
                // 设置模组配置数据
                AutoLoadModuleSettingConfigure(callback);
                // 导入Bean配置数据
                AutoLoadBeanImportConfigure(callback);
            }

            /// <summary>
            /// 自动加载扩展的配置数据
            /// </summary>
            /// <param name="callback">回调句柄</param>
            private static async UniTask AutoLoadExternalConfigure(NovaEngine.Definition.File.OnFileTextLoadingAsyncHandler callback)
            {
                // 设置模组配置数据
                await AutoLoadModuleSettingConfigure(callback);
                // 导入Bean配置数据
                await AutoLoadBeanImportConfigure(callback);
            }

            #endregion
        }
    }
}
