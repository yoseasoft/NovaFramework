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

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace NovaEngine.ObjectPool
{
    /// <summary>
    /// 对象池的管理器实现类，该类通过完成管理器标准接口实现对象池的全部管理流程<br/>
    /// 当您需要使用对象池技术时，无需自己去实现一个对象池管理器类，建议您直接通过该类去达到目的
    /// </summary>
    internal sealed partial class ObjectPoolManager : IObjectPoolManager
    {
        private const int DefaultCapacity = int.MaxValue;
        private const float DefaultExpireTime = float.MaxValue;
        private const int DefaultPriority = 0;

        private readonly IDictionary<TypeNamePair, ObjectPoolBase> _objectPools;
        private readonly IList<ObjectPoolBase> _cachedAllObjectPools;
        private readonly Comparison<ObjectPoolBase> _objectPoolComparer;

        /// <summary>
        /// 获取对象池的数量
        /// </summary>
        public int Count
        {
            get { return _objectPools.Count; }
        }

        public ObjectPoolManager()
        {
            _objectPools = new Dictionary<TypeNamePair, ObjectPoolBase>();
            _cachedAllObjectPools = new List<ObjectPoolBase>();
            _objectPoolComparer = ObjectPoolComparer;
        }

        /// <summary>
        /// 检查是否存在指定类型的对象池
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <returns>若存在给定类型的对象池则返回true，否则返回false</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool HasObjectPool<T>() where T : ObjectBase
        {
            return InternalHasObjectPool(new TypeNamePair(typeof(T)));
        }

        /// <summary>
        /// 检查是否存在指定类型的对象池
        /// </summary>
        /// <param name="objectType">对象类型</param>
        /// <returns>若存在给定类型的对象池则返回true，否则返回false</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool HasObjectPool(Type objectType)
        {
            if (null == objectType)
            {
                throw new CFrameworkException("Object type is invalid.");
            }

            if (false == typeof(ObjectBase).IsAssignableFrom(objectType))
            {
                throw new CFrameworkException("Object type '{%t}' is invalid.", objectType);
            }

            return InternalHasObjectPool(new TypeNamePair(objectType));
        }

        /// <summary>
        /// 检查是否存在指定类型和名称的对象池
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="name">对象池名称</param>
        /// <returns>若存在给定类型和名称的对象池则返回true，否则返回false</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool HasObjectPool<T>(string name) where T : ObjectBase
        {
            return InternalHasObjectPool(new TypeNamePair(typeof(T), name));
        }

        /// <summary>
        /// 检查是否存在指定类型和名称的对象池
        /// </summary>
        /// <param name="objectType">对象类型</param>
        /// <param name="name">对象池名称</param>
        /// <returns>若存在给定类型和名称的对象池则返回true，否则返回false</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool HasObjectPool(Type objectType, string name)
        {
            if (null == objectType)
            {
                throw new CFrameworkException("Object type is invalid.");
            }

            if (false == typeof(ObjectBase).IsAssignableFrom(objectType))
            {
                throw new CFrameworkException("Object type '{%t}' is invalid.", objectType);
            }

            return InternalHasObjectPool(new TypeNamePair(objectType, name));
        }

        /// <summary>
        /// 检查是否存在匹配指定条件的对象池
        /// </summary>
        /// <param name="condition">要检查的条件</param>
        /// <returns>若存在匹配给定条件的对象池则返回true，否则返回false</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool HasObjectPool(Predicate<ObjectPoolBase> condition)
        {
            if (null == condition)
            {
                throw new CFrameworkException("Condition is invalid.");
            }

            foreach (KeyValuePair<TypeNamePair, ObjectPoolBase> objectPool in _objectPools)
            {
                if (condition(objectPool.Value))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 获取指定类型对应的对象池实例
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <returns>返回与类型对应的对象池实例</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IObjectPool<T> GetObjectPool<T>() where T : ObjectBase
        {
            return (IObjectPool<T>) InternalGetObjectPool(new TypeNamePair(typeof(T)));
        }

        /// <summary>
        /// 获取指定类型对应的对象池实例
        /// </summary>
        /// <param name="objectType">对象类型</param>
        /// <returns>返回与类型对应的对象池实例</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ObjectPoolBase GetObjectPool(Type objectType)
        {
            if (null == objectType)
            {
                throw new CFrameworkException("Object type is invalid.");
            }

            if (false == typeof(ObjectBase).IsAssignableFrom(objectType))
            {
                throw new CFrameworkException("Object type '{%t}' is invalid.", objectType);
            }

            return InternalGetObjectPool(new TypeNamePair(objectType));
        }

        /// <summary>
        /// 获取指定类型和名称对应的对象池实例
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="name">对象池名称</param>
        /// <returns>返回与类型和名称对应的对象池实例</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IObjectPool<T> GetObjectPool<T>(string name) where T : ObjectBase
        {
            return (IObjectPool<T>) InternalGetObjectPool(new TypeNamePair(typeof(T), name));
        }

        /// <summary>
        /// 获取指定类型和名称对应的对象池实例
        /// </summary>
        /// <param name="objectType">对象类型</param>
        /// <param name="name">对象池名称</param>
        /// <returns>返回与类型和名称对应的对象池实例</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ObjectPoolBase GetObjectPool(Type objectType, string name)
        {
            if (null == objectType)
            {
                throw new CFrameworkException("Object type is invalid.");
            }

            if (false == typeof(ObjectBase).IsAssignableFrom(objectType))
            {
                throw new CFrameworkException("Object type '{%t}' is invalid.", objectType);
            }

            return InternalGetObjectPool(new TypeNamePair(objectType, name));
        }

        /// <summary>
        /// 获取匹配指定条件的对象池实例
        /// </summary>
        /// <param name="condition">要检查的条件</param>
        /// <returns>返回匹配给定条件的对象池实例</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ObjectPoolBase GetObjectPool(Predicate<ObjectPoolBase> condition)
        {
            if (null == condition)
            {
                throw new CFrameworkException("Condition is invalid.");
            }

            foreach (KeyValuePair<TypeNamePair, ObjectPoolBase> objectPool in _objectPools)
            {
                if (condition(objectPool.Value))
                {
                    return objectPool.Value;
                }
            }

            return null;
        }

        /// <summary>
        /// 批量获取匹配指定条件的对象池实例的集合
        /// </summary>
        /// <param name="condition">要检查的条件</param>
        /// <returns>返回匹配给定条件的对象池实例的集合</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ObjectPoolBase[] GetObjectPools(Predicate<ObjectPoolBase> condition)
        {
            if (null == condition)
            {
                throw new CFrameworkException("Condition is invalid.");
            }

            List<ObjectPoolBase> results = new List<ObjectPoolBase>();
            foreach (KeyValuePair<TypeNamePair, ObjectPoolBase> objectPool in _objectPools)
            {
                if (condition(objectPool.Value))
                {
                    results.Add(objectPool.Value);
                }
            }

            return results.ToArray();
        }

        /// <summary>
        /// 获取匹配指定条件的对象池实例，并填充到指定容器中
        /// </summary>
        /// <param name="condition">要检查的条件</param>
        /// <param name="results">填充对象池的容器</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void GetObjectPools(Predicate<ObjectPoolBase> condition, IList<ObjectPoolBase> results)
        {
            if (null == condition)
            {
                throw new CFrameworkException("Condition is invalid.");
            }

            if (null == results)
            {
                throw new CFrameworkException("Results is invalid.");
            }

            results.Clear();
            foreach (KeyValuePair<TypeNamePair, ObjectPoolBase> objectPool in _objectPools)
            {
                if (condition(objectPool.Value))
                {
                    results.Add(objectPool.Value);
                }
            }
        }

        /// <summary>
        /// 获取当前管理容器中所有的对象池实例
        /// </summary>
        /// <returns>返回所有对象池实例</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ObjectPoolBase[] GetAllObjectPools()
        {
            return GetAllObjectPools(false);
        }

        /// <summary>
        /// 获取当前管理容器中所有的对象池实例，并填充到指定容器中
        /// </summary>
        /// <param name="results">填充对象池的容器</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void GetAllObjectPools(List<ObjectPoolBase> results)
        {
            GetAllObjectPools(false, results);
        }

        /// <summary>
        /// 获取当前管理容器中所有的对象池实例，并根据标识是否按排序返回
        /// </summary>
        /// <param name="sort">是否按优先级排序</param>
        /// <returns>返回所有对象池实例</returns>
        public ObjectPoolBase[] GetAllObjectPools(bool sort)
        {
            if (sort)
            {
                List<ObjectPoolBase> results = new List<ObjectPoolBase>();
                foreach (KeyValuePair<TypeNamePair, ObjectPoolBase> objectPool in _objectPools)
                {
                    results.Add(objectPool.Value);
                }

                results.Sort(_objectPoolComparer);
                return results.ToArray();
            }
            else
            {
                int index = 0;
                ObjectPoolBase[] results = new ObjectPoolBase[_objectPools.Count];
                foreach (KeyValuePair<TypeNamePair, ObjectPoolBase> objectPool in _objectPools)
                {
                    results[index++] = objectPool.Value;
                }

                return results;
            }
        }

        /// <summary>
        /// 获取当前管理容器中所有的对象池实例，并根据标识是否按排序填充到指定容器中
        /// </summary>
        /// <param name="sort">是否按优先级排序</param>
        /// <param name="results">填充对象池的容器</param>
        public void GetAllObjectPools(bool sort, List<ObjectPoolBase> results)
        {
            if (null == results)
            {
                throw new CFrameworkException("Results is invalid.");
            }

            results.Clear();
            foreach (KeyValuePair<TypeNamePair, ObjectPoolBase> objectPool in _objectPools)
            {
                results.Add(objectPool.Value);
            }

            if (sort)
            {
                results.Sort(_objectPoolComparer);
            }
        }

        /// <summary>
        /// 创建允许单次获取的指定类型的对象池实例
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <returns>返回创建的给定类型的对象池实例</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IObjectPool<T> CreateSingleSpawnObjectPool<T>() where T : ObjectBase
        {
            return InternalCreateObjectPool<T>(null, false, DefaultExpireTime, DefaultCapacity, DefaultExpireTime, DefaultPriority);
        }

        /// <summary>
        /// 创建允许单次获取的指定类型的对象池实例
        /// </summary>
        /// <param name="objectType">对象类型</param>
        /// <returns>返回创建的给定类型的对象池实例</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ObjectPoolBase CreateSingleSpawnObjectPool(Type objectType)
        {
            return InternalCreateObjectPool(objectType, null, false, DefaultExpireTime, DefaultCapacity, DefaultExpireTime, DefaultPriority);
        }

        /// <summary>
        /// 创建允许单次获取的指定类型和名称的对象池实例
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="name">对象池名称</param>
        /// <returns>返回创建的给定类型和名称的对象池实例</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IObjectPool<T> CreateSingleSpawnObjectPool<T>(string name) where T : ObjectBase
        {
            return InternalCreateObjectPool<T>(name, false, DefaultExpireTime, DefaultCapacity, DefaultExpireTime, DefaultPriority);
        }

        /// <summary>
        /// 创建允许单次获取的指定类型和名称的对象池实例
        /// </summary>
        /// <param name="objectType">对象类型</param>
        /// <param name="name">对象池名称</param>
        /// <returns>返回创建的给定类型和名称的对象池实例</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ObjectPoolBase CreateSingleSpawnObjectPool(Type objectType, string name)
        {
            return InternalCreateObjectPool(objectType, name, false, DefaultExpireTime, DefaultCapacity, DefaultExpireTime, DefaultPriority);
        }

        /// <summary>
        /// 创建允许单次获取的指定类型的对象池实例
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="capacity">对象池的容量</param>
        /// <returns>返回创建的给定类型的对象池实例</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IObjectPool<T> CreateSingleSpawnObjectPool<T>(int capacity) where T : ObjectBase
        {
            return InternalCreateObjectPool<T>(null, false, DefaultExpireTime, capacity, DefaultExpireTime, DefaultPriority);
        }

        /// <summary>
        /// 创建允许单次获取的指定类型的对象池实例
        /// </summary>
        /// <param name="objectType">对象类型</param>
        /// <param name="capacity">对象池的容量</param>
        /// <returns>返回创建的给定类型的对象池实例</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ObjectPoolBase CreateSingleSpawnObjectPool(Type objectType, int capacity)
        {
            return InternalCreateObjectPool(objectType, null, false, DefaultExpireTime, capacity, DefaultExpireTime, DefaultPriority);
        }

        /// <summary>
        /// 创建允许单次获取的指定类型的对象池实例
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="expireTime">对象池过期时间</param>
        /// <returns>返回创建的给定类型的对象池实例</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IObjectPool<T> CreateSingleSpawnObjectPool<T>(float expireTime) where T : ObjectBase
        {
            return InternalCreateObjectPool<T>(null, false, expireTime, DefaultCapacity, expireTime, DefaultPriority);
        }

        /// <summary>
        /// 创建允许单次获取的指定类型的对象池实例
        /// </summary>
        /// <param name="objectType">对象类型</param>
        /// <param name="expireTime">对象池过期时间</param>
        /// <returns>返回创建的给定类型的对象池实例</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ObjectPoolBase CreateSingleSpawnObjectPool(Type objectType, float expireTime)
        {
            return InternalCreateObjectPool(objectType, null, false, expireTime, DefaultCapacity, expireTime, DefaultPriority);
        }

        /// <summary>
        /// 创建允许单次获取的指定类型和名称的对象池实例
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="name">对象池名称</param>
        /// <param name="capacity">对象池的容量</param>
        /// <returns>返回创建的给定类型和名称的对象池实例</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IObjectPool<T> CreateSingleSpawnObjectPool<T>(string name, int capacity) where T : ObjectBase
        {
            return InternalCreateObjectPool<T>(name, false, DefaultExpireTime, capacity, DefaultExpireTime, DefaultPriority);
        }

        /// <summary>
        /// 创建允许单次获取的指定类型和名称的对象池实例
        /// </summary>
        /// <param name="objectType">对象类型</param>
        /// <param name="name">对象池名称</param>
        /// <param name="capacity">对象池的容量</param>
        /// <returns>返回创建的给定类型和名称的对象池实例</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ObjectPoolBase CreateSingleSpawnObjectPool(Type objectType, string name, int capacity)
        {
            return InternalCreateObjectPool(objectType, name, false, DefaultExpireTime, capacity, DefaultExpireTime, DefaultPriority);
        }

        /// <summary>
        /// 创建允许单次获取的指定类型和名称的对象池实例
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="name">对象池名称</param>
        /// <param name="expireTime">对象池过期时间</param>
        /// <returns>返回创建的给定类型和名称的对象池实例</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IObjectPool<T> CreateSingleSpawnObjectPool<T>(string name, float expireTime) where T : ObjectBase
        {
            return InternalCreateObjectPool<T>(name, false, expireTime, DefaultCapacity, expireTime, DefaultPriority);
        }

        /// <summary>
        /// 创建允许单次获取的指定类型和名称的对象池实例
        /// </summary>
        /// <param name="objectType">对象类型</param>
        /// <param name="name">对象池名称</param>
        /// <param name="expireTime">对象池过期时间</param>
        /// <returns>返回创建的给定类型和名称的对象池实例</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ObjectPoolBase CreateSingleSpawnObjectPool(Type objectType, string name, float expireTime)
        {
            return InternalCreateObjectPool(objectType, name, false, expireTime, DefaultCapacity, expireTime, DefaultPriority);
        }

        /// <summary>
        /// 创建允许单次获取的指定类型的对象池实例
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="capacity">对象池的容量</param>
        /// <param name="expireTime">对象池过期时间</param>
        /// <returns>返回创建的给定类型的对象池实例</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IObjectPool<T> CreateSingleSpawnObjectPool<T>(int capacity, float expireTime) where T : ObjectBase
        {
            return InternalCreateObjectPool<T>(null, false, expireTime, capacity, expireTime, DefaultPriority);
        }

        /// <summary>
        /// 创建允许单次获取的指定类型的对象池实例
        /// </summary>
        /// <param name="objectType">对象类型</param>
        /// <param name="capacity">对象池的容量</param>
        /// <param name="expireTime">对象池过期时间</param>
        /// <returns>返回创建的给定类型的对象池实例</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ObjectPoolBase CreateSingleSpawnObjectPool(Type objectType, int capacity, float expireTime)
        {
            return InternalCreateObjectPool(objectType, null, false, expireTime, capacity, expireTime, DefaultPriority);
        }

        /// <summary>
        /// 创建允许单次获取的指定类型的对象池实例
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="capacity">对象池的容量</param>
        /// <param name="priority">对象池的优先级</param>
        /// <returns>返回创建的给定类型的对象池实例</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IObjectPool<T> CreateSingleSpawnObjectPool<T>(int capacity, int priority) where T : ObjectBase
        {
            return InternalCreateObjectPool<T>(null, false, DefaultExpireTime, capacity, DefaultExpireTime, priority);
        }

        /// <summary>
        /// 创建允许单次获取的指定类型的对象池实例
        /// </summary>
        /// <param name="objectType">对象类型</param>
        /// <param name="capacity">对象池的容量</param>
        /// <param name="priority">对象池的优先级</param>
        /// <returns>返回创建的给定类型的对象池实例</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ObjectPoolBase CreateSingleSpawnObjectPool(Type objectType, int capacity, int priority)
        {
            return InternalCreateObjectPool(objectType, null, false, DefaultExpireTime, capacity, DefaultExpireTime, priority);
        }

        /// <summary>
        /// 创建允许单次获取的指定类型的对象池实例
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="expireTime">对象池过期时间</param>
        /// <param name="priority">对象池的优先级</param>
        /// <returns>返回创建的给定类型的对象池实例</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IObjectPool<T> CreateSingleSpawnObjectPool<T>(float expireTime, int priority) where T : ObjectBase
        {
            return InternalCreateObjectPool<T>(null, false, expireTime, DefaultCapacity, expireTime, priority);
        }

        /// <summary>
        /// 创建允许单次获取的指定类型的对象池实例
        /// </summary>
        /// <param name="objectType">对象类型</param>
        /// <param name="expireTime">对象池过期时间</param>
        /// <param name="priority">对象池的优先级</param>
        /// <returns>返回创建的给定类型的对象池实例</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ObjectPoolBase CreateSingleSpawnObjectPool(Type objectType, float expireTime, int priority)
        {
            return InternalCreateObjectPool(objectType, null, false, expireTime, DefaultCapacity, expireTime, priority);
        }

        /// <summary>
        /// 创建允许单次获取的指定类型和名称的对象池实例
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="name">对象池名称</param>
        /// <param name="capacity">对象池的容量</param>
        /// <param name="expireTime">对象池过期时间</param>
        /// <returns>返回创建的给定类型和名称的对象池实例</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IObjectPool<T> CreateSingleSpawnObjectPool<T>(string name, int capacity, float expireTime) where T : ObjectBase
        {
            return InternalCreateObjectPool<T>(name, false, expireTime, capacity, expireTime, DefaultPriority);
        }

        /// <summary>
        /// 创建允许单次获取的指定类型和名称的对象池实例
        /// </summary>
        /// <param name="objectType">对象类型</param>
        /// <param name="name">对象池名称</param>
        /// <param name="capacity">对象池的容量</param>
        /// <param name="expireTime">对象池过期时间</param>
        /// <returns>返回创建的给定类型和名称的对象池实例</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ObjectPoolBase CreateSingleSpawnObjectPool(Type objectType, string name, int capacity, float expireTime)
        {
            return InternalCreateObjectPool(objectType, name, false, expireTime, capacity, expireTime, DefaultPriority);
        }

        /// <summary>
        /// 创建允许单次获取的指定类型和名称的对象池实例
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="name">对象池名称</param>
        /// <param name="capacity">对象池的容量</param>
        /// <param name="priority">对象池的优先级</param>
        /// <returns>返回创建的给定类型和名称的对象池实例</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IObjectPool<T> CreateSingleSpawnObjectPool<T>(string name, int capacity, int priority) where T : ObjectBase
        {
            return InternalCreateObjectPool<T>(name, false, DefaultExpireTime, capacity, DefaultExpireTime, priority);
        }

        /// <summary>
        /// 创建允许单次获取的指定类型和名称的对象池实例
        /// </summary>
        /// <param name="objectType">对象类型</param>
        /// <param name="name">对象池名称</param>
        /// <param name="capacity">对象池的容量</param>
        /// <param name="priority">对象池的优先级</param>
        /// <returns>返回创建的给定类型和名称的对象池实例</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ObjectPoolBase CreateSingleSpawnObjectPool(Type objectType, string name, int capacity, int priority)
        {
            return InternalCreateObjectPool(objectType, name, false, DefaultExpireTime, capacity, DefaultExpireTime, priority);
        }

        /// <summary>
        /// 创建允许单次获取的指定类型和名称的对象池实例
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="name">对象池名称</param>
        /// <param name="expireTime">对象池过期时间</param>
        /// <param name="priority">对象池的优先级</param>
        /// <returns>返回创建的给定类型和名称的对象池实例</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IObjectPool<T> CreateSingleSpawnObjectPool<T>(string name, float expireTime, int priority) where T : ObjectBase
        {
            return InternalCreateObjectPool<T>(name, false, expireTime, DefaultCapacity, expireTime, priority);
        }

        /// <summary>
        /// 创建允许单次获取的指定类型和名称的对象池实例
        /// </summary>
        /// <param name="objectType">对象类型</param>
        /// <param name="name">对象池名称</param>
        /// <param name="expireTime">对象池过期时间</param>
        /// <param name="priority">对象池的优先级</param>
        /// <returns>返回创建的给定类型和名称的对象池实例</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ObjectPoolBase CreateSingleSpawnObjectPool(Type objectType, string name, float expireTime, int priority)
        {
            return InternalCreateObjectPool(objectType, name, false, expireTime, DefaultCapacity, expireTime, priority);
        }

        /// <summary>
        /// 创建允许单次获取的指定类型的对象池实例
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="capacity">对象池的容量</param>
        /// <param name="expireTime">对象池过期时间</param>
        /// <param name="priority">对象池的优先级</param>
        /// <returns>返回创建的给定类型的对象池实例</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IObjectPool<T> CreateSingleSpawnObjectPool<T>(int capacity, float expireTime, int priority) where T : ObjectBase
        {
            return InternalCreateObjectPool<T>(null, false, expireTime, capacity, expireTime, priority);
        }

        /// <summary>
        /// 创建允许单次获取的指定类型的对象池实例
        /// </summary>
        /// <param name="objectType">对象类型</param>
        /// <param name="capacity">对象池的容量</param>
        /// <param name="expireTime">对象池过期时间</param>
        /// <param name="priority">对象池的优先级</param>
        /// <returns>返回创建的给定类型的对象池实例</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ObjectPoolBase CreateSingleSpawnObjectPool(Type objectType, int capacity, float expireTime, int priority)
        {
            return InternalCreateObjectPool(objectType, null, false, expireTime, capacity, expireTime, priority);
        }

        /// <summary>
        /// 创建允许单次获取的指定类型和名称的对象池实例
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="name">对象池名称</param>
        /// <param name="capacity">对象池的容量</param>
        /// <param name="expireTime">对象池过期时间</param>
        /// <param name="priority">对象池的优先级</param>
        /// <returns>返回创建的给定类型和名称的对象池实例</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IObjectPool<T> CreateSingleSpawnObjectPool<T>(string name, int capacity, float expireTime, int priority) where T : ObjectBase
        {
            return InternalCreateObjectPool<T>(name, false, expireTime, capacity, expireTime, priority);
        }

        /// <summary>
        /// 创建允许单次获取的指定类型和名称的对象池实例
        /// </summary>
        /// <param name="objectType">对象类型</param>
        /// <param name="name">对象池名称</param>
        /// <param name="capacity">对象池的容量</param>
        /// <param name="expireTime">对象池过期时间</param>
        /// <param name="priority">对象池的优先级</param>
        /// <returns>返回创建的给定类型和名称的对象池实例</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ObjectPoolBase CreateSingleSpawnObjectPool(Type objectType, string name, int capacity, float expireTime, int priority)
        {
            return InternalCreateObjectPool(objectType, name, false, expireTime, capacity, expireTime, priority);
        }

        /// <summary>
        /// 创建允许单次获取的指定类型和名称的对象池实例
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="name">对象池名称</param>
        /// <param name="autoReleaseInterval">对象池自动释放的间隔时间</param>
        /// <param name="capacity">对象池的容量</param>
        /// <param name="expireTime">对象池过期时间</param>
        /// <param name="priority">对象池的优先级</param>
        /// <returns>返回创建的给定类型和名称的对象池实例</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IObjectPool<T> CreateSingleSpawnObjectPool<T>(string name, float autoReleaseInterval, int capacity, float expireTime, int priority) where T : ObjectBase
        {
            return InternalCreateObjectPool<T>(name, false, autoReleaseInterval, capacity, expireTime, priority);
        }

        /// <summary>
        /// 创建允许单次获取的指定类型和名称的对象池实例
        /// </summary>
        /// <param name="objectType">对象类型</param>
        /// <param name="name">对象池名称</param>
        /// <param name="autoReleaseInterval">对象池自动释放的间隔时间</param>
        /// <param name="capacity">对象池的容量</param>
        /// <param name="expireTime">对象池过期时间</param>
        /// <param name="priority">对象池的优先级</param>
        /// <returns>返回创建的给定类型和名称的对象池实例</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ObjectPoolBase CreateSingleSpawnObjectPool(Type objectType, string name, float autoReleaseInterval, int capacity, float expireTime, int priority)
        {
            return InternalCreateObjectPool(objectType, name, false, autoReleaseInterval, capacity, expireTime, priority);
        }

        /// <summary>
        /// 创建允许多次获取的指定类型的对象池实例
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <returns>返回创建的给定类型的对象池实例</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IObjectPool<T> CreateMultiSpawnObjectPool<T>() where T : ObjectBase
        {
            return InternalCreateObjectPool<T>(null, true, DefaultExpireTime, DefaultCapacity, DefaultExpireTime, DefaultPriority);
        }

        /// <summary>
        /// 创建允许多次获取的指定类型的对象池实例
        /// </summary>
        /// <param name="objectType">对象类型</param>
        /// <returns>返回创建的给定类型的对象池实例</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ObjectPoolBase CreateMultiSpawnObjectPool(Type objectType)
        {
            return InternalCreateObjectPool(objectType, null, true, DefaultExpireTime, DefaultCapacity, DefaultExpireTime, DefaultPriority);
        }

        /// <summary>
        /// 创建允许多次获取的指定类型和名称的对象池实例
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="name">对象池名称</param>
        /// <returns>返回创建的给定类型和名称的对象池实例</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IObjectPool<T> CreateMultiSpawnObjectPool<T>(string name) where T : ObjectBase
        {
            return InternalCreateObjectPool<T>(name, true, DefaultExpireTime, DefaultCapacity, DefaultExpireTime, DefaultPriority);
        }

        /// <summary>
        /// 创建允许多次获取的指定类型和名称的对象池实例
        /// </summary>
        /// <param name="objectType">对象类型</param>
        /// <param name="name">对象池名称</param>
        /// <returns>返回创建的给定类型和名称的对象池实例</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ObjectPoolBase CreateMultiSpawnObjectPool(Type objectType, string name)
        {
            return InternalCreateObjectPool(objectType, name, true, DefaultExpireTime, DefaultCapacity, DefaultExpireTime, DefaultPriority);
        }

        /// <summary>
        /// 创建允许多次获取的指定类型的对象池实例
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="capacity">对象池的容量</param>
        /// <returns>返回创建的给定类型的对象池实例</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IObjectPool<T> CreateMultiSpawnObjectPool<T>(int capacity) where T : ObjectBase
        {
            return InternalCreateObjectPool<T>(null, true, DefaultExpireTime, capacity, DefaultExpireTime, DefaultPriority);
        }

        /// <summary>
        /// 创建允许多次获取的指定类型的对象池实例
        /// </summary>
        /// <param name="objectType">对象类型</param>
        /// <param name="capacity">对象池的容量</param>
        /// <returns>返回创建的给定类型的对象池实例</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ObjectPoolBase CreateMultiSpawnObjectPool(Type objectType, int capacity)
        {
            return InternalCreateObjectPool(objectType, null, true, DefaultExpireTime, capacity, DefaultExpireTime, DefaultPriority);
        }

        /// <summary>
        /// 创建允许多次获取的指定类型的对象池实例
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="expireTime">对象池过期时间</param>
        /// <returns>返回创建的给定类型的对象池实例</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IObjectPool<T> CreateMultiSpawnObjectPool<T>(float expireTime) where T : ObjectBase
        {
            return InternalCreateObjectPool<T>(null, true, expireTime, DefaultCapacity, expireTime, DefaultPriority);
        }

        /// <summary>
        /// 创建允许多次获取的指定类型的对象池实例
        /// </summary>
        /// <param name="objectType">对象类型</param>
        /// <param name="expireTime">对象池过期时间</param>
        /// <returns>返回创建的给定类型的对象池实例</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ObjectPoolBase CreateMultiSpawnObjectPool(Type objectType, float expireTime)
        {
            return InternalCreateObjectPool(objectType, null, true, expireTime, DefaultCapacity, expireTime, DefaultPriority);
        }

        /// <summary>
        /// 创建允许多次获取的指定类型和名称的对象池实例
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="name">对象池名称</param>
        /// <param name="capacity">对象池的容量</param>
        /// <returns>返回创建的给定类型和名称的对象池实例</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IObjectPool<T> CreateMultiSpawnObjectPool<T>(string name, int capacity) where T : ObjectBase
        {
            return InternalCreateObjectPool<T>(name, true, DefaultExpireTime, capacity, DefaultExpireTime, DefaultPriority);
        }

        /// <summary>
        /// 创建允许多次获取的指定类型和名称的对象池实例
        /// </summary>
        /// <param name="objectType">对象类型</param>
        /// <param name="name">对象池名称</param>
        /// <param name="capacity">对象池的容量</param>
        /// <returns>返回创建的给定类型和名称的对象池实例</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ObjectPoolBase CreateMultiSpawnObjectPool(Type objectType, string name, int capacity)
        {
            return InternalCreateObjectPool(objectType, name, true, DefaultExpireTime, capacity, DefaultExpireTime, DefaultPriority);
        }

        /// <summary>
        /// 创建允许多次获取的指定类型和名称的对象池实例
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="name">对象池名称</param>
        /// <param name="expireTime">对象池过期时间</param>
        /// <returns>返回创建的给定类型和名称的对象池实例</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IObjectPool<T> CreateMultiSpawnObjectPool<T>(string name, float expireTime) where T : ObjectBase
        {
            return InternalCreateObjectPool<T>(name, true, expireTime, DefaultCapacity, expireTime, DefaultPriority);
        }

        /// <summary>
        /// 创建允许多次获取的指定类型和名称的对象池实例
        /// </summary>
        /// <param name="objectType">对象类型</param>
        /// <param name="name">对象池名称</param>
        /// <param name="expireTime">对象池过期时间</param>
        /// <returns>返回创建的给定类型和名称的对象池实例</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ObjectPoolBase CreateMultiSpawnObjectPool(Type objectType, string name, float expireTime)
        {
            return InternalCreateObjectPool(objectType, name, true, expireTime, DefaultCapacity, expireTime, DefaultPriority);
        }

        /// <summary>
        /// 创建允许多次获取的指定类型的对象池实例
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="capacity">对象池的容量</param>
        /// <param name="expireTime">对象池过期时间</param>
        /// <returns>返回创建的给定类型的对象池实例</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IObjectPool<T> CreateMultiSpawnObjectPool<T>(int capacity, float expireTime) where T : ObjectBase
        {
            return InternalCreateObjectPool<T>(null, true, expireTime, capacity, expireTime, DefaultPriority);
        }

        /// <summary>
        /// 创建允许多次获取的指定类型的对象池实例
        /// </summary>
        /// <param name="objectType">对象类型</param>
        /// <param name="capacity">对象池的容量</param>
        /// <param name="expireTime">对象池过期时间</param>
        /// <returns>返回创建的给定类型的对象池实例</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ObjectPoolBase CreateMultiSpawnObjectPool(Type objectType, int capacity, float expireTime)
        {
            return InternalCreateObjectPool(objectType, null, true, expireTime, capacity, expireTime, DefaultPriority);
        }

        /// <summary>
        /// 创建允许多次获取的指定类型的对象池实例
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="capacity">对象池的容量</param>
        /// <param name="priority">对象池的优先级</param>
        /// <returns>返回创建的给定类型的对象池实例</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IObjectPool<T> CreateMultiSpawnObjectPool<T>(int capacity, int priority) where T : ObjectBase
        {
            return InternalCreateObjectPool<T>(null, true, DefaultExpireTime, capacity, DefaultExpireTime, priority);
        }

        /// <summary>
        /// 创建允许多次获取的指定类型的对象池实例
        /// </summary>
        /// <param name="objectType">对象类型</param>
        /// <param name="capacity">对象池的容量</param>
        /// <param name="priority">对象池的优先级</param>
        /// <returns>返回创建的给定类型的对象池实例</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ObjectPoolBase CreateMultiSpawnObjectPool(Type objectType, int capacity, int priority)
        {
            return InternalCreateObjectPool(objectType, null, true, DefaultExpireTime, capacity, DefaultExpireTime, priority);
        }

        /// <summary>
        /// 创建允许多次获取的指定类型的对象池实例
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="expireTime">对象池过期时间</param>
        /// <param name="priority">对象池的优先级</param>
        /// <returns>返回创建的给定类型的对象池实例</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IObjectPool<T> CreateMultiSpawnObjectPool<T>(float expireTime, int priority) where T : ObjectBase
        {
            return InternalCreateObjectPool<T>(null, true, expireTime, DefaultCapacity, expireTime, priority);
        }

        /// <summary>
        /// 创建允许多次获取的指定类型的对象池实例
        /// </summary>
        /// <param name="objectType">对象类型</param>
        /// <param name="expireTime">对象池过期时间</param>
        /// <param name="priority">对象池的优先级</param>
        /// <returns>返回创建的给定类型的对象池实例</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ObjectPoolBase CreateMultiSpawnObjectPool(Type objectType, float expireTime, int priority)
        {
            return InternalCreateObjectPool(objectType, null, true, expireTime, DefaultCapacity, expireTime, priority);
        }

        /// <summary>
        /// 创建允许多次获取的指定类型和名称的对象池实例
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="name">对象池名称</param>
        /// <param name="capacity">对象池的容量</param>
        /// <param name="expireTime">对象池过期时间</param>
        /// <returns>返回创建的给定类型和名称的对象池实例</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IObjectPool<T> CreateMultiSpawnObjectPool<T>(string name, int capacity, float expireTime) where T : ObjectBase
        {
            return InternalCreateObjectPool<T>(name, true, expireTime, capacity, expireTime, DefaultPriority);
        }

        /// <summary>
        /// 创建允许多次获取的指定类型和名称的对象池实例
        /// </summary>
        /// <param name="objectType">对象类型</param>
        /// <param name="name">对象池名称</param>
        /// <param name="capacity">对象池的容量</param>
        /// <param name="expireTime">对象池过期时间</param>
        /// <returns>返回创建的给定类型和名称的对象池实例</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ObjectPoolBase CreateMultiSpawnObjectPool(Type objectType, string name, int capacity, float expireTime)
        {
            return InternalCreateObjectPool(objectType, name, true, expireTime, capacity, expireTime, DefaultPriority);
        }

        /// <summary>
        /// 创建允许多次获取的指定类型和名称的对象池实例
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="name">对象池名称</param>
        /// <param name="capacity">对象池的容量</param>
        /// <param name="priority">对象池的优先级</param>
        /// <returns>返回创建的给定类型和名称的对象池实例</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IObjectPool<T> CreateMultiSpawnObjectPool<T>(string name, int capacity, int priority) where T : ObjectBase
        {
            return InternalCreateObjectPool<T>(name, true, DefaultExpireTime, capacity, DefaultExpireTime, priority);
        }

        /// <summary>
        /// 创建允许多次获取的指定类型和名称的对象池实例
        /// </summary>
        /// <param name="objectType">对象类型</param>
        /// <param name="name">对象池名称</param>
        /// <param name="capacity">对象池的容量</param>
        /// <param name="priority">对象池的优先级</param>
        /// <returns>返回创建的给定类型和名称的对象池实例</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ObjectPoolBase CreateMultiSpawnObjectPool(Type objectType, string name, int capacity, int priority)
        {
            return InternalCreateObjectPool(objectType, name, true, DefaultExpireTime, capacity, DefaultExpireTime, priority);
        }

        /// <summary>
        /// 创建允许多次获取的指定类型和名称的对象池实例
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="name">对象池名称</param>
        /// <param name="expireTime">对象池过期时间</param>
        /// <param name="priority">对象池的优先级</param>
        /// <returns>返回创建的给定类型和名称的对象池实例</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IObjectPool<T> CreateMultiSpawnObjectPool<T>(string name, float expireTime, int priority) where T : ObjectBase
        {
            return InternalCreateObjectPool<T>(name, true, expireTime, DefaultCapacity, expireTime, priority);
        }

        /// <summary>
        /// 创建允许多次获取的指定类型和名称的对象池实例
        /// </summary>
        /// <param name="objectType">对象类型</param>
        /// <param name="name">对象池名称</param>
        /// <param name="expireTime">对象池过期时间</param>
        /// <param name="priority">对象池的优先级</param>
        /// <returns>返回创建的给定类型和名称的对象池实例</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ObjectPoolBase CreateMultiSpawnObjectPool(Type objectType, string name, float expireTime, int priority)
        {
            return InternalCreateObjectPool(objectType, name, true, expireTime, DefaultCapacity, expireTime, priority);
        }

        /// <summary>
        /// 创建允许多次获取的指定类型的对象池实例
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="capacity">对象池的容量</param>
        /// <param name="expireTime">对象池过期时间</param>
        /// <param name="priority">对象池的优先级</param>
        /// <returns>返回创建的给定类型的对象池实例</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IObjectPool<T> CreateMultiSpawnObjectPool<T>(int capacity, float expireTime, int priority) where T : ObjectBase
        {
            return InternalCreateObjectPool<T>(null, true, expireTime, capacity, expireTime, priority);
        }

        /// <summary>
        /// 创建允许多次获取的指定类型的对象池实例
        /// </summary>
        /// <param name="objectType">对象类型</param>
        /// <param name="capacity">对象池的容量</param>
        /// <param name="expireTime">对象池过期时间</param>
        /// <param name="priority">对象池的优先级</param>
        /// <returns>返回创建的给定类型的对象池实例</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ObjectPoolBase CreateMultiSpawnObjectPool(Type objectType, int capacity, float expireTime, int priority)
        {
            return InternalCreateObjectPool(objectType, null, true, expireTime, capacity, expireTime, priority);
        }

        /// <summary>
        /// 创建允许多次获取的指定类型和名称的对象池实例
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="name">对象池名称</param>
        /// <param name="capacity">对象池的容量</param>
        /// <param name="expireTime">对象池过期时间</param>
        /// <param name="priority">对象池的优先级</param>
        /// <returns>返回创建的给定类型和名称的对象池实例</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IObjectPool<T> CreateMultiSpawnObjectPool<T>(string name, int capacity, float expireTime, int priority) where T : ObjectBase
        {
            return InternalCreateObjectPool<T>(name, true, expireTime, capacity, expireTime, priority);
        }

        /// <summary>
        /// 创建允许多次获取的指定类型和名称的对象池实例
        /// </summary>
        /// <param name="objectType">对象类型</param>
        /// <param name="name">对象池名称</param>
        /// <param name="capacity">对象池的容量</param>
        /// <param name="expireTime">对象池过期时间</param>
        /// <param name="priority">对象池的优先级</param>
        /// <returns>返回创建的给定类型和名称的对象池实例</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ObjectPoolBase CreateMultiSpawnObjectPool(Type objectType, string name, int capacity, float expireTime, int priority)
        {
            return InternalCreateObjectPool(objectType, name, true, expireTime, capacity, expireTime, priority);
        }

        /// <summary>
        /// 创建允许多次获取的指定类型和名称的对象池实例
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="name">对象池名称</param>
        /// <param name="autoReleaseInterval">对象池自动释放的间隔时间</param>
        /// <param name="capacity">对象池的容量</param>
        /// <param name="expireTime">对象池过期时间</param>
        /// <param name="priority">对象池的优先级</param>
        /// <returns>返回创建的给定类型和名称的对象池实例</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IObjectPool<T> CreateMultiSpawnObjectPool<T>(string name, float autoReleaseInterval, int capacity, float expireTime, int priority) where T : ObjectBase
        {
            return InternalCreateObjectPool<T>(name, true, autoReleaseInterval, capacity, expireTime, priority);
        }

        /// <summary>
        /// 创建允许多次获取的指定类型和名称的对象池实例
        /// </summary>
        /// <param name="objectType">对象类型</param>
        /// <param name="name">对象池名称</param>
        /// <param name="autoReleaseInterval">对象池自动释放的间隔时间</param>
        /// <param name="capacity">对象池的容量</param>
        /// <param name="expireTime">对象池过期时间</param>
        /// <param name="priority">对象池的优先级</param>
        /// <returns>返回创建的给定类型和名称的对象池实例</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ObjectPoolBase CreateMultiSpawnObjectPool(Type objectType, string name, float autoReleaseInterval, int capacity, float expireTime, int priority)
        {
            return InternalCreateObjectPool(objectType, name, true, autoReleaseInterval, capacity, expireTime, priority);
        }

        /// <summary>
        /// 销毁指定类型的对象池实例
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <returns>若对象池销毁成功返回true，否则返回false</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool DestroyObjectPool<T>() where T : ObjectBase
        {
            return InternalDestroyObjectPool(new TypeNamePair(typeof(T)));
        }

        /// <summary>
        /// 销毁指定类型的对象池实例
        /// </summary>
        /// <param name="objectType">对象类型</param>
        /// <returns>若对象池销毁成功返回true，否则返回false</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool DestroyObjectPool(Type objectType)
        {
            if (null == objectType)
            {
                throw new CFrameworkException("Object type is invalid.");
            }

            if (false == typeof(ObjectBase).IsAssignableFrom(objectType))
            {
                throw new CFrameworkException("Object type '{%t}' is invalid.", objectType);
            }

            return InternalDestroyObjectPool(new TypeNamePair(objectType));
        }

        /// <summary>
        /// 销毁指定类型和名称的对象池实例
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="name">对象池名称</param>
        /// <returns>若对象池销毁成功返回true，否则返回false</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool DestroyObjectPool<T>(string name) where T : ObjectBase
        {
            return InternalDestroyObjectPool(new TypeNamePair(typeof(T), name));
        }

        /// <summary>
        /// 销毁指定类型和名称的对象池实例
        /// </summary>
        /// <param name="objectType">对象类型</param>
        /// <param name="name">对象池名称</param>
        /// <returns>若对象池销毁成功返回true，否则返回false</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool DestroyObjectPool(Type objectType, string name)
        {
            if (null == objectType)
            {
                throw new CFrameworkException("Object type is invalid.");
            }

            if (false == typeof(ObjectBase).IsAssignableFrom(objectType))
            {
                throw new CFrameworkException("Object type '{%t}' is invalid.", objectType);
            }

            return InternalDestroyObjectPool(new TypeNamePair(objectType, name));
        }

        /// <summary>
        /// 从当前管理容器中销毁指定的对象池实例
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="objectPool">目标对象池实例</param>
        /// <returns>若对象池销毁成功返回true，否则返回false</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool DestroyObjectPool<T>(IObjectPool<T> objectPool) where T : ObjectBase
        {
            if (null == objectPool)
            {
                throw new CFrameworkException("Object pool is invalid.");
            }

            return InternalDestroyObjectPool(new TypeNamePair(typeof(T), objectPool.Name));
        }

        /// <summary>
        /// 从当前管理容器中销毁指定的对象池实例
        /// </summary>
        /// <param name="objectPool">目标对象池实例</param>
        /// <returns>若对象池销毁成功返回true，否则返回false</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool DestroyObjectPool(ObjectPoolBase objectPool)
        {
            if (null == objectPool)
            {
                throw new CFrameworkException("Object pool is invalid.");
            }

            return InternalDestroyObjectPool(new TypeNamePair(objectPool.ObjectType, objectPool.Name));
        }

        /// <summary>
        /// 释放对象池中的所有可释放对象实例
        /// </summary>
        public void Release()
        {
            GetAllObjectPools(true, (List<ObjectPoolBase>) _cachedAllObjectPools);
            foreach (ObjectPoolBase objectPool in _cachedAllObjectPools)
            {
                objectPool.Release();
            }
        }

        /// <summary>
        /// 释放对象池中的所有未使用对象实例
        /// </summary>
        public void ReleaseAllUnused()
        {
            GetAllObjectPools(true, (List<ObjectPoolBase>) _cachedAllObjectPools);
            foreach (ObjectPoolBase objectPool in _cachedAllObjectPools)
            {
                objectPool.ReleaseAllUnused();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool InternalHasObjectPool(TypeNamePair typeNamePair)
        {
            return _objectPools.ContainsKey(typeNamePair);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ObjectPoolBase InternalGetObjectPool(TypeNamePair typeNamePair)
        {
            if (_objectPools.TryGetValue(typeNamePair, out ObjectPoolBase objectPool))
            {
                return objectPool;
            }

            return null;
        }

        private IObjectPool<T> InternalCreateObjectPool<T>(string name, bool allowMultiSpawn, float autoReleaseInterval, int capacity, float expireTime, int priority) where T : ObjectBase
        {
            TypeNamePair typeNamePair = new TypeNamePair(typeof(T), name);
            if (HasObjectPool<T>(name))
            {
                throw new CFrameworkException("Already exist object pool '{%i}'.", typeNamePair);
            }

            ObjectPool<T> objectPool = new ObjectPool<T>(name, allowMultiSpawn, autoReleaseInterval, capacity, expireTime, priority);
            _objectPools.Add(typeNamePair, objectPool);
            return objectPool;
        }

        private ObjectPoolBase InternalCreateObjectPool(Type objectType, string name, bool allowMultiSpawn, float autoReleaseInterval, int capacity, float expireTime, int priority)
        {
            if (null == objectType)
            {
                throw new CFrameworkException("Object type is invalid.");
            }

            if (false == typeof(ObjectBase).IsAssignableFrom(objectType))
            {
                throw new CFrameworkException("Object type '{%t}' is invalid.", objectType);
            }

            TypeNamePair typeNamePair = new TypeNamePair(objectType, name);
            if (HasObjectPool(objectType, name))
            {
                throw new CFrameworkException("Already exist object pool '{%i}'.", typeNamePair);
            }

            Type objectPoolType = typeof(ObjectPool<>).MakeGenericType(objectType);
            ObjectPoolBase objectPool = (ObjectPoolBase) Activator.CreateInstance(objectPoolType, name, allowMultiSpawn, autoReleaseInterval, capacity, expireTime, priority);
            _objectPools.Add(typeNamePair, objectPool);
            return objectPool;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool InternalDestroyObjectPool(TypeNamePair typeNamePair)
        {
            if (_objectPools.TryGetValue(typeNamePair, out ObjectPoolBase objectPool))
            {
                objectPool.Shutdown();
                return _objectPools.Remove(typeNamePair);
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int ObjectPoolComparer(ObjectPoolBase arg0, ObjectPoolBase arg1)
        {
            return arg0.Priority.CompareTo(arg1.Priority);
        }
    }
}
