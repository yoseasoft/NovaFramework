/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
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

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Cysharp.Threading.Tasks;

using UnityObject = UnityEngine.Object;
using UnityTransform = UnityEngine.Transform;
using UnityVector3 = UnityEngine.Vector3;
using UnityQuaternion = UnityEngine.Quaternion;

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
        /// <param name="type">资源类型</param>
        public AssetSource LoadAsset(string name, string url, Type type)
        {
            if (TryGetAssetSource(name, out AssetSource assetSource))
            {
                return assetSource;
            }

            UnityObject obj = ResourceHandler.Instance.LoadAsset(url, type);
            return CacheTargetAssetObject(name, url, type, obj);
        }

        /// <summary>
        /// 异步加载对象资源
        /// </summary>
        /// <param name="name">资源名称</param>
        /// <param name="url">资源地址</param>
        public async UniTask<AssetSource> LoadAssetAsync<T>(string name, string url) where T : UnityObject
        {
            if (TryGetAssetSource(name, out AssetSource assetSource))
            {
                Debugger.Assert(typeof(T) == assetSource.Type, NovaEngine.ErrorText.InvalidArguments);
                return assetSource;
            }

            T obj = await ResourceHandler.Instance.LoadAssetAsync<T>(url);
            return CacheTargetAssetObject(name, url, typeof(T), obj);
        }

        /// <summary>
        /// 释放已加载的对象资源
        /// </summary>
        /// <param name="name">资源名称</param>
        public void UnloadAsset(string name)
        {
            if (null == _assetSources) return;

            if (_assetSources.TryGetValue(name, out AssetSource assetSource))
            {
                assetSource.Clear();
                _assetSources.Remove(name);
            }
        }

        /// <summary>
        /// 指定资源对象的实例化函数
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="name">资源名称</param>
        /// <param name="url">资源地址</param>
        /// <param name="position">位置</param>
        /// <param name="rotation">旋转</param>
        /// <returns>返回实例化的对象实例</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Instantiate<T>(string name, string url, UnityVector3 position, UnityQuaternion rotation) where T : UnityObject
        {
            AssetSource assetSource = LoadAsset(name, url, typeof(T));
            return assetSource.Instantiate<T>(position, rotation);
        }

        /// <summary>
        /// 指定资源对象的实例化函数
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="name">资源名称</param>
        /// <param name="url">资源地址</param>
        /// <param name="position">位置</param>
        /// <param name="rotation">旋转</param>
        /// <param name="parent">父对象实例</param>
        /// <returns>返回实例化的对象实例</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Instantiate<T>(string name, string url, UnityVector3 position, UnityQuaternion rotation, UnityTransform parent) where T : UnityObject
        {
            AssetSource assetSource = LoadAsset(name, url, typeof(T));
            return assetSource.Instantiate<T>(position, rotation, parent);
        }

        /// <summary>
        /// 指定资源对象的实例化函数
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="name">资源名称</param>
        /// <param name="url">资源地址</param>
        /// <param name="parent">父对象实例</param>
        /// <returns>返回实例化的对象实例</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Instantiate<T>(string name, string url, UnityTransform parent) where T : UnityObject
        {
            AssetSource assetSource = LoadAsset(name, url, typeof(T));
            return assetSource.Instantiate<T>(parent);
        }

        /// <summary>
        /// 指定资源对象的实例化函数
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
            AssetSource assetSource = await LoadAssetAsync<T>(name, url);
            return assetSource.Instantiate<T>(position, rotation);
        }

        /// <summary>
        /// 指定资源对象的实例化函数
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="name">资源名称</param>
        /// <param name="url">资源地址</param>
        /// <param name="position">位置</param>
        /// <param name="rotation">旋转</param>
        /// <param name="parent">父对象实例</param>
        /// <returns>返回实例化的对象实例</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public async UniTask<T> InstantiateAsync<T>(string name, string url, UnityVector3 position, UnityQuaternion rotation, UnityTransform parent) where T : UnityObject
        {
            AssetSource assetSource = await LoadAssetAsync<T>(name, url);
            return assetSource.Instantiate<T>(position, rotation, parent);
        }

        /// <summary>
        /// 指定资源对象的实例化函数
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="name">资源名称</param>
        /// <param name="url">资源地址</param>
        /// <param name="parent">父对象实例</param>
        /// <returns>返回实例化的对象实例</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public async UniTask<T> InstantiateAsync<T>(string name, string url, UnityTransform parent) where T : UnityObject
        {
            AssetSource assetSource = await LoadAssetAsync<T>(name, url);
            return assetSource.Instantiate<T>(parent);
        }

        /// <summary>
        /// 销毁场景对象实例
        /// </summary>
        /// <param name="obj">场景对象实例</param>
        public void DestroyObject(UnityObject obj)
        {
            AssetSource assetSource = null;
            foreach (KeyValuePair<string, AssetSource> kvp in _assetSources)
            {
                if (kvp.Value.ContainsObject(obj))
                {
                    assetSource = kvp.Value;
                    break;
                }
            }

            if (null == assetSource)
            {
                Debugger.Warn(LogGroupTag.Bean, "");
                return;
            }

            assetSource.DestroyObject(obj);
        }

        /// <summary>
        /// 通过资源名称在当前缓存容器中查找对应的资源对象实例
        /// </summary>
        /// <param name="name">资源名称</param>
        /// <param name="obj">资源对象</param>
        /// <returns>若存在指定的资源对象则返回true，否则返回false</returns>
        private bool TryGetAssetSource(string name, out AssetSource assetSource)
        {
            if (null == _assetSources)
            {
                _assetSources = new Dictionary<string, AssetSource>();
                assetSource = null;
                return false;
            }

            return _assetSources.TryGetValue(name, out assetSource);
        }

        /// <summary>
        /// 缓存目标资源对象
        /// </summary>
        /// <param name="name">资源名称</param>
        /// <param name="url">资源地址</param>
        /// <param name="type">资源类型</param>
        /// <param name="obj">Unity资源对象</param>
        /// <returns>返回缓存的目标资源对象实例</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private AssetSource CacheTargetAssetObject(string name, string url, Type type, UnityObject obj)
        {
            Debugger.Assert(false == _assetSources.ContainsKey(name), NovaEngine.ErrorText.InvalidArguments);

            AssetSource assetSource = new AssetSource(name, url, type, obj);
            _assetSources.Add(name, assetSource);

            return assetSource;
        }

        #endregion
    }
}
