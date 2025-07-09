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

namespace NovaEngine
{
    /// <summary>
    /// 引用计数器功能的封装对象类
    /// 该类仅用于计数管理，可选择安全或非安全模式的计数统计供用户选择
    /// </summary>
    public sealed class ReferenceCount
    {
        /// <summary>
        /// 初始引用计数值
        /// </summary>
        private int _count = 0;

        private readonly object _locked = new object();

        /// <summary>
        /// 获取对象实例当前引用计数值
        /// </summary>
        public int Count => _count;

        /// <summary>
        /// 检测对象实例当前是否被引用
        /// </summary>
        public bool IsUnused => (_count == 0);

        /// <summary>
        /// 增加对象实例的引用计数值
        /// </summary>
        public void Increase()
        {
            ++_count;
        }

        /// <summary>
        /// 减少对象实例的引用计数值
        /// </summary>
        public void Decrease()
        {
            --_count;
        }

        /// <summary>
        /// 重置对象实例的引用计数值
        /// </summary>
        public void Reset()
        {
            _count = 0;
        }
    }
}
