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
        /// 世界加载器的启动运行函数
        /// </summary>
        public static void Run()
        {
            Startup();
        }

        /// <summary>
        /// 世界加载器的停止运行函数
        /// </summary>
        public static void Stop()
        {
            Shutdown();
        }

        /// <summary>
        /// 世界加载器的重载函数
        /// </summary>
        public static void Reload()
        {
            Restart();
        }

        /// <summary>
        /// 调用游戏业务层的指定函数
        /// </summary>
        /// <param name="methodName">函数名称</param>
        private static void CallGameFunc(string methodName)
        {
            System.Type type = NovaEngine.Utility.Assembly.GetType(GameMacros.GAME_WORLD_MODULE_ENTRANCE_NAME);
            if (type == null)
            {
                Debugger.Error("Could not found '{0}' class type with current assemblies list, call that function '{1}' failed.",
                        GameMacros.GAME_WORLD_MODULE_ENTRANCE_NAME, methodName);
                return;
            }

            NovaEngine.Utility.Reflection.CallMethod(type, methodName);
        }
    }
}
