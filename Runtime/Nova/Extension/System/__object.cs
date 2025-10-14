/// -------------------------------------------------------------------------------
/// NovaEngine Framework
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

namespace NovaEngine
{
    /// <summary>
    /// 为系统默认的基础对象类型提供扩展接口支持
    /// </summary>
    public static class __object
    {
        /// <summary>
        /// 判断对象是否为空
        /// </summary>
        /// <param name="self">对象实例</param>
        /// <returns>若对象为空返回true，否则返回false</returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        [UnityEngine.Scripting.Preserve]
        public static bool IsNull(this object self)
        {
            return null == self;
        }

        /// <summary>
        /// 判断对象是否不为空
        /// </summary>
        /// <param name="self">对象实例</param>
        /// <returns>若对象不为空返回true，否则返回false</returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        [UnityEngine.Scripting.Preserve]
        public static bool IsNotNull(this object self)
        {
            return null != self;
        }

        /// <summary>
        /// 对象类型转换函数
        /// </summary>
        /// <typeparam name="T">目标对象类型</typeparam>
        /// <param name="self">对象实例</param>
        /// <returns>返回转换后的类型实例</returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        [UnityEngine.Scripting.Preserve]
        public static T As<T>(this object self) where T : class
        {
            return self as T;
        }

        /// <summary>
        /// 对象强制类型转换函数
        /// </summary>
        /// <typeparam name="T">目标对象类型</typeparam>
        /// <param name="self">对象实例</param>
        /// <returns>返回转换后的类型实例</returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        [UnityEngine.Scripting.Preserve]
        public static T CastTo<T>(this object self) where T : class
        {
            return (T) self;
        }
    }
}
