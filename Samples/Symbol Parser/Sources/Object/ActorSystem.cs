/// <summary>
/// 基于 NovaFramework 的测试用例
/// 
/// 创建者：Hurley
/// 创建时间：2025-06-22
/// 功能描述：
/// </summary>

namespace Game.Sample.SymbolParser
{
    /// <summary>
    /// 角色对象逻辑类
    /// </summary>
    [GameEngine.Aspect]
    public static class ActorSystem
    {
        [GameEngine.OnAspectAfterCallOfTarget(typeof(Actor), GameEngine.AspectBehaviourType.Awake)]
        static void Awake(this Actor self)
        {
        }

        [GameEngine.OnAspectAfterCallOfTarget(typeof(Actor), GameEngine.AspectBehaviourType.Start)]
        static void Start(this Actor self)
        {
        }

        [GameEngine.OnAspectAfterCallOfTarget(typeof(Actor), GameEngine.AspectBehaviourType.Destroy)]
        static void Destroy(this Actor self)
        {
        }
    }
}
