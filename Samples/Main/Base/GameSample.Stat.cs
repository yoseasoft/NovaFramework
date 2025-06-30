/// <summary>
/// 基于 NovaFramework 的演示案例
/// 
/// 创建者：Hurley
/// 创建时间：2025-06-29
/// 功能描述：
/// </summary>

using System.Collections.Generic;

namespace Game.Sample
{
    /// <summary>
    /// 演示案例总控
    /// </summary>
    public static partial class GameSample
    {
        private static IDictionary<int, int> GameEntityUpdateCallStat = null;

        /// <summary>
        /// 一次性更新调度逻辑控制可行状态检测
        /// </summary>
        /// <param name="obj">对象实例</param>
        /// <returns>满足一次性刷新调度条件</returns>
        internal static bool OnceTimeUpdateCallPassed(object obj)
        {
            if (!GameSampleMacros.LoopOutputEnabled)
            {
                return false;
            }

            if (null == GameEntityUpdateCallStat)
            {
                GameEntityUpdateCallStat = new Dictionary<int, int>();
            }

            int hash = obj.GetHashCode();
            int frame = NovaEngine.Facade.Timestamp.FrameCount;

            if (false == GameEntityUpdateCallStat.TryGetValue(hash, out int v))
            {
                GameEntityUpdateCallStat.Add(hash, frame);
                return true;
            }

            if (v == frame)
            {
                return true;
            }

            return false;
        }
    }
}
