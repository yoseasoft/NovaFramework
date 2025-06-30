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
    /// 玩家对象基类
    /// </summary>
    [GameEngine.DeclareActorClass("Player")]
    public class Player : Actor
    {
        protected override void OnInitialize()
        {
            Debugger.Info("Call Player.OnInitialize Method.");
        }

        protected override void OnStartup()
        {
            Debugger.Info("Call Player.OnStartup Method.");
        }

        protected override void OnAwake()
        {
            Debugger.Info("Call Player.OnAwake Method.");
        }

        protected override void OnStart()
        {
            Debugger.Info("Call Player.OnStart Method.");
        }

        protected override void OnDestroy()
        {
            Debugger.Info("Call Player.OnDestroy Method.");
        }

        protected override void OnShutdown()
        {
            Debugger.Info("Call Player.OnShutdown Method.");
        }

        protected override void OnCleanup()
        {
            Debugger.Info("Call Player.OnCleanup Method.");
        }

        protected override void OnUpdate()
        {
            if (GameSample.OnceTimeUpdateCallPassed(this)) Debugger.Info("Call Player.OnUpdate Method.");
        }

        protected override void OnLateUpdate()
        {
            if (GameSample.OnceTimeUpdateCallPassed(this)) Debugger.Info("Call Player.OnLateUpdate Method.");
        }
    }
}
