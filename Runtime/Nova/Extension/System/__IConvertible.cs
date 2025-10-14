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

using SystemType = System.Type;
using SystemIConvertible = System.IConvertible;

namespace NovaEngine
{
    /// <summary>
    /// 为系统默认的数据转换接口提供扩展接口支持
    /// </summary>
    public static class __IConvertible
    {
        /// <summary>
        /// 类型转换
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <param name="self">源对象实例</param>
        /// <returns>返回转换类型后的对象实例</returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static T ConvertTo<T>(this SystemIConvertible self) where T : SystemIConvertible
        {
            return (T) ConvertTo(self, typeof(T));
        }

        /// <summary>
        /// 类型转换
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <param name="self">源对象实例</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>返回转换类型后的对象实例</returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static T TryConvertTo<T>(this SystemIConvertible self, T defaultValue = default) where T : SystemIConvertible
        {
            try
            {
                return (T) ConvertTo(self, typeof(T));
            }
            catch
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// 类型转换
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <param name="self">源对象实例</param>
        /// <param name="result">转换的目标对象实例</param>
        /// <returns>若转换成功返回true，否则返回false</returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static bool TryConvertTo<T>(this SystemIConvertible self, out T result) where T : SystemIConvertible
        {
            try
            {
                result = (T) ConvertTo(self, typeof(T));
                return true;
            }
            catch
            {
                result = default;
                return false;
            }
        }

        /// <summary>
        /// 类型转换
        /// </summary>
        /// <param name="self">源对象实例</param>
        /// <param name="type">目标类型</param>
        /// <param name="result">转换的目标对象实例</param>
        /// <returns>若转换成功返回true，否则返回false</returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static bool TryConvertTo(this SystemIConvertible self, SystemType type, out object result)
        {
            try
            {
                result = ConvertTo(self, type);
                return true;
            }
            catch
            {
                result = default;
                return false;
            }
        }

        /// <summary>
        /// 类型转换
        /// </summary>
        /// <param name="self">源对象实例</param>
        /// <param name="type">目标类型</param>
        /// <returns>返回转换类型后的对象实例</returns>
        public static object ConvertTo(this SystemIConvertible self, SystemType type)
        {
            if (null == self)
                return default;

            if (type.IsEnum)
            {
                return System.Enum.Parse(type, self.ToString(System.Globalization.CultureInfo.InvariantCulture));
            }

            if (type.IsGenericType && typeof(System.Nullable<>) == type.GetGenericTypeDefinition())
            {
                SystemType underlyingType = System.Nullable.GetUnderlyingType(type);
                if (null != underlyingType)
                {
                    return underlyingType.IsEnum ?
                            System.Enum.Parse(underlyingType, self.ToString(System.Globalization.CultureInfo.CurrentCulture)) :
                            System.Convert.ChangeType(self, underlyingType);
                }
            }

            return System.Convert.ChangeType(self, type);
        }
    }
}
