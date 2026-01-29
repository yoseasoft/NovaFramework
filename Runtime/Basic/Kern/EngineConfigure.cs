/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyright (C) 2020 - 2022, Guangzhou Xinyuan Technology Co., Ltd.
/// Copyright (C) 2022 - 2023, Shanghai Bilibili Technology Co., Ltd.
/// Copyright (C) 2023 - 2024, Guangzhou Shiyue Network Technology Co., Ltd.
/// Copyright (C) 2025, Hurley, Independent Studio.
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

using System;
using System.Collections.Generic;
using System.Reflection;

namespace GameEngine
{
    /// <summary>
    /// 游戏层配置封装管理类
    /// </summary>
    internal static class EngineConfigure
    {
        /// <summary>
        /// 初始化平台环境及相关接口
        /// </summary>
        private static void InitPlatform()
        {
            // 引擎帧率设置
            UnityEngine.Application.targetFrameRate = NovaEngine.Environment.AnimationRate;

            NovaEngine.Application.Instance.AddProtocolTransformationHandler(EngineDispatcher.OnApplicationResponseCallback);

            // 日志开启
            NovaEngine.Logger.Startup();
        }

        /// <summary>
        /// 游戏配置相关初始化函数
        /// </summary>
        public static void InitGameConfig(IReadOnlyDictionary<string, string> variables)
        {
            // 加载配置
            LoadEnvironmentAndConfigureSettings(variables);

            // 初始化平台
            InitPlatform();

            // 调试器参数初始化
            Debugger.Startup();
        }

        /// <summary>
        /// 游戏配置相关清理函数
        /// </summary>
        public static void CleanupGameConfig()
        {
            // 调试器参数清理
            Debugger.Shutdown();

            // 日志关闭
            NovaEngine.Logger.Shutdown();

            NovaEngine.Application.Instance.RemoveProtocolTransformationHandler(EngineDispatcher.OnApplicationResponseCallback);
        }

        #region 配置参数加载相关接口函数

        /// <summary>
        /// 加载配置参数数据
        /// </summary>
        /// <param name="variables">环境参数集合</param>
        private static void LoadEnvironmentAndConfigureSettings(IReadOnlyDictionary<string, string> variables)
        {
            // 加载环境参数
            NovaEngine.Environment.Load(variables);

            Debugger.Log("Environment={{{%s}}}, Configuration={{{%s}}}",
                NovaEngine.Environment.ToCString(), NovaEngine.Configuration.ToCString());
        }

        #endregion
    }
}
