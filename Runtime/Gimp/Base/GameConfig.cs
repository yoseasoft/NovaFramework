/// <summary>
/// 2024-05-05 GameEngine Framework Code By Hurley
/// </summary>

namespace GameEngine
{
    /// <summary>
    /// 程序的配置管理器封装对象类，提供业务层对配置数据的读写访问接口
    /// </summary>
    public static class GameConfig
    {
        /// <summary>
        /// 动态生成库名称
        /// </summary>
        public const string AGEN_LIBRARY_NAME = "Agen";
        /// <summary>
        /// 游戏对象库名称
        /// </summary>
        public const string GAME_LIBRARY_NAME = "Game";
        /// <summary>
        /// 游戏逻辑库名称
        /// </summary>
        public const string GAME_HOTFIX_LIBRARY_NAME = "GameHotfix";

        /// <summary>
        /// 库资源路径信息
        /// </summary>
        public const string LIBRARY_ASSET_PATH = "Assets/_Resources/Code";
    }
}
