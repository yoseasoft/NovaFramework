/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
/// Copyring (C) 2022 - 2023, Shanghai Bilibili Technology Co., Ltd.
/// Copyring (C) 2023, Guangzhou Shiyue Network Technology Co., Ltd.
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

using SystemType = System.Type;
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
        /// 用于管理对象池实例的内部引用关联对象类，记录了管理的对象池实例当前的使用状态
        /// </summary>
        /// <typeparam name="T"></typeparam>
        private sealed class ObjectPool<T> : ObjectPoolBase, IObjectPool<T> where T : ObjectBase
        {
            private readonly MultiDictionary<string, Object<T>> m_objects;
            private readonly Dictionary<object, Object<T>> m_objectMap;
            private readonly ReleaseObjectFilterCallback<T> m_defaultReleaseObjectFilterCallback;
            private readonly IList<T> m_cachedCanReleaseObjects;
            private readonly IList<T> m_cachedToReleaseObjects;
            private readonly bool m_allowMultiSpawn;
            private float m_autoReleaseInterval;
            private int m_capacity;
            private float m_expireTime;
            private int m_priority;
            private float m_autoReleaseTime;

            /// <summary>
            /// 获取对象池中的对象类型
            /// </summary>
            public override SystemType ObjectType
            {
                get { return typeof(T); }
            }

            /// <summary>
            /// 获取对象池中对象的数量
            /// </summary>
            public override int Count
            {
                get { return m_objectMap.Count; }
            }

            /// <summary>
            /// 获取对象池中可被释放的对象的数量
            /// </summary>
            public override int CanReleaseCount
            {
                get
                {
                    GetCanReleaseObjects(m_cachedCanReleaseObjects);
                    return m_cachedCanReleaseObjects.Count;
                }
            }

            /// <summary>
            /// 获取是否允许对象可被多次分配
            /// </summary>
            public override bool AllowMultiSpawn
            {
                get { return m_allowMultiSpawn; }
            }

            /// <summary>
            /// 获取或设置对象池自动释放内部对象的间隔秒数
            /// </summary>
            public override float AutoReleaseInterval
            {
                get { return m_autoReleaseInterval; }
                set { m_autoReleaseInterval = value; }
            }

            /// <summary>
            /// 获取或设置对象池的容量
            /// </summary>
            public override int Capacity
            {
                get { return m_capacity; }
                set
                {
                    if (value < 0)
                    {
                        throw new CFrameworkException("Capacity is invalid.");
                    }

                    if (m_capacity == value)
                    {
                        return;
                    }

                    m_capacity = value;
                    Release();
                }
            }

            /// <summary>
            /// 获取或设置对象池中对象的过期时间
            /// </summary>
            public override float ExpireTime
            {
                get { return m_expireTime; }
                set
                {
                    if (value < 0f)
                    {
                        throw new CFrameworkException("ExpireTime is invalid.");
                    }

                    if (m_expireTime == value)
                    {
                        return;
                    }

                    m_expireTime = value;
                    Release();
                }
            }

            /// <summary>
            /// 获取或设置对象池的优先级
            /// </summary>
            public override int Priority
            {
                get { return m_priority; }
                set { m_priority = value; }
            }

            public ObjectPool(string name, bool allowMultiSpawn, float autoReleaseInterval, int capacity, float expireTime, int priority)
                : base(name)
            {
                m_objects = new MultiDictionary<string, Object<T>>();
                m_objectMap = new Dictionary<object, Object<T>>();
                m_defaultReleaseObjectFilterCallback = DefaultReleaseObjectFilterCallback;
                m_cachedCanReleaseObjects = new List<T>();
                m_cachedToReleaseObjects = new List<T>();
                m_allowMultiSpawn = allowMultiSpawn;
                m_autoReleaseInterval = autoReleaseInterval;
                Capacity = capacity;
                ExpireTime = expireTime;
                m_priority = priority;
                m_autoReleaseTime = 0f;
            }

            /// <summary>
            /// 注册一个用于孵化的对象实例
            /// </summary>
            /// <param name="obj">对象实例</param>
            /// <param name="spawned">对象是否已被孵化的标识</param>
            public void Register(T obj, bool spawned)
            {
                if (null == obj)
                {
                    throw new CFrameworkException("Object is invalid.");
                }

                Object<T> internalObject = Object<T>.Create(obj, spawned);
                m_objects.Add(obj.Name, internalObject);
                m_objectMap.Add(obj.Target, internalObject);

                if (Count > m_capacity)
                {
                    Release();
                }
            }

            /// <summary>
            /// 检测当前对象池是否处于可孵化状态
            /// </summary>
            /// <returns>若当前对象池可孵化对象则返回true，否则返回false</returns>
            public bool CanSpawn()
            {
                return CanSpawn(string.Empty);
            }

            /// <summary>
            /// 检测当前对象池是否可孵化指定名称的对象
            /// </summary>
            /// <param name="name">对象名称</param>
            /// <returns>若当前对象池可孵化对象则返回true，否则返回false</returns>
            public bool CanSpawn(string name)
            {
                if (null == name)
                {
                    throw new CFrameworkException("Name is invalid.");
                }

                DoubleLinkedList<Object<T>> objectRange = default(DoubleLinkedList<Object<T>>);
                if (m_objects.TryGetValue(name, out objectRange))
                {
                    foreach (Object<T> internalObject in objectRange)
                    {
                        // 允许多次分配或对象实例尚未被使用
                        if (m_allowMultiSpawn || false == internalObject.IsOnUsed)
                        {
                            return true;
                        }
                    }
                }

                return false;
            }

            /// <summary>
            /// 从当前对象池中孵化一个对象实例
            /// </summary>
            /// <returns>返回孵化的对象实例</returns>
            public T Spawn()
            {
                return Spawn(string.Empty);
            }

            /// <summary>
            /// 从当前对象池中孵化一个指定名称的对象实例
            /// </summary>
            /// <param name="name">对象名称</param>
            /// <returns>返回孵化的对象实例</returns>
            public T Spawn(string name)
            {
                if (null == name)
                {
                    throw new CFrameworkException("Name is invalid.");
                }

                DoubleLinkedList<Object<T>> objectRange = default(DoubleLinkedList<Object<T>>);
                if (m_objects.TryGetValue(name, out objectRange))
                {
                    foreach (Object<T> internalObject in objectRange)
                    {
                        // 允许多次分配或对象实例尚未被使用
                        if (m_allowMultiSpawn || false == internalObject.IsOnUsed)
                        {
                            return internalObject.Spawn();
                        }
                    }
                }

                return null;
            }

            /// <summary>
            /// 回收一个指定的孵化对象实例
            /// </summary>
            /// <param name="obj">目标对象实例</param>
            public void Unspawn(T obj)
            {
                if (null == obj)
                {
                    throw new CFrameworkException("Object is invalid.");
                }

                Unspawn(obj.Target);
            }

            /// <summary>
            /// 回收一个指定的孵化对象实例
            /// </summary>
            /// <param name="target">目标对象实例</param>
            public void Unspawn(object target)
            {
                if (null == target)
                {
                    throw new CFrameworkException("Target is invalid.");
                }

                Object<T> internalObject = GetObject(target);
                if (null != internalObject)
                {
                    internalObject.Unspawn();
                    if (Count > m_capacity && internalObject.SpawnCount <= 0)
                    {
                        Release();
                    }
                }
                else
                {
                    throw new CFrameworkException("Can not find target in object pool '{0}', target type is '{1}', target value is '{2}'.",
                                         new TypeNamePair(typeof(T), Name), target.GetType().FullName, target);
                }
            }

            /// <summary>
            /// 设置一个指定的对象实例是否被锁定
            /// </summary>
            /// <param name="obj">目标对象实例</param>
            /// <param name="locked">锁定状态</param>
            public void SetLocked(T obj, bool locked)
            {
                if (null == obj)
                {
                    throw new CFrameworkException("Object is invalid.");
                }

                SetLocked(obj.Target, locked);
            }

            /// <summary>
            /// 设置一个指定的对象实例是否被加锁
            /// </summary>
            /// <param name="target">目标对象实例</param>
            /// <param name="locked">锁定状态</param>
            public void SetLocked(object target, bool locked)
            {
                if (null == target)
                {
                    throw new CFrameworkException("Target is invalid.");
                }

                Object<T> internalObject = GetObject(target);
                if (null != internalObject)
                {
                    internalObject.Locked = locked;
                }
                else
                {
                    throw new CFrameworkException("Can not find target in object pool '{0}', target type is '{1}', target value is '{2}'.",
                                         new TypeNamePair(typeof(T), Name), target.GetType().FullName, target);
                }
            }

            /// <summary>
            /// 设置一个指定的对象实例的优先级
            /// </summary>
            /// <param name="obj">目标对象实例</param>
            /// <param name="priority">优先级</param>
            public void SetPriority(T obj, int priority)
            {
                if (null == obj)
                {
                    throw new CFrameworkException("Object is invalid.");
                }

                SetPriority(obj.Target, priority);
            }

            /// <summary>
            /// 设置一个指定的对象实例的优先级
            /// </summary>
            /// <param name="target">目标对象实例</param>
            /// <param name="priority">优先级</param>
            public void SetPriority(object target, int priority)
            {
                if (null == target)
                {
                    throw new CFrameworkException("Target is invalid.");
                }

                Object<T> internalObject = GetObject(target);
                if (internalObject != null)
                {
                    internalObject.Priority = priority;
                }
                else
                {
                    throw new CFrameworkException("Can not find target in object pool '{0}', target type is '{1}', target value is '{2}'.",
                                         new TypeNamePair(typeof(T), Name), target.GetType().FullName, target);
                }
            }

            /// <summary>
            /// 释放指定的对象实例
            /// </summary>
            /// <param name="obj">目标对象实例</param>
            /// <returns>释放对象实例成功则返回true，否则返回false</returns>
            public bool ReleaseObject(T obj)
            {
                if (null == obj)
                {
                    throw new CFrameworkException("Object is invalid.");
                }

                return ReleaseObject(obj.Target);
            }

            /// <summary>
            /// 释放指定的对象实例
            /// </summary>
            /// <param name="target">目标对象实例</param>
            /// <returns>释放对象实例成功则返回true，否则返回false</returns>
            public bool ReleaseObject(object target)
            {
                if (null == target)
                {
                    throw new CFrameworkException("Target is invalid.");
                }

                Object<T> internalObject = GetObject(target);
                if (null == internalObject)
                {
                    return false;
                }

                if (internalObject.IsOnUsed || internalObject.Locked || false == internalObject.Releasabled)
                {
                    return false;
                }

                m_objects.Remove(internalObject.Name, internalObject);
                m_objectMap.Remove(internalObject.Peek().Target);

                internalObject.Release(false);
                ReferencePool.Release(internalObject);

                return true;
            }

            /// <summary>
            /// 释放对象池中的所有可释放对象实例
            /// </summary>
            public override void Release()
            {
                Release(Count - m_capacity, m_defaultReleaseObjectFilterCallback);
            }

            /// <summary>
            /// 释放对象池中指定数量的所有可释放对象实例<br/>
            /// 若剩余对象的数量不足，则释放全部对象实例
            /// </summary>
            /// <param name="releaseCount">尝试释放的对象数量</param>
            public override void Release(int releaseCount)
            {
                Release(releaseCount, m_defaultReleaseObjectFilterCallback);
            }

            /// <summary>
            /// 根据指定筛选规则释放对象池中的所有可释放对象实例
            /// </summary>
            /// <param name="callback">释放对象筛选函数</param>
            public void Release(ReleaseObjectFilterCallback<T> callback)
            {
                Release(Count - m_capacity, callback);
            }

            /// <summary>
            /// 根据指定筛选规则释放对象池中指定数量的所有可释放对象实例
            /// </summary>
            /// <param name="releaseCount">尝试释放的对象数量</param>
            /// <param name="callback">释放对象筛选函数</param>
            public void Release(int releaseCount, ReleaseObjectFilterCallback<T> callback)
            {
                if (null == callback)
                {
                    throw new CFrameworkException("Release object filter callback is invalid.");
                }

                if (releaseCount < 0)
                {
                    releaseCount = 0;
                }

                SystemDateTime expireTime = SystemDateTime.MinValue;
                if (m_expireTime < float.MaxValue)
                {
                    expireTime = SystemDateTime.UtcNow.AddSeconds(-m_expireTime);
                }

                m_autoReleaseTime = 0f;
                GetCanReleaseObjects(m_cachedCanReleaseObjects);
                IList<T> toReleaseObjects = callback(m_cachedCanReleaseObjects, releaseCount, expireTime);
                if (null == toReleaseObjects || toReleaseObjects.Count <= 0)
                {
                    return;
                }

                foreach (T toReleaseObject in toReleaseObjects)
                {
                    ReleaseObject(toReleaseObject);
                }
            }

            /// <summary>
            /// 释放对象池中的所有未使用的对象实例
            /// </summary>
            public override void ReleaseAllUnused()
            {
                m_autoReleaseTime = 0f;
                GetCanReleaseObjects(m_cachedCanReleaseObjects);
                foreach (T toReleaseObject in m_cachedCanReleaseObjects)
                {
                    ReleaseObject(toReleaseObject);
                }
            }

            internal override void Update(float elapseSeconds, float realElapseSeconds)
            {
                m_autoReleaseTime += realElapseSeconds;
                if (m_autoReleaseTime < m_autoReleaseInterval)
                {
                    return;
                }

                Release();
            }

            internal override void Shutdown()
            {
                foreach (KeyValuePair<object, Object<T>> objectInMap in m_objectMap)
                {
                    objectInMap.Value.Release(true);
                    ReferencePool.Release(objectInMap.Value);
                }

                m_objects.Clear();
                m_objectMap.Clear();
                m_cachedCanReleaseObjects.Clear();
                m_cachedToReleaseObjects.Clear();
            }

            /// <summary>
            /// 获取所有实例的对象信息集合
            /// </summary>
            /// <returns>返回所有实例的对象信息集合</returns>
            public override ObjectInfo[] GetAllObjectInfos()
            {
                List<ObjectInfo> results = new List<ObjectInfo>();
                foreach (KeyValuePair<string, DoubleLinkedList<Object<T>>> objectRanges in m_objects)
                {
                    foreach (Object<T> internalObject in objectRanges.Value)
                    {
                        results.Add(new ObjectInfo(internalObject.Name,
                                                   internalObject.Locked,
                                                   internalObject.Releasabled,
                                                   internalObject.Priority,
                                                   internalObject.LastUseTime,
                                                   internalObject.SpawnCount));
                    }
                }

                return results.ToArray();
            }

            /// <summary>
            /// 通过对象的引用实例获取其对应的内部对象
            /// </summary>
            /// <param name="target">引用实例</param>
            /// <returns>返回引用实例对应的内部对象</returns>
            private Object<T> GetObject(object target)
            {
                if (null == target)
                {
                    return null;
                }

                Object<T> internalObject = null;
                if (m_objectMap.TryGetValue(target, out internalObject))
                {
                    return internalObject;
                }

                return null;
            }

            /// <summary>
            /// 获取可释放的对象实例集合，并放入指定的容器中
            /// </summary>
            /// <param name="results">结果容器</param>
            private void GetCanReleaseObjects(IList<T> results)
            {
                if (null == results)
                {
                    throw new CFrameworkException("Results is invalid.");
                }

                results.Clear();
                foreach (KeyValuePair<object, Object<T>> objectInMap in m_objectMap)
                {
                    Object<T> internalObject = objectInMap.Value;
                    if (internalObject.IsOnUsed || internalObject.Locked || false == internalObject.Releasabled)
                    {
                        continue;
                    }

                    results.Add(internalObject.Peek());
                }
            }

            private IList<T> DefaultReleaseObjectFilterCallback(IList<T> candidateObjects, int releaseCount, SystemDateTime expireTime)
            {
                m_cachedToReleaseObjects.Clear();

                if (expireTime > SystemDateTime.MinValue)
                {
                    for (int n = candidateObjects.Count - 1; n >= 0; --n)
                    {
                        if (candidateObjects[n].LastUseTime <= expireTime)
                        {
                            m_cachedToReleaseObjects.Add(candidateObjects[n]);
                            candidateObjects.RemoveAt(n);
                            continue;
                        }
                    }

                    releaseCount -= m_cachedToReleaseObjects.Count;
                }

                for (int n = 0; releaseCount > 0 && n < candidateObjects.Count; ++n)
                {
                    for (int m = n + 1; m < candidateObjects.Count; ++m)
                    {
                        if (candidateObjects[n].Priority > candidateObjects[m].Priority ||
                            candidateObjects[n].Priority == candidateObjects[m].Priority &&
                            candidateObjects[n].LastUseTime > candidateObjects[m].LastUseTime)
                        {
                            T temp = candidateObjects[n];
                            candidateObjects[n] = candidateObjects[m];
                            candidateObjects[m] = temp;
                        }
                    }

                    m_cachedToReleaseObjects.Add(candidateObjects[n]);
                    releaseCount--;
                }

                return m_cachedToReleaseObjects;
            }
        }
    }
}
