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
    /// Logo场景类
    /// </summary>
    [GameEngine.DeclareSceneClass("Logo")]
    [GameEngine.EntityActivationComponent(typeof(LogoDataComponent))]
    public class LogoScene : GameEngine.CScene
    {
        protected override void OnInitialize()
        {
            Debugger.Info("Call Logo.OnInitialize Method.");
        }

        protected override void OnStartup()
        {
            Debugger.Info("Call Logo.OnStartup Method.");
        }

        protected override void OnAwake()
        {
            Debugger.Info("Call Logo.OnAwake Method.");
        }

        protected override void OnStart()
        {
            Debugger.Info("Call Logo.OnStart Method.");
        }

        protected override void OnDestroy()
        {
            Debugger.Info("Call Logo.OnDestroy Method.");
        }

        protected override void OnShutdown()
        {
            Debugger.Info("Call Logo.OnShutdown Method.");
        }

        protected override void OnCleanup()
        {
            Debugger.Info("Call Logo.OnCleanup Method.");
        }

        protected override void OnUpdate()
        {
            if (GameSampleMacros.LoopOutputEnabled) Debugger.Info("Call Logo.OnUpdate Method.");
        }

        protected override void OnLateUpdate()
        {
            if (GameSampleMacros.LoopOutputEnabled) Debugger.Info("Call Logo.OnLateUpdate Method.");
        }
    }
}
