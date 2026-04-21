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

using UnityVector3 = UnityEngine.Vector3;
using UnityQuaternion = UnityEngine.Quaternion;
using UnityObject = UnityEngine.Object;
using UnityTransform = UnityEngine.Transform;

namespace GameEngine
{
    /// 基于ECS模式的组件对象封装类
    public abstract partial class CComponent
    {
        /// <summary>
        /// 实例化对象列表
        /// </summary>
        private IList<UnityObject> _instantiateObjects;

        /// <summary>
        /// 组件对象的资源收集初始化回调接口
        /// </summary>
        private void OnAssetCollectInitialize()
        {
        }

        /// <summary>
        /// 组件对象的资源收集清理回调接口
        /// </summary>
        private void OnAssetCollectCleanup()
        {
            if (null != _instantiateObjects)
            {
                while (_instantiateObjects.Count > 0)
                {
                    DestroyObject(_instantiateObjects[0]);
                }

                _instantiateObjects = null;
            }
        }

        /// <summary>
        /// 缓存实例化的Unity场景对象
        /// </summary>
        /// <param name="obj">对象实例</param>
        private void CacheInstantiateObject(UnityObject obj)
        {
            _instantiateObjects ??= new List<UnityObject>();

            Asserter.IsNotNull(obj);
            Asserter.IsFalse(_instantiateObjects.Contains(obj));

            _instantiateObjects.Add(obj);
        }

        #region 组件对象资源加载/卸载操作相关的函数接口

        /// <summary>
        /// 同步加载对象资源
        /// </summary>
        /// <param name="name">资源名称</param>
        /// <param name="url">资源地址</param>
        /// <returns>返回对象资源数据实例</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public AssetSource LoadAssetSync(string name, string url)
        {
            Asserter.IsNotNull(Entity);

            return Entity.LoadAssetSync(name, url);
        }

        /// <summary>
        /// 同步加载对象资源
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="name">资源名称</param>
        /// <param name="url">资源地址</param>
        /// <returns>返回对象资源数据实例</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public AssetSource LoadAssetSync<T>(string name, string url) where T : UnityObject
        {
            Asserter.IsNotNull(Entity);

            return Entity.LoadAssetSync<T>(name, url);
        }

        /// <summary>
        /// 同步加载对象资源
        /// </summary>
        /// <param name="name">资源名称</param>
        /// <param name="url">资源地址</param>
        /// <param name="type">资源类型</param>
        /// <returns>返回对象资源数据实例</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public AssetSource LoadAssetSync(string name, string url, Type type)
        {
            Asserter.IsNotNull(Entity);

            return Entity.LoadAssetSync(name, url, type);
        }

        /// <summary>
        /// 异步加载对象资源
        /// </summary>
        /// <param name="name">资源名称</param>
        /// <param name="url">资源地址</param>
        /// <returns>返回对象资源数据实例</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public AssetSource LoadAssetAsync(string name, string url)
        {
            Asserter.IsNotNull(Entity);

            return Entity.LoadAssetAsync(name, url);
        }

        /// <summary>
        /// 异步加载对象资源
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="name">资源名称</param>
        /// <param name="url">资源地址</param>
        /// <returns>返回对象资源数据实例</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public AssetSource LoadAssetAsync<T>(string name, string url) where T : UnityObject
        {
            Asserter.IsNotNull(Entity);

            return Entity.LoadAssetAsync<T>(name, url);
        }

        /// <summary>
        /// 异步加载对象资源
        /// </summary>
        /// <param name="name">资源名称</param>
        /// <param name="url">资源地址</param>
        /// <param name="type">资源类型</param>
        /// <returns>返回对象资源数据实例</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public AssetSource LoadAssetAsync(string name, string url, Type type)
        {
            Asserter.IsNotNull(Entity);

            return Entity.LoadAssetAsync(name, url, type);
        }

        /// <summary>
        /// 释放已加载的对象资源
        /// </summary>
        /// <param name="name">资源名称</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void UnloadAsset(string name)
        {
            Asserter.IsNotNull(Entity);

            Entity.UnloadAsset(name);
        }

        /// <summary>
        /// 指定资源对象的实例化函数
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="name">资源名称</param>
        /// <param name="url">资源地址</param>
        /// <returns>返回实例化的对象实例</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T InstantiateSync<T>(string name, string url) where T : UnityObject
        {
            Asserter.IsNotNull(Entity);

            T obj = Entity.InstantiateSync<T>(name, url);

            CacheInstantiateObject(obj);

            return obj;
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
        public T InstantiateSync<T>(string name, string url, UnityTransform parent) where T : UnityObject
        {
            Asserter.IsNotNull(Entity);

            T obj = Entity.InstantiateSync<T>(name, url, parent);

            CacheInstantiateObject(obj);

            return obj;
        }

        /// <summary>
        /// 指定资源对象的实例化函数
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="name">资源名称</param>
        /// <param name="url">资源地址</param>
        /// <param name="parent">父对象实例</param>
        /// <param name="worldPositionStays">使用世界坐标</param>
        /// <returns>返回实例化的对象实例</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T InstantiateSync<T>(string name, string url, UnityTransform parent, bool worldPositionStays) where T : UnityObject
        {
            Asserter.IsNotNull(Entity);

            T obj = Entity.InstantiateSync<T>(name, url, parent, worldPositionStays);

            CacheInstantiateObject(obj);

            return obj;
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
        public T InstantiateSync<T>(string name, string url, UnityVector3 position, UnityQuaternion rotation) where T : UnityObject
        {
            Asserter.IsNotNull(Entity);

            T obj = Entity.InstantiateSync<T>(name, url, position, rotation);

            CacheInstantiateObject(obj);

            return obj;
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
        public T InstantiateSync<T>(string name, string url, UnityVector3 position, UnityQuaternion rotation, UnityTransform parent) where T : UnityObject
        {
            Asserter.IsNotNull(Entity);

            T obj = Entity.InstantiateSync<T>(name, url, position, rotation, parent);

            CacheInstantiateObject(obj);

            return obj;
        }

        /// <summary>
        /// 指定资源对象的实例化函数
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="name">资源名称</param>
        /// <param name="url">资源地址</param>
        /// <returns>返回实例化的对象实例</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public async UniTask<T> InstantiateAsync<T>(string name, string url) where T : UnityObject
        {
            Asserter.IsNotNull(Entity);

            T obj = await Entity.InstantiateAsync<T>(name, url);

            CacheInstantiateObject(obj);

            return obj;
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
            Asserter.IsNotNull(Entity);

            T obj = await Entity.InstantiateAsync<T>(name, url, parent);

            CacheInstantiateObject(obj);

            return obj;
        }

        /// <summary>
        /// 指定资源对象的实例化函数
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="name">资源名称</param>
        /// <param name="url">资源地址</param>
        /// <param name="parent">父对象实例</param>
        /// <param name="worldPositionStays">使用世界坐标</param>
        /// <returns>返回实例化的对象实例</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public async UniTask<T> InstantiateAsync<T>(string name, string url, UnityTransform parent, bool worldPositionStays) where T : UnityObject
        {
            Asserter.IsNotNull(Entity);

            T obj = await Entity.InstantiateAsync<T>(name, url, parent, worldPositionStays);

            CacheInstantiateObject(obj);

            return obj;
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
            Asserter.IsNotNull(Entity);

            T obj = await Entity.InstantiateAsync<T>(name, url, position, rotation);

            CacheInstantiateObject(obj);

            return obj;
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
            Asserter.IsNotNull(Entity);

            T obj = await Entity.InstantiateAsync<T>(name, url, position, rotation, parent);

            CacheInstantiateObject(obj);

            return obj;
        }

        /// <summary>
        /// 销毁场景对象实例
        /// </summary>
        /// <param name="obj">场景对象实例</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DestroyObject(UnityObject obj)
        {
            Asserter.IsNotNull(Entity);

            if (null != _instantiateObjects)
            {
                if (_instantiateObjects.Contains(obj))
                {
                    _instantiateObjects.Remove(obj);
                }
                else
                {
                    Debugger.Warn(LogGroupTag.Bean, "当前的组件对象‘{%t}’不存在指定场景对象实例‘{%t}’的实例化记录，销毁组件内部资源记录失败！", BeanType, obj);
                }
            }

            Entity.DestroyObject(obj);
        }

        #endregion
    }
}
