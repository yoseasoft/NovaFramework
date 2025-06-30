/// <summary>
/// 基于 NovaFramework 的演示案例
/// 
/// 创建者：Hurley
/// 创建时间：2025-06-24
/// 功能描述：
/// </summary>

namespace Game.Sample.ObjectLifecycle
{
    /// <summary>
    /// 玩家对象逻辑类
    /// </summary>
    [GameEngine.Aspect]
    public static class PlayerSystem
    {
        [GameEngine.OnAspectBeforeCallOfTarget(typeof(Player), GameEngine.AspectBehaviourType.Initialize)]
        static void BeforeInitialize(this Player self)
        {
            GameEngine.Debugger.Info("目标玩家实例{%t}前置初始化完成！", self);
        }

        [GameEngine.OnAspectAfterCallOfTarget(typeof(Player), GameEngine.AspectBehaviourType.Initialize)]
        static void AfterInitialize(this Player self)
        {
            GameEngine.Debugger.Info("目标玩家实例{%t}后置初始化完成！", self);
        }

        [GameEngine.OnAspectBeforeCallOfTarget(typeof(Player), GameEngine.AspectBehaviourType.Startup)]
        static void BeforeStartup(this Player self)
        {
            GameEngine.Debugger.Info("目标玩家实例{%t}前置开启完成！", self);
        }

        [GameEngine.OnAspectAfterCallOfTarget(typeof(Player), GameEngine.AspectBehaviourType.Startup)]
        static void AfterStartup(this Player self)
        {
            GameEngine.Debugger.Info("目标玩家实例{%t}后置开启完成！", self);
        }

        [GameEngine.OnAspectBeforeCallOfTarget(typeof(Player), GameEngine.AspectBehaviourType.Awake)]
        static void BeforeAwake(this Player self)
        {
            GameEngine.Debugger.Info("目标玩家实例{%t}前置唤醒完成！", self);
        }

        [GameEngine.OnAspectAfterCallOfTarget(typeof(Player), GameEngine.AspectBehaviourType.Awake)]
        static void AfterAwake(this Player self)
        {
            GameEngine.Debugger.Info("目标玩家实例{%t}后置唤醒完成！", self);

            self.AddComponent<LeapAttackComponent>();
        }

        [GameEngine.OnAspectBeforeCallOfTarget(typeof(Player), GameEngine.AspectBehaviourType.Start)]
        static void BeforeStart(this Player self)
        {
            GameEngine.Debugger.Info("目标玩家实例{%t}前置启动完成！", self);
        }

        [GameEngine.OnAspectAfterCallOfTarget(typeof(Player), GameEngine.AspectBehaviourType.Start)]
        static void AfterStart(this Player self)
        {
            GameEngine.Debugger.Info("目标玩家实例{%t}后置启动完成！", self);
        }

        [GameEngine.OnAspectBeforeCallOfTarget(typeof(Player), GameEngine.AspectBehaviourType.Update)]
        static void BeforeUpdate(this Player self)
        {
            if (GameSample.OnceTimeUpdateCallPassed(self)) GameEngine.Debugger.Info("目标玩家实例{%t}前置刷新完成！", self);
        }

        [GameEngine.OnAspectAfterCallOfTarget(typeof(Player), GameEngine.AspectBehaviourType.Update)]
        static void AfterUpdate(this Player self)
        {
            if (GameSample.OnceTimeUpdateCallPassed(self)) GameEngine.Debugger.Info("目标玩家实例{%t}后置刷新完成！", self);
        }

        [GameEngine.OnAspectBeforeCallOfTarget(typeof(Player), GameEngine.AspectBehaviourType.LateUpdate)]
        static void BeforeLateUpdate(this Player self)
        {
            if (GameSample.OnceTimeUpdateCallPassed(self)) GameEngine.Debugger.Info("目标玩家实例{%t}前置延迟刷新完成！", self);
        }

        [GameEngine.OnAspectAfterCallOfTarget(typeof(Player), GameEngine.AspectBehaviourType.LateUpdate)]
        static void AfterLateUpdate(this Player self)
        {
            if (GameSample.OnceTimeUpdateCallPassed(self)) GameEngine.Debugger.Info("目标玩家实例{%t}后置延迟刷新完成！", self);
        }

        [GameEngine.OnAspectBeforeCallOfTarget(typeof(Player), GameEngine.AspectBehaviourType.Destroy)]
        static void BeforeDestroy(this Player self)
        {
            GameEngine.Debugger.Info("目标玩家实例{%t}前置销毁完成！", self);
        }

        [GameEngine.OnAspectAfterCallOfTarget(typeof(Player), GameEngine.AspectBehaviourType.Destroy)]
        static void AfterDestroy(this Player self)
        {
            GameEngine.Debugger.Info("目标玩家实例{%t}后置销毁完成！", self);
        }

        [GameEngine.OnAspectBeforeCallOfTarget(typeof(Player), GameEngine.AspectBehaviourType.Shutdown)]
        static void BeforeShutdown(this Player self)
        {
            GameEngine.Debugger.Info("目标玩家实例{%t}前置关闭完成！", self);
        }

        [GameEngine.OnAspectAfterCallOfTarget(typeof(Player), GameEngine.AspectBehaviourType.Shutdown)]
        static void AfterShutdown(this Player self)
        {
            GameEngine.Debugger.Info("目标玩家实例{%t}后置关闭完成！", self);
        }

        [GameEngine.OnAspectBeforeCallOfTarget(typeof(Player), GameEngine.AspectBehaviourType.Cleanup)]
        static void BeforeCleanup(this Player self)
        {
            GameEngine.Debugger.Info("目标玩家实例{%t}前置清理完成！", self);
        }

        [GameEngine.OnAspectAfterCallOfTarget(typeof(Player), GameEngine.AspectBehaviourType.Cleanup)]
        static void AfterCleanup(this Player self)
        {
            GameEngine.Debugger.Info("目标玩家实例{%t}后置清理完成！", self);
        }
    }
}
