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
using System.Runtime.CompilerServices;

namespace GameEngine.Profiler.Statistics
{
    /// <summary>
    /// 统计模块通用基类，对统计类接口提供一些标准形式的封装
    /// </summary>
    internal abstract class BaseStat<TObject, TRecord> : StatSingleton<TObject, TRecord>, IStat
        where TObject : class, IStat, new()
        where TRecord : StatInfo
    {
        /// <summary>
        /// 初始化统计模块实例的回调接口
        /// </summary>
        // protected override void OnInitialize() { }

        /// <summary>
        /// 清理统计模块实例的回调接口
        /// </summary>
        // protected override void OnCleanup() { }

        /// <summary>
        /// 回收统计模块实例的回调接口
        /// </summary>
        // protected override void OnDump() { }

        /// <summary>
        /// 获取当前统计模块实例中指定标识对应的记录信息
        /// </summary>
        /// <param name="uid">统计信息标识</param>
        /// <returns>返回给定标识对应的统计项信息</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public StatInfo GetStateInfoByUid(int uid)
        {
            return TryGetValue(uid);
        }

        /// <summary>
        /// 获取当前所有视图访问的统计信息
        /// </summary>
        /// <returns>返回所有的操作访问统计信息</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IReadOnlyList<StatInfo> GetAllStatInfos()
        {
            return TryGetAllValues();
        }
    }
}
