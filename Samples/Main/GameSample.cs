/// <summary>
/// 基于 NovaFramework 的测试用例
/// 
/// 创建者：Hurley
/// 创建时间：2025-06-22
/// 功能描述：
/// </summary>

namespace Game.Sample
{
    /// <summary>
    /// 测试案例总控
    /// </summary>
    public static partial class GameSample
    {
        /// <summary>
        /// 当前游戏案例运行的具体案例类型<br/>
        /// 用户通过修改该类型，来测试不同的案例
        /// </summary>
        public readonly static GameSampleType GameSampleRunningType = GameSampleType.SymbolParser;
    }
}
