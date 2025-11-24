/// -------------------------------------------------------------------------------
/// NovaEngine Framework
///
/// Copyright (C) 2018, Shanghai Tommon Network Technology Co., Ltd.
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

namespace NovaEngine
{
    /// <summary>
    /// 基础常量数据定义类，将基础数据类型中的一些通用常量在此统一进行定义使用
    /// </summary>
    public static partial class Definition
    {
        /// <summary>
        /// 文件访问相关常量数据定义
        /// </summary>
        public static class File
        {
            /// <summary>
            /// 文件流数据加载的函数句柄定义
            /// </summary>
            /// <param name="url">资源路径</param>
            /// <param name="ms">数据流</param>
            /// <returns>加载成功返回true，若加载失败返回false</returns>
            public delegate bool OnFileStreamLoadingHandler(string url, MemoryStream ms);

            /// <summary>
            /// 文件字符数据加载的函数句柄定义
            /// </summary>
            /// <param name="url">资源路径</param>
            /// <returns>返回加载成功的配置数据内容，若加载失败返回null</returns>
            public delegate string OnFileTextLoadingHandler(string url);

            // 资源文件
            public const string JPG         = ".jpg";
            public const string PNG         = ".png";
            public const string PDF         = ".pdf";
            public const string TGA         = ".tga";
            public const string TIF         = ".tif";
            public const string DDS         = ".dds";
            public const string MAT         = ".mat";
            public const string FBX         = ".fbx";
            public const string CONTROLLER  = ".controller";
            public const string PREFAB      = ".prefab";
            public const string MP3         = ".mp3";
            public const string MP4         = ".mp4";
            public const string OGG         = ".ogg";
            public const string UNITY       = ".unity";
            public const string UNITY3D     = ".unity3d";

            // 编码文件
            public const string CS          = ".cs";
            public const string JS          = ".js";
            public const string TS          = ".ts";
            public const string LUA         = ".lua";
            public const string BYTES       = ".bytes";
            public const string SHADER      = ".shader";

            // 库文件
            public const string PB          = ".pb";
            public const string PDB         = ".pdb"; 
            public const string DLL         = ".dll";
            public const string SO          = ".so";
            public const string A           = ".a";

            // 文本文件
            public const string TXT         = ".txt";
            public const string INI         = ".ini";
            public const string DAT         = ".dat";
            public const string CSV         = ".csv";
            public const string XML         = ".xml";
            public const string JSON        = ".json";
            public const string YAML        = ".yaml";
            public const string PLIST       = ".plist";

            // 可执行脚本文件
            public const string SH          = ".sh";
            public const string BAT         = ".bat";
        }
    }
}
