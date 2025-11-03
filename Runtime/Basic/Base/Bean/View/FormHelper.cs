/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyright (C) 2025, Hurley, Independent Studio.
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

using SystemType = System.Type;

using UniTask = Cysharp.Threading.Tasks.UniTask;

namespace GameEngine
{
    /// <summary>
    /// GUI的窗口对象辅助工具类
    /// </summary>
    public static class FormHelper
    {
        /// <summary>
        /// 工具类启动状态标识
        /// </summary>
        private static bool _isOnStartup = false;

        /// <summary>
        /// 工具类启动函数
        /// </summary>
        internal static void Startup()
        {
            _isOnStartup = true;

            // UGUI表单支持
            if (NovaEngine.Configuration.unityFormSupported)
            {
                UnityFormHelper.Startup();
            }
            // FairyGUI表单支持
            if (NovaEngine.Configuration.fairyFormSupported)
            {
                FairyFormHelper.Startup();
            }
        }

        /// <summary>
        /// 工具类关闭函数
        /// </summary>
        internal static void Shutdown()
        {
            // UGUI表单支持
            if (NovaEngine.Configuration.unityFormSupported)
            {
                UnityFormHelper.Shutdown();
            }
            // FairyGUI表单支持
            if (NovaEngine.Configuration.fairyFormSupported)
            {
                FairyFormHelper.Shutdown();
            }

            _isOnStartup = false;
        }

        /// <summary>
        /// 工具类刷新函数
        /// </summary>
        internal static void Update()
        {
            Debugger.Assert(_isOnStartup, "Invalide status.");

            // UGUI表单支持
            if (NovaEngine.Configuration.unityFormSupported)
            {
                UnityFormHelper.Update();
            }
            // FairyGUI表单支持
            if (NovaEngine.Configuration.fairyFormSupported)
            {
                FairyFormHelper.Update();
            }
        }

        /// <summary>
        /// 创建一个指定类型的窗口对象实例
        /// </summary>
        /// <param name="formType">窗口类型标识</param>
        /// <param name="viewType">视图类型</param>
        /// <returns>返回窗口对象实例</returns>
        internal static Form CreateForm(ViewFormType formType, SystemType viewType)
        {
            return formType switch
            {
                ViewFormType.UGUI => CreateUnityForm(viewType),
                ViewFormType.FairyGUI => CreateFairyForm(viewType),
                _ => null,
            };
        }

        /// <summary>
        /// 创建一个指定类型的UGUI窗口对象实例
        /// </summary>
        /// <param name="viewType">视图类型</param>
        /// <returns>返回窗口对象实例</returns>
        private static UnityForm CreateUnityForm(SystemType viewType)
        {
            UnityForm form = new UnityForm(viewType);
            return form;
        }

        /// <summary>
        /// 创建一个指定类型的FairyGui窗口对象实例
        /// </summary>
        /// <param name="viewType">视图类型</param>
        /// <returns>返回窗口对象实例</returns>
        private static FairyForm CreateFairyForm(SystemType viewType)
        {
            FairyForm form = new FairyForm(viewType);
            return form;
        }

        /// <summary>
        /// 添加视图窗口的通用包资源
        /// </summary>
        /// <param name="url">资源地址</param>
        public static async UniTask AddCommonPackage(string url)
        {
            await FairyFormHelper.AddCommonPackage(url);
        }

        /// <summary>
        /// 移除视图窗口的通用包资源
        /// </summary>
        /// <param name="url">资源地址</param>
        public static void RemoveCommonPackage(string url)
        {
            FairyFormHelper.RemoveCommonPackage(url);
        }
    }
}
