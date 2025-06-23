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
    /// 玩家对象逻辑类
    /// </summary>
    [GameEngine.Aspect]
    public static class PlayerSystem
    {
        [GameEngine.OnAspectAfterCallOfTarget(typeof(Player), GameEngine.AspectBehaviourType.Awake)]
        static void Awake(this Player self)
        {
        }

        [GameEngine.OnAspectAfterCallOfTarget(typeof(Player), GameEngine.AspectBehaviourType.Start)]
        static void Start(this Player self)
        {
        }

        [GameEngine.OnAspectAfterCallOfTarget(typeof(Player), GameEngine.AspectBehaviourType.Destroy)]
        static void Destroy(this Player self)
        {
        }
    }
}
