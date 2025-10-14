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
using SystemEncoding = System.Text.Encoding;
using SystemStream = System.IO.Stream;
using SystemAssembly = System.Reflection.Assembly;

namespace NovaEngine
{
    /// <summary>
    /// 为系统默认的对象类型提供扩展接口支持
    /// </summary>
    public static class __Type
    {
        /// <summary>
        /// 检查当前对象类型是否可以对指定类型进行赋值
        /// </summary>
        /// <param name="self">源对象类型</param>
        /// <param name="type">目标对象类型</param>
        /// <returns>可以赋值给目标对象类型则返回true，否则返回false</returns>
        public static bool IsAssignableTo(this SystemType self, SystemType type)
        {
            return type.IsAssignableFrom(self);
        }

        /// <summary>
        /// 检查当前对象类型是否可以对指定类型进行赋值
        /// </summary>
        /// <param name="self">源对象类型</param>
        /// <param name="type">目标对象类型</param>
        /// <returns>可以赋值给目标对象类型则返回true，否则返回false</returns>
        public static bool Is(this SystemType self, SystemType type)
        {
            return self.IsAssignableTo(type);
        }

        /// <summary>
        /// 检查当前对象类型是否可以对指定类型进行赋值
        /// </summary>
        /// <typeparam name="T">目标对象类型</typeparam>
        /// <param name="self">源对象类型</param>
        /// <returns>可以赋值给目标对象类型则返回true，否则返回false</returns>
        public static bool Is<T>(this SystemType self)
        {
            return self.IsAssignableTo(typeof(T));
        }

        /// <summary>
        /// 获取当前对象类型所在程序集的缩略名
        /// </summary>
        /// <param name="self">对象类型</param>
        /// <returns>返回所在程序集的缩略名</returns>
        public static string GetAssemblyShortName(this SystemType self)
        {
            return self.Assembly.GetName().Name;
        }

        /// <summary>
        /// 以文本形式获取程序集嵌入资源
        /// </summary>
        /// <param name="self">对象类型</param>
        /// <param name="charset">转换文本的字符集</param>
        /// <param name="resourceName">资源名称</param>
        /// <returns>返回资源的文本字符串</returns>
        public static string GetManifestString(this SystemType self, string charset, string resourceName)
        {
            SystemAssembly assembly = SystemAssembly.GetAssembly(self);
            SystemStream stream = assembly.GetManifestResourceStream(string.Concat(self.Namespace, Definition.CString.Dot,
                    resourceName.Replace(Definition.CString.Slash, Definition.CString.Dot)));
            if (null == stream)
            {
                return string.Empty;
            }

            int iLen = (int) stream.Length;
            byte[] bytes = new byte[iLen];
            stream.Read(bytes, 0, iLen);
            stream.Dispose();
            return SystemEncoding.GetEncoding(charset).GetString(bytes);
        }
    }
}
