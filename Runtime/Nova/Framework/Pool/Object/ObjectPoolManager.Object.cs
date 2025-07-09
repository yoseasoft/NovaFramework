/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
/// Copyright (C) 2022 - 2023, Shanghai Bilibili Technology Co., Ltd.
/// Copyright (C) 2023, Guangzhou Shiyue Network Technology Co., Ltd.
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

using System.Collections.Generic;

using SystemDateTime = System.DateTime;

namespace NovaEngine.ObjectPool
{
    /// <summary>
    /// 对象池的管理器实现类，该类通过完成管理器标准接口实现对象池的全部管理流程<br/>
    /// 当您需要使用对象池技术时，无需自己去实现一个对象池管理器类，建议您直接通过该类去达到目的
    /// </summary>
    internal sealed partial class ObjectPoolManager
    {
        /// <summary>
        /// 用于管理对象实例的内部引用关联对象类，记录了管理的对象实例当前的使用状态
        /// </summary>
        /// <typeparam name="T"></typeparam>
        private sealed class Object<T> : IReference where T : ObjectBase
        {
            /// <summary>
            /// 引用对象实例
            /// </summary>
            private T _object;
            /// <summary>
            /// 对象实例的孵化计数
            /// </summary>
            private int _spawnCount;

            /// <summary>
            /// 获取对象的名称
            /// </summary>
            public string Name
            {
                get { return _object.Name; }
            }

            /// <summary>
            /// 获取或设置对象实例是否被加锁的状态
            /// </summary>
            public bool Locked
            {
                get { return _object.Locked; }
                internal set { _object.Locked = value; }
            }

            /// <summary>
            /// 获取或设置对象实例的优先级
            /// </summary>
            public int Priority
            {
                get { return _object.Priority; }
                internal set { _object.Priority = value; }
            }

            /// <summary>
            /// 获取对象实例的可释放检查标记
            /// </summary>
            public bool Releasabled
            {
                get { return _object.Releasabled; }
            }

            /// <summary>
            /// 获取对象实例的上次使用时间
            /// </summary>
            public SystemDateTime LastUseTime
            {
                get { return _object.LastUseTime; }
            }

            /// <summary>
            /// 获取对象实例的孵化计数
            /// </summary>
            public int SpawnCount
            {
                get { return _spawnCount; }
            }

            /// <summary>
            /// 获取对象实例当前是否正在使用的状态
            /// </summary>
            public bool IsOnUsed
            {
                get { return _spawnCount > 0; }
            }

            public Object()
            {
                _object = null;
                _spawnCount = 0;
            }

            /// <summary>
            /// 对象实例的默认初始化回调函数
            /// </summary>
            public void Initialize()
            {
            }

            /// <summary>
            /// 对象实例的默认清理回调函数
            /// </summary>
            public void Cleanup()
            {
                _object = null;
                _spawnCount = 0;
            }

            /// <summary>
            /// 通过指定的对象实例创建一个有效的管理对象
            /// </summary>
            /// <param name="obj">对象实例</param>
            /// <param name="spawned">对象被孵化的状态标识</param>
            /// <returns>返回创建的内部对象</returns>
            public static Object<T> Create(T obj, bool spawned)
            {
                if (null == obj)
                {
                    throw new CFrameworkException("Object is invalid.");
                }

                Object<T> internalObject = ReferencePool.Acquire<Object<T>>();
                internalObject._object = obj;
                internalObject._spawnCount = spawned ? 1 : 0;
                if (spawned)
                {
                    obj.OnSpawn();
                }

                return internalObject;
            }

            /// <summary>
            /// 获取管理的目标对象实例
            /// </summary>
            /// <returns>返回托管的对象实例</returns>
            public T Peek()
            {
                return _object;
            }

            /// <summary>
            /// 孵化对象实例
            /// </summary>
            /// <returns>返回孵化的对象实例</returns>
            public T Spawn()
            {
                _spawnCount++;
                _object.LastUseTime = SystemDateTime.UtcNow;
                _object.OnSpawn();
                return _object;
            }

            /// <summary>
            /// 回收对象实例
            /// </summary>
            public void Unspawn()
            {
                _object.OnUnspawn();
                _object.LastUseTime = SystemDateTime.UtcNow;
                _spawnCount--;
                if (_spawnCount < 0)
                {
                    throw new CFrameworkException("Object '{0}' spawn count is less than zero.", Name);
                }
            }

            /// <summary>
            /// 释放托管的对象实例
            /// </summary>
            /// <param name="shutdown">关闭对象池时触发的状态标识</param>
            public void Release(bool shutdown)
            {
                _object.Release(shutdown);
                ReferencePool.Release(_object);
            }
        }
    }
}
