/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
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

using UnityEngine.Scripting;

using Cysharp.Threading.Tasks;
using NovaFramework.AssetLoader;

using UnityVector3 = UnityEngine.Vector3;
using UnityQuaternion = UnityEngine.Quaternion;
using UnityObject = UnityEngine.Object;
using UnityGameObject = UnityEngine.GameObject;
using UnityTransform = UnityEngine.Transform;

namespace GameEngine
{
    /// 资源模块封装的句柄对象类
    public sealed partial class ResourceHandler
    {
        /// <summary>
        /// 实例化对象资源句柄缓存容器
        /// </summary>
        private IDictionary<string, IAssetHandler> _instantiateHandlers;
        /// <summary>
        /// 实例化对象列表缓存容器
        /// </summary>
        private IDictionary<string, IList<UnityGameObject>> _instantiateGameObjects;

        /// <summary>
        /// 资源对象实例化处理初始化回调函数
        /// </summary>
        [Preserve]
        [OnSubmoduleInitCallback]
        private void OnAssetObjectInstantiatingInitialize()
        {
            // 初始化实例化对象相关管理容器
            _instantiateHandlers = new Dictionary<string, IAssetHandler>();
            _instantiateGameObjects = new Dictionary<string, IList<UnityGameObject>>();
        }

        /// <summary>
        /// 资源对象实例化处理清理回调函数
        /// </summary>
        [Preserve]
        [OnSubmoduleCleanupCallback]
        private void OnAssetObjectInstantiatingCleanup()
        {
            // 清理实例化对象管理容器
            ReleaseAllInstantiateAssetObjects();

            _instantiateHandlers = null;
            _instantiateHandlers = null;
        }

        /// <summary>
        /// 资源对象实例化处理重载回调函数
        /// </summary>
        [Preserve]
        [OnSubmoduleReloadCallback]
        private void OnAssetObjectInstantiatingReload()
        {
        }

        /// <summary>
        /// 同步初始化游戏对象
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public UnityGameObject InstantiateAssetSync(string name, string url)
        {
            return InstantiateAssetSyncInternal(name, url, false, UnityVector3.zero, UnityQuaternion.identity, null, false);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public UnityGameObject InstantiateAssetSync(string name, string url, UnityTransform parent)
        {
            return InstantiateAssetSyncInternal(name, url, false, UnityVector3.zero, UnityQuaternion.identity, parent, false);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public UnityGameObject InstantiateAssetSync(string name, string url, UnityTransform parent, bool worldPositionStays)
        {
            return InstantiateAssetSyncInternal(name, url, false, UnityVector3.zero, UnityQuaternion.identity, parent, worldPositionStays);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public UnityGameObject InstantiateAssetSync(string name, string url, UnityVector3 position, UnityQuaternion rotation)
        {
            return InstantiateAssetSyncInternal(name, url, true, position, rotation, null, false);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public UnityGameObject InstantiateAssetSync(string name, string url, UnityVector3 position, UnityQuaternion rotation, UnityTransform parent)
        {
            return InstantiateAssetSyncInternal(name, url, true, position, rotation, parent, false);
        }

        /// <summary>
        /// 异步初始化游戏对象
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public async UniTask<UnityGameObject> InstantiateAssetAsync(string name, string url)
        {
            return await InstantiateAssetAsyncInternal(name, url, false, UnityVector3.zero, UnityQuaternion.identity, null, false);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public async UniTask<UnityGameObject> InstantiateAssetAsync(string name, string url, UnityTransform parent)
        {
            return await InstantiateAssetAsyncInternal(name, url, false, UnityVector3.zero, UnityQuaternion.identity, parent, false);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public async UniTask<UnityGameObject> InstantiateAssetAsync(string name, string url, UnityTransform parent, bool worldPositionStays)
        {
            return await InstantiateAssetAsyncInternal(name, url, false, UnityVector3.zero, UnityQuaternion.identity, parent, worldPositionStays);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public async UniTask<UnityGameObject> InstantiateAssetAsync(string name, string url, UnityVector3 position, UnityQuaternion rotation)
        {
            return await InstantiateAssetAsyncInternal(name, url, true, position, rotation, null, false);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public async UniTask<UnityGameObject> InstantiateAssetAsync(string name, string url, UnityVector3 position, UnityQuaternion rotation, UnityTransform parent)
        {
            return await InstantiateAssetAsyncInternal(name, url, true, position, rotation, parent, false);
        }

        private UnityGameObject InstantiateAssetSyncInternal(string name, string url, bool setPositionAndRotation, UnityVector3 position, UnityQuaternion rotation, UnityTransform parent, bool worldPositionStays)
        {
            if (false == _instantiateHandlers.TryGetValue(name, out IAssetHandler assetHandler))
            {
                assetHandler = ResourceModule.LoadAssetSync(url);
                if (null == assetHandler)
                {
                    Debugger.Warn("Could not found any asset with target url '{%s}', instantiated game object failed.", url);
                    return null;
                }

                _instantiateHandlers.Add(name, assetHandler);
            }

            return InstantiateAssetObjectInternal(name, assetHandler.AssetObject, setPositionAndRotation, position, rotation, parent, worldPositionStays);
        }

        private async UniTask<UnityGameObject> InstantiateAssetAsyncInternal(string name, string url, bool setPositionAndRotation, UnityVector3 position, UnityQuaternion rotation, UnityTransform parent, bool worldPositionStays)
        {
            if (false == _instantiateHandlers.TryGetValue(name, out IAssetHandler assetHandler))
            {
                assetHandler = ResourceModule.LoadAssetAsync(url);
                if (null == assetHandler)
                {
                    Debugger.Warn("Could not found any asset with target url '{%s}', instantiated game object failed.", url);
                    return null;
                }

                _instantiateHandlers.Add(name, assetHandler);

                // 等待资源加载完成
                await assetHandler.Task;
            }

            return InstantiateAssetObjectInternal(name, assetHandler.AssetObject, setPositionAndRotation, position, rotation, parent, worldPositionStays);
        }

        private UnityGameObject InstantiateAssetObjectInternal(string name, UnityObject assetObject, bool setPositionAndRotation, UnityVector3 position, UnityQuaternion rotation, UnityTransform parent, bool worldPositionStays)
        {
            if (null == assetObject)
                return null;

            UnityGameObject go;

            if (setPositionAndRotation)
            {
                if (null != parent)
                    go = UnityObject.Instantiate(assetObject as UnityGameObject, position, rotation, parent);
                else
                    go = UnityObject.Instantiate(assetObject as UnityGameObject, position, rotation);
            }
            else
            {
                if (null != parent)
                    go = UnityObject.Instantiate(assetObject as UnityGameObject, parent, worldPositionStays);
                else
                    go = UnityObject.Instantiate(assetObject as UnityGameObject);
            }

            if (false == _instantiateGameObjects.TryGetValue(name, out IList<UnityGameObject> list))
            {
                list = new List<UnityGameObject>();
                _instantiateGameObjects.Add(name, list);
            }

            list.Add(go);

            return go;
        }

        /// <summary>
        /// 销毁指定名称对应的资源句柄及其加载的所有对象实例
        /// </summary>
        /// <param name="name">对象名称</param>
        public void DestroyAssetObject(string name)
        {
            if (_instantiateHandlers.TryGetValue(name, out IAssetHandler assetHandler))
            {
                if (_instantiateGameObjects.TryGetValue(name, out IList<UnityGameObject> list))
                {
                    for (int n = 0; n < list.Count; ++n)
                    {
                        UnityObject.Destroy(list[n]);
                    }

                    list.Clear();
                    _instantiateGameObjects.Remove(name);
                }

                assetHandler.Release();
                _instantiateHandlers.Remove(name);
            }
        }

        /// <summary>
        /// 销毁已加载的目标对象实例
        /// </summary>
        /// <param name="name">对象名称</param>
        /// <param name="gameObject">对象实例</param>
        public void DestroyAssetObject(string name, UnityGameObject gameObject)
        {
            if (false == _instantiateGameObjects.TryGetValue(name, out IList<UnityGameObject> list) ||
                false == list.Contains(gameObject))
            {
                Debugger.Warn("Could not found any record of target game object instance from instantiate list, destroyed it failed.");
                return;
            }

            UnityObject.Destroy(gameObject);

            list.Remove(gameObject);
        }

        /// <summary>
        /// 销毁所有实例化的资源对象，同时释放其对应的资源句柄
        /// </summary>
        private void ReleaseAllInstantiateAssetObjects()
        {
            foreach (KeyValuePair<string, IAssetHandler> kvp in _instantiateHandlers)
            {
                if (_instantiateGameObjects.TryGetValue(kvp.Key, out IList<UnityGameObject> list))
                {
                    for (int n = 0; n < list.Count; ++n)
                    {
                        UnityObject.Destroy(list[n]);
                    }
                }

                kvp.Value.Release();
            }

            _instantiateHandlers.Clear();
            _instantiateGameObjects.Clear();
        }
    }
}
