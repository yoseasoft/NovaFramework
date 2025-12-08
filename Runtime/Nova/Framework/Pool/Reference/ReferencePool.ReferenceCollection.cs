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

using System;

namespace NovaEngine
{
    using ReferenceQueue = System.Collections.Generic.Queue<IReference>;

    /// <summary>
    /// 引用对象缓冲池句柄定义
    /// </summary>
    public static partial class ReferencePool
    {
        /// <summary>
        /// 引用管理容器对象类
        /// </summary>
        private sealed class ReferenceCollection
        {
            /// <summary>
            /// 引用对象实例缓存队列
            /// </summary>
            private readonly ReferenceQueue _references;
            /// <summary>
            /// 引用对象类型标识
            /// </summary>
            private readonly Type _referenceType;
            /// <summary>
            /// 引用对象后处理信息
            /// </summary>
            private readonly ReferencePostProcessInfo _postProcessInfo;
            /// <summary>
            /// 当前缓存容器中处于使用状态的引用对象计数
            /// </summary>
            private int _usingReferenceCount;
            /// <summary>
            /// 当前缓存容器分配引用对象的计数
            /// </summary>
            private int _acquireReferenceCount;
            /// <summary>
            /// 当前缓存容器回收引用对象的计数
            /// </summary>
            private int _releaseReferenceCount;
            /// <summary>
            /// 当前缓存容器新增引用对象的计数
            /// </summary>
            private int _addReferenceCount;
            /// <summary>
            /// 当前缓存容器移除引用对象的计数
            /// </summary>
            private int _removeReferenceCount;

            /// <summary>
            /// 缓存容器的构造函数
            /// </summary>
            /// <param name="referenceType">引用对象类型</param>
            public ReferenceCollection(Type referenceType)
            {
                ReferencePostProcessInfo postProcessInfo;
                TryGetReferencePostProcessInfo(referenceType, out postProcessInfo);

                _references = new ReferenceQueue();
                _referenceType = referenceType;
                _postProcessInfo = postProcessInfo;
                _usingReferenceCount = 0;
                _acquireReferenceCount = 0;
                _releaseReferenceCount = 0;
                _addReferenceCount = 0;
                _removeReferenceCount = 0;
            }

            /// <summary>
            /// 获取引用对象类型标识
            /// </summary>
            public Type ReferenceType
            {
                get { return _referenceType; }
            }

            /// <summary>
            /// 获取当前缓存容器中可使用的引用对象实例数量
            /// </summary>
            public int UnusedReferenceCount
            {
                get { return _references.Count; }
            }

            /// <summary>
            /// 获取当前缓存容器中处于使用状态的引用对象计数
            /// </summary>
            public int UsingReferenceCount
            {
                get { return _usingReferenceCount; }
            }

            /// <summary>
            /// 获取当前缓存容器分配引用对象的计数
            /// </summary>
            public int AcquireReferenceCount
            {
                get { return _acquireReferenceCount; }
            }

            /// <summary>
            /// 获取当前缓存容器回收引用对象的计数
            /// </summary>
            public int ReleaseReferenceCount
            {
                get { return _releaseReferenceCount; }
            }

            /// <summary>
            /// 获取当前缓存容器新增引用对象的计数
            /// </summary>
            public int AddReferenceCount
            {
                get { return _addReferenceCount; }
            }

            /// <summary>
            /// 获取当前缓存容器移除引用对象的计数
            /// </summary>
            public int RemoveReferenceCount
            {
                get { return _removeReferenceCount; }
            }

            /// <summary>
            /// 从缓冲池中分配一个指定类型的引用对象实例
            /// </summary>
            /// <typeparam name="T">引用对象类型</typeparam>
            /// <returns>返回一个新分配的引用对象实例</returns>
            /// <exception cref="CFrameworkException"></exception>
            public T Acquire<T>() where T : class, IReference, new()
            {
                if (typeof(T) != _referenceType)
                {
                    throw new CFrameworkException("Type is invalid.");
                }

                T inst = null;

                _usingReferenceCount++;
                _acquireReferenceCount++;
                lock (_references)
                {
                    if (_references.Count > 0)
                    {
                        inst = (T) _references.Dequeue();
                    }
                    else
                    {
                        _addReferenceCount++;
                        inst = new T();
                    }
                }

                OnReferenceInitialize(inst);

                return inst;
            }

            /// <summary>
            /// 从缓冲池中分配一个新的引用对象实例
            /// </summary>
            /// <returns>返回一个新分配的引用对象实例</returns>
            public IReference Acquire()
            {
                IReference inst = null;

                _usingReferenceCount++;
                _acquireReferenceCount++;
                lock (_references)
                {
                    if (_references.Count > 0)
                    {
                        inst = _references.Dequeue();
                    }
                    else
                    {
                        _addReferenceCount++;
                        inst = (IReference) Activator.CreateInstance(_referenceType);
                    }
                }

                OnReferenceInitialize(inst);

                return inst;
            }

            /// <summary>
            /// 回收指定引用对象实例到缓冲池中
            /// </summary>
            /// <param name="reference">引用对象实例</param>
            /// <exception cref="CFrameworkException"></exception>
            public void Release(IReference reference)
            {
                OnReferenceCleanup(reference);

                lock (_references)
                {
                    if (_strictCheckEnabled && _references.Contains(reference))
                    {
                        throw new CFrameworkException("The reference has been released.");
                    }

                    _references.Enqueue(reference);
                }

                _releaseReferenceCount++;
                _usingReferenceCount--;
            }

            /// <summary>
            /// 向缓冲池中新增指定数量的缓存实例
            /// </summary>
            /// <typeparam name="T">缓存实例类型</typeparam>
            /// <param name="count">缓存数量</param>
            /// <exception cref="CFrameworkException"></exception>
            public void Add<T>(int count) where T : class, IReference, new()
            {
                if (typeof(T) != _referenceType)
                {
                    throw new CFrameworkException("Type is invalid.");
                }

                lock (_references)
                {
                    _addReferenceCount += count;
                    while (count > 0)
                    {
                        count--;
                        _references.Enqueue(new T());
                    }
                }
            }

            /// <summary>
            /// 向缓冲池中新增指定数量的缓存实例
            /// </summary>
            /// <param name="count">缓存数量</param>
            public void Add(int count)
            {
                lock (_references)
                {
                    _addReferenceCount += count;
                    while (count > 0)
                    {
                        count--;
                        _references.Enqueue((IReference) Activator.CreateInstance(_referenceType));
                    }
                }
            }

            /// <summary>
            /// 从缓冲池中移除指定数量的缓存实例
            /// </summary>
            /// <param name="count">缓存数量</param>
            public void Remove(int count)
            {
                lock (_references)
                {
                    if (count > _references.Count)
                    {
                        count = _references.Count;
                    }

                    _removeReferenceCount += count;
                    while (count > 0)
                    {
                        count--;
                        _references.Dequeue();
                    }
                }
            }

            /// <summary>
            /// 移除所有外部引用
            /// </summary>
            public void RemoveAll()
            {
                lock (_references)
                {
                    _removeReferenceCount += _references.Count;
                    _references.Clear();
                }
            }

            /// <summary>
            /// 引用对象初始化回调
            /// </summary>
            /// <param name="reference">引用对象实例</param>
            private void OnReferenceInitialize(IReference reference)
            {
                if (null == _postProcessInfo)
                {
                    reference.Initialize();
                }
                else
                {
                    _postProcessInfo.CreateCallback(reference);
                }
            }

            /// <summary>
            /// 引用对象清理回调
            /// </summary>
            /// <param name="reference">引用对象实例</param>
            private void OnReferenceCleanup(IReference reference)
            {
                if (null == _postProcessInfo)
                {
                    reference.Cleanup();
                }
                else
                {
                    _postProcessInfo.ReleaseCallback(reference);
                }
            }
        }
    }
}
