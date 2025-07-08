/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
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

namespace NovaEngine
{
    /// <summary>
    /// 以链表形式进行排序的字典对象类
    /// 该类继承自<see cref="System.Collections.Generic.Dictionary{TKey, TValue}"/>结构
    /// 并在此基础上维护了一份链表，对存入字典的数据进行排序
    /// </summary>
    public class SortedLinkDictionary<TKey, TValue> : Dictionary<TKey, TValue> where TKey : System.IComparable<TKey>, System.Collections.IEnumerable
    {
        /// <summary>
        /// 缓冲池的最大长度的常量定义
        /// </summary>
        private const int MAX_POOL_SIZE = 1024;

        /// <summary>
        /// 节点数据结构定义
        /// </summary>
        private class Node
        {
            public TKey Value;
            public Node Next;
        }

        /// <summary>
        /// 针对该容器内部使用的缓冲池
        /// </summary>
        private readonly Queue<Node> _pool = new Queue<Node>();

        /// <summary>
        /// 容器中节点对象的头索引
        /// </summary>
        private Node _head;

        /// <summary>
        /// 从缓冲池中分配节点对象实例的操作函数
        /// 若缓冲池为空，则新创建一个节点对象实例
        /// </summary>
        /// <returns>返回有效的节点对象实例</returns>
        private Node Fetch()
        {
            if (_pool.Count == 0)
            {
                return new Node();
            }

            return _pool.Dequeue();
        }

        /// <summary>
        /// 回收节点对象实例到缓冲池中的操作函数
        /// 若缓冲池中存储的实例已超过最大上限<see cref="MAX_POOL_SIZE"/>，则直接丢弃
        /// </summary>
        /// <param name="node">节点对象实例</param>
        private void Recycle(Node node)
        {
            node.Value = default;
            node.Next = null;

            if (_pool.Count < MAX_POOL_SIZE)
            {
                _pool.Enqueue(node);
            }
        }

        /// <summary>
        /// 向当前映射字典中新增指定的键值对
        /// 新增的节点对象实例将先从缓冲池中申请，若没有多余的缓存实例再自行创建
        /// </summary>
        /// <param name="key">数据键</param>
        /// <param name="value">数据值</param>
        public new void Add(TKey key, TValue value)
        {
            // 首次添加节点
            if (null == _head)
            {
                Node node = Fetch();
                node.Value = key;
                _head = node;
                return;
            }

            // 与头节点进行对比
            if (key.CompareTo(_head.Value) < 0)
            {
                Node node = Fetch();
                node.Value = key;
                node.Next = _head;
                _head = node;
                base.Add(key, value);
                return;
            }

            Node p = _head;
            // 遍历对比进行插入
            while (true)
            {
                Node node = null;
                if (null == p.Next)
                {
                    node = Fetch();
                    node.Value = key;
                    p.Next = node;
                    break;
                }

                int ret = key.CompareTo(p.Next.Value);

                if (ret == 0)
                {
                    break;
                }

                if (ret > 0)
                {
                    p = p.Next;
                    continue;
                }

                node = Fetch();
                node.Value = key;
                node.Next = p.Next;
                p.Next = node;
            }

            base.Add(key, value);
        }

        /// <summary>
        /// 通过指定的键移除容器中其对应的映射关联数据项
        /// 移除成功后，其对应的节点对象实例将被回收到缓冲池中
        /// </summary>
        /// <param name="key">数据键</param>
        /// <returns>从当前映射容器中移除数据项成功则返回true，否则返回false</returns>
        public new bool Remove(TKey key)
        {
            if (null == _head)
            {
                return false;
            }

            Node p = _head;
            // 遍历查找进行移除
            while (true)
            {
                if (null == p.Next)
                {
                    break;
                }

                int ret = key.CompareTo(p.Next.Value);

                if (ret == 0)
                {
                    Recycle(p.Next);
                    p.Next = p.Next.Next;
                    break;
                }

                if (ret > 0)
                {
                    p = p.Next;
                }
            }

            return base.Remove(key);
        }
    }
}
