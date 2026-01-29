/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyright (C) 2022 - 2023, Shanghai Bilibili Technology Co., Ltd.
/// Copyright (C) 2023 - 2024, Guangzhou Shiyue Network Technology Co., Ltd.
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

namespace GameEngine
{
    /// <summary>
    /// 基础框架的配置参数定义文件，包括环境参数，版本参数等内容
    /// </summary>
    public static class GameMacros
    {
        #region 引擎内部使用的全局常量定义

        /// <summary>
        /// 业务导入模块的对外关口名称
        /// </summary>
        public const string GAME_IMPORT_MODULE_EXTERNAL_GATEWAY_NAME = @"GameEngine.GameImport";

        /// <summary>
        /// 业务远程服务调用的运行服务接口名称
        /// </summary>
        public const string GAME_REMOTE_PROCESS_CALL_RUN_SERVICE_NAME = @"Run";
        /// <summary>
        /// 业务远程服务调用的停止服务接口名称
        /// </summary>
        public const string GAME_REMOTE_PROCESS_CALL_STOP_SERVICE_NAME = @"Stop";
        /// <summary>
        /// 业务远程服务调用的重启服务接口名称
        /// </summary>
        public const string GAME_REMOTE_PROCESS_CALL_REBOOT_SERVICE_NAME = @"Reboot";
        /// <summary>
        /// 业务远程服务调用的重载服务接口名称
        /// </summary>
        public const string GAME_REMOTE_PROCESS_CALL_RELOAD_SERVICE_NAME = @"Reload";

        #endregion
    }
}
