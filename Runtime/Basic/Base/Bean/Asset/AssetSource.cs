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

using UnityObject = UnityEngine.Object;
using UnityTransform = UnityEngine.Transform;
using UnityVector3 = UnityEngine.Vector3;
using UnityQuaternion = UnityEngine.Quaternion;

namespace GameEngine
{
    /// <summary>
    /// 对实体对象内部加载的资产进行封装管理的对象类，用于管理指定类型的资产数据
    /// </summary>
    internal sealed class AssetSource
    {
        /// <summary>
        /// 资产名称
        /// </summary>
        private readonly string _name;
        /// <summary>
        /// 资产路径
        /// </summary>
        private readonly string _url;
        /// <summary>
        /// 资产类型
        /// </summary>
        private readonly Type _type;
        /// <summary>
        /// 原始资产对象（Unity对象）
        /// </summary>
        private UnityObject _original;
        /// <summary>
        /// 实例化对象实例
        /// </summary>
        private IList<UnityObject> _objects;

        public string Name => _name;
        public string Url => _url;
        public Type Type => _type;
        public UnityObject Original => _original;

        public AssetSource(string name, string url, Type type, UnityObject obj)
        {
            Debugger.Assert(false == string.IsNullOrEmpty(name), NovaEngine.ErrorText.InvalidArguments);

            _name = name;
            _url = url;
            _type = type;
            _original = obj;
            _objects = new List<UnityObject>();
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
            if (null != _objects)
            {
                for (int n = 0; n < _objects.Count; ++n)
                {
                    UnityObject.Destroy(_objects[n]);
                }
                _objects.Clear();
                _objects = null;
            }

            if (null != _original)
            {
                ResourceHandler.Instance.UnloadAsset(_original);
                _original = null;
            }
        }

        /// <summary>
        /// 创建场景对象实例
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="position">位置</param>
        /// <param name="rotation">旋转</param>
        /// <returns>返回创建的场景对象实例</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Instantiate<T>(UnityVector3 position, UnityQuaternion rotation) where T : UnityObject
        {
            Debugger.Assert(typeof(T) == _type, NovaEngine.ErrorText.InvalidArguments);

            UnityObject obj = UnityObject.Instantiate(_original, position, rotation);
            _objects.Add(obj);
            return (T) obj;
        }

        /// <summary>
        /// 创建场景对象实例
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="position">位置</param>
        /// <param name="rotation">旋转</param>
        /// <param name="parent">父对象实例</param>
        /// <returns>返回创建的场景对象实例</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Instantiate<T>(UnityVector3 position, UnityQuaternion rotation, UnityTransform parent) where T : UnityObject
        {
            Debugger.Assert(typeof(T) == _type, NovaEngine.ErrorText.InvalidArguments);

            UnityObject obj = UnityObject.Instantiate(_original, position, rotation, parent);
            _objects.Add(obj);
            return (T) obj;
        }

        /// <summary>
        /// 创建场景对象实例
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="parent">父对象实例</param>
        /// <returns>返回创建的场景对象实例</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Instantiate<T>(UnityTransform parent) where T : UnityObject
        {
            Debugger.Assert(typeof(T) == _type, NovaEngine.ErrorText.InvalidArguments);

            UnityObject obj = UnityObject.Instantiate(_original, parent);
            _objects.Add(obj);
            return (T) obj;
        }

        /// <summary>
        /// 检测当前的实例化管理容器中是否包含指定的场景对象实例
        /// </summary>
        /// <param name="obj">场景对象实例</param>
        /// <returns>若容器包含目标场景对象实例则返回true，否则返回false</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ContainsObject(UnityObject obj)
        {
            return _objects.Contains(obj);
        }

        /// <summary>
        /// 销毁场景对象实例
        /// </summary>
        /// <param name="obj">场景对象实例</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DestroyObject(UnityObject obj)
        {
            Debugger.Assert(_objects.Contains(obj), NovaEngine.ErrorText.InvalidArguments);

            _objects.Remove(obj);
            UnityObject.Destroy(obj);
        }
    }
}
