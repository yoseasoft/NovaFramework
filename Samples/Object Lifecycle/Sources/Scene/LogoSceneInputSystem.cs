/// <summary>
/// 基于 NovaFramework 的演示案例
/// 
/// 创建者：Hurley
/// 创建时间：2025-06-25
/// 功能描述：
/// </summary>

namespace Game.Sample.ObjectLifecycle
{
    /// <summary>
    /// Logo场景输入逻辑类
    /// </summary>
    [GameEngine.KeycodeSystem]
    static class LogoSceneInputSystem
    {
        [GameEngine.OnKeycodeDispatchResponse((int) UnityEngine.KeyCode.A, GameEngine.InputOperationType.Released)]
        static void OnSceneInputed(int keycode, int operationType)
        {
            LogoScene logo = GameEngine.SceneHandler.Instance.GetCurrentScene() as LogoScene;
            Debugger.Assert(null != logo, "Invalid activated scene.");

            LogoDataComponent logoDataComponent = logo.GetComponent<LogoDataComponent>();
            if (null == logoDataComponent.player)
            {
                logoDataComponent.player = GameEngine.ActorHandler.Instance.CreateActor<Player>();
            }
            else
            {
                GameEngine.ActorHandler.Instance.DestroyActor(logoDataComponent.player);
                logoDataComponent.player = null;
            }
        }
    }
}
