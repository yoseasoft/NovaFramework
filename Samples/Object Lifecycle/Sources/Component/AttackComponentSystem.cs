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
    /// 攻击组件逻辑类
    /// </summary>
    [GameEngine.AspectOfTarget(typeof(AttackComponent))]
    public static class AttackComponentSystem
    {
        [GameEngine.OnAspectBeforeCall(GameEngine.AspectBehaviourType.Initialize)]
        static void BeforeInitialize(this AttackComponent self)
        {
            GameEngine.Debugger.Info("目标攻击组件实例{%t}前置初始化完成！", self);
        }

        [GameEngine.OnAspectAfterCall(GameEngine.AspectBehaviourType.Initialize)]
        static void AfterInitialize(this AttackComponent self)
        {
            GameEngine.Debugger.Info("目标攻击组件实例{%t}后置初始化完成！", self);
        }

        [GameEngine.OnAspectBeforeCall(GameEngine.AspectBehaviourType.Startup)]
        static void BeforeStartup(this AttackComponent self)
        {
            GameEngine.Debugger.Info("目标攻击组件实例{%t}前置开启完成！", self);
        }

        [GameEngine.OnAspectAfterCall(GameEngine.AspectBehaviourType.Startup)]
        static void AfterStartup(this AttackComponent self)
        {
            GameEngine.Debugger.Info("目标攻击组件实例{%t}后置开启完成！", self);
        }

        [GameEngine.OnAspectBeforeCall(GameEngine.AspectBehaviourType.Awake)]
        static void BeforeAwake(this AttackComponent self)
        {
            GameEngine.Debugger.Info("目标攻击组件实例{%t}前置唤醒完成！", self);
        }

        [GameEngine.OnAspectAfterCall(GameEngine.AspectBehaviourType.Awake)]
        static void AfterAwake(this AttackComponent self)
        {
            GameEngine.Debugger.Info("目标攻击组件实例{%t}后置唤醒完成！", self);
        }

        [GameEngine.OnAspectBeforeCall(GameEngine.AspectBehaviourType.Start)]
        static void BeforeStart(this AttackComponent self)
        {
            GameEngine.Debugger.Info("目标攻击组件实例{%t}前置启动完成！", self);
        }

        [GameEngine.OnAspectAfterCall(GameEngine.AspectBehaviourType.Start)]
        static void AfterStart(this AttackComponent self)
        {
            GameEngine.Debugger.Info("目标攻击组件实例{%t}后置启动完成！", self);
        }

        [GameEngine.OnAspectBeforeCall(GameEngine.AspectBehaviourType.Update)]
        static void BeforeUpdate(this AttackComponent self)
        {
            if (GameSampleMacros.LoopOutputEnabled) GameEngine.Debugger.Info("目标攻击组件实例{%t}前置刷新完成！", self);
        }

        [GameEngine.OnAspectAfterCall(GameEngine.AspectBehaviourType.Update)]
        static void AfterUpdate(this AttackComponent self)
        {
            if (GameSampleMacros.LoopOutputEnabled) GameEngine.Debugger.Info("目标攻击组件实例{%t}后置刷新完成！", self);
        }

        [GameEngine.OnAspectBeforeCall(GameEngine.AspectBehaviourType.LateUpdate)]
        static void BeforeLateUpdate(this AttackComponent self)
        {
            if (GameSampleMacros.LoopOutputEnabled) GameEngine.Debugger.Info("目标攻击组件实例{%t}前置延迟刷新完成！", self);
        }

        [GameEngine.OnAspectAfterCall(GameEngine.AspectBehaviourType.LateUpdate)]
        static void AfterLateUpdate(this AttackComponent self)
        {
            if (GameSampleMacros.LoopOutputEnabled) GameEngine.Debugger.Info("目标攻击组件实例{%t}后置延迟刷新完成！", self);
        }

        [GameEngine.OnAspectBeforeCall(GameEngine.AspectBehaviourType.Destroy)]
        static void BeforeDestroy(this AttackComponent self)
        {
            GameEngine.Debugger.Info("目标攻击组件实例{%t}前置销毁完成！", self);
        }

        [GameEngine.OnAspectAfterCall(GameEngine.AspectBehaviourType.Destroy)]
        static void AfterDestroy(this AttackComponent self)
        {
            GameEngine.Debugger.Info("目标攻击组件实例{%t}后置销毁完成！", self);
        }

        [GameEngine.OnAspectBeforeCall(GameEngine.AspectBehaviourType.Shutdown)]
        static void BeforeShutdown(this AttackComponent self)
        {
            GameEngine.Debugger.Info("目标攻击组件实例{%t}前置关闭完成！", self);
        }

        [GameEngine.OnAspectAfterCall(GameEngine.AspectBehaviourType.Shutdown)]
        static void AfterShutdown(this AttackComponent self)
        {
            GameEngine.Debugger.Info("目标攻击组件实例{%t}后置关闭完成！", self);
        }

        [GameEngine.OnAspectBeforeCall(GameEngine.AspectBehaviourType.Cleanup)]
        static void BeforeCleanup(this AttackComponent self)
        {
            GameEngine.Debugger.Info("目标攻击组件实例{%t}前置清理完成！", self);
        }

        [GameEngine.OnAspectAfterCall(GameEngine.AspectBehaviourType.Cleanup)]
        static void AfterCleanup(this AttackComponent self)
        {
            GameEngine.Debugger.Info("目标攻击组件实例{%t}后置清理完成！", self);
        }
    }
}
