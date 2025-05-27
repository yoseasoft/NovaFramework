/// -------------------------------------------------------------------------------
/// GameEngine Framework
///
/// Copyring (C) 2023 - 2024, Guangzhou Shiyue Network Technology Co., Ltd.
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
    /// 游戏层接口调用封装类，用于对远程游戏业务提供的函数访问接口进行方法封装
    /// </summary>
    public static partial class GameCall
    {
        /// <summary>
        /// 启动业务层模块
        /// </summary>
        public static void Startup()
        {
            // 在业务启动前打开系统库
            GameLibrary.Startup();

            // 业务启动
            RunGame();
        }

        /// <summary>
        /// 关闭业务层模块
        /// </summary>
        public static void Shutdown()
        {
            // 业务停止
            StopGame();

            // 在业务停止后关闭系统库
            GameLibrary.Shutdown();
        }

        /// <summary>
        /// 重启业务层模块
        /// </summary>
        public static void Restart()
        {
            if (false == GameMacros.EDITOR_COMPILING_CODE_HOTFIX_RELOAD_SUPPORTED)
            {
                // 禁用热重载功能
                Debugger.Error("Not supported compiling code hotfix reload with current running mode, restarted game context failed.");
                return;
            }

            // 先重启系统库在进行业务重载
            GameLibrary.Restart();

            // 业务重载
            ReloadGame();

            // 业务重载完成后，对其上下文进行刷新操作
            GameLibrary.ReloadContext();
        }

        /// <summary>
        /// 运行游戏业务层模块
        /// </summary>
        private static void RunGame()
        {
            BeforeRunGame();

            // 这里调整为运行中间层模块，通过中间层对业务层进行加载及调用
            // 这样的话，可以将热更环节调整到该模块进行
            CallRemoteService(GameMacros.GAME_REMOTE_PROCESS_CALL_RUN_SERVICE_NAME);
        }

        /// <summary>
        /// 停止游戏业务层模块
        /// </summary>
        private static void StopGame()
        {
            // 通知中间层模块停止业务
            CallRemoteService(GameMacros.GAME_REMOTE_PROCESS_CALL_STOP_SERVICE_NAME);

            AfterStopGame();
        }

        /// <summary>
        /// 重载游戏业务层模块
        /// </summary>
        private static void ReloadGame()
        {
            // 通知中间层模块重载业务
            CallRemoteService(GameMacros.GAME_REMOTE_PROCESS_CALL_RELOAD_SERVICE_NAME);
        }

        /// <summary>
        /// 调用远程服务的指定函数
        /// </summary>
        /// <param name="methodName">函数名称</param>
        private static void CallRemoteService(string methodName)
        {
            string targetName = GameMacros.GAME_WORLD_MODULE_ENTRANCE_NAME;
            if (GameMacros.GAME_IMPORT_DISPATCHING_FORWARD_ENABLED)
            {
                targetName = GameMacros.GAME_IMPORT_MODULE_ENTRANCE_NAME;
            }

            System.Type type = NovaEngine.Utility.Assembly.GetType(targetName);
            if (type == null)
            {
                Debugger.Error("Could not found '{0}' class type with current assemblies list, call that function '{1}' failed.", targetName, methodName);
                return;
            }

            NovaEngine.Utility.Reflection.CallMethod(type, methodName);
        }
    }
}
