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

using UnityApplication = UnityEngine.Application;
using UnityRuntimePlatform = UnityEngine.RuntimePlatform;

namespace NovaEngine
{
    /// <summary>
    /// 实用函数集合工具类
    /// </summary>
    public static partial class Utility
    {
        /// <summary>
        /// 资源访问相关实用函数集合
        /// </summary>
        public static class Resource
        {
            /// <summary>
            /// 标准资源目录名称
            /// </summary>
            private const string STREAMING_ASSETS_DIR_NAME = "StreamingAssets";

            /// <summary>
            /// 程序资源存储目录路径
            /// </summary>
            public static string StreamingAssetsPath
            {
                get
                {
                    return UnityApplication.streamingAssetsPath;
                }
            }

            /// <summary>
            /// 程序应用存储目录路径
            /// </summary>
            public static string ApplicationDataPath
            {
                get
                {
                    return UnityApplication.dataPath;
                }
            }

            /// <summary>
            /// 程序数据存储目录路径
            /// 一般作为程序外部写入目录的标准路径
            /// </summary>
            public static string PersistentDataPath
            {
                get
                {
                    return UnityApplication.persistentDataPath;
                }
            }

            /// <summary>
            /// 程序数据存储缓存目录路径
            /// </summary>
            public static string PersistentDataCachePath
            {
                get
                {
                    return PersistentDataPath + "/cache";
                }
            }

            /// <summary>
            /// 程序临时数据缓存目录路径
            /// </summary>
            public static string TemporaryCachePath
            {
                get
                {
                    return UnityApplication.temporaryCachePath;
                }
            }

            /// <summary>
            /// 程序原生内容数据存储路径
            /// 程序的只读目录的StreamingAssets路径
            /// </summary>
            public static string ApplicationContentPath
            {
                get
                {
                    if (UnityApplication.platform == UnityRuntimePlatform.Android)
                    {
                        return "jar:file://" + UnityApplication.dataPath + "!/assets";
                    }
                    else if (UnityApplication.platform == UnityRuntimePlatform.IPhonePlayer)
                    {
                        return "file://" + UnityApplication.dataPath + "/Raw";
                    }
                    else // other platform
                    {
                        return "file://" + UnityApplication.dataPath + "/" + STREAMING_ASSETS_DIR_NAME;
                    }
                }
            }
        }
    }
}
