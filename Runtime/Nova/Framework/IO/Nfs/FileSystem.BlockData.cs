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

namespace NovaEngine.IO.FileSystem
{
    /// <summary>
    /// 文件系统基类
    /// </summary>
    internal sealed partial class FileSystem : IFileSystem
    {
        /// <summary>
        /// 文件系统的块数据流对象类
        /// </summary>
        private struct BlockData
        {
            public static readonly BlockData Empty = new BlockData(0, 0);

            private readonly int _stringIndex;
            private readonly int _clusterIndex;
            private readonly int _length;

            public BlockData(int clusterIndex, int length)
                : this(-1, clusterIndex, length)
            {
            }

            public BlockData(int stringIndex, int clusterIndex, int length)
            {
                this._stringIndex = stringIndex;
                this._clusterIndex = clusterIndex;
                this._length = length;
            }

            public bool Using
            {
                get { return _stringIndex >= 0; }
            }

            public int StringIndex
            {
                get { return _stringIndex; }
            }

            public int ClusterIndex
            {
                get { return _clusterIndex; }
            }

            public int Length
            {
                get { return _length; }
            }

            public BlockData Free()
            {
                return new BlockData(_clusterIndex, (int) GetUpBoundClusterOffset(_length));
            }
        }
    }
}
