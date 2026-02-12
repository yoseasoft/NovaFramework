/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyright (C) 2024 - 2025, Hurley, Independent Studio.
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

namespace GameEngine
{
    /// 用于框架内部的生成工具
    internal static partial class _Generator
    {
        /// <summary>
        /// 生成实体类型对象实例的唯一会话标识
        /// </summary>
        /// <returns>返回唯一会话标识</returns>
        public static int GenBeanId()
        {
            return GenSessionId("GameEngine.Bean");
        }

        /// <summary>
        /// 生成标记类型对象实例的唯一会话标识
        /// </summary>
        /// <returns>返回唯一会话标识</returns>
        public static int GenSymbolId()
        {
            return GenSessionId("GameEngine.Symbol");
        }

        /// <summary>
        /// 生成唯一会话标识<br/>
        /// 该方法为非安全模式，适用于主线程逻辑调度
        /// </summary>
        /// <param name="name">标识名称</param>
        /// <returns>返回唯一会话标识</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int GenSessionId(string name)
        {
            return NovaEngine.Session.UnsafeNextSession(name);
        }
    }
}
