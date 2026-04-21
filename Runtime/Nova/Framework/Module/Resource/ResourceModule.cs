/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
/// Copyright (C) 2017 - 2020, Shanghai Tommon Network Technology Co., Ltd.
/// Copyright (C) 2020 - 2022, Guangzhou Xinyuan Technology Co., Ltd.
/// Copyright (C) 2022 - 2023, Shanghai Bilibili Technology Co., Ltd.
/// Copyright (C) 2023 - 2024, Guangzhou Shiyue Network Technology Co., Ltd.
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

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using UnityEngine.SceneManagement;
using NovaFramework.AssetLoader;

using UnityObject = UnityEngine.Object;

namespace NovaEngine.Module
{
    /// <summary>
    /// 资源管理器对象类，统一处理打包资源的加载读取，缓存释放等功能，为其提供操作接口
    /// </summary>
    internal sealed partial class ResourceModule : ModuleObject
    {
        /// <summary>
        /// 资源模块事件类型
        /// </summary>
        public override sealed int EventType => (int) ModuleEventType.Resource;

        /// <summary>
        /// 管理器对象初始化接口函数
        /// </summary>
        protected override sealed void OnInitialize()
        {
        }

        /// <summary>
        /// 管理器对象清理接口函数
        /// </summary>
        protected override sealed void OnCleanup()
        {
        }

        /// <summary>
        /// 管理器对象初始启动接口
        /// </summary>
        protected override sealed void OnStartup()
        {
        }

        /// <summary>
        /// 管理器对象结束关闭接口
        /// </summary>
        protected override sealed void OnShutdown()
        {
        }

        /// <summary>
        /// 管理器对象垃圾回收调度接口
        /// </summary>
        protected override sealed void OnDump()
        {
        }

        /// <summary>
        /// 管理器内部事务更新接口
        /// </summary>
        protected override sealed void OnUpdate()
        {
        }

        /// <summary>
        /// 管理器内部后置更新接口
        /// </summary>
        protected override sealed void OnLateUpdate()
        {
        }

        #region 场景资源加载及卸载操作相关的接口函数

        /// <summary>
        /// 同步加载场景资源
        /// </summary>
        /// <param name="url">资源地址</param>
        /// <returns>返回场景资源句柄实例</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ISceneHandler LoadSceneSync(string url)
        {
            return AssetManagement.LoadSceneSync(url);
        }

        /// <summary>
        /// 同步加载场景资源
        /// </summary>
        /// <param name="url">资源地址</param>
        /// <param name="sceneMode">场景加载模式</param>
        /// <param name="physicsMode">场景物理模式</param>
        /// <returns>返回场景资源句柄实例</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ISceneHandler LoadSceneSync(string url, LoadSceneMode sceneMode, LocalPhysicsMode physicsMode)
        {
            return AssetManagement.LoadSceneSync(url, sceneMode, physicsMode);
        }

        /// <summary>
        /// 异步加载场景资源
        /// </summary>
        /// <param name="url">资源地址</param>
        /// <returns>返回场景资源句柄实例</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ISceneHandler LoadSceneAsync(string url)
        {
            return AssetManagement.LoadSceneAsync(url);
        }

        /// <summary>
        /// 异步加载场景资源
        /// </summary>
        /// <param name="url">资源地址</param>
        /// <param name="sceneMode">场景加载模式</param>
        /// <param name="physicsMode">场景物理模式</param>
        /// <returns>返回场景资源句柄实例</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ISceneHandler LoadSceneAsync(string url, LoadSceneMode sceneMode, LocalPhysicsMode physicsMode)
        {
            return AssetManagement.LoadSceneAsync(url, sceneMode, physicsMode);
        }

        #endregion

        #region 普通资源加载及卸载操作相关的接口函数

        /// <summary>
        /// 同步加载普通资源
        /// </summary>
        /// <param name="url">资源地址</param>
        /// <returns>返回普通资源句柄实例</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IAssetHandler LoadAssetSync(string url)
        {
            return AssetManagement.LoadAssetSync(url);
        }

        /// <summary>
        /// 同步加载普通资源
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="url">资源地址</param>
        /// <returns>返回普通资源句柄实例</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IAssetHandler LoadAssetSync<T>(string url) where T : UnityObject
        {
            return AssetManagement.LoadAssetSync<T>(url);
        }

        /// <summary>
        /// 同步加载普通资源
        /// </summary>
        /// <param name="url">资源地址</param>
        /// <param name="type">资源类型</param>
        /// <returns>返回普通资源句柄实例</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IAssetHandler LoadAssetSync(string url, Type type)
        {
            return AssetManagement.LoadAssetSync(url, type);
        }

        /// <summary>
        /// 异步加载普通资源
        /// </summary>
        /// <param name="url">资源地址</param>
        /// <returns>返回普通资源句柄实例</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IAssetHandler LoadAssetAsync(string url)
        {
            return AssetManagement.LoadAssetAsync(url);
        }

        /// <summary>
        /// 异步加载普通资源
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="url">资源地址</param>
        /// <returns>返回普通资源句柄实例</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IAssetHandler LoadAssetAsync<T>(string url) where T : UnityObject
        {
            return AssetManagement.LoadAssetAsync<T>(url);
        }

        /// <summary>
        /// 异步加载普通资源
        /// </summary>
        /// <param name="url">资源地址</param>
        /// <param name="type">资源类型</param>
        /// <returns>返回普通资源句柄实例</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IAssetHandler LoadAssetAsync(string url, Type type)
        {
            return AssetManagement.LoadAssetAsync(url, type);
        }

        /// <summary>
        /// 释放当前加载的所有普通资源对象实例
        /// </summary>
        public void UnloadAllAssets()
        {
            Debugger.Throw<InvalidOperationException>();
        }

        #endregion

        # region 原始文件资源加载及卸载操作相关的接口函数

        /// <summary>
        /// 同步加载原始文件资源
        /// </summary>
        /// <param name="url">资源路径</param>
        /// <returns>返回原始文件资源句柄实例</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IRawFileHandler LoadRawFileSync(string url)
        {
            return AssetManagement.LoadRawFileSync(url);
        }

        /// <summary>
        /// 异步加载原始文件资源
        /// </summary>
        /// <param name="url">资源路径</param>
        /// <returns>返回原始文件资源句柄实例</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IRawFileHandler LoadRawFileAsync(string url)
        {
            return AssetManagement.LoadRawFileAsync(url);
        }

        #endregion
    }
}
