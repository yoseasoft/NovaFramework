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
    /// 玩家对象基类
    /// </summary>
    [GameEngine.DeclareActorClass("Player")]
    [GameEngine.EntityActivationComponent(typeof(AttackComponent))]
    public class Player : Soldier
    {
    }
}
