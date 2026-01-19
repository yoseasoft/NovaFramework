/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyright (C) 2020 - 2022, Guangzhou Xinyuan Technology Co., Ltd.
/// Copyright (C) 2022 - 2023, Shanghai Bilibili Technology Co., Ltd.
/// Copyright (C) 2023 - 2024, Guangzhou Shiyue Network Technology Co., Ltd.
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
using System.Runtime.CompilerServices;

namespace GameEngine.Profiler.Statistics
{
    /// <summary>
    /// 业务框架统计信息基础对象类，用于标识该类是一个统计信息对象的类，并提供部分通用属性的访问接口
    /// </summary>
    public abstract class StatInfo
    {
        /// <summary>
        /// 统计信息对象唯一标识
        /// </summary>
        private readonly int _uid;

        /// <summary>
        /// 统计信息对象创建时间
        /// </summary>
        private readonly DateTime _createTime;
        /// <summary>
        /// 统计信息对象释放时间
        /// </summary>
        private DateTime _releaseTime;
        /// <summary>
        /// 统计信息对象访问时间
        /// </summary>
        private DateTime _accessTime;

        public int Uid => _uid;

        public DateTime CreateTime => _createTime;
        public DateTime ReleaseTime => _releaseTime;
        public DateTime AccessTime => _accessTime;

        protected StatInfo(int uid)
        {
            _uid = uid;
            _createTime = Now();
            _releaseTime = DateTime.MinValue;
            _accessTime = _createTime;
        }

        internal void Release()
        {
            _releaseTime = Now();
        }

        internal void Access()
        {
            _accessTime = Now();
        }

        /// <summary>
        /// 获取当前系统时间
        /// </summary>
        /// <returns>返回当前系统时间</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected DateTime Now()
        {
            return DateTime.UtcNow;
        }
    }
}
