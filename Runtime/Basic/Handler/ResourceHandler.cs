/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyright (C) 2020 - 2022, Guangzhou Xinyuan Technology Co., Ltd.
/// Copyright (C) 2022 - 2023, Shanghai Bilibili Technology Co., Ltd.
/// Copyright (C) 2023 - 2024, Guangzhou Shiyue Network Technology Co., Ltd.
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
using Cysharp.Threading.Tasks;

using UnityObject = UnityEngine.Object;

namespace GameEngine
{
    /// <summary>
    /// 资源模块封装的句柄对象类
    /// 模块具体功能接口请参考<see cref="NovaEngine.Module.ResourceModule"/>类
    /// </summary>
    public sealed partial class ResourceHandler : BaseHandler
    {
        /// <summary>
        /// 句柄对象的单例访问获取接口
        /// </summary>
        public static ResourceHandler Instance => HandlerManagement.ResourceHandler;

        /// <summary>
        /// 句柄对象内置初始化接口函数
        /// </summary>
        /// <returns>若句柄对象初始化成功则返回true，否则返回false</returns>
        protected override bool OnInitialize()
        {
            return true;
        }

        /// <summary>
        /// 句柄对象内置清理接口函数
        /// </summary>
        protected override void OnCleanup()
        {
        }

        /// <summary>
        /// 句柄对象内置重载接口函数
        /// </summary>
        protected override void OnReload()
        {
        }

        /// <summary>
        /// 句柄对象内置执行接口
        /// </summary>
        protected override void OnExecute()
        {
        }

        /// <summary>
        /// 句柄对象内置延迟执行接口
        /// </summary>
        protected override void OnLateExecute()
        {
        }

        /// <summary>
        /// 句柄对象内置刷新接口
        /// </summary>
        protected override void OnUpdate()
        {
        }

        /// <summary>
        /// 句柄对象内置延迟刷新接口
        /// </summary>
        protected override void OnLateUpdate()
        {
        }

        /// <summary>
        /// 句柄对象的模块事件转发回调接口
        /// </summary>
        /// <param name="e">模块事件参数</param>
        public override void OnEventDispatch(NovaEngine.Module.ModuleEventArgs e)
        {
        }

        #region 资源加载/卸载相关的接口函数

        /// <summary>
        /// 同步加载资源
        /// </summary>
        /// <param name="url">资源地址(名字或路径)</param>
        /// <param name="type">资源类型</param>
        public UnityObject LoadAsset(string url, Type type)
        {
            return ResourceModule.LoadAsset(url, type);
        }

        /// <summary>
        /// 异步加载资源
        /// </summary>
        /// <param name="url">资源地址(名字或路径)</param>
        /// <param name="completed">加载完成回调</param>
        public GooAsset.Asset LoadAssetAsync<T>(string url, Action<UnityObject> completed) where T : UnityObject
        {
            return ResourceModule.LoadAssetAsync(url, typeof(T), completed);
        }

        /// <summary>
        /// 异步加载资源
        /// </summary>
        /// <param name="url">资源地址(名字或路径)</param>
        public async UniTask<T> LoadAssetAsync<T>(string url) where T : UnityObject
        {
            GooAsset.Asset asset = ResourceModule.LoadAssetAsync(url, typeof(T));
            if (asset is null)
                return null;

            await asset.Task;
            return (T) asset.result;
        }

        /// <summary>
        /// 释放资源(加载完成或加载中都可以使用此接口释放资源)
        /// </summary>
        /// <param name="asset">资源对象</param>
        public void UnloadAsset(GooAsset.Asset asset)
        {
            ResourceModule.UnloadAsset(asset);
        }

        /// <summary>
        /// 释放已加载的资源
        /// </summary>
        /// <param name="obj">Unity对象</param>
        public void UnloadAsset(UnityObject obj)
        {
            ResourceModule.UnloadAsset(obj);
        }

        /// <summary>
        /// 清理所有资源
        /// </summary>
        public void RemoveAllAssets()
        {
            ResourceModule.RemoveAllAssets();
        }

        #endregion

        #region 场景加载/卸载相关的接口函数

        /// <summary>
        /// 同步加载场景
        /// </summary>
        /// <param name="url">资源地址(名字或路径)</param>
        /// <param name="isAdditive">是否使用叠加方式加载</param>
        public GooAsset.Scene LoadScene(string url, bool isAdditive = false)
        {
            return ResourceModule.LoadScene(url, isAdditive);
        }

        /// <summary>
        /// 异步加载场景(回调)
        /// </summary>
        /// <param name="url">资源地址(名字或路径)</param>
        /// <param name="isAdditive">是否使用叠加方式加载</param>
        /// <param name="completed">加载完成回调</param>
        public GooAsset.Scene LoadSceneAsync(string url, bool isAdditive, Action<GooAsset.Scene> completed)
        {
            return ResourceModule.LoadSceneAsync(url, isAdditive, completed);
        }

        /// <summary>
        /// 异步加载场景
        /// </summary>
        /// <param name="url">资源地址(名字或路径)</param>
        /// <param name="isAdditive">是否使用叠加方式加载</param>
        public async UniTask<GooAsset.Scene> LoadSceneAsync(string url, bool isAdditive = false)
        {
            GooAsset.Scene scene = ResourceModule.LoadSceneAsync(url, isAdditive);
            await scene.Task;
            return scene;
        }

        /// <summary>
        /// 卸载场景
        /// </summary>
        /// <param name="scene">场景对象</param>
        public void UnloadScene(GooAsset.Scene scene)
        {
            ResourceModule.UnloadScene(scene);
        }

        #endregion

        # region 原始文件加载相关的接口函数

        /// <summary>
        /// 同步加载原始流式文件(直接读取persistentDataPath中的文件, 然后可根据文件保存路径(RawFile.savePath)读取文件, 使用同步加载前需已保证文件更新)
        /// </summary>
        /// <param name="url">文件原打包路径('%ORIGINAL_RESOURCE_PATH%/......', 若为Assets外部文件则为:'Assets文件夹同级目录/...'或'Assets文件夹同级文件')</param>
        public GooAsset.RawFile LoadRawFile(string url)
        {
            return ResourceModule.LoadRawFile(url);
        }

        /// <summary>
        /// 异步加载原始流式文件(将所需的文件下载到persistentDataPath中, 完成后可根据文件保存路径(RawFile.savePath)读取文件)
        /// </summary>
        /// <param name="url">文件原打包路径('%ORIGINAL_RESOURCE_PATH%/......', 若为Assets外部文件则为:'Assets文件夹同级目录/...'或'Assets文件夹同级文件')</param>
        public GooAsset.RawFile LoadRawFileAsync(string url, Action<GooAsset.RawFile> completed)
        {
            return ResourceModule.LoadRawFileAsync(url, completed);
        }

        /// <summary>
        /// 异步加载原始流式文件(将所需的文件下载到persistentDataPath中, 完成后可根据文件保存路径(RawFile.savePath)读取文件)
        /// </summary>
        /// <param name="address">文件原打包路径('%ORIGINAL_RESOURCE_PATH%/......', 若为Assets外部文件则为:'Assets文件夹同级目录/...'或'Assets文件夹同级文件')</param>
        public async UniTask<GooAsset.RawFile> LoadRawFileAsync(string address)
        {
            GooAsset.RawFile rawFile = ResourceModule.LoadRawFileAsync(address);
            await rawFile.Task;
            return rawFile;
        }

        #endregion
    }
}
