/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
/// Copyring (C) 2020 - 2022, Guangzhou Xinyuan Technology Co., Ltd.
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

using System.Collections;
using System.Collections.Generic;

using SystemArray = System.Array;

namespace NovaEngine
{
    /// <summary>
    /// 带缓存的标准链表数据结构模型
    /// </summary>
    /// <typeparam name="T">指定链表的元素类型</typeparam>
    public sealed class CacheLinkedList<T> : ICollection<T>, IEnumerable<T>, ICollection, IEnumerable
    {
        /// <summary>
        /// 进行实例管理的对象链表容器
        /// </summary>
        private readonly LinkedList<T> m_linkedList;
        /// <summary>
        /// 缓存实例的对象队列容器
        /// </summary>
        private readonly Queue<LinkedListNode<T>> m_cachedNodes;

        /// <summary>
        /// 标准链表的新实例构建接口
        /// </summary>
        public CacheLinkedList()
        {
            m_linkedList = new LinkedList<T>();
            m_cachedNodes = new Queue<LinkedListNode<T>>();
        }

        /// <summary>
        /// 获取链表中实际包含的节点数量
        /// </summary>
        public int Count
        {
            get { return m_linkedList.Count; }
        }

        /// <summary>
        /// 获取链表节点缓存数量
        /// </summary>
        public int CachedCount
        {
            get { return m_cachedNodes.Count; }
        }

        /// <summary>
        /// 获取链表的第一个节点
        /// </summary>
        public LinkedListNode<T> First
        {
            get { return m_linkedList.First; }
        }

        /// <summary>
        /// 获取链表的最后一个节点
        /// </summary>
        public LinkedListNode<T> Last
        {
            get { return m_linkedList.Last; }
        }

        /// <summary>
        /// 获取当前链表是否为只读的状态标识
        /// </summary>
        public bool IsReadOnly
        {
            get { return ((ICollection<T>) m_linkedList).IsReadOnly; }
        }

        /// <summary>
        /// 获取对当前链表可用于同步访问的对象
        /// </summary>
        public object SyncRoot
        {
            get { return ((ICollection) m_linkedList).SyncRoot; }
        }

        /// <summary>
        /// 获取对当前链表是否同步访问（线程安全）的状态标识
        /// </summary>
        public bool IsSynchronized
        {
            get { return ((ICollection) m_linkedList).IsSynchronized; }
        }

        /// <summary>
        /// 将值添加到链表的结尾处
        /// </summary>
        /// <param name="value">要添加的值</param>
        public void Add(T value)
        {
            AddLast(value);
        }

        /// <summary>
        /// 在链表中指定的现有节点后添加包含指定值的新节点
        /// </summary>
        /// <param name="node">指定的现有节点</param>
        /// <param name="value">指定值</param>
        /// <returns>返回包含指定值的新节点</returns>
        public LinkedListNode<T> AddAfter(LinkedListNode<T> node, T value)
        {
            LinkedListNode<T> newNode = AcquireNode(value);
            m_linkedList.AddAfter(node, newNode);
            return newNode;
        }

        /// <summary>
        /// 在链表中指定的现有节点后添加指定的新节点
        /// </summary>
        /// <param name="node">指定的现有节点</param>
        /// <param name="newNode">指定的新节点</param>
        public void AddAfter(LinkedListNode<T> node, LinkedListNode<T> newNode)
        {
            m_linkedList.AddAfter(node, newNode);
        }

        /// <summary>
        /// 在链表中指定的现有节点前添加包含指定值的新节点
        /// </summary>
        /// <param name="node">指定的现有节点</param>
        /// <param name="value">指定值</param>
        /// <returns>返回包含指定值的新节点</returns>
        public LinkedListNode<T> AddBefore(LinkedListNode<T> node, T value)
        {
            LinkedListNode<T> newNode = AcquireNode(value);
            m_linkedList.AddBefore(node, newNode);
            return newNode;
        }

        /// <summary>
        /// 在链表中指定的现有节点前添加指定的新节点
        /// </summary>
        /// <param name="node">指定的现有节点</param>
        /// <param name="newNode">指定的新节点</param>
        public void AddBefore(LinkedListNode<T> node, LinkedListNode<T> newNode)
        {
            m_linkedList.AddBefore(node, newNode);
        }

        /// <summary>
        /// 在链表的开头处添加包含指定值的新节点
        /// </summary>
        /// <param name="value">指定值</param>
        /// <returns>返回包含指定值的新节点</returns>
        public LinkedListNode<T> AddFirst(T value)
        {
            LinkedListNode<T> node = AcquireNode(value);
            m_linkedList.AddFirst(node);
            return node;
        }

        /// <summary>
        /// 在链表的开头处添加指定的新节点
        /// </summary>
        /// <param name="node">指定的新节点</param>
        public void AddFirst(LinkedListNode<T> node)
        {
            m_linkedList.AddFirst(node);
        }

        /// <summary>
        /// 在链表的结尾处添加包含指定值的新节点
        /// </summary>
        /// <param name="value">指定值</param>
        /// <returns>返回包含指定值的新节点</returns>
        public LinkedListNode<T> AddLast(T value)
        {
            LinkedListNode<T> node = AcquireNode(value);
            m_linkedList.AddLast(node);
            return node;
        }

        /// <summary>
        /// 在链表的结尾处添加指定的新节点
        /// </summary>
        /// <param name="node">指定的新节点</param>
        public void AddLast(LinkedListNode<T> node)
        {
            m_linkedList.AddLast(node);
        }

        /// <summary>
        /// 从链表中移除所有节点
        /// </summary>
        public void Clear()
        {
            LinkedListNode<T> current = m_linkedList.First;
            while (null != current)
            {
                LinkedListNode<T> next = current.Next;
                ReleaseNode(current);
                current = next;
            }

            m_linkedList.Clear();
        }

        /// <summary>
        /// 从链表中移除所有缓存节点
        /// </summary>
        public void RemoveAllCachedNodes()
        {
            m_cachedNodes.Clear();
        }

        /// <summary>
        /// 检测当前链表中是否存在指定值
        /// </summary>
        /// <param name="value">指定值</param>
        /// <returns>若链表中存在指定值则返回true，否则返回false</returns>
        public bool Contains(T value)
        {
            return m_linkedList.Contains(value);
        }

        /// <summary>
        /// 从目标数组的指定索引处开始将整个链表复制到兼容的一维数组
        /// </summary>
        /// <param name="array">一维数组，它是从链表复制的元素的目标，数组必须具有从零开始的索引</param>
        /// <param name="index">数组中从零开始的索引，从此处开始复制</param>
        public void CopyTo(T[] array, int index)
        {
            m_linkedList.CopyTo(array, index);
        }

        /// <summary>
        /// 从特定的链表索引开始，将数组的元素复制到一个数组中
        /// </summary>
        /// <param name="array">一维数组，它是从链表复制的元素的目标，数组必须具有从零开始的索引</param>
        /// <param name="index">数组中从零开始的索引，从此处开始复制</param>
        public void CopyTo(SystemArray array, int index)
        {
            ((ICollection) m_linkedList).CopyTo(array, index);
        }

        /// <summary>
        /// 查找链表包含指定值的第一个节点
        /// </summary>
        /// <param name="value">要查找的指定值</param>
        /// <returns>返回包含指定值的第一个节点</returns>
        public LinkedListNode<T> Find(T value)
        {
            return m_linkedList.Find(value);
        }

        /// <summary>
        /// 查找链表包含指定值的最后一个节点
        /// </summary>
        /// <param name="value">要查找的指定值</param>
        /// <returns>返回包含指定值的最后一个节点</returns>
        public LinkedListNode<T> FindLast(T value)
        {
            return m_linkedList.FindLast(value);
        }

        /// <summary>
        /// 从链表中移除指定值的第一个匹配项
        /// </summary>
        /// <param name="value">指定值</param>
        /// <returns>若节点移除成功则返回true，否则返回false</returns>
        public bool Remove(T value)
        {
            LinkedListNode<T> node = m_linkedList.Find(value);
            if (null != node)
            {
                m_linkedList.Remove(node);
                ReleaseNode(node);
                return true;
            }

            return false;
        }

        /// <summary>
        /// 从链表中移除指定的节点
        /// </summary>
        /// <param name="node">指定的节点</param>
        public void Remove(LinkedListNode<T> node)
        {
            m_linkedList.Remove(node);
            ReleaseNode(node);
        }

        /// <summary>
        /// 移除位于链表开头处的节点
        /// </summary>
        public void RemoveFirst()
        {
            LinkedListNode<T> node = m_linkedList.First;
            if (null == node)
            {
                throw new CFrameworkException("First node is invalid.");
            }

            m_linkedList.RemoveFirst();
            ReleaseNode(node);
        }

        /// <summary>
        /// 移除位于链表结尾处的节点
        /// </summary>
        public void RemoveLast()
        {
            LinkedListNode<T> node = m_linkedList.Last;
            if (null == node)
            {
                throw new CFrameworkException("Last node is invalid.");
            }

            m_linkedList.RemoveLast();
            ReleaseNode(node);
        }

        private LinkedListNode<T> AcquireNode(T value)
        {
            LinkedListNode<T> node = null;
            if (m_cachedNodes.Count > 0)
            {
                node = m_cachedNodes.Dequeue();
                node.Value = value;
            }
            else
            {
                node = new LinkedListNode<T>(value);
            }

            return node;
        }

        private void ReleaseNode(LinkedListNode<T> node)
        {
            node.Value = default(T);
            m_cachedNodes.Enqueue(node);
        }

        /// <summary>
        /// 返回循环访问集合的枚举数
        /// </summary>
        /// <returns>循环访问集合的枚举数</returns>
        public Enumerator GetEnumerator()
        {
            return new Enumerator(m_linkedList);
        }

        /// <summary>
        /// 将指定值添加到ICollection的结尾处
        /// </summary>
        /// <param name="value">指定值</param>
        void ICollection<T>.Add(T value)
        {
            AddLast(value);
        }

        /// <summary>
        /// 返回循环访问集合的枚举数
        /// </summary>
        /// <returns>循环访问集合的枚举数</returns>
        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// 返回循环访问集合的枚举数
        /// </summary>
        /// <returns>循环访问集合的枚举数</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// 循环访问集合的枚举数
        /// </summary>
        public struct Enumerator : IEnumerator<T>, IEnumerator
        {
            private LinkedList<T>.Enumerator m_enumerator;

            internal Enumerator(LinkedList<T> linkedList)
            {
                if (null == linkedList)
                {
                    throw new CFrameworkException("Linked list is invalid.");
                }

                m_enumerator = linkedList.GetEnumerator();
            }

            /// <summary>
            /// 获取当前节点
            /// </summary>
            public T Current
            {
                get { return m_enumerator.Current; }
            }

            /// <summary>
            /// 获取当前的枚举数
            /// </summary>
            object IEnumerator.Current
            {
                get { return m_enumerator.Current; }
            }

            /// <summary>
            /// 清理枚举数
            /// </summary>
            public void Dispose()
            {
                m_enumerator.Dispose();
            }

            /// <summary>
            /// 获取下一个节点
            /// </summary>
            /// <returns>返回下一个节点</returns>
            public bool MoveNext()
            {
                return m_enumerator.MoveNext();
            }

            /// <summary>
            /// 重置枚举数
            /// </summary>
            void IEnumerator.Reset()
            {
                ((IEnumerator<T>) m_enumerator).Reset();
            }
        }
    }
}
