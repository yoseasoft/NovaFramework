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
    /// Logo场景逻辑类
    /// </summary>
    [GameEngine.AspectOfTarget(typeof(LogoScene))]
    static class LogoSceneSystem
    {
        [GameEngine.OnAspectBeforeCall(GameEngine.AspectBehaviourType.Initialize)]
        static void BeforeInitialize(this LogoScene self)
        {
            GameEngine.Debugger.Info("目标场景实例{%t}前置初始化完成！", self);
        }

        [GameEngine.OnAspectAfterCall(GameEngine.AspectBehaviourType.Initialize)]
        static void AfterInitialize(this LogoScene self)
        {
            GameEngine.Debugger.Info("目标场景实例{%t}后置初始化完成！", self);
        }

        [GameEngine.OnAspectBeforeCall(GameEngine.AspectBehaviourType.Startup)]
        static void BeforeStartup(this LogoScene self)
        {
            GameEngine.Debugger.Info("目标场景实例{%t}前置开启完成！", self);
        }

        [GameEngine.OnAspectAfterCall(GameEngine.AspectBehaviourType.Startup)]
        static void AfterStartup(this LogoScene self)
        {
            GameEngine.Debugger.Info("目标场景实例{%t}后置开启完成！", self);
        }

        [GameEngine.OnAspectBeforeCall(GameEngine.AspectBehaviourType.Awake)]
        static void BeforeAwake(this LogoScene self)
        {
            GameEngine.Debugger.Info("目标场景实例{%t}前置唤醒完成！", self);
        }

        [GameEngine.OnAspectAfterCall(GameEngine.AspectBehaviourType.Awake)]
        static void AfterAwake(this LogoScene self)
        {
            GameEngine.Debugger.Info("目标场景实例{%t}后置唤醒完成！", self);
        }

        [GameEngine.OnAspectBeforeCall(GameEngine.AspectBehaviourType.Start)]
        static void BeforeStart(this LogoScene self)
        {
            GameEngine.Debugger.Info("目标场景实例{%t}前置启动完成！", self);
        }

        [GameEngine.OnAspectAfterCall(GameEngine.AspectBehaviourType.Start)]
        static void AfterStart(this LogoScene self)
        {
            GameEngine.Debugger.Info("目标场景实例{%t}后置启动完成！", self);
        }

        [GameEngine.OnAspectBeforeCall(GameEngine.AspectBehaviourType.Update)]
        static void BeforeUpdate(this LogoScene self)
        {
            if (GameSampleMacros.LoopOutputEnabled) GameEngine.Debugger.Info("目标场景实例{%t}前置刷新完成！", self);
        }

        [GameEngine.OnAspectAfterCall(GameEngine.AspectBehaviourType.Update)]
        static void AfterUpdate(this LogoScene self)
        {
            if (GameSampleMacros.LoopOutputEnabled) GameEngine.Debugger.Info("目标场景实例{%t}后置刷新完成！", self);
        }

        [GameEngine.OnAspectBeforeCall(GameEngine.AspectBehaviourType.LateUpdate)]
        static void BeforeLateUpdate(this LogoScene self)
        {
            if (GameSampleMacros.LoopOutputEnabled) GameEngine.Debugger.Info("目标场景实例{%t}前置延迟刷新完成！", self);
        }

        [GameEngine.OnAspectAfterCall(GameEngine.AspectBehaviourType.LateUpdate)]
        static void AfterLateUpdate(this LogoScene self)
        {
            if (GameSampleMacros.LoopOutputEnabled) GameEngine.Debugger.Info("目标场景实例{%t}后置延迟刷新完成！", self);
        }

        [GameEngine.OnAspectBeforeCall(GameEngine.AspectBehaviourType.Destroy)]
        static void BeforeDestroy(this LogoScene self)
        {
            GameEngine.Debugger.Info("目标场景实例{%t}前置销毁完成！", self);
        }

        [GameEngine.OnAspectAfterCall(GameEngine.AspectBehaviourType.Destroy)]
        static void AfterDestroy(this LogoScene self)
        {
            GameEngine.Debugger.Info("目标场景实例{%t}后置销毁完成！", self);
        }

        [GameEngine.OnAspectBeforeCall(GameEngine.AspectBehaviourType.Shutdown)]
        static void BeforeShutdown(this LogoScene self)
        {
            GameEngine.Debugger.Info("目标场景实例{%t}前置关闭完成！", self);
        }

        [GameEngine.OnAspectAfterCall(GameEngine.AspectBehaviourType.Shutdown)]
        static void AfterShutdown(this LogoScene self)
        {
            GameEngine.Debugger.Info("目标场景实例{%t}后置关闭完成！", self);
        }

        [GameEngine.OnAspectBeforeCall(GameEngine.AspectBehaviourType.Cleanup)]
        static void BeforeCleanup(this LogoScene self)
        {
            GameEngine.Debugger.Info("目标场景实例{%t}前置清理完成！", self);
        }

        [GameEngine.OnAspectAfterCall(GameEngine.AspectBehaviourType.Cleanup)]
        static void AfterCleanup(this LogoScene self)
        {
            GameEngine.Debugger.Info("目标场景实例{%t}后置清理完成！", self);
        }
    }
}
