/// <summary>
/// 基于 NovaFramework 的演示案例
/// 
/// 创建者：Hurley
/// 创建时间：2025-06-23
/// 功能描述：
/// </summary>

namespace Game
{
    /// <summary>
    /// 演示案例全局观察器类
    /// </summary>
    public static class GameSampleObserver
    {
        public static void Startup()
        {
            StartGame();
        }

        public static void Shutdown()
        {
            StopGame();
        }

        public static void FixedUpdate()
        { }

        public static void Update()
        { }

        public static void LateUpdate()
        { }

        public static void FixedExecute()
        { }

        public static void Execute()
        { }

        public static void LateExecute()
        { }

        private static void StartGame()
        {
        }

        private static void StopGame()
        {
        }
    }
}
