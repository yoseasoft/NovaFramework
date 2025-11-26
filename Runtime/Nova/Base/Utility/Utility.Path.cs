/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
/// Copyright (C) 2022 - 2023, Shanghai Bilibili Technology Co., Ltd.
/// Copyright (C) 2023, Guangzhou Shiyue Network Technology Co., Ltd.
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

using System.IO;
using System.Text;
using System.Runtime.CompilerServices;
using System.Customize.Extension;

using UnityTextAsset = UnityEngine.TextAsset;
using UnityResources = UnityEngine.Resources;

namespace NovaEngine
{
    /// <summary>
    /// 实用函数集合工具类
    /// </summary>
    public static partial class Utility
    {
        /// <summary>
        /// 文件目录路径相关实用函数集合
        /// </summary>
        public static class Path
        {
            /// <summary>
            /// 获取规范的路径
            /// </summary>
            /// <param name="path">要规范的路径</param>
            /// <returns>返回规范的路径</returns>
            public static string GetRegularPath(string path)
            {
                if (path.IsNullOrEmpty())
                {
                    return null;
                }

                return path.Replace(Definition.CCharacter.Backslash, Definition.CCharacter.Slash);
            }

            /// <summary>
            /// 获取远程格式的路径（带有file:// 或 http:// 前缀）
            /// </summary>
            /// <param name="path">原始路径</param>
            /// <returns>返回远程格式路径</returns>
            public static string GetRemotePath(string path)
            {
                string regularPath = GetRegularPath(path);
                if (regularPath.IsNullOrEmpty())
                {
                    return null;
                }

                return regularPath.Contains("://") ? regularPath : ("file:///" + regularPath).Replace("file:////", "file:///");
            }

            /// <summary>
            /// 从指定路径的末端开始截断给定层级的目录，然后返回截断后的目录地址
            /// </summary>
            /// <param name="path">文件路径</param>
            /// <param name="level">截断层级</param>
            /// <returns>返回截断后的目录地址</returns>
            public static string TruncatePath(string path, int level)
            {
                for (int i = 0; i < level; i++)
                {
                    path = System.IO.Path.GetDirectoryName(path);
                    if (path.IsNullOrEmpty())
                        break;
                }

                return path;
            }

            /// <summary>
            /// 判断目标目录是否存在
            /// </summary>
            /// <param name="path">目标目录名称</param>
            /// <returns>若目标目录存在则返回true，否则返回false</returns>
            public static bool IsDirectoryExists(string path)
            {
                if (path.IsNullOrEmpty())
                {
                    return false;
                }

                return Directory.Exists(path);
            }

            /// <summary>
            /// 安全模式创建目标目录
            /// </summary>
            /// <param name="path">目标目录名称</param>
            public static void CreateDirectory(string path)
            {
                if (false == Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
            }

            /// <summary>
            /// 根据指定文件路径创建文件路径相关目录
            /// </summary>
            /// <param name="path">文件路径名称</param>
            public static void CreateDirectoryOnFilePath(string path)
            {
                if (path.IsNullOrEmpty())
                {
                    return;
                }

                FileInfo f = new FileInfo(path);
                DirectoryInfo dir = f.Directory;
                if (false == dir.Exists)
                {
                    Directory.CreateDirectory(dir.FullName);
                }
            }

            /// <summary>
            /// 安全模式删除指定路径目录文件
            /// </summary>
            /// <param name="path">目标目录名称</param>
            public static void DeleteDirectory(string path)
            {
                if (false == IsDirectoryExists(path))
                {
                    return;
                }

                string[] files = Directory.GetFiles(path);
                string[] dirs = Directory.GetDirectories(path);

                foreach (string file in files)
                {
                    File.SetAttributes(file, FileAttributes.Normal);
                    File.Delete(file);
                }

                foreach (string dir in dirs)
                {
                    DeleteDirectory(dir);
                }

                Directory.Delete(path, false);
            }

            /// <summary>
            /// 判断目标文件是否存在
            /// </summary>
            /// <param name="path">目标文件名称</param>
            /// <returns>若目标文件存在则返回true，否则返回false</returns>
            public static bool IsFileExists(string path)
            {
                if (path.IsNullOrEmpty())
                {
                    return false;
                }

                return File.Exists(path);
            }

            /// <summary>
            /// 安全模式创建目标文件
            /// </summary>
            /// <param name="path">目标文件名称</param>
            public static void CreateFile(string path)
            {
                if (false == IsFileExists(path))
                {
                    CreateDirectory(path);

                    File.Create(path);
                }
            }

            /// <summary>
            /// 安全模式删除目标文件
            /// </summary>
            /// <param name="path">目标文件名称</param>
            public static void DeleteFile(string path)
            {
                if (IsFileExists(path))
                {
                    File.Delete(path);
                }
            }

            /// <summary>
            /// 安全模式下复制目标文件
            /// </summary>
            /// <param name="src">拷贝源文件名称</param>
            /// <param name="dest">拷贝目标文件名称</param>
            public static void CopyFile(string src, string dest)
            {
                CreateDirectoryOnFilePath(dest);
                File.Copy(src, dest, true);
            }

            /// <summary>
            /// 以字节流方式读取指定目标文件
            /// </summary>
            /// <param name="file">目标文件名称</param>
            /// <returns>若打开文件成功则返回对应文件字节流内容，否则返回null</returns>
            public static byte[] ReadAllBytes(string file)
            {
                if (false == IsFileExists(file))
                {
                    return null;
                }

                File.SetAttributes(file, FileAttributes.Normal);
                return File.ReadAllBytes(file);
            }

            /// <summary>
            /// 以文本行方式读取指定目标文件
            /// </summary>
            /// <param name="file">目标文件名称</param>
            /// <returns>若打开文件成功则返回对应文件文本行内容，否则返回null</returns>
            public static string[] ReadAllLines(string file)
            {
                return ReadAllLines(file, Encoding.UTF8);
            }

            /// <summary>
            /// 用特定编码以文本行方式读取指定目标文件
            /// </summary>
            /// <param name="file">目标文件名称</param>
            /// <param name="encoding">文件编码</param>
            /// <returns>若打开文件成功则返回对应文件文本行内容，否则返回null</returns>
            public static string[] ReadAllLines(string file, Encoding encoding)
            {
                if (false == IsFileExists(file))
                {
                    return null;
                }

                File.SetAttributes(file, FileAttributes.Normal);
                return File.ReadAllLines(file, encoding);
            }

            /// <summary>
            /// 以文本串方式读取指定目标文件
            /// </summary>
            /// <param name="file">目标文件名称</param>
            /// <returns>若打开文件成功则返回对应文件文本串内容，否则返回null</returns>
            public static string ReadAllText(string file)
            {
                return File.ReadAllText(file, Encoding.UTF8);
            }

            /// <summary>
            /// 用特定编码以文本串方式读取指定目标文件
            /// </summary>
            /// <param name="file">目标文件名称</param>
            /// <param name="encoding">文件编码</param>
            /// <returns>若打开文件成功则返回对应文件文本串内容，否则返回null</returns>
            public static string ReadAllText(string file, Encoding encoding)
            {
                if (false == IsFileExists(file))
                {
                    return null;
                }

                File.SetAttributes(file, FileAttributes.Normal);
                return File.ReadAllText(file, encoding);
            }

            /// <summary>
            /// 以字节流方式向指定目标文件进行写入操作
            /// </summary>
            /// <param name="file">目标文件名称</param>
            /// <param name="bytes">字节流数据</param>
            /// <returns>若目标文件写入成功则返回true，否则返回false</returns>
            public static bool WriteAllBytes(string file, byte[] bytes)
            {
                if (file.IsNullOrEmpty())
                {
                    return false;
                }

                CreateDirectoryOnFilePath(file);
                if (File.Exists(file))
                {
                    File.SetAttributes(file, FileAttributes.Normal);
                }
                File.WriteAllBytes(file, bytes);
                return true;
            }

            /// <summary>
            /// 以文本行方式向指定目标文件进行写入操作
            /// </summary>
            /// <param name="file">目标文件名称</param>
            /// <param name="lines">文本行数据</param>
            /// <returns>若目标文件写入成功则返回true，否则返回false</returns>
            public static bool WriteAllLines(string file, string[] lines)
            {
                return WriteAllLines(file, lines, Encoding.UTF8);
            }

            /// <summary>
            /// 用特定编码以文本行方式向指定目标文件进行写入操作
            /// </summary>
            /// <param name="file">目标文件名称</param>
            /// <param name="lines">文本行数据</param>
            /// <param name="encoding">文件编码</param>
            /// <returns>若目标文件写入成功则返回true，否则返回false</returns>
            public static bool WriteAllLines(string file, string[] lines, Encoding encoding)
            {
                if (file.IsNullOrEmpty())
                {
                    return false;
                }

                CreateDirectoryOnFilePath(file);
                if (File.Exists(file))
                {
                    File.SetAttributes(file, FileAttributes.Normal);
                }
                File.WriteAllLines(file, lines, encoding);
                return true;
            }

            /// <summary>
            /// 以文本串方式向指定目标文件进行写入操作
            /// </summary>
            /// <param name="file">目标文件名称</param>
            /// <param name="text">文本串数据</param>
            /// <returns>若目标文件写入成功则返回true，否则返回false</returns>
            public static bool WriteAllText(string file, string text)
            {
                return WriteAllText(file, text, Encoding.UTF8);
            }

            /// <summary>
            /// 用特定编码以文本串方式向指定目标文件进行写入操作
            /// </summary>
            /// <param name="file">目标文件名称</param>
            /// <param name="text">文本串数据</param>
            /// <param name="encoding">文件编码</param>
            /// <returns>若目标文件写入成功则返回true，否则返回false</returns>
            public static bool WriteAllText(string file, string text, Encoding encoding)
            {
                if (file.IsNullOrEmpty())
                {
                    return false;
                }

                CreateDirectoryOnFilePath(file);
                if (File.Exists(file))
                {
                    File.SetAttributes(file, FileAttributes.Normal);
                }
                File.WriteAllText(file, text, encoding);
                return true;
            }

            /// <summary>
            /// 获取目标文件的占用空间大小
            /// </summary>
            /// <param name="path">目标文件名称</param>
            /// <returns>若目标文件存在则返回占用空间大小，否则返回0</returns>
            public static long GetFileSize(string path)
            {
                if (IsFileExists(path))
                {
                    return new FileInfo(path).Length;
                }

                return 0;
            }

            /// <summary>
            /// 移除目标文件名称中包含的后缀名，若文件不存在后缀则不做任何处理
            /// </summary>
            /// <param name="path">目标文件名称</param>
            /// <returns>返回目标路径经过移除文件后缀名之后的路径信息</returns>
            public static string RemoveFileSuffixName(string path)
            {
                string extension = System.IO.Path.GetExtension(path);
                if (extension.Length > 0)
                {
                    path = path.Remove(path.IndexOf(extension), extension.Length);
                }

                return path;
            }

            /// <summary>
            /// 清理指定目录内的全部内容，该操作将直接以递归方式删除掉该目录及其内部全部文件，然后创建一个新的同名目录
            /// </summary>
            /// <param name="path">目标文件目录的路径</param>
            public static void CleanupDirectory(string path)
            {
                if (Directory.Exists(path))
                {
                    Directory.Delete(path, true);
                }
                Directory.CreateDirectory(path);
            }

            /// <summary>
            /// 移动源目录下的所有文件（包含子目录）到目标路径下
            /// </summary>
            /// <param name="source">源目录路径</param>
            /// <param name="destination">目标目录路径</param>
            public static void MoveDirectory(string source, string destination)
            {
                if (false == Directory.Exists(destination))
                {
                    Directory.CreateDirectory(destination);
                }

                DirectoryInfo directoryInfo = new DirectoryInfo(source);
                FileInfo[] files = directoryInfo.GetFiles();
                for (int n = 0; n < files.Length; ++n)
                {
                    FileInfo file = files[n];
                    file.MoveTo(System.IO.Path.Combine(destination, file.Name));
                }

                DirectoryInfo[] dirs = directoryInfo.GetDirectories();
                for (int n = 0; n < dirs.Length; ++n)
                {
                    DirectoryInfo dir = dirs[n];
                    MoveDirectory(System.IO.Path.Combine(source, dir.Name), System.IO.Path.Combine(destination, dir.Name));
                }
            }

            /// <summary>
            /// 拷贝源目录下的所有文件（包含子目录）到目标路径下
            /// </summary>
            /// <param name="source">源目录路径</param>
            /// <param name="destination">目标目录路径</param>
            public static void CopyDirectory(string source, string destination)
            {
                if (false == Directory.Exists(destination))
                {
                    Directory.CreateDirectory(destination);
                }

                DirectoryInfo directoryInfo = new DirectoryInfo(source);
                FileInfo[] files = directoryInfo.GetFiles();
                for (int n = 0; n < files.Length; ++n)
                {
                    FileInfo file = files[n];
                    file.CopyTo(System.IO.Path.Combine(destination, file.Name));
                }

                DirectoryInfo[] dirs = directoryInfo.GetDirectories();
                for (int n = 0; n < dirs.Length; ++n)
                {
                    DirectoryInfo dir = dirs[n];
                    CopyDirectory(System.IO.Path.Combine(source, dir.Name), System.IO.Path.Combine(destination, dir.Name));
                }
            }

            /// <summary>
            /// 通过指定路径及文件名称获取对应的文件路径
            /// </summary>
            /// <param name="absolutePath">完整路径</param>
            /// <param name="filePath">文件路径</param>
            /// <returns>返回文件的完整路径</returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private static string __GetFullPath(string absolutePath, string filePath)
            {
                if (filePath.IsNullOrEmpty())
                {
                    return absolutePath;
                }

                return System.IO.Path.Combine(absolutePath, filePath);
            }

            /// <summary>
            /// 获取程序资源存储路径下指定文件名称的对应路径
            /// </summary>
            /// <param name="filePath">文件路径</param>
            /// <returns>返回文件的完整路径</returns>
            public static string GetFileOnStreamingAssetsPath(string filePath)
            {
                return __GetFullPath(Resource.StreamingAssetsPath, filePath);
            }

            /// <summary>
            /// 获取程序应用存储路径下指定文件名称的对应路径
            /// </summary>
            /// <param name="filePath">文件路径</param>
            /// <returns>返回文件的完整路径</returns>
            public static string GetFileOnApplicationDataPath(string filePath)
            {
                return __GetFullPath(Resource.ApplicationDataPath, filePath);
            }

            /// <summary>
            /// 获取程序执行数据存储路径下指定文件名称的对应路径
            /// </summary>
            /// <param name="filePath">文件路径</param>
            /// <returns>返回文件的完整路径</returns>
            public static string GetFileOnPersistentDataPath(string filePath)
            {
                return __GetFullPath(Resource.PersistentDataPath, filePath);
            }

            /// <summary>
            /// 获取本地数据缓存路径下指定文件名称的对应路径
            /// </summary>
            /// <param name="filePath">文件路径</param>
            /// <returns>返回文件的完整路径</returns>
            public static string GetFileOnPersistentDataCachePath(string filePath)
            {
                return __GetFullPath(Resource.PersistentDataCachePath, filePath);
            }

            /// <summary>
            /// 获取数据临时缓存路径下指定文件名称的对应路径
            /// </summary>
            /// <param name="filePath">文件路径</param>
            /// <returns>返回文件的完整路径</returns>
            public static string GetFileOnTemporaryCachePath(string filePath)
            {
                return __GetFullPath(Resource.TemporaryCachePath, filePath);
            }

            /// <summary>
            /// 加载基础文本资源，优先从外部目录加载，若外部目录不存在该资源则从资源默认路径加载
            /// </summary>
            /// <param name="path">文件资源路径</param>
            /// <returns>若加载该文本资源成功则返回对应的文本数据，否则返回null</returns>
            public static string LoadTextAsset(string path)
            {
                try
                {
                    string url = System.IO.Path.Combine(Resource.PersistentDataPath + "/", path);
                    if (IsFileExists(url))
                    {
                        return File.ReadAllText(url);
                    }
                    else
                    {
                        url = RemoveFileSuffixName(path);

                        // Resources读取配置文件
                        UnityTextAsset textAsset = UnityResources.Load<UnityTextAsset>(url);
                        if (null == textAsset)
                        {
                            return null;
                        }

                        return textAsset.text;
                    }
                }
                catch (System.Exception e)
                {
                    throw new CFrameworkException(e.ToString());
                }
            }

            /// <summary>
            /// 对基础文本资源进行持久化处理
            /// </summary>
            /// <param name="path">文本资源路径</param>
            /// <param name="text">文本数据</param>
            public static void SaveTextAsset(string path, string text)
            {
                try
                {
                    string url = System.IO.Path.Combine(Resource.PersistentDataPath + "/", path);
                    if (false == IsFileExists(url))
                    {
                        // 创建相关目录及文件
                    }
                    File.WriteAllText(url, text);
                }
                catch (System.Exception e)
                {
                    throw new CFrameworkException("Save text asset to file '{0}' failed, reason {1}", path, e);
                }
            }
        }
    }
}
