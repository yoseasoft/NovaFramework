/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyright (C) 2024 - 2025, Hurley, Independent Studio.
/// Copyright (C) 2025, Hainan Yuanyou Information Tecdhnology Co., Ltd. Guangzhou Branch
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
using SystemEncoding = System.Text.Encoding;
using SystemMethodInfo = System.Reflection.MethodInfo;
using SystemSHA256 = System.Security.Cryptography.SHA256;

namespace GameEngine
{
    /// <summary>
    /// 用于框架内部的生成工具，集中管理对象内部唯一键生成所需的接口函数<br/>
    /// <br/>
    /// 注意：这里所谓的唯一键并非全局唯一，而是依托于一个目标元素，在不同元素类型的情况下唯一<br/>
    /// 一般使用目标元素类型即可达到目的，这里统一转成字符串考虑到后期通过配置控制的情况
    /// </summary>
    internal static class GenTools
    {
        /// <summary>
        /// 根据对象类型信息生成唯一的数字序列
        /// </summary>
        /// <param name="typeInfo">对象类型信息</param>
        /// <returns>返回通过对象类型信息生成的数字序列</returns>
        public static long GenUniqueId(SystemType typeInfo)
        {
            return NovaEngine.Utility.Convertion.GuidToLong(typeInfo.GUID);
        }

        /// <summary>
        /// 根据对象类型信息生成唯一的名字标签
        /// </summary>
        /// <param name="typeInfo">对象类型信息</param>
        /// <returns>返回通过对象类型信息生成的名字标签</returns>
        public static string GenUniqueName(SystemType typeInfo)
        {
            return NovaEngine.Utility.Text.GetFullName(typeInfo);
        }

        /// <summary>
        /// 根据函数信息生成唯一的名字标签
        /// </summary>
        /// <param name="methodInfo">函数对象信息</param>
        /// <returns>返回通过函数信息生成的名字标签</returns>
        public static string GenUniqueName(SystemMethodInfo methodInfo)
        {
            return NovaEngine.Utility.Text.GetFullName(methodInfo);
        }

        /// <summary>
        /// 通过生成一个基于字符串的哈希值，获取该字符串唯一的数字序列
        /// </summary>
        /// <param name="text">字符串值</param>
        /// <returns>返回通过字符串信息生成的数字序列</returns>
        private static long StringToUniqueLong(string text)
        {
            SystemSHA256 hash = SystemSHA256.Create();
            byte[] buffer = hash.ComputeHash(SystemEncoding.UTF8.GetBytes(text));
            long v = System.BitConverter.ToInt64(buffer, 0);
            hash.Dispose();

            return v;
        }
    }
}
