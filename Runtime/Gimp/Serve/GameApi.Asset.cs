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

using UnityObject = UnityEngine.Object;

namespace GameEngine
{
    // API接口管理工具类
    public static partial class GameApi
    {
        #region 针对通用资源加载相关的服务接口函数

        /// <summary>
        /// 同步加载资源
        /// </summary>
        /// <param name="url">资源地址(名字或路径)</param>
        /// <param name="type">资源类型</param>
        public static UnityObject LoadAsset(string url, Type type)
        {
            return ResourceHandler.Instance.LoadAsset(url, type);
        }

        /// <summary>
        /// 异步加载资源
        /// </summary>
        /// <param name="url">资源地址(名字或路径)</param>
        /// <param name="completed">加载完成回调</param>
        public static GooAsset.Asset AsyncLoadAsset<T>(string url, Action<UnityObject> completed) where T : UnityObject
        {
            return ResourceHandler.Instance.AsyncLoadAsset<T>(url, completed);
        }

        /// <summary>
        /// 异步加载资源
        /// </summary>
        /// <param name="url">资源地址(名字或路径)</param>
        public static async UniTask<T> AsyncLoadAsset<T>(string url) where T : UnityObject
        {
            return await ResourceHandler.Instance.AsyncLoadAsset<T>(url);
        }

        /// <summary>
        /// 异步加载资源
        /// </summary>
        /// <param name="url">资源地址(名字或路径)</param>
        /// <param name="type">资源类型</param>
        public static async UniTask<UnityObject> AsyncLoadAsset(string url, Type type)
        {
            return await ResourceHandler.Instance.AsyncLoadAsset(url, type);
        }

        /// <summary>
        /// 释放资源(加载完成或加载中都可以使用此接口释放资源)
        /// </summary>
        /// <param name="asset">资源对象</param>
        public static void UnloadAsset(GooAsset.Asset asset)
        {
            ResourceHandler.Instance.UnloadAsset(asset);
        }

        /// <summary>
        /// 释放已加载的资源
        /// </summary>
        /// <param name="obj">Unity对象</param>
        public static void UnloadAsset(UnityObject obj)
        {
            ResourceHandler.Instance.UnloadAsset(obj);
        }

        /// <summary>
        /// 清理所有资源
        /// </summary>
        public static void RemoveAllAssets()
        {
            ResourceHandler.Instance.RemoveAllAssets();
        }

        #endregion

        #region 针对场景资源加载相关的服务接口函数

        /// <summary>
        /// 加载指定名称及路径的场景资源对象实例
        /// </summary>
        /// <param name="assetName">场景资源名称</param>
        /// <param name="url">场景资源路径</param>
        /// <param name="completed">结束回调</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static GooAsset.Scene LoadAssetScene(string assetName, string url, Action<GooAsset.Scene> completed = null)
        {
            return SceneHandler.Instance.LoadAssetScene(assetName, url, completed);
        }

        /// <summary>
        /// 异步加载场景资源
        /// </summary>
        /// <param name="assetName">场景资源名称</param>
        /// <param name="url">场景资源路径</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async UniTask<GooAsset.Scene> AsyncLoadAssetScene(string assetName, string url)
        {
            return await SceneHandler.Instance.AsyncLoadAssetScene(assetName, url);
        }

        /// <summary>
        /// 卸载指定名称的场景资源对象实例
        /// </summary>
        /// <param name="assetName">场景资源名称</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void UnloadAssetScene(string assetName)
        {
            SceneHandler.Instance.UnloadAssetScene(assetName);
        }

        #endregion

        #region 针对原始文件资源加载相关的服务接口函数

        /// <summary>
        /// 同步加载原始流式文件
        /// </summary>
        /// <param name="url">文件原打包路径</param>
        public static GooAsset.RawFile LoadRawFile(string url)
        {
            return ResourceHandler.Instance.LoadRawFile(url);
        }

        /// <summary>
        /// 异步加载原始流式文件
        /// </summary>
        /// <param name="url">文件原打包路径</param>
        public static GooAsset.RawFile AsyncLoadRawFile(string url, Action<GooAsset.RawFile> completed)
        {
            return ResourceHandler.Instance.AsyncLoadRawFile(url, completed);
        }

        /// <summary>
        /// 异步加载原始流式文件
        /// </summary>
        /// <param name="url">文件原打包路径</param>
        public static async UniTask<GooAsset.RawFile> AsyncLoadRawFile(string url)
        {
            return await ResourceHandler.Instance.AsyncLoadRawFile(url);
        }

        #endregion
    }
}
