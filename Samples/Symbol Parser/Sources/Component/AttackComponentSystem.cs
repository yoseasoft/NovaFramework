/// <summary>
/// 基于 NovaFramework 的演示案例
/// 
/// 创建者：Hurley
/// 创建时间：2025-06-22
/// 功能描述：
/// </summary>

namespace Game.Sample.SymbolParser
{
    /// <summary>
    /// 攻击组件逻辑类
    /// </summary>
    [GameEngine.Aspect]
    public static class AttackComponentSystem
    {
        [GameEngine.OnAspectAfterCallOfTarget(typeof(AttackComponent), GameEngine.AspectBehaviourType.Awake)]
        static void Awake(this AttackComponent self)
        {
        }

        [GameEngine.OnAspectAfterCallOfTarget(typeof(AttackComponent), GameEngine.AspectBehaviourType.Start)]
        static void Start(this AttackComponent self)
        {
        }

        [GameEngine.OnAspectAfterCallOfTarget(typeof(AttackComponent), GameEngine.AspectBehaviourType.Destroy)]
        static void Destroy(this AttackComponent self)
        {
        }
    }
}
