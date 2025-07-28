/// -------------------------------------------------------------------------------
/// NovaEngine Framework Samples
///
/// Copyright (C) 2024 - 2025, Hurley, Independent Studio.
/// Copyright (C) 2025, Hainan Yuanyou Information Tecdhnology Co., Ltd. Guangzhou Branch
///
/// Permission is hereby granted, free of charge, to any person obtaining a copy
/// of this software and associated documentation files (the "Software"), to deal
/// in the Software without restriction, including without limitation the rights
/// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
/// copies of the Software, and to permit persons to whom the Software is
/// furnished to do so, subject to the following conditions:
///
/// The above copyright notice and this permission notice shall be included in
/// all copies or substantial portions of the Software.
///
/// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
/// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
/// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
/// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
/// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
/// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
/// THE SOFTWARE.
/// -------------------------------------------------------------------------------

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
            int frame = NovaEngine.Timestamp.FrameCount;

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

        /// <summary>
        /// 每一帧更新调度逻辑控制可行状态检测
        /// </summary>
        /// <param name="obj">对象实例</param>
        /// <returns>满足每一帧刷新调度条件</returns>
        internal static bool EachFrameUpdateCallPassed(object obj)
        {
            if (!GameSampleMacros.LoopOutputEnabled)
            {
                return false;
            }

            return false;
        }
    }
}
