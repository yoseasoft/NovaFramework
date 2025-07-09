/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
/// Copyright (C) 2017 - 2020, Shanghai Tommon Network Technology Co., Ltd.
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

using SystemType = System.Type;

using UnityObject = UnityEngine.Object;

using Asset = AssetModule.Asset;
using AssetManagement = AssetModule.AssetManagement;
using ObjectAssetMapping = System.Collections.Generic.Dictionary<UnityEngine.Object, AssetModule.Asset>;

namespace NovaEngine
{
    /// <summary>
    /// 资源管理器，统一处理打包资源的加载读取，缓存释放等功能，为其提供操作接口
    /// </summary>
    public sealed partial class ResourceModule : ModuleObject
    {
        /// <summary>
        /// 资源模块事件类型
        /// </summary>
        public override int EventType => (int) EEventType.Resource;

        /// <summary>
        /// 管理器对象初始化接口函数
        /// </summary>
        protected override void OnInitialize()
        {
        }

        /// <summary>
        /// 管理器对象清理接口函数
        /// </summary>
        protected override void OnCleanup()
        {
            RemoveAllAssets();
        }

        /// <summary>
        /// 管理器对象初始启动接口
        /// </summary>
        protected override void OnStartup()
        {
        }

        /// <summary>
        /// 管理器对象结束关闭接口
        /// </summary>
        protected override void OnShutdown()
        {
        }

        /// <summary>
        /// 管理器对象垃圾回收调度接口
        /// </summary>
        protected override void OnDump()
        {
        }

        /// <summary>
        /// 管理器内部事务更新接口
        /// </summary>
        protected override void OnUpdate()
        {
        }

        /// <summary>
        /// 管理器内部后置更新接口
        /// </summary>
        protected override void OnLateUpdate()
        {
        }

        #region 资源加载/卸载通用接口

        /// <summary>
        /// Unity对象和资源对象的对照表
        /// </summary>
        static ObjectAssetMapping m_objectAssets = new ObjectAssetMapping();

        /// <summary>
        /// 同步加载资源
        /// </summary>
        /// <param name="url">资源地址(名字或路径)</param>
        /// <param name="type">资源类型</param>
        public static UnityObject LoadAsset(string url, SystemType type)
        {
            Asset asset = AssetManagement.LoadAsset(url, type);
            UnityObject Object = asset?.result;
            if (Object != null && !m_objectAssets.ContainsKey(Object))
                m_objectAssets.Add(Object, asset);
            return Object;
        }

        /// <summary>
        /// 异步加载资源
        /// </summary>
        /// <param name="url">资源地址(名字或路径)</param>
        /// <param name="type">资源类型</param>
        /// <param name="completed">加载完成回调</param>
        public static Asset LoadAssetAsync(string url, SystemType type, System.Action<UnityObject> completed = null)
        {
            Asset asset = AssetManagement.LoadAssetAsync(url, type, a =>
            {
                if (a.result != null && !m_objectAssets.ContainsKey(a.result))
                    m_objectAssets.Add(a.result, a);

                completed?.Invoke(a.result);
            });
            return asset;
        }

        /// <summary>
        /// 释放资源(加载完成或加载中都可以使用此接口释放资源)
        /// </summary>
        /// <param name="asset">资源对象</param>
        public static void UnloadAsset(Asset asset)
        {
            if (asset.result != null)
                UnloadAsset(asset.result);
            else
                asset.Release();
        }

        /// <summary>
        /// 释放已加载的资源
        /// </summary>
        /// <param name="Object">Unity对象</param>
        public static void UnloadAsset(UnityObject Object)
        {
            if (Object == null || !m_objectAssets.TryGetValue(Object, out Asset asset))
                return;

            asset.Release();
            if (asset.reference.IsUnused)
                m_objectAssets.Remove(Object);
        }

        /// <summary>
        /// 清理所有资源
        /// </summary>
        public static void RemoveAllAssets()
        {
            foreach (Asset asset in m_objectAssets.Values)
            {
                asset.Release();
            }

            m_objectAssets.Clear();
        }

        #endregion

        #region 场景加载/卸载接口

        /// <summary>
        /// 同步加载场景
        /// </summary>
        /// <param name="url">资源地址(名字或路径)</param>
        /// <param name="isAdditive">是否使用叠加方式加载</param>
        public static AssetModule.Scene LoadScene(string url, bool isAdditive = false)
        {
            return AssetManagement.LoadScene(url, isAdditive);
        }

        /// <summary>
        /// 异步加载场景
        /// </summary>
        /// <param name="url">资源地址(名字或路径)</param>
        /// <param name="isAdditive">是否使用叠加方式加载</param>
        /// <param name="completed">加载完成回调</param>
        public static AssetModule.Scene LoadSceneAsync(string url, bool isAdditive = false, System.Action<AssetModule.Scene> completed = null)
        {
            return AssetManagement.LoadSceneAsync(url, isAdditive, completed);
        }

        /// <summary>
        /// 卸载场景
        /// </summary>
        /// <param name="scene">场景对象</param>
        public static void UnloadScene(AssetModule.Scene scene)
        {
            scene?.Release();
        }

        #endregion

        # region 原始文件加载接口

        /// <summary>
        /// 同步加载原文件(直接读取persistentDataPath中的文件, 然后可根据文件保存路径(RawFile.savePath)读取文件, 使用同步加载前需已保证文件更新)
        /// <param name="url">文件原打包路径('Assets/_Resources/......', 若为Assets外部文件则为:'Assets文件夹同级目录/...'或'Assets文件夹同级文件')</param>
        /// </summary>
        public static AssetModule.RawFile LoadRawFile(string url)
        {
            return AssetManagement.LoadRawFile(url);
        }

        /// <summary>
        /// 异步加载原文件(将所需的文件下载到persistentDataPath中, 完成后可根据文件保存路径(RawFile.savePath)读取文件)
        /// /// <param name="url">文件原打包路径('Assets/_Resources/......', 若为Assets外部文件则为:'Assets文件夹同级目录/...'或'Assets文件夹同级文件')</param>
        /// </summary>
        public static AssetModule.RawFile LoadRawFileAsync(string url, System.Action<AssetModule.RawFile> completed = null)
        {
            return AssetManagement.LoadRawFileAsync(url, completed);
        }

        #endregion
    }
}
