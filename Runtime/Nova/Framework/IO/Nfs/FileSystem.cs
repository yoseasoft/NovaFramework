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
using System.Runtime.InteropServices;

using SystemStringComparer = System.StringComparer;
using SystemStream = System.IO.Stream;
using SystemFile = System.IO.File;
using SystemPath = System.IO.Path;
using SystemDirectory = System.IO.Directory;
using SystemFileStream = System.IO.FileStream;
using SystemFileMode = System.IO.FileMode;
using SystemFileAccess = System.IO.FileAccess;
using SystemFileShare = System.IO.FileShare;

namespace NovaEngine.IO.FileSystem
{
    /// <summary>
    /// 文件系统基类
    /// </summary>
    internal sealed partial class FileSystem : IFileSystem
    {
        private const int CLUSTER_SIZE = 1024 * 4;
        private const int CACHED_BYTES_LENGTH = 0x1000;

        private static readonly string[] EmptyStringArray = new string[] { };
        private static readonly byte[] s_cachedBytes = new byte[CACHED_BYTES_LENGTH];

        private static readonly int HeaderDataSize = Marshal.SizeOf(typeof(HeaderData));
        private static readonly int BlockDataSize = Marshal.SizeOf(typeof(BlockData));
        private static readonly int StringDataSize = Marshal.SizeOf(typeof(StringData));

        private readonly string m_fullPath;
        private readonly FileSystemAccessType m_accessType;
        private readonly FileSystemStream m_stream;
        private readonly Dictionary<string, int> m_fileDatas;
        private readonly List<BlockData> m_blockDatas;
        private readonly MultiDictionary<int, int> m_freeBlockIndexes;
        private readonly SortedDictionary<int, StringData> m_stringDatas;
        private readonly Queue<KeyValuePair<int, StringData>> m_freeStringDatas;

        private HeaderData m_headerData;
        private int m_blockDataOffset;
        private int m_stringDataOffset;
        private int m_fileDataOffset;

        /// <summary>
        /// 初始化文件系统的新实例
        /// </summary>
        /// <param name="fullPath">文件系统完整路径</param>
        /// <param name="access">文件系统访问方式</param>
        /// <param name="stream">文件系统数据流</param>
        private FileSystem(string fullPath, FileSystemAccessType accessType, FileSystemStream stream)
        {
            if (string.IsNullOrEmpty(fullPath))
            {
                throw new CFrameworkException("Full path is invalid.");
            }

            if (FileSystemAccessType.Unspecified == accessType)
            {
                throw new CFrameworkException("Access type is invalid.");
            }

            if (null == stream)
            {
                throw new CFrameworkException("Stream is invalid.");
            }

            this.m_fullPath = fullPath;
            this.m_accessType = accessType;
            this.m_stream = stream;
            this.m_fileDatas = new Dictionary<string, int>(SystemStringComparer.Ordinal);
            this.m_blockDatas = new List<BlockData>();
            this.m_freeBlockIndexes = new MultiDictionary<int, int>();
            this.m_stringDatas = new SortedDictionary<int, StringData>();
            this.m_freeStringDatas = new Queue<KeyValuePair<int, StringData>>();

            this.m_headerData = default(HeaderData);
            this.m_blockDataOffset = 0;
            this.m_stringDataOffset = 0;
            this.m_fileDataOffset = 0;

            Utility.Marshal.EnsureCachedHGlobalSize(CACHED_BYTES_LENGTH);
        }

        /// <summary>
        /// 获取文件系统完整路径
        /// </summary>
        public string FullPath
        {
            get { return m_fullPath; }
        }

        /// <summary>
        /// 获取文件系统访问方式
        /// </summary>
        public FileSystemAccessType AccessType
        {
            get { return m_accessType; }
        }

        /// <summary>
        /// 获取文件数量
        /// </summary>
        public int FileCount
        {
            get { return m_fileDatas.Count; }
        }

        /// <summary>
        /// 获取最大文件数量
        /// </summary>
        public int MaxFileCount
        {
            get { return m_headerData.MaxFileCount; }
        }

        /// <summary>
        /// 创建文件系统
        /// </summary>
        /// <param name="fullPath">要创建的文件系统的完整路径</param>
        /// <param name="accessType">要创建的文件系统的访问方式</param>
        /// <param name="stream">要创建的文件系统的文件系统数据流</param>
        /// <param name="maxFileCount">要创建的文件系统的最大文件数量</param>
        /// <param name="maxBlockCount">要创建的文件系统的最大块数据数量</param>
        /// <returns>返回创建的文件系统对象实例</returns>
        public static FileSystem Create(string fullPath, FileSystemAccessType accessType, FileSystemStream stream, int maxFileCount, int maxBlockCount)
        {
            if (maxFileCount <= 0)
            {
                throw new CFrameworkException("Max file count is invalid.");
            }

            if (maxBlockCount <= 0)
            {
                throw new CFrameworkException("Max block count is invalid.");
            }

            if (maxFileCount > maxBlockCount)
            {
                throw new CFrameworkException("Max file count can not larger than max block count.");
            }

            FileSystem fileSystem = new FileSystem(fullPath, accessType, stream);
            fileSystem.m_headerData = new HeaderData(maxFileCount, maxBlockCount);
            CalcOffsets(fileSystem);
            Utility.Marshal.StructureToBytes(fileSystem.m_headerData, HeaderDataSize, s_cachedBytes);

            try
            {
                stream.Write(s_cachedBytes, 0, HeaderDataSize);
                stream.SetLength(fileSystem.m_fileDataOffset);
                return fileSystem;
            }
            catch
            {
                fileSystem.Shutdown();
                return null;
            }
        }

        /// <summary>
        /// 加载文件系统
        /// </summary>
        /// <param name="fullPath">要加载的文件系统的完整路径</param>
        /// <param name="accessType">要加载的文件系统的访问方式</param>
        /// <param name="stream">要加载的文件系统的文件系统数据流</param>
        /// <returns>返回加载的文件系统</returns>
        public static FileSystem Load(string fullPath, FileSystemAccessType accessType, FileSystemStream stream)
        {
            FileSystem fileSystem = new FileSystem(fullPath, accessType, stream);

            stream.Read(s_cachedBytes, 0, HeaderDataSize);
            fileSystem.m_headerData = Utility.Marshal.BytesToStructure<HeaderData>(HeaderDataSize, s_cachedBytes);
            CalcOffsets(fileSystem);

            if (fileSystem.m_blockDatas.Capacity < fileSystem.m_headerData.BlockCount)
            {
                fileSystem.m_blockDatas.Capacity = fileSystem.m_headerData.BlockCount;
            }

            for (int n = 0; n < fileSystem.m_headerData.BlockCount; ++n)
            {
                stream.Read(s_cachedBytes, 0, BlockDataSize);
                BlockData blockData = Utility.Marshal.BytesToStructure<BlockData>(BlockDataSize, s_cachedBytes);
                fileSystem.m_blockDatas.Add(blockData);
            }

            for (int n = 0; n < fileSystem.m_blockDatas.Count; ++n)
            {
                BlockData blockData = fileSystem.m_blockDatas[n];
                if (blockData.Using)
                {
                    StringData stringData = fileSystem.ReadStringData(blockData.StringIndex);
                    fileSystem.m_stringDatas.Add(blockData.StringIndex, stringData);
                    fileSystem.m_fileDatas.Add(stringData.GetString(fileSystem.m_headerData.GetEncryptBytes()), n);
                }
                else
                {
                    fileSystem.m_freeBlockIndexes.Add(blockData.Length, n);
                }
            }

            return fileSystem;
        }

        /// <summary>
        /// 关闭并清理文件系统
        /// </summary>
        public void Shutdown()
        {
            this.m_stream.Close();

            this.m_fileDatas.Clear();
            this.m_blockDatas.Clear();
            this.m_freeBlockIndexes.Clear();
            this.m_stringDatas.Clear();
            this.m_freeStringDatas.Clear();

            this.m_blockDataOffset = 0;
            this.m_stringDataOffset = 0;
            this.m_fileDataOffset = 0;
        }

        /// <summary>
        /// 根据指定文件名称获取对应文件信息
        /// </summary>
        /// <param name="name">要获取文件信息的文件名称</param>
        /// <returns>返回文件信息实例</returns>
        public FileInfo GetFileInfo(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new CFrameworkException("Name is invalid.");
            }

            if (false == m_fileDatas.TryGetValue(name, out int blockIndex))
            {
                return default(FileInfo);
            }

            BlockData blockData = m_blockDatas[blockIndex];
            return new FileInfo(name, GetClusterOffset(blockData.ClusterIndex), blockData.Length);
        }

        /// <summary>
        /// 获取当前系统中所有文件信息
        /// </summary>
        /// <returns>返回全部文件信息实例</returns>
        public FileInfo[] GetAllFileInfos()
        {
            int index = 0;
            FileInfo[] results = new FileInfo[m_fileDatas.Count];
            foreach (KeyValuePair<string, int> fileData in m_fileDatas)
            {
                BlockData blockData = m_blockDatas[fileData.Value];
                results[index++] = new FileInfo(fileData.Key, GetClusterOffset(blockData.ClusterIndex), blockData.Length);
            }

            return results;
        }

        /// <summary>
        /// 获取当前系统中所有文件信息
        /// </summary>
        /// <param name="results">获取的所有文件信息</param>
        public void GetAllFileInfos(List<FileInfo> results)
        {
            if (null == results)
            {
                throw new CFrameworkException("Results is invalid.");
            }

            results.Clear();
            foreach (KeyValuePair<string, int> fileData in m_fileDatas)
            {
                BlockData blockData = m_blockDatas[fileData.Value];
                results.Add(new FileInfo(fileData.Key, GetClusterOffset(blockData.ClusterIndex), blockData.Length));
            }
        }

        /// <summary>
        /// 检查是否存在指定文件名称的文件实例
        /// </summary>
        /// <param name="name">要检查的文件名称</param>
        /// <returns>返回是否存在指定文件</returns>
        public bool HasFile(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new CFrameworkException("Name is invalid.");
            }

            return m_fileDatas.ContainsKey(name);
        }

        /// <summary>
        /// 读取指定文件
        /// </summary>
        /// <param name="name">要读取的文件名称</param>
        /// <returns>返回读取文件内容的二进制流</returns>
        public byte[] ReadFile(string name)
        {
            if (m_accessType != FileSystemAccessType.ReadOnly && m_accessType != FileSystemAccessType.ReadWrite)
            {
                throw new CFrameworkException("File system is not readable.");
            }

            if (string.IsNullOrEmpty(name))
            {
                throw new CFrameworkException("Name is invalid.");
            }

            FileInfo fileInfo = GetFileInfo(name);
            if (false == fileInfo.IsValid)
            {
                return null;
            }

            int length = fileInfo.Length;
            byte[] buffer = new byte[length];
            if (length > 0)
            {
                m_stream.Position = fileInfo.Offset;
                m_stream.Read(buffer, 0, length);
            }

            return buffer;
        }

        /// <summary>
        /// 读取指定文件
        /// </summary>
        /// <param name="name">要读取的文件名称</param>
        /// <param name="buffer">存储读取文件内容的二进制流</param>
        /// <returns>返回实际读取的文件字节数</returns>
        public int ReadFile(string name, byte[] buffer)
        {
            if (null == buffer)
            {
                throw new CFrameworkException("Buffer is invalid.");
            }

            return ReadFile(name, buffer, 0, buffer.Length);
        }

        /// <summary>
        /// 读取指定文件
        /// </summary>
        /// <param name="name">要读取的文件名称</param>
        /// <param name="buffer">存储读取文件内容的二进制流</param>
        /// <param name="startIndex">存读取文件内容的二进制流的起始位置</param>
        /// <returns>返回实际读取的文件字节数</returns>
        public int ReadFile(string name, byte[] buffer, int startIndex)
        {
            if (null == buffer)
            {
                throw new CFrameworkException("Buffer is invalid.");
            }

            return ReadFile(name, buffer, startIndex, buffer.Length - startIndex);
        }

        /// <summary>
        /// 读取指定文件
        /// </summary>
        /// <param name="name">要读取的文件名称</param>
        /// <param name="buffer">存储读取文件内容的二进制流</param>
        /// <param name="startIndex">存储读取文件内容的二进制流的起始位置</param>
        /// <param name="length">存储读取文件内容的二进制流的长度</param>
        /// <returns>返回实际读取的文件字节数</returns>
        public int ReadFile(string name, byte[] buffer, int startIndex, int length)
        {
            if (m_accessType != FileSystemAccessType.ReadOnly && m_accessType != FileSystemAccessType.ReadWrite)
            {
                throw new CFrameworkException("File system is not readable.");
            }

            if (string.IsNullOrEmpty(name))
            {
                throw new CFrameworkException("Name is invalid.");
            }

            if (null == buffer)
            {
                throw new CFrameworkException("Buffer is invalid.");
            }

            if (startIndex < 0 || length < 0 || startIndex + length > buffer.Length)
            {
                throw new CFrameworkException("Start index or length is invalid.");
            }

            FileInfo fileInfo = GetFileInfo(name);
            if (false == fileInfo.IsValid)
            {
                return 0;
            }

            m_stream.Position = fileInfo.Offset;
            if (length > fileInfo.Length)
            {
                length = fileInfo.Length;
            }

            if (length > 0)
            {
                return m_stream.Read(buffer, startIndex, length);
            }

            return 0;
        }

        /// <summary>
        /// 读取指定文件
        /// </summary>
        /// <param name="name">要读取的文件名称</param>
        /// <param name="stream">存储读取文件内容的二进制流</param>
        /// <returns>返回实际读取的文件字节数</returns>
        public int ReadFile(string name, SystemStream stream)
        {
            if (m_accessType != FileSystemAccessType.ReadOnly && m_accessType != FileSystemAccessType.ReadWrite)
            {
                throw new CFrameworkException("File system is not readable.");
            }

            if (string.IsNullOrEmpty(name))
            {
                throw new CFrameworkException("Name is invalid.");
            }

            if (null == stream)
            {
                throw new CFrameworkException("Stream is invalid.");
            }

            if (false == stream.CanWrite)
            {
                throw new CFrameworkException("Stream is not writable.");
            }

            FileInfo fileInfo = GetFileInfo(name);
            if (false == fileInfo.IsValid)
            {
                return 0;
            }

            int length = fileInfo.Length;
            if (length > 0)
            {
                m_stream.Position = fileInfo.Offset;
                return m_stream.Read(stream, length);
            }

            return 0;
        }

        /// <summary>
        /// 读取指定文件的指定片段
        /// </summary>
        /// <param name="name">要读取片段的文件名称</param>
        /// <param name="length">要读取片段的长度</param>
        /// <returns>返回读取文件片段内容的二进制流</returns>
        public byte[] ReadFileSegment(string name, int length)
        {
            return ReadFileSegment(name, 0, length);
        }

        /// <summary>
        /// 读取指定文件的指定片段
        /// </summary>
        /// <param name="name">要读取片段的文件名称</param>
        /// <param name="offset">要读取片段的偏移</param>
        /// <param name="length">要读取片段的长度</param>
        /// <returns>返回读取文件片段内容的二进制流</returns>
        public byte[] ReadFileSegment(string name, int offset, int length)
        {
            if (m_accessType != FileSystemAccessType.ReadOnly && m_accessType != FileSystemAccessType.ReadWrite)
            {
                throw new CFrameworkException("File system is not readable.");
            }

            if (string.IsNullOrEmpty(name))
            {
                throw new CFrameworkException("Name is invalid.");
            }

            if (offset < 0)
            {
                throw new CFrameworkException("Index is invalid.");
            }

            if (length < 0)
            {
                throw new CFrameworkException("Length is invalid.");
            }

            FileInfo fileInfo = GetFileInfo(name);
            if (false == fileInfo.IsValid)
            {
                return null;
            }

            if (offset > fileInfo.Length)
            {
                offset = fileInfo.Length;
            }

            int leftLength = fileInfo.Length - offset;
            if (length > leftLength)
            {
                length = leftLength;
            }

            byte[] buffer = new byte[length];
            if (length > 0)
            {
                m_stream.Position = fileInfo.Offset + offset;
                m_stream.Read(buffer, 0, length);
            }

            return buffer;
        }

        /// <summary>
        /// 读取指定文件的指定片段
        /// </summary>
        /// <param name="name">要读取片段的文件名称</param>
        /// <param name="buffer">存储读取文件片段内容的二进制流</param>
        /// <returns>返回实际读取的文件片段字节数</returns>
        public int ReadFileSegment(string name, byte[] buffer)
        {
            if (null == buffer)
            {
                throw new CFrameworkException("Buffer is invalid.");
            }

            return ReadFileSegment(name, 0, buffer, 0, buffer.Length);
        }

        /// <summary>
        /// 读取指定文件的指定片段
        /// </summary>
        /// <param name="name">要读取片段的文件名称</param>
        /// <param name="buffer">存储读取文件片段内容的二进制流</param>
        /// <param name="length">要读取片段的长度</param>
        /// <returns>返回实际读取的文件片段字节数</returns>
        public int ReadFileSegment(string name, byte[] buffer, int length)
        {
            return ReadFileSegment(name, 0, buffer, 0, length);
        }

        /// <summary>
        /// 读取指定文件的指定片段
        /// </summary>
        /// <param name="name">要读取片段的文件名称</param>
        /// <param name="buffer">存储读取文件片段内容的二进制流</param>
        /// <param name="startIndex">存储读取文件片段内容的二进制流的起始位置</param>
        /// <param name="length">要读取片段的长度</param>
        /// <returns>返回实际读取的文件片段字节数</returns>
        public int ReadFileSegment(string name, byte[] buffer, int startIndex, int length)
        {
            return ReadFileSegment(name, 0, buffer, startIndex, length);
        }

        /// <summary>
        /// 读取指定文件的指定片段
        /// </summary>
        /// <param name="name">要读取片段的文件名称</param>
        /// <param name="offset">要读取片段的偏移</param>
        /// <param name="buffer">存储读取文件片段内容的二进制流</param>
        /// <returns>返回实际读取的文件片段字节数</returns>
        public int ReadFileSegment(string name, int offset, byte[] buffer)
        {
            if (null == buffer)
            {
                throw new CFrameworkException("Buffer is invalid.");
            }

            return ReadFileSegment(name, offset, buffer, 0, buffer.Length);
        }

        /// <summary>
        /// 读取指定文件的指定片段
        /// </summary>
        /// <param name="name">要读取片段的文件名称</param>
        /// <param name="offset">要读取片段的偏移</param>
        /// <param name="buffer">存储读取文件片段内容的二进制流</param>
        /// <param name="length">要读取片段的长度</param>
        /// <returns>返回实际读取的文件片段字节数</returns>
        public int ReadFileSegment(string name, int offset, byte[] buffer, int length)
        {
            return ReadFileSegment(name, offset, buffer, 0, length);
        }

        /// <summary>
        /// 读取指定文件的指定片段
        /// </summary>
        /// <param name="name">要读取片段的文件名称</param>
        /// <param name="offset">要读取片段的偏移</param>
        /// <param name="buffer">存储读取文件片段内容的二进制流</param>
        /// <param name="startIndex">存储读取文件片段内容的二进制流的起始位置</param>
        /// <param name="length">要读取片段的长度</param>
        /// <returns>返回实际读取的文件片段字节数</returns>
        public int ReadFileSegment(string name, int offset, byte[] buffer, int startIndex, int length)
        {
            if (m_accessType != FileSystemAccessType.ReadOnly && m_accessType != FileSystemAccessType.ReadWrite)
            {
                throw new CFrameworkException("File system is not readable.");
            }

            if (string.IsNullOrEmpty(name))
            {
                throw new CFrameworkException("Name is invalid.");
            }

            if (offset < 0)
            {
                throw new CFrameworkException("Index is invalid.");
            }

            if (null == buffer)
            {
                throw new CFrameworkException("Buffer is invalid.");
            }

            if (startIndex < 0 || length < 0 || startIndex + length > buffer.Length)
            {
                throw new CFrameworkException("Start index or length is invalid.");
            }

            FileInfo fileInfo = GetFileInfo(name);
            if (false == fileInfo.IsValid)
            {
                return 0;
            }

            if (offset > fileInfo.Length)
            {
                offset = fileInfo.Length;
            }

            int leftLength = fileInfo.Length - offset;
            if (length > leftLength)
            {
                length = leftLength;
            }

            if (length > 0)
            {
                m_stream.Position = fileInfo.Offset + offset;
                return m_stream.Read(buffer, startIndex, length);
            }

            return 0;
        }

        /// <summary>
        /// 读取指定文件的指定片段
        /// </summary>
        /// <param name="name">要读取片段的文件名称</param>
        /// <param name="stream">存储读取文件片段内容的二进制流</param>
        /// <param name="length">要读取片段的长度</param>
        /// <returns>返回实际读取的文件片段字节数</returns>
        public int ReadFileSegment(string name, SystemStream stream, int length)
        {
            return ReadFileSegment(name, 0, stream, length);
        }

        /// <summary>
        /// 读取指定文件的指定片段
        /// </summary>
        /// <param name="name">要读取片段的文件名称</param>
        /// <param name="offset">要读取片段的偏移</param>
        /// <param name="stream">存储读取文件片段内容的二进制流</param>
        /// <param name="length">要读取片段的长度</param>
        /// <returns>返回实际读取的文件片段字节数</returns>
        public int ReadFileSegment(string name, int offset, SystemStream stream, int length)
        {
            if (m_accessType != FileSystemAccessType.ReadOnly && m_accessType != FileSystemAccessType.ReadWrite)
            {
                throw new CFrameworkException("File system is not readable.");
            }

            if (string.IsNullOrEmpty(name))
            {
                throw new CFrameworkException("Name is invalid.");
            }

            if (offset < 0)
            {
                throw new CFrameworkException("Index is invalid.");
            }

            if (null == stream)
            {
                throw new CFrameworkException("Stream is invalid.");
            }

            if (false == stream.CanWrite)
            {
                throw new CFrameworkException("Stream is not writable.");
            }

            if (length < 0)
            {
                throw new CFrameworkException("Length is invalid.");
            }

            FileInfo fileInfo = GetFileInfo(name);
            if (false == fileInfo.IsValid)
            {
                return 0;
            }

            if (offset > fileInfo.Length)
            {
                offset = fileInfo.Length;
            }

            int leftLength = fileInfo.Length - offset;
            if (length > leftLength)
            {
                length = leftLength;
            }

            if (length > 0)
            {
                m_stream.Position = fileInfo.Offset + offset;
                return m_stream.Read(stream, length);
            }

            return 0;
        }

        /// <summary>
        /// 写入指定文件
        /// </summary>
        /// <param name="name">要写入的文件名称</param>
        /// <param name="buffer">存储写入文件内容的二进制流</param>
        /// <returns>返回写入指定文件是否成功</returns>
        public bool WriteFile(string name, byte[] buffer)
        {
            if (null == buffer)
            {
                throw new CFrameworkException("Buffer is invalid.");
            }

            return WriteFile(name, buffer, 0, buffer.Length);
        }

        /// <summary>
        /// 写入指定文件
        /// </summary>
        /// <param name="name">要写入的文件名称</param>
        /// <param name="buffer">存储写入文件内容的二进制流</param>
        /// <param name="startIndex">存储写入文件内容的二进制流的起始位置</param>
        /// <returns>返回写入指定文件是否成功</returns>
        public bool WriteFile(string name, byte[] buffer, int startIndex)
        {
            if (null == buffer)
            {
                throw new CFrameworkException("Buffer is invalid.");
            }

            return WriteFile(name, buffer, startIndex, buffer.Length - startIndex);
        }

        /// <summary>
        /// 写入指定文件
        /// </summary>
        /// <param name="name">要写入的文件名称</param>
        /// <param name="buffer">存储写入文件内容的二进制流</param>
        /// <param name="startIndex">存储写入文件内容的二进制流的起始位置</param>
        /// <param name="length">存储写入文件内容的二进制流的长度</param>
        /// <returns>返回写入指定文件是否成功</returns>
        public bool WriteFile(string name, byte[] buffer, int startIndex, int length)
        {
            if (m_accessType != FileSystemAccessType.WriteOnly && m_accessType != FileSystemAccessType.ReadWrite)
            {
                throw new CFrameworkException("File system is not writable.");
            }

            if (string.IsNullOrEmpty(name))
            {
                throw new CFrameworkException("Name is invalid.");
            }

            if (name.Length > byte.MaxValue)
            {
                throw new CFrameworkException("Name '{0}' is too long.", name);
            }

            if (null == buffer)
            {
                throw new CFrameworkException("Buffer is invalid.");
            }

            if (startIndex < 0 || length < 0 || startIndex + length > buffer.Length)
            {
                throw new CFrameworkException("Start index or length is invalid.");
            }

            bool hasFile = false;
            int oldBlockIndex = -1;
            if (m_fileDatas.TryGetValue(name, out oldBlockIndex))
            {
                hasFile = true;
            }

            if (false == hasFile && m_fileDatas.Count >= m_headerData.MaxFileCount)
            {
                return false;
            }

            int blockIndex = AllocBlock(length);
            if (blockIndex < 0)
            {
                return false;
            }

            if (length > 0)
            {
                m_stream.Position = GetClusterOffset(m_blockDatas[blockIndex].ClusterIndex);
                m_stream.Write(buffer, startIndex, length);
            }

            ProcessWriteFile(name, hasFile, oldBlockIndex, blockIndex, length);
            m_stream.Flush();
            return true;
        }

        /// <summary>
        /// 写入指定文件
        /// </summary>
        /// <param name="name">要写入的文件名称</param>
        /// <param name="stream">存储写入文件内容的二进制流</param>
        /// <returns>返回写入指定文件是否成功</returns>
        public bool WriteFile(string name, SystemStream stream)
        {
            if (m_accessType != FileSystemAccessType.WriteOnly && m_accessType != FileSystemAccessType.ReadWrite)
            {
                throw new CFrameworkException("File system is not writable.");
            }

            if (string.IsNullOrEmpty(name))
            {
                throw new CFrameworkException("Name is invalid.");
            }

            if (name.Length > byte.MaxValue)
            {
                throw new CFrameworkException("Name '{0}' is too long.", name);
            }

            if (null == stream)
            {
                throw new CFrameworkException("Stream is invalid.");
            }

            if (false == stream.CanRead)
            {
                throw new CFrameworkException("Stream is not readable.");
            }

            bool hasFile = false;
            int oldBlockIndex = -1;
            if (m_fileDatas.TryGetValue(name, out oldBlockIndex))
            {
                hasFile = true;
            }

            if (false == hasFile && m_fileDatas.Count >= m_headerData.MaxFileCount)
            {
                return false;
            }

            int length = (int) (stream.Length - stream.Position);
            int blockIndex = AllocBlock(length);
            if (blockIndex < 0)
            {
                return false;
            }

            if (length > 0)
            {
                m_stream.Position = GetClusterOffset(m_blockDatas[blockIndex].ClusterIndex);
                m_stream.Write(stream, length);
            }

            ProcessWriteFile(name, hasFile, oldBlockIndex, blockIndex, length);
            m_stream.Flush();
            return true;
        }

        /// <summary>
        /// 写入指定文件
        /// </summary>
        /// <param name="name">要写入的文件名称</param>
        /// <param name="filePath">存储写入文件内容的文件路径</param>
        /// <returns>返回写入指定文件是否成功</returns>
        public bool WriteFile(string name, string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                throw new CFrameworkException("File path is invalid");
            }

            if (false == SystemFile.Exists(filePath))
            {
                return false;
            }

            using (SystemFileStream fileStream = new SystemFileStream(filePath, SystemFileMode.Open, SystemFileAccess.Read, SystemFileShare.Read))
            {
                return WriteFile(name, fileStream);
            }
        }

        /// <summary>
        /// 将指定文件另存为物理文件
        /// </summary>
        /// <param name="name">要另存为的文件名称</param>
        /// <param name="filePath">存储写入文件内容的文件路径</param>
        /// <returns>返回将指定文件另存为物理文件是否成功</returns>
        public bool SaveAsFile(string name, string filePath)
        {
            if (m_accessType != FileSystemAccessType.ReadOnly && m_accessType != FileSystemAccessType.ReadWrite)
            {
                throw new CFrameworkException("File system is not readable.");
            }

            if (string.IsNullOrEmpty(name))
            {
                throw new CFrameworkException("Name is invalid.");
            }

            if (string.IsNullOrEmpty(filePath))
            {
                throw new CFrameworkException("File path is invalid");
            }

            FileInfo fileInfo = GetFileInfo(name);
            if (false == fileInfo.IsValid)
            {
                return false;
            }

            try
            {
                if (SystemFile.Exists(filePath))
                {
                    SystemFile.Delete(filePath);
                }

                string directory = SystemPath.GetDirectoryName(filePath);
                if (false == SystemDirectory.Exists(directory))
                {
                    SystemDirectory.CreateDirectory(directory);
                }

                using (SystemFileStream fileStream = new SystemFileStream(filePath, SystemFileMode.Create, SystemFileAccess.Write, SystemFileShare.None))
                {
                    int length = fileInfo.Length;
                    if (length > 0)
                    {
                        m_stream.Position = fileInfo.Offset;
                        return m_stream.Read(fileStream, length) == length;
                    }

                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 重命名指定文件
        /// </summary>
        /// <param name="oldName">要重命名的文件名称</param>
        /// <param name="newName">重命名后的文件名称</param>
        /// <returns>返回重命名指定文件是否成功</returns>
        public bool RenameFile(string oldName, string newName)
        {
            if (m_accessType != FileSystemAccessType.WriteOnly && m_accessType != FileSystemAccessType.ReadWrite)
            {
                throw new CFrameworkException("File system is not writable.");
            }

            if (string.IsNullOrEmpty(oldName))
            {
                throw new CFrameworkException("Old name is invalid.");
            }

            if (string.IsNullOrEmpty(newName))
            {
                throw new CFrameworkException("New name is invalid.");
            }

            if (newName.Length > byte.MaxValue)
            {
                throw new CFrameworkException("New name '{0}' is too long.", newName);
            }

            if (oldName == newName)
            {
                return true;
            }

            if (m_fileDatas.ContainsKey(newName))
            {
                return false;
            }

            if (false == m_fileDatas.TryGetValue(oldName, out int blockIndex))
            {
                return false;
            }

            int stringIndex = m_blockDatas[blockIndex].StringIndex;
            StringData stringData = m_stringDatas[stringIndex].SetString(newName, m_headerData.GetEncryptBytes());
            m_stringDatas[stringIndex] = stringData;
            WriteStringData(stringIndex, stringData);
            m_fileDatas.Add(newName, blockIndex);
            m_fileDatas.Remove(oldName);
            m_stream.Flush();
            return true;
        }

        /// <summary>
        /// 删除指定文件
        /// </summary>
        /// <param name="name">要删除的文件名称</param>
        /// <returns>返回删除指定文件是否成功</returns>
        public bool DeleteFile(string name)
        {
            if (m_accessType != FileSystemAccessType.WriteOnly && m_accessType != FileSystemAccessType.ReadWrite)
            {
                throw new CFrameworkException("File system is not writable.");
            }

            if (string.IsNullOrEmpty(name))
            {
                throw new CFrameworkException("Name is invalid.");
            }

            if (false == m_fileDatas.TryGetValue(name, out int blockIndex))
            {
                return false;
            }

            m_fileDatas.Remove(name);

            BlockData blockData = m_blockDatas[blockIndex];
            int stringIndex = blockData.StringIndex;
            StringData stringData = m_stringDatas[stringIndex].Clear();
            m_freeStringDatas.Enqueue(new KeyValuePair<int, StringData>(stringIndex, stringData));
            m_stringDatas.Remove(stringIndex);
            WriteStringData(stringIndex, stringData);

            blockData = blockData.Free();
            m_blockDatas[blockIndex] = blockData;
            if (false == TryCombineFreeBlocks(blockIndex))
            {
                m_freeBlockIndexes.Add(blockData.Length, blockIndex);
                WriteBlockData(blockIndex);
            }

            m_stream.Flush();
            return true;
        }

        private void ProcessWriteFile(string name, bool hasFile, int oldBlockIndex, int blockIndex, int length)
        {
            BlockData blockData = m_blockDatas[blockIndex];
            if (hasFile)
            {
                BlockData oldBlockData = m_blockDatas[oldBlockIndex];
                blockData = new BlockData(oldBlockData.StringIndex, blockData.ClusterIndex, length);
                m_blockDatas[blockIndex] = blockData;
                WriteBlockData(blockIndex);

                oldBlockData = oldBlockData.Free();
                m_blockDatas[oldBlockIndex] = oldBlockData;
                if (false == TryCombineFreeBlocks(oldBlockIndex))
                {
                    m_freeBlockIndexes.Add(oldBlockData.Length, oldBlockIndex);
                    WriteBlockData(oldBlockIndex);
                }
            }
            else
            {
                int stringIndex = AllocString(name);
                blockData = new BlockData(stringIndex, blockData.ClusterIndex, length);
                m_blockDatas[blockIndex] = blockData;
                WriteBlockData(blockIndex);
            }

            if (hasFile)
            {
                m_fileDatas[name] = blockIndex;
            }
            else
            {
                m_fileDatas.Add(name, blockIndex);
            }
        }

        private bool TryCombineFreeBlocks(int freeBlockIndex)
        {
            BlockData freeBlockData = m_blockDatas[freeBlockIndex];
            if (freeBlockData.Length <= 0)
            {
                return false;
            }

            int previousFreeBlockIndex = -1;
            int nextFreeBlockIndex = -1;
            int nextBlockDataClusterIndex = freeBlockData.ClusterIndex + GetUpBoundClusterCount(freeBlockData.Length);
            foreach (KeyValuePair<int, DoubleLinkedList<int>> blockIndexes in m_freeBlockIndexes)
            {
                if (blockIndexes.Key <= 0)
                {
                    continue;
                }

                int blockDataClusterCount = GetUpBoundClusterCount(blockIndexes.Key);
                foreach (int blockIndex in blockIndexes.Value)
                {
                    BlockData blockData = m_blockDatas[blockIndex];
                    if (blockData.ClusterIndex + blockDataClusterCount == freeBlockData.ClusterIndex)
                    {
                        previousFreeBlockIndex = blockIndex;
                    }
                    else if (blockData.ClusterIndex == nextBlockDataClusterIndex)
                    {
                        nextFreeBlockIndex = blockIndex;
                    }
                }
            }

            if (previousFreeBlockIndex < 0 && nextFreeBlockIndex < 0)
            {
                return false;
            }

            m_freeBlockIndexes.Remove(freeBlockData.Length, freeBlockIndex);
            if (previousFreeBlockIndex >= 0)
            {
                BlockData previousFreeBlockData = m_blockDatas[previousFreeBlockIndex];
                m_freeBlockIndexes.Remove(previousFreeBlockData.Length, previousFreeBlockIndex);
                freeBlockData = new BlockData(previousFreeBlockData.ClusterIndex, previousFreeBlockData.Length + freeBlockData.Length);
                m_blockDatas[previousFreeBlockIndex] = BlockData.Empty;
                m_freeBlockIndexes.Add(0, previousFreeBlockIndex);
                WriteBlockData(previousFreeBlockIndex);
            }

            if (nextFreeBlockIndex >= 0)
            {
                BlockData nextFreeBlockData = m_blockDatas[nextFreeBlockIndex];
                m_freeBlockIndexes.Remove(nextFreeBlockData.Length, nextFreeBlockIndex);
                freeBlockData = new BlockData(freeBlockData.ClusterIndex, freeBlockData.Length + nextFreeBlockData.Length);
                m_blockDatas[nextFreeBlockIndex] = BlockData.Empty;
                m_freeBlockIndexes.Add(0, nextFreeBlockIndex);
                WriteBlockData(nextFreeBlockIndex);
            }

            m_blockDatas[freeBlockIndex] = freeBlockData;
            m_freeBlockIndexes.Add(freeBlockData.Length, freeBlockIndex);
            WriteBlockData(freeBlockIndex);
            return true;
        }

        private int GetEmptyBlockIndex()
        {
            DoubleLinkedList<int> lengthRange = default(DoubleLinkedList<int>);
            if (m_freeBlockIndexes.TryGetValue(0, out lengthRange))
            {
                int blockIndex = lengthRange.First.Value;
                m_freeBlockIndexes.Remove(0, blockIndex);
                return blockIndex;
            }

            if (m_blockDatas.Count < m_headerData.MaxBlockCount)
            {
                int blockIndex = m_blockDatas.Count;
                m_blockDatas.Add(BlockData.Empty);
                WriteHeaderData();
                return blockIndex;
            }

            return -1;
        }

        private int AllocBlock(int length)
        {
            if (length <= 0)
            {
                return GetEmptyBlockIndex();
            }

            length = (int) GetUpBoundClusterOffset(length);

            int lengthFound = -1;
            DoubleLinkedList<int> lengthRange = default(DoubleLinkedList<int>);
            foreach (KeyValuePair<int, DoubleLinkedList<int>> i in m_freeBlockIndexes)
            {
                if (i.Key < length)
                {
                    continue;
                }

                if (lengthFound >= 0 && lengthFound < i.Key)
                {
                    continue;
                }

                lengthFound = i.Key;
                lengthRange = i.Value;
            }

            if (lengthFound >= 0)
            {
                if (lengthFound > length && m_blockDatas.Count >= m_headerData.MaxBlockCount)
                {
                    return -1;
                }

                int blockIndex = lengthRange.First.Value;
                m_freeBlockIndexes.Remove(lengthFound, blockIndex);
                if (lengthFound > length)
                {
                    BlockData blockData = m_blockDatas[blockIndex];
                    m_blockDatas[blockIndex] = new BlockData(blockData.ClusterIndex, length);
                    WriteBlockData(blockIndex);

                    int deltaLength = lengthFound - length;
                    int anotherBlockIndex = GetEmptyBlockIndex();
                    m_blockDatas[anotherBlockIndex] = new BlockData(blockData.ClusterIndex + GetUpBoundClusterCount(length), deltaLength);
                    m_freeBlockIndexes.Add(deltaLength, anotherBlockIndex);
                    WriteBlockData(anotherBlockIndex);
                }

                return blockIndex;
            }
            else
            {
                int blockIndex = GetEmptyBlockIndex();
                if (blockIndex < 0)
                {
                    return -1;
                }

                long fileLength = m_stream.Length;
                try
                {
                    m_stream.SetLength(fileLength + length);
                }
                catch
                {
                    return -1;
                }

                m_blockDatas[blockIndex] = new BlockData(GetUpBoundClusterCount(fileLength), length);
                WriteBlockData(blockIndex);
                return blockIndex;
            }
        }

        private int AllocString(string value)
        {
            int stringIndex = -1;
            StringData stringData = default(StringData);

            if (m_freeStringDatas.Count > 0)
            {
                KeyValuePair<int, StringData> freeStringData = m_freeStringDatas.Dequeue();
                stringIndex = freeStringData.Key;
                stringData = freeStringData.Value;
            }
            else
            {
                int index = 0;
                foreach (KeyValuePair<int, StringData> k in m_stringDatas)
                {
                    if (k.Key == index)
                    {
                        index++;
                        continue;
                    }

                    break;
                }

                if (index < m_headerData.MaxFileCount)
                {
                    stringIndex = index;
                    byte[] bytes = new byte[byte.MaxValue];
                    Utility.Random.GetRandomBytes(bytes);
                    stringData = new StringData(0, bytes);
                }
            }

            if (stringIndex < 0)
            {
                throw new CFrameworkException("Alloc string internal error.");
            }

            stringData = stringData.SetString(value, m_headerData.GetEncryptBytes());
            m_stringDatas.Add(stringIndex, stringData);
            WriteStringData(stringIndex, stringData);
            return stringIndex;
        }

        private void WriteHeaderData()
        {
            m_headerData = m_headerData.SetBlockCount(m_blockDatas.Count);
            Utility.Marshal.StructureToBytes(m_headerData, HeaderDataSize, s_cachedBytes);
            m_stream.Position = 0L;
            m_stream.Write(s_cachedBytes, 0, HeaderDataSize);
        }

        private void WriteBlockData(int blockIndex)
        {
            Utility.Marshal.StructureToBytes(m_blockDatas[blockIndex], BlockDataSize, s_cachedBytes);
            m_stream.Position = m_blockDataOffset + BlockDataSize * blockIndex;
            m_stream.Write(s_cachedBytes, 0, BlockDataSize);
        }

        private StringData ReadStringData(int stringIndex)
        {
            m_stream.Position = m_stringDataOffset + StringDataSize * stringIndex;
            m_stream.Read(s_cachedBytes, 0, StringDataSize);
            return Utility.Marshal.BytesToStructure<StringData>(StringDataSize, s_cachedBytes);
        }

        private void WriteStringData(int stringIndex, StringData stringData)
        {
            Utility.Marshal.StructureToBytes(stringData, StringDataSize, s_cachedBytes);
            m_stream.Position = m_stringDataOffset + StringDataSize * stringIndex;
            m_stream.Write(s_cachedBytes, 0, StringDataSize);
        }

        private static void CalcOffsets(FileSystem fileSystem)
        {
            fileSystem.m_blockDataOffset = HeaderDataSize;
            fileSystem.m_stringDataOffset = fileSystem.m_blockDataOffset + BlockDataSize * fileSystem.m_headerData.MaxBlockCount;
            fileSystem.m_fileDataOffset = (int) GetUpBoundClusterOffset(fileSystem.m_stringDataOffset + StringDataSize * fileSystem.m_headerData.MaxFileCount);
        }

        private static long GetUpBoundClusterOffset(long offset)
        {
            return (offset - 1L + CLUSTER_SIZE) / CLUSTER_SIZE * CLUSTER_SIZE;
        }

        private static int GetUpBoundClusterCount(long length)
        {
            return (int) ((length - 1L + CLUSTER_SIZE) / CLUSTER_SIZE);
        }

        private static long GetClusterOffset(int clusterIndex)
        {
            return (long) CLUSTER_SIZE * clusterIndex;
        }
    }
}
