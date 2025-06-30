/// <summary>
/// 基于 NovaFramework 的演示案例
/// 
/// 创建者：Hurley
/// 创建时间：2025-06-29
/// 功能描述：
/// </summary>

namespace Game.Sample.TextFormat
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
            GameEngine.Debugger.Info("目标场景实例{%t}后置唤醒完成！", self);
        }

        [GameEngine.OnAspectAfterCall(GameEngine.AspectBehaviourType.Start)]
        static void AfterStart(this LogoScene self)
        {
            GameEngine.Debugger.Info("目标场景实例{%t}后置启动完成！", self);
        }

        [GameEngine.OnAspectAfterCall(GameEngine.AspectBehaviourType.Update)]
        static void AfterUpdate(this LogoScene self)
        {
            if (GameSample.OnceTimeUpdateCallPassed(self)) GameEngine.Debugger.Info("目标场景实例{%t}后置刷新完成！", self);
        }

        [GameEngine.OnAspectAfterCall(GameEngine.AspectBehaviourType.LateUpdate)]
        static void AfterLateUpdate(this LogoScene self)
        {
            if (GameSample.OnceTimeUpdateCallPassed(self)) GameEngine.Debugger.Info("目标场景实例{%t}后置延迟刷新完成！", self);
        }

        [GameEngine.OnAspectBeforeCall(GameEngine.AspectBehaviourType.Destroy)]
        static void BeforeDestroy(this LogoScene self)
        {
            GameEngine.Debugger.Info("目标场景实例{%t}前置销毁完成！", self);
        }
    }
}
