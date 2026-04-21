/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyright (C) 2024 - 2025, Hurley, Independent Studio.
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

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Cysharp.Threading.Tasks;

using NovaFramework.AssetLoader;

using UnityVector3 = UnityEngine.Vector3;
using UnityQuaternion = UnityEngine.Quaternion;
using UnityObject = UnityEngine.Object;
using UnityTransform = UnityEngine.Transform;

namespace GameEngine
{
    /// <summary>
    /// 基于引擎层的资源加载接口封装的资源加载器对象类<br/>
    /// 该对象类仅服务于实体对象类，并为其提供所有的资源加载/卸载的管理接口<br/>
    /// <br/>
    /// 加载器内部对所属的实体对象运行过程中加载的所有资产进行管理，并确保在实体对象销毁时，其内部所有资产都被正确卸载
    /// </summary>
    internal sealed class AssetLoader
    {
        /// <summary>
        /// 对象实例
        /// </summary>
        private readonly CEntity _entity;

        /// <summary>
        /// 对象资产数据集合
        /// </summary>
        private IDictionary<string, AssetSource> _assetSources;

        /// <summary>
        /// 获取所属对象实例
        /// </summary>
        public CEntity Entity => _entity;

        public AssetLoader(CEntity entity)
        {
            _entity = entity;
            _assetSources = new Dictionary<string, AssetSource>();
        }

        ~AssetLoader()
        {
            Clear();
        }

        /// <summary>
        /// 清理对象资产数据
        /// </summary>
        public void Clear()
        {
            if (null == _assetSources)
            {
                return;
            }

            foreach (KeyValuePair<string, AssetSource> kvp in _assetSources)
            {
                kvp.Value.Clear();
            }

            _assetSources.Clear();
            _assetSources = null;
        }

        #region 对象资源加载/卸载管理相关的接口函数

        /// <summary>
        /// 同步加载对象资源
        /// </summary>
        /// <param name="name">资源名称</param>
        /// <param name="url">资源地址</param>
        /// <returns>返回对象资源数据实例</returns>
        public AssetSource LoadSync(string name, string url)
        {
            if (TryGetAssetSource(name, out AssetSource assetSource))
            {
                return assetSource;
            }

            IAssetHandler assetHandler = ResourceHandler.Instance.LoadAssetSync(url);
            return TryCacheTargetAssetObject(name, assetHandler);
        }

        /// <summary>
        /// 同步加载对象资源
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="name">资源名称</param>
        /// <param name="url">资源地址</param>
        /// <returns>返回对象资源数据实例</returns>
        public AssetSource LoadSync<T>(string name, string url) where T : UnityObject
        {
            if (TryGetAssetSource(name, out AssetSource assetSource))
            {
                Debugger.Assert(typeof(T) == assetSource.Type, NovaEngine.ErrorText.InvalidArguments);
                return assetSource;
            }

            IAssetHandler assetHandler = ResourceHandler.Instance.LoadAssetSync<T>(url);
            return TryCacheTargetAssetObject(name, assetHandler);
        }

        /// <summary>
        /// 同步加载对象资源
        /// </summary>
        /// <param name="name">资源名称</param>
        /// <param name="url">资源地址</param>
        /// <param name="type">资源类型</param>
        /// <returns>返回对象资源数据实例</returns>
        public AssetSource LoadSync(string name, string url, Type type)
        {
            if (TryGetAssetSource(name, out AssetSource assetSource))
            {
                Debugger.Assert(type == assetSource.Type, NovaEngine.ErrorText.InvalidArguments);
                return assetSource;
            }

            IAssetHandler assetHandler = ResourceHandler.Instance.LoadAssetSync(url, type);
            return TryCacheTargetAssetObject(name, assetHandler);
        }

        /// <summary>
        /// 异步加载对象资源
        /// </summary>
        /// <param name="name">资源名称</param>
        /// <param name="url">资源地址</param>
        /// <returns>返回对象资源数据实例</returns>
        public AssetSource LoadAsync(string name, string url)
        {
            if (TryGetAssetSource(name, out AssetSource assetSource))
            {
                return assetSource;
            }

            IAssetHandler assetHandler = ResourceHandler.Instance.LoadAssetAsync(url);
            return TryCacheTargetAssetObject(name, assetHandler);
        }

        /// <summary>
        /// 异步加载对象资源
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="name">资源名称</param>
        /// <param name="url">资源地址</param>
        /// <returns>返回对象资源数据实例</returns>
        public AssetSource LoadAsync<T>(string name, string url) where T : UnityObject
        {
            if (TryGetAssetSource(name, out AssetSource assetSource))
            {
                Debugger.Assert(typeof(T) == assetSource.Type, NovaEngine.ErrorText.InvalidArguments);
                return assetSource;
            }

            IAssetHandler assetHandler = ResourceHandler.Instance.LoadAssetAsync<T>(url);
            return TryCacheTargetAssetObject(name, assetHandler);
        }

        /// <summary>
        /// 异步加载对象资源
        /// </summary>
        /// <param name="name">资源名称</param>
        /// <param name="url">资源地址</param>
        /// <param name="type">资源类型</param>
        /// <returns>返回对象资源数据实例</returns>
        public AssetSource LoadAsync(string name, string url, Type type)
        {
            if (TryGetAssetSource(name, out AssetSource assetSource))
            {
                Debugger.Assert(type == assetSource.Type, NovaEngine.ErrorText.InvalidArguments);
                return assetSource;
            }

            IAssetHandler assetHandler = ResourceHandler.Instance.LoadAssetAsync(url, type);
            return TryCacheTargetAssetObject(name, assetHandler);
        }

        /// <summary>
        /// 释放已加载的对象资源
        /// </summary>
        /// <param name="name">资源名称</param>
        public void Unload(string name)
        {
            if (null == _assetSources) return;

            if (_assetSources.TryGetValue(name, out AssetSource assetSource))
            {
                assetSource.Clear();
                _assetSources.Remove(name);
            }
        }

        /// <summary>
        /// 同步进行资源对象的实例化函数
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="name">资源名称</param>
        /// <param name="url">资源地址</param>
        /// <returns>返回实例化的对象实例</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T InstantiateSync<T>(string name, string url) where T : UnityObject
        {
            AssetSource assetSource = LoadSync<T>(name, url);

            return assetSource.Instantiate<T>();
        }

        /// <summary>
        /// 同步进行资源对象的实例化函数
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="name">资源名称</param>
        /// <param name="url">资源地址</param>
        /// <param name="parent">父节点对象</param>
        /// <returns>返回实例化的对象实例</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T InstantiateSync<T>(string name, string url, UnityTransform parent) where T : UnityObject
        {
            AssetSource assetSource = LoadSync<T>(name, url);

            return assetSource.Instantiate<T>(parent);
        }

        /// <summary>
        /// 同步进行资源对象的实例化函数
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="name">资源名称</param>
        /// <param name="url">资源地址</param>
        /// <param name="parent">父节点对象</param>
        /// <param name="worldPositionStays">使用世界坐标</param>
        /// <returns>返回实例化的对象实例</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T InstantiateSync<T>(string name, string url, UnityTransform parent, bool worldPositionStays) where T : UnityObject
        {
            AssetSource assetSource = LoadSync<T>(name, url);

            return assetSource.Instantiate<T>(parent, worldPositionStays);
        }

        /// <summary>
        /// 同步进行资源对象的实例化函数
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="name">资源名称</param>
        /// <param name="url">资源地址</param>
        /// <param name="position">位置</param>
        /// <param name="rotation">旋转</param>
        /// <returns>返回实例化的对象实例</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T InstantiateSync<T>(string name, string url, UnityVector3 position, UnityQuaternion rotation) where T : UnityObject
        {
            AssetSource assetSource = LoadSync<T>(name, url);

            return assetSource.Instantiate<T>(position, rotation);
        }

        /// <summary>
        /// 同步进行资源对象的实例化函数
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="name">资源名称</param>
        /// <param name="url">资源地址</param>
        /// <param name="position">位置</param>
        /// <param name="rotation">旋转</param>
        /// <param name="parent">父节点对象</param>
        /// <returns>返回实例化的对象实例</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T InstantiateSync<T>(string name, string url, UnityVector3 position, UnityQuaternion rotation, UnityTransform parent) where T : UnityObject
        {
            AssetSource assetSource = LoadSync<T>(name, url);

            return assetSource.Instantiate<T>(position, rotation, parent);
        }

        /// <summary>
        /// 异步进行资源对象的实例化函数
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="name">资源名称</param>
        /// <param name="url">资源地址</param>
        /// <returns>返回实例化的对象实例</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public async UniTask<T> InstantiateAsync<T>(string name, string url) where T : UnityObject
        {
            AssetSource assetSource = LoadAsync<T>(name, url);
            await assetSource.Task;

            return assetSource.Instantiate<T>();
        }

        /// <summary>
        /// 异步进行资源对象的实例化函数
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="name">资源名称</param>
        /// <param name="url">资源地址</param>
        /// <param name="parent">父节点对象</param>
        /// <returns>返回实例化的对象实例</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public async UniTask<T> InstantiateAsync<T>(string name, string url, UnityTransform parent) where T : UnityObject
        {
            AssetSource assetSource = LoadSync<T>(name, url);
            await assetSource.Task;

            return assetSource.Instantiate<T>(parent);
        }

        /// <summary>
        /// 异步进行资源对象的实例化函数
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="name">资源名称</param>
        /// <param name="url">资源地址</param>
        /// <param name="parent">父节点对象</param>
        /// <param name="worldPositionStays">使用世界坐标</param>
        /// <returns>返回实例化的对象实例</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public async UniTask<T> InstantiateAsync<T>(string name, string url, UnityTransform parent, bool worldPositionStays) where T : UnityObject
        {
            AssetSource assetSource = LoadSync<T>(name, url);
            await assetSource.Task;

            return assetSource.Instantiate<T>(parent, worldPositionStays);
        }

        /// <summary>
        /// 异步进行资源对象的实例化函数
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="name">资源名称</param>
        /// <param name="url">资源地址</param>
        /// <param name="position">位置</param>
        /// <param name="rotation">旋转</param>
        /// <returns>返回实例化的对象实例</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public async UniTask<T> InstantiateAsync<T>(string name, string url, UnityVector3 position, UnityQuaternion rotation) where T : UnityObject
        {
            AssetSource assetSource = LoadSync<T>(name, url);
            await assetSource.Task;

            return assetSource.Instantiate<T>(position, rotation);
        }

        /// <summary>
        /// 异步进行资源对象的实例化函数
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="name">资源名称</param>
        /// <param name="url">资源地址</param>
        /// <param name="position">位置</param>
        /// <param name="rotation">旋转</param>
        /// <param name="parent">父节点对象</param>
        /// <returns>返回实例化的对象实例</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public async UniTask<T> InstantiateAsync<T>(string name, string url, UnityVector3 position, UnityQuaternion rotation, UnityTransform parent) where T : UnityObject
        {
            AssetSource assetSource = LoadSync<T>(name, url);
            await assetSource.Task;

            return assetSource.Instantiate<T>(position, rotation, parent);
        }

        /// <summary>
        /// 销毁场景对象实例
        /// </summary>
        /// <param name="obj">场景对象实例</param>
        public void DestroyObject(UnityObject obj)
        {
            foreach (KeyValuePair<string, AssetSource> kvp in _assetSources)
            {
                if (kvp.Value.ContainsObject(obj))
                {
                    kvp.Value.DestroyObject(obj);
                    return;
                }
            }

            Debugger.Warn(LogGroupTag.Bean, "Could not found any asset source with target instantiation object '{%t}', destroyed it failed.", obj);
        }

        /// <summary>
        /// 通过资源名称在当前缓存容器中查找对应的资源对象实例
        /// </summary>
        /// <param name="name">资源名称</param>
        /// <param name="assetSource">资源对象</param>
        /// <returns>若存在指定的资源对象则返回true，否则返回false</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool TryGetAssetSource(string name, out AssetSource assetSource)
        {
            _assetSources ??= new Dictionary<string, AssetSource>();

            return _assetSources.TryGetValue(name, out assetSource);
        }

        /// <summary>
        /// 缓存目标资源对象
        /// </summary>
        /// <param name="name">资源名称</param>
        /// <param name="assetHandler">资源装载句柄</param>
        /// <returns>返回缓存的目标资源对象实例</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private AssetSource TryCacheTargetAssetObject(string name, IAssetHandler assetHandler)
        {
            Debugger.IsFalse(_assetSources.ContainsKey(name));

            AssetSource assetSource = new AssetSource(name, assetHandler);
            _assetSources.Add(name, assetSource);

            return assetSource;
        }

        #endregion
    }
}
