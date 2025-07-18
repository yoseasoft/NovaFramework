/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
/// Copyright (C) 2020 - 2022, Guangzhou Xinyuan Technology Co., Ltd.
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
        private readonly LinkedListNode<T> _first;
        private readonly LinkedListNode<T> _terminal;

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

            _first = first;
            _terminal = terminal;
        }

        /// <summary>
        /// 获取双向链表当前状态是否有效
        /// </summary>
        public bool IsValid
        {
            get
            {
                return (null != _first && null != _terminal && _first != _terminal);
            }
        }

        /// <summary>
        /// 获取双向链表的开始节点
        /// </summary>
        public LinkedListNode<T> First
        {
            get { return _first; }
        }

        /// <summary>
        /// 获取双向链表的终结标记节点
        /// </summary>
        public LinkedListNode<T> Terminal
        {
            get { return _terminal; }
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
                for (LinkedListNode<T> current = _first; current != null && current != _terminal; current = current.Next)
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
            for (LinkedListNode<T> current = _first; current != null && current != _terminal; current = current.Next)
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
            private readonly DoubleLinkedList<T> _linkedListRange;
            private LinkedListNode<T> _current;
            private T _currentValue;

            internal Enumerator(DoubleLinkedList<T> range)
            {
                if (false == range.IsValid)
                {
                    throw new CFrameworkException("Range is invalid.");
                }

                _linkedListRange = range;
                _current = _linkedListRange._first;
                _currentValue = default(T);
            }

            /// <summary>
            /// 获取当前节点
            /// </summary>
            public T Current
            {
                get
                {
                    return _currentValue;
                }
            }

            /// <summary>
            /// 获取当前的枚举数
            /// </summary>
            object IEnumerator.Current
            {
                get
                {
                    return _currentValue;
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
                if (null == _current || _current == _linkedListRange._terminal)
                {
                    return false;
                }

                _currentValue = _current.Value;
                _current = _current.Next;
                return true;
            }

            /// <summary>
            /// 重置枚举数
            /// </summary>
            void IEnumerator.Reset()
            {
                _current = _linkedListRange._first;
                _currentValue = default(T);
            }
        }
    }
}
