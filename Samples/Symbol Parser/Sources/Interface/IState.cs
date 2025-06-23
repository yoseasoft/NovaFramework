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
    /// 状态接口类
    /// </summary>
    public interface IState
    {
        void OnState(float dt);
    }
}
