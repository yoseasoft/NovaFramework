/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
/// Copyring (C) 2020 - 2022, Guangzhou Xinyuan Technology Co., Ltd.
/// Copyring (C) 2022 - 2023, Shanghai Bilibili Technology Co., Ltd.
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

using System.Collections.Generic;

namespace NovaEngine.IO.FileSystem
{
    /// <summary>
    /// 文件系统控制器预定义接口类
    /// </summary>
    public interface IFileSystemController
    {
        /// <summary>
        /// 获取文件系统数量
        /// </summary>
        int Count
        {
            get;
        }

        /// <summary>
        /// 设置文件系统辅助器
        /// </summary>
        /// <param name="fileSystemHelper">文件系统辅助器</param>
        void SetFileSystemHelper(IFileSystemHelper fileSystemHelper);

        /// <summary>
        /// 检查是否存在文件系统
        /// </summary>
        /// <param name="fullPath">要检查的文件系统的完整路径</param>
        /// <returns>返回文件系统是否存在</returns>
        bool HasFileSystem(string fullPath);

        /// <summary>
        /// 获取文件系统
        /// </summary>
        /// <param name="fullPath">要获取的文件系统的完整路径</param>
        /// <returns>返回获取的文件系统</returns>
        IFileSystem GetFileSystem(string fullPath);

        /// <summary>
        /// 创建文件系统
        /// </summary>
        /// <param name="fullPath">要创建的文件系统的完整路径</param>
        /// <param name="accessType">要创建的文件系统的访问方式</param>
        /// <param name="maxFileCount">要创建的文件系统的最大文件数量</param>
        /// <param name="maxBlockCount">要创建的文件系统的最大块数据数量</param>
        /// <returns>返回创建的文件系统实例</returns>
        IFileSystem CreateFileSystem(string fullPath, FileSystemAccessType accessType, int maxFileCount, int maxBlockCount);

        /// <summary>
        /// 加载文件系统
        /// </summary>
        /// <param name="fullPath">要加载的文件系统的完整路径</param>
        /// <param name="accessType">要加载的文件系统的访问方式</param>
        /// <returns>返回加载的文件系统</returns>
        IFileSystem LoadFileSystem(string fullPath, FileSystemAccessType accessType);

        /// <summary>
        /// 销毁文件系统
        /// </summary>
        /// <param name="fileSystem">要销毁的文件系统</param>
        /// <param name="deletePhysicalFile">是否删除文件系统对应的物理文件</param>
        void DestroyFileSystem(IFileSystem fileSystem, bool deletePhysicalFile);

        /// <summary>
        /// 获取所有文件系统集合
        /// </summary>
        /// <returns>返回获取的所有文件系统集合</returns>
        IFileSystem[] GetAllFileSystems();

        /// <summary>
        /// 获取所有文件系统集合
        /// </summary>
        /// <param name="results">获取的所有文件系统集合</param>
        void GetAllFileSystems(List<IFileSystem> results);
    }
}
