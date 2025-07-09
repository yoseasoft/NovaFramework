/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
/// Copyright (C) 2020 - 2022, Guangzhou Xinyuan Technology Co., Ltd.
/// Copyright (C) 2022 - 2023, Shanghai Bilibili Technology Co., Ltd.
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
using SystemStream = System.IO.Stream;

namespace NovaEngine.IO.FileSystem
{
    /// <summary>
    /// 文件系统预定义接口类
    /// </summary>
    public interface IFileSystem
    {
        /// <summary>
        /// 获取文件系统完整路径
        /// </summary>
        string FullPath
        {
            get;
        }

        /// <summary>
        /// 获取文件系统访问方式
        /// </summary>
        FileSystemAccessType AccessType
        {
            get;
        }

        /// <summary>
        /// 获取文件数量
        /// </summary>
        int FileCount
        {
            get;
        }

        /// <summary>
        /// 获取文件最大数量
        /// </summary>
        int MaxFileCount
        {
            get;
        }

        /// <summary>
        /// 根据指定文件名称获取对应文件信息
        /// </summary>
        /// <param name="name">要获取文件信息的文件名称</param>
        /// <returns>返回文件信息实例</returns>
        FileInfo GetFileInfo(string name);

        /// <summary>
        /// 获取当前系统中所有文件信息
        /// </summary>
        /// <returns>返回全部文件信息实例</returns>
        FileInfo[] GetAllFileInfos();

        /// <summary>
        /// 获取当前系统中所有文件信息
        /// </summary>
        /// <param name="results">获取的所有文件信息</param>
        void GetAllFileInfos(List<FileInfo> results);

        /// <summary>
        /// 检查是否存在指定文件名称的文件实例
        /// </summary>
        /// <param name="name">要检查的文件名称</param>
        /// <returns>返回是否存在指定文件</returns>
        bool HasFile(string name);

        /// <summary>
        /// 读取指定文件
        /// </summary>
        /// <param name="name">要读取的文件名称</param>
        /// <returns>返回读取文件内容的二进制流</returns>
        byte[] ReadFile(string name);

        /// <summary>
        /// 读取指定文件
        /// </summary>
        /// <param name="name">要读取的文件名称</param>
        /// <param name="buffer">存储读取文件内容的二进制流</param>
        /// <returns>返回实际读取的文件字节数</returns>
        int ReadFile(string name, byte[] buffer);

        /// <summary>
        /// 读取指定文件
        /// </summary>
        /// <param name="name">要读取的文件名称</param>
        /// <param name="buffer">存储读取文件内容的二进制流</param>
        /// <param name="startIndex">存读取文件内容的二进制流的起始位置</param>
        /// <returns>返回实际读取的文件字节数</returns>
        int ReadFile(string name, byte[] buffer, int startIndex);

        /// <summary>
        /// 读取指定文件
        /// </summary>
        /// <param name="name">要读取的文件名称</param>
        /// <param name="buffer">存储读取文件内容的二进制流</param>
        /// <param name="startIndex">存储读取文件内容的二进制流的起始位置</param>
        /// <param name="length">存储读取文件内容的二进制流的长度</param>
        /// <returns>返回实际读取的文件字节数</returns>
        int ReadFile(string name, byte[] buffer, int startIndex, int length);

        /// <summary>
        /// 读取指定文件
        /// </summary>
        /// <param name="name">要读取的文件名称</param>
        /// <param name="stream">存储读取文件内容的二进制流</param>
        /// <returns>返回实际读取的文件字节数</returns>
        int ReadFile(string name, SystemStream stream);

        /// <summary>
        /// 读取指定文件的指定片段
        /// </summary>
        /// <param name="name">要读取片段的文件名称</param>
        /// <param name="length">要读取片段的长度</param>
        /// <returns>返回读取文件片段内容的二进制流</returns>
        byte[] ReadFileSegment(string name, int length);

        /// <summary>
        /// 读取指定文件的指定片段
        /// </summary>
        /// <param name="name">要读取片段的文件名称</param>
        /// <param name="offset">要读取片段的偏移</param>
        /// <param name="length">要读取片段的长度</param>
        /// <returns>返回读取文件片段内容的二进制流</returns>
        byte[] ReadFileSegment(string name, int offset, int length);

        /// <summary>
        /// 读取指定文件的指定片段
        /// </summary>
        /// <param name="name">要读取片段的文件名称</param>
        /// <param name="buffer">存储读取文件片段内容的二进制流</param>
        /// <returns>返回实际读取的文件片段字节数</returns>
        int ReadFileSegment(string name, byte[] buffer);

        /// <summary>
        /// 读取指定文件的指定片段
        /// </summary>
        /// <param name="name">要读取片段的文件名称</param>
        /// <param name="buffer">存储读取文件片段内容的二进制流</param>
        /// <param name="length">要读取片段的长度</param>
        /// <returns>返回实际读取的文件片段字节数</returns>
        int ReadFileSegment(string name, byte[] buffer, int length);

        /// <summary>
        /// 读取指定文件的指定片段
        /// </summary>
        /// <param name="name">要读取片段的文件名称</param>
        /// <param name="buffer">存储读取文件片段内容的二进制流</param>
        /// <param name="startIndex">存储读取文件片段内容的二进制流的起始位置</param>
        /// <param name="length">要读取片段的长度</param>
        /// <returns>返回实际读取的文件片段字节数</returns>
        int ReadFileSegment(string name, byte[] buffer, int startIndex, int length);

        /// <summary>
        /// 读取指定文件的指定片段
        /// </summary>
        /// <param name="name">要读取片段的文件名称</param>
        /// <param name="offset">要读取片段的偏移</param>
        /// <param name="buffer">存储读取文件片段内容的二进制流</param>
        /// <returns>返回实际读取的文件片段字节数</returns>
        int ReadFileSegment(string name, int offset, byte[] buffer);

        /// <summary>
        /// 读取指定文件的指定片段
        /// </summary>
        /// <param name="name">要读取片段的文件名称</param>
        /// <param name="offset">要读取片段的偏移</param>
        /// <param name="buffer">存储读取文件片段内容的二进制流</param>
        /// <param name="length">要读取片段的长度</param>
        /// <returns>返回实际读取的文件片段字节数</returns>
        int ReadFileSegment(string name, int offset, byte[] buffer, int length);

        /// <summary>
        /// 读取指定文件的指定片段
        /// </summary>
        /// <param name="name">要读取片段的文件名称</param>
        /// <param name="offset">要读取片段的偏移</param>
        /// <param name="buffer">存储读取文件片段内容的二进制流</param>
        /// <param name="startIndex">存储读取文件片段内容的二进制流的起始位置</param>
        /// <param name="length">要读取片段的长度</param>
        /// <returns>返回实际读取的文件片段字节数</returns>
        int ReadFileSegment(string name, int offset, byte[] buffer, int startIndex, int length);

        /// <summary>
        /// 读取指定文件的指定片段
        /// </summary>
        /// <param name="name">要读取片段的文件名称</param>
        /// <param name="stream">存储读取文件片段内容的二进制流</param>
        /// <param name="length">要读取片段的长度</param>
        /// <returns>返回实际读取的文件片段字节数</returns>
        int ReadFileSegment(string name, SystemStream stream, int length);

        /// <summary>
        /// 读取指定文件的指定片段
        /// </summary>
        /// <param name="name">要读取片段的文件名称</param>
        /// <param name="offset">要读取片段的偏移</param>
        /// <param name="stream">存储读取文件片段内容的二进制流</param>
        /// <param name="length">要读取片段的长度</param>
        /// <returns>返回实际读取的文件片段字节数</returns>
        int ReadFileSegment(string name, int offset, SystemStream stream, int length);

        /// <summary>
        /// 写入指定文件
        /// </summary>
        /// <param name="name">要写入的文件名称</param>
        /// <param name="buffer">存储写入文件内容的二进制流</param>
        /// <returns>返回写入指定文件是否成功</returns>
        bool WriteFile(string name, byte[] buffer);

        /// <summary>
        /// 写入指定文件
        /// </summary>
        /// <param name="name">要写入的文件名称</param>
        /// <param name="buffer">存储写入文件内容的二进制流</param>
        /// <param name="startIndex">存储写入文件内容的二进制流的起始位置</param>
        /// <returns>返回写入指定文件是否成功</returns>
        bool WriteFile(string name, byte[] buffer, int startIndex);

        /// <summary>
        /// 写入指定文件
        /// </summary>
        /// <param name="name">要写入的文件名称</param>
        /// <param name="buffer">存储写入文件内容的二进制流</param>
        /// <param name="startIndex">存储写入文件内容的二进制流的起始位置</param>
        /// <param name="length">存储写入文件内容的二进制流的长度</param>
        /// <returns>返回写入指定文件是否成功</returns>
        bool WriteFile(string name, byte[] buffer, int startIndex, int length);

        /// <summary>
        /// 写入指定文件
        /// </summary>
        /// <param name="name">要写入的文件名称</param>
        /// <param name="stream">存储写入文件内容的二进制流</param>
        /// <returns>返回写入指定文件是否成功</returns>
        bool WriteFile(string name, SystemStream stream);

        /// <summary>
        /// 写入指定文件
        /// </summary>
        /// <param name="name">要写入的文件名称</param>
        /// <param name="filePath">存储写入文件内容的文件路径</param>
        /// <returns>返回写入指定文件是否成功</returns>
        bool WriteFile(string name, string filePath);

        /// <summary>
        /// 将指定文件另存为物理文件
        /// </summary>
        /// <param name="name">要另存为的文件名称</param>
        /// <param name="filePath">存储写入文件内容的文件路径</param>
        /// <returns>返回将指定文件另存为物理文件是否成功</returns>
        bool SaveAsFile(string name, string filePath);

        /// <summary>
        /// 重命名指定文件
        /// </summary>
        /// <param name="oldName">要重命名的文件名称</param>
        /// <param name="newName">重命名后的文件名称</param>
        /// <returns>返回重命名指定文件是否成功</returns>
        bool RenameFile(string oldName, string newName);

        /// <summary>
        /// 删除指定文件
        /// </summary>
        /// <param name="name">要删除的文件名称</param>
        /// <returns>返回删除指定文件是否成功</returns>
        bool DeleteFile(string name);
    }
}
