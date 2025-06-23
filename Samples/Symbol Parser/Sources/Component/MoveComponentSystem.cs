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
    /// 移动组件逻辑类
    /// </summary>
    [GameEngine.Aspect]
    public static class MoveComponentSystem
    {
        [GameEngine.OnAspectAfterCallOfTarget(typeof(MoveComponent), GameEngine.AspectBehaviourType.Awake)]
        static void Awake(this MoveComponent self)
        {
        }

        [GameEngine.OnAspectAfterCallOfTarget(typeof(MoveComponent), GameEngine.AspectBehaviourType.Start)]
        static void Start(this MoveComponent self)
        {
        }

        [GameEngine.OnAspectAfterCallOfTarget(typeof(MoveComponent), GameEngine.AspectBehaviourType.Destroy)]
        static void Destroy(this MoveComponent self)
        {
        }
    }
}
