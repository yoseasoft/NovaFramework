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

namespace NovaEngine
{
    /// <summary>
    /// 双向链表数据结构模型
    /// </summary>
    /// <typeparam name="T">指定双向链表的元素类型</typeparam>
    public struct DoubleLinkedList<T> : IEnumerable<T>, IEnumerable
    {
        private readonly LinkedListNode<T> m_first;
        private readonly LinkedListNode<T> m_terminal;

        /// <summary>
        /// 双向链表的新实例构建接口
        /// </summary>
        /// <param name="first">双向链表的开始节点</param>
        /// <param name="terminal">双向链表的终结标记节点</param>
        public DoubleLinkedList(LinkedListNode<T> first, LinkedListNode<T> terminal)
        {
            if (null == first || null == terminal || first == terminal)
            {
                throw new CFrameworkException("Range is invalid.");
            }

            m_first = first;
            m_terminal = terminal;
        }

        /// <summary>
        /// 获取双向链表当前状态是否有效
        /// </summary>
        public bool IsValid
        {
            get
            {
                return (null != m_first && null != m_terminal && m_first != m_terminal);
            }
        }

        /// <summary>
        /// 获取双向链表的开始节点
        /// </summary>
        public LinkedListNode<T> First
        {
            get { return m_first; }
        }

        /// <summary>
        /// 获取双向链表的终结标记节点
        /// </summary>
        public LinkedListNode<T> Terminal
        {
            get { return m_terminal; }
        }

        /// <summary>
        /// 获取双向链表的节点数量
        /// </summary>
        public int Count
        {
            get
            {
                if (false == IsValid)
                {
                    return 0;
                }

                int count = 0;
                for (LinkedListNode<T> current = m_first; current != null && current != m_terminal; current = current.Next)
                {
                    count++;
                }

                return count;
            }
        }

        /// <summary>
        /// 检查当前双向链表是否包含指定值
        /// </summary>
        /// <param name="value">检查的目标值</param>
        /// <returns>若当前双向链表包含指定值则返回true，否则返回false</returns>
        public bool Contains(T value)
        {
            for (LinkedListNode<T> current = m_first; current != null && current != m_terminal; current = current.Next)
            {
                if (current.Value.Equals(value))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 返回循环访问集合的枚举数
        /// </summary>
        /// <returns>循环访问集合的枚举数</returns>
        public Enumerator GetEnumerator()
        {
            return new Enumerator(this);
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
            private readonly DoubleLinkedList<T> m_linkedListRange;
            private LinkedListNode<T> m_current;
            private T m_currentValue;

            internal Enumerator(DoubleLinkedList<T> range)
            {
                if (false == range.IsValid)
                {
                    throw new CFrameworkException("Range is invalid.");
                }

                m_linkedListRange = range;
                m_current = m_linkedListRange.m_first;
                m_currentValue = default(T);
            }

            /// <summary>
            /// 获取当前节点
            /// </summary>
            public T Current
            {
                get
                {
                    return m_currentValue;
                }
            }

            /// <summary>
            /// 获取当前的枚举数
            /// </summary>
            object IEnumerator.Current
            {
                get
                {
                    return m_currentValue;
                }
            }

            /// <summary>
            /// 清理枚举数
            /// </summary>
            public void Dispose()
            {
            }

            /// <summary>
            /// 获取下一个节点
            /// </summary>
            /// <returns>返回下一个节点</returns>
            public bool MoveNext()
            {
                if (null == m_current || m_current == m_linkedListRange.m_terminal)
                {
                    return false;
                }

                m_currentValue = m_current.Value;
                m_current = m_current.Next;
                return true;
            }

            /// <summary>
            /// 重置枚举数
            /// </summary>
            void IEnumerator.Reset()
            {
                m_current = m_linkedListRange.m_first;
                m_currentValue = default(T);
            }
        }
    }
}
