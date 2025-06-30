/// <summary>
/// 基于 NovaFramework 的演示案例
/// 
/// 创建者：Hurley
/// 创建时间：2025-06-25
/// 功能描述：
/// </summary>

namespace Game.Sample.DispatchCall
{
    /// <summary>
    /// Logo场景逻辑类
    /// </summary>
    [GameEngine.AspectOfTarget(typeof(LogoScene))]
    static class LogoSceneSystem
    {
        [GameEngine.OnAspectAfterCall(GameEngine.AspectBehaviourType.Awake)]
        static void AfterAwake(this LogoScene self)
        {
        }

        [GameEngine.OnAspectAfterCall(GameEngine.AspectBehaviourType.Start)]
        static void AfterStart(this LogoScene self)
        {
        }

        [GameEngine.OnAspectAfterCall(GameEngine.AspectBehaviourType.Update)]
        static void AfterUpdate(this LogoScene self)
        {
        }

        [GameEngine.OnAspectAfterCall(GameEngine.AspectBehaviourType.LateUpdate)]
        static void AfterLateUpdate(this LogoScene self)
        {
        }

        [GameEngine.OnAspectBeforeCall(GameEngine.AspectBehaviourType.Destroy)]
        static void BeforeDestroy(this LogoScene self)
        {
        }
    }
}
