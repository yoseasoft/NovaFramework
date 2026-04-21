/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyright (C) 2024 - 2025, Hurley, Independent Studio.
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
using System.Runtime.CompilerServices;

using Cysharp.Threading.Tasks;
using NovaFramework.AssetLoader;

using UnityObject = UnityEngine.Object;

namespace GameEngine
{
    // API接口管理工具类
    public static partial class GameApi
    {
        #region 针对场景资源加载相关的服务接口函数

        /// <summary>
        /// 加载指定名称及路径的场景资源对象实例
        /// </summary>
        /// <param name="sceneName">场景资源名称</param>
        /// <param name="url">场景资源路径</param>
        /// <returns>返回场景资源句柄实例</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ISceneHandler LoadSceneAssets(string sceneName, string url)
        {
            return SceneHandler.Instance.LoadSceneAssets(sceneName, url);
        }

        /// <summary>
        /// 卸载指定名称的场景资源对象实例
        /// </summary>
        /// <param name="sceneName">场景资源名称</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void UnloadSceneAssets(string sceneName)
        {
            SceneHandler.Instance.UnloadSceneAssets(sceneName);
        }

        #endregion

        #region 针对通用资源加载相关的服务接口函数

        /// <summary>
        /// 同步加载普通资源
        /// </summary>
        /// <param name="url">资源地址</param>
        /// <returns>返回普通资源句柄实例</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IAssetHandler LoadAssetSync(string url)
        {
            return ResourceHandler.Instance.LoadAssetSync(url);
        }

        /// <summary>
        /// 同步加载普通资源
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="url">资源地址</param>
        /// <returns>返回普通资源句柄实例</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IAssetHandler LoadAssetSync<T>(string url) where T : UnityObject
        {
            return ResourceHandler.Instance.LoadAssetSync<T>(url);
        }

        /// <summary>
        /// 同步加载普通资源
        /// </summary>
        /// <param name="url">资源地址</param>
        /// <param name="type">资源类型</param>
        /// <returns>返回普通资源句柄实例</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IAssetHandler LoadAssetSync(string url, Type type)
        {
            return ResourceHandler.Instance.LoadAssetSync(url, type);
        }

        /// <summary>
        /// 异步加载普通资源
        /// </summary>
        /// <param name="url">资源地址</param>
        /// <returns>返回普通资源句柄实例</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IAssetHandler LoadAssetAsync(string url)
        {
            return ResourceHandler.Instance.LoadAssetAsync(url);
        }

        /// <summary>
        /// 异步加载普通资源
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="url">资源地址</param>
        /// <returns>返回普通资源句柄实例</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IAssetHandler LoadAssetAsync<T>(string url) where T : UnityObject
        {
            return ResourceHandler.Instance.LoadAssetAsync<T>(url);
        }

        /// <summary>
        /// 异步加载普通资源
        /// </summary>
        /// <param name="url">资源地址</param>
        /// <param name="type">资源类型</param>
        /// <returns>返回普通资源句柄实例</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IAssetHandler LoadAssetAsync(string url, Type type)
        {
            return ResourceHandler.Instance.LoadAssetAsync(url, type);
        }

        /// <summary>
        /// 释放当前加载的所有普通资源对象实例
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void UnloadAllAssets()
        {
            ResourceHandler.Instance.UnloadAllAssets();
        }

        #endregion

        #region 针对原始文件资源加载相关的服务接口函数

        /// <summary>
        /// 同步加载原始文件资源
        /// </summary>
        /// <param name="url">资源路径</param>
        /// <returns>返回原始文件资源句柄实例</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IRawFileHandler LoadRawFileSync(string url)
        {
            return ResourceHandler.Instance.LoadRawFileSync(url);
        }

        /// <summary>
        /// 异步加载原始文件资源
        /// </summary>
        /// <param name="url">资源路径</param>
        /// <returns>返回原始文件资源句柄实例</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IRawFileHandler LoadRawFileAsync(string url)
        {
            return ResourceHandler.Instance.LoadRawFileAsync(url);
        }

        #endregion
    }
}
