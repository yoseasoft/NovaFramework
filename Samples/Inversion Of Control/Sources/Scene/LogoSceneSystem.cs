/// <summary>
/// 基于 NovaFramework 的测试用例
/// 
/// 创建者：Hurley
/// 创建时间：2025-06-23
/// 功能描述：
/// </summary>

namespace Game.Sample.InversionOfControl
{
    /// <summary>
    /// Logo场景类
    /// </summary>
    [GameEngine.AspectOfTarget(typeof(LogoScene))]
    static class LogoSceneSystem
    {
        [GameEngine.OnAspectBeforeCall(GameEngine.AspectBehaviourType.Awake)]
        static void Awake(this LogoScene self)
        {
            GameEngine.Debugger.Info("目标场景{%t}实例唤醒成功！", self);
        }

        [GameEngine.OnAspectBeforeCall(GameEngine.AspectBehaviourType.Start)]
        static void Start(this LogoScene self)
        {
            GameEngine.Debugger.Info("目标场景{%t}实例启动成功！", self);
        }

        [GameEngine.OnAspectBeforeCall(GameEngine.AspectBehaviourType.Update)]
        static void Update(this LogoScene self)
        {
            // GameEngine.Debugger.Info("目标场景{%t}实例刷新成功！", self);
        }

        [GameEngine.OnAspectBeforeCall(GameEngine.AspectBehaviourType.Destroy)]
        static void Destroy(this LogoScene self)
        {
            GameEngine.Debugger.Info("目标场景{%t}实例销毁成功！", self);
        }
    }
}
