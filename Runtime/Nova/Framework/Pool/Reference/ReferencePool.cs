/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
/// Copyright (C) 2022 - 2023, Shanghai Bilibili Technology Co., Ltd.
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

namespace NovaEngine
{
    /// <summary>
    /// 引用对象缓冲池句柄定义
    /// </summary>
    public static partial class ReferencePool
    {
        /// <summary>
        /// 引用对象缓冲池数据集合
        /// </summary>
        private static readonly Dictionary<SystemType, ReferenceCollection> _referenceCollections = new Dictionary<SystemType, ReferenceCollection>();

        /// <summary>
        /// 强制检查启动标识
        /// </summary>
        private static bool _strictCheckEnabled = false;

        /// <summary>
        /// 获取或设置是否开启强制检查标识
        /// </summary>
        public static bool StrictCheckEnabled
        {
            get { return _strictCheckEnabled; }
            set { _strictCheckEnabled = value; }
        }

        /// <summary>
        /// 获取引用对象缓冲池的数量
        /// </summary>
        public static int Count
        {
            get { return _referenceCollections.Count; }
        }

        /// <summary>
        /// 获取所有引用池的描述信息
        /// </summary>
        /// <returns>当前引用缓冲池的全部描述信息</returns>
        public static ReferencePoolInfo[] GetAllReferencePoolInfos()
        {
            ReferencePoolInfo[] results = null;

            lock (_referenceCollections)
            {
                int index = 0;

                results = new ReferencePoolInfo[_referenceCollections.Count];
                foreach (KeyValuePair<SystemType, ReferenceCollection> referenceCollection in _referenceCollections)
                {
                    ReferenceCollection collection = referenceCollection.Value;
                    results[index] = new ReferencePoolInfo(referenceCollection.Key,
                                                           collection.UnusedReferenceCount,
                                                           collection.UsingReferenceCount,
                                                           collection.AcquireReferenceCount,
                                                           collection.ReleaseReferenceCount,
                                                           collection.AddReferenceCount,
                                                           collection.RemoveReferenceCount);

                    index++;
                }
            }

            return results;
        }

        /// <summary>
        /// 清除所有引用池数据实例
        /// </summary>
        public static void ClearAllCollections()
        {
            lock (_referenceCollections)
            {
                foreach (KeyValuePair<SystemType, ReferenceCollection> referenceCollection in _referenceCollections)
                {
                    referenceCollection.Value.RemoveAll();
                }

                _referenceCollections.Clear();
            }
        }

        /// <summary>
        /// 从引用池获取引用对象实例
        /// </summary>
        /// <typeparam name="T">引用类型</typeparam>
        /// <returns>返回引用对象实例</returns>
        public static T Acquire<T>() where T : class, IReference, new()
        {
            return GetReferenceCollection(typeof(T)).Acquire<T>();
        }

        /// <summary>
        /// 从引用池获取引用对象实例
        /// </summary>
        /// <param name="referenceType">引用类型</param>
        /// <returns>返回引用对象实例</returns>
        public static IReference Acquire(SystemType referenceType)
        {
            InternalCheckReferenceType(referenceType);
            return GetReferenceCollection(referenceType).Acquire();
        }

        /// <summary>
        /// 将引用对象实例归还引用缓冲池
        /// </summary>
        /// <param name="reference">引用对象实例</param>
        public static void Release(IReference reference)
        {
            if (null == reference)
            {
                throw new CFrameworkException("Reference is invalid.");
            }

            SystemType referenceType = reference.GetType();
            InternalCheckReferenceType(referenceType);
            GetReferenceCollection(referenceType).Release(reference);
        }

        /// <summary>
        /// 向引用池中追加指定数量的引用对象实例
        /// </summary>
        /// <typeparam name="T">引用类型</typeparam>
        /// <param name="count">追加数量</param>
        public static void Add<T>(int count) where T : class, IReference, new()
        {
            GetReferenceCollection(typeof(T)).Add<T>(count);
        }

        /// <summary>
        /// 向引用池中追加指定数量的引用对象实例
        /// </summary>
        /// <param name="referenceType">引用类型</param>
        /// <param name="count">追加数量</param>
        public static void Add(SystemType referenceType, int count)
        {
            InternalCheckReferenceType(referenceType);
            GetReferenceCollection(referenceType).Add(count);
        }

        /// <summary>
        /// 从引用池中移除指定数量的引用对象实例
        /// </summary>
        /// <typeparam name="T">引用类型</typeparam>
        /// <param name="count">移除数量</param>
        public static void Remove<T>(int count) where T : class, IReference
        {
            GetReferenceCollection(typeof(T)).Remove(count);
        }

        /// <summary>
        /// 从引用池中移除指定数量的引用对象实例
        /// </summary>
        /// <param name="referenceType">引用类型</param>
        /// <param name="count">移除数量</param>
        public static void Remove(SystemType referenceType, int count)
        {
            InternalCheckReferenceType(referenceType);
            GetReferenceCollection(referenceType).Remove(count);
        }

        /// <summary>
        /// 从引用池中移除所有的引用对象实例
        /// </summary>
        /// <typeparam name="T">引用类型</typeparam>
        public static void RemoveAll<T>() where T : class, IReference
        {
            GetReferenceCollection(typeof(T)).RemoveAll();
        }

        /// <summary>
        /// 从引用池中移除所有的引用对象实例
        /// </summary>
        /// <param name="referenceType">引用类型</param>
        public static void RemoveAll(SystemType referenceType)
        {
            InternalCheckReferenceType(referenceType);
            GetReferenceCollection(referenceType).RemoveAll();
        }

        private static void InternalCheckReferenceType(SystemType referenceType)
        {
            if (false == _strictCheckEnabled)
            {
                return;
            }

            if (null == referenceType)
            {
                throw new CFrameworkException("Reference type is invalid");
            }

            if (false == referenceType.IsClass || referenceType.IsAbstract)
            {
                throw new CFrameworkException("Reference type is not a non-abstract class type.");
            }

            if (false == typeof(IReference).IsAssignableFrom(referenceType))
            {
                throw new CFrameworkException("Reference type '{0}' is invalid", referenceType.FullName);
            }
        }

        private static ReferenceCollection GetReferenceCollection(SystemType referenceType)
        {
            if (null == referenceType)
            {
                throw new CFrameworkException("Reference type is invalid");
            }

            ReferenceCollection referenceCollection = null;
            lock (_referenceCollections)
            {
                if (false == _referenceCollections.TryGetValue(referenceType, out referenceCollection))
                {
                    referenceCollection = new ReferenceCollection(referenceType);
                    _referenceCollections.Add(referenceType, referenceCollection);
                }
            }

            return referenceCollection;
        }
    }
}
