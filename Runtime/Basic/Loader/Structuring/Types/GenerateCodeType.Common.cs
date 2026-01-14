/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyright (C) 2024 - 2025, Hurley, Independent Studio.
/// Copyright (C) 2025, Hainan Yuanyou Information Technology Co., Ltd. Guangzhou Branch
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
using System.Reflection;
using System.Runtime.CompilerServices;

namespace GameEngine.Loader.Structuring
{
    /// <summary>
    /// 函数类型的编码结构信息对象类
    /// </summary>
    internal abstract class MethodTypeCodeInfo
    {
        /// <summary>
        /// 函数类型的完整名称
        /// </summary>
        public string Fullname { get; internal set; }
        /// <summary>
        /// 函数类型的目标对象类型
        /// </summary>
        public Type TargetType { get; internal set; }
        /// <summary>
        /// 函数类型的回调函数
        /// </summary>
        public MethodInfo Method { get; internal set; }
    }

    /// <summary>
    /// 函数类型列表容器对象类，用于管理多个函数类型的结构信息对象实例
    /// </summary>
    /// <typeparam name="T">继承自函数类型结构信息的对象类型</typeparam>
    internal sealed class MethodTypeList<T> where T : MethodTypeCodeInfo
    {
        /// <summary>
        /// 函数类型列表容器
        /// </summary>
        private IList<T> _methodTypes;

        public MethodTypeList()
        { }

        ~MethodTypeList()
        {
            Clear();
        }

        /// <summary>
        /// 新增指定的函数类型结构信息对象实例
        /// </summary>
        /// <param name="codeInfo">结构信息对象实例</param>
        public void Add(T codeInfo)
        {
            if (null == _methodTypes)
            {
                _methodTypes = new List<T>();
            }

            if (_methodTypes.Contains(codeInfo))
            {
                Debugger.Warn(LogGroupTag.CodeLoader, "新增函数类型结构信息‘{%s}’已存在于目标类型‘{%t}’的当前列表容器中，请勿重复对相同对象实例进行多次添加操作！",
                        codeInfo.Fullname, codeInfo.TargetType);
                return;
            }

            _methodTypes.Add(codeInfo);
        }

        /// <summary>
        /// 获取当前函数类型结构信息的数量
        /// </summary>
        /// <returns>返回函数类型结构信息的数量</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Count()
        {
            return _methodTypes?.Count ?? 0;
        }

        /// <summary>
        /// 检测当前函数类型结构信息数据容器是否为空
        /// </summary>
        /// <returns>若容器为空则返回true，否则返回false</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsNullOrEmpty()
        {
            return (null == _methodTypes || _methodTypes.Count == 0);
        }

        /// <summary>
        /// 获取当前指定索引值对应的函数类型结构信息对象实例
        /// </summary>
        /// <param name="index">索引值</param>
        /// <returns>返回给定索引值对应的实例，若不存在对应实例则返回null</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Get(int index)
        {
            if (null == _methodTypes || index < 0 || index >= _methodTypes.Count)
            {
                Debugger.Warn(LogGroupTag.CodeLoader, "当前传入的索引值‘{%d}’超出了目标函数类型数据列表容器的可读范围，访问列表元素失败！", index);
                return null;
            }

            return _methodTypes[index];
        }

        /// <summary>
        /// 获取当前指定名称对应的函数类型结构信息对象实例
        /// </summary>
        /// <param name="fullname">函数类型名称</param>
        /// <returns>返回给定名称对应的实例，若不存在对应实例则返回null</returns>
        public T Get(string fullname)
        {
            if (string.IsNullOrEmpty(fullname))
            {
                Debugger.Warn(LogGroupTag.CodeLoader, "当前传入的函数类型名称不能为空！");
                return null;
            }

            for (int n = 0; null != _methodTypes && n < _methodTypes.Count; ++n)
            {
                T obj = _methodTypes[n];
                if (obj.Fullname.Equals(fullname))
                {
                    return obj;
                }
            }

            return null;
        }

        /// <summary>
        /// 获取当前函数类型结构容器中的全部数据实例
        /// </summary>
        /// <returns>返回全部数据实例</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IList<T> Values()
        {
            return _methodTypes;
        }

        /// <summary>
        /// 移除所有函数类型结构信息对象实例
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear()
        {
            _methodTypes?.Clear();
            _methodTypes = null;
        }
    }
}
