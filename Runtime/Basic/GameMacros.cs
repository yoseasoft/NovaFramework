/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyright (C) 2022 - 2023, Shanghai Bilibili Technology Co., Ltd.
/// Copyright (C) 2023 - 2024, Guangzhou Shiyue Network Technology Co., Ltd.
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

namespace GameEngine
{
    /// <summary>
    /// 基础框架的配置参数定义文件，包括环境参数，版本参数等内容
    /// </summary>
    public static class GameMacros
    {
        /// <summary>
        /// 调试模块的输出分组策略启动状态标识
        /// </summary>
        [DisableOnReleaseMode]
        public static readonly bool DEBUGGING_OUTPUT_GROUP_POLICY_ENABLED = true;

        /// <summary>
        /// 调试模式的解析窗口自动挂载的状态标识
        /// </summary>
        [DisableOnReleaseMode]
        public static readonly bool DEBUGGING_PROFILER_WINDOW_AUTO_MOUNTED = false;

        /// <summary>
        /// 编辑器环境下的编译代码支持补丁热重载功能
        /// </summary>
        [DisableOnReleaseMode]
        public static readonly bool EDITOR_COMPILING_CODE_HOTFIX_RELOAD_SUPPORTED = true;

        /// <summary>
        /// 业务导入流程的调度转发功能启动的状态标识<br/>
        /// 注意这个标识需手动设置，确定当前项目是否需要接入业务导入流程，从而决定是否需要开启该表示
        /// </summary>
        public static readonly bool GAME_IMPORT_DISPATCHING_FORWARD_ENABLED = true;

        #region 引擎内部使用的全局常量定义

        /// <summary>
        /// 业务导入模块的入口名称
        /// </summary>
        public const string GAME_IMPORT_MODULE_ENTRANCE_NAME = @"GameEngine.GameImport";
        /// <summary>
        /// 业务管理模块的入口名称
        /// </summary>
        public const string GAME_WORLD_MODULE_ENTRANCE_NAME = @"Game.GameWorld";

        /// <summary>
        /// 业务远程服务调用的运行服务接口名称
        /// </summary>
        public const string GAME_REMOTE_PROCESS_CALL_RUN_SERVICE_NAME = "Run";
        /// <summary>
        /// 业务远程服务调用的停止服务接口名称
        /// </summary>
        public const string GAME_REMOTE_PROCESS_CALL_STOP_SERVICE_NAME = "Stop";
        /// <summary>
        /// 业务远程服务调用的重载服务接口名称
        /// </summary>
        public const string GAME_REMOTE_PROCESS_CALL_RELOAD_SERVICE_NAME = "Reload";

        #endregion
    }
}
