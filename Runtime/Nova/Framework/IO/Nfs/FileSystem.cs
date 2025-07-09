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
        private static readonly byte[] _cachedBytes = new byte[CACHED_BYTES_LENGTH];

        private static readonly int HeaderDataSize = Marshal.SizeOf(typeof(HeaderData));
        private static readonly int BlockDataSize = Marshal.SizeOf(typeof(BlockData));
        private static readonly int StringDataSize = Marshal.SizeOf(typeof(StringData));

        private readonly string _fullPath;
        private readonly FileSystemAccessType _accessType;
        private readonly FileSystemStream _stream;
        private readonly Dictionary<string, int> _fileDatas;
        private readonly List<BlockData> _blockDatas;
        private readonly MultiDictionary<int, int> _freeBlockIndexes;
        private readonly SortedDictionary<int, StringData> _stringDatas;
        private readonly Queue<KeyValuePair<int, StringData>> _freeStringDatas;

        private HeaderData _headerData;
        private int _blockDataOffset;
        private int _stringDataOffset;
        private int _fileDataOffset;

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

            this._fullPath = fullPath;
            this._accessType = accessType;
            this._stream = stream;
            this._fileDatas = new Dictionary<string, int>(SystemStringComparer.Ordinal);
            this._blockDatas = new List<BlockData>();
            this._freeBlockIndexes = new MultiDictionary<int, int>();
            this._stringDatas = new SortedDictionary<int, StringData>();
            this._freeStringDatas = new Queue<KeyValuePair<int, StringData>>();

            this._headerData = default(HeaderData);
            this._blockDataOffset = 0;
            this._stringDataOffset = 0;
            this._fileDataOffset = 0;

            Utility.Marshal.EnsureCachedHGlobalSize(CACHED_BYTES_LENGTH);
        }

        /// <summary>
        /// 获取文件系统完整路径
        /// </summary>
        public string FullPath
        {
            get { return _fullPath; }
        }

        /// <summary>
        /// 获取文件系统访问方式
        /// </summary>
        public FileSystemAccessType AccessType
        {
            get { return _accessType; }
        }

        /// <summary>
        /// 获取文件数量
        /// </summary>
        public int FileCount
        {
            get { return _fileDatas.Count; }
        }

        /// <summary>
        /// 获取最大文件数量
        /// </summary>
        public int MaxFileCount
        {
            get { return _headerData.MaxFileCount; }
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
            fileSystem._headerData = new HeaderData(maxFileCount, maxBlockCount);
            CalcOffsets(fileSystem);
            Utility.Marshal.StructureToBytes(fileSystem._headerData, HeaderDataSize, _cachedBytes);

            try
            {
                stream.Write(_cachedBytes, 0, HeaderDataSize);
                stream.SetLength(fileSystem._fileDataOffset);
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

            stream.Read(_cachedBytes, 0, HeaderDataSize);
            fileSystem._headerData = Utility.Marshal.BytesToStructure<HeaderData>(HeaderDataSize, _cachedBytes);
            CalcOffsets(fileSystem);

            if (fileSystem._blockDatas.Capacity < fileSystem._headerData.BlockCount)
            {
                fileSystem._blockDatas.Capacity = fileSystem._headerData.BlockCount;
            }

            for (int n = 0; n < fileSystem._headerData.BlockCount; ++n)
            {
                stream.Read(_cachedBytes, 0, BlockDataSize);
                BlockData blockData = Utility.Marshal.BytesToStructure<BlockData>(BlockDataSize, _cachedBytes);
                fileSystem._blockDatas.Add(blockData);
            }

            for (int n = 0; n < fileSystem._blockDatas.Count; ++n)
            {
                BlockData blockData = fileSystem._blockDatas[n];
                if (blockData.Using)
                {
                    StringData stringData = fileSystem.ReadStringData(blockData.StringIndex);
                    fileSystem._stringDatas.Add(blockData.StringIndex, stringData);
                    fileSystem._fileDatas.Add(stringData.GetString(fileSystem._headerData.GetEncryptBytes()), n);
                }
                else
                {
                    fileSystem._freeBlockIndexes.Add(blockData.Length, n);
                }
            }

            return fileSystem;
        }

        /// <summary>
        /// 关闭并清理文件系统
        /// </summary>
        public void Shutdown()
        {
            this._stream.Close();

            this._fileDatas.Clear();
            this._blockDatas.Clear();
            this._freeBlockIndexes.Clear();
            this._stringDatas.Clear();
            this._freeStringDatas.Clear();

            this._blockDataOffset = 0;
            this._stringDataOffset = 0;
            this._fileDataOffset = 0;
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

            if (false == _fileDatas.TryGetValue(name, out int blockIndex))
            {
                return default(FileInfo);
            }

            BlockData blockData = _blockDatas[blockIndex];
            return new FileInfo(name, GetClusterOffset(blockData.ClusterIndex), blockData.Length);
        }

        /// <summary>
        /// 获取当前系统中所有文件信息
        /// </summary>
        /// <returns>返回全部文件信息实例</returns>
        public FileInfo[] GetAllFileInfos()
        {
            int index = 0;
            FileInfo[] results = new FileInfo[_fileDatas.Count];
            foreach (KeyValuePair<string, int> fileData in _fileDatas)
            {
                BlockData blockData = _blockDatas[fileData.Value];
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
            foreach (KeyValuePair<string, int> fileData in _fileDatas)
            {
                BlockData blockData = _blockDatas[fileData.Value];
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

            return _fileDatas.ContainsKey(name);
        }

        /// <summary>
        /// 读取指定文件
        /// </summary>
        /// <param name="name">要读取的文件名称</param>
        /// <returns>返回读取文件内容的二进制流</returns>
        public byte[] ReadFile(string name)
        {
            if (_accessType != FileSystemAccessType.ReadOnly && _accessType != FileSystemAccessType.ReadWrite)
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
                _stream.Position = fileInfo.Offset;
                _stream.Read(buffer, 0, length);
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
            if (_accessType != FileSystemAccessType.ReadOnly && _accessType != FileSystemAccessType.ReadWrite)
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

            _stream.Position = fileInfo.Offset;
            if (length > fileInfo.Length)
            {
                length = fileInfo.Length;
            }

            if (length > 0)
            {
                return _stream.Read(buffer, startIndex, length);
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
            if (_accessType != FileSystemAccessType.ReadOnly && _accessType != FileSystemAccessType.ReadWrite)
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
                _stream.Position = fileInfo.Offset;
                return _stream.Read(stream, length);
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
            if (_accessType != FileSystemAccessType.ReadOnly && _accessType != FileSystemAccessType.ReadWrite)
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
                _stream.Position = fileInfo.Offset + offset;
                _stream.Read(buffer, 0, length);
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
            if (_accessType != FileSystemAccessType.ReadOnly && _accessType != FileSystemAccessType.ReadWrite)
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
                _stream.Position = fileInfo.Offset + offset;
                return _stream.Read(buffer, startIndex, length);
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
            if (_accessType != FileSystemAccessType.ReadOnly && _accessType != FileSystemAccessType.ReadWrite)
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
                _stream.Position = fileInfo.Offset + offset;
                return _stream.Read(stream, length);
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
            if (_accessType != FileSystemAccessType.WriteOnly && _accessType != FileSystemAccessType.ReadWrite)
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
            if (_fileDatas.TryGetValue(name, out oldBlockIndex))
            {
                hasFile = true;
            }

            if (false == hasFile && _fileDatas.Count >= _headerData.MaxFileCount)
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
                _stream.Position = GetClusterOffset(_blockDatas[blockIndex].ClusterIndex);
                _stream.Write(buffer, startIndex, length);
            }

            ProcessWriteFile(name, hasFile, oldBlockIndex, blockIndex, length);
            _stream.Flush();
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
            if (_accessType != FileSystemAccessType.WriteOnly && _accessType != FileSystemAccessType.ReadWrite)
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
            if (_fileDatas.TryGetValue(name, out oldBlockIndex))
            {
                hasFile = true;
            }

            if (false == hasFile && _fileDatas.Count >= _headerData.MaxFileCount)
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
                _stream.Position = GetClusterOffset(_blockDatas[blockIndex].ClusterIndex);
                _stream.Write(stream, length);
            }

            ProcessWriteFile(name, hasFile, oldBlockIndex, blockIndex, length);
            _stream.Flush();
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
            if (_accessType != FileSystemAccessType.ReadOnly && _accessType != FileSystemAccessType.ReadWrite)
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
                        _stream.Position = fileInfo.Offset;
                        return _stream.Read(fileStream, length) == length;
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
            if (_accessType != FileSystemAccessType.WriteOnly && _accessType != FileSystemAccessType.ReadWrite)
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

            if (_fileDatas.ContainsKey(newName))
            {
                return false;
            }

            if (false == _fileDatas.TryGetValue(oldName, out int blockIndex))
            {
                return false;
            }

            int stringIndex = _blockDatas[blockIndex].StringIndex;
            StringData stringData = _stringDatas[stringIndex].SetString(newName, _headerData.GetEncryptBytes());
            _stringDatas[stringIndex] = stringData;
            WriteStringData(stringIndex, stringData);
            _fileDatas.Add(newName, blockIndex);
            _fileDatas.Remove(oldName);
            _stream.Flush();
            return true;
        }

        /// <summary>
        /// 删除指定文件
        /// </summary>
        /// <param name="name">要删除的文件名称</param>
        /// <returns>返回删除指定文件是否成功</returns>
        public bool DeleteFile(string name)
        {
            if (_accessType != FileSystemAccessType.WriteOnly && _accessType != FileSystemAccessType.ReadWrite)
            {
                throw new CFrameworkException("File system is not writable.");
            }

            if (string.IsNullOrEmpty(name))
            {
                throw new CFrameworkException("Name is invalid.");
            }

            if (false == _fileDatas.TryGetValue(name, out int blockIndex))
            {
                return false;
            }

            _fileDatas.Remove(name);

            BlockData blockData = _blockDatas[blockIndex];
            int stringIndex = blockData.StringIndex;
            StringData stringData = _stringDatas[stringIndex].Clear();
            _freeStringDatas.Enqueue(new KeyValuePair<int, StringData>(stringIndex, stringData));
            _stringDatas.Remove(stringIndex);
            WriteStringData(stringIndex, stringData);

            blockData = blockData.Free();
            _blockDatas[blockIndex] = blockData;
            if (false == TryCombineFreeBlocks(blockIndex))
            {
                _freeBlockIndexes.Add(blockData.Length, blockIndex);
                WriteBlockData(blockIndex);
            }

            _stream.Flush();
            return true;
        }

        private void ProcessWriteFile(string name, bool hasFile, int oldBlockIndex, int blockIndex, int length)
        {
            BlockData blockData = _blockDatas[blockIndex];
            if (hasFile)
            {
                BlockData oldBlockData = _blockDatas[oldBlockIndex];
                blockData = new BlockData(oldBlockData.StringIndex, blockData.ClusterIndex, length);
                _blockDatas[blockIndex] = blockData;
                WriteBlockData(blockIndex);

                oldBlockData = oldBlockData.Free();
                _blockDatas[oldBlockIndex] = oldBlockData;
                if (false == TryCombineFreeBlocks(oldBlockIndex))
                {
                    _freeBlockIndexes.Add(oldBlockData.Length, oldBlockIndex);
                    WriteBlockData(oldBlockIndex);
                }
            }
            else
            {
                int stringIndex = AllocString(name);
                blockData = new BlockData(stringIndex, blockData.ClusterIndex, length);
                _blockDatas[blockIndex] = blockData;
                WriteBlockData(blockIndex);
            }

            if (hasFile)
            {
                _fileDatas[name] = blockIndex;
            }
            else
            {
                _fileDatas.Add(name, blockIndex);
            }
        }

        private bool TryCombineFreeBlocks(int freeBlockIndex)
        {
            BlockData freeBlockData = _blockDatas[freeBlockIndex];
            if (freeBlockData.Length <= 0)
            {
                return false;
            }

            int previousFreeBlockIndex = -1;
            int nextFreeBlockIndex = -1;
            int nextBlockDataClusterIndex = freeBlockData.ClusterIndex + GetUpBoundClusterCount(freeBlockData.Length);
            foreach (KeyValuePair<int, DoubleLinkedList<int>> blockIndexes in _freeBlockIndexes)
            {
                if (blockIndexes.Key <= 0)
                {
                    continue;
                }

                int blockDataClusterCount = GetUpBoundClusterCount(blockIndexes.Key);
                foreach (int blockIndex in blockIndexes.Value)
                {
                    BlockData blockData = _blockDatas[blockIndex];
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

            _freeBlockIndexes.Remove(freeBlockData.Length, freeBlockIndex);
            if (previousFreeBlockIndex >= 0)
            {
                BlockData previousFreeBlockData = _blockDatas[previousFreeBlockIndex];
                _freeBlockIndexes.Remove(previousFreeBlockData.Length, previousFreeBlockIndex);
                freeBlockData = new BlockData(previousFreeBlockData.ClusterIndex, previousFreeBlockData.Length + freeBlockData.Length);
                _blockDatas[previousFreeBlockIndex] = BlockData.Empty;
                _freeBlockIndexes.Add(0, previousFreeBlockIndex);
                WriteBlockData(previousFreeBlockIndex);
            }

            if (nextFreeBlockIndex >= 0)
            {
                BlockData nextFreeBlockData = _blockDatas[nextFreeBlockIndex];
                _freeBlockIndexes.Remove(nextFreeBlockData.Length, nextFreeBlockIndex);
                freeBlockData = new BlockData(freeBlockData.ClusterIndex, freeBlockData.Length + nextFreeBlockData.Length);
                _blockDatas[nextFreeBlockIndex] = BlockData.Empty;
                _freeBlockIndexes.Add(0, nextFreeBlockIndex);
                WriteBlockData(nextFreeBlockIndex);
            }

            _blockDatas[freeBlockIndex] = freeBlockData;
            _freeBlockIndexes.Add(freeBlockData.Length, freeBlockIndex);
            WriteBlockData(freeBlockIndex);
            return true;
        }

        private int GetEmptyBlockIndex()
        {
            DoubleLinkedList<int> lengthRange = default(DoubleLinkedList<int>);
            if (_freeBlockIndexes.TryGetValue(0, out lengthRange))
            {
                int blockIndex = lengthRange.First.Value;
                _freeBlockIndexes.Remove(0, blockIndex);
                return blockIndex;
            }

            if (_blockDatas.Count < _headerData.MaxBlockCount)
            {
                int blockIndex = _blockDatas.Count;
                _blockDatas.Add(BlockData.Empty);
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
            foreach (KeyValuePair<int, DoubleLinkedList<int>> i in _freeBlockIndexes)
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
                if (lengthFound > length && _blockDatas.Count >= _headerData.MaxBlockCount)
                {
                    return -1;
                }

                int blockIndex = lengthRange.First.Value;
                _freeBlockIndexes.Remove(lengthFound, blockIndex);
                if (lengthFound > length)
                {
                    BlockData blockData = _blockDatas[blockIndex];
                    _blockDatas[blockIndex] = new BlockData(blockData.ClusterIndex, length);
                    WriteBlockData(blockIndex);

                    int deltaLength = lengthFound - length;
                    int anotherBlockIndex = GetEmptyBlockIndex();
                    _blockDatas[anotherBlockIndex] = new BlockData(blockData.ClusterIndex + GetUpBoundClusterCount(length), deltaLength);
                    _freeBlockIndexes.Add(deltaLength, anotherBlockIndex);
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

                long fileLength = _stream.Length;
                try
                {
                    _stream.SetLength(fileLength + length);
                }
                catch
                {
                    return -1;
                }

                _blockDatas[blockIndex] = new BlockData(GetUpBoundClusterCount(fileLength), length);
                WriteBlockData(blockIndex);
                return blockIndex;
            }
        }

        private int AllocString(string value)
        {
            int stringIndex = -1;
            StringData stringData = default(StringData);

            if (_freeStringDatas.Count > 0)
            {
                KeyValuePair<int, StringData> freeStringData = _freeStringDatas.Dequeue();
                stringIndex = freeStringData.Key;
                stringData = freeStringData.Value;
            }
            else
            {
                int index = 0;
                foreach (KeyValuePair<int, StringData> k in _stringDatas)
                {
                    if (k.Key == index)
                    {
                        index++;
                        continue;
                    }

                    break;
                }

                if (index < _headerData.MaxFileCount)
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

            stringData = stringData.SetString(value, _headerData.GetEncryptBytes());
            _stringDatas.Add(stringIndex, stringData);
            WriteStringData(stringIndex, stringData);
            return stringIndex;
        }

        private void WriteHeaderData()
        {
            _headerData = _headerData.SetBlockCount(_blockDatas.Count);
            Utility.Marshal.StructureToBytes(_headerData, HeaderDataSize, _cachedBytes);
            _stream.Position = 0L;
            _stream.Write(_cachedBytes, 0, HeaderDataSize);
        }

        private void WriteBlockData(int blockIndex)
        {
            Utility.Marshal.StructureToBytes(_blockDatas[blockIndex], BlockDataSize, _cachedBytes);
            _stream.Position = _blockDataOffset + BlockDataSize * blockIndex;
            _stream.Write(_cachedBytes, 0, BlockDataSize);
        }

        private StringData ReadStringData(int stringIndex)
        {
            _stream.Position = _stringDataOffset + StringDataSize * stringIndex;
            _stream.Read(_cachedBytes, 0, StringDataSize);
            return Utility.Marshal.BytesToStructure<StringData>(StringDataSize, _cachedBytes);
        }

        private void WriteStringData(int stringIndex, StringData stringData)
        {
            Utility.Marshal.StructureToBytes(stringData, StringDataSize, _cachedBytes);
            _stream.Position = _stringDataOffset + StringDataSize * stringIndex;
            _stream.Write(_cachedBytes, 0, StringDataSize);
        }

        private static void CalcOffsets(FileSystem fileSystem)
        {
            fileSystem._blockDataOffset = HeaderDataSize;
            fileSystem._stringDataOffset = fileSystem._blockDataOffset + BlockDataSize * fileSystem._headerData.MaxBlockCount;
            fileSystem._fileDataOffset = (int) GetUpBoundClusterOffset(fileSystem._stringDataOffset + StringDataSize * fileSystem._headerData.MaxFileCount);
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
