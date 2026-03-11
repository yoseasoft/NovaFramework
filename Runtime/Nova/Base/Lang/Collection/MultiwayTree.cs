/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
/// Copyright (C) 2025 - 2026, Hainan Yuanyou Information Technology Co., Ltd. Guangzhou Branch
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
using System.Runtime.CompilerServices;

namespace NovaEngine
{
    /// <summary>
    /// 多叉树数据结构对象类
    /// 该树的同级节点无需排序，通过键进行索引
    /// </summary>
    /// <typeparam name="TKey">节点键类型</typeparam>
    /// <typeparam name="TValue">节点数据类型</typeparam>
    public class MultiwayTree<TKey, TValue> where TKey : notnull where TValue : class
    {
        /// <summary>
        /// 内部节点映射容器
        /// </summary>
        private readonly IDictionary<TKey, TreeNode> _nodes;

        public MultiwayTree()
        {
            _nodes = new Dictionary<TKey, TreeNode>();
        }

        ~MultiwayTree()
        {
            Clear();
        }

        /// <summary>
        /// 添加节点数据到指定索引路径
        /// </summary>
        /// <param name="keys">索引键</param>
        /// <param name="value">节点值</param>
        public void Add(TKey[] keys, TValue value)
        {
            Debugger.Assert(keys.Length > 0, NovaEngine.ErrorText.InvalidArguments);

            TKey key = keys[0];
            if (false == _nodes.TryGetValue(key, out TreeNode sub_node))
            {
                sub_node = new TreeNode();
                _nodes.Add(key, sub_node);
            }

            sub_node[keys] = value;
        }

        /// <summary>
        /// 获取指定索引路径下的节点数据
        /// </summary>
        /// <param name="keys">索引键</param>
        /// <returns>返回索引的节点数据</returns>
        public TValue Get(TKey[] keys)
        {
            Debugger.Assert(keys.Length > 0, NovaEngine.ErrorText.InvalidArguments);

            TKey key = keys[0];
            if (false == _nodes.TryGetValue(key, out TreeNode sub_node))
            {
                return null;
            }

            return sub_node[keys];
        }

        /// <summary>
        /// 尝试获取指定索引路径下的节点数据
        /// </summary>
        /// <param name="keys">索引键</param>
        /// <param name="value">数据实例</param>
        /// <returns>若获取数据实例成功则返回true，否则返回false</returns>
        public bool TryGetValue(TKey[] keys, out TValue value)
        {
            Debugger.Assert(keys.Length > 0, NovaEngine.ErrorText.InvalidArguments);

            TKey key = keys[0];
            if (false == _nodes.TryGetValue(key, out TreeNode sub_node))
            {
                value = null;
                return false;
            }

            return sub_node.TryGetValue(keys, out value);
        }

        /// <summary>
        /// 检测指定索引路径下是否存在合法的节点数据
        /// </summary>
        /// <param name="keys">索引键</param>
        /// <returns>若存在节点数据则返回true，否则返回false</returns>
        public bool ContainsKey(TKey[] keys)
        {
            Debugger.Assert(keys.Length > 0, NovaEngine.ErrorText.InvalidArguments);

            TKey key = keys[0];
            if (false == _nodes.TryGetValue(key, out TreeNode sub_node))
            {
                return false;
            }

            if (keys.Length == 1)
            {
                return true;
            }

            return sub_node.ContainsKey(keys);
        }

        /// <summary>
        /// 移除指定索引路径下的节点数据
        /// </summary>
        /// <param name="keys">索引键</param>
        public void Remove(TKey[] keys)
        {
            Debugger.Assert(keys.Length > 0, NovaEngine.ErrorText.InvalidArguments);

            TKey key = keys[0];
            if (false == _nodes.TryGetValue(key, out TreeNode sub_node))
            {
                return;
            }

            if (keys.Length == 1)
            {
                sub_node.Clear();
                _nodes.Remove(key);
                return;
            }

            sub_node.Remove(keys);
        }

        /// <summary>
        /// 清空多叉树的全部节点数据
        /// </summary>
        public void Clear()
        {
            foreach (KeyValuePair<TKey, TreeNode> kvp in _nodes)
            {
                kvp.Value.Clear();
            }

            _nodes.Clear();
        }

        /// <summary>
        /// 内部树节点对象类
        /// </summary>
        private sealed class TreeNode
        {
            /// <summary>
            /// 值节点管理容器
            /// </summary>
            private TValue _value;
            /// <summary>
            /// 树节点管理容器
            /// </summary>
            private IDictionary<TKey, TreeNode> _childs;

            /// <summary>
            /// 获取节点的值数据
            /// </summary>
            public TValue Value => _value;

            /// <summary>
            /// 检测当前节点是否为空（无值且无子节点）
            /// </summary>
            public bool IsEmpty => null == _value && (null == _childs || _childs.Count == 0);

            /// <summary>
            /// 设置当前节点的值数据
            /// </summary>
            /// <param name="value">节点值</param>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void SetValue(TValue value)
            {
                Debugger.Assert(null == _value, NovaEngine.ErrorText.InvalidOperation);

                _value = value;
            }

            /// <summary>
            /// 添加节点数据到指定索引序列
            /// </summary>
            /// <param name="keys">索引键</param>
            /// <param name="value">节点值</param>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Add(TKey[] keys, TValue value)
            {
                Add(keys, 0, value);
            }

            /// <summary>
            /// 添加节点数据到指定索引序列
            /// </summary>
            /// <param name="keys">索引键</param>
            /// <param name="index">索引位置</param>
            /// <param name="value">节点值</param>
            private void Add(TKey[] keys, int index, TValue value)
            {
                Debugger.Assert(index >= 0 && index < keys.Length, NovaEngine.ErrorText.InvalidArguments);

                if (keys.Length - index == 1)
                {
                    SetValue(value);
                    return;
                }

                int next_index = index + 1;
                TKey k = keys[next_index];

                _childs ??= new Dictionary<TKey, TreeNode>();
                if (false == _childs.TryGetValue(k, out TreeNode sub_node))
                {
                    // 构建一个新节点实例
                    sub_node = new TreeNode();
                    _childs.Add(k, sub_node);
                }

                sub_node.Add(keys, next_index, value);
            }

            /// <summary>
            /// 获取指定索引序列下的节点数据
            /// </summary>
            /// <param name="keys">索引键</param>
            /// <returns>返回索引的节点数据</returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public TValue Get(TKey[] keys)
            {
                return Get(keys, 0);
            }

            /// <summary>
            /// 获取指定索引序列下的节点数据
            /// </summary>
            /// <param name="keys">索引键</param>
            /// <param name="index">索引位置</param>
            /// <returns>返回索引的节点数据</returns>
            private TValue Get(TKey[] keys, int index)
            {
                Debugger.Assert(index >= 0 && index < keys.Length, NovaEngine.ErrorText.InvalidArguments);

                if (keys.Length - index == 1)
                {
                    return _value;
                }

                int next_index = index + 1;
                TKey k = keys[next_index];
                if (null == _childs || false == _childs.TryGetValue(k, out TreeNode sub_node))
                {
                    Debugger.Warn("Could not found any tree node with target key '{%s}', getting node value failed.", k.ToString());
                    return null;
                }

                return sub_node.Get(keys, next_index);
            }

            /// <summary>
            /// 尝试获取指定索引序列下的节点数据
            /// </summary>
            /// <param name="keys">索引键</param>
            /// <param name="value">节点数据</param>
            /// <returns>若获取节点数据成功则返回true，否则返回false</returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool TryGetValue(TKey[] keys, out TValue value)
            {
                return TryGetValue(keys, 0, out value);
            }

            /// <summary>
            /// 尝试获取指定索引序列下的节点数据
            /// </summary>
            /// <param name="keys">索引键</param>
            /// <param name="index">索引位置</param>
            /// <param name="value">节点数据</param>
            /// <returns>若获取节点数据成功则返回true，否则返回false</returns>
            private bool TryGetValue(TKey[] keys, int index, out TValue value)
            {
                Debugger.Assert(index >= 0 && index < keys.Length, NovaEngine.ErrorText.InvalidArguments);

                if (keys.Length - index == 1)
                {
                    value = _value;
                    return true;
                }

                int next_index = index + 1;
                TKey k = keys[next_index];
                if (null == _childs || false == _childs.TryGetValue(k, out TreeNode sub_node))
                {
                    value = null;
                    return false;
                }

                return sub_node.TryGetValue(keys, next_index, out value);
            }

            /// <summary>
            /// 检测指定索引序列下的是否存在合法的节点数据
            /// </summary>
            /// <param name="keys">索引键</param>
            /// <returns>若节点数据存在则返回true，否则返回false</returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool ContainsKey(TKey[] keys)
            {
                return ContainsKey(keys, 0);
            }

            /// <summary>
            /// 检测指定索引序列下的是否存在合法的节点数据
            /// </summary>
            /// <param name="keys">索引键</param>
            /// <param name="index">索引位置</param>
            /// <returns>若节点数据存在则返回true，否则返回false</returns>
            private bool ContainsKey(TKey[] keys, int index)
            {
                Debugger.Assert(index >= 0 && index < keys.Length, NovaEngine.ErrorText.InvalidArguments);

                if (keys.Length - index == 1)
                {
                    return true;
                }

                int next_index = index + 1;
                TKey k = keys[next_index];
                if (null == _childs || false == _childs.TryGetValue(k, out TreeNode sub_node))
                {
                    return false;
                }

                return sub_node.ContainsKey(keys, next_index);
            }

            /// <summary>
            /// 移除指定索引序列下的节点数据
            /// </summary>
            /// <param name="keys">索引键</param>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Remove(TKey[] keys)
            {
                Remove(keys, 0);
            }

            /// <summary>
            /// 移除指定索引序列下的节点数据
            /// </summary>
            /// <param name="keys">索引键</param>
            /// <param name="index">索引位置</param>
            private void Remove(TKey[] keys, int index)
            {
                Debugger.Assert(index >= 0 && index < keys.Length, NovaEngine.ErrorText.InvalidArguments);

                int next_index = index + 1;
                TKey k = keys[next_index];
                if (null == _childs || false == _childs.TryGetValue(k, out TreeNode sub_node))
                {
                    Debugger.Warn("Could not found any tree node with target key '{%s}', removed it failed.", k.ToString());
                    return;
                }

                if (keys.Length - next_index == 1)
                {
                    sub_node.Clear();
                    _childs.Remove(k);
                    return;
                }

                sub_node.Remove(keys, next_index);
            }

            /// <summary>
            /// 清空树节点的全部数据
            /// </summary>
            public void Clear()
            {
                _value = null;

                if (null != _childs)
                {
                    foreach (KeyValuePair<TKey, TreeNode> kvp in _childs)
                    {
                        kvp.Value.Clear();
                    }
                    _childs.Clear();
                    _childs = null;
                }
            }

            /// <summary>
            /// 获取多叉树中指定主键的数据值
            /// </summary>
            /// <param name="keys">索引键</param>
            /// <returns>返回指定键的数据值</returns>
            public TValue this[TKey[] keys]
            {
                get
                {
                    return Get(keys);
                }
                set
                {
                    Add(keys, value);
                }
            }
        }
    }
}
