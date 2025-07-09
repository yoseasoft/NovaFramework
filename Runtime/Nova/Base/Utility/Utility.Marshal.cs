/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
/// Copyright (C) 2022 - 2023, Shanghai Bilibili Technology Co., Ltd.
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

using SystemIntPtr = System.IntPtr;

namespace NovaEngine
{
    /// <summary>
    /// 实用函数集合工具类
    /// </summary>
    public static partial class Utility
    {
        /// <summary>
        /// 数据转换相关实用函数集合
        /// </summary>
        public static class Marshal
        {
            private const int CACHED_BLOCK_SIZE = 1024 * 4;

            private static SystemIntPtr _cachedHGlobalPtr = SystemIntPtr.Zero;
            private static int _cachedHGlobalSize = 0;

            /// <summary>
            /// 获取缓存的指向从进程的非托管内存中分配的内存的指针
            /// </summary>
            internal static SystemIntPtr CachedHGlobalPtr
            {
                get { return _cachedHGlobalPtr; }
            }

            /// <summary>
            /// 获取缓存的从进程的非托管内存中分配的内存的大小
            /// </summary>
            public static int CachedHGlobalSize
            {
                get { return _cachedHGlobalSize; }
            }

            /// <summary>
            /// 确保从进程的非托管内存中分配足够大小的内存并缓存
            /// </summary>
            /// <param name="ensureSize">要确保从进程的非托管内存中分配内存的大小</param>
            public static void EnsureCachedHGlobalSize(int ensureSize)
            {
                if (ensureSize < 0)
                {
                    throw new CFrameworkException("Ensure size is invalid.");
                }

                if (_cachedHGlobalPtr == SystemIntPtr.Zero || _cachedHGlobalSize < ensureSize)
                {
                    FreeCachedHGlobal();
                    int size = (ensureSize - 1 + CACHED_BLOCK_SIZE) / CACHED_BLOCK_SIZE * CACHED_BLOCK_SIZE;
                    _cachedHGlobalPtr = System.Runtime.InteropServices.Marshal.AllocHGlobal(size);
                    _cachedHGlobalSize = size;
                }
            }

            /// <summary>
            /// 释放缓存的从进程的非托管内存中分配的内存
            /// </summary>
            public static void FreeCachedHGlobal()
            {
                if (_cachedHGlobalPtr != SystemIntPtr.Zero)
                {
                    System.Runtime.InteropServices.Marshal.FreeHGlobal(_cachedHGlobalPtr);
                    _cachedHGlobalPtr = SystemIntPtr.Zero;
                    _cachedHGlobalSize = 0;
                }
            }

            /// <summary>
            /// 将数据从对象转换为二进制流
            /// </summary>
            /// <typeparam name="T">要转换的对象的类型</typeparam>
            /// <param name="structure">要转换的对象</param>
            /// <returns>返回存储转换结果的二进制流</returns>
            public static byte[] StructureToBytes<T>(T structure)
            {
                return StructureToBytes(structure, System.Runtime.InteropServices.Marshal.SizeOf(typeof(T)));
            }

            /// <summary>
            /// 将数据从对象转换为二进制流
            /// </summary>
            /// <typeparam name="T">要转换的对象的类型</typeparam>
            /// <param name="structure">要转换的对象</param>
            /// <param name="structureSize">要转换的对象的大小</param>
            /// <returns>返回存储转换结果的二进制流</returns>
            internal static byte[] StructureToBytes<T>(T structure, int structureSize)
            {
                if (structureSize < 0)
                {
                    throw new CFrameworkException("Structure size is invalid.");
                }

                EnsureCachedHGlobalSize(structureSize);
                System.Runtime.InteropServices.Marshal.StructureToPtr(structure, _cachedHGlobalPtr, true);
                byte[] result = new byte[structureSize];
                System.Runtime.InteropServices.Marshal.Copy(_cachedHGlobalPtr, result, 0, structureSize);
                return result;
            }

            /// <summary>
            /// 将数据从对象转换为二进制流
            /// </summary>
            /// <typeparam name="T">要转换的对象的类型</typeparam>
            /// <param name="structure">要转换的对象</param>
            /// <param name="result">存储转换结果的二进制流</param>
            public static void StructureToBytes<T>(T structure, byte[] result)
            {
                StructureToBytes(structure, System.Runtime.InteropServices.Marshal.SizeOf(typeof(T)), result, 0);
            }

            /// <summary>
            /// 将数据从对象转换为二进制流
            /// </summary>
            /// <typeparam name="T">要转换的对象的类型</typeparam>
            /// <param name="structure">要转换的对象</param>
            /// <param name="structureSize">要转换的对象的大小</param>
            /// <param name="result">存储转换结果的二进制流</param>
            internal static void StructureToBytes<T>(T structure, int structureSize, byte[] result)
            {
                StructureToBytes(structure, structureSize, result, 0);
            }

            /// <summary>
            /// 将数据从对象转换为二进制流
            /// </summary>
            /// <typeparam name="T">要转换的对象的类型</typeparam>
            /// <param name="structure">要转换的对象</param>
            /// <param name="result">存储转换结果的二进制流</param>
            /// <param name="startIndex">写入存储转换结果的二进制流的起始位置</param>
            public static void StructureToBytes<T>(T structure, byte[] result, int startIndex)
            {
                StructureToBytes(structure, System.Runtime.InteropServices.Marshal.SizeOf(typeof(T)), result, startIndex);
            }

            /// <summary>
            /// 将数据从对象转换为二进制流
            /// </summary>
            /// <typeparam name="T">要转换的对象的类型</typeparam>
            /// <param name="structure">要转换的对象</param>
            /// <param name="structureSize">要转换的对象的大小</param>
            /// <param name="result">存储转换结果的二进制流</param>
            /// <param name="startIndex">写入存储转换结果的二进制流的起始位置</param>
            internal static void StructureToBytes<T>(T structure, int structureSize, byte[] result, int startIndex)
            {
                if (structureSize < 0)
                {
                    throw new CFrameworkException("Structure size is invalid.");
                }

                if (result == null)
                {
                    throw new CFrameworkException("Result is invalid.");
                }

                if (startIndex < 0)
                {
                    throw new CFrameworkException("Start index is invalid.");
                }

                if (startIndex + structureSize > result.Length)
                {
                    throw new CFrameworkException("Result length is not enough.");
                }

                EnsureCachedHGlobalSize(structureSize);
                System.Runtime.InteropServices.Marshal.StructureToPtr(structure, _cachedHGlobalPtr, true);
                System.Runtime.InteropServices.Marshal.Copy(_cachedHGlobalPtr, result, startIndex, structureSize);
            }

            /// <summary>
            /// 将数据从二进制流转换为对象
            /// </summary>
            /// <typeparam name="T">要转换的对象的类型</typeparam>
            /// <param name="buffer">要转换的二进制流</param>
            /// <returns>返回存储转换结果的对象</returns>
            public static T BytesToStructure<T>(byte[] buffer)
            {
                return BytesToStructure<T>(System.Runtime.InteropServices.Marshal.SizeOf(typeof(T)), buffer, 0);
            }

            /// <summary>
            /// 将数据从二进制流转换为对象
            /// </summary>
            /// <typeparam name="T">要转换的对象的类型</typeparam>
            /// <param name="buffer">要转换的二进制流</param>
            /// <param name="startIndex">读取要转换的二进制流的起始位置</param>
            /// <returns>返回存储转换结果的对象</returns>
            public static T BytesToStructure<T>(byte[] buffer, int startIndex)
            {
                return BytesToStructure<T>(System.Runtime.InteropServices.Marshal.SizeOf(typeof(T)), buffer, startIndex);
            }

            /// <summary>
            /// 将数据从二进制流转换为对象
            /// </summary>
            /// <typeparam name="T">要转换的对象的类型</typeparam>
            /// <param name="structureSize">要转换的对象的大小</param>
            /// <param name="buffer">要转换的二进制流</param>
            /// <returns>返回存储转换结果的对象</returns>
            internal static T BytesToStructure<T>(int structureSize, byte[] buffer)
            {
                return BytesToStructure<T>(structureSize, buffer, 0);
            }

            /// <summary>
            /// 将数据从二进制流转换为对象
            /// </summary>
            /// <typeparam name="T">要转换的对象的类型</typeparam>
            /// <param name="structureSize">要转换的对象的大小</param>
            /// <param name="buffer">要转换的二进制流</param>
            /// <param name="startIndex">读取要转换的二进制流的起始位置</param>
            /// <returns>返回存储转换结果的对象</returns>
            internal static T BytesToStructure<T>(int structureSize, byte[] buffer, int startIndex)
            {
                if (structureSize < 0)
                {
                    throw new CFrameworkException("Structure size is invalid.");
                }

                if (buffer == null)
                {
                    throw new CFrameworkException("Buffer is invalid.");
                }

                if (startIndex < 0)
                {
                    throw new CFrameworkException("Start index is invalid.");
                }

                if (startIndex + structureSize > buffer.Length)
                {
                    throw new CFrameworkException("Buffer length is not enough.");
                }

                EnsureCachedHGlobalSize(structureSize);
                System.Runtime.InteropServices.Marshal.Copy(buffer, startIndex, _cachedHGlobalPtr, structureSize);
                return (T) System.Runtime.InteropServices.Marshal.PtrToStructure(_cachedHGlobalPtr, typeof(T));
            }
        }
    }
}
