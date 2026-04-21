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
using System.Customize.Extension;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

using NovaFramework.AssetLoader;

using UnityVector3 = UnityEngine.Vector3;
using UnityQuaternion = UnityEngine.Quaternion;
using UnityObject = UnityEngine.Object;
using UnityTransform = UnityEngine.Transform;

namespace GameEngine
{
    /// <summary>
    /// 对实体对象内部加载的资产进行封装管理的对象类，用于管理指定类型的资产数据
    /// </summary>
    public sealed class AssetSource
    {
        /// <summary>
        /// 资产名称
        /// </summary>
        private readonly string _name;

        /// <summary>
        /// 资产装载句柄
        /// </summary>
        private IAssetHandler _assetHandler;
        /// <summary>
        /// 实例化对象实例
        /// </summary>
        private IList<UnityObject> _gameObjects;

        public string Name => _name;
        public string Url => _assetHandler?.Url;
        public Type Type => _assetHandler?.AssetType;
        public UnityObject AssetObject => _assetHandler?.AssetObject;

        public Task Task => _assetHandler?.Task;

        public AssetSource(string name, IAssetHandler assetHandler)
        {
            Debugger.IsTrue(name.IsNotNullOrEmpty());
            Debugger.IsNotNull(assetHandler);

            _name = name;
            _assetHandler = assetHandler;
            _gameObjects = new List<UnityObject>();
        }

        ~AssetSource()
        {
            Clear();
        }

        /// <summary>
        /// 清理对象资产数据
        /// </summary>
        public void Clear()
        {
            if (null != _gameObjects)
            {
                for (int n = 0; n < _gameObjects.Count; ++n)
                {
                    UnityObject.Destroy(_gameObjects[n]);
                }
                _gameObjects.Clear();
                _gameObjects = null;
            }

            if (null != _assetHandler)
            {
                _assetHandler.Release();
                _assetHandler = null;
            }
        }

        /// <summary>
        /// 创建资源对象实例
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <returns>返回资源对象实例</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Instantiate<T>() where T : UnityObject
        {
            return InstantiateObjectInternal<T>(false, UnityVector3.zero, UnityQuaternion.identity, null, false);
        }

        /// <summary>
        /// 创建资源对象实例
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="parent">父节点对象实例</param>
        /// <returns>返回资源对象实例</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Instantiate<T>(UnityTransform parent) where T : UnityObject
        {
            return InstantiateObjectInternal<T>(false, UnityVector3.zero, UnityQuaternion.identity, parent, false);
        }

        /// <summary>
        /// 创建资源对象实例
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="parent">父节点对象实例</param>
        /// <param name="worldPositionStays">使用世界坐标</param>
        /// <returns>返回资源对象实例</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Instantiate<T>(UnityTransform parent, bool worldPositionStays) where T : UnityObject
        {
            return InstantiateObjectInternal<T>(false, UnityVector3.zero, UnityQuaternion.identity, parent, worldPositionStays);
        }

        /// <summary>
        /// 创建资源对象实例
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="position">位置</param>
        /// <param name="rotation">旋转</param>
        /// <returns>返回资源对象实例</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Instantiate<T>(UnityVector3 position, UnityQuaternion rotation) where T : UnityObject
        {
            return InstantiateObjectInternal<T>(true, position, rotation, null, false);
        }

        /// <summary>
        /// 创建资源对象实例
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="position">位置</param>
        /// <param name="rotation">旋转</param>
        /// <param name="parent">父节点对象实例</param>
        /// <returns>返回资源对象实例</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Instantiate<T>(UnityVector3 position, UnityQuaternion rotation, UnityTransform parent) where T : UnityObject
        {
            return InstantiateObjectInternal<T>(true, position, rotation, parent, false);
        }

        private T InstantiateObjectInternal<T>(bool setPositionAndRotation, UnityVector3 position, UnityQuaternion rotation, UnityTransform parent, bool worldPositionStays) where T : UnityObject
        {
            Debugger.IsNotNull(AssetObject);
            Debugger.IsTrue(Type.Is<T>());

            T go;
            if (setPositionAndRotation)
            {
                if (null != parent)
                    go = UnityObject.Instantiate(AssetObject.As<T>(), position, rotation, parent);
                else
                    go = UnityObject.Instantiate(AssetObject.As<T>(), position, rotation);
            }
            else
            {
                if (null != parent)
                    go = UnityObject.Instantiate(AssetObject.As<T>(), parent, worldPositionStays);
                else
                    go = UnityObject.Instantiate(AssetObject.As<T>());
            }

            _gameObjects.Add(go);

            return go;
        }

        /// <summary>
        /// 检测当前的实例化管理容器中是否包含指定的场景对象实例
        /// </summary>
        /// <param name="obj">场景对象实例</param>
        /// <returns>若容器包含目标场景对象实例则返回true，否则返回false</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ContainsObject(UnityObject obj)
        {
            return _gameObjects.Contains(obj);
        }

        /// <summary>
        /// 销毁场景对象实例
        /// </summary>
        /// <param name="obj">场景对象实例</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DestroyObject(UnityObject obj)
        {
            Debugger.IsTrue(_gameObjects.Contains(obj));

            _gameObjects.Remove(obj);
            UnityObject.Destroy(obj);
        }
    }
}
