/// <summary>
/// 基于 NovaFramework 的演示案例
/// 
/// 创建者：Hurley
/// 创建时间：2025-06-23
/// 功能描述：
/// </summary>

namespace Game.Sample.ObjectLifecycle
{
    /// <summary>
    /// 案例入口类
    /// </summary>
    public static class SampleGate
    {
        public static void Run()
        {
            GameEngine.SceneHandler.Instance.ReplaceScene<LogoScene>();
        }

        public static void Stop()
        {
        }
    }
}
