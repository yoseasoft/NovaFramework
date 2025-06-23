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
    /// 战斗对象逻辑类
    /// </summary>
    [GameEngine.Aspect]
    public static class SoldierSystem
    {
        [GameEngine.OnAspectAfterCallOfTarget(typeof(Soldier), GameEngine.AspectBehaviourType.Awake)]
        static void Awake(this Soldier self)
        {
        }

        [GameEngine.OnAspectAfterCallOfTarget(typeof(Soldier), GameEngine.AspectBehaviourType.Start)]
        static void Start(this Soldier self)
        {
        }

        [GameEngine.OnAspectAfterCallOfTarget(typeof(Soldier), GameEngine.AspectBehaviourType.Destroy)]
        static void Destroy(this Soldier self)
        {
        }
    }
}
