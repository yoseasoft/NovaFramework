/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyright (C) 2025 - 2026, Hainan Yuanyou Information Technology Co., Ltd. Guangzhou Branch
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
using System.Runtime.CompilerServices;

namespace GameEngine.Context.Configuring
{
    /// <summary>
    /// 应用配置信息的对象类，用于存放加载的应用配置信息，并提供对外访问操作接口
    /// </summary>
    static class ModuleConfigureInfo
    {
        /// <summary>
        /// 热加载模块包列表
        /// </summary>
        private static IList<string> _hotLoadPacks;

        public static IList<string> HotLoadPacks => _hotLoadPacks;

        /// <summary>
        /// 配置信息初始化函数
        /// </summary>
        public static void Initialize()
        {
            // 初始化容器
            _hotLoadPacks = new List<string>();
        }

        /// <summary>
        /// 配置信息清理函数
        /// </summary>
        public static void Cleanup()
        {
            // 清理容器
            RemoveAllConfigureInfos();

            _hotLoadPacks = null;
        }

        /// <summary>
        /// 移除所有的配置信息
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RemoveAllConfigureInfos()
        {
            RemoveAllHotLoadPacks();
        }

        /// <summary>
        /// 新增热加载模块的包名称
        /// </summary>
        /// <param name="packName">包名称</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddHotLoadPack(string packName)
        {
            if (_hotLoadPacks.Contains(packName))
            {
                Debugger.Warn(LogGroupTag.Basic, "The hot load pack '{%s}' was already exists, repeat added it failed.", packName);
                return;
            }

            _hotLoadPacks.Add(packName);
        }

        /// <summary>
        /// 移除当前记录的所有热加载模块包名称
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void RemoveAllHotLoadPacks()
        {
            _hotLoadPacks.Clear();
        }
    }
}
