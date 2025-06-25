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
    /// 角色对象基类
    /// </summary>
    [GameEngine.DeclareActorClass("Actor")]
    public abstract class Actor : GameEngine.CActor, GameEngine.IUpdateActivation
    {
        protected override void OnInitialize()
        {
            Debugger.Info("Call Actor.OnInitialize Method.");
        }

        protected override void OnStartup()
        {
            Debugger.Info("Call Actor.OnStartup Method.");
        }

        protected override void OnAwake()
        {
            Debugger.Info("Call Actor.OnAwake Method.");
        }

        protected override void OnStart()
        {
            Debugger.Info("Call Actor.OnStart Method.");
        }

        protected override void OnDestroy()
        {
            Debugger.Info("Call Actor.OnDestroy Method.");
        }

        protected override void OnShutdown()
        {
            Debugger.Info("Call Actor.OnShutdown Method.");
        }

        protected override void OnCleanup()
        {
            Debugger.Info("Call Actor.OnCleanup Method.");
        }

        protected override void OnUpdate()
        {
            if (GameSampleMacros.LoopOutputEnabled) Debugger.Info("Call Actor.OnUpdate Method.");
        }

        protected override void OnLateUpdate()
        {
            if (GameSampleMacros.LoopOutputEnabled) Debugger.Info("Call Actor.OnLateUpdate Method.");
        }
    }
}
