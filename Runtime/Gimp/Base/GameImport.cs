/// <summary>
/// 2023-11-20 GameEngine Framework Code By Hurley
/// </summary>

namespace GameEngine
{
    /// <summary>
    /// 程序的世界加载器入口封装对象类，提供业务层相关模块数据的动态加载
    /// </summary>
    public static partial class GameImport
    {
        /// <summary>
        /// 世界加载器的开始函数
        /// </summary>
        public static void Startup()
        {
            CheckVersion();
        }

        /// <summary>
        /// 世界加载器的关闭函数
        /// </summary>
        public static void Shutdown()
        {
            if (NovaEngine.AppEntry.HasManager<Updation>())
            {
                NovaEngine.AppEntry.RemoveManager<Updation>();
            }

            CallGameFunc(GameMacros.GAME_REMOTE_PROCESS_CALL_STOP_SERVICE_NAME);
        }

        /// <summary>
        /// 世界加载器的重启函数
        /// </summary>
        public static void Restart()
        {
            CallGameFunc(GameMacros.GAME_REMOTE_PROCESS_CALL_RELOAD_SERVICE_NAME);
        }

        internal static void CheckVersion()
        {
            NovaEngine.AppEntry.CreateManager<Updation>();
        }

        internal static void OnVersionUpdateCompleted()
        {
            Debugger.Log("版本更新结束，进入业务层服务逻辑！");

            NovaEngine.AppEntry.RemoveManager<Updation>();

            CallGameFunc(GameMacros.GAME_REMOTE_PROCESS_CALL_RUN_SERVICE_NAME);
        }
    }
}
