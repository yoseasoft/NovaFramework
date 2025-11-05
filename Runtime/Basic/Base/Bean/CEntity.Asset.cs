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

using Cysharp.Threading.Tasks;

using SystemType = System.Type;
using UnityObject = UnityEngine.Object;
using UnityTransform = UnityEngine.Transform;
using UnityVector3 = UnityEngine.Vector3;
using UnityQuaternion = UnityEngine.Quaternion;

namespace GameEngine
{
    /// <summary>
    /// 基于ECS模式的实体对象封装类，该类定义实体对象的常用接口及基础调度逻辑
    /// </summary>
    public abstract partial class CEntity
    {
        private AssetLoader _assetLoader;

        /// <summary>
        /// 实体对象的资源加载器初始化回调接口
        /// </summary>
        private void OnAssetLoaderInitialize()
        {
            _assetLoader = new AssetLoader(this);
        }

        /// <summary>
        /// 实体对象的资源加载器清理回调接口
        /// </summary>
        private void OnAssetLoaderCleanup()
        {
            if (null != _assetLoader)
            {
                _assetLoader.Clear();
                _assetLoader = null;
            }
        }

        #region 实体对象资源加载/卸载操作相关的函数接口

        /// <summary>
        /// 同步加载对象资源
        /// </summary>
        /// <param name="name">资源名称</param>
        /// <param name="url">资源地址</param>
        /// <param name="type">资源类型</param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public UnityObject LoadAsset(string name, string url, SystemType type)
        {
            AssetSource assetSource = _assetLoader.LoadAsset(name, url, type);
            return assetSource.Original;
        }

        /// <summary>
        /// 异步加载对象资源
        /// </summary>
        /// <param name="name">资源名称</param>
        /// <param name="url">资源地址</param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public async UniTask<T> LoadAssetAsync<T>(string name, string url) where T : UnityObject
        {
            AssetSource assetSource = await _assetLoader.LoadAssetAsync<T>(name, url);
            return assetSource.Original as T;
        }

        /// <summary>
        /// 释放已加载的对象资源
        /// </summary>
        /// <param name="name">资源名称</param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public void UnloadAsset(string name)
        {
            _assetLoader.UnloadAsset(name);
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
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public T Instantiate<T>(string name, string url, UnityVector3 position, UnityQuaternion rotation) where T : UnityObject
        {
            return _assetLoader.Instantiate<T>(name, url, position, rotation);
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
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public T Instantiate<T>(string name, string url, UnityVector3 position, UnityQuaternion rotation, UnityTransform parent) where T : UnityObject
        {
            return _assetLoader.Instantiate<T>(name, url, position, rotation, parent);
        }

        /// <summary>
        /// 指定资源对象的实例化函数
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="name">资源名称</param>
        /// <param name="url">资源地址</param>
        /// <param name="parent">父对象实例</param>
        /// <returns>返回实例化的对象实例</returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public T Instantiate<T>(string name, string url, UnityTransform parent) where T : UnityObject
        {
            return _assetLoader.Instantiate<T>(name, url, parent);
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
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public async UniTask<T> InstantiateAsync<T>(string name, string url, UnityVector3 position, UnityQuaternion rotation) where T : UnityObject
        {
            return await _assetLoader.InstantiateAsync<T>(name, url, position, rotation);
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
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public async UniTask<T> InstantiateAsync<T>(string name, string url, UnityVector3 position, UnityQuaternion rotation, UnityTransform parent) where T : UnityObject
        {
            return await _assetLoader.InstantiateAsync<T>(name, url, position, rotation, parent);
        }

        /// <summary>
        /// 指定资源对象的实例化函数
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="name">资源名称</param>
        /// <param name="url">资源地址</param>
        /// <param name="parent">父对象实例</param>
        /// <returns>返回实例化的对象实例</returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public async UniTask<T> InstantiateAsync<T>(string name, string url, UnityTransform parent) where T : UnityObject
        {
            return await _assetLoader.InstantiateAsync<T>(name, url, parent);
        }

        /// <summary>
        /// 销毁场景对象实例
        /// </summary>
        /// <param name="obj">场景对象实例</param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public void DestroyObject(UnityObject obj)
        {
            _assetLoader.DestroyObject(obj);
        }

        #endregion
    }
}
