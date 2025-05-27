/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
/// Copyring (C) 2022 - 2023, Shanghai Bilibili Technology Co., Ltd.
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

using SystemType = System.Type;
using SystemActivator = System.Activator;

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
            private readonly ReferenceQueue m_references;
            /// <summary>
            /// 引用对象类型标识
            /// </summary>
            private readonly SystemType m_referenceType;
            /// <summary>
            /// 引用对象后处理信息
            /// </summary>
            private readonly ReferencePostProcessInfo m_postProcessInfo;
            /// <summary>
            /// 当前缓存容器中处于使用状态的引用对象计数
            /// </summary>
            private int m_usingReferenceCount;
            /// <summary>
            /// 当前缓存容器分配引用对象的计数
            /// </summary>
            private int m_acquireReferenceCount;
            /// <summary>
            /// 当前缓存容器回收引用对象的计数
            /// </summary>
            private int m_releaseReferenceCount;
            /// <summary>
            /// 当前缓存容器新增引用对象的计数
            /// </summary>
            private int m_addReferenceCount;
            /// <summary>
            /// 当前缓存容器移除引用对象的计数
            /// </summary>
            private int m_removeReferenceCount;

            /// <summary>
            /// 缓存容器的构造函数
            /// </summary>
            /// <param name="referenceType">引用对象类型</param>
            public ReferenceCollection(SystemType referenceType)
            {
                ReferencePostProcessInfo postProcessInfo;
                TryGetReferencePostProcessInfo(referenceType, out postProcessInfo);

                m_references = new ReferenceQueue();
                m_referenceType = referenceType;
                m_postProcessInfo = postProcessInfo;
                m_usingReferenceCount = 0;
                m_acquireReferenceCount = 0;
                m_releaseReferenceCount = 0;
                m_addReferenceCount = 0;
                m_removeReferenceCount = 0;
            }

            /// <summary>
            /// 获取引用对象类型标识
            /// </summary>
            public SystemType ReferenceType
            {
                get { return m_referenceType; }
            }

            /// <summary>
            /// 获取当前缓存容器中可使用的引用对象实例数量
            /// </summary>
            public int UnusedReferenceCount
            {
                get { return m_references.Count; }
            }

            /// <summary>
            /// 获取当前缓存容器中处于使用状态的引用对象计数
            /// </summary>
            public int UsingReferenceCount
            {
                get { return m_usingReferenceCount; }
            }

            /// <summary>
            /// 获取当前缓存容器分配引用对象的计数
            /// </summary>
            public int AcquireReferenceCount
            {
                get { return m_acquireReferenceCount; }
            }

            /// <summary>
            /// 获取当前缓存容器回收引用对象的计数
            /// </summary>
            public int ReleaseReferenceCount
            {
                get { return m_releaseReferenceCount; }
            }

            /// <summary>
            /// 获取当前缓存容器新增引用对象的计数
            /// </summary>
            public int AddReferenceCount
            {
                get { return m_addReferenceCount; }
            }

            /// <summary>
            /// 获取当前缓存容器移除引用对象的计数
            /// </summary>
            public int RemoveReferenceCount
            {
                get { return m_removeReferenceCount; }
            }

            /// <summary>
            /// 从缓冲池中分配一个指定类型的引用对象实例
            /// </summary>
            /// <typeparam name="T">引用对象类型</typeparam>
            /// <returns>返回一个新分配的引用对象实例</returns>
            /// <exception cref="CFrameworkException"></exception>
            public T Acquire<T>() where T : class, IReference, new()
            {
                if (typeof(T) != m_referenceType)
                {
                    throw new CFrameworkException("Type is invalid.");
                }

                T inst = null;

                m_usingReferenceCount++;
                m_acquireReferenceCount++;
                lock (m_references)
                {
                    if (m_references.Count > 0)
                    {
                        inst = (T) m_references.Dequeue();
                    }
                    else
                    {
                        m_addReferenceCount++;
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

                m_usingReferenceCount++;
                m_acquireReferenceCount++;
                lock (m_references)
                {
                    if (m_references.Count > 0)
                    {
                        inst = m_references.Dequeue();
                    }
                    else
                    {
                        m_addReferenceCount++;
                        inst = (IReference) SystemActivator.CreateInstance(m_referenceType);
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

                lock (m_references)
                {
                    if (s_strictCheckEnabled && m_references.Contains(reference))
                    {
                        throw new CFrameworkException("The reference has been released.");
                    }

                    m_references.Enqueue(reference);
                }

                m_releaseReferenceCount++;
                m_usingReferenceCount--;
            }

            /// <summary>
            /// 向缓冲池中新增指定数量的缓存实例
            /// </summary>
            /// <typeparam name="T">缓存实例类型</typeparam>
            /// <param name="count">缓存数量</param>
            /// <exception cref="CFrameworkException"></exception>
            public void Add<T>(int count) where T : class, IReference, new()
            {
                if (typeof(T) != m_referenceType)
                {
                    throw new CFrameworkException("Type is invalid.");
                }

                lock (m_references)
                {
                    m_addReferenceCount += count;
                    while (count > 0)
                    {
                        count--;
                        m_references.Enqueue(new T());
                    }
                }
            }

            /// <summary>
            /// 向缓冲池中新增指定数量的缓存实例
            /// </summary>
            /// <param name="count">缓存数量</param>
            public void Add(int count)
            {
                lock (m_references)
                {
                    m_addReferenceCount += count;
                    while (count > 0)
                    {
                        count--;
                        m_references.Enqueue((IReference) SystemActivator.CreateInstance(m_referenceType));
                    }
                }
            }

            /// <summary>
            /// 从缓冲池中移除指定数量的缓存实例
            /// </summary>
            /// <param name="count">缓存数量</param>
            public void Remove(int count)
            {
                lock (m_references)
                {
                    if (count > m_references.Count)
                    {
                        count = m_references.Count;
                    }

                    m_removeReferenceCount += count;
                    while (count > 0)
                    {
                        count--;
                        m_references.Dequeue();
                    }
                }
            }

            /// <summary>
            /// 移除所有外部引用
            /// </summary>
            public void RemoveAll()
            {
                lock (m_references)
                {
                    m_removeReferenceCount += m_references.Count;
                    m_references.Clear();
                }
            }

            /// <summary>
            /// 引用对象初始化回调
            /// </summary>
            /// <param name="reference">引用对象实例</param>
            private void OnReferenceInitialize(IReference reference)
            {
                if (null == m_postProcessInfo)
                {
                    reference.Initialize();
                }
                else
                {
                    m_postProcessInfo.CreateCallback(reference);
                }
            }

            /// <summary>
            /// 引用对象清理回调
            /// </summary>
            /// <param name="reference">引用对象实例</param>
            private void OnReferenceCleanup(IReference reference)
            {
                if (null == m_postProcessInfo)
                {
                    reference.Cleanup();
                }
                else
                {
                    m_postProcessInfo.ReleaseCallback(reference);
                }
            }
        }
    }
}
