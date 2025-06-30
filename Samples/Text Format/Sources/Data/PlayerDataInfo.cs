/// <summary>
/// 基于 NovaFramework 的演示案例
/// 
/// 创建者：Hurley
/// 创建时间：2025-06-29
/// 功能描述：
/// </summary>

using System.Collections.Generic;

namespace Game.Sample.TextFormat
{
    public struct PlayerCardInfo
    {
        public int card_id;
        public int card_type;
        public string card_name;
    }

    public struct PlayerGemInfo
    {
        public int gem_count;
        public IList<GemInfo> gem_list;
    }

    public struct GemInfo
    {
        public int gem_id;
        public int gem_type;
        public string gem_name;

        public int gem_value;
    }
}
